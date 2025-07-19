-- 🗄️ Setup 8 Core DataTables with Proper Storage Mechanisms
-- Thiết lập 8 bảng dữ liệu chính với cơ chế lưu trữ tối ưu
-- Created: 2025-07-19
-- Author: System Architecture Team

SET QUOTED_IDENTIFIER ON;
GO

PRINT '🗄️ SETTING UP 8 CORE DATATABLES WITH OPTIMAL STORAGE';
PRINT '====================================================';
PRINT '';

-- ==========================================
-- 1. GL01 - Partitioned Table (Huge daily data)
-- ==========================================
PRINT '📊 Step 1: Setting up GL01 with Partitioned storage...';

-- Drop existing GL01 if exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GL01')
BEGIN
    -- Drop temporal if enabled
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GL01' AND temporal_type = 2)
    BEGIN
        ALTER TABLE GL01 SET (SYSTEM_VERSIONING = OFF);
        PRINT '   - Temporal disabled for GL01';
    END

    DROP TABLE IF EXISTS GL01_History;
    DROP TABLE GL01;
    PRINT '   - Existing GL01 dropped';
END

-- Create partition function for GL01 (by date)
IF NOT EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'PF_GL01_Date')
BEGIN
    CREATE PARTITION FUNCTION PF_GL01_Date (datetime2)
    AS RANGE RIGHT FOR VALUES (
        '2024-01-01', '2024-02-01', '2024-03-01', '2024-04-01',
        '2024-05-01', '2024-06-01', '2024-07-01', '2024-08-01',
        '2024-09-01', '2024-10-01', '2024-11-01', '2024-12-01',
        '2025-01-01', '2025-02-01', '2025-03-01', '2025-04-01',
        '2025-05-01', '2025-06-01', '2025-07-01', '2025-08-01',
        '2025-09-01', '2025-10-01', '2025-11-01', '2025-12-01'
    );
    PRINT '   - Partition function PF_GL01_Date created';
END

-- Create partition scheme for GL01
IF NOT EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'PS_GL01_Date')
BEGIN
    CREATE PARTITION SCHEME PS_GL01_Date
    AS PARTITION PF_GL01_Date
    ALL TO ([PRIMARY]);
    PRINT '   - Partition scheme PS_GL01_Date created';
END

-- Create GL01 table with business columns first, system columns last
CREATE TABLE [dbo].[GL01] (
    -- Business Columns (First)
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [DataDate] DATETIME2(7) NOT NULL,
    [BranchCode] NVARCHAR(50) NULL,
    [AccountNumber] NVARCHAR(100) NULL,
    [CustomerCode] NVARCHAR(50) NULL,
    [Currency] NVARCHAR(10) NULL,
    [DebitAmount] DECIMAL(18,2) NULL,
    [CreditAmount] DECIMAL(18,2) NULL,
    [Balance] DECIMAL(18,2) NULL,
    [TransactionCode] NVARCHAR(50) NULL,
    [Description] NVARCHAR(500) NULL,
    [Reference] NVARCHAR(200) NULL,
    [ValueDate] DATETIME2(7) NULL,
    [BookingDate] DATETIME2(7) NULL,

    -- System Columns (Last)
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0,

    CONSTRAINT [PK_GL01] PRIMARY KEY CLUSTERED ([Id], [DataDate])
) ON PS_GL01_Date([DataDate]);

-- Create columnstore index for GL01
CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_GL01_Columnstore] ON [dbo].[GL01]
(
    [DataDate], [BranchCode], [AccountNumber], [CustomerCode], [Currency],
    [DebitAmount], [CreditAmount], [Balance], [TransactionCode], [Description]
);

PRINT '✅ GL01: Partitioned table with columnstore index created';
PRINT '';

-- ==========================================
-- 2. DP01 - Temporal Table
-- ==========================================
PRINT '📊 Step 2: Setting up DP01 with Temporal storage...';

-- Drop existing DP01 if exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DP01')
BEGIN
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DP01' AND temporal_type = 2)
    BEGIN
        ALTER TABLE DP01 SET (SYSTEM_VERSIONING = OFF);
    END
    DROP TABLE IF EXISTS DP01_History;
    DROP TABLE DP01;
    PRINT '   - Existing DP01 dropped';
END

