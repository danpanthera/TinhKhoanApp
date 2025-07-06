-- Kiểm tra tình trạng dữ liệu hiện tại trong database TinhKhoanDB
USE TinhKhoanDB;

PRINT '=== KIỂM TRA TÌNH TRẠNG DỮ LIỆU HIỆN TẠI ===';
PRINT '';

-- Kiểm tra các bảng cơ bản
PRINT '1. Bảng Users:';
SELECT COUNT(*) as 'Số lượng Users' FROM Users;
SELECT Username FROM Users;
PRINT '';

PRINT '2. Bảng Units (Đơn vị):';
SELECT COUNT(*) as 'Số lượng Units' FROM Units;
IF EXISTS (SELECT * FROM Units)
BEGIN
    SELECT TOP 5 Id, Name, Code FROM Units ORDER BY Id;
    PRINT '(Hiển thị 5 bản ghi đầu tiên)';
END
PRINT '';

PRINT '3. Bảng Positions (Chức vụ):';
SELECT COUNT(*) as 'Số lượng Positions' FROM Positions;
IF EXISTS (SELECT * FROM Positions)
BEGIN
    SELECT TOP 5 Id, Name FROM Positions ORDER BY Id;
    PRINT '(Hiển thị 5 bản ghi đầu tiên)';
END
PRINT '';

PRINT '4. Bảng Employees (Nhân viên):';
SELECT COUNT(*) as 'Số lượng Employees' FROM Employees;
IF EXISTS (SELECT * FROM Employees)
BEGIN
    SELECT TOP 5 Id, FullName, EmployeeCode FROM Employees ORDER BY Id;
    PRINT '(Hiển thị 5 bản ghi đầu tiên)';
END
PRINT '';

PRINT '5. Bảng KPIDefinitions:';
SELECT COUNT(*) as 'Số lượng KPIDefinitions' FROM KPIDefinitions;
IF EXISTS (SELECT * FROM KPIDefinitions)
BEGIN
    SELECT TOP 5 Id, Name, Code FROM KPIDefinitions ORDER BY Id;
    PRINT '(Hiển thị 5 bản ghi đầu tiên)';
END
PRINT '';

PRINT '6. Bảng KpiIndicators:';
SELECT COUNT(*) as 'Số lượng KpiIndicators' FROM KpiIndicators;
PRINT '';

PRINT '7. Bảng KpiScoringRules:';
SELECT COUNT(*) as 'Số lượng KpiScoringRules' FROM KpiScoringRules;
PRINT '';

PRINT '8. Bảng BusinessPlanTargets:';
SELECT COUNT(*) as 'Số lượng BusinessPlanTargets' FROM BusinessPlanTargets;
PRINT '';

-- Kiểm tra các bảng raw data (dữ liệu thô)
PRINT '9. Các bảng dữ liệu thô:';
SELECT
    'DP01_RawData' as TableName, COUNT(*) as RecordCount FROM DP01_RawData
UNION ALL
SELECT
    'DP02_RawData' as TableName, COUNT(*) as RecordCount FROM DP02_RawData
UNION ALL
SELECT
    'DP03_RawData' as TableName, COUNT(*) as RecordCount FROM DP03_RawData
UNION ALL
SELECT
    'DP04_RawData' as TableName, COUNT(*) as RecordCount FROM DP04_RawData
UNION ALL
SELECT
    'DP05_RawData' as TableName, COUNT(*) as RecordCount FROM DP05_RawData
UNION ALL
SELECT
    'DP06_RawData' as TableName, COUNT(*) as RecordCount FROM DP06_RawData;

PRINT '';
PRINT '=== KẾT THÚC KIỂM TRA ===';
