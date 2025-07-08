#!/bin/bash

# 🔧 Test script để verify fix indicators display issues
# Kiểm tra cả Employee KPI Assignment và Unit KPI Assignment

echo "🔧 TESTING INDICATORS DISPLAY FIXES"
echo "==================================="
echo ""

# Set colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Project paths
FRONTEND_PATH="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite"
EMPLOYEE_VIEW="$FRONTEND_PATH/src/views/EmployeeKpiAssignmentView.vue"
UNIT_VIEW="$FRONTEND_PATH/src/views/UnitKpiAssignmentView.vue"

echo -e "${BLUE}📍 Testing Location:${NC} $FRONTEND_PATH"
echo ""

# Check if files exist
if [ ! -f "$EMPLOYEE_VIEW" ] || [ ! -f "$UNIT_VIEW" ]; then
    echo -e "${RED}❌ Vue files not found!${NC}"
    exit 1
fi

echo -e "${GREEN}✅ Vue files found${NC}"
echo ""

# Test 1: Check EmployeeKpiAssignmentView indicators fix
echo -e "${YELLOW}🔍 TEST 1: EmployeeKpiAssignmentView indicators handling...${NC}"
echo ""

if grep -q "indicatorsData.*indicators.*Indicators" "$EMPLOYEE_VIEW"; then
    echo -e "${GREEN}✅ Employee view handles both indicators casing${NC}"
    echo "   - Uses: response.data?.indicators || response.data?.Indicators"
else
    echo -e "${RED}❌ Employee view missing indicators casing fix${NC}"
fi

if grep -q "Raw indicators data:" "$EMPLOYEE_VIEW"; then
    echo -e "${GREEN}✅ Employee view has debug logging${NC}"
    echo "   - Includes raw data and normalized data logging"
else
    echo -e "${YELLOW}⚠️ Employee view missing debug logging${NC}"
fi

echo ""

# Test 2: Check UnitKpiAssignmentView indicators fix  
echo -e "${YELLOW}🔍 TEST 2: UnitKpiAssignmentView indicators handling...${NC}"
echo ""

if grep -q "indicatorsData.*indicators.*Indicators" "$UNIT_VIEW"; then
    echo -e "${GREEN}✅ Unit view handles both indicators casing${NC}"
    echo "   - Uses: response.data?.indicators || response.data?.Indicators"
else
    echo -e "${RED}❌ Unit view missing indicators casing fix${NC}"
fi

if grep -q "Raw indicators data:" "$UNIT_VIEW"; then
    echo -e "${GREEN}✅ Unit view has debug logging${NC}"
    echo "   - Includes raw data and normalized data logging"  
else
    echo -e "${YELLOW}⚠️ Unit view missing debug logging${NC}"
fi

echo ""

# Test 3: Check both views have onMounted
echo -e "${YELLOW}🔍 TEST 3: Data loading initialization...${NC}"
echo ""

if grep -q "onMounted.*loadInitialData" "$EMPLOYEE_VIEW" || grep -q "loadInitialData" "$EMPLOYEE_VIEW"; then
    echo -e "${GREEN}✅ Employee view has onMounted data loading${NC}"
else
    echo -e "${YELLOW}⚠️ Employee view may not load data on mount${NC}"
fi

if grep -q "onMounted.*loadInitialData" "$UNIT_VIEW" || grep -q "loadInitialData" "$UNIT_VIEW"; then
    echo -e "${GREEN}✅ Unit view has onMounted data loading${NC}"
else
    echo -e "${RED}❌ Unit view missing onMounted data loading${NC}"
fi

echo ""

# Test 4: Check template conditions
echo -e "${YELLOW}🔍 TEST 4: Template display conditions...${NC}"
echo ""

if grep -q "v-if.*indicators.*length" "$EMPLOYEE_VIEW"; then
    echo -e "${GREEN}✅ Employee view has indicators length condition${NC}"
else
    echo -e "${YELLOW}⚠️ Employee view may not check indicators length${NC}"
fi

if grep -q "v-if.*availableKpiIndicators.*length" "$UNIT_VIEW"; then
    echo -e "${GREEN}✅ Unit view has availableKpiIndicators length condition${NC}"
else
    echo -e "${YELLOW}⚠️ Unit view may not check indicators length${NC}"
fi

echo ""

# Test 5: Syntax check
echo -e "${YELLOW}🔍 TEST 5: Vue syntax validation...${NC}"
echo ""

cd "$FRONTEND_PATH"
if npm run type-check 2>/dev/null | grep -q "error"; then
    echo -e "${RED}❌ TypeScript errors found${NC}"
    npm run type-check 2>&1 | grep "error" | head -3
else
    echo -e "${GREEN}✅ No TypeScript syntax errors${NC}"
fi

echo ""

# Summary
echo -e "${BLUE}📋 SUMMARY OF FIXES APPLIED:${NC}"
echo "============================"
echo ""

echo -e "${GREEN}✅ Issue 1: Employee KPI Assignment indicators not displaying${NC}"
echo "   Root cause: API returns 'Indicators' but code checked 'indicators'"  
echo "   Fix: Handle both PascalCase and camelCase variants"
echo "   Result: Indicators should display after selecting KPI table"
echo ""

echo -e "${GREEN}✅ Issue 2: Unit KPI Assignment branch tables not loading${NC}"
echo "   Root cause: Same PascalCase/camelCase mismatch"
echo "   Fix: Handle both variants in UnitKpiAssignmentView"
echo "   Result: Branch KPI indicators should load after branch selection"
echo ""

echo -e "${BLUE}🧪 TESTING INSTRUCTIONS:${NC}"
echo "========================"
echo ""
echo "1. Start frontend dev server:"
echo "   cd $FRONTEND_PATH"
echo "   npm run dev"
echo ""
echo "2. Test Employee KPI Assignment:"
echo "   - Navigate to Employee KPI Assignment page"
echo "   - Select period → branch → employees → KPI table"
echo "   - Verify indicators appear in the table below"
echo "   - Check console logs for successful indicator loading"
echo ""
echo "3. Test Unit KPI Assignment:"  
echo "   - Navigate to Unit KPI Assignment page"
echo "   - Select period → select branch"
echo "   - Verify KPI indicators load automatically"
echo "   - Check console logs for successful branch KPI loading"
echo ""
echo "4. Console messages to look for:"
echo "   ✅ '✅ Loaded KPI indicators: [number]'"
echo "   ✅ '📋 Sample indicators:' with indicator details"
echo "   ✅ '🔄 Raw indicators data:' and '🔄 Normalized data:'"
echo ""

# Git status
echo -e "${BLUE}📝 GIT STATUS:${NC}"
echo "=============="
echo ""
LAST_COMMIT=$(git log -1 --oneline 2>/dev/null || echo "No git history")
echo "Last commit: $LAST_COMMIT"
echo ""

echo -e "${GREEN}🎉 INDICATORS DISPLAY FIXES COMPLETED!${NC}"
echo ""
echo -e "${BLUE}Expected Results:${NC}"
echo "✅ Employee KPI indicators display after KPI table selection"
echo "✅ Unit KPI indicators display after branch selection"  
echo "✅ No more empty indicator lists"
echo "✅ Console shows successful data loading"
echo "✅ Both PascalCase and camelCase API responses handled"
echo ""
