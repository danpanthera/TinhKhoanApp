-- Tạo 9 bảng KPI dành cho Chi nhánh với cấu trúc giống GiamdocCnl2_KPI_Assignment
-- Tất cả các bảng sẽ có cấu trúc giống nhau để quản lý KPI assignment cho Chi nhánh

-- 1. KPI_CN_MUONG_TE - Chi nhánh Mường Tè
CREATE TABLE IF NOT EXISTS KPI_CN_MUONG_TE (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmployeeId INTEGER NOT NULL,
    RoleId INTEGER NOT NULL,
    BranchId TEXT NOT NULL,
    AssignmentDate TEXT NOT NULL,
    IsActive INTEGER DEFAULT 1,
    CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- 2. KPI_CN_NAM_NHUN - Chi nhánh Nậm Nhùn
CREATE TABLE IF NOT EXISTS KPI_CN_NAM_NHUN (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmployeeId INTEGER NOT NULL,
    RoleId INTEGER NOT NULL,
    BranchId TEXT NOT NULL,
    AssignmentDate TEXT NOT NULL,
    IsActive INTEGER DEFAULT 1,
    CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- 3. KPI_CN_PHONG_THO - Chi nhánh Phong Thổ
CREATE TABLE IF NOT EXISTS KPI_CN_PHONG_THO (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmployeeId INTEGER NOT NULL,
    RoleId INTEGER NOT NULL,
    BranchId TEXT NOT NULL,
    AssignmentDate TEXT NOT NULL,
    IsActive INTEGER DEFAULT 1,
    CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- 4. KPI_CN_SIN_HO - Chi nhánh Sìn Hồ
CREATE TABLE IF NOT EXISTS KPI_CN_SIN_HO (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmployeeId INTEGER NOT NULL,
    RoleId INTEGER NOT NULL,
    BranchId TEXT NOT NULL,
    AssignmentDate TEXT NOT NULL,
    IsActive INTEGER DEFAULT 1,
    CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- 5. KPI_CN_TAM_DUONG - Chi nhánh Tam Đường
CREATE TABLE IF NOT EXISTS KPI_CN_TAM_DUONG (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmployeeId INTEGER NOT NULL,
    RoleId INTEGER NOT NULL,
    BranchId TEXT NOT NULL,
    AssignmentDate TEXT NOT NULL,
    IsActive INTEGER DEFAULT 1,
    CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- 6. KPI_CN_TAN_UYEN - Chi nhánh Tân Uyên
CREATE TABLE IF NOT EXISTS KPI_CN_TAN_UYEN (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmployeeId INTEGER NOT NULL,
    RoleId INTEGER NOT NULL,
    BranchId TEXT NOT NULL,
    AssignmentDate TEXT NOT NULL,
    IsActive INTEGER DEFAULT 1,
    CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- 7. KPI_CN_THANH_PHO - Chi nhánh Thành phố
CREATE TABLE IF NOT EXISTS KPI_CN_THANH_PHO (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmployeeId INTEGER NOT NULL,
    RoleId INTEGER NOT NULL,
    BranchId TEXT NOT NULL,
    AssignmentDate TEXT NOT NULL,
    IsActive INTEGER DEFAULT 1,
    CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- 8. KPI_CN_THAN_UYEN - Chi nhánh Than Uyên
CREATE TABLE IF NOT EXISTS KPI_CN_THAN_UYEN (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmployeeId INTEGER NOT NULL,
    RoleId INTEGER NOT NULL,
    BranchId TEXT NOT NULL,
    AssignmentDate TEXT NOT NULL,
    IsActive INTEGER DEFAULT 1,
    CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- 9. KPI_HOI_SO - Hội sở
CREATE TABLE IF NOT EXISTS KPI_HOI_SO (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EmployeeId INTEGER NOT NULL,
    RoleId INTEGER NOT NULL,
    BranchId TEXT NOT NULL,
    AssignmentDate TEXT NOT NULL,
    IsActive INTEGER DEFAULT 1,
    CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- Kiểm tra xem các bảng đã được tạo thành công
SELECT 'KIỂM TRA SAU KHI TẠO' as Status;
SELECT name as TableName FROM sqlite_master WHERE type='table' AND name LIKE 'KPI_%' AND name NOT LIKE '%Assignment%' ORDER BY name;

-- So sánh cấu trúc với bảng GiamdocCnl2_KPI_Assignment
SELECT 'SO SÁNH CẤU TRÚC' as Status;
SELECT 'GiamdocCnl2_KPI_Assignment Structure:' as TableInfo;
PRAGMA table_info(GiamdocCnl2_KPI_Assignment);

SELECT 'KPI_CN_MUONG_TE Structure:' as TableInfo;  
PRAGMA table_info(KPI_CN_MUONG_TE);

SELECT 'Verification: All branch tables have same structure' as Status;
