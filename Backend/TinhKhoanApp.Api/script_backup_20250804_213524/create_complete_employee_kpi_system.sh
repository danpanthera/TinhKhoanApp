#!/bin/bash

# =============================================================================
# COMPLETE EMPLOYEE KPI ASSIGNMENTS - ALL EMPLOYEES + ALL APPROPRIATE KPIs
# Tạo assignments cho tất cả 10 employees với KPI indicators phù hợp theo role
# =============================================================================

echo "🚀 BẮT ĐẦU TẠO COMPLETE EMPLOYEE KPI ASSIGNMENTS..."

# Database connection
SERVER="localhost,1433"
DATABASE="TinhKhoanDB"
USERNAME="sa"
PASSWORD="YourStrong@Password123"

# 1. Clear existing assignments để tạo lại
echo "🧹 1. CLEARING EXISTING ASSIGNMENTS..."
sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -Q "DELETE FROM EmployeeKpiAssignments;"

# 2. Tạo assignments với comprehensive mapping
echo "🎯 2. TẠO COMPREHENSIVE EMPLOYEE KPI ASSIGNMENTS..."

sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -Q "
-- Comprehensive Employee KPI Assignments với tất cả mappings
INSERT INTO EmployeeKpiAssignments (EmployeeId, KpiIndicatorId, KhoanPeriodId, TargetValue, ActualValue, Status, AssignedAt, UpdatedAt)
SELECT DISTINCT
    e.Id as EmployeeId,
    ki.Id as KpiIndicatorId,
    kp.Id as KhoanPeriodId,
    CASE
        WHEN r.Name LIKE '%Trưởng phòng%' THEN 1500.00
        WHEN r.Name LIKE '%Phó phòng%' THEN 1200.00
        WHEN r.Name LIKE '%Giám đốc%' THEN 2000.00
        WHEN r.Name = 'GDV' THEN 800.00
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
INNER JOIN KpiAssignmentTables kat ON ki.TableId = kat.Id
CROSS JOIN (SELECT TOP 1 Id FROM KhoanPeriods WHERE YEAR(StartDate) = 2025 ORDER BY StartDate) kp
WHERE
    -- Comprehensive mapping cho tất cả roles
    (
        (r.Name = 'Trưởng phòng KHDN' AND kat.TableName = 'TruongphongKhdn') OR
        (r.Name = 'Trưởng phòng KHCN' AND kat.TableName = 'TruongphongKhcn') OR
        (r.Name = 'Phó phòng KHDN' AND kat.TableName = 'PhophongKhdn') OR
        (r.Name = 'Trưởng phòng KH&QLRR' AND kat.TableName = 'TruongphongKhqlrr') OR
        (r.Name = 'Phó phòng KH&QLRR' AND kat.TableName = 'PhophongKhqlrr') OR
        (r.Name = 'Trưởng phòng KTNQ CNL1' AND kat.TableName = 'TruongphongKtnqCnl1') OR
        (r.Name = 'Phó phòng KTNQ CNL1' AND kat.TableName = 'PhophongKtnqCnl1') OR
        (r.Name = 'Cán bộ tín dụng' AND kat.TableName = 'Cbtd') OR
        (r.Name = 'GDV' AND kat.TableName = 'Gdv') OR
        (r.Name = 'Giám đốc CNL2' AND kat.TableName IN ('ChinhanhCap2', 'TruongphongKhCnl2')) OR
        (r.Name = 'Phó giám đốc CNL2 phụ trách TD' AND kat.TableName = 'PhongchucnangCap2') OR
        (r.Name = 'Phó giám đốc CNL2 phụ trách KT' AND kat.TableName = 'TruongphongKtnqCnl2') OR
        (r.Name = 'Giám đốc Phòng giao dịch' AND kat.TableName = 'GiamdocPgd') OR
        (r.Name = 'Phó giám đốc Phòng giao dịch' AND kat.TableName = 'PhogiamdocPgd') OR
        (r.Name = 'Phó giám đốc PGD kiêm CBTD' AND kat.TableName = 'PhogiamdocPgdCbtd') OR
        (r.Name = 'Trưởng phó IT | Tổng hợp | KTGS' AND kat.TableName = 'TruongphongItThKtgs') OR
        (r.Name = 'Cán bộ IT | Tổng hợp | KTGS | KH&QLRR' AND kat.TableName = 'CbItThKtgsKhqlrr')
    );

-- Báo cáo kết quả
SELECT 'Total EmployeeKpiAssignments created: ' + CAST(@@ROWCOUNT as VARCHAR(10)) as Result;
"

# 3. Verification chi tiết
echo "✅ 3. VERIFICATION VÀ STATISTICS..."

sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -Q "
-- Tổng thống kê
SELECT
    COUNT(*) as TotalAssignments,
    COUNT(DISTINCT EmployeeId) as UniqueEmployees,
    COUNT(DISTINCT KpiIndicatorId) as UniqueKpiIndicators,
    COUNT(DISTINCT KhoanPeriodId) as UniquePeriods,
    AVG(TargetValue) as AvgTargetValue,
    MIN(TargetValue) as MinTargetValue,
    MAX(TargetValue) as MaxTargetValue
FROM EmployeeKpiAssignments;

-- Top 5 employees với nhiều assignments nhất
SELECT TOP 5
    e.FullName,
    r.Name as RoleName,
    COUNT(eka.Id) as AssignmentCount,
    AVG(eka.TargetValue) as AvgTarget
FROM Employees e
INNER JOIN EmployeeRoles er ON e.Id = er.EmployeeId
INNER JOIN Roles r ON er.RoleId = r.Id
LEFT JOIN EmployeeKpiAssignments eka ON e.Id = eka.EmployeeId
GROUP BY e.Id, e.FullName, r.Name
ORDER BY COUNT(eka.Id) DESC;

-- Sample assignments
SELECT TOP 10
    e.FullName,
    ki.IndicatorName,
    eka.TargetValue,
    eka.Status
FROM EmployeeKpiAssignments eka
INNER JOIN Employees e ON eka.EmployeeId = e.Id
INNER JOIN KpiIndicators ki ON eka.KpiIndicatorId = ki.Id
ORDER BY e.FullName;
"

# 4. Test API endpoints
echo "🔗 4. TESTING API ENDPOINTS..."

echo "Total assignments via API:"
curl -s "http://localhost:5055/api/EmployeeKpiAssignment" | jq 'length // "Error occurred"'

echo ""
echo "Sample assignment via API:"
curl -s "http://localhost:5055/api/EmployeeKpiAssignment" | jq '.[0] // "No data"' 2>/dev/null

echo ""
echo "✅ HOÀN THÀNH! Complete Employee KPI Assignments system đã được thiết lập."
echo "📊 Chi tiết: http://localhost:5055/api/EmployeeKpiAssignment"
