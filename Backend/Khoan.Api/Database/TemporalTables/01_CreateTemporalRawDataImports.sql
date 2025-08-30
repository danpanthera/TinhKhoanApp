-- =============================================
-- Script: 01_CreateTemporalRawDataImports.sql
-- Purpose: Create Temporal Tables for RawData Imports with Change Tracking
-- Author: System Architecture
-- Date: 2025-01-15
-- Notes: Optimized for 100K-200K records/day with full history tracking
-- =============================================

USE TinhKhoanDB;
GO

-- Drop existing tables if they exist (for development only)
IF OBJECT_ID('dbo.RawDataImports', 'U') IS NOT NULL
BEGIN
    ALTER TABLE dbo.RawDataImports SET (SYSTEM_VERSIONING = OFF);
    DROP TABLE IF EXISTS dbo.RawDataImports;
    DROP TABLE IF EXISTS dbo.RawDataImports_History;
END
GO

-- Create main RawDataImports table with temporal support
CREATE TABLE dbo.RawDataImports
(
    Id BIGINT IDENTITY(1,1) NOT NULL,
    ImportDate DATE NOT NULL,
    BranchCode NVARCHAR(10) NOT NULL,
    DepartmentCode NVARCHAR(10) NOT NULL,
    EmployeeCode NVARCHAR(20) NOT NULL,
    KpiCode NVARCHAR(20) NOT NULL,
    KpiValue DECIMAL(18,4) NOT NULL,
    Unit NVARCHAR(10) NULL,
    Target DECIMAL(18,4) NULL,
    Achievement DECIMAL(18,4) NULL,
    Score DECIMAL(5,2) NULL,
    Note NVARCHAR(500) NULL,
    
    -- Metadata fields
    ImportBatchId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NOT NULL DEFAULT 'SYSTEM',
    LastModifiedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    LastModifiedBy NVARCHAR(100) NOT NULL DEFAULT 'SYSTEM',
    IsDeleted BIT NOT NULL DEFAULT 0,
    
    -- Temporal columns (required for system-versioned tables)
    ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    
    CONSTRAINT PK_RawDataImports PRIMARY KEY CLUSTERED (Id),
    
    -- Period definition for temporal table
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
)
WITH 
(
    SYSTEM_VERSIONING = ON 
    (
        HISTORY_TABLE = dbo.RawDataImports_History,
        DATA_CONSISTENCY_CHECK = ON,
        HISTORY_RETENTION_PERIOD = 7 YEARS
    )
);
GO

-- Create optimized indexes for performance
-- 1. Covering index for common queries by date and branch
CREATE NONCLUSTERED INDEX IX_RawDataImports_ImportDate_BranchCode_Covering
ON dbo.RawDataImports (ImportDate, BranchCode, DepartmentCode)
INCLUDE (EmployeeCode, KpiCode, KpiValue, Score, Achievement, Target);
GO

-- 2. Index for employee-specific queries
CREATE NONCLUSTERED INDEX IX_RawDataImports_EmployeeCode_ImportDate
ON dbo.RawDataImports (EmployeeCode, ImportDate)
INCLUDE (KpiCode, KpiValue, Score);
GO

-- 3. Index for KPI analysis
CREATE NONCLUSTERED INDEX IX_RawDataImports_KpiCode_ImportDate
ON dbo.RawDataImports (KpiCode, ImportDate)
INCLUDE (BranchCode, EmployeeCode, KpiValue, Score);
GO

-- 4. Index for batch operations
CREATE NONCLUSTERED INDEX IX_RawDataImports_ImportBatchId
ON dbo.RawDataImports (ImportBatchId)
INCLUDE (ImportDate, CreatedDate);
GO

-- 5. Index for soft delete queries
CREATE NONCLUSTERED INDEX IX_RawDataImports_IsDeleted_ImportDate
ON dbo.RawDataImports (IsDeleted, ImportDate)
WHERE IsDeleted = 0;
GO

CREATE NONCLUSTERED INDEX IX_RawDataImports_Employee_Kpi
ON RawDataImports (EmployeeCode, KpiCode)
INCLUDE (ImportDate, BranchCode, Value, Unit);
GO

CREATE NONCLUSTERED INDEX IX_RawDataImports_Batch
ON RawDataImports (ImportBatchId)
INCLUDE (ImportDate, EmployeeCode, KpiCode, Value);
GO

-- 4. Create Filtered Index for Recent Data (Last 3 months)
CREATE NONCLUSTERED INDEX IX_RawDataImports_Recent
ON RawDataImports (ImportDate, EmployeeCode, KpiCode)
INCLUDE (BranchCode, Value, Unit, DataType)
WHERE ImportDate >= DATEADD(month, -3, GETDATE()) AND IsDeleted = 0;
GO

-- Create Columnstore Index for analytical queries on history table
CREATE CLUSTERED COLUMNSTORE INDEX CCI_RawDataImports_History
ON dbo.RawDataImports_History;
GO

-- Create Import Log table for batch tracking
CREATE TABLE dbo.ImportLogs
(
    Id BIGINT IDENTITY(1,1) NOT NULL,
    ImportBatchId UNIQUEIDENTIFIER NOT NULL,
    FileName NVARCHAR(255) NOT NULL,
    FileSize BIGINT NOT NULL,
    RecordCount INT NOT NULL,
    SuccessCount INT NOT NULL,
    ErrorCount INT NOT NULL,
    ImportStartTime DATETIME2 NOT NULL,
    ImportEndTime DATETIME2 NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'InProgress', -- InProgress, Completed, Failed, Cancelled
    ErrorMessage NVARCHAR(MAX) NULL,
    ImportedBy NVARCHAR(100) NOT NULL,
    
    -- Compression settings
    IsCompressed BIT NOT NULL DEFAULT 0,
    CompressionRatio DECIMAL(5,2) NULL,
    
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    
    CONSTRAINT PK_ImportLogs PRIMARY KEY CLUSTERED (Id)
);
GO

-- Index for Import Logs
CREATE NONCLUSTERED INDEX IX_ImportLogs_ImportBatchId
ON dbo.ImportLogs (ImportBatchId);
GO

CREATE NONCLUSTERED INDEX IX_ImportLogs_ImportStartTime_Status
ON dbo.ImportLogs (ImportStartTime, Status)
INCLUDE (FileName, RecordCount, SuccessCount, ErrorCount);
GO

-- Create summary statistics table for quick reporting
CREATE TABLE dbo.DailyImportSummary
(
    Id BIGINT IDENTITY(1,1) NOT NULL,
    SummaryDate DATE NOT NULL,
    BranchCode NVARCHAR(10) NOT NULL,
    TotalRecords INT NOT NULL,
    TotalEmployees INT NOT NULL,
    TotalKpiTypes INT NOT NULL,
    AverageScore DECIMAL(5,2) NULL,
    TopPerformerEmployee NVARCHAR(20) NULL,
    TopPerformerScore DECIMAL(5,2) NULL,
    
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    LastUpdatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    
    CONSTRAINT PK_DailyImportSummary PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_DailyImportSummary_Date_Branch UNIQUE (SummaryDate, BranchCode)
);
GO

-- Index for summary queries
CREATE NONCLUSTERED INDEX IX_DailyImportSummary_SummaryDate
ON dbo.DailyImportSummary (SummaryDate)
INCLUDE (BranchCode, TotalRecords, AverageScore);
GO

PRINT 'Temporal Tables created successfully for RawDataImports!';
PRINT 'Temporal Tables created successfully with optimized indexes for high-volume imports';
PRINT 'Next steps: Configure partitioning, archiving strategy, and stored procedures';
