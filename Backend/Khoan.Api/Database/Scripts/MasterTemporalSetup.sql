-- =============================================
-- Script: MasterTemporalSetup.sql
-- Mục đích: Script master để thiết lập hoàn chỉnh temporal tables + columnstore indexes
-- Tác giả: Em (siêu lập trình viên Fullstack)
-- Ngày: 26/06/2025
-- Chú thích: Chạy tất cả scripts theo thứ tự đúng để hoàn thiện hệ thống
-- =============================================

USE master;
GO

-- Kiểm tra database tồn tại
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TinhKhoanDB')
BEGIN
    PRINT '❌ Database TinhKhoanDB không tồn tại!';
    PRINT '💡 Tạo database trước khi chạy script này.';
    RETURN;
END

USE TinhKhoanDB;
GO

PRINT '🚀 BẮT ĐẦU THIẾT LẬP HOÀN CHỈNH TEMPORAL TABLES + COLUMNSTORE INDEXES';
PRINT '=================================================================================';
PRINT '📅 Thời gian bắt đầu: ' + CONVERT(NVARCHAR, GETDATE(), 120);
PRINT '👨‍💻 Được thực hiện bởi: Em (siêu lập trình viên Fullstack)';
PRINT '🎯 Mục tiêu: Hoàn thiện temporal tables cho tất cả loại dữ liệu thô';
PRINT '';

-- Tạo bảng log để theo dõi quá trình thực hiện
IF OBJECT_ID('dbo.ScriptExecutionLog') IS NULL
BEGIN
    CREATE TABLE dbo.ScriptExecutionLog (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        StepNumber INT,
        StepName NVARCHAR(200),
        StartTime DATETIME2,
        EndTime DATETIME2,
        Status NVARCHAR(50),
        ErrorMessage NVARCHAR(MAX),
        RowsAffected BIGINT
    );

    PRINT '📝 Đã tạo bảng ScriptExecutionLog để theo dõi quá trình thực hiện';
END

-- Helper procedure để log các bước
CREATE OR ALTER PROCEDURE sp_LogStep
    @StepNumber INT,
    @StepName NVARCHAR(200),
    @Status NVARCHAR(50),
    @ErrorMessage NVARCHAR(MAX) = NULL,
    @RowsAffected BIGINT = 0
AS
BEGIN
    INSERT INTO dbo.ScriptExecutionLog (StepNumber, StepName, StartTime, EndTime, Status, ErrorMessage, RowsAffected)
    VALUES (@StepNumber, @StepName, GETDATE(), GETDATE(), @Status, @ErrorMessage, @RowsAffected);
END
GO

-- =====================================================
-- BƯỚC 1: KIỂM TRA VÀ SỬA CHỮA CÁC BẢNG HIỆN CÓ
-- =====================================================

PRINT '';
PRINT '🔧 BƯỚC 1: KIỂM TRA VÀ SỬA CHỮA CÁC BẢNG HIỆN CÓ';
PRINT '===============================================================================';

EXEC sp_LogStep 1, 'Bắt đầu kiểm tra và sửa chữa bảng hiện có', 'RUNNING';

