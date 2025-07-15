#!/bin/bash

echo "🎯 SINGLE TABLE TEST - DPDA Manual Temporal Setup"
echo "================================================="
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

echo "🔧 Manual setup for DPDA table - Step by step"
echo "=============================================="

# Step 1: Clean up any existing setup
echo "Step 1: Clean up existing setup..."
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    -- Disable system versioning if exists
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DPDA' AND temporal_type = 2)
        ALTER TABLE DPDA SET (SYSTEM_VERSIONING = OFF);

    -- Drop history table
    DROP TABLE IF EXISTS DPDA_History;

    -- Drop period if exists
    IF EXISTS (SELECT * FROM sys.periods WHERE object_id = OBJECT_ID('DPDA'))
        ALTER TABLE DPDA DROP PERIOD FOR SYSTEM_TIME;

    -- Drop constraints on temporal columns
    DECLARE @sql NVARCHAR(MAX) = '';
    SELECT @sql = @sql + 'ALTER TABLE DPDA DROP CONSTRAINT ' + name + ';'
    FROM sys.default_constraints
    WHERE parent_object_id = OBJECT_ID('DPDA')
    AND parent_column_id IN (
        SELECT column_id FROM sys.columns
        WHERE object_id = OBJECT_ID('DPDA')
        AND name IN ('ValidFrom', 'ValidTo')
    );
    EXEC sp_executesql @sql;

    -- Drop temporal columns
    IF COL_LENGTH('DPDA', 'ValidFrom') IS NOT NULL
        ALTER TABLE DPDA DROP COLUMN ValidFrom;
    IF COL_LENGTH('DPDA', 'ValidTo') IS NOT NULL
        ALTER TABLE DPDA DROP COLUMN ValidTo;
"

if [ $? -eq 0 ]; then
    echo -e "  ✅ ${GREEN}Cleanup completed successfully${NC}"
else
    echo -e "  ❌ ${RED}Cleanup failed${NC}"
    exit 1
fi

# Step 2: Add temporal columns with GENERATED ALWAYS
echo "Step 2: Add temporal columns with GENERATED ALWAYS..."
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    ALTER TABLE DPDA ADD
        ValidFrom datetime2(7) GENERATED ALWAYS AS ROW START NOT NULL DEFAULT SYSUTCDATETIME() CONSTRAINT DF_DPDA_ValidFrom DEFAULT SYSUTCDATETIME(),
        ValidTo datetime2(7) GENERATED ALWAYS AS ROW END NOT NULL DEFAULT CONVERT(datetime2(7), '9999-12-31 23:59:59.9999999') CONSTRAINT DF_DPDA_ValidTo DEFAULT CONVERT(datetime2(7), '9999-12-31 23:59:59.9999999');
"

if [ $? -eq 0 ]; then
    echo -e "  ✅ ${GREEN}Temporal columns added successfully${NC}"
else
    echo -e "  ❌ ${RED}Failed to add temporal columns${NC}"
    echo "Trying alternative approach..."

    # Alternative: Add normal columns first, then period, then modify
    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        ALTER TABLE DPDA ADD
            ValidFrom datetime2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
            ValidTo datetime2(7) NOT NULL DEFAULT CONVERT(datetime2(7), '9999-12-31 23:59:59.9999999');
    "

    if [ $? -eq 0 ]; then
        echo -e "  ✅ ${GREEN}Normal columns added, continuing...${NC}"
    else
        echo -e "  ❌ ${RED}Failed to add normal columns${NC}"
        exit 1
    fi
fi

# Step 3: Add SYSTEM_TIME period
echo "Step 3: Add SYSTEM_TIME period..."
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    ALTER TABLE DPDA ADD PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo);
"

if [ $? -eq 0 ]; then
    echo -e "  ✅ ${GREEN}SYSTEM_TIME period added successfully${NC}"
else
    echo -e "  ❌ ${RED}Failed to add SYSTEM_TIME period${NC}"
    exit 1
fi

# Step 4: Create history table
echo "Step 4: Create history table..."
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    -- Create history table with same structure
    SELECT TOP 0 * INTO DPDA_History FROM DPDA;

    -- Create clustered index on history table
    CREATE CLUSTERED INDEX IX_DPDA_History_Period_Columns
    ON DPDA_History (ValidTo, ValidFrom, Id);
"

if [ $? -eq 0 ]; then
    echo -e "  ✅ ${GREEN}History table created successfully${NC}"
else
    echo -e "  ❌ ${RED}Failed to create history table${NC}"
    exit 1
fi

# Step 5: Enable system versioning
echo "Step 5: Enable system versioning..."
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    ALTER TABLE DPDA SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.DPDA_History));
"

if [ $? -eq 0 ]; then
    echo -e "  🎉 ${GREEN}DPDA TEMPORAL TABLE ENABLED SUCCESSFULLY!${NC}"
else
    echo -e "  ❌ ${RED}Failed to enable system versioning${NC}"
    exit 1
fi

# Step 6: Verify
echo "Step 6: Verify temporal setup..."
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT
        name as TableName,
        temporal_type_desc as TemporalType,
        CASE WHEN temporal_type = 2 THEN 'YES' ELSE 'NO' END as TemporalEnabled
    FROM sys.tables
    WHERE name = 'DPDA';
"

echo ""
echo "🎯 Manual test completed for DPDA"
echo "=================================="
echo "Completed at: $(date '+%Y-%m-%d %H:%M:%S')"
