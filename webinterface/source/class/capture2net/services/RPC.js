qx.Class.define("capture2net.services.RPC",
{
	extend : qx.core.Object,
	
	statics :
	{
		__rpc : new qx.io.remote.Rpc("rpc.php", "RPC"),
		
		callMethod : function(method, caller, event, params)
		{
			this.__rpc.callAsync(function(result, exception)
			{
				switch (result)
				{
					case "forbidden":
						alert("You do not have the permission to use this function!");
						break;
					case "login_required":
						alert("Login required");
						break;
					default:
						caller.fireDataEvent(event, result);
						break;
				}
			}, method, params);
		}
	}
});