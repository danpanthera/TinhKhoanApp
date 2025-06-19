#!/bin/bash
# SCD Endpoints Validation Script

API_BASE="http://localhost:5055/api"
DATETIME=$(date '+%Y-%m-%d %H:%M:%S')

echo "🔍 SCD Endpoints Validation Test Suite"
echo "======================================"
echo "Started at: $DATETIME"
echo ""

# Test 1: Check if API is running
echo "📡 Test 1: API Health Check..."
response=$(curl -s -w "\n%{http_code}" "$API_BASE/RawData/scd/current?pageSize=1" 2>/dev/null)
http_code=$(echo "$response" | tail -n1)
response_body=$(echo "$response" | head -n -1)

if [ "$http_code" = "200" ]; then
    echo "✅ API is running (HTTP $http_code)"
    record_count=$(echo "$response_body" | grep -o '"sourceId"' | wc -l | tr -d ' ')
    echo "   Current SCD records available: $record_count"
else
    echo "❌ API is not responding properly (HTTP $http_code)"
    echo "   Response: $response_body"
    exit 1
fi

echo ""

# Test 2: Get available imports
echo "📋 Test 2: Available Imports Check..."
imports_response=$(curl -s -w "\n%{http_code}" "$API_BASE/DataImport" 2>/dev/null)
imports_http_code=$(echo "$imports_response" | tail -n1)
imports_body=$(echo "$imports_response" | head -n -1)

if [ "$imports_http_code" = "200" ]; then
    import_count=$(echo "$imports_body" | grep -o '"importId"' | wc -l | tr -d ' ')
    echo "✅ Found $import_count available imports"
    
    # Get the first import ID for testing
    first_import_id=$(echo "$imports_body" | grep -o '"importId":[0-9]*' | head -1 | grep -o '[0-9]*')
    if [ -n "$first_import_id" ]; then
        echo "   First available import ID: $first_import_id"
    fi
else
    echo "❌ Failed to get imports (HTTP $imports_http_code)"
fi

echo ""

# Test 3: Test SCD current with filters
echo "🔍 Test 3: SCD Current Records with Filters..."
filtered_response=$(curl -s -w "\n%{http_code}" "$API_BASE/RawData/scd/current?branchCode=7800&pageSize=5" 2>/dev/null)
filtered_http_code=$(echo "$filtered_response" | tail -n1)
filtered_body=$(echo "$filtered_response" | head -n -1)

if [ "$filtered_http_code" = "200" ]; then
    filtered_count=$(echo "$filtered_body" | grep -o '"sourceId"' | wc -l | tr -d ' ')
    echo "✅ Filtered query successful - $filtered_count records for branch 7800"
else
    echo "❌ Filtered query failed (HTTP $filtered_http_code)"
fi

echo ""

# Test 4: Test SCD upsert endpoint (if we have an import)
if [ -n "$first_import_id" ]; then
    echo "⚙️  Test 4: SCD Upsert Test with Import $first_import_id..."
    
    # Check current count before upsert
    current_before=$(curl -s "$API_BASE/RawData/scd/current" | grep -o '"sourceId"' | wc -l | tr -d ' ')
    echo "   Records before upsert: $current_before"
    
    # Perform upsert
    upsert_response=$(curl -s -X POST -w "\n%{http_code}" "$API_BASE/RawData/scd/upsert/$first_import_id" 2>/dev/null)
    upsert_http_code=$(echo "$upsert_response" | tail -n1)
    upsert_body=$(echo "$upsert_response" | head -n -1)
    
    if [ "$upsert_http_code" = "200" ]; then
        echo "✅ SCD upsert successful (HTTP $upsert_http_code)"
        echo "   Response: $upsert_body"
        
        # Check count after upsert
        sleep 1
        current_after=$(curl -s "$API_BASE/RawData/scd/current" | grep -o '"sourceId"' | wc -l | tr -d ' ')
        echo "   Records after upsert: $current_after"
        
        if [ "$current_after" -gt "$current_before" ]; then
            echo "✅ SCD records increased as expected"
        else
            echo "ℹ️  Record count unchanged (may indicate no new records or updates)"
        fi
    else
        echo "❌ SCD upsert failed (HTTP $upsert_http_code)"
        echo "   Response: $upsert_body" 
    fi
else
    echo "⚠️  Test 4: Skipped - No import ID available"
fi

echo ""

# Test 5: Test SCD history/versions endpoint
echo "📈 Test 5: SCD History/Versions Test..."
if [ "$record_count" -gt "0" ]; then
    # Get a source ID from current records
    source_id=$(curl -s "$API_BASE/RawData/scd/current?pageSize=1" | grep -o '"sourceId":"[^"]*"' | head -1 | cut -d'"' -f4)
    
    if [ -n "$source_id" ]; then
        echo "   Testing with source ID: $source_id"
        versions_response=$(curl -s -w "\n%{http_code}" "$API_BASE/RawData/scd/versions/$source_id" 2>/dev/null)
        versions_http_code=$(echo "$versions_response" | tail -n1)
        versions_body=$(echo "$versions_response" | head -n -1)
        
        if [ "$versions_http_code" = "200" ]; then
            version_count=$(echo "$versions_body" | grep -o '"version"' | wc -l | tr -d ' ')
            echo "✅ Versions endpoint working - $version_count versions found"
        else
            echo "❌ Versions endpoint failed (HTTP $versions_http_code)"
        fi
    else
        echo "⚠️  Could not extract source ID for testing"
    fi
else
    echo "⚠️  Skipped - No current records available"
fi

echo ""

# Summary
echo "📊 Test Summary"
echo "==============="
echo "✅ Tests completed at: $(date '+%Y-%m-%d %H:%M:%S')"
echo "✅ API Base URL: $API_BASE"
echo "✅ All major SCD endpoints have been tested"
echo ""
echo "🎯 Next Steps:"
echo "   1. Open the web-based validation suite for interactive testing"
echo "   2. Use the detailed test pages for specific endpoint validation"
echo "   3. Monitor the API logs for any issues"
echo ""
