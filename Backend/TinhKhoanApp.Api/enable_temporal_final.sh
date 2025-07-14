#!/bin/bash

echo "=== Enable Temporal Tables (Final Fix - No Identity in History) ==="
echo "Thời gian bắt đầu: $(date)"

# Database connection parameters
SERVER="localhost,1433"
USER="sa"
PASSWORD="YourStrong@Password123"
DATABASE="TinhKhoanDB"

# Danh sách 7 bảng còn lại
tables=("DP01" "DPDA" "EI01" "GL01" "GL41" "LN01" "LN03")

echo "Processing ${#tables[@]} remaining tables..."

for table in "${tables[@]}"; do
    echo ""
    echo "=== Processing table: $table ==="

    # Bước 1: Drop history table cũ
    echo "Step 1: Cleaning up existing artifacts for $table"
    sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
        IF EXISTS (SELECT * FROM sys.tables WHERE name = '${table}_History')
            DROP TABLE [${table}_History];
    " > /dev/null 2>&1

    # Bước 2: Tạo history table đúng cách (không có IDENTITY)
    echo "Step 2: Creating proper history table for $table"
    sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
        -- Lấy definition của main table
        DECLARE @sql NVARCHAR(MAX);

        -- Tạo history table bằng cách copy structure nhưng không copy IDENTITY
        SELECT @sql = 'CREATE TABLE [${table}_History] (' +
            STUFF((
                SELECT ', [' + c.name + '] ' +
                    CASE
                        WHEN c.is_identity = 1 THEN
                            UPPER(t.name) +
                            CASE
                                WHEN t.name IN ('varchar', 'char', 'nvarchar', 'nchar')
                                    THEN '(' + CAST(c.max_length as varchar) + ')'
                                WHEN t.name IN ('decimal', 'numeric')
                                    THEN '(' + CAST(c.precision as varchar) + ',' + CAST(c.scale as varchar) + ')'
                                ELSE ''
                            END + ' NOT NULL'
                        ELSE
                            UPPER(t.name) +
                            CASE
                                WHEN t.name IN ('varchar', 'char', 'nvarchar', 'nchar')
                                    THEN '(' + CAST(c.max_length as varchar) + ')'
                                WHEN t.name IN ('decimal', 'numeric')
                                    THEN '(' + CAST(c.precision as varchar) + ',' + CAST(c.scale as varchar) + ')'
                                ELSE ''
                            END +
                            CASE WHEN c.is_nullable = 1 THEN ' NULL' ELSE ' NOT NULL' END
                    END
                FROM sys.columns c
                INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
                WHERE c.object_id = OBJECT_ID('$table')
                    AND c.name NOT IN ('ValidFrom', 'ValidTo')
                FOR XML PATH('')
            ), 1, 2, '') +
            ', ValidFrom datetime2 NOT NULL, ValidTo datetime2 NOT NULL)';

        EXEC sp_executesql @sql;
    " -o "create_history_${table}.log" 2>&1

    if [ $? -eq 0 ]; then
        echo "✅ History table created for $table"

        # Bước 3: Enable system versioning
        echo "Step 3: Enabling system versioning for $table"
        sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
            ALTER TABLE [$table]
            SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[${table}_History]));
        " -o "final_enable_${table}.log" 2>&1

        if [ $? -eq 0 ]; then
            echo "✅ System versioning enabled for $table"
        else
            echo "❌ Failed to enable system versioning for $table"
            cat "final_enable_${table}.log"
        fi
    else
        echo "❌ Failed to create history table for $table"
        cat "create_history_${table}.log"
    fi

    echo "--- Completed $table ---"
done

echo ""
echo "=== FINAL VERIFICATION ==="

sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
SELECT
    t.name as TableName,
    CASE t.temporal_type
        WHEN 0 THEN '❌ NON_TEMPORAL'
        WHEN 2 THEN '✅ SYSTEM_VERSIONED'
        ELSE 'Unknown'
    END as Status,
    h.name as HistoryTable
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
ORDER BY t.name
"

echo ""
echo "Count of temporal tables enabled:"
sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
SELECT
    COUNT(*) as TemporalTablesCount
FROM sys.tables
WHERE name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
    AND temporal_type = 2
"

echo ""
echo "=== Hoàn thành: $(date) ==="
