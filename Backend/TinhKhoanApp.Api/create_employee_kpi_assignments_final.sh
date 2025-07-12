#!/bin/bash

# =============================================================================
# TẠOSYSTEMKPI ASSIGNMENTS CHO EMPLOYEES - FINAL VERSION
# Giao khoán KPI cho 10 employees dựa trên roles và KpiAssignmentTables
# =============================================================================

echo "🚀 BẮT ĐẦU TẠO EMPLOYEE KPI ASSIGNMENTS..."

# Database connection
SERVER="localhost,1433"
DATABASE="TinhKhoanDB"
USERNAME="sa"
PASSWORD="YourStrong@Password123"

# 1. Kiểm tra data hiện tại
echo "📊 1. KIỂM TRA DỮ LIỆU HIỆN TẠI..."

sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -Q "
SELECT 'Employees' as TableName, COUNT(*) as RecordCount FROM Employees
UNION ALL
SELECT 'Roles', COUNT(*) FROM Roles
UNION ALL
SELECT 'EmployeeRoles', COUNT(*) FROM EmployeeRoles
UNION ALL
SELECT 'KpiAssignmentTables', COUNT(*) FROM KpiAssignmentTables
UNION ALL
SELECT 'KpiIndicators', COUNT(*) FROM KpiIndicators
UNION ALL
SELECT 'EmployeeKpiAssignments', COUNT(*) FROM EmployeeKpiAssignments
UNION ALL
SELECT 'KhoanPeriods', COUNT(*) FROM KhoanPeriods;
"

# 2. Tạo EmployeeKpiAssignments dựa trên role mappings
echo "🎯 2. TẠO EMPLOYEE KPI ASSIGNMENTS..."

sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -Q "
-- Tạo EmployeeKpiAssignments cho employees có roles matching với KpiAssignmentTables
INSERT INTO EmployeeKpiAssignments (EmployeeId, KpiIndicatorId, KhoanPeriodId, TargetValue, ActualValue, PercentageAchieved, Status, IsActive, CreatedDate, UpdatedDate)
SELECT
    e.Id as EmployeeId,
    ki.Id as KpiIndicatorId,
    kp.Id as KhoanPeriodId,
    1000.00 as TargetValue,  -- Default target value
    0.00 as ActualValue,     -- Starting actual value
    0.00 as PercentageAchieved,
    'Active' as Status,
    1 as IsActive,
    GETDATE() as CreatedDate,
    GETDATE() as UpdatedDate
FROM Employees e
INNER JOIN EmployeeRoles er ON e.Id = er.EmployeeId
INNER JOIN Roles r ON er.RoleId = r.Id
INNER JOIN KpiAssignmentTables kat ON r.Name = kat.TableName  -- Match role name với table name
INNER JOIN KpiIndicators ki ON kat.Id = ki.TableId
CROSS JOIN (SELECT TOP 1 Id FROM KhoanPeriods WHERE Year = 2025 ORDER BY StartDate) kp
WHERE NOT EXISTS (
    SELECT 1 FROM EmployeeKpiAssignments eka
    WHERE eka.EmployeeId = e.Id AND eka.KpiIndicatorId = ki.Id AND eka.KhoanPeriodId = kp.Id
);

-- Báo cáo kết quả
SELECT 'EmployeeKpiAssignments created: ' + CAST(@@ROWCOUNT as VARCHAR(10)) as Result;
"

# 3. Verification và báo cáo
echo "✅ 3. VERIFICATION VÀ BÁO CÁO..."

sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -Q "
-- Thống kê assignments đã tạo
SELECT
    COUNT(*) as TotalAssignments,
    COUNT(DISTINCT EmployeeId) as UniqueEmployees,
    COUNT(DISTINCT KpiIndicatorId) as UniqueKpiIndicators
FROM EmployeeKpiAssignments;

-- Chi tiết assignments theo employee
SELECT
    e.FullName,
    r.Name as RoleName,
    kat.TableName,
    COUNT(eka.Id) as AssignmentCount
FROM Employees e
LEFT JOIN EmployeeRoles er ON e.Id = er.EmployeeId
LEFT JOIN Roles r ON er.RoleId = r.Id
LEFT JOIN KpiAssignmentTables kat ON r.Name = kat.TableName
LEFT JOIN KpiIndicators ki ON kat.Id = ki.TableId
LEFT JOIN EmployeeKpiAssignments eka ON e.Id = eka.EmployeeId AND ki.Id = eka.KpiIndicatorId
GROUP BY e.Id, e.FullName, r.Name, kat.TableName
ORDER BY e.FullName;
"

# 4. Test API endpoints
echo "🔗 4. TESTING API ENDPOINTS..."

echo "Testing EmployeeKpiAssignment API..."
curl -s "http://localhost:5055/api/EmployeeKpiAssignment" | head -200

echo ""
echo "✅ HOÀN THÀNH TẠO EMPLOYEE KPI ASSIGNMENTS!"
echo "📊 Kiểm tra kết quả tại: http://localhost:5055/api/EmployeeKpiAssignment"
