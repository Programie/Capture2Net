using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace Capture2Net
{
	class SSLChain
	{
		List<string> keys = new List<string>();
		
		public SSLChain()
		{
			this.LoadChain();
		}

		public bool LoadChain()
		{
			this.keys.Clear();
			try
			{
				var reader = new XmlTextReader("SSLChain.xml");
				while (reader.Read())
				{
					if (reader.Name == "certificate")
					{
						string key = reader.ReadElementContentAsString();
						if (!this.keys.Contains(key))
						{
							this.keys.Add(key);
						}
					}
				}
				reader.Close();
				return true;
			}
			catch (System.IO.FileNotFoundException)
			{
				// File not found -> OK, the chain does not exist yet
			}
			catch (XmlException exception)
			{
				MessageBox.Show("An error occured while loading the SSL Chain!\n\n\n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return false;
		}

		public void SaveChain()
		{
			var writer = new XmlTextWriter("SSLChain.xml", System.Text.Encoding.UTF8);
			writer.WriteStartDocument();
			writer.WriteStartElement("chain");
			foreach (var key in this.keys)
			{
				writer.WriteElementString("certificate", key);
			}
			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Close();
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

		public bool CheckCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
		{
			// Check if the certificate is valid
			if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
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