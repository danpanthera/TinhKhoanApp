-- ============================================================================
-- FIX TEMPORAL TABLES IMPORT ISSUES
-- Tạm thời disable temporal versioning để cho phép import data
-- Date: 2025-08-30
-- ============================================================================

PRINT '🔧 Fixing Temporal Tables Import Issues...'

-- 1. Disable temporal versioning cho DP01
PRINT 'Disabling temporal versioning for DP01...'
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DP01' AND temporal_type = 2)
BEGIN
    ALTER TABLE DP01 SET (SYSTEM_VERSIONING = OFF);
    PRINT '✅ DP01 temporal versioning disabled'
END

-- 2. Disable temporal versioning cho EI01
PRINT 'Disabling temporal versioning for EI01...'
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EI01' AND temporal_type = 2)
BEGIN
    ALTER TABLE EI01 SET (SYSTEM_VERSIONING = OFF);
    PRINT '✅ EI01 temporal versioning disabled'
END

-- 3. Disable temporal versioning cho LN01
PRINT 'Disabling temporal versioning for LN01...'
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN01' AND temporal_type = 2)
BEGIN
    ALTER TABLE LN01 SET (SYSTEM_VERSIONING = OFF);
    PRINT '✅ LN01 temporal versioning disabled'
END

-- 4. Import DP01 data với temporal disabled
PRINT 'Importing DP01 data with temporal disabled...'
DECLARE @DP01Count int
SELECT @DP01Count = COUNT(*) FROM DP01
IF @DP01Count = 0
BEGIN
    -- Create temporary table for import
    CREATE TABLE #DP01_Temp (
        MA_CN NVARCHAR(200),
        TAI_KHOAN_HACH_TOAN NVARCHAR(200),
        MA_KH NVARCHAR(200),
        TEN_KH NVARCHAR(200),
        DP_TYPE_NAME NVARCHAR(200),
        CCY NVARCHAR(200),
        CURRENT_BALANCE DECIMAL(18,2),
        RATE DECIMAL(18,6),
        SO_TAI_KHOAN NVARCHAR(200),
        OPENING_DATE NVARCHAR(20),
        MATURITY_DATE NVARCHAR(20),
        ADDRESS NVARCHAR(1000),
        NOTENO NVARCHAR(200),
        MONTH_TERM NVARCHAR(200),
        TERM_DP_NAME NVARCHAR(200),
        TIME_DP_NAME NVARCHAR(200),
        MA_PGD NVARCHAR(200),
        TEN_PGD NVARCHAR(200),
        DP_TYPE_CODE NVARCHAR(200),
        RENEW_DATE NVARCHAR(20),
        CUST_TYPE NVARCHAR(200),
        CUST_TYPE_NAME NVARCHAR(200),
        CUST_TYPE_DETAIL NVARCHAR(200),
        CUST_DETAIL_NAME NVARCHAR(200),
        PREVIOUS_DP_CAP_DATE NVARCHAR(20),
        NEXT_DP_CAP_DATE NVARCHAR(20),
        ID_NUMBER NVARCHAR(200),
        ISSUED_BY NVARCHAR(200),
        ISSUE_DATE NVARCHAR(20),
        SEX_TYPE NVARCHAR(200),
        BIRTH_DATE NVARCHAR(20),
        TELEPHONE NVARCHAR(200),
        ACRUAL_AMOUNT DECIMAL(18,2),
        ACRUAL_AMOUNT_END DECIMAL(18,2),
        ACCOUNT_STATUS NVARCHAR(200),
        DRAMT DECIMAL(18,2),
        CRAMT DECIMAL(18,2),
        EMPLOYEE_NUMBER NVARCHAR(200),
        EMPLOYEE_NAME NVARCHAR(200),
        SPECIAL_RATE DECIMAL(18,2),
        AUTO_RENEWAL NVARCHAR(200),
        CLOSE_DATE NVARCHAR(20),
        LOCAL_PROVIN_NAME NVARCHAR(200),
        LOCAL_DISTRICT_NAME NVARCHAR(200),
        LOCAL_WARD_NAME NVARCHAR(200),
        TERM_DP_TYPE NVARCHAR(200),
        TIME_DP_TYPE NVARCHAR(200),
        STATES_CODE NVARCHAR(200),
        ZIP_CODE NVARCHAR(200),
        COUNTRY_CODE NVARCHAR(200),
        TAX_CODE_LOCATION NVARCHAR(200),
        MA_CAN_BO_PT NVARCHAR(200),
        TEN_CAN_BO_PT NVARCHAR(200),
        PHONG_CAN_BO_PT NVARCHAR(200),
        NGUOI_NUOC_NGOAI NVARCHAR(200),
        QUOC_TICH NVARCHAR(200),
        MA_CAN_BO_AGRIBANK NVARCHAR(200),
        NGUOI_GIOI_THIEU NVARCHAR(200),
        TEN_NGUOI_GIOI_THIEU NVARCHAR(200),
        CONTRACT_COUTS_DAY NVARCHAR(200),
        SO_KY_AD_LSDB NVARCHAR(200),
        UNTBUSCD NVARCHAR(200),
        TYGIA DECIMAL(18,2)
    );

    PRINT 'Bulk inserting into temporary table...'
    BULK INSERT #DP01_Temp
    FROM '/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/7800_dp01_20241231.csv'
    WITH (
        FIRSTROW = 2,
        FIELDTERMINATOR = ',',
        ROWTERMINATOR = '\n',
        TABLOCK,
        FORMAT = 'CSV',
        FIELDQUOTE = '"'
    );

    -- Insert from temp table with date conversion
    INSERT INTO DP01 (
        NGAY_DL, MA_CN, TAI_KHOAN_HACH_TOAN, MA_KH, TEN_KH, DP_TYPE_NAME, CCY,
        CURRENT_BALANCE, RATE, SO_TAI_KHOAN,
        OPENING_DATE, MATURITY_DATE, ADDRESS, NOTENO, MONTH_TERM, TERM_DP_NAME,
        TIME_DP_NAME, MA_PGD, TEN_PGD, DP_TYPE_CODE, RENEW_DATE, CUST_TYPE,
        CUST_TYPE_NAME, CUST_TYPE_DETAIL, CUST_DETAIL_NAME, PREVIOUS_DP_CAP_DATE,
        NEXT_DP_CAP_DATE, ID_NUMBER, ISSUED_BY, ISSUE_DATE, SEX_TYPE, BIRTH_DATE,
        TELEPHONE, ACRUAL_AMOUNT, ACRUAL_AMOUNT_END, ACCOUNT_STATUS, DRAMT, CRAMT,
        EMPLOYEE_NUMBER, EMPLOYEE_NAME, SPECIAL_RATE, AUTO_RENEWAL, CLOSE_DATE,
        LOCAL_PROVIN_NAME, LOCAL_DISTRICT_NAME, LOCAL_WARD_NAME, TERM_DP_TYPE,
        TIME_DP_TYPE, STATES_CODE, ZIP_CODE, COUNTRY_CODE, TAX_CODE_LOCATION,
        MA_CAN_BO_PT, TEN_CAN_BO_PT, PHONG_CAN_BO_PT, NGUOI_NUOC_NGOAI, QUOC_TICH,
        MA_CAN_BO_AGRIBANK, NGUOI_GIOI_THIEU, TEN_NGUOI_GIOI_THIEU, CONTRACT_COUTS_DAY,
        SO_KY_AD_LSDB, UNTBUSCD, TYGIA,
        ImportDateTime, CreatedAt, UpdatedAt, FILE_NAME, DataSource
    )
    SELECT
        '2024-12-31' as NGAY_DL,
        MA_CN, TAI_KHOAN_HACH_TOAN, MA_KH, TEN_KH, DP_TYPE_NAME, CCY,
        CURRENT_BALANCE, RATE, SO_TAI_KHOAN,
        -- Date conversion từ 'YYYYMMDD format
        CASE
            WHEN OPENING_DATE IS NOT NULL AND OPENING_DATE != '' AND LEN(LTRIM(RTRIM(REPLACE(OPENING_DATE, '''', '')))) = 8
            THEN TRY_CONVERT(datetime2, SUBSTRING(REPLACE(OPENING_DATE, '''', ''), 1, 8), 112)
            ELSE NULL
        END as OPENING_DATE,
        CASE
            WHEN MATURITY_DATE IS NOT NULL AND MATURITY_DATE != '' AND LEN(LTRIM(RTRIM(REPLACE(MATURITY_DATE, '''', '')))) = 8
            THEN TRY_CONVERT(datetime2, SUBSTRING(REPLACE(MATURITY_DATE, '''', ''), 1, 8), 112)
            ELSE NULL
        END as MATURITY_DATE,
        ADDRESS, NOTENO, MONTH_TERM, TERM_DP_NAME, TIME_DP_NAME, MA_PGD, TEN_PGD,
        DP_TYPE_CODE,
        CASE
            WHEN RENEW_DATE IS NOT NULL AND RENEW_DATE != '' AND LEN(LTRIM(RTRIM(REPLACE(RENEW_DATE, '''', '')))) = 8
            THEN TRY_CONVERT(datetime2, SUBSTRING(REPLACE(RENEW_DATE, '''', ''), 1, 8), 112)
            ELSE NULL
        END as RENEW_DATE,
        CUST_TYPE, CUST_TYPE_NAME, CUST_TYPE_DETAIL, CUST_DETAIL_NAME,
        CASE
            WHEN PREVIOUS_DP_CAP_DATE IS NOT NULL AND PREVIOUS_DP_CAP_DATE != '' AND LEN(LTRIM(RTRIM(REPLACE(PREVIOUS_DP_CAP_DATE, '''', '')))) = 8
            THEN TRY_CONVERT(datetime2, SUBSTRING(REPLACE(PREVIOUS_DP_CAP_DATE, '''', ''), 1, 8), 112)
            ELSE NULL
        END as PREVIOUS_DP_CAP_DATE,
        CASE
            WHEN NEXT_DP_CAP_DATE IS NOT NULL AND NEXT_DP_CAP_DATE != '' AND LEN(LTRIM(RTRIM(REPLACE(NEXT_DP_CAP_DATE, '''', '')))) = 8
            THEN TRY_CONVERT(datetime2, SUBSTRING(REPLACE(NEXT_DP_CAP_DATE, '''', ''), 1, 8), 112)
            ELSE NULL
        END as NEXT_DP_CAP_DATE,
        ID_NUMBER, ISSUED_BY,
        CASE
            WHEN ISSUE_DATE IS NOT NULL AND ISSUE_DATE != '' AND LEN(LTRIM(RTRIM(REPLACE(ISSUE_DATE, '''', '')))) = 8
            THEN TRY_CONVERT(datetime2, SUBSTRING(REPLACE(ISSUE_DATE, '''', ''), 1, 8), 112)
            ELSE NULL
        END as ISSUE_DATE,
        SEX_TYPE,
        CASE
            WHEN BIRTH_DATE IS NOT NULL AND BIRTH_DATE != '' AND LEN(LTRIM(RTRIM(REPLACE(BIRTH_DATE, '''', '')))) = 8
            THEN TRY_CONVERT(datetime2, SUBSTRING(REPLACE(BIRTH_DATE, '''', ''), 1, 8), 112)
            ELSE NULL
        END as BIRTH_DATE,
        TELEPHONE, ACRUAL_AMOUNT, ACRUAL_AMOUNT_END, ACCOUNT_STATUS, DRAMT, CRAMT,
        EMPLOYEE_NUMBER, EMPLOYEE_NAME, SPECIAL_RATE, AUTO_RENEWAL,
        CASE
            WHEN CLOSE_DATE IS NOT NULL AND CLOSE_DATE != '' AND LEN(LTRIM(RTRIM(REPLACE(CLOSE_DATE, '''', '')))) = 8
            THEN TRY_CONVERT(datetime2, SUBSTRING(REPLACE(CLOSE_DATE, '''', ''), 1, 8), 112)
            ELSE NULL
        END as CLOSE_DATE,
        LOCAL_PROVIN_NAME, LOCAL_DISTRICT_NAME, LOCAL_WARD_NAME, TERM_DP_TYPE,
        TIME_DP_TYPE, STATES_CODE, ZIP_CODE, COUNTRY_CODE, TAX_CODE_LOCATION,
        MA_CAN_BO_PT, TEN_CAN_BO_PT, PHONG_CAN_BO_PT, NGUOI_NUOC_NGOAI, QUOC_TICH,
        MA_CAN_BO_AGRIBANK, NGUOI_GIOI_THIEU, TEN_NGUOI_GIOI_THIEU, CONTRACT_COUTS_DAY,
        SO_KY_AD_LSDB, UNTBUSCD, TYGIA,
        GETDATE() as ImportDateTime,
        GETDATE() as CreatedAt,
        GETDATE() as UpdatedAt,
        '7800_dp01_20241231.csv' as FILE_NAME,
        'SQL Import Fixed' as DataSource
    FROM #DP01_Temp;

    DROP TABLE #DP01_Temp;

    SELECT COUNT(*) as DP01_ImportedRecords FROM DP01;
    PRINT '✅ DP01 data imported successfully'
END
ELSE
    PRINT '⚠️ DP01 already has data, skipping import'

-- 5. Re-enable temporal versioning cho DP01
PRINT 'Re-enabling temporal versioning for DP01...'
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DP01' AND temporal_type = 0)
BEGIN
    ALTER TABLE DP01 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.DP01_History));
    PRINT '✅ DP01 temporal versioning re-enabled'
END

-- 6. Re-enable temporal versioning cho EI01
PRINT 'Re-enabling temporal versioning for EI01...'
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EI01' AND temporal_type = 0)
BEGIN
    ALTER TABLE EI01 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.EI01_History));
    PRINT '✅ EI01 temporal versioning re-enabled'
END

-- 7. Re-enable temporal versioning cho LN01
PRINT 'Re-enabling temporal versioning for LN01...'
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN01' AND temporal_type = 0)
BEGIN
    ALTER TABLE LN01 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.LN01_History));
    PRINT '✅ LN01 temporal versioning re-enabled'
END

PRINT '🎉 Temporal Tables Import Issues Fixed!'

-- Verify data
SELECT
    'DP01' as TableName, COUNT(*) as RecordCount FROM DP01
UNION ALL
SELECT
    'EI01' as TableName, COUNT(*) as RecordCount FROM EI01
UNION ALL
SELECT
    'LN01' as TableName, COUNT(*) as RecordCount FROM LN01
ORDER BY TableName;
