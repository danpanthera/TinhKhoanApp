-- =============================================================
-- EXTRACT DỮ LIỆU THỰC TẾ TỪ BACKUP - PHIÊN BẢN CHÍNH XÁC
-- Backup Database: TinhKhoanDB_Restored (SQL Server 2022)
-- Target: Azure SQL Edge ARM64 (TinhKhoanDB)
-- =============================================================

USE TinhKhoanDB_Restored;
GO

PRINT '🚀 EXTRACT DỮ LIỆU THỰC TẾ - COMPATIBLE VERSION';
PRINT '===============================================';
PRINT '';

-- =============================================================
-- 1. EXTRACT UNITS (46 đơn vị)
-- =============================================================
PRINT '📤 1. EXTRACTING UNITS (46 đơn vị)...';
PRINT '';
PRINT '-- ===== UNITS DATA =====';

SELECT
    'INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsActive, CreatedAt, UpdatedAt) VALUES (' +
    CAST(Id AS VARCHAR) + ', ''' +
    REPLACE(Code, '''', '''''') + ''', ''' +
    REPLACE(Name, '''', '''''') + ''', ''' +
    ISNULL('''' + Type + '''', 'NULL') + ', ' +
    CASE WHEN ParentUnitId IS NULL THEN 'NULL' ELSE CAST(ParentUnitId AS VARCHAR) END + ', ' +
    CASE WHEN IsDeleted = 0 THEN '1' ELSE '0' END + ', ''' +
    '2025-07-06 10:00:00.000' + ''', ''' +
    '2025-07-06 10:00:00.000' + ''');' as SqlStatement
FROM Units
WHERE IsDeleted = 0
ORDER BY Id;

PRINT '';

-- =============================================================
-- 2. EXTRACT POSITIONS (7 chức vụ)
-- =============================================================
PRINT '📤 2. EXTRACTING POSITIONS (7 chức vụ)...';
PRINT '';
PRINT '-- ===== POSITIONS DATA =====';

SELECT
    'INSERT INTO Positions (Id, Name, Description, IsActive, CreatedAt, UpdatedAt) VALUES (' +
    CAST(Id AS VARCHAR) + ', ''' +
    REPLACE(Name, '''', '''''') + ''', ''' +
    ISNULL('''' + REPLACE(Description, '''', '''''') + '''', 'NULL') + ', ' +
    '1' + ', ''' +
    '2025-07-06 10:00:00.000' + ''', ''' +
    '2025-07-06 10:00:00.000' + ''');' as SqlStatement
FROM Positions
ORDER BY Id;

PRINT '';

-- =============================================================
-- 3. EXTRACT EMPLOYEES (loại trừ admin)
-- =============================================================
PRINT '📤 3. EXTRACTING EMPLOYEES (loại trừ admin)...';
PRINT '';
PRINT '-- ===== EMPLOYEES DATA =====';

SELECT
    'INSERT INTO Employees (Id, EmployeeCode, CbCode, FullName, Username, Email, PhoneNumber, IsActive, UnitId, PositionId, CreatedAt, UpdatedAt) VALUES (' +
    CAST(Id AS VARCHAR) + ', ''' +
    REPLACE(EmployeeCode, '''', '''''') + ''', ''' +
    REPLACE(CBCode, '''', '''''') + ''', ''' +
    REPLACE(FullName, '''', '''''') + ''', ''' +
    REPLACE(Username, '''', '''''') + ''', ''' +
    ISNULL('''' + REPLACE(Email, '''', '''''') + '''', 'NULL') + ', ''' +
    ISNULL('''' + REPLACE(PhoneNumber, '''', '''''') + '''', 'NULL') + ', ' +
    CASE WHEN IsActive = 1 THEN '1' ELSE '0' END + ', ' +
    CAST(UnitId AS VARCHAR) + ', ' +
    CAST(PositionId AS VARCHAR) + ', ''' +
    '2025-07-06 10:00:00.000' + ''', ''' +
    '2025-07-06 10:00:00.000' + ''');' as SqlStatement
FROM Employees
WHERE Username != 'admin' AND IsActive = 1
ORDER BY Id;

PRINT '';
PRINT '✅ EXTRACT HOÀN THÀNH!';

GO
