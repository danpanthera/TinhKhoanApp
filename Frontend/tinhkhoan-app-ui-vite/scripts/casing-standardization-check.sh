#!/bin/bash

# 🎯 TINH KHOAN APP - CASING STANDARDIZATION CHECK
# Script để kiểm tra và báo cáo tình hình chuẩn hóa PascalCase
# Date: 07/07/2025

echo "🔍 TINH KHOAN APP CASING STANDARDIZATION CHECK"
echo "=============================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo ""
echo "📊 1. STORES ANALYSIS"
echo "---------------------"

stores=(
    "src/stores/employeeStore.js"
    "src/stores/unitStore.js"
    "src/stores/khoanPeriodStore.js"
    "src/stores/roleStore.js"
    "src/stores/positionStore.js"
    "src/stores/offlineStore.js"
    "src/stores/themeStore.js"
)

for store in "${stores[@]}"; do
    if [ -f "$store" ]; then
        echo -n "  🔧 $store: "

        # Check if store imports casingSafeAccess
        if grep -q "casingSafeAccess" "$store"; then
            echo -e "${GREEN}✅ Using helper${NC}"
        else
            echo -e "${YELLOW}⚠️  No helper imported${NC}"
        fi

        # Check for problematic patterns
        camelcase_count=$(grep -c "\.id\|\.name\|\.type\|\.status" "$store" 2>/dev/null || echo 0)
        if [ "$camelcase_count" -gt 0 ]; then
            echo -e "    ${RED}❌ Found $camelcase_count camelCase usages${NC}"
        fi
    else
        echo -e "  ❓ $store: ${RED}File not found${NC}"
    fi
done

echo ""
echo "📱 2. VIEWS ANALYSIS"
echo "--------------------"

critical_views=(
    "src/views/EmployeesView.vue"
    "src/views/RolesView.vue"
    "src/views/PositionsView.vue"
    "src/views/KhoanPeriodsView.vue"
    "src/views/KpiDefinitionsView.vue"
    "src/views/EmployeeKpiAssignmentView.vue"
    "src/views/UnitKpiAssignmentView.vue"
)

for view in "${critical_views[@]}"; do
    if [ -f "$view" ]; then
        echo -n "  📄 $(basename $view): "

        # Check template binding patterns
        camelcase_template=$(grep -c "\.id\|\.name\|\.type\|\.status" "$view" 2>/dev/null || echo 0)
        pascalcase_template=$(grep -c "\.Id\|\.Name\|\.Type\|\.Status" "$view" 2>/dev/null || echo 0)

        if [ "$pascalcase_template" -gt "$camelcase_template" ]; then
            echo -e "${GREEN}✅ Mostly PascalCase ($pascalcase_template vs $camelcase_template)${NC}"
        elif [ "$camelcase_template" -gt "$pascalcase_template" ]; then
            echo -e "${RED}❌ Mostly camelCase ($camelcase_template vs $pascalcase_template)${NC}"
        else
            echo -e "${YELLOW}⚠️  Mixed or no patterns${NC}"
        fi
    else
        echo -e "  ❓ $(basename $view): ${RED}File not found${NC}"
    fi
done

echo ""
echo "🔧 3. COMPONENTS ANALYSIS"
echo "-------------------------"

component_dirs=(
    "src/components"
)

for dir in "${component_dirs[@]}"; do
    if [ -d "$dir" ]; then
        component_count=$(find "$dir" -name "*.vue" | wc -l)
        echo "  📁 $dir: Found $component_count Vue components"

        # Quick check for problematic patterns in components
        camelcase_total=0
        pascalcase_total=0

        while IFS= read -r component; do
            if [ -f "$component" ]; then
                camelcase=$(grep -c "\.id\|\.name\|\.type\|\.status" "$component" 2>/dev/null || echo 0)
                pascalcase=$(grep -c "\.Id\|\.Name\|\.Type\|\.Status" "$component" 2>/dev/null || echo 0)
                camelcase_total=$((camelcase_total + camelcase))
                pascalcase_total=$((pascalcase_total + pascalcase))
            fi
        done < <(find "$dir" -name "*.vue")

        if [ "$pascalcase_total" -gt "$camelcase_total" ]; then
            echo -e "    ${GREEN}✅ Components mostly use PascalCase ($pascalcase_total vs $camelcase_total)${NC}"
        elif [ "$camelcase_total" -gt "$pascalcase_total" ]; then
            echo -e "    ${RED}❌ Components mostly use camelCase ($camelcase_total vs $pascalcase_total)${NC}"
        else
            echo -e "    ${YELLOW}⚠️  Components have mixed patterns${NC}"
        fi
    else
        echo -e "  ❓ $dir: ${RED}Directory not found${NC}"
    fi
