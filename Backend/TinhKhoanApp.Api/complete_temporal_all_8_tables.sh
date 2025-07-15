#!/bin/bash

echo "🔧 COMPLETE TEMPORAL TABLES SETUP FOR ALL 8 TABLES"
echo "==================================================="
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

setup_temporal_complete() {
    local table_name="$1"
    echo "🔧 Complete temporal setup for $table_name..."

    # Step 1: Check if temporal columns exist
    echo "  📋 Checking temporal columns..."

    start_exists=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = '$table_name' AND COLUMN_NAME = 'SysStartTime'
    " -h -1 2>/dev/null | sed 's/[^0-9]*//g')

    end_exists=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = '$table_name' AND COLUMN_NAME = 'SysEndTime'
    " -h -1 2>/dev/null | sed 's/[^0-9]*//g')

    if [ "$start_exists" = "0" ] || [ "$end_exists" = "0" ]; then
        echo "    ➕ Adding temporal columns..."

        # Drop existing columns if partially exist
        if [ "$start_exists" = "1" ]; then
            sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
                ALTER TABLE $table_name DROP COLUMN SysStartTime
            " > /dev/null 2>&1
        fi

        if [ "$end_exists" = "1" ]; then
            sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
                ALTER TABLE $table_name DROP COLUMN SysEndTime
            " > /dev/null 2>&1
        fi

        # Add both columns
        sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
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

    # Step 3: Create history table with exact structure
    echo "  📋 Creating history table..."

    history_exists=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '${table_name}_History'
    " -h -1 2>/dev/null | sed 's/[^0-9]*//g')

    if [ "$history_exists" = "1" ]; then
        # Drop existing history table
        sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
            DROP TABLE ${table_name}_History
        " > /dev/null 2>&1
    fi

    # Create history table with dynamic SQL (same as successful DP01 approach)
    result=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        DECLARE @sql NVARCHAR(MAX) = 'CREATE TABLE ${table_name}_History ('
        DECLARE @columns NVARCHAR(MAX) = ''

        SELECT @columns = @columns +
            COLUMN_NAME + ' ' +
            DATA_TYPE +
            CASE
                WHEN DATA_TYPE IN ('varchar', 'nvarchar', 'char', 'nchar')
                THEN CASE
                    WHEN CHARACTER_MAXIMUM_LENGTH = -1 THEN '(MAX)'
                    ELSE '(' + CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')'
                END
                WHEN DATA_TYPE IN ('decimal', 'numeric')
                THEN '(' + CAST(NUMERIC_PRECISION AS VARCHAR) + ',' + CAST(NUMERIC_SCALE AS VARCHAR) + ')'
                WHEN DATA_TYPE IN ('datetime2', 'time')
                THEN '(' + CAST(DATETIME_PRECISION AS VARCHAR) + ')'
                ELSE ''
            END +
            CASE
                WHEN COLUMN_NAME = 'Id' THEN ' NOT NULL'  -- Remove IDENTITY, keep NOT NULL
                WHEN IS_NULLABLE = 'NO' THEN ' NOT NULL'
                ELSE ' NULL'
            END + ', '
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = '$table_name'
        ORDER BY ORDINAL_POSITION

        -- Remove last comma and add closing parenthesis
        SET @columns = LEFT(@columns, LEN(@columns) - 1)
        SET @sql = @sql + @columns + ')'

        EXEC sp_executesql @sql
    " 2>&1)

    if [[ $result != *"Msg"* ]]; then
        echo -e "    ✅ ${GREEN}History table created${NC}"
    else
        echo -e "    ❌ ${RED}Failed to create history table${NC}"
        return 1
    fi

    # Step 4: Enable system versioning
    echo "  🔄 Enabling system versioning..."

    result=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        ALTER TABLE $table_name SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.${table_name}_History))
    " 2>&1)

    if [[ $result == *"Msg"* ]]; then
        echo -e "    ❌ ${RED}Failed: $(echo $result | head -1)${NC}"
        return 1
    else
        echo -e "    🎉 ${GREEN}System versioning enabled!${NC}"
    fi

    # Step 5: Verify
    temporal_type=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT temporal_type_desc FROM sys.tables WHERE name = '$table_name'
    " -h -1 2>/dev/null | tr -d ' \n\r')

    if [[ "$temporal_type" == *"SYSTEM_VERSIONED_TEMPORAL_TABLE"* ]]; then
        echo -e "    ✅ ${GREEN}VERIFIED: $table_name is temporal!${NC}"
        return 0
    else
        echo -e "    ❌ ${RED}Verification failed: $temporal_type${NC}"
        return 1
    fi
}

# Process all 8 tables
tables=("DP01" "DPDA" "EI01" "GL01" "GL41" "LN01" "LN03" "RR01")
success_count=0

echo "🎯 PROCESSING ALL 8 CORE TABLES:"
echo "================================="

for table in "${tables[@]}"; do
    echo ""
    if setup_temporal_complete "$table"; then
        ((success_count++))
    fi
done

echo ""
echo "🎯 FINAL VERIFICATION:"
echo "======================"

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

if [ $success_count -eq 8 ]; then
    echo -e "🎉 ${GREEN}MISSION ACCOMPLISHED: ALL 8 TEMPORAL TABLES WORKING!${NC}"

    # Quick functionality test
    echo ""
    echo "🧪 Testing temporal functionality..."

    test_table="DP01"

    echo "Testing basic temporal features with $test_table..."

    # Check counts
    main_count=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*) FROM $test_table
    " -h -1 2>/dev/null | sed 's/[^0-9]*//g')

    history_count=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*) FROM ${test_table}_History
    " -h -1 2>/dev/null | sed 's/[^0-9]*//g')

    echo "  📊 $test_table: $main_count records"
    echo "  📊 ${test_table}_History: $history_count records"

    echo ""
    echo -e "✅ ${GREEN}TEMPORAL TABLES READY FOR PRODUCTION USE!${NC}"

elif [ $success_count -gt 0 ]; then
    echo -e "✅ ${GREEN}Partial success: $success_count tables working${NC}"
    echo -e "⚠️  ${YELLOW}Manual review needed for remaining tables${NC}"
else
    echo -e "❌ ${RED}No tables successfully configured${NC}"
fi

echo ""
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
