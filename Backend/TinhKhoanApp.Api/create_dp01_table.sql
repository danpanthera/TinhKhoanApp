-- Create DP01 Clean Table with Temporal + Columnstore
USE TinhKhoanDB;

-- Drop table if exists
IF OBJECT_ID('dbo.DP01_Clean', 'U') IS NOT NULL
BEGIN
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DP01_Clean' AND temporal_type = 2)
    BEGIN
        ALTER TABLE [dbo].[DP01_Clean] SET (SYSTEM_VERSIONING = OFF);
        DROP TABLE IF EXISTS [dbo].[DP01_Clean_History];
    END
    DROP TABLE [dbo].[DP01_Clean];
END;

-- Create DP01_Clean table with temporal configuration
CREATE TABLE [dbo].[DP01_Clean] (
    -- Primary Key
    [Id] bigint IDENTITY(1,1) NOT NULL,

    -- Data Date (from filename parsing)
    [NGAY_DL] datetime2 NOT NULL,

    -- 63 Business Columns (matching CSV exactly)
    [ACCOUNT_BALANCE] decimal(18,2) NULL,
    [ACCOUNT_CODE] nvarchar(20) NULL,
    [ACCOUNT_CURRENCY] nvarchar(3) NULL,
    [ACCOUNT_DATE] datetime2 NULL,
    [ACCOUNT_MATURITY_DATE] datetime2 NULL,
    [ACCOUNT_NUMBER] nvarchar(50) NULL,
    [ACCOUNT_STATUS] nvarchar(10) NULL,
    [ACCOUNT_TERM] nvarchar(20) NULL,
    [ACCOUNT_TYPE] nvarchar(20) NULL,
    [ADDRESS] nvarchar(500) NULL,
    [AMOUNT_BLOCKED] decimal(18,2) NULL,
    [AMOUNT_NORMAL] decimal(18,2) NULL,
    [AVERAGE_DAILY_BALANCE] decimal(18,2) NULL,
    [BRANCH_CODE] nvarchar(10) NULL,
    [BRANCH_NAME] nvarchar(200) NULL,
    [CITY_CODE] nvarchar(10) NULL,
    [CITY_NAME] nvarchar(100) NULL,
    [CUST_TYPE] nvarchar(10) NULL,
    [CUSTOMER_CLASS] nvarchar(20) NULL,
    [CUSTOMER_CODE] nvarchar(50) NULL,
    [CUSTOMER_ID] nvarchar(50) NULL,
    [CUSTOMER_NAME] nvarchar(200) NULL,
    [CUSTOMER_SEGMENT] nvarchar(50) NULL,
    [CUSTOMER_TYPE] nvarchar(20) NULL,
    [DESCRIPTION] nvarchar(500) NULL,
    [EFFECTIVE_RATE] decimal(18,6) NULL,
    [EMPLOYEE_CODE] nvarchar(20) NULL,
    [EMPLOYEE_NAME] nvarchar(100) NULL,
    [EXCHANGE_RATE] decimal(18,6) NULL,
    [FINAL_BALANCE] decimal(18,2) NULL,
    [FINAL_BALANCE_LCY] decimal(18,2) NULL,
    [GENDER] nvarchar(10) NULL,
    [GRACE_PERIOD] int NULL,
    [ID_NUMBER] nvarchar(50) NULL,
    [ID_TYPE] nvarchar(20) NULL,
    [INITIAL_DEPOSIT] decimal(18,2) NULL,
    [INTEREST_ACCRUED] decimal(18,2) NULL,
    [INTEREST_RATE] decimal(18,6) NULL,
    [INTEREST_RATE_TYPE] nvarchar(20) NULL,
    [IS_STAFF] bit NULL,
    [JOINT_ACCOUNT_FLAG] bit NULL,
    [LAST_TRANSACTION_DATE] datetime2 NULL,
    [MATURITY_AMOUNT] decimal(18,2) NULL,
    [MOBILE_NUMBER] nvarchar(20) NULL,
    [NEXT_ROLLOVER_DATE] datetime2 NULL,
    [OPENING_BALANCE] decimal(18,2) NULL,
    [PHONE_NUMBER] nvarchar(20) NULL,
    [PRODUCT_CODE] nvarchar(20) NULL,
    [PRODUCT_NAME] nvarchar(200) NULL,
    [PRODUCT_TYPE] nvarchar(50) NULL,
    [PURPOSE_CODE] nvarchar(10) NULL,
    [PURPOSE_DESCRIPTION] nvarchar(200) NULL,
    [REGION_CODE] nvarchar(10) NULL,
    [REGION_NAME] nvarchar(100) NULL,
    [RELATIONSHIP_OFFICER] nvarchar(100) NULL,
    [RENEWAL_COUNT] int NULL,
    [ROLLOVER_FLAG] bit NULL,
    [SETTLEMENT_ACCOUNT] nvarchar(50) NULL,
    [SOURCE_SYSTEM] nvarchar(50) NULL,
    [SUB_BRANCH_CODE] nvarchar(10) NULL,
    [SUB_BRANCH_NAME] nvarchar(200) NULL,
    [TERM_UNIT] nvarchar(10) NULL,
    [TOTAL_INTEREST_PAID] decimal(18,2) NULL,
    [TRANSACTION_COUNT] int NULL,

    -- System Audit Fields
    [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] nvarchar(100) NULL,
    [UpdatedBy] nvarchar(100) NULL,

    -- Temporal table period columns
    [ValidFrom] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [ValidTo] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo]),

    -- Primary Key Constraint
    CONSTRAINT [PK_DP01_Clean] PRIMARY KEY CLUSTERED ([Id] ASC)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[DP01_Clean_History]));

-- Create columnstore index for analytics performance
CREATE NONCLUSTERED COLUMNSTORE INDEX [NCCI_DP01_Clean_Analytics]
ON [dbo].[DP01_Clean] (
    [NGAY_DL], [BRANCH_CODE], [CUSTOMER_CODE], [ACCOUNT_NUMBER],
    [ACCOUNT_BALANCE], [PRODUCT_CODE], [CUSTOMER_SEGMENT]
);

-- Create performance indexes
CREATE NONCLUSTERED INDEX [IX_DP01_Clean_NGAY_DL]
ON [dbo].[DP01_Clean] ([NGAY_DL]);

CREATE NONCLUSTERED INDEX [IX_DP01_Clean_BRANCH_CODE]
ON [dbo].[DP01_Clean] ([BRANCH_CODE]);

CREATE NONCLUSTERED INDEX [IX_DP01_Clean_CUSTOMER_CODE]
ON [dbo].[DP01_Clean] ([CUSTOMER_CODE]);

CREATE NONCLUSTERED INDEX [IX_DP01_Clean_ACCOUNT_NUMBER]
ON [dbo].[DP01_Clean] ([ACCOUNT_NUMBER]);

PRINT 'DP01_Clean table created successfully with temporal + columnstore configuration!';
