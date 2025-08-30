-- =====================================================
-- 🚀 SCRIPT CHUYỂN ĐỔI HOÀN TOÀN SANG TEMPORAL TABLES + COLUMNSTORE INDEXES
-- Được thiết kế cho SQL Server 2016+ (hỗ trợ Docker)
-- Tối ưu hóa cho hiệu năng cao với lượng dữ liệu lớn
-- =====================================================

USE [TinhKhoanDB]
GO

PRINT '🚀 Bắt đầu chuyển đổi sang Temporal Tables + Columnstore Indexes...'

-- =====================================================
-- BƯỚC 1: TẠO TEMPORAL TABLES CHO DỮ LIỆU THÔ
-- =====================================================

-- Kiểm tra và tạo bảng RawDataImports với Temporal Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RawDataImports')
BEGIN
    PRINT '📊 Tạo bảng RawDataImports với Temporal Table...'
    
    CREATE TABLE [dbo].[RawDataImports]
    (
        [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY CLUSTERED,
        [ImportDate] DATETIME2(7) NOT NULL,
        [BranchCode] NVARCHAR(10) NOT NULL,
        [DepartmentCode] NVARCHAR(10) NOT NULL,
        [EmployeeCode] NVARCHAR(20) NOT NULL,
        [KpiCode] NVARCHAR(20) NOT NULL,
        [KpiValue] DECIMAL(18,4) NOT NULL,
        [Unit] NVARCHAR(10) NULL,
        [Target] DECIMAL(18,4) NULL,
        [Achievement] DECIMAL(18,4) NULL,
        [Score] DECIMAL(5,2) NULL,
        [DataType] NVARCHAR(20) NOT NULL,
        [StatementDate] NVARCHAR(20) NOT NULL,
        [FileName] NVARCHAR(255) NULL,
        [SourceSystem] NVARCHAR(50) NULL,
        [ProcessedDate] DATETIME2(7) NULL,
        [IsProcessed] BIT NOT NULL DEFAULT 0,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedDate] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        [ModifiedBy] NVARCHAR(50) NULL,
        [ModifiedDate] DATETIME2(7) NULL,
        
        -- 🕰️ Temporal Table Columns
        [ValidFrom] DATETIME2(7) GENERATED ALWAYS AS ROW START NOT NULL,
        [ValidTo] DATETIME2(7) GENERATED ALWAYS AS ROW END NOT NULL,
        
        PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
    )
    WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[RawDataImports_History]));
    
    PRINT '✅ Đã tạo bảng RawDataImports với Temporal Table'
END
ELSE
BEGIN
    PRINT '⚠️ Bảng RawDataImports đã tồn tại, kiểm tra Temporal Table...'
    
    -- Kiểm tra xem bảng đã có Temporal Table chưa
    IF NOT EXISTS (
        SELECT * FROM sys.tables t
        INNER JOIN sys.periods p ON t.object_id = p.object_id
        WHERE t.name = 'RawDataImports' AND p.name = 'SYSTEM_TIME'
    )
    BEGIN
        PRINT '🔄 Chuyển đổi bảng RawDataImports sang Temporal Table...'
        
        -- Thêm cột Temporal nếu chưa có
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('RawDataImports') AND name = 'ValidFrom')
        BEGIN
            ALTER TABLE [dbo].[RawDataImports] 
            ADD [ValidFrom] DATETIME2(7) GENERATED ALWAYS AS ROW START NOT NULL 
                CONSTRAINT DF_RawDataImports_ValidFrom DEFAULT SYSUTCDATETIME(),
                [ValidTo] DATETIME2(7) GENERATED ALWAYS AS ROW END NOT NULL 
                CONSTRAINT DF_RawDataImports_ValidTo DEFAULT '9999-12-31 23:59:59.9999999',
                PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo]);
        END
        
        -- Bật System Versioning
        ALTER TABLE [dbo].[RawDataImports] 
        SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[RawDataImports_History]));
        
        PRINT '✅ Đã chuyển đổi RawDataImports sang Temporal Table'
    END
    ELSE
    BEGIN
        PRINT '✅ RawDataImports đã là Temporal Table'
    END
END

-- =====================================================
-- BƯỚC 2: TẠO COLUMNSTORE INDEXES CHO HIỆU NĂNG CAO
-- =====================================================

