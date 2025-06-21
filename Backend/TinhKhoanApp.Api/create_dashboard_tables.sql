-- Create Dashboard Tables Script
-- This script creates only the dashboard-related tables if they don't exist

-- First, add IsDeleted column to Units table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'Units') AND name = 'IsDeleted')
BEGIN
    ALTER TABLE Units ADD IsDeleted bit NOT NULL DEFAULT 0;
END

-- Create DashboardIndicators table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DashboardIndicators')
BEGIN
    CREATE TABLE DashboardIndicators (
        Id int IDENTITY(1,1) NOT NULL,
        Code nvarchar(50) NOT NULL,
        Name nvarchar(200) NOT NULL,
        Unit nvarchar(50) NULL,
        Icon nvarchar(100) NULL,
        Color nvarchar(10) NULL,
        SortOrder int NOT NULL,
        IsActive bit NOT NULL,
        Description nvarchar(500) NULL,
        CreatedDate datetime2 NOT NULL DEFAULT GETDATE(),
        ModifiedDate datetime2 NULL,
        IsDeleted bit NOT NULL DEFAULT 0,
        CONSTRAINT PK_DashboardIndicators PRIMARY KEY (Id)
    );
    
    CREATE UNIQUE INDEX IX_DashboardIndicators_Code ON DashboardIndicators (Code);
END

-- Create BusinessPlanTargets table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BusinessPlanTargets')
BEGIN
    CREATE TABLE BusinessPlanTargets (
        Id int IDENTITY(1,1) NOT NULL,
        DashboardIndicatorId int NOT NULL,
        UnitId int NOT NULL,
        Year int NOT NULL,
        Quarter int NULL,
        Month int NULL,
        TargetValue decimal(18,2) NOT NULL,
        Notes nvarchar(500) NULL,
        Status nvarchar(50) NULL DEFAULT 'Draft',
        CreatedDate datetime2 NOT NULL DEFAULT GETDATE(),
        CreatedBy nvarchar(100) NULL,
        ModifiedDate datetime2 NULL,
        ModifiedBy nvarchar(100) NULL,
        IsDeleted bit NOT NULL DEFAULT 0,
        ApprovedBy nvarchar(100) NULL,
        ApprovedAt datetime2 NULL,
        CONSTRAINT PK_BusinessPlanTargets PRIMARY KEY (Id),
        CONSTRAINT FK_BusinessPlanTargets_DashboardIndicators_DashboardIndicatorId 
            FOREIGN KEY (DashboardIndicatorId) REFERENCES DashboardIndicators (Id) ON DELETE CASCADE,
        CONSTRAINT FK_BusinessPlanTargets_Units_UnitId 
            FOREIGN KEY (UnitId) REFERENCES Units (Id) ON DELETE CASCADE
    );
    
    CREATE UNIQUE INDEX IX_BusinessPlanTarget_Unique ON BusinessPlanTargets 
        (DashboardIndicatorId, UnitId, Year, Quarter, Month) 
        WHERE Quarter IS NOT NULL AND Month IS NOT NULL;
    
    CREATE INDEX IX_BusinessPlanTargets_UnitId ON BusinessPlanTargets (UnitId);
END

-- Create DashboardCalculations table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DashboardCalculations')
BEGIN
    CREATE TABLE DashboardCalculations (
        Id int IDENTITY(1,1) NOT NULL,
        DashboardIndicatorId int NOT NULL,
        UnitId int NOT NULL,
        Year int NOT NULL,
        Quarter int NULL,
        Month int NULL,
        CalculationDate datetime2 NOT NULL,
        ActualValue decimal(18,2) NULL,
        CalculationDetails nvarchar(max) NULL,
        DataSource nvarchar(50) NULL,
        DataDate datetime2 NOT NULL,
        CreatedDate datetime2 NOT NULL DEFAULT GETDATE(),
        CreatedBy nvarchar(100) NULL,
        ModifiedDate datetime2 NULL,
        ModifiedBy nvarchar(100) NULL,
        Status nvarchar(20) NOT NULL DEFAULT 'Success',
        ErrorMessage nvarchar(1000) NULL,
        ExecutionTime time NULL,
        AppliedFilters nvarchar(max) NULL,
        IsDeleted bit NOT NULL DEFAULT 0,
        CONSTRAINT PK_DashboardCalculations PRIMARY KEY (Id),
        CONSTRAINT FK_DashboardCalculations_DashboardIndicators_DashboardIndicatorId 
            FOREIGN KEY (DashboardIndicatorId) REFERENCES DashboardIndicators (Id) ON DELETE CASCADE,
        CONSTRAINT FK_DashboardCalculations_Units_UnitId 
            FOREIGN KEY (UnitId) REFERENCES Units (Id) ON DELETE CASCADE
    );
    
    CREATE UNIQUE INDEX IX_DashboardCalculation_Unique ON DashboardCalculations 
        (DashboardIndicatorId, UnitId, CalculationDate);
    
    CREATE INDEX IX_DashboardCalculations_UnitId ON DashboardCalculations (UnitId);
END

PRINT 'Dashboard tables created successfully!'
