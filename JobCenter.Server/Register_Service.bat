@echo off
@echo ��װ����... 
%~dp0\JobCenter.Server.exe  -instance "JobCenter.Server"  -servicename "JobCenter.Server" -description "������ȷ���������" -displayname "JobCenter.Server"  install 

@echo ��װ���.
@pause