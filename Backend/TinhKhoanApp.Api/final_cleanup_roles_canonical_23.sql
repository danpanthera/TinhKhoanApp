-- Final cleanup to ensure exactly 23 canonical roles from SeedKPIDefinitionMaxScore.cs
-- This removes all existing roles and recreates only the 23 canonical roleCodes

-- Step 1: Drop all KPI assignment tables
DROP TABLE IF EXISTS TruongphongKhdn_KPI_Assignment;
DROP TABLE IF EXISTS TruongphongKhcn_KPI_Assignment;
DROP TABLE IF EXISTS PhophongKhdn_KPI_Assignment;
DROP TABLE IF EXISTS PhophongKhcn_KPI_Assignment;
DROP TABLE IF EXISTS TruongphongKhqlrr_KPI_Assignment;
DROP TABLE IF EXISTS PhophongKhqlrr_KPI_Assignment;
DROP TABLE IF EXISTS Cbtd_KPI_Assignment;
DROP TABLE IF EXISTS TruongphongKtnqCnl1_KPI_Assignment;
DROP TABLE IF EXISTS PhophongKtnqCnl1_KPI_Assignment;
DROP TABLE IF EXISTS Gdv_KPI_Assignment;
DROP TABLE IF EXISTS TqHkKtnb_KPI_Assignment;
DROP TABLE IF EXISTS TruongphongItThKtgs_KPI_Assignment;
DROP TABLE IF EXISTS CbItThKtgsKhqlrr_KPI_Assignment;
DROP TABLE IF EXISTS GiamdocPgd_KPI_Assignment;
DROP TABLE IF EXISTS PhogiamdocPgd_KPI_Assignment;
DROP TABLE IF EXISTS PhogiamdocPgdCbtd_KPI_Assignment;
DROP TABLE IF EXISTS GiamdocCnl2_KPI_Assignment;
DROP TABLE IF EXISTS PhogiamdocCnl2Td_KPI_Assignment;
DROP TABLE IF EXISTS PhogiamdocCnl2Kt_KPI_Assignment;
DROP TABLE IF EXISTS TruongphongKhCnl2_KPI_Assignment;
DROP TABLE IF EXISTS PhophongKhCnl2_KPI_Assignment;
DROP TABLE IF EXISTS TruongphongKtnqCnl2_KPI_Assignment;
DROP TABLE IF EXISTS PhophongKtnqCnl2_KPI_Assignment;

-- Drop any other potential KPI assignment tables
DROP TABLE IF EXISTS CBTD_KPI_Assignment;
DROP TABLE IF EXISTS GDV_KPI_Assignment;

-- Step 2: Delete all existing roles
DELETE FROM Roles;

