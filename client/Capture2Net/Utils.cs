using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Capture2Net
{
	static class Utils
	{
		public static string GetValidPath(string path)
		{
			if (path.Length == 0 || path.Substring(0, 1) != "/")
			{
				path = "/" + path;
			}
			if (path.Substring(path.Length - 1, 1) != "/")
			{
				path += "/";
			}
			return path;
		}

		public static string GetHumanReadableFileSize(long size)
		{
			string[] units = {"B", "KB", "MB", "GB", "TB", "PB", "EB"};

			string suffix = "";
			for (int unitIndex = 0; unitIndex < units.Length; unitIndex++)
			{
				if (size < 1024)
				{
					suffix = units[unitIndex];
					break;
				}
				size /= 1024;
			}
			return size.ToString("0.##") + " " + suffix;
		}
	}
}