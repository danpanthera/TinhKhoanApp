#!/bin/bash

echo "🔧 CREATE COMPLETE TEMPORAL SETUP FOR ALL 8 TABLES"
echo "===================================================="
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# Function to setup temporal for one table
setup_temporal_table() {
    local table_name="$1"
    echo "🔧 Setting up temporal for $table_name..."

    # Step 1: Create History table with same structure but no IDENTITY
    echo "  📋 Creating history table..."
    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        -- Drop existing history table if exists
        DROP TABLE IF EXISTS ${table_name}_History;

        -- Create history table with same structure but without IDENTITY
        SELECT * INTO ${table_name}_History FROM $table_name WHERE 1=0;

        -- Remove IDENTITY property from history table
        ALTER TABLE ${table_name}_History DROP CONSTRAINT PK_${table_name};
        ALTER TABLE ${table_name}_History DROP COLUMN Id;
        ALTER TABLE ${table_name}_History ADD Id int NOT NULL;
    " > /dev/null 2>&1

    if [ $? -eq 0 ]; then
        echo -e "    ✅ ${GREEN}History table created${NC}"
    else
        echo -e "    ❌ ${RED}Failed to create history table${NC}"
        return 1
    fi

    # Step 2: Enable System Versioning
    echo "  🔄 Enabling system versioning..."
    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        ALTER TABLE $table_name SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.${table_name}_History))
    " > /dev/null 2>&1

    if [ $? -eq 0 ]; then
        echo -e "    ✅ ${GREEN}System versioning enabled${NC}"
    else
        echo -e "    ❌ ${RED}Failed to enable system versioning${NC}"
        return 1
    fi

    # Step 3: Verify
    temporal_type=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT temporal_type_desc FROM sys.tables WHERE name = '$table_name'
    " -h -1 2>/dev/null | tr -d ' \n\r')

    if [[ "$temporal_type" == *"SYSTEM_VERSIONED_TEMPORAL_TABLE"* ]]; then
        echo -e "    🎉 ${GREEN}SUCCESS: $table_name is now temporal!${NC}"
        return 0
    else
        echo -e "    ❌ ${RED}FAILED: $table_name is still: $temporal_type${NC}"
        return 1
    fi
}

# Process all 8 tables
tables=("DP01" "DPDA" "EI01" "GL01" "GL41" "LN01" "LN03" "RR01")
success_count=0
total_count=8

echo "🎯 PROCESSING ALL 8 CORE TABLES:"
echo "================================="

for table in "${tables[@]}"; do
    echo ""
    if setup_temporal_table "$table"; then
        ((success_count++))
    fi
done

echo ""
echo "🎯 FINAL SUMMARY:"
echo "=================="

sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT
        t.name as TableName,
        t.temporal_type_desc as TemporalType,
        h.name as HistoryTable,
        CASE WHEN t.temporal_type_desc = 'SYSTEM_VERSIONED_TEMPORAL_TABLE' THEN 'YES' ELSE 'NO' END as IsEnabled
    FROM sys.tables t
    LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
    WHERE t.name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
    ORDER BY t.name
"

echo ""
echo -e "📊 ${BLUE}Results: $success_count/$total_count tables successfully configured${NC}"

if [ $success_count -eq $total_count ]; then
    echo -e "🎉 ${GREEN}ALL TEMPORAL TABLES SETUP COMPLETED SUCCESSFULLY!${NC}"
else
    echo -e "⚠️  ${YELLOW}Some tables may need manual attention${NC}"
fi

echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
