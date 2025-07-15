#!/bin/bash

# 🔍 COMPREHENSIVE TABLE STRUCTURE VERIFICATION
# Kiểm tra chi tiết cấu trúc 8 bảng: columns, temporal, columnstore

echo "🔍 COMPREHENSIVE TABLE STRUCTURE VERIFICATION"
echo "📊 Checking: DP01, DPDA, EI01, GL01, GL41, LN01, LN03, RR01"
echo ""

echo "==============================================================================="
echo "📋 PART 1: COLUMN COUNT & STRUCTURE VERIFICATION"
echo "==============================================================================="

for table in DP01 DPDA EI01 GL01 GL41 LN01 LN03 RR01; do
    echo ""
    echo "📊 TABLE: $table"
    echo "----------------------------------------"

    # Đếm tổng số columns
    total_cols=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -h -1 -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$table'" | grep -v '^$' | tail -1 | tr -d ' ')
    echo "🔢 Total Columns: $total_cols"

    # Kiểm tra NGAY_DL column
    ngay_dl_exists=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -h -1 -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$table' AND COLUMN_NAME = 'NGAY_DL'" | grep -v '^$' | tail -1 | tr -d ' ')
    if [ "$ngay_dl_exists" = "1" ]; then
        echo "✅ NGAY_DL: YES"
    else
        echo "❌ NGAY_DL: NO"
    fi

    # Kiểm tra temporal columns
    temporal_cols=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -h -1 -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$table' AND COLUMN_NAME IN ('ValidFrom', 'ValidTo')" | grep -v '^$' | tail -1 | tr -d ' ')
    echo "🕐 Temporal Columns (ValidFrom/ValidTo): $temporal_cols/2"

    # Kiểm tra system columns
    system_cols=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -h -1 -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$table' AND COLUMN_NAME IN ('Id', 'CREATED_DATE', 'UPDATED_DATE', 'FILE_NAME')" | grep -v '^$' | tail -1 | tr -d ' ')
    echo "⚙️  System Columns (Id, CREATED_DATE, UPDATED_DATE, FILE_NAME): $system_cols/4"

    # Business columns = Total - System - Temporal
    business_cols=$((total_cols - system_cols - temporal_cols - ngay_dl_exists))
    echo "💼 Business Columns: $business_cols"
done

echo ""
echo "==============================================================================="
echo "📋 PART 2: TEMPORAL TABLES STATUS"
echo "==============================================================================="

sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -Q "
SELECT
    t.name AS TableName,
    t.temporal_type_desc AS TemporalType,
    ht.name AS HistoryTable,
    CASE WHEN t.temporal_type_desc = 'SYSTEM_VERSIONED_TEMPORAL_TABLE' THEN '✅ YES' ELSE '❌ NO' END AS Status
FROM sys.tables t
LEFT JOIN sys.tables ht ON t.history_table_id = ht.object_id
WHERE t.name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
ORDER BY t.name;
"

echo ""
echo "==============================================================================="
echo "📋 PART 3: COLUMNSTORE INDEXES STATUS"
echo "==============================================================================="

sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -Q "
SELECT
    t.name AS TableName,
    COUNT(CASE WHEN i.type_desc = 'NONCLUSTERED COLUMNSTORE' THEN 1 END) AS ColumnstoreCount,
    CASE WHEN COUNT(CASE WHEN i.type_desc = 'NONCLUSTERED COLUMNSTORE' THEN 1 END) > 0 THEN '✅ YES' ELSE '❌ NO' END AS HasColumnstore
FROM sys.tables t
LEFT JOIN sys.indexes i ON t.object_id = i.object_id
WHERE t.name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
GROUP BY t.name
ORDER BY t.name;
"

echo ""
echo "==============================================================================="
echo "📋 PART 4: DETAILED COLUMN ORDER VERIFICATION"
echo "==============================================================================="

for table in DP01 DPDA EI01 GL01 GL41 LN01 LN03 RR01; do
    echo ""
    echo "📊 COLUMN ORDER FOR TABLE: $table"
    echo "----------------------------------------"
    sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -Q "
    SELECT
        ORDINAL_POSITION,
        COLUMN_NAME,
        DATA_TYPE,
        CASE
            WHEN COLUMN_NAME IN ('Id', 'CREATED_DATE', 'UPDATED_DATE', 'FILE_NAME') THEN 'SYSTEM'
            WHEN COLUMN_NAME IN ('ValidFrom', 'ValidTo') THEN 'TEMPORAL'
            WHEN COLUMN_NAME = 'NGAY_DL' THEN 'BUSINESS_DATE'
            ELSE 'BUSINESS'
        END AS ColumnType
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = '$table'
    ORDER BY ORDINAL_POSITION;
    "
done

echo ""
echo "==============================================================================="
echo "🎯 FINAL SUMMARY"
echo "==============================================================================="

echo "✅ Verification completed for all 8 tables!"
echo "📊 Check results above for:"
echo "   - Column counts and NGAY_DL presence"
echo "   - Temporal tables status"
echo "   - Columnstore indexes status"
echo "   - Detailed column order"
