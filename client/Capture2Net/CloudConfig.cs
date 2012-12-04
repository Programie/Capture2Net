using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace Capture2Net
{
	public class CloudConfig
	{
		JObject jsonData;

		public JObject JsonData
		{
			get
			{
				return this.jsonData;
			}
		}

		public bool Load()
		{
			try
			{
				ServicePointManager.ServerCertificateValidationCallback = this.CheckSSLCertificate;

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
					try
					{
						this.jsonData = JObject.Parse(readStream.ReadToEnd());
						return true;
					}
					catch (JsonReaderException exception)
					{
						MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
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

		private bool CheckSSLCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
		{
			var sslChain = new SSLChain();
			return sslChain.CheckCertificate(sender, certification, chain, sslPolicyErrors);
		}
	}
}