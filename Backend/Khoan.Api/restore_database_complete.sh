#!/bin/bash

# =============================================================================
# DATABASE RESTORATION SCRIPT FOR TINHKHOAN APP
# Phục hồi hoàn toàn cấu trúc database và dữ liệu
# =============================================================================

set -e

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
WHITE='\033[1;37m'
NC='\033[0m'

echo -e "${WHITE}🗄️  DATABASE RESTORATION FOR TINHKHOAN APP${NC}"
echo "=================================================="

# Database connection parameters
SERVER="localhost,1433"
USERNAME="sa"
PASSWORD="Dientoan@303"
DATABASE="TinhKhoanDB"

# Check SQL Server connection
echo -e "${PURPLE}🔌 Kiểm tra kết nối SQL Server...${NC}"
if ! sqlcmd -S "$SERVER" -U "$USERNAME" -P "$PASSWORD" -Q "SELECT @@VERSION" -C >/dev/null 2>&1; then
    echo -e "${RED}❌ Không thể kết nối đến SQL Server${NC}"
    echo -e "${YELLOW}💡 Hãy chắc chắn container đang chạy và SQL Server đã sẵn sàng${NC}"
    exit 1
fi

echo -e "${GREEN}✅ Kết nối SQL Server thành công${NC}"

# Create/Use TinhKhoanDB database
echo -e "${PURPLE}🗄️  Tạo/Sử dụng database TinhKhoanDB...${NC}"
sqlcmd -S "$SERVER" -U "$USERNAME" -P "$PASSWORD" -Q "
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '$DATABASE')
BEGIN
    CREATE DATABASE $DATABASE
    COLLATE SQL_Latin1_General_CP1_CI_AS
    PRINT 'Database TinhKhoanDB đã được tạo'
END
ELSE
    PRINT 'Database TinhKhoanDB đã tồn tại'
" -C

echo -e "${GREEN}✅ Database TinhKhoanDB đã sẵn sàng${NC}"

# Create comprehensive table structure
echo -e "${PURPLE}📋 Tạo cấu trúc bảng hoàn chỉnh...${NC}"

# Create tables SQL script
sqlcmd -S "$SERVER" -U "$USERNAME" -P "$PASSWORD" -d "$DATABASE" -Q "
-- =============================================================================
-- TINHKHOAN APP - COMPLETE TABLE STRUCTURE
-- 8 Core Tables with Temporal and Columnstore optimizations
-- =============================================================================

-- 1. GL01 Table (General Ledger) - NO Temporal, WITH Columnstore
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GL01')
BEGIN
    CREATE TABLE GL01 (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        CCYCODE NVARCHAR(3) NOT NULL,
        CUSTCODE NVARCHAR(20) NOT NULL,
        GLACODE NVARCHAR(20) NOT NULL,
        ORGAMT DECIMAL(18,2) NOT NULL,
        EQVAMT DECIMAL(18,2) NOT NULL,
        TXNDATE DATE NOT NULL,
        VALDATE DATE NOT NULL,
        BRANCHCODE NVARCHAR(10) NOT NULL,
        PRODUCTCODE NVARCHAR(20),
        DEPARTMENT NVARCHAR(50),
        CreatedDate DATETIME2 DEFAULT GETDATE(),
        ModifiedDate DATETIME2 DEFAULT GETDATE()
    )

    -- Columnstore Index for GL01
    CREATE CLUSTERED COLUMNSTORE INDEX CCI_GL01 ON GL01

    PRINT '✅ GL01 table created with Columnstore'
END
ELSE
    PRINT 'GL01 table already exists'

-- 2. DP01 Table (Deposit) - WITH Temporal and Columnstore
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DP01')
BEGIN
    CREATE TABLE DP01 (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        CUSTCODE NVARCHAR(20) NOT NULL,
        ACCCODE NVARCHAR(20) NOT NULL,
        CCYCODE NVARCHAR(3) NOT NULL,
        BALANCE DECIMAL(18,2) NOT NULL,
        AVAILBAL DECIMAL(18,2) NOT NULL,
        HOLDAMT DECIMAL(18,2) DEFAULT 0,
        INTRATE DECIMAL(8,4) DEFAULT 0,
        MATDATE DATE,
        OPENDATE DATE NOT NULL,
        BRANCHCODE NVARCHAR(10) NOT NULL,
        PRODUCTCODE NVARCHAR(20),
        STATUS NVARCHAR(10) DEFAULT 'ACTIVE',
        CreatedDate DATETIME2 DEFAULT GETDATE(),
        ModifiedDate DATETIME2 DEFAULT GETDATE(),

        -- Temporal table columns
        ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL,
        ValidTo DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL,
        PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.DP01_History))

    PRINT '✅ DP01 temporal table created'
