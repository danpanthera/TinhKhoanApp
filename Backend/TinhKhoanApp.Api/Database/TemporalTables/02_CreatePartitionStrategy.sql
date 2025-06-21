-- =============================================
-- Script: 02_CreatePartitionStrategy.sql
-- Purpose: Create Partition Strategy for High-Volume Temporal Tables
-- Author: System Architecture
-- Date: 2025-01-15
-- Notes: Monthly partitioning optimized for 100K-200K records/day
-- =============================================

USE TinhKhoanDB;
GO

-- 1. Create Partition Function for Monthly Partitions
-- This will create partitions for each month starting from 2025
CREATE PARTITION FUNCTION pf_Monthly_ImportDate (DATE)
AS RANGE RIGHT FOR VALUES (
    '2025-01-01', '2025-02-01', '2025-03-01', '2025-04-01', '2025-05-01', '2025-06-01',
    '2025-07-01', '2025-08-01', '2025-09-01', '2025-10-01', '2025-11-01', '2025-12-01',
    '2026-01-01', '2026-02-01', '2026-03-01', '2026-04-01', '2026-05-01', '2026-06-01',
    '2026-07-01', '2026-08-01', '2026-09-01', '2026-10-01', '2026-11-01', '2026-12-01',
    '2027-01-01', '2027-02-01', '2027-03-01', '2027-04-01', '2027-05-01', '2027-06-01',
    '2027-07-01', '2027-08-01', '2027-09-01', '2027-10-01', '2027-11-01', '2027-12-01'
);
GO

-- 2. Create Partition Scheme
-- All partitions will be placed on PRIMARY filegroup for now
-- In production, you might want to create multiple filegroups for better performance
CREATE PARTITION SCHEME ps_Monthly_ImportDate
AS PARTITION pf_Monthly_ImportDate
ALL TO ([PRIMARY]);
GO

-- 3. Create Partitioned Archive Table for Old Data
-- This table will store archived data older than 2 years
CREATE TABLE dbo.RawDataImports_Archive
(
    Id BIGINT NOT NULL,
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
    ImportBatchId UNIQUEIDENTIFIER NOT NULL,
    CreatedDate DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(100) NOT NULL,
    LastModifiedDate DATETIME2 NOT NULL,
    LastModifiedBy NVARCHAR(100) NOT NULL,
    
    -- Archive metadata
    ArchivedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ArchivedBy NVARCHAR(100) NOT NULL DEFAULT 'SYSTEM',
    
    CONSTRAINT PK_RawDataImports_Archive PRIMARY KEY CLUSTERED (Id, ImportDate),
    
    -- Check constraint to ensure only old data is archived
    CONSTRAINT CK_RawDataImports_Archive_ImportDate 
        CHECK (ImportDate < DATEADD(YEAR, -2, GETDATE()))
) ON ps_Monthly_ImportDate(ImportDate);
GO

-- 4. Create Clustered Columnstore Index on Archive Table
CREATE CLUSTERED COLUMNSTORE INDEX CCI_RawDataImports_Archive
ON dbo.RawDataImports_Archive;
GO

-- 5. Create a procedure to add new partitions automatically
CREATE PROCEDURE sp_AddNewMonthlyPartition
    @TargetDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @FirstDayOfMonth DATE = DATEFROMPARTS(YEAR(@TargetDate), MONTH(@TargetDate), 1);
    DECLARE @NextMonth DATE = DATEADD(MONTH, 1, @FirstDayOfMonth);
    
    -- Check if partition already exists
    IF NOT EXISTS (
        SELECT 1 FROM sys.partition_range_values prv
        INNER JOIN sys.partition_functions pf ON prv.function_id = pf.function_id
        WHERE pf.name = 'pf_Monthly_ImportDate'
        AND CAST(prv.value AS DATE) = @NextMonth
    )
    BEGIN
        -- Add new partition boundary
        ALTER PARTITION SCHEME ps_Monthly_ImportDate
        NEXT USED [PRIMARY];
        
        ALTER PARTITION FUNCTION pf_Monthly_ImportDate()
        SPLIT RANGE (@NextMonth);
        
        PRINT 'Added new partition for: ' + CAST(@NextMonth AS NVARCHAR(10));
    END
    ELSE
    BEGIN
        PRINT 'Partition already exists for: ' + CAST(@NextMonth AS NVARCHAR(10));
    END
END;
GO