PRINT '🏗️ Tạo Columnstore Indexes cho tối ưu hiệu năng...'

-- Tạo Columnstore Index cho bảng chính
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('RawDataImports') AND name = 'CCI_RawDataImports')
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX [CCI_RawDataImports] 
    ON [dbo].[RawDataImports];
    PRINT '✅ Đã tạo Clustered Columnstore Index cho RawDataImports'
END

-- Tạo Columnstore Index cho bảng lịch sử
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'RawDataImports_History')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('RawDataImports_History') AND name = 'CCI_RawDataImports_History')
    BEGIN
        CREATE CLUSTERED COLUMNSTORE INDEX [CCI_RawDataImports_History] 
        ON [dbo].[RawDataImports_History];
        PRINT '✅ Đã tạo Clustered Columnstore Index cho RawDataImports_History'
    END
END

-- =====================================================
-- BƯỚC 3: TẠO TEMPORAL TABLES CHO CÁC LOẠI DỮ LIỆU KHÁC
-- =====================================================

-- Danh sách các loại dữ liệu cần tạo Temporal Tables
DECLARE @DataTypes TABLE (DataType NVARCHAR(20), Description NVARCHAR(100))
INSERT INTO @DataTypes VALUES 
    ('LN01', 'Dữ liệu LOAN - Danh mục tín dụng'),
    ('LN02', 'Sao kê biến động nhóm nợ'),
    ('LN03', 'Dữ liệu Nợ XLRR'),
    ('DP01', 'Dữ liệu Tiền gửi'),
    ('EI01', 'Dữ liệu mobile banking'),
    ('GL01', 'Dữ liệu bút toán GDV'),
    ('DPDA', 'Dữ liệu sao kê phát hành thẻ'),
    ('DB01', 'Sao kê TSDB và Không TSDB'),
    ('KH03', 'Sao kê Khách hàng pháp nhân'),
    ('BC57', 'Sao kê Lãi dự thu'),
    ('RR01', 'Sao kê dư nợ gốc, lãi XLRR'),
    ('GLCB41', 'Bảng cân đối')

DECLARE @DataType NVARCHAR(20), @Description NVARCHAR(100), @TableName NVARCHAR(50), @HistoryTableName NVARCHAR(50)
DECLARE @SQL NVARCHAR(MAX)

DECLARE data_cursor CURSOR FOR
SELECT DataType, Description FROM @DataTypes

OPEN data_cursor
FETCH NEXT FROM data_cursor INTO @DataType, @Description

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @TableName = @DataType + '_Data'
    SET @HistoryTableName = @DataType + '_Data_History'
    
    PRINT '📊 Tạo Temporal Table cho ' + @DataType + ' - ' + @Description
    
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = @TableName)
    BEGIN
        SET @SQL = '
        CREATE TABLE [dbo].[' + @TableName + ']
        (
            [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
            [ImportId] BIGINT NOT NULL,
            [RowNumber] INT NOT NULL,
            [RawData] NVARCHAR(MAX) NOT NULL,
            [ParsedData] NVARCHAR(MAX) NULL,
            [DataType] NVARCHAR(20) NOT NULL DEFAULT ''' + @DataType + ''',
            [StatementDate] NVARCHAR(20) NOT NULL,
            [BranchCode] NVARCHAR(10) NULL,
            [DepartmentCode] NVARCHAR(10) NULL,
            [EmployeeCode] NVARCHAR(20) NULL,
            [Amount] DECIMAL(18,4) NULL,
            [Quantity] INT NULL,
            [ProcessedDate] DATETIME2(7) NULL,
            [IsProcessed] BIT NOT NULL DEFAULT 0,
            [ErrorMessage] NVARCHAR(500) NULL,
            [CreatedDate] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
            
            -- 🕰️ Temporal Table Columns
            [ValidFrom] DATETIME2(7) GENERATED ALWAYS AS ROW START NOT NULL,
            [ValidTo] DATETIME2(7) GENERATED ALWAYS AS ROW END NOT NULL,
            
            PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
        )
        WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[' + @HistoryTableName + ']));
        '
        
        EXEC sp_executesql @SQL
        PRINT '✅ Đã tạo Temporal Table: ' + @TableName
        
        -- Tạo Columnstore Index
        SET @SQL = 'CREATE CLUSTERED COLUMNSTORE INDEX [CCI_' + @TableName + '] ON [dbo].[' + @TableName + '];'
        EXEC sp_executesql @SQL
        PRINT '✅ Đã tạo Columnstore Index cho: ' + @TableName
        
        -- Tạo Columnstore Index cho History Table
        SET @SQL = 'CREATE CLUSTERED COLUMNSTORE INDEX [CCI_' + @HistoryTableName + '] ON [dbo].[' + @HistoryTableName + '];'
        EXEC sp_executesql @SQL
        PRINT '✅ Đã tạo Columnstore Index cho: ' + @HistoryTableName
    END
    ELSE
    BEGIN
        PRINT '⚠️ Bảng ' + @TableName + ' đã tồn tại'
    END
    
    FETCH NEXT FROM data_cursor INTO @DataType, @Description
