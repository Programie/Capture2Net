using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using System.Xml;

namespace Capture2Net
{
	class SSLChain
	{
		List<string> keys;
		
		public SSLChain()
		{
			this.LoadChain();
		}

		public bool LoadChain()
		{
			if (this.keys != null)
			{
				this.keys.Clear();
			}
			var registryKey = Registry.CurrentUser.OpenSubKey("Software\\Selfcoders\\Capture2Net");
			if (registryKey == null)
			{
				this.keys = new List<string>();
				return false;
			}
			var data = (string[])registryKey.GetValue("SSLKeyChain");
			if (data == null)
			{
				this.keys = new List<string>();
			}
			else
			{
				this.keys = new List<string>(data);
			}
			return true;
		}

		public void SaveChain()
		{
			var registryKey = Registry.CurrentUser.CreateSubKey("Software\\Selfcoders\\Capture2Net");
			registryKey.SetValue("SSLKeyChain", this.keys.ToArray(), RegistryValueKind.MultiString);
		}

		public bool CheckPublicKey(string publicKey)
		{
			foreach (var key in this.keys)
			{
				if (key == publicKey)
				{
					return true;
				}
			}
			return false;
		}

		public void AddPublicKey(string publicKey)
		{
			if (this.keys.Contains(publicKey))
			{
				return;
			}
			this.keys.Add(publicKey);
		}

		public bool CheckCertificate(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			// Check if the certificate is valid
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				return true;
			}

			// Search for the certificate in our SSLChain
			if (this.CheckPublicKey(certification.GetPublicKeyString()))
			{
				return true;
			}

			// Ask the user
			if (MessageBox.Show("The certificate could not be verified! Do you want to add it to the SSL Chain?\n\n\n" + certification.Subject, "Certificate validation failed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
			{
				this.AddPublicKey(certification.GetPublicKeyString());
				this.SaveChain();
				return true;
			}

			// Do not accept the certificate
			return false;
		}
	}
}