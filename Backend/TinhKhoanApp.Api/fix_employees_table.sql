-- Tạo bảng Employees cơ bản để test bulk delete
USE TinhKhoanDB;

-- Tạo bảng Units nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Units')
BEGIN
    CREATE TABLE Units (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Name nvarchar(255) NOT NULL,
        Code nvarchar(50) NOT NULL
    );

    INSERT INTO Units (Name, Code) VALUES
    (N'Hội Sở', 'HoiSo'),
    (N'Chi nhánh Bình Lư', 'BinhLu');
END

-- Tạo bảng Positions nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Positions')
BEGIN
    CREATE TABLE Positions (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Name nvarchar(255) NOT NULL,
        Code nvarchar(50) NOT NULL
    );

    INSERT INTO Positions (Name, Code) VALUES
    (N'Giám đốc', 'GD'),
    (N'Trưởng phòng', 'TP'),
    (N'Nhân viên', 'NV');
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Employees')
BEGIN
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
        UpdatedAt datetime2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_Employees_Units FOREIGN KEY (UnitId) REFERENCES Units(Id),
        CONSTRAINT FK_Employees_Positions FOREIGN KEY (PositionId) REFERENCES Positions(Id)
    );
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EmployeeRoles')
BEGIN
    CREATE TABLE EmployeeRoles (
        Id int IDENTITY(1,1) PRIMARY KEY,
        EmployeeId int NOT NULL,
        RoleId int NOT NULL,
        CONSTRAINT FK_EmployeeRoles_Employees FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE
    );
END

-- Thêm dữ liệu test
DELETE FROM Employees; -- Xóa dữ liệu cũ trước

INSERT INTO Employees (EmployeeCode, CBCode, FullName, Username, PasswordHash, UnitId, PositionId, Email) VALUES
('EMP001', '123456001', N'Nguyễn Văn A', 'admin', 'admin123', 1, 1, 'admin@example.com'),
('EMP002', '123456002', N'Trần Thị B', 'user1', 'user123', 1, 2, 'user1@example.com'),
('EMP003', '123456003', N'Lê Văn C', 'user2', 'user123', 2, 3, 'user2@example.com');
