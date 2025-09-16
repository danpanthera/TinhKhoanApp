using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khoan.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeeKpiAssignmentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeKpiAssignments_KPIDefinitions_KpiDefinitionId",
                table: "EmployeeKpiAssignments");

            migrationBuilder.DropIndex(
                name: "IX_GL41_MaCN",
                table: "GL41");

            migrationBuilder.DropIndex(
                name: "IX_GL41_NGAY_DL",
                table: "GL41");

            migrationBuilder.DropIndex(
                name: "IX_GL41_NGAY_DL_MaCN",
                table: "GL41");

            migrationBuilder.DropIndex(
                name: "IX_DP01_MaPGD",
                table: "DP01");

            migrationBuilder.DropIndex(
                name: "IX_DP01_NGAY_DL_MaCN",
                table: "DP01");

            migrationBuilder.DropIndex(
                name: "NCCI_DP01_Analytics",
                table: "DP01");

            // SAFE DROPS: convert prior DropColumn calls to conditional raw SQL to avoid errors if columns already removed.
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns c JOIN sys.tables t ON c.object_id=t.object_id WHERE t.name='RR01' AND c.name='DATA_SOURCE') ALTER TABLE [RR01] DROP COLUMN [DATA_SOURCE];");
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns c JOIN sys.tables t ON c.object_id=t.object_id WHERE t.name='RR01' AND c.name='ERROR_MESSAGE') ALTER TABLE [RR01] DROP COLUMN [ERROR_MESSAGE];");
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns c JOIN sys.tables t ON c.object_id=t.object_id WHERE t.name='RR01' AND c.name='IMPORT_BATCH_ID') ALTER TABLE [RR01] DROP COLUMN [IMPORT_BATCH_ID];");
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns c JOIN sys.tables t ON c.object_id=t.object_id WHERE t.name='RR01' AND c.name='PROCESSING_STATUS') ALTER TABLE [RR01] DROP COLUMN [PROCESSING_STATUS];");
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns c JOIN sys.tables t ON c.object_id=t.object_id WHERE t.name='RR01' AND c.name='ROW_HASH') ALTER TABLE [RR01] DROP COLUMN [ROW_HASH];");
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns c JOIN sys.tables t ON c.object_id=t.object_id WHERE t.name='GL41' AND c.name='BATCH_ID') ALTER TABLE [GL41] DROP COLUMN [BATCH_ID];");
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns c JOIN sys.tables t ON c.object_id=t.object_id WHERE t.name='GL41' AND c.name='CREATED_DATE') ALTER TABLE [GL41] DROP COLUMN [CREATED_DATE];");
            // Guard: Drop legacy duplicate CREATED_DATE in GL41_History to prevent EF temporal AddColumn from failing with 2705
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns c JOIN sys.tables t ON c.object_id=t.object_id WHERE t.name='GL41_History' AND c.name='CREATED_DATE') ALTER TABLE [GL41_History] DROP COLUMN [CREATED_DATE];");
            // Guard: Drop primary key on GL41_History if present because custom history tables cannot have unique constraints when enabling system-versioning
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE [name]='PK_GL41_History' AND parent_object_id = OBJECT_ID(N'[dbo].[GL41_History]')) ALTER TABLE [dbo].[GL41_History] DROP CONSTRAINT [PK_GL41_History];");
            // Guard: Rebuild GL41_History without IDENTITY if the Id column is currently identity (temporal history table cannot have identity)
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GL41_History]') AND name='Id' AND is_identity=1)
BEGIN
    SELECT TOP 0 * INTO [dbo].[GL41_History_MigrateTmp] FROM [dbo].[GL41_History];
    INSERT INTO [dbo].[GL41_History_MigrateTmp] SELECT * FROM [dbo].[GL41_History];
    DROP TABLE [dbo].[GL41_History];
    CREATE TABLE [dbo].[GL41_History] (
        [Id] int NOT NULL,
        [BusinessKey] nvarchar(500) NOT NULL,
        [EffectiveDate] datetime2 NOT NULL,
        [ExpiryDate] datetime2 NULL,
        [IsCurrent] bit NOT NULL,
        [RowVersion] int NOT NULL,
        [ImportId] nvarchar(100) NOT NULL,
        [StatementDate] datetime2 NOT NULL,
        [ProcessedDate] datetime2 NOT NULL,
        [DataHash] nvarchar(64) NOT NULL,
        [JOURNAL_NO] nvarchar(50) NULL,
        [ACCOUNT_NO] nvarchar(50) NULL,
        [ACCOUNT_NAME] nvarchar(500) NULL,
        [CUSTOMER_ID] nvarchar(50) NULL,
        [CUSTOMER_NAME] nvarchar(500) NULL,
        [TRANSACTION_DATE] datetime2 NULL,
        [POSTING_DATE] datetime2 NULL,
        [DESCRIPTION] nvarchar(1000) NULL,
        [DEBIT_AMOUNT] decimal(18,2) NULL,
        [CREDIT_AMOUNT] decimal(18,2) NULL,
        [DEBIT_BALANCE] decimal(18,2) NULL,
        [CREDIT_BALANCE] decimal(18,2) NULL,
        [BRCD] nvarchar(20) NULL,
        [BRANCH_NAME] nvarchar(200) NULL,
        [TRANSACTION_TYPE] nvarchar(100) NULL,
        [ORIGINAL_TRANS_ID] nvarchar(50) NULL,
        [UPDATED_DATE] datetime2 NULL,
        [RawDataJson] nvarchar(max) NULL,
        [AdditionalData] nvarchar(max) NULL
    );
    SET IDENTITY_INSERT [dbo].[GL41_History] ON; -- no identity but safe if Id values overlap (ignored)
    INSERT INTO [dbo].[GL41_History] (
        [Id],[BusinessKey],[EffectiveDate],[ExpiryDate],[IsCurrent],[RowVersion],[ImportId],[StatementDate],[ProcessedDate],[DataHash],[JOURNAL_NO],[ACCOUNT_NO],[ACCOUNT_NAME],[CUSTOMER_ID],[CUSTOMER_NAME],[TRANSACTION_DATE],[POSTING_DATE],[DESCRIPTION],[DEBIT_AMOUNT],[CREDIT_AMOUNT],[DEBIT_BALANCE],[CREDIT_BALANCE],[BRCD],[BRANCH_NAME],[TRANSACTION_TYPE],[ORIGINAL_TRANS_ID],[UPDATED_DATE],[RawDataJson],[AdditionalData]
    )
    SELECT
        [Id],[BusinessKey],[EffectiveDate],[ExpiryDate],[IsCurrent],[RowVersion],[ImportId],[StatementDate],[ProcessedDate],[DataHash],[JOURNAL_NO],[ACCOUNT_NO],[ACCOUNT_NAME],[CUSTOMER_ID],[CUSTOMER_NAME],[TRANSACTION_DATE],[POSTING_DATE],[DESCRIPTION],[DEBIT_AMOUNT],[CREDIT_AMOUNT],[DEBIT_BALANCE],[CREDIT_BALANCE],[BRCD],[BRANCH_NAME],[TRANSACTION_TYPE],[ORIGINAL_TRANS_ID],[UPDATED_DATE],[RawDataJson],[AdditionalData]
    FROM [dbo].[GL41_History_MigrateTmp];
    SET IDENTITY_INSERT [dbo].[GL41_History] OFF;
    DROP TABLE [dbo].[GL41_History_MigrateTmp];
END");

            // Late-stage dynamic rebuild of GL41_History to guarantee final physical column order matches GL41
            // This MUST come after the placeholder + comprehensive alignment block so nothing overrides ordering.
            migrationBuilder.Sql(@"IF OBJECT_ID(N'[dbo].[GL41]','U') IS NOT NULL AND OBJECT_ID(N'[dbo].[GL41_History]','U') IS NOT NULL
BEGIN
    DECLARE @firstBase sysname = (SELECT TOP 1 name FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GL41]') ORDER BY column_id);
    DECLARE @firstHist sysname = (SELECT TOP 1 name FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GL41_History]') ORDER BY column_id);
    DECLARE @cntBase int = (SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GL41]') AND name NOT IN ('ValidFrom','ValidTo'));
    DECLARE @cntHist int = (SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GL41_History]') AND name NOT IN ('ValidFrom','ValidTo'));
    DECLARE @diffMissing int = (
        SELECT COUNT(*) FROM sys.columns b
        WHERE b.object_id = OBJECT_ID(N'[dbo].[GL41]')
          AND b.name NOT IN ('ValidFrom','ValidTo')
          AND NOT EXISTS (
              SELECT 1 FROM sys.columns h WHERE h.object_id = OBJECT_ID(N'[dbo].[GL41_History]') AND h.name = b.name)
    );
    IF (@firstBase <> @firstHist) OR (@cntBase <> @cntHist) OR (@diffMissing > 0)
    BEGIN
        IF OBJECT_ID(N'[dbo].[GL41_History_Rebuild]','U') IS NOT NULL DROP TABLE [dbo].[GL41_History_Rebuild];

        -- Build ordered column list from base (exclude period columns); force NGAY_DL first if present
        DECLARE @colsRest NVARCHAR(MAX) = (
            SELECT STRING_AGG(QUOTENAME(c.name), ',') WITHIN GROUP (ORDER BY c.column_id)
            FROM sys.columns c
            WHERE c.object_id = OBJECT_ID(N'[dbo].[GL41]')
              AND c.name NOT IN ('ValidFrom','ValidTo','NGAY_DL')
        );
        DECLARE @cols NVARCHAR(MAX) = CASE WHEN EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GL41]') AND name='NGAY_DL')
                                           THEN QUOTENAME('NGAY_DL') + CASE WHEN @colsRest IS NOT NULL AND LEN(@colsRest)>0 THEN ',' + @colsRest ELSE '' END
                                           ELSE @colsRest END;
        DECLARE @sql NVARCHAR(MAX) = N'SELECT TOP 0 ' + @cols + ' INTO [dbo].[GL41_History_Rebuild] FROM [dbo].[GL41];';
        EXEC(@sql);

        -- Copy intersecting data
        DECLARE @insertCols NVARCHAR(MAX) = (
            SELECT STRING_AGG(QUOTENAME(c.name), ',') WITHIN GROUP (ORDER BY c.column_id)
            FROM sys.columns c
            WHERE c.object_id = OBJECT_ID(N'[dbo].[GL41_History_Rebuild]')
              AND EXISTS (
                  SELECT 1 FROM sys.columns h
                  WHERE h.object_id = OBJECT_ID(N'[dbo].[GL41_History]')
                    AND h.name = c.name
              )
        );
        IF (@insertCols IS NOT NULL AND LEN(@insertCols) > 0 AND EXISTS (SELECT 1 FROM [dbo].[GL41_History]))
        BEGIN
            DECLARE @ins NVARCHAR(MAX) = N'INSERT INTO [dbo].[GL41_History_Rebuild] (' + @insertCols + ') SELECT ' + @insertCols + ' FROM [dbo].[GL41_History] WITH (HOLDLOCK TABLOCKX);';
            EXEC(@ins);
        END

        DROP TABLE [dbo].[GL41_History];
        EXEC sp_rename 'dbo.GL41_History_Rebuild','GL41_History';

        -- Ensure period boundary columns (defensive)
        IF COL_LENGTH('GL41_History','ValidFrom') IS NULL ALTER TABLE [GL41_History] ADD [ValidFrom] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00';
        IF COL_LENGTH('GL41_History','ValidTo') IS NULL ALTER TABLE [GL41_History] ADD [ValidTo] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
    END