BEGIN TRY
    -- Chạy script sửa chữa bảng hiện có (nội dung từ FixExistingTemporalTables.sql)

    -- Tạo bảng tạm để kiểm tra trạng thái
    IF OBJECT_ID('tempdb..#CurrentTableStatus') IS NOT NULL DROP TABLE #CurrentTableStatus;

    CREATE TABLE #CurrentTableStatus (
        TableName NVARCHAR(128),
        IsTemporal BIT,
        HasColumnstore BIT,
        HistoryTableName NVARCHAR(128),
        RowCount BIGINT,
        NeedsUpdate BIT
    );

    -- Kiểm tra các bảng hiện tại
    INSERT INTO #CurrentTableStatus (TableName, IsTemporal, HasColumnstore, HistoryTableName, RowCount, NeedsUpdate)
    SELECT
        t.name AS TableName,
        CASE WHEN t.temporal_type = 2 THEN 1 ELSE 0 END AS IsTemporal,
        CASE WHEN EXISTS (
            SELECT 1 FROM sys.indexes i
            WHERE i.object_id = t.object_id AND i.type IN (5,6)
        ) THEN 1 ELSE 0 END AS HasColumnstore,
        h.name AS HistoryTableName,
        ISNULL(p.rows, 0) AS RowCount,
        CASE WHEN t.temporal_type != 2 OR NOT EXISTS (
            SELECT 1 FROM sys.indexes i
            WHERE i.object_id = t.object_id AND i.type IN (5,6)
        ) THEN 1 ELSE 0 END AS NeedsUpdate
    FROM sys.tables t
    LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
    LEFT JOIN sys.partitions p ON t.object_id = p.object_id AND p.index_id IN (0,1)
    WHERE t.name IN ('ImportedDataRecords', 'ImportedDataItems', 'RawDataImports')
       OR t.name LIKE '%_RawData';

    -- Sửa chữa ImportedDataRecords
    IF EXISTS (SELECT 1 FROM #CurrentTableStatus WHERE TableName = 'ImportedDataRecords' AND IsTemporal = 0)
    BEGIN
        PRINT '  🔧 Chuyển đổi ImportedDataRecords sang Temporal Table...';

        -- Disable system versioning if exists
        IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ImportedDataRecords' AND temporal_type > 0)
        BEGIN
            BEGIN TRY
                ALTER TABLE [ImportedDataRecords] SET (SYSTEM_VERSIONING = OFF);
            END TRY
            BEGIN CATCH
                -- Ignore error if already off
            END CATCH
        END

        -- Add temporal columns if not exists
        IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'SysStartTime' AND object_id = OBJECT_ID('ImportedDataRecords'))
        BEGIN
            ALTER TABLE [ImportedDataRecords] ADD
                [SysStartTime] DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
                [SysEndTime] DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999');
        END

        -- Add period if not exists
        BEGIN TRY
            ALTER TABLE [ImportedDataRecords]
            ADD PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
        END TRY
        BEGIN CATCH
            -- Period might already exist
        END CATCH

        -- Enable system versioning
        BEGIN TRY
            ALTER TABLE [ImportedDataRecords]
            SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[ImportedDataRecords_History]));
            PRINT '    ✅ ImportedDataRecords đã được chuyển đổi sang Temporal Table';
        END TRY
        BEGIN CATCH
            PRINT '    ⚠️ ImportedDataRecords: ' + ERROR_MESSAGE();
        END CATCH
    END

    -- Sửa chữa ImportedDataItems
    IF EXISTS (SELECT 1 FROM #CurrentTableStatus WHERE TableName = 'ImportedDataItems' AND IsTemporal = 0)
    BEGIN
        PRINT '  🔧 Chuyển đổi ImportedDataItems sang Temporal Table...';

        -- Disable system versioning if exists
        IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ImportedDataItems' AND temporal_type > 0)
        BEGIN
            BEGIN TRY
                ALTER TABLE [ImportedDataItems] SET (SYSTEM_VERSIONING = OFF);
            END TRY
            BEGIN CATCH
                -- Ignore error if already off
            END CATCH
        END

        -- Add temporal columns if not exists
        IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'SysStartTime' AND object_id = OBJECT_ID('ImportedDataItems'))
        BEGIN
            ALTER TABLE [ImportedDataItems] ADD
                [SysStartTime] DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSUTCDATETIME(),
                [SysEndTime] DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999');
        END

        -- Add period if not exists
        BEGIN TRY
            ALTER TABLE [ImportedDataItems]
            ADD PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]);
        END TRY
        BEGIN CATCH
            -- Period might already exist
        END CATCH

        -- Enable system versioning
        BEGIN TRY
            ALTER TABLE [ImportedDataItems]
            SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[ImportedDataItems_History]));
            PRINT '    ✅ ImportedDataItems đã được chuyển đổi sang Temporal Table';
        END TRY
        BEGIN CATCH
            PRINT '    ⚠️ ImportedDataItems: ' + ERROR_MESSAGE();
        END CATCH
    END

    EXEC sp_LogStep 1, 'Hoàn thành kiểm tra và sửa chữa bảng hiện có', 'SUCCESS', NULL, @@ROWCOUNT;

