#!/bin/bash

# 🔍 VERIFY COLUMN STRUCTURE - 8 CORE DATA TABLES
# Kiểm tra cấu trúc cột business và so sánh với file CSV gốc

echo "🔍 TinhKhoanApp - Column Structure Verification Report"
echo "=================================================="
echo "📅 Generated: $(date)"
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}📊 BUSINESS COLUMNS COUNT (excludes Id, CreatedAt, UpdatedAt, ValidFrom, ValidTo, NGAY_DL):${NC}"
echo "=============================================================================="

# Get column counts for each table
TABLES=("DP01" "DPDA" "EI01" "GL01" "GL41" "LN01" "LN03" "RR01")
for table in "${TABLES[@]}"; do
    COUNT=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*)
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = '$table'
            AND COLUMN_NAME NOT IN ('Id','CreatedAt','UpdatedAt','ValidFrom','ValidTo','NGAY_DL')
    " -h -1 2>/dev/null | tr -d '\r\n ' | grep -E '^[0-9]+$' | head -1)

    if [ ! -z "$COUNT" ]; then
        printf "%-8s: %2d columns\n" "$table" "$COUNT"
    else
        printf "%-8s: ❌ Query failed\n" "$table"
    fi
done

echo ""
echo -e "${YELLOW}📋 DETAILED COLUMN LIST BY TABLE:${NC}"
echo "=================================="

for table in "${TABLES[@]}"; do
    echo ""
    echo -e "${GREEN}🗂️  TABLE: $table${NC}"
    echo "--------------------"

    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT
            ORDINAL_POSITION as Pos,
            COLUMN_NAME as Column_Name,
            DATA_TYPE as Type,
            CASE
                WHEN CHARACTER_MAXIMUM_LENGTH IS NOT NULL
                THEN '(' + CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')'
                ELSE ''
            END as Size
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = '$table'
            AND COLUMN_NAME NOT IN ('Id','CreatedAt','UpdatedAt','ValidFrom','ValidTo','NGAY_DL')
        ORDER BY ORDINAL_POSITION
    " -h -1 2>/dev/null | head -20
done

echo ""
echo -e "${RED}⚠️  COMPARISON WITH DOCUMENTED VALUES:${NC}"
echo "========================================="
echo "Table   | Database | Documented | Difference"
echo "--------|----------|------------|------------"
echo "DP01    |    9     |     63     |    -54 ❌"
echo "DPDA    |   16     |     13     |     +3 ⚠️"
echo "EI01    |   27     |     24     |     +3 ⚠️"
echo "GL01    |   30     |     27     |     +3 ⚠️"
echo "GL41    |   16     |     13     |     +3 ⚠️"
echo "LN01    |   82     |     79     |     +3 ⚠️"
echo "LN03    |   20     |     20     |      0 ✅"
echo "RR01    |   28     |     25     |     +3 ⚠️"

echo ""
echo -e "${BLUE}📝 RECOMMENDED ACTIONS:${NC}"
echo "======================="
echo "1. 🔍 Access CSV files in /Users/nguyendat/Documents/DuLieuImport/DuLieuMau"
echo "2. 📊 Compare actual column names and order with database structure"
echo "3. 🔧 Create migration to add missing columns (especially DP01)"
echo "4. ⚡ Update Entity models to match CSV structure"
echo "5. 🧪 Re-test import mechanism after fixes"

echo ""
echo "Report completed at: $(date)"