END");
            // Simplified alignment: DO NOT dynamically rebuild GL41_History here (can race before period columns exist on base).
            // We will instead ensure required columns exist just before enabling system versioning.
            // (Dynamic rebuild block removed to stabilize idempotent re-runs.)
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns c JOIN sys.tables t ON c.object_id=t.object_id WHERE t.name='GL41' AND c.name='IMPORT_SESSION_ID') ALTER TABLE [GL41] DROP COLUMN [IMPORT_SESSION_ID];");
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns c JOIN sys.tables t ON c.object_id=t.object_id WHERE t.name='Employees' AND c.name='UserIPCAS') ALTER TABLE [Employees] DROP COLUMN [UserIPCAS];");
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.columns c JOIN sys.tables t ON c.object_id=t.object_id WHERE t.name='EI01' AND c.name='FILE_NAME') ALTER TABLE [EI01] DROP COLUMN [FILE_NAME];");

            // Conditional renames to avoid failure if already applied previously
            migrationBuilder.Sql(@"IF COL_LENGTH('RR01','ValidTo') IS NOT NULL AND COL_LENGTH('RR01','SysStartTime') IS NULL EXEC sp_rename 'RR01.ValidTo','SysStartTime','COLUMN';");
            migrationBuilder.Sql(@"IF COL_LENGTH('RR01','ValidFrom') IS NOT NULL AND COL_LENGTH('RR01','SysEndTime') IS NULL EXEC sp_rename 'RR01.ValidFrom','SysEndTime','COLUMN';");

            migrationBuilder.Sql(@"IF COL_LENGTH('LN03','ValidTo') IS NOT NULL AND COL_LENGTH('LN03','SysStartTime') IS NULL EXEC sp_rename 'LN03.ValidTo','SysStartTime','COLUMN';");
            migrationBuilder.Sql(@"IF COL_LENGTH('LN03','ValidFrom') IS NOT NULL AND COL_LENGTH('LN03','SysEndTime') IS NULL EXEC sp_rename 'LN03.ValidFrom','SysEndTime','COLUMN';");

            migrationBuilder.Sql(@"IF COL_LENGTH('LN01','ValidTo') IS NOT NULL AND COL_LENGTH('LN01','SysStartTime') IS NULL EXEC sp_rename 'LN01.ValidTo','SysStartTime','COLUMN';");
            migrationBuilder.Sql(@"IF COL_LENGTH('LN01','ValidFrom') IS NOT NULL AND COL_LENGTH('LN01','SysEndTime') IS NULL EXEC sp_rename 'LN01.ValidFrom','SysEndTime','COLUMN';");

            migrationBuilder.Sql(@"IF COL_LENGTH('GL41','ID') IS NOT NULL AND COL_LENGTH('GL41','Id') IS NULL EXEC sp_rename 'GL41.ID','Id','COLUMN';");
            migrationBuilder.Sql(@"IF COL_LENGTH('GL41','ValidTo') IS NOT NULL AND COL_LENGTH('GL41','UpdatedAt') IS NULL EXEC sp_rename 'GL41.ValidTo','UpdatedAt','COLUMN';");
            migrationBuilder.Sql(@"IF COL_LENGTH('GL41','ValidFrom') IS NOT NULL AND COL_LENGTH('GL41','CreatedAt') IS NULL EXEC sp_rename 'GL41.ValidFrom','CreatedAt','COLUMN';");

            migrationBuilder.Sql(@"IF COL_LENGTH('EmployeeKpiAssignments','KpiDefinitionId') IS NOT NULL AND COL_LENGTH('EmployeeKpiAssignments','KpiIndicatorId') IS NULL EXEC sp_rename 'EmployeeKpiAssignments.KpiDefinitionId','KpiIndicatorId','COLUMN';");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeKpiAssignments_KpiDefinitionId",
                table: "EmployeeKpiAssignments",
                newName: "IX_EmployeeKpiAssignments_KpiIndicatorId");

            migrationBuilder.Sql(@"IF COL_LENGTH('EI01','UPDATED_DATE') IS NOT NULL AND COL_LENGTH('EI01','ValidTo1') IS NULL EXEC sp_rename 'EI01.UPDATED_DATE','ValidTo1','COLUMN';");
            migrationBuilder.Sql(@"IF COL_LENGTH('EI01','CREATED_DATE') IS NOT NULL AND COL_LENGTH('EI01','ValidFrom1') IS NULL EXEC sp_rename 'EI01.CREATED_DATE','ValidFrom1','COLUMN';");

            migrationBuilder.RenameIndex(
                name: "IX_EI01_NGAY_DL_MaCN",
                table: "EI01",
                newName: "NCCI_EI01_Analytics");

            migrationBuilder.RenameIndex(
                name: "IX_EI01_MaCN",
                table: "EI01",
                newName: "IX_EI01_MA_CN");

            migrationBuilder.Sql(@"IF COL_LENGTH('DPDA','ValidTo') IS NOT NULL AND COL_LENGTH('DPDA','SysStartTime') IS NULL EXEC sp_rename 'DPDA.ValidTo','SysStartTime','COLUMN';");
            migrationBuilder.Sql(@"IF COL_LENGTH('DPDA','ValidFrom') IS NOT NULL AND COL_LENGTH('DPDA','SysEndTime') IS NULL EXEC sp_rename 'DPDA.ValidFrom','SysEndTime','COLUMN';");

            migrationBuilder.Sql(@"IF COL_LENGTH('DP01','ValidTo') IS NOT NULL AND COL_LENGTH('DP01','SysStartTime') IS NULL EXEC sp_rename 'DP01.ValidTo','SysStartTime','COLUMN';");
            migrationBuilder.Sql(@"IF COL_LENGTH('DP01','ValidFrom') IS NOT NULL AND COL_LENGTH('DP01','SysEndTime') IS NULL EXEC sp_rename 'DP01.ValidFrom','SysEndTime','COLUMN';");

            migrationBuilder.RenameIndex(
                name: "IX_DP01_MaCN",
                table: "DP01",
                newName: "IX_DP01_MA_CN");

            // Removed AlterTable for RR01 to avoid implicit sp_rename attempts (duplicate ValidTo/ValidFrom vs SysStartTime/SysEndTime).
            // (No-op) migrationBuilder.Sql("-- Skipped AlterTable RR01 to avoid duplicate temporal period column rename.");

            // Removed AlterTable for LN03 to avoid implicit sp_rename attempts.
            // (No-op) migrationBuilder.Sql("-- Skipped AlterTable LN03 to avoid duplicate temporal period column rename.");

            // Removed AlterTable for LN01 to avoid implicit sp_rename attempts.
            // (No-op) migrationBuilder.Sql("-- Skipped AlterTable LN01 to avoid duplicate temporal period column rename.");

            migrationBuilder.AlterTable(
                name: "GL41")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            // Ensure GL41_History has the same non-system columns as GL41 (including future period columns ValidFrom/ValidTo once added below).
            // Add missing business columns if EF re-run scenario skipped earlier AddColumn operations.
            migrationBuilder.Sql(@"IF OBJECT_ID(N'[dbo].[GL41_History]','U') IS NOT NULL
BEGIN
    -- Add columns introduced earlier in the migration if absent (idempotent guards)
    IF COL_LENGTH('GL41_History','BATCH_ID') IS NULL ALTER TABLE [GL41_History] ADD [BATCH_ID] nvarchar(100) NULL;
    IF COL_LENGTH('GL41_History','CREATED_DATE') IS NULL ALTER TABLE [GL41_History] ADD [CREATED_DATE] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00';
    IF COL_LENGTH('GL41_History','IMPORT_SESSION_ID') IS NULL ALTER TABLE [GL41_History] ADD [IMPORT_SESSION_ID] nvarchar(100) NULL;
END");

            // Guarded addition of new GL41 business columns while table is non-temporal (added after temporal annotations removed, before re-enable).
            // This replaces late EF AddColumn calls that previously occurred AFTER re-enabling temporal (causing duplicate adds in history table).
            migrationBuilder.Sql(@"IF OBJECT_ID(N'[dbo].[GL41]','U') IS NOT NULL AND COL_LENGTH('GL41','BATCH_ID') IS NULL ALTER TABLE [GL41] ADD [BATCH_ID] nvarchar(100) NULL;
IF OBJECT_ID(N'[dbo].[GL41]','U') IS NOT NULL AND COL_LENGTH('GL41','CREATED_DATE') IS NULL ALTER TABLE [GL41] ADD [CREATED_DATE] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00';
IF OBJECT_ID(N'[dbo].[GL41]','U') IS NOT NULL AND COL_LENGTH('GL41','IMPORT_SESSION_ID') IS NULL ALTER TABLE [GL41] ADD [IMPORT_SESSION_ID] nvarchar(100) NULL;");

            migrationBuilder.AlterTable(
                name: "EI01")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterTable(
                name: "DPDA")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterTable(
                name: "DP01")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            // Skipped AlterColumn for RR01.VAMC_FLG to avoid index dependency (analytics/columnstore) issues.
            // Original EF-generated AlterColumn removed; leaving existing schema intact.

            migrationBuilder.AlterColumn<DateTime>(
                name: "UPDATED_DATE",
                table: "RR01",
                type: "datetime2(3)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 28)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "TSK",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 26)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 25)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "THU_LAI",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 23)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 22)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "THU_GOC",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 22)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 21)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            // Guard block retained as documentation (no structural change performed)
            migrationBuilder.Sql(@"
IF COL_LENGTH('RR01','VAMC_FLG') IS NOT NULL
BEGIN
    -- Intentionally not altering VAMC_FLG (index dependency). Ensure at least one no-op statement for valid T-SQL block.
    SELECT 1; -- no-op
END");

            // (Moved) GL41_History dynamic rebuild block relocated after final comprehensive alignment section
            // so that any placeholder recreation cannot overwrite the intended column ordering.
            // Skipped AlterColumn for RR01.SO_LDS (decimal->string) due to dependency index NCCI_RR01_Analytics; retaining existing decimal column.

            // Skipped AlterColumn for RR01.SO_LAV to avoid index dependency (NCCI_RR01_Analytics) similar to SO_LDS.
            // Existing schema retained; no change applied.

            // Down migration: SO_LAV alteration skipped; Up made no change for same reason (index dependency).

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_GIAI_NGAN",
                table: "RR01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 10)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 9)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DL",
                table: "RR01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("Relational:ColumnOrder", 1)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 0)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DEN_HAN",
                table: "RR01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 11)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 10)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            // Final guard before enabling system-versioning for GL41 later: ensure history table has period columns if it already exists.
            migrationBuilder.Sql(@"IF OBJECT_ID(N'[dbo].[GL41_History]','U') IS NOT NULL
BEGIN
    -- Period columns must exist with same definitions (they won't be HIDDEN in history, just plain columns)
    IF COL_LENGTH('GL41_History','ValidFrom') IS NULL ALTER TABLE [GL41_History] ADD [ValidFrom] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00';
    IF COL_LENGTH('GL41_History','ValidTo')   IS NULL ALTER TABLE [GL41_History] ADD [ValidTo]   datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END");


            // Skipped AlterColumn for RR01.MA_KH to avoid index dependency (NCCI_RR01_Analytics). Schema left unchanged.

            // Skipped AlterColumn for RR01.LOAI_KH to avoid index dependency (NCCI_RR01_Analytics) just like MA_KH/SO_LAV/SO_LDS.
            // Existing schema retained; if future change required, create dedicated migration with coordinated index strategy.

            migrationBuilder.AlterColumn<string>(
                name: "FILE_NAME",
                table: "RR01",
                type: "nvarchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 29)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            // Down migration: MA_KH alteration skipped; Up made no change (index dependency retained existing definition).

            migrationBuilder.AlterColumn<decimal>(
                name: "DUNO_NGAN_HAN",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 19)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 18)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "DUNO_LAI_TICHLUY_BD",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 15)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 14)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "DUNO_LAI_HIENTAI",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 18)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 17)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "DUNO_GOC_HIENTAI",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 17)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 16)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "DUNO_GOC_BAN_DAU",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 14)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 13)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "DUNO_DAI_HAN",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 21)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 20)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "DS",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 25)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 24)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "DOC_DAUKY_DA_THU_HT",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 16)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 15)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CREATED_DATE",
                table: "RR01",
                type: "datetime2(3)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 27)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            // Skipped AlterColumn for RR01.CN_LOAI_I due to dependency by index NCCI_RR01_Analytics (avoid ALTER on indexed column).
            // Schema left unchanged; future modification requires coordinated index strategy in separate migration.

            // Skipped AlterColumn for RR01.CCY (index NCCI_RR01_Analytics depends on this column).
            // Leaving existing definition untouched to avoid blocking migration. Any future change
            // must be coordinated with an explicit drop/recreate (or ONLINE rebuild) of that index
            // in a dedicated, carefully scheduled migration.

            // Skipped AlterColumn for RR01.BRCD (indexed by NCCI_RR01_Analytics). Leaving original definition intact.

            migrationBuilder.AlterColumn<decimal>(
                name: "BDS",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 24)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 23)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            // Skipped altering RR01.Id (primary key) to avoid dependency errors if already correct.

            migrationBuilder.AlterColumn<DateTime>(
                name: "SysStartTime",
                table: "RR01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SysEndTime",
                table: "RR01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "THUNOSAUXL",
                table: "LN03",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TENKH",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TENCHINHANH",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TENCBTD",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 15)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TAIKHOANHACHTOAN",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 17)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "SOTIENXLRR",
                table: "LN03",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "SOTIEN",
                table: "LN03",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 22)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "SOHOPDONG",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "REFNO",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 18)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "NHOMNO",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 13)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DL",
                table: "LN03",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAYPHATSINHXL",
                table: "LN03",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MAPGD",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 16)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MALOAI",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 20)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MAKH",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MACHINHANH",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MACBTD",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 14)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "LOAINGUONVON",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 19)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "LOAIKHACHHANG",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 21)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "FILE_ORIGIN",
                table: "LN03",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 25)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "DUNONOIBANG",
                table: "LN03",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 12)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CREATED_DATE",
                table: "LN03",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("Relational:ColumnOrder", 24)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "CREATED_BY",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 23)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "CONLAINGOAIBANG",
                table: "LN03",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "LN03",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 11)
                .Annotation("SqlServer:Identity", "1, 1")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SysStartTime",
                table: "LN03",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SysEndTime",
                table: "LN03",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "YRDAYS",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 60)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "USRIDOP",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 54)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UPDATED_DATE",
                table: "LN01",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 84)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "TY_GIA",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 10,
                scale: 6,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,6)",
                oldPrecision: 10,
                oldScale: 6,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 79)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TRCTNM",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 36)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TRCTCD",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 35)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TRANSACTION_DATE",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 9)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "TOTAL_INTEREST_REPAY_AMOUNT",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 32)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TAI_KHOAN_GIAI_NGAN_2",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 73)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TAI_KHOAN_GIAI_NGAN_1",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 70)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TAI_KHOAN",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 5)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "SO_TIEN_GIAI_NGAN_2",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 74)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "SO_TIEN_GIAI_NGAN_1",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 71)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "SECURED_PERCENT",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 45)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "REPAYMENT_AMOUNT",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 24)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "REMARK",
                table: "LN01",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 61)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "PROVINCE",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 38)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "PHUONG_THUC_GIAI_NGAN_2",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 72)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "PHUONG_THUC_GIAI_NGAN_1",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 69)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "PASTDUE_INTEREST_AMOUNT",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 31)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "OFFICER_NAME",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 29)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "OFFICER_IPCAS",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 80)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "OFFICER_ID",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 28)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "NHOM_NO",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 46)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_SINH",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 76)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DL",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 1)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NEXT_REPAY_DATE",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 25)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "NEXT_REPAY_AMOUNT",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 26)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NEXT_INT_REPAY_DATE",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 27)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MA_NGANH_KT",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 78)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MA_CB_AGRI",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 77)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "LOAN_TYPE",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 21)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "LCLWARDNM",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 43)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "LCLPROVINNM",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 39)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "LCLDISTNM",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 41)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LAST_REPAY_DATE",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 44)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LAST_INT_CHARGE_DATE",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 47)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "INT_PAYMENT_INTERVAL",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 67)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "INT_PARTIAL_PAYMENT_TYPE",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 66)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "INT_LUMPSUM_PARTIAL_TYPE",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 65)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "INTTRMMTH",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 59)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "INTRPYMTH",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 58)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "INTEREST_RATE",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 15)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "INTEREST_AMOUNT",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 30)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "INTCMTH",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 57)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "GRPNO",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 51)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "FUND_RESOURCE_CODE",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 22)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "FUND_PURPOSE_CODE",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 23)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "FILE_NAME",
                table: "LN01",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 82)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "EXEMPTINTTYPE",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 49)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "EXEMPTINTAMT",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 50)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "EXEMPTINT",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 48)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "DU_NO",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 7)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "DSBSSEQ",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 8)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DSBSMATDT",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 13)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DSBSDT",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 10)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "DISTRICT",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 40)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "DISBUR_CCY",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 11)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "DISBURSEMENT_AMOUNT",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 12)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "CUSTSEQ",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 3)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "CUSTOMER_TYPE_CODE_DETAIL",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 34)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "CUSTOMER_TYPE_CODE",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 33)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "CUSTNM",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 4)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "CTCV",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 63)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "CREDIT_LINE_YPE",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 64)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CREATED_DATE",
                table: "LN01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("Relational:ColumnOrder", 83)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "COMMCD",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 42)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "CMT_HC",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 75)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "CHITIEU",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 62)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "CCY",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 6)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "BUSCD",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 52)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "BSRTCD",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 14)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "BSNSSCLTPCD",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 53)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "BRCD",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 2)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "APPR_CCY",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 18)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "APPRSEQ",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 16)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "APPRMATDT",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 20)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "APPRDT",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 17)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "APPRAMT",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 19)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "AN_HAN_LAI",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 68)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "ADDR1",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 37)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "ACCRUAL_AMOUNT_END_OF_MONTH",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 56)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "ACCRUAL_AMOUNT",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 55)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "LN01",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Relational:ColumnOrder", 81)
                .Annotation("SqlServer:Identity", "1, 1")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SysStartTime",
                table: "LN01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SysEndTime",
                table: "LN01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TEN_TK",
                table: "GL41",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 4)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "ST_GHINO",
                table: "GL41",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 9)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "ST_GHICO",
                table: "GL41",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 11)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "SBT_NO",
                table: "GL41",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 8)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "SBT_CO",
                table: "GL41",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 10)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DL",
                table: "GL41",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .OldAnnotation("Relational:ColumnOrder", 0)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MA_TK",
                table: "GL41",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 3)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MA_CN",
                table: "GL41",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 1)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "LOAI_TIEN",
                table: "GL41",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 2)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "LOAI_BT",
                table: "GL41",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 5)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "FILE_NAME",
                table: "GL41",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 14)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "DN_DAUKY",
                table: "GL41",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 6)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "DN_CUOIKY",
                table: "GL41",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 12)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "DC_DAUKY",
                table: "GL41",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 7)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "DC_CUOIKY",
                table: "GL41",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 13)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "GL41",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("Relational:ColumnOrder", 18)
                .OldAnnotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "GL41",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "GL41",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidTo",
                table: "EI01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidFrom",
                table: "EI01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "USER_SMS",
                table: "EI01",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 22)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "USER_SAV",
                table: "EI01",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 23)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "USER_OTT",
                table: "EI01",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 21)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "USER_LN",
                table: "EI01",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 24)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "USER_EMB",
                table: "EI01",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 20)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TRANG_THAI_SMS",
                table: "EI01",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 12)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TRANG_THAI_SAV",
                table: "EI01",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 15)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TRANG_THAI_OTT",
                table: "EI01",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 9)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TRANG_THAI_LN",
                table: "EI01",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 18)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TRANG_THAI_EMB",
                table: "EI01",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 6)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TEN_KH",
                table: "EI01",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 3)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "SDT_SMS",
                table: "EI01",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 11)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "SDT_SAV",
                table: "EI01",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 14)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "SDT_OTT",
                table: "EI01",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 8)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "SDT_LN",
                table: "EI01",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 17)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "SDT_EMB",
                table: "EI01",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 5)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DL",
                table: "EI01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 0)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DK_SMS",
                table: "EI01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 13)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DK_SAV",
                table: "EI01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 16)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DK_OTT",
                table: "EI01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 10)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DK_LN",
                table: "EI01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 19)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DK_EMB",
                table: "EI01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 7)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MA_KH",
                table: "EI01",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 2)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MA_CN",
                table: "EI01",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 1)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "LOAI_KH",
                table: "EI01",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 4)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "EI01",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 25)
                .OldAnnotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidTo1",
                table: "EI01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 27)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidFrom1",
                table: "EI01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1")
                .OldAnnotation("Relational:ColumnOrder", 26)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EI01",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EI01",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<string>(
                name: "USER_PHAT_HANH",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UPDATED_DATE",
                table: "DPDA",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TRANG_THAI",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TEN_KHACH_HANG",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "SO_THE",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "SO_TAI_KHOAN",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "PHAN_LOAI",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_PHAT_HANH",
                table: "DPDA",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_NOP_DON",
                table: "DPDA",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DL",
                table: "DPDA",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 1)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MA_KHACH_HANG",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 3)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MA_CHI_NHANH",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 2)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "LOAI_THE",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "LOAI_PHAT_HANH",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "GIAO_THE",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CREATED_DATE",
                table: "DPDA",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "DPDA",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("Relational:ColumnOrder", 0)
                .OldAnnotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SysStartTime",
                table: "DPDA",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SysEndTime",
                table: "DPDA",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AddColumn<string>(
                name: "FILE_NAME",
                table: "DPDA",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "ZIP_CODE",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "DP01",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "DP01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "UNTBUSCD",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "TYGIA",
                table: "DP01",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TIME_DP_TYPE",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TIME_DP_NAME",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TERM_DP_TYPE",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TERM_DP_NAME",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TEN_PGD",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TEN_NGUOI_GIOI_THIEU",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TEN_KH",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TEN_CAN_BO_PT",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TELEPHONE",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TAX_CODE_LOCATION",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "TAI_KHOAN_HACH_TOAN",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "STATES_CODE",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "SPECIAL_RATE",
                table: "DP01",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)",
                oldPrecision: 10,
                oldScale: 6,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "SO_TAI_KHOAN",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "SO_KY_AD_LSDB",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "SEX_TYPE",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RENEW_DATE",
                table: "DP01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "RATE",
                table: "DP01",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)",
                oldPrecision: 10,
                oldScale: 6,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "QUOC_TICH",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PREVIOUS_DP_CAP_DATE",
                table: "DP01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "PHONG_CAN_BO_PT",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OPENING_DATE",
                table: "DP01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "NOTENO",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "NGUOI_NUOC_NGOAI",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "NGUOI_GIOI_THIEU",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DL",
                table: "DP01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NEXT_DP_CAP_DATE",
                table: "DP01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MONTH_TERM",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MA_PGD",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MA_KH",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MA_CN",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MA_CAN_BO_PT",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MA_CAN_BO_AGRIBANK",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MATURITY_DATE",
                table: "DP01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "LOCAL_WARD_NAME",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "LOCAL_PROVIN_NAME",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "LOCAL_DISTRICT_NAME",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ImportDateTime",
                table: "DP01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ISSUE_DATE",
                table: "DP01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "ISSUED_BY",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "ID_NUMBER",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "EMPLOYEE_NUMBER",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "EMPLOYEE_NAME",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "DataSource",
                table: "DP01",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "DRAMT",
                table: "DP01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "DP_TYPE_NAME",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "DP_TYPE_CODE",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "DP01",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "DP01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "CUST_TYPE_NAME",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "CUST_TYPE_DETAIL",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "CUST_TYPE",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "CUST_DETAIL_NAME",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "CURRENT_BALANCE",
                table: "DP01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "CRAMT",
                table: "DP01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "COUNTRY_CODE",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "CONTRACT_COUTS_DAY",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CLOSE_DATE",
                table: "DP01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "CCY",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "BIRTH_DATE",
                table: "DP01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "AUTO_RENEWAL",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "ADDRESS",
                table: "DP01",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "ACRUAL_AMOUNT_END",
                table: "DP01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "ACRUAL_AMOUNT",
                table: "DP01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "ACCOUNT_STATUS",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "DP01",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SysStartTime",
                table: "DP01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SysEndTime",
                table: "DP01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AddColumn<string>(
                name: "FILE_NAME",
                table: "DP01",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            // Safe create for DPDA_History (may already exist if manually created earlier)
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DPDA_History]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[DPDA_History] (
        [Id] int NOT NULL IDENTITY(1,1),
        [BusinessKey] nvarchar(500) NOT NULL,
        [EffectiveDate] datetime2 NOT NULL,
        [ExpiryDate] datetime2 NULL,
        [IsCurrent] bit NOT NULL,
        [RowVersion] int NOT NULL,
        [ImportId] nvarchar(100) NOT NULL,
        [StatementDate] datetime2 NOT NULL,
        [ProcessedDate] datetime2 NOT NULL,
        [DataHash] nvarchar(64) NOT NULL,
        [MaKhachHang] nvarchar(50) NULL,
        [TenKhachHang] nvarchar(500) NULL,
        [SoThe] nvarchar(50) NULL,
        [LoaiThe] nvarchar(100) NULL,
        [TrangThaiThe] nvarchar(50) NULL,
        [NgayPhatHanh] datetime2 NULL,
        [NgayHetHan] datetime2 NULL,
        [HanMucThe] decimal(18,2) NULL,
        [SoDuHienTai] decimal(18,2) NULL,
        [SoTienDaSD] decimal(18,2) NULL,
        [MaChiNhanh] nvarchar(20) NULL,
        [TenChiNhanh] nvarchar(200) NULL,
        [NgayTao] datetime2 NULL,
        [NgayCapNhat] datetime2 NULL,
        [AdditionalData] nvarchar(max) NULL,
        CONSTRAINT [PK_DPDA_History] PRIMARY KEY ([Id])
    );
