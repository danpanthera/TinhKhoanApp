#!/bin/bash

# 🎯 TINH KHOAN APP - COMMIT CASING STANDARDIZATION
# Script để commit từng phần nhỏ của việc chuẩn hóa PascalCase
# Date: 07/07/2025

echo "📝 TINH KHOAN APP CASING STANDARDIZATION COMMIT"
echo "================================================"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Check if we're in a git repo
if [ ! -d ".git" ]; then
    echo -e "${RED}❌ Not in a git repository. Please run from project root.${NC}"
    exit 1
fi

echo "🔍 Checking git status..."
git status --porcelain

echo ""
echo "📦 COMMITTING CASING STANDARDIZATION CHANGES"
echo "============================================="

# Stage 1: Commit helper utility and scripts
echo "1️⃣ Committing helper utilities and scripts..."
git add src/utils/casingSafeAccess.js
git add scripts/casing-standardization-check.sh
git add scripts/auto-casing-fix.sh
git add public/all-fixes-summary.html

if git diff --staged --quiet; then
    echo -e "  ${YELLOW}⚠️  No utility changes to commit${NC}"
else
    git commit -m "feat: Add PascalCase standardization helper utilities

- Add casingSafeAccess.js helper for safe property access
- Add casing-standardization-check.sh for project analysis
- Add auto-casing-fix.sh for automated pattern fixes
- Update all-fixes-summary.html with progress tracking

Utilities provide fallback support for PascalCase/camelCase properties
and enable gradual migration from camelCase to PascalCase."
    echo -e "  ${GREEN}✅ Utilities committed${NC}"
fi

# Stage 2: Commit store improvements
echo ""
echo "2️⃣ Committing store standardization..."
git add src/stores/employeeStore.js
git add src/stores/unitStore.js
git add src/stores/khoanPeriodStore.js
git add src/stores/roleStore.js
git add src/stores/positionStore.js
git add src/stores/offlineStore.js
git add src/stores/themeStore.js

if git diff --staged --quiet; then
    echo -e "  ${YELLOW}⚠️  No store changes to commit${NC}"
else
    git commit -m "refactor: Standardize all stores to use PascalCase with helper

- Import casingSafeAccess helper in all stores
- Replace manual .Id||.id fallback with getId() helper
- Use normalizeArray() for consistent data normalization
- Apply safeGet() for reliable property access
- Maintain backward compatibility with camelCase

Stores now consistently handle PascalCase data from backend
while providing safe fallback for any remaining camelCase."
    echo -e "  ${GREEN}✅ Stores committed${NC}"
fi

# Stage 3: Commit view improvements
echo ""
echo "3️⃣ Committing view standardization..."
git add src/views/RolesView.vue
git add src/views/EmployeesView.vue

if git diff --staged --quiet; then
    echo -e "  ${YELLOW}⚠️  No view changes to commit${NC}"
else
    git commit -m "refactor: Update critical views to use PascalCase helpers

- Add casingSafeAccess import to EmployeesView and RolesView
- Replace manual property access with helper functions
- Update form binding to use PascalCase (Name, Description, Id)
- Fix extractEmployeePrimitives function to use safeGet()
- Update role dropdown to use getId() and getName()

Views now consistently use PascalCase while maintaining
compatibility with any remaining camelCase data."
    echo -e "  ${GREEN}✅ Views committed${NC}"
fi

# Stage 4: Check for remaining changes
echo ""
echo "4️⃣ Checking for remaining changes..."
remaining_changes=$(git status --porcelain | wc -l)

if [ "$remaining_changes" -gt 0 ]; then
    echo -e "  ${YELLOW}⚠️  $remaining_changes files still have uncommitted changes${NC}"
    echo "Remaining files:"
    git status --porcelain | head -10

    if [ "$remaining_changes" -gt 10 ]; then
        echo "... and $((remaining_changes - 10)) more files"
    fi

    echo ""
    echo "📋 Manual review recommended for:"
    echo "  - Complex template bindings"
    echo "  - Component prop passing"
    echo "  - Computed property logic"
    echo "  - API request/response handling"
else
    echo -e "  ${GREEN}✅ All major changes committed${NC}"
fi

echo ""
echo "📊 COMMIT SUMMARY"
echo "=================="

# Show recent commits
echo "Recent commits:"
git log --oneline -5

echo ""
echo "🎯 NEXT STEPS"
echo "============="
echo "1. 🔄 Test the application thoroughly"
echo "2. 🔄 Run standardization check again"
echo "3. 🔄 Fix any remaining complex patterns manually"
echo "4. 🔄 Update remaining views and components"
echo "5. ✅ Final testing and validation"

echo ""
echo "✅ Commit process complete!"
