using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
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
		Shortcuts shortcutsInstance;
		ShortcutInfo shortcutInfoForm;
		RegistryKey autostartRegistryKey;
		PendingUploads pendingUploadsInstance;

		public ConfigWindow(ParameterManager parameterManagerInstance, CloudConfig cloudConfigInstance, Shortcuts shortcutsInstance)
		{
			InitializeComponent();

			this.parameterManagerInstance = parameterManagerInstance;
			this.cloudConfigInstance = cloudConfigInstance;
			this.shortcutsInstance = shortcutsInstance;

			this.autostartRegistryKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
		}

		private void Save_Click(object sender, EventArgs e)
		{
			if (this.Hostname.Text == "" || this.Username.Text == "" || this.Password.Text == "")
			{
				MessageBox.Show("Hostname, username and password are required fields!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			Program.settingsInstance.Protocol = (string)this.Protocol.SelectedItem;
			Program.settingsInstance.Hostname = this.Hostname.Text;
			Program.settingsInstance.Port = this.Port.Value;
			Program.settingsInstance.Path = this.Path.Text;
			Program.settingsInstance.Username = this.Username.Text;
			Program.settingsInstance.Password = this.Password.Text;
			Program.settingsInstance.LimitToOneInstance = this.LimitToOneInstance.CheckState == CheckState.Checked;
			Program.settingsInstance.CheckUpdatesOnStart = this.CheckUpdatesOnStart.CheckState == CheckState.Checked;

			if (this.cloudConfigInstance.Load())
			{
				Program.settingsInstance.Save();
				this.shortcutsInstance.Register();

				switch (this.StartWithWindows.CheckState)
				{
					case CheckState.Checked:
						this.autostartRegistryKey.SetValue("Capture2Net", "\"" + Environment.GetCommandLineArgs()[0]+ "\"");
						break;
					case CheckState.Unchecked:
						if (this.IsAutostartEnabled() != CheckState.Unchecked)
						{
							this.autostartRegistryKey.DeleteValue("Capture2Net");
						}
						break;
				}

				MessageBox.Show("Configuration saved!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

				this.Hide();

				if (Program.settingsInstance.ShowHiddenBalloonTip)
				{
					this.showTrayBalloonTip();
				}
			}
			else
			{
				Program.settingsInstance.Password = "";// Unset password
			}
		}

		private void ConfigWindow_Load(object sender, EventArgs e)
		{
			this.Protocol.SelectedItem = Program.settingsInstance.Protocol.ToUpper();
			this.Hostname.Text = Program.settingsInstance.Hostname;
			this.Port.Value = Program.settingsInstance.Port;
			this.Path.Text = Program.settingsInstance.Path;
			this.Username.Text = Program.settingsInstance.Username;
			this.Password.Text = Program.settingsInstance.Password;
			this.LimitToOneInstance.CheckState = Program.settingsInstance.LimitToOneInstance ? CheckState.Checked : CheckState.Unchecked;
			this.CheckUpdatesOnStart.CheckState = Program.settingsInstance.CheckUpdatesOnStart ? CheckState.Checked : CheckState.Unchecked;

			this.StartWithWindows.CheckState = this.IsAutostartEnabled();
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
			this.WindowState = FormWindowState.Normal;
			this.BringToFront();// TODO: Does not bring the window to the front?
		}

		private void ConfigWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.closingFromTrayIconMenu || Program.settingsInstance.Hostname == "" || Program.settingsInstance.Username == "" || Program.settingsInstance.Password == "")
			{
				Application.Exit();
				return;
			}
			e.Cancel = true;
			this.Hide();

			if (Program.settingsInstance.ShowHiddenBalloonTip)
			{
				this.showTrayBalloonTip();
			}
		}

		private void trayIconMenu_Show_Click(object sender, EventArgs e)
		{
			this.Show();
			this.WindowState = FormWindowState.Normal;
			this.BringToFront();
		}

		private void trayIconMenu_Quit_Click(object sender, EventArgs e)
		{
			this.closingFromTrayIconMenu = true;
			this.Close();
			Application.Exit();
		}

		private void trayIcon_BalloonTipClicked(object sender, EventArgs e)
		{
			Program.settingsInstance.ShowHiddenBalloonTip = false;
			Program.settingsInstance.Save();
		}

		private void trayIconMenu_ShortcutInfo_Click(object sender, EventArgs e)
		{
			if (this.shortcutsInstance.RegisteredShortcutScreen == null || this.shortcutsInstance.RegisteredShortcutSelection == null || this.shortcutsInstance.RegisteredShortcutWindow == null)
			{
				MessageBox.Show("No configuration data loaded yet!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				if (this.shortcutInfoForm == null || this.shortcutInfoForm.IsDisposed)
				{
					this.shortcutInfoForm = new ShortcutInfo();
				}
				this.shortcutInfoForm.ShortcutScreen = string.Join("+", this.shortcutsInstance.RegisteredShortcutScreen.ToArray());
				this.shortcutInfoForm.ShortcutSelection = string.Join("+", this.shortcutsInstance.RegisteredShortcutSelection.ToArray());
				this.shortcutInfoForm.ShortcutWindow = string.Join("+", this.shortcutsInstance.RegisteredShortcutWindow.ToArray());
				this.shortcutInfoForm.Show();
				this.shortcutInfoForm.BringToFront();
			}
		}

		private CheckState IsAutostartEnabled()
		{
			var autostartValue = (string)this.autostartRegistryKey.GetValue("Capture2Net");
			if (autostartValue == "\"" + Environment.GetCommandLineArgs()[0] + "\"" || autostartValue == Environment.GetCommandLineArgs()[0])
			{
				return CheckState.Checked;
			}
			else
			{
				if (autostartValue == null)
				{
					return CheckState.Unchecked;
				}
				else
				{
					return CheckState.Indeterminate;
				}
			}
		}

		private void menuHelp_Click(object sender, EventArgs e)
		{
			Process.Start("http://selfcoders.com/projects/capture2net/help");
		}

		private void menuAbout_Click(object sender, EventArgs e)
		{
			MessageBox.Show("You are currently using version " + Assembly.GetEntryAssembly().GetName().Version.ToString() + " of Capture2Net.", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void menuShowPendingUploads_Click(object sender, EventArgs e)
		{
			if (this.pendingUploadsInstance == null)
			{
				this.pendingUploadsInstance = new PendingUploads();
				this.pendingUploadsInstance.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PendingUploads_FormClosed);
				this.pendingUploadsInstance.Show();
			}
			else
			{
				this.pendingUploadsInstance.BringToFront();
			}
		}

		private void PendingUploads_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.pendingUploadsInstance = null;
		}
	}
}