#!/bin/bash

echo "=== Enable Temporal Tables cho các bảng còn lại ==="
echo "Thời gian bắt đầu: $(date)"

# Database connection parameters
SERVER="localhost,1433"
USER="sa"
PASSWORD="YourStrong@Password123"
DATABASE="TinhKhoanDB"

# Danh sách 7 bảng còn lại (DP01 và RR01 đã done)
tables=("DPDA" "EI01" "GL01" "GL41" "LN01" "LN03")

echo "Processing ${#tables[@]} remaining tables..."

for table in "${tables[@]}"; do
    echo ""
    echo "=== Processing table: $table ==="

    # Generate và execute history table creation
    sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
        DECLARE @sql NVARCHAR(MAX) = 'IF EXISTS (SELECT * FROM sys.tables WHERE name = ''${table}_History'') DROP TABLE [${table}_History]; CREATE TABLE [${table}_History] ('

        SELECT @sql = @sql + '[' + c.name + '] ' +
            CASE
                WHEN c.is_identity = 1 THEN
                    '[' + UPPER(t.name) + ']' +
                    CASE
                        WHEN t.name IN ('varchar', 'char', 'nvarchar', 'nchar')
                            THEN '(' + CASE WHEN c.max_length = -1 THEN 'MAX'
                                           WHEN t.name LIKE 'n%' THEN CAST(c.max_length/2 as varchar)
                                           ELSE CAST(c.max_length as varchar) END + ')'
                        WHEN t.name IN ('decimal', 'numeric')
                            THEN '(' + CAST(c.precision as varchar) + ',' + CAST(c.scale as varchar) + ')'
                        WHEN t.name IN ('datetime2', 'time')
                            THEN '(' + CAST(c.scale as varchar) + ')'
                        ELSE ''
                    END + ' NOT NULL,'
                ELSE
                    '[' + UPPER(t.name) + ']' +
                    CASE
                        WHEN t.name IN ('varchar', 'char', 'nvarchar', 'nchar')
                            THEN '(' + CASE WHEN c.max_length = -1 THEN 'MAX'
                                           WHEN t.name LIKE 'n%' THEN CAST(c.max_length/2 as varchar)
                                           ELSE CAST(c.max_length as varchar) END + ')'
                        WHEN t.name IN ('decimal', 'numeric')
                            THEN '(' + CAST(c.precision as varchar) + ',' + CAST(c.scale as varchar) + ')'
                        WHEN t.name IN ('datetime2', 'time')
                            THEN '(' + CAST(c.scale as varchar) + ')'
                        ELSE ''
                    END +
                    CASE WHEN c.is_nullable = 1 THEN ' NULL,' ELSE ' NOT NULL,' END
            END
        FROM sys.columns c
        INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
        WHERE c.object_id = OBJECT_ID('$table')
        ORDER BY c.column_id

        SET @sql = LEFT(@sql, LEN(@sql) - 1) + ');'
        SET @sql = @sql + ' ALTER TABLE [$table] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[${table}_History]));'

        EXEC sp_executesql @sql;
        PRINT '$table Temporal enabled successfully!';
    " -o "temporal_${table}.log" 2>&1

    if [ $? -eq 0 ]; then
        echo "✅ $table temporal enabled successfully"
    else
        echo "❌ Failed to enable temporal for $table"
        cat "temporal_${table}.log"
    fi

    echo "--- Completed $table ---"
done

echo ""
echo "=== FINAL VERIFICATION ALL 8 TABLES ==="

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
echo "Summary - Count of enabled temporal tables:"
sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
SELECT
    COUNT(*) as TemporalTablesEnabled,
    8 as TotalTargetTables,
    CAST(COUNT(*) * 100.0 / 8 as decimal(5,1)) as PercentComplete
FROM sys.tables
WHERE name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
    AND temporal_type = 2
"

echo ""
echo "Columnstore indexes summary:"
sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
SELECT
    COUNT(*) as ColumnstoreIndexesCreated
FROM sys.tables t
INNER JOIN sys.indexes i ON t.object_id = i.object_id
WHERE t.name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
    AND i.type_desc = 'NONCLUSTERED COLUMNSTORE'
"

echo ""
echo "=== Hoàn thành: $(date) ==="
echo "🎉 TinhKhoanDB optimization complete!"
echo "💡 Temporal Tables: Automatic audit trail for all changes"
echo "⚡ Columnstore Indexes: High-performance analytics queries"
