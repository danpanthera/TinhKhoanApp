#!/bin/bash

# 🔍 ANALYZE CSV STRUCTURE FOR MIGRATION CREATION
# Phân tích cấu trúc file CSV gốc để tạo migration cho 8 core data tables

echo "🔍 TinhKhoanApp - CSV Structure Analysis for Migration"
echo "=================================================="
echo "📅 Generated: $(date)"
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

CSV_PATH="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau"

echo -e "${BLUE}📂 CSV Files Location: ${CSV_PATH}${NC}"
echo ""

# Check if CSV path exists
if [ ! -d "$CSV_PATH" ]; then
    echo -e "${RED}❌ CSV path not found: $CSV_PATH${NC}"
    echo "Please verify the path to CSV sample files."
    exit 1
fi

echo -e "${YELLOW}📋 ANALYZING CSV FILES STRUCTURE:${NC}"
echo "================================="

# Array of file patterns to search for
CSV_FILES=("dp01" "dpda" "ei01" "gl01" "gl41" "ln01" "ln03" "rr01")

for file_type in "${CSV_FILES[@]}"; do
    echo ""
    echo -e "${GREEN}🗂️  ANALYZING: $(echo ${file_type} | tr '[:lower:]' '[:upper:]')${NC}"
    echo "--------------------"

    # Find CSV files matching the pattern
    CSV_FILE=$(find "$CSV_PATH" -iname "*${file_type}*" -name "*.csv" | head -1)

    if [ -f "$CSV_FILE" ]; then
        echo "✅ Found: $(basename "$CSV_FILE")"

        # Get header line (first line) and count columns
        HEADER=$(head -1 "$CSV_FILE")
        COLUMN_COUNT=$(echo "$HEADER" | tr ',' '\n' | wc -l | tr -d ' ')

        echo "📊 Total Columns: $COLUMN_COUNT"
        echo "📝 Column Headers:"

        # Display first 20 columns
        echo "$HEADER" | tr ',' '\n' | head -20 | nl -nln

        if [ "$COLUMN_COUNT" -gt 20 ]; then
            echo "... and $(($COLUMN_COUNT - 20)) more columns"
        fi

        # Save detailed analysis to file
        OUTPUT_FILE="csv_analysis_${file_type}.txt"
        echo "=== CSV ANALYSIS FOR $(echo ${file_type} | tr '[:lower:]' '[:upper:]') ===" > "$OUTPUT_FILE"
        echo "File: $CSV_FILE" >> "$OUTPUT_FILE"
        echo "Total Columns: $COLUMN_COUNT" >> "$OUTPUT_FILE"
        echo "Headers:" >> "$OUTPUT_FILE"
        echo "$HEADER" | tr ',' '\n' | nl -nln >> "$OUTPUT_FILE"

        echo "💾 Detailed analysis saved to: $OUTPUT_FILE"

    else
        echo "❌ CSV file not found for pattern: *${file_type}*"

        # List available files for reference
        echo "Available CSV files:"
        find "$CSV_PATH" -name "*.csv" | head -5
    fi
done

echo ""
echo -e "${BLUE}📊 CURRENT DATABASE STRUCTURE COMPARISON:${NC}"
echo "=========================================="

# Get current database column counts
echo "Table   | Database | Expected | Status"
echo "--------|----------|----------|--------"

TABLES=("DP01" "DPDA" "EI01" "GL01" "GL41" "LN01" "LN03" "RR01")
for table in "${TABLES[@]}"; do
    COUNT=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*)
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = '$table'
            AND COLUMN_NAME NOT IN ('Id','CreatedAt','UpdatedAt','ValidFrom','ValidTo','NGAY_DL')
    " -h -1 2>/dev/null | tr -d '\r\n ' | grep -E '^[0-9]+$' | head -1)

    if [ ! -z "$COUNT" ]; then
        printf "%-8s| %8s |    ???   | VERIFY\n" "$table" "$COUNT"
    else
        printf "%-8s|   ERROR  |    ???   | FAILED\n" "$table"
    fi
done

echo ""
echo -e "${YELLOW}📝 MIGRATION CREATION PLAN:${NC}"
echo "============================"
echo "1. 🔍 Verify CSV column structure from analysis files"
echo "2. 🏗️  Create migration to add missing columns"
echo "3. 🔧 Update Entity models to match CSV structure"
echo "4. 🧪 Test import mechanism with new structure"
echo "5. 📊 Verify data integrity after migration"

echo ""
echo "Analysis completed at: $(date)"
echo "Next step: Review csv_analysis_*.txt files and create migration script"
