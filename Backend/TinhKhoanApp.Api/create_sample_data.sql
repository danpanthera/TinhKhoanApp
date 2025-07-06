-- =====================================================
-- SCRIPT TẠO DỮ LIỆU MẪU CHO TINHKHOAN APP
-- Bao gồm: Admin user, Units, Positions, Employees
-- =====================================================

USE TinhKhoanDB;
GO

-- Tạo đơn vị mẫu
INSERT INTO Units (UnitCode, UnitName, UnitType, Level, IsActive, CreatedDate) VALUES
('AGR_HQ', 'Trụ sở chính Agribank', 'HEADQUARTERS', 1, 1, GETDATE()),
('AGR_VN', 'Agribank Việt Nam', 'REGION', 2, 1, GETDATE()),
('AGR_HN', 'Chi nhánh Hà Nội', 'BRANCH', 3, 1, GETDATE()),
('AGR_HCM', 'Chi nhánh TP.HCM', 'BRANCH', 3, 1, GETDATE()),
('AGR_DN', 'Chi nhánh Đà Nẵng', 'BRANCH', 3, 1, GETDATE());

-- Tạo chức vụ mẫu
INSERT INTO Positions (PositionCode, PositionName, Level, IsActive, CreatedDate) VALUES
('CEO', 'Tổng Giám đốc', 1, 1, GETDATE()),
('DEPUTY_CEO', 'Phó Tổng Giám đốc', 2, 1, GETDATE()),
('DIRECTOR', 'Giám đốc', 3, 1, GETDATE()),
('DEPUTY_DIRECTOR', 'Phó Giám đốc', 4, 1, GETDATE()),
('MANAGER', 'Trưởng phòng', 5, 1, GETDATE()),
('STAFF', 'Nhân viên', 6, 1, GETDATE());

-- Lấy ID của unit và position
DECLARE @HQUnitId INT = (SELECT Id FROM Units WHERE UnitCode = 'AGR_HQ');
DECLARE @CEOPositionId INT = (SELECT Id FROM Positions WHERE PositionCode = 'CEO');
DECLARE @DirectorPositionId INT = (SELECT Id FROM Positions WHERE PositionCode = 'DIRECTOR');
DECLARE @ManagerPositionId INT = (SELECT Id FROM Positions WHERE PositionCode = 'MANAGER');
DECLARE @StaffPositionId INT = (SELECT Id FROM Positions WHERE PositionCode = 'STAFF');

-- Tạo admin user và nhân viên mẫu
INSERT INTO Employees (EmployeeCode, CBCode, FullName, Username, PasswordHash, Email, PhoneNumber, IsActive, UnitId, PositionId, CreatedDate) VALUES
('EMP001', 'ADM001', 'Quản trị viên hệ thống', 'admin', '$2a$11$rGZx8mF5WvQzHzDwVqMJjOD.YHLl3Z9mVJpH8lGdBKwJxl9uFqN1e', 'admin@agribank.com.vn', '0988123456', 1, @HQUnitId, @CEOPositionId, GETDATE()),
('EMP002', 'DIR001', 'Nguyễn Văn A', 'director1', '$2a$11$rGZx8mF5WvQzHzDwVqMJjOD.YHLl3Z9mVJpH8lGdBKwJxl9uFqN1e', 'director1@agribank.com.vn', '0988123457', 1, @HQUnitId, @DirectorPositionId, GETDATE()),
('EMP003', 'MGR001', 'Trần Thị B', 'manager1', '$2a$11$rGZx8mF5WvQzHzDwVqMJjOD.YHLl3Z9mVJpH8lGdBKwJxl9uFqN1e', 'manager1@agribank.com.vn', '0988123458', 1, @HQUnitId, @ManagerPositionId, GETDATE()),
('EMP004', 'STF001', 'Lê Văn C', 'staff1', '$2a$11$rGZx8mF5WvQzHzDwVqMJjOD.YHLl3Z9mVJpH8lGdBKwJxl9uFqN1e', 'staff1@agribank.com.vn', '0988123459', 1, @HQUnitId, @StaffPositionId, GETDATE()),
('EMP005', 'STF002', 'Phạm Thị D', 'staff2', '$2a$11$rGZx8mF5WvQzHzDwVqMJjOD.YHLl3Z9mVJpH8lGdBKwJxl9uFqN1e', 'staff2@agribank.com.vn', '0988123460', 1, @HQUnitId, @StaffPositionId, GETDATE());

