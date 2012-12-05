using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace Capture2Net
{
	class Uploader : IDisposable
	{
		NotifyIcon notifyIcon;
		string fileName;
		string screenshotUrl;
		
		public Uploader(string fileName)
		{
			this.fileName = fileName;
			this.CreateTrayIcon();
			this.SendFile();
		}

		~Uploader()
		{
			this.Dispose(false);
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
			}
		}

		private void CreateTrayIcon()
		{
			this.notifyIcon = new NotifyIcon();
			this.notifyIcon.Icon = Properties.Resources.UploadIcon;
			this.notifyIcon.Text = "Capture2Net is uploading a screenshot (" + Utils.GetHumanReadableFileSize(new FileInfo(this.fileName).Length) + ")...";
			this.notifyIcon.Visible = true;
		}

		private void SendFile()
		{
			ServicePointManager.ServerCertificateValidationCallback = this.CheckSSLCertificate;
			try
			{
				// Initialization
				var fileStream = new FileStream(this.fileName, FileMode.Open);
				var webRequest = (HttpWebRequest)WebRequest.Create(Program.settingsInstance.Protocol + "://" + Program.settingsInstance.Hostname + ":" + Program.settingsInstance.Port + Program.settingsInstance.Path);

				webRequest.Method = "PUT";
				webRequest.UserAgent = "Capture2Net";
				webRequest.AllowWriteStreamBuffering = true;

				// Set basic authentication credentials
				var userPassword = Program.settingsInstance.Username + ":" + Program.settingsInstance.Password;
				webRequest.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(userPassword)));

				// Copy file stream to web request stream
				var dataStream = webRequest.GetRequestStream();
				fileStream.CopyTo(dataStream);

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
			
			// Remove notification icon
			notifyIcon.Dispose();
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

		private bool CheckSSLCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
		{
			var sslChain = new SSLChain();
			return sslChain.CheckCertificate(sender, certification, chain, sslPolicyErrors);
		}
	}
}