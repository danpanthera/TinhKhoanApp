#!/bin/bash

echo "🎯 RECREATE APPROACH - Tạo lại bảng với Temporal từ đầu"
echo "======================================================"
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Test với một bảng nhỏ trước: DPDA
table="DPDA"

echo "🔧 RECREATE APPROACH for table: $table"
echo "======================================="

# Step 1: Backup existing data
echo "Step 1: Backup existing data..."
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT * INTO ${table}_BACKUP FROM $table;
"

if [ $? -eq 0 ]; then
    echo -e "  ✅ ${GREEN}Data backed up to ${table}_BACKUP${NC}"
else
    echo -e "  ❌ ${RED}Failed to backup data${NC}"
    exit 1
fi

# Step 2: Drop original table
echo "Step 2: Drop original table..."
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    DROP TABLE IF EXISTS ${table}_History;
    DROP TABLE $table;
"

if [ $? -eq 0 ]; then
    echo -e "  ✅ ${GREEN}Original table dropped${NC}"
else
    echo -e "  ❌ ${RED}Failed to drop original table${NC}"
    exit 1
fi

# Step 3: Recreate table with temporal structure from scratch
echo "Step 3: Recreate table with temporal from scratch..."
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    -- Create new temporal table with all original columns plus temporal
    CREATE TABLE $table (
        Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        NGAY_DL date NULL,
        CREATED_DATE datetime2(7) NULL,
        UPDATED_DATE datetime2(7) NULL,
        FILE_NAME nvarchar(255) NULL,

        -- Add all business columns (DPDA specific - 13 columns)
        ACCOUNTNO nvarchar(255) NULL,
        CURRENCY nvarchar(255) NULL,
        CUSTOMER nvarchar(255) NULL,
        ACCTOPENINGDATE nvarchar(255) NULL,
        CURRENTBALANCE nvarchar(255) NULL,
        CLEAREDBALANCE nvarchar(255) NULL,
        PRODUCTTYPE nvarchar(255) NULL,
        PRODUCTGROUP nvarchar(255) NULL,
        STATUS nvarchar(255) NULL,
        PRODUCTCLASS nvarchar(255) NULL,
        SUBGROUP nvarchar(255) NULL,
        GROUPDESCRIPTION nvarchar(255) NULL,
        CO_CODE nvarchar(255) NULL,

        -- Temporal columns
        ValidFrom datetime2(7) GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME(),
        ValidTo datetime2(7) GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(datetime2(7), '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.${table}_History));
"

if [ $? -eq 0 ]; then
    echo -e "  🎉 ${GREEN}Table recreated with temporal successfully!${NC}"
else
    echo -e "  ❌ ${RED}Failed to recreate temporal table${NC}"
    exit 1
fi

# Step 4: Restore data (excluding temporal columns)
echo "Step 4: Restore data..."
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SET IDENTITY_INSERT $table ON;

    INSERT INTO $table (
        Id, NGAY_DL, CREATED_DATE, UPDATED_DATE, FILE_NAME,
        ACCOUNTNO, CURRENCY, CUSTOMER, ACCTOPENINGDATE, CURRENTBALANCE,
        CLEAREDBALANCE, PRODUCTTYPE, PRODUCTGROUP, STATUS, PRODUCTCLASS,
        SUBGROUP, GROUPDESCRIPTION, CO_CODE
    )
    SELECT
        Id, NGAY_DL, CREATED_DATE, UPDATED_DATE, FILE_NAME,
        ACCOUNTNO, CURRENCY, CUSTOMER, ACCTOPENINGDATE, CURRENTBALANCE,
        CLEAREDBALANCE, PRODUCTTYPE, PRODUCTGROUP, STATUS, PRODUCTCLASS,
        SUBGROUP, GROUPDESCRIPTION, CO_CODE
    FROM ${table}_BACKUP;

    SET IDENTITY_INSERT $table OFF;
"

if [ $? -eq 0 ]; then
    echo -e "  ✅ ${GREEN}Data restored successfully${NC}"
else
    echo -e "  ❌ ${RED}Failed to restore data${NC}"
    exit 1
fi

# Step 5: Add columnstore index
echo "Step 5: Add columnstore index..."
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    CREATE NONCLUSTERED COLUMNSTORE INDEX IX_${table}_Columnstore
    ON $table (
        NGAY_DL, CREATED_DATE, UPDATED_DATE, FILE_NAME,
        ACCOUNTNO, CURRENCY, CUSTOMER, ACCTOPENINGDATE, CURRENTBALANCE,
        CLEAREDBALANCE, PRODUCTTYPE, PRODUCTGROUP, STATUS, PRODUCTCLASS,
        SUBGROUP, GROUPDESCRIPTION, CO_CODE
    );
"

if [ $? -eq 0 ]; then
    echo -e "  ✅ ${GREEN}Columnstore index added successfully${NC}"
else
    echo -e "  ❌ ${RED}Failed to add columnstore index${NC}"
fi

# Step 6: Verify
echo "Step 6: Verify temporal setup..."
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT
        name as TableName,
        temporal_type_desc as TemporalType,
        CASE WHEN temporal_type = 2 THEN 'YES' ELSE 'NO' END as TemporalEnabled
    FROM sys.tables
    WHERE name = '$table';
"

echo ""
echo "Step 7: Check columnstore..."
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT
        t.name as TableName,
        COUNT(i.index_id) as ColumnstoreCount,
        CASE WHEN COUNT(i.index_id) > 0 THEN 'YES' ELSE 'NO' END as ColumnstoreEnabled
    FROM sys.tables t
    LEFT JOIN sys.indexes i ON t.object_id = i.object_id AND i.type IN (5,6)
    WHERE t.name = '$table'
    GROUP BY t.name;
"

echo ""
echo "Step 8: Check record count..."
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT COUNT(*) as RecordCount FROM $table;
"

echo ""
echo "🎯 Recreate approach test completed for $table"
echo "=============================================="
echo "Completed at: $(date '+%Y-%m-%d %H:%M:%S')"
