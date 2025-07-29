-- Drop old DP01 table safely to avoid EF migration conflicts
-- This script will be run manually before creating new migration

USE TinhKhoanDB;
GO

-- Check if old DP01 table exists and drop it safely
IF OBJECT_ID(N'dbo.DP01', N'U') IS NOT NULL
BEGIN
    PRINT 'Found existing DP01 table, attempting to drop...'

    -- Disable temporal versioning if it exists
    IF OBJECTPROPERTY(OBJECT_ID(N'dbo.DP01'), 'TableTemporalType') = 2
    BEGIN
        PRINT 'Disabling temporal versioning on DP01...'
        ALTER TABLE [dbo].[DP01] SET (SYSTEM_VERSIONING = OFF)
    END

    -- Drop all indexes first to avoid conflicts
    DECLARE @sql NVARCHAR(MAX) = ''
    SELECT @sql = @sql + 'DROP INDEX [' + i.name + '] ON [dbo].[DP01];' + CHAR(13)
    FROM sys.indexes i
    INNER JOIN sys.tables t ON i.object_id = t.object_id
    WHERE t.name = 'DP01'
      AND i.name IS NOT NULL
      AND i.is_primary_key = 0
      AND i.is_unique_constraint = 0

    IF LEN(@sql) > 0
    BEGIN
        PRINT 'Dropping indexes on DP01 table...'
        EXEC sp_executesql @sql
    END

    -- Drop history tables if they exist
    DROP TABLE IF EXISTS [dbo].[DP01_History]
    DROP TABLE IF EXISTS [dbo].[DP01History]
    PRINT 'Dropped history tables if they existed'

    -- Drop main table
    DROP TABLE [dbo].[DP01]
    PRINT 'Successfully dropped DP01 table'
END
ELSE
BEGIN
    PRINT 'DP01 table does not exist, nothing to drop'
END

PRINT 'DP01 cleanup completed successfully!'
