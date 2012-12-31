qx.Class.define("capture2net.view.dialogbox.Main",
{
	extend : qx.core.Object,
	
	statics :
	{
		_currentItem : null,
		_queue : [],
		_window : null,
		
		acceptAction : function(data)
		{
			this._window.close();
			if (this._currentItem.actionFunction)
			{
				this._currentItem.actionFunction.call(this._currentItem.caller, true, data);
			}
			this.processQueue();
		},
		
		createWindow : function(applicationMain)
		{
			this._window = new qx.ui.window.Window();
			this._window.set(
			{
				allowClose : false,
				allowMaximize : false,
				allowMinimize : false,
				layout : new qx.ui.layout.VBox,
				modal : true,
				movable : false,
				resizable : false,
				showClose : false,
				showMaximize : false,
				showMinimize : false
			});
			applicationMain.getRoot().add(this._window);
		},
		
		declineAction : function()
		{
			this._window.close();
			if (this._currentItem.actionFunction)
			{
				this._currentItem.actionFunction.call(this._currentItem.caller, false);
			}
			this.processQueue();
		},
		
		processQueue : function()
		{
			if (!this._queue.length)
			{
				return false;
			}
			
			if (this._window.isVisible())
			{
				return false;
			}
			
			this._currentItem = this._queue.shift();
			
			// Setup window
			this._window.removeAll();
			this._window.setCaption(this._currentItem.title);
			
			// Create window content specified by type and additional data
			switch (this._currentItem.type)
			{
				case "alert":
					new capture2net.view.dialogbox.AlertBox(this);
					break;
				case "form":
					new capture2net.view.dialogbox.Form(this);
					break;
				case "prompt":
					new capture2net.view.dialogbox.PromptBox(this);
					break;
				default:
					// Invalid type
					return false;
			}
			
			// Show window
			this._window.open();
			this._window.center();
			
			return true;
		},
		
		show : function(data)
		{
			this._queue.push(data);
			this.processQueue();
		}
	}
});