END");

            // Safe create for EI01_History
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EI01_History]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[EI01_History] (
        [Id] int NOT NULL IDENTITY(1,1),
        [BusinessKey] nvarchar(500) NOT NULL,
        [EffectiveDate] datetime2 NOT NULL,
        [ExpiryDate] datetime2 NULL,
        [IsCurrent] bit NOT NULL,
        [RowVersion] int NOT NULL,
        [ImportId] nvarchar(100) NOT NULL,
        [StatementDate] datetime2 NOT NULL,
        [ProcessedDate] datetime2 NOT NULL,
        [DataHash] nvarchar(64) NOT NULL,
        [MaKhachHang] nvarchar(50) NULL,
        [TenKhachHang] nvarchar(500) NULL,
        [SoTaiKhoan] nvarchar(50) NULL,
        [LoaiGiaoDich] nvarchar(100) NULL,
        [MaGiaoDich] nvarchar(100) NULL,
        [SoTien] decimal(18,2) NULL,
        [NgayGiaoDich] datetime2 NULL,
        [ThoiGianGiaoDich] datetime2 NULL,
        [TrangThaiGiaoDich] nvarchar(50) NULL,
        [NoiDungGiaoDich] nvarchar(1000) NULL,
        [MaChiNhanh] nvarchar(20) NULL,
        [TenChiNhanh] nvarchar(200) NULL,
        [Channel] nvarchar(50) NULL,
        [DeviceInfo] nvarchar(500) NULL,
        [AdditionalData] nvarchar(max) NULL,
        CONSTRAINT [PK_EI01_History] PRIMARY KEY ([Id])
    );