-- Tạo roles mẫu
INSERT INTO Roles (RoleName, Description, IsActive, CreatedDate) VALUES
('Admin', 'Quản trị viên hệ thống', 1, GETDATE()),
('Director', 'Giám đốc', 1, GETDATE()),
('Manager', 'Trưởng phòng', 1, GETDATE()),
('Staff', 'Nhân viên', 1, GETDATE());

-- Gán roles cho employees
DECLARE @AdminRoleId INT = (SELECT Id FROM Roles WHERE RoleName = 'Admin');
DECLARE @DirectorRoleId INT = (SELECT Id FROM Roles WHERE RoleName = 'Director');
DECLARE @ManagerRoleId INT = (SELECT Id FROM Roles WHERE RoleName = 'Manager');
DECLARE @StaffRoleId INT = (SELECT Id FROM Roles WHERE RoleName = 'Staff');

DECLARE @AdminEmployeeId INT = (SELECT Id FROM Employees WHERE Username = 'admin');
DECLARE @Director1EmployeeId INT = (SELECT Id FROM Employees WHERE Username = 'director1');
DECLARE @Manager1EmployeeId INT = (SELECT Id FROM Employees WHERE Username = 'manager1');
DECLARE @Staff1EmployeeId INT = (SELECT Id FROM Employees WHERE Username = 'staff1');
DECLARE @Staff2EmployeeId INT = (SELECT Id FROM Employees WHERE Username = 'staff2');

INSERT INTO EmployeeRoles (EmployeeId, RoleId, IsActive, CreatedDate) VALUES
(@AdminEmployeeId, @AdminRoleId, 1, GETDATE()),
(@Director1EmployeeId, @DirectorRoleId, 1, GETDATE()),
(@Manager1EmployeeId, @ManagerRoleId, 1, GETDATE()),
(@Staff1EmployeeId, @StaffRoleId, 1, GETDATE()),
(@Staff2EmployeeId, @StaffRoleId, 1, GETDATE());

-- Tạo một số KPI definitions mẫu
INSERT INTO KPIDefinitions (KpiCode, KpiName, KpiType, Description, IsActive, CreatedDate) VALUES
('DEPOSIT_GROWTH', 'Tăng trưởng huy động vốn', 'BUSINESS', 'Chỉ tiêu tăng trưởng huy động vốn', 1, GETDATE()),
('LOAN_GROWTH', 'Tăng trưởng cho vay', 'BUSINESS', 'Chỉ tiêu tăng trưởng dư nợ cho vay', 1, GETDATE()),
('CUSTOMER_SATISFACTION', 'Hài lòng khách hàng', 'QUALITY', 'Chỉ số hài lòng của khách hàng', 1, GETDATE()),
('PROFIT_MARGIN', 'Tỷ suất lợi nhuận', 'FINANCIAL', 'Tỷ suất lợi nhuận hoạt động', 1, GETDATE());

PRINT '✅ Đã tạo thành công dữ liệu mẫu:';
PRINT '   - 5 Units (đơn vị)';
PRINT '   - 6 Positions (chức vụ)';
PRINT '   - 5 Employees (nhân viên, bao gồm admin)';
PRINT '   - 4 Roles (vai trò)';
PRINT '   - 4 KPI Definitions (định nghĩa KPI)';
PRINT '';
PRINT '🔑 Thông tin đăng nhập:';
PRINT '   Username: admin';
PRINT '   Password: admin123';
PRINT '';

-- Kiểm tra kết quả
SELECT 'UNITS' as TableName, COUNT(*) as RecordCount FROM Units
UNION ALL
SELECT 'POSITIONS', COUNT(*) FROM Positions
UNION ALL
SELECT 'EMPLOYEES', COUNT(*) FROM Employees
UNION ALL
SELECT 'ROLES', COUNT(*) FROM Roles
UNION ALL
SELECT 'EMPLOYEE_ROLES', COUNT(*) FROM EmployeeRoles
UNION ALL
SELECT 'KPI_DEFINITIONS', COUNT(*) FROM KPIDefinitions;

GO