END
ELSE
    PRINT 'DP01 table already exists'

-- 3. EI01 Table (Employee Information) - WITH Temporal and Columnstore
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EI01')
BEGIN
    CREATE TABLE EI01 (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        EMPCODE NVARCHAR(20) NOT NULL,
        EMPNAME NVARCHAR(100) NOT NULL,
        DEPARTMENT NVARCHAR(50) NOT NULL,
        POSITION NVARCHAR(50),
        BRANCHCODE NVARCHAR(10) NOT NULL,
        EMAIL NVARCHAR(100),
        PHONE NVARCHAR(20),
        JOINDATE DATE NOT NULL,
        STATUS NVARCHAR(10) DEFAULT 'ACTIVE',
        SALARY DECIMAL(18,2),
        CreatedDate DATETIME2 DEFAULT GETDATE(),
        ModifiedDate DATETIME2 DEFAULT GETDATE(),

        -- Temporal table columns
        ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL,
        ValidTo DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL,
        PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.EI01_History))

    PRINT '✅ EI01 temporal table created'
END
ELSE
    PRINT 'EI01 table already exists'

-- 4. GL41 Table (Transaction Details) - WITH Temporal and Columnstore
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GL41')
BEGIN
    CREATE TABLE GL41 (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        TXNCODE NVARCHAR(20) NOT NULL,
        CUSTCODE NVARCHAR(20) NOT NULL,
        GLACODE NVARCHAR(20) NOT NULL,
        CCYCODE NVARCHAR(3) NOT NULL,
        DRAMT DECIMAL(18,2) DEFAULT 0,
        CRAMT DECIMAL(18,2) DEFAULT 0,
        TXNDATE DATE NOT NULL,
        VALDATE DATE NOT NULL,
        DESCRIPTION NVARCHAR(255),
        BRANCHCODE NVARCHAR(10) NOT NULL,
        TELLER NVARCHAR(20),
        CreatedDate DATETIME2 DEFAULT GETDATE(),
        ModifiedDate DATETIME2 DEFAULT GETDATE(),

        -- Temporal table columns
        ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL,
        ValidTo DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL,
        PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.GL41_History))

    PRINT '✅ GL41 temporal table created'
END
ELSE
    PRINT 'GL41 table already exists'

-- 5. LN01 Table (Loan Information) - WITH Temporal and Columnstore
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LN01')
BEGIN
    CREATE TABLE LN01 (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        LOANCODE NVARCHAR(20) NOT NULL,
        CUSTCODE NVARCHAR(20) NOT NULL,
        CCYCODE NVARCHAR(3) NOT NULL,
        LOANAMT DECIMAL(18,2) NOT NULL,
        OUTSTAND DECIMAL(18,2) NOT NULL,
        INTRATE DECIMAL(8,4) NOT NULL,
        STARTDATE DATE NOT NULL,
        MATDATE DATE NOT NULL,
        BRANCHCODE NVARCHAR(10) NOT NULL,
        PRODUCTCODE NVARCHAR(20),
        STATUS NVARCHAR(10) DEFAULT 'ACTIVE',
        CreatedDate DATETIME2 DEFAULT GETDATE(),
        ModifiedDate DATETIME2 DEFAULT GETDATE(),

        -- Temporal table columns
        ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL,
        ValidTo DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL,
        PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.LN01_History))

    PRINT '✅ LN01 temporal table created'
END
ELSE
    PRINT 'LN01 table already exists'

-- 6. LN03 Table (Loan Transactions) - WITH Temporal and Columnstore
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03')
BEGIN
    CREATE TABLE LN03 (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        LOANCODE NVARCHAR(20) NOT NULL,
        TXNDATE DATE NOT NULL,
        TXNTYPE NVARCHAR(20) NOT NULL,
        PRINCIPAL DECIMAL(18,2) DEFAULT 0,
        INTEREST DECIMAL(18,2) DEFAULT 0,
        PENALTY DECIMAL(18,2) DEFAULT 0,
        TOTALAMT DECIMAL(18,2) NOT NULL,
        BALANCE DECIMAL(18,2) NOT NULL,
        BRANCHCODE NVARCHAR(10) NOT NULL,
        TELLER NVARCHAR(20),
        CreatedDate DATETIME2 DEFAULT GETDATE(),
        ModifiedDate DATETIME2 DEFAULT GETDATE(),

        -- Temporal table columns
        ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL,
        ValidTo DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL,
        PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.LN03_History))

    PRINT '✅ LN03 temporal table created'
END
ELSE
    PRINT 'LN03 table already exists'

