#!/bin/bash

echo "🔧 ULTIMATE TEMPORAL TABLES FIX - MANUAL APPROACH"
echo "=================================================="
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

tables=("DP01" "DPDA" "EI01" "GL01" "GL41" "LN01" "LN03" "RR01")

echo "🎯 STEP 1: Drop all existing History tables"
echo "==========================================="

for table in "${tables[@]}"; do
    echo "🗑️  Dropping ${table}_History..."
    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        DROP TABLE IF EXISTS ${table}_History
    " > /dev/null 2>&1

    if [ $? -eq 0 ]; then
        echo -e "  ✅ ${GREEN}${table}_History dropped${NC}"
    else
        echo -e "  ❌ ${RED}Failed to drop ${table}_History${NC}"
    fi
done

echo ""
echo "🎯 STEP 2: Create History tables manually without IDENTITY"
echo "========================================================="

for table in "${tables[@]}"; do
    echo "🔧 Creating ${table}_History manually..."

    # Create history table step by step
    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        -- Step 1: Create empty table with basic structure
        CREATE TABLE ${table}_History (
            Id int NOT NULL,
            NGAY_DL date NULL,
            CREATED_DATE datetime2(7) NULL,
            UPDATED_DATE datetime2(7) NULL,
            FILE_NAME nvarchar(255) NULL,
            ValidFrom datetime2(7) NOT NULL,
            ValidTo datetime2(7) NOT NULL
        );
    " > /dev/null 2>&1

    if [ $? -ne 0 ]; then
        echo -e "  ⚠️  ${YELLOW}Basic structure failed, trying SELECT INTO...${NC}"

        # Alternative: Use SELECT INTO then modify
        sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
            -- Create using SELECT INTO
            SELECT TOP 0 * INTO ${table}_History FROM $table;

            -- Try to remove identity (this might fail, but we'll try)
            DECLARE @sql NVARCHAR(MAX)
            SET @sql = 'ALTER TABLE ${table}_History DROP CONSTRAINT ' +
                      (SELECT name FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('${table}_History'))
            IF @sql IS NOT NULL EXEC sp_executesql @sql
        " > /dev/null 2>&1
    fi

    # Check if table was created
    table_exists=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '${table}_History'
    " -h -1 2>/dev/null | sed 's/[^0-9]*//g')

    if [ "$table_exists" = "1" ]; then
        echo -e "  ✅ ${GREEN}${table}_History created${NC}"
    else
        echo -e "  ❌ ${RED}Failed to create ${table}_History${NC}"
    fi
done

echo ""
echo "🎯 STEP 3: Attempt System Versioning (one by one)"
echo "================================================="

success_count=0

for table in "${tables[@]}"; do
    echo "🔄 Enabling temporal for $table..."

    # Check if history table exists
    history_exists=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '${table}_History'
    " -h -1 2>/dev/null | sed 's/[^0-9]*//g')

    if [ "$history_exists" = "1" ]; then
        # Try to enable system versioning
        result=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
            ALTER TABLE $table SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.${table}_History))
        " 2>&1)

        if [[ $result == *"Msg"* ]]; then
            echo -e "  ❌ ${RED}Error: $result${NC}"
        else
            # Verify it worked
            temporal_type=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
                SELECT temporal_type_desc FROM sys.tables WHERE name = '$table'
            " -h -1 2>/dev/null | tr -d ' \n\r')

            if [[ "$temporal_type" == *"SYSTEM_VERSIONED"* ]]; then
                echo -e "  🎉 ${GREEN}SUCCESS: $table is now temporal!${NC}"
                ((success_count++))
            else
                echo -e "  ❌ ${RED}Still not temporal: $temporal_type${NC}"
            fi
        fi
    else
        echo -e "  ❌ ${RED}History table missing${NC}"
    fi
done

echo ""
echo "🎯 FINAL VERIFICATION"
echo "====================="

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

if [ $success_count -gt 0 ]; then
    echo -e "🎉 ${GREEN}At least some temporal tables are working!${NC}"

    # Test one temporal table if any
    if [ $success_count -gt 0 ]; then
        echo ""
        echo "🧪 Testing temporal functionality..."

        # Find first temporal table
        temporal_table=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
            SELECT TOP 1 name FROM sys.tables
            WHERE temporal_type_desc = 'SYSTEM_VERSIONED_TEMPORAL_TABLE'
            AND name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
        " -h -1 2>/dev/null | tr -d ' \n\r')

        if [ ! -z "$temporal_table" ]; then
            echo "Testing with table: $temporal_table"
            # Simple test - just show it exists
            sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
                SELECT COUNT(*) as RecordCount FROM $temporal_table;
                SELECT COUNT(*) as HistoryCount FROM ${temporal_table}_History;
            "
        fi
    fi
else
    echo -e "❌ ${RED}No temporal tables configured successfully${NC}"
    echo -e "⚠️  ${YELLOW}Azure SQL Edge may have limited temporal table support${NC}"
fi

echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
