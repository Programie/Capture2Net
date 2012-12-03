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
OutputBaseFilename=Setup_Capture2Net
Compression=lzma
SolidCompression=yes
AppMutex=Capture2Net_RunCheck

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "..\Capture2Net\bin\Release\Capture2Net.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Capture2Net\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

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