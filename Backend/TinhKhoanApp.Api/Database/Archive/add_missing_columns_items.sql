-- Script để thêm các cột còn thiếu cho table ImportedDataItems

-- Kiểm tra và thêm cột SysStartTime nếu chưa có cho ImportedDataItems
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ImportedDataItems') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE ImportedDataItems ADD SysStartTime datetime2 DEFAULT GETDATE();
    PRINT 'Đã thêm cột SysStartTime cho ImportedDataItems';
END
ELSE
BEGIN
    PRINT 'Cột SysStartTime đã tồn tại cho ImportedDataItems';
END

-- Kiểm tra và thêm cột SysEndTime nếu chưa có cho ImportedDataItems
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ImportedDataItems') AND name = 'SysEndTime')
BEGIN
    ALTER TABLE ImportedDataItems ADD SysEndTime datetime2 DEFAULT '9999-12-31 23:59:59.9999999';
    PRINT 'Đã thêm cột SysEndTime cho ImportedDataItems';
END
ELSE
BEGIN
    PRINT 'Cột SysEndTime đã tồn tại cho ImportedDataItems';
END

PRINT 'Hoàn thành việc thêm các cột còn thiếu cho ImportedDataItems';
