-- ========================================
-- 🔄 SCRIPT RENAME COLUMNS ĐỂ MATCH VỚI CSV HEADERS GỐC
-- ========================================

USE TinhKhoanDB;
GO

PRINT '🔄 Renaming columns to match original CSV headers...';

-- Kiểm tra trạng thái hiện tại
PRINT '📋 Current temporal table status:';
SELECT
    t.name AS TableName,
    t.temporal_type_desc AS TemporalType,
    h.name AS HistoryTableName
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.name IN ('7800_DT_KHKD1', 'DPDA', 'EI01', 'GAHR26', 'GLCB41')
ORDER BY t.name;

-- Lưu ý: Để rename cột trong temporal table, cần tắt system versioning trước
-- Ví dụ cho bảng GAHR26 (nếu cần rename cột)

/*
-- Tắt temporal versioning
ALTER TABLE [GAHR26] SET (SYSTEM_VERSIONING = OFF);

-- Rename columns để match CSV headers (ví dụ)
EXEC sp_rename 'GAHR26.MaNhanVien', 'EMP_ID', 'COLUMN';
EXEC sp_rename 'GAHR26.TenNhanVien', 'EMP_NAME', 'COLUMN';
EXEC sp_rename 'GAHR26.ChucVu', 'POSITION', 'COLUMN';
-- ... các cột khác

-- Bật lại temporal versioning
ALTER TABLE [GAHR26] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[GAHR26_History]));
*/

PRINT '📊 Note: Column renaming trong temporal tables đòi hỏi:';
PRINT '1. Tắt SYSTEM_VERSIONING';
PRINT '2. Rename columns trong cả main table và history table';
PRINT '3. Bật lại SYSTEM_VERSIONING';
PRINT '4. Cập nhật import logic để map đúng tên cột';

-- Kiểm tra một số columns hiện tại
PRINT '🔍 Sample columns in existing tables:';

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GAHR26')
BEGIN
    SELECT TOP 5 COLUMN_NAME
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'GAHR26'
    AND COLUMN_NAME NOT IN ('Id', 'BusinessKey', 'EffectiveDate', 'ExpiryDate', 'IsCurrent', 'RowVersion', 'ImportId', 'StatementDate', 'ProcessedDate', 'DataHash', 'AdditionalData', 'SysStartTime', 'SysEndTime')
    ORDER BY ORDINAL_POSITION;
END

PRINT '✅ Để thực hiện full rename, cần list đầy đủ mapping CSV header → Database column';

GO
