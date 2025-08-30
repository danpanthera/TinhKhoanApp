-- ============================================================================
-- CREATE COLUMNSTORE INDEXES FOR DATA TABLES
-- Tạo Nonclustered Columnstore Indexes cho các bảng dữ liệu chính
-- Date: 2025-08-30
-- Purpose: Performance optimization cho analytical queries
-- ============================================================================

-- DP01 - Deposit Data Columnstore Indexes
PRINT 'Creating DP01 Columnstore Indexes...'
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'NCCI_DP01_Analytics')
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_DP01_Analytics ON DP01
    (
        NGAY_DL, MA_CN, CCY, DP_TYPE_CODE, CUST_TYPE,
        MA_PGD, CURRENT_BALANCE, RATE, SPECIAL_RATE
    )
    WITH (MAXDOP = 1, ONLINE = OFF);
    PRINT '✅ DP01 Analytics Columnstore Index created'
END
ELSE
    PRINT '⚠️ DP01 Analytics Columnstore Index already exists'

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'NCCI_DP01_Amounts')
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_DP01_Amounts ON DP01
    (
        MA_CN, CCY, CURRENT_BALANCE, ACRUAL_AMOUNT,
        ACRUAL_AMOUNT_END, DRAMT, CRAMT, RATE, SPECIAL_RATE, TYGIA
    )
    WITH (MAXDOP = 1, ONLINE = OFF);
    PRINT '✅ DP01 Amounts Columnstore Index created'
END
ELSE
    PRINT '⚠️ DP01 Amounts Columnstore Index already exists'

-- EI01 - Electronic Banking Columnstore Indexes
PRINT 'Creating EI01 Columnstore Indexes...'
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'NCCI_EI01_Analytics')
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_EI01_Analytics ON EI01
    (
        NGAY_DL, MA_CN, MA_KH, LOAI_KH,
        TRANG_THAI_EMB, TRANG_THAI_OTT, TRANG_THAI_SMS,
        TRANG_THAI_SAV, TRANG_THAI_LN
    )
    WITH (MAXDOP = 1, ONLINE = OFF);
    PRINT '✅ EI01 Analytics Columnstore Index created'
END
ELSE
    PRINT '⚠️ EI01 Analytics Columnstore Index already exists'

-- LN01 - Loan Data Columnstore Indexes
PRINT 'Creating LN01 Columnstore Indexes...'
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'NCCI_LN01_Analytics')
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_LN01_Analytics ON LN01
    (
        NGAY_DL, BRCD, CCY, DU_NO, DSBSSEQ, LOAN_TYPE,
        CUSTNM, TAI_KHOAN, RATE, MATDATE
    )
    WITH (MAXDOP = 1, ONLINE = OFF);
    PRINT '✅ LN01 Analytics Columnstore Index created'
END
ELSE
    PRINT '⚠️ LN01 Analytics Columnstore Index already exists'

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'NCCI_LN01_Amounts')
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_LN01_Amounts ON LN01
    (
        BRCD, CCY, DU_NO, PRINOTP, INTACCR, RATE,
        PENALTY_BALANCE, INTPRC, PRINPRC, TOTAL_BALANCE
    )
    WITH (MAXDOP = 1, ONLINE = OFF);
    PRINT '✅ LN01 Amounts Columnstore Index created'
END
ELSE
    PRINT '⚠️ LN01 Amounts Columnstore Index already exists'

-- GL01 - General Ledger Columnstore Indexes
PRINT 'Creating GL01 Columnstore Indexes...'
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'NCCI_GL01_Analytics')
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_GL01_Analytics ON GL01
    (
        NGAY_DL, MA_CN, LOAI_TIEN, MA_TK, TEN_TK, LOAI_BT
    )
    WITH (MAXDOP = 1, ONLINE = OFF);
    PRINT '✅ GL01 Analytics Columnstore Index created'
END
ELSE
    PRINT '⚠️ GL01 Analytics Columnstore Index already exists'

-- GL02 - General Ledger Detail Columnstore Indexes
PRINT 'Creating GL02 Columnstore Indexes...'
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'NCCI_GL02_Analytics')
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_GL02_Analytics ON GL02
    (
        NGAY_DL, MA_CN, LOAI_TIEN, MA_TK, TEN_TK, LOAI_BT
    )
    WITH (MAXDOP = 1, ONLINE = OFF);
    PRINT '✅ GL02 Analytics Columnstore Index created'
END
ELSE
    PRINT '⚠️ GL02 Analytics Columnstore Index already exists'

-- LN03 - Loan Detail Columnstore Indexes
PRINT 'Creating LN03 Columnstore Indexes...'
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'NCCI_LN03_Analytics')
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_LN03_Analytics ON LN03
    (
        NGAY_DL, MA_CN, MA_KH, TEN_KH, SO_TAI_KHOAN,
        LOAI_TIEN, DU_NO_GOC, LAI_PHAI_THU
    )
    WITH (MAXDOP = 1, ONLINE = OFF);
    PRINT '✅ LN03 Analytics Columnstore Index created'
END
ELSE
    PRINT '⚠️ LN03 Analytics Columnstore Index already exists'

-- DPDA - Deposit Detail Columnstore Indexes
PRINT 'Creating DPDA Columnstore Indexes...'
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'NCCI_DPDA_Analytics')
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_DPDA_Analytics ON DPDA
    (
        NGAY_DL, MA_CN, MA_KH, TEN_KH,
        SO_TAI_KHOAN, LOAI_TIEN, SO_DU
    )
    WITH (MAXDOP = 1, ONLINE = OFF);
    PRINT '✅ DPDA Analytics Columnstore Index created'
END
ELSE
    PRINT '⚠️ DPDA Analytics Columnstore Index already exists'

PRINT '🎉 All Columnstore Indexes creation completed!'

-- Verify created indexes
PRINT 'Verifying created indexes...'
SELECT
    t.name as TableName,
    i.name as IndexName,
    i.type_desc as IndexType,
    COUNT(ic.column_id) as ColumnCount
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
LEFT JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
WHERE i.type_desc = 'NONCLUSTERED COLUMNSTORE'
    AND t.name IN ('DP01', 'EI01', 'LN01', 'GL01', 'GL02', 'LN03', 'DPDA', 'RR01', 'GL41')
GROUP BY t.name, i.name, i.type_desc
ORDER BY t.name, i.name;