-- Create DP01 table with business columns first, temporal columns last
CREATE TABLE [dbo].[DP01] (
    -- Business Columns (First)
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [DataDate] DATETIME2(7) NOT NULL,
    [BranchCode] NVARCHAR(50) NULL,
    [AccountNumber] NVARCHAR(100) NULL,
    [CustomerCode] NVARCHAR(50) NULL,
    [ProductType] NVARCHAR(100) NULL,
    [Currency] NVARCHAR(10) NULL,
    [DepositAmount] DECIMAL(18,2) NULL,
    [Balance] DECIMAL(18,2) NULL,
    [InterestRate] DECIMAL(10,4) NULL,
    [MaturityDate] DATETIME2(7) NULL,
    [OpeningDate] DATETIME2(7) NULL,
    [Status] NVARCHAR(50) NULL,

    -- System Columns (Last)
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0,

    -- Temporal Columns (Last)
    [SysStartTime] DATETIME2(7) GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [SysEndTime] DATETIME2(7) GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),

    CONSTRAINT [PK_DP01] PRIMARY KEY CLUSTERED ([Id])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[DP01_History]));

-- Create columnstore index for DP01
CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_DP01_Columnstore] ON [dbo].[DP01]
(
    [DataDate], [BranchCode], [AccountNumber], [CustomerCode], [ProductType],
    [Currency], [DepositAmount], [Balance], [InterestRate], [Status]
);

PRINT '✅ DP01: Temporal table with columnstore index created';
PRINT '';

-- ==========================================
-- 3. DPDA - Temporal Table
-- ==========================================
PRINT '📊 Step 3: Setting up DPDA with Temporal storage...';

-- Drop existing DPDA if exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DPDA')
BEGIN
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DPDA' AND temporal_type = 2)
    BEGIN
        ALTER TABLE DPDA SET (SYSTEM_VERSIONING = OFF);
    END
    DROP TABLE IF EXISTS DPDA_History;
    DROP TABLE DPDA;
    PRINT '   - Existing DPDA dropped';
END

-- Create DPDA table
CREATE TABLE [dbo].[DPDA] (
    -- Business Columns (First)
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [DataDate] DATETIME2(7) NOT NULL,
    [BranchCode] NVARCHAR(50) NULL,
    [AccountNumber] NVARCHAR(100) NULL,
    [CustomerCode] NVARCHAR(50) NULL,
    [DepositType] NVARCHAR(100) NULL,
    [Amount] DECIMAL(18,2) NULL,
    [Term] INT NULL,
    [InterestRate] DECIMAL(10,4) NULL,
    [MaturityAmount] DECIMAL(18,2) NULL,

    -- System Columns (Last)
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0,

    -- Temporal Columns (Last)
    [SysStartTime] DATETIME2(7) GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [SysEndTime] DATETIME2(7) GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),

    CONSTRAINT [PK_DPDA] PRIMARY KEY CLUSTERED ([Id])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[DPDA_History]));

-- Create columnstore index for DPDA
CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_DPDA_Columnstore] ON [dbo].[DPDA]
([DataDate], [BranchCode], [AccountNumber], [CustomerCode], [DepositType], [Amount]);

PRINT '✅ DPDA: Temporal table with columnstore index created';
PRINT '';

-- ==========================================
-- 4. EI01 - Temporal Table
-- ==========================================
PRINT '📊 Step 4: Setting up EI01 with Temporal storage...';

-- Drop existing EI01 if exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EI01')
BEGIN
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EI01' AND temporal_type = 2)
    BEGIN
        ALTER TABLE EI01 SET (SYSTEM_VERSIONING = OFF);
    END
    DROP TABLE IF EXISTS EI01_History;
    DROP TABLE EI01;
    PRINT '   - Existing EI01 dropped';
END

-- Create EI01 table
CREATE TABLE [dbo].[EI01] (
    -- Business Columns (First)
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [DataDate] DATETIME2(7) NOT NULL,
    [EmployeeCode] NVARCHAR(50) NULL,
    [EmployeeName] NVARCHAR(200) NULL,
    [DepartmentCode] NVARCHAR(50) NULL,
    [Position] NVARCHAR(100) NULL,
    [Salary] DECIMAL(18,2) NULL,
    [Bonus] DECIMAL(18,2) NULL,
    [TotalIncome] DECIMAL(18,2) NULL,
    [StartDate] DATETIME2(7) NULL,
    [Status] NVARCHAR(50) NULL,

    -- System Columns (Last)
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0,

    -- Temporal Columns (Last)
    [SysStartTime] DATETIME2(7) GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [SysEndTime] DATETIME2(7) GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),

    CONSTRAINT [PK_EI01] PRIMARY KEY CLUSTERED ([Id])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[EI01_History]));

