using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace Capture2Net
{
	public class Screenshot
	{
		private ScreenCapture screenCaptureInstance;
		private CloudConfig cloudConfigInstance;
		private string fileExtension;
		private string tempFile;
		private ImageFormat imageFormat;

		public Screenshot(string fileExtension, CloudConfig cloudConfigInstance)
		{
			this.screenCaptureInstance = new ScreenCapture();
			this.cloudConfigInstance = cloudConfigInstance;
			this.fileExtension = fileExtension;

			switch (this.fileExtension)
			{
				case "bmp":
					this.imageFormat = ImageFormat.Bmp;
					break;
				case "gif":
					this.imageFormat = ImageFormat.Gif;
					break;
				case "png":
					this.imageFormat = ImageFormat.Png;
					break;
				case "jpg":
					this.imageFormat = ImageFormat.Jpeg;
					break;
				case "tif":
					this.imageFormat = ImageFormat.Tiff;
					break;
				default:
					this.fileExtension = "jpg";
					this.imageFormat = ImageFormat.Jpeg;
					break;
			}

			this.tempFile = System.IO.Path.GetTempFileName();
		}

		// Screenshot of whole screen
		public void Screen()
		{
			var image = this.screenCaptureInstance.Capture();
			image.Save(this.tempFile, this.imageFormat);

			this.PostProcess();
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
				var bitmapCrop = bitmap.Clone(selectionForm.cropArea, bitmap.PixelFormat);
				bitmapCrop.Save(this.tempFile, this.imageFormat);
				this.PostProcess();
			}
		}

		// Screenshot of current active window
		public void Window()
		{
			var image = this.screenCaptureInstance.Capture(this.screenCaptureInstance.GetForegroundWindow());
			image.Save(this.tempFile, this.imageFormat);

			this.PostProcess();
		}

		private void PostProcess()
		{
			if (Properties.Settings.Default.playSound)
			{
				new SoundPlayer(Properties.Resources.Camera).Play();
			}
			if (Properties.Settings.Default.acceptAllCertificates)
			{
				ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(this.AcceptAllCertifications);
			}
			try
			{
				// Initialization
				var fileStream = new FileStream(this.tempFile, FileMode.Open);
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
			
				// Close streams
				dataStream.Close();
				fileStream.Close();

				// Remove temporary file
				File.Delete(this.tempFile);

				var response = (HttpWebResponse)webRequest.GetResponse();
				if (response.StatusCode == HttpStatusCode.OK)
				{
					var responseStream = response.GetResponseStream();
					var readStream = new StreamReader(responseStream, Encoding.Default);

					var responseText = readStream.ReadToEnd();

					readStream.Close();
					responseStream.Close();

					if (responseText.Substring(0, 7) == "http://" || responseText.Substring(0, 8) == "https://")
					{
						Clipboard.SetText(responseText);
						// TODO: Notify user about complete upload
					}
					else
					{
						MessageBox.Show("Unexpected response from server!\n\n" + responseText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				else
				{
					MessageBox.Show("Server returned status " + response.StatusCode.ToString() + " (" + response.StatusDescription + ")");
				}
			}
			catch (WebException exception)
			{
				MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}
	}
}