-- 6. Create a procedure to remove old partitions (for archiving)
CREATE PROCEDURE sp_RemoveOldPartition
    @PartitionDate DATE,
    @ArchiveData BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SQL NVARCHAR(MAX);
    
    -- Archive data if requested
    IF @ArchiveData = 1
    BEGIN
        -- Move data from history table to archive
        INSERT INTO dbo.RawDataImports_Archive
        SELECT 
            Id, ImportDate, BranchCode, DepartmentCode, EmployeeCode, KpiCode,
            KpiValue, Unit, Target, Achievement, Score, Note,
            ImportBatchId, CreatedDate, CreatedBy, LastModifiedDate, LastModifiedBy, IsDeleted,
            SYSUTCDATETIME(), 'SYSTEM'
        FROM dbo.RawDataImports_History
        WHERE ImportDate < @PartitionDate;
        
        PRINT 'Archived ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' rows to archive table';
    END
    
    -- Remove the partition boundary
    ALTER PARTITION FUNCTION pf_Monthly_ImportDate()
    MERGE RANGE (@PartitionDate);
    
    PRINT 'Removed partition for: ' + CAST(@PartitionDate AS NVARCHAR(10));
END;
GO

-- 7. Create a job to automatically maintain partitions
-- This should be scheduled to run monthly
CREATE PROCEDURE sp_MaintainMonthlyPartitions
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Today DATE = CAST(GETDATE() AS DATE);
    DECLARE @NextMonth DATE = DATEADD(MONTH, 1, @Today);
    DECLARE @TwoYearsAgo DATE = DATEADD(YEAR, -2, @Today);
    
    -- Add next month partition
    EXEC sp_AddNewMonthlyPartition @NextMonth;
    
    -- Remove partitions older than 2 years (archive data first)
    DECLARE @OldPartitionDate DATE = DATEFROMPARTS(YEAR(@TwoYearsAgo), MONTH(@TwoYearsAgo), 1);
    
    -- Only remove if we have data to archive
    IF EXISTS (
        SELECT 1 FROM dbo.RawDataImports_History 
        WHERE ImportDate < @OldPartitionDate
    )
    BEGIN
        EXEC sp_RemoveOldPartition @OldPartitionDate, 1;
    END
    
    PRINT 'Partition maintenance completed successfully';
END;
GO

-- 8. Create Statistics Management for Partitioned Tables
CREATE PROCEDURE sp_UpdatePartitionedTableStats
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Update statistics on current table
    UPDATE STATISTICS dbo.RawDataImports;
    
    -- Update statistics on history table
    UPDATE STATISTICS dbo.RawDataImports_History;
    
    -- Update statistics on archive table
    UPDATE STATISTICS dbo.RawDataImports_Archive;
    
    PRINT 'Statistics updated for all partitioned tables';
END;
GO

-- 9. Create Partition Health Check Procedure
CREATE PROCEDURE sp_CheckPartitionHealth
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check partition distribution
    SELECT 
        p.partition_number,
        p.rows,
        CASE 
            WHEN p.rows = 0 THEN 'Empty'
            WHEN p.rows < 100000 THEN 'Low'
            WHEN p.rows BETWEEN 100000 AND 500000 THEN 'Normal'
            WHEN p.rows > 500000 THEN 'High'
        END AS [Status],
        CAST(prv.value AS DATE) AS PartitionBoundary
    FROM sys.partitions p
    INNER JOIN sys.objects o ON p.object_id = o.object_id
    LEFT JOIN sys.partition_range_values prv ON p.partition_number = prv.boundary_id + 1
    WHERE o.name = 'RawDataImports'
    ORDER BY p.partition_number;
    
    -- Check for partitions that need attention
    DECLARE @HighVolumePartitions INT = (
        SELECT COUNT(*)
        FROM sys.partitions p
        INNER JOIN sys.objects o ON p.object_id = o.object_id
        WHERE o.name = 'RawDataImports' AND p.rows > 500000
    );
    
    IF @HighVolumePartitions > 0
    BEGIN
        PRINT 'WARNING: ' + CAST(@HighVolumePartitions AS NVARCHAR(10)) + ' partition(s) have high volume (>500K rows)';
        PRINT 'Consider reviewing partition strategy or archiving old data';
    END
    ELSE
    BEGIN
        PRINT 'All partitions are within normal volume ranges';
    END
END;
GO

PRINT 'Partition strategy created successfully';
PRINT 'Run sp_MaintainMonthlyPartitions monthly to maintain partitions';
PRINT 'Run sp_CheckPartitionHealth to monitor partition health';

-- Example usage:
-- EXEC sp_MaintainMonthlyPartitions;
-- EXEC sp_CheckPartitionHealth;
-- EXEC sp_UpdatePartitionedTableStats;
    
    PRINT 'Added partition for: ' + CONVERT(VARCHAR(10), @NextBoundary, 120);
END;
GO

PRINT 'Partition strategy created successfully!';
