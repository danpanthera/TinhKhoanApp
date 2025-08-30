using Microsoft.EntityFrameworkCore.Migrations;
using System.Data;
using Microsoft.Data.SqlClient;

namespace TinhKhoanApp.Api.Extensions
{
    /// <summary>
    /// Extension methods for safe database operations in migrations
    /// </summary>
    public static class MigrationExtensions
    {
        /// <summary>
        /// Execute SQL with error handling and logging
        /// </summary>
        public static void ExecuteSafeSql(this MigrationBuilder migrationBuilder, string sql, string operationName = "SQL Operation")
        {
            var safeSql = $@"
                BEGIN TRY
                    {sql}
                    PRINT '✅ {operationName} completed successfully'
                END TRY
                BEGIN CATCH
                    PRINT '❌ {operationName} failed: ' + ERROR_MESSAGE()
                    -- Continue execution, don't fail the migration
                END CATCH";

            migrationBuilder.Sql(safeSql);
        }

        /// <summary>
        /// Drop index with existence check
        /// </summary>
        public static void DropIndexIfExists(this MigrationBuilder migrationBuilder, string indexName, string tableName, string schema = "dbo")
        {
            var sql = $@"
                DECLARE @sql NVARCHAR(MAX)
                IF EXISTS (
                    SELECT 1
                    FROM sys.indexes i
                    INNER JOIN sys.objects o ON i.object_id = o.object_id
                    INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
                    WHERE i.name = '{indexName}'
                        AND o.name = '{tableName}'
                        AND s.name = '{schema}'
                )
                BEGIN
                    SET @sql = 'DROP INDEX [{indexName}] ON [{schema}].[{tableName}]'
                    EXEC sp_executesql @sql
                    PRINT '🗑️ Dropped index {indexName} on {schema}.{tableName}'
                END
                ELSE
                BEGIN
                    PRINT '⚠️ Index {indexName} does not exist on {schema}.{tableName} - skipping drop'
                END";

            migrationBuilder.ExecuteSafeSql(sql, $"Drop Index {indexName}");
        }

        /// <summary>
        /// Create index with existence check
        /// </summary>
        public static void CreateIndexIfNotExists(this MigrationBuilder migrationBuilder, string indexName, string tableName, string columns, bool isUnique = false, string schema = "dbo")
        {
            var uniqueKeyword = isUnique ? "UNIQUE" : "";
            var sql = $@"
                DECLARE @sql NVARCHAR(MAX)
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes i
                    INNER JOIN sys.objects o ON i.object_id = o.object_id
                    INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
                    WHERE i.name = '{indexName}'
                        AND o.name = '{tableName}'
                        AND s.name = '{schema}'
                )
                BEGIN
                    SET @sql = 'CREATE {uniqueKeyword} INDEX [{indexName}] ON [{schema}].[{tableName}] ({columns})'
                    EXEC sp_executesql @sql
                    PRINT '🆕 Created index {indexName} on {schema}.{tableName}'
                END
                ELSE
                BEGIN
                    PRINT '✅ Index {indexName} already exists on {schema}.{tableName} - skipping creation'
                END";

            migrationBuilder.ExecuteSafeSql(sql, $"Create Index {indexName}");
        }

        /// <summary>
        /// List all indexes on a table for debugging
        /// </summary>
        public static void ListTableIndexes(this MigrationBuilder migrationBuilder, string tableName, string schema = "dbo")
        {
            var sql = $@"
                PRINT '📋 Indexes on [{schema}].[{tableName}]:'
                SELECT
                    '  - ' + i.name + ' (' + i.type_desc + ')' AS IndexInfo
                FROM sys.indexes i
                INNER JOIN sys.objects o ON i.object_id = o.object_id
                INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
                WHERE o.name = '{tableName}'
                    AND s.name = '{schema}'
                    AND i.type > 0  -- Exclude heap
                ORDER BY i.name";

            migrationBuilder.Sql(sql);
        }
    }
}
