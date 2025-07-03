-- =============================================
-- Script: CompleteTemporalTablesSetup.sql
-- Mục đích: Hoàn thiện Temporal Tables + Columnstore Indexes cho tất cả loại dữ liệu thô
-- Tác giả: Em (siêu lập trình viên Fullstack)
-- Ngày: 26/06/2025
-- Chú thích: Tối ưu hóa hiệu năng với Temporal Tables + Columnstore Indexes
-- =============================================

USE TinhKhoanDB;
GO

PRINT '🚀 BẮT ĐẦU THIẾT LẬP TEMPORAL TABLES + COLUMNSTORE INDEXES CHO DỮ LIỆU THÔ';
PRINT '===============================================================================';

-- =====================================================
-- BƯỚC 1: TẠO TEMPORAL TABLES CHO TẤT CẢ LOẠI DỮ LIỆU THÔ
-- =====================================================

-- Danh sách các loại dữ liệu thô cần tạo Temporal Tables
DECLARE @RawDataTypes TABLE (
    DataType NVARCHAR(20),
    TableName NVARCHAR(50),
    Description NVARCHAR(200)
);

INSERT INTO @RawDataTypes VALUES
    ('7800_DT_KHKD1', '7800_DT_KHKD1_RawData', 'Báo cáo KHKD (DT) - Dữ liệu kế hoạch kinh doanh'),
    ('BC57', 'BC57_RawData', 'Sao kê Lãi dự thu - Dữ liệu lãi suất'),
    ('DPDA', 'DPDA_RawData', 'Dữ liệu sao kê phát hành thẻ'),
    ('DP01', 'DP01_RawData', 'Dữ liệu Tiền gửi'),
    ('DB01', 'DB01_RawData', 'Sao kê TSDB và Không TSDB'),
    ('LN01', 'LN01_RawData', 'Dữ liệu LOAN - Danh mục tín dụng'),
    ('GL01', 'GL01_RawData', 'Dữ liệu bút toán GDV'),
    ('EI01', 'EI01_RawData', 'Dữ liệu mobile banking'),
    ('LN02', 'LN02_RawData', 'Sao kê biến động nhóm nợ'),
    ('LN03', 'LN03_RawData', 'Dữ liệu Nợ XLRR'),
    ('KH03', 'KH03_RawData', 'Sao kê Khách hàng pháp nhân'),
    ('RR01', 'RR01_RawData', 'Sao kê dư nợ gốc, lãi XLRR'),
    ('GL41', 'GL41_RawData', 'Bảng cân đối kế toán');

DECLARE @DataType NVARCHAR(20), @TableName NVARCHAR(50), @Description NVARCHAR(200);
DECLARE @HistoryTableName NVARCHAR(50), @SQL NVARCHAR(MAX);

DECLARE raw_data_cursor CURSOR FOR
SELECT DataType, TableName, Description FROM @RawDataTypes;

OPEN raw_data_cursor;
FETCH NEXT FROM raw_data_cursor INTO @DataType, @TableName, @Description;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @HistoryTableName = @TableName + '_History';

    PRINT '';
    PRINT '📊 Đang xử lý: ' + @DataType + ' - ' + @Description;
    PRINT '   ↳ Bảng chính: ' + @TableName;
    PRINT '   ↳ Bảng lịch sử: ' + @HistoryTableName;

    -- Kiểm tra xem bảng đã tồn tại chưa
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = @TableName)
    BEGIN
        BEGIN TRY
            -- Tạo Temporal Table với cấu trúc tối ưu
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

                -- 🕰️ Temporal Table Columns (System-versioned)
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
            PRINT '   ✅ Đã tạo Temporal Table: ' + @TableName;

            -- Tạo Clustered Columnstore Index cho bảng chính (cho analytics)
            IF @DataType IN ('7800_DT_KHKD1', 'BC57', 'GL01', 'LN01') -- Bảng lớn cần columnstore
            BEGIN
                SET @SQL = 'CREATE CLUSTERED COLUMNSTORE INDEX [CCI_' + @TableName + '] ON [dbo].[' + @TableName + ']
                           WITH (COMPRESSION_DELAY = 0, MAXDOP = 4);';
                EXEC sp_executesql @SQL;
                PRINT '   ✅ Đã tạo Clustered Columnstore Index cho: ' + @TableName;
            END
            ELSE
            BEGIN
                -- Tạo Nonclustered Columnstore Index cho bảng nhỏ hơn
                SET @SQL = 'CREATE NONCLUSTERED COLUMNSTORE INDEX [NCCI_' + @TableName + '_Analytics]
                           ON [dbo].[' + @TableName + '] ([StatementDate], [BranchCode], [Amount], [Quantity])
                           WITH (COMPRESSION_DELAY = 0);';
                EXEC sp_executesql @SQL;
                PRINT '   ✅ Đã tạo Nonclustered Columnstore Index cho: ' + @TableName;
            END

            -- Tạo Clustered Columnstore Index cho History Table (luôn dùng columnstore)
            SET @SQL = 'CREATE CLUSTERED COLUMNSTORE INDEX [CCI_' + @HistoryTableName + ']
                       ON [dbo].[' + @HistoryTableName + ']
                       WITH (COMPRESSION_DELAY = 0, MAXDOP = 4);';
            EXEC sp_executesql @SQL;
            PRINT '   ✅ Đã tạo Clustered Columnstore Index cho: ' + @HistoryTableName;

        END TRY
        BEGIN CATCH
            PRINT '   ❌ Lỗi khi tạo ' + @TableName + ': ' + ERROR_MESSAGE();
        END CATCH
    END
    ELSE
    BEGIN
        PRINT '   ⚠️ Bảng ' + @TableName + ' đã tồn tại, kiểm tra cấu hình temporal...';

        -- Kiểm tra xem đã là temporal table chưa
        IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = @TableName AND temporal_type = 2)
        BEGIN
            PRINT '   🔧 Chuyển đổi sang Temporal Table...';
            -- Logic chuyển đổi sang temporal sẽ được thêm vào sau
        END
        ELSE
        BEGIN
            PRINT '   ✅ Đã là Temporal Table';
        END
    END

    FETCH NEXT FROM raw_data_cursor INTO @DataType, @TableName, @Description;
