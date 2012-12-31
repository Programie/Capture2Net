qx.Class.define("capture2net.view.panel.Main",
{
	extend : qx.ui.tabview.TabView,
	
	construct : function()
	{
		this.base(arguments);
		
		this.setBarPosition("left");
		this.setContentPadding(0);
		
		// Initialize pages
		this.add(new capture2net.view.panel.ScreenshotManager("Screenshot Manager"));
	}
});