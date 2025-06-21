-- ================================================================
-- 🔍 KIỂM TRA HỆ THỐNG TEMPORAL TABLES & COLUMNSTORE INDEXES 
-- Script đánh giá mức độ sẵn sàng cho SQL Server Temporal Tables
-- Author: Agribank Lai Châu Dev Team
-- Date: 2025-01-20
-- ================================================================

PRINT '🏦 AGRIBANK LAI CHÂU - RÀ SOÁT HỆ THỐNG TEMPORAL TABLES & COLUMNSTORE';
PRINT '================================================================';
PRINT '';

-- 1. KIỂM TRA PHIÊN BẢN SQL SERVER
PRINT '1️⃣ KIỂM TRA PHIÊN BẢN SQL SERVER';
PRINT '=================================';
SELECT 
    @@VERSION as [SQL Server Version],
    SERVERPROPERTY('ProductVersion') as [Product Version],
    SERVERPROPERTY('ProductLevel') as [Product Level],
    SERVERPROPERTY('Edition') as [Edition],
    CASE 
        WHEN CAST(SERVERPROPERTY('ProductMajorVersion') AS INT) >= 13 
        THEN '✅ Hỗ trợ Temporal Tables (SQL Server 2016+)'
        ELSE '❌ Không hỗ trợ Temporal Tables (Cần SQL Server 2016+)'
    END as [Temporal Support],
    CASE 
        WHEN CAST(SERVERPROPERTY('ProductMajorVersion') AS INT) >= 11 
        THEN '✅ Hỗ trợ Columnstore Indexes (SQL Server 2012+)'
        ELSE '❌ Không hỗ trợ Columnstore Indexes (Cần SQL Server 2012+)'
    END as [Columnstore Support];

PRINT '';
PRINT '2️⃣ KIỂM TRA CÁC BẢNG TEMPORAL HIỆN TẠI';
PRINT '====================================';

-- Kiểm tra các bảng có temporal được bật
IF OBJECT_ID('tempdb..#TemporalTables') IS NOT NULL DROP TABLE #TemporalTables;
SELECT 
    t.name as [Table Name],
    ht.name as [History Table],
    t.temporal_type_desc as [Temporal Type],
    '✅ Đã kích hoạt' as [Status]
INTO #TemporalTables
FROM sys.tables t
LEFT JOIN sys.tables ht ON t.history_table_id = ht.object_id
WHERE t.temporal_type = 2; -- SYSTEM_VERSIONED_TEMPORAL_TABLE

IF (SELECT COUNT(*) FROM #TemporalTables) > 0
BEGIN
    SELECT * FROM #TemporalTables;
END
ELSE
BEGIN
    PRINT '⚠️  Chưa có bảng nào được cấu hình temporal tables';
END

PRINT '';
PRINT '3️⃣ KIỂM TRA CÁC BẢNG DỮ LIỆU THÔ CẦN TEMPORAL';
PRINT '============================================';

-- Danh sách các bảng cần temporal
IF OBJECT_ID('tempdb..#RequiredTemporalTables') IS NOT NULL DROP TABLE #RequiredTemporalTables;
CREATE TABLE #RequiredTemporalTables (
    TableName NVARCHAR(128),
    Purpose NVARCHAR(255),
    Priority NVARCHAR(20),
    IsExist BIT,
    IsTemporal BIT
);

