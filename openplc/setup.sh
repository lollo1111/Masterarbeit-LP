#!/bin/bash
SQL_SCRIPT="INSERT INTO Programs (Name, Description, File, Date_upload) VALUES ('Factory I/O Function Block Diagram', 'PLC Program for the Simulation', 'testprogramm.st', strftime('%s', 'now'));"
SQL_DEVICE="INSERT INTO Slave_dev (dev_name, dev_type, slave_id, ip_address, ip_port, di_start, di_size, coil_start, coil_size, ir_start, ir_size, hr_read_start, hr_read_size, hr_write_start, hr_write_size) VALUES ('FactoryIO', 'TCP', 1, '${MODBUS_SERVER}', ${MODBUS_PORT}, 0, 128, 0, 128, 0, 32, 0, 32, 0, 32);"
# SQL_DEVICE="INSERT INTO Slave_dev (dev_name, dev_type, slave_id, ip_address, ip_port, di_start, di_size, coil_start, coil_size, ir_start, ir_size, hr_read_start, hr_read_size, hr_write_start, hr_write_size) VALUES ('FactoryIO', 'TCP', 1, '${MODBUS_SERVER}', ${MODBUS_PORT}, 0, 16, 0, 16, 0, 8, 0, 8, 0, 8);"
SQL_DNP="UPDATE Settings SET Value = 'disabled' WHERE Key = 'Dnp3_port';"
SQL_ENIP="UPDATE Settings SET Value = 'disabled' WHERE Key = 'Enip_port'"
SQL_DELETE="DELETE FROM Programs WHERE Name = 'Blank Program';"
PROG_FILE="testprogramm.st"
SQL_AUTOST="UPDATE Settings SET Value = 'true' WHERE Key = 'Start_run_mode';"

sqlite3 /workdir/OpenPLC_v3/webserver/openplc.db "$SQL_SCRIPT"
cd /workdir/OpenPLC_v3/webserver
/workdir/OpenPLC_v3/webserver/scripts/compile_program.sh  $PROG_FILE
sqlite3 /workdir/OpenPLC_v3/webserver/openplc.db "$SQL_DEVICE"
sqlite3 /workdir/OpenPLC_v3/webserver/openplc.db "$SQL_DELETE"

sqlite3 /workdir/OpenPLC_v3/webserver/openplc.db "$SQL_DNP"
sqlite3 /workdir/OpenPLC_v3/webserver/openplc.db "$SQL_ENIP"
sqlite3 /workdir/OpenPLC_v3/webserver/openplc.db "$SQL_AUTOST"
cd /workdir/OpenPLC_v3
/workdir/OpenPLC_v3/webserver/scripts/start_openplc.sh