Procedure.w GetNotifyIconDataSize()
	hDll = OpenLibrary(#PB_Any, "VERSION.DLL")
	If IsLibrary(hDll)
		lBufferSize= CallFunction(hDll, "GetFileVersionInfoSizeA", @"shell32.dll", 0)
		If lBufferSize>0
			sDataBuffer$ = Space(lBufferSize - 1)
			CallFunction(hDll, "GetFileVersionInfoA", @"shell32.dll", 0, lBufferSize,@sDataBuffer$)
			CallFunction(hDll, "VerQueryValueA", @sDataBuffer$, @"\", @pBuffer, @lLength)
			CopyMemory(pBuffer + 10, @wVersion, 2)
		EndIf
		CloseLibrary(hDll)
		Select wVersion
			Case 6
				ProcedureReturn #NOTIFYICONDATA_V3_SIZE
			Case 5
				ProcedureReturn #NOTIFYICONDATA_V2_SIZE
			Default
				ProcedureReturn #NOTIFYICONDATA_V1_SIZE
		EndSelect
	EndIf
EndProcedure

Procedure AddSysTrayIconEx(hWindow, hTray, lImage, sToolTipText$)
	eBalloon.eIconData\cbSize = GetNotifyIconDataSize()
	eBalloon\hwnd = hWindow
	eBalloon\uId = hTray
	eBalloon\uFlags = #NIF_MESSAGE | #NIF_ICON | #NIF_TIP
	eBalloon\hIcon = lImage
	eBalloon\dwState = #NIS_SHAREDICON
	eBalloon\uCallbackMessage = #WM_USER
	If OSVersion() < #PB_OS_Windows_2000
		eBalloon\uVersion = 0
	Else
		eBalloon\uVersion = #NOTIFYICON_VERSION
	EndIf
		eBalloon\uTimeout = 11000
		eBalloon\dwInfoFlags = #NIIF_INFO
	If eBalloon.eIconData\cbSize = #NOTIFYICONDATA_V1_SIZE
		PokeS(@eBalloon\szTip, sToolTipText$, 64)
	Else
		PokeS(@eBalloon\szTip, sToolTipText$, 128)
	EndIf
	hDll = OpenLibrary(#PB_Any, "shell32.dll")
	If IsLibrary(hDll)
		CallFunction(hDll, "Shell_NotifyIcon", #NIM_ADD, @eBalloon)
		CloseLibrary(hDll)
	EndIf
EndProcedure

Procedure RemoveSysTrayIconEx(hWindow, hTray)
	eBalloon.eIconData\cbSize = GetNotifyIconDataSize()
	eBalloon\hwnd = hWindow
	eBalloon\uId = hTray
	hDll = OpenLibrary(#PB_Any, "shell32.dll")
	If IsLibrary(hDll)
		CallFunction(hDll, "Shell_NotifyIcon", #NIM_DELETE, @eBalloon)
		CloseLibrary(hDll)
	EndIf
EndProcedure

Procedure ShowSysTrayBalloonTip(hWindow, hTray, lImage, sTitle$, sMainText$, sToolTipText$, lIconType)
	eBalloon.eIconData\cbSize = GetNotifyIconDataSize()
	eBalloon\hwnd = hWindow
	eBalloon\uId = hTray
	eBalloon\uFlags =  #NIF_INFO | #NIF_MESSAGE | #NIF_ICON | #NIF_TIP
	eBalloon\hIcon = lImage
	eBalloon\dwState = #NIS_SHAREDICON
	eBalloon\uCallbackMessage = #WM_USER
	eBalloon\uTimeout = 10000
	If OSVersion() < #PB_OS_Windows_2000
		eBalloon\uVersion = 0
	Else
		eBalloon\uVersion = #NOTIFYICON_VERSION
	EndIf
	eBalloon\dwInfoFlags = lIconType
	If eBalloon.eIconData\cbSize = #NOTIFYICONDATA_V1_SIZE
		PokeS(@eBalloon\szTip, sToolTipText$, 64)
	Else
		PokeS(@eBalloon\szTip, sToolTipText$, 128)
		PokeS(@eBalloon\szInfo, sMainText$, 255)
		PokeS(@eBalloon\szInfoTitle, sTitle$, 63)
	EndIf
	hDll = OpenLibrary(#PB_Any, "shell32.dll")
	If IsLibrary(hDll)
		CallFunction(hDll, "Shell_NotifyIcon", #NIM_MODIFY, @eBalloon)
		CloseLibrary(hDll)
	EndIf
EndProcedure

Procedure DeleteRegValue(hKey, sSubKey$, sValue$)
	If RegOpenKeyEx_(hKey, sSubKey$, #Null, #KEY_ALL_ACCESS, @hKey) = #ERROR_SUCCESS
		If RegDeleteValue_(hKey, sValue$) = #ERROR_SUCCESS
			bDeleted.b = #True
		EndIf
		RegCloseKey_(hKey)
	EndIf
	ProcedureReturn bDeleted
EndProcedure

Procedure.s GetRegValue(hKey, sSubKey$, sName$, sDefaultValue$ = "")
	lSize = 255
	sString$ = Space(lSize)
	If RegOpenKeyEx_(hKey, sSubKey$, #Null, #KEY_ALL_ACCESS, @hKey) = #ERROR_SUCCESS
		If RegQueryValueEx_(hKey, sName$, #Null, @lType, @sString$, @lSize) = #ERROR_SUCCESS
			Select lType
				Case #REG_SZ
					If RegQueryValueEx_(hKey, sName$, #Null, @lType, @sString$, @lSize) = #ERROR_SUCCESS
						ProcedureReturn Trim(Left(sString$, lSize - 1))
					EndIf
				Case #REG_DWORD
					If RegQueryValueEx_(hKey, sName$, #Null, @lType, @lValue, @lSize) = #ERROR_SUCCESS
						ProcedureReturn Str(lValue)
					EndIf
			EndSelect
		EndIf
	EndIf
	ProcedureReturn sDefaultValue$
EndProcedure

Procedure SetRegValue(hKey, sSubKey$, sName$, sValue$)
	If RegCreateKeyEx_(hKey, sSubKey$, #Null, #Null, #REG_OPTION_NON_VOLATILE, #KEY_ALL_ACCESS, #Null, @hNewKey, @lKeyInfo) = #ERROR_SUCCESS
		RegSetValueEx_(hNewKey, sName$, #Null, #REG_SZ, sValue$, StringByteLength(sValue$))
		RegCloseKey_(hNewKey)
		ProcedureReturn #True
	EndIf
EndProcedure

Procedure Editor_Format(lGadget, lFlags)
	CHARFORMAT.CHARFORMAT
	CHARFORMAT\cbSize = SizeOf(CHARFORMAT)
	CHARFORMAT\dwMask = #CFM_ITALIC | #CFM_BOLD | #CFM_STRIKEOUT | #CFM_UNDERLINE
	CHARFORMAT\dwEffects = lFlags
	SendMessage_(GadgetID(lGadget), #EM_SETCHARFORMAT, #SCF_SELECTION, @CHARFORMAT)
EndProcedure

Procedure Editor_Select(lGadget, lLineStart = 0, lCharStart = 0, lLineEnd = 0, lCharEnd = 0)
	CHARRANGE.CHARRANGE
	CHARRANGE\cpMin = SendMessage_(GadgetID(lGadget), #EM_LINEINDEX, lLineStart, 0) + lCharStart - 1
	If lLineEnd = -1
		lLineEnd = SendMessage_(GadgetID(lGadget), #EM_GETLINECOUNT, 0, 0) - 1
	EndIf
	CHARRANGE\cpMax = SendMessage_(GadgetID(lGadget), #EM_LINEINDEX, lLineEnd, 0)
	If lCharEnd = -1
		CHARRANGE\cpMax + SendMessage_(GadgetID(lGadget), #EM_LINELENGTH, CHARRANGE\cpMax, 0)
	Else
		CHARRANGE\cpMax + lCharEnd
	EndIf
	SendMessage_(GadgetID(lGadget), #EM_EXSETSEL, 0, @CHARRANGE)
EndProcedure

Procedure FormatEditorText(lGadget, sText$)
	SetGadgetText(lGadget, RemoveString(RemoveString(sText$, "[b]", #PB_String_NoCase), "[/b]", #PB_String_NoCase))
	Repeat
		lSelectStart = FindString(sText$, "[b]")
		If lSelectStart
			sText$ = RemoveString(sText$, "[b]", #PB_String_CaseSensitive, lSelectStart, 1)
		Else
			Break
		EndIf
		lSelectEnd = FindString(sText$, "[/b]")
		If lSelectEnd
			sText$ = RemoveString(sText$, "[/b]", #PB_String_CaseSensitive, lSelectEnd, 1)
			CHARRANGE.CHARRANGE
			CHARRANGE\cpMin = lSelectStart - 1
			CHARRANGE\cpMax = lSelectEnd - 1
			SendMessage_(GadgetID(lGadget), #EM_EXSETSEL, 0, @CHARRANGE)
			Editor_Format(lGadget, #CFM_BOLD)
		Else
			Break
		EndIf
	ForEver
	Editor_Select(lGadget)
EndProcedure

Procedure Set3StateCheckbox(lGadget)
	SendMessage_(GadgetID(lGadget), #BM_SETSTYLE, #BS_AUTO3STATE, #Null)
	SendMessage_(GadgetID(lGadget), #BM_SETCHECK, #BST_INDETERMINATE, #Null)
EndProcedure

Procedure Memory2File(sFile$, *pMemory, lSize)
	lFile = CreateFile(#PB_Any, sFile$)
	If IsFile(lFile)
		WriteData(lFile, *pMemory, lSize)
		CloseFile(lFile)
	EndIf
EndProcedure

Procedure CreateQRCode(sString$, lSizeFactor)
	bSuccess.b
	sFile$ = GetPathPart(ProgramFilename()) + "qrcodelib.dll"
	lFile = ReadFile(#PB_Any, sFile$)
	If IsFile(lFile)
		CloseFile(lFile)
	Else
		Memory2File(sFile$, ?Lib_QRCode, ?Sound_Camera - ?Lib_QRCode)
	EndIf
	lDll = OpenLibrary(#PB_Any, sFile$)
	If IsLibrary(lDll)
		*pQRCode.eQRCode = CallCFunction(lDll, "QRcode_encodeString8bit", @sString$, 0, #QR_ECLEVEL_H)
		If Not *pQRCode Or Not *pQRCode\Width
			CloseLibrary(lDll)
			ProcedureReturn #Null
		Else
			*pSymbolData = *pQRCode\pSymbolData
			lSize = *pQRCode\Width
		EndIf
		lImage = CreateImage(#PB_Any, lSize, lSize)
		If IsImage(lImage)
			If StartDrawing(ImageOutput(lImage))
				Box(0, 0, lSize, lSize, #White)
				For lY = 0 To lSize - 1
					For lX = 0 To lSize - 1
						If (PeekB(*pSymbolData) & $FF) & 1
							Plot( lX, lY, #Black)
						EndIf
						*pSymbolData + 1
					Next
				Next
				StopDrawing()
				If ResizeImage(lImage, lSize * lSizeFactor, lSize * lSizeFactor, #PB_Image_Raw)
					bSuccess = #True
				EndIf
			EndIf
			If Not bSuccess
				FreeImage(lImage)
			EndIf
		EndIf
		CallCFunction(lDll, "QRcode_free", *pQRCode)
		CloseLibrary(lDll)
		If bSuccess
			ProcedureReturn lImage
		EndIf
	EndIf
EndProcedure

Procedure ShowQRCode(sString$)
	lImage = CreateQRCode(sString$, 4)
	If IsImage(lImage)
		If OpenWindow(#QRCodeDisplay_Window, 100, 100, ImageWidth(lImage), ImageHeight(lImage), "QR Code", #PB_Window_SystemMenu | #PB_Window_Tool | #PB_Window_WindowCentered, WindowID(#Window))
			ImageGadget(#QRCodeDisplay_Image, 0, 0, WindowWidth(#QRCodeDisplay_Window), WindowHeight(#QRCodeDisplay_Window), ImageID(lImage))
			SetGadgetData(#QRCodeDisplay_Image, lImage)
			DisableWindow(#Window, #True)
			SetForegroundWindow_(WindowID(#QRCodeDisplay_Window))
		Else
			FreeImage(lImage)
		EndIf
	EndIf
EndProcedure

Procedure SwapValue(lValue)
	If lValue
		ProcedureReturn #False
	Else
		ProcedureReturn #True
	EndIf
EndProcedure

Procedure OpenConfig()
	lFile = ReadFile(#PB_Any, eGlobals\sConfigFile)
	If IsFile(lFile)
		lFactor = ReadLong(lFile)
		Repeat
			lLength = ReadLong(lFile)
			sString$ = ""
			For lChar = 1 To lLength
				sString$ + Chr(ReadLong(lFile) / lFactor)
			Next
			lSeparator = FindString(sString$, ":", 1)
			If lSeparator
				AddElement(ConfigValues())
				ConfigValues()\sKey = Trim(Mid(sString$, 1, lSeparator - 1))
				ConfigValues()\sValue = Mid(sString$, lSeparator + 1)
			EndIf
		Until Eof(lFile)
		CloseFile(lFile)
	EndIf
EndProcedure

Procedure SaveConfig()
	lFile = CreateFile(#PB_Any, eGlobals\sConfigFile)
	If IsFile(lFile)
		lFactor = Random(9) + 1
		WriteLong(lFile, lFactor)
		ForEach ConfigValues()
			sString$ = ConfigValues()\sKey + ":" + ConfigValues()\sValue
			WriteLong(lFile, Len(sString$))
			For lChar=1 To Len(sString$)
				WriteLong(lFile,Asc(Mid(sString$, lChar, 1)) * lFactor)
			Next
		Next
		CloseFile(lFile)
	EndIf
EndProcedure

Procedure WriteConfigString(sFieldName$, sValue$)
	ForEach ConfigValues()
		If LCase(ConfigValues()\sKey) = LCase(sFieldName$)
			ConfigValues()\sValue = sValue$
			ProcedureReturn #True
		EndIf
	Next
	AddElement(ConfigValues())
	ConfigValues()\sKey = sFieldName$
	ConfigValues()\sValue = sValue$
EndProcedure

Procedure WriteConfigLong(sFieldName$, lValue)
	WriteConfigString(sFieldName$, Str(lValue))
EndProcedure

Procedure.s ReadConfigString(sFieldName$, sDefaultValue$ = "")
	ForEach ConfigValues()
		If LCase(ConfigValues()\sKey) = LCase(sFieldName$)
			ProcedureReturn ConfigValues()\sValue
		EndIf
	Next
	ProcedureReturn sDefaultValue$
EndProcedure

Procedure ReadConfigLong(sFieldName$, lDefaultValue = 0)
	ProcedureReturn Val(ReadConfigString(sFieldName$, Str(lDefaultValue)))
EndProcedure

Procedure ComboBoxGadgetEx(lGadget, lX, lY, lWidth, lHeight, sFieldName$, lDefaultState = 0, sItem1$ = "", sItem2$ = "", sItem3$ = "", sItem4$ = "", lFlags = 0)
	hGadgetID = ComboBoxGadget(lGadget, lX, lY, lWidth, lHeight, lFlags)
	If hGadgetID
		AddElement(ConfigGadgets())
		ConfigGadgets()\bIsString = #False
		ConfigGadgets()\lGadget = lGadget
		ConfigGadgets()\sFieldName = sFieldName$
		If sItem1$
			AddGadgetItem(lGadget, -1, sItem1$)
		EndIf
		If sItem2$
			AddGadgetItem(lGadget, -1, sItem2$)
		EndIf
		If sItem3$
			AddGadgetItem(lGadget, -1, sItem3$)
		EndIf
		If sItem4$
			AddGadgetItem(lGadget, -1, sItem4$)
		EndIf
		SetGadgetState(lGadget, ReadConfigLong(sFieldName$, lDefaultState))
	EndIf
	ProcedureReturn hGadgetID
EndProcedure

Procedure CheckBoxGadgetEx(lGadget, lX, lY, lWidth, lHeight, sText$, sFieldName$, lDefaultState = #False, lFlags = 0)
	hGadgetID = CheckBoxGadget(lGadget, lX, lY, lWidth, lHeight, sText$, lFlags)
	If hGadgetID
		AddElement(ConfigGadgets())
		ConfigGadgets()\bIsString = #False
		ConfigGadgets()\lGadget = lGadget
		ConfigGadgets()\sFieldName = sFieldName$
		SetGadgetState(lGadget, ReadConfigLong(sFieldName$, lDefaultState))
	EndIf
	ProcedureReturn hGadgetID
EndProcedure

Procedure ShortcutGadgetEx(lGadget, lX, lY, lWidth, lHeight, sFieldName$, lDefaultShortcut = 0)
	hGadgetID = ShortcutGadget(lGadget, lX, lY, lWidth, lHeight, ReadConfigLong(sFieldName$, lDefaultShortcut))
	If hGadgetID
		AddElement(ConfigGadgets())
		ConfigGadgets()\bIsString = #False
		ConfigGadgets()\lGadget = lGadget
		ConfigGadgets()\sFieldName = sFieldName$
	EndIf
	ProcedureReturn hGadgetID
EndProcedure

Procedure SpinGadgetEx(lGadget, lX, lY, lWidth, lHeight, lMinimum, lMaximum, sFieldName$, lDefaultValue = 0, sFormat$ = "", lFlags = 0)
	sFormat$ = "%"
	hGadgetID = SpinGadget(lGadget, lX, lY, lWidth, lHeight, lMinimum, lMaximum, lFlags)
	If hGadgetID
		AddElement(ConfigGadgets())
		ConfigGadgets()\bIsString = #False
		ConfigGadgets()\lGadget = lGadget
		ConfigGadgets()\sFieldName = sFieldName$
		SetGadgetState(lGadget, ReadConfigLong(sFieldName$, lDefaultValue))
		SetGadgetText(lGadget, ReplaceString(sFormat$, "%", Str(GetGadgetState(lGadget))))
	EndIf
	ProcedureReturn hGadgetID
EndProcedure

Procedure StringGadgetEx(lGadget, lX, lY, lWidth, lHeight, sFieldName$, sDefaultValue$ = "", lFlags = 0)
	hGadgetID = StringGadget(lGadget, lX, lY, lWidth, lHeight, ReadConfigString(sFieldName$, sDefaultValue$), lFlags)
	If hGadgetID
		AddElement(ConfigGadgets())
		ConfigGadgets()\bIsString = #True
		ConfigGadgets()\lGadget = lGadget
		ConfigGadgets()\sFieldName = sFieldName$
	EndIf
	ProcedureReturn hGadgetID
EndProcedure

Procedure TrackBarGadgetEx(lGadget, lX, lY, lWidth, lHeight, lMinimum, lMaximum, sFieldName$, lDefaultValue = 0, lFlags = 0)
	hGadgetID = TrackBarGadget(lGadget, lX, lY, lWidth, lHeight, lMinimum, lMaximum, lFlags)
	If hGadgetID
		AddElement(ConfigGadgets())
		ConfigGadgets()\bIsString = #False
		ConfigGadgets()\lGadget = lGadget
		ConfigGadgets()\sFieldName = sFieldName$
		SetGadgetState(lGadget, ReadConfigLong(sFieldName$, lDefaultValue))
	EndIf
	ProcedureReturn hGadgetID
EndProcedure

Procedure.s Base64EncoderEx(sString$, lLength = 1024)
	sEncoded$ = Space(lLength)
	Base64Encoder(@sString$, StringByteLength(sString$), @sEncoded$, lLength)
	ProcedureReturn sEncoded$
EndProcedure

Procedure.s HTTPRequest(sHostname$, lPort, sMethod$, sUrl$, bUseAuth.b = #False, sAppendData$ = "", sAddHeader$ ="", bReturnBody.b = #True, lGadgetItem = -1)
	hConnection = OpenNetworkConnection(sHostname$, lPort)
	If hConnection
		sHeader$ = sMethod$ + " " + sUrl$ + " HTTP/1.1"+Chr(13) + Chr(10)
		sHeader$ + "Host: " + sHostname$ + Chr(13) + Chr(10)
		If bUseAuth
			sHeader$ + "Authorization: Basic " + Base64EncoderEx(GetGadgetText(#WebDAV_Username) + ":" + GetGadgetText(#WebDAV_Password)) + Chr(13) + Chr(10)
		EndIf
		sHeader$ + "User-Agent: " + #Title + Chr(13) + Chr(10)
		If sAddHeader$
			sHeader$ + sAddHeader$ + Chr(13) + Chr(10)
		EndIf
		Select UCase(sMethod$)
			Case "POST"
				sHeader$ + "Content-Length: " + Str(Len(sAppendData$)) + Chr(13) + Chr(10)
				sHeader$ + "Content-Type: application/x-www-form-urlencoded" + Chr(13) + Chr(10)
			Case "PUT"
				sHeader$ + "Content-Length: " + Str(FileSize(sAppendData$)) + Chr(13) + Chr(10)
		EndSelect
		sHeader$ + Chr(13) + Chr(10)
		SendNetworkData(hConnection, @sHeader$, StringByteLength(sHeader$))
		Select UCase(sMethod$)
			Case "POST"
				SendNetworkData(hConnection, @sAppendData$, StringByteLength(sAppendData$))
			Case "PUT"
				lBuffer = AllocateMemory(1024 * 10)
				If lBuffer
					lFile = ReadFile(#PB_Any, sAppendData$)
					If IsFile(lFile)
						Repeat
							SendNetworkData(hConnection, lBuffer, ReadData(lFile, lBuffer, MemorySize(lBuffer)))
							If lGadgetItem <> -1
								sProgress$ = StrF((Loc(lFile) / Lof(lFile)) * 100, 0)
								If sProgress$ <> sOldProgress$
									sOldProgress$ = sProgress$
									LockMutex(eGlobals\lMutex)
									SetGadgetItemText(#UploadHistory, lGadgetItem, "Uploading... (" + sProgress$ + "%)", #UploadHistory_Column_Status)
									UnlockMutex(eGlobals\lMutex)
								EndIf
							EndIf
						Until Eof(lFile)
						CloseFile(lFile)
					EndIf
					FreeMemory(lBuffer)
				EndIf
		EndSelect
		Repeat
			If NetworkClientEvent(hConnection) = #PB_NetworkEvent_Data
				lLength = 4096
				sString$ = Space(lLength)
				ReceiveNetworkData(hConnection, @sString$, lLength)
				If bReturnBody
					For lLine = 1 To CountString(sString$, Chr(13))
						sLine$ = Trim(RemoveString(StringField(sString$, lLine, Chr(13)), Chr(10)))
						If LCase(Trim(StringField(sLine$, 1, ":"))) = "content-length"
							lLength = Val(Trim(StringField(sLine$, 2, ":")))
							Break
						EndIf
					Next
					lBodyStart = FindString(sString$, Chr(13) + Chr(10) + Chr(13) + Chr(10), 1) + 4
					If lBodyStart And lLength
						sReturn$ = Trim(Mid(sString$, lBodyStart))
						If 2048 - lBodyStart < lLength
							lSize = lLength - 2048 + lBodyStart
							sString$ = Space(lSize)
							ReceiveNetworkData(hConnection, @sString$, lSize)
							sReturn$ + sString$
						EndIf
					EndIf
				Else
					sReturn$ = Trim(Mid(sString$, 1, FindString(sString$, Chr(13) + Chr(10), 1) - 1))
					sReturn$ = Mid(sReturn$, FindString(sReturn$, " ", 1) + 1)
				EndIf
				Break
			EndIf
			Delay(10)
		ForEver
		CloseNetworkConnection(hConnection)
		ProcedureReturn sReturn$
	Else
		ProcedureReturn "Connection failed"
	EndIf
EndProcedure

Procedure GetImageFormat(lFileFormatGadget)
	Select GetGadgetState(lFileFormatGadget)
		Case #ImageFormat_BMP
			ProcedureReturn #PB_ImagePlugin_BMP
		Case #ImageFormat_JPEG
			ProcedureReturn #PB_ImagePlugin_JPEG
		Case #ImageFormat_JPEG2000
			ProcedureReturn #PB_ImagePlugin_JPEG2000
		Case #ImageFormat_PNG
			ProcedureReturn #PB_ImagePlugin_PNG
	EndSelect
EndProcedure

Procedure.s MakeFileName(lFileNameGadget, lFileFormatGadget)
	lDate = Date()
	lSize = 1024
	sHostname$ = Space(lSize)
	GetComputerName_(@sHostname$, @lSize)
	lSize = 1024
	sUsername$ = Space(lSize)
	GetUserName_(@sUsername$, @lSize)
	lSize = 1024
	sTitle$ = Space(lSize)
	GetWindowText_(GetForegroundWindow_(), @sTitle$, lSize)
	sString$ = GetGadgetText(lFileNameGadget)
	sString$ = ReplaceString(sString$, "%%", " %% ")
	sString$ = ReplaceString(sString$, "%Y", Str(Year(lDate)))
	sString$ = ReplaceString(sString$, "%y", Right(Str(Year(lDate)), 2))
	sString$ = ReplaceString(sString$, "%M", RSet(Str(Month(lDate)), 2, "0"))
	sString$ = ReplaceString(sString$, "%d", RSet(Str(Day(lDate)), 2, "0"))
	sString$ = ReplaceString(sString$, "%h", RSet(Str(Hour(lDate)), 2, "0"))
	sString$ = ReplaceString(sString$, "%m", RSet(Str(Minute(lDate)), 2, "0"))
	sString$ = ReplaceString(sString$, "%s", RSet(Str(Second(lDate)), 2, "0"))
	sString$ = ReplaceString(sString$, "%W", sTitle$)
	sString$ = ReplaceString(sString$, "%H", sHostname$)
	sString$ = ReplaceString(sString$, "%U", sUsername$)
	sString$ = ReplaceString(sString$, " %% ", "%")
	For lAscii = 0 To 255
		If (lAscii >= 32 And lAscii <= 46) Or (lAscii >= 48 And lAscii <= 57) Or (lAscii >= 65 And lAscii <= 90) Or lAscii = 95 Or (lAscii >= 97 And lAscii <= 122)
			; OK
		Else
			sString$ = ReplaceString(sString$, Chr(lAscii), "_")
		EndIf
	Next
	Select GetGadgetState(lFileFormatGadget)
		Case #ImageFormat_BMP
			sString$ + ".bmp"
		Case #ImageFormat_JPEG
			sString$ + ".jpg"
		Case #ImageFormat_JPEG2000
			sString$ + ".jp2"
		Case #ImageFormat_PNG
			sString$ + ".png"
	EndSelect
	ProcedureReturn sString$
EndProcedure

Procedure UpdateUrlPreview()
	SetGadgetText(#UrlPreview_ActiveWindow, GetGadgetText(#WebDAV_AccessPath) + MakeFileName(#FileName_ActiveWindow, #FileFormat_ActiveWindow))
	SetGadgetText(#UrlPreview_Selection, GetGadgetText(#WebDAV_AccessPath) + MakeFileName(#FileName_Selection, #FileFormat_Selection))
	SetGadgetText(#UrlPreview_Desktop, GetGadgetText(#WebDAV_AccessPath) + MakeFileName(#FileName_Desktop, #FileFormat_Desktop))
EndProcedure

Procedure HideWindowEx(bState.b)
	HideWindow(#Window, bState)
	If Not bState
		SetActiveWindow_(WindowID(#Window))
		UpdateUrlPreview()
	EndIf
EndProcedure

Procedure CaptureScreen(hWnd = 0, lRectX = 0, lRectY = 0, lRectWidth = 0, lRectHeight = 0)
	lX = GetSystemMetrics_(#SM_XVIRTUALSCREEN)
	lY = GetSystemMetrics_(#SM_YVIRTUALSCREEN)
	lWidth = GetSystemMetrics_(#SM_CXVIRTUALSCREEN)
	lHeight = GetSystemMetrics_(#SM_CYVIRTUALSCREEN)
	lImage = CreateImage(#PB_Any, lWidth, lHeight)
	If IsImage(lImage)
		hDcDesktop = GetDC_(GetDesktopWindow_())
		hDC = StartDrawing(ImageOutput(lImage))
		BitBlt_(hDC, 0, 0, lWidth, lHeight, hDcDesktop, lX, lY, #SRCCOPY)
		StopDrawing()
		ReleaseDC_(GetDesktopWindow_(), hDcDesktop)
		If hWnd
			GetWindowRect_(hWnd, @RECT.RECT)
			lRectX = Abs(lX) + RECT\left
			lRectY = Abs(lY) + RECT\top
			lRectWidth = RECT\right - RECT\left
			lRectHeight = RECT\bottom - RECT\top
		EndIf
		If lRectWidth And lRectHeight
			If lRectX < 0
				lRectX = 0
			EndIf
			If lRectY < 0
				lRectY = 0
			EndIf
			lImage2 = GrabImage(lImage, #PB_Any, lRectX, lRectY, lRectWidth, lRectHeight)
			FreeImage(lImage)
			lImage = lImage2
		EndIf
		ProcedureReturn lImage
	EndIf
EndProcedure

Procedure.s FormatSize(fSize.f)
	If fSize<1024
		sType$="B"
	ElseIf fSize<1024 * 1024
		sType$="KB"
		fSize / 1024
	ElseIf fSize<1024 * 1024 * 1024
		sType$="MB"
		fSize / 1024 / 1024
	ElseIf fSize<1024 * 1024 * 1024 * 1024
		sType$="GB"
		fSize / 1024 / 1024 / 1024
	ElseIf fSize<1024 * 1024 * 1024 * 1024 * 1024
		sType$="TB"
		fSize / 1024 / 1024 / 1024 / 1024
	Else
		sType$="B"
	EndIf
	sSize$ = StrF(fSize, 2)
	If Right(sSize$, 2) = "00"
		sSize$ = Left(sSize$, Len(sSize$) - 3)
	EndIf
	ProcedureReturn sSize$ + " " + sType$
EndProcedure

Procedure.s MakeTime(lDate)
	ProcedureReturn FormatDate("%yyyy-%mm-%dd %hh:%ii:%ss", lDate)
EndProcedure

Procedure UploadQueueThread(lNull)
	Repeat
		LockMutex(eGlobals\lMutex)
		sFilename$ = ""
		If ListSize(UploadQueue())
			FirstElement(UploadQueue())
			lItem = UploadQueue()\lItem
			sFilename$ = UploadQueue()\sFilename
			SetGadgetItemText(#UploadHistory, lItem, "Uploading...", #UploadHistory_Column_Status)
		EndIf
		UnlockMutex(eGlobals\lMutex)
		If sFilename$
			sUrl$ = URLEncoder(GetGadgetText(#WebDAV_AccessPath) + GetFilePart(sFilename$))
			sResponseCode$ = HTTPRequest(GetGadgetText(#WebDAV_Server), Val(GetGadgetText(#WebDAV_Port)), "PUT", URLEncoder(GetGadgetText(#WebDAV_Path) + GetFilePart(sFilename$)), GetGadgetState(#WebDAV_AuthRequired), sFilename$, "", #False, lItem)
			If GetGadgetState(#OtherSettings_ShortUrl)
				Select GetGadgetState(#OtherSettings_ShortUrlProvider)
					Case #ShortUrlProvider_Sh0rtAt
						sReturnData$ = HTTPRequest("sh0rt.at", 80, "POST", "/add", #False, "url=" + sUrl$, "", #True)
						If StringField(sReturnData$, 1, Chr(10)) = "OK"
							sShortUrl$ = "http://sh0rt.at/" + StringField(sReturnData$, 2, Chr(10))
						EndIf
					Case #ShortUrlProvider_TinyURL
						sShortUrl$ = HTTPRequest("tinyurl.com", 80, "GET", "/api-create.php?url=" + sUrl$, #False, "", "", #True)
				EndSelect
				lFindHTTP = FindString(sShortUrl$, "http://", 1)
				If lFindHTTP
					sShortUrl$ = StringField(Trim(Mid(sShortUrl$, lFindHTTP)), 1, Chr(13))
				Else
					sShortUrl$ = ""
				EndIf
			EndIf
			lHTTPCode = Val(sResponseCode$)
			LockMutex(eGlobals\lMutex)
			sType$ = GetGadgetItemText(#UploadHistory, lItem, #UploadHistory_Column_Type)
			sDimensions$ = GetGadgetItemText(#UploadHistory, lItem, #UploadHistory_Column_Dimensions)
			RemoveGadgetItem(#UploadHistory, lItem)
			If lHTTPCode < 200 Or lHTTPCode > 299
				AddGadgetItem(#UploadHistory, lItem, GetFilePart(sFilename$) + Chr(10) + FormatSize(FileSize(sFilename$)) + Chr(10) + "Failed" + Chr(10) + sType$ + Chr(10) + sDimensions$, ImageID(#Icon_Error))
				PlaySound_("SystemHand",0, #SND_ALIAS | #SND_NODEFAULT | #SND_NOWAIT | #SND_ASYNC)
				ShowSysTrayBalloonTip(WindowID(#Window), #Tray, eGlobals\hAppIcon, "Upload failed!", "Server response: " + sResponseCode$, #Title, #NIIF_ERROR)
			Else
				If lHTTPCode = 201
					sStatus$ = "Done"
				Else
					sStatus$ = sResponseCode$
				EndIf
				PlaySound_("SystemAsterisk",0, #SND_ALIAS | #SND_NODEFAULT | #SND_NOWAIT | #SND_ASYNC)
				lDate = Date()
				If GetGadgetState(#History_SaveToFile)
					lXML = LoadXML(#PB_Any, GetGadgetText(#History_File))
					If IsXML(lXML)
						*pMainNode = MainXMLNode(lXML)
					Else
						lXML = CreateXML(#PB_Any)
						If IsXML(lXML)
							*pMainNode = CreateXMLNode(RootXMLNode(lXML))
							If *pMainNode
								SetXMLNodeName(*pMainNode, "history")
							EndIf
						EndIf
					EndIf
					If *pMainNode
						*pNode = CreateXMLNode(*pMainNode, -1)
						If *pNode
							SetXMLNodeName(*pNode, "item")
							SetXMLAttribute(*pNode, "file", GetFilePart(sFilename$))
							SetXMLAttribute(*pNode, "size", Str(FileSize(sFilename$)))
							SetXMLAttribute(*pNode, "type", sType$)
							SetXMLAttribute(*pNode, "width", Trim(StringField(sDimensions$, 1, "x")))
							SetXMLAttribute(*pNode, "height", Trim(StringField(sDimensions$, 2, "x")))
							SetXMLAttribute(*pNode, "url", sUrl$)
							SetXMLAttribute(*pNode, "shorturl", sShortUrl$)
							SetXMLAttribute(*pNode, "time", Str(lDate))
						EndIf
						FormatXML(lXML, #PB_XML_LinuxNewline | #PB_XML_ReduceNewline | #PB_XML_ReduceSpace | #PB_XML_ReFormat | #PB_XML_ReIndent, 4)
						SaveXML(lXML, GetGadgetText(#History_File))
						FreeXML(lXML)
					EndIf
				EndIf
				AddGadgetItem(#UploadHistory, lItem, GetFilePart(sFilename$) + Chr(10) + FormatSize(FileSize(sFilename$)) + Chr(10) + sStatus$ + Chr(10) + sType$ + Chr(10) + sDimensions$ + Chr(10) + sUrl$ + Chr(10) + sShortUrl$ + Chr(10) + MakeTime(lDate), ImageID(#Icon_Complete))
				If sShortUrl$
					sUrl$ = sShortUrl$
				EndIf
				If GetGadgetState(#OtherSettings_CopyUrl)
					SetClipboardText(sUrl$)
				EndIf
				ShowSysTrayBalloonTip(WindowID(#Window), #Tray, eGlobals\hAppIcon, "Upload complete!", "Url: " + sUrl$, #Title, #NIIF_INFO)
			EndIf
			FirstElement(UploadQueue())
			DeleteElement(UploadQueue())
			UnlockMutex(eGlobals\lMutex)
			DeleteFile(sFilename$)
		EndIf
		Delay(10)
	ForEver
EndProcedure

Procedure AddUpload(lImage, lFileNameGadget, lFileFormatGadget)
	If IsImage(lImage)
		sFilename$ = GetTemporaryDirectory() + MakeFileName(lFileNameGadget, lFileFormatGadget)
		SaveImage(lImage, sFilename$, GetImageFormat(lFileFormatGadget), 10)
		lImageWidth = ImageWidth(lImage)
		lImageHeight = ImageHeight(lImage)
		FreeImage(lImage)
		PlaySound(#Sound_Camera)
		Select lFileNameGadget
			Case #FileName_ActiveWindow
				sType$ = "Window"
			Case #FileName_Selection
				sType$ = "Selection"
			Case #FileName_Desktop
				sType$ = "Desktop"
		EndSelect
		bLocked.b
		If TryLockMutex(eGlobals\lMutex)
			bLocked = #True
		Else
			For lDelay = 1 To 10
				Delay(10)
				If TryLockMutex(eGlobals\lMutex)
					bLocked = #True
					Break
				EndIf
			Next
		EndIf
		If bLocked
			AddGadgetItem(#UploadHistory, -1, GetFilePart(sFilename$) + Chr(10) + FormatSize(FileSize(sFilename$)) + Chr(10) + "Waiting" + Chr(10) + sType$ + Chr(10) + Str(lImageWidth) + " x " + Str(lImageHeight), ImageID(#Icon_Queued))
			AddElement(UploadQueue())
			UploadQueue()\lItem = CountGadgetItems(#UploadHistory) - 1
			UploadQueue()\sFilename = sFilename$
			UnlockMutex(eGlobals\lMutex)
		Else
			MessageRequester(#Title, "Thread lock failed after to many reties!", #MB_ICONERROR)
		EndIf
	EndIf
EndProcedure

Procedure AddGadgetString(lGadget, sString$)
	SetGadgetText(lGadget, GetGadgetText(lGadget) + sString$)
	UpdateUrlPreview()
EndProcedure

Procedure LoadHistory()
	ClearGadgetItems(#UploadHistory)
	If GetGadgetState(#History_SaveToFile)
		lXML = LoadXML(#PB_Any, GetGadgetText(#History_File))
		If IsXML(lXML)
			If XMLStatus(lXML) = #PB_XML_Success
				*pMainNode = MainXMLNode(lXML)
				If *pMainNode
					*pHistoryItemNode = ChildXMLNode(*pMainNode)
					While *pHistoryItemNode
						If LCase(GetXMLNodeName(*pHistoryItemNode)) = "item"
							AddGadgetItem(#UploadHistory, -1, GetXMLAttribute(*pHistoryItemNode, "file") + Chr(10) + FormatSize(Val(GetXMLAttribute(*pHistoryItemNode, "size"))) + Chr(10) + "Done" + Chr(10) + GetXMLAttribute(*pHistoryItemNode, "type") + Chr(10) + GetXMLAttribute(*pHistoryItemNode, "width") + " x " + GetXMLAttribute(*pHistoryItemNode, "height") + Chr(10) + GetXMLAttribute(*pHistoryItemNode, "url") + Chr(10) + GetXMLAttribute(*pHistoryItemNode, "shorturl") + Chr(10) + MakeTime(Val(GetXMLAttribute(*pHistoryItemNode, "time"))), ImageID(#Icon_Complete))
						EndIf
						*pHistoryItemNode = NextXMLNode(*pHistoryItemNode)
					Wend
				EndIf
			Else
				MessageRequester(#Title, "Invalid history file!" + Chr(13) + Chr(13) + "Error: " + XMLError(lXML) + Chr(13) + "Line: " + Str(XMLErrorLine(lXML)) + Chr(13) + Str(XMLErrorPosition(lXML)), #MB_ICONERROR)
			EndIf
			FreeXML(lXML)
		EndIf
	EndIf
EndProcedure

Procedure CheckHistoryUrls(lNull)
	For lItem = 0 To CountGadgetItems(#UploadHistory) - 1
		SetGadgetItemText(#UploadHistory, lItem, "Waiting for check...", #UploadHistory_Column_Status)
	Next
	SetGadgetAttribute(#CheckHistoryUrls_Progress, #PB_ProgressBar_Maximum, CountGadgetItems(#UploadHistory))
	For lItem = 0 To CountGadgetItems(#UploadHistory) - 1
		If eGlobals\bStopCheckHistoryUrls
			Break
		EndIf
		SetGadgetText(#CheckHistoryUrls_Text, "Checking Url " + Str(lItem + 1) + " of " + Str(CountGadgetItems(#UploadHistory)) + "...")
		SetGadgetState(#CheckHistoryUrls_Progress, lItem)
		sUrl$ = GetGadgetItemText(#UploadHistory, lItem, #UploadHistory_Column_Url)
		If sUrl$
			lHTTPStatusCode = Val(StringField(StringField(GetHTTPHeader(sUrl$), 1, Chr(10)), 2, " "))
			If lHTTPStatusCode >= 200 And lHTTPStatusCode <= 299
				SetGadgetItemColor(#UploadHistory, lItem, #PB_Gadget_BackColor, $00FF00, -1)
			ElseIf lHTTPStatusCode >= 300 And lHTTPStatusCode <= 399
				SetGadgetItemColor(#UploadHistory, lItem, #PB_Gadget_BackColor, $00FFFF, -1)
			Else
				SetGadgetItemColor(#UploadHistory, lItem, #PB_Gadget_BackColor, $0000FF, -1)
			EndIf
			SetGadgetItemText(#UploadHistory, lItem, StringField(GetHTTPHeader(sUrl$), 1, Chr(10)) , #UploadHistory_Column_Status)
		Else
			SetGadgetItemText(#UploadHistory, lItem, "Check: No Url", #UploadHistory_Column_Status)
		EndIf
	Next
	For lItem = 0 To CountGadgetItems(#UploadHistory) - 1
		If GetGadgetItemText(#UploadHistory, lItem, #UploadHistory_Column_Status) = "Waiting for check..."
			SetGadgetItemText(#UploadHistory, lItem, "Done", #UploadHistory_Column_Status)
		EndIf
	Next
EndProcedure

Procedure Quit()
	LockMutex(eGlobals\lMutex)
	lQueueSize = ListSize(UploadQueue())
	UnlockMutex(eGlobals\lMutex)
	If lQueueSize
		If MessageRequester(#Title, "There are some items in your upload queue!" + Chr(13) + "If you quit the application the upload will be canceled!" + Chr(13) + Chr(13) + "Are you sure to quit?", #MB_ICONWARNING | #MB_YESNO)
			eGlobals\bQuit = #True
		EndIf
	Else
		eGlobals\bQuit = #True
	EndIf
EndProcedure

Procedure GetParameter(sName$)
	For lIndex = 1 To CountProgramParameters()
		If LCase(ProgramParameter(lIndex - 1)) = "/" + LCase(sName$)
			ProcedureReturn lIndex
		EndIf
	Next
EndProcedure

Procedure CheckUpdate(bAlwaysShowMessage.b, lTryNo = 1)
	sUpdateInfo$ = HTTPRequest("updates.selfcoders.com", 80, "GET", "/getupdate.php?project=" + LCase(#InternName), #False, "", "", #True)
	If sUpdateInfo$
		lBuild = Val(StringField(sUpdateInfo$, 1, Chr(10)))
		If lBuild > #PB_Editor_CompileCount
			If MessageRequester(#Title, "An update is available!" + Chr(13) + Chr(13) + "Installed build: " + Str(#PB_Editor_CompileCount) + Chr(13) + "Available build: " + Str(lBuild) + Chr(13) + Chr(13) + "Do you want to download and install it now?" + Chr(13) + Chr(13) + "Warning: The upload queue will be stopped!", #MB_ICONQUESTION | #MB_YESNO) = #PB_MessageRequester_Yes
				RunProgram(ProgramFilename(), "/update1 " + Chr(34) + StringField(sUpdateInfo$, 2, Chr(10)) + Chr(34), GetPathPart(ProgramFilename()))
				End
			EndIf
		Else
			If bAlwaysShowMessage
				MessageRequester(#Title, "You already have the latest version!", #MB_ICONINFORMATION)
			EndIf
		EndIf
	Else
		If lTryNo < 5
			Delay(1000)
			CheckUpdate(bAlwaysShowMessage, lTryNo + 1)
		Else
			MessageRequester(#Title, "Update check failed!" + Chr(13) + Chr(13) + "If this error continues, please look on selfcoders.com for the current version.", #MB_ICONERROR)
		EndIf
	EndIf
EndProcedure

Procedure WindowCallback(hWnd, lMessage, wParam, lParam)
	Select lMessage
		Case eGlobals\lTaskBarWindowMessage
			RemoveSysTrayIconEx(WindowID(#Window), #Tray)
			AddSysTrayIconEx(WindowID(#Window), #Tray, eGlobals\hAppIcon, #Title)
		Case #WM_NOTIFY
			*pENLINK.ENLINK = lParam
			If *pENLINK\nmhdr\code = #EN_LINK
				If *pENLINK\msg = #WM_LBUTTONDOWN
					*pString = AllocateMemory(4096)
					TEXTRANGE.TEXTRANGE
					TEXTRANGE\chrg\cpMin = *pENLINK\chrg\cpMin
					TEXTRANGE\chrg\cpMax = *pENLINK\chrg\cpMax
					TEXTRANGE\lpstrText = *pString
					SendMessage_(GadgetID(#AboutText), #EM_GETTEXTRANGE, 0, TEXTRANGE)
					RunProgram(PeekS(*pString))
					FreeMemory(*pString)
				EndIf
			EndIf
		Case #WM_QUERYENDSESSION
			LockMutex(eGlobals\lMutex)
			lQueueSize = ListSize(UploadQueue())
			UnlockMutex(eGlobals\lMutex)
			If lQueueSize
				If MessageRequester(#Title, "There are some items in your upload queue!" + Chr(13) + "If you quit the application the upload will be canceled!" + Chr(13) + Chr(13) + "Are you sure to quit?", #MB_ICONWARNING | #MB_YESNO)
					ProcedureReturn #True
				EndIf
			Else
				ProcedureReturn #True
			EndIf
		Case #WM_USER
			Select lParam
				Case #WM_LBUTTONDBLCLK
					HideWindowEx(#False)
				Case #WM_RBUTTONDOWN
					DisplayPopupMenu(#TrayMenu, WindowID(#Window))
			EndSelect
	EndSelect
	ProcedureReturn #PB_ProcessPureBasicEvents
EndProcedure

Procedure OnError()
	sErrorMessage$ = "A program error occured!" + Chr(13) + Chr(10)
	sErrorMessage$ + Chr(13) + Chr(10)
	sErrorMessage$ + "Error Message:   " + ErrorMessage() + Chr(13) + Chr(10)
	sErrorMessage$ + "Error Code:      " + Str(ErrorCode()) + Chr(13) + Chr(10)
	sErrorMessage$ + "Code Address:    " + Str(ErrorAddress()) + Chr(13) + Chr(10)
	If ErrorCode() = #PB_OnError_InvalidMemory
		sErrorMessage$ + "Target Address:  " + Str(ErrorTargetAddress()) + Chr(13) + Chr(10)
	EndIf
	sErrorMessage$ + "Sourcecode line: " + Str(ErrorLine()) + Chr(13) + Chr(10)
	sErrorMessage$ + "Sourcecode file: " + ErrorFile() + Chr(13) + Chr(10)
	sErrorMessage$ + Chr(13) + Chr(10)
	sErrorMessage$ + "Register content:" + Chr(13) + Chr(10)
	CompilerSelect #PB_Compiler_Processor
		CompilerCase #PB_Processor_x86
			sErrorMessage$ + "EAX = " + Str(ErrorRegister(#PB_OnError_EAX)) + Chr(13) + Chr(10)
			sErrorMessage$ + "EBX = " + Str(ErrorRegister(#PB_OnError_EBX)) + Chr(13) + Chr(10)
			sErrorMessage$ + "ECX = " + Str(ErrorRegister(#PB_OnError_ECX)) + Chr(13) + Chr(10)
			sErrorMessage$ + "EDX = " + Str(ErrorRegister(#PB_OnError_EDX)) + Chr(13) + Chr(10)
			sErrorMessage$ + "EBP = " + Str(ErrorRegister(#PB_OnError_EBP)) + Chr(13) + Chr(10)
			sErrorMessage$ + "ESI = " + Str(ErrorRegister(#PB_OnError_ESI)) + Chr(13) + Chr(10)
			sErrorMessage$ + "EDI = " + Str(ErrorRegister(#PB_OnError_EDI)) + Chr(13) + Chr(10)
			sErrorMessage$ + "ESP = " + Str(ErrorRegister(#PB_OnError_ESP)) + Chr(13) + Chr(10)
		CompilerCase #PB_Processor_x64
			sErrorMessage$ + "RAX = " + Str(ErrorRegister(#PB_OnError_RAX)) + Chr(13) + Chr(10)
			sErrorMessage$ + "RBX = " + Str(ErrorRegister(#PB_OnError_RBX)) + Chr(13) + Chr(10)
			sErrorMessage$ + "RCX = " + Str(ErrorRegister(#PB_OnError_RCX)) + Chr(13) + Chr(10)
			sErrorMessage$ + "RDX = " + Str(ErrorRegister(#PB_OnError_RDX)) + Chr(13) + Chr(10)
			sErrorMessage$ + "RBP = " + Str(ErrorRegister(#PB_OnError_RBP)) + Chr(13) + Chr(10)
			sErrorMessage$ + "RSI = " + Str(ErrorRegister(#PB_OnError_RSI)) + Chr(13) + Chr(10)
			sErrorMessage$ + "RDI = " + Str(ErrorRegister(#PB_OnError_RDI)) + Chr(13) + Chr(10)
			sErrorMessage$ + "RSP = " + Str(ErrorRegister(#PB_OnError_RSP)) + Chr(13) + Chr(10)
		CompilerCase #PB_Processor_PowerPC
			sErrorMessage$ + "r0 = " + Str(ErrorRegister(#PB_OnError_r0)) + Chr(13) + Chr(10)
			sErrorMessage$ + "r1 = " + Str(ErrorRegister(#PB_OnError_r1)) + Chr(13) + Chr(10)
			sErrorMessage$ + "r2 = " + Str(ErrorRegister(#PB_OnError_r2)) + Chr(13) + Chr(10)
			sErrorMessage$ + "r3 = " + Str(ErrorRegister(#PB_OnError_r3)) + Chr(13) + Chr(10)
			sErrorMessage$ + "r4 = " + Str(ErrorRegister(#PB_OnError_r4)) + Chr(13) + Chr(10)
			sErrorMessage$ + "r5 = " + Str(ErrorRegister(#PB_OnError_r5)) + Chr(13) + Chr(10)
			sErrorMessage$ + "r6 = " + Str(ErrorRegister(#PB_OnError_r6)) + Chr(13) + Chr(10)
			sErrorMessage$ + "r7 = " + Str(ErrorRegister(#PB_OnError_r7)) + Chr(13) + Chr(10)
	CompilerEndSelect
	sErrorMessage$ + Chr(13) + Chr(10)
	sErrorMessage$ + "Please send this error message to the developer of this program (" + LCase(#InternName) + "@selfcoders.com)." + Chr(13) + Chr(10)
	sErrorMessage$ + "Describe the steps to reproduce the error so it is possible to fix this problem."
	lDoRestart = MessageRequester(#Title, sErrorMessage$ + Chr(13) + Chr(13) + "Do you want to restart the program?", #MB_YESNO | #MB_ICONWARNING)
	sFilename$ = GetTemporaryDirectory() + #InternName + " Crash Message.txt"
	lFile = CreateFile(#PB_Any, sFilename$)
	If IsFile(lFile)
		WriteString(lFile, sErrorMessage$)
		CloseFile(lFile)
		RunProgram("notepad", sFilename$, "")
	EndIf
	RemoveSysTrayIconEx(WindowID(#Window), #Tray)
	If lDoRestart = #PB_MessageRequester_Yes
		RunProgram(ProgramFilename())
	EndIf
	End
EndProcedure
; IDE Options = PureBasic 4.60 (Windows - x86)
; Folding = --------
; EnableXP
; EnableCompileCount = 0
; EnableBuildCount = 0
; EnableExeConstant