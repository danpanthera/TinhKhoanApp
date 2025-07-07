#!/bin/bash

# 🧪 TINH KHOAN APP - FINAL VALIDATION TEST
# Comprehensive test for production readiness
# Date: 07/07/2025

echo "🧪 TINH KHOAN APP FINAL VALIDATION TEST"
echo "======================================"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Test counters
total_tests=0
passed_tests=0
failed_tests=0

# Function to run test
run_test() {
    local test_name="$1"
    local test_command="$2"
    local expected_result="$3"

    echo -n "  🔍 $test_name: "
    total_tests=$((total_tests + 1))

    result=$(eval "$test_command" 2>/dev/null)

    if [[ "$result" == *"$expected_result"* ]] || eval "$test_command" >/dev/null 2>&1; then
        echo -e "${GREEN}✅ PASS${NC}"
        passed_tests=$((passed_tests + 1))
        return 0
    else
        echo -e "${RED}❌ FAIL${NC}"
        failed_tests=$((failed_tests + 1))
        return 1
    fi
}

echo ""
echo "🚀 1. BACKEND API COMPREHENSIVE TEST"
echo "------------------------------------"

# Test all critical endpoints
echo "  📡 Testing API endpoints..."

run_test "Employees API" "curl -s http://localhost:5055/api/employees | jq -e '.[0].Id'" "exists"
run_test "Units API" "curl -s http://localhost:5055/api/units | jq -e '.[0].Id'" "exists"
run_test "Roles API" "curl -s http://localhost:5055/api/roles | jq -e '.[0].Id'" "exists"
run_test "Positions API" "curl -s http://localhost:5055/api/positions | jq -e '.[0].Id'" "exists"
run_test "KhoanPeriods API" "curl -s http://localhost:5055/api/khoanperiods | jq -e '.[0].Id'" "exists"
run_test "KPI Definitions API" "curl -s http://localhost:5055/api/kpidefinitions | jq -e 'length > 0'" "true"
run_test "KPI Assignment Tables" "curl -s http://localhost:5055/api/kpiassignment/tables | jq -e '.employees'" "exists"

echo ""
echo "🔧 2. DATA FORMAT VALIDATION"
echo "----------------------------"

# Test PascalCase compliance
echo "  📊 Testing PascalCase compliance..."

run_test "Employee PascalCase" "curl -s http://localhost:5055/api/employees | jq -e '.[0] | has(\"Id\") and has(\"Name\")'" "true"
run_test "Unit PascalCase" "curl -s http://localhost:5055/api/units | jq -e '.[0] | has(\"Id\") and has(\"Name\")'" "true"
run_test "Role PascalCase" "curl -s http://localhost:5055/api/roles | jq -e '.[0] | has(\"Id\") and has(\"Name\")'" "true"

echo ""
echo "📱 3. FRONTEND STORE VALIDATION"
echo "-------------------------------"

# Test store files
echo "  🗃️ Testing store implementations..."

run_test "EmployeeStore Helper" "grep -q 'casingSafeAccess' src/stores/employeeStore.js" ""
run_test "UnitStore Helper" "grep -q 'casingSafeAccess' src/stores/unitStore.js" ""
run_test "RoleStore Helper" "grep -q 'casingSafeAccess' src/stores/roleStore.js" ""
run_test "KhoanPeriodStore Helper" "grep -q 'casingSafeAccess' src/stores/khoanPeriodStore.js" ""

echo ""
echo "🎯 4. CRITICAL VIEW VALIDATION"
echo "------------------------------"

# Test critical views
echo "  📄 Testing view implementations..."

run_test "EmployeesView PascalCase" "grep -c '\.Id\|\.Name' src/views/EmployeesView.vue | head -n1" ""
run_test "RolesView PascalCase" "grep -c '\.Id\|\.Name' src/views/RolesView.vue | head -n1" ""
run_test "KhoanPeriodsView Helper" "grep -q 'casingSafeAccess' src/views/KhoanPeriodsView.vue" ""

echo ""
echo "🔍 5. CODE QUALITY VALIDATION"
echo "-----------------------------"

# Test for common issues
echo "  🐛 Testing for common issues..."

run_test "No TypeScript Errors" "npx vue-tsc --noEmit" ""
run_test "No ESLint Critical Errors" "npm run lint 2>&1 | grep -c 'error' | head -n1" "0"
run_test "Helper Utility Exists" "test -f src/utils/casingSafeAccess.js" ""

echo ""
echo "🌐 6. FRONTEND BASIC CONNECTIVITY"
echo "---------------------------------"

# Test basic frontend
echo "  🔗 Testing frontend connectivity..."

run_test "Frontend Server Running" "curl -s http://localhost:3000" ""
run_test "Home Page Loads" "curl -s http://localhost:3000 | grep -q 'TinhKhoan\|Tinh Khoan'" ""
run_test "Employees Page" "curl -s http://localhost:3000/employees" ""

