-- Attach TinhKhoanDB if not exists
USE master;
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TinhKhoanDB')
BEGIN
    CREATE DATABASE TinhKhoanDB ON 
    (FILENAME = '/var/opt/mssql/data/TinhKhoanDB.mdf'),
    (FILENAME = '/var/opt/mssql/data/TinhKhoanDB_log.ldf')
    FOR ATTACH;
    PRINT 'TinhKhoanDB attached successfully';
END
ELSE
BEGIN
    PRINT 'TinhKhoanDB already exists';
END
GO