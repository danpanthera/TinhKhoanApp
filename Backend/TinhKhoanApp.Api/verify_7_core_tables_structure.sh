#!/bin/bash

# 🔍 VERIFY ALL 7 REMAINING CORE TABLES STRUCTURE
# Compare database columns vs CSV files structure
# Date: July 15, 2025

echo "🔍 VERIFY 7 REMAINING CORE TABLES STRUCTURE"
echo "=============================================="
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
echo ""

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

CSV_DIR="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau"

# Function to get CSV column count
get_csv_columns() {
    local csv_file="$1"
    if [ -f "$csv_file" ]; then
        head -1 "$csv_file" | tr ',' '\n' | wc -l | tr -d ' '
    else
        echo "0"
    fi
}

# Function to get database column info
get_db_columns() {
    local table_name="$1"

    echo "🔍 Checking $table_name table structure..."

    # Total columns
    TOTAL_COLS=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$table_name'" -h -1 2>/dev/null | tr -d '\r\n ' | grep -E '^[0-9]+$' | head -1)

    # Business columns (excluding system)
    BUSINESS_COLS=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$table_name' AND COLUMN_NAME NOT IN ('Id','CreatedAt','UpdatedAt','ValidFrom','ValidTo','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 2>/dev/null | tr -d '\r\n ' | grep -E '^[0-9]+$' | head -1)

    # System columns
    SYSTEM_COLS=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$table_name' AND COLUMN_NAME IN ('Id','CreatedAt','UpdatedAt','ValidFrom','ValidTo','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 2>/dev/null | tr -d '\r\n ' | grep -E '^[0-9]+$' | head -1)

    echo "  📊 Total: $TOTAL_COLS, Business: $BUSINESS_COLS, System: $SYSTEM_COLS"
}

# Function to get column order for comparison
get_column_order() {
    local table_name="$1"
    echo "📋 Column order for $table_name:"
    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT ORDINAL_POSITION, COLUMN_NAME, DATA_TYPE
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = '$table_name'
        ORDER BY ORDINAL_POSITION
    " -h -1 2>/dev/null | head -20
    echo ""
}

echo "🗂️  CSV FILES ANALYSIS:"
echo "======================"

# Check CSV files exist and count columns
declare -a tables=("DPDA" "EI01" "GL01" "GL41" "LN01" "LN03" "RR01")
declare -a csv_files=("7808_dpda_20250331.csv" "7808_ei01_20241231.csv" "7808_gl01_2025030120250331.csv" "7808_gl41_20250630.csv" "7808_ln01_20241231.csv" "7808_ln03_20241231.csv" "7800_rr01_20250531.csv")

for i in "${!tables[@]}"; do
    table="${tables[$i]}"
    csv_file="$CSV_DIR/${csv_files[$i]}"
    if [ -f "$csv_file" ]; then
        cols=$(get_csv_columns "$csv_file")
        echo "✅ $table: ${csv_files[$i]} = $cols columns"
        # Store in a simple way
        eval "csv_cols_$table=$cols"
    else
        echo "❌ $table: ${csv_files[$i]} = FILE NOT FOUND"
        eval "csv_cols_$table=0"
    fi
done

echo ""
echo "🗄️  DATABASE STRUCTURE ANALYSIS:"
echo "================================="

# Check each table in database
for i in "${!tables[@]}"; do
    table="${tables[$i]}"
    echo ""
    echo -e "${BLUE}📋 TABLE: $table${NC}"
    echo "----------------------------------------"

    get_db_columns "$table"

    # Get first 10 columns for structure verification
    echo "  📝 First 10 columns:"
    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT TOP 10 COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = '$table'
        ORDER BY ORDINAL_POSITION
    " -h -1 2>/dev/null | head -15
done

echo ""
echo "📊 COMPARISON SUMMARY:"
echo "====================="

for i in "${!tables[@]}"; do
    table="${tables[$i]}"

    # Get stored CSV column count
    eval "csv_cols=\$csv_cols_$table"

    # Get database info again for comparison
    TOTAL_COLS=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$table'" -h -1 2>/dev/null | tr -d '\r\n ' | grep -E '^[0-9]+$' | head -1)

    BUSINESS_COLS=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$table' AND COLUMN_NAME NOT IN ('Id','CreatedAt','UpdatedAt','ValidFrom','ValidTo','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 2>/dev/null | tr -d '\r\n ' | grep -E '^[0-9]+$' | head -1)

    SYSTEM_COLS=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$table' AND COLUMN_NAME IN ('Id','CreatedAt','UpdatedAt','ValidFrom','ValidTo','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 2>/dev/null | tr -d '\r\n ' | grep -E '^[0-9]+$' | head -1)

    echo ""
    echo -e "${BLUE}$table:${NC}"
    echo "  📁 CSV: $csv_cols columns"
    echo "  🗄️  DB Total: $TOTAL_COLS (Business: $BUSINESS_COLS + System: $SYSTEM_COLS)"

    if [ "$csv_cols" -eq "$BUSINESS_COLS" ] 2>/dev/null; then
        echo -e "  ✅ ${GREEN}MATCH: Business columns = CSV columns${NC}"
    elif [ "$csv_cols" -gt 0 ] 2>/dev/null && [ "$BUSINESS_COLS" -gt 0 ] 2>/dev/null; then
        diff=$((BUSINESS_COLS - csv_cols))
        if [ $diff -gt 0 ]; then
            echo -e "  ⚠️  ${YELLOW}EXTRA: +$diff business columns in DB${NC}"
        else
            echo -e "  ❌ ${RED}MISSING: $((csv_cols - BUSINESS_COLS)) columns in DB${NC}"
        fi
    else
        echo -e "  ❌ ${RED}ERROR: Cannot compare (CSV: $csv_cols, DB: $BUSINESS_COLS)${NC}"
    fi
done

echo ""
echo "🎯 NEXT ACTIONS:"
echo "==============="
echo "1. 🔍 Review any mismatches above"
echo "2. 📝 Check column order if needed"
echo "3. 🔧 Create migrations for missing columns"
echo "4. ✅ Update Entity models if needed"
echo "5. 🧪 Test CSV import after fixes"

echo ""
echo "Verification completed at: $(date '+%Y-%m-%d %H:%M:%S')"
