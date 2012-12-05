using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capture2Net
{
	class Settings
	{
		const string subKey = "Software\\SelfCoders\\Capture2Net";
		
		string protocol;
		public string Protocol
		{
			get
			{
				var value = this.protocol.ToLower();
				if (value != "http" && value != "https")
				{
					value = "http";
				}
				return value;
			}
			set
			{
				value = value.ToLower();
				if (value != "http" && value != "https")
				{
					value = "http";
				}
				this.protocol = value;
			}
		}

		string hostname;
		public string Hostname
		{
			get
			{
				return this.hostname;
			}
			set
			{
				this.hostname = value;
			}
		}

		decimal port;
		public decimal Port
		{
			get
			{
				if (this.port < 1 || this.port > 65535)
				{
					return 80;
				}
				return this.port;
			}
			set
			{
				if (value < 1 || value > 65535)
				{
					this.port = 80;
				}
				else
				{
					this.port = value;
				}
			}
		}

		string path;
		public string Path
		{
			get
			{
				return Utils.GetValidPath(this.path);
			}
			set
			{
				this.path = Utils.GetValidPath(value);
			}
		}

		string username;
		public string Username
		{
			get
			{
				return this.username;
			}
			set
			{
				this.username = value;
			}
		}

		string password;
		public string Password
		{
			get
			{
				return this.password;
			}
			set
			{
				this.password = value;
			}
		}

		bool limitToOneInstance;
		public bool LimitToOneInstance
		{
			get
			{
				return this.limitToOneInstance;
			}
			set
			{
				this.limitToOneInstance = value;
			}
		}

		bool showHiddenBalloonTip;
		public bool ShowHiddenBalloonTip
		{
			get
			{
				return this.showHiddenBalloonTip;
			}
			set
			{
				this.showHiddenBalloonTip = value;
			}
		}
		
		public Settings()
		{
			this.Load();
		}

		public void Load()
		{
			var key = Registry.CurrentUser.CreateSubKey(subKey);
			this.Protocol = (string)key.GetValue("Protocol", "http");
			this.Hostname = (string)key.GetValue("Hostname");
			this.Port = Convert.ToDecimal(key.GetValue("Port", 80));
			this.Path = (string)key.GetValue("Path", "/");
			this.Username = (string)key.GetValue("Username");
			this.Password = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String((string)key.GetValue("Password")));
			this.LimitToOneInstance = Convert.ToBoolean(key.GetValue("LimitToOneInstance", true));
			this.ShowHiddenBalloonTip = Convert.ToBoolean(key.GetValue("ShowHiddenBalloonTip", true));
		}

		public void Save()
		{
			var key = Registry.CurrentUser.CreateSubKey(subKey);
			key.SetValue("Protocol", this.Protocol);
			key.SetValue("Hostname", this.Hostname);
			key.SetValue("Port", this.Port, RegistryValueKind.DWord);
			key.SetValue("Path", this.Path);
			key.SetValue("Username", this.Username);
			key.SetValue("Password", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(this.Password)));
			key.SetValue("LimitToOneInstance", this.LimitToOneInstance, RegistryValueKind.DWord);
			key.SetValue("ShowHiddenBalloonTip", this.ShowHiddenBalloonTip, RegistryValueKind.DWord);
		}
	}
}