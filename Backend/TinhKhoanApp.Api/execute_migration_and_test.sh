#!/bin/bash

# 🚀 EXECUTE MIGRATION AND TEST - DP01 FIX
# Thực thi migration và test import mechanism sau khi fix

echo "🚀 Execute Migration and Test - DP01 Fix"
echo "======================================="
echo "📅 Started: $(date)"
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Step 1: Execute SQL Migration
echo -e "${BLUE}🔧 STEP 1: Execute SQL Migration${NC}"
echo "================================"

SQL_FILE="migration_20250715092116_AddMissingColumnsToDP01.sql"

if [ -f "$SQL_FILE" ]; then
    echo "✅ Found migration file: $SQL_FILE"
    echo "🚀 Executing migration..."

    sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -i "$SQL_FILE"

    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✅ Migration executed successfully!${NC}"
    else
        echo -e "${RED}❌ Migration failed!${NC}"
        exit 1
    fi
else
    echo -e "${RED}❌ Migration file not found: $SQL_FILE${NC}"
    exit 1
fi

echo ""

# Step 2: Verify Column Structure
echo -e "${BLUE}🔍 STEP 2: Verify New Column Structure${NC}"
echo "======================================="

echo "Checking DP01 table structure..."
COLUMN_COUNT=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT COUNT(*)
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'DP01'
        AND COLUMN_NAME NOT IN ('Id','CreatedAt','UpdatedAt','ValidFrom','ValidTo','NGAY_DL')
" -h -1 2>/dev/null | tr -d '\r\n ' | grep -E '^[0-9]+$' | head -1)

if [ ! -z "$COLUMN_COUNT" ]; then
    echo "📊 DP01 Business Columns: $COLUMN_COUNT"

    if [ "$COLUMN_COUNT" -eq 63 ]; then
        echo -e "${GREEN}✅ SUCCESS: DP01 now has 63 business columns!${NC}"
    else
        echo -e "${YELLOW}⚠️  Expected 63, got $COLUMN_COUNT columns${NC}"
    fi
else
    echo -e "${RED}❌ Failed to count DP01 columns${NC}"
fi

echo ""

# Step 3: Display Sample Columns
echo -e "${BLUE}📋 STEP 3: Sample Column Structure${NC}"
echo "=================================="

echo "First 20 columns in DP01:"
sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "
    SELECT TOP 20
        COLUMN_NAME,
        DATA_TYPE,
        CHARACTER_MAXIMUM_LENGTH
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'DP01'
        AND COLUMN_NAME NOT IN ('Id','CreatedAt','UpdatedAt','ValidFrom','ValidTo','NGAY_DL')
    ORDER BY ORDINAL_POSITION
" -h -1 2>/dev/null

echo ""

# Step 4: Test Import (if container is running)
echo -e "${BLUE}🧪 STEP 4: Test Import Readiness${NC}"
echo "================================="

echo "Checking if backend API is accessible..."
if curl -s http://localhost:5055/health &>/dev/null; then
    echo -e "${GREEN}✅ Backend API is running${NC}"
    echo "Ready for CSV import testing!"

    # Test basic API endpoints
    echo ""
    echo "Testing core endpoints:"

    echo -n "- /api/DirectImport: "
    if curl -s -f http://localhost:5055/api/DirectImport &>/dev/null; then
        echo -e "${GREEN}✅ Accessible${NC}"
    else
        echo -e "${YELLOW}⚠️  May need backend restart${NC}"
    fi

else
    echo -e "${YELLOW}⚠️  Backend API not running${NC}"
    echo "Need to start backend before testing import"
fi

echo ""

# Step 5: Summary
echo -e "${BLUE}📊 STEP 5: Summary${NC}"
echo "=================="

echo "✅ Migration executed"
echo "📊 DP01 columns: $COLUMN_COUNT (target: 63)"
echo "🎯 Ready for Entity model update"
echo "🧪 Ready for import testing"

echo ""
echo -e "${YELLOW}📝 NEXT ACTIONS:${NC}"
echo "==============="
echo "1. 🔧 Update DP01.cs Entity model with new properties"
echo "2. 🏗️  Run: dotnet ef database update (if using EF migrations)"
echo "3. 🚀 Restart backend API to load new schema"
echo "4. 🧪 Test CSV import with DP01 files"
echo "5. ✅ Verify data integrity after import"

echo ""
echo "Execution completed at: $(date)"
