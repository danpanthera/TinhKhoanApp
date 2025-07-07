#!/bin/bash

# 🎯 TINH KHOAN APP - COMPONENT CASING FIX
# Script để fix camelCase patterns trong components
# Date: 07/07/2025

echo "🔧 COMPONENT CASING FIX"
echo "======================"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Backup directory
BACKUP_DIR="component_backup_$(date +%Y%m%d_%H%M%S)"
mkdir -p "$BACKUP_DIR"

echo "📂 Creating backup in: $BACKUP_DIR"

# Find components with camelCase patterns
components_with_issues=$(grep -r "\.id\|\.name" src/components/ --include="*.vue" -l)

if [ -z "$components_with_issues" ]; then
    echo -e "${GREEN}✅ No camelCase patterns found in components${NC}"
    exit 0
fi

echo "🔍 Found components with camelCase patterns:"
echo "$components_with_issues"
echo ""

# Fix each component
while IFS= read -r component; do
    if [ -f "$component" ]; then
        echo -e "🔧 Processing: ${BLUE}$component${NC}"

        # Create backup
        cp "$component" "$BACKUP_DIR/$(basename $component).backup"

        # Count before
        before_count=$(grep -c "\.id\|\.name\|\.type\|\.status" "$component" 2>/dev/null || echo 0)

        # Apply targeted fixes for common patterns
        # Fix .name patterns (but not .name() method calls)
        sed -i '' 's/\.name\([^(a-zA-Z]\|$\)/.Name\1/g' "$component"

        # Fix .id patterns (but be careful with .id() method calls)
        sed -i '' 's/\.id\([^(a-zA-Z]\|$\)/.Id\1/g' "$component"

        # Count after
        after_count=$(grep -c "\.id\|\.name\|\.type\|\.status" "$component" 2>/dev/null || echo 0)

        fixed_count=$((before_count - after_count))

        if [ "$fixed_count" -gt 0 ]; then
            echo -e "  ${GREEN}✅ Fixed $fixed_count patterns${NC}"
        else
            echo -e "  ${YELLOW}⚠️  No changes made${NC}"
        fi
    fi
done <<< "$components_with_issues"

echo ""
echo "📊 VERIFICATION"
echo "==============="

# Check remaining issues
remaining_issues=$(grep -r "\.id\|\.name" src/components/ --include="*.vue" | wc -l)

if [ "$remaining_issues" -eq 0 ]; then
    echo -e "${GREEN}🎉 All component camelCase patterns fixed!${NC}"
else
    echo -e "${YELLOW}⚠️  $remaining_issues patterns still need manual review${NC}"
    echo ""
    echo "Remaining patterns:"
    grep -r "\.id\|\.name" src/components/ --include="*.vue" | head -5
fi

echo ""
echo -e "📂 Backup saved in: ${BLUE}$BACKUP_DIR${NC}"
echo "✅ Component fix complete!"
