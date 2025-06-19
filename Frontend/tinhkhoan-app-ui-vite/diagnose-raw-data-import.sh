#!/bin/bash
# Raw Data Import Issue Diagnostic Script

API_BASE="http://localhost:5055/api"
DATETIME=$(date '+%Y-%m-%d %H:%M:%S')

echo "🔍 RAW DATA IMPORT DIAGNOSTIC SCRIPT"
echo "====================================="
echo "Started at: $DATETIME"
echo ""

# Function to make API calls and parse responses
make_api_call() {
    local url=$1
    local method=${2:-GET}
    local data=${3:-}
    
    if [ "$method" = "POST" ]; then
        if [ -n "$data" ]; then
            response=$(curl -s -w "HTTPSTATUS:%{http_code}" -X POST -H "Content-Type: application/json" -d "$data" "$url" 2>/dev/null)
        else
            response=$(curl -s -w "HTTPSTATUS:%{http_code}" -X POST "$url" 2>/dev/null)
        fi
    else
        response=$(curl -s -w "HTTPSTATUS:%{http_code}" "$url" 2>/dev/null)
    fi
    
    http_code=$(echo "$response" | tr -d '\n' | sed -e 's/.*HTTPSTATUS://')
    response_body=$(echo "$response" | sed -e 's/HTTPSTATUS:.*//g')
    
    echo "HTTP_CODE:$http_code"
    echo "RESPONSE_BODY:$response_body"
}

# Test 1: Check backend connectivity
echo "📡 Test 1: Backend Connectivity Check..."
result=$(make_api_call "$API_BASE/RawData")
http_code=$(echo "$result" | grep "HTTP_CODE:" | cut -d: -f2)
response_body=$(echo "$result" | grep "RESPONSE_BODY:" | cut -d: -f2-)

if [ "$http_code" = "200" ]; then
    echo "✅ Backend API accessible (HTTP $http_code)"
    import_count=$(echo "$response_body" | grep -o '"id"' | wc -l | tr -d ' ')
    echo "   Current imports in system: $import_count"
else
    echo "❌ Backend API connection failed (HTTP $http_code)"
    echo "   Error: $response_body"
    exit 1
fi

echo ""

# Test 2: Check if server accepts multiple files
echo "📁 Test 2: Multiple File Upload Test..."
temp_dir="/tmp/raw_data_test"
mkdir -p "$temp_dir"

# Create test CSV files
cat > "$temp_dir/LN01_20250617_test1.csv" << 'EOF'
Employee_Code,Full_Name,Department,Salary
EMP001,Nguyen Van A,IT,15000000
EMP002,Tran Thi B,Accounting,12000000
EOF

cat > "$temp_dir/LN01_20250617_test2.csv" << 'EOF'
Employee_Code,Full_Name,Department,Salary
EMP003,Le Van C,HR,11000000
EMP004,Pham Thi D,Marketing,13000000
EOF

# Test single file upload first
echo "   Testing single file upload..."
single_response=$(curl -s -w "HTTPSTATUS:%{http_code}" \
    -X POST "$API_BASE/RawData/import/LN01" \
    -F "Files=@$temp_dir/LN01_20250617_test1.csv" \
    -F "Notes=Single file test" 2>/dev/null)

single_http_code=$(echo "$single_response" | tr -d '\n' | sed -e 's/.*HTTPSTATUS://')
single_body=$(echo "$single_response" | sed -e 's/HTTPSTATUS:.*//g')

if [ "$single_http_code" = "200" ]; then
    echo "   ✅ Single file upload working (HTTP $single_http_code)"
    single_records=$(echo "$single_body" | grep -o '"recordsProcessed":[0-9]*' | grep -o '[0-9]*')
    echo "   Records processed: $single_records"
else
    echo "   ❌ Single file upload failed (HTTP $single_http_code)"
    echo "   Error: $single_body"
fi

# Test multiple file upload
echo "   Testing multiple file upload..."
multi_response=$(curl -s -w "HTTPSTATUS:%{http_code}" \
    -X POST "$API_BASE/RawData/import/LN01" \
    -F "Files=@$temp_dir/LN01_20250617_test1.csv" \
    -F "Files=@$temp_dir/LN01_20250617_test2.csv" \
    -F "Notes=Multiple files test" 2>/dev/null)

multi_http_code=$(echo "$multi_response" | tr -d '\n' | sed -e 's/.*HTTPSTATUS://')
multi_body=$(echo "$multi_response" | sed -e 's/HTTPSTATUS:.*//g')

if [ "$multi_http_code" = "200" ]; then
    echo "   ✅ Multiple file upload working (HTTP $multi_http_code)"
    files_processed=$(echo "$multi_body" | grep -o '"success":true' | wc -l | tr -d ' ')
    echo "   Files processed successfully: $files_processed"
    echo "   Response: $multi_body" | head -c 200
    echo "..."
else
    echo "   ❌ Multiple file upload failed (HTTP $multi_http_code)"
    echo "   Error: $multi_body"
fi

echo ""

