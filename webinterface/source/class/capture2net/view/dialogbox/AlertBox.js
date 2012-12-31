qx.Class.define("capture2net.view.dialogbox.AlertBox",
{
	extend : qx.core.Object,
	
	construct : function(parent)
	{
		var data = parent._currentItem;
		var icon = data.icon;
		if (icon)
		{
			icon = "resource/capture2net/dialogBox/" + icon + ".png";
		}
		var text = new qx.ui.basic.Atom(data.text, icon);
		text.setRich(true);
		parent._window.add(text);
		
		this.setAdditionalContent(parent._window, data);
		
		var buttonContainer = new qx.ui.container.Composite();
		var buttonContainerLayout = new qx.ui.layout.HBox(10);
		buttonContainerLayout.setAlignX(data.buttonAlign ? data.buttonAlign : "center");
		buttonContainer.setLayout(buttonContainerLayout);
		
		var acceptButton = new qx.ui.form.Button(data.acceptButton ? data.acceptButton : "OK");
		acceptButton.addListener("execute", function()
		{
			parent.acceptAction(this.getActionData());
		}, this);
		buttonContainer.add(acceptButton);
		
		if (data.declineButton)
		{
			var declineButton = new qx.ui.form.Button(data.declineButton);
			declineButton.addListener("execute", function()
			{
				parent.declineAction();
			}, this);
			buttonContainer.add(declineButton);
		}
		
		parent._window.add(buttonContainer);
	},
	
	members :
	{
		setAdditionalContent : function(window, data)
		{
		},
		
		getActionData : function()
		{
			return null;
		}
	}
});