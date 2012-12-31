qx.Class.define("capture2net.view.dialogbox.Form",
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
		text.setPadding(5);
		parent._window.add(text);
		
		if (data.formFields)
		{
			var form = new qx.ui.form.Form();
			for (var formFieldIndex in data.formFields)
			{
				var formField = data.formFields[formFieldIndex];
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
			parent._window.add(formRenderer);
		}
		
		var buttonContainer = new qx.ui.container.Composite();
		var buttonContainerLayout = new qx.ui.layout.HBox(10);
		buttonContainerLayout.setAlignX(data.buttonAlign ? data.buttonAlign : "right");
		buttonContainer.setLayout(buttonContainerLayout);
		
		var acceptButton = new qx.ui.form.Button(data.acceptButton ? data.acceptButton : "OK");
		acceptButton.addListener("execute", function()
		{
			var error = false;
			var values = {};
			for (var formFieldIndex in data.formFields)
			{
				var formField = data.formFields[formFieldIndex];
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
				parent.acceptAction(values);
			}
		}, this);
		buttonContainer.add(acceptButton);
		
		if (data.declineButton)
		{
			var declineButton = new qx.ui.form.Button(data.declineButton);
			declineButton.addListener("execute", function()
			{
				parent.acceptAction();
			}, this);
			buttonContainer.add(declineButton);
		}
		
		parent._window.add(buttonContainer);
	}
});