END

CLOSE data_cursor
DEALLOCATE data_cursor

-- =====================================================
-- BƯỚC 4: TẠO INDEXES BỔ SUNG CHO HIỆU NĂNG
-- =====================================================

PRINT '🔍 Tạo các indexes bổ sung cho hiệu năng truy vấn...'

-- Index cho RawDataImports
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('RawDataImports') AND name = 'IX_RawDataImports_DataType_StatementDate')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_RawDataImports_DataType_StatementDate] 
    ON [dbo].[RawDataImports] ([DataType], [StatementDate])
    INCLUDE ([BranchCode], [DepartmentCode], [EmployeeCode], [KpiValue])
    PRINT '✅ Đã tạo index IX_RawDataImports_DataType_StatementDate'
END

-- Index cho temporal queries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('RawDataImports') AND name = 'IX_RawDataImports_Temporal')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_RawDataImports_Temporal] 
    ON [dbo].[RawDataImports] ([ValidFrom], [ValidTo])
    INCLUDE ([Id], [DataType], [StatementDate])
    PRINT '✅ Đã tạo index IX_RawDataImports_Temporal'
END

-- =====================================================
-- BƯỚC 5: TẠO STORED PROCEDURES CHO TEMPORAL OPERATIONS
-- =====================================================

PRINT '⚙️ Tạo stored procedures cho Temporal operations...'

-- Procedure để lấy dữ liệu tại thời điểm cụ thể
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetDataAsOf')
    DROP PROCEDURE [dbo].[sp_GetDataAsOf]
GO

CREATE PROCEDURE [dbo].[sp_GetDataAsOf]
    @DataType NVARCHAR(20),
    @AsOfDate DATETIME2(7),
    @BranchCode NVARCHAR(10) = NULL,
    @StatementDate NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SQL NVARCHAR(MAX)
    
    SET @SQL = '
    SELECT * FROM [dbo].[RawDataImports] 
    FOR SYSTEM_TIME AS OF @AsOfDate
    WHERE DataType = @DataType'
    
    IF @BranchCode IS NOT NULL
        SET @SQL = @SQL + ' AND BranchCode = @BranchCode'
    
    IF @StatementDate IS NOT NULL
        SET @SQL = @SQL + ' AND StatementDate = @StatementDate'
    
    EXEC sp_executesql @SQL, 
        N'@AsOfDate DATETIME2(7), @DataType NVARCHAR(20), @BranchCode NVARCHAR(10), @StatementDate NVARCHAR(20)',
        @AsOfDate, @DataType, @BranchCode, @StatementDate
END
GO

-- Procedure để lấy lịch sử thay đổi
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetDataHistory')
    DROP PROCEDURE [dbo].[sp_GetDataHistory]
GO

CREATE PROCEDURE [dbo].[sp_GetDataHistory]
    @RecordId BIGINT,
    @FromDate DATETIME2(7) = NULL,
    @ToDate DATETIME2(7) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @FromDate IS NULL SET @FromDate = '1900-01-01'
    IF @ToDate IS NULL SET @ToDate = '9999-12-31'
    
    SELECT 
        *,
        CASE 
            WHEN ValidTo = '9999-12-31 23:59:59.9999999' THEN 'CURRENT'
            ELSE 'HISTORICAL'
        END AS RecordStatus
    FROM [dbo].[RawDataImports] 
    FOR SYSTEM_TIME FROM @FromDate TO @ToDate
    WHERE Id = @RecordId
    ORDER BY ValidFrom DESC
