using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Capture2Net
{
	static class Program
	{
		public static Settings settingsInstance;
		
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			var parameterManagerInstance = new ParameterManager(args);
			settingsInstance = new Settings();
			var cloudConfigInstance = new CloudConfig();
			var shortcutsInstance = new Shortcuts(cloudConfigInstance);
			settingsInstance.Load();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			new Mutex(true, "Capture2Net_RunCheck");// Only used for installer to check if Capture2Net is running

			if (parameterManagerInstance.GetParameter("updated") != null)
			{
				MessageBox.Show("Capture2Net has been successfully updated to version " + Assembly.GetEntryAssembly().GetName().Version.ToString() + ".", "Capture2Net Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}

			var uploadFile = parameterManagerInstance.GetParameter("upload");
			if (uploadFile == null)
			{
				var mutex = new Mutex(true, "Capture2Net_Main");
				if (settingsInstance.LimitToOneInstance)
				{
					if (!mutex.WaitOne(TimeSpan.Zero, true))
					{
						MessageBox.Show("Another instance is already running!", "Capture2Net", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
				}

				var updateCheckInstance = new UpdateCheck();
				if (updateCheckInstance.DoExit)
				{
					return;
				}

				var configWindow = new ConfigWindow(parameterManagerInstance, cloudConfigInstance, shortcutsInstance);
				if (settingsInstance.Hostname == "" || settingsInstance.Username == "" || settingsInstance.Password == "")
				{
					configWindow.Show();
				}
				else
				{
					if (cloudConfigInstance.Load())
					{
						shortcutsInstance.Register();
					}
					if (settingsInstance.ShowHiddenBalloonTip)
					{
						configWindow.showTrayBalloonTip();
					}
				}
				Application.Run();
			}
			else
			{
				if (uploadFile == "")
				{
					MessageBox.Show("No file specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					new Uploader(uploadFile);
					
					// Try to remove temporary file
					File.Delete(uploadFile);
				}
			}
		}
	}
}