INSERT INTO #RequiredTemporalTables VALUES 
('raw_data_imports', 'Lưu trữ thông tin import dữ liệu thô', 'HIGH', 0, 0),
('raw_data_records', 'Lưu trữ records dữ liệu thô chi tiết', 'HIGH', 0, 0),
('LN01History', 'Lịch sử dữ liệu LN01 (Cho vay)', 'HIGH', 0, 0),
('GL01History', 'Lịch sử dữ liệu GL01 (Tổng đài)', 'HIGH', 0, 0),
('LN03History', 'Lịch sử dữ liệu LN03', 'MEDIUM', 0, 0),
('EI01History', 'Lịch sử dữ liệu EI01', 'MEDIUM', 0, 0),
('DPDAHistory', 'Lịch sử dữ liệu DPDA', 'MEDIUM', 0, 0),
('DB01History', 'Lịch sử dữ liệu DB01', 'MEDIUM', 0, 0),
('KH03History', 'Lịch sử dữ liệu KH03', 'MEDIUM', 0, 0),
('BC57History', 'Lịch sử dữ liệu BC57', 'MEDIUM', 0, 0);

-- Cập nhật trạng thái tồn tại
UPDATE rtt 
SET IsExist = CASE WHEN o.object_id IS NOT NULL THEN 1 ELSE 0 END
FROM #RequiredTemporalTables rtt
LEFT JOIN sys.objects o ON o.name = rtt.TableName AND o.type = 'U';

-- Cập nhật trạng thái temporal
UPDATE rtt 
SET IsTemporal = CASE WHEN t.temporal_type = 2 THEN 1 ELSE 0 END
FROM #RequiredTemporalTables rtt
LEFT JOIN sys.tables t ON t.name = rtt.TableName;

SELECT 
    TableName as [Tên Bảng],
    Purpose as [Mục Đích], 
    Priority as [Ưu Tiên],
    CASE 
        WHEN IsExist = 1 THEN '✅ Tồn tại'
        ELSE '❌ Chưa tạo'
    END as [Trạng Thái Bảng],
    CASE 
        WHEN IsTemporal = 1 THEN '✅ Đã temporal'
        WHEN IsExist = 1 THEN '⚠️ Chưa temporal'
        ELSE '❌ Chưa tạo bảng'
    END as [Trạng Thái Temporal]
FROM #RequiredTemporalTables
ORDER BY 
    CASE Priority WHEN 'HIGH' THEN 1 WHEN 'MEDIUM' THEN 2 ELSE 3 END,
    TableName;

PRINT '';
PRINT '4️⃣ KIỂM TRA COLUMNSTORE INDEXES';
PRINT '===============================';

-- Kiểm tra các columnstore index hiện tại
IF OBJECT_ID('tempdb..#ColumnstoreIndexes') IS NOT NULL DROP TABLE #ColumnstoreIndexes;
SELECT 
    t.name as [Table Name],
    i.name as [Index Name], 
    i.type_desc as [Index Type],
    '✅ Đã có' as [Status]
INTO #ColumnstoreIndexes
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.type IN (5, 6); -- CLUSTERED COLUMNSTORE, NONCLUSTERED COLUMNSTORE

