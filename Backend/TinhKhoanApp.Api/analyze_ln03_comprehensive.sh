#!/bin/bash
# Phân tích toàn diện LN03 - CSV, Model, Database

echo "🔍 COMPREHENSIVE LN03 ANALYSIS REPORT - $(date)"
echo "=================================================================="

# 1. CSV Analysis
echo ""
echo "📊 1. CSV FILE ANALYSIS:"
echo "File: /Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7800_ln03_20241231.csv"

# Kiểm tra file tồn tại
if [ -f "/Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7800_ln03_20241231.csv" ]; then
    echo "✅ CSV file exists"

    # Đếm số cột trong header
    header_cols=$(head -n 1 "/Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7800_ln03_20241231.csv" | tr ',' '\n' | wc -l | xargs)
    echo "📍 Header columns: $header_cols"

    # Đếm số cột trong data row
    data_cols=$(awk -F',' 'NR==2 {print NF}' "/Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7800_ln03_20241231.csv")
    echo "📍 Data row columns: $data_cols"

    echo ""
    echo "🔍 Headers (17 columns):"
    head -n 1 "/Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7800_ln03_20241231.csv" | tr ',' '\n' | nl

    echo ""
    echo "🔍 Sample data row columns (20 total - 17 headers + 3 no-header):"
    awk -F',' 'NR==2 {for(i=1; i<=NF; i++) printf "%2d. %s\n", i, (i<=17 ? "Header_"i : "NoHeader_"(i-17))}' "/Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7800_ln03_20241231.csv"
else
    echo "❌ CSV file not found!"
fi

# 2. Model Analysis
echo ""
echo "📋 2. MODEL ANALYSIS:"
model_file="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/Models/DataTables/LN03.cs"

if [ -f "$model_file" ]; then
    echo "✅ LN03 model exists"

    # Count business columns (Order 2-21)
    business_cols=$(grep -c "Column(Order = [2-9][0-9]*" "$model_file" || echo "0")
    echo "📍 Business columns in model: $business_cols"

    # Count all properties
    total_props=$(grep -c "public.*{.*get.*set.*}" "$model_file")
    echo "📍 Total properties: $total_props"

    # Check for temporal columns
    echo ""
    echo "🔍 Column Order Analysis:"
    grep -n "Column(Order" "$model_file" | head -25

    # Check for temporal functionality
    echo ""
    echo "🔍 Temporal Features:"
    if grep -q "SysStartTime\|SysEndTime" "$model_file"; then
        echo "✅ Has temporal columns"
    else
        echo "❌ Missing temporal columns"
    fi

else
    echo "❌ LN03 model not found!"
fi

# 3. Database Schema Analysis
echo ""
echo "🗄️  3. DATABASE SCHEMA ANALYSIS:"
echo "Checking LN03 table structure in TinhKhoanDB..."

cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api

# Get column info using sqlcmd
if command -v sqlcmd > /dev/null; then
    echo "📍 LN03 table columns in database:"
    sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "
    SELECT
        COLUMN_NAME,
        ORDINAL_POSITION,
        DATA_TYPE,
        IS_NULLABLE,
        CHARACTER_MAXIMUM_LENGTH
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'LN03'
    ORDER BY ORDINAL_POSITION
    " 2>/dev/null || echo "❌ Database connection failed"

    echo ""
    echo "📍 LN03 temporal table status:"
    sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "
    SELECT
        t.name as table_name,
        t.temporal_type_desc,
        CASE WHEN t.temporal_type = 2 THEN 'SYSTEM_VERSIONED' ELSE 'NOT_TEMPORAL' END as status
    FROM sys.tables t
    WHERE t.name = 'LN03'
    " 2>/dev/null || echo "❌ Database temporal check failed"

else
    echo "❌ sqlcmd not available"
fi

# 4. Issues Summary
echo ""
echo "⚠️  4. ISSUES ANALYSIS:"
echo "Expected LN03 structure:"
echo "- NGAY_DL (Order=1) - System field"
echo "- 17 named business columns (Order=2-18)"
echo "- 3 unnamed business columns (Order=19-21)"
echo "- System/temporal columns (Order=22+)"
echo ""

# 5. Recommendations
echo "🎯 5. RECOMMENDATIONS:"
echo "1. Fix model column order: NGAY_DL first, then business columns"
echo "2. Add temporal columns with shadow properties"
echo "3. Verify DirectImport configuration for LN03"
echo "4. Test CSV import with corrected structure"
echo ""
echo "=================================================================="
echo "Analysis completed at $(date)"
