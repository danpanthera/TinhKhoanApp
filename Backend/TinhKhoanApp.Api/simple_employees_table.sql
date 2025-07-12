-- Script đơn giản để test bulk delete
USE TinhKhoanDB;

-- Xóa và tạo lại bảng Employees đơn giản
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Employees')
    DROP TABLE Employees;

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EmployeeRoles')
    DROP TABLE EmployeeRoles;

CREATE TABLE Employees (
    Id int IDENTITY(1,1) PRIMARY KEY,
    EmployeeCode nvarchar(50) NOT NULL,
    CBCode nvarchar(50) NOT NULL DEFAULT '123456000',
    FullName nvarchar(255) NOT NULL,
    Username nvarchar(100) NOT NULL UNIQUE,
    Email nvarchar(255) DEFAULT '',
    PhoneNumber nvarchar(20) DEFAULT '',
    PasswordHash nvarchar(255) NOT NULL DEFAULT 'defaultpassword',
    IsActive bit NOT NULL DEFAULT 1,
    UnitId int NOT NULL DEFAULT 1,
    PositionId int NOT NULL DEFAULT 1,
    CreatedAt datetime2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt datetime2 NOT NULL DEFAULT GETDATE()
);

CREATE TABLE EmployeeRoles (
    Id int IDENTITY(1,1) PRIMARY KEY,
    EmployeeId int NOT NULL,
    RoleId int NOT NULL,
    CONSTRAINT FK_EmployeeRoles_Employees FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE
);

-- Thêm dữ liệu test
INSERT INTO Employees (EmployeeCode, CBCode, FullName, Username, PasswordHash, Email) VALUES
('EMP001', '123456001', N'Nguyễn Văn A', 'admin', 'admin123', 'admin@example.com'),
('EMP002', '123456002', N'Trần Thị B', 'user1', 'user123', 'user1@example.com'),
('EMP003', '123456003', N'Lê Văn C', 'user2', 'user123', 'user2@example.com');

SELECT 'Created ' + CAST(COUNT(*) AS varchar) + ' employees' AS Result FROM Employees;
