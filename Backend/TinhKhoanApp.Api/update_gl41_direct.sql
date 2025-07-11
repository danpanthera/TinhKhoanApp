-- Cập nhật bảng GL41 trực tiếp để match với CSV structure (13 columns)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GL41') AND name = 'SO_TK')
BEGIN
    EXEC sp_rename 'GL41.SO_TK', 'MA_TK', 'COLUMN'
    PRINT 'Renamed SO_TK to MA_TK'
END

-- Thêm cột LOAI_TIEN nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GL41') AND name = 'LOAI_TIEN')
BEGIN
    ALTER TABLE GL41 ADD LOAI_TIEN NVARCHAR(50)
    PRINT 'Added LOAI_TIEN column'
END

-- Thêm cột LOAI_BT nếu chưa có  
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GL41') AND name = 'LOAI_BT')
BEGIN
    ALTER TABLE GL41 ADD LOAI_BT NVARCHAR(50)
    PRINT 'Added LOAI_BT column'
END

-- Kiểm tra và update data type của NgayDL nếu cần
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'GL41' AND COLUMN_NAME = 'NgayDL' 
           AND DATA_TYPE = 'datetime2')
BEGIN
    -- Tạo cột tạm thời
    ALTER TABLE GL41 ADD NgayDL_temp NVARCHAR(50)
    
    -- Copy dữ liệu sang format string
    UPDATE GL41 SET NgayDL_temp = FORMAT(NgayDL, 'yyyyMMdd') WHERE NgayDL IS NOT NULL
    
    -- Drop cột cũ
    ALTER TABLE GL41 DROP COLUMN NgayDL
    
    -- Rename cột mới
    EXEC sp_rename 'GL41.NgayDL_temp', 'NgayDL', 'COLUMN'
    
    PRINT 'Converted NgayDL from datetime to string'
END

-- Verify kết quả
SELECT 
    'GL41 Column Verification' as TableCheck,
    COUNT(*) as TotalColumns
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
