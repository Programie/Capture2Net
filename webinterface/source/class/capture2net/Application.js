/**
 * This is the main application class of your custom application "capture2net"
 */
qx.Class.define("capture2net.Application",
{
	extend : qx.application.Standalone,
	
	events :
	{
		configLoaded : "qx.event.type.Data"
	},
	
	members :
	{
		/**
		 * This method contains the initial application code and gets called during startup of the application
		 */
		main : function()
		{
			// Call super class
			this.base(arguments);
			
			// Enable logging in debug variant
			if (qx.core.Environment.get("qx.debug"))
			{
				// support native logging capabilities, e.g. Firebug for Firefox
				qx.log.appender.Native;
				// support additional cross-browser console. Press F7 to toggle visibility
				qx.log.appender.Console;
			}
			
			// Register event listeners
			this.addListener("loadConfig", this.configLoaded, this);
			
			// Try to load the configuration
			capture2net.services.RPC.callMethod("loadConfig", this, "configLoaded");
		},
		
		/**
		 * This method gets called as soon as the loadConfig call returns
		 */
		configLoaded : function(event)
		{
			var result = event.getData();
			alert(result);
		}
	}
});