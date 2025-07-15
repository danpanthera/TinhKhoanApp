#!/bin/bash

echo "🔧 FIX DP01 + ENABLE TEMPORAL + COLUMNSTORE FOR 8 TABLES"
echo "========================================================="
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

echo "🎯 STEP 1: FIX DP01 - Remove 2 extra columns"
echo "============================================="
echo "CSV has 63 columns, DB has 65. Removing DATA_DATE and TEN_TAI_KHOAN..."

sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    ALTER TABLE DP01 DROP COLUMN DATA_DATE;
    ALTER TABLE DP01 DROP COLUMN TEN_TAI_KHOAN;
"

if [ $? -eq 0 ]; then
    echo -e "✅ ${GREEN}DP01 columns fixed - now matches CSV (63 columns)${NC}"
else
    echo -e "❌ ${RED}Failed to remove columns from DP01${NC}"
fi

echo ""
echo "🎯 STEP 2: ENABLE TEMPORAL TABLES FOR 8 CORE TABLES"
echo "==================================================="

tables=("DP01" "DPDA" "EI01" "GL01" "GL41" "LN01" "LN03" "RR01")

for table in "${tables[@]}"; do
    echo "🔧 Enabling temporal for $table..."

    # Add ValidFrom and ValidTo columns if not exist
    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$table' AND COLUMN_NAME = 'ValidFrom')
        BEGIN
            ALTER TABLE $table ADD ValidFrom datetime2 GENERATED ALWAYS AS ROW START NOT NULL DEFAULT GETUTCDATE();
        END

        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$table' AND COLUMN_NAME = 'ValidTo')
        BEGIN
            ALTER TABLE $table ADD ValidTo datetime2 GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(datetime2, '9999-12-31 23:59:59.9999999');
        END

        -- Add period
        IF NOT EXISTS (SELECT * FROM sys.periods WHERE object_id = OBJECT_ID('$table'))
        BEGIN
            ALTER TABLE $table ADD PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo);
        END
    " 2>/dev/null

    # Enable system versioning
    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '$table' AND temporal_type = 2)
        BEGIN
            ALTER TABLE $table SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.${table}_History));
        END
    " 2>/dev/null

    if [ $? -eq 0 ]; then
        echo -e "  ✅ ${GREEN}$table temporal enabled${NC}"
    else
        echo -e "  ❌ ${RED}$table temporal failed${NC}"
    fi
done

echo ""
echo "🎯 STEP 3: ADD COLUMNSTORE INDEXES"
echo "=================================="

for table in "${tables[@]}"; do
    echo "📊 Adding columnstore index for $table..."

    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('$table') AND type_desc LIKE '%COLUMNSTORE%')
        BEGIN
            CREATE NONCLUSTERED COLUMNSTORE INDEX IX_${table}_Columnstore ON $table (
                NGAY_DL, CREATED_DATE, UPDATED_DATE, FILE_NAME
            );
        END
    " 2>/dev/null

    if [ $? -eq 0 ]; then
        echo -e "  ✅ ${GREEN}$table columnstore added${NC}"
    else
        echo -e "  ❌ ${RED}$table columnstore failed${NC}"
    fi
done

echo ""
echo "🎯 STEP 4: VERIFICATION"
echo "======================"

echo "📋 Final column count for DP01:"
final_count=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DP01' AND COLUMN_NAME NOT IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME','ValidFrom','ValidTo')" -h -1 | sed 's/[^0-9]*//g')
echo "  Business columns: $final_count (should be 63)"

echo ""
echo "📋 Temporal status:"
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT
        t.name as TableName,
        CASE
            WHEN t.temporal_type_desc = 'SYSTEM_VERSIONED_TEMPORAL_TABLE' THEN 'ENABLED'
            ELSE 'DISABLED'
        END as TemporalStatus
    FROM sys.tables t
    WHERE t.name IN ('DP01','DPDA','EI01','GL01','GL41','LN01','LN03','RR01')
    ORDER BY t.name
" -h -1

echo ""
echo "📋 Columnstore status:"
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT
        t.name as TableName,
        CASE
            WHEN COUNT(i.index_id) > 0 THEN 'ENABLED'
            ELSE 'DISABLED'
        END as ColumnstoreStatus
    FROM sys.tables t
    LEFT JOIN sys.indexes i ON t.object_id = i.object_id AND i.type_desc LIKE '%COLUMNSTORE%'
    WHERE t.name IN ('DP01','DPDA','EI01','GL01','GL41','LN01','LN03','RR01')
    GROUP BY t.name
    ORDER BY t.name
" -h -1

echo ""
echo "✅ Fix completed at: $(date '+%Y-%m-%d %H:%M:%S')"
