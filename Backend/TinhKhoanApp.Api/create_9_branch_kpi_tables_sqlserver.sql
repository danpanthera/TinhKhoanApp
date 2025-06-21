-- Tạo 9 bảng KPI dành cho Chi nhánh với cấu trúc giống GiamdocCnl2_KPI_Assignment
-- Tất cả các bảng sẽ có cấu trúc giống nhau để quản lý KPI assignment cho Chi nhánh
-- SQL Server compatible version

-- 1. KPI_CN_MUONG_TE - Chi nhánh Mường Tè
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='KPI_CN_MUONG_TE' AND xtype='U')
CREATE TABLE KPI_CN_MUONG_TE (
    Id INT PRIMARY KEY IDENTITY(1,1),
    EmployeeId INT NOT NULL,
    RoleId INT NOT NULL,
    BranchId NVARCHAR(50) NOT NULL,
    AssignmentDate DATETIME NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- 2. KPI_CN_NAM_NHUN - Chi nhánh Nậm Nhùn
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='KPI_CN_NAM_NHUN' AND xtype='U')
CREATE TABLE KPI_CN_NAM_NHUN (
    Id INT PRIMARY KEY IDENTITY(1,1),
    EmployeeId INT NOT NULL,
    RoleId INT NOT NULL,
    BranchId NVARCHAR(50) NOT NULL,
    AssignmentDate DATETIME NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- 3. KPI_CN_PHONG_THO - Chi nhánh Phong Thổ
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='KPI_CN_PHONG_THO' AND xtype='U')
CREATE TABLE KPI_CN_PHONG_THO (
    Id INT PRIMARY KEY IDENTITY(1,1),
    EmployeeId INT NOT NULL,
    RoleId INT NOT NULL,
    BranchId NVARCHAR(50) NOT NULL,
    AssignmentDate DATETIME NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- 4. KPI_CN_SIN_HO - Chi nhánh Sìn Hồ
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='KPI_CN_SIN_HO' AND xtype='U')
CREATE TABLE KPI_CN_SIN_HO (
    Id INT PRIMARY KEY IDENTITY(1,1),
    EmployeeId INT NOT NULL,
    RoleId INT NOT NULL,
    BranchId NVARCHAR(50) NOT NULL,
    AssignmentDate DATETIME NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- 5. KPI_CN_TAM_DUONG - Chi nhánh Tam Đường
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='KPI_CN_TAM_DUONG' AND xtype='U')
CREATE TABLE KPI_CN_TAM_DUONG (
    Id INT PRIMARY KEY IDENTITY(1,1),
    EmployeeId INT NOT NULL,
    RoleId INT NOT NULL,
    BranchId NVARCHAR(50) NOT NULL,
    AssignmentDate DATETIME NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- 6. KPI_CN_TAN_UYEN - Chi nhánh Tân Uyên
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='KPI_CN_TAN_UYEN' AND xtype='U')
CREATE TABLE KPI_CN_TAN_UYEN (
    Id INT PRIMARY KEY IDENTITY(1,1),
    EmployeeId INT NOT NULL,
    RoleId INT NOT NULL,
    BranchId NVARCHAR(50) NOT NULL,
    AssignmentDate DATETIME NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- 7. KPI_CN_THANH_PHO - Chi nhánh Thành phố
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='KPI_CN_THANH_PHO' AND xtype='U')
CREATE TABLE KPI_CN_THANH_PHO (
    Id INT PRIMARY KEY IDENTITY(1,1),
    EmployeeId INT NOT NULL,
    RoleId INT NOT NULL,
    BranchId NVARCHAR(50) NOT NULL,
    AssignmentDate DATETIME NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- 8. KPI_CN_THAN_UYEN - Chi nhánh Than Uyên
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='KPI_CN_THAN_UYEN' AND xtype='U')
CREATE TABLE KPI_CN_THAN_UYEN (
    Id INT PRIMARY KEY IDENTITY(1,1),
    EmployeeId INT NOT NULL,
    RoleId INT NOT NULL,
    BranchId NVARCHAR(50) NOT NULL,
    AssignmentDate DATETIME NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- 9. KPI_HOI_SO - Hội sở
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='KPI_HOI_SO' AND xtype='U')
CREATE TABLE KPI_HOI_SO (
    Id INT PRIMARY KEY IDENTITY(1,1),
    EmployeeId INT NOT NULL,
    RoleId INT NOT NULL,
    BranchId NVARCHAR(50) NOT NULL,
    AssignmentDate DATETIME NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- Kiểm tra xem các bảng đã được tạo thành công
PRINT 'KIỂM TRA SAU KHI TẠO';
SELECT name as TableName 
FROM sys.tables 
WHERE name LIKE 'KPI_%' AND name NOT LIKE '%Assignment%' 
ORDER BY name;

-- Verification script
PRINT 'So sánh cấu trúc các bảng KPI branch';
SELECT 
    t.name AS TableName,
    c.name AS ColumnName,
    ty.name AS DataType,
    c.max_length,
    c.is_nullable
FROM sys.tables t
INNER JOIN sys.columns c ON t.object_id = c.object_id
INNER JOIN sys.types ty ON c.system_type_id = ty.system_type_id
WHERE t.name LIKE 'KPI_CN_%' OR t.name = 'KPI_HOI_SO'
ORDER BY t.name, c.column_id;
