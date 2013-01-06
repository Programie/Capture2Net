qx.Class.define("capture2net.view.dialogbox.AlertBox",
{
	extend : qx.core.Object,
	
	construct : function(parent)
	{
		this._parent = parent;
		this._data = this._parent._currentItem;
		var icon = this._data.icon;
		if (icon)
		{
			icon = "resource/capture2net/icons/dialogBox/" + icon + ".png";
		}
		var text = new qx.ui.basic.Atom(this._data.text, icon);
		text.setRich(true);
		this._parent._window.add(text);
		
		this.setAdditionalContent(this._parent._window, this._data);
		
		var buttonContainer = new qx.ui.container.Composite();
		var buttonContainerLayout = new qx.ui.layout.HBox(10);
		buttonContainerLayout.setAlignX(this._data.buttonAlign ? this._data.buttonAlign : "center");
		buttonContainer.setLayout(buttonContainerLayout);
		
		var acceptButton = new qx.ui.form.Button(this._data.acceptButton ? this._data.acceptButton : "OK");
		acceptButton.addListener("execute", this.triggerAccept, this);
		buttonContainer.add(acceptButton);
		
		if (this._data.declineButton)
		{
			var declineButton = new qx.ui.form.Button(this._data.declineButton);
			declineButton.addListener("execute", this.triggerDecline, this);
			buttonContainer.add(declineButton);
		}
		
		this._parent._window.add(buttonContainer);
	},
	
	members :
	{
		_data : null,
		_parent : null,
		
		getActionData : function()
		{
			return null;
		},
		
		setAdditionalContent : function(window, data)
		{
		},
		
		triggerAccept : function()
		{
			this._parent.acceptAction(this.getActionData());
		},
		
		triggerDecline : function()
		{
			if (this._data.declineButton)
			{
				this._parent.declineAction();
			}
		}
	}
});