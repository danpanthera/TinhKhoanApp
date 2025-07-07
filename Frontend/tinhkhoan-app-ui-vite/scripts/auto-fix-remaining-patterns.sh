#!/bin/bash

# 🔧 TINH KHOAN APP - AUTO FIX REMAINING PATTERNS
# Script để tự động sửa các pattern camelCase còn lại
# Date: 07/07/2025

echo "🔧 TINH KHOAN APP AUTO FIX REMAINING PATTERNS"
echo "============================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Backup function
backup_file() {
    local file="$1"
    if [ -f "$file" ]; then
        cp "$file" "${file}.backup.$(date +%Y%m%d_%H%M%S)"
        echo -e "  📁 Backed up: ${BLUE}$file${NC}"
    fi
}

# Fix function for specific patterns
fix_patterns_in_file() {
    local file="$1"
    local changes=0
    
    if [ ! -f "$file" ]; then
        echo -e "  ❌ File not found: ${RED}$file${NC}"
        return 1
    fi
    
    echo -n "  🔧 Fixing patterns in $(basename $file): "
    
    # Backup original file
    backup_file "$file"
    
    # Apply fixes with sed (safer than manual editing)
    # Fix .id with getId() helper calls
    sed -i '' 's/\.id\([^a-zA-Z]\)/\.Id\1/g' "$file" && changes=$((changes + 1))
    
    # Fix .name with getName() helper calls  
    sed -i '' 's/\.name\([^a-zA-Z]\)/\.Name\1/g' "$file" && changes=$((changes + 1))
    
    # Fix common field patterns
    sed -i '' 's/\.type\([^a-zA-Z]\)/\.Type\1/g' "$file" && changes=$((changes + 1))
    sed -i '' 's/\.status\([^a-zA-Z]\)/\.Status\1/g' "$file" && changes=$((changes + 1))
    sed -i '' 's/\.code\([^a-zA-Z]\)/\.Code\1/g' "$file" && changes=$((changes + 1))
    
    if [ "$changes" -gt 0 ]; then
        echo -e "${GREEN}✅ Fixed $changes patterns${NC}"
        return 0
    else
        echo -e "${YELLOW}⚠️  No changes needed${NC}"
        return 0
    fi
}

echo ""
echo "🎯 1. FIXING CRITICAL VIEWS"
echo "---------------------------"

critical_views=(
    "src/views/EmployeesView.vue"
    "src/views/KhoanPeriodsView.vue"
    "src/views/RolesView.vue"
    "src/views/PositionsView.vue"
    "src/views/KpiDefinitionsView.vue"
    "src/views/EmployeeKpiAssignmentView.vue"
    "src/views/UnitKpiAssignmentView.vue"
)

for view in "${critical_views[@]}"; do
    if [ -f "$view" ]; then
        fix_patterns_in_file "$view"
    else
        echo -e "  ❓ View not found: ${RED}$view${NC}"
    fi
done

echo ""
echo "🗃️ 2. FIXING STORES WITH REMAINING ISSUES"
echo "------------------------------------------"

problem_stores=(
    "src/stores/employeeStore.js"
    "src/stores/unitStore.js"
    "src/stores/roleStore.js"
    "src/stores/offlineStore.js"
)

for store in "${problem_stores[@]}"; do
    if [ -f "$store" ]; then
        fix_patterns_in_file "$store"
    else
        echo -e "  ❓ Store not found: ${RED}$store${NC}"
    fi
done

echo ""
echo "🔧 3. FIXING COMPONENTS WITH CAMELCASE"
echo "--------------------------------------"

# Find components with most camelCase usage
components_with_issues=$(find src/components -name "*.vue" -exec grep -l "\.id\|\.name\|\.type\|\.status" {} \; | head -10)

if [ -n "$components_with_issues" ]; then
    echo "$components_with_issues" | while read -r component; do
        if [ -f "$component" ]; then
            fix_patterns_in_file "$component"
        fi
    done
else
    echo -e "  ✅ No components with camelCase issues found"
fi

echo ""
echo "🔍 4. VALIDATION AFTER FIXES"
echo "----------------------------"

# Count remaining issues
echo "  📊 Counting remaining camelCase patterns..."

id_remaining=$(grep -r "\.id[^a-zA-Z]" src/ --include="*.js" --include="*.vue" 2>/dev/null | wc -l | tr -d ' ')
name_remaining=$(grep -r "\.name[^a-zA-Z]" src/ --include="*.js" --include="*.vue" 2>/dev/null | wc -l | tr -d ' ')

echo -e "  🔸 Remaining .id patterns: ${id_remaining}"
echo -e "  🔸 Remaining .name patterns: ${name_remaining}"

total_remaining=$((id_remaining + name_remaining))

if [ "$total_remaining" -lt 50 ]; then
    echo -e "  ✅ Good progress: ${GREEN}Only $total_remaining patterns remaining${NC}"
elif [ "$total_remaining" -lt 100 ]; then
    echo -e "  ⚠️  Moderate: ${YELLOW}$total_remaining patterns remaining${NC}"
else
    echo -e "  ❌ Many patterns: ${RED}$total_remaining patterns remaining${NC}"
fi

echo ""
echo "📋 5. DETAILED REMAINING ISSUES"
echo "-------------------------------"

# Show top files with most remaining issues
echo "  🔍 Files with most remaining .id/.name patterns:"
grep -r "\.id[^a-zA-Z]\|\.name[^a-zA-Z]" src/ --include="*.js" --include="*.vue" 2>/dev/null | \
cut -d: -f1 | sort | uniq -c | sort -nr | head -5 | while read count file; do
    echo -e "    📄 $file: ${count} patterns"
done

echo ""
echo "🚀 6. NEXT STEPS RECOMMENDATION"
echo "==============================="

if [ "$total_remaining" -lt 25 ]; then
    echo -e "🎉 Status: ${GREEN}EXCELLENT - Ready for final validation${NC}"
    echo "  ✅ Run comprehensive tests"
    echo "  ✅ Manual UI testing"
    echo "  ✅ Production readiness check"
elif [ "$total_remaining" -lt 75 ]; then
    echo -e "👍 Status: ${GREEN}GOOD - Minor cleanup needed${NC}"
    echo "  🔧 Manual review of remaining patterns"
    echo "  🔧 Fix edge cases if needed"
    echo "  ✅ Run validation tests"
else
    echo -e "⚠️  Status: ${YELLOW}NEEDS MORE WORK${NC}"
    echo "  🔧 Run this script again"
    echo "  🔧 Manual review of complex patterns"
    echo "  🔧 Consider more aggressive fixes"
fi

echo ""
echo "✅ Auto-fix complete!"
echo "📊 Summary: Processed views, stores, and components"
echo "🔍 Review backup files if any issues occur"
