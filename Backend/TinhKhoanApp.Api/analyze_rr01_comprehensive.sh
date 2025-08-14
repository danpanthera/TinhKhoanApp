#!/bin/bash

echo "🔍 ===== COMPREHENSIVE RR01 ANALYSIS REPORT ====="

echo ""
echo "📋 1. CSV STRUCTURE ANALYSIS"
echo "==============================="

CSV_FILE="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7800_rr01_20241231.csv"

if [ -f "$CSV_FILE" ]; then
    echo "✅ CSV File Found: $CSV_FILE"

    echo ""
    echo "📊 Headers (25 business columns expected):"
    head -1 "$CSV_FILE" | tr ',' '\n' | nl

    echo ""
    echo "📊 Column Count:"
    COLUMN_COUNT=$(head -1 "$CSV_FILE" | tr ',' '\n' | wc -l)
    echo "   Total columns: $COLUMN_COUNT"

    echo ""
    echo "📊 Record Count:"
    RECORD_COUNT=$(wc -l < "$CSV_FILE")
    echo "   Total records: $((RECORD_COUNT - 1)) (excluding header)"

    echo ""
    echo "📊 Sample Data (first 2 records):"
    head -3 "$CSV_FILE" | tail -2
else
    echo "❌ CSV File Not Found: $CSV_FILE"
fi

echo ""
echo "🏗️ 2. MODEL STRUCTURE ANALYSIS"
echo "=============================="

MODEL_FILE="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/Models/DataTables/RR01.cs"

if [ -f "$MODEL_FILE" ]; then
    echo "✅ Model File Found: $MODEL_FILE"

    echo ""
    echo "📊 Business Columns in Model:"
    grep -n "Order.*TypeName" "$MODEL_FILE" | head -25

    echo ""
    echo "📊 System Columns in Model:"
    grep -n "NGAY_DL\|FILE_NAME\|CREATED_DATE\|UPDATED_DATE" "$MODEL_FILE"

    echo ""
    echo "📊 Date Fields (should be datetime2):"
    grep -n "NGAY\|DATE" "$MODEL_FILE" | grep "datetime2"

    echo ""
    echo "📊 Amount Fields (should be decimal):"
    grep -n "DUNO\|THU_\|BDS\|DS\|TSK" "$MODEL_FILE" | grep "decimal"

else
    echo "❌ Model File Not Found: $MODEL_FILE"
fi

echo ""
echo "🗄️ 3. DATABASE STRUCTURE ANALYSIS"
echo "================================="

echo "📊 Table Columns:"
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "
SELECT
    ORDINAL_POSITION,
    COLUMN_NAME,
    DATA_TYPE +
    CASE
        WHEN CHARACTER_MAXIMUM_LENGTH IS NOT NULL THEN '(' + CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')'
        WHEN NUMERIC_PRECISION IS NOT NULL THEN '(' + CAST(NUMERIC_PRECISION AS VARCHAR) + ',' + CAST(NUMERIC_SCALE AS VARCHAR) + ')'
        ELSE ''
    END AS DATA_TYPE_FULL,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'RR01'
ORDER BY ORDINAL_POSITION;
" 2>/dev/null | head -30

echo ""
echo "📊 Temporal Table Status:"
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "
SELECT
    name,
    temporal_type_desc,
    history_table_name = OBJECT_NAME(history_table_id)
FROM sys.tables
WHERE name = 'RR01';
" 2>/dev/null

echo ""
echo "📊 Record Count in Database:"
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "
SELECT COUNT(*) as RecordCount FROM RR01;
" 2>/dev/null

echo ""
echo "🔧 4. SERVICES & API ANALYSIS"
echo "============================="

SERVICE_FILE="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/Services/RR01Service.cs"
CONTROLLER_FILE="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/Controllers/RR01Controller.cs"

if [ -f "$SERVICE_FILE" ]; then
    echo "✅ Service File Found: $SERVICE_FILE"
    echo "📊 Service Methods:"
    grep -n "public.*async Task" "$SERVICE_FILE" | head -10
else
    echo "❌ Service File Not Found: $SERVICE_FILE"
fi

if [ -f "$CONTROLLER_FILE" ]; then
    echo "✅ Controller File Found: $CONTROLLER_FILE"
    echo "📊 API Endpoints:"
    grep -n "\[Http.*\]" "$CONTROLLER_FILE" | head -10
else
    echo "❌ Controller File Not Found: $CONTROLLER_FILE"
fi

echo ""
echo "⚙️ 5. DIRECTIMPORT CONFIGURATION"
echo "==============================="

APPSETTINGS_FILE="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/appsettings.json"

if [ -f "$APPSETTINGS_FILE" ]; then
    echo "📊 DirectImport Settings:"
    if grep -q "RR01" "$APPSETTINGS_FILE"; then
        grep -A 10 "RR01" "$APPSETTINGS_FILE"
    else
        echo "❌ No RR01-specific DirectImport configuration found"
        echo "📊 Available DirectImport configurations:"
        grep -A 5 "DirectImport" "$APPSETTINGS_FILE"
    fi
else
    echo "❌ appsettings.json Not Found: $APPSETTINGS_FILE"
fi

echo ""
echo "🎯 6. COMPLIANCE SUMMARY"
echo "======================"

echo "📋 Requirements Check:"
echo "   ✅ Temporal Table: Required"
echo "   ✅ Columnstore: Required"
echo "   ✅ 25 Business Columns: Required"
echo "   ✅ NGAY_DL from filename: Required"
echo "   ✅ DATE/NGAY → datetime2: Required"
echo "   ✅ AMT/DUNO/THU_*/BDS/DS → decimal: Required"
echo "   ✅ Other columns → nvarchar(200): Required"
echo "   ✅ DirectImport only: Required"
echo "   ✅ CSV column names unchanged: Required"

echo ""
echo "🏁 ANALYSIS COMPLETED"
echo "===================="
