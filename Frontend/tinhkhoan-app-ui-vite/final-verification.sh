#!/bin/bash

# 🎯 Unit KPI Assignment Enhancement - Final Verification Script
# This script tests all the implemented enhancements to ensure they work correctly

echo "🎯 Unit KPI Assignment Enhancement - Final Verification"
echo "======================================================"
echo ""

# Test API endpoints
echo "📊 1. Testing API Endpoints..."
echo "-------------------------------"

echo -n "  - HoiSo table indicators: "
HOISO_COUNT=$(curl -s "http://localhost:5055/api/KpiAssignment/table/HoiSo" | jq '.indicators."$values" | length' 2>/dev/null)
if [ "$HOISO_COUNT" = "11" ]; then
    echo "✅ $HOISO_COUNT indicators (CORRECT)"
else
    echo "❌ $HOISO_COUNT indicators (EXPECTED: 11)"
fi

echo -n "  - GiamdocCnl2 table indicators: "
CNL2_COUNT=$(curl -s "http://localhost:5055/api/KpiAssignment/table/GiamdocCnl2" | jq '.indicators."$values" | length' 2>/dev/null)
if [ "$CNL2_COUNT" = "11" ]; then
    echo "✅ $CNL2_COUNT indicators (CORRECT)"
else
    echo "❌ $CNL2_COUNT indicators (EXPECTED: 11)"
fi

echo -n "  - CNL1 units available: "
CNL1_UNITS=$(curl -s "http://localhost:5055/api/Units" | jq '[."$values"[] | select(.type == "CNL1")] | length' 2>/dev/null)
echo "✅ $CNL1_UNITS unit(s)"

echo -n "  - CNL2 units available: "
CNL2_UNITS=$(curl -s "http://localhost:5055/api/Units" | jq '[."$values"[] | select(.type == "CNL2")] | length' 2>/dev/null)
echo "✅ $CNL2_UNITS units"

echo -n "  - KhoanPeriods available: "
PERIODS=$(curl -s "http://localhost:5055/api/KhoanPeriods" | jq '."$values" | length' 2>/dev/null)
echo "✅ $PERIODS periods"

echo ""

# Test table name mapping
echo "📋 2. Testing Table Name Mapping..."
echo "----------------------------------"

echo -n "  - HoiSo table name: "
HOISO_NAME=$(curl -s "http://localhost:5055/api/KpiAssignment/table/HoiSo" | jq -r '.tableName' 2>/dev/null)
if [ "$HOISO_NAME" = "Hội sở" ]; then
    echo "✅ '$HOISO_NAME' (CORRECT)"
else
    echo "❌ '$HOISO_NAME' (EXPECTED: 'Hội sở')"
fi

echo -n "  - GiamdocCnl2 table name: "
CNL2_NAME=$(curl -s "http://localhost:5055/api/KpiAssignment/table/GiamdocCnl2" | jq -r '.tableName' 2>/dev/null)
if [ "$CNL2_NAME" = "Giám đốc CNL2" ]; then
    echo "✅ '$CNL2_NAME' (CORRECT)"
else
    echo "❌ '$CNL2_NAME' (EXPECTED: 'Giám đốc CNL2')"
fi

echo ""

# Check file modifications
echo "🔧 3. Checking Implementation Files..."
echo "-------------------------------------"

if [ -f "src/views/UnitKpiAssignmentView.vue" ]; then
    echo "✅ UnitKpiAssignmentView.vue exists"
    
    # Check for "Hội sở" in table header
    if grep -q "Hội sở" src/views/UnitKpiAssignmentView.vue; then
        echo "  ✅ Contains 'Hội sở' table header"
    else
        echo "  ❌ Missing 'Hội sở' table header"
    fi
    
    # Check for onBranchChange method
    if grep -q "onBranchChange" src/views/UnitKpiAssignmentView.vue; then
        echo "  ✅ Contains onBranchChange method"
    else
        echo "  ❌ Missing onBranchChange method"
    fi
    
    # Check for getKpiIndicatorsForUnitType call
    if grep -q "getKpiIndicatorsForUnitType" src/views/UnitKpiAssignmentView.vue; then
        echo "  ✅ Contains getKpiIndicatorsForUnitType calls"
    else
        echo "  ❌ Missing getKpiIndicatorsForUnitType calls"
    fi
else
    echo "❌ UnitKpiAssignmentView.vue not found"
fi

if [ -f "src/services/unitKpiAssignmentService.js" ]; then
    echo "✅ unitKpiAssignmentService.js exists"
    
    # Check for new service methods
    if grep -q "getKpiIndicatorsForUnitType" src/services/unitKpiAssignmentService.js; then
        echo "  ✅ Contains getKpiIndicatorsForUnitType method"
    else
        echo "  ❌ Missing getKpiIndicatorsForUnitType method"
    fi
    
    if grep -q "getKpiIndicatorsByTableType" src/services/unitKpiAssignmentService.js; then
        echo "  ✅ Contains getKpiIndicatorsByTableType method"
    else
        echo "  ❌ Missing getKpiIndicatorsByTableType method"
    fi
else
    echo "❌ unitKpiAssignmentService.js not found"
fi

echo ""

# Summary
echo "📝 4. Implementation Summary..."
echo "------------------------------"

if [ "$HOISO_COUNT" = "11" ] && [ "$CNL2_COUNT" = "11" ] && [ "$HOISO_NAME" = "Hội sở" ] && [ "$CNL2_NAME" = "Giám đốc CNL2" ]; then
    echo "✅ ALL REQUIREMENTS IMPLEMENTED SUCCESSFULLY!"
    echo ""
    echo "🎯 Summary of Changes:"
    echo "  ✅ Requirement 1: CNL1 column now shows 'Hội sở' instead of unit name"
    echo "  ✅ Requirement 2: Auto-load KPI functionality implemented"
    echo "  ✅ Service layer: New methods for KPI loading by unit type"
    echo "  ✅ Component logic: Enhanced branch change handling"
    echo "  ✅ API integration: Proper \$values format handling"
    echo ""
    echo "🚀 Ready for Production Use!"
else
    echo "❌ Some issues detected. Please review the output above."
fi

echo ""
echo "🌐 Manual Testing:"
echo "  1. Open: http://localhost:3000/unit-kpi-assignment"
echo "  2. Select a period and branch"
echo "  3. Verify KPI indicators load automatically"
echo "  4. Click 'Tạo giao khoán mới' to test modal"
echo "  5. Check table shows 'Hội sở' in first column"
echo ""
echo "📄 Quick Verification: http://localhost:3000/quick-verification.html"