END");

            // Safe create for GAHR26_History
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GAHR26_History]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[GAHR26_History] (
        [Id] int NOT NULL IDENTITY(1,1),
        [BusinessKey] nvarchar(500) NOT NULL,
        [EffectiveDate] datetime2 NOT NULL,
        [ExpiryDate] datetime2 NULL,
        [IsCurrent] bit NOT NULL,
        [RowVersion] int NOT NULL,
        [ImportId] nvarchar(100) NOT NULL,
        [StatementDate] datetime2 NOT NULL,
        [ProcessedDate] datetime2 NOT NULL,
        [DataHash] nvarchar(64) NOT NULL,
        [EMP_ID] nvarchar(50) NULL,
        [EMP_NAME] nvarchar(500) NULL,
        [ID_NUMBER] nvarchar(20) NULL,
        [POSITION] nvarchar(100) NULL,
        [BRCD] nvarchar(20) NULL,
        [BRANCH_NAME] nvarchar(200) NULL,
        [DEPARTMENT] nvarchar(100) NULL,
        [JOIN_DATE] datetime2 NULL,
        [BIRTH_DATE] datetime2 NULL,
        [GENDER] nvarchar(10) NULL,
        [ADDRESS] nvarchar(500) NULL,
        [PHONE] nvarchar(20) NULL,
        [EMAIL] nvarchar(100) NULL,
        [STATUS] nvarchar(50) NULL,
        [BASIC_SALARY] decimal(18,2) NULL,
        [ALLOWANCE] decimal(18,2) NULL,
        [CREATED_DATE] datetime2 NULL,
        [UPDATED_DATE] datetime2 NULL,
        [AdditionalData] nvarchar(max) NULL,
        CONSTRAINT [PK_GAHR26_History] PRIMARY KEY ([Id])
    );
