#!/bin/bash

echo "🔧 CORRECT TEMPORAL TABLES SETUP - AZURE SQL EDGE"
echo "================================================="
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# Test with DP01 first
TABLE="DP01"

echo "🎯 TESTING CORRECT APPROACH WITH $TABLE"
echo "======================================="

echo "🔍 Step 1: Add normal datetime2 columns first"
echo "---------------------------------------------"
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    ALTER TABLE $TABLE ADD
        ValidFrom datetime2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        ValidTo datetime2(7) NOT NULL DEFAULT CONVERT(datetime2, '9999-12-31 23:59:59.9999999')
"

if [ $? -eq 0 ]; then
    echo -e "✅ ${GREEN}Basic datetime2 columns added${NC}"
else
    echo -e "❌ ${RED}Failed to add basic columns${NC}"
fi

echo ""
echo "🔍 Step 2: Check columns were added"
echo "----------------------------------"
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = '$TABLE'
    AND COLUMN_NAME IN ('ValidFrom', 'ValidTo')
    ORDER BY COLUMN_NAME
"

echo ""
echo "🔍 Step 3: Add PERIOD FOR SYSTEM_TIME"
echo "-------------------------------------"
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    ALTER TABLE $TABLE ADD PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
"

if [ $? -eq 0 ]; then
    echo -e "✅ ${GREEN}SYSTEM_TIME period added${NC}"
else
    echo -e "❌ ${RED}Failed to add SYSTEM_TIME period${NC}"
fi

echo ""
echo "🔍 Step 4: Create History Table (if not exists)"
echo "----------------------------------------------"
# Check if history table exists
history_exists=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '${TABLE}_History'
" -h -1 2>/dev/null | sed 's/[^0-9]*//g')

if [ "$history_exists" = "0" ]; then
    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        SELECT * INTO ${TABLE}_History FROM $TABLE WHERE 1=0
    "

    if [ $? -eq 0 ]; then
        echo -e "✅ ${GREEN}History table created${NC}"
    else
        echo -e "❌ ${RED}Failed to create history table${NC}"
    fi
else
    echo -e "ℹ️  ${YELLOW}History table already exists${NC}"
fi

echo ""
echo "🔍 Step 5: Enable System Versioning"
echo "-----------------------------------"
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    ALTER TABLE $TABLE SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.${TABLE}_History))
"

if [ $? -eq 0 ]; then
    echo -e "✅ ${GREEN}System versioning enabled${NC}"
else
    echo -e "❌ ${RED}Failed to enable system versioning${NC}"
fi

echo ""
echo "🔍 Step 6: Final Verification"
echo "-----------------------------"
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT
        t.name as TableName,
        t.temporal_type_desc as TemporalType,
        h.name as HistoryTable,
        CASE WHEN t.temporal_type_desc = 'SYSTEM_VERSIONED_TEMPORAL_TABLE' THEN 'YES' ELSE 'NO' END as IsEnabled
    FROM sys.tables t
    LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
    WHERE t.name = '$TABLE'
"

echo ""
echo "🔍 Step 7: Test Temporal Functionality"
echo "--------------------------------------"
echo "Testing by inserting and updating a record..."

# Get count before
before_count=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM ${TABLE}_History" -h -1 2>/dev/null | sed 's/[^0-9]*//g')

echo "History records before test: $before_count"

# Insert test record
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    INSERT INTO $TABLE (NGAY_DL, CREATED_DATE, UPDATED_DATE, FILE_NAME)
    VALUES ('2025-07-15', GETDATE(), GETDATE(), 'temporal_test.csv')
" > /dev/null 2>&1

# Update the record
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    UPDATE $TABLE SET FILE_NAME = 'temporal_test_updated.csv'
    WHERE FILE_NAME = 'temporal_test.csv'
" > /dev/null 2>&1

# Get count after
after_count=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM ${TABLE}_History" -h -1 2>/dev/null | sed 's/[^0-9]*//g')

echo "History records after test: $after_count"

if [ "$after_count" -gt "$before_count" ]; then
    echo -e "✅ ${GREEN}Temporal functionality working - history records captured!${NC}"
else
    echo -e "❌ ${RED}Temporal functionality may not be working${NC}"
fi

# Clean up test record
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    DELETE FROM $TABLE WHERE FILE_NAME = 'temporal_test_updated.csv'
" > /dev/null 2>&1

echo ""
echo -e "📋 ${BLUE}Temporal setup test completed for $TABLE${NC}"
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
