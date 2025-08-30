-- ============================================================================
-- FIX GL02 COLUMN MAPPING ISSUES
-- Import GL02 data with proper column mapping for CRTDTM
-- Date: 2025-08-30
-- ============================================================================

PRINT '🔧 Fixing GL02 Column Mapping Issues...'

-- Check if GL02 already has data
DECLARE @GL02Count int
SELECT @GL02Count = COUNT(*) FROM GL02
IF @GL02Count = 0
BEGIN
    PRINT 'Importing GL02 data...'

    -- Create temporary table matching CSV structure exactly
    CREATE TABLE #GL02_Temp (
        TRDATE NVARCHAR(20),
        TRBRCD NVARCHAR(200),
        USERID NVARCHAR(200),
        JOURSEQ NVARCHAR(200),
        DYTRSEQ NVARCHAR(200),
        LOCAC NVARCHAR(200),
        CCY NVARCHAR(200),
        BUSCD NVARCHAR(200),
        UNIT NVARCHAR(200),
        TRCD NVARCHAR(200),
        CUSTOMER NVARCHAR(200),
        TRTP NVARCHAR(200),
        REFERENCE NVARCHAR(200),
        REMARK NVARCHAR(1000),
        DRAMOUNT NVARCHAR(50),
        CRAMOUNT NVARCHAR(50),
        CRTDTM NVARCHAR(50)
    );

    PRINT 'Bulk inserting GL02 from CSV...'
    BULK INSERT #GL02_Temp
    FROM '/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/7800_gl02_2024120120241231.csv'
    WITH (
        FIRSTROW = 2,
        FIELDTERMINATOR = ',',
        ROWTERMINATOR = '\n',
        TABLOCK,
        FORMAT = 'CSV',
        FIELDQUOTE = '"'
    );

    -- Insert into GL02 with proper type conversions
    INSERT INTO GL02 (
        NGAY_DL, TRBRCD, USERID, JOURSEQ, DYTRSEQ, LOCAC, CCY, BUSCD, UNIT, TRCD,
        CUSTOMER, TRTP, REFERENCE, REMARK, DRAMOUNT, CRAMOUNT, CRTDTM,
        CREATED_DATE, UPDATED_DATE, FILE_NAME
    )
    SELECT
        -- Convert TRDATE to NGAY_DL
        CASE
            WHEN TRDATE IS NOT NULL AND TRDATE != '' AND LEN(LTRIM(RTRIM(TRDATE))) = 8
            THEN TRY_CONVERT(datetime2, TRDATE, 112)
            ELSE '2024-12-31'
        END as NGAY_DL,
        TRBRCD, USERID, JOURSEQ, DYTRSEQ, LOCAC, CCY, BUSCD, UNIT, TRCD,
        CUSTOMER, TRTP, REFERENCE, REMARK,
        -- Convert amounts
        TRY_CONVERT(decimal(18,2), DRAMOUNT) as DRAMOUNT,
        TRY_CONVERT(decimal(18,2), CRAMOUNT) as CRAMOUNT,
        -- Convert CRTDTM
        CASE
            WHEN CRTDTM IS NOT NULL AND CRTDTM != ''
            THEN TRY_CONVERT(datetime2, CRTDTM)
            ELSE NULL
        END as CRTDTM,
        GETDATE() as CREATED_DATE,
        GETDATE() as UPDATED_DATE,
        '7800_gl02_2024120120241231.csv' as FILE_NAME
    FROM #GL02_Temp;

    DROP TABLE #GL02_Temp;

    SELECT COUNT(*) as GL02_ImportedRecords FROM GL02;
    PRINT '✅ GL02 data imported successfully'
END
ELSE
    PRINT '⚠️ GL02 already has data, skipping import'

PRINT '🎉 GL02 Column Mapping Issues Fixed!'
