using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace Capture2Net
{
	public class CloudConfig
	{
		public JObject jsonData;

		Hotkey shortcutScreen;
		Hotkey shortcutSelection;
		Hotkey shortcutWindow;
		bool hotkeysRegistered;

		public bool Load()
		{
			try
			{
				if (Properties.Settings.Default.acceptAllCertificates)
				{
					ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(this.AcceptAllCertifications);
				}

				// Initialization
				var uri = new Uri(Properties.Settings.Default.protocol.ToLower() + "://" + Properties.Settings.Default.hostname + ":" + Properties.Settings.Default.port + Utils.GetValidPath(Properties.Settings.Default.path) + "getconfig.php");
				var webRequest = (HttpWebRequest)WebRequest.Create(uri);

				webRequest.Method = "GET";
				webRequest.UserAgent = "Capture2Net";
				webRequest.AllowWriteStreamBuffering = true;

				// Set basic authentication credentials
				var password = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(Properties.Settings.Default.password));
				var userPassword = Properties.Settings.Default.username + ":" + password;
				webRequest.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(userPassword)));
				var response = (HttpWebResponse)webRequest.GetResponse();
				if (response.StatusCode == HttpStatusCode.OK)
				{
					var responseStream = response.GetResponseStream();
					var readStream = new StreamReader(responseStream);
					this.jsonData = JObject.Parse(readStream.ReadToEnd());
					this.RegisterGlobalHotkeys();
					return true;
				}
			}
			catch (WebException exception)
			{
				var response = (HttpWebResponse)exception.Response;
				if (response == null)
				{
					MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					if (response.StatusCode == HttpStatusCode.Forbidden)
					{
						MessageBox.Show("Login failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					else
					{
						MessageBox.Show("Unexpected response from server!\n\n\n" + response.StatusDescription, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
			return false;
		}

		private bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		public void RegisterGlobalHotkeys()
		{
			if (hotkeysRegistered)
			{
				Console.WriteLine("Try unregister");
				this.shortcutScreen.UnregisterShortcut();
				this.shortcutSelection.UnregisterShortcut();
				this.shortcutWindow.UnregisterShortcut();
			}
			try
			{
				this.shortcutScreen = new Hotkey(100, Keys.PrintScreen, Hotkey.KeyModifiers.None, new System.EventHandler(this.ShortcutScreen));
			}
			catch (ApplicationException)
			{
				MessageBox.Show("Unable to register hotkey for 'Screen'!");
			}
			try
			{
				this.shortcutSelection = new Hotkey(101, Keys.PrintScreen, Hotkey.KeyModifiers.Control, new System.EventHandler(this.ShortcutSelection));
			}
			catch (ApplicationException)
			{
				MessageBox.Show("Unable to register hotkey for 'Selection'!");
			}
			try
			{
				this.shortcutWindow = new Hotkey(102, Keys.PrintScreen, Hotkey.KeyModifiers.Alt, new System.EventHandler(this.ShortcutWindow));
			}
			catch (ApplicationException)
			{
				MessageBox.Show("Unable to register hotkey for 'Window'!");
			}
			this.hotkeysRegistered = true;
		}

		private void ShortcutScreen(object sender, EventArgs e)
		{
			Process.Start(Environment.GetCommandLineArgs()[0], "/capture screen");
		}

		private void ShortcutSelection(object sender, EventArgs e)
		{
			Process.Start(Environment.GetCommandLineArgs()[0], "/capture selection");
		}

		private void ShortcutWindow(object sender, EventArgs e)
		{
			Process.Start(Environment.GetCommandLineArgs()[0], "/capture window");
		}
	}
}