-- Step 3: Insert exactly 23 canonical roles with proper descriptions
INSERT INTO Roles (Name, Description, IsActive, CreatedDate) VALUES
('TruongphongKhdn', 'Trưởng phòng KHDN', 1, CURRENT_TIMESTAMP),
('TruongphongKhcn', 'Trưởng phòng KHCN', 1, CURRENT_TIMESTAMP),
('PhophongKhdn', 'Phó phòng KHDN', 1, CURRENT_TIMESTAMP),
('PhophongKhcn', 'Phó phòng KHCN', 1, CURRENT_TIMESTAMP),
('TruongphongKhqlrr', 'Trưởng phòng KHQLRR', 1, CURRENT_TIMESTAMP),
('PhophongKhqlrr', 'Phó phòng KHQLRR', 1, CURRENT_TIMESTAMP),
('Cbtd', 'Cán bộ Tín dụng', 1, CURRENT_TIMESTAMP),
('TruongphongKtnqCnl1', 'Trưởng phòng KTNQ CNL1', 1, CURRENT_TIMESTAMP),
('PhophongKtnqCnl1', 'Phó phòng KTNQ CNL1', 1, CURRENT_TIMESTAMP),
('Gdv', 'Giao dịch viên', 1, CURRENT_TIMESTAMP),
('TqHkKtnb', 'TQ/Hậu kiểm/Kế toán nội bộ', 1, CURRENT_TIMESTAMP),
('TruongphongItThKtgs', 'Trưởng phòng IT/TH/KTGS', 1, CURRENT_TIMESTAMP),
('CbItThKtgsKhqlrr', 'CB IT/TH/KTGS/KHQLRR', 1, CURRENT_TIMESTAMP),
('GiamdocPgd', 'Giám đốc PGD', 1, CURRENT_TIMESTAMP),
('PhogiamdocPgd', 'Phó giám đốc PGD', 1, CURRENT_TIMESTAMP),
('PhogiamdocPgdCbtd', 'Phó giám đốc PGD kiêm CBTD', 1, CURRENT_TIMESTAMP),
('GiamdocCnl2', 'Giám đốc CNL2', 1, CURRENT_TIMESTAMP),
('PhogiamdocCnl2Td', 'Phó giám đốc CNL2 phụ trách Tín dụng', 1, CURRENT_TIMESTAMP),
('PhogiamdocCnl2Kt', 'Phó giám đốc CNL2 phụ trách Kinh tế', 1, CURRENT_TIMESTAMP),
('TruongphongKhCnl2', 'Trưởng phòng KH CNL2', 1, CURRENT_TIMESTAMP),
('PhophongKhCnl2', 'Phó phòng KH CNL2', 1, CURRENT_TIMESTAMP),
('TruongphongKtnqCnl2', 'Trưởng phòng KTNQ CNL2', 1, CURRENT_TIMESTAMP),
('PhophongKtnqCnl2', 'Phó phòng KTNQ CNL2', 1, CURRENT_TIMESTAMP);

-- Step 4: Create KPI assignment tables for all 23 canonical roles
CREATE TABLE TruongphongKhdn_KPI_Assignment (
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

CREATE TABLE TruongphongKhcn_KPI_Assignment (
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

CREATE TABLE PhophongKhdn_KPI_Assignment (
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

CREATE TABLE PhophongKhcn_KPI_Assignment (
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

CREATE TABLE TruongphongKhqlrr_KPI_Assignment (
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

CREATE TABLE PhophongKhqlrr_KPI_Assignment (
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

CREATE TABLE Cbtd_KPI_Assignment (
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

CREATE TABLE TruongphongKtnqCnl1_KPI_Assignment (
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

CREATE TABLE PhophongKtnqCnl1_KPI_Assignment (
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

CREATE TABLE Gdv_KPI_Assignment (
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

CREATE TABLE TqHkKtnb_KPI_Assignment (
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

CREATE TABLE TruongphongItThKtgs_KPI_Assignment (
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

CREATE TABLE CbItThKtgsKhqlrr_KPI_Assignment (
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

CREATE TABLE GiamdocPgd_KPI_Assignment (
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

CREATE TABLE PhogiamdocPgd_KPI_Assignment (
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

CREATE TABLE PhogiamdocPgdCbtd_KPI_Assignment (
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

CREATE TABLE GiamdocCnl2_KPI_Assignment (
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

CREATE TABLE PhogiamdocCnl2Td_KPI_Assignment (
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

CREATE TABLE PhogiamdocCnl2Kt_KPI_Assignment (
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

CREATE TABLE TruongphongKhCnl2_KPI_Assignment (
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

CREATE TABLE PhophongKhCnl2_KPI_Assignment (
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

CREATE TABLE TruongphongKtnqCnl2_KPI_Assignment (
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

CREATE TABLE PhophongKtnqCnl2_KPI_Assignment (
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

-- Verification queries
SELECT 'Total Roles:' as Query, COUNT(*) as Count FROM Roles;
SELECT 'Role Names:' as Query, Name as RoleName FROM Roles ORDER BY Name;