IF (SELECT COUNT(*) FROM #ColumnstoreIndexes) > 0
BEGIN
    SELECT * FROM #ColumnstoreIndexes;
END
ELSE
BEGIN
    PRINT '⚠️  Chưa có columnstore index nào được tạo';
END

PRINT '';
PRINT '5️⃣ ĐÁNH GIÁ MỨC ĐỘ SẴN SÀNG';
PRINT '============================';

DECLARE @TemporalTablesCount INT = (SELECT COUNT(*) FROM #RequiredTemporalTables WHERE IsTemporal = 1);
DECLARE @RequiredTablesCount INT = (SELECT COUNT(*) FROM #RequiredTemporalTables);
DECLARE @ColumnstoreCount INT = (SELECT COUNT(*) FROM #ColumnstoreIndexes);
DECLARE @ReadinessScore FLOAT = 0;

-- Tính điểm sẵn sàng
SET @ReadinessScore = (@TemporalTablesCount * 1.0 / @RequiredTablesCount) * 70 + 
                      (CASE WHEN @ColumnstoreCount > 0 THEN 20 ELSE 0 END) +
                      (CASE WHEN CAST(SERVERPROPERTY('ProductMajorVersion') AS INT) >= 13 THEN 10 ELSE 0 END);

SELECT 
    @RequiredTablesCount as [Tổng Bảng Cần Temporal],
    @TemporalTablesCount as [Bảng Đã Có Temporal],
    @ColumnstoreCount as [Số Columnstore Index],
    CAST(@ReadinessScore AS INT) as [Điểm Sẵn Sàng (%)],
    CASE 
        WHEN @ReadinessScore >= 90 THEN '🟢 Sẵn sàng hoàn toàn'
        WHEN @ReadinessScore >= 70 THEN '🟡 Sẵn sàng cơ bản'
        WHEN @ReadinessScore >= 50 THEN '🟠 Cần hoàn thiện'
        ELSE '🔴 Chưa sẵn sàng'
    END as [Trạng Thái Sẵn Sàng];

PRINT '';
PRINT '6️⃣ KHUYẾN NGHỊ HÀNH ĐỘNG';
PRINT '=======================';

-- Tạo danh sách khuyến nghị
IF @ReadinessScore < 90
BEGIN
    PRINT '📋 CÁC HÀNH ĐỘNG CẦN THỰC HIỆN:';
    PRINT '';
    
    IF CAST(SERVERPROPERTY('ProductMajorVersion') AS INT) < 13
    BEGIN
        PRINT '   🔴 1. Nâng cấp SQL Server lên phiên bản 2016 trở lên để hỗ trợ Temporal Tables';
    END
    
    IF @TemporalTablesCount < @RequiredTablesCount
    BEGIN
        PRINT '   ⚠️  2. Kích hoạt Temporal Tables cho các bảng chưa có:';
        
        SELECT '      - ' + TableName + ' (' + Priority + ' priority)' as [Bảng Cần Kích Hoạt]
        FROM #RequiredTemporalTables 
        WHERE IsExist = 1 AND IsTemporal = 0;
    END
    
    IF @ColumnstoreCount = 0
    BEGIN
        PRINT '   📊 3. Tạo Columnstore Indexes cho các bảng lớn để tối ưu hiệu suất truy vấn';
    END
    
    PRINT '   🔧 4. Chạy script tạo Temporal Tables và Columnstore Indexes';
    PRINT '   ✅ 5. Kiểm thử import dữ liệu thật với Temporal Tables';
END
ELSE
BEGIN
    PRINT '🎉 HỆ THỐNG ĐÃ SẴN SÀNG CHO TEMPORAL TABLES!';
    PRINT '   ✅ Có thể bắt đầu import dữ liệu thật';
    PRINT '   ✅ Temporal Tables hoạt động tốt';
    PRINT '   ✅ Columnstore Indexes đã được tối ưu';
END

-- Script tạo temporal tables mẫu
PRINT '';
PRINT '7️⃣ SCRIPT TẠO TEMPORAL TABLES MẪU';
PRINT '=================================';
PRINT '';
PRINT '-- Script để kích hoạt temporal cho bảng raw_data_imports:';
PRINT 'ALTER TABLE raw_data_imports ADD';
PRINT '    ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),';
PRINT '    ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, ''9999-12-31 23:59:59.9999999''),';
PRINT '    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo);';
PRINT '';
PRINT 'ALTER TABLE raw_data_imports SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.raw_data_imports_history));';
PRINT '';
PRINT '-- Script tạo columnstore index:';
PRINT 'CREATE NONCLUSTERED COLUMNSTORE INDEX IX_raw_data_imports_columnstore';
PRINT 'ON raw_data_imports (id, data_type, statement_date, total_records, file_size);';

-- Cleanup
DROP TABLE #TemporalTables;
DROP TABLE #RequiredTemporalTables;
DROP TABLE #ColumnstoreIndexes;

PRINT '';
PRINT '================================================================';
PRINT '✅ HOÀN THÀNH RÀ SOÁT HỆ THỐNG TEMPORAL TABLES & COLUMNSTORE';
PRINT '================================================================';
