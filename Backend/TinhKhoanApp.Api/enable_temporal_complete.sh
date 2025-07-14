#!/bin/bash

echo "=== Enable Temporal Tables với Period Columns + Columnstore ==="
echo "Thời gian bắt đầu: $(date)"

# Database connection parameters
SERVER="localhost,1433"
USER="sa"
PASSWORD="YourStrong@Password123"
DATABASE="TinhKhoanDB"

# Danh sách 8 bảng core data
tables=("DP01" "DPDA" "EI01" "GL01" "GL41" "LN01" "LN03" "RR01")

echo "Processing ${#tables[@]} core data tables..."

for table in "${tables[@]}"; do
    echo ""
    echo "=== Processing table: $table ==="

    # Step 1: Thêm period columns
    echo "Step 1: Adding period columns to $table"
    sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
        ALTER TABLE [$table]
        ADD
            ValidFrom datetime2 GENERATED ALWAYS AS ROW START HIDDEN,
            ValidTo datetime2 GENERATED ALWAYS AS ROW END HIDDEN,
            PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo);
    " -o "temporal_add_columns_${table}.log" 2>&1

    if [ $? -eq 0 ]; then
        echo "✅ Period columns added successfully to $table"
    else
        echo "❌ Failed to add period columns to $table"
        echo "Error details:"
        cat "temporal_add_columns_${table}.log"
        continue
    fi

    # Step 2: Enable system versioning
    echo "Step 2: Enabling system versioning for $table"
    sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
        ALTER TABLE [$table]
        SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[${table}_History]));
    " -o "temporal_enable_${table}.log" 2>&1

    if [ $? -eq 0 ]; then
        echo "✅ System versioning enabled for $table"
    else
        echo "❌ Failed to enable system versioning for $table"
        echo "Error details:"
        cat "temporal_enable_${table}.log"
        continue
    fi

    # Step 3: Create columnstore index
    echo "Step 3: Creating columnstore index for $table"
    sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
        CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_${table}_Columnstore]
        ON [$table] ([ImportDate], [Unit], [IndicatorCode]);
    " -o "columnstore_${table}.log" 2>&1

    if [ $? -eq 0 ]; then
        echo "✅ Columnstore index created for $table"
    else
        echo "❌ Failed to create columnstore index for $table (might already exist)"
        echo "Error details:"
        cat "columnstore_${table}.log"
    fi

    echo "--- Completed $table ---"
done

echo ""
echo "=== Final Verification ==="

# Verify temporal tables
echo "Checking temporal configuration:"
sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
SELECT
    t.name as TableName,
    t.temporal_type_desc,
    h.name as HistoryTable
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
ORDER BY t.name
"

# Verify columnstore indexes
echo ""
echo "Checking columnstore indexes:"
sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
SELECT
    t.name as TableName,
    i.name as IndexName,
    i.type_desc
FROM sys.tables t
INNER JOIN sys.indexes i ON t.object_id = i.object_id
WHERE t.name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
    AND i.type_desc = 'NONCLUSTERED COLUMNSTORE'
ORDER BY t.name
"

echo ""
echo "=== Hoàn thành: $(date) ==="
echo "Note: Temporal Tables cung cấp audit trail tự động"
echo "Note: Columnstore Indexes tăng performance cho analytics queries"
