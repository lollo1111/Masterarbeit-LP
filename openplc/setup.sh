#!/bin/bash

SQL_DEVICE="INSERT INTO Slave_dev (dev_name, dev_type, slave_id, ip_address, ip_port, di_start, di_size, coil_start, coil_size, ir_start, ir_size, hr_read_start, hr_read_size, hr_write_start, hr_write_size) VALUES ('FactoryIO', 'TCP', 1, '127.0.0.1', 503, 0, 8, 0, 8, 0, 8, 0, 8, 0, 8);"
sqlite3 /workdir/OpenPLC_v3/webserver/openplc.db "$SQL_DEVICE"
/workdir/OpenPLC_v3/webserver/scripts/start_openplc.sh