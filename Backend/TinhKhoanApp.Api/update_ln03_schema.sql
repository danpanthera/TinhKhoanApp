-- Thêm 3 cột mới cho LN03
USE TinhKhoanDB;
GO

-- Kiểm tra và thêm cột COLUMN_18
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LN03') AND name = 'COLUMN_18')
BEGIN
    ALTER TABLE LN03 ADD COLUMN_18 NVARCHAR(200) NULL;
    PRINT 'Added COLUMN_18 to LN03';
END

-- Kiểm tra và thêm cột COLUMN_19  
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LN03') AND name = 'COLUMN_19')
BEGIN
    ALTER TABLE LN03 ADD COLUMN_19 NVARCHAR(200) NULL;
    PRINT 'Added COLUMN_19 to LN03';
END

-- Kiểm tra và thêm cột COLUMN_20
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LN03') AND name = 'COLUMN_20')
BEGIN
    ALTER TABLE LN03 ADD COLUMN_20 DECIMAL(18,2) NULL;
    PRINT 'Added COLUMN_20 to LN03';
END

-- Kiểm tra số cột hiện tại
SELECT COUNT(*) AS ColumnCount FROM sys.columns WHERE object_id = OBJECT_ID('LN03');

PRINT 'LN03 schema update completed successfully!';
GO
