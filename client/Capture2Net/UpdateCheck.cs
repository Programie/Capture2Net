using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Capture2Net
{
	class UpdateCheck
	{
		string downloadUrl;
		bool doExit = false;

		public bool DoExit
		{
			get
			{
				return this.doExit;
			}
		}
		
		public UpdateCheck()
		{
			try
			{
				var webRequest = (HttpWebRequest)WebRequest.Create("http://updates.selfcoders.com/getupdate.php?project=capture2net");

				webRequest.Method = "GET";
				webRequest.UserAgent = "Capture2Net";

				var response = (HttpWebResponse)webRequest.GetResponse();
				if (response.StatusCode == HttpStatusCode.OK)
				{
					var responseStream = response.GetResponseStream();
					var readStream = new StreamReader(responseStream);
					var responseText = readStream.ReadToEnd().Split('\n');
					var version = new Version(responseText[0]);
					this.downloadUrl = responseText[1];

					if (version.CompareTo(Assembly.GetEntryAssembly().GetName().Version) > 0)
					{
						if (MessageBox.Show("An update was found!\n\nVersion: " + version.ToString() + "\n\nDo you want to download and install it now?", "Update check", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
						{
							this.DownloadUpdate();
						}
					}
				}
			}
			catch (WebException exception)
			{
				MessageBox.Show(exception.Message + "\n\nCheck http://www.selfcoders.com for updates.", "Update check failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void DownloadUpdate()
		{
			var trayIcon = new NotifyIcon();
			trayIcon.Icon = Properties.Resources.DownloadUpdateTray;
			trayIcon.Text = "Capture2Net is downloading an update";
			trayIcon.Visible = true;
			trayIcon.ShowBalloonTip(5000, "Capture2Net Update", "Capture2Net is now downloading the update.\n\nThis may take a while.", ToolTipIcon.Info);
			try
			{
				var webRequest = (HttpWebRequest)WebRequest.Create(this.downloadUrl);

				webRequest.Method = "GET";
				webRequest.UserAgent = "Capture2Net";

				var response = (HttpWebResponse)webRequest.GetResponse();
				if (response.StatusCode == HttpStatusCode.OK)
				{
					var responseStream = response.GetResponseStream();

					var tempFile = System.IO.Path.GetTempFileName();

					var outputStream = new FileStream(tempFile, FileMode.Create);

					responseStream.CopyTo(outputStream);
					
					outputStream.Close();
					responseStream.Close();

					File.Move(tempFile, tempFile + ".exe");
					tempFile += ".exe";

					Process.Start(tempFile);
					this.doExit = true;
				}
			}
			catch (WebException exception)
			{
				MessageBox.Show(exception.Message + "\n\nCheck http://www.selfcoders.com for updates.", "Update download failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			trayIcon.Dispose();
		}
	}
}