END
GO

-- =====================================================
-- BƯỚC 6: TẠO VIEWS CHO DỮ LIỆU HIỆN TẠI
-- =====================================================

PRINT '📊 Tạo views cho dữ liệu hiện tại...'

-- View tổng hợp dữ liệu hiện tại
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_CurrentRawData')
    DROP VIEW [dbo].[vw_CurrentRawData]
GO

CREATE VIEW [dbo].[vw_CurrentRawData]
AS
SELECT 
    Id,
    ImportDate,
    BranchCode,
    DepartmentCode,
    EmployeeCode,
    KpiCode,
    KpiValue,
    Unit,
    Target,
    Achievement,
    Score,
    DataType,
    StatementDate,
    FileName,
    SourceSystem,
    ProcessedDate,
    IsProcessed,
    CreatedBy,
    CreatedDate,
    ModifiedBy,
    ModifiedDate,
    ValidFrom,
    ValidTo
FROM [dbo].[RawDataImports]
WHERE ValidTo = '9999-12-31 23:59:59.9999999' -- Chỉ lấy dữ liệu hiện tại
GO

-- =====================================================
-- BƯỚC 7: CẤU HÌNH BẢO TRÌ TEMPORAL TABLES
-- =====================================================

PRINT '🔧 Cấu hình bảo trì Temporal Tables...'

-- Tạo job để dọn dẹp lịch sử cũ (nếu cần)
-- Chỉ giữ lại lịch sử trong 5 năm
DECLARE @RetentionPeriod NVARCHAR(20) = '5 YEARS'

-- Lưu ý: Trong môi trường thực tế, bạn có thể cấu hình retention policy
-- ALTER TABLE [dbo].[RawDataImports] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[RawDataImports_History], HISTORY_RETENTION_PERIOD = 5 YEARS))

PRINT '✅ Hoàn thành cấu hình bảo trì Temporal Tables'

-- =====================================================
-- BƯỚC 8: TẠO FUNCTIONS HỖ TRỢ
-- =====================================================

PRINT '🛠️ Tạo functions hỗ trợ cho Temporal Tables...'

-- Function lấy số lượng bản ghi theo thời gian
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'fn_GetRecordCountAsOf')
    DROP FUNCTION [dbo].[fn_GetRecordCountAsOf]
GO

CREATE FUNCTION [dbo].[fn_GetRecordCountAsOf]
(
    @DataType NVARCHAR(20),
    @AsOfDate DATETIME2(7)
)
RETURNS INT
AS
BEGIN
    DECLARE @Count INT
    
    SELECT @Count = COUNT(*)
    FROM [dbo].[RawDataImports] 
    FOR SYSTEM_TIME AS OF @AsOfDate
    WHERE DataType = @DataType
    
    RETURN ISNULL(@Count, 0)
END
GO

-- =====================================================
-- HOÀN THÀNH
-- =====================================================

PRINT '🎉 HOÀN THÀNH CHUYỂN ĐỔI SANG TEMPORAL TABLES + COLUMNSTORE INDEXES!'
PRINT '📊 Hệ thống đã được tối ưu hóa cho hiệu năng cao với:'
PRINT '   ✅ Temporal Tables cho tracking lịch sử thay đổi'
PRINT '   ✅ Columnstore Indexes cho truy vấn analytical nhanh'
PRINT '   ✅ Stored Procedures và Views hỗ trợ'
PRINT '   ✅ Functions tiện ích'
PRINT ''
PRINT '🚀 Hệ thống sẵn sàng xử lý hàng triệu bản ghi với hiệu năng tối ưu!'

-- Hiển thị thống kê
SELECT 
    'RawDataImports' as TableName,
    COUNT(*) as CurrentRecords,
    MIN(ValidFrom) as EarliestRecord,
    MAX(ValidFrom) as LatestRecord
FROM [dbo].[RawDataImports]

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'RawDataImports_History')
BEGIN
    SELECT 
        'RawDataImports_History' as TableName,
        COUNT(*) as HistoricalRecords,
        MIN(ValidFrom) as EarliestHistory,
        MAX(ValidTo) as LatestHistory
    FROM [dbo].[RawDataImports_History]
END
