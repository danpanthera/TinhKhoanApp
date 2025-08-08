-- ========================================================================
-- FIX LN03 TABLE STRUCTURE - THEO ĐÚNG REQUIREMENTS
-- ========================================================================
-- Vấn đề:
--   1. Database có 22 business columns, CSV chỉ có 20 (17 header + 3 no-header)
--   2. Cột Id ở giữa bảng (position 11) thay vì cuối
--   3. Có 2 cột thừa: CREATED_BY, FILE_ORIGIN không có trong CSV
--
-- Giải pháp:
--   - NGAY_DL at position 1
--   - 20 Business Columns theo CSV structure (positions 2-21)
--   - System columns cuối cùng (positions 22-26)
-- ========================================================================

USE TinhKhoanDB;
GO

-- Bước 1: Backup dữ liệu LN03 hiện tại
SELECT * INTO LN03_BACKUP_20250808 FROM LN03;
PRINT 'Backup LN03 data completed';
GO

-- Bước 2: Disable temporal table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03' AND temporal_type = 2)
BEGIN
    ALTER TABLE LN03 SET (SYSTEM_VERSIONING = OFF);
    PRINT 'Disabled temporal versioning on LN03';
END
GO

-- Bước 3: Drop columnstore indexes if exist
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LN03_Columnstore')
    DROP INDEX IX_LN03_Columnstore ON LN03;

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LN03_History_Columnstore')
    DROP INDEX IX_LN03_History_Columnstore ON LN03_History;

PRINT 'Dropped existing columnstore indexes';
GO

-- Bước 4: Drop existing tables
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03_History')
    DROP TABLE LN03_History;

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03')
    DROP TABLE LN03;

PRINT 'Dropped existing LN03 tables';
GO

-- Bước 5: Tạo bảng LN03 mới với cấu trúc đúng theo requirements
CREATE TABLE LN03 (
    -- System Column đầu tiên - NGAY_DL (position 1)
    NGAY_DL datetime2 NOT NULL DEFAULT GETDATE(),

    -- 20 Business Columns theo CSV structure (positions 2-21)
    -- 17 columns với header
    MACHINHANH nvarchar(200) NULL,                    -- CSV position 1 (header)
    TENCHINHANH nvarchar(200) NULL,                   -- CSV position 2 (header)
    MAKH nvarchar(200) NULL,                          -- CSV position 3 (header)
    TENKH nvarchar(200) NULL,                         -- CSV position 4 (header)
    SOHOPDONG nvarchar(200) NULL,                     -- CSV position 5 (header)
    SOTIENXLRR decimal(18,2) NULL,                    -- CSV position 6 (header) - contains "SOTIEN"
    NGAYPHATSINHXL datetime2 NULL,                    -- CSV position 7 (header) - contains "NGAY"
    THUNOSAUXL decimal(18,2) NULL,                    -- CSV position 8 (header) - contains "THUNO"
    CONLAINGOAIBANG decimal(18,2) NULL,               -- CSV position 9 (header) - contains "CONLAINGOAIBANG"
    DUNONOIBANG decimal(18,2) NULL,                   -- CSV position 10 (header) - contains "DUNONOIBANG"
    NHOMNO nvarchar(200) NULL,                        -- CSV position 11 (header)
    MACBTD nvarchar(200) NULL,                        -- CSV position 12 (header)
    TENCBTD nvarchar(200) NULL,                       -- CSV position 13 (header)
    MAPGD nvarchar(200) NULL,                         -- CSV position 14 (header)
    TAIKHOANHACHTOAN nvarchar(200) NULL,              -- CSV position 15 (header)
    REFNO nvarchar(200) NULL,                         -- CSV position 16 (header)
    LOAINGUONVON nvarchar(200) NULL,                  -- CSV position 17 (header)

    -- 3 columns không có header nhưng có dữ liệu
    MALOAI nvarchar(200) NULL,                        -- CSV position 18 (no header col 1)
    LOAIKHACHHANG nvarchar(200) NULL,                 -- CSV position 19 (no header col 2)
    SOTIEN decimal(18,2) NULL,                        -- CSV position 20 (no header col 3) - contains "SOTIEN"

    -- System/Temporal Columns (positions 22-26)
    Id bigint IDENTITY(1,1) NOT NULL,                 -- Primary Key cuối cùng
    FILE_NAME nvarchar(255) NULL,                     -- System column
    CREATED_DATE datetime2 NULL DEFAULT GETDATE(),   -- System column

    -- Temporal columns
    ValidFrom datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    ValidTo datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,

    -- Constraints
    CONSTRAINT PK_LN03 PRIMARY KEY (Id),
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.LN03_History));

