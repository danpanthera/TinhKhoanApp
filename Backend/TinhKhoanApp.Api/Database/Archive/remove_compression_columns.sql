-- Script to manually remove compression columns from ImportedDataRecords table
-- This should be run manually against the SQL Server database

USE TinhKhoanDB;
GO

-- Check if we need to disable temporal table first
IF OBJECTPROPERTY(OBJECT_ID('ImportedDataRecords'), 'TableTemporalType') = 2
BEGIN
    PRINT 'Disabling temporal table versioning...';
    ALTER TABLE [ImportedDataRecords] SET (SYSTEM_VERSIONING = OFF);
END

-- Drop CompressionRatio column if it exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ImportedDataRecords') AND name = 'CompressionRatio')
BEGIN
    PRINT 'Dropping CompressionRatio column...';
    
    -- First drop any default constraint
    DECLARE @sql NVARCHAR(MAX);
    SELECT @sql = 'ALTER TABLE [ImportedDataRecords] DROP CONSTRAINT [' + d.name + '];'
    FROM sys.default_constraints d
    INNER JOIN sys.columns c ON d.parent_column_id = c.column_id AND d.parent_object_id = c.object_id
    WHERE d.parent_object_id = OBJECT_ID('ImportedDataRecords') AND c.name = 'CompressionRatio';
    
    IF @sql IS NOT NULL 
    BEGIN
        PRINT 'Dropping default constraint: ' + @sql;
        EXEC sp_executesql @sql;
    END
    
    -- Now drop the column
    ALTER TABLE [ImportedDataRecords] DROP COLUMN [CompressionRatio];
    PRINT 'CompressionRatio column dropped successfully.';
END
ELSE
BEGIN
    PRINT 'CompressionRatio column does not exist.';
END

-- Drop CompressedData column if it exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ImportedDataRecords') AND name = 'CompressedData')
BEGIN
    PRINT 'Dropping CompressedData column...';
    
    -- First drop any default constraint
    DECLARE @sql2 NVARCHAR(MAX);
    SELECT @sql2 = 'ALTER TABLE [ImportedDataRecords] DROP CONSTRAINT [' + d.name + '];'
    FROM sys.default_constraints d
    INNER JOIN sys.columns c ON d.parent_column_id = c.column_id AND d.parent_object_id = c.object_id
    WHERE d.parent_object_id = OBJECT_ID('ImportedDataRecords') AND c.name = 'CompressedData';
    
    IF @sql2 IS NOT NULL 
    BEGIN
        PRINT 'Dropping default constraint: ' + @sql2;
        EXEC sp_executesql @sql2;
    END
    
    -- Now drop the column
    ALTER TABLE [ImportedDataRecords] DROP COLUMN [CompressedData];
    PRINT 'CompressedData column dropped successfully.';
END
ELSE
BEGIN
    PRINT 'CompressedData column does not exist.';
END

-- Display remaining columns to verify
PRINT 'Remaining columns in ImportedDataRecords:';
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'ImportedDataRecords'
ORDER BY ORDINAL_POSITION;

PRINT 'Script completed.';
