using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khoan.Api.Migrations
{
    /// <inheritdoc />
    public partial class GL41_Edge_NonTemporal_Analytics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Make GL41 NON-TEMPORAL on Azure SQL Edge and ensure analytics indexes (rowstore)
            var sql = @"
                -- 1) Turn OFF SYSTEM_VERSIONING if on
                IF EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID('dbo.GL41') AND temporal_type = 2)
                BEGIN
                    PRINT '⏸️ Disabling SYSTEM_VERSIONING on dbo.GL41 (Edge optimization)...';
                    ALTER TABLE dbo.GL41 SET (SYSTEM_VERSIONING = OFF);
                END

                -- 2) Drop history table if exists (we're making GL41 non-temporal on Edge)
                IF OBJECT_ID('dbo.GL41_History', 'U') IS NOT NULL
                BEGIN
                    PRINT '🗑️ Dropping GL41_History (Edge non-temporal mode)...';
                    DROP TABLE dbo.GL41_History;
                END

                -- 3) Ensure rowstore analytics indexes (Edge-friendly)
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL41_NGAY_DL' AND object_id = OBJECT_ID('dbo.GL41'))
                    CREATE NONCLUSTERED INDEX IX_GL41_NGAY_DL ON dbo.GL41 (NGAY_DL);

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL41_MA_CN' AND object_id = OBJECT_ID('dbo.GL41'))
                    CREATE NONCLUSTERED INDEX IX_GL41_MA_CN ON dbo.GL41 (MA_CN);

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL41_MA_TK' AND object_id = OBJECT_ID('dbo.GL41'))
                    CREATE NONCLUSTERED INDEX IX_GL41_MA_TK ON dbo.GL41 (MA_TK);

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL41_LOAI_TIEN' AND object_id = OBJECT_ID('dbo.GL41'))
                    CREATE NONCLUSTERED INDEX IX_GL41_LOAI_TIEN ON dbo.GL41 (LOAI_TIEN);

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL41_LOAI_BT' AND object_id = OBJECT_ID('dbo.GL41'))
                    CREATE NONCLUSTERED INDEX IX_GL41_LOAI_BT ON dbo.GL41 (LOAI_BT);

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'NCCI_GL41_Analytics' AND object_id = OBJECT_ID('dbo.GL41'))
                    CREATE NONCLUSTERED INDEX NCCI_GL41_Analytics ON dbo.GL41 (NGAY_DL, MA_CN, LOAI_TIEN, MA_TK, LOAI_BT);
            ";

            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rolling back: drop the analytics indexes only (can't auto-restore history data)
            var sql = @"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'NCCI_GL41_Analytics' AND object_id = OBJECT_ID('dbo.GL41'))
                    DROP INDEX NCCI_GL41_Analytics ON dbo.GL41;
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL41_LOAI_BT' AND object_id = OBJECT_ID('dbo.GL41'))
                    DROP INDEX IX_GL41_LOAI_BT ON dbo.GL41;
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL41_LOAI_TIEN' AND object_id = OBJECT_ID('dbo.GL41'))
                    DROP INDEX IX_GL41_LOAI_TIEN ON dbo.GL41;
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL41_MA_TK' AND object_id = OBJECT_ID('dbo.GL41'))
                    DROP INDEX IX_GL41_MA_TK ON dbo.GL41;
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL41_MA_CN' AND object_id = OBJECT_ID('dbo.GL41'))
                    DROP INDEX IX_GL41_MA_CN ON dbo.GL41;
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_GL41_NGAY_DL' AND object_id = OBJECT_ID('dbo.GL41'))
                    DROP INDEX IX_GL41_NGAY_DL ON dbo.GL41;
            ";

            migrationBuilder.Sql(sql);
        }
    }
}