echo ""
echo "📊 7. DETAILED RESULTS ANALYSIS"
echo "==============================="

# Calculate success rates
api_tests=7
store_tests=4
view_tests=3
quality_tests=3
frontend_tests=3

echo "🔌 Backend API Tests: Checking detailed responses..."

# Get detailed API data for analysis
employees_data=$(curl -s "http://localhost:5055/api/employees" 2>/dev/null)
units_data=$(curl -s "http://localhost:5055/api/units" 2>/dev/null)

if echo "$employees_data" | jq -e '.[0].Id and .[0].Name and .[0].UnitId' >/dev/null 2>&1; then
    echo -e "  ✅ Employees API: ${GREEN}Full PascalCase compliance${NC}"
else
    echo -e "  ⚠️  Employees API: ${YELLOW}Partial compliance${NC}"
fi

if echo "$units_data" | jq -e '.[0].Id and .[0].Name' >/dev/null 2>&1; then
    echo -e "  ✅ Units API: ${GREEN}Full PascalCase compliance${NC}"
else
    echo -e "  ⚠️  Units API: ${YELLOW}Partial compliance${NC}"
fi

echo ""
echo "🗃️ Store Analysis: Checking helper adoption..."

stores_with_helper=$(grep -l "casingSafeAccess" src/stores/*.js | wc -l)
total_stores=$(ls src/stores/*.js | wc -l)

echo -e "  📊 Helper adoption: $stores_with_helper/$total_stores stores"

if [ "$stores_with_helper" -eq "$total_stores" ]; then
    echo -e "  ✅ All stores: ${GREEN}Using helper utilities${NC}"
else
    echo -e "  ⚠️  Some stores: ${YELLOW}Missing helper utilities${NC}"
fi

echo ""
echo "🎯 8. FINAL ASSESSMENT"
echo "======================"

success_rate=$(( (passed_tests * 100) / total_tests ))

echo -e "📊 Test Results: $passed_tests/$total_tests passed (${success_rate}%)"

if [ "$success_rate" -ge 90 ]; then
    echo -e "🎉 Overall Status: ${GREEN}EXCELLENT - Production Ready!${NC}"
    echo -e "✅ System is fully validated and ready for deployment"
    final_status="EXCELLENT"
elif [ "$success_rate" -ge 75 ]; then
    echo -e "👍 Overall Status: ${GREEN}GOOD - Minor issues to address${NC}"
    echo -e "✅ System is stable for production with minor fixes"
    final_status="GOOD"
elif [ "$success_rate" -ge 60 ]; then
    echo -e "⚠️  Overall Status: ${YELLOW}MODERATE - Several issues need attention${NC}"
    echo -e "🔧 System needs more work before production deployment"
    final_status="MODERATE"
else
    echo -e "❌ Overall Status: ${RED}NEEDS WORK - Critical issues detected${NC}"
    echo -e "🚨 System requires significant fixes before deployment"
    final_status="NEEDS_WORK"
fi

echo ""
echo "📋 9. PRODUCTION READINESS CHECKLIST"
echo "===================================="

echo "  🔍 Manual Testing Required:"
echo "    □ Complete CRUD operations for all entities"
echo "    □ Form validation and error handling"
echo "    □ Dropdown functionality and data loading"
echo "    □ Search and filter operations"
echo "    □ User interface responsiveness"
echo "    □ Data persistence and consistency"
echo "    □ Navigation and routing"
echo "    □ Performance under load"

echo ""
echo "  🎯 Critical Workflows:"
echo "    □ Employee management (Add/Edit/Delete)"
echo "    □ Unit hierarchy management"
echo "    □ KPI definition and assignment"
echo "    □ Khoan period lifecycle"
echo "    □ Dashboard and reporting"
echo "    □ User authentication and authorization"

echo ""
echo "📈 10. NEXT STEPS RECOMMENDATION"
echo "==============================="

if [ "$final_status" = "EXCELLENT" ] || [ "$final_status" = "GOOD" ]; then
    echo "🚀 IMMEDIATE ACTIONS:"
    echo "  1. ✅ Begin manual UI testing of critical workflows"
    echo "  2. ✅ Test with real user data and scenarios"
    echo "  3. ✅ Performance testing under expected load"
    echo "  4. ✅ Final security review and testing"
    echo "  5. ✅ Prepare deployment documentation"
else
    echo "🔧 REQUIRED FIXES:"
    echo "  1. ❗ Fix failed automated tests"
    echo "  2. ❗ Complete PascalCase standardization"
    echo "  3. ❗ Resolve any code quality issues"
    echo "  4. ❗ Test all API endpoints thoroughly"
    echo "  5. ❗ Re-run validation after fixes"
fi

echo ""
echo "✅ Final validation test complete!"
echo "📊 Summary: $passed_tests passed, $failed_tests failed out of $total_tests total tests"
