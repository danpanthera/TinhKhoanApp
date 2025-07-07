#!/bin/bash

# 🧪 TINH KHOAN APP - MANUAL UI TEST GUIDE
# Script để hướng dẫn test thủ công các workflow chính
# Date: 07/07/2025

echo "🧪 TINH KHOAN APP MANUAL UI TEST GUIDE"
echo "======================================"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo ""
echo "🚀 1. PREREQUISITES CHECK"
echo "------------------------"

# Check if dev server is running
if curl -s "http://localhost:3000" > /dev/null 2>&1; then
    echo -e "  ✅ Frontend dev server: ${GREEN}Running (http://localhost:3000)${NC}"
else
    echo -e "  ❌ Frontend dev server: ${RED}Not running${NC}"
    echo "  🔧 Run: npm run dev"
    exit 1
fi

# Check if backend is running
if curl -s "http://localhost:5055/api/employees" > /dev/null 2>&1; then
    echo -e "  ✅ Backend API server: ${GREEN}Running (http://localhost:5055)${NC}"
else
    echo -e "  ❌ Backend API server: ${RED}Not running${NC}"
    echo "  🔧 Run backend server first"
    exit 1
fi

echo ""
echo "📋 2. CRITICAL WORKFLOWS TO TEST"
echo "--------------------------------"

echo ""
echo "🔸 2.1 EMPLOYEES MANAGEMENT"
echo "  📍 URL: http://localhost:3000/employees"
echo "  ✅ Test Items:"
echo "    • Page loads without errors"
echo "    • Employee list displays data"
echo "    • Add new employee form works"
echo "    • Edit employee functionality"
echo "    • Delete employee with confirmation"
echo "    • Dropdown filters (Unit, Role, Position)"
echo "    • Pagination works correctly"
echo "    • Search functionality"

echo ""
echo "🔸 2.2 UNITS MANAGEMENT"
echo "  📍 URL: http://localhost:3000/units"
echo "  ✅ Test Items:"
echo "    • Unit tree/hierarchy displays"
echo "    • Add new unit with parent selection"
echo "    • Edit unit information"
echo "    • Delete unit (check cascade rules)"
echo "    • Unit code generation/validation"

echo ""
echo "🔸 2.3 ROLES & POSITIONS"
echo "  📍 URL: http://localhost:3000/roles"
echo "  📍 URL: http://localhost:3000/positions"
echo "  ✅ Test Items:"
echo "    • CRUD operations work"
echo "    • Data validation"
echo "    • List display and sorting"

echo ""
echo "🔸 2.4 KHOAN PERIODS"
echo "  📍 URL: http://localhost:3000/khoan-periods"
echo "  ✅ Test Items:"
echo "    • Period list displays"
echo "    • Create new period"
echo "    • Edit existing period"
echo "    • Date validation (start < end)"
echo "    • Status transitions"
echo "    • Type selection dropdown"

echo ""
echo "🔸 2.5 KPI DEFINITIONS"
echo "  📍 URL: http://localhost:3000/kpi-definitions"
echo "  ✅ Test Items:"
echo "    • KPI list displays"
echo "    • Create KPI with all fields"
echo "    • Formula validation"
echo "    • Category/type selection"
echo "    • Units and data types"

echo ""
echo "🔸 2.6 KPI ASSIGNMENTS"
echo "  📍 URL: http://localhost:3000/employee-kpi-assignment"
echo "  📍 URL: http://localhost:3000/unit-kpi-assignment"
echo "  ✅ Test Items:"
echo "    • Assignment interface loads"
echo "    • Employee/Unit selection dropdowns"
echo "    • KPI selection with targets"
echo "    • Period selection"
echo "    • Save assignments"
echo "    • View existing assignments"

echo ""
echo "🔸 2.7 DASHBOARD & REPORTING"
echo "  📍 URL: http://localhost:3000/"
echo "  ✅ Test Items:"
echo "    • Dashboard loads with data"
echo "    • KPI cards display correctly"
echo "    • Charts render properly"
    "    • Filter by period works"
echo "    • Real-time data updates"

echo ""
echo "🔧 3. DATA VALIDATION TESTS"
echo "---------------------------"

echo "  🔸 Test PascalCase consistency:"
echo "    • Check API responses in browser DevTools"
echo "    • Verify dropdown options display correctly"
echo "    • Ensure form submissions work"
echo "    • Validate data binding in tables"

echo "  🔸 Test error handling:"
echo "    • Submit invalid forms"
echo "    • Test network disconnection"
echo "    • Try duplicate entries"
echo "    • Test required field validation"

echo ""
echo "🚀 4. AUTOMATED TESTS"
echo "---------------------"

echo "  🤖 Running automated validation..."

# Basic API validation
echo "  🔍 Testing core APIs..."
apis=("employees" "units" "roles" "positions" "khoanperiods" "kpiassignment/tables")

for api in "${apis[@]}"; do
    if curl -s "http://localhost:5055/api/$api" | jq -e '.[0].Id' > /dev/null 2>&1; then
        echo -e "    ✅ API /$api: ${GREEN}PascalCase OK${NC}"
    else
        echo -e "    ❌ API /$api: ${RED}Format issue${NC}"
    fi
done

echo ""
echo "📊 5. PERFORMANCE & UX TESTS"
echo "----------------------------"

echo "  🔸 Test responsive design:"
echo "    • Test on mobile viewport (DevTools)"
echo "    • Check tablet layout"
echo "    • Verify desktop experience"

echo "  🔸 Test loading states:"
echo "    • Slow 3G network simulation"
echo "    • Loading spinners display"
echo "    • Error states handle gracefully"

echo ""
echo "✅ 6. COMPLETION CHECKLIST"
echo "============================"

echo "  📋 Before deployment:"
echo "    □ All CRUD operations tested"
echo "    □ Data integrity verified"
echo "    □ No console errors"
echo "    □ Forms validation works"
echo "    □ Dropdowns populated correctly"
echo "    □ Navigation works"
echo "    □ Authentication flows"
echo "    □ Responsive design OK"
echo "    □ Performance acceptable"
echo "    □ Error handling graceful"

echo ""
echo "🎯 NEXT STEPS"
echo "============="
echo "  1. Open each URL listed above"
echo "  2. Perform the test items manually"
echo "  3. Note any issues or errors"
echo "  4. Check browser DevTools for errors"
echo "  5. Validate data format consistency"

echo ""
echo -e "${GREEN}🚀 Ready to start manual testing!${NC}"
echo -e "${BLUE}💡 Tip: Keep DevTools open to monitor network and console${NC}"
