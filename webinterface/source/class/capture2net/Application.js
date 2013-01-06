/**
 * This is the main application class of your custom application "capture2net"
 */
qx.Class.define("capture2net.Application",
{
	extend : qx.application.Standalone,
	
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
			
			// Set blocker data
			this.getRoot().set(
			{
				blockerColor : "#000000",
				blockerOpacity : 0.5
			});
			
			this.initializeChildWidgets();
			
			// Try to load the configuration
			capture2net.services.RPC.callMethod("loadConfig", this, this.configLoaded, [[]]);
		},
		
		/**
		 * This method gets called as soon as the loadConfig call returns
		 */
		configLoaded : function(result)
		{
			var container = new qx.ui.container.Composite();
			container.setLayout(new qx.ui.layout.VBox);
			
			var headerContainer = new qx.ui.container.Composite();
			headerContainer.setLayout(new qx.ui.layout.HBox);
			headerContainer.setPadding(10);
			headerContainer.setDecorator("app-header");
			var headerText = new qx.ui.basic.Label("Capture2Net Webinterface");
			headerText.setFont(new qx.bom.Font(22, ["JosefinSlab", "serif"]));
			headerText.setTextColor("#FFFFFF");
			headerContainer.add(headerText);
			headerContainer.add(new qx.ui.core.Spacer, {flex : 1});
			var logoutButton = new qx.ui.form.Button("Logout", "resource/capture2net/icons/common/22/logout.png");
			logoutButton.addListener("execute", this.logout, this);
			headerContainer.add(logoutButton);
			container.add(headerContainer);
			
			var panel = new capture2net.view.panel.Main();
			container.add(panel, {flex : 1});
			
			this.getRoot().add(container, {edge : 0});
		},
		
		initializeChildWidgets : function()
		{
			// Remove all child widgets
			this.getRoot().removeAll();
			
			// Initialize static views
			capture2net.view.dialogbox.Main.createWindow(this);
		},
		
		logout : function()
		{
			capture2net.services.RPC.callMethod("logout", this, function()
			{
				this.initializeChildWidgets();
				capture2net.services.RPC.callMethod("loadConfig", this, this.configLoaded, [[]]);// Should now return "login_required" and show the login form
			}, [[]]);
		}
	}
});