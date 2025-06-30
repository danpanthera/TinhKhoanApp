-- 📊 Script tạo 3 bảng dữ liệu thô mới với Temporal Tables + Columnstore Indexes
-- Chạy script này trực tiếp trên SQL Server Management Studio

USE [TinhKhoanApp_DB];
GO

-- 💰 Tạo bảng ThuXLRR (Thu nợ đã XLRR) với Temporal Tables
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ThuXLRR')
BEGIN
    CREATE TABLE ThuXLRR (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
        ImportedDataRecordId INT NOT NULL,
        RawData NVARCHAR(MAX) NULL,
        ProcessedData NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        ModifiedAt DATETIME2 NOT NULL DEFAULT GETDATE(),

        -- Temporal Tables columns
        SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL,
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL,

        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),

        FOREIGN KEY (ImportedDataRecordId) REFERENCES ImportedDataRecords(Id)
    )
    WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.ThuXLRR_History));

    PRINT '✅ Tạo bảng ThuXLRR thành công';
END
ELSE
BEGIN
    PRINT '⚠️ Bảng ThuXLRR đã tồn tại';
END
GO

-- 🏦 Tạo bảng MSIT72_TSBD (Sao kê TSBD) với Temporal Tables
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MSIT72_TSBD')
BEGIN
    CREATE TABLE MSIT72_TSBD (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
        ImportedDataRecordId INT NOT NULL,
        RawData NVARCHAR(MAX) NULL,
        ProcessedData NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        ModifiedAt DATETIME2 NOT NULL DEFAULT GETDATE(),

        -- Temporal Tables columns
        SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL,
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL,

        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),

        FOREIGN KEY (ImportedDataRecordId) REFERENCES ImportedDataRecords(Id)
    )
    WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.MSIT72_TSBD_History));

    PRINT '✅ Tạo bảng MSIT72_TSBD thành công';
END
ELSE
BEGIN
    PRINT '⚠️ Bảng MSIT72_TSBD đã tồn tại';
END
GO

-- 🏦 Tạo bảng MSIT72_TSGH (Sao kê TSGH) với Temporal Tables
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MSIT72_TSGH')
BEGIN
    CREATE TABLE MSIT72_TSGH (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
        ImportedDataRecordId INT NOT NULL,
        RawData NVARCHAR(MAX) NULL,
        ProcessedData NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        ModifiedAt DATETIME2 NOT NULL DEFAULT GETDATE(),

        -- Temporal Tables columns
        SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL,
        SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL,

        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),

        FOREIGN KEY (ImportedDataRecordId) REFERENCES ImportedDataRecords(Id)
    )
    WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.MSIT72_TSGH_History));

    PRINT '✅ Tạo bảng MSIT72_TSGH thành công';
END
ELSE
BEGIN
    PRINT '⚠️ Bảng MSIT72_TSGH đã tồn tại';
END
GO

-- 🚀 Tạo Columnstore Indexes cho các bảng History để tối ưu hiệu năng
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_ThuXLRR_History')
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_ThuXLRR_History
    ON dbo.ThuXLRR_History;
    PRINT '✅ Tạo Columnstore Index cho ThuXLRR_History thành công';
END
ELSE
BEGIN
    PRINT '⚠️ Columnstore Index cho ThuXLRR_History đã tồn tại';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_MSIT72_TSBD_History')
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_MSIT72_TSBD_History
    ON dbo.MSIT72_TSBD_History;
    PRINT '✅ Tạo Columnstore Index cho MSIT72_TSBD_History thành công';
END
ELSE
BEGIN
    PRINT '⚠️ Columnstore Index cho MSIT72_TSBD_History đã tồn tại';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_MSIT72_TSGH_History')
BEGIN
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_MSIT72_TSGH_History
    ON dbo.MSIT72_TSGH_History;
    PRINT '✅ Tạo Columnstore Index cho MSIT72_TSGH_History thành công';
END
ELSE
BEGIN
    PRINT '⚠️ Columnstore Index cho MSIT72_TSGH_History đã tồn tại';
END
GO

-- 📈 Tạo các index thông thường cho bảng chính để query nhanh
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ThuXLRR_ImportedDataRecordId_CreatedAt')
BEGIN
    CREATE NONCLUSTERED INDEX IX_ThuXLRR_ImportedDataRecordId_CreatedAt
    ON ThuXLRR (ImportedDataRecordId, CreatedAt DESC)
    INCLUDE (RawData, ProcessedData);
    PRINT '✅ Tạo Index cho ThuXLRR thành công';
END
ELSE
BEGIN
    PRINT '⚠️ Index cho ThuXLRR đã tồn tại';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MSIT72_TSBD_ImportedDataRecordId_CreatedAt')
BEGIN
    CREATE NONCLUSTERED INDEX IX_MSIT72_TSBD_ImportedDataRecordId_CreatedAt
    ON MSIT72_TSBD (ImportedDataRecordId, CreatedAt DESC)
    INCLUDE (RawData, ProcessedData);
    PRINT '✅ Tạo Index cho MSIT72_TSBD thành công';
END
ELSE
BEGIN
    PRINT '⚠️ Index cho MSIT72_TSBD đã tồn tại';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MSIT72_TSGH_ImportedDataRecordId_CreatedAt')
BEGIN
    CREATE NONCLUSTERED INDEX IX_MSIT72_TSGH_ImportedDataRecordId_CreatedAt
    ON MSIT72_TSGH (ImportedDataRecordId, CreatedAt DESC)
    INCLUDE (RawData, ProcessedData);
    PRINT '✅ Tạo Index cho MSIT72_TSGH thành công';
END
ELSE
BEGIN
    PRINT '⚠️ Index cho MSIT72_TSGH đã tồn tại';
END
GO

PRINT '🎉 Hoàn thành tạo 3 bảng dữ liệu thô mới với Temporal Tables + Columnstore Indexes';
GO
