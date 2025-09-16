using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khoan.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAdAndMaCbtdToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotent FK drop: only drop legacy FK to KpiIndicators if it still exists
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name='FK_EmployeeKpiAssignments_KpiIndicators_KpiIndicatorId')
ALTER TABLE [EmployeeKpiAssignments] DROP CONSTRAINT [FK_EmployeeKpiAssignments_KpiIndicators_KpiIndicatorId];");

            // Conditional column rename (legacy -> new) if not already applied
            migrationBuilder.Sql(@"IF COL_LENGTH('EmployeeKpiAssignments','KpiIndicatorId') IS NOT NULL AND COL_LENGTH('EmployeeKpiAssignments','KpiDefinitionId') IS NULL
EXEC sp_rename 'EmployeeKpiAssignments.KpiIndicatorId','KpiDefinitionId','COLUMN';");

            // Conditional index rename if old index still present
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_EmployeeKpiAssignments_KpiIndicatorId' AND object_id=OBJECT_ID('EmployeeKpiAssignments'))
EXEC sp_rename N'EmployeeKpiAssignments.IX_EmployeeKpiAssignments_KpiIndicatorId', N'IX_EmployeeKpiAssignments_KpiDefinitionId', N'INDEX';");

            // Add new employee columns if missing (or adjust length to desired final definition)
            migrationBuilder.Sql(@"IF COL_LENGTH('Employees','MaCBTD') IS NULL ALTER TABLE [Employees] ADD [MaCBTD] nvarchar(50) NULL;");
            migrationBuilder.Sql(@"IF COL_LENGTH('Employees','UserAD') IS NULL ALTER TABLE [Employees] ADD [UserAD] nvarchar(100) NULL;");

            // Ensure UserIPCAS exists and has nvarchar(100)
            migrationBuilder.Sql(@"IF COL_LENGTH('Employees','UserIPCAS') IS NULL
    ALTER TABLE [Employees] ADD [UserIPCAS] nvarchar(100) NULL;
ELSE BEGIN
    DECLARE @maxLen smallint = (SELECT max_length FROM sys.columns WHERE object_id = OBJECT_ID('Employees') AND name='UserIPCAS');
    -- nvarchar length = max_length/2; upgrade if current length < 100
    IF (@maxLen IS NOT NULL AND @maxLen < 200) ALTER TABLE [Employees] ALTER COLUMN [UserIPCAS] nvarchar(100) NULL;
END");

            // Create audit table only if it does not already exist
            migrationBuilder.Sql(@"IF OBJECT_ID(N'dbo.EmployeeAuditLogs','U') IS NULL
BEGIN
    CREATE TABLE [dbo].[EmployeeAuditLogs] (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_EmployeeAuditLogs] PRIMARY KEY,
        [EmployeeId] int NOT NULL,
        [Action] nvarchar(100) NOT NULL,
        [PerformedBy] nvarchar(100) NOT NULL,
        [FieldChanged] nvarchar(255) NULL,
        [OldValue] nvarchar(max) NULL,
        [NewValue] nvarchar(max) NULL,
        [PerformedAt] datetime2 NOT NULL
    );
END");

            // Ensure final FK to KPIDefinitions exists
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name='FK_EmployeeKpiAssignments_KPIDefinitions_KpiDefinitionId')
ALTER TABLE [EmployeeKpiAssignments] WITH CHECK ADD CONSTRAINT [FK_EmployeeKpiAssignments_KPIDefinitions_KpiDefinitionId] FOREIGN KEY([KpiDefinitionId]) REFERENCES [KPIDefinitions]([Id]) ON DELETE CASCADE;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeKpiAssignments_KPIDefinitions_KpiDefinitionId",
                table: "EmployeeKpiAssignments");

            migrationBuilder.DropTable(
                name: "EmployeeAuditLogs");

            migrationBuilder.DropColumn(
                name: "MaCBTD",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "UserAD",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "UserIPCAS",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "KpiDefinitionId",
                table: "EmployeeKpiAssignments",
                newName: "KpiIndicatorId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeKpiAssignments_KpiDefinitionId",
                table: "EmployeeKpiAssignments",
                newName: "IX_EmployeeKpiAssignments_KpiIndicatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeKpiAssignments_KpiIndicators_KpiIndicatorId",
                table: "EmployeeKpiAssignments",
                column: "KpiIndicatorId",
                principalTable: "KpiIndicators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