done

echo ""
echo "🎯 4. HELPER UTILITY STATUS"
echo "---------------------------"

helper_file="src/utils/casingSafeAccess.js"
if [ -f "$helper_file" ]; then
    echo -e "  ✅ Helper utility exists: ${GREEN}$helper_file${NC}"

    # Count usage across project
    usage_count=$(grep -r "casingSafeAccess" src/ --include="*.js" --include="*.vue" | wc -l)
    echo "  📊 Helper usage count: $usage_count files"

    if [ "$usage_count" -lt 5 ]; then
        echo -e "    ${YELLOW}⚠️  Low usage - consider expanding adoption${NC}"
    else
        echo -e "    ${GREEN}✅ Good adoption across project${NC}"
    fi
else
    echo -e "  ❌ Helper utility missing: ${RED}$helper_file${NC}"
fi

echo ""
echo "🔍 5. SPECIFIC PROBLEM PATTERNS"
echo "--------------------------------"

echo "  🔸 Searching for .id (should be .Id):"
id_issues=$(grep -r "\.id[^a-zA-Z]" src/ --include="*.js" --include="*.vue" | wc -l)
if [ "$id_issues" -gt 0 ]; then
    echo -e "    ${RED}❌ Found $id_issues instances of .id${NC}"
else
    echo -e "    ${GREEN}✅ No .id issues found${NC}"
fi

echo "  🔸 Searching for .name (should be .Name):"
name_issues=$(grep -r "\.name[^a-zA-Z]" src/ --include="*.js" --include="*.vue" | wc -l)
if [ "$name_issues" -gt 0 ]; then
    echo -e "    ${RED}❌ Found $name_issues instances of .name${NC}"
else
    echo -e "    ${GREEN}✅ No .name issues found${NC}"
fi

echo ""
echo "📋 6. RECOMMENDED ACTIONS"
echo "-------------------------"

echo "  1. 🔄 Apply casingSafeAccess helper to all stores"
echo "  2. 🔄 Update remaining views to use PascalCase consistently"
echo "  3. 🔄 Add import statements for helper utilities"
echo "  4. 🔄 Review and fix any remaining .id/.name patterns"
echo "  5. ✅ Test all dropdowns and forms after changes"

echo ""
echo "🎯 STANDARDIZATION STATUS"
echo "========================="

# Calculate overall score
stores_ok=0
for store in "${stores[@]}"; do
    if [ -f "$store" ] && grep -q "casingSafeAccess" "$store"; then
        stores_ok=$((stores_ok + 1))
    fi
done

views_ok=0
for view in "${critical_views[@]}"; do
    if [ -f "$view" ]; then
        camelcase_template=$(grep -c "\.id\|\.name\|\.type\|\.status" "$view" 2>/dev/null || echo 0)
        pascalcase_template=$(grep -c "\.Id\|\.Name\|\.Type\|\.Status" "$view" 2>/dev/null || echo 0)
        if [ "$pascalcase_template" -gt "$camelcase_template" ]; then
            views_ok=$((views_ok + 1))
        fi
    fi
done

stores_total=${#stores[@]}
views_total=${#critical_views[@]}

stores_percent=$(( (stores_ok * 100) / stores_total ))
views_percent=$(( (views_ok * 100) / views_total ))

echo "📊 Stores standardized: $stores_ok/$stores_total ($stores_percent%)"
echo "📊 Views standardized: $views_ok/$views_total ($views_percent%)"

overall_percent=$(( (stores_percent + views_percent) / 2 ))

if [ "$overall_percent" -ge 80 ]; then
    echo -e "🎉 Overall status: ${GREEN}GOOD ($overall_percent%)${NC}"
elif [ "$overall_percent" -ge 60 ]; then
    echo -e "⚠️  Overall status: ${YELLOW}MODERATE ($overall_percent%)${NC}"
else
    echo -e "❌ Overall status: ${RED}NEEDS WORK ($overall_percent%)${NC}"
fi

echo ""
echo "✅ Analysis complete!"
