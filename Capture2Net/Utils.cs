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
	}
}