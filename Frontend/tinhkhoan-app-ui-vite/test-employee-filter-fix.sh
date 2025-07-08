#!/bin/bash

echo "🧪 Testing Employee Filter & Unit KPI Dropdown Fixes"
echo "================================================="

echo ""
echo "1. 🔍 Testing Employee API data structure..."
curl -s "http://localhost:5055/api/employees" | jq '.[0] | {Id, FullName, UnitId, UnitName}' | head -n 10

echo ""
echo "2. 🏢 Testing Units API data structure..."
curl -s "http://localhost:5055/api/units" | jq '.[] | select(.Type == "CNL1" or .Type == "CNL2") | {Id, Name, Type, ParentUnitId}' | head -n 20

echo ""
echo "3. 📊 Testing Branch Employee Count..."
echo "Getting employee count per branch (by UnitId)..."

# Get all employee UnitIds and count them
curl -s "http://localhost:5055/api/employees" | jq -r '.[].UnitId' | sort | uniq -c | sort -nr | head -n 10

echo ""
echo "4. 🎯 Testing specific branch filter (Hội Sở - ID=2)..."
# Test filtering for Hội Sở (ID=2)
EMPLOYEES_IN_HOI_SO=$(curl -s "http://localhost:5055/api/employees" | jq '[.[] | select(.UnitId == 2 or (.UnitId | tostring | startswith("3") or startswith("4") or startswith("5") or startswith("6") or startswith("7") or startswith("8") or startswith("9")))]')
echo "Employees directly in Hội Sở (ID=2) and its departments:"
echo "$EMPLOYEES_IN_HOI_SO" | jq '.[].FullName' | wc -l

echo ""
echo "5. 🏢 Testing CNL2 branch filter (Bình Lư - ID=10)..."
# Test filtering for Bình Lư (ID=10)
EMPLOYEES_IN_BINH_LU=$(curl -s "http://localhost:5055/api/employees" | jq '[.[] | select(.UnitId == 10 or (.UnitId | tostring | startswith("18") or startswith("19") or startswith("20")))]')
echo "Employees in Bình Lư (ID=10) and its departments:"
echo "$EMPLOYEES_IN_BINH_LU" | jq '.[].FullName' | wc -l

echo ""
echo "✅ Test completed! Check the UI to verify filtering works."
echo "🌐 Open: http://localhost:5173/employee-kpi-assignment"
echo "🌐 Open: http://localhost:5173/unit-kpi-assignment"
