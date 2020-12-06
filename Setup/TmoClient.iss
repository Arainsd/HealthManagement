; �ű��� Inno Setup �ű��� ���ɣ�
; �йش��� Inno Setup �ű��ļ�����ϸ��������İ����ĵ���

#define MyAppName "��������"
#define MyAppVersion "1.3.7644.41950"
#define MyAppPublisher "��������"
#define MyAppExeName "TmoClient.exe"
#define FileDir "..\1-TmoClient\TmoClient\bin\x86\Debug\"
#define SetupDir "..\Setup\"

[Setup]
; ע: AppId��ֵΪ������ʶ��Ӧ�ó���
; ��ҪΪ������װ����ʹ����ͬ��AppIdֵ��
; (�����µ�GUID����� ����|��IDE������GUID��)
AppId={{F22C2D8D-ABD4-40C8-B643-C2272F2EB83D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\TmoClient
DefaultGroupName={#MyAppName}
OutputDir={#SetupDir}Setup
OutputBaseFilename=TmoClientSetup
SetupIconFile={#SetupDir}ClientSetup.ico
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
Source: "{#FileDir}TmoClient.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}TmoClient.exe.config"; DestDir: "{app}"; Flags: ignoreversion onlyifdoesntexist
Source: "{#FileDir}DBModel.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}EnvLib.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}log4net.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}SyncDataLib.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}TmoCommon.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}TmoControl.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}TmoEvaluation.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}TmoExtendServer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}TmoGeneral.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}TmoLinkServer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}TmoOpinion.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}TmoPointsCenter.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}TmoProject.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}TmoPurchaseSellStock.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}TmoQuestionnaire.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}TmoReport.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}TmoSkin.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}TmoWeb.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#FileDir}Update.exe"; DestDir: "{app}"; Flags: ignoreversion
; ע��: ��Ҫ���κι���ϵͳ�ļ���ʹ�á�Flags: ignoreversion��

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "{app}\Update.exe"; Description: "��ʼ������"; StatusMsg: "����������л���"
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent runascurrentuser

[code]
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var appWnd:HWND;
begin
  appWnd:= FindWindowByWindowName('logoForm');
  if(appWnd<>0) then
    PostMessage(appWnd,18,0,0);
  appWnd:= FindWindowByWindowName('��ӭʹ��-���¼');
  if(appWnd<>0) then
    PostMessage(appWnd,18,0,0);
  appWnd:= FindWindowByWindowName('���������ͻ���');
  if(appWnd<>0) then
    PostMessage(appWnd,18,0,0);
  if CurUninstallStep = usDone then
    DelTree(ExpandConstant('{app}'), True, True, True);
end;
