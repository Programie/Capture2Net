qx.Class.define("capture2net.view.panel.ScreenshotManager",
{
	extend : qx.ui.tabview.Page,
	
	construct : function()
	{
		this.base(arguments);
		
		this.setLabel("Screenshot Manager");
		this.setLayout(new qx.ui.layout.VBox);
		
		var splitPane = new qx.ui.splitpane.Pane();
		splitPane.add(this.createTable());
		splitPane.add(this.createPreviewArea());
		this.add(splitPane, {flex: 1});
		
		this.loadData();
	},
	
	members :
	{
		_previewArea :
		{
			container : null,
			image : null,
			imageContainer : null,
			imageInfoContainer : null,
			info : null,
			scrollArea : null,
			url : null
		},
		_table : null,
		_url : null,
		
		createPreviewArea : function()
		{
			// Scroll Area
			this._previewArea.scrollArea = new qx.ui.container.Scroll();
			this._previewArea.scrollArea.setScrollbarY("off");
			this._previewArea.scrollArea.addListener("resize", this.updatePreviewImageSize, this);
			
			// Container
			this._previewArea.container = new qx.ui.container.Composite();
			this._previewArea.container.setLayout(new qx.ui.layout.VBox(10));
			this._previewArea.container.setPadding(10);
			this._previewArea.scrollArea.add(this._previewArea.container);
			
			// Image container
			this._previewArea.imageContainer = new qx.ui.container.Composite();
			this._previewArea.imageContainer.setLayout(new qx.ui.layout.Canvas);
			this._previewArea.imageContainer.setAllowGrowY(false);
			this._previewArea.imageContainer.setAllowShrinkY(false);
			this._previewArea.imageContainer.setAlignY("middle");
			this._previewArea.container.add(this._previewArea.imageContainer);
			
			// Image
			this._previewArea.image = new qx.ui.basic.Image();
			this._previewArea.image.setAllowGrowX(true);
			this._previewArea.image.setAllowShrinkX(true);
			this._previewArea.image.setScale(true);
			this._previewArea.imageContainer.add(this._previewArea.image);
			
			// Image info
			this._previewArea.imageInfoContainer = new qx.ui.container.Composite();
			this._previewArea.imageInfoContainer.setLayout(new qx.ui.layout.Grid(10, 10));
			this._previewArea.imageInfoContainer.setAllowGrowX(false);
			this._previewArea.imageInfoContainer.setAllowShrinkX(false);
			this._previewArea.imageInfoContainer.setAlignX("center");
			this._previewArea.container.add(this._previewArea.imageInfoContainer);
			
			return this._previewArea.scrollArea;
		},
		
		createTable : function()
		{
			var columns =
			{
				names : ["id", "date", "type", "fileName", "activeWindow", "userName", "hostName"],
				titles : ["ID", "Date", "Type", "Filename", "Active Window", "Username", "Hostname"]
			};
			
			var tableModel = new qx.ui.table.model.Simple();
			tableModel.setColumns(columns.titles, columns.names);
			this._table = new qx.ui.table.Table(tableModel);
			
			this._table.setShowCellFocusIndicator(false);
			
			this._table.addListener("cellDblclick", function(event)
			{
				var rowData = this._table.getTableModel().getRowDataAsMap(event.getRow());
				var newUrl = this._url + "/" + rowData.fileName;
				qx.io.ImageLoader.abort(this._previewArea.url);
				this._previewArea.imageInfoContainer.removeAll();
				this._previewArea.imageInfoContainer.add(new qx.ui.basic.Image("resource/capture2net/loading.gif"), {column: 0, row: 0});
				qx.io.ImageLoader.load(newUrl, function(url, data)
				{
					if (qx.io.ImageLoader.isLoaded(newUrl))
					{
						this._previewArea.url = url;
						this._previewArea.info = data;
						this._previewArea.image.setSource(url);
						
						// Show image info
						this._previewArea.imageInfoContainer.removeAll();
						var row = 0;
						this._previewArea.imageInfoContainer.add(new qx.ui.basic.Label("Filename:"), {column : 0, row : row});
						var fileNameLabel = new qx.ui.basic.Label("<a href='" + url + "' target='_blank'>" + rowData.fileName + "</a>");
						fileNameLabel.setRich(true);
						this._previewArea.imageInfoContainer.add(fileNameLabel, {column : 1, row : row});
						row++;
						this._previewArea.imageInfoContainer.add(new qx.ui.basic.Label("Original size:"), {column : 0, row : row});
						this._previewArea.imageInfoContainer.add(new qx.ui.basic.Label(data.width + " x " + data.height), {column : 1, row : row});
						row++;
						
						this.updatePreviewImageSize();
					}
					else
					{
						this._previewArea.imageInfoContainer.removeAll();
						this._previewArea.imageInfoContainer.add(new qx.ui.basic.Label("Loading failed!"), {column: 0, row: 0});
					}
				}, this);
			}, this);
			
			return this._table;
		},
		
		dataLoaded : function(result)
		{
			var tableModel = this._table.getTableModel();
			this._url = result.url;
			tableModel.setDataAsMapArray(result.screenshots, false, false);
		},
		
		loadData : function()
		{
			capture2net.services.RPC.callMethod("getScreenshots", this, this.dataLoaded, [[]]);
		},
		
		updatePreviewImageSize : function(event)
		{
			var containerBounds = this._previewArea.scrollArea.getBounds();
			var imageInfoContainerBounds = this._previewArea.imageInfoContainer.getBounds();
			if (containerBounds && imageInfoContainerBounds && this._previewArea.info)
			{
				var containerPadding =
				{
					left : this._previewArea.container.getPaddingLeft(),
					top : this._previewArea.container.getPaddingTop(),
					right : this._previewArea.container.getPaddingRight(),
					bottom : this._previewArea.container.getPaddingBottom()
				};
				var widthRatio = (containerBounds.width - containerPadding.left - containerPadding.right) / this._previewArea.info.width;
				var heightRatio = (containerBounds.height - imageInfoContainerBounds.height - containerPadding.top - containerPadding.bottom) / this._previewArea.info.height;
				var ratio;
				if (widthRatio < heightRatio)
				{
					ratio = widthRatio;
				}
				else
				{
					ratio = heightRatio;
				}
				this._previewArea.image.setWidth(parseInt(this._previewArea.info.width * ratio));
				this._previewArea.image.setHeight(parseInt(this._previewArea.info.height * ratio));
			}
		}
	}
});