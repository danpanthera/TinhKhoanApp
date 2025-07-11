-- Kiểm tra và rename SO_TK thành MA_TK
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GL41') AND name = 'SO_TK')
BEGIN
    EXEC sp_rename 'GL41.SO_TK', 'MA_TK', 'COLUMN'
    PRINT 'Renamed SO_TK to MA_TK'
END
ELSE
    PRINT 'SO_TK column not found or already renamed'

-- Thêm LOAI_TIEN column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GL41') AND name = 'LOAI_TIEN')
BEGIN
    ALTER TABLE GL41 ADD LOAI_TIEN NVARCHAR(50)
    PRINT 'Added LOAI_TIEN column'
END
ELSE
    PRINT 'LOAI_TIEN column already exists'

-- Thêm LOAI_BT column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GL41') AND name = 'LOAI_BT')
BEGIN
    ALTER TABLE GL41 ADD LOAI_BT NVARCHAR(50)
    PRINT 'Added LOAI_BT column'
END
ELSE
    PRINT 'LOAI_BT column already exists'

-- Verify kết quả
SELECT 
    'GL41 Column Count' as CheckType,
    COUNT(*) as ColumnCount
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'GL41'

SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'GL41'
ORDER BY ORDINAL_POSITION
