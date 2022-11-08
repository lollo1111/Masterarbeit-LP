@echo off
for /f "delims== tokens=1,2" %%G in (.env) do set %%G=%%H
set /p vpn=Connected with vpn.univie.ac.at? (y/n):
if %vpn%==n (echo\ && echo Please connect with the VPN. && exit)
echo\
set /p required=Is docker running and .NET Runtime and SDK preinstalled? (y/n):
if %required%==n (echo\ && echo Please start Docker and make sure .NET Runtime and SDK are preinstalled. && exit)
echo\
echo Don't forget to upadte the MODBUS_SERVER IP in the .env File (press any key to continue)
pause > nul
echo\ && echo Starting Factory I/O ...
start "" %FACTORY_IO%
echo\ && echo Build and run Docker Container
docker-compose up --build -d
echo\ && echo Open Workflow Engine
explorer "https://cpee.org/flow/?monitor=https://cpee.org/flow/engine/5768/"
echo\ && echo Start Factory I/O SDK and Producer
cd factoryio/sdk
dotnet run