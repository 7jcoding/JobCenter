@echo.服务启动......  
@echo off  
@net stop JobCenter.Server$JobCenter.Server
@net start JobCenter.Server$JobCenter.Server
@echo off  
@echo.启动完毕！  
@pause  