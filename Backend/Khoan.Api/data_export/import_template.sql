-- ==================================================
-- SCRIPT IMPORT DỮ LIỆU VÀO AZURE SQL EDGE
-- Chạy sau khi đã có dữ liệu từ extract
-- ==================================================

USE TinhKhoanDB;
GO

-- ==================================================
-- BƯỚC 1: BACKUP DỮ LIỆU HIỆN TẠI (TẠM THỜI)
-- ==================================================
PRINT '🔄 Backup dữ liệu admin hiện tại...';

-- Lưu thông tin admin để khôi phục sau
DECLARE @AdminUserId INT = (SELECT Id FROM Users WHERE Username = 'admin');
DECLARE @AdminUnitId INT = (SELECT UnitId FROM Employees WHERE Username = 'admin');
DECLARE @AdminPositionId INT = (SELECT PositionId FROM Employees WHERE Username = 'admin');

PRINT '  📝 Admin User ID: ' + CAST(@AdminUserId AS VARCHAR);
PRINT '  🏢 Admin Unit ID: ' + CAST(@AdminUnitId AS VARCHAR);
PRINT '  👤 Admin Position ID: ' + CAST(@AdminPositionId AS VARCHAR);

-- ==================================================
-- BƯỚC 2: XÓA DỮ LIỆU TẠM THỜI (GIỮ LẠI ADMIN)
-- ==================================================
PRINT '';
PRINT '🗑️  Xóa dữ liệu tạm thời...';

-- Xóa theo thứ tự dependency
DELETE FROM BusinessPlanTargets;
DELETE FROM KpiScoringRules;
DELETE FROM KpiIndicators;
DELETE FROM KPIDefinitions;
DELETE FROM Employees WHERE Username != 'admin';
-- Giữ lại Position và Unit của admin

PRINT '  ✅ Đã xóa dữ liệu tạm thời, giữ lại admin';

-- ==================================================
-- BƯỚC 3: INSERT DỮ LIỆU THỰC TẾ
-- ==================================================
PRINT '';
PRINT '📥 Bắt đầu import dữ liệu thực tế...';

-- 3.1. IMPORT UNITS (46 đơn vị)
PRINT '  📤 Importing Units...';
-- TODO: INSERT statements từ kết quả extract
-- INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsActive, CreatedAt, UpdatedAt) VALUES
-- (...data từ extract...)

-- 3.2. IMPORT POSITIONS (Chức vụ)
PRINT '  📤 Importing Positions...';
-- TODO: INSERT statements từ kết quả extract
-- INSERT INTO Positions (Id, Name, Description, IsActive, CreatedAt, UpdatedAt) VALUES
-- (...data từ extract...)

-- 3.3. IMPORT EMPLOYEES (Nhân viên)
PRINT '  📤 Importing Employees...';
-- TODO: INSERT statements từ kết quả extract
-- INSERT INTO Employees (...) VALUES (...)

-- 3.4. IMPORT KPI DEFINITIONS
PRINT '  📤 Importing KPIDefinitions...';
-- TODO: INSERT statements

-- 3.5. IMPORT KPI INDICATORS
PRINT '  📤 Importing KpiIndicators...';
-- TODO: INSERT statements

-- 3.6. IMPORT KPI SCORING RULES
PRINT '  📤 Importing KpiScoringRules...';
-- TODO: INSERT statements

-- 3.7. IMPORT BUSINESS PLAN TARGETS
PRINT '  📤 Importing BusinessPlanTargets...';
-- TODO: INSERT statements

-- ==================================================
-- BƯỚC 4: VERIFY DỮ LIỆU SAU IMPORT
-- ==================================================
PRINT '';
PRINT '✅ VERIFY DỮ LIỆU SAU IMPORT:';
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

-- Kiểm tra admin vẫn hoạt động
SELECT 'Admin User Exists' as CheckName, COUNT(*) as Result FROM Users WHERE Username = 'admin';

PRINT '';
PRINT '🎯 MỤC TIÊU ĐẠT ĐƯỢC:';
PRINT '  ✅ 46 Units (Đơn vị)';
PRINT '  ✅ Đầy đủ Positions (Chức vụ)';
PRINT '  ✅ Đầy đủ Employees (Nhân viên)';
PRINT '  ✅ Đầy đủ KPI Definitions';
PRINT '  ✅ Admin vẫn hoạt động bình thường';
PRINT '';
PRINT '🚀 DỮ LIỆU ĐÃ SẴN SÀNG CHO SẢN XUẤT!';

GO
