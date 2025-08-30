-- REBUILD GL41 TO PARTITIONED COLUMNSTORE STRUCTURE
-- Chuyển từ Temporal Table sang Partitioned Columnstore như GL01/GL02
-- Tương tự như GL01 và GL02: hoàn toàn rebuild với cấu trúc mới

USE TinhKhoanDB;
GO

PRINT '=== REBUILD GL41 TO PARTITIONED COLUMNSTORE ==='

-- BƯỚC 1: Backup dữ liệu hiện tại
PRINT 'Backup dữ liệu GL41 hiện tại...'
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GL41_Backup]') AND type in (N'U'))
BEGIN
    DROP TABLE [dbo].[GL41_Backup]
END
GO

SELECT * INTO [dbo].[GL41_Backup] FROM [dbo].[GL41]
GO

PRINT 'Đã backup toàn bộ dữ liệu GL41'
GO

-- BƯỚC 2: Drop constraints và indexes
PRINT 'Xóa constraints và indexes...'
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_GL41_Department]') AND parent_object_id = OBJECT_ID(N'[dbo].[GL41]'))
    ALTER TABLE [dbo].[GL41] DROP CONSTRAINT [FK_GL41_Department]
GO

-- Drop indexes nếu có
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[GL41]') AND name = N'IX_GL41_NGAY_DL')
    DROP INDEX [IX_GL41_NGAY_DL] ON [dbo].[GL41]
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[GL41]') AND name = N'IX_GL41_Department_Date')
    DROP INDEX [IX_GL41_Department_Date] ON [dbo].[GL41]
GO

-- BƯỚC 3: Drop bảng GL41 hiện tại (kể cả temporal)
PRINT 'Xóa bảng GL41 hiện tại...'
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GL41]') AND type in (N'U'))
BEGIN
    -- Tắt system versioning nếu là temporal table
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GL41' AND temporal_type_desc = 'SYSTEM_VERSIONED_TEMPORAL_TABLE')
    BEGIN
        ALTER TABLE [dbo].[GL41] SET (SYSTEM_VERSIONING = OFF)

        -- Drop history table nếu có
        IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GL41_History]') AND type in (N'U'))
        BEGIN
            DROP TABLE [dbo].[GL41_History]
        END
    END

    DROP TABLE [dbo].[GL41]
END
GO

PRINT 'Đã xóa bảng GL41 cũ'

-- BƯỚC 4: Tạo bảng GL41 mới với cấu trúc Partitioned Columnstore
PRINT 'Tạo bảng GL41 mới với cấu trúc Partitioned Columnstore...'

CREATE TABLE [dbo].[GL41] (
    -- Primary Key
    [Id] BIGINT IDENTITY(1,1) NOT NULL,

    -- Core Data Column (ngày dữ liệu)
    [NGAY_DL] DATETIME2(0) NOT NULL,

    -- 13 Business Columns theo thứ tự CSV
    [MA_DVCS] NVARCHAR(20) NOT NULL,           -- 1: Mã đơn vị cơ sở
    [TEN_DVCS] NVARCHAR(200) NOT NULL,         -- 2: Tên đơn vị cơ sở
    [MA_TK] NVARCHAR(20) NOT NULL,             -- 3: Mã tài khoản
    [TEN_TK] NVARCHAR(200) NOT NULL,           -- 4: Tên tài khoản
    [SO_DU_DAU_NO] DECIMAL(18,2) NOT NULL,     -- 5: Số dư đầu nợ
    [SO_DU_DAU_CO] DECIMAL(18,2) NOT NULL,     -- 6: Số dư đầu có
    [PHAT_SINH_NO] DECIMAL(18,2) NOT NULL,     -- 7: Phát sinh nợ
    [PHAT_SINH_CO] DECIMAL(18,2) NOT NULL,     -- 8: Phát sinh có
    [SO_DU_CUOI_NO] DECIMAL(18,2) NOT NULL,    -- 9: Số dư cuối nợ
    [SO_DU_CUOI_CO] DECIMAL(18,2) NOT NULL,    -- 10: Số dư cuối có
    [LOAI_TK] NVARCHAR(10) NOT NULL,           -- 11: Loại tài khoản
    [CAP_TK] INT NOT NULL,                     -- 12: Cấp tài khoản
    [TT_TK] NVARCHAR(10) NOT NULL,             -- 13: Trạng thái tài khoản

    -- 4 System Columns
    [CreatedDate] DATETIME2(0) NOT NULL DEFAULT GETDATE(),
    [CreatedBy] NVARCHAR(100) NOT NULL DEFAULT N'SYSTEM',
    [LastModifiedDate] DATETIME2(0) NOT NULL DEFAULT GETDATE(),
    [LastModifiedBy] NVARCHAR(100) NOT NULL DEFAULT N'SYSTEM',

    CONSTRAINT [PK_GL41] PRIMARY KEY ([Id])
);
GO

