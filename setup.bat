@echo off
cls
set required=""
set down=""
set /p required=Wurde Docker gestartet sowie .NET Runtime und SDK vorinstalliert? (beliebige Taste/n):
if "%required%"=="" (set "required=y")
if %required%==n (echo Starte Docker und installiere .NET Runtime sowie SDK && exit)
echo\ && echo Vergiss nicht das .env File zu aktualisieren (beliebige Taste zum Fortsetzen)
pause > nul
for /f "delims== tokens=1,2" %%G in (.env) do set %%G=%%H
echo\ && echo OpenPLC mbconfig.cfg File wird erstellt.
(
echo Num_Devices = "1"
echo Polling_Period = "100"
echo Timeout = "1000"
echo # ------------
echo #   DEVICE 0
echo # ------------
echo device0.name = "FactoryIO"
echo device0.slave_id = "1"
echo device0.protocol = "TCP"
echo device0.address = "%MODBUS_SERVER%"
echo device0.IP_Port = "%MODBUS_PORT%"
echo device0.RTU_Baud_Rate = "None"
echo device0.RTU_Parity = "None"
echo device0.RTU_Data_Bits = "None"
echo device0.RTU_Stop_Bits = "None"
echo device0.RTU_TX_Pause = "None"
echo\
echo device0.Discrete_Inputs_Start = "0"
echo device0.Discrete_Inputs_Size = "128"
echo device0.Coils_Start = "0"
echo device0.Coils_Size = "128"
echo device0.Input_Registers_Start = "0"
echo device0.Input_Registers_Size = "32"
echo device0.Holding_Registers_Read_Start = "0"
echo device0.Holding_Registers_Read_Size = "32"
echo device0.Holding_Registers_Start = "0"
echo device0.Holding_Registers_Size = "32"
) > openplc/mbconfig.cfg
echo\ && echo Factory I/O wird gestartet ...
start "" %FACTORY_IO%
echo\ && echo Docker Container wird erstellt ...
docker-compose up --build -d
echo\ && echo Docker Container wird gestartet ...
timeout /t 10 /nobreak
explorer "http://localhost"
cd factoryio/sdk
cls
echo Web Anwendung des digitalen Zwillings: ^<http://localhost/^>
echo SDK wird gestartet ... (beliebige Taste zum Beenden klicken, nicht mit Ctr + C^)
dotnet run
cd %~dp0
set /p down=Docker Container beenden sowie Volumes entfernen? (y/beliebige Taste):
if %down%==y (docker-compose down -v)
echo SDK wurde beendet.