qx.Class.define("capture2net.services.RPC",
{
	extend : qx.core.Object,
	
	statics :
	{
		__rpc : new qx.io.remote.Rpc("rpc.php", "RPC"),
		
		loadConfig : function()
		{
			this.__rpc.callAsync(function(result, exception)
			{
				if (!result || result == "login_required")
				{
					alert("Login required");
				}
				else
				{
					alert(result);
				}
			}, "loadConfig");
		}
	}
});