qx.Class.define("capture2net.view.dialogbox.Main",
{
	extend : qx.core.Object,
	
	statics :
	{
		_currentItem : null,
		_queue : [],
		_subInstance : null,
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
			this._window.addListener("keypress", this.processKeypress, this);
			applicationMain.getRoot().addListener("resize", this.tryCenter, this);
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
		
		processKeypress : function(event)
		{
			var key = event.getKeyIdentifier();
			
			if (!this._subInstance)
			{
				return;
			}
			
			switch (key)
			{
				case "Enter":
					this._subInstance.triggerAccept();
					break;
				case "Escape":
					this._subInstance.triggerDecline();
					break;
			}
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
			this._subInstance = null;
			switch (this._currentItem.type)
			{
				case "alert":
					this._subInstance = new capture2net.view.dialogbox.AlertBox(this);
					break;
				case "form":
					this._subInstance = new capture2net.view.dialogbox.Form(this);
					break;
				case "prompt":
					this._subInstance = new capture2net.view.dialogbox.PromptBox(this);
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
		},
		
		tryCenter : function()
		{
			if (this._window.isVisible())
			{
				this._window.center();
			}
		}
	}
});