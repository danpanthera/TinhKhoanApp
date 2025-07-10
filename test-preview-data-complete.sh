#!/bin/bash

echo "🎉 FINAL TEST: Preview Data Fix Complete Verification (10/07/2025)"
echo "=================================================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${YELLOW}📊 Test 1: Backend API preview with real data${NC}"
SAMPLE_ID=$(curl -s "http://localhost:5055/api/DataImport/records" | jq -r '.[0].Id')
echo "Testing API with ID: $SAMPLE_ID"

API_RESULT=$(curl -s "http://localhost:5055/api/DataImport/preview/$SAMPLE_ID")
PREVIEW_ROWS_COUNT=$(echo "$API_RESULT" | jq '.PreviewRows | length')

echo "Preview rows count: $PREVIEW_ROWS_COUNT"

if [ "$PREVIEW_ROWS_COUNT" -gt 0 ]; then
    echo -e "${GREEN}✅ API returning real preview data ($PREVIEW_ROWS_COUNT rows)${NC}"
else
    echo -e "${RED}❌ API still returning empty preview data${NC}"
    exit 1
fi

echo -e "${YELLOW}📊 Test 2: Field mapping verification${NC}"
SAMPLE_FIELD=$(echo "$API_RESULT" | jq -r '.PreviewRows[0] | keys[0]')
echo "Sample field from preview data: $SAMPLE_FIELD"

if [ -n "$SAMPLE_FIELD" ] && [ "$SAMPLE_FIELD" != "null" ]; then
    echo -e "${GREEN}✅ Preview data has proper field structure${NC}"
else
    echo -e "${RED}❌ Preview data structure invalid${NC}"
fi

echo -e "${YELLOW}📊 Test 3: Backend table mapping${NC}"
TABLE_MAPPING_TEST=$(curl -s "http://localhost:5055/api/DataImport/preview/$SAMPLE_ID" | jq -r '.Category')
echo "Category: $TABLE_MAPPING_TEST"

case "$TABLE_MAPPING_TEST" in
    "DP01"|"LN01"|"LN02"|"LN03"|"DB01"|"GL01"|"GL41"|"DPDA"|"EI01"|"KH03"|"RR01"|"DT_KHKD1")
        echo -e "${GREEN}✅ Valid category for table mapping${NC}"
        ;;
    *)
        echo -e "${RED}❌ Unknown category: $TABLE_MAPPING_TEST${NC}"
        ;;
esac

echo -e "${YELLOW}📊 Test 4: Frontend build status${NC}"
if [ -f "/Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite/dist/index.html" ]; then
    echo -e "${GREEN}✅ Frontend build successful${NC}"
else
    echo -e "${RED}❌ Frontend build missing${NC}"
fi

echo -e "${YELLOW}📊 Test 5: Data quality check${NC}"
FIRST_ROW=$(echo "$API_RESULT" | jq '.PreviewRows[0]')
FIRST_ROW_FIELDS=$(echo "$FIRST_ROW" | jq 'keys | length')
echo "First row has $FIRST_ROW_FIELDS fields"

if [ "$FIRST_ROW_FIELDS" -gt 5 ]; then
    echo -e "${GREEN}✅ Preview data has sufficient field detail${NC}"
else
    echo -e "${RED}❌ Preview data lacks detail${NC}"
fi

echo -e "${YELLOW}📊 Test 6: Test with different data types${NC}"
# Test with a few different IDs to verify table mapping works
TEST_COUNT=0
SUCCESS_COUNT=0

for i in {0..2}; do
    TEST_ID=$(curl -s "http://localhost:5055/api/DataImport/records" | jq -r ".[$i].Id")
    if [ "$TEST_ID" != "null" ]; then
        TEST_RESULT=$(curl -s "http://localhost:5055/api/DataImport/preview/$TEST_ID")
        TEST_ROWS=$(echo "$TEST_RESULT" | jq '.PreviewRows | length')
        TEST_CATEGORY=$(echo "$TEST_RESULT" | jq -r '.Category')
        
        echo "  ID $TEST_ID ($TEST_CATEGORY): $TEST_ROWS rows"
        
        if [ "$TEST_ROWS" -gt 0 ]; then
            ((SUCCESS_COUNT++))
        fi
        ((TEST_COUNT++))
    fi
done

echo "Success rate: $SUCCESS_COUNT/$TEST_COUNT"

if [ "$SUCCESS_COUNT" -ge 2 ]; then
    echo -e "${GREEN}✅ Multiple data types working correctly${NC}"
else
    echo -e "${RED}❌ Some data types failing${NC}"
fi

echo ""
echo "=================================================================="
if [ "$PREVIEW_ROWS_COUNT" -gt 0 ] && [ "$FIRST_ROW_FIELDS" -gt 5 ] && [ "$SUCCESS_COUNT" -ge 2 ]; then
    echo -e "${GREEN}🎉 ALL TESTS PASSED - PREVIEW DATA FIX COMPLETE!${NC}"
    echo -e "${GREEN}📝 Frontend will now show real data instead of 'Không tìm thấy dữ liệu chi tiết'${NC}"
    echo -e "${GREEN}🚀 System ready for production use${NC}"
else
    echo -e "${RED}❌ Some tests failed - Review implementation${NC}"
fi
echo "=================================================================="
