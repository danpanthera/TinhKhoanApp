-- Script để thêm các cột còn thiếu cho table ImportedDataRecords

-- Kiểm tra và thêm cột CompressedData nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ImportedDataRecords') AND name = 'CompressedData')
BEGIN
    ALTER TABLE ImportedDataRecords ADD CompressedData varbinary(max) NULL;
    PRINT 'Đã thêm cột CompressedData';
END
ELSE
BEGIN
    PRINT 'Cột CompressedData đã tồn tại';
END

-- Kiểm tra và thêm cột CompressionRatio nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ImportedDataRecords') AND name = 'CompressionRatio')
BEGIN
    ALTER TABLE ImportedDataRecords ADD CompressionRatio float DEFAULT 0.0;
    PRINT 'Đã thêm cột CompressionRatio';
END
ELSE
BEGIN
    PRINT 'Cột CompressionRatio đã tồn tại';
END

-- Kiểm tra và thêm cột SysStartTime nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ImportedDataRecords') AND name = 'SysStartTime')
BEGIN
    ALTER TABLE ImportedDataRecords ADD SysStartTime datetime2 DEFAULT GETDATE();
    PRINT 'Đã thêm cột SysStartTime';
END
ELSE
BEGIN
    PRINT 'Cột SysStartTime đã tồn tại';
END

-- Kiểm tra và thêm cột SysEndTime nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ImportedDataRecords') AND name = 'SysEndTime')
BEGIN
    ALTER TABLE ImportedDataRecords ADD SysEndTime datetime2 DEFAULT '9999-12-31 23:59:59.9999999';
    PRINT 'Đã thêm cột SysEndTime';
END
ELSE
BEGIN
    PRINT 'Cột SysEndTime đã tồn tại';
END

PRINT 'Hoàn thành việc thêm các cột còn thiếu cho ImportedDataRecords';
