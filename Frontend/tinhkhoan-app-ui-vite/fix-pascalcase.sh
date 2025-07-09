#!/bin/bash

# PascalCase Fixer Script for TinhKhoanApp
# This script modifies files to use safeGet for consistent property access

echo "🔧 Starting PascalCase fixer..."

# Navigate to project root
cd "$(dirname "$0")"

# Files to process - add files that need attention here
FILES_TO_FIX=(
  "src/views/UnitKpiAssignmentView.vue"
  "src/api/employeeKpiAssignmentApi.js"
  # Add more files as needed based on review results
)

for file in "${FILES_TO_FIX[@]}"; do
  if [ -f "$file" ]; then
    echo "🔧 Processing file: $file"

    # Create backup
    cp "$file" "${file}.bak"
    echo "   📑 Created backup: ${file}.bak"

    # Check if file already imports safeGet
    if ! grep -q "import.*safeGet" "$file"; then
      # Add import if needed
      if grep -q "import.*from.*utils" "$file"; then
        # If already importing from utils, modify existing import
        sed -i '' -E 's/import \{(.*)\} from "..\/utils\/(.*)"./import \{\1, safeGet\} from "..\/utils\/\2"./g' "$file"
      else
        # Add new import
        sed -i '' '1s/^/<script setup>\nimport { safeGet } from "..\/utils\/casingSafeAccess.js";\n/' "$file"
      fi
      echo "   ➕ Added safeGet import"
    fi

    echo "   ✅ File processed"
  else
    echo "❌ File not found: $file"
  fi
done

echo "✨ Fixing completed. Review the changes manually and test thoroughly."
echo "📝 Note: This script only adds safeGet imports. You'll need to manually replace direct property access with safeGet calls."
