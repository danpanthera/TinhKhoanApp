#!/bin/bash

echo "🎯 ========================================"
echo "🎯 KIỂM THỬ CUỐI CÙNG - RAW DATA FIX"
echo "🎯 ========================================"
echo ""

BASE_URL="http://localhost:5055"
SUCCESS_COUNT=0
TOTAL_COUNT=0

# Function to test endpoint
test_endpoint() {
    local name="$1"
    local method="$2"
    local url="$3"
    local expected_status="${4:-200}"
    
    echo "🧪 Test: $name"
    echo "   Command: curl -X $method '$url'"
    
    TOTAL_COUNT=$((TOTAL_COUNT + 1))
    
    if [ "$method" = "GET" ]; then
        response=$(curl -s -w "%{http_code}" "$url")
    else
        response=$(curl -s -w "%{http_code}" -X "$method" "$url")
    fi
    
    status_code="${response: -3}"
    body="${response%???}"
    
    if [ "$status_code" -eq "$expected_status" ]; then
        echo "   ✅ PASS - Status: $status_code"
        if echo "$body" | jq . >/dev/null 2>&1; then
            echo "   ✅ Valid JSON"
            SUCCESS_COUNT=$((SUCCESS_COUNT + 1))
        else
            echo "   ❌ Invalid JSON"
        fi
    else
        echo "   ❌ FAIL - Expected: $expected_status, Got: $status_code"
    fi
    
    # Show response preview
    if echo "$body" | jq . >/dev/null 2>&1; then
        echo "   Response preview:"
        echo "$body" | jq . | head -10
    fi
    echo ""
}

echo "🔍 Kiểm tra backend server..."
if curl -s "$BASE_URL/api/health" >/dev/null 2>&1; then
    echo "✅ Backend server running"
else
    echo "❌ Backend server not responding"
    exit 1
fi
echo ""

echo "🎯 ====== CRITICAL RAW DATA ENDPOINTS ======"
echo ""

# Test the main endpoints that were causing 500 errors
test_endpoint "Dashboard Stats (Main Dashboard)" "GET" "$BASE_URL/api/rawdata/dashboard/stats"
test_endpoint "Clear All Data (Cleanup Operation)" "DELETE" "$BASE_URL/api/rawdata/clear-all"
test_endpoint "Check Duplicate LN01 (Import Validation)" "GET" "$BASE_URL/api/rawdata/check-duplicate/LN01/20250130"
test_endpoint "Check Duplicate DP01 (Import Validation)" "GET" "$BASE_URL/api/rawdata/check-duplicate/DP01/20250130"
test_endpoint "Get Data by Date LN01 (Query Operation)" "GET" "$BASE_URL/api/rawdata/by-date/LN01/20250130"
test_endpoint "Get Data by Date DP01 (Query Operation)" "GET" "$BASE_URL/api/rawdata/by-date/DP01/20250130"
test_endpoint "Get Data by Date Range (Query Operation)" "GET" "$BASE_URL/api/rawdata/by-date-range/LN01?fromDate=20250120&toDate=20250130"
test_endpoint "Get Optimized Records (Performance Query)" "GET" "$BASE_URL/api/rawdata/optimized/records"

echo "🎯 ====== EDGE CASES ======"
echo ""

# Test edge cases
test_endpoint "Invalid Date Format (Should be 400)" "GET" "$BASE_URL/api/rawdata/by-date/LN01/invaliddate" "400"
test_endpoint "Empty Date Range" "GET" "$BASE_URL/api/rawdata/by-date-range/LN01?fromDate=20250130&toDate=20250130"

echo "🎯 ========================================"
echo "🎯 FINAL RESULTS"
echo "🎯 ========================================"
echo ""

if [ $SUCCESS_COUNT -eq $TOTAL_COUNT ]; then
    echo "🎉 ALL TESTS PASSED! ($SUCCESS_COUNT/$TOTAL_COUNT)"
    echo ""
    echo "✅ Raw Data endpoints no longer return 500 errors"
    echo "✅ All responses are valid JSON"
    echo "✅ Frontend will have smooth UX"
    echo "✅ Dashboard operations work correctly"
    echo ""
    echo "🚀 TASK COMPLETED SUCCESSFULLY!"
else
    echo "⚠️  Some tests failed: $SUCCESS_COUNT/$TOTAL_COUNT passed"
    echo ""
    echo "Need to investigate remaining issues..."
fi

echo ""
echo "📋 Quick Manual Test Commands:"
echo "   Dashboard: curl '$BASE_URL/api/rawdata/dashboard/stats' | jq ."
echo "   Clear All: curl -X DELETE '$BASE_URL/api/rawdata/clear-all' | jq ."
echo "   Check Dup: curl '$BASE_URL/api/rawdata/check-duplicate/LN01/20250130' | jq ."
echo ""
