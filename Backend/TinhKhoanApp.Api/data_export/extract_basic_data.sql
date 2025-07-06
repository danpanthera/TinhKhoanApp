-- =============================================================
-- EXTRACT DỮ LIỆU TỪ BACKUP CHO AZURE SQL EDGE
-- Backup Database: TinhKhoanDB_Restored (SQL Server 2022)
-- Target: Azure SQL Edge ARM64 (TinhKhoanDB)
-- =============================================================

USE TinhKhoanDB_Restored;
GO

PRINT '🚀 BẮT ĐẦU EXTRACT DỮ LIỆU TỪ BACKUP';
PRINT '=====================================';
PRINT '';

-- =============================================================
-- 1. EXTRACT UNITS (46 đơn vị)
-- =============================================================
PRINT '📤 1. EXTRACTING UNITS (46 đơn vị)...';
PRINT '';

SELECT
    'INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsActive, CreatedAt, UpdatedAt) VALUES (' +
    CAST(Id AS VARCHAR) + ', ''' +
    REPLACE(Code, '''', '''''') + ''', ''' +
    REPLACE(Name, '''', '''''') + ''', ''' +
    Type + ''', ' +
    CASE WHEN ParentUnitId IS NULL THEN 'NULL' ELSE CAST(ParentUnitId AS VARCHAR) END + ', ' +
    CASE WHEN IsActive = 1 THEN '1' ELSE '0' END + ', ''' +
    CONVERT(VARCHAR, CreatedAt, 120) + ''', ''' +
    CONVERT(VARCHAR, UpdatedAt, 120) + ''');'
FROM Units
ORDER BY Id;

PRINT '';
PRINT '✅ Units extracted: 46 records';
PRINT '';

-- =============================================================
-- 2. EXTRACT POSITIONS (7 chức vụ)
-- =============================================================
PRINT '📤 2. EXTRACTING POSITIONS (7 chức vụ)...';
PRINT '';

SELECT
    'INSERT INTO Positions (Id, Name, Description, IsActive, CreatedAt, UpdatedAt) VALUES (' +
    CAST(Id AS VARCHAR) + ', ''' +
    REPLACE(Name, '''', '''''') + ''', ''' +
    ISNULL(REPLACE(Description, '''', ''''''), '') + ''', ' +
    CASE WHEN IsActive = 1 THEN '1' ELSE '0' END + ', ''' +
    CONVERT(VARCHAR, CreatedAt, 120) + ''', ''' +
    CONVERT(VARCHAR, UpdatedAt, 120) + ''');'
FROM Positions
ORDER BY Id;

PRINT '';
PRINT '✅ Positions extracted: 7 records';
PRINT '';

-- =============================================================
-- 3. EXTRACT EMPLOYEES (3 nhân viên - loại trừ admin)
-- =============================================================
PRINT '📤 3. EXTRACTING EMPLOYEES (loại trừ admin)...';
PRINT '';

SELECT
    'INSERT INTO Employees (Id, EmployeeCode, CbCode, FullName, Username, Email, PhoneNumber, IsActive, UnitId, PositionId, CreatedAt, UpdatedAt) VALUES (' +
    CAST(Id AS VARCHAR) + ', ''' +
    ISNULL(REPLACE(EmployeeCode, '''', ''''''), '') + ''', ''' +
    ISNULL(REPLACE(CbCode, '''', ''''''), '') + ''', ''' +
    REPLACE(FullName, '''', '''''') + ''', ''' +
    ISNULL(REPLACE(Username, '''', ''''''), '') + ''', ''' +
    ISNULL(REPLACE(Email, '''', ''''''), '') + ''', ''' +
    ISNULL(REPLACE(PhoneNumber, '''', ''''''), '') + ''', ' +
    CASE WHEN IsActive = 1 THEN '1' ELSE '0' END + ', ' +
    CAST(UnitId AS VARCHAR) + ', ' +
    CAST(PositionId AS VARCHAR) + ', ''' +
    CONVERT(VARCHAR, CreatedAt, 120) + ''', ''' +
    CONVERT(VARCHAR, UpdatedAt, 120) + ''');'
FROM Employees
WHERE Username != 'admin' OR Username IS NULL
ORDER BY Id;

PRINT '';
PRINT '✅ Employees extracted (excluding admin)';
PRINT '';

GO
