qx.Class.define("capture2net.services.RPC",
{
	extend : qx.core.Object,
	
	statics :
	{
		__rpc : new qx.io.remote.Rpc("rpc.php", "RPC"),
		_loginDialogData :
		{
			type : "form",
			title : "Login",
			icon : "password",
			text : "You are currently not logged in.<br />Enter your username and password.",
			acceptButton : "Login",
			formFields :
			{
				userName :
				{
					type : "text",
					text : "Username",
					required : true
				},
				password :
				{
					type : "password",
					text : "Password",
					required : true
				}
			},
			actionFunction : function(accepted, data)
			{
				if (accepted)
				{
					capture2net.services.RPC.callMethod("login", this, this.loginDone, ["ok", "login_failed"], data);
				}
			}
		},
		
		callMethod : function(method, caller, callbackFunction, validReturnValues, params)
		{
			var thisClass = this;
			this.__rpc.callAsync(function(result, exception)
			{
				switch (result)
				{
					case "forbidden":
						alert("You do not have the permission to use this function!");
						break;
					case "login_required":
						thisClass._loginDialogData.caller = thisClass;
						capture2net.view.dialogbox.Main.show(thisClass._loginDialogData);
						break;
					default:
						var validReturn = false;
						if (typeof(validReturnValues) == "object")
						{
							for (var index in validReturnValues)
							{
								var returnValue = validReturnValues[index];
								if (typeof(returnValue) == "string")
								{
									if (result == returnValue)
									{
										validReturn = true;
										break;
									}
								}
								else
								{
									if (typeof(returnValue) == typeof(result))
									{
										validReturn = true;
										break;
									}
								}
							}
						}
						else
						{
							if (typeof(validReturnValues) == "string")
							{
								if (validReturnValues == result)
								{
									validReturn = true;
								}
							}
							else
							{
								validReturn = true;
							}
						}
						if (validReturn)
						{
							callbackFunction.call(caller, result);
						}
						else
						{
							var dialogData =
							{
								type : "alert",
								title : "Unexpected data",
								text : "The server sent unexpected data!<br /><br />Returned data:<br /><br />" + result,
								icon : "error"
							};
							capture2net.view.dialogbox.Main.show(dialogData);
						}
						break;
				}
			}, method, params);
		},
		
		/**
		 * This method gets called as soon as the user logged in (Returns 'ok" if the login was successful or "login_failed" if the login failed)
		 */
		loginDone :function(result)
		{
			switch (result)
			{
				case "ok":
					// Login OK
					break;
				case "login_failed":
					var data =
					{
						type : "alert",
						title : "Login failed",
						text : "The entered username or password is wrong!<br /><br />Please try again.",
						icon : "error"
					};
					capture2net.view.dialogbox.Main.show(data);
					this._loginDialogData.caller = this;
					capture2net.view.dialogbox.Main.show(this._loginDialogData);
					break;
			}
		}
	}
});