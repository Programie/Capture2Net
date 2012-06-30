;- Includes
IncludeFile "Enumerations.pbi"
IncludeFile "Structures.pbi"
IncludeFile "Globals.pbi"
IncludeFile "Procedures.pbi"
IncludeFile "Macros.pbi"
IncludeFile "DataSection.pbi"

;- Initializations
OnErrorCall(@OnError())

InitNetwork()
InitSound()

UseJPEG2000ImageEncoder()
UseJPEGImageEncoder()
UsePNGImageEncoder()
UsePNGImageDecoder()

eGlobals\sConfigFile = GetPathPart(ProgramFilename()) + "Settings.dat"
eGlobals\lMutex = CreateMutex()
eGlobals\hAppicon = ExtractIcon_(0, ProgramFilename(), 0)
eGlobals\lTaskBarWindowMessage = RegisterWindowMessage_("TaskbarCreated")

hWnd = FindWindow_(0, #Title)
If hWnd
	ShowWindow_(hWnd, #SW_SHOW)
	SetForegroundWindow_(hWnd)
	End
EndIf

If GetParameter("getbuild")
	MessageRequester(#Title, "Current build: " + Str(#PB_Editor_CompileCount), #MB_ICONINFORMATION)
	End
EndIf

lIndex = GetParameter("update1")
If lIndex
	sUpdateFile$ = GetPathPart(ProgramFilename()) + ReplaceString(GetFilePart(ProgramFilename()), ".exe", "_Update.exe", #PB_String_NoCase)
	If ReceiveHTTPFile(ProgramParameter(lIndex), sUpdateFile$)
		RunProgram(sUpdateFile$, "/update2", GetPathPart(sUpdateFile$))
		End
	Else
		MessageRequester(#Title, "Download failed!" + Chr(13) + Chr(13) + "If this error continues, please look on selfcoders.com for the current version.", #MB_ICONERROR)
	EndIf
EndIf

If GetParameter("update2")
	Delay(1000)
	sOriginalFile$ = GetPathPart(ProgramFilename()) + ReplaceString(GetFilePart(ProgramFilename()), "_Update.exe", ".exe", #PB_String_NoCase)
	CopyFile(ProgramFilename(), sOriginalFile$)
	RunProgram(sOriginalFile$, "/update3", GetPathPart(sOriginalFile$))
	End
EndIf

If GetParameter("update3")
	Delay(1000)
	DeleteFile(GetPathPart(ProgramFilename()) + ReplaceString(GetFilePart(ProgramFilename()), ".exe", "_Update.exe", #PB_String_NoCase))
	MessageRequester(#Title, "Update OK", #MB_ICONINFORMATION)
	RunProgram(ProgramFilename(), "", GetPathPart(ProgramFilename()))
	End
EndIf

OpenConfig()

If ReadConfigLong("OtherSettings_AutoCheckUpdates", #True)
	CheckUpdate(#False)
EndIf

CatchImage(#Icon_CheckUpdate, ?Icon_CheckUpdate)
CatchImage(#Icon_Complete, ?Icon_Complete)
CatchImage(#Icon_Error, ?Icon_Error)
CatchImage(#Icon_Queued, ?Icon_Queued)
CatchImage(#Icon_Save, ?Icon_Save)
CatchSound(#Sound_Camera, ?Sound_Camera)

;- Main window
If OpenWindow(#Window, 100, 100, 960, 530, #Title, #PB_Window_MinimizeGadget | #PB_Window_MaximizeGadget | #PB_Window_SizeGadget | #PB_Window_ScreenCentered | #PB_Window_Invisible)
	lToolBarSize = 32
	lToolBarIconSpacing = 4
	lToolBarIconSize = lToolBarSize - lToolBarIconSpacing * 2
	ResizeImage(#Icon_CheckUpdate, lToolBarIconSize, lToolBarIconSize)
	ResizeImage(#Icon_Save, lToolBarIconSize, lToolBarIconSize)
	CanvasGadget(#ToolBar_Save, lToolBarIconSpacing, lToolBarIconSpacing, lToolBarIconSize, lToolBarIconSize)
	SetGadgetAttribute(#ToolBar_Save, #PB_Canvas_Image, ImageID(#Icon_Save))
	SetGadgetAttribute(#ToolBar_Save, #PB_Canvas_Cursor, #PB_Cursor_Hand)
	CanvasGadget(#ToolBar_CheckUpdate, lToolBarIconSpacing * 2 + lToolBarIconSize, lToolBarIconSpacing, lToolBarIconSize, lToolBarIconSize)
	SetGadgetAttribute(#ToolBar_CheckUpdate, #PB_Canvas_Image, ImageID(#Icon_CheckUpdate))
	SetGadgetAttribute(#ToolBar_CheckUpdate, #PB_Canvas_Cursor, #PB_Cursor_Hand)
	GadgetToolTip(#ToolBar_Save, "Save")
	GadgetToolTip(#ToolBar_CheckUpdate, "Check for updates")
	PanelGadget(#Panel, 0, lToolBarSize, WindowWidth(#Window), WindowHeight(#Window) - lToolBarSize)
	AddGadgetItem(#Panel, -1, "Upload Queue and History");- Panel - Upload Queue and History
	CheckBoxGadgetEx(#History_SaveToFile, 10, 10, 70, 20, "Log to file:", "History_SaveToFile", #True)
	StringGadgetEx(#History_File, 80, 10, 0, 20, "History_File", GetPathPart(ProgramFilename()) + "History.xml", #PB_String_ReadOnly)
	ButtonGadget(#History_SelectFile, 0, 10, 50, 20, "Open")
	ButtonGadget(#History_CheckUrls, 0, 10, 80, 20, "Check Urls")
	ListIconGadget(#UploadHistory, 0, 40, 0, 0, "File name", 250, #PB_ListIcon_FullRowSelect)
	AddGadgetColumn(#UploadHistory, #UploadHistory_Column_Size, "Size", 120)
	AddGadgetColumn(#UploadHistory, #UploadHistory_Column_Status, "Status", 120)
	AddGadgetColumn(#UploadHistory, #UploadHistory_Column_Type, "Type", 90)
	AddGadgetColumn(#UploadHistory, #UploadHistory_Column_Dimensions, "Dimensions", 100)
	AddGadgetColumn(#UploadHistory, #UploadHistory_Column_Url, "Url", 300)
	AddGadgetColumn(#UploadHistory, #UploadHistory_Column_ShortUrl, "Short Url", 150)
	AddGadgetColumn(#UploadHistory, #UploadHistory_Column_Time, "Time", 150)
	AddGadgetItem(#Panel, -1, "Files and Shortcuts");- Panel - Files and Shortcuts
	lColumn1Width = 70
	lColumnWidth = 270
	lColumn1X = 10
	lColumn2X = lColumn1X + lColumn1Width + 10
	lColumn3X = lColumn2X + lColumnWidth + 20
	lColumn4X = lColumn3X + lColumnWidth + 20
	TextGadget(#FilesShortcuts_Text1, lColumn2X, 10, lColumnWidth, 20, "Active Window", #PB_Text_Center)
	TextGadget(#FilesShortcuts_Text2, lColumn3X, 10, lColumnWidth, 20, "Selection", #PB_Text_Center)
	TextGadget(#FilesShortcuts_Text3, lColumn4X, 10, lColumnWidth, 20, "Desktop", #PB_Text_Center)
	TextGadget(#FilesShortcuts_Text4, lColumn1X, 40, lColumn1Width, 20, "File name:")
	TextGadget(#FilesShortcuts_Text5, lColumn1X, 70, lColumn1Width, 20, "File format:")
	TextGadget(#FilesShortcuts_Text6, lColumn1X, 100, lColumn1Width, 20, "URL Preview:")
	TextGadget(#FilesShortcuts_Text7, lColumn1X, 130, lColumn1Width, 20, "Quality:")
	TextGadget(#FilesShortcuts_Text8, lColumn1X, 160, lColumn1Width, 20, "Shortcut:")
	StringGadgetEx(#FileName_ActiveWindow, lColumn2X, 40, lColumnWidth - 20, 20, "Filename_ActiveWindow", "%W_%Y-%M-%d_%h-%m-%s")
	ButtonGadget(#FileNameHelp_ActiveWindow, lColumn2X + lColumnWidth - 20, 40, 20, 20, "?")
	StringGadgetEx(#FileName_Selection, lColumn3X, 40, lColumnWidth - 20, 20, "Filename_Selection", "Selection_%Y-%M-%d_%h-%m-%s")
	ButtonGadget(#FileNameHelp_Selection, lColumn3X + lColumnWidth - 20, 40, 20, 20, "?")
	StringGadgetEx(#FileName_Desktop, lColumn4X, 40, lColumnWidth - 20, 20, "Filename_Desktop", "Screen_%Y-%M-%d_%h-%m-%s")
	ButtonGadget(#FileNameHelp_Desktop, lColumn4X + lColumnWidth - 20, 40, 20, 20, "?")
	ComboBoxGadgetEx(#FileFormat_ActiveWindow, lColumn2X, 70, lColumnWidth, 20, "FileFormat_ActiveWindow", #ImageFormat_JPEG, "Windows Bitmap", "JPEG", "JPEG 2000", "Portable Network Graphics")
	ComboBoxGadgetEx(#FileFormat_Selection, lColumn3X, 70, lColumnWidth, 20, "FileFormat_Selection", #ImageFormat_JPEG, "Windows Bitmap", "JPEG", "JPEG 2000", "Portable Network Graphics")
	ComboBoxGadgetEx(#FileFormat_Desktop, lColumn4X, 70, lColumnWidth, 20, "FileFormat_Desktop", #ImageFormat_JPEG, "Windows Bitmap", "JPEG", "JPEG 2000", "Portable Network Graphics")
	StringGadget(#UrlPreview_ActiveWindow, lColumn2X, 100, lColumnWidth, 20, "", #PB_String_ReadOnly)
	StringGadget(#UrlPreview_Selection, lColumn3X, 100, lColumnWidth, 20, "", #PB_String_ReadOnly)
	StringGadget(#UrlPreview_Desktop, lColumn4X, 100, lColumnWidth, 20, "", #PB_String_ReadOnly)
	TextGadget(#FilesShortcuts_Text9, lColumn2X, 130, 20, 20, "Min")
	TrackBarGadgetEx(#ImageQuality_ActiveWindow, lColumn2X + 20, 130, lColumnWidth - 40, 20, 0, 10, "ImageQuality_ActiveWindow", 7, #PB_TrackBar_Ticks)
	TextGadget(#FilesShortcuts_Text10, lColumn2X + lColumnWidth - 20, 130, 20, 20, "Max")
	TextGadget(#FilesShortcuts_Text11, lColumn3X, 130, 20, 20, "Min")
	TrackBarGadgetEx(#ImageQuality_Selection,  lColumn3X + 20, 130, lColumnWidth - 40, 20, 0, 10, "ImageQuality_Selection", 7, #PB_TrackBar_Ticks)
	TextGadget(#FilesShortcuts_Text12, lColumn3X + lColumnWidth - 20, 130, 20, 20, "Max")
	TextGadget(#FilesShortcuts_Text13, lColumn4X, 130, 20, 20, "Min")
	TrackBarGadgetEx(#ImageQuality_Desktop,  lColumn4X + 20, 130, lColumnWidth - 40, 20, 0, 10, "ImageQuality_Desktop", 7, #PB_TrackBar_Ticks)
	TextGadget(#FilesShortcuts_Text14, lColumn4X + lColumnWidth - 20, 130, 20, 20, "Max")
	ShortcutGadgetEx(#Shortcut_ActiveWindow, lColumn2X, 160, lColumnWidth, 20, "Shortcut_ActiveWindow", #PB_Shortcut_Alt | #PB_Shortcut_Print)
	ShortcutGadgetEx(#Shortcut_Selection, lColumn3X, 160, lColumnWidth, 20, "Shortcut_Selection", #PB_Shortcut_Control | #PB_Shortcut_Print)
	ShortcutGadgetEx(#Shortcut_Desktop, lColumn4X, 160, lColumnWidth, 20, "Shortcut_Desktop", #PB_Shortcut_Print)
	AddGadgetItem(#Panel, -1, "WebDAV Access");- Panel - WebDAV Access
	TextGadget(#WebDAV_Text1, 10, 10, 70, 20, "Host name:")
	TextGadget(#WebDAV_Text2, 10, 40, 70, 20, "Authentication:")
	TextGadget(#WebDAV_Text3, 10, 70, 70, 20, "User name:")
	TextGadget(#WebDAV_Text4, 10, 100, 70, 20, "Password:")
	TextGadget(#WebDAV_Text5, 10, 130, 70, 20, "Path:")
	TextGadget(#WebDAV_Text6, 10, 160, 70, 20, "Access path:")
	StringGadgetEx(#WebDAV_Server, 90, 10, 300, 20, "WebDAV_Hostname", "dav.example.com")
	TextGadget(#WebDAV_Text7, 390, 10, 10, 20, ":", #PB_Text_Center)
	SpinGadgetEx(#WebDAV_Port, 400, 10, 50, 20, 0, 65000, "WebDAV_Port", 80, "", #PB_Spin_Numeric)
	CheckBoxGadgetEx(#WebDAV_AuthRequired, 90, 40, 60, 20, "Required", "WebDAV_AuthRequired", #True)
	StringGadgetEx(#WebDAV_Username, 90, 70, 380, 20, "WebDAV_Username", "MyAccount")
	StringGadgetEx(#WebDAV_Password, 90, 100, 380, 20, "WebDAV_Password", "MyPassword", #PB_String_Password)
	StringGadgetEx(#WebDAV_Path, 90, 130, 380, 20, "WebDAV_Path", "/screenshots/")
	StringGadgetEx(#WebDAV_AccessPath, 90, 160, 380, 20, "WebDAV_AccessPath", "http://www.example.com/screenshots/")
	AddGadgetItem(#Panel, -1, "Other Settings");- Panel - Other Settings
	lColumnWidth = 210
	lColumn1X = 10
	lColumn2X = lColumn1X + lColumnWidth + 20
	CheckBoxGadgetEx(#OtherSettings_StartHidden, lColumn1X, 10, lColumnWidth, 20, "Start hidden", "OtherSettings_StartHidden", #False)
	CheckBoxGadgetEx(#OtherSettings_HideOnClose, lColumn1X, 40, lColumnWidth, 20, "Hide on close", "OtherSettings_HideOnClose", #True)
	CheckBoxGadget(#OtherSettings_StartWithWindows, lColumn1X, 70, lColumnWidth, 20, "Start with Windows")
	CheckBoxGadgetEx(#OtherSettings_ShortUrl, lColumn1X, 100, 60, 20, "Short Url", "OtherSettings_ShortUrl", #False)
	ComboBoxGadgetEx(#OtherSettings_ShortUrlProvider, lColumn1X + 70, 100, lColumnWidth - 70, 20, "OtherSettings_ShortUrlProvider", #ShortUrlProvider_Sh0rtAt, "Sh0rt.at", "TinyURL")
	CheckBoxGadgetEx(#OtherSettings_CopyUrl, lColumn1X, 130, lColumnWidth, 20, "Copy Url to clip board", "OtherSettings_CopyUrl", #True)
	CheckBoxGadgetEx(#OtherSettings_AutoCheckUpdates, lColumn1X, 160, lColumnWidth, 20, "Automatically check for updates on start", "OtherSettings_AutoCheckUpdates", #True)
	AddGadgetItem(#Panel, -1, "Help and More");- Panel - Help and More
	EditorGadget(#AboutText, 10, 10, 0, 0, #PB_Editor_ReadOnly)
	SendMessage_(GadgetID(#AboutText), #EM_SETEVENTMASK, 0, #ENM_LINK | SendMessage_(GadgetID(#AboutText), #EM_GETEVENTMASK, 0, 0))
	SendMessage_(GadgetID(#AboutText), #EM_AUTOURLDETECT, #True, 0)
	UpdateInfoText()
	CloseGadgetList()
	lState = SwapValue(GetGadgetState(#WebDAV_AuthRequired))
	DisableGadget(#WebDAV_Text3, lState)
	DisableGadget(#WebDAV_Text4, lState)
	DisableGadget(#WebDAV_Username, lState)
	DisableGadget(#WebDAV_Password, lState)
	lState = SwapValue(GetGadgetState(#History_SaveToFile))
	DisableGadget(#History_File, lState)
	DisableGadget(#History_SelectFile, lState)
	LoadHistory()
	sAutorun$ = RemoveString(GetRegValue(#HKEY_CURRENT_USER, "Software\Microsoft\Windows\CurrentVersion\Run", #InternName), Chr(34))
	If sAutorun$ = ProgramFilename()
		SetGadgetState(#OtherSettings_StartWithWindows, #True)
	ElseIf Not sAutorun$
		SetGadgetState(#OtherSettings_StartWithWindows, #False)
	Else
		Set3StateCheckbox(#OtherSettings_StartWithWindows)
	EndIf
	bHideState = GetGadgetState(#OtherSettings_StartHidden)
	If GetParameter("hide")
		bHideState = #True
	EndIf
	If GetParameter("nohide")
		bHideState = #False
	EndIf
	HideWindowEx(bHideState)
	AddSysTrayIconEx(WindowID(#Window), #Tray, eGlobals\hAppIcon, #Title)
	If CreatePopupMenu(#TrayMenu)
		MenuItem(#TrayMenu_ShowWindow, "Show")
		MenuBar()
		MenuItem(#TrayMenu_Quit, "Quit")
		MENUITEMINFO.MENUITEMINFO
		MENUITEMINFO\cbSize = SizeOf(MENUITEMINFO)
		MENUITEMINFO\fMask = #MIIM_STATE
		MENUITEMINFO\fState = #MFS_DEFAULT
		SetMenuItemInfo_(MenuID(#TrayMenu), 0, #True, MENUITEMINFO)
	EndIf
	WindowBounds(#Window, 960, 530, #PB_Ignore, #PB_Ignore)
	SetWindowCallback(@WindowCallback(), #Window)
	CreateThread(@UploadQueueThread(), #Null)
	Repeat;- Main loop start
		If GetAsyncKeyState_(#VK_SNAPSHOT)
			If GetAsyncKeyState_(#VK_MENU)
				If Not bKeyState
					bKeyState = #True
					AddUpload(CaptureScreen(GetForegroundWindow_()), #FileName_ActiveWindow, #FileFormat_ActiveWindow)
				EndIf
			ElseIf GetAsyncKeyState_(#VK_CONTROL)
				If Not bKeyState
					bKeyState = #True
					lImage = CaptureScreen()
					If IsImage(lImage)
						If OpenWindow(#RectangleSelection_Window, 0, 0, ImageWidth(lImage), ImageHeight(lImage), #Title, #PB_Window_BorderLess)
							StickyWindow(#RectangleSelection_Window, #True)
							AddKeyboardShortcut(#RectangleSelection_Window, #PB_Shortcut_Escape, #RectangleSelection_Close)
							AddKeyboardShortcut(#RectangleSelection_Window, #PB_Shortcut_Return, #RectangleSelection_OK)
							If StartDrawing(WindowOutput(#RectangleSelection_Window))
								DrawImage(ImageID(lImage), 0, 0)
								StopDrawing()
							EndIf
							bRectDone.b = #False
							Repeat
								Select WaitWindowEvent()
									Case #WM_LBUTTONDOWN
										lStartX = WindowMouseX(#RectangleSelection_Window)
										lStartY = WindowMouseY(#RectangleSelection_Window)
									Case #WM_MOUSEMOVE
										If EventwParam() = #MK_LBUTTON
											lEndX = WindowMouseX(#RectangleSelection_Window)
											lEndY = WindowMouseY(#RectangleSelection_Window)
											If StartDrawing(WindowOutput(#RectangleSelection_Window))
												DrawImage(ImageID(lImage), 0, 0)
												DrawingMode(#PB_2DDrawing_Outlined)
												Box(lStartX, lStartY, lEndX - lStartX, lEndY - lStartY, $FF0000)
												StopDrawing()
											EndIf
										EndIf
									Case #PB_Event_Menu
										Select EventMenu()
											Case #RectangleSelection_Close
												bRectDone = #True
											Case #RectangleSelection_OK
												If lStartX < lEndX
													lImageX = lStartX
												Else
													lImageX = lEndX
												EndIf
												If lStartY < lEndY
													lImageY = lStartY
												Else
													lImageY = lEndY
												EndIf
												AddUpload(GrabImage(lImage, #PB_Any, lImageX, lImageY, Abs(lEndX - lStartX), Abs(lEndY - lStartY)), #FileName_Selection, #FileFormat_Selection)
												bRectDone = #True
										EndSelect
								EndSelect
							Until bRectDone
							CloseWindow(#RectangleSelection_Window)
						EndIf
						FreeImage(lImage)
					EndIf
				EndIf
			Else
				If Not bKeyState
					bKeyState = #True
					AddUpload(CaptureScreen(), #FileName_Desktop, #FileFormat_Desktop)
				EndIf
			EndIf
		Else
			bKeyState = #False
		EndIf
		Select WaitWindowEvent(10)
			Case #PB_Event_CloseWindow
				Macro_Events_CloseWindow
			Case #PB_Event_Gadget
				Macro_Events_Gadget
			Case #PB_Event_Menu
				Macro_Events_Menu
			Case #PB_Event_SizeWindow
				Macro_Events_SizeWindow
		EndSelect
	Until eGlobals\bQuit;- Main loop end
	RemoveSysTrayIconEx(WindowID(#Window), #Tray)
EndIf
; IDE Options = PureBasic 4.60 (Windows - x86)
; EnableXP
; EnableOnError
; UseIcon = Capture2Net.ico
; Executable = Capture2Net.exe
; EnableCompileCount = 787
; EnableBuildCount = 76
; EnableExeConstant
; IncludeVersionInfo
; VersionField0 = 1,0,0,0
; VersionField1 = 1,0,0,0
; VersionField2 = SelfCoders
; VersionField3 = Capture2Net
; VersionField4 = 1.00
; VersionField5 = 1.00
; VersionField6 = Capture2Net
; VersionField7 = Capture2Net
; VersionField8 = %EXECUTABLE
; VersionField13 = capture2net@selfcoders.com
; VersionField14 = http://www.selfcoders.com
; VersionField15 = VOS_NT_WINDOWS32
; VersionField16 = VFT_APP
; VersionField17 = 0409 English (United States)
; VersionField18 = Build
; VersionField19 = Project Start
; VersionField20 = Compile Time
; VersionField21 = %BUILDCOUNT
; VersionField22 = 2011-10-10
; VersionField23 = %yyyy-%mm-%dd %hh:%ii:%ss