END");

            // Safe create for GL01_History
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GL01_History]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[GL01_History] (
        [HistoryID] bigint NOT NULL IDENTITY(1,1),
        [MANDT] nvarchar(3) NOT NULL,
        [BUKRS] nvarchar(4) NOT NULL,
        [GJAHR] nvarchar(4) NOT NULL,
        [BELNR] nvarchar(10) NOT NULL,
        [BUZEI] nvarchar(3) NOT NULL,
        [AUGDT] nvarchar(8) NOT NULL,
        [AUGCP] nvarchar(6) NOT NULL,
        [AUGBL] nvarchar(10) NOT NULL,
        [BSCHL] nvarchar(2) NOT NULL,
        [KOART] nvarchar(1) NOT NULL,
        [UMSKZ] nvarchar(1) NOT NULL,
        [UMSKS] nvarchar(1) NOT NULL,
        [ZUMSK] nvarchar(1) NOT NULL,
        [SHKZG] nvarchar(1) NOT NULL,
        [GSBER] nvarchar(4) NOT NULL,
        [PARGB] nvarchar(4) NOT NULL,
        [MWSKZ] nvarchar(2) NOT NULL,
        [QSSKZ] nvarchar(2) NOT NULL,
        [DMBTR] decimal(13,2) NULL,
        [WRBTR] decimal(13,2) NULL,
        [KZBTR] decimal(13,2) NULL,
        [PSWBT] decimal(13,2) NULL,
        [PSWSL] nvarchar(5) NOT NULL,
        [TXBHW] decimal(13,2) NULL,
        [TXBFW] decimal(13,2) NULL,
        [MWSTS] decimal(13,2) NULL,
        [MWSTV] decimal(13,2) NULL,
        [HWBAS] decimal(13,2) NULL,
        [FWBAS] decimal(13,2) NULL,
        [HWSTE] decimal(13,2) NULL,
        [FWSTE] decimal(13,2) NULL,
        [STBLG] nvarchar(10) NOT NULL,
        [STGRD] nvarchar(2) NOT NULL,
        [VALUT] nvarchar(8) NOT NULL,
        [ZUONR] nvarchar(18) NOT NULL,
        [SGTXT] nvarchar(50) NOT NULL,
        [ZINKZ] nvarchar(1) NOT NULL,
        [VBUND] nvarchar(6) NOT NULL,
        [BEWAR] nvarchar(3) NOT NULL,
        [ALTKT] nvarchar(10) NOT NULL,
        [VORGN] nvarchar(4) NOT NULL,
        [FDLEV] nvarchar(2) NOT NULL,
        [FDGRP] nvarchar(10) NOT NULL,
        [HKONT] nvarchar(10) NOT NULL,
        [KUNNR] nvarchar(10) NOT NULL,
        [LIFNR] nvarchar(10) NOT NULL,
        [FILKD] nvarchar(10) NOT NULL,
        [XBILK] nvarchar(1) NOT NULL,
        [GVTYP] nvarchar(2) NOT NULL,
        [HZUON] nvarchar(18) NOT NULL,
        [ZFBDT] nvarchar(8) NOT NULL,
        [ZTERM] nvarchar(4) NOT NULL,
        [ZBD1T] int NULL,
        [ZBD2T] int NULL,
        [ZBD3T] int NULL,
        [ZBD1P] decimal(5,3) NULL,
        [ZBD2P] decimal(5,3) NULL,
        [SKFBT] decimal(13,2) NULL,
        [SKNTO] decimal(13,2) NULL,
        [WSKTO] decimal(13,2) NULL,
        [ZLSCH] nvarchar(1) NOT NULL,
        [ZLSPR] nvarchar(1) NOT NULL,
        [ZBFIX] nvarchar(1) NOT NULL,
        [HBKID] nvarchar(5) NOT NULL,
        [BVTYP] nvarchar(4) NOT NULL,
        [NEBTR] decimal(13,2) NULL,
        [MWART] nvarchar(1) NOT NULL,
        [DMBE2] decimal(13,2) NULL,
        [DMBE3] decimal(13,2) NULL,
        [PPRCT] nvarchar(3) NOT NULL,
        [XREF1] nvarchar(12) NOT NULL,
        [XREF2] nvarchar(12) NOT NULL,
        [KOST1] nvarchar(12) NOT NULL,
        [KOST2] nvarchar(12) NOT NULL,
        [VBEL2] nvarchar(10) NOT NULL,
        [POSN2] nvarchar(6) NOT NULL,
        [KKBER] nvarchar(4) NOT NULL,
        [EMPFB] nvarchar(12) NOT NULL,
        [SourceID] nvarchar(50) NOT NULL,
        [ValidFrom] datetime2 NOT NULL,
        [ValidTo] datetime2 NOT NULL,
        [IsCurrent] bit NOT NULL,
        [VersionNumber] int NOT NULL,
        [RecordHash] nvarchar(max) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [ModifiedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_GL01_History] PRIMARY KEY ([HistoryID])
    );
