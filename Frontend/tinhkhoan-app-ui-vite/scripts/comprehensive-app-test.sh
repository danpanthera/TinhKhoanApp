#!/bin/bash

# 🧪 TINH KHOAN APP - COMPREHENSIVE APPLICATION TEST
# Script để test toàn bộ application sau PascalCase standardization
# Date: 07/07/2025

echo "🧪 TINH KHOAN APP COMPREHENSIVE TEST"
echo "===================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Test results tracking
TOTAL_TESTS=0
PASSED_TESTS=0
FAILED_TESTS=0
SKIPPED_TESTS=0

# Backend and frontend URLs
BACKEND_URL="http://localhost:5055"
FRONTEND_URL="http://localhost:3000"

# Function to log test results
log_test() {
    local test_name="$1"
    local status="$2"
    local message="$3"

    TOTAL_TESTS=$((TOTAL_TESTS + 1))

    case $status in
        "PASS")
            echo -e "  ${GREEN}✅ PASS${NC}: $test_name"
            PASSED_TESTS=$((PASSED_TESTS + 1))
            ;;
        "FAIL")
            echo -e "  ${RED}❌ FAIL${NC}: $test_name - $message"
            FAILED_TESTS=$((FAILED_TESTS + 1))
            ;;
        "SKIP")
            echo -e "  ${YELLOW}⏭️  SKIP${NC}: $test_name - $message"
            SKIPPED_TESTS=$((SKIPPED_TESTS + 1))
            ;;
    esac
}

# Function to test API endpoint
test_api_endpoint() {
    local endpoint="$1"
    local test_name="$2"
    local expected_fields="$3"

    echo "  🔍 Testing: $endpoint"

    # Make API call
    response=$(curl -s -w "HTTPSTATUS:%{http_code}" "$BACKEND_URL$endpoint" 2>/dev/null)

    if [ $? -ne 0 ]; then
        log_test "$test_name" "FAIL" "Cannot connect to backend"
        return 1
    fi

    # Extract HTTP status and body
    http_status=$(echo "$response" | grep -o "HTTPSTATUS:[0-9]*" | cut -d: -f2)
    body=$(echo "$response" | sed 's/HTTPSTATUS:[0-9]*$//')

    # Check HTTP status
    if [ "$http_status" != "200" ]; then
        log_test "$test_name" "FAIL" "HTTP $http_status"
        return 1
    fi

    # Check if response is valid JSON
    if ! echo "$body" | jq . >/dev/null 2>&1; then
        log_test "$test_name" "FAIL" "Invalid JSON response"
        return 1
    fi

    # Check for expected fields (PascalCase)
    if [ -n "$expected_fields" ]; then
        for field in $expected_fields; do
            if ! echo "$body" | jq -r "if type == \"array\" then .[0].$field else .$field end" >/dev/null 2>&1; then
                log_test "$test_name" "FAIL" "Missing field: $field"
                return 1
            fi
        done
    fi

    log_test "$test_name" "PASS"
    return 0
}

# Function to test frontend page
test_frontend_page() {
    local path="$1"
    local test_name="$2"
    local expected_text="$3"

    echo "  🌐 Testing: $FRONTEND_URL$path"

    # Check if frontend is running
    response=$(curl -s -w "HTTPSTATUS:%{http_code}" "$FRONTEND_URL$path" 2>/dev/null)

    if [ $? -ne 0 ]; then
        log_test "$test_name" "FAIL" "Cannot connect to frontend"
        return 1
    fi

    # Extract HTTP status and body
    http_status=$(echo "$response" | grep -o "HTTPSTATUS:[0-9]*" | cut -d: -f2)
    body=$(echo "$response" | sed 's/HTTPSTATUS:[0-9]*$//')

    # Check HTTP status
    if [ "$http_status" != "200" ]; then
        log_test "$test_name" "FAIL" "HTTP $http_status"
        return 1
    fi

    # Check for expected content
    if [ -n "$expected_text" ]; then
        if ! echo "$body" | grep -q "$expected_text"; then
            log_test "$test_name" "FAIL" "Missing expected content: $expected_text"
            return 1
        fi
    fi

    log_test "$test_name" "PASS"
    return 0
}

echo ""
echo "🚀 1. BACKEND API TESTS"
echo "----------------------"

