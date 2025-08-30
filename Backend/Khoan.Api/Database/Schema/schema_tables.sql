CreateStatement                                                                                                                                                                                                                                                 
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE TABLE [dbo].[__EFMigrationsHistory] (
    [MigrationId] nvarchar(150) NOT NULL,&#x0D;
    [ProductVersion] nvarchar(32) NOT NULL,&#x0D;
);

                                                                                                          
CREATE TABLE [dbo].[EmployeeKhoanAssignmentDetails] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [EmployeeKhoanAssignmentId] int(10,0) NOT NULL,&#x0D;
    [TargetValue] decimal(18,2) NOT NULL,&#x0D;
    [ActualValue] decimal(18,2) NULL,&#x0D;
    [Score] deci
CREATE TABLE [dbo].[EmployeeKhoanAssignments] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [EmployeeId] int(10,0) NOT NULL,&#x0D;
    [KhoanPeriodId] int(10,0) NOT NULL,&#x0D;
    [AssignedDate] datetime2 NOT NULL,&#x0D;
    [Note] nvarchar(-1) NULL,&#x0D;
);
CREATE TABLE [dbo].[EmployeeKpiAssignments] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [EmployeeId] int(10,0) NOT NULL,&#x0D;
    [KpiDefinitionId] int(10,0) NOT NULL,&#x0D;
    [KhoanPeriodId] int(10,0) NOT NULL,&#x0D;
    [TargetValue] decimal(18,2) NOT N
CREATE TABLE [dbo].[EmployeeKpiTargets] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [EmployeeId] int(10,0) NOT NULL,&#x0D;
    [IndicatorId] int(10,0) NOT NULL,&#x0D;
    [KhoanPeriodId] int(10,0) NOT NULL,&#x0D;
    [TargetValue] decimal(18,2) NULL,&#x0D;
 
CREATE TABLE [dbo].[EmployeeRoles] (
    [EmployeeId] int(10,0) NOT NULL,&#x0D;
    [RoleId] int(10,0) NOT NULL,&#x0D;
);

                                                                                                                                  
CREATE TABLE [dbo].[Employees] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [EmployeeCode] nvarchar(20) NOT NULL,&#x0D;
    [FullName] nvarchar(255) NOT NULL,&#x0D;
    [Username] nvarchar(100) NOT NULL,&#x0D;
    [PasswordHash] nvarchar(-1) NOT NULL,&#x0D;
 
CREATE TABLE [dbo].[FinalPayouts] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [EmployeeId] int(10,0) NOT NULL,&#x0D;
    [KhoanPeriodId] int(10,0) NOT NULL,&#x0D;
    [TotalAmount] decimal(18,2) NOT NULL,&#x0D;
    [V1] decimal(18,2) NULL,&#x0D;
    [V2] dec
CREATE TABLE [dbo].[ImportedDataItems] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [ImportedDataRecordId] int(10,0) NOT NULL,&#x0D;
    [RawData] nvarchar(-1) NOT NULL,&#x0D;
    [ProcessedDate] datetime2 NOT NULL,&#x0D;
    [ProcessingNotes] nvarchar(1000) 
CREATE TABLE [dbo].[ImportedDataRecords] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [FileName] nvarchar(255) NOT NULL,&#x0D;
    [FileType] nvarchar(100) NOT NULL,&#x0D;
    [Category] nvarchar(100) NOT NULL,&#x0D;
    [ImportDate] datetime2 NOT NULL,&#x0D;
CREATE TABLE [dbo].[KhoanPeriods] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [Name] nvarchar(100) NOT NULL,&#x0D;
    [Type] int(10,0) NOT NULL,&#x0D;
    [StartDate] datetime2 NOT NULL,&#x0D;
    [EndDate] datetime2 NOT NULL,&#x0D;
    [Status] int(10,0) N
