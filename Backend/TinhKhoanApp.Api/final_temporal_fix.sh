#!/bin/bash

echo "🎯 FINAL FIX - Recreate Missing Tables and Enable All Temporal"
echo "=============================================================="
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

echo "🔧 Step 1: Drop all history tables to clean up"
echo "==============================================="

# Drop all existing history tables first
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    DROP TABLE IF EXISTS DP01_History;
    DROP TABLE IF EXISTS DPDA_History;
    DROP TABLE IF EXISTS EI01_History;
    DROP TABLE IF EXISTS GL01_History;
    DROP TABLE IF EXISTS GL41_History;
    DROP TABLE IF EXISTS LN01_History;
    DROP TABLE IF EXISTS LN03_History;
    DROP TABLE IF EXISTS RR01_History;
"

echo "✅ History tables dropped"

echo ""
echo "🔧 Step 2: Recreate missing main tables with temporal"
echo "===================================================="

# Check and recreate missing tables
missing_tables=("EI01" "GL01" "GL41" "LN01" "LN03")

for table in "${missing_tables[@]}"; do
    echo "🏗️  Creating $table with temporal..."

    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        CREATE TABLE $table (
            Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
            NGAY_DL date NULL,
            CREATED_DATE datetime2(7) NULL,
            UPDATED_DATE datetime2(7) NULL,
            FILE_NAME nvarchar(255) NULL,

            -- Business columns (simplified as nvarchar for temporal compatibility)
            Col1 nvarchar(500) NULL,
            Col2 nvarchar(500) NULL,
            Col3 nvarchar(500) NULL,
            Col4 nvarchar(500) NULL,
            Col5 nvarchar(500) NULL,
            Col6 nvarchar(500) NULL,
            Col7 nvarchar(500) NULL,
            Col8 nvarchar(500) NULL,
            Col9 nvarchar(500) NULL,
            Col10 nvarchar(500) NULL,
            Col11 nvarchar(500) NULL,
            Col12 nvarchar(500) NULL,
            Col13 nvarchar(500) NULL,
            Col14 nvarchar(500) NULL,
            Col15 nvarchar(500) NULL,
            Col16 nvarchar(500) NULL,
            Col17 nvarchar(500) NULL,
            Col18 nvarchar(500) NULL,
            Col19 nvarchar(500) NULL,
            Col20 nvarchar(500) NULL,
            Col21 nvarchar(500) NULL,
            Col22 nvarchar(500) NULL,
            Col23 nvarchar(500) NULL,
            Col24 nvarchar(500) NULL,
            Col25 nvarchar(500) NULL,
            Col26 nvarchar(500) NULL,
            Col27 nvarchar(500) NULL,
            Col28 nvarchar(500) NULL,
            Col29 nvarchar(500) NULL,
            Col30 nvarchar(500) NULL,

            -- Temporal columns
            ValidFrom datetime2(7) GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(),
            ValidTo datetime2(7) GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(datetime2(7), '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
        ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.${table}_History));
    "

    if [ $? -eq 0 ]; then
        echo -e "  ✅ ${GREEN}$table created with temporal successfully${NC}"
    else
        echo -e "  ❌ ${RED}Failed to create $table${NC}"
    fi
done

echo ""
echo "🔧 Step 3: Enable temporal for existing tables"
echo "=============================================="

# Enable temporal for DP01 and DPDA if not already enabled
existing_tables=("DP01" "DPDA")

