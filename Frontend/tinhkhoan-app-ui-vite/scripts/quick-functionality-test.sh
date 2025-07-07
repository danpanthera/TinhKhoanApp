#!/bin/bash

# 🧪 TINH KHOAN APP - QUICK FUNCTIONALITY TEST
# Script để test nhanh các chức năng chính
# Date: 07/07/2025

echo "🧪 TINH KHOAN APP QUICK FUNCTIONALITY TEST"
echo "=========================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to test API endpoint
test_api_endpoint() {
    local endpoint=$1
    local name=$2

    echo -n "  🔍 Testing $name API: "

    if curl -s "http://localhost:5055/api/$endpoint" > /dev/null 2>&1; then
        # Check if response contains PascalCase data
        response=$(curl -s "http://localhost:5055/api/$endpoint")
        if echo "$response" | jq -e '.[0].Id' > /dev/null 2>&1; then
            echo -e "${GREEN}✅ PASS (PascalCase)${NC}"
            return 0
        else
            echo -e "${YELLOW}⚠️  PARTIAL (No ID field)${NC}"
            return 1
        fi
    else
        echo -e "${RED}❌ FAIL (Connection)${NC}"
        return 1
    fi
}

# Function to test frontend page
test_frontend_page() {
    local url=$1
    local name=$2
    local expected_text=$3

    echo -n "  🌐 Testing $name Page: "

    if curl -s "$url" > /dev/null 2>&1; then
        response=$(curl -s "$url")
        if echo "$response" | grep -q "$expected_text"; then
            echo -e "${GREEN}✅ PASS${NC}"
            return 0
        else
            echo -e "${YELLOW}⚠️  PARTIAL (Content differs)${NC}"
            return 1
        fi
    else
        echo -e "${RED}❌ FAIL (Connection)${NC}"
        return 1
    fi
}

echo ""
echo "🚀 1. BACKEND API VALIDATION"
echo "----------------------------"

api_passed=0
api_total=0

# Test core APIs
apis=(
    "employees:Employees"
    "units:Units"
    "roles:Roles"
    "positions:Positions"
    "khoanperiods:KhoanPeriods"
    "kpiassignment/tables:KPI Tables"
)

for api_info in "${apis[@]}"; do
    endpoint=$(echo "$api_info" | cut -d: -f1)
    name=$(echo "$api_info" | cut -d: -f2)

    if test_api_endpoint "$endpoint" "$name"; then
        api_passed=$((api_passed + 1))
    fi
    api_total=$((api_total + 1))
done

echo ""
echo "🌐 2. FRONTEND PAGE VALIDATION"
echo "------------------------------"

frontend_passed=0
frontend_total=0

# Test core pages
pages=(
    "http://localhost:3000/:Home:TinhKhoan"
    "http://localhost:3000/employees:Employees:Quản lý"
    "http://localhost:3000/units:Units:Đơn vị"
    "http://localhost:3000/roles:Roles:Vai trò"
    "http://localhost:3000/khoan-periods:KhoanPeriods:Kỳ Khoán"
)

for page_info in "${pages[@]}"; do
    url=$(echo "$page_info" | cut -d: -f1)
    name=$(echo "$page_info" | cut -d: -f2)
    expected=$(echo "$page_info" | cut -d: -f3)

    if test_frontend_page "$url" "$name" "$expected"; then
        frontend_passed=$((frontend_passed + 1))
    fi
    frontend_total=$((frontend_total + 1))
done

echo ""
echo "🔍 3. SPECIFIC FUNCTIONALITY TESTS"
echo "----------------------------------"

echo "  🧪 Testing critical workflows..."

# Test 1: Employee data structure
echo -n "  📊 Employee data format: "
employee_data=$(curl -s "http://localhost:5055/api/employees" | jq '.[0]' 2>/dev/null)
if echo "$employee_data" | jq -e '.Id and .Name and .UnitId' > /dev/null 2>&1; then
    echo -e "${GREEN}✅ PASS${NC}"
    workflow_passed=$((workflow_passed + 1))
else
    echo -e "${RED}❌ FAIL${NC}"
fi

