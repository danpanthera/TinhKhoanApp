-- ============================================================================
-- IMPORT DATA FOR ALL TABLES
-- Import CSV data into empty tables using BULK INSERT
-- Date: 2025-08-30
-- Purpose: Populate tables with sample data
-- ============================================================================

PRINT 'Starting data import for all tables...'

-- DP01 Import
PRINT 'Importing DP01 data...'
DECLARE @DP01Count int
SELECT @DP01Count = COUNT(*) FROM DP01
IF @DP01Count = 0
BEGIN
    BULK INSERT DP01
    FROM '/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/7800_dp01_20241231.csv'
    WITH (
        FIRSTROW = 2,
        FIELDTERMINATOR = ',',
        ROWTERMINATOR = '\n',
        ERRORFILE = '/tmp/dp01_errors.txt',
        TABLOCK,
        FORMAT = 'CSV',
        FIELDQUOTE = '"'
    );

    -- Update NGAY_DL and system fields
    UPDATE DP01 SET
        NGAY_DL = '2024-12-31',
        ImportDateTime = GETDATE(),
        CreatedAt = GETDATE(),
        UpdatedAt = GETDATE(),
        FILE_NAME = '7800_dp01_20241231.csv',
        DataSource = 'CSV Import'
    WHERE NGAY_DL IS NULL;

    SELECT COUNT(*) as DP01_Records FROM DP01;
    PRINT '✅ DP01 data imported successfully'
END
ELSE
    PRINT '⚠️ DP01 already has data, skipping import'

PRINT '🎉 Data import completed!'