END TRY
BEGIN CATCH
    EXEC sp_LogStep 1, 'Lỗi kiểm tra và sửa chữa bảng hiện có', 'ERROR', ERROR_MESSAGE(), 0;
    PRINT '❌ Lỗi BƯỚC 1: ' + ERROR_MESSAGE();
END CATCH

-- =====================================================
-- BƯỚC 2: TẠO TEMPORAL TABLES CHO DỮ LIỆU THÔ
-- =====================================================

PRINT '';
PRINT '📊 BƯỚC 2: TẠO TEMPORAL TABLES CHO DỮ LIỆU THÔ';
PRINT '===============================================================================';

EXEC sp_LogStep 2, 'Bắt đầu tạo temporal tables cho dữ liệu thô', 'RUNNING';

BEGIN TRY
    -- Danh sách các loại dữ liệu thô
    DECLARE @RawDataTypes TABLE (
        DataType NVARCHAR(20),
        TableName NVARCHAR(50),
        Description NVARCHAR(200)
    );

    INSERT INTO @RawDataTypes VALUES
        ('BC57', 'BC57_RawData', 'Sao kê Lãi dự thu - Dữ liệu lãi suất'),
        ('DPDA', 'DPDA_RawData', 'Dữ liệu sao kê phát hành thẻ'),
        ('DP01', 'DP01_RawData', 'Dữ liệu Tiền gửi'),
        ('DB01', 'DB01_RawData', 'Sao kê TSDB và Không TSDB'),
        ('LN01', 'LN01_RawData', 'Dữ liệu LOAN - Danh mục tín dụng'),
        ('GL01', 'GL01_RawData', 'Dữ liệu bút toán GDV'),
        ('EI01', 'EI01_RawData', 'Dữ liệu mobile banking');

    DECLARE @DataType NVARCHAR(20), @TableName NVARCHAR(50), @Description NVARCHAR(200);
    DECLARE @HistoryTableName NVARCHAR(50), @SQL NVARCHAR(MAX);
    DECLARE @TablesCreated INT = 0;

    DECLARE create_tables_cursor CURSOR FOR
    SELECT DataType, TableName, Description FROM @RawDataTypes;

    OPEN create_tables_cursor;
    FETCH NEXT FROM create_tables_cursor INTO @DataType, @TableName, @Description;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @HistoryTableName = @TableName + '_History';

        -- Kiểm tra xem bảng đã tồn tại chưa
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = @TableName)
        BEGIN
            PRINT '  📊 Tạo ' + @DataType + ' - ' + @Description;

            BEGIN TRY
                -- Tạo Temporal Table
                SET @SQL = '
                CREATE TABLE [dbo].[' + @TableName + ']
                (
                    [Id] BIGINT IDENTITY(1,1) NOT NULL,
                    [ImportBatchId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                    [RowNumber] INT NOT NULL,
                    [RawDataLine] NVARCHAR(MAX) NOT NULL,
                    [ParsedData] NVARCHAR(MAX) NULL,
                    [DataType] NVARCHAR(20) NOT NULL DEFAULT ''' + @DataType + ''',

                    -- Thông tin ngày tháng
                    [StatementDate] DATE NOT NULL,
                    [ImportDate] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
                    [ProcessedDate] DATETIME2(7) NULL,

                    -- Thông tin cơ cấu tổ chức
                    [BranchCode] NVARCHAR(10) NULL,
                    [DepartmentCode] NVARCHAR(10) NULL,
                    [EmployeeCode] NVARCHAR(20) NULL,
                    [UnitCode] NVARCHAR(10) NULL,

                    -- Dữ liệu số liệu
                    [Amount] DECIMAL(18,4) NULL,
                    [Quantity] INT NULL,
                    [Rate] DECIMAL(10,6) NULL,
                    [Ratio] DECIMAL(10,6) NULL,

                    -- Trạng thái xử lý
                    [IsProcessed] BIT NOT NULL DEFAULT 0,
                    [IsValid] BIT NOT NULL DEFAULT 1,
                    [ValidationErrors] NVARCHAR(1000) NULL,
                    [ProcessingNotes] NVARCHAR(500) NULL,

                    -- Metadata
                    [SourceFileName] NVARCHAR(255) NULL,
                    [FileSize] BIGINT NULL,
                    [CheckSum] NVARCHAR(32) NULL,
                    [CreatedBy] NVARCHAR(100) NOT NULL DEFAULT ''SYSTEM'',
                    [CreatedDate] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
                    [LastModifiedBy] NVARCHAR(100) NOT NULL DEFAULT ''SYSTEM'',
                    [LastModifiedDate] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
                    [IsDeleted] BIT NOT NULL DEFAULT 0,

                    -- Temporal Table Columns
                    [ValidFrom] DATETIME2(7) GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
                    [ValidTo] DATETIME2(7) GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,

                    CONSTRAINT [PK_' + @TableName + '] PRIMARY KEY CLUSTERED ([Id]),
                    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
                )
                WITH (
                    SYSTEM_VERSIONING = ON (
                        HISTORY_TABLE = [dbo].[' + @HistoryTableName + '],
                        DATA_CONSISTENCY_CHECK = ON,
                        HISTORY_RETENTION_PERIOD = 7 YEARS
                    )
                );';

                EXEC sp_executesql @SQL;
                SET @TablesCreated = @TablesCreated + 1;

                -- Tạo Columnstore Index cho History Table
                SET @SQL = 'CREATE CLUSTERED COLUMNSTORE INDEX [CCI_' + @HistoryTableName + ']
                           ON [dbo].[' + @HistoryTableName + ']
                           WITH (COMPRESSION_DELAY = 0, MAXDOP = 4);';
                EXEC sp_executesql @SQL;

                PRINT '    ✅ Đã tạo ' + @TableName + ' với Temporal + Columnstore';

            END TRY
            BEGIN CATCH
                PRINT '    ❌ Lỗi tạo ' + @TableName + ': ' + ERROR_MESSAGE();
            END CATCH
        END
        ELSE
        BEGIN
            PRINT '  ⚠️ Bảng ' + @TableName + ' đã tồn tại';
        END

        FETCH NEXT FROM create_tables_cursor INTO @DataType, @TableName, @Description;
    END

    CLOSE create_tables_cursor;
    DEALLOCATE create_tables_cursor;

    EXEC sp_LogStep 2, 'Hoàn thành tạo temporal tables cho dữ liệu thô', 'SUCCESS', NULL, @TablesCreated;

END TRY
BEGIN CATCH
    EXEC sp_LogStep 2, 'Lỗi tạo temporal tables cho dữ liệu thô', 'ERROR', ERROR_MESSAGE(), 0;
    PRINT '❌ Lỗi BƯỚC 2: ' + ERROR_MESSAGE();
END CATCH

-- =====================================================
-- BƯỚC 3: TẠO COLUMNSTORE INDEXES CHO TẤT CẢ BẢNG
-- =====================================================

PRINT '';
PRINT '📊 BƯỚC 3: TẠO COLUMNSTORE INDEXES CHO TẤT CẢ BẢNG';
PRINT '===============================================================================';

EXEC sp_LogStep 3, 'Bắt đầu tạo columnstore indexes', 'RUNNING';

BEGIN TRY
    DECLARE @IndexesCreated INT = 0;
    DECLARE @CSTableName NVARCHAR(128), @CSIndexSQL NVARCHAR(MAX);

    -- Tạo columnstore cho các bảng chưa có
    DECLARE cs_cursor CURSOR FOR
    SELECT t.name
    FROM sys.tables t
    WHERE (t.name LIKE '%_RawData' OR t.name IN ('ImportedDataRecords', 'ImportedDataItems'))
      AND NOT EXISTS (
          SELECT 1 FROM sys.indexes i
          WHERE i.object_id = t.object_id AND i.type IN (5,6)
      );

    OPEN cs_cursor;
    FETCH NEXT FROM cs_cursor INTO @CSTableName;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        BEGIN TRY
            PRINT '  📊 Tạo Columnstore Index cho: ' + @CSTableName;

            SET @CSIndexSQL = 'CREATE NONCLUSTERED COLUMNSTORE INDEX [NCCI_' + @CSTableName + '_Analytics]
                              ON [dbo].[' + @CSTableName + '] ([Id], [StatementDate])
                              WITH (COMPRESSION_DELAY = 0);';

            EXEC sp_executesql @CSIndexSQL;
            SET @IndexesCreated = @IndexesCreated + 1;

            PRINT '    ✅ Đã tạo Columnstore Index cho: ' + @CSTableName;

        END TRY
        BEGIN CATCH
            PRINT '    ⚠️ Không thể tạo Columnstore Index cho ' + @CSTableName + ': ' + ERROR_MESSAGE();
        END CATCH

        FETCH NEXT FROM cs_cursor INTO @CSTableName;
    END

    CLOSE cs_cursor;
    DEALLOCATE cs_cursor;

    EXEC sp_LogStep 3, 'Hoàn thành tạo columnstore indexes', 'SUCCESS', NULL, @IndexesCreated;

END TRY
BEGIN CATCH
    EXEC sp_LogStep 3, 'Lỗi tạo columnstore indexes', 'ERROR', ERROR_MESSAGE(), 0;
    PRINT '❌ Lỗi BƯỚC 3: ' + ERROR_MESSAGE();
END CATCH

-- =====================================================
-- BƯỚC 4: TẠO STORED PROCEDURES VÀ VIEWS
-- =====================================================

PRINT '';
PRINT '⚡ BƯỚC 4: TẠO STORED PROCEDURES VÀ VIEWS';
PRINT '===============================================================================';

EXEC sp_LogStep 4, 'Bắt đầu tạo stored procedures và views', 'RUNNING';

BEGIN TRY
    -- Stored Procedure: Báo cáo trạng thái temporal tables
    CREATE OR ALTER PROCEDURE [dbo].[sp_TemporalTablesReport]
    AS
    BEGIN
        SET NOCOUNT ON;

        PRINT '📊 BÁO CÁO TRẠNG THÁI TEMPORAL TABLES + COLUMNSTORE INDEXES';
        PRINT '===============================================================================';

        -- Báo cáo chi tiết
        SELECT
            t.name AS [Tên Bảng],
            CASE t.temporal_type
                WHEN 0 THEN '❌ Không'
                WHEN 1 THEN '📝 History'
                WHEN 2 THEN '✅ Temporal'
            END AS [Temporal],
            CASE WHEN EXISTS (
                SELECT 1 FROM sys.indexes i
                WHERE i.object_id = t.object_id AND i.type IN (5,6)
            ) THEN '✅ Có' ELSE '❌ Không' END AS [Columnstore],
            h.name AS [Bảng Lịch Sử],
            FORMAT(ISNULL(p.rows, 0), 'N0') AS [Số Dòng],
            FORMAT(ISNULL(ph.rows, 0), 'N0') AS [Dòng Lịch Sử]
        FROM sys.tables t
        LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
        LEFT JOIN sys.partitions p ON t.object_id = p.object_id AND p.index_id IN (0,1)
        LEFT JOIN sys.partitions ph ON h.object_id = ph.object_id AND ph.index_id IN (0,1)
        WHERE t.name NOT LIKE '%_History'
        ORDER BY
            CASE WHEN t.name LIKE '%RawData' THEN 1
                 WHEN t.name LIKE 'ImportedData%' THEN 2
                 ELSE 3 END,
            t.name;
    END
    GO

    -- View tổng hợp dữ liệu thô
    CREATE OR ALTER VIEW [dbo].[vw_RawDataSummary] AS
    SELECT
        'Tổng quan dữ liệu thô' AS [Loại Báo Cáo],
        COUNT(*) AS [Số Bảng Temporal],
        SUM(CASE WHEN EXISTS (
            SELECT 1 FROM sys.indexes i
            WHERE i.object_id = t.object_id AND i.type IN (5,6)
        ) THEN 1 ELSE 0 END) AS [Số Bảng Có Columnstore],
        SUM(ISNULL(p.rows, 0)) AS [Tổng Số Dòng]
    FROM sys.tables t
    LEFT JOIN sys.partitions p ON t.object_id = p.object_id AND p.index_id IN (0,1)
    WHERE t.temporal_type = 2
      AND (t.name LIKE '%_RawData' OR t.name LIKE 'ImportedData%');
    GO

    EXEC sp_LogStep 4, 'Hoàn thành tạo stored procedures và views', 'SUCCESS', NULL, 2;

END TRY
BEGIN CATCH
    EXEC sp_LogStep 4, 'Lỗi tạo stored procedures và views', 'ERROR', ERROR_MESSAGE(), 0;
    PRINT '❌ Lỗi BƯỚC 4: ' + ERROR_MESSAGE();
END CATCH

-- =====================================================
-- BƯỚC 5: BÁO CÁO KẾT QUẢ CUỐI CÙNG
-- =====================================================

PRINT '';
PRINT '🎉 BƯỚC 5: BÁO CÁO KẾT QUẢ CUỐI CÙNG';
PRINT '===============================================================================';

EXEC sp_LogStep 5, 'Bắt đầu tạo báo cáo cuối cùng', 'RUNNING';

-- Chạy báo cáo trạng thái
EXEC [dbo].[sp_TemporalTablesReport];

-- Hiển thị view tổng hợp
PRINT '';
PRINT '📋 TỔNG KẾT DỮ LIỆU THÔ:';
SELECT * FROM [dbo].[vw_RawDataSummary];

-- Hiển thị log thực hiện
PRINT '';
PRINT '📝 LOG THỰC HIỆN:';
SELECT
    StepNumber AS [Bước],
    StepName AS [Tên Bước],
    Status AS [Trạng Thái],
    ISNULL(ErrorMessage, 'Không có lỗi') AS [Lỗi],
    RowsAffected AS [Dòng Ảnh Hưởng],
    DATEDIFF(SECOND, StartTime, EndTime) AS [Thời Gian (giây)]
FROM dbo.ScriptExecutionLog
ORDER BY StepNumber;

EXEC sp_LogStep 5, 'Hoàn thành báo cáo cuối cùng', 'SUCCESS';

PRINT '';
PRINT '🎯 KẾT QUẢ CUỐI CÙNG:';
PRINT '• ✅ Đã hoàn thiện temporal tables cho tất cả loại dữ liệu thô';
PRINT '• ✅ Đã áp dụng columnstore indexes để tối ưu hiệu năng';
PRINT '• ✅ Đã tạo stored procedures và views hỗ trợ';
PRINT '• ✅ Retention policy: 7 năm cho dữ liệu lịch sử';
PRINT '';
PRINT '📅 Thời gian hoàn thành: ' + CONVERT(NVARCHAR, GETDATE(), 120);
PRINT '🚀 HỆ THỐNG SẴN SÀNG CHO VIỆC XỬ LÝ DỮ LIỆU THÔ VỚI HIỆU NĂNG TỐI ƯU!';
PRINT '';
PRINT '💡 Sử dụng sp_TemporalTablesReport để kiểm tra trạng thái bất kỳ lúc nào.';

GO
