; �ű��� Inno Setup �ű��� ���ɣ�
; �йش��� Inno Setup �ű��ļ�����ϸ��������İ����ĵ���

#define MyAppName "������������"
#define MyAppVersion "1.3.7649.33821"
#define MyAppPublisher "��������"
#define MyAppExeName "TmoServer.exe"
#define FileDir "..\2-TmoServer\TmoServer\bin\Debug\"
#define SetupDir "..\Setup\"

[Setup]
; ע: AppId��ֵΪ������ʶ��Ӧ�ó���
; ��ҪΪ������װ����ʹ����ͬ��AppIdֵ��
; (�����µ�GUID����� ����|��IDE������GUID��)
AppId={{8EC1B976-231B-4699-86CE-BCABB52125A6}
AppName={#MyAppName}
AppVersion={#MyAppVersion};AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\TmoServer
DefaultGroupName={#MyAppName}
OutputDir={#SetupDir}Setup
OutputBaseFilename=TmoServerSetup
SetupIconFile={#SetupDir}ServerSetup.ico
Compression=lzma
SolidCompression=yes

DisableStartupPrompt=yes
DisableWelcomePage=no
DisableDirPage=no
DisableProgramGroupPage=no
DisableReadyPage=no
DisableFinishedPage=no

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "{#FileDir}*.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}*.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}TmoServer.exe.config"; DestDir: "{app}"; Flags: ignoreversion onlyifdoesntexist
; ע��: ��Ҫ���κι���ϵͳ�ļ���ʹ�á�Flags: ignoreversion��

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent runascurrentuser

[code]
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var appWnd:HWND;
begin  appWnd:= FindWindowByWindowName('Tmo������');
  if(appWnd<>0) then
    PostMessage(appWnd,18,0,0);
  if CurUninstallStep = usDone then
    DelTree(ExpandConstant('{app}'), True, True, True);
end;