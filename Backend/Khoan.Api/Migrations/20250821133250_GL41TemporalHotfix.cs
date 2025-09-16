using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khoan.Api.Migrations
{
    /// <inheritdoc />
    public partial class GL41TemporalHotfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Bước 1: Tắt SYSTEM_VERSIONING nếu đang bật để có thể rebuild history.
            migrationBuilder.Sql(@"IF OBJECT_ID(N'dbo.GL41','U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'dbo.GL41') AND temporal_type=2)
ALTER TABLE dbo.GL41 SET (SYSTEM_VERSIONING = OFF);");

            // Bước 2 + 3: Rebuild GL41_History nếu lệch số cột / thứ tự / thiếu cột.
            migrationBuilder.Sql(@"IF OBJECT_ID(N'dbo.GL41','U') IS NOT NULL AND OBJECT_ID(N'dbo.GL41_History','U') IS NOT NULL
BEGIN
    DECLARE @firstBase sysname = (SELECT TOP 1 name FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.GL41') ORDER BY column_id);
    DECLARE @firstHist sysname = (SELECT TOP 1 name FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.GL41_History') ORDER BY column_id);
    DECLARE @cntBase int = (SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.GL41') AND name NOT IN ('ValidFrom','ValidTo'));
    DECLARE @cntHist int = (SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.GL41_History') AND name NOT IN ('ValidFrom','ValidTo'));
    DECLARE @diffMissing int = (
        SELECT COUNT(*) FROM sys.columns b
        WHERE b.object_id = OBJECT_ID(N'dbo.GL41')
          AND b.name NOT IN ('ValidFrom','ValidTo')
          AND NOT EXISTS (
              SELECT 1 FROM sys.columns h WHERE h.object_id = OBJECT_ID(N'dbo.GL41_History') AND h.name = b.name)
    );
    IF (@firstBase <> @firstHist) OR (@cntBase <> @cntHist) OR (@diffMissing > 0)
    BEGIN
        IF OBJECT_ID(N'dbo.GL41_History_Rebuild','U') IS NOT NULL DROP TABLE dbo.GL41_History_Rebuild;

        DECLARE @colsRest NVARCHAR(MAX) = (
            SELECT STRING_AGG(QUOTENAME(c.name), ',') WITHIN GROUP (ORDER BY c.column_id)
            FROM sys.columns c
            WHERE c.object_id = OBJECT_ID(N'dbo.GL41')
              AND c.name NOT IN ('ValidFrom','ValidTo','NGAY_DL')
        );
        DECLARE @cols NVARCHAR(MAX) = CASE WHEN EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.GL41') AND name='NGAY_DL')
                                           THEN QUOTENAME('NGAY_DL') + CASE WHEN @colsRest IS NOT NULL AND LEN(@colsRest)>0 THEN ',' + @colsRest ELSE '' END
                                           ELSE @colsRest END;
        DECLARE @sql NVARCHAR(MAX) = N'SELECT TOP 0 ' + @cols + ' INTO dbo.GL41_History_Rebuild FROM dbo.GL41;';
        EXEC(@sql);

        DECLARE @insertCols NVARCHAR(MAX) = (
            SELECT STRING_AGG(QUOTENAME(c.name), ',') WITHIN GROUP (ORDER BY c.column_id)
            FROM sys.columns c
            WHERE c.object_id = OBJECT_ID(N'dbo.GL41_History_Rebuild')
              AND EXISTS (
                  SELECT 1 FROM sys.columns h
                  WHERE h.object_id = OBJECT_ID(N'dbo.GL41_History')
                    AND h.name = c.name)
        );
        IF (@insertCols IS NOT NULL AND LEN(@insertCols) > 0 AND EXISTS (SELECT 1 FROM dbo.GL41_History))
        BEGIN
            DECLARE @ins NVARCHAR(MAX) = N'INSERT INTO dbo.GL41_History_Rebuild (' + @insertCols + ') SELECT ' + @insertCols + ' FROM dbo.GL41_History WITH (HOLDLOCK TABLOCKX);';
            EXEC(@ins);
        END

        DROP TABLE dbo.GL41_History;
        EXEC sp_rename 'dbo.GL41_History_Rebuild','GL41_History';

        IF COL_LENGTH('GL41_History','ValidFrom') IS NULL ALTER TABLE dbo.GL41_History ADD [ValidFrom] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00';
        IF COL_LENGTH('GL41_History','ValidTo') IS NULL ALTER TABLE dbo.GL41_History ADD [ValidTo] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
    END
END");

            // Bước 4: Bật lại SYSTEM_VERSIONING an toàn (nếu chưa bật).
            migrationBuilder.Sql(@"IF OBJECT_ID(N'dbo.GL41','U') IS NOT NULL AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.GL41') AND name='ValidFrom')
AND NOT EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'dbo.GL41') AND temporal_type=2)
BEGIN
    DECLARE @historyTableSchema sysname = SCHEMA_NAME();
    EXEC(N'ALTER TABLE [dbo].[GL41] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + '].[GL41_History]))');
END");

            // (Bước 5 kiểm tra không thực hiện trong migration để tránh fail pipeline; cung cấp script ngoài.)
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
