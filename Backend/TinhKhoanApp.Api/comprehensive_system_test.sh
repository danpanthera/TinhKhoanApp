#!/bin/bash

# Comprehensive System Test for TinhKhoanApp
# Tests backend API, database, frontend, and key functionality

echo "🧪 === COMPREHENSIVE SYSTEM TEST ==="
echo "📅 Date: $(date)"
echo "🏗️ Testing TinhKhoanApp system health and functionality"
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Test counters
TOTAL_TESTS=0
PASSED_TESTS=0
FAILED_TESTS=0

# Function to run test
run_test() {
    local test_name="$1"
    local test_command="$2"
    local expected_result="$3"

    TOTAL_TESTS=$((TOTAL_TESTS + 1))
    echo -n "🔍 Testing: $test_name... "

    if [ -n "$expected_result" ]; then
        result=$(eval "$test_command" 2>/dev/null)
        if [[ "$result" == *"$expected_result"* ]]; then
            echo -e "${GREEN}PASSED${NC}"
            PASSED_TESTS=$((PASSED_TESTS + 1))
            return 0
        else
            echo -e "${RED}FAILED${NC} (Expected: $expected_result, Got: $result)"
            FAILED_TESTS=$((FAILED_TESTS + 1))
            return 1
        fi
    else
        if eval "$test_command" >/dev/null 2>&1; then
            echo -e "${GREEN}PASSED${NC}"
            PASSED_TESTS=$((PASSED_TESTS + 1))
            return 0
        else
            echo -e "${RED}FAILED${NC}"
            FAILED_TESTS=$((FAILED_TESTS + 1))
            return 1
        fi
    fi
}

# Check if backend is running
echo -e "${BLUE}🎯 Backend API Tests${NC}"
run_test "Backend Process Running" "pgrep -f 'TinhKhoanApp.Api'" ""
run_test "Backend Health Check" "curl -s http://localhost:5055/health" "Healthy"
run_test "DirectImport Status" "curl -s http://localhost:5055/api/DirectImport/status" "Direct Import System Online"

# Check database connectivity
echo -e "${BLUE}🗄️ Database Tests${NC}"
run_test "Database Connection" "curl -s http://localhost:5055/health | jq -r '.status'" "Healthy"

# Check frontend
echo -e "${BLUE}🎨 Frontend Tests${NC}"
run_test "Frontend Process Running" "pgrep -f 'vite --host'" ""
run_test "Frontend HTTP Response" "curl -s -o /dev/null -w '%{http_code}' http://localhost:3000" "200"

# Check key API endpoints
echo -e "${BLUE}📡 API Endpoints Tests${NC}"
run_test "KPI Definitions API" "curl -s -o /dev/null -w '%{http_code}' http://localhost:5055/api/KPIDefinitions" "200"
run_test "KPI Tables API" "curl -s -o /dev/null -w '%{http_code}' http://localhost:5055/api/KpiAssignment/tables" "200"
run_test "Units API" "curl -s -o /dev/null -w '%{http_code}' http://localhost:5055/api/Units" "200"
run_test "Employees API" "curl -s -o /dev/null -w '%{http_code}' http://localhost:5055/api/Employees" "200"
run_test "Khoan Periods API" "curl -s -o /dev/null -w '%{http_code}' http://localhost:5055/api/KhoanPeriods" "200"

# Check file structure
echo -e "${BLUE}📁 File Structure Tests${NC}"
run_test "Backend Build Files" "test -f /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/bin/Debug/net8.0/TinhKhoanApp.Api.dll" ""
run_test "Frontend Node Modules" "test -d /Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite/node_modules" ""
run_test "Enhanced KPI Views" "grep -q 'handleTargetInput' /Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite/src/views/EmployeeKpiAssignmentView.vue" ""

# Check recent commits
echo -e "${BLUE}📝 Git Status Tests${NC}"
run_test "Git Repository Status" "cd /Users/nguyendat/Documents/Projects/TinhKhoanApp && git status --porcelain | wc -l | grep -E '^[0-9]+$'" ""
run_test "Recent Commits" "cd /Users/nguyendat/Documents/Projects/TinhKhoanApp && git log --oneline -5 | grep -E '(Enhanced|KPI|validation)'" ""

# Memory and performance check
echo -e "${BLUE}⚡ Performance Tests${NC}"
backend_memory=$(ps -o pid,rss,comm -p $(pgrep -f 'TinhKhoanApp.Api') | tail -1 | awk '{print $2}')
frontend_memory=$(ps -o pid,rss,comm -p $(pgrep -f 'vite --host') | tail -1 | awk '{print $2}')

if [ -n "$backend_memory" ] && [ "$backend_memory" -lt 1000000 ]; then
    echo -e "🔍 Testing: Backend Memory Usage... ${GREEN}PASSED${NC} (${backend_memory} KB)"
    PASSED_TESTS=$((PASSED_TESTS + 1))
else
    echo -e "🔍 Testing: Backend Memory Usage... ${YELLOW}WARNING${NC} (${backend_memory} KB)"
fi

if [ -n "$frontend_memory" ] && [ "$frontend_memory" -lt 500000 ]; then
    echo -e "🔍 Testing: Frontend Memory Usage... ${GREEN}PASSED${NC} (${frontend_memory} KB)"
    PASSED_TESTS=$((PASSED_TESTS + 1))
else
    echo -e "🔍 Testing: Frontend Memory Usage... ${YELLOW}WARNING${NC} (${frontend_memory} KB)"
fi

TOTAL_TESTS=$((TOTAL_TESTS + 2))

# Summary
echo ""
echo -e "${BLUE}📊 === TEST SUMMARY ===${NC}"
echo "🎯 Total Tests: $TOTAL_TESTS"
echo -e "✅ Passed: ${GREEN}$PASSED_TESTS${NC}"
echo -e "❌ Failed: ${RED}$FAILED_TESTS${NC}"

if [ $FAILED_TESTS -eq 0 ]; then
    echo -e "${GREEN}🎉 ALL TESTS PASSED! System is healthy and ready.${NC}"
    exit 0
else
    echo -e "${YELLOW}⚠️ Some tests failed. Please check the issues above.${NC}"
    exit 1
fi