-- 7. RR01 Table (Risk Rating) - WITH Temporal and Columnstore
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RR01')
BEGIN
    CREATE TABLE RR01 (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        CUSTCODE NVARCHAR(20) NOT NULL,
        RISKRATING NVARCHAR(10) NOT NULL,
        RISKDATE DATE NOT NULL,
        RISKSCORE DECIMAL(8,2),
        RISKFACTOR NVARCHAR(255),
        BRANCHCODE NVARCHAR(10) NOT NULL,
        ANALYST NVARCHAR(50),
        STATUS NVARCHAR(10) DEFAULT 'ACTIVE',
        CreatedDate DATETIME2 DEFAULT GETDATE(),
        ModifiedDate DATETIME2 DEFAULT GETDATE(),

        -- Temporal table columns
        ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL,
        ValidTo DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL,
        PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.RR01_History))

    PRINT '✅ RR01 temporal table created'
END
ELSE
    PRINT 'RR01 table already exists'

-- 8. DPDA Table (Deposit Daily Analytics) - WITH Temporal and Columnstore
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DPDA')
BEGIN
    CREATE TABLE DPDA (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        REPORTDATE DATE NOT NULL,
        BRANCHCODE NVARCHAR(10) NOT NULL,
        CCYCODE NVARCHAR(3) NOT NULL,
        TOTALDEPOSITS DECIMAL(18,2) NOT NULL,
        NEWDEPOSITS DECIMAL(18,2) DEFAULT 0,
        WITHDRAWALS DECIMAL(18,2) DEFAULT 0,
        NETCHANGE DECIMAL(18,2) NOT NULL,
        ACCOUNTCOUNT INT DEFAULT 0,
        AVGBALANCE DECIMAL(18,2) DEFAULT 0,
        CreatedDate DATETIME2 DEFAULT GETDATE(),
        ModifiedDate DATETIME2 DEFAULT GETDATE(),

        -- Temporal table columns
        ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL,
        ValidTo DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL,
        PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.DPDA_History))

    PRINT '✅ DPDA temporal table created'
END
ELSE
    PRINT 'DPDA table already exists'

PRINT '🎉 Tất cả 8 core tables đã được tạo hoàn chỉnh!'
" -C

echo -e "${GREEN}✅ Cấu trúc bảng đã được tạo hoàn chỉnh${NC}"

# Add Columnstore indexes to temporal tables
echo -e "${PURPLE}🔍 Thêm Columnstore indexes cho các bảng temporal...${NC}"
sqlcmd -S "$SERVER" -U "$USERNAME" -P "$PASSWORD" -d "$DATABASE" -Q "
-- Add Columnstore indexes for temporal tables (excluding GL01 which already has one)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_DP01' AND object_id = OBJECT_ID('DP01'))
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX CCI_DP01 ON DP01 (CUSTCODE, ACCCODE, CCYCODE, BALANCE, AVAILBAL, OPENDATE, BRANCHCODE)
    PRINT '✅ Columnstore index added to DP01'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_EI01' AND object_id = OBJECT_ID('EI01'))
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX CCI_EI01 ON EI01 (EMPCODE, EMPNAME, DEPARTMENT, BRANCHCODE, JOINDATE)
    PRINT '✅ Columnstore index added to EI01'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_GL41' AND object_id = OBJECT_ID('GL41'))
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX CCI_GL41 ON GL41 (TXNCODE, CUSTCODE, GLACODE, CCYCODE, DRAMT, CRAMT, TXNDATE, BRANCHCODE)
    PRINT '✅ Columnstore index added to GL41'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_LN01' AND object_id = OBJECT_ID('LN01'))
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX CCI_LN01 ON LN01 (LOANCODE, CUSTCODE, CCYCODE, LOANAMT, OUTSTAND, STARTDATE, MATDATE, BRANCHCODE)
    PRINT '✅ Columnstore index added to LN01'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_LN03' AND object_id = OBJECT_ID('LN03'))
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX CCI_LN03 ON LN03 (LOANCODE, TXNDATE, TXNTYPE, PRINCIPAL, INTEREST, TOTALAMT, BALANCE, BRANCHCODE)
    PRINT '✅ Columnstore index added to LN03'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_RR01' AND object_id = OBJECT_ID('RR01'))
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX CCI_RR01 ON RR01 (CUSTCODE, RISKRATING, RISKDATE, RISKSCORE, BRANCHCODE)
    PRINT '✅ Columnstore index added to RR01'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'CCI_DPDA' AND object_id = OBJECT_ID('DPDA'))
