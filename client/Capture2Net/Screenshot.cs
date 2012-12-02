using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Capture2Net
{
	public class Screenshot :IDisposable
	{
		ScreenCapture screenCaptureInstance;
		CloudConfig cloudConfigInstance;
		Image image;
		IntPtr activeWindow;
		NotifyIcon notifyIcon;
		string screenshotUrl;

		enum ScreenshotType
		{
			Screen,
			Selection,
			Window
		}

		internal class NativeMethods
		{
			[DllImport("user32.dll")]
			public static extern IntPtr GetForegroundWindow();
			[DllImport("user32.dll", CharSet = CharSet.Unicode)]
			public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
			[DllImport("user32.dll")]
			public static extern int GetWindowTextLength(IntPtr hWnd);
		}

		public Screenshot(CloudConfig cloudConfigInstance)
		{
			this.screenCaptureInstance = new ScreenCapture();
			this.cloudConfigInstance = cloudConfigInstance;
			this.activeWindow = NativeMethods.GetForegroundWindow();
		}

		~Screenshot()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.notifyIcon != null)
			{
				this.notifyIcon.Dispose();
				this.notifyIcon = null;
			}
		}

		// Screenshot of whole screen
		public void Screen()
		{
			this.image = this.screenCaptureInstance.Capture();
			this.PostProcess(ScreenshotType.Screen);
		}

		// Allow to select the area
		public void Selection()
		{
			var image = this.screenCaptureInstance.Capture();
			var selectionForm = new SelectionForm(image);
			Application.Run(selectionForm);
			if (selectionForm.accepted)
			{
				var bitmap = new Bitmap(image);
				this.image = bitmap.Clone(selectionForm.cropArea, bitmap.PixelFormat);
				this.PostProcess(ScreenshotType.Selection);
			}
		}

		// Screenshot of current active window
		public void Window()
		{
			this.image = this.screenCaptureInstance.Capture(this.activeWindow);
			this.PostProcess(ScreenshotType.Window);
		}

		private void PostProcess(ScreenshotType type)
		{
			new SoundPlayer(Properties.Resources.CameraSound).PlaySync();
			this.cloudConfigInstance.Load();
			var tempFile = System.IO.Path.GetTempFileName();
			var jsonData = this.cloudConfigInstance.jsonData["screenshots"][type.ToString().ToLower()];
			var fileExtension = jsonData["imageFormat"].ToString().ToLower();
			ImageFormat imageFormat;
			switch (fileExtension)
			{
				case "bmp":
					imageFormat = ImageFormat.Bmp;
					break;
				case "gif":
					imageFormat = ImageFormat.Gif;
					break;
				case "png":
					imageFormat = ImageFormat.Png;
					break;
				case "jpg":
					imageFormat = ImageFormat.Jpeg;
					break;
				case "tif":
					imageFormat = ImageFormat.Tiff;
					break;
				default:
					fileExtension = "jpg";
					imageFormat = ImageFormat.Jpeg;
					break;
			}
			this.image.Save(tempFile, imageFormat);

			this.notifyIcon = new NotifyIcon();
			this.notifyIcon.Icon = Properties.Resources.UploadIcon;
			this.notifyIcon.Text = "Capture2Net is uploading a screenshot (" + Utils.GetHumanReadableFileSize(new FileInfo(tempFile).Length) + ")...";
			this.notifyIcon.Visible = true;
			if (Properties.Settings.Default.acceptAllCertificates)
			{
				ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(this.AcceptAllCertifications);
			}
			try
			{
				// Initialization
				var fileStream = new FileStream(tempFile, FileMode.Open);
				var uri = new Uri(Properties.Settings.Default.protocol.ToLower() + "://" + Properties.Settings.Default.hostname + ":" + Properties.Settings.Default.port + Utils.GetValidPath(Properties.Settings.Default.path));
				var webRequest = (HttpWebRequest)WebRequest.Create(uri);

				webRequest.Method = "PUT";
				webRequest.UserAgent = "Capture2Net";
				webRequest.AllowWriteStreamBuffering = true;

				// Set basic authentication credentials
				var password = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(Properties.Settings.Default.password));
				var userPassword = Properties.Settings.Default.username + ":" + password;
				webRequest.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(userPassword)));

				// Write data from file stream
				var dataStream = webRequest.GetRequestStream();
				var buffer = new byte[10240];// 10 KB buffer
				var bytesRead = fileStream.Read(buffer, 0, buffer.Length);
				while (bytesRead > 0)
				{
					dataStream.Write(buffer, 0, buffer.Length);
					bytesRead = fileStream.Read(buffer, 0, buffer.Length);
				}

				// Write info data block
				List<string> infoDataList = new List<string>();

				var activeWindowTitle = new StringBuilder(NativeMethods.GetWindowTextLength(this.activeWindow) + 1);// Length of window title + 1 for the null character
				NativeMethods.GetWindowText(this.activeWindow, activeWindowTitle, activeWindowTitle.Capacity);

				infoDataList.Add("\tINFODATA\t");// Code to identify the start of the info data block (Must contain characters which are never used in the info data)
				infoDataList.Add("screenshotType=" + type.ToString().ToLower());// Screenshot type
				infoDataList.Add("fileExtension=" + fileExtension);// Image type
				infoDataList.Add("activeWindow=" + activeWindowTitle);// Title of the current active window
				infoDataList.Add("userName=" + System.Security.Principal.WindowsIdentity.GetCurrent().Name);// Username of current logged in user (Windows user)
				infoDataList.Add("hostName=" + Dns.GetHostName());// Hostname of this computer

				var infoData = ASCIIEncoding.ASCII.GetBytes(string.Join("\n", infoDataList.ToArray()));
				dataStream.Write(infoData, 0, infoData.Length);

				// Close streams
				dataStream.Close();
				fileStream.Close();

				var response = (HttpWebResponse)webRequest.GetResponse();
				if (response.StatusCode == HttpStatusCode.OK)
				{
					var responseStream = response.GetResponseStream();
					var readStream = new StreamReader(responseStream, Encoding.Default);

					var responseText = readStream.ReadToEnd();

					responseStream.Close();

					if (responseText.Length > 8 && (responseText.Substring(0, 7) == "http://" || responseText.Substring(0, 8) == "https://"))
					{
						this.screenshotUrl = responseText;
						Clipboard.SetText(this.screenshotUrl);
						this.notifyIcon.Text = "Capture2Net - Upload complete";
						this.notifyIcon.BalloonTipClicked += new EventHandler(this.OpenUrlInBrowser);
						this.notifyIcon.ShowBalloonTip(5000, "Capture2Net - Upload complete", "The screenshot URL has been copied to the clipboard.\n\nClick here to open the URL in your browser.", ToolTipIcon.Info);
						this.notifyIcon.BalloonTipClosed += new EventHandler(this.BalloonTipClosed);
						Application.Run();
					}
					else
					{
						MessageBox.Show("Unexpected response from server!\n\n" + responseText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				else
				{
					MessageBox.Show("Server returned status " + response.StatusCode.ToString() + " (" + response.StatusDescription + ")", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			catch (WebException exception)
			{
				MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			
			// Try to remove temporary file
			File.Delete(tempFile);
			
			// Remove notification icon
			notifyIcon.Dispose();
		}

		private bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		private void OpenUrlInBrowser(object sender, EventArgs e)
		{
			Process.Start(this.screenshotUrl);
			this.notifyIcon.Dispose();
			Application.Exit();
		}

		private void BalloonTipClosed(object sender, EventArgs e)
		{
			this.notifyIcon.Dispose();
			Application.Exit();
		}
	}
}