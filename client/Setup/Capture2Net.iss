; Build target "Release" using Visual Studio before building setup!

#define ApplicationName "Capture2Net"
#define ProjectURL "http://www.selfcoders.com/projects/capture2net"
#define MainExecutable "Capture2Net.exe"
#define Version GetFileVersion("..\Capture2Net\bin\Release\Capture2Net.exe")

[Setup]
AppId={{578D140E-5FD1-48B4-957E-4EDD7833FB13}
AppName={#ApplicationName}
AppVersion={#Version}
AppPublisher="SelfCoders"
AppPublisherURL="http://www.selfcoders.com"
AppSupportURL={#ProjectURL}
AppUpdatesURL={#ProjectURL}
DefaultDirName={pf}\SelfCoders\Capture2Net
DefaultGroupName={#ApplicationName}
AllowNoIcons=yes
InfoBeforeFile=Readme-BeforeSetup.txt
OutputDir=.
OutputBaseFilename=Capture2Net_Setup
Compression=lzma
SolidCompression=yes
AppMutex=Capture2Net_RunCheck
UninstallDisplayIcon={app}\{#MainExecutable}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "..\Capture2Net\bin\Release\Capture2Net.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Capture2Net\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#ApplicationName}"; Filename: "{app}\{#MainExecutable}"
Name: "{group}\{cm:ProgramOnTheWeb,{#ApplicationName}}"; Filename: "{#ProjectURL}"
Name: "{group}\{cm:UninstallProgram,{#ApplicationName}}"; Filename: "{uninstallexe}"

[Tasks]
Name: resetConfiguration; Description: "Reset configuration"; Flags: checkedonce unchecked
Name: startWithWindows; Description: "&Start with Windows"

[Registry]
Root: HKCU; Subkey: "Software\SelfCoders\Capture2Net"; Tasks: resetConfiguration; Flags: deletekey
Root: HKCU; Subkey: "Software\SelfCoders\Capture2Net"; Flags: uninsdeletekeyifempty
Root: HKCU; Subkey: "Software\SelfCoders"; Flags: uninsdeletekeyifempty
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "Capture2Net"; ValueData: """{app}\{#MainExecutable}"""; Tasks:startWithWindows; Flags: uninsdeletevalue

[Run]
Filename: "{#ProjectURL}"; Description: "Show help"; Flags: nowait postinstall skipifsilent shellexec
Filename: "{app}\{#MainExecutable}"; Description: "Launch application"; Flags: nowait postinstall skipifsilent

[Code]
function InitializeSetup(): Boolean;
var
	errorCode: integer;
	key: string;
	release: cardinal;
	dotNetInstalled: boolean;
begin
	key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full';

	dotNetInstalled := RegQueryDWordValue(HKLM, key, 'Release', release);
	dotNetInstalled := dotNetInstalled and (release >= 378389);
	if not dotNetInstalled then
	begin
		MsgBox('Capture2Net requires .NET Framework 4.5 which is currently not installed.'#13#13'This installer will now open the download page of .NET Framework 4.5 in your browser.'#13#13'Download and install it and try to install Capture2Net again.', mbError, MB_OK);
		ShellExecAsOriginalUser('open', 'http://www.microsoft.com/download/details.aspx?id=30653', '', '', SW_SHOWNORMAL, ewNoWait, errorCode);
		result := false;
	end else
	begin
		result := true;
	end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
	if CurUninstallStep = usPostUninstall then
	begin
		if RegKeyExists(HKEY_CURRENT_USER, 'Software\SelfCoders\Capture2Net') then
		begin
			if MsgBox('Do you want to remove your Capture2Net configuration?', mbConfirmation, MB_YESNO) = IDYES then
			begin
				RegDeleteKeyIncludingSubkeys(HKEY_CURRENT_USER, 'Software\SelfCoders\Capture2Net');
				RegDeleteKeyIfEmpty(HKEY_CURRENT_USER, 'Software\SelfCoders');
			end;
		end;
	end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
	isSilent, isUpdate: boolean;
	errorCode, index: integer;
begin
	if CurStep = ssDone then
	begin
		for index:=1 to ParamCount do
		begin
			if uppercase(ParamStr(index))='/SILENT' then
			begin
				isSilent := true;
			end;
			if uppercase(ParamStr(index))='/VERYSILENT' then
			begin
				isSilent := true;
			end;
			if uppercase(ParamStr(index))='/UPDATE' then
			begin
				isUpdate := true;
			end;
		end;
		if isSilent and isUpdate then
		begin
			ExecAsOriginalUser(ExpandConstant('{app}\{#MainExecutable}'), '/updated', '', SW_SHOWNORMAL, ewNoWait, errorCode);
		end;
	end;
end;