# Test 3: Check record completeness
echo "📊 Test 3: Record Completeness Check..."
# Get current imports to check record counts
result=$(make_api_call "$API_BASE/RawData")
response_body=$(echo "$result" | grep "RESPONSE_BODY:" | cut -d: -f2-)

if [ -n "$response_body" ]; then
    echo "   Analyzing import record completeness..."
    
    # Check for any imports with 0 records
    zero_records=$(echo "$response_body" | grep -o '"recordsCount":0' | wc -l | tr -d ' ')
    if [ "$zero_records" -gt "0" ]; then
        echo "   ⚠️  Found $zero_records imports with 0 records"
    else
        echo "   ✅ No imports with 0 records found"
    fi
    
    # Check total imports vs total records
    total_imports=$(echo "$response_body" | grep -o '"id":[0-9]*' | wc -l | tr -d ' ')
    echo "   Total imports: $total_imports"
    
    # Sample recent import details
    echo "   Recent import details:"
    echo "$response_body" | grep -o '"fileName":"[^"]*"' | head -3
else
    echo "   ⚠️  No import data available for analysis"
fi

echo ""

# Test 4: Test archive processing
echo "📦 Test 4: Archive Processing Test..."
# Create a test ZIP file
cd "$temp_dir"
zip -q "LN01_20250617_archive.zip" "LN01_20250617_test1.csv" "LN01_20250617_test2.csv"

if [ -f "LN01_20250617_archive.zip" ]; then
    echo "   Testing ZIP file processing..."
    zip_response=$(curl -s -w "HTTPSTATUS:%{http_code}" \
        -X POST "$API_BASE/RawData/import/LN01" \
        -F "Files=@LN01_20250617_archive.zip" \
        -F "Notes=Archive processing test" 2>/dev/null)
    
    zip_http_code=$(echo "$zip_response" | tr -d '\n' | sed -e 's/.*HTTPSTATUS://')
    zip_body=$(echo "$zip_response" | sed -e 's/HTTPSTATUS:.*//g')
    
    if [ "$zip_http_code" = "200" ]; then
        echo "   ✅ ZIP file processing working (HTTP $zip_http_code)"
        archive_files=$(echo "$zip_body" | grep -o '"success":true' | wc -l | tr -d ' ')
        echo "   Files extracted and processed: $archive_files"
    else
        echo "   ❌ ZIP file processing failed (HTTP $zip_http_code)"
        echo "   Error: $zip_body"
    fi
else
    echo "   ⚠️  Could not create test ZIP file"
fi

echo ""

# Test 5: Check database transaction integrity
echo "🗄️  Test 5: Database Transaction Integrity..."
echo "   Checking current import-record relationship..."

# Get detailed import info
detailed_response=$(curl -s "$API_BASE/RawData" 2>/dev/null)
if echo "$detailed_response" | grep -q '"id"'; then
    # Extract first import ID for detailed check
    first_import_id=$(echo "$detailed_response" | grep -o '"id":[0-9]*' | head -1 | grep -o '[0-9]*')
    
    if [ -n "$first_import_id" ]; then
        echo "   Testing detailed import data for ID: $first_import_id"
        detail_result=$(make_api_call "$API_BASE/RawData/$first_import_id/preview")
        detail_http_code=$(echo "$detail_result" | grep "HTTP_CODE:" | cut -d: -f2)
        
        if [ "$detail_http_code" = "200" ]; then
            echo "   ✅ Import detail retrieval working"
        else
            echo "   ❌ Import detail retrieval failed (HTTP $detail_http_code)"
        fi
    fi
else
    echo "   ℹ️  No imports available for detailed testing"
fi

echo ""

# Test 6: Connection pool and timeout test
echo "⏱️  Test 6: Connection Stability Test..."
echo "   Testing rapid consecutive requests..."

for i in {1..5}; do
    echo -n "   Request $i: "
    rapid_result=$(make_api_call "$API_BASE/RawData")
    rapid_code=$(echo "$rapid_result" | grep "HTTP_CODE:" | cut -d: -f2)
    
    if [ "$rapid_code" = "200" ]; then
        echo "✅ OK"
    else
        echo "❌ Failed (HTTP $rapid_code)"
    fi
    sleep 0.5
done

echo ""

# Cleanup
echo "🧹 Cleanup..."
rm -rf "$temp_dir"
echo "   Temporary files cleaned up"

echo ""

# Final Summary
echo "📊 Diagnostic Summary"
echo "===================="
echo "✅ Test completed at: $(date '+%Y-%m-%d %H:%M:%S')"
echo "🔗 API Base URL: $API_BASE"
echo ""
echo "🎯 Issues to Check:"
echo "   1. Multiple file upload handling in backend controller"
echo "   2. Transaction integrity for batch operations"  
echo "   3. Archive processing completeness"
echo "   4. Record count validation after import"
echo "   5. Frontend timeout settings for large uploads"
echo ""
echo "📝 Recommended Next Steps:"
echo "   1. Check backend logs during multiple file imports"
echo "   2. Verify database transaction scope in ProcessSingleFile"
echo "   3. Add progress tracking for multi-file operations"
echo "   4. Implement retry mechanism for failed imports"
echo ""