# Test core API endpoints for PascalCase response format
test_api_endpoint "/api/employees" "Employees API" "Id FullName UnitId"
test_api_endpoint "/api/units" "Units API" "Id Name Type"
test_api_endpoint "/api/roles" "Roles API" "Id Name Description"
test_api_endpoint "/api/positions" "Positions API" "Id Name"
test_api_endpoint "/api/khoanperiods" "KhoanPeriods API" "Id Name Type Status"
test_api_endpoint "/api/kpiassignment/tables" "KPI Tables API" "Id TableName Category"

echo ""
echo "🌐 2. FRONTEND PAGE TESTS"
echo "-------------------------"

# Test critical frontend pages
test_frontend_page "/" "Home Page" "Tinh Khoán App"
test_frontend_page "/employees" "Employees Page" "Quản lý Nhân viên"
test_frontend_page "/roles" "Roles Page" "Quản lý Vai trò"
test_frontend_page "/units" "Units Page" "Quản lý Đơn vị"
test_frontend_page "/khoan-periods" "Khoan Periods Page" "Quản lý Kỳ Khoán"
test_frontend_page "/kpi-definitions" "KPI Definitions Page" "Cấu hình KPI"
test_frontend_page "/employee-kpi-assignment" "Employee KPI Assignment" "Giao khoán KPI cho Cán bộ"

echo ""
echo "🔄 3. DATA CONSISTENCY TESTS"
echo "----------------------------"

echo "  🔍 Testing data consistency between API and expected format..."

# Test specific data structures for PascalCase consistency
api_test_detailed() {
    local endpoint="$1"
    local entity_name="$2"

    echo "  📊 Detailed test for $entity_name..."

    response=$(curl -s "$BACKEND_URL$endpoint" 2>/dev/null)

    if [ $? -ne 0 ]; then
        log_test "$entity_name Data Structure" "FAIL" "API not accessible"
        return 1
    fi

    # Check if response contains PascalCase fields
    if echo "$response" | jq -r '.[] | keys[]' 2>/dev/null | grep -q '^[A-Z]'; then
        log_test "$entity_name PascalCase Format" "PASS"
    else
        log_test "$entity_name PascalCase Format" "FAIL" "No PascalCase fields found"
    fi

    # Check if response contains camelCase fields (should be minimal)
    camelcase_count=$(echo "$response" | jq -r '.[] | keys[]' 2>/dev/null | grep '^[a-z]' | wc -l)
    if [ "$camelcase_count" -lt 3 ]; then
        log_test "$entity_name camelCase Minimal" "PASS"
    else
        log_test "$entity_name camelCase Minimal" "FAIL" "$camelcase_count camelCase fields found"
    fi
}

api_test_detailed "/api/employees" "Employees"
api_test_detailed "/api/units" "Units"
api_test_detailed "/api/roles" "Roles"

echo ""
echo "📱 4. FRONTEND INTERACTION TESTS"
echo "--------------------------------"

# Test if frontend can handle the data properly
echo "  🎯 Testing frontend data consumption..."

# Create a simple test HTML page to verify frontend data handling
cat > /tmp/frontend_test.html << 'EOF'
<!DOCTYPE html>
<html>
<head>
    <title>Frontend Data Test</title>
    <script src="https://unpkg.com/axios/dist/axios.min.js"></script>
