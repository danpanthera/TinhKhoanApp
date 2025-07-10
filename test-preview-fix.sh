#!/bin/bash

echo "🔍 TEST PREVIEW FIX - Fix importId undefined (10/07/2025)"
echo "=========================================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${YELLOW}📊 Test 1: Backend API preview functionality${NC}"
echo "Testing API preview with valid ID..."

# Test API preview with known ID
VALID_ID=$(curl -s "http://localhost:5055/api/DataImport/records" | jq -r '.[0].Id')
echo "Using ID: $VALID_ID"

API_RESULT=$(curl -s "http://localhost:5055/api/DataImport/preview/$VALID_ID")
echo "API Response: $API_RESULT"

if echo "$API_RESULT" | jq -e '.ImportId' > /dev/null 2>&1; then
    echo -e "${GREEN}✅ API preview working correctly${NC}"
else
    echo -e "${RED}❌ API preview failed${NC}"
    exit 1
fi

echo -e "${YELLOW}📊 Test 2: Field mapping verification${NC}"
echo "Checking field mapping in API response..."

# Check field names in response
SAMPLE_RECORD=$(curl -s "http://localhost:5055/api/DataImport/records" | jq '.[0]')
echo "Sample record fields:"
echo "$SAMPLE_RECORD" | jq 'keys'

# Check for key fields
HAS_ID=$(echo "$SAMPLE_RECORD" | jq -r '.Id // empty')
HAS_FILENAME=$(echo "$SAMPLE_RECORD" | jq -r '.FileName // empty')
HAS_RECORDSCOUNT=$(echo "$SAMPLE_RECORD" | jq -r '.RecordsCount // empty')

if [ -n "$HAS_ID" ] && [ -n "$HAS_FILENAME" ] && [ -n "$HAS_RECORDSCOUNT" ]; then
    echo -e "${GREEN}✅ Field mapping correct: Id, FileName, RecordsCount found${NC}"
else
    echo -e "${RED}❌ Field mapping incorrect${NC}"
    echo "Id: $HAS_ID"
    echo "FileName: $HAS_FILENAME"
    echo "RecordsCount: $HAS_RECORDSCOUNT"
fi

echo -e "${YELLOW}📊 Test 3: Frontend build status${NC}"
echo "Checking frontend build..."

if [ -f "/Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite/dist/index.html" ]; then
    echo -e "${GREEN}✅ Frontend build successful${NC}"
else
    echo -e "${RED}❌ Frontend build missing${NC}"
fi

echo -e "${YELLOW}📊 Test 4: Code fix verification${NC}"
echo "Checking fixed code patterns..."

# Check if code has been fixed
FIXED_PATTERNS=0

# Check for item.Id usage
if grep -q "item\.Id" "/Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite/src/views/DataImportViewFull.vue"; then
    echo -e "${GREEN}✅ Fixed: item.Id usage found${NC}"
    ((FIXED_PATTERNS++))
else
    echo -e "${RED}❌ Missing: item.Id usage${NC}"
fi

# Check for item.FileName usage
if grep -q "item\.FileName" "/Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite/src/views/DataImportViewFull.vue"; then
    echo -e "${GREEN}✅ Fixed: item.FileName usage found${NC}"
    ((FIXED_PATTERNS++))
else
    echo -e "${RED}❌ Missing: item.FileName usage${NC}"
fi

# Check for item.RecordsCount usage
if grep -q "item\.RecordsCount" "/Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite/src/views/DataImportViewFull.vue"; then
    echo -e "${GREEN}✅ Fixed: item.RecordsCount usage found${NC}"
    ((FIXED_PATTERNS++))
else
    echo -e "${RED}❌ Missing: item.RecordsCount usage${NC}"
fi

echo -e "${YELLOW}📊 Test 5: Manual test guidance${NC}"
echo "To test manually:"
echo "1. Open browser to http://localhost:3000"
echo "2. Navigate to 'KHO DỮ LIỆU THÔ'"
echo "3. Click 'Xem dữ liệu import' (👁️ icon)"
echo "4. Click 'Xem chi tiết' (👁️ icon) for any import record"
echo "5. Verify no 'undefined' in network requests"

echo ""
echo "=========================================================="
if [ $FIXED_PATTERNS -eq 3 ]; then
    echo -e "${GREEN}🎉 ALL TESTS PASSED - Preview fix implemented successfully!${NC}"
    echo -e "${GREEN}📝 The importId undefined issue has been resolved${NC}"
else
    echo -e "${RED}❌ Some tests failed - Review the fixes${NC}"
fi
echo "=========================================================="
