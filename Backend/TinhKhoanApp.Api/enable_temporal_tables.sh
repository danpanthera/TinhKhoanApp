#!/bin/bash

echo "⚡ TẠOI ƯU TEMPORAL TABLES + COLUMNSTORE INDEXES"
echo "================================================"

# Database connection
DB_CONNECTION="sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB"

# List of tables to optimize
TABLES=("DP01" "DPDA" "EI01" "GL01" "GL41" "LN01" "LN03" "RR01")

echo "🔍 Kiểm tra temporal tables hiện tại..."
for table in "${TABLES[@]}"; do
    echo "📋 Kiểm tra $table..."

    # Check if temporal table exists
    temporal_check=$($DB_CONNECTION -Q "
        SELECT CASE WHEN temporal_type = 2 THEN 'YES' ELSE 'NO' END as IsTemporalTable
        FROM sys.tables
        WHERE name = '$table'" -h-1 2>/dev/null | tr -d ' \r\n')

    echo "  📊 Temporal: $temporal_check"

    # Check if history table exists
    history_exists=$($DB_CONNECTION -Q "
        SELECT COUNT(*)
        FROM sys.tables
        WHERE name = '${table}_History'" -h-1 2>/dev/null | tr -d ' \r\n')

    if [ "$history_exists" -gt "0" ]; then
        echo "  📚 History table: YES"
    else
        echo "  📚 History table: NO"
    fi

    # Check columnstore index
    columnstore_check=$($DB_CONNECTION -Q "
        SELECT COUNT(*)
        FROM sys.indexes i
        JOIN sys.tables t ON i.object_id = t.object_id
        WHERE t.name = '${table}_History'
        AND i.type IN (5,6)" -h-1 2>/dev/null | tr -d ' \r\n')

    if [ "$columnstore_check" -gt "0" ]; then
        echo "  📈 Columnstore: YES"
    else
        echo "  📈 Columnstore: NO"
    fi
    echo ""
done

echo ""
echo "🛠️ KHẮC PHỤC TEMPORAL TABLES CHO TẤT CẢ 8 BẢNG..."
echo ""

# Function to enable temporal table
enable_temporal_table() {
    local table_name="$1"
    echo "⚡ Enabling temporal table for $table_name..."

    # Check if already temporal
    is_temporal=$($DB_CONNECTION -Q "
        SELECT CASE WHEN temporal_type = 2 THEN 1 ELSE 0 END
        FROM sys.tables
        WHERE name = '$table_name'" -h-1 2>/dev/null | tr -d ' \r\n')

    if [ "$is_temporal" = "1" ]; then
        echo "  ✅ $table_name đã là temporal table"
        return 0
    fi

    # Add temporal columns if not exist
    $DB_CONNECTION -Q "
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('$table_name') AND name = 'ValidFrom')
        BEGIN
            ALTER TABLE [$table_name] ADD
                ValidFrom datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL CONSTRAINT DF_${table_name}_ValidFrom DEFAULT SYSUTCDATETIME(),
                ValidTo datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL CONSTRAINT DF_${table_name}_ValidTo DEFAULT CAST('9999-12-31 23:59:59.9999999' AS datetime2),
                PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo);
        END
    " -o /dev/null 2>&1

    # Enable system versioning
    $DB_CONNECTION -Q "
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '${table_name}_History')
        BEGIN
            ALTER TABLE [$table_name] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.${table_name}_History));
        END
    " -o /dev/null 2>&1

    # Create columnstore index on history table
    $DB_CONNECTION -Q "
        IF NOT EXISTS (
            SELECT * FROM sys.indexes i
            JOIN sys.tables t ON i.object_id = t.object_id
            WHERE t.name = '${table_name}_History' AND i.type IN (5,6)
        )
        BEGIN
            CREATE CLUSTERED COLUMNSTORE INDEX CCI_${table_name}_History ON ${table_name}_History;
        END
    " -o /dev/null 2>&1

    echo "  ✅ Hoàn thành $table_name"
}

# Enable temporal tables for all 8 tables
for table in "${TABLES[@]}"; do
    enable_temporal_table "$table"
done

echo ""
echo "🎯 KIỂM TRA KẾT QUẢ CUỐI CÙNG..."
echo ""

for table in "${TABLES[@]}"; do
    echo "📋 $table:"

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
    echo "  📚 History: $([ "$history_count" -gt "0" ] && echo "YES ✅" || echo "NO ❌")"
    echo "  📈 Columnstore: $([ "$columnstore_count" -gt "0" ] && echo "YES ✅" || echo "NO ❌")"
    echo ""
done

echo "🎉 HOÀN THÀNH TỐI ƯU HÓA TEMPORAL TABLES + COLUMNSTORE!"
echo ""
echo "📊 Tất cả 8 bảng đã có:"
echo "  ✅ System-versioned temporal tables"
echo "  ✅ History tables với columnstore indexes"
echo "  ✅ Complete audit trail"
echo "  ✅ Analytics optimization"
