#!/bin/bash

echo "🔍 KIỂM TRA CẤU TRÚC CỘT DATABASE VS CSV DOCUMENTED"
echo "=================================================="
echo "$(date '+%Y-%m-%d %H:%M:%S') - Bắt đầu kiểm tra chi tiết"
echo ""

# Function to check table structure
check_table_structure() {
    local table_name=$1
    echo "📊 Bảng: $table_name"
    echo "-------------------"

    # Get total columns
    total_cols=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -h -1 -Q "
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$table_name'
    " 2>/dev/null | grep -E '^[0-9]+$' | head -1)

    # Get business columns (exclude system columns)
    business_cols=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -h -1 -Q "
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = '$table_name'
    AND COLUMN_NAME NOT IN ('Id', 'NGAY_DL', 'CREATED_DATE', 'UPDATED_DATE', 'FILE_NAME', 'ValidFrom', 'ValidTo')
    " 2>/dev/null | grep -E '^[0-9]+$' | head -1)

    # Get first 5 business column names to check naming pattern
    echo "🔤 First 5 business columns:"
    sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -Q "
    SELECT TOP 5 COLUMN_NAME, ORDINAL_POSITION
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = '$table_name'
    AND COLUMN_NAME NOT IN ('Id', 'NGAY_DL', 'CREATED_DATE', 'UPDATED_DATE', 'FILE_NAME', 'ValidFrom', 'ValidTo')
    ORDER BY ORDINAL_POSITION
    " 2>/dev/null | grep -E '^Col[0-9]+|^[A-Z]'

    echo "📈 Total columns: $total_cols"
    echo "💼 Business columns: $business_cols"
    echo "🗃️ System columns: $((total_cols - business_cols))"
    echo ""
}

# CSV Expected columns (from README documentation)
declare -A csv_expected
csv_expected["DP01"]="63"
csv_expected["DPDA"]="13"
csv_expected["EI01"]="24"
csv_expected["GL01"]="27"
csv_expected["GL41"]="13"
csv_expected["LN01"]="79"
csv_expected["LN03"]="17"
csv_expected["RR01"]="25"

echo "📋 SUMMARY COMPARISON TABLE:"
echo "============================"
printf "%-6s | %-12s | %-12s | %-10s | %-8s | %s\n" "Table" "CSV Expected" "DB Business" "DB Total" "Match" "Status"
echo "-------|--------------|--------------|------------|----------|--------"

# Check all tables
for table in DP01 DPDA EI01 GL01 GL41 LN01 LN03 RR01; do
    check_table_structure $table

    # Get actual business columns count
    actual_business=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -h -1 -Q "
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = '$table'
    AND COLUMN_NAME NOT IN ('Id', 'NGAY_DL', 'CREATED_DATE', 'UPDATED_DATE', 'FILE_NAME', 'ValidFrom', 'ValidTo')
    " 2>/dev/null | grep -E '^[0-9]+$' | head -1)

    actual_total=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -h -1 -Q "
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$table'
    " 2>/dev/null | grep -E '^[0-9]+$' | head -1)

    expected=${csv_expected[$table]}

    if [ "$actual_business" = "$expected" ]; then
        match="✅ YES"
        status="GOOD"
    else
        match="❌ NO"
        status="PROBLEM"
    fi

    printf "%-6s | %-12s | %-12s | %-10s | %-8s | %s\n" "$table" "$expected" "$actual_business" "$actual_total" "$match" "$status"
done

echo ""
echo "🚨 ISSUES DETECTED:"
echo "==================="

# Check for generic column naming (Col1, Col2, etc.)
echo "🔍 Checking for generic column naming (Col1, Col2, etc.)..."
for table in DP01 DPDA EI01 GL01 GL41 LN01 LN03 RR01; do
    generic_count=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -h -1 -Q "
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = '$table' AND COLUMN_NAME LIKE 'Col%'
    " 2>/dev/null | grep -E '^[0-9]+$' | head -1)

    if [ "$generic_count" -gt 0 ]; then
        echo "⚠️  Table $table has $generic_count generic columns (Col1, Col2, etc.)"
    fi
done

echo ""
echo "📝 RECOMMENDATIONS:"
echo "==================="
echo "1. Các bảng đang sử dụng generic column names (Col1, Col2, etc.)"
echo "2. Cần import lại từ CSV với đúng column headers"
echo "3. Verify CSV files có đúng column names không"
echo "4. Update table schema để match với CSV structure"
echo ""
echo "✅ Kiểm tra hoàn thành $(date '+%H:%M:%S')"