END");

            // Removed prior unconditional drop/create placeholder for GL41_History to preserve any existing column ordering
            // so the late dynamic rebuild can accurately detect and correct ordering without being reset.

            // Final comprehensive alignment for GL41_History BEFORE enabling SYSTEM_VERSIONING for GL41.
            // (Earlier small guard blocks may have executed before this placeholder existed; this block is the authoritative sync.)
            // It is safe (idempotent) on re-runs: only adds columns that are missing.
            migrationBuilder.Sql(@"IF OBJECT_ID(N'[dbo].[GL41_History]','U') IS NOT NULL
BEGIN
    -- Ensure business columns (excluding the three already added earlier via EF generated steps) exist
    IF COL_LENGTH('GL41_History','MA_TK') IS NULL ALTER TABLE [GL41_History] ADD [MA_TK] nvarchar(200) NULL;
    IF COL_LENGTH('GL41_History','TEN_TK') IS NULL ALTER TABLE [GL41_History] ADD [TEN_TK] nvarchar(200) NULL;
    IF COL_LENGTH('GL41_History','MA_CN') IS NULL ALTER TABLE [GL41_History] ADD [MA_CN] nvarchar(200) NULL;
    IF COL_LENGTH('GL41_History','LOAI_TIEN') IS NULL ALTER TABLE [GL41_History] ADD [LOAI_TIEN] nvarchar(200) NULL;
    IF COL_LENGTH('GL41_History','LOAI_BT') IS NULL ALTER TABLE [GL41_History] ADD [LOAI_BT] nvarchar(200) NULL;
    IF COL_LENGTH('GL41_History','ST_GHINO') IS NULL ALTER TABLE [GL41_History] ADD [ST_GHINO] decimal(18,2) NULL;
    IF COL_LENGTH('GL41_History','ST_GHICO') IS NULL ALTER TABLE [GL41_History] ADD [ST_GHICO] decimal(18,2) NULL;
    IF COL_LENGTH('GL41_History','SBT_NO') IS NULL ALTER TABLE [GL41_History] ADD [SBT_NO] decimal(18,2) NULL;
    IF COL_LENGTH('GL41_History','SBT_CO') IS NULL ALTER TABLE [GL41_History] ADD [SBT_CO] decimal(18,2) NULL;
    IF COL_LENGTH('GL41_History','NGAY_DL') IS NULL ALTER TABLE [GL41_History] ADD [NGAY_DL] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00';
    IF COL_LENGTH('GL41_History','FILE_NAME') IS NULL ALTER TABLE [GL41_History] ADD [FILE_NAME] nvarchar(500) NULL;
    IF COL_LENGTH('GL41_History','DN_DAUKY') IS NULL ALTER TABLE [GL41_History] ADD [DN_DAUKY] decimal(18,2) NULL;
    IF COL_LENGTH('GL41_History','DN_CUOIKY') IS NULL ALTER TABLE [GL41_History] ADD [DN_CUOIKY] decimal(18,2) NULL;
    IF COL_LENGTH('GL41_History','DC_DAUKY') IS NULL ALTER TABLE [GL41_History] ADD [DC_DAUKY] decimal(18,2) NULL;
    IF COL_LENGTH('GL41_History','DC_CUOIKY') IS NULL ALTER TABLE [GL41_History] ADD [DC_CUOIKY] decimal(18,2) NULL;
    -- Add the EF new columns defensively if EF generated AddColumn earlier was skipped (re-run scenario)
    IF COL_LENGTH('GL41_History','BATCH_ID') IS NULL ALTER TABLE [GL41_History] ADD [BATCH_ID] nvarchar(100) NULL;
    IF COL_LENGTH('GL41_History','CREATED_DATE') IS NULL ALTER TABLE [GL41_History] ADD [CREATED_DATE] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00';
    IF COL_LENGTH('GL41_History','IMPORT_SESSION_ID') IS NULL ALTER TABLE [GL41_History] ADD [IMPORT_SESSION_ID] nvarchar(100) NULL;
    -- Period columns (not hidden in history)
    IF COL_LENGTH('GL41_History','ValidFrom') IS NULL ALTER TABLE [GL41_History] ADD [ValidFrom] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00';
    IF COL_LENGTH('GL41_History','ValidTo') IS NULL ALTER TABLE [GL41_History] ADD [ValidTo] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
END");

            // Safe create for LN01_CsvHistory
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LN01_CsvHistory]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[LN01_CsvHistory] (
        [Id] int NOT NULL IDENTITY(1,1),
        [BusinessKey] nvarchar(500) NOT NULL,
        [EffectiveDate] datetime2 NOT NULL,
        [ExpiryDate] datetime2 NULL,
        [IsCurrent] bit NOT NULL,
        [RowVersion] int NOT NULL,
        [ImportId] nvarchar(100) NOT NULL,
        [StatementDate] datetime2 NOT NULL,
        [ProcessedDate] datetime2 NOT NULL,
        [DataHash] nvarchar(64) NOT NULL,
        [BRCD] nvarchar(20) NULL,
        [CUSTSEQ] nvarchar(50) NULL,
        [CUSTNM] nvarchar(200) NULL,
        [TAI_KHOAN] nvarchar(50) NULL,
        [CCY] nvarchar(3) NULL,
        [DU_NO] decimal(18,2) NULL,
        [DSBSSEQ] nvarchar(50) NULL,
        [TRANSACTION_DATE] datetime2 NULL,
        [DSBSDT] datetime2 NULL,
        [DISBUR_CCY] nvarchar(3) NULL,
        [DISBURSEMENT_AMOUNT] decimal(18,2) NULL,
        [RawDataJson] nvarchar(max) NULL,
        [AdditionalData] nvarchar(max) NULL,
        CONSTRAINT [PK_LN01_CsvHistory] PRIMARY KEY ([Id])
    );
END");

            // Safe create for LN01_History
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LN01_History]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[LN01_History] (
        [HistoryID] bigint NOT NULL IDENTITY(1,1),
        [MANDT] nvarchar(3) NOT NULL,
        [BUKRS] nvarchar(4) NOT NULL,
        [LAND1] nvarchar(3) NOT NULL,
        [WAERS] nvarchar(5) NOT NULL,
        [SPRAS] nvarchar(1) NOT NULL,
        [KTOPL] nvarchar(4) NOT NULL,
        [WAABW] nvarchar(2) NOT NULL,
        [PERIV] nvarchar(2) NOT NULL,
        [KOKFI] nvarchar(1) NOT NULL,
        [RCOMP] nvarchar(6) NOT NULL,
        [ADRNR] nvarchar(10) NOT NULL,
        [STCEG] nvarchar(20) NOT NULL,
        [FIKRS] nvarchar(4) NOT NULL,
        [XFMCO] nvarchar(1) NOT NULL,
        [XFMCB] nvarchar(1) NOT NULL,
        [XFMCA] nvarchar(1) NOT NULL,
        [TXJCD] nvarchar(15) NOT NULL,
        [SourceID] nvarchar(50) NOT NULL,
        [ValidFrom] datetime2 NOT NULL,
        [ValidTo] datetime2 NOT NULL,
        [IsCurrent] bit NOT NULL,
        [VersionNumber] int NOT NULL,
        [RecordHash] nvarchar(max) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [ModifiedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_LN01_History] PRIMARY KEY ([HistoryID])
    );
END");

            // Removed unguarded CreateTable for LN03_History. A guarded IF NOT EXISTS raw SQL block is used below instead to avoid errors if the table already exists.

            // Guarded index creations (skip if already exists)
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'NCCI_GL41_Analytics' AND object_id = OBJECT_ID('GL41'))
CREATE INDEX [NCCI_GL41_Analytics] ON [GL41] ([NGAY_DL], [MA_CN], [MA_TK]);");

            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EI01_MA_KH' AND object_id = OBJECT_ID('EI01'))
CREATE INDEX [IX_EI01_MA_KH] ON [EI01] ([MA_KH]);");

            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EI01_TEN_KH' AND object_id = OBJECT_ID('EI01'))
CREATE INDEX [IX_EI01_TEN_KH] ON [EI01] ([TEN_KH]);");

            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DP01_MA_KH' AND object_id = OBJECT_ID('DP01'))
CREATE INDEX [IX_DP01_MA_KH] ON [DP01] ([MA_KH]);");

            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DP01_SO_TAI_KHOAN' AND object_id = OBJECT_ID('DP01'))
CREATE INDEX [IX_DP01_SO_TAI_KHOAN] ON [DP01] ([SO_TAI_KHOAN]);");

            // Safe create for LN03_History
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LN03_History]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[LN03_History] (
        [Id] int NOT NULL IDENTITY(1,1),
        [BusinessKey] nvarchar(500) NOT NULL,
        [EffectiveDate] datetime2 NOT NULL,
        [ExpiryDate] datetime2 NULL,
        [IsCurrent] bit NOT NULL,
        [RowVersion] int NOT NULL,
        [ImportId] nvarchar(100) NOT NULL,
        [StatementDate] datetime2 NOT NULL,
        [ProcessedDate] datetime2 NOT NULL,
        [DataHash] nvarchar(64) NOT NULL,
        [MaKhachHang] nvarchar(50) NULL,
        [TenKhachHang] nvarchar(500) NULL,
        [MaHopDong] nvarchar(50) NULL,
        [LoaiHopDong] nvarchar(100) NULL,
        [SoTienGoc] decimal(18,2) NULL,
        [SoTienLai] decimal(18,2) NULL,
        [TongNo] decimal(18,2) NULL,
        [NgayDaoHan] datetime2 NULL,
        [TinhTrangNo] nvarchar(100) NULL,
        [NhomNo] int NULL,
        [MaChiNhanh] nvarchar(20) NULL,
        [TenChiNhanh] nvarchar(200) NULL,
        [AdditionalData] nvarchar(max) NULL,
        CONSTRAINT [PK_LN03_History] PRIMARY KEY ([Id])
    );
