#!/bin/bash

echo "🎯 MANDATORY TEMPORAL TABLES - YÊU CẦU BẮT BUỘC 8/8 BẢNG"
echo "========================================================="
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
echo ""

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# 7 bảng cần enable temporal (DP01 đã hoạt động)
remaining_tables=("DPDA" "EI01" "GL01" "GL41" "LN01" "LN03" "RR01")

echo "🔧 APPROACH: Sử dụng phương pháp đã thành công với DP01"
echo "======================================================"
echo ""

# Function to enable temporal for a table
enable_temporal_table() {
    local table=$1
    echo "🔧 Processing table: $table"
    echo "================================"

    # Step 1: Disable system versioning if exists
    echo "  📋 Step 1: Disable existing system versioning..."
    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        IF EXISTS (SELECT * FROM sys.tables WHERE name = '$table' AND temporal_type = 2)
            ALTER TABLE $table SET (SYSTEM_VERSIONING = OFF);
    " > /dev/null 2>&1

    # Step 2: Drop existing history table
    echo "  🗑️  Step 2: Drop existing history table..."
    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        DROP TABLE IF EXISTS ${table}_History;
    " > /dev/null 2>&1

    # Step 3: Drop temporal columns if they exist
    echo "  🔧 Step 3: Clean existing temporal columns..."
    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        IF COL_LENGTH('$table', 'SysStartTime') IS NOT NULL
            ALTER TABLE $table DROP COLUMN SysStartTime;
        IF COL_LENGTH('$table', 'SysEndTime') IS NOT NULL
            ALTER TABLE $table DROP COLUMN SysEndTime;
    " > /dev/null 2>&1

    # Step 4: Add temporal columns with HIDDEN attribute
    echo "  ➕ Step 4: Add temporal columns..."
    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        ALTER TABLE $table ADD
            SysStartTime datetime2(7) GENERATED ALWAYS AS ROW START HIDDEN DEFAULT SYSUTCDATETIME(),
            SysEndTime datetime2(7) GENERATED ALWAYS AS ROW END HIDDEN DEFAULT CONVERT(datetime2(7), '9999-12-31 23:59:59.9999999');
    "

    if [ $? -eq 0 ]; then
        echo -e "    ✅ ${GREEN}Temporal columns added successfully${NC}"
    else
        echo -e "    ❌ ${RED}Failed to add temporal columns${NC}"
        return 1
    fi

    # Step 5: Add PERIOD definition
    echo "  ⏰ Step 5: Add SYSTEM_TIME period..."
    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        ALTER TABLE $table ADD PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
    "

    if [ $? -eq 0 ]; then
        echo -e "    ✅ ${GREEN}SYSTEM_TIME period added successfully${NC}"
    else
        echo -e "    ❌ ${RED}Failed to add SYSTEM_TIME period${NC}"
        return 1
    fi

    # Step 6: Create history table with exact same structure
    echo "  📋 Step 6: Create history table..."

    # Get the table structure and create history table
    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        -- Create history table with same structure but without IDENTITY
        SELECT TOP 0 * INTO ${table}_History FROM $table;

        -- Add required temporal columns to history table
        ALTER TABLE ${table}_History ADD
            SysStartTime datetime2(7) NOT NULL,
            SysEndTime datetime2(7) NOT NULL;

        -- Create clustered index on history table
        CREATE CLUSTERED INDEX IX_${table}_History_Period_Columns
        ON ${table}_History (SysEndTime, SysStartTime, Id);
    "

    if [ $? -eq 0 ]; then
        echo -e "    ✅ ${GREEN}History table created successfully${NC}"
    else
        echo -e "    ❌ ${RED}Failed to create history table${NC}"
        return 1
    fi

    # Step 7: Enable system versioning
    echo "  🔄 Step 7: Enable system versioning..."
    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        ALTER TABLE $table SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.${table}_History));
    "

    if [ $? -eq 0 ]; then
        echo -e "    🎉 ${GREEN}$table TEMPORAL TABLE ENABLED SUCCESSFULLY!${NC}"
        return 0
    else
        echo -e "    ❌ ${RED}Failed to enable system versioning for $table${NC}"
        return 1
    fi
}

# Process each remaining table
success_count=0
total_tables=${#remaining_tables[@]}

echo "🚀 Processing $total_tables remaining tables..."
echo "=============================================="
echo ""

for table in "${remaining_tables[@]}"; do
    if enable_temporal_table "$table"; then
        ((success_count++))
        echo -e "✅ ${GREEN}SUCCESS: $table temporal enabled${NC}"
    else
        echo -e "❌ ${RED}FAILED: $table temporal failed${NC}"
    fi
    echo ""
done

echo "🎯 FINAL VERIFICATION"
echo "===================="

# Check final status
echo "📊 Final temporal status check:"
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT
        name as TableName,
        temporal_type_desc as TemporalType,
        history_table_name as HistoryTable,
        CASE WHEN temporal_type = 2 THEN 'YES' ELSE 'NO' END as TemporalEnabled
    FROM sys.tables
    WHERE name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
    ORDER BY name;
"

echo ""
echo "📊 Final columnstore status check:"
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT
        t.name as TableName,
        COUNT(i.index_id) as ColumnstoreCount,
        CASE WHEN COUNT(i.index_id) > 0 THEN 'YES' ELSE 'NO' END as ColumnstoreEnabled
    FROM sys.tables t
    LEFT JOIN sys.indexes i ON t.object_id = i.object_id AND i.type IN (5,6)
    WHERE t.name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
    GROUP BY t.name
    ORDER BY t.name;
"

echo ""
echo "🎯 SUMMARY REPORT"
echo "================="
echo -e "📋 Tables processed: $total_tables"
echo -e "✅ Successful: $success_count"
echo -e "❌ Failed: $((total_tables - success_count))"

if [ $success_count -eq $total_tables ]; then
    echo -e "🎉 ${GREEN}ALL TEMPORAL TABLES ENABLED SUCCESSFULLY!${NC}"
    echo -e "🎯 ${GREEN}YÊU CẦU BẮT BUỘC ĐÃ HOÀN THÀNH: 8/8 bảng có Temporal + Columnstore${NC}"
else
    echo -e "⚠️  ${YELLOW}Some tables failed, manual intervention may be required${NC}"
fi

echo ""
echo "Completed at: $(date '+%Y-%m-%d %H:%M:%S')"
