#!/bin/bash

# PascalCase Review Script for TinhKhoanApp
# This script scans for camelCase properties that should be PascalCase
# and suggests files that may need review

echo "🔍 Starting PascalCase consistency review..."

# Navigate to project root
cd "$(dirname "$0")"

# Create report directory
mkdir -p reports
REPORT_FILE="reports/pascalcase_review_$(date +%Y%m%d_%H%M%S).txt"

echo "TinhKhoanApp PascalCase Consistency Review" > "$REPORT_FILE"
echo "Generated: $(date)" >> "$REPORT_FILE"
echo "---------------------------------------------" >> "$REPORT_FILE"

# Find Vue files that don't use safeGet but access properties directly
echo -e "\n🔎 Scanning for direct property access that should use safeGet..." | tee -a "$REPORT_FILE"
grep -r --include="*.vue" --include="*.js" -E '(\.[a-z][a-zA-Z0-9]*\b)' src/ | grep -v "safeGet" | grep -v "import" | grep -v "require" | grep -v "computed" | grep -v "function" | head -n 50 >> "$REPORT_FILE"

# Find inconsistent casing (both PascalCase and camelCase versions used)
echo -e "\n🔎 Scanning for inconsistent casing usage..." | tee -a "$REPORT_FILE"
PATTERNS=(
  "id/Id"
  "name/Name"
  "startDate/StartDate"
  "endDate/EndDate"
  "employeeId/EmployeeId"
  "unitId/UnitId"
  "indicators/Indicators"
  "description/Description"
  "status/Status"
  "type/Type"
  "code/Code"
  "value/Value"
  "isActive/IsActive"
)

for pattern in "${PATTERNS[@]}"; do
  camel=$(echo $pattern | cut -d'/' -f1)
  pascal=$(echo $pattern | cut -d'/' -f2)
  
  echo -e "\n🔍 Checking $camel vs $pascal usage:" | tee -a "$REPORT_FILE"
  grep -r --include="*.vue" --include="*.js" -E "\\.$camel\\b|\\.$pascal\\b|'$camel'|'$pascal'" src/ | grep -v "safeGet" | grep -v "import" | grep -v "require" | head -n 10 >> "$REPORT_FILE"
done

# Files that might need comprehensive review
echo -e "\n📋 Files that may need comprehensive review:" | tee -a "$REPORT_FILE"
for file in $(grep -l -r --include="*.vue" --include="*.js" -E '(\.[a-z][a-zA-Z0-9]*\b)' src/ | grep -v "casingSafeAccess.js" | sort | uniq); do
  if ! grep -q "safeGet" "$file"; then
    echo "⚠️ $file (no safeGet usage)" >> "$REPORT_FILE"
  fi
done

# API response handlers that might need review
echo -e "\n📋 API handlers that may need review:" | tee -a "$REPORT_FILE"
grep -r --include="*.vue" --include="*.js" -E "\.then\((.*)\s=>\s{" src/ | head -n 20 >> "$REPORT_FILE"

echo -e "\n✅ Review completed. Report saved to $REPORT_FILE"
echo "📝 Manually review the report and use safeGet for property access to ensure casing consistency"
