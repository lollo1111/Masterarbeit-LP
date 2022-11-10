@echo off
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
echo device0.Discrete_Inputs_Size = "16"
echo device0.Coils_Start = "0"
echo device0.Coils_Size = "16"
echo device0.Input_Registers_Start = "0"
echo device0.Input_Registers_Size = "8"
echo device0.Holding_Registers_Read_Start = "0"
echo device0.Holding_Registers_Read_Size = "8"
echo device0.Holding_Registers_Start = "0"
echo device0.Holding_Registers_Size = "8"
) > openplc/mbconfig.cfg