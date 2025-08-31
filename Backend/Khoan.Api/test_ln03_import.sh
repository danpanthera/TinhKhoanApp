#!/bin/bash

# 🧪 LN03 CSV Import Test Script
# Tests CSV import functionality with the actual 7800_ln03_20241231.csv file

GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m'

echo -e "${BLUE}🧪 Starting LN03 CSV Import Test...${NC}"

# Check if API is running
echo -e "${YELLOW}⏳ Checking API health...${NC}"
if curl -s http://localhost:5055/health >/dev/null 2>&1; then
    echo -e "${GREEN}✅ API is running${NC}"
else
    echo -e "${RED}❌ API is not running. Start it first with: dotnet run --urls=http://localhost:5055${NC}"
    exit 1
fi

# Test CSV import endpoint
echo -e "${YELLOW}📄 Testing CSV import with 7800_ln03_20241231.csv...${NC}"
IMPORT_RESPONSE=$(curl -s -X POST "http://localhost:5055/api/LN03/import-csv" \
    -H "Content-Type: multipart/form-data" \
    -F "file=@7800_ln03_20241231.csv" \
    -F "replaceExistingData=false")

if echo "$IMPORT_RESPONSE" | grep -q "Success.*true"; then
    echo -e "${GREEN}✅ CSV Import successful${NC}"
    echo "$IMPORT_RESPONSE" | jq . 2>/dev/null || echo "$IMPORT_RESPONSE"
else
    echo -e "${RED}❌ CSV Import failed${NC}"
    echo "$IMPORT_RESPONSE"
fi

# Test count endpoint
echo -e "${YELLOW}📊 Testing record count...${NC}"
COUNT_RESPONSE=$(curl -s http://localhost:5055/api/LN03/count)
echo "Count response: $COUNT_RESPONSE"

# Test get records endpoint  
echo -e "${YELLOW}📋 Testing get records (first 5)...${NC}"
RECORDS_RESPONSE=$(curl -s "http://localhost:5055/api/LN03?pageSize=5&page=1")
echo "$RECORDS_RESPONSE" | jq . 2>/dev/null || echo "$RECORDS_RESPONSE"

# Test by branch endpoint
echo -e "${YELLOW}🏢 Testing get by branch (7800)...${NC}"
BRANCH_RESPONSE=$(curl -s "http://localhost:5055/api/LN03/by-branch/7800?pageSize=3")
echo "$BRANCH_RESPONSE" | jq . 2>/dev/null || echo "$BRANCH_RESPONSE"

# Test summary endpoint
echo -e "${YELLOW}📈 Testing summary statistics...${NC}"
SUMMARY_RESPONSE=$(curl -s "http://localhost:5055/api/LN03/summary")
echo "$SUMMARY_RESPONSE" | jq . 2>/dev/null || echo "$SUMMARY_RESPONSE"

echo -e "${BLUE}🎉 LN03 CSV Import Test Completed!${NC}"
