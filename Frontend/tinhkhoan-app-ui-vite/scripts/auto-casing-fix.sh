#!/bin/bash

# 🎯 TINH KHOAN APP - AUTO CASING FIX SCRIPT
# Script để tự động sửa các pattern camelCase thành PascalCase
# Date: 07/07/2025

echo "🔧 TINH KHOAN APP AUTO CASING FIX"
echo "================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Backup directory
BACKUP_DIR="backup_$(date +%Y%m%d_%H%M%S)"
mkdir -p "$BACKUP_DIR"

echo "📂 Creating backup in: $BACKUP_DIR"

# List of critical files to fix
FILES_TO_FIX=(
    "src/views/EmployeeKpiAssignmentView.vue"
    "src/views/UnitKpiAssignmentView.vue"
    "src/views/KpiDefinitionsView.vue"
    "src/stores/offlineStore.js"
    "src/stores/themeStore.js"
)

# Function to backup and fix a file
fix_file() {
    local file="$1"
    local description="$2"

    if [ ! -f "$file" ]; then
        echo -e "  ❌ File not found: ${RED}$file${NC}"
        return 1
    fi

    echo -e "  🔧 Processing: ${BLUE}$file${NC} - $description"

    # Create backup
    cp "$file" "$BACKUP_DIR/$(basename $file).backup"

    # Count issues before fix
    before_count=$(grep -c "\.id[^a-zA-Z]\|\.name[^a-zA-Z]\|\.type[^a-zA-Z]\|\.status[^a-zA-Z]" "$file" 2>/dev/null || echo 0)

    # Apply fixes
    sed -i '' 's/\.id\([^a-zA-Z]\)/\.Id\1/g' "$file"
    sed -i '' 's/\.name\([^a-zA-Z]\)/\.Name\1/g' "$file"
    sed -i '' 's/\.type\([^a-zA-Z]\)/\.Type\1/g' "$file"
    sed -i '' 's/\.status\([^a-zA-Z]\)/\.Status\1/g' "$file"

    # Fix specific Vue patterns
    if [[ "$file" == *.vue ]]; then
        # Fix v-model bindings
        sed -i '' 's/:value="[^"]*\.id"/:value="getId(&)"/g' "$file" 2>/dev/null || true
        sed -i '' 's/:value="[^"]*\.name"/:value="getName(&)"/g' "$file" 2>/dev/null || true

        # Fix template bindings
        sed -i '' 's/{{ *[^}]*\.id *}}/{{ getId(&) }}/g' "$file" 2>/dev/null || true
        sed -i '' 's/{{ *[^}]*\.name *}}/{{ getName(&) }}/g' "$file" 2>/dev/null || true
    fi

    # Count issues after fix
    after_count=$(grep -c "\.id[^a-zA-Z]\|\.name[^a-zA-Z]\|\.type[^a-zA-Z]\|\.status[^a-zA-Z]" "$file" 2>/dev/null || echo 0)

    local fixed_count=$((before_count - after_count))

    if [ "$fixed_count" -gt 0 ]; then
        echo -e "    ${GREEN}✅ Fixed $fixed_count patterns${NC}"
    else
        echo -e "    ${YELLOW}⚠️  No patterns found to fix${NC}"
    fi
}

echo ""
echo "🔄 1. FIXING CRITICAL VIEW FILES"
echo "--------------------------------"

fix_file "src/views/EmployeeKpiAssignmentView.vue" "Employee KPI Assignment View"
fix_file "src/views/UnitKpiAssignmentView.vue" "Unit KPI Assignment View"
fix_file "src/views/KpiDefinitionsView.vue" "KPI Definitions View"

echo ""
echo "🔄 2. FIXING REMAINING STORES"
echo "-----------------------------"

fix_file "src/stores/offlineStore.js" "Offline Store"
fix_file "src/stores/themeStore.js" "Theme Store"

echo ""
echo "🔄 3. ADDING HELPER IMPORTS"
echo "---------------------------"

# Files that need helper imports
STORES_NEED_IMPORT=(
    "src/stores/offlineStore.js"
    "src/stores/themeStore.js"
)

for store in "${STORES_NEED_IMPORT[@]}"; do
    if [ -f "$store" ]; then
        echo -e "  📦 Adding helper import to: ${BLUE}$store${NC}"

        # Check if import already exists
        if ! grep -q "casingSafeAccess" "$store"; then
            # Add import after existing imports
            sed -i '' '1a\
import { normalizeArray, getId, getName, getType, getStatus } from "../utils/casingSafeAccess.js";
' "$store"
            echo -e "    ${GREEN}✅ Import added${NC}"
        else
            echo -e "    ${YELLOW}⚠️  Import already exists${NC}"
        fi
    fi
done

echo ""
echo "🔄 4. FIXING SPECIFIC PATTERNS"
echo "------------------------------"

# Fix specific common patterns
echo "  🔸 Fixing .Id !== undefined patterns..."
find src/ -name "*.js" -o -name "*.vue" | xargs sed -i '' 's/\.id !== undefined/getId(&) !== null/g' 2>/dev/null || true

echo "  🔸 Fixing findIndex patterns..."
find src/ -name "*.js" -o -name "*.vue" | xargs sed -i '' 's/\.findIndex((.*) => .*\.id === /\.findIndex(\1 => getId(\1) === /g' 2>/dev/null || true

echo "  🔸 Fixing filter patterns..."
find src/ -name "*.js" -o -name "*.vue" | xargs sed -i '' 's/\.filter((.*) => .*\.id !== /\.filter(\1 => getId(\1) !== /g' 2>/dev/null || true

echo ""
echo "📊 5. VERIFICATION"
echo "------------------"

total_issues=0
for file in "${FILES_TO_FIX[@]}"; do
    if [ -f "$file" ]; then
        issues=$(grep -c "\.id[^a-zA-Z]\|\.name[^a-zA-Z]\|\.type[^a-zA-Z]\|\.status[^a-zA-Z]" "$file" 2>/dev/null || echo 0)
        total_issues=$((total_issues + issues))

        if [ "$issues" -eq 0 ]; then
            echo -e "  ${GREEN}✅ $(basename $file): Clean${NC}"
        else
            echo -e "  ${YELLOW}⚠️  $(basename $file): $issues remaining issues${NC}"
        fi
    fi
done

echo ""
echo "🎯 SUMMARY"
echo "=========="

if [ "$total_issues" -eq 0 ]; then
    echo -e "${GREEN}🎉 All targeted files have been fixed!${NC}"
    echo -e "📂 Backup saved in: ${BLUE}$BACKUP_DIR${NC}"
    echo ""
    echo "📋 Next steps:"
    echo "  1. Test the application to ensure all changes work"
    echo "  2. Run the standardization check script again"
    echo "  3. Fix any remaining issues in other files"
    echo "  4. Remove backup if everything works correctly"
else
    echo -e "${YELLOW}⚠️  $total_issues patterns still need manual review${NC}"
    echo -e "📂 Backup saved in: ${BLUE}$BACKUP_DIR${NC}"
    echo ""
    echo "📋 Manual fixes needed for complex patterns"
fi

echo ""
echo "✅ Auto-fix complete!"