# Test 2: Unit hierarchy
echo -n "  🏢 Unit hierarchy data: "
unit_data=$(curl -s "http://localhost:5055/api/units" | jq '.[0]' 2>/dev/null)
if echo "$unit_data" | jq -e '.Id and .Name' > /dev/null 2>&1; then
    echo -e "${GREEN}✅ PASS${NC}"
else
    echo -e "${RED}❌ FAIL${NC}"
fi

# Test 3: KPI Assignment tables
echo -n "  📈 KPI Assignment data: "
kpi_data=$(curl -s "http://localhost:5055/api/kpiassignment/tables" 2>/dev/null)
if echo "$kpi_data" | jq -e '.employees and .units and .kpiDefinitions' > /dev/null 2>&1; then
    echo -e "${GREEN}✅ PASS${NC}"
else
    echo -e "${RED}❌ FAIL${NC}"
fi

# Test 4: Frontend store helper adoption
echo -n "  🔧 Store helper adoption: "
helper_count=$(grep -r "casingSafeAccess" src/stores/ --include="*.js" | wc -l | tr -d ' ')
if [ "$helper_count" -gt 5 ]; then
    echo -e "${GREEN}✅ PASS ($helper_count files)${NC}"
else
    echo -e "${YELLOW}⚠️  PARTIAL ($helper_count files)${NC}"
fi

echo ""
echo "📊 4. VALIDATION SUMMARY"
echo "========================="

api_percent=$(( (api_passed * 100) / api_total ))
frontend_percent=$(( (frontend_passed * 100) / frontend_total ))

echo -e "🔌 Backend APIs: $api_passed/$api_total (${api_percent}%) ${GREEN}passed${NC}"
echo -e "🌐 Frontend Pages: $frontend_passed/$frontend_total (${frontend_percent}%) ${GREEN}passed${NC}"

overall_score=$(( (api_percent + frontend_percent) / 2 ))

echo ""
echo "🎯 OVERALL HEALTH SCORE"
echo "======================="

if [ "$overall_score" -ge 90 ]; then
    echo -e "🎉 Overall: ${GREEN}EXCELLENT ($overall_score%)${NC}"
    echo -e "✅ System is ready for production testing!"
elif [ "$overall_score" -ge 75 ]; then
    echo -e "👍 Overall: ${GREEN}GOOD ($overall_score%)${NC}"
    echo -e "✅ System is stable for most workflows!"
elif [ "$overall_score" -ge 60 ]; then
    echo -e "⚠️  Overall: ${YELLOW}MODERATE ($overall_score%)${NC}"
    echo -e "🔧 Some issues need attention before full deployment."
else
    echo -e "❌ Overall: ${RED}NEEDS WORK ($overall_score%)${NC}"
    echo -e "🚨 Critical issues detected - require immediate attention."
fi

echo ""
echo "📋 RECOMMENDED NEXT ACTIONS"
echo "============================"

if [ "$api_percent" -eq 100 ]; then
    echo -e "✅ Backend: ${GREEN}All APIs working correctly${NC}"
else
    echo -e "🔧 Backend: Check failed API endpoints"
fi

if [ "$frontend_percent" -ge 80 ]; then
    echo -e "✅ Frontend: ${GREEN}Most pages working correctly${NC}"
else
    echo -e "🔧 Frontend: Review page content and routing"
fi

echo ""
echo "🎯 CRITICAL WORKFLOWS TO TEST MANUALLY"
echo "======================================"
echo "  1. 👤 Employee CRUD (Create, Read, Update, Delete)"
echo "  2. 🏢 Unit management with hierarchy"
echo "  3. 📊 KPI Definition creation and editing"
echo "  4. 🎯 KPI Assignment workflows"
echo "  5. 📅 Khoan Period management"
echo "  6. 🔍 Search and filter functionality"
echo "  7. 📱 Responsive design on mobile"

echo ""
echo -e "${BLUE}💡 Use browser DevTools to monitor for JavaScript errors${NC}"
echo -e "${BLUE}🔍 Check Network tab for failed API calls${NC}"

echo ""
echo "✅ Quick functionality test complete!"
