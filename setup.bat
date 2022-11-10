@echo off
echo\
set /p required=Is docker running and .NET Runtime and SDK preinstalled? (y/n):
if %required%==n (echo\ && echo Please start Docker and make sure .NET Runtime and SDK are preinstalled. && exit)
echo\ && echo Don't forget to update the .env File (press any key to continue)
pause > nul
for /f "delims== tokens=1,2" %%G in (.env) do set %%G=%%H
echo\ && echo Create mbconfig.cfg File for OpenPLC
call createCfg.bat
echo\
echo\ && echo Starting Factory I/O ...
start "" %FACTORY_IO%
echo\ && echo Build and run Docker Container
docker-compose up --build -d
echo\ && echo Give Docker some time ...
timeout /t 10 /nobreak
echo\ && echo Open Workflow Engine
explorer "http://localhost:8081"
echo\ && echo Start Factory I/O SDK and Producer
cd factoryio/sdk
dotnet run