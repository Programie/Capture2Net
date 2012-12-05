using System;
using System.IO;
using System.Reflection;
using System.Threading;
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
			var shortcutsInstance = new Shortcuts(cloudConfigInstance);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			new Mutex(true, "Capture2Net_RunCheck");// Only used for installer to check if Capture2Net is running

			var uploadFile = parameterManagerInstance.GetParameter("upload");
			if (uploadFile == null)
			{
				var mutex = new Mutex(true, "Capture2Net_Main");
				if (Properties.Settings.Default.limitToOneInstance)
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
				if (Properties.Settings.Default.hostname == "" || Properties.Settings.Default.username == "" || Properties.Settings.Default.password == "")
				{
					configWindow.Show();
				}
				else
				{
					if (cloudConfigInstance.Load())
					{
						shortcutsInstance.Register();
					}
					if (Properties.Settings.Default.showHiddenBalloonTip)
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