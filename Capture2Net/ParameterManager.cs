using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Capture2Net
{
	public class ParameterManager
	{
		private string[] args;

		public ParameterManager(string[] args)
		{
			this.args = args;
		}

		public string GetParameter(string name)
		{
			bool useArgument = false;
			foreach (string arg in this.args)
			{
				// Check if the arguments starts with / or -
				if (arg.Substring(0, 1) == "/" || arg.Substring(0, 1) == "-")
				{
					// The previous argument was the right argument but there is no value
					if (useArgument)
					{
						return "";
					}

					// This argument is the searched one
					if (arg.ToLower() == "/" + name.ToLower() || arg.ToLower() == "-" + name.ToLower())
					{
						useArgument = true;
					}
				}
				else
				{
					// The previous argument was the right argument, so we will use the current argument as value
					if (useArgument)
					{
						return arg;
					}
				}
			}
			
			// We found the argument but there are no more following arguments
			if (useArgument)
			{
				return "";
			}

			// Argument not found
			return null;
		}
	}
}