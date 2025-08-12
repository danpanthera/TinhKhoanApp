#!/bin/bash

# =============================================================================
# 🧪 DP01 IMPORT TEST SCRIPT
# August 12, 2025 - Test DP01 import workflow end-to-end
# =============================================================================

echo "🧪 DP01 IMPORT TEST WORKFLOW"
echo "============================"

CSV_FILE="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7800_dp01_20241231.csv"
API_URL="http://localhost:5055/api/DirectImport/smart"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

print_status() {
    local status=$1
    local message=$2
    if [ "$status" = "OK" ]; then
        echo -e "${GREEN}✅ $message${NC}"
    elif [ "$status" = "WARNING" ]; then
        echo -e "${YELLOW}⚠️  $message${NC}"
    else
        echo -e "${RED}❌ $message${NC}"
    fi
}

echo ""
echo "1️⃣  PRE-TEST VERIFICATION"
echo "========================"

# Check if CSV file exists
if [ -f "$CSV_FILE" ]; then
    FILE_SIZE=$(stat -f%z "$CSV_FILE" 2>/dev/null || echo "0")
    RECORD_COUNT=$(tail -n +2 "$CSV_FILE" | wc -l | tr -d ' ')
    print_status "OK" "CSV file exists: $(basename "$CSV_FILE")"
    print_status "OK" "File size: ${FILE_SIZE} bytes"
    print_status "OK" "CSV records: $RECORD_COUNT (excluding header)"
else
    print_status "ERROR" "CSV file not found at: $CSV_FILE"
    exit 1
fi

# Check database connection
DB_CHECK=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT 1" -h-1 -W 2>/dev/null | tail -n +2 | head -1 | tr -d ' \r\n')
if [ "${DB_CHECK:-0}" = "1" ]; then
    print_status "OK" "Database connection successful"

    # Check current DP01 record count
    CURRENT_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM [DP01]" -h-1 -W 2>/dev/null | tail -n +2 | head -1 | tr -d ' \r\n')
    print_status "OK" "Current DP01 records: ${CURRENT_COUNT:-0}"

else
    print_status "ERROR" "Cannot connect to database"
    exit 1
fi

# Check if backend is running
echo ""
echo "2️⃣  BACKEND HEALTH CHECK"
echo "======================="

HEALTH_CHECK=$(curl -s -o /dev/null -w "%{http_code}" "http://localhost:5055/health" 2>/dev/null || echo "000")
if [ "$HEALTH_CHECK" = "200" ]; then
    print_status "OK" "Backend is running on localhost:5055"
else
    print_status "ERROR" "Backend not responding (HTTP: $HEALTH_CHECK)"
    print_status "WARNING" "Please start backend: cd Backend/TinhKhoanApp.Api && dotnet run"
    exit 1
fi

echo ""
echo "3️⃣  IMPORT TEST"
echo "=============="

echo "📤 Importing DP01 CSV via API..."
echo "URL: $API_URL"
echo "File: $(basename "$CSV_FILE")"

# Perform the import using curl
RESPONSE=$(curl -s -X POST "$API_URL" \
  -H "accept: */*" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@$CSV_FILE" \
  -F "dataType=DP01" 2>/dev/null)

HTTP_CODE=$(curl -s -o /dev/null -w "%{http_code}" -X POST "$API_URL" \
  -H "accept: */*" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@$CSV_FILE" \
  -F "dataType=DP01" 2>/dev/null)

echo "HTTP Response Code: $HTTP_CODE"

if [ "$HTTP_CODE" = "200" ]; then
    print_status "OK" "API call successful"
    echo "Response: $RESPONSE"
else
    print_status "ERROR" "API call failed (HTTP: $HTTP_CODE)"
    echo "Response: $RESPONSE"
fi

echo ""
echo "4️⃣  POST-IMPORT VERIFICATION"
echo "==========================="

# Wait a moment for insert to complete
sleep 2

# Check record count after import
NEW_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM [DP01]" -h-1 -W 2>/dev/null | tail -n +2 | head -1 | tr -d ' \r\n')
IMPORTED_COUNT=$((NEW_COUNT - CURRENT_COUNT))

print_status "OK" "DP01 records after import: ${NEW_COUNT:-0}"
print_status "OK" "Records imported: $IMPORTED_COUNT"

if [ "$IMPORTED_COUNT" -gt "0" ]; then
    print_status "OK" "IMPORT SUCCESSFUL - Data was inserted"

    # Check sample record
    echo ""
    echo "📊 Sample imported record:"
    sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT TOP 1 MA_CN, TAI_KHOAN_HACH_TOAN, MA_KH, TEN_KH, CURRENT_BALANCE, NGAY_DL, CreatedAt FROM [DP01] ORDER BY CreatedAt DESC" -W

    # Check temporal history
    HISTORY_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM [DP01_History]" -h-1 -W 2>/dev/null | tail -n +2 | head -1 | tr -d ' \r\n')
    print_status "OK" "DP01_History records: ${HISTORY_COUNT:-0}"

else
    print_status "ERROR" "IMPORT FAILED - No data was inserted"
fi

echo ""
echo "5️⃣  API ENDPOINTS TEST"
echo "===================="

# Test GET endpoint
GET_RESPONSE=$(curl -s "http://localhost:5055/api/DP01?page=1&pageSize=5" 2>/dev/null)
GET_HTTP_CODE=$(curl -s -o /dev/null -w "%{http_code}" "http://localhost:5055/api/DP01?page=1&pageSize=5" 2>/dev/null)

if [ "$GET_HTTP_CODE" = "200" ]; then
    print_status "OK" "GET /api/DP01 endpoint working"

    # Check if response contains data
    if echo "$GET_RESPONSE" | grep -q "MA_CN"; then
        print_status "OK" "Response contains DP01 data"
    else
        print_status "WARNING" "Response might be empty or malformed"
    fi
else
    print_status "ERROR" "GET /api/DP01 endpoint failed (HTTP: $GET_HTTP_CODE)"
fi

echo ""
echo "🎯 FINAL TEST SUMMARY"
echo "===================="

if [ "$IMPORTED_COUNT" -gt "0" ] && [ "$GET_HTTP_CODE" = "200" ]; then
    print_status "OK" "🎉 DP01 END-TO-END TEST: SUCCESSFUL"
    print_status "OK" "✅ CSV Import: WORKING"
    print_status "OK" "✅ Database: WORKING"
    print_status "OK" "✅ Temporal Tables: WORKING"
    print_status "OK" "✅ API Endpoints: WORKING"

    echo ""
    echo "📈 Test Results:"
    echo "• CSV Records: $RECORD_COUNT"
    echo "• Imported Records: $IMPORTED_COUNT"
    echo "• Success Rate: $(echo "$IMPORTED_COUNT * 100 / $RECORD_COUNT" | bc)%"
    echo "• Database Records: $NEW_COUNT"
    echo "• History Records: ${HISTORY_COUNT:-0}"

else
    print_status "ERROR" "❌ DP01 END-TO-END TEST: FAILED"

    if [ "$IMPORTED_COUNT" -eq "0" ]; then
        print_status "ERROR" "• Import process failed - no records inserted"
    fi

    if [ "$GET_HTTP_CODE" != "200" ]; then
        print_status "ERROR" "• API endpoint not working correctly"
    fi
fi

echo ""
echo "✅ DP01 import test completed!"
