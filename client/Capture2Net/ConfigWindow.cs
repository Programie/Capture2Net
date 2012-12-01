using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Capture2Net
{
	public partial class ConfigWindow : Form
	{
		bool closingFromTrayIconMenu;
		ParameterManager parameterManagerInstance;
		CloudConfig cloudConfigInstance;
		ShortcutInfo shortcutInfoForm;

		public ConfigWindow(ParameterManager parameterManagerInstance, CloudConfig cloudConfigInstance)
		{
			InitializeComponent();

			this.parameterManagerInstance = parameterManagerInstance;
			this.cloudConfigInstance = cloudConfigInstance;
		}

		private void Save_Click(object sender, EventArgs e)
		{
			if (Properties.Settings.Default.hostname == "" || Properties.Settings.Default.username == "" || this.Password.Text == "")
			{
				MessageBox.Show("Hostname, username and password are required fields!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			Properties.Settings.Default.protocol = this.Protocol.SelectedItem.ToString();

			Properties.Settings.Default.path = Utils.GetValidPath(Properties.Settings.Default.path);
			Properties.Settings.Default.password = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(this.Password.Text));

			if (this.cloudConfigInstance.Load())
			{
				this.cloudConfigInstance.RegisterGlobalHotkeys();
				Properties.Settings.Default.Save();

				MessageBox.Show("Configuration saved!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

				this.Hide();

				if (Properties.Settings.Default.showHiddenBalloonTip)
				{
					this.showTrayBalloonTip();
				}
			}
			else
			{
				Properties.Settings.Default.password = "";// Unsed password
			}
		}

		private void ConfigWindow_Load(object sender, EventArgs e)
		{
			this.Protocol.SelectedItem = Properties.Settings.Default.protocol;
			this.Password.Text = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(Properties.Settings.Default.password));
		}

		private void Protocol_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (this.Protocol.SelectedItem.ToString().ToLower())
			{
				case "http":
					this.Port.Value = 80;
					break;
				case "https":
					this.Port.Value = 443;
					break;
			}
		}

		internal void showTrayBalloonTip()
		{
			this.trayIcon.ShowBalloonTip(5000);
		}

		private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			this.Show();
		}

		private void ConfigWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.closingFromTrayIconMenu || Properties.Settings.Default.hostname == "" || Properties.Settings.Default.username == "" || Properties.Settings.Default.password == "")
			{
				Application.Exit();
				return;
			}
			e.Cancel = true;
			this.Hide();
			
			if (Properties.Settings.Default.showHiddenBalloonTip)
			{
				this.showTrayBalloonTip();
			}
		}

		private void trayIconMenu_Show_Click(object sender, EventArgs e)
		{
			this.Show();
		}

		private void trayIconMenu_Quit_Click(object sender, EventArgs e)
		{
			this.closingFromTrayIconMenu = true;
			this.Close();
			Application.Exit();
		}

		private void trayIcon_BalloonTipClicked(object sender, EventArgs e)
		{
			Properties.Settings.Default.showHiddenBalloonTip = false;
			Properties.Settings.Default.Save();
		}

		private void trayIconMenu_ShortcutInfo_Click(object sender, EventArgs e)
		{
			if (this.shortcutInfoForm == null || this.shortcutInfoForm.IsDisposed)
			{
				this.shortcutInfoForm = new ShortcutInfo();
			}
			this.shortcutInfoForm.ShortcutScreen = string.Join("+", this.cloudConfigInstance.RegisteredShortcutScreen.ToArray());
			this.shortcutInfoForm.ShortcutSelection = string.Join("+", this.cloudConfigInstance.RegisteredShortcutSelection.ToArray());
			this.shortcutInfoForm.ShortcutWindow = string.Join("+", this.cloudConfigInstance.RegisteredShortcutWindow.ToArray());
			this.shortcutInfoForm.Show();
		}
	}
}