PRINT 'Đã tạo bảng GL41 mới'

-- BƯỚC 5: Tạo Columnstore Index (Partitioned)
PRINT 'Tạo Clustered Columnstore Index...'
CREATE CLUSTERED COLUMNSTORE INDEX [CCI_GL41] ON [dbo].[GL41]
GO

PRINT 'Đã tạo Clustered Columnstore Index cho GL41'

-- BƯỚC 6: Tạo indexes cho performance
PRINT 'Tạo các indexes bổ sung...'

-- Index cho NGAY_DL (quan trọng cho partition)
CREATE NONCLUSTERED INDEX [IX_GL41_NGAY_DL] ON [dbo].[GL41]
(
    [NGAY_DL] ASC
)
GO

-- Index cho MA_DVCS + NGAY_DL
CREATE NONCLUSTERED INDEX [IX_GL41_Department_Date] ON [dbo].[GL41]
(
    [MA_DVCS] ASC,
    [NGAY_DL] ASC
)
GO

-- Index cho MA_TK
CREATE NONCLUSTERED INDEX [IX_GL41_MA_TK] ON [dbo].[GL41]
(
    [MA_TK] ASC
)
GO

PRINT 'Đã tạo các indexes cho GL41'

-- BƯỚC 7: Khôi phục dữ liệu từ backup (nếu có)
PRINT 'Khôi phục dữ liệu từ backup...'
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GL41_Backup]') AND type in (N'U'))
BEGIN
    INSERT INTO [dbo].[GL41] (
        [NGAY_DL], [MA_DVCS], [TEN_DVCS], [MA_TK], [TEN_TK],
        [SO_DU_DAU_NO], [SO_DU_DAU_CO], [PHAT_SINH_NO], [PHAT_SINH_CO],
        [SO_DU_CUOI_NO], [SO_DU_CUOI_CO], [LOAI_TK], [CAP_TK], [TT_TK],
        [CreatedDate], [CreatedBy], [LastModifiedDate], [LastModifiedBy]
    )
    SELECT
        -- Map từ cấu trúc cũ sang mới, lấy các columns tương ứng
        ISNULL([NGAY_DL], '2024-12-31') as [NGAY_DL],
        ISNULL([MA_DVCS], N'') as [MA_DVCS],
        ISNULL([TEN_DVCS], N'') as [TEN_DVCS],
        ISNULL([MA_TK], N'') as [MA_TK],
        ISNULL([TEN_TK], N'') as [TEN_TK],
        ISNULL([SO_DU_DAU_NO], 0) as [SO_DU_DAU_NO],
        ISNULL([SO_DU_DAU_CO], 0) as [SO_DU_DAU_CO],
        ISNULL([PHAT_SINH_NO], 0) as [PHAT_SINH_NO],
        ISNULL([PHAT_SINH_CO], 0) as [PHAT_SINH_CO],
        ISNULL([SO_DU_CUOI_NO], 0) as [SO_DU_CUOI_NO],
        ISNULL([SO_DU_CUOI_CO], 0) as [SO_DU_CUOI_CO],
        ISNULL([LOAI_TK], N'') as [LOAI_TK],
        ISNULL([CAP_TK], 0) as [CAP_TK],
        ISNULL([TT_TK], N'') as [TT_TK],
        GETDATE() as [CreatedDate],
        N'MIGRATION' as [CreatedBy],
        GETDATE() as [LastModifiedDate],
        N'MIGRATION' as [LastModifiedBy]
    FROM [dbo].[GL41_Backup]

    DECLARE @RecordCount INT = @@ROWCOUNT
    PRINT CONCAT('Đã khôi phục ', @RecordCount, ' records từ backup')
END
ELSE
BEGIN
    PRINT 'Không có dữ liệu backup để khôi phục'
END
GO

-- BƯỚC 8: Thống kê kết quả
PRINT '=== THỐNG KÊ KẾT QUẢ ==='
SELECT
    'GL41' as TableName,
    COUNT(*) as TotalRecords,
    MIN(NGAY_DL) as MinDate,
    MAX(NGAY_DL) as MaxDate,
    COUNT(DISTINCT MA_DVCS) as UniqueUnits
FROM [dbo].[GL41]

-- Kiểm tra columnstore index
SELECT
    t.name as TableName,
    i.name as IndexName,
    i.type_desc as IndexType
FROM sys.tables t
JOIN sys.indexes i ON t.object_id = i.object_id
WHERE t.name = 'GL41'
ORDER BY i.type_desc

PRINT '=== HOÀN THÀNH REBUILD GL41 PARTITIONED COLUMNSTORE ==='
GO
