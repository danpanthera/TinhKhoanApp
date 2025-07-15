#!/bin/bash

echo "🔧 ENABLE TEMPORAL TABLES FOR 8 CORE DATA TABLES"
echo "================================================="
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# Core tables array
tables=("DP01" "DPDA" "EI01" "GL01" "GL41" "LN01" "LN03" "RR01")

echo "🎯 STEP 1: Add Temporal Columns (ValidFrom, ValidTo)"
echo "===================================================="

for table in "${tables[@]}"; do
    echo "🔧 Adding temporal columns to $table..."

    # Check if columns already exist
    validfrom_exists=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = '$table' AND COLUMN_NAME = 'ValidFrom'
    " -h -1 2>/dev/null | sed 's/[^0-9]*//g')

    validto_exists=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = '$table' AND COLUMN_NAME = 'ValidTo'
    " -h -1 2>/dev/null | sed 's/[^0-9]*//g')

    if [ "$validfrom_exists" = "0" ]; then
        echo "  ➕ Adding ValidFrom column..."
        sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
            ALTER TABLE $table ADD ValidFrom datetime2 GENERATED ALWAYS AS ROW START HIDDEN
            CONSTRAINT DF_${table}_ValidFrom DEFAULT DATEADD(SECOND, -1, SYSUTCDATETIME());
        " > /dev/null 2>&1

        if [ $? -eq 0 ]; then
            echo -e "    ✅ ${GREEN}ValidFrom column added${NC}"
        else
            echo -e "    ❌ ${RED}Failed to add ValidFrom column${NC}"
        fi
    else
        echo -e "    ℹ️  ${YELLOW}ValidFrom column already exists${NC}"
    fi

    if [ "$validto_exists" = "0" ]; then
        echo "  ➕ Adding ValidTo column..."
        sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
            ALTER TABLE $table ADD ValidTo datetime2 GENERATED ALWAYS AS ROW END HIDDEN
            CONSTRAINT DF_${table}_ValidTo DEFAULT CONVERT(datetime2, '9999-12-31 23:59:59.9999999');
        " > /dev/null 2>&1

        if [ $? -eq 0 ]; then
            echo -e "    ✅ ${GREEN}ValidTo column added${NC}"
        else
            echo -e "    ❌ ${RED}Failed to add ValidTo column${NC}"
        fi
    else
        echo -e "    ℹ️  ${YELLOW}ValidTo column already exists${NC}"
    fi

    echo ""
done

echo "🎯 STEP 2: Add PERIOD FOR SYSTEM_TIME"
echo "====================================="

for table in "${tables[@]}"; do
    echo "🔧 Adding SYSTEM_TIME period to $table..."

    # Check if period already exists
    period_exists=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*) FROM sys.periods p
        JOIN sys.tables t ON p.object_id = t.object_id
        WHERE t.name = '$table' AND p.name = 'SYSTEM_TIME'
    " -h -1 2>/dev/null | sed 's/[^0-9]*//g')

    if [ "$period_exists" = "0" ]; then
        sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
            ALTER TABLE $table ADD PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo);
        " > /dev/null 2>&1

        if [ $? -eq 0 ]; then
            echo -e "  ✅ ${GREEN}SYSTEM_TIME period added${NC}"
        else
            echo -e "  ❌ ${RED}Failed to add SYSTEM_TIME period${NC}"
        fi
    else
        echo -e "  ℹ️  ${YELLOW}SYSTEM_TIME period already exists${NC}"
    fi
    echo ""
done

echo "🎯 STEP 3: Create History Tables"
echo "================================"

for table in "${tables[@]}"; do
    echo "🔧 Creating history table for $table..."

    # Check if history table already exists
    history_exists=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '${table}_History'
    " -h -1 2>/dev/null | sed 's/[^0-9]*//g')

    if [ "$history_exists" = "0" ]; then
        # Get the table schema for creating history table
        sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
            -- Create history table with same structure
            SELECT
                'CREATE TABLE ${table}_History (' +
                STRING_AGG(
                    COLUMN_NAME + ' ' + DATA_TYPE +
                    CASE WHEN CHARACTER_MAXIMUM_LENGTH IS NOT NULL
                         THEN '(' + CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')'
                         ELSE '' END +
                    CASE WHEN IS_NULLABLE = 'NO' THEN ' NOT NULL' ELSE ' NULL' END,
                    ', '
                ) +
                ');' AS CreateScript
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_NAME = '$table'
            ORDER BY ORDINAL_POSITION;
        " -h -1 > temp_create_${table}_history.sql 2>/dev/null

        # Execute the create script if it was generated properly
        if [ -f "temp_create_${table}_history.sql" ]; then
            # Simple approach: create history table with same structure
            sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
                SELECT * INTO ${table}_History FROM $table WHERE 1=0;
            " > /dev/null 2>&1

            if [ $? -eq 0 ]; then
                echo -e "  ✅ ${GREEN}History table ${table}_History created${NC}"
            else
                echo -e "  ❌ ${RED}Failed to create history table${NC}"
            fi

            # Clean up
            rm -f "temp_create_${table}_history.sql"
        fi
    else
        echo -e "  ℹ️  ${YELLOW}History table ${table}_History already exists${NC}"
    fi
    echo ""
done

echo "🎯 STEP 4: Enable System Versioning"
echo "==================================="

for table in "${tables[@]}"; do
    echo "🔧 Enabling system versioning for $table..."

    # Check if already system versioned
    is_temporal=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT temporal_type_desc FROM sys.tables WHERE name = '$table'
    " -h -1 2>/dev/null | tr -d ' \n\r')

    if [ "$is_temporal" = "NON_TEMPORAL_TABLE" ]; then
        sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
            ALTER TABLE $table SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.${table}_History));
        " > /dev/null 2>&1

        if [ $? -eq 0 ]; then
            echo -e "  ✅ ${GREEN}System versioning enabled${NC}"
        else
            echo -e "  ❌ ${RED}Failed to enable system versioning${NC}"
        fi
    else
        echo -e "  ℹ️  ${YELLOW}Table is already temporal: $is_temporal${NC}"
    fi
    echo ""
done

echo "🎯 FINAL VERIFICATION"
echo "====================="

echo "📊 Checking temporal status for all tables:"
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT
        t.name as TableName,
        t.temporal_type_desc as TemporalType,
        h.name as HistoryTable,
        CASE WHEN t.temporal_type_desc = 'SYSTEM_VERSIONED_TEMPORAL_TABLE' THEN 'YES' ELSE 'NO' END as IsEnabled
    FROM sys.tables t
    LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
    WHERE t.name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
    ORDER BY t.name;
"

echo ""
echo -e "✅ ${GREEN}Temporal Tables configuration completed!${NC}"
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
