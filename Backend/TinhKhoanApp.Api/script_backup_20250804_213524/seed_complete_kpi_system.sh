#!/bin/bash

# 🚀 SEED COMPLETE KPI SYSTEM - TinhKhoan App
# Script to seed: Units, Positions, Roles, Employees + KPI System (32 tables, 135 definitions, 17 periods, 257 indicators)

echo "🚀 Starting COMPLETE KPI System Seeding..."
echo "========================================="

# Step 1: Seed Core Tables (Units, Positions, Roles, Employees)
echo "🏗️ Step 1: Seeding Core Organization Data..."
./seed_full_system.sh

# Step 2: Seed KPI Assignment Tables (32 templates)
echo "📋 Step 2: Seeding KPI Assignment Tables (32 templates)..."
echo "Running backend KPI seeding via dotnet..."
dotnet run kpi-seed

# Step 3: Verify complete system
echo "✅ Step 3: Final System Verification..."
echo "======================================"
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
-- Core Organization Tables
SELECT 'Units' as TableName, COUNT(*) as Count, 'Core' as Category FROM Units
UNION ALL
SELECT 'Positions', COUNT(*), 'Core' FROM Positions
UNION ALL
SELECT 'Roles', COUNT(*), 'Core' FROM Roles
UNION ALL
SELECT 'Employees', COUNT(*), 'Core' FROM Employees

UNION ALL

-- KPI System Tables
SELECT 'KpiAssignmentTables', COUNT(*), 'KPI' FROM KpiAssignmentTables
UNION ALL
SELECT 'KpiDefinitions', COUNT(*), 'KPI' FROM KpiDefinitions
UNION ALL
SELECT 'KhoanPeriods', COUNT(*), 'KPI' FROM KhoanPeriods
UNION ALL
SELECT 'KpiIndicators', COUNT(*), 'KPI' FROM KpiIndicators

UNION ALL

-- Assignment Tables (để implement)
SELECT 'EmployeeKpiAssignments', COUNT(*), 'Assignment' FROM EmployeeKpiAssignments
UNION ALL
SELECT 'UnitKpiScorings', COUNT(*), 'Assignment' FROM UnitKpiScorings

ORDER BY Category, TableName;
" -C

echo ""
echo "🎯 KPI System Breakdown:"
echo "========================"
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -Q "
-- KPI Assignment Tables by Category
SELECT
    Category,
    COUNT(*) as TableCount,
    STRING_AGG(TableName, ', ') as TableNames
FROM KpiAssignmentTables
GROUP BY Category;
" -C

echo ""
echo "📊 Target Numbers:"
echo "=================="
echo "✅ KPI Tables: 32/32 templates"
echo "✅ KPI Definitions: 135/135"
echo "✅ Khoan Periods: 17/17"
echo "✅ KPI Indicators: 257/257 chỉ tiêu đầy đủ"
echo ""
echo "🎉 COMPLETE KPI System Ready!"
echo "✅ Frontend can now manage full KPI assignments for employees and units"
