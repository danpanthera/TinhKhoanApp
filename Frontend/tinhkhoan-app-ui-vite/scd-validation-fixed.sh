#!/bin/bash
# SCD Endpoints Validation Script - Fixed Version

API_BASE="http://localhost:5055/api"
DATETIME=$(date '+%Y-%m-%d %H:%M:%S')

echo "🔍 SCD Endpoints Validation Test Suite (Fixed)"
echo "=============================================="
echo "Started at: $DATETIME"
echo ""

# Function to make API calls and parse responses
make_api_call() {
    local url=$1
    local method=${2:-GET}
    
    if [ "$method" = "POST" ]; then
        response=$(curl -s -w "HTTPSTATUS:%{http_code}" -X POST "$url" 2>/dev/null)
    else
        response=$(curl -s -w "HTTPSTATUS:%{http_code}" "$url" 2>/dev/null)
    fi
    
    http_code=$(echo "$response" | tr -d '\n' | sed -e 's/.*HTTPSTATUS://')
    response_body=$(echo "$response" | sed -e 's/HTTPSTATUS:.*//g')
    
    echo "HTTP_CODE:$http_code"
    echo "RESPONSE_BODY:$response_body"
}

# Test 1: Check SCD Current endpoint
echo "📡 Test 1: SCD Current Records Check..."
result=$(make_api_call "$API_BASE/RawData/scd/current?pageSize=5")
http_code=$(echo "$result" | grep "HTTP_CODE:" | cut -d: -f2)
response_body=$(echo "$result" | grep "RESPONSE_BODY:" | cut -d: -f2-)

if [ "$http_code" = "200" ]; then
    echo "✅ SCD Current endpoint working (HTTP $http_code)"
    record_count=$(echo "$response_body" | grep -o '"sourceId"' | wc -l | tr -d ' ')
    echo "   Current SCD records: $record_count"
    
    if [ "$record_count" -gt "0" ]; then
        echo "   📋 Sample record structure:"
        echo "$response_body" | head -c 500 | tail -c +1
        echo "..."
    else
        echo "   ℹ️  No current SCD records found (this is normal for a fresh system)"
    fi
else
    echo "❌ SCD Current endpoint failed (HTTP $http_code)"
    echo "   Error: $response_body"
fi

echo ""

# Test 2: Check DataImport endpoint
echo "📋 Test 2: DataImport Endpoint Check..."
result=$(make_api_call "$API_BASE/DataImport")
http_code=$(echo "$result" | grep "HTTP_CODE:" | cut -d: -f2)
response_body=$(echo "$result" | grep "RESPONSE_BODY:" | cut -d: -f2-)

if [ "$http_code" = "200" ]; then
    echo "✅ DataImport endpoint working (HTTP $http_code)"
    import_count=$(echo "$response_body" | grep -o '"importId"' | wc -l | tr -d ' ')
    echo "   Available imports: $import_count"
    
    if [ "$import_count" -gt "0" ]; then
        # Extract first import ID more carefully
        first_import_id=$(echo "$response_body" | grep -o '"importId":[0-9]*' | head -1 | grep -o '[0-9]*')
        echo "   First import ID: $first_import_id"
    fi
else
    echo "❌ DataImport endpoint failed (HTTP $http_code)"
    echo "   Error: $response_body"
    
    # Let's try a direct test with a known import ID
    echo "   🔧 Trying direct test with known import IDs..."
    for test_id in 13 14 15 16 17 18 19 20; do
        echo "   Testing import ID: $test_id"
        result=$(make_api_call "$API_BASE/RawData/scd/upsert/$test_id" "POST")
        http_code=$(echo "$result" | grep "HTTP_CODE:" | cut -d: -f2)
        
        if [ "$http_code" = "200" ]; then
            echo "   ✅ Found working import ID: $test_id"
            first_import_id=$test_id
            break
        elif [ "$http_code" = "404" ]; then
            echo "   ⚠️  Import $test_id not found"
        else
            response_body=$(echo "$result" | grep "RESPONSE_BODY:" | cut -d: -f2-)
            echo "   ❌ Import $test_id failed (HTTP $http_code): $response_body"
        fi
    done
fi

echo ""

# Test 3: Test SCD Current with specific filters
echo "🔍 Test 3: SCD Current with Branch Filter..."
result=$(make_api_call "$API_BASE/RawData/scd/current?branchCode=7800&pageSize=10")
http_code=$(echo "$result" | grep "HTTP_CODE:" | cut -d: -f2)
response_body=$(echo "$result" | grep "RESPONSE_BODY:" | cut -d: -f2-)

