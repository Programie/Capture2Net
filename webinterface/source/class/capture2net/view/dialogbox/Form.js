qx.Class.define("capture2net.view.dialogbox.Form",
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
		text.setPadding(5);
		this._parent._window.add(text);
		
		if (this._data.formFields)
		{
			var form = new qx.ui.form.Form();
			for (var formFieldIndex in this._data.formFields)
			{
				var formField = this._data.formFields[formFieldIndex];
				switch (formField.type)
				{
					case "checkBox":
						formField.field = new qx.ui.form.CheckBox(formField.label);
						formField.field.setValue(formField.value);
						break;
					case "password":
						formField.field = new qx.ui.form.PasswordField(formField.value);
						break;
					case "text":
						formField.field = new qx.ui.form.TextField(formField.value);
						break;
				}
				if (formField.field)
				{
					form.add(formField.field, formField.text);
				}
			}
			var formRenderer = new qx.ui.form.renderer.Single(form);
			formRenderer.setMargin(10);
			this._parent._window.add(formRenderer);
		}
		
		var buttonContainer = new qx.ui.container.Composite();
		var buttonContainerLayout = new qx.ui.layout.HBox(10);
		buttonContainerLayout.setAlignX(this._data.buttonAlign ? this._data.buttonAlign : "right");
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
		triggerAccept : function()
		{
			var error = false;
			var values = {};
			for (var formFieldIndex in this._data.formFields)
			{
				var formField = this._data.formFields[formFieldIndex];
				if (formField.field)
				{
					if (formField.required && !formField.field.getValue())
					{
						formField.field.setInvalidMessage("This field is required!");
						formField.field.setValid(false);
						error = true;
					}
					else
					{
						formField.field.setValid(true);
						values[formFieldIndex] = formField.field.getValue();
					}
				}
			}
			if (!error)
			{
				this._parent.acceptAction(values);
			}
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