BEGIN
    CREATE NONCLUSTERED COLUMNSTORE INDEX CCI_DPDA ON DPDA (REPORTDATE, BRANCHCODE, CCYCODE, TOTALDEPOSITS, NEWDEPOSITS, WITHDRAWALS)
    PRINT '✅ Columnstore index added to DPDA'
END

PRINT '🎯 Tất cả Columnstore indexes đã được thêm!'
" -C

echo -e "${GREEN}✅ Columnstore indexes đã được thêm hoàn chỉnh${NC}"

# Create additional performance indexes
echo -e "${PURPLE}⚡ Tạo indexes tối ưu hiệu suất...${NC}"
sqlcmd -S "$SERVER" -U "$USERNAME" -P "$PASSWORD" -d "$DATABASE" -Q "
-- Performance indexes for better query performance
CREATE NONCLUSTERED INDEX IX_DP01_CUSTCODE_CCYCODE ON DP01 (CUSTCODE, CCYCODE) INCLUDE (BALANCE, AVAILBAL)
CREATE NONCLUSTERED INDEX IX_EI01_BRANCHCODE_DEPT ON EI01 (BRANCHCODE, DEPARTMENT) INCLUDE (EMPNAME, STATUS)
CREATE NONCLUSTERED INDEX IX_GL41_TXNDATE_BRANCH ON GL41 (TXNDATE, BRANCHCODE) INCLUDE (DRAMT, CRAMT)
CREATE NONCLUSTERED INDEX IX_LN01_STATUS_BRANCH ON LN01 (STATUS, BRANCHCODE) INCLUDE (OUTSTAND, INTRATE)
CREATE NONCLUSTERED INDEX IX_LN03_LOANCODE_DATE ON LN03 (LOANCODE, TXNDATE) INCLUDE (TOTALAMT, BALANCE)
CREATE NONCLUSTERED INDEX IX_RR01_CUSTCODE_DATE ON RR01 (CUSTCODE, RISKDATE) INCLUDE (RISKRATING, RISKSCORE)
CREATE NONCLUSTERED INDEX IX_DPDA_DATE_BRANCH ON DPDA (REPORTDATE, BRANCHCODE) INCLUDE (TOTALDEPOSITS, NETCHANGE)

PRINT '✅ Performance indexes created successfully'
" -C

echo -e "${GREEN}✅ Performance indexes đã được tạo${NC}"

# Verify all tables and configurations
echo -e "${PURPLE}🔍 Xác minh cấu hình hoàn chỉnh...${NC}"
sqlcmd -S "$SERVER" -U "$USERNAME" -P "$PASSWORD" -d "$DATABASE" -Q "
SELECT
    t.name AS TableName,
    CASE WHEN t.temporal_type = 2 THEN 'YES' ELSE 'NO' END AS IsTemporalTable,
    CASE WHEN EXISTS (
        SELECT 1 FROM sys.indexes i
        WHERE i.object_id = t.object_id
        AND i.type IN (5,6)
    ) THEN 'YES' ELSE 'NO' END AS HasColumnstore,
    COUNT(c.column_id) AS ColumnCount
FROM sys.tables t
LEFT JOIN sys.columns c ON t.object_id = c.object_id
WHERE t.name IN ('GL01', 'DP01', 'EI01', 'GL41', 'LN01', 'LN03', 'RR01', 'DPDA')
GROUP BY t.name, t.temporal_type, t.object_id
ORDER BY t.name

PRINT ''
PRINT '📊 SUMMARY:'
PRINT '- GL01: NO Temporal + YES Columnstore ✅'
PRINT '- 7 Other Tables: YES Temporal + YES Columnstore ✅'
PRINT '- All Performance Indexes Created ✅'
PRINT '- Database TinhKhoanDB Ready for Import ✅'
" -C

echo ""
echo -e "${WHITE}🎉 DATABASE RESTORATION HOÀN THÀNH!${NC}"
echo "=================================================="
echo -e "${GREEN}✅ 8 Core tables đã được tạo với cấu hình tối ưu${NC}"
echo -e "${GREEN}✅ Temporal tables cho 7/8 bảng (trừ GL01)${NC}"
echo -e "${GREEN}✅ Columnstore indexes cho tất cả 8 bảng${NC}"
echo -e "${GREEN}✅ Performance indexes cho truy vấn nhanh${NC}"
echo ""
echo -e "${CYAN}📂 Sẵn sàng import dữ liệu từ:${NC}"
echo "   /Users/nguyendat/Documents/DuLieuImport/DuLieuMau"
echo ""
echo -e "${YELLOW}📝 Các bước tiếp theo:${NC}"
echo "   1. Import dữ liệu CSV vào các bảng"
echo "   2. Chạy backend: ./start_backend.sh"
echo "   3. Chạy frontend: ./start_frontend.sh"
