#!/bin/bash

echo "🔍 KIỂM TRA TEMPORAL TABLES + COLUMNSTORE INDEXES"
echo "================================================="
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# Function to check temporal tables
check_temporal() {
    local table_name="$1"
    echo "🔍 Checking $table_name..."

    # Check if table has temporal features
    temporal_check=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT
            t.name as TableName,
            t.temporal_type_desc,
            h.name as HistoryTable
        FROM sys.tables t
        LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
        WHERE t.name = '$table_name'
    " -h -1 2>/dev/null)

    # Check columnstore indexes
    columnstore_check=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*)
        FROM sys.indexes i
        JOIN sys.tables t ON i.object_id = t.object_id
        WHERE t.name = '$table_name'
        AND i.type_desc LIKE '%COLUMNSTORE%'
    " -h -1 2>/dev/null | sed 's/[^0-9]*//g')

    echo "  📋 Temporal info:"
    echo "$temporal_check" | head -5
    echo "  📊 Columnstore indexes: $columnstore_check"

    if [ "$columnstore_check" -gt 0 ]; then
        echo -e "  ✅ ${GREEN}Columnstore: ENABLED${NC}"
    else
        echo -e "  ❌ ${RED}Columnstore: MISSING${NC}"
    fi
    echo ""
}

echo "📊 CHECKING ALL 8 CORE TABLES:"
echo "==============================="

tables=("DP01" "DPDA" "EI01" "GL01" "GL41" "LN01" "LN03" "RR01")

for table in "${tables[@]}"; do
    check_temporal "$table"
done

echo ""
echo "🎯 SUMMARY CHECK - TEMPORAL TABLES:"
echo "==================================="

sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT
        t.name as TableName,
        t.temporal_type_desc as TemporalType,
        h.name as HistoryTable,
        CASE
            WHEN t.temporal_type_desc = 'SYSTEM_VERSIONED_TEMPORAL_TABLE' THEN 'YES'
            ELSE 'NO'
        END as IsTemporal
    FROM sys.tables t
    LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
    WHERE t.name IN ('DP01','DPDA','EI01','GL01','GL41','LN01','LN03','RR01')
    ORDER BY t.name
" -h -1

echo ""
echo "🎯 SUMMARY CHECK - COLUMNSTORE INDEXES:"
echo "======================================="

sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT
        t.name as TableName,
        COUNT(i.index_id) as ColumnstoreCount,
        CASE
            WHEN COUNT(i.index_id) > 0 THEN 'YES'
            ELSE 'NO'
        END as HasColumnstore
    FROM sys.tables t
    LEFT JOIN sys.indexes i ON t.object_id = i.object_id AND i.type_desc LIKE '%COLUMNSTORE%'
    WHERE t.name IN ('DP01','DPDA','EI01','GL01','GL41','LN01','LN03','RR01')
    GROUP BY t.name
    ORDER BY t.name
" -h -1

echo ""
echo "Completed at: $(date '+%Y-%m-%d %H:%M:%S')"
