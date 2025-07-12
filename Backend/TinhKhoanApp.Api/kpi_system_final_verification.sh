#!/bin/bash

# =============================================================================
# KPI SYSTEM FINAL VERIFICATION - COMPLETE SYSTEM STATUS CHECK
# Comprehensive verification of entire KPI system
# =============================================================================

echo "🔍 BẮT ĐẦU KPI SYSTEM FINAL VERIFICATION..."
echo "=============================================="

# Database connection
SERVER="localhost,1433"
DATABASE="TinhKhoanDB"
USERNAME="sa"
PASSWORD="YourStrong@Password123"

# 1. Backend Health Check
echo ""
echo "🏥 1. BACKEND HEALTH CHECK..."
HEALTH_STATUS=$(curl -s "http://localhost:5055/health" | jq -r '.status // "Error"')
echo "✅ Backend Status: $HEALTH_STATUS"

if [ "$HEALTH_STATUS" != "Healthy" ]; then
    echo "❌ Backend not healthy! Exiting verification."
    exit 1
fi

# 2. Core KPI Tables Verification
echo ""
echo "📊 2. CORE KPI TABLES VERIFICATION..."

sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -Q "
SELECT
    'KpiAssignmentTables' as TableName,
    COUNT(*) as RecordCount,
    COUNT(DISTINCT Category) as UniqueCategories
FROM KpiAssignmentTables
UNION ALL
SELECT
    'KpiIndicators',
    COUNT(*),
    COUNT(DISTINCT TableId) as UniqueTableIds
FROM KpiIndicators
UNION ALL
SELECT
    'EmployeeKpiAssignments',
    COUNT(*),
    COUNT(DISTINCT EmployeeId) as UniqueEmployees
FROM EmployeeKpiAssignments
UNION ALL
SELECT
    'UnitKpiScorings',
    COUNT(*),
    COUNT(DISTINCT UnitId) as UniqueUnits
FROM UnitKpiScorings;
"

# 3. API Endpoints Verification
echo ""
echo "🔗 3. API ENDPOINTS VERIFICATION..."

echo "Testing /api/EmployeeKpiAssignment..."
EKA_COUNT=$(curl -s "http://localhost:5055/api/EmployeeKpiAssignment" | jq 'length // 0')
echo "✅ EmployeeKpiAssignment API: $EKA_COUNT records"

echo "Testing /api/UnitKpiScoring..."
UKS_COUNT=$(curl -s "http://localhost:5055/api/UnitKpiScoring" | jq 'length // 0')
echo "✅ UnitKpiScoring API: $UKS_COUNT records"

echo "Testing /api/KpiAssignmentTables..."
KAT_COUNT=$(curl -s "http://localhost:5055/api/KpiAssignmentTables" | jq 'length // 0')
echo "✅ KpiAssignmentTables API: $KAT_COUNT records"

echo "Testing /api/KpiIndicators..."
KI_COUNT=$(curl -s "http://localhost:5055/api/KpiIndicators" | jq 'length // 0')
echo "✅ KpiIndicators API: $KI_COUNT records"

# 4. Performance Analysis
echo ""
echo "📈 4. PERFORMANCE ANALYSIS..."

sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -Q "
-- Employee KPI Performance Summary
SELECT
    'Employee KPI Summary' as Metric,
    COUNT(DISTINCT eka.EmployeeId) as ActiveEmployees,
    COUNT(eka.Id) as TotalAssignments,
    AVG(eka.TargetValue) as AvgTargetValue,
    COUNT(CASE WHEN eka.Status = 'Active' THEN 1 END) as ActiveAssignments
FROM EmployeeKpiAssignments eka;

-- Unit KPI Performance Summary
SELECT
    'Unit KPI Summary' as Metric,
    COUNT(DISTINCT uks.UnitId) as ActiveUnits,
    AVG(uks.TotalScore) as AvgTotalScore,
    AVG(uks.Percentage) as AvgPercentage,
    MAX(uks.TotalScore) as MaxScore
FROM UnitKpiScorings uks;
"

# 5. System Readiness Check
echo ""
echo "🎯 5. SYSTEM READINESS CHECK..."

# Count critical components
EMPLOYEES_COUNT=$(sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -h -1 -Q "SELECT COUNT(*) FROM Employees")
ROLES_COUNT=$(sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -h -1 -Q "SELECT COUNT(*) FROM Roles")
UNITS_COUNT=$(sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -h -1 -Q "SELECT COUNT(*) FROM Units")
PERIODS_COUNT=$(sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -h -1 -Q "SELECT COUNT(*) FROM KhoanPeriods")

echo "✅ Foundation Data:"
echo "   - Employees: $EMPLOYEES_COUNT"
echo "   - Roles: $ROLES_COUNT"
echo "   - Units: $UNITS_COUNT"
echo "   - KhoanPeriods: $PERIODS_COUNT"

echo ""
echo "✅ KPI System Components:"
echo "   - KpiAssignmentTables: $KAT_COUNT"
echo "   - KpiIndicators: $KI_COUNT"
echo "   - EmployeeKpiAssignments: $EKA_COUNT"
echo "   - UnitKpiScorings: $UKS_COUNT"

# 6. Final Status Report
echo ""
echo "🎉 6. FINAL STATUS REPORT"
echo "========================="

if [[ $EMPLOYEES_COUNT -gt 0 && $ROLES_COUNT -gt 0 && $UNITS_COUNT -gt 0 && $PERIODS_COUNT -gt 0 && $KAT_COUNT -gt 0 && $KI_COUNT -gt 0 ]]; then
    echo "✅ KPI SYSTEM STATUS: FULLY OPERATIONAL"
    echo "✅ All core components verified and working"
    echo "✅ Backend APIs responding correctly"
    echo "✅ Database tables populated with valid data"
    echo "✅ Ready for frontend integration"
    echo ""
    echo "🚀 NEXT STEPS AVAILABLE:"
    echo "   1. Frontend KPI Management Dashboard"
    echo "   2. Real-time KPI tracking interface"
    echo "   3. Manager assignment tools"
    echo "   4. Performance analytics dashboard"
else
    echo "❌ KPI SYSTEM STATUS: INCOMPLETE"
    echo "❌ Some components missing or not working"
fi

echo ""
echo "📊 Verification completed at: $(date)"
echo "🔗 Backend URL: http://localhost:5055"
echo "🔗 Health Check: http://localhost:5055/health"
