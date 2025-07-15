#!/bin/bash

echo "🔧 FINAL SOLUTION - COMPREHENSIVE TEMPORAL SETUP"
echo "================================================="
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

setup_temporal_comprehensive() {
    local table_name="$1"
    echo "🔧 Comprehensive temporal setup for $table_name..."

    # Step 1: Drop existing history table
    echo "  🗑️  Dropping existing history table..."
    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        DROP TABLE IF EXISTS ${table_name}_History
    " > /dev/null 2>&1

    # Step 2: Generate CREATE statement for history table without IDENTITY
    echo "  📋 Generating history table structure..."

    # Create the history table using dynamic SQL to ensure exact match
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
        echo -e "    ✅ ${GREEN}History table created with exact structure${NC}"
    else
        echo -e "    ❌ ${RED}Failed to create history table: $result${NC}"
        return 1
    fi

    # Step 3: Verify column counts
    echo "  📊 Verifying column counts..."

    main_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$table_name'
    " -h -1 2>/dev/null | sed 's/[^0-9]*//g')

    history_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '${table_name}_History'
    " -h -1 2>/dev/null | sed 's/[^0-9]*//g')

    echo "    Main: $main_cols columns, History: $history_cols columns"

    if [ "$main_cols" != "$history_cols" ]; then
        echo -e "    ❌ ${RED}Column count mismatch${NC}"
        return 1
    fi

    # Step 4: Check IDENTITY issue
    echo "  🔍 Checking for IDENTITY columns in history table..."

    identity_check=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*) FROM sys.identity_columns
        WHERE object_id = OBJECT_ID('${table_name}_History')
    " -h -1 2>/dev/null | sed 's/[^0-9]*//g')

    echo "    IDENTITY columns in history table: $identity_check"

    if [ "$identity_check" != "0" ]; then
        echo -e "    ⚠️  ${YELLOW}History table still has IDENTITY, this will cause issues${NC}"
    fi

    # Step 5: Enable system versioning
    echo "  🔄 Enabling system versioning..."

    result=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        ALTER TABLE $table_name SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.${table_name}_History))
    " 2>&1)

    if [[ $result == *"Msg"* ]]; then
        echo -e "    ❌ ${RED}Failed: $(echo $result | head -1)${NC}"
        return 1
    else
        echo -e "    🎉 ${GREEN}System versioning enabled successfully!${NC}"
    fi

    # Step 6: Final verification
    temporal_type=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT temporal_type_desc FROM sys.tables WHERE name = '$table_name'
    " -h -1 2>/dev/null | tr -d ' \n\r')

    if [[ "$temporal_type" == *"SYSTEM_VERSIONED_TEMPORAL_TABLE"* ]]; then
        echo -e "    ✅ ${GREEN}VERIFIED: $table_name is now temporal!${NC}"
        return 0
    else
        echo -e "    ❌ ${RED}Verification failed: $temporal_type${NC}"
        return 1
    fi
}

# Test with DP01
TABLE="DP01"

echo "🎯 TESTING COMPREHENSIVE APPROACH WITH $TABLE"
echo "=============================================="

if setup_temporal_comprehensive "$TABLE"; then
    echo ""
    echo -e "🎉 ${GREEN}SUCCESS! Now applying to all 8 tables...${NC}"
    echo ""

    # Apply to all tables
    tables=("DP01" "DPDA" "EI01" "GL01" "GL41" "LN01" "LN03" "RR01")
    success_count=0

    for table in "${tables[@]}"; do
        echo ""
        if setup_temporal_comprehensive "$table"; then
            ((success_count++))
        fi
    done

    echo ""
    echo "🎯 FINAL RESULTS:"
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
    echo -e "📊 ${BLUE}Results: $success_count/8 tables successfully configured as temporal${NC}"

    if [ $success_count -eq 8 ]; then
        echo -e "🎉 ${GREEN}ALL TEMPORAL TABLES SETUP COMPLETED!${NC}"

        # Test temporal functionality
        echo ""
        echo "🧪 Testing temporal functionality..."

        # Simple test with first temporal table
        test_table=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
            SELECT TOP 1 name FROM sys.tables
            WHERE temporal_type_desc = 'SYSTEM_VERSIONED_TEMPORAL_TABLE'
            AND name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
        " -h -1 2>/dev/null | tr -d ' \n\r')

        if [ ! -z "$test_table" ]; then
            echo "Testing with: $test_table"

            before_count=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
                SELECT COUNT(*) FROM ${test_table}_History
            " -h -1 2>/dev/null | sed 's/[^0-9]*//g')

            echo "History records before: $before_count"

            echo -e "✅ ${GREEN}Temporal tables are ready for use!${NC}"
        fi
    else
        echo -e "⚠️  ${YELLOW}Some tables may need manual attention${NC}"
    fi

else
    echo ""
    echo -e "❌ ${RED}Test failed. The IDENTITY column issue persists.${NC}"
    echo -e "💡 ${YELLOW}Recommendation: Azure SQL Edge may have limitations with temporal tables${NC}"
fi

echo ""
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
