qx.Class.define("capture2net.view.dialogbox.PromptBox",
{
	extend : capture2net.view.dialogbox.AlertBox,
	
	members :
	{
		_inputField : null,
		
		//overridden
		getActionData : function()
		{
			return this._inputField.getValue();
		},
		
		//overridden
		setAdditionalContent : function(window, data)
		{
			var inputBox = new qx.ui.form.TextField(data.defaultInputText);
			window.add(inputBox);
		}
	}
});