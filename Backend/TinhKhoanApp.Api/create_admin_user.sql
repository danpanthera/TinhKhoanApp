-- ==================================================
-- SCRIPT TẠO USER ADMIN VÀ CHUẨN BỊ PHỤC HỒI DỮ LIỆU
-- TINHKHOAN APP - AZURE SQL EDGE ARM64
-- ==================================================

USE TinhKhoanDB;
GO

-- ==================================================
-- 1. TẠO USER ADMIN (CHỈ MỘT USER DUY NHẤT)
-- ==================================================
PRINT 'Bước 1: Tạo user admin...';

-- Hash password "admin123" bằng SHA256 (đơn giản)
DECLARE @PasswordHash NVARCHAR(255) = CONVERT(NVARCHAR(255), HASHBYTES('SHA2_256', 'admin123'), 2);

-- Tạo unit và position tối thiểu cho admin (sẽ được thay thế bằng dữ liệu thực)
IF NOT EXISTS (SELECT 1 FROM Units WHERE Id = 1)
BEGIN
    SET IDENTITY_INSERT Units ON;
    INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted)
    VALUES (1, 'ADMIN', N'Ban Quản Trị', 'HEAD_OFFICE', NULL, 0);
    SET IDENTITY_INSERT Units OFF;
    PRINT '  - Tạo đơn vị quản trị tạm thời';
END

IF NOT EXISTS (SELECT 1 FROM Positions WHERE Id = 1)
BEGIN
    SET IDENTITY_INSERT Positions ON;
    INSERT INTO Positions (Id, Name, Description)
    VALUES (1, N'Quản Trị Viên', N'Quản trị hệ thống TinhKhoan');
    SET IDENTITY_INSERT Positions OFF;
    PRINT '  - Tạo chức vụ quản trị tạm thời';
END

-- Tạo user admin
IF NOT EXISTS (SELECT 1 FROM Employees WHERE Username = 'admin')
BEGIN
    INSERT INTO Employees (EmployeeCode, CBCode, FullName, Username, PasswordHash, Email, PhoneNumber, IsActive, UnitId, PositionId)
    VALUES ('ADMIN001', 'CB001', N'Quản Trị Viên Hệ Thống', 'admin', @PasswordHash, 'admin@tinhkhoan.com', '0123456789', 1, 1, 1);
    PRINT '  ✅ Tạo user admin thành công!';
END
ELSE
BEGIN
    PRINT '  ⚠️ User admin đã tồn tại';
END

-- ==================================================
-- 2. KIỂM TRA DỮ LIỆU HIỆN TẠI
-- ==================================================
PRINT '';
PRINT 'Bước 2: Kiểm tra dữ liệu hiện tại...';

SELECT
    'Units' as TableName,
    COUNT(*) as RecordCount
FROM Units
UNION ALL
SELECT
    'Positions' as TableName,
    COUNT(*) as RecordCount
FROM Positions
UNION ALL
SELECT
    'Employees' as TableName,
    COUNT(*) as RecordCount
FROM Employees
UNION ALL
SELECT
    'KPIDefinitions' as TableName,
    COUNT(*) as RecordCount
FROM KPIDefinitions
ORDER BY TableName;

-- ==================================================
-- 3. THÔNG TIN PHỤC HỒI DỮ LIỆU
-- ==================================================
PRINT '';
PRINT '========================================================';
PRINT '📋 CẦN PHỤC HỒI DỮ LIỆU TỪ DATABASE CŨ:';
PRINT '========================================================';
PRINT '🏢 Units (Đơn vị)';
PRINT '👥 Positions (Chức vụ)';
PRINT '👨‍💼 Employees (Nhân viên)';
PRINT '📊 KPIDefinitions (23 bảng KPI)';
PRINT '📈 KpiIndicators';
PRINT '⚖️ KpiScoringRules';
PRINT '🎯 BusinessPlanTargets';
PRINT '📋 Tất cả dữ liệu nghiệp vụ thực tế';
PRINT '';
PRINT '⚠️ LƯU Ý: KHÔNG sử dụng dữ liệu mẫu/mock data';
PRINT '✅ CHỈ phục hồi dữ liệu thực từ SQL Server cũ';
PRINT '========================================================';

-- ==================================================
-- 4. XÁC NHẬN USER ADMIN
-- ==================================================
PRINT '';
PRINT 'Bước 3: Xác nhận user admin đã tạo...';

SELECT
    Id,
    EmployeeCode,
    FullName,
    Username,
    Email,
    IsActive,
    UnitId,
    PositionId
FROM Employees
WHERE Username = 'admin';

PRINT '';
PRINT '🎉 HOÀN THÀNH: User admin/admin123 đã sẵn sàng!';
PRINT '📝 Tiếp theo: Phục hồi dữ liệu từ database cũ';

GO
