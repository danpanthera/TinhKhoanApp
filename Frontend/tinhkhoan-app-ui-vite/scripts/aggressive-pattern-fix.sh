#!/bin/bash

# 🔧 TINH KHOAN APP - AGGRESSIVE PATTERN FIX
# Script để fix aggressive các pattern còn lại
# Date: 07/07/2025

echo "🔧 TINH KHOAN APP AGGRESSIVE PATTERN FIX"
echo "========================================"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Top problematic files from analysis
problem_files=(
    "src/views/KpiActualValuesView.vue"
    "src/views/dashboard/BusinessPlanDashboard.vue"
    "src/views/dashboard/CalculationDashboard.vue"
    "src/views/UnitKpiScoringView.vue"
    "src/views/UnitsView.vue"
)

# Function to add helper import if not exists
add_helper_import() {
    local file="$1"
    
    if ! grep -q "casingSafeAccess" "$file"; then
        echo -n "    📦 Adding helper import: "
        
        # Find the line with other imports
        import_line=$(grep -n "import.*from.*vue" "$file" | head -n1 | cut -d: -f1)
        
        if [ -n "$import_line" ]; then
            # Add helper import after Vue imports
            sed -i '' "${import_line}a\\
import { getId, getName, safeGet, ensurePascalCase } from \"../utils/casingSafeAccess.js\";
" "$file"
            echo -e "${GREEN}✅ Added${NC}"
        else
            echo -e "${YELLOW}⚠️  Could not find import location${NC}"
        fi
    else
        echo -e "    📦 Helper import: ${GREEN}Already exists${NC}"
    fi
}

# Function to fix patterns more aggressively
aggressive_fix() {
    local file="$1"
    
    echo -e "  🎯 Processing: ${BLUE}$(basename $file)${NC}"
    
    if [ ! -f "$file" ]; then
        echo -e "    ❌ File not found: ${RED}$file${NC}"
        return 1
    fi
    
    # Backup
    cp "$file" "${file}.backup.$(date +%Y%m%d_%H%M%S)"
    
    # Add helper import
    add_helper_import "$file"
    
    # Count before
    before_id=$(grep -c "\.Id[^a-zA-Z]" "$file" 2>/dev/null || echo 0)
    before_name=$(grep -c "\.Name[^a-zA-Z]" "$file" 2>/dev/null || echo 0)
    
    echo -n "    🔧 Applying fixes: "
    
    # More specific pattern fixes
    changes=0
    
    # Fix .id patterns (not already .Id)
    if sed -i '' 's/\([^\.]\)\.id\([^a-zA-Z]\)/\1.Id\2/g' "$file"; then
        changes=$((changes + 1))
    fi
    
    # Fix .name patterns (not already .Name)  
    if sed -i '' 's/\([^\.]\)\.name\([^a-zA-Z]\)/\1.Name\2/g' "$file"; then
        changes=$((changes + 1))
    fi
    
    # Fix other common patterns
    sed -i '' 's/\([^\.]\)\.type\([^a-zA-Z]\)/\1.Type\2/g' "$file"
    sed -i '' 's/\([^\.]\)\.status\([^a-zA-Z]\)/\1.Status\2/g' "$file"
    sed -i '' 's/\([^\.]\)\.code\([^a-zA-Z]\)/\1.Code\2/g' "$file"
    sed -i '' 's/\([^\.]\)\.value\([^a-zA-Z]\)/\1.Value\2/g' "$file"
    
    # Count after
    after_id=$(grep -c "\.Id[^a-zA-Z]" "$file" 2>/dev/null || echo 0)
    after_name=$(grep -c "\.Name[^a-zA-Z]" "$file" 2>/dev/null || echo 0)
    
    improvement_id=$((after_id - before_id))
    improvement_name=$((after_name - before_name))
    
    if [ "$improvement_id" -gt 0 ] || [ "$improvement_name" -gt 0 ]; then
        echo -e "${GREEN}✅ Improved (+$improvement_id .Id, +$improvement_name .Name)${NC}"
    else
        echo -e "${YELLOW}⚠️  No improvements detected${NC}"
    fi
    
    return 0
}

echo ""
echo "🎯 PROCESSING TOP PROBLEMATIC FILES"
echo "===================================="

for file in "${problem_files[@]}"; do
    if [ -f "$file" ]; then
        aggressive_fix "$file"
    else
        echo -e "  ❓ File not found: ${RED}$file${NC}"
    fi
done

echo ""
echo "🔍 VALIDATION AFTER AGGRESSIVE FIXES"
echo "===================================="

# Count remaining issues
id_remaining=$(grep -r "\.id[^a-zA-Z]" src/ --include="*.js" --include="*.vue" 2>/dev/null | wc -l | tr -d ' ')
name_remaining=$(grep -r "\.name[^a-zA-Z]" src/ --include="*.js" --include="*.vue" 2>/dev/null | wc -l | tr -d ' ')
total_remaining=$((id_remaining + name_remaining))

echo -e "📊 Remaining patterns: ${total_remaining} total"
echo -e "  🔸 .id patterns: ${id_remaining}"
echo -e "  🔸 .name patterns: ${name_remaining}"

if [ "$total_remaining" -lt 50 ]; then
    echo -e "🎉 Status: ${GREEN}EXCELLENT - Ready for validation${NC}"
elif [ "$total_remaining" -lt 100 ]; then
    echo -e "👍 Status: ${GREEN}GOOD - Minor cleanup needed${NC}"
else
    echo -e "⚠️  Status: ${YELLOW}STILL NEEDS WORK${NC}"
fi

echo ""
echo "🎯 FILES STILL NEEDING ATTENTION"
echo "================================="

# Show remaining problem files
grep -r "\.id[^a-zA-Z]\|\.name[^a-zA-Z]" src/ --include="*.js" --include="*.vue" 2>/dev/null | \
cut -d: -f1 | sort | uniq -c | sort -nr | head -10 | while read count file; do
    if [ "$count" -gt 5 ]; then
        echo -e "  📄 $file: ${count} patterns ${RED}(needs attention)${NC}"
    else
        echo -e "  📄 $file: ${count} patterns ${GREEN}(acceptable)${NC}"
    fi
done

echo ""
echo "✅ Aggressive fix complete!"