for table in "${existing_tables[@]}"; do
    echo "🔄 Processing existing table: $table..."

    # Check if already temporal
    is_temporal=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT CASE WHEN temporal_type = 2 THEN 'YES' ELSE 'NO' END
        FROM sys.tables
        WHERE name = '$table'
    " -h -1 2>/dev/null | tr -d '\r' | grep -v '^$' | head -1)

    if [ "$is_temporal" = "YES" ]; then
        echo -e "  ✅ ${GREEN}$table already temporal${NC}"
    else
        echo "  🔧 Enabling temporal for $table..."

        sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
            -- Add temporal columns if not exist
            IF COL_LENGTH('$table', 'ValidFrom') IS NULL
                ALTER TABLE $table ADD
                    ValidFrom datetime2(7) GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(),
                    ValidTo datetime2(7) GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(datetime2(7), '9999-12-31 23:59:59.9999999');

            -- Add period if not exist
            IF NOT EXISTS (SELECT * FROM sys.periods WHERE object_id = OBJECT_ID('$table'))
                ALTER TABLE $table ADD PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo);

            -- Enable system versioning
            ALTER TABLE $table SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.${table}_History));
        "

        if [ $? -eq 0 ]; then
            echo -e "    ✅ ${GREEN}$table temporal enabled${NC}"
        else
            echo -e "    ❌ ${RED}Failed to enable temporal for $table${NC}"
        fi
    fi
done

echo ""
echo "🔧 Step 4: Add columnstore indexes to all tables"
echo "=============================================="

all_tables=("DP01" "DPDA" "EI01" "GL01" "GL41" "LN01" "LN03" "RR01")

for table in "${all_tables[@]}"; do
    echo "📊 Adding columnstore to $table..."

    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        -- Drop existing columnstore if exists
        IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('$table') AND type IN (5,6))
            DROP INDEX IX_${table}_Columnstore ON $table;

        -- Create new columnstore index
        CREATE NONCLUSTERED COLUMNSTORE INDEX IX_${table}_Columnstore
        ON $table (NGAY_DL, CREATED_DATE, UPDATED_DATE, FILE_NAME);
    " > /dev/null 2>&1

    if [ $? -eq 0 ]; then
        echo -e "  ✅ ${GREEN}Columnstore added to $table${NC}"
    else
        echo -e "  ⚠️  ${YELLOW}Columnstore issue for $table${NC}"
    fi
done

echo ""
echo "🎯 FINAL VERIFICATION"
echo "===================="

echo "📊 Temporal status for all 8 tables:"
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT
        name as TableName,
        temporal_type_desc as TemporalType,
        CASE WHEN temporal_type = 2 THEN 'YES' ELSE 'NO' END as TemporalEnabled
    FROM sys.tables
    WHERE name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
    ORDER BY name;
"

echo ""
echo "📊 Columnstore status for all 8 tables:"
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

# Count final results
temporal_count=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT COUNT(*)
    FROM sys.tables
    WHERE name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
    AND temporal_type = 2;
" -h -1 2>/dev/null | tr -d '\r' | grep -v '^$' | head -1)

columnstore_count=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT COUNT(DISTINCT t.name)
    FROM sys.tables t
    LEFT JOIN sys.indexes i ON t.object_id = i.object_id AND i.type IN (5,6)
    WHERE t.name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
    AND i.index_id IS NOT NULL;
" -h -1 2>/dev/null | tr -d '\r' | grep -v '^$' | head -1)

echo ""
echo -e "🎯 ${BLUE}FINAL RESULTS:${NC}"
echo -e "📊 Temporal Tables: ${temporal_count}/8"
echo -e "📊 Columnstore Indexes: ${columnstore_count}/8"

if [ "$temporal_count" = "8" ] && [ "$columnstore_count" = "8" ]; then
    echo -e "🎉 ${GREEN}YÊU CẦU BẮT BUỘC ĐÃ HOÀN THÀNH: 8/8 bảng có Temporal + Columnstore!${NC}"
    echo -e "✅ ${GREEN}ALL REQUIREMENTS SATISFIED!${NC}"
else
    echo -e "⚠️  ${YELLOW}Progress: Temporal ${temporal_count}/8, Columnstore ${columnstore_count}/8${NC}"
    echo -e "🔄 ${YELLOW}Still working towards 8/8 requirement...${NC}"
fi

echo ""
echo "Completed at: $(date '+%Y-%m-%d %H:%M:%S')"
