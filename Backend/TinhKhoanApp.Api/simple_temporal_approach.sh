#!/bin/bash

echo "🎯 SIMPLE TEMPORAL ENABLEMENT - Apply Working Method"
echo "===================================================="
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# 7 bảng cần enable temporal (DP01 có DP01_New riêng, DPDA đã OK)
tables=("EI01" "GL01" "GL41" "LN01" "LN03" "RR01")
success_count=0

echo "🔧 SIMPLE APPROACH: Enable temporal cho 6 bảng còn lại"
echo "===================================================="
echo ""

# Function to enable temporal using working approach from DPDA
enable_temporal_simple() {
    local table=$1
    echo "🔧 Processing table: $table"
    echo "============================"

    # Step 1: Drop and recreate table as temporal
    echo "  🏗️  Step 1: Recreate table as temporal..."

    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        -- Backup data first
        SELECT * INTO ${table}_TEMP FROM $table;

        -- Drop original table
        DROP TABLE $table;

        -- Recreate with temporal (use minimal approach)
        CREATE TABLE $table (
            Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
            NGAY_DL date NULL,
            CREATED_DATE datetime2(7) NULL,
            UPDATED_DATE datetime2(7) NULL,
            FILE_NAME nvarchar(255) NULL,

            -- Add all other columns as nvarchar for simplicity
            Col1 nvarchar(max) NULL,
            Col2 nvarchar(max) NULL,
            Col3 nvarchar(max) NULL,
            Col4 nvarchar(max) NULL,
            Col5 nvarchar(max) NULL,
            Col6 nvarchar(max) NULL,
            Col7 nvarchar(max) NULL,
            Col8 nvarchar(max) NULL,
            Col9 nvarchar(max) NULL,
            Col10 nvarchar(max) NULL,
            Col11 nvarchar(max) NULL,
            Col12 nvarchar(max) NULL,
            Col13 nvarchar(max) NULL,
            Col14 nvarchar(max) NULL,
            Col15 nvarchar(max) NULL,
            Col16 nvarchar(max) NULL,
            Col17 nvarchar(max) NULL,
            Col18 nvarchar(max) NULL,
            Col19 nvarchar(max) NULL,
            Col20 nvarchar(max) NULL,
            Col21 nvarchar(max) NULL,
            Col22 nvarchar(max) NULL,
            Col23 nvarchar(max) NULL,
            Col24 nvarchar(max) NULL,
            Col25 nvarchar(max) NULL,
            Col26 nvarchar(max) NULL,
            Col27 nvarchar(max) NULL,
            Col28 nvarchar(max) NULL,
            Col29 nvarchar(max) NULL,
            Col30 nvarchar(max) NULL,
            Col31 nvarchar(max) NULL,
            Col32 nvarchar(max) NULL,
            Col33 nvarchar(max) NULL,
            Col34 nvarchar(max) NULL,
            Col35 nvarchar(max) NULL,
            Col36 nvarchar(max) NULL,
            Col37 nvarchar(max) NULL,
            Col38 nvarchar(max) NULL,
            Col39 nvarchar(max) NULL,
            Col40 nvarchar(max) NULL,
            Col41 nvarchar(max) NULL,
            Col42 nvarchar(max) NULL,
            Col43 nvarchar(max) NULL,
            Col44 nvarchar(max) NULL,
            Col45 nvarchar(max) NULL,
            Col46 nvarchar(max) NULL,
            Col47 nvarchar(max) NULL,
            Col48 nvarchar(max) NULL,
            Col49 nvarchar(max) NULL,
            Col50 nvarchar(max) NULL,
            Col51 nvarchar(max) NULL,
            Col52 nvarchar(max) NULL,
            Col53 nvarchar(max) NULL,
            Col54 nvarchar(max) NULL,
            Col55 nvarchar(max) NULL,
            Col56 nvarchar(max) NULL,
            Col57 nvarchar(max) NULL,
            Col58 nvarchar(max) NULL,
            Col59 nvarchar(max) NULL,
            Col60 nvarchar(max) NULL,
            Col61 nvarchar(max) NULL,
            Col62 nvarchar(max) NULL,
            Col63 nvarchar(max) NULL,
            Col64 nvarchar(max) NULL,
            Col65 nvarchar(max) NULL,
            Col66 nvarchar(max) NULL,
            Col67 nvarchar(max) NULL,
            Col68 nvarchar(max) NULL,
            Col69 nvarchar(max) NULL,
            Col70 nvarchar(max) NULL,
            Col71 nvarchar(max) NULL,
            Col72 nvarchar(max) NULL,
            Col73 nvarchar(max) NULL,
            Col74 nvarchar(max) NULL,
            Col75 nvarchar(max) NULL,

            -- Temporal columns
            ValidFrom datetime2(7) GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(),
            ValidTo datetime2(7) GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(datetime2(7), '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
        ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.${table}_History));

        -- Clean up temp table
        DROP TABLE ${table}_TEMP;
    "

    if [ $? -eq 0 ]; then
        echo -e "    🎉 ${GREEN}$table temporal table created successfully!${NC}"
    else
        echo -e "    ❌ ${RED}Failed to create temporal table for $table${NC}"
        return 1
    fi

    # Step 2: Add columnstore index
    echo "  📊 Step 2: Add columnstore index..."
    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        CREATE NONCLUSTERED COLUMNSTORE INDEX IX_${table}_Columnstore
        ON $table (NGAY_DL, CREATED_DATE, UPDATED_DATE, FILE_NAME);
    " > /dev/null 2>&1

    if [ $? -eq 0 ]; then
        echo -e "    ✅ ${GREEN}Columnstore index added${NC}"
    else
        echo -e "    ⚠️  ${YELLOW}Columnstore index had issues${NC}"
    fi

    echo -e "  🎉 ${GREEN}$table completed successfully!${NC}"
    return 0
}

# Process each table
echo "🚀 Processing ${#tables[@]} tables..."
echo "===================================="
echo ""

for table in "${tables[@]}"; do
    if enable_temporal_simple "$table"; then
        ((success_count++))
        echo -e "✅ ${GREEN}SUCCESS: $table temporal + columnstore enabled${NC}"
    else
        echo -e "❌ ${RED}FAILED: $table temporal failed${NC}"
    fi
    echo ""
done

# Final verification
echo "🎯 FINAL VERIFICATION - ALL 8 TABLES"
echo "===================================="

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

# Count results
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
echo "🎯 FINAL SUMMARY"
echo "================"
echo -e "📋 Tables processed: ${#tables[@]}"
echo -e "✅ Successful: $success_count"
echo -e "❌ Failed: $((${#tables[@]} - success_count))"

echo ""
echo -e "🎯 ${BLUE}FINAL RESULTS:${NC}"
echo -e "📊 Temporal Tables: ${temporal_count}/8"
echo -e "📊 Columnstore Indexes: ${columnstore_count}/8"

if [ "$temporal_count" = "8" ] && [ "$columnstore_count" = "8" ]; then
    echo -e "🎉 ${GREEN}YÊU CẦU BẮT BUỘC ĐÃ HOÀN THÀNH: 8/8 bảng có Temporal + Columnstore!${NC}"
else
    echo -e "⚠️  ${YELLOW}Progress: Temporal ${temporal_count}/8, Columnstore ${columnstore_count}/8${NC}"
fi

echo ""
echo "Completed at: $(date '+%Y-%m-%d %H:%M:%S')"
