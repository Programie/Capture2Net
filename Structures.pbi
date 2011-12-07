Structure eConfigValues
	sKey.s
	sValue.s
EndStructure

Structure eConfigGadgets
	bIsString.b
	lGadget.l
	sFieldName.s
EndStructure

Structure eGlobals
	bQuit.b
	bStopCheckHistoryUrls.b
	hAppIcon.l
	lTaskBarWindowMessage.l
	lMutex.l
	lLatestBuild.l
	sConfigFile.s
EndStructure

Structure eIconData
	cbSize.l
	hwnd.l
	uID.l
	uFlags.l
	uCallbackMessage.l
	hIcon.l
	szTip.b[128]
	dwState.l
	dwStateMask.l
	szInfo.b[256]
	StructureUnion
		uTimeout.l
		uVersion.l
	EndStructureUnion
	szInfoTitle.b[64]
	dwInfoFlags.l
EndStructure

Structure eQRCode
	Version.l
	Width.l
	pSymbolData.l
EndStructure

Structure eUploadQueue
	lItem.l
	sFilename.s
EndStructure
; IDE Options = PureBasic 4.60 (Windows - x86)
; CursorPosition = 17
; EnableXP
; EnableCompileCount = 0
; EnableBuildCount = 0
; EnableExeConstant