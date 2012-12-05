; Build target "Release" using Visual Studio before building setup!

#define MyAppName "Capture2Net"
#define MyAppVersion "2.0"
#define MyAppPublisher "SelfCoders"
#define MyAppURL "http://www.selfcoders.com/projects/capture2net"
#define MyAppExeName "Capture2Net.exe"

[Setup]
AppId={{578D140E-5FD1-48B4-957E-4EDD7833FB13}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL="http://www.selfcoders.com"
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\SelfCoders\Capture2Net
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
InfoBeforeFile=Readme-BeforeSetup.txt
OutputBaseFilename=Capture2Net_Setup
Compression=lzma
SolidCompression=yes
AppMutex=Capture2Net_RunCheck

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "..\Capture2Net\bin\Release\Capture2Net.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Capture2Net\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"

[Tasks]
Name: startWithWindows; Description: "&Start with Windows"

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "Capture2Net"; ValueData: """{app}\Capture2Net.exe"""; Tasks:startWithWindows; Flags: uninsdeletevalue

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

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
	if not dotNetInstalled then begin
		MsgBox('Capture2Net requires .NET Framework 4.5 which is currently not installed.'#13#13'This installer will now open the download page of .NET Framework 4.5 in your browser.'#13#13'Download and install it and try to install Capture2Net again.', mbError, MB_OK);
		ShellExec('open', 'http://www.microsoft.com/download/details.aspx?id=30653', '', '', SW_SHOWNORMAL, ewNoWait, errorCode);
		result := false;
	end else
		result := true;
end;