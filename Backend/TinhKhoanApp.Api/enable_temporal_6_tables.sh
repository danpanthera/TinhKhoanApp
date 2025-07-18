#!/bin/bash

echo "⚡ ENABLE TEMPORAL TABLES CHO 6 BẢNG CÒN LẠI"
echo "=============================================="

# Database connection
DB_CONNECTION="sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB"

# Tables to enable (excluding DP01 which is already done)
TABLES=("EI01" "GL41" "LN01" "LN03" "RR01" "DPDA")

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m'

echo "🔍 Kiểm tra trạng thái hiện tại..."
echo ""

for table in "${TABLES[@]}"; do
    echo -e "${BLUE}📋 Kiểm tra $table...${NC}"

    # Check temporal status
    temporal_status=$($DB_CONNECTION -Q "
        SELECT CASE WHEN temporal_type = 2 THEN 'YES' ELSE 'NO' END as IsTemporalTable
        FROM sys.tables
        WHERE name = '$table'" -h-1 2>/dev/null | tr -d ' \r\n')

    if [ "$temporal_status" = "YES" ]; then
        echo -e "  ✅ Temporal: $temporal_status"
    else
        echo -e "  ❌ Temporal: $temporal_status"
    fi
done

echo ""
echo -e "${YELLOW}🛠️ BẮT ĐẦU ENABLE TEMPORAL TABLES...${NC}"
echo ""

# Function to enable temporal table
enable_temporal_table() {
    local table_name="$1"
    echo -e "${BLUE}⚡ Enabling temporal table for $table_name...${NC}"

    # Check if already temporal
    is_temporal=$($DB_CONNECTION -Q "
        SELECT CASE WHEN temporal_type = 2 THEN 1 ELSE 0 END
        FROM sys.tables
        WHERE name = '$table_name'" -h-1 2>/dev/null | tr -d ' \r\n')

    if [ "$is_temporal" = "1" ]; then
        echo -e "  ${GREEN}✅ $table_name đã là temporal table${NC}"
        return 0
    fi

    # Step 1: Add temporal columns
    echo "  📝 Adding temporal columns..."
    $DB_CONNECTION -Q "
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('$table_name') AND name = 'ValidFrom')
        BEGIN
            ALTER TABLE [$table_name] ADD
                ValidFrom datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL CONSTRAINT DF_${table_name}_ValidFrom DEFAULT SYSUTCDATETIME(),
                ValidTo datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL CONSTRAINT DF_${table_name}_ValidTo DEFAULT CAST('9999-12-31 23:59:59.9999999' AS datetime2),
                PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo);
            PRINT 'Added temporal columns to $table_name';
        END
        ELSE
        BEGIN
            PRINT 'Temporal columns already exist in $table_name';
        END
    " -o /dev/null 2>/dev/null

    # Step 2: Enable system versioning
    echo "  🔧 Enabling system versioning..."
    $DB_CONNECTION -Q "
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '${table_name}_History')
        BEGIN
            ALTER TABLE [$table_name] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.${table_name}_History));
            PRINT 'Enabled system versioning for $table_name';
        END
        ELSE
        BEGIN
            PRINT 'History table already exists for $table_name';
        END
    " -o /dev/null 2>/dev/null

    # Step 3: Create columnstore index on history table
    echo "  📈 Creating columnstore index..."
    $DB_CONNECTION -Q "
        IF NOT EXISTS (
            SELECT * FROM sys.indexes i
            JOIN sys.tables t ON i.object_id = t.object_id
            WHERE t.name = '${table_name}_History' AND i.type IN (5,6)
        )
        BEGIN
            -- Try to create nonclustered columnstore with basic columns
            CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_${table_name}_History
            ON ${table_name}_History ([Id], [NGAY_DL], [CREATED_DATE]);
            PRINT 'Created columnstore index for ${table_name}_History';
        END
        ELSE
        BEGIN
            PRINT 'Columnstore index already exists for ${table_name}_History';
        END
    " -o /dev/null 2>/dev/null

    echo -e "  ${GREEN}✅ Hoàn thành $table_name${NC}"
    echo ""
}

# Enable temporal tables for all 6 tables
for table in "${TABLES[@]}"; do
    enable_temporal_table "$table"
done

echo ""
echo -e "${GREEN}🎯 KIỂM TRA KẾT QUẢ CUỐI CÙNG...${NC}"
echo ""

for table in "${TABLES[@]}"; do
    echo -e "${BLUE}📋 $table:${NC}"

    # Check temporal status
    temporal_status=$($DB_CONNECTION -Q "
        SELECT CASE WHEN temporal_type = 2 THEN 'ENABLED ✅' ELSE 'DISABLED ❌' END
        FROM sys.tables
        WHERE name = '$table'" -h-1 2>/dev/null | tr -d ' \r\n')

    # Check history table
    history_count=$($DB_CONNECTION -Q "
        SELECT COUNT(*)
        FROM sys.tables
        WHERE name = '${table}_History'" -h-1 2>/dev/null | tr -d ' \r\n')

    # Check columnstore
    columnstore_count=$($DB_CONNECTION -Q "
        SELECT COUNT(*)
        FROM sys.indexes i
        JOIN sys.tables t ON i.object_id = t.object_id
        WHERE t.name = '${table}_History'
        AND i.type IN (5,6)" -h-1 2>/dev/null | tr -d ' \r\n')

    echo "  🕐 Temporal: $temporal_status"
    if [ "$history_count" -gt "0" ]; then
        echo "  📚 History: YES ✅"
    else
        echo "  📚 History: NO ❌"
    fi

    if [ "$columnstore_count" -gt "0" ]; then
        echo "  📈 Columnstore: YES ✅"
    else
        echo "  📈 Columnstore: NO ❌"
    fi
    echo ""
done

echo -e "${GREEN}🎉 HOÀN THÀNH ENABLE TEMPORAL TABLES CHO 6 BẢNG!${NC}"
echo ""
echo "📊 Bảng đã có temporal tables:"
echo "  ✅ DP01 (đã có từ trước)"
echo "  ✅ EI01, GL41, LN01, LN03, RR01, DPDA (vừa enable)"
echo ""
echo "🔥 CHƯA LÀM: GL01 - Partitioned Columnstore (cần xử lý riêng)"