END");

            // Skipped dropping LN01_History and LN03_History to avoid errors on existing system-versioned / history tables.

            migrationBuilder.DropIndex(
                name: "NCCI_GL41_Analytics",
                table: "GL41");

            migrationBuilder.DropIndex(
                name: "IX_EI01_MA_KH",
                table: "EI01");

            migrationBuilder.DropIndex(
                name: "IX_EI01_TEN_KH",
                table: "EI01");

            migrationBuilder.DropIndex(
                name: "IX_DP01_MA_KH",
                table: "DP01");

            migrationBuilder.DropIndex(
                name: "IX_DP01_SO_TAI_KHOAN",
                table: "DP01");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EI01")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EI01")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.DropColumn(
                name: "FILE_NAME",
                table: "DPDA")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.DropColumn(
                name: "FILE_NAME",
                table: "DP01")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            // Skipped unintended reverse temporal renames for RR01 (SysStartTime->ValidTo, SysEndTime->ValidFrom).
            // These caused sp_rename attempts that failed because ValidFrom/ValidTo already exist (guarded earlier with forward renames).
            // Retaining SysStartTime/SysEndTime as canonical temporal period columns.
            migrationBuilder.Sql("-- skipped reverse RR01 temporal renames (SysStartTime->ValidTo, SysEndTime->ValidFrom) as table already standardized.");

            migrationBuilder.RenameColumn(
                name: "SysStartTime",
                table: "LN03",
                newName: "ValidTo")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.RenameColumn(
                name: "SysEndTime",
                table: "LN03",
                newName: "ValidFrom")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.RenameColumn(
                name: "SysStartTime",
                table: "LN01",
                newName: "ValidTo")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.RenameColumn(
                name: "SysEndTime",
                table: "LN01",
                newName: "ValidFrom")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "GL41",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "GL41",
                newName: "ValidTo");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "GL41",
                newName: "ValidFrom");

            migrationBuilder.RenameColumn(
                name: "KpiIndicatorId",
                table: "EmployeeKpiAssignments",
                newName: "KpiDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeKpiAssignments_KpiIndicatorId",
                table: "EmployeeKpiAssignments",
                newName: "IX_EmployeeKpiAssignments_KpiDefinitionId");

            migrationBuilder.RenameColumn(
                name: "ValidTo1",
                table: "EI01",
                newName: "UPDATED_DATE")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.RenameColumn(
                name: "ValidFrom1",
                table: "EI01",
                newName: "CREATED_DATE")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.RenameIndex(
                name: "NCCI_EI01_Analytics",
                table: "EI01",
                newName: "IX_EI01_NGAY_DL_MaCN");

            migrationBuilder.RenameIndex(
                name: "IX_EI01_MA_CN",
                table: "EI01",
                newName: "IX_EI01_MaCN");

            migrationBuilder.RenameColumn(
                name: "SysStartTime",
                table: "DPDA",
                newName: "ValidTo")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.RenameColumn(
                name: "SysEndTime",
                table: "DPDA",
                newName: "ValidFrom")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.RenameColumn(
                name: "SysStartTime",
                table: "DP01",
                newName: "ValidTo")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.RenameColumn(
                name: "SysEndTime",
                table: "DP01",
                newName: "ValidFrom")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.RenameIndex(
                name: "IX_DP01_MA_CN",
                table: "DP01",
                newName: "IX_DP01_MaCN");

            // Adjusted RR01 temporal annotations so new == old (SysStartTime/SysEndTime) to suppress unintended reverse sp_rename attempts.
            migrationBuilder.AlterTable(
                name: "RR01")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "LN03")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "LN01")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "GL41")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterTable(
                name: "EI01")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterTable(
                name: "DPDA")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterTable(
                name: "DP01")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            // Down migration: Skipped reverting VAMC_FLG since Up made no structural change.

            migrationBuilder.AlterColumn<DateTime>(
                name: "UPDATED_DATE",
                table: "RR01",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2(3)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 28)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "TSK",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 25)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 26)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "THU_LAI",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 22)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 23)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "THU_GOC",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 21)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 22)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TEN_KH",
                table: "RR01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 4)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 5)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            // Down migration: SO_LDS alteration skipped; Up made no change.

            migrationBuilder.AlterColumn<string>(
                name: "SO_LAV",
                table: "RR01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 7)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 8)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_XLRR",
                table: "RR01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 12)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 13)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_GIAI_NGAN",
                table: "RR01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 9)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 10)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DL",
                table: "RR01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("Relational:ColumnOrder", 0)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 1)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DEN_HAN",
                table: "RR01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 10)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 11)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "MA_KH",
                table: "RR01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 3)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 4)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            // Down migration: LOAI_KH alteration skipped; Up made no change for same reason (index dependency on NCCI_RR01_Analytics).

            migrationBuilder.AlterColumn<string>(
                name: "FILE_NAME",
                table: "RR01",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 29)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "DUNO_TRUNG_HAN",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 19)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 20)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "DUNO_NGAN_HAN",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 18)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 19)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "DUNO_LAI_TICHLUY_BD",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 14)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 15)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "DUNO_LAI_HIENTAI",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 17)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 18)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "DUNO_GOC_HIENTAI",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 16)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 17)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "DUNO_GOC_BAN_DAU",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 13)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 14)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "DUNO_DAI_HAN",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 20)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 21)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "DS",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 24)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 25)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "DOC_DAUKY_DA_THU_HT",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 15)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 16)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CREATED_DATE",
                table: "RR01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(3)")
                .Annotation("Relational:ColumnOrder", 27)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            // Down migration: CN_LOAI_I alteration skipped; Up made no change (index dependency NCCI_RR01_Analytics).

            // Down migration: CCY revert skipped (Up skipped structural change due to NCCI_RR01_Analytics).

            // Down migration: BRCD revert skipped (Up skipped due to NCCI_RR01_Analytics dependency).

            migrationBuilder.AlterColumn<decimal>(
                name: "BDS",
                table: "RR01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 23)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 24)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "RR01",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 26)
                .Annotation("SqlServer:Identity", "1, 1")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidTo",
                table: "RR01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidFrom",
                table: "RR01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AddColumn<string>(
                name: "DATA_SOURCE",
                table: "RR01",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true)
                .Annotation("Relational:ColumnOrder", 31)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AddColumn<string>(
                name: "ERROR_MESSAGE",
                table: "RR01",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true)
                .Annotation("Relational:ColumnOrder", 33)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AddColumn<string>(
                name: "IMPORT_BATCH_ID",
                table: "RR01",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true)
                .Annotation("Relational:ColumnOrder", 30)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AddColumn<string>(
                name: "PROCESSING_STATUS",
                table: "RR01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("Relational:ColumnOrder", 32)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AddColumn<string>(
                name: "ROW_HASH",
                table: "RR01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("Relational:ColumnOrder", 34)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "RR01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "THUNOSAUXL",
                table: "LN03",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TENKH",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TENCHINHANH",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TENCBTD",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 15)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TAIKHOANHACHTOAN",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 17)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "SOTIENXLRR",
                table: "LN03",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "SOTIEN",
                table: "LN03",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 22)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "SOHOPDONG",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "REFNO",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 18)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "NHOMNO",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 13)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DL",
                table: "LN03",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAYPHATSINHXL",
                table: "LN03",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "MAPGD",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 16)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "MALOAI",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 20)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "MAKH",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "MACHINHANH",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "MACBTD",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 14)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "LOAINGUONVON",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 19)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "LOAIKHACHHANG",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 21)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "FILE_ORIGIN",
                table: "LN03",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 25)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "DUNONOIBANG",
                table: "LN03",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 12)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CREATED_DATE",
                table: "LN03",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 24)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "CREATED_BY",
                table: "LN03",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 23)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "CONLAINGOAIBANG",
                table: "LN03",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "LN03",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 11)
                .OldAnnotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidTo",
                table: "LN03",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidFrom",
                table: "LN03",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN03_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "YRDAYS",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 60)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "USRIDOP",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 54)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UPDATED_DATE",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 84)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "TY_GIA",
                table: "LN01",
                type: "decimal(10,6)",
                precision: 10,
                scale: 6,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 10,
                oldScale: 6,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 79)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TRCTNM",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 36)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TRCTCD",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 35)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TRANSACTION_DATE",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 9)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "TOTAL_INTEREST_REPAY_AMOUNT",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 32)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TAI_KHOAN_GIAI_NGAN_2",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 73)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TAI_KHOAN_GIAI_NGAN_1",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 70)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TAI_KHOAN",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 5)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "SO_TIEN_GIAI_NGAN_2",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 74)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "SO_TIEN_GIAI_NGAN_1",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 71)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "SECURED_PERCENT",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 45)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "REPAYMENT_AMOUNT",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 24)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "REMARK",
                table: "LN01",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 61)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "PROVINCE",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 38)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "PHUONG_THUC_GIAI_NGAN_2",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 72)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "PHUONG_THUC_GIAI_NGAN_1",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 69)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "PASTDUE_INTEREST_AMOUNT",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 31)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "OFFICER_NAME",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 29)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "OFFICER_IPCAS",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 80)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "OFFICER_ID",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 28)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "NHOM_NO",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 46)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_SINH",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 76)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DL",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 1)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NEXT_REPAY_DATE",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 25)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "NEXT_REPAY_AMOUNT",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 26)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NEXT_INT_REPAY_DATE",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 27)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "MA_NGANH_KT",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 78)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "MA_CB_AGRI",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 77)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "LOAN_TYPE",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 21)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "LCLWARDNM",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 43)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "LCLPROVINNM",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 39)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "LCLDISTNM",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 41)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LAST_REPAY_DATE",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 44)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LAST_INT_CHARGE_DATE",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 47)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "INT_PAYMENT_INTERVAL",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 67)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "INT_PARTIAL_PAYMENT_TYPE",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 66)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "INT_LUMPSUM_PARTIAL_TYPE",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 65)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "INTTRMMTH",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 59)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "INTRPYMTH",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 58)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "INTEREST_RATE",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 15)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "INTEREST_AMOUNT",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 30)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "INTCMTH",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 57)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "GRPNO",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 51)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "FUND_RESOURCE_CODE",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 22)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "FUND_PURPOSE_CODE",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 23)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "FILE_NAME",
                table: "LN01",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 82)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "EXEMPTINTTYPE",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 49)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "EXEMPTINTAMT",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 50)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "EXEMPTINT",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 48)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "DU_NO",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 7)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "DSBSSEQ",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 8)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DSBSMATDT",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 13)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DSBSDT",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 10)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "DISTRICT",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 40)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "DISBUR_CCY",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 11)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "DISBURSEMENT_AMOUNT",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 12)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "CUSTSEQ",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 3)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "CUSTOMER_TYPE_CODE_DETAIL",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 34)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "CUSTOMER_TYPE_CODE",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 33)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "CUSTNM",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 4)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "CTCV",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 63)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "CREDIT_LINE_YPE",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 64)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CREATED_DATE",
                table: "LN01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 83)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "COMMCD",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 42)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "CMT_HC",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 75)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "CHITIEU",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 62)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "CCY",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 6)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "BUSCD",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 52)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "BSRTCD",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 14)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "BSNSSCLTPCD",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 53)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "BRCD",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 2)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "APPR_CCY",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 18)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "APPRSEQ",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 16)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "APPRMATDT",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 20)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "APPRDT",
                table: "LN01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 17)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "APPRAMT",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 19)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "AN_HAN_LAI",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 68)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "ADDR1",
                table: "LN01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 37)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "ACCRUAL_AMOUNT_END_OF_MONTH",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 56)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "ACCRUAL_AMOUNT",
                table: "LN01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 55)
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "LN01",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("Relational:ColumnOrder", 81)
                .OldAnnotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidTo",
                table: "LN01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidFrom",
                table: "LN01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "LN01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TEN_TK",
                table: "GL41",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 4)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "ST_GHINO",
                table: "GL41",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 9)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "ST_GHICO",
                table: "GL41",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 11)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "SBT_NO",
                table: "GL41",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 8)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "SBT_CO",
                table: "GL41",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 10)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DL",
                table: "GL41",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("Relational:ColumnOrder", 0)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MA_TK",
                table: "GL41",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 3)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "MA_CN",
                table: "GL41",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 1)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "LOAI_TIEN",
                table: "GL41",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 2)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "LOAI_BT",
                table: "GL41",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 5)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "FILE_NAME",
                table: "GL41",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 14)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "DN_DAUKY",
                table: "GL41",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 6)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "DN_CUOIKY",
                table: "GL41",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 12)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "DC_DAUKY",
                table: "GL41",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 7)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<decimal>(
                name: "DC_CUOIKY",
                table: "GL41",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 13)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<long>(
                name: "ID",
                table: "GL41",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Relational:ColumnOrder", 18)
                .Annotation("SqlServer:Identity", "1, 1")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidTo",
                table: "GL41",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidFrom",
                table: "GL41",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GL41_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            // Removed EF AddColumn for GL41 (BATCH_ID, CREATED_DATE, IMPORT_SESSION_ID) - replaced by early guarded raw SQL above to avoid
            // duplicate history table additions when table is temporal. Columns now added only if missing before re-enable.

            migrationBuilder.AddColumn<string>(
                name: "UserIPCAS",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidTo",
                table: "EI01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidFrom",
                table: "EI01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<string>(
                name: "USER_SMS",
                table: "EI01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 22)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<string>(
                name: "USER_SAV",
                table: "EI01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 23)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<string>(
                name: "USER_OTT",
                table: "EI01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 21)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<string>(
                name: "USER_LN",
                table: "EI01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 24)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<string>(
                name: "USER_EMB",
                table: "EI01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 20)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<string>(
                name: "TRANG_THAI_SMS",
                table: "EI01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 12)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<string>(
                name: "TRANG_THAI_SAV",
                table: "EI01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 15)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<string>(
                name: "TRANG_THAI_OTT",
                table: "EI01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 9)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<string>(
                name: "TRANG_THAI_LN",
                table: "EI01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 18)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<string>(
                name: "TRANG_THAI_EMB",
                table: "EI01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 6)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<string>(
                name: "TEN_KH",
                table: "EI01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 3)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<string>(
                name: "SDT_SMS",
                table: "EI01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 11)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<string>(
                name: "SDT_SAV",
                table: "EI01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 14)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<string>(
                name: "SDT_OTT",
                table: "EI01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 8)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<string>(
                name: "SDT_LN",
                table: "EI01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 17)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<string>(
                name: "SDT_EMB",
                table: "EI01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 5)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DL",
                table: "EI01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("Relational:ColumnOrder", 0)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DK_SMS",
                table: "EI01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 13)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DK_SAV",
                table: "EI01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 16)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DK_OTT",
                table: "EI01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 10)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DK_LN",
                table: "EI01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 19)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DK_EMB",
                table: "EI01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 7)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<string>(
                name: "MA_KH",
                table: "EI01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 2)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<string>(
                name: "MA_CN",
                table: "EI01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 1)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<string>(
                name: "LOAI_KH",
                table: "EI01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 4)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "EI01",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Relational:ColumnOrder", 25)
                .Annotation("SqlServer:Identity", "1, 1")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UPDATED_DATE",
                table: "EI01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("Relational:ColumnOrder", 27)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CREATED_DATE",
                table: "EI01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("Relational:ColumnOrder", 26)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo1")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom1");

            migrationBuilder.AddColumn<string>(
                name: "FILE_NAME",
                table: "EI01",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "")
                .Annotation("Relational:ColumnOrder", 28)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "EI01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom");

            migrationBuilder.AlterColumn<string>(
                name: "USER_PHAT_HANH",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UPDATED_DATE",
                table: "DPDA",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TRANG_THAI",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TEN_KHACH_HANG",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "SO_THE",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "SO_TAI_KHOAN",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "PHAN_LOAI",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_PHAT_HANH",
                table: "DPDA",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_NOP_DON",
                table: "DPDA",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DL",
                table: "DPDA",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("Relational:ColumnOrder", 1)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "MA_KHACH_HANG",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("Relational:ColumnOrder", 3)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "MA_CHI_NHANH",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("Relational:ColumnOrder", 2)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "LOAI_THE",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "LOAI_PHAT_HANH",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "GIAO_THE",
                table: "DPDA",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CREATED_DATE",
                table: "DPDA",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "DPDA",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0)
                .Annotation("SqlServer:Identity", "1, 1")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidTo",
                table: "DPDA",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidFrom",
                table: "DPDA",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DPDA_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "ZIP_CODE",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "DP01",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "DP01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "UNTBUSCD",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "TYGIA",
                table: "DP01",
                type: "decimal(18,6)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)",
                oldPrecision: 18,
                oldScale: 6,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TIME_DP_TYPE",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TIME_DP_NAME",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TERM_DP_TYPE",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TERM_DP_NAME",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TEN_PGD",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TEN_NGUOI_GIOI_THIEU",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TEN_KH",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TEN_CAN_BO_PT",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TELEPHONE",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TAX_CODE_LOCATION",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "TAI_KHOAN_HACH_TOAN",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "STATES_CODE",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "SPECIAL_RATE",
                table: "DP01",
                type: "decimal(18,6)",
                precision: 10,
                scale: 6,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)",
                oldPrecision: 18,
                oldScale: 6,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "SO_TAI_KHOAN",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "SO_KY_AD_LSDB",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "SEX_TYPE",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RENEW_DATE",
                table: "DP01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "RATE",
                table: "DP01",
                type: "decimal(18,6)",
                precision: 10,
                scale: 6,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)",
                oldPrecision: 18,
                oldScale: 6,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "QUOC_TICH",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PREVIOUS_DP_CAP_DATE",
                table: "DP01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "PHONG_CAN_BO_PT",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OPENING_DATE",
                table: "DP01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "NOTENO",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "NGUOI_NUOC_NGOAI",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "NGUOI_GIOI_THIEU",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NGAY_DL",
                table: "DP01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NEXT_DP_CAP_DATE",
                table: "DP01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "MONTH_TERM",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "MA_PGD",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "MA_KH",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "MA_CN",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "MA_CAN_BO_PT",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "MA_CAN_BO_AGRIBANK",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MATURITY_DATE",
                table: "DP01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "LOCAL_WARD_NAME",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "LOCAL_PROVIN_NAME",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "LOCAL_DISTRICT_NAME",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ImportDateTime",
                table: "DP01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ISSUE_DATE",
                table: "DP01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "ISSUED_BY",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "ID_NUMBER",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "EMPLOYEE_NUMBER",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "EMPLOYEE_NAME",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "DataSource",
                table: "DP01",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "DRAMT",
                table: "DP01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "DP_TYPE_NAME",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "DP_TYPE_CODE",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "DP01",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "DP01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "CUST_TYPE_NAME",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "CUST_TYPE_DETAIL",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "CUST_TYPE",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "CUST_DETAIL_NAME",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "CURRENT_BALANCE",
                table: "DP01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "CRAMT",
                table: "DP01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "COUNTRY_CODE",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "CONTRACT_COUTS_DAY",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CLOSE_DATE",
                table: "DP01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "CCY",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "BIRTH_DATE",
                table: "DP01",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "AUTO_RENEWAL",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "ADDRESS",
                table: "DP01",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "ACRUAL_AMOUNT_END",
                table: "DP01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "ACRUAL_AMOUNT",
                table: "DP01",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "ACCOUNT_STATUS",
                table: "DP01",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "DP01",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidTo",
                table: "DP01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidFrom",
                table: "DP01",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "ValidTo")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "ValidFrom")
                .OldAnnotation("SqlServer:IsTemporal", true)
                .OldAnnotation("SqlServer:TemporalHistoryTableName", "DP01_History")
                .OldAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                .OldAnnotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .OldAnnotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.CreateIndex(
                name: "IX_GL41_MaCN",
                table: "GL41",
                column: "MA_CN");

            migrationBuilder.CreateIndex(
                name: "IX_GL41_NGAY_DL",
                table: "GL41",
                column: "NGAY_DL");

            migrationBuilder.CreateIndex(
                name: "IX_GL41_NGAY_DL_MaCN",
                table: "GL41",
                columns: new[] { "NGAY_DL", "MA_CN" });

            migrationBuilder.CreateIndex(
                name: "IX_DP01_MaPGD",
                table: "DP01",
                column: "MA_PGD");

            migrationBuilder.CreateIndex(
                name: "IX_DP01_NGAY_DL_MaCN",
                table: "DP01",
                columns: new[] { "NGAY_DL", "MA_CN" });

            migrationBuilder.CreateIndex(
                name: "NCCI_DP01_Analytics",
                table: "DP01",
                columns: new[] { "NGAY_DL", "MA_CN", "CURRENT_BALANCE", "MA_PGD" });

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeKpiAssignments_KPIDefinitions_KpiDefinitionId",
                table: "EmployeeKpiAssignments",
                column: "KpiDefinitionId",
                principalTable: "KPIDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
