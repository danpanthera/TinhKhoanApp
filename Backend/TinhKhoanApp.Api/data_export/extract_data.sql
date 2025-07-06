-- ==================================================
-- SCRIPT EXTRACT DỮ LIỆU TỪ BACKUP
-- Chạy script này trên SQL Server có restore backup
-- ==================================================

USE TinhKhoanDB;  -- Hoặc tên database sau khi restore
GO

-- Tạo thư mục export (cần quyền admin trên SQL Server)
-- EXEC xp_cmdshell 'mkdir C:\temp\tinhkhoan_export'

-- ==================================================
-- 1. EXPORT UNITS (46 đơn vị)
-- ==================================================
PRINT '📤 Exporting Units...';
SELECT
    Id,
    Code,
    Name,
    Type,
    ParentUnitId,
    IsActive,
    CreatedAt,
    UpdatedAt
FROM Units
ORDER BY Id;

-- Export to CSV (nếu có quyền)
-- EXEC xp_cmdshell 'bcp "SELECT Id,Code,Name,Type,ParentUnitId,IsActive,CreatedAt,UpdatedAt FROM TinhKhoanDB.dbo.Units ORDER BY Id" queryout "C:\temp\tinhkhoan_export\units.csv" -c -t"," -T -S localhost'

-- ==================================================
-- 2. EXPORT POSITIONS (Chức vụ)
-- ==================================================
PRINT '📤 Exporting Positions...';
SELECT
    Id,
    Name,
    Description,
    IsActive,
    CreatedAt,
    UpdatedAt
FROM Positions
ORDER BY Id;

-- ==================================================
-- 3. EXPORT EMPLOYEES (Nhân viên)
-- ==================================================
PRINT '📤 Exporting Employees...';
SELECT
    Id,
    EmployeeCode,
    CbCode,
    FullName,
    Username,
    Email,
    PhoneNumber,
    IsActive,
    UnitId,
    PositionId,
    CreatedAt,
    UpdatedAt
FROM Employees
WHERE Username != 'admin'  -- Loại trừ admin vì đã có
ORDER BY Id;

-- ==================================================
-- 4. EXPORT KPI DEFINITIONS
-- ==================================================
PRINT '📤 Exporting KPIDefinitions...';
SELECT
    Id,
    Code,
    Name,
    Description,
    UnitOfMeasure,
    TargetType,
    IsActive,
    CreatedAt,
    UpdatedAt
FROM KPIDefinitions
ORDER BY Id;

-- ==================================================
-- 5. EXPORT KPI INDICATORS
-- ==================================================
PRINT '📤 Exporting KpiIndicators...';
SELECT
    Id,
    KPIDefinitionId,
    Name,
    Description,
    Weight,
    IsActive,
    CreatedAt,
    UpdatedAt
FROM KpiIndicators
ORDER BY Id;

-- ==================================================
-- 6. EXPORT KPI SCORING RULES
-- ==================================================
PRINT '📤 Exporting KpiScoringRules...';
SELECT
    Id,
    KpiIndicatorId,
    MinValue,
    MaxValue,
    Score,
    Description,
    IsActive,
    CreatedAt,
    UpdatedAt
FROM KpiScoringRules
ORDER BY Id;

-- ==================================================
-- 7. EXPORT BUSINESS PLAN TARGETS
-- ==================================================
PRINT '📤 Exporting BusinessPlanTargets...';
SELECT
    Id,
    Year,
    Quarter,
    Month,
    UnitId,
    KPIDefinitionId,
    TargetValue,
    IsActive,
    CreatedAt,
    UpdatedAt
FROM BusinessPlanTargets
ORDER BY Year, Quarter, Month, UnitId, KPIDefinitionId;

-- ==================================================
-- THỐNG KÊ DỮ LIỆU
-- ==================================================
PRINT '';
PRINT '📊 THỐNG KÊ DỮ LIỆU CẦN EXPORT:';
SELECT 'Units' as TableName, COUNT(*) as RecordCount FROM Units
UNION ALL
SELECT 'Positions' as TableName, COUNT(*) as RecordCount FROM Positions
UNION ALL
SELECT 'Employees' as TableName, COUNT(*) as RecordCount FROM Employees
UNION ALL
SELECT 'KPIDefinitions' as TableName, COUNT(*) as RecordCount FROM KPIDefinitions
UNION ALL
SELECT 'KpiIndicators' as TableName, COUNT(*) as RecordCount FROM KpiIndicators
UNION ALL
SELECT 'KpiScoringRules' as TableName, COUNT(*) as RecordCount FROM KpiScoringRules
UNION ALL
SELECT 'BusinessPlanTargets' as TableName, COUNT(*) as RecordCount FROM BusinessPlanTargets
ORDER BY TableName;

PRINT '';
PRINT '✅ HOÀN THÀNH EXTRACT. Copy kết quả và chuyển sang Azure SQL Edge.';

GO