-- Create columnstore index for EI01
CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_EI01_Columnstore] ON [dbo].[EI01]
([DataDate], [EmployeeCode], [DepartmentCode], [Position], [Salary], [TotalIncome]);

PRINT '✅ EI01: Temporal table with columnstore index created';
PRINT '';

-- ==========================================
-- 5. GL41 - Temporal Table
-- ==========================================
PRINT '📊 Step 5: Setting up GL41 with Temporal storage...';

-- Drop existing GL41 if exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GL41')
BEGIN
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GL41' AND temporal_type = 2)
    BEGIN
        ALTER TABLE GL41 SET (SYSTEM_VERSIONING = OFF);
    END
    DROP TABLE IF EXISTS GL41_History;
    DROP TABLE GL41;
    PRINT '   - Existing GL41 dropped';
END

-- Create GL41 table
CREATE TABLE [dbo].[GL41] (
    -- Business Columns (First)
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [DataDate] DATETIME2(7) NOT NULL,
    [AccountCode] NVARCHAR(50) NULL,
    [AccountName] NVARCHAR(200) NULL,
    [BranchCode] NVARCHAR(50) NULL,
    [CostCenter] NVARCHAR(50) NULL,
    [DebitAmount] DECIMAL(18,2) NULL,
    [CreditAmount] DECIMAL(18,2) NULL,
    [NetAmount] DECIMAL(18,2) NULL,
    [Description] NVARCHAR(500) NULL,

    -- System Columns (Last)
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0,

    -- Temporal Columns (Last)
    [SysStartTime] DATETIME2(7) GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [SysEndTime] DATETIME2(7) GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),

    CONSTRAINT [PK_GL41] PRIMARY KEY CLUSTERED ([Id])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[GL41_History]));

-- Create columnstore index for GL41
CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_GL41_Columnstore] ON [dbo].[GL41]
([DataDate], [AccountCode], [BranchCode], [CostCenter], [DebitAmount], [CreditAmount]);

PRINT '✅ GL41: Temporal table with columnstore index created';
PRINT '';

-- ==========================================
-- 6. LN01 - Temporal Table
-- ==========================================
PRINT '📊 Step 6: Setting up LN01 with Temporal storage...';

-- Drop existing LN01 if exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN01')
BEGIN
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN01' AND temporal_type = 2)
    BEGIN
        ALTER TABLE LN01 SET (SYSTEM_VERSIONING = OFF);
    END
    DROP TABLE IF EXISTS LN01_History;
    DROP TABLE LN01;
    PRINT '   - Existing LN01 dropped';
END

-- Create LN01 table
CREATE TABLE [dbo].[LN01] (
    -- Business Columns (First)
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [DataDate] DATETIME2(7) NOT NULL,
    [LoanNumber] NVARCHAR(100) NULL,
    [CustomerCode] NVARCHAR(50) NULL,
    [BranchCode] NVARCHAR(50) NULL,
    [LoanType] NVARCHAR(100) NULL,
    [Currency] NVARCHAR(10) NULL,
    [LoanAmount] DECIMAL(18,2) NULL,
    [OutstandingBalance] DECIMAL(18,2) NULL,
    [InterestRate] DECIMAL(10,4) NULL,
    [MaturityDate] DATETIME2(7) NULL,
    [DisbursementDate] DATETIME2(7) NULL,
    [Status] NVARCHAR(50) NULL,

    -- System Columns (Last)
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0,

    -- Temporal Columns (Last)
    [SysStartTime] DATETIME2(7) GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [SysEndTime] DATETIME2(7) GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),

    CONSTRAINT [PK_LN01] PRIMARY KEY CLUSTERED ([Id])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[LN01_History]));

-- Create columnstore index for LN01
CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_LN01_Columnstore] ON [dbo].[LN01]
([DataDate], [LoanNumber], [CustomerCode], [BranchCode], [LoanType], [LoanAmount], [OutstandingBalance]);

PRINT '✅ LN01: Temporal table with columnstore index created';
PRINT '';

-- ==========================================
-- 7. LN03 - Temporal Table
-- ==========================================
PRINT '📊 Step 7: Setting up LN03 with Temporal storage...';

-- Drop existing LN03 if exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03')
BEGIN
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03' AND temporal_type = 2)
    BEGIN
        ALTER TABLE LN03 SET (SYSTEM_VERSIONING = OFF);
    END
    DROP TABLE IF EXISTS LN03_History;
    DROP TABLE LN03;
    PRINT '   - Existing LN03 dropped';
END

