using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khoan.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGL41ColumnstoreIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Guarded SQL: Create GL41 columnstore index only on supported editions
            var sql = @"
                DECLARE @edition NVARCHAR(128) = CAST(SERVERPROPERTY('Edition') AS NVARCHAR(128));
                PRINT '🔎 SQL Server Edition: ' + ISNULL(@edition, 'Unknown');

                IF (@edition LIKE '%Azure SQL Edge%')
                BEGIN
                    PRINT '⚠️ Azure SQL Edge detected – skipping Columnstore creation for GL41.';
                    RETURN;
                END

                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes WHERE name = 'IX_GL41_Columnstore' AND object_id = OBJECT_ID('dbo.GL41')
                )
                BEGIN
                    BEGIN TRY
                        CREATE NONCLUSTERED COLUMNSTORE INDEX IX_GL41_Columnstore ON dbo.GL41
                        (
                            NGAY_DL, MA_CN, LOAI_TIEN, MA_TK, TEN_TK,
                            DN_DAUKY, DC_DAUKY, SBT_NO, ST_GHINO, SBT_CO, ST_GHICO, DN_CUOIKY, DC_CUOIKY
                        );
                        PRINT '✅ Created IX_GL41_Columnstore successfully.';
                    END TRY
                    BEGIN CATCH
                        PRINT '❌ Failed to create IX_GL41_Columnstore: ' + ERROR_MESSAGE();
                    END CATCH
                END
                ELSE
                BEGIN
                    PRINT '✅ IX_GL41_Columnstore already exists – skipping creation.';
                END
            ";

            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL41_Columnstore' AND object_id = OBJECT_ID('dbo.GL41'))
                BEGIN
                    DROP INDEX IX_GL41_Columnstore ON dbo.GL41;
                    PRINT '🗑️ Dropped IX_GL41_Columnstore.';
                END
                ELSE
                BEGIN
                    PRINT 'ℹ️ IX_GL41_Columnstore does not exist – nothing to drop.';
                END
            ");
        }
    }
}