if [ "$http_code" = "200" ]; then
    filtered_count=$(echo "$response_body" | grep -o '"sourceId"' | wc -l | tr -d ' ')
    echo "✅ Branch filter test successful"
    echo "   Records for branch 7800: $filtered_count"
else
    echo "❌ Branch filter test failed (HTTP $http_code)"
fi

echo ""

# Test 4: Test SCD Upsert if we have an import ID
if [ -n "$first_import_id" ]; then
    echo "⚙️  Test 4: SCD Upsert with Import $first_import_id..."
    
    # Get current count
    result=$(make_api_call "$API_BASE/RawData/scd/current")
    current_count=$(echo "$result" | grep "RESPONSE_BODY:" | cut -d: -f2- | grep -o '"sourceId"' | wc -l | tr -d ' ')
    echo "   Records before upsert: $current_count"
    
    # Perform upsert
    result=$(make_api_call "$API_BASE/RawData/scd/upsert/$first_import_id" "POST")
    http_code=$(echo "$result" | grep "HTTP_CODE:" | cut -d: -f2)
    response_body=$(echo "$result" | grep "RESPONSE_BODY:" | cut -d: -f2-)
    
    if [ "$http_code" = "200" ]; then
        echo "✅ SCD upsert successful"
        echo "   Response: $response_body"
        
        # Check count after upsert
        sleep 2
        result=$(make_api_call "$API_BASE/RawData/scd/current")
        new_count=$(echo "$result" | grep "RESPONSE_BODY:" | cut -d: -f2- | grep -o '"sourceId"' | wc -l | tr -d ' ')
        echo "   Records after upsert: $new_count"
        
        if [ "$new_count" -gt "$current_count" ]; then
            echo "✅ Record count increased by $((new_count - current_count)) records"
        elif [ "$new_count" -eq "$current_count" ]; then
            echo "ℹ️  Record count unchanged (no new records or all were updates)"
        else
            echo "⚠️  Unexpected record count change"
        fi
    else
        echo "❌ SCD upsert failed (HTTP $http_code)"
        echo "   Error: $response_body"
    fi
else
    echo "⚠️  Test 4: Skipped - No valid import ID found"
fi

echo ""

# Test 5: Test Record Versions (if we have records)
echo "📈 Test 5: Record Versions Test..."
result=$(make_api_call "$API_BASE/RawData/scd/current?pageSize=1")
response_body=$(echo "$result" | grep "RESPONSE_BODY:" | cut -d: -f2-)
source_id=$(echo "$response_body" | grep -o '"sourceId":"[^"]*"' | head -1 | cut -d'"' -f4)

if [ -n "$source_id" ]; then
    echo "   Testing versions for source ID: $source_id"
    result=$(make_api_call "$API_BASE/RawData/scd/versions/$source_id")
    http_code=$(echo "$result" | grep "HTTP_CODE:" | cut -d: -f2)
    response_body=$(echo "$result" | grep "RESPONSE_BODY:" | cut -d: -f2-)
    
    if [ "$http_code" = "200" ]; then
        version_count=$(echo "$response_body" | grep -o '"version"' | wc -l | tr -d ' ')
        echo "✅ Versions endpoint working - $version_count versions found"
    else
        echo "❌ Versions endpoint failed (HTTP $http_code)"
        echo "   Error: $response_body"
    fi
else
    echo "⚠️  Skipped - No source ID available for testing"
fi

echo ""

# Final Summary
echo "📊 Validation Summary"
echo "===================="
echo "✅ Test completed at: $(date '+%Y-%m-%d %H:%M:%S')"
echo "🔗 API Base URL: $API_BASE"

# Check overall status
result=$(make_api_call "$API_BASE/RawData/scd/current")
final_count=$(echo "$result" | grep "RESPONSE_BODY:" | cut -d: -f2- | grep -o '"sourceId"' | wc -l | tr -d ' ')
echo "📊 Final SCD record count: $final_count"

echo ""
echo "🎯 Next Steps:"
echo "   1. Open the web validation suite: file:///Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite/scd-validation-tests.html"
echo "   2. Use the detailed test page: file:///Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite/test-scd-endpoints.html"
echo "   3. Check backend logs for detailed processing information"
echo ""
