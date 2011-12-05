Macro Macro_Events_CloseWindow
	Select EventWindow()
		Case #Window
			If GetGadgetState(#OtherSettings_HideOnClose)
				HideWindowEx(#True)
			Else
				Quit()
			EndIf
		Case #QRCodeDisplay_Window
			If IsImage(GetGadgetData(#QRCodeDisplay_Image))
				FreeImage(GetGadgetData(#QRCodeDisplay_Image))
			EndIf
			DisableWindow(#Window, #False)
			CloseWindow(#QRCodeDisplay_Window)
	EndSelect
EndMacro

Macro Macro_Events_Gadget
	Select EventGadget()
		Case #ToolBar_Save
			If EventType() = #PB_EventType_LeftClick
				ForEach ConfigGadgets()
					If IsGadget(ConfigGadgets()\lGadget)
						If ConfigGadgets()\bIsString
							WriteConfigString(ConfigGadgets()\sFieldName, GetGadgetText(ConfigGadgets()\lGadget))
						Else
							WriteConfigLong(ConfigGadgets()\sFieldName, GetGadgetState(ConfigGadgets()\lGadget))
						EndIf
					EndIf
				Next
				SaveConfig()
				Select GetGadgetState(#OtherSettings_StartWithWindows)
					Case -1; 3rd state
					Case 0
						DeleteRegValue(#HKEY_CURRENT_USER, "Software\Microsoft\Windows\CurrentVersion\Run", #InternName)
					Case 1
						SetRegValue(#HKEY_CURRENT_USER, "Software\Microsoft\Windows\CurrentVersion\Run", #InternName, Chr(34) + ProgramFilename() + Chr(34))
				EndSelect
			EndIf
		Case #ToolBar_CheckUpdate
			If EventType() = #PB_EventType_LeftClick
				CheckUpdate(#True)
			EndIf
		Case #FileName_ActiveWindow
			If EventType() = #PB_EventType_Change
				UpdateUrlPreview()
			EndIf
		Case #FileName_Selection
			If EventType() = #PB_EventType_Change
				UpdateUrlPreview()
			EndIf
		Case #FileName_Desktop
			If EventType() = #PB_EventType_Change
				UpdateUrlPreview()
			EndIf
		Case #FileNameHelp_ActiveWindow
			If CreatePopupMenu(#FileNameHelp_ActiveWindow)
				MenuItem(#FileNameHelp_ActiveWindow_Percent, "%")
				MenuBar()
				MenuItem(#FileNameHelp_ActiveWindow_4Year, "Long Year (" + Str(Year(Date())) + ")")
				MenuItem(#FileNameHelp_ActiveWindow_2Year, "Short Year (" + Right(Str(Year(Date())), 2) + ")")
				MenuItem(#FileNameHelp_ActiveWindow_Month, "Month (" + RSet(Str(Month(Date())), 2, "0") + ")")
				MenuItem(#FileNameHelp_ActiveWindow_Day, "Day (" + RSet(Str(Day(Date())), 2, "0") + ")")
				MenuItem(#FileNameHelp_ActiveWindow_Hour, "Hour (" + RSet(Str(Hour(Date())), 2, "0") + ")")
				MenuItem(#FileNameHelp_ActiveWindow_Minute, "Minute (" + RSet(Str(Minute(Date())), 2, "0") + ")")
				MenuItem(#FileNameHelp_ActiveWindow_Second, "Second (" + RSet(Str(Second(Date())), 2, "0") + ")")
				MenuBar()
				MenuItem(#FileNameHelp_ActiveWindow_WindowTitle, "Window title")
				MenuItem(#FileNameHelp_ActiveWindow_Hostname, "Host name")
				MenuItem(#FileNameHelp_ActiveWindow_Username, "User name")
				DisplayPopupMenu(#FileNameHelp_ActiveWindow, WindowID(#Window))
			EndIf
		Case #FileNameHelp_Selection
			If CreatePopupMenu(#FileNameHelp_Selection)
				MenuItem(#FileNameHelp_Selection_Percent, "%")
				MenuBar()
				MenuItem(#FileNameHelp_Selection_4Year, "Long Year (" + Str(Year(Date())) + ")")
				MenuItem(#FileNameHelp_Selection_2Year, "Short Year (" + Right(Str(Year(Date())), 2) + ")")
				MenuItem(#FileNameHelp_Selection_Month, "Month (" + RSet(Str(Month(Date())), 2, "0") + ")")
				MenuItem(#FileNameHelp_Selection_Day, "Day (" + RSet(Str(Day(Date())), 2, "0") + ")")
				MenuItem(#FileNameHelp_Selection_Hour, "Hour (" + RSet(Str(Hour(Date())), 2, "0") + ")")
				MenuItem(#FileNameHelp_Selection_Minute, "Minute (" + RSet(Str(Minute(Date())), 2, "0") + ")")
				MenuItem(#FileNameHelp_Selection_Second, "Second (" + RSet(Str(Second(Date())), 2, "0") + ")")
				MenuBar()
				MenuItem(#FileNameHelp_Selection_Hostname, "Host name")
				MenuItem(#FileNameHelp_Selection_Username, "User name")
				DisplayPopupMenu(#FileNameHelp_Selection, WindowID(#Window))
			EndIf
		Case #FileNameHelp_Desktop
			If CreatePopupMenu(#FileNameHelp_Desktop)
				MenuItem(#FileNameHelp_Desktop_Percent, "%")
				MenuBar()
				MenuItem(#FileNameHelp_Desktop_4Year, "Long Year (" + Str(Year(Date())) + ")")
				MenuItem(#FileNameHelp_Desktop_2Year, "Short Year (" + Right(Str(Year(Date())), 2) + ")")
				MenuItem(#FileNameHelp_Desktop_Month, "Month (" + RSet(Str(Month(Date())), 2, "0") + ")")
				MenuItem(#FileNameHelp_Desktop_Day, "Day (" + RSet(Str(Day(Date())), 2, "0") + ")")
				MenuItem(#FileNameHelp_Desktop_Hour, "Hour (" + RSet(Str(Hour(Date())), 2, "0") + ")")
				MenuItem(#FileNameHelp_Desktop_Minute, "Minute (" + RSet(Str(Minute(Date())), 2, "0") + ")")
				MenuItem(#FileNameHelp_Desktop_Second, "Second (" + RSet(Str(Second(Date())), 2, "0") + ")")
				MenuBar()
				MenuItem(#FileNameHelp_Desktop_Hostname, "Host name")
				MenuItem(#FileNameHelp_Desktop_Username, "User name")
				DisplayPopupMenu(#FileNameHelp_Desktop, WindowID(#Window))
			EndIf
		Case #FileFormat_ActiveWindow
			UpdateUrlPreview()
		Case #FileFormat_Selection
			UpdateUrlPreview()
		Case #FileFormat_Desktop
			UpdateUrlPreview()
		Case #History_SaveToFile
			lState = SwapValue(GetGadgetState(#History_SaveToFile))
			DisableGadget(#History_File, lState)
			DisableGadget(#History_SelectFile, lState)
		Case #History_SelectFile
			sFile$ = OpenFileRequester("Select history file", GetGadgetText(#History_File), "XML document|*.xml", 0)
			If sFile$
				SetGadgetText(#History_File, sFile$)
				LoadHistory()
			EndIf
		Case #History_CheckUrls
			LockMutex(eGlobals\lMutex)
			lQueueSize = ListSize(UploadQueue())
			UnlockMutex(eGlobals\lMutex)
			If lQueueSize
				MessageRequester(#Title, "The upload queue must be empty to check the urls!" + Chr(13) + "Wait to finish all uploads and try again.", #MB_ICONERROR)
			Else
				lThreadID = CreateThread(@CheckHistoryUrls(), #Null)
				If OpenWindow(#CheckHistoryUrls_Window, 100, 100, 500, 80, "", #PB_Window_BorderLess | #PB_Window_WindowCentered, WindowID(#Window))
					TextGadget(#CheckHistoryUrls_Text, 10, 10, 480, 20, "", #PB_Text_Center)
					ProgressBarGadget(#CheckHistoryUrls_Progress, 10, 40, 370, 30, 0, 0, #PB_ProgressBar_Smooth)
					ButtonGadget(#CheckHistoryUrls_Stop, 390, 40, 100, 30, "Stop")
					DisableWindow(#Window, #True)
					eGlobals\bStopCheckHistoryUrls = #False
					Repeat
						Select WaitWindowEvent(100)
							Case #PB_Event_Gadget
								Select EventGadget()
									Case #CheckHistoryUrls_Stop
										eGlobals\bStopCheckHistoryUrls = #True
										SetGadgetText(#CheckHistoryUrls_Stop, "Stopping...")
								EndSelect
						EndSelect
					Until Not IsThread(lThreadID)
					DisableWindow(#Window, #False)
					CloseWindow(#CheckHistoryUrls_Window)
				EndIf
			EndIf
		Case #WebDAV_AuthRequired
			lState = SwapValue(GetGadgetState(#WebDAV_AuthRequired))
			DisableGadget(#WebDAV_Text3, lState)
			DisableGadget(#WebDAV_Text4, lState)
			DisableGadget(#WebDAV_Username, lState)
			DisableGadget(#WebDAV_Password, lState)
		Case #OtherSettings_ShortUrl
			DisableGadget(#OtherSettings_ShortUrlProvider, SwapValue(GetGadgetState(#OtherSettings_ShortUrl)))
		Case #UploadHistory
			lItem = GetGadgetState(#UploadHistory)
			If lItem <> -1
				Select EventType()
					Case #PB_EventType_LeftDoubleClick
						sUrl$ = GetGadgetItemText(#UploadHistory, lItem, #UploadHistory_Column_Url)
						If sUrl$
							RunProgram(sUrl$)
						EndIf
					Case #PB_EventType_RightClick
						If CreatePopupMenu(#UploadHistoryPopupMenu)
							MenuItem(#UploadHistoryPopupMenu_OpenUrl, "Open Url in browser")
							If GetGadgetItemText(#UploadHistory, lItem, #UploadHistory_Column_ShortUrl)
								MenuItem(#UploadHistoryPopupMenu_OpenShortUrl, "Open short Url in browser")
								MenuBar()
							EndIf
							MenuItem(#UploadHistoryPopupMenu_CopyUrl, "Copy url to clip board")
							If GetGadgetItemText(#UploadHistory, lItem, #UploadHistory_Column_ShortUrl)
								MenuItem(#UploadHistoryPopupMenu_CopyShortUrl, "Copy short Url to clip board")
							EndIf
							MenuBar()
							MenuItem(#UploadHistoryPopupMenu_ShowQRCodeUrl, "Show QR code of url")
							If GetGadgetItemText(#UploadHistory, lItem, #UploadHistory_Column_ShortUrl)
								MenuItem(#UploadHistoryPopupMenu_ShowQRCodeShortUrl, "Show QR code of short url")
							EndIf
							DisplayPopupMenu(#UploadHistoryPopupMenu, WindowID(#Window))
						EndIf
				EndSelect
			EndIf
	EndSelect
EndMacro

Macro Macro_Events_Menu
	Select EventMenu()
		Case #FileNameHelp_ActiveWindow_Percent
			AddGadgetString(#FileName_ActiveWindow, "%%")
		Case #FileNameHelp_ActiveWindow_4Year
			AddGadgetString(#FileName_ActiveWindow, "%Y")
		Case #FileNameHelp_ActiveWindow_2Year
			AddGadgetString(#FileName_ActiveWindow, "%y")
		Case #FileNameHelp_ActiveWindow_Month
			AddGadgetString(#FileName_ActiveWindow, "%M")
		Case #FileNameHelp_ActiveWindow_Day
			AddGadgetString(#FileName_ActiveWindow, "%d")
		Case #FileNameHelp_ActiveWindow_Hour
			AddGadgetString(#FileName_ActiveWindow, "%h")
		Case #FileNameHelp_ActiveWindow_Minute
			AddGadgetString(#FileName_ActiveWindow, "%m")
		Case #FileNameHelp_ActiveWindow_Second
			AddGadgetString(#FileName_ActiveWindow, "%s")
		Case #FileNameHelp_ActiveWindow_WindowTitle
			AddGadgetString(#FileName_ActiveWindow, "%W")
		Case #FileNameHelp_ActiveWindow_Hostname
			AddGadgetString(#FileName_ActiveWindow, "%H")
		Case #FileNameHelp_ActiveWindow_Username
			AddGadgetString(#FileName_ActiveWindow, "%U")
		Case #FileNameHelp_Selection_Percent
			AddGadgetString(#FileName_Selection, "%%")
		Case #FileNameHelp_Selection_4Year
			AddGadgetString(#FileName_Selection, "%Y")
		Case #FileNameHelp_Selection_2Year
			AddGadgetString(#FileName_Selection, "%y")
		Case #FileNameHelp_Selection_Month
			AddGadgetString(#FileName_Selection, "%M")
		Case #FileNameHelp_Selection_Day
			AddGadgetString(#FileName_Selection, "%d")
		Case #FileNameHelp_Selection_Hour
			AddGadgetString(#FileName_Selection, "%h")
		Case #FileNameHelp_Selection_Minute
			AddGadgetString(#FileName_Selection, "%m")
		Case #FileNameHelp_Selection_Second
			AddGadgetString(#FileName_Selection, "%s")
		Case #FileNameHelp_Selection_Hostname
			AddGadgetString(#FileName_Selection, "%H")
		Case #FileNameHelp_Selection_Username
			AddGadgetString(#FileName_Selection, "%U")
		Case #FileNameHelp_Desktop_Percent
			AddGadgetString(#FileName_Desktop, "%%")
		Case #FileNameHelp_Desktop_4Year
			AddGadgetString(#FileName_Desktop, "%Y")
		Case #FileNameHelp_Desktop_2Year
			AddGadgetString(#FileName_Desktop, "%y")
		Case #FileNameHelp_Desktop_Month
			AddGadgetString(#FileName_Desktop, "%M")
		Case #FileNameHelp_Desktop_Day
			AddGadgetString(#FileName_Desktop, "%d")
		Case #FileNameHelp_Desktop_Hour
			AddGadgetString(#FileName_Desktop, "%h")
		Case #FileNameHelp_Desktop_Minute
			AddGadgetString(#FileName_Desktop, "%m")
		Case #FileNameHelp_Desktop_Second
			AddGadgetString(#FileName_Desktop, "%s")
		Case #FileNameHelp_Desktop_Hostname
			AddGadgetString(#FileName_Desktop, "%H")
		Case #FileNameHelp_Desktop_Username
			AddGadgetString(#FileName_Desktop, "%U")
		Case #UploadHistoryPopupMenu_OpenUrl
			RunProgram(GetGadgetItemText(#UploadHistory, GetGadgetState(#UploadHistory), #UploadHistory_Column_Url))
		Case #UploadHistoryPopupMenu_OpenShortUrl
			RunProgram(GetGadgetItemText(#UploadHistory, GetGadgetState(#UploadHistory), #UploadHistory_Column_ShortUrl))
		Case #UploadHistoryPopupMenu_CopyUrl
			SetClipboardText(GetGadgetItemText(#UploadHistory, GetGadgetState(#UploadHistory), #UploadHistory_Column_Url))
		Case #UploadHistoryPopupMenu_CopyShortUrl
			SetClipboardText(GetGadgetItemText(#UploadHistory, GetGadgetState(#UploadHistory), #UploadHistory_Column_ShortUrl))
		Case #UploadHistoryPopupMenu_ShowQRCodeUrl
			ShowQRCode(GetGadgetItemText(#UploadHistory, GetGadgetState(#UploadHistory), #UploadHistory_Column_Url))
		Case #UploadHistoryPopupMenu_ShowQRCodeShortUrl
			ShowQRCode(GetGadgetItemText(#UploadHistory, GetGadgetState(#UploadHistory), #UploadHistory_Column_ShortUrl))
		Case #TrayMenu_ShowWindow
			HideWindowEx(#False)
		Case #TrayMenu_Quit
			Quit()
	EndSelect
EndMacro

Macro Macro_Events_SizeWindow
	If EventWindow() = #Window
		ResizeGadget(#Panel, #PB_Ignore, #PB_Ignore, WindowWidth(#Window), WindowHeight(#Window) - 32)
		ResizeGadget(#History_File, #PB_Ignore, #PB_Ignore, GetGadgetAttribute(#Panel, #PB_Panel_ItemWidth) - 240, #PB_Ignore)
		ResizeGadget(#History_SelectFile, GadgetWidth(#History_File) + 90, #PB_Ignore, #PB_Ignore, #PB_Ignore)
		ResizeGadget(#History_CheckUrls, GadgetWidth(#History_File) + 150, #PB_Ignore, #PB_Ignore, #PB_Ignore)
		ResizeGadget(#UploadHistory, #PB_Ignore, #PB_Ignore, GetGadgetAttribute(#Panel, #PB_Panel_ItemWidth), GetGadgetAttribute(#Panel, #PB_Panel_ItemHeight) - 40)
		ResizeGadget(#AboutText, #PB_Ignore, #PB_Ignore, GetGadgetAttribute(#Panel, #PB_Panel_ItemWidth) - 20, GetGadgetAttribute(#Panel, #PB_Panel_ItemHeight) - 20)
	EndIf
EndMacro
; IDE Options = PureBasic 4.60 (Windows - x86)
; Folding = -
; EnableXP
; EnableCompileCount = 0
; EnableBuildCount = 0
; EnableExeConstant