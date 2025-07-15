#!/bin/bash

echo "🔧 FINAL TEMPORAL SETUP - NO IDENTITY COLUMNS"
echo "=============================================="
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

TABLE="DP01"

echo "🎯 SETTING UP TEMPORAL FOR $TABLE (NO IDENTITY)"
echo "==============================================="

echo "🔍 Step 1: Create History Table without IDENTITY"
echo "------------------------------------------------"
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    -- Get all column definitions except IDENTITY
    CREATE TABLE ${TABLE}_History (
        Id int NOT NULL,
        NGAY_DL date NULL,
        CREATED_DATE datetime2(7) NULL,
        UPDATED_DATE datetime2(7) NULL,
        FILE_NAME nvarchar(255) NULL,
        ValidFrom datetime2(7) NOT NULL,
        ValidTo datetime2(7) NOT NULL
    );
"

if [ $? -eq 0 ]; then
    echo -e "✅ ${GREEN}History table created without IDENTITY${NC}"
else
    echo -e "❌ ${RED}Failed to create history table${NC}"
fi

echo ""
echo "🔍 Step 2: Enable System Versioning"
echo "-----------------------------------"
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    ALTER TABLE $TABLE SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.${TABLE}_History))
"

if [ $? -eq 0 ]; then
    echo -e "✅ ${GREEN}System versioning enabled successfully!${NC}"
else
    echo -e "❌ ${RED}Failed to enable system versioning${NC}"
fi

echo ""
echo "🔍 Step 3: Final Verification"
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
echo "🔍 Step 4: Test Temporal Functionality"
echo "--------------------------------------"

# Get current max ID
max_id=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT ISNULL(MAX(Id), 0) FROM $TABLE" -h -1 2>/dev/null | sed 's/[^0-9]*//g')

# Insert test record with explicit ID
next_id=$((max_id + 1))
echo "Inserting test record with ID: $next_id"

sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SET IDENTITY_INSERT $TABLE ON;
    INSERT INTO $TABLE (Id, NGAY_DL, CREATED_DATE, UPDATED_DATE, FILE_NAME)
    VALUES ($next_id, '2025-07-15', GETDATE(), GETDATE(), 'temporal_test.csv');
    SET IDENTITY_INSERT $TABLE OFF;
" > /dev/null 2>&1

if [ $? -eq 0 ]; then
    echo -e "✅ ${GREEN}Test record inserted${NC}"

    # Wait a moment
    sleep 1

    # Update the record
    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
        UPDATE $TABLE SET FILE_NAME = 'temporal_test_updated.csv'
        WHERE Id = $next_id
    " > /dev/null 2>&1

    if [ $? -eq 0 ]; then
        echo -e "✅ ${GREEN}Test record updated${NC}"

        # Check history
        history_count=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM ${TABLE}_History WHERE Id = $next_id" -h -1 2>/dev/null | sed 's/[^0-9]*//g')

        echo "History records for test ID: $history_count"

        if [ "$history_count" -gt "0" ]; then
            echo -e "🎉 ${GREEN}TEMPORAL TABLES WORKING! History captured successfully!${NC}"
        else
            echo -e "❌ ${RED}No history captured${NC}"
        fi

        # Clean up
        sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
            DELETE FROM $TABLE WHERE Id = $next_id
        " > /dev/null 2>&1

    else
        echo -e "❌ ${RED}Failed to update test record${NC}"
    fi
else
    echo -e "❌ ${RED}Failed to insert test record${NC}"
fi

echo ""
echo -e "🎯 ${BLUE}Temporal setup completed for $TABLE${NC}"
echo "📅 $(date '+%Y-%m-%d %H:%M:%S')"
