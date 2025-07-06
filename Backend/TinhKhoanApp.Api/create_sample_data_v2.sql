-- =====================================================
-- SCRIPT TẠO DỮ LIỆU MẪU CHO TINHKHOAN APP - CHUẨN SCHEMA
-- Bao gồm: Admin user, Units, Positions, Employees
-- =====================================================

USE TinhKhoanDB;
GO

-- Tạo đơn vị mẫu
INSERT INTO Units (Code, Name, Type) VALUES
('AGR_HQ', N'Trụ sở chính Agribank', N'HEADQUARTERS'),
('AGR_VN', N'Agribank Việt Nam', N'REGION'),
('AGR_HN', N'Chi nhánh Hà Nội', N'BRANCH'),
('AGR_HCM', N'Chi nhánh TP.HCM', N'BRANCH'),
('AGR_DN', N'Chi nhánh Đà Nẵng', N'BRANCH');

-- Tạo chức vụ mẫu
INSERT INTO Positions (Code, Name, Level) VALUES
('CEO', N'Tổng Giám đốc', 1),
('DEPUTY_CEO', N'Phó Tổng Giám đốc', 2),
('DIRECTOR', N'Giám đốc', 3),
('DEPUTY_DIRECTOR', N'Phó Giám đốc', 4),
('MANAGER', N'Trưởng phòng', 5),
('STAFF', N'Nhân viên', 6);

-- Lấy ID của unit và position
DECLARE @HQUnitId INT = (SELECT Id FROM Units WHERE Code = 'AGR_HQ');
DECLARE @CEOPositionId INT = (SELECT Id FROM Positions WHERE Code = 'CEO');
DECLARE @DirectorPositionId INT = (SELECT Id FROM Positions WHERE Code = 'DIRECTOR');
DECLARE @ManagerPositionId INT = (SELECT Id FROM Positions WHERE Code = 'MANAGER');
DECLARE @StaffPositionId INT = (SELECT Id FROM Positions WHERE Code = 'STAFF');

-- Tạo admin user và nhân viên mẫu (password hash cho 'admin123')
INSERT INTO Employees (EmployeeCode, CBCode, FullName, Username, PasswordHash, Email, PhoneNumber, IsActive, UnitId, PositionId) VALUES
('EMP001', 'ADM001', N'Quản trị viên hệ thống', 'admin', '$2a$11$rGZx8mF5WvQzHzDwVqMJjOD.YHLl3Z9mVJpH8lGdBKwJxl9uFqN1e', 'admin@agribank.com.vn', '0988123456', 1, @HQUnitId, @CEOPositionId),
('EMP002', 'DIR001', N'Nguyễn Văn A', 'director1', '$2a$11$rGZx8mF5WvQzHzDwVqMJjOD.YHLl3Z9mVJpH8lGdBKwJxl9uFqN1e', 'director1@agribank.com.vn', '0988123457', 1, @HQUnitId, @DirectorPositionId),
('EMP003', 'MGR001', N'Trần Thị B', 'manager1', '$2a$11$rGZx8mF5WvQzHzDwVqMJjOD.YHLl3Z9mVJpH8lGdBKwJxl9uFqN1e', 'manager1@agribank.com.vn', '0988123458', 1, @HQUnitId, @ManagerPositionId),
('EMP004', 'STF001', N'Lê Văn C', 'staff1', '$2a$11$rGZx8mF5WvQzHzDwVqMJjOD.YHLl3Z9mVJpH8lGdBKwJxl9uFqN1e', 'staff1@agribank.com.vn', '0988123459', 1, @HQUnitId, @StaffPositionId),
('EMP005', 'STF002', N'Phạm Thị D', 'staff2', '$2a$11$rGZx8mF5WvQzHzDwVqMJjOD.YHLl3Z9mVJpH8lGdBKwJxl9uFqN1e', 'staff2@agribank.com.vn', '0988123460', 1, @HQUnitId, @StaffPositionId);

-- Tạo roles mẫu
INSERT INTO Roles (Name, Description) VALUES
(N'Admin', N'Quản trị viên hệ thống'),
(N'Director', N'Giám đốc'),
(N'Manager', N'Trưởng phòng'),
(N'Staff', N'Nhân viên');

-- Gán roles cho employees
DECLARE @AdminRoleId INT = (SELECT Id FROM Roles WHERE Name = 'Admin');
DECLARE @DirectorRoleId INT = (SELECT Id FROM Roles WHERE Name = 'Director');
DECLARE @ManagerRoleId INT = (SELECT Id FROM Roles WHERE Name = 'Manager');
DECLARE @StaffRoleId INT = (SELECT Id FROM Roles WHERE Name = 'Staff');

DECLARE @AdminEmployeeId INT = (SELECT Id FROM Employees WHERE Username = 'admin');
DECLARE @Director1EmployeeId INT = (SELECT Id FROM Employees WHERE Username = 'director1');
DECLARE @Manager1EmployeeId INT = (SELECT Id FROM Employees WHERE Username = 'manager1');
DECLARE @Staff1EmployeeId INT = (SELECT Id FROM Employees WHERE Username = 'staff1');
DECLARE @Staff2EmployeeId INT = (SELECT Id FROM Employees WHERE Username = 'staff2');

INSERT INTO EmployeeRoles (EmployeeId, RoleId) VALUES
(@AdminEmployeeId, @AdminRoleId),
(@Director1EmployeeId, @DirectorRoleId),
(@Manager1EmployeeId, @ManagerRoleId),
(@Staff1EmployeeId, @StaffRoleId),
(@Staff2EmployeeId, @StaffRoleId);

PRINT N'✅ Đã tạo thành công dữ liệu mẫu:';
PRINT N'   - 5 Units (đơn vị)';
PRINT N'   - 6 Positions (chức vụ)';
PRINT N'   - 5 Employees (nhân viên, bao gồm admin)';
PRINT N'   - 4 Roles (vai trò)';
PRINT N'';
PRINT N'🔑 Thông tin đăng nhập:';
PRINT N'   Username: admin';
PRINT N'   Password: admin123';
PRINT N'';

-- Kiểm tra kết quả
SELECT 'UNITS' as TableName, COUNT(*) as RecordCount FROM Units
UNION ALL
SELECT 'POSITIONS', COUNT(*) FROM Positions
UNION ALL
SELECT 'EMPLOYEES', COUNT(*) FROM Employees
UNION ALL
SELECT 'ROLES', COUNT(*) FROM Roles
UNION ALL
SELECT 'EMPLOYEE_ROLES', COUNT(*) FROM EmployeeRoles;

GO
