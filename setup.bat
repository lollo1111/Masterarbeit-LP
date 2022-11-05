@echo off 
set /p vpn=Connected with vpn.univie.ac.at? (y/n):
if %vpn%==n (echo\ && echo Please connect with the VPN. && exit)
echo\
set /p required=Is docker running and .NET Runtime and SDK preinstalled? (y/n):
if %required%==n (echo\ && echo Please start Docker and make sure .NET Runtime and SDK are preinstalled. && exit)
echo\
echo Open FactoryI/O and if necessary, update the IP in the Repository (press any key to continue)
pause > nul
echo\ && echo Build and run Docker Container
docker-compose up --build -d
echo\ && echo Important: Open http://localhost:8080, upload the .st Program and start the PLC
echo\ && echo Start Factory I/O SDK and Producer
cd factoryio/sdk
dotnet run