CREATE TABLE [dbo].[KpiAssignmentTables] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [TableType] int(10,0) NOT NULL,&#x0D;
    [TableName] nvarchar(200) NOT NULL,&#x0D;
    [Description] nvarchar(500) NULL,&#x0D;
    [IsActive] bit NOT NULL,&#x0D;
    [Creat
CREATE TABLE [dbo].[KPIDefinitions] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [KpiCode] nvarchar(100) NOT NULL,&#x0D;
    [KpiName] nvarchar(500) NOT NULL,&#x0D;
    [Description] nvarchar(1000) NULL,&#x0D;
    [MaxScore] decimal(18,2) NOT NULL,&#x0D;
    
CREATE TABLE [dbo].[KpiIndicators] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [TableId] int(10,0) NOT NULL,&#x0D;
    [IndicatorName] nvarchar(300) NOT NULL,&#x0D;
    [MaxScore] decimal(18,2) NOT NULL,&#x0D;
    [Unit] nvarchar(50) NOT NULL,&#x0D;
    [Ord
CREATE TABLE [dbo].[KpiScoringRules] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [KpiIndicatorName] nvarchar(200) NOT NULL,&#x0D;
    [KpiIndicatorCode] nvarchar(50) NULL,&#x0D;
    [ScoringMethod] nvarchar(20) NOT NULL,&#x0D;
    [PercentageStep] decimal(10
CREATE TABLE [dbo].[Positions] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [Name] nvarchar(150) NOT NULL,&#x0D;
    [Description] nvarchar(500) NULL,&#x0D;
);

                                                                                                
CREATE TABLE [dbo].[RawDataImports] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [FileName] nvarchar(200) NOT NULL,&#x0D;
    [DataType] nvarchar(50) NOT NULL,&#x0D;
    [ImportDate] datetime2 NOT NULL,&#x0D;
    [StatementDate] datetime2 NOT NULL,&#x0D;
    
CREATE TABLE [dbo].[RawDataRecords] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [RawDataImportId] int(10,0) NOT NULL,&#x0D;
    [JsonData] nvarchar(-1) NOT NULL,&#x0D;
    [ProcessedDate] datetime2 NOT NULL,&#x0D;
    [ProcessingNotes] nvarchar(500) NOT NULL
CREATE TABLE [dbo].[Roles] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [Name] nvarchar(100) NOT NULL,&#x0D;
    [Description] nvarchar(255) NULL,&#x0D;
);

                                                                                                    
CREATE TABLE [dbo].[SalaryParameters] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [Name] nvarchar(-1) NOT NULL,&#x0D;
    [Value] decimal(18,2) NOT NULL,&#x0D;
    [Note] nvarchar(-1) NULL,&#x0D;
);

                                                        
CREATE TABLE [dbo].[TransactionAdjustmentFactors] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [Factor] decimal(18,2) NOT NULL,&#x0D;
    [Note] nvarchar(-1) NULL,&#x0D;
    [LegacyKPIDefinitionId] int(10,0) NULL,&#x0D;
);

                                 
CREATE TABLE [dbo].[UnitKhoanAssignmentDetails] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [UnitKhoanAssignmentId] int(10,0) NOT NULL,&#x0D;
    [TargetValue] decimal(18,2) NOT NULL,&#x0D;
    [ActualValue] decimal(18,2) NULL,&#x0D;
    [Score] decimal(18,2
CREATE TABLE [dbo].[UnitKhoanAssignments] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [UnitId] int(10,0) NOT NULL,&#x0D;
    [KhoanPeriodId] int(10,0) NOT NULL,&#x0D;
    [AssignedDate] datetime2 NOT NULL,&#x0D;
    [Note] nvarchar(-1) NULL,&#x0D;
);

    
CREATE TABLE [dbo].[UnitKpiScoringCriterias] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [UnitKpiScoringId] int(10,0) NOT NULL,&#x0D;
    [ViolationType] nvarchar(30) NOT NULL,&#x0D;
    [ViolationLevel] nvarchar(20) NOT NULL,&#x0D;
    [ViolationCount] int(
CREATE TABLE [dbo].[UnitKpiScoringDetails] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [UnitKpiScoringId] int(10,0) NOT NULL,&#x0D;
    [KpiIndicatorId] int(10,0) NULL,&#x0D;
    [IndicatorName] nvarchar(200) NOT NULL,&#x0D;
    [TargetValue] decimal(15,2) N
CREATE TABLE [dbo].[UnitKpiScorings] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [UnitKhoanAssignmentId] int(10,0) NOT NULL,&#x0D;
    [KhoanPeriodId] int(10,0) NOT NULL,&#x0D;
    [UnitId] int(10,0) NOT NULL,&#x0D;
    [ScoringDate] datetime2 NOT NULL,&#x0D
CREATE TABLE [dbo].[Units] (
    [Id] int(10,0) NOT NULL,&#x0D;
    [Code] nvarchar(50) NOT NULL,&#x0D;
    [Name] nvarchar(255) NOT NULL,&#x0D;
    [Type] nvarchar(100) NULL,&#x0D;
    [ParentUnitId] int(10,0) NULL,&#x0D;
);

                          
