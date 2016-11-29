@echo off
@echo 安装服务... 
%~dp0\JobCenter.Server.exe  -instance "JobCenter.Server"  -servicename "JobCenter.Server" -description "任务调度服务处理中心" -displayname "JobCenter.Server"  install 

@echo 安装完成.
@pause