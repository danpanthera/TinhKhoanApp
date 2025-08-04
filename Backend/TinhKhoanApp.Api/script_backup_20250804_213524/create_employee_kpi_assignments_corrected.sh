#!/bin/bash

# =============================================================================
# TẠO EMPLOYEE KPI ASSIGNMENTS - CORRECT SCHEMA VERSION
# Giao khoán KPI cho employees với đúng column names
# =============================================================================

echo "🚀 BẮT ĐẦU TẠO EMPLOYEE KPI ASSIGNMENTS (CORRECTED)..."

# Database connection
SERVER="localhost,1433"
DATABASE="TinhKhoanDB"
USERNAME="sa"
PASSWORD="YourStrong@Password123"

# 1. Kiểm tra role name mapping
echo "📊 1. KIỂM TRA ROLE NAME MAPPING..."

sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -Q "
-- Kiểm tra mapping giữa roles và KpiAssignmentTables
SELECT
    r.Id as RoleId,
    r.Name as RoleName,
    kat.Id as TableId,
    kat.TableName,
    kat.Category,
    ki.IndicatorName
FROM Roles r
LEFT JOIN KpiAssignmentTables kat ON r.Name = kat.TableName
LEFT JOIN KpiIndicators ki ON kat.Id = ki.TableId
WHERE kat.Category = 'EMPLOYEE'
ORDER BY r.Id;
"

# 2. Tạo EmployeeKpiAssignments với correct column names
echo "🎯 2. TẠO EMPLOYEE KPI ASSIGNMENTS..."

sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -Q "
-- Tạo EmployeeKpiAssignments cho employees có role matching với KpiAssignmentTables
INSERT INTO EmployeeKpiAssignments (EmployeeId, KpiIndicatorId, KhoanPeriodId, TargetValue, ActualValue, Status, AssignedAt, UpdatedAt)
SELECT
    e.Id as EmployeeId,
    ki.Id as KpiIndicatorId,
    kp.Id as KhoanPeriodId,
    1000.00 as TargetValue,  -- Default target value
    0.00 as ActualValue,     -- Starting actual value
    'Active' as Status,
    GETDATE() as AssignedAt,
    GETDATE() as UpdatedAt
FROM Employees e
INNER JOIN EmployeeRoles er ON e.Id = er.EmployeeId
INNER JOIN Roles r ON er.RoleId = r.Id
INNER JOIN KpiAssignmentTables kat ON r.Name = kat.TableName  -- Match role name với table name
INNER JOIN KpiIndicators ki ON kat.Id = ki.TableId
CROSS JOIN (SELECT TOP 1 Id FROM KhoanPeriods WHERE YEAR(StartDate) = 2025 ORDER BY StartDate) kp
WHERE kat.Category = 'EMPLOYEE'  -- Chỉ employee assignments
AND NOT EXISTS (
    SELECT 1 FROM EmployeeKpiAssignments eka
    WHERE eka.EmployeeId = e.Id AND eka.KpiIndicatorId = ki.Id AND eka.KhoanPeriodId = kp.Id
);

-- Báo cáo kết quả
SELECT 'EmployeeKpiAssignments created: ' + CAST(@@ROWCOUNT as VARCHAR(10)) as Result;
"

# 3. Verification và báo cáo chi tiết
echo "✅ 3. VERIFICATION VÀ BÁO CÁO..."

sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -Q "
-- Thống kê assignments đã tạo
SELECT
    COUNT(*) as TotalAssignments,
    COUNT(DISTINCT EmployeeId) as UniqueEmployees,
    COUNT(DISTINCT KpiIndicatorId) as UniqueKpiIndicators,
    COUNT(DISTINCT KhoanPeriodId) as UniquePeriods
FROM EmployeeKpiAssignments;

-- Chi tiết assignments theo employee
SELECT
    e.FullName,
    r.Name as RoleName,
    COUNT(eka.Id) as AssignmentCount,
    STRING_AGG(ki.IndicatorName, '; ') as KpiIndicators
FROM Employees e
INNER JOIN EmployeeRoles er ON e.Id = er.EmployeeId
INNER JOIN Roles r ON er.RoleId = r.Id
LEFT JOIN KpiAssignmentTables kat ON r.Name = kat.TableName
LEFT JOIN KpiIndicators ki ON kat.Id = ki.TableId
LEFT JOIN EmployeeKpiAssignments eka ON e.Id = eka.EmployeeId AND ki.Id = eka.KpiIndicatorId
WHERE kat.Category = 'EMPLOYEE' OR kat.Category IS NULL
GROUP BY e.Id, e.FullName, r.Name
ORDER BY e.FullName;
"

# 4. Test API endpoints
echo "🔗 4. TESTING API ENDPOINTS..."

echo "Testing EmployeeKpiAssignment count..."
curl -s "http://localhost:5055/api/EmployeeKpiAssignment" | jq 'length // "Error occurred"'

echo ""
echo "✅ HOÀN THÀNH TẠO EMPLOYEE KPI ASSIGNMENTS!"
echo "📊 Tổng assignments: $(sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -h -1 -Q "SELECT COUNT(*) FROM EmployeeKpiAssignments")"
