#!/bin/bash

# =============================================================================
# TẠO EMPLOYEE KPI ASSIGNMENTS - WITH MANUAL MAPPING
# Map specific roles to KPI tables dựa trên business logic
# =============================================================================

echo "🚀 BẮT ĐẦU TẠO EMPLOYEE KPI ASSIGNMENTS (MANUAL MAPPING)..."

# Database connection
SERVER="localhost,1433"
DATABASE="TinhKhoanDB"
USERNAME="sa"
PASSWORD="YourStrong@Password123"

# 1. Tạo manual mapping và assignments
echo "🎯 1. TẠO EMPLOYEE KPI ASSIGNMENTS VỚI MANUAL MAPPING..."

sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -Q "
-- Tạo EmployeeKpiAssignments với manual mapping dựa trên role names
INSERT INTO EmployeeKpiAssignments (EmployeeId, KpiIndicatorId, KhoanPeriodId, TargetValue, ActualValue, Status, AssignedAt, UpdatedAt)
SELECT
    e.Id as EmployeeId,
    ki.Id as KpiIndicatorId,
    kp.Id as KhoanPeriodId,
    CASE
        WHEN r.Name LIKE '%Trưởng phòng%' THEN 1500.00
        WHEN r.Name LIKE '%Phó phòng%' THEN 1200.00
        WHEN r.Name LIKE '%Giám đốc%' THEN 2000.00
        WHEN r.Name LIKE '%GDV%' THEN 800.00
        WHEN r.Name LIKE '%Cán bộ%' THEN 1000.00
        ELSE 1000.00
    END as TargetValue,
    0.00 as ActualValue,
    'Active' as Status,
    GETDATE() as AssignedAt,
    GETDATE() as UpdatedAt
FROM Employees e
INNER JOIN EmployeeRoles er ON e.Id = er.EmployeeId
INNER JOIN Roles r ON er.RoleId = r.Id
CROSS JOIN KpiIndicators ki
CROSS JOIN (SELECT TOP 1 Id FROM KhoanPeriods WHERE YEAR(StartDate) = 2025 ORDER BY StartDate) kp
WHERE
    -- Map roles to appropriate KPI indicators
    (
        (r.Name = 'Trưởng phòng KHDN' AND ki.TableId = (SELECT Id FROM KpiAssignmentTables WHERE TableName = 'TruongphongKhdn')) OR
        (r.Name = 'Trưởng phòng KHCN' AND ki.TableId = (SELECT Id FROM KpiAssignmentTables WHERE TableName = 'TruongphongKhcn')) OR
        (r.Name = 'Phó phòng KHDN' AND ki.TableId = (SELECT Id FROM KpiAssignmentTables WHERE TableName = 'PhophongKhdn')) OR
        (r.Name = 'Trưởng phòng KH&QLRR' AND ki.TableId = (SELECT Id FROM KpiAssignmentTables WHERE TableName = 'TruongphongKhqlrr')) OR
        (r.Name = 'Trưởng phòng KTNQ CNL1' AND ki.TableId = (SELECT Id FROM KpiAssignmentTables WHERE TableName = 'TruongphongKtnqCnl1')) OR
        (r.Name = 'Cán bộ tín dụng' AND ki.TableId = (SELECT Id FROM KpiAssignmentTables WHERE TableName = 'Cbtd')) OR
        (r.Name = 'GDV' AND ki.TableId = (SELECT Id FROM KpiAssignmentTables WHERE TableName = 'Gdv')) OR
        (r.Name = 'Giám đốc CNL2' AND ki.TableId = (SELECT Id FROM KpiAssignmentTables WHERE TableName = 'ChinhanhCap2')) OR
        (r.Name = 'Phó giám đốc CNL2 phụ trách TD' AND ki.TableId = (SELECT Id FROM KpiAssignmentTables WHERE TableName = 'PhongchucnangCap2'))
    )
AND NOT EXISTS (
    SELECT 1 FROM EmployeeKpiAssignments eka
    WHERE eka.EmployeeId = e.Id AND eka.KpiIndicatorId = ki.Id AND eka.KhoanPeriodId = kp.Id
);

-- Báo cáo kết quả
SELECT 'EmployeeKpiAssignments created: ' + CAST(@@ROWCOUNT as VARCHAR(10)) as Result;
"

# 2. Verification chi tiết
echo "✅ 2. VERIFICATION CHI TIẾT..."

sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -Q "
-- Thống kê assignments đã tạo
SELECT
    COUNT(*) as TotalAssignments,
    COUNT(DISTINCT EmployeeId) as UniqueEmployees,
    COUNT(DISTINCT KpiIndicatorId) as UniqueKpiIndicators,
    COUNT(DISTINCT KhoanPeriodId) as UniquePeriods
FROM EmployeeKpiAssignments;

-- Chi tiết theo employee
SELECT
    e.FullName,
    r.Name as RoleName,
    COUNT(eka.Id) as AssignmentCount,
    AVG(eka.TargetValue) as AvgTargetValue
FROM Employees e
INNER JOIN EmployeeRoles er ON e.Id = er.EmployeeId
INNER JOIN Roles r ON er.RoleId = r.Id
LEFT JOIN EmployeeKpiAssignments eka ON e.Id = eka.EmployeeId
GROUP BY e.Id, e.FullName, r.Name
ORDER BY COUNT(eka.Id) DESC;
"

# 3. Test API
echo "🔗 3. TESTING API..."

echo "API Response:"
curl -s "http://localhost:5055/api/EmployeeKpiAssignment" | jq 'length // "Error occurred"'

echo ""
echo "✅ HOÀN THÀNH! Employee KPI Assignments đã được tạo với manual mapping."
