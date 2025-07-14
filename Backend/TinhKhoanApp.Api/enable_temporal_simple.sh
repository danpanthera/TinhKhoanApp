#!/bin/bash

echo "=== Enable Temporal Tables (Simple approach for existing data) ==="
echo "Thời gian bắt đầu: $(date)"

# Database connection parameters
SERVER="localhost,1433"
USER="sa"
PASSWORD="YourStrong@Password123"
DATABASE="TinhKhoanDB"

# Danh sách 7 bảng còn lại (trừ RR01 đã thành công)
tables=("DP01" "DPDA" "EI01" "GL01" "GL41" "LN01" "LN03")

echo "Processing ${#tables[@]} remaining tables..."

for table in "${tables[@]}"; do
    echo ""
    echo "=== Processing table: $table ==="

    # Bước 1: Drop các cột ValidFrom/ValidTo cũ nếu có
    echo "Step 1: Cleaning existing period columns for $table"
    sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('$table') AND name = 'ValidFrom')
            ALTER TABLE [$table] DROP COLUMN ValidFrom;
        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('$table') AND name = 'ValidTo')
            ALTER TABLE [$table] DROP COLUMN ValidTo;
    " -o "clean_${table}.log" 2>&1

    # Bước 2: Tạo history table trước
    echo "Step 2: Creating history table for $table"
    sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '${table}_History')
        BEGIN
            SELECT * INTO [${table}_History] FROM [$table] WHERE 1=0;
            ALTER TABLE [${table}_History]
            ADD
                ValidFrom datetime2 NOT NULL,
                ValidTo datetime2 NOT NULL;
        END
    " -o "history_${table}.log" 2>&1

    # Bước 3: Thêm period columns với defaults cho main table
    echo "Step 3: Adding period columns with defaults for $table"
    sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
        ALTER TABLE [$table]
        ADD
            ValidFrom datetime2 GENERATED ALWAYS AS ROW START HIDDEN
                CONSTRAINT DF_${table}_ValidFrom DEFAULT SYSUTCDATETIME(),
            ValidTo datetime2 GENERATED ALWAYS AS ROW END HIDDEN
                CONSTRAINT DF_${table}_ValidTo DEFAULT CONVERT(datetime2, '9999-12-31 23:59:59.9999999'),
            PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo);
    " -o "add_period_${table}.log" 2>&1

    if [ $? -eq 0 ]; then
        echo "✅ Period columns added for $table"

        # Bước 4: Enable system versioning
        echo "Step 4: Enabling system versioning for $table"
        sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
            ALTER TABLE [$table]
            SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[${table}_History]));
        " -o "enable_${table}.log" 2>&1

        if [ $? -eq 0 ]; then
            echo "✅ System versioning enabled for $table"
        else
            echo "❌ Failed to enable system versioning for $table"
            cat "enable_${table}.log"
        fi
    else
        echo "❌ Failed to add period columns for $table"
        cat "add_period_${table}.log"
    fi

    echo "--- Completed $table ---"
done

echo ""
echo "=== Final Verification ==="

sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
SELECT
    t.name as TableName,
    t.temporal_type_desc as TemporalType,
    h.name as HistoryTable,
    CASE WHEN t.temporal_type = 2 THEN '✅ ENABLED' ELSE '❌ NOT ENABLED' END as Status
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
ORDER BY t.name
"

echo ""
echo "=== Hoàn thành: $(date) ==="
echo "Temporal Tables provides automatic audit trail for all data changes"