-- Create LN03 table
CREATE TABLE [dbo].[LN03] (
    -- Business Columns (First)
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [DataDate] DATETIME2(7) NOT NULL,
    [LoanNumber] NVARCHAR(100) NULL,
    [CustomerCode] NVARCHAR(50) NULL,
    [CollateralType] NVARCHAR(100) NULL,
    [CollateralValue] DECIMAL(18,2) NULL,
    [CollateralDescription] NVARCHAR(500) NULL,
    [AppraisalDate] DATETIME2(7) NULL,
    [AppraisalValue] DECIMAL(18,2) NULL,
    [SecurityRatio] DECIMAL(10,4) NULL,

    -- System Columns (Last)
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0,

    -- Temporal Columns (Last)
    [SysStartTime] DATETIME2(7) GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [SysEndTime] DATETIME2(7) GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),

    CONSTRAINT [PK_LN03] PRIMARY KEY CLUSTERED ([Id])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[LN03_History]));

-- Create columnstore index for LN03
CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_LN03_Columnstore] ON [dbo].[LN03]
([DataDate], [LoanNumber], [CustomerCode], [CollateralType], [CollateralValue], [SecurityRatio]);

PRINT '✅ LN03: Temporal table with columnstore index created';
PRINT '';

-- ==========================================
-- 8. RR01 - Temporal Table
-- ==========================================
PRINT '📊 Step 8: Setting up RR01 with Temporal storage...';

-- Drop existing RR01 if exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'RR01')
BEGIN
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'RR01' AND temporal_type = 2)
    BEGIN
        ALTER TABLE RR01 SET (SYSTEM_VERSIONING = OFF);
    END
    DROP TABLE IF EXISTS RR01_History;
    DROP TABLE RR01;
    PRINT '   - Existing RR01 dropped';
END

-- Create RR01 table
CREATE TABLE [dbo].[RR01] (
    -- Business Columns (First)
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [DataDate] DATETIME2(7) NOT NULL,
    [RiskCategory] NVARCHAR(100) NULL,
    [CustomerCode] NVARCHAR(50) NULL,
    [LoanNumber] NVARCHAR(100) NULL,
    [RiskRating] NVARCHAR(50) NULL,
    [ExposureAmount] DECIMAL(18,2) NULL,
    [ProbabilityOfDefault] DECIMAL(10,4) NULL,
    [LossGivenDefault] DECIMAL(10,4) NULL,
    [ExpectedLoss] DECIMAL(18,2) NULL,
    [RiskWeight] DECIMAL(10,4) NULL,

    -- System Columns (Last)
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0,

    -- Temporal Columns (Last)
    [SysStartTime] DATETIME2(7) GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [SysEndTime] DATETIME2(7) GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),

    CONSTRAINT [PK_RR01] PRIMARY KEY CLUSTERED ([Id])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[RR01_History]));

-- Create columnstore index for RR01
CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_RR01_Columnstore] ON [dbo].[RR01]
([DataDate], [RiskCategory], [CustomerCode], [LoanNumber], [RiskRating], [ExposureAmount]);

PRINT '✅ RR01: Temporal table with columnstore index created';
PRINT '';

-- ==========================================
-- Final Verification
-- ==========================================
PRINT '🔍 Final Verification of 8 Core DataTables...';

SELECT
    'TABLE_SUMMARY' as Category,
    t.name as TableName,
    CASE
        WHEN t.temporal_type = 2 THEN 'Temporal'
        WHEN EXISTS (SELECT 1 FROM sys.partition_schemes ps
                    JOIN sys.indexes i ON i.data_space_id = ps.data_space_id
                    WHERE i.object_id = t.object_id) THEN 'Partitioned'
        ELSE 'Standard'
    END as StorageType,
    CASE WHEN EXISTS (SELECT 1 FROM sys.indexes i WHERE i.object_id = t.object_id AND i.type = 6)
         THEN 'Yes' ELSE 'No' END as HasColumnstore
FROM sys.tables t
WHERE t.name IN ('GL01', 'DP01', 'DPDA', 'EI01', 'GL41', 'LN01', 'LN03', 'RR01')
ORDER BY t.name;

PRINT '';
PRINT '🎉 ALL 8 CORE DATATABLES SETUP COMPLETED!';
PRINT '✅ GL01: Partitioned with Columnstore Index';
PRINT '✅ DP01, DPDA, EI01, GL41, LN01, LN03, RR01: Temporal with Columnstore Index';
PRINT '✅ All tables have business columns first, system/temporal columns last';
PRINT '✅ Direct Import capability enabled for all tables';
PRINT '✅ Preview Direct from actual tables (no mock data)';
PRINT '';