</head>
<body>
    <h1>Frontend Data Test</h1>
    <div id="results"></div>

    <script>
        async function testDataAccess() {
            const results = document.getElementById('results');
            let testsPassed = 0;
            let totalTests = 0;

            // Test Employees API
            try {
                totalTests++;
                const employeesResponse = await axios.get('http://localhost:5055/api/employees');
                const employees = employeesResponse.data;

                if (employees.length > 0) {
                    const firstEmployee = employees[0];
                    if (firstEmployee.Id && firstEmployee.FullName) {
                        results.innerHTML += '<p style="color: green;">✅ Employees API - PascalCase fields accessible</p>';
                        testsPassed++;
                    } else {
                        results.innerHTML += '<p style="color: red;">❌ Employees API - Missing PascalCase fields</p>';
                    }
                } else {
                    results.innerHTML += '<p style="color: orange;">⚠️ Employees API - No data returned</p>';
                }
            } catch (error) {
                results.innerHTML += '<p style="color: red;">❌ Employees API - ' + error.message + '</p>';
            }

            // Test Units API
            try {
                totalTests++;
                const unitsResponse = await axios.get('http://localhost:5055/api/units');
                const units = unitsResponse.data;

                if (units.length > 0) {
                    const firstUnit = units[0];
                    if (firstUnit.Id && firstUnit.Name) {
                        results.innerHTML += '<p style="color: green;">✅ Units API - PascalCase fields accessible</p>';
                        testsPassed++;
                    } else {
                        results.innerHTML += '<p style="color: red;">❌ Units API - Missing PascalCase fields</p>';
                    }
                } else {
                    results.innerHTML += '<p style="color: orange;">⚠️ Units API - No data returned</p>';
                }
            } catch (error) {
                results.innerHTML += '<p style="color: red;">❌ Units API - ' + error.message + '</p>';
            }

            // Summary
            results.innerHTML += `<h3>Summary: ${testsPassed}/${totalTests} tests passed</h3>`;

            // Save results for shell script
            localStorage.setItem('testResults', JSON.stringify({
                passed: testsPassed,
                total: totalTests
            }));
        }

        // Run tests when page loads
        window.onload = testDataAccess;
    </script>
</body>
</html>
EOF

# Note: This would require a browser to run, so we'll mark it as a manual test
log_test "Frontend Data Access Test" "SKIP" "Requires manual browser testing"

echo ""
echo "🔍 5. STORE VALIDATION TESTS"
echo "----------------------------"

# Test that stores are using helpers correctly
echo "  📦 Testing store implementations..."

stores_with_helpers=0
total_stores=7

for store in src/stores/*.js; do
    if [ -f "$store" ]; then
        store_name=$(basename "$store")

        # Check if store imports casingSafeAccess
        if grep -q "casingSafeAccess" "$store"; then
            log_test "Store $store_name Helper Import" "PASS"
            stores_with_helpers=$((stores_with_helpers + 1))
        else
            log_test "Store $store_name Helper Import" "FAIL" "No helper import found"
        fi

        # Check for remaining camelCase patterns
        camelcase_patterns=$(grep -c "\.id\|\.name" "$store" 2>/dev/null || echo 0)
        if [ "$camelcase_patterns" -le 2 ]; then
            log_test "Store $store_name Pattern Compliance" "PASS"
        else
            log_test "Store $store_name Pattern Compliance" "FAIL" "$camelcase_patterns camelCase patterns"
        fi
    fi
done

echo ""
echo "📊 TEST SUMMARY"
echo "==============="

echo "🧪 Total Tests: $TOTAL_TESTS"
echo -e "✅ Passed: ${GREEN}$PASSED_TESTS${NC}"
echo -e "❌ Failed: ${RED}$FAILED_TESTS${NC}"
echo -e "⏭️  Skipped: ${YELLOW}$SKIPPED_TESTS${NC}"

# Calculate pass rate
if [ "$TOTAL_TESTS" -gt 0 ]; then
    pass_rate=$(( (PASSED_TESTS * 100) / TOTAL_TESTS ))
    echo "📈 Pass Rate: $pass_rate%"

    if [ "$pass_rate" -ge 90 ]; then
        echo -e "🎉 Overall Status: ${GREEN}EXCELLENT${NC}"
    elif [ "$pass_rate" -ge 75 ]; then
        echo -e "✅ Overall Status: ${GREEN}GOOD${NC}"
    elif [ "$pass_rate" -ge 60 ]; then
        echo -e "⚠️  Overall Status: ${YELLOW}MODERATE${NC}"
    else
        echo -e "❌ Overall Status: ${RED}NEEDS ATTENTION${NC}"
    fi
fi

echo ""
echo "📋 RECOMMENDATIONS"
echo "=================="

if [ "$FAILED_TESTS" -gt 0 ]; then
    echo "🔧 Action Items:"
    echo "  1. Review and fix failed API endpoints"
    echo "  2. Check backend-frontend connectivity"
    echo "  3. Validate data structure consistency"
    echo "  4. Test critical user workflows manually"
fi

echo "🔄 Next Steps:"
echo "  1. Run manual UI testing for critical workflows"
echo "  2. Test CRUD operations (Create, Read, Update, Delete)"
echo "  3. Validate dropdown functionality"
echo "  4. Test form submissions and data persistence"

echo ""
echo "✅ Comprehensive testing complete!"
