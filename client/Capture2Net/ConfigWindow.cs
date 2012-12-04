﻿using Microsoft.Win32;
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
		Shortcuts shortcutsInstance;
		ShortcutInfo shortcutInfoForm;
		RegistryKey autostartRegistryKey;

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
				this.shortcutsInstance.Register();
				Properties.Settings.Default.Save();

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

				if (Properties.Settings.Default.showHiddenBalloonTip)
				{
					this.showTrayBalloonTip();
				}
			}
			else
			{
				Properties.Settings.Default.password = "";// Unset password
			}
		}

		private void ConfigWindow_Load(object sender, EventArgs e)
		{
			this.Protocol.SelectedItem = Properties.Settings.Default.protocol;
			this.Password.Text = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(Properties.Settings.Default.password));

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
			Properties.Settings.Default.showHiddenBalloonTip = false;
			Properties.Settings.Default.Save();
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
	}
}