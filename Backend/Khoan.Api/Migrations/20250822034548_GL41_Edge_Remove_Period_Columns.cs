using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khoan.Api.Migrations
{
    /// <inheritdoc />
    public partial class GL41_Edge_Remove_Period_Columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = @"
                -- Remove PERIOD FOR SYSTEM_TIME if any remains and drop hidden period columns
                DECLARE @hasPeriod int = 0;
                SELECT @hasPeriod = COUNT(*)
                FROM sys.periods p
                WHERE p.object_id = OBJECT_ID('dbo.GL41');

                IF (@hasPeriod > 0)
                BEGIN
                    PRINT 'Dropping PERIOD FOR SYSTEM_TIME from dbo.GL41...';
                    ALTER TABLE dbo.GL41 DROP PERIOD FOR SYSTEM_TIME;
                END

                -- Drop potential system time columns if they exist (names vary by creation method)
                IF COL_LENGTH('dbo.GL41', 'SysStartTime') IS NOT NULL
                BEGIN
                    ALTER TABLE dbo.GL41 DROP COLUMN SysStartTime;
                END
                IF COL_LENGTH('dbo.GL41', 'SysEndTime') IS NOT NULL
                BEGIN
                    ALTER TABLE dbo.GL41 DROP COLUMN SysEndTime;
                END
                IF COL_LENGTH('dbo.GL41', 'ValidFrom') IS NOT NULL
                BEGIN
                    ALTER TABLE dbo.GL41 DROP COLUMN ValidFrom;
                END
                IF COL_LENGTH('dbo.GL41', 'ValidTo') IS NOT NULL
                BEGIN
                    ALTER TABLE dbo.GL41 DROP COLUMN ValidTo;
                END
            ";
            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op: can't recreate temporal period columns automatically without data loss decisions
        }
    }
}
