using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace Capture2Net
{
	public class CloudConfig : IDisposable
	{
		public JObject jsonData;

		Hotkey shortcutScreen;
		Hotkey shortcutSelection;
		Hotkey shortcutWindow;
		List<string> registeredShortcutScreen;
		List<string> registeredShortcutSelection;
		List<string> registeredShortcutWindow;

		public List<string> RegisteredShortcutScreen
		{
			get
			{
				return this.registeredShortcutScreen;
			}
		}

		public List<string> RegisteredShortcutSelection
		{
			get
			{
				return this.registeredShortcutSelection;
			}
		}

		public List<string> RegisteredShortcutWindow
		{
			get
			{
				return this.registeredShortcutWindow;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			this.UnregisterGlobalHotkeys();
		}

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
			registeredShortcutScreen = new List<string>();
			registeredShortcutSelection = new List<string>();
			registeredShortcutWindow = new List<string>();
			this.UnregisterGlobalHotkeys();
			try
			{
				var jsonObject = this.jsonData["screenshots"]["screen"]["shortcut"];
				var key = (Keys)System.Convert.ToInt32(jsonObject["key"].ToString());
				int keyModifiers = (int)Hotkey.KeyModifiers.None;
				if ((bool)jsonObject["control"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Control;
					this.registeredShortcutScreen.Add("Ctrl");
				}
				if ((bool)jsonObject["alt"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Alt;
					this.registeredShortcutScreen.Add("Alt");
				}
				if ((bool)jsonObject["shift"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Shift;
					this.registeredShortcutScreen.Add("Shift");
				}
				if ((bool)jsonObject["windows"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Windows;
					this.registeredShortcutScreen.Add("Windows");
				}
				this.registeredShortcutScreen.Add(((Keys)key).ToString());
				this.shortcutScreen = new Hotkey(100, key, (Hotkey.KeyModifiers)keyModifiers, new System.EventHandler(this.ShortcutScreen));
			}
			catch (ApplicationException)
			{
				MessageBox.Show("Unable to register hotkey for 'Screen'!");
			}
			try
			{
				var jsonObject = this.jsonData["screenshots"]["selection"]["shortcut"];
				var key = (Keys)System.Convert.ToInt32(jsonObject["key"].ToString());
				int keyModifiers = (int)Hotkey.KeyModifiers.None;
				if ((bool)jsonObject["control"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Control;
					this.registeredShortcutSelection.Add("Ctrl");
				}
				if ((bool)jsonObject["alt"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Alt;
					this.registeredShortcutSelection.Add("Alt");
				}
				if ((bool)jsonObject["shift"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Shift;
					this.registeredShortcutSelection.Add("Shift");
				}
				if ((bool)jsonObject["windows"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Windows;
					this.registeredShortcutSelection.Add("Windows");
				}
				this.registeredShortcutSelection.Add(((Keys)key).ToString());
				this.shortcutSelection = new Hotkey(101, key, (Hotkey.KeyModifiers)keyModifiers, new System.EventHandler(this.ShortcutSelection));
			}
			catch (ApplicationException)
			{
				MessageBox.Show("Unable to register hotkey for 'Selection'!");
			}
			try
			{
				var jsonObject = this.jsonData["screenshots"]["window"]["shortcut"];
				var key = (Keys)System.Convert.ToInt32(jsonObject["key"].ToString());
				int keyModifiers = (int)Hotkey.KeyModifiers.None;
				if ((bool)jsonObject["control"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Control;
					this.registeredShortcutWindow.Add("Ctrl");
				}
				if ((bool)jsonObject["alt"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Alt;
					this.registeredShortcutWindow.Add("Alt");
				}
				if ((bool)jsonObject["shift"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Shift;
					this.registeredShortcutWindow.Add("Shift");
				}
				if ((bool)jsonObject["windows"])
				{
					keyModifiers += (int)Hotkey.KeyModifiers.Windows;
					this.registeredShortcutWindow.Add("Windows");
				}
				this.registeredShortcutWindow.Add(((Keys)key).ToString());
				this.shortcutWindow = new Hotkey(102, key, (Hotkey.KeyModifiers)keyModifiers, new System.EventHandler(this.ShortcutWindow));
			}
			catch (ApplicationException)
			{
				MessageBox.Show("Unable to register hotkey for 'Window'!");
			}
		}

		private void UnregisterGlobalHotkeys()
		{
			if (this.shortcutScreen != null)
			{
				this.shortcutScreen.Dispose();
				this.shortcutScreen = null;
			}
			if (this.shortcutSelection != null)
			{
				this.shortcutSelection.Dispose();
				this.shortcutSelection = null;
			}
			if (this.shortcutWindow != null)
			{
				this.shortcutWindow.Dispose();
				this.shortcutWindow = null;
			}
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