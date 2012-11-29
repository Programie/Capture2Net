﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Capture2Net
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			var parameterManagerInstance = new ParameterManager(args);
			var cloudConfigInstance = new CloudConfig();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var captureMode = parameterManagerInstance.GetParameter("capture");
			if (captureMode == null)
			{
				var configWindow = new ConfigWindow(parameterManagerInstance, cloudConfigInstance);
				if (parameterManagerInstance.GetParameter("config") != null || Properties.Settings.Default.hostname == "" || Properties.Settings.Default.username == "" || Properties.Settings.Default.password == "")
				{
					configWindow.Show();
				}
				else
				{
					cloudConfigInstance.Load();
					if (Properties.Settings.Default.showHiddenBalloonTip)
					{
						configWindow.showTrayBalloonTip();
					}
				}
				Application.Run();
			}
			else
			{
				var fileExtension = parameterManagerInstance.GetParameter("imageformat");

				var screenshotInstance = new Screenshot(fileExtension, cloudConfigInstance);

				switch (captureMode.ToLower())
				{
					case "screen":
						screenshotInstance.Screen();
						break;
					case "selection":
						screenshotInstance.Selection();
						break;
					case "window":
						screenshotInstance.Window();
						break;
					default:
						var text = "";
						if (captureMode != "")
						{
							text = " '" + captureMode + "'";
						}
						MessageBox.Show("Invalid capture mode" + text + "!\n\n\nAvailable modes:\n\nscreen\nselection\nwindow", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						break;
				}
			}
		}
	}
}