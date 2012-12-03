namespace Capture2Net
{
 partial class ConfigWindow
 {
  /// <summary>
  /// Required designer variable.
  /// </summary>
  private System.ComponentModel.IContainer components = null;

  /// <summary>
  /// Clean up any resources being used.
  /// </summary>
  /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
  protected override void Dispose(bool disposing)
  {
   if (disposing && (components != null))
   {
	components.Dispose();
   }
   base.Dispose(disposing);
  }

  #region Windows Form Designer generated code

  /// <summary>
  /// Required method for Designer support - do not modify
  /// the contents of this method with the code editor.
  /// </summary>
  private void InitializeComponent()
  {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigWindow));
			this.HostnameText = new System.Windows.Forms.Label();
			this.PathText = new System.Windows.Forms.Label();
			this.UsernameText = new System.Windows.Forms.Label();
			this.PasswordText = new System.Windows.Forms.Label();
			this.Save = new System.Windows.Forms.Button();
			this.HostPortSeparatorLabel = new System.Windows.Forms.Label();
			this.Password = new System.Windows.Forms.TextBox();
			this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.trayIconMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.trayIconMenu_Show = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.trayIconMenu_ShortcutInfo = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.trayIconMenu_Quit = new System.Windows.Forms.ToolStripMenuItem();
			this.StartWithWindows = new System.Windows.Forms.CheckBox();
			this.Port = new System.Windows.Forms.NumericUpDown();
			this.Protocol = new System.Windows.Forms.ComboBox();
			this.Username = new System.Windows.Forms.TextBox();
			this.Path = new System.Windows.Forms.TextBox();
			this.Hostname = new System.Windows.Forms.TextBox();
			this.LimitToOneInstance = new System.Windows.Forms.CheckBox();
			this.trayIconMenu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.Port)).BeginInit();
			this.SuspendLayout();
			// 
			// HostnameText
			// 
			this.HostnameText.AutoSize = true;
			this.HostnameText.Location = new System.Drawing.Point(12, 9);
			this.HostnameText.Name = "HostnameText";
			this.HostnameText.Size = new System.Drawing.Size(41, 13);
			this.HostnameText.TabIndex = 0;
			this.HostnameText.Text = "Server:";
			// 
			// PathText
			// 
			this.PathText.AutoSize = true;
			this.PathText.Location = new System.Drawing.Point(12, 36);
			this.PathText.Name = "PathText";
			this.PathText.Size = new System.Drawing.Size(32, 13);
			this.PathText.TabIndex = 2;
			this.PathText.Text = "Path:";
			// 
			// UsernameText
			// 
			this.UsernameText.AutoSize = true;
			this.UsernameText.Location = new System.Drawing.Point(12, 62);
			this.UsernameText.Name = "UsernameText";
			this.UsernameText.Size = new System.Drawing.Size(58, 13);
			this.UsernameText.TabIndex = 3;
			this.UsernameText.Text = "Username:";
			// 
			// PasswordText
			// 
			this.PasswordText.AutoSize = true;
			this.PasswordText.Location = new System.Drawing.Point(12, 88);
			this.PasswordText.Name = "PasswordText";
			this.PasswordText.Size = new System.Drawing.Size(56, 13);
			this.PasswordText.TabIndex = 4;
			this.PasswordText.Text = "Password:";
			// 
			// Save
			// 
			this.Save.Location = new System.Drawing.Point(272, 111);
			this.Save.Name = "Save";
			this.Save.Size = new System.Drawing.Size(100, 30);
			this.Save.TabIndex = 13;
			this.Save.Text = "Save";
			this.Save.UseVisualStyleBackColor = true;
			this.Save.Click += new System.EventHandler(this.Save_Click);
			// 
			// HostPortSeparatorLabel
			// 
			this.HostPortSeparatorLabel.AutoSize = true;
			this.HostPortSeparatorLabel.Location = new System.Drawing.Point(303, 9);
			this.HostPortSeparatorLabel.Name = "HostPortSeparatorLabel";
			this.HostPortSeparatorLabel.Size = new System.Drawing.Size(10, 13);
			this.HostPortSeparatorLabel.TabIndex = 16;
			this.HostPortSeparatorLabel.Text = ":";
			// 
			// Password
			// 
			this.Password.Location = new System.Drawing.Point(76, 85);
			this.Password.Name = "Password";
			this.Password.Size = new System.Drawing.Size(296, 20);
			this.Password.TabIndex = 9;
			this.Password.UseSystemPasswordChar = true;
			// 
			// trayIcon
			// 
			this.trayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
			this.trayIcon.BalloonTipText = "Capture2Net is now running in the background to receive hotkeys.\r\nDoubleclick on " +
    "this icon to show the configuration window.\r\n\r\nClick on this tool tip to never s" +
    "how this message again.";
			this.trayIcon.BalloonTipTitle = "Capture2Net";
			this.trayIcon.ContextMenuStrip = this.trayIconMenu;
			this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
			this.trayIcon.Text = "Capture2Net";
			this.trayIcon.Visible = true;
			this.trayIcon.BalloonTipClicked += new System.EventHandler(this.trayIcon_BalloonTipClicked);
			this.trayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseDoubleClick);
			// 
			// trayIconMenu
			// 
			this.trayIconMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trayIconMenu_Show,
            this.toolStripSeparator1,
            this.trayIconMenu_ShortcutInfo,
            this.toolStripSeparator2,
            this.trayIconMenu_Quit});
			this.trayIconMenu.Name = "trayIconMenu";
			this.trayIconMenu.Size = new System.Drawing.Size(186, 82);
			// 
			// trayIconMenu_Show
			// 
			this.trayIconMenu_Show.Name = "trayIconMenu_Show";
			this.trayIconMenu_Show.Size = new System.Drawing.Size(185, 22);
			this.trayIconMenu_Show.Text = "Show";
			this.trayIconMenu_Show.ToolTipText = "Show the configuration window.";
			this.trayIconMenu_Show.Click += new System.EventHandler(this.trayIconMenu_Show_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(182, 6);
			// 
			// trayIconMenu_ShortcutInfo
			// 
			this.trayIconMenu_ShortcutInfo.Name = "trayIconMenu_ShortcutInfo";
			this.trayIconMenu_ShortcutInfo.Size = new System.Drawing.Size(185, 22);
			this.trayIconMenu_ShortcutInfo.Text = "Shortcut Information";
			this.trayIconMenu_ShortcutInfo.Click += new System.EventHandler(this.trayIconMenu_ShortcutInfo_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(182, 6);
			// 
			// trayIconMenu_Quit
			// 
			this.trayIconMenu_Quit.Name = "trayIconMenu_Quit";
			this.trayIconMenu_Quit.Size = new System.Drawing.Size(185, 22);
			this.trayIconMenu_Quit.Text = "Quit";
			this.trayIconMenu_Quit.Click += new System.EventHandler(this.trayIconMenu_Quit_Click);
			// 
			// StartWithWindows
			// 
			this.StartWithWindows.AutoSize = true;
			this.StartWithWindows.Location = new System.Drawing.Point(144, 124);
			this.StartWithWindows.Name = "StartWithWindows";
			this.StartWithWindows.Size = new System.Drawing.Size(117, 17);
			this.StartWithWindows.TabIndex = 19;
			this.StartWithWindows.Text = "Start with Windows";
			this.StartWithWindows.ThreeState = true;
			this.StartWithWindows.UseVisualStyleBackColor = true;
			// 
			// Port
			// 
			this.Port.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Capture2Net.Properties.Settings.Default, "port", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.Port.Location = new System.Drawing.Point(315, 7);
			this.Port.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.Port.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.Port.Name = "Port";
			this.Port.Size = new System.Drawing.Size(57, 20);
			this.Port.TabIndex = 17;
			this.Port.Value = global::Capture2Net.Properties.Settings.Default.port;
			// 
			// Protocol
			// 
			this.Protocol.DataBindings.Add(new System.Windows.Forms.Binding("ValueMember", global::Capture2Net.Properties.Settings.Default, "protocol", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.Protocol.DisplayMember = "HTTP";
			this.Protocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Protocol.Items.AddRange(new object[] {
            "HTTP",
            "HTTPS"});
			this.Protocol.Location = new System.Drawing.Point(76, 6);
			this.Protocol.Name = "Protocol";
			this.Protocol.Size = new System.Drawing.Size(60, 21);
			this.Protocol.TabIndex = 15;
			this.Protocol.ValueMember = global::Capture2Net.Properties.Settings.Default.protocol;
			this.Protocol.SelectedIndexChanged += new System.EventHandler(this.Protocol_SelectedIndexChanged);
			// 
			// Username
			// 
			this.Username.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Capture2Net.Properties.Settings.Default, "username", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.Username.Location = new System.Drawing.Point(76, 59);
			this.Username.Name = "Username";
			this.Username.Size = new System.Drawing.Size(296, 20);
			this.Username.TabIndex = 8;
			this.Username.Text = global::Capture2Net.Properties.Settings.Default.username;
			// 
			// Path
			// 
			this.Path.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Capture2Net.Properties.Settings.Default, "path", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.Path.Location = new System.Drawing.Point(76, 33);
			this.Path.Name = "Path";
			this.Path.Size = new System.Drawing.Size(296, 20);
			this.Path.TabIndex = 7;
			this.Path.Text = global::Capture2Net.Properties.Settings.Default.path;
			// 
			// Hostname
			// 
			this.Hostname.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Capture2Net.Properties.Settings.Default, "hostname", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.Hostname.Location = new System.Drawing.Point(142, 6);
			this.Hostname.Name = "Hostname";
			this.Hostname.Size = new System.Drawing.Size(160, 20);
			this.Hostname.TabIndex = 5;
			this.Hostname.Text = global::Capture2Net.Properties.Settings.Default.hostname;
			// 
			// LimitToOneInstance
			// 
			this.LimitToOneInstance.AutoSize = true;
			this.LimitToOneInstance.Checked = global::Capture2Net.Properties.Settings.Default.limitToOneInstance;
			this.LimitToOneInstance.CheckState = System.Windows.Forms.CheckState.Checked;
			this.LimitToOneInstance.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Capture2Net.Properties.Settings.Default, "limitToOneInstance", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.LimitToOneInstance.Location = new System.Drawing.Point(15, 124);
			this.LimitToOneInstance.Name = "LimitToOneInstance";
			this.LimitToOneInstance.Size = new System.Drawing.Size(123, 17);
			this.LimitToOneInstance.TabIndex = 20;
			this.LimitToOneInstance.Text = "Limit to one instance";
			this.LimitToOneInstance.UseVisualStyleBackColor = true;
			// 
			// ConfigWindow
			// 
			this.AcceptButton = this.Save;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 152);
			this.Controls.Add(this.LimitToOneInstance);
			this.Controls.Add(this.StartWithWindows);
			this.Controls.Add(this.Port);
			this.Controls.Add(this.HostPortSeparatorLabel);
			this.Controls.Add(this.Protocol);
			this.Controls.Add(this.Save);
			this.Controls.Add(this.Password);
			this.Controls.Add(this.Username);
			this.Controls.Add(this.Path);
			this.Controls.Add(this.Hostname);
			this.Controls.Add(this.PasswordText);
			this.Controls.Add(this.UsernameText);
			this.Controls.Add(this.PathText);
			this.Controls.Add(this.HostnameText);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "ConfigWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Capture2Net";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigWindow_FormClosing);
			this.Load += new System.EventHandler(this.ConfigWindow_Load);
			this.trayIconMenu.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.Port)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

  }

  #endregion

  private System.Windows.Forms.Label HostnameText;
  private System.Windows.Forms.Label PathText;
  private System.Windows.Forms.Label UsernameText;
  private System.Windows.Forms.Label PasswordText;
  private System.Windows.Forms.TextBox Hostname;
  private System.Windows.Forms.TextBox Path;
  private System.Windows.Forms.TextBox Username;
  private System.Windows.Forms.TextBox Password;
  private System.Windows.Forms.Button Save;
  private System.Windows.Forms.ComboBox Protocol;
  private System.Windows.Forms.Label HostPortSeparatorLabel;
  private System.Windows.Forms.NumericUpDown Port;
  private System.Windows.Forms.NotifyIcon trayIcon;
  private System.Windows.Forms.ContextMenuStrip trayIconMenu;
  private System.Windows.Forms.ToolStripMenuItem trayIconMenu_Show;
  private System.Windows.Forms.ToolStripMenuItem trayIconMenu_Quit;
  private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
  private System.Windows.Forms.ToolStripMenuItem trayIconMenu_ShortcutInfo;
  private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
  private System.Windows.Forms.CheckBox StartWithWindows;
  private System.Windows.Forms.CheckBox LimitToOneInstance;
 }
}

