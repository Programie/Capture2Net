; Build target "Release" using Visual Studio before building setup!

#include "itdownload.iss"

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
UninstallDisplayName={#ApplicationName}

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
Name: installDotNetFramework45; Description: "Install .NET Framework 4.5"
Name: resetConfiguration; Description: "Reset configuration"; Flags: checkedonce unchecked
Name: startWithWindows; Description: "Start with Windows"

[Registry]
Root: HKCU; Subkey: "Software\SelfCoders\Capture2Net"; Tasks: resetConfiguration; Flags: deletekey
Root: HKCU; Subkey: "Software\SelfCoders\Capture2Net"; Flags: uninsdeletekeyifempty
Root: HKCU; Subkey: "Software\SelfCoders"; Flags: uninsdeletekeyifempty
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "Capture2Net"; ValueData: """{app}\{#MainExecutable}"""; Tasks:startWithWindows; Flags: uninsdeletevalue

[Run]
Filename: "{#ProjectURL}"; Description: "Show help"; Flags: nowait postinstall skipifsilent shellexec
Filename: "{app}\{#MainExecutable}"; Description: "Launch application"; Flags: nowait postinstall skipifsilent


[Code]
var
	dotNetInstalled: boolean;

function CheckPendingReboot(): boolean;
var
	names: String;
begin
	if (RegQueryMultiStringValue(HKEY_LOCAL_MACHINE, 'SYSTEM\CurrentControlSet\Control\Session Manager', 'PendingFileRenameOperations', names)) then
	begin
		result := true;
	end else
	begin
		if ((RegQueryMultiStringValue(HKEY_LOCAL_MACHINE, 'SYSTEM\CurrentControlSet\Control\Session Manager', 'SetupExecute', names)) and (names <> '')) then
		begin
			result := true;
		end else
		begin
			result := false;
		end;
	end;
end;

procedure InitializeWizard();
begin
	if not dotNetInstalled then
	begin
		itd_init();
		itd_addfile('http://download.microsoft.com/download/B/A/4/BA4A7E71-2906-4B2D-A0E1-80CF16844F5F/dotNetFx45_Full_setup.exe', ExpandConstant('{tmp}\dotNetFx45_Full_setup.exe'));
		itd_downloadafter(wpReady);
		MsgBox('Capture2Net requires .NET Framework 4.5 which is currently not installed.'#13#13'This installer will download and install it automatically while installing Capture2Net.', mbError, MB_OK);
	end;
end;

function InitializeSetup(): boolean;
var
	release: cardinal;
begin
	dotNetInstalled := RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', release);
	if dotNetInstalled and (release >= 378389) then
	begin
		dotNetInstalled := true;
	end;
	if not dotNetInstalled and CheckPendingReboot() then
	begin
		MsgBox('A reboot is currently pending.'#13#13'Please reboot your computer before continuing!', mbError, MB_OK);
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

procedure CurPageChanged(CurPageID: Integer);
begin
	if CurPageID = wpSelectTasks then
	begin
		WizardForm.TasksList.Checked[0] := not dotNetInstalled;
		WizardForm.TasksList.ItemEnabled[0] := false;
	end;
end;
procedure CurStepChanged(CurStep: TSetupStep);
var
	isSilent, isUpdate: boolean;
	errorCode, index: integer;
begin
	if CurStep = ssInstall then
	begin
		if not dotNetInstalled then
		begin
			Exec(ExpandConstant('{tmp}\dotNetFx45_Full_setup.exe'), '', '', SW_SHOWNORMAL, ewWaitUntilTerminated, errorCode);
		end;
	end else
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
end;