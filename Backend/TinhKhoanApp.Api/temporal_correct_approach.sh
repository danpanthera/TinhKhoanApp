#!/bin/bash

echo "🔧 TEMPORAL TABLES - CORRECT APPROACH (SysStartTime/SysEndTime)"
echo "==============================================================="
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# Function to setup temporal table correctly
setup_temporal_correct() {
    local table_name="$1"
    echo "🔧 Setting up temporal for $table_name (correct approach)..."

    # Step 1: Add temporal columns if not exist
    echo "  📋 Adding SysStartTime and SysEndTime columns..."

    # Check if columns exist
    start_exists=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = '$table_name' AND COLUMN_NAME = 'SysStartTime'
    " -h -1 2>/dev/null | sed 's/[^0-9]*//g')

    end_exists=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = '$table_name' AND COLUMN_NAME = 'SysEndTime'
    " -h -1 2>/dev/null | sed 's/[^0-9]*//g')

    if [ "$start_exists" = "0" ] || [ "$end_exists" = "0" ]; then
        sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
            -- Add temporal columns
            ALTER TABLE $table_name ADD
                SysStartTime datetime2(7) GENERATED ALWAYS AS ROW START HIDDEN
                    CONSTRAINT DF_${table_name}_SysStartTime DEFAULT SYSUTCDATETIME(),
                SysEndTime datetime2(7) GENERATED ALWAYS AS ROW END HIDDEN
                    CONSTRAINT DF_${table_name}_SysEndTime DEFAULT CONVERT(datetime2, '9999-12-31 23:59:59.9999999')
        " > /dev/null 2>&1

        if [ $? -eq 0 ]; then
            echo -e "    ✅ ${GREEN}Temporal columns added${NC}"
        else
            echo -e "    ❌ ${RED}Failed to add temporal columns${NC}"
            return 1
        fi
    else
        echo -e "    ℹ️  ${YELLOW}Temporal columns already exist${NC}"
    fi

    # Step 2: Add PERIOD FOR SYSTEM_TIME
    echo "  📋 Adding SYSTEM_TIME period..."

    period_exists=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*) FROM sys.periods p
        JOIN sys.tables t ON p.object_id = t.object_id
        WHERE t.name = '$table_name' AND p.name = 'SYSTEM_TIME'
    " -h -1 2>/dev/null | sed 's/[^0-9]*//g')

    if [ "$period_exists" = "0" ]; then
        sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
            ALTER TABLE $table_name ADD PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime)
        " > /dev/null 2>&1

        if [ $? -eq 0 ]; then
            echo -e "    ✅ ${GREEN}SYSTEM_TIME period added${NC}"
        else
            echo -e "    ❌ ${RED}Failed to add SYSTEM_TIME period${NC}"
            return 1
        fi
    else
        echo -e "    ℹ️  ${YELLOW}SYSTEM_TIME period already exists${NC}"
    fi

    # Step 3: Create history table using same approach as ImportedDataRecords
    echo "  📋 Creating history table..."

    history_exists=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '${table_name}_History'
    " -h -1 2>/dev/null | sed 's/[^0-9]*//g')

    if [ "$history_exists" = "0" ]; then
        # Use the working approach: create with same structure but no constraints
        sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
            SELECT * INTO ${table_name}_History FROM $table_name WHERE 1=0;

            -- Remove primary key constraint from history table if exists
            DECLARE @constraintName NVARCHAR(200)
            SELECT @constraintName = name FROM sys.key_constraints
            WHERE parent_object_id = OBJECT_ID('${table_name}_History') AND type = 'PK'

            IF @constraintName IS NOT NULL
            BEGIN
                DECLARE @sql NVARCHAR(500)
                SET @sql = 'ALTER TABLE ${table_name}_History DROP CONSTRAINT ' + @constraintName
                EXEC sp_executesql @sql
            END
        " > /dev/null 2>&1

        if [ $? -eq 0 ]; then
            echo -e "    ✅ ${GREEN}History table created${NC}"
        else
            echo -e "    ❌ ${RED}Failed to create history table${NC}"
            return 1
        fi
    else
        echo -e "    ℹ️  ${YELLOW}History table already exists${NC}"
    fi

    # Step 4: Enable system versioning
    echo "  📋 Enabling system versioning..."

    result=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        ALTER TABLE $table_name SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.${table_name}_History))
    " 2>&1)

    if [[ $result == *"Msg"* ]]; then
        echo -e "    ❌ ${RED}Error: $(echo $result | grep -o 'Msg.*' | head -1)${NC}"
        return 1
    else
        echo -e "    ✅ ${GREEN}System versioning enabled${NC}"
    fi

    # Step 5: Verify
    temporal_type=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT temporal_type_desc FROM sys.tables WHERE name = '$table_name'
    " -h -1 2>/dev/null | tr -d ' \n\r' | grep -o 'SYSTEM_VERSIONED_TEMPORAL_TABLE\|NON_TEMPORAL_TABLE')

    if [[ "$temporal_type" == "SYSTEM_VERSIONED_TEMPORAL_TABLE" ]]; then
        echo -e "    🎉 ${GREEN}SUCCESS: $table_name is now temporal!${NC}"
        return 0
    else
        echo -e "    ❌ ${RED}FAILED: $table_name is still: $temporal_type${NC}"
        return 1
    fi
}

# Test with DP01 first
TABLE="DP01"

echo "🎯 TESTING WITH $TABLE FIRST"
echo "============================="

if setup_temporal_correct "$TABLE"; then
    echo ""
    echo "🎉 SUCCESS! Let's test all tables..."
    echo ""

    # If DP01 worked, try all tables
    tables=("DP01" "DPDA" "EI01" "GL01" "GL41" "LN01" "LN03" "RR01")
    success_count=0

    for table in "${tables[@]}"; do
        echo ""
        if setup_temporal_correct "$table"; then
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
    echo -e "📊 ${BLUE}Final Results: $success_count/8 tables successfully configured as temporal${NC}"

else
    echo ""
    echo -e "❌ ${RED}Test with $TABLE failed. There may be a fundamental issue.${NC}"
fi

echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