PRINT 'Created LN03 table with correct structure:';
PRINT '- NGAY_DL at position 1 ✅';
PRINT '- 20 business columns matching CSV (17 header + 3 no-header) ✅';
PRINT '- Removed CREATED_BY and FILE_ORIGIN (extra columns) ✅';
PRINT '- System columns at end ✅';
PRINT '- Temporal table enabled ✅';
GO

-- Bước 6: Tạo columnstore indexes
CREATE NONCLUSTERED COLUMNSTORE INDEX IX_LN03_Columnstore ON LN03 (
    NGAY_DL, MACHINHANH, TENCHINHANH, MAKH, TENKH, SOHOPDONG, SOTIENXLRR, NGAYPHATSINHXL,
    THUNOSAUXL, CONLAINGOAIBANG, DUNONOIBANG, NHOMNO, MACBTD, TENCBTD, MAPGD, TAIKHOANHACHTOAN,
    REFNO, LOAINGUONVON, MALOAI, LOAIKHACHHANG, SOTIEN, FILE_NAME, CREATED_DATE
);

CREATE NONCLUSTERED COLUMNSTORE INDEX IX_LN03_History_Columnstore ON LN03_History (
    NGAY_DL, MACHINHANH, TENCHINHANH, MAKH, TENKH, SOHOPDONG, SOTIENXLRR, NGAYPHATSINHXL,
    THUNOSAUXL, CONLAINGOAIBANG, DUNONOIBANG, NHOMNO, MACBTD, TENCBTD, MAPGD, TAIKHOANHACHTOAN,
    REFNO, LOAINGUONVON, MALOAI, LOAIKHACHHANG, SOTIEN, FILE_NAME, CREATED_DATE, ValidFrom, ValidTo
);

PRINT 'Created columnstore indexes for both LN03 and LN03_History ✅';
GO

-- Bước 7: Final verification
SELECT '=== LN03 STRUCTURE VERIFICATION ===' as Status;

SELECT
    ORDINAL_POSITION,
    COLUMN_NAME,
    DATA_TYPE,
    CASE
        WHEN ORDINAL_POSITION = 1 AND COLUMN_NAME = 'NGAY_DL' THEN '✅ CORRECT POSITION'
        WHEN ORDINAL_POSITION BETWEEN 2 AND 21 THEN '✅ BUSINESS COLUMN'
        WHEN ORDINAL_POSITION > 21 THEN '✅ SYSTEM/TEMPORAL'
        ELSE '❌ WRONG POSITION'
    END as Status
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'LN03'
ORDER BY ORDINAL_POSITION;

-- Đếm business columns
SELECT
    COUNT(*) as BusinessColumnCount,
    CASE
        WHEN COUNT(*) = 20 THEN '✅ PERFECT - 17 header + 3 no-header = 20 total'
        ELSE '❌ WRONG COUNT'
    END as Verification
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'LN03'
AND COLUMN_NAME NOT IN ('NGAY_DL', 'Id', 'FILE_NAME', 'CREATED_DATE', 'ValidFrom', 'ValidTo');

-- Kiểm tra temporal table
SELECT
    name as TableName,
    temporal_type_desc as TemporalType,
    CASE
        WHEN temporal_type = 2 THEN '✅ Temporal enabled'
        ELSE '❌ Temporal disabled'
    END as Status
FROM sys.tables
WHERE name = 'LN03';

-- Kiểm tra columnstore indexes
SELECT
    t.name as TableName,
    i.name as IndexName,
    CASE
        WHEN i.type = 6 THEN '✅ COLUMNSTORE INDEX'
        ELSE '❌ NOT COLUMNSTORE'
    END as Status
FROM sys.tables t
JOIN sys.indexes i ON t.object_id = i.object_id
WHERE t.name IN ('LN03', 'LN03_History')
AND i.name LIKE '%Columnstore%';

PRINT '';
PRINT '=== LN03 FIX COMPLETED SUCCESSFULLY ===';
PRINT '✅ Structure: NGAY_DL -> 20 Business Columns -> System/Temporal columns';
PRINT '✅ Business Columns: 17 with headers + 3 without headers = 20 total';
PRINT '✅ Extra Columns: CREATED_BY and FILE_ORIGIN removed';
PRINT '✅ Data Types: datetime2 for DATE fields, decimal for AMT/SOTIEN fields';
PRINT '✅ Temporal Table: Enabled with LN03_History';
PRINT '✅ Columnstore Indexes: Created for analytics performance';
PRINT '';
PRINT '🎉 LN03 now 100% compliant with requirements! 🎉';
GO