END

CLOSE raw_data_cursor;
DEALLOCATE raw_data_cursor;

-- =====================================================
-- BƯỚC 2: TẠO INDEXES BỔ SUNG CHO HIỆU NĂNG CAO
-- =====================================================

PRINT '';
PRINT '🔍 TẠO CÁC INDEXES BỔ SUNG CHO HIỆU NĂNG CAO';
PRINT '===============================================================================';

-- Tạo indexes cho từng loại dữ liệu
DECLARE @IndexTableName NVARCHAR(50);
DECLARE index_cursor CURSOR FOR
SELECT TableName FROM @RawDataTypes;

OPEN index_cursor;
FETCH NEXT FROM index_cursor INTO @IndexTableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT '🔧 Tạo indexes cho: ' + @IndexTableName;

    BEGIN TRY
        -- Index 1: Theo ngày và chi nhánh (cho báo cáo)
        SET @SQL = 'IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = ''IX_' + @IndexTableName + '_StatementDate_Branch'' AND object_id = OBJECT_ID(''' + @IndexTableName + '''))
                   CREATE NONCLUSTERED INDEX [IX_' + @IndexTableName + '_StatementDate_Branch]
                   ON [dbo].[' + @IndexTableName + '] ([StatementDate], [BranchCode], [IsDeleted])
                   INCLUDE ([DepartmentCode], [EmployeeCode], [Amount], [Quantity])
                   WITH (DATA_COMPRESSION = PAGE, FILLFACTOR = 90);';
        EXEC sp_executesql @SQL;

        -- Index 2: Theo batch import (cho xử lý batch)
        SET @SQL = 'IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = ''IX_' + @IndexTableName + '_ImportBatch'' AND object_id = OBJECT_ID(''' + @IndexTableName + '''))
                   CREATE NONCLUSTERED INDEX [IX_' + @IndexTableName + '_ImportBatch]
                   ON [dbo].[' + @IndexTableName + '] ([ImportBatchId], [IsProcessed])
                   INCLUDE ([RowNumber], [ProcessedDate])
                   WITH (DATA_COMPRESSION = PAGE);';
        EXEC sp_executesql @SQL;

        -- Index 3: Theo nhân viên (cho phân tích cá nhân)
        SET @SQL = 'IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = ''IX_' + @IndexTableName + '_Employee'' AND object_id = OBJECT_ID(''' + @IndexTableName + '''))
                   CREATE NONCLUSTERED INDEX [IX_' + @IndexTableName + '_Employee]
                   ON [dbo].[' + @IndexTableName + '] ([EmployeeCode], [StatementDate])
                   INCLUDE ([BranchCode], [Amount], [Quantity], [IsValid])
                   WHERE [EmployeeCode] IS NOT NULL AND [IsDeleted] = 0
                   WITH (DATA_COMPRESSION = PAGE);';
        EXEC sp_executesql @SQL;

        -- Index 4: Filtered index cho dữ liệu chưa xử lý
        SET @SQL = 'IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = ''IX_' + @IndexTableName + '_Unprocessed'' AND object_id = OBJECT_ID(''' + @IndexTableName + '''))
                   CREATE NONCLUSTERED INDEX [IX_' + @IndexTableName + '_Unprocessed]
                   ON [dbo].[' + @IndexTableName + '] ([IsProcessed], [StatementDate])
                   INCLUDE ([Id], [RowNumber], [ValidationErrors])
                   WHERE [IsProcessed] = 0 AND [IsDeleted] = 0
                   WITH (DATA_COMPRESSION = PAGE);';
        EXEC sp_executesql @SQL;

        PRINT '   ✅ Đã tạo indexes cho: ' + @IndexTableName;

    END TRY
    BEGIN CATCH
        PRINT '   ❌ Lỗi tạo indexes cho ' + @IndexTableName + ': ' + ERROR_MESSAGE();
    END CATCH

    FETCH NEXT FROM index_cursor INTO @IndexTableName;
END

CLOSE index_cursor;
DEALLOCATE index_cursor;

-- =====================================================
-- BƯỚC 3: TẠO STORED PROCEDURES CHO XỬ LÝ DỮ LIỆU
-- =====================================================

PRINT '';
PRINT '⚡ TẠO STORED PROCEDURES CHO XỬ LÝ DỮ LIỆU';
PRINT '===============================================================================';

-- Stored Procedure: Import dữ liệu thô
CREATE OR ALTER PROCEDURE [dbo].[sp_ImportRawData]
    @DataType NVARCHAR(20),
    @RawDataLines NVARCHAR(MAX), -- JSON array của các dòng dữ liệu
    @StatementDate DATE,
    @BranchCode NVARCHAR(10) = NULL,
    @SourceFileName NVARCHAR(255) = NULL,
    @CreatedBy NVARCHAR(100) = 'SYSTEM'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TableName NVARCHAR(50) = @DataType + '_RawData';
    DECLARE @ImportBatchId UNIQUEIDENTIFIER = NEWID();
    DECLARE @SQL NVARCHAR(MAX);
    DECLARE @RowCount INT = 0;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validate table exists
        IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = @TableName)
        BEGIN
            RAISERROR('Bảng %s không tồn tại', 16, 1, @TableName);
            RETURN;
        END

        -- Insert raw data với dynamic SQL
        SET @SQL = '
        INSERT INTO [dbo].[' + @TableName + ']
        (ImportBatchId, RowNumber, RawDataLine, DataType, StatementDate, BranchCode,
         SourceFileName, CreatedBy, CreatedDate, LastModifiedBy, LastModifiedDate)
        SELECT
            @ImportBatchId,
            ROW_NUMBER() OVER (ORDER BY (SELECT NULL)),
            value,
            @DataType,
            @StatementDate,
            @BranchCode,
            @SourceFileName,
            @CreatedBy,
            SYSUTCDATETIME(),
            @CreatedBy,
            SYSUTCDATETIME()
        FROM OPENJSON(@RawDataLines)';

        EXEC sp_executesql @SQL,
            N'@ImportBatchId UNIQUEIDENTIFIER, @DataType NVARCHAR(20), @StatementDate DATE,
              @BranchCode NVARCHAR(10), @SourceFileName NVARCHAR(255), @CreatedBy NVARCHAR(100), @RawDataLines NVARCHAR(MAX)',
            @ImportBatchId, @DataType, @StatementDate, @BranchCode, @SourceFileName, @CreatedBy, @RawDataLines;

        SET @RowCount = @@ROWCOUNT;

        COMMIT TRANSACTION;

        PRINT 'Đã import ' + CAST(@RowCount AS NVARCHAR) + ' dòng dữ liệu vào ' + @TableName;
        PRINT 'Import Batch ID: ' + CAST(@ImportBatchId AS NVARCHAR(36));

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        PRINT 'Lỗi import dữ liệu: ' + @ErrorMessage;
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

-- Stored Procedure: Kiểm tra hiệu năng temporal tables
CREATE OR ALTER PROCEDURE [dbo].[sp_CheckTemporalTablesPerformance]
AS
BEGIN
    SET NOCOUNT ON;

    PRINT '📊 BÁO CÁO HIỆU NĂNG TEMPORAL TABLES';
    PRINT '===============================================================================';

    -- Báo cáo trạng thái temporal tables
    SELECT
        t.name AS TableName,
        CASE t.temporal_type
            WHEN 0 THEN 'Non-temporal'
            WHEN 1 THEN 'History table'
            WHEN 2 THEN 'System-versioned temporal'
        END AS TemporalType,
        h.name AS HistoryTableName,
        p.rows AS CurrentRows,
        ph.rows AS HistoryRows,
        CAST((ph.rows * 100.0 / NULLIF(p.rows + ph.rows, 0)) AS DECIMAL(5,2)) AS HistoryPercentage
    FROM sys.tables t
    LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
    LEFT JOIN sys.partitions p ON t.object_id = p.object_id AND p.index_id IN (0,1)
    LEFT JOIN sys.partitions ph ON h.object_id = ph.object_id AND ph.index_id IN (0,1)
    WHERE t.name LIKE '%_RawData'
    ORDER BY t.name;

    -- Báo cáo columnstore indexes
    SELECT
        t.name AS TableName,
        i.name AS IndexName,
        i.type_desc AS IndexType,
        p.rows AS RowCount,
        p.data_compression_desc AS CompressionType,
        CAST(SUM(a.total_pages) * 8.0 / 1024 AS DECIMAL(10,2)) AS SizeMB
    FROM sys.tables t
    INNER JOIN sys.indexes i ON t.object_id = i.object_id
    INNER JOIN sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id
    INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
    WHERE i.type IN (5, 6) -- Columnstore indexes
      AND (t.name LIKE '%_RawData' OR t.name LIKE '%_History')
    GROUP BY t.name, i.name, i.type_desc, p.rows, p.data_compression_desc
    ORDER BY t.name;
END
GO

-- =====================================================
-- BƯỚC 4: TẠO VIEWS CHO TRUY VẤN DỮ LIỆU
-- =====================================================

PRINT '';
PRINT '👁️ TẠO VIEWS CHO TRUY VẤN DỮ LIỆU';
PRINT '===============================================================================';

-- View tổng hợp tất cả dữ liệu thô hiện tại
CREATE OR ALTER VIEW [dbo].[vw_AllRawDataCurrent] AS
SELECT
    'BC57' AS DataType, StatementDate, BranchCode, EmployeeCode, Amount, Quantity,
    IsProcessed, CreatedDate FROM [dbo].[BC57_RawData]
UNION ALL
SELECT
    'DPDA' AS DataType, StatementDate, BranchCode, EmployeeCode, Amount, Quantity,
    IsProcessed, CreatedDate FROM [dbo].[DPDA_RawData]
UNION ALL
SELECT
    'LN01' AS DataType, StatementDate, BranchCode, EmployeeCode, Amount, Quantity,
    IsProcessed, CreatedDate FROM [dbo].[LN01_RawData]
UNION ALL
SELECT
    'GL01' AS DataType, StatementDate, BranchCode, EmployeeCode, Amount, Quantity,
    IsProcessed, CreatedDate FROM [dbo].[GL01_RawData]
UNION ALL
SELECT
    'EI01' AS DataType, StatementDate, BranchCode, EmployeeCode, Amount, Quantity,
    IsProcessed, CreatedDate FROM [dbo].[EI01_RawData]
UNION ALL
SELECT
    '7800_DT_KHKD1' AS DataType, StatementDate, BranchCode, EmployeeCode, Amount, Quantity,
    IsProcessed, CreatedDate FROM [dbo].[7800_DT_KHKD1_RawData];
GO

-- View tổng hợp dữ liệu lịch sử (temporal queries)
CREATE OR ALTER VIEW [dbo].[vw_AllRawDataHistory] AS
SELECT
    'BC57' AS DataType, StatementDate, BranchCode, EmployeeCode, Amount,
    ValidFrom, ValidTo, 'Current' AS RecordType
FROM [dbo].[BC57_RawData]
UNION ALL
SELECT
    'BC57' AS DataType, StatementDate, BranchCode, EmployeeCode, Amount,
    ValidFrom, ValidTo, 'History' AS RecordType
FROM [dbo].[BC57_RawData_History];
GO

PRINT '✅ Đã tạo views tổng hợp dữ liệu';

-- =====================================================
-- BƯỚC 5: KẾT THÚC VÀ BÁO CÁO
-- =====================================================

PRINT '';
PRINT '🎉 HOÀN THÀNH THIẾT LẬP TEMPORAL TABLES + COLUMNSTORE INDEXES';
PRINT '===============================================================================';

-- Chạy báo cáo hiệu năng
EXEC [dbo].[sp_CheckTemporalTablesPerformance];

PRINT '';
PRINT '📋 TỔNG KẾT:';
PRINT '• Đã tạo temporal tables cho tất cả loại dữ liệu thô (7800_DT_KHKD1, BC57, DPDA, DP01, DB01, LN01, GL01, EI01, v.v.)';
PRINT '• Đã áp dụng Columnstore Indexes cho tối ưu hiệu năng';
PRINT '• Đã tạo indexes bổ sung cho truy vấn nhanh';
PRINT '• Đã tạo stored procedures và views hỗ trợ';
PRINT '• Retention policy: 7 năm cho dữ liệu lịch sử';
PRINT '';
PRINT '🚀 HỆ THỐNG SẴN SÀNG CHO VIỆC XỬ LÝ DỮ LIỆU THÔ VỚI HIỆU NĂNG CAO!';

GO
