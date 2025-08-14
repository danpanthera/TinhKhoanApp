#!/bin/bash

echo "🔍 COMPREHENSIVE LN01 VERIFICATION ANALYSIS"
echo "=============================================="

echo ""
echo "📊 1. CSV STRUCTURE ANALYSIS (79 business columns expected)"
echo "-----------------------------------------------------------"
CSV_COUNT=$(head -1 /Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7800_ln01_20241231.csv | tr ',' '\n' | wc -l)
echo "CSV Column Count: $CSV_COUNT"
echo "Expected: 79 business columns"
echo ""
echo "First 10 CSV headers:"
head -1 /Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7800_ln01_20241231.csv | tr ',' '\n' | head -10 | nl -v1

echo ""
echo "📋 2. DATABASE SCHEMA ANALYSIS"
echo "-----------------------------------------------------------"
sqlcmd -S "localhost,1433" -U sa -P "Dientoan@303" -d TinhKhoanDB -C -Q "
SELECT
    COUNT(*) AS TotalColumns,
    SUM(CASE WHEN c.COLUMN_NAME NOT IN ('Id', 'NGAY_DL', 'FILE_NAME', 'CREATED_DATE', 'UPDATED_DATE', 'SysStartTime', 'SysEndTime') THEN 1 ELSE 0 END) AS BusinessColumns,
    SUM(CASE WHEN c.COLUMN_NAME IN ('Id', 'NGAY_DL', 'FILE_NAME', 'CREATED_DATE', 'UPDATED_DATE', 'SysStartTime', 'SysEndTime') THEN 1 ELSE 0 END) AS SystemColumns
FROM INFORMATION_SCHEMA.COLUMNS c
WHERE c.TABLE_NAME = 'LN01'"

echo ""
echo "📊 3. COLUMN ORDER ANALYSIS (First 15 columns)"
echo "-----------------------------------------------------------"
sqlcmd -S "localhost,1433" -U sa -P "Dientoan@303" -d TinhKhoanDB -C -Q "
SELECT TOP 15
    c.ORDINAL_POSITION,
    c.COLUMN_NAME,
    CASE
        WHEN c.COLUMN_NAME = 'Id' THEN 'SYSTEM-ID'
        WHEN c.COLUMN_NAME = 'NGAY_DL' THEN 'DATE-KEY'
        WHEN c.COLUMN_NAME IN ('FILE_NAME', 'CREATED_DATE', 'UPDATED_DATE', 'SysStartTime', 'SysEndTime') THEN 'SYSTEM'
        ELSE 'BUSINESS'
    END AS COLUMN_TYPE
FROM INFORMATION_SCHEMA.COLUMNS c
WHERE c.TABLE_NAME = 'LN01'
ORDER BY c.ORDINAL_POSITION"

echo ""
echo "🎯 4. REQUIRED COLUMN ORDER CHECK"
echo "-----------------------------------------------------------"
NGAY_DL_POS=$(sqlcmd -S "localhost,1433" -U sa -P "Dientoan@303" -d TinhKhoanDB -C -Q -h-1 -w999 "SELECT ORDINAL_POSITION FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LN01' AND COLUMN_NAME = 'NGAY_DL'" | tr -d ' ')
ID_POS=$(sqlcmd -S "localhost,1433" -U sa -P "Dientoan@303" -d TinhKhoanDB -C -Q -h-1 -w999 "SELECT ORDINAL_POSITION FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LN01' AND COLUMN_NAME = 'Id'" | tr -d ' ')

echo "NGAY_DL Position: $NGAY_DL_POS (should be 1)"
echo "Id Position: $ID_POS (should be > 80)"

if [ "$NGAY_DL_POS" -eq 1 ]; then
    echo "✅ NGAY_DL is at position 1 - CORRECT"
else
    echo "❌ NGAY_DL is NOT at position 1 - INCORRECT"
fi

if [ "$ID_POS" -gt 80 ]; then
    echo "✅ Id is after business columns - CORRECT"
else
    echo "❌ Id is before business columns - INCORRECT"
fi

echo ""
echo "📚 5. MODEL STRUCTURE ANALYSIS"
echo "-----------------------------------------------------------"
echo "Checking LN01 model file structure..."
if [ -f "Models/DataTables/LN01.cs" ]; then
    NGAY_DL_ORDER=$(grep -n "Column.*NGAY_DL.*Order.*1" Models/DataTables/LN01.cs | wc -l)
    ID_ORDER=$(grep -n "Column.*Id.*Order.*8[1-9]" Models/DataTables/LN01.cs | wc -l)

    echo "Model has NGAY_DL with Order=1: $([ $NGAY_DL_ORDER -gt 0 ] && echo "✅ YES" || echo "❌ NO")"
    echo "Model has Id with Order>80: $([ $ID_ORDER -gt 0 ] && echo "✅ YES" || echo "❌ NO")"
else
    echo "❌ LN01.cs model file not found"
fi

echo ""
echo "🏗️ 6. TEMPORAL TABLE STATUS"
echo "-----------------------------------------------------------"
sqlcmd -S "localhost,1433" -U sa -P "Dientoan@303" -d TinhKhoanDB -C -Q "
SELECT
    t.name AS TableName,
    t.temporal_type_desc AS TemporalType,
    CASE WHEN h.name IS NOT NULL THEN '✅ EXISTS' ELSE '❌ MISSING' END AS HistoryTableStatus
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.name = 'LN01'"

echo ""
echo "📈 7. BUSINESS LOGIC VALIDATION"
echo "-----------------------------------------------------------"
echo "Checking critical business columns mapping..."

# Check first 5 business columns match CSV
FIRST_BUSINESS_COLS=$(sqlcmd -S "localhost,1433" -U sa -P "Dientoan@303" -d TinhKhoanDB -C -Q -h-1 -w999 "
SELECT TOP 5 c.COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS c
WHERE c.TABLE_NAME = 'LN01'
AND c.ORDINAL_POSITION BETWEEN 2 AND 6
ORDER BY c.ORDINAL_POSITION" | tr '\n' ',' | sed 's/,$//')

CSV_FIRST_COLS=$(head -1 /Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7800_ln01_20241231.csv | cut -d',' -f1-5 | tr ',' '\n' | tr '\n' ',' | sed 's/,$//')

echo "Database first 5 business columns: $FIRST_BUSINESS_COLS"
echo "CSV first 5 business columns: $CSV_FIRST_COLS"

if [ "$FIRST_BUSINESS_COLS" = "$CSV_FIRST_COLS" ]; then
    echo "✅ Business columns mapping MATCHES"
else
    echo "❌ Business columns mapping MISMATCH"
fi

echo ""
echo "🎯 SUMMARY ASSESSMENT"
echo "====================="
echo "LN01 Table Status:"
echo "- Total database columns: $(sqlcmd -S "localhost,1433" -U sa -P "Dientoan@303" -d TinhKhoanDB -C -Q -h-1 -w999 "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LN01'")"
echo "- Expected structure: NGAY_DL(1) -> Business(2-80) -> System(81+)"
echo "- Column order issue: $([ "$NGAY_DL_POS" -eq 1 ] && echo "✅ RESOLVED" || echo "❌ NEEDS FIX")"
echo "- CSV compatibility: $([ "$FIRST_BUSINESS_COLS" = "$CSV_FIRST_COLS" ] && echo "✅ COMPATIBLE" || echo "❌ NEEDS VERIFICATION")"
