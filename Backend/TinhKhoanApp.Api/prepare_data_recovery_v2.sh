#!/bin/bash

# ==================================================
# SCRIPT PHỤC HỒI DỮ LIỆU CHO TINHKHOAN APP
# AZURE SQL EDGE ARM64 - MACOS
# ==================================================

echo "🚀 BẮT ĐẦU QUY TRÌNH PHỤC HỒI DỮ LIỆU"
echo "======================================"

# Thông tin database
SOURCE_BACKUP="/Users/nguyendat/Documents/Projects/TinhKhoanApp/database_backup/backup/TinhKhoanDB_backup_20250706_160954.bak"
TEMP_DB="TinhKhoanDB_Backup_Temp"
TARGET_DB="TinhKhoanDB"
OUTPUT_DIR="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/data_export"

echo "📁 Backup file: $SOURCE_BACKUP"
echo "🎯 Target database: $TARGET_DB"
echo "📤 Export directory: $OUTPUT_DIR"
echo ""

# Tạo thư mục export nếu chưa có
mkdir -p "$OUTPUT_DIR"

echo "🔹 BƯỚC 1: HƯỚNG DẪN EXTRACT DỮ LIỆU"
echo "====================================="
echo ""
echo "⚠️  Azure SQL Edge ARM64 không support trực tiếp RESTORE từ .bak file"
echo "💡 Cần sử dụng SQL Server (Windows/Docker x64) để extract dữ liệu"
echo ""
echo "🔧 CÁC CÁCH THỰC HIỆN:"
echo ""
echo "📌 CÁCH 1: Sử dụng SQL Server Docker x64 (KHUYẾN NGHỊ)"
echo "   1. docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=YourStrong!Passw0rd' -p 1434:1433 --name sqlserver-extract mcr.microsoft.com/mssql/server:2019-latest"
echo "   2. Restore backup vào SQL Server này"
echo "   3. Export dữ liệu ra CSV"
echo ""
echo "📌 CÁCH 2: Sử dụng SSMS trên Windows"
echo "   1. Restore backup file"
echo "   2. Export dữ liệu qua wizard"
echo ""
echo "📌 CÁCH 3: Sử dụng Azure Data Studio"
echo "   1. Connect tới SQL Server có backup"
echo "   2. Export qua query results"
echo ""

echo "🔹 BƯỚC 2: TẠO SCRIPT EXTRACT SQL"
echo "=================================="

# Tạo script SQL để extract dữ liệu
cat > "$OUTPUT_DIR/extract_data.sql" << 'EOF'
-- ==================================================
-- SCRIPT EXTRACT DỮ LIỆU TỪ BACKUP
-- Chạy script này trên SQL Server có restore backup
-- ==================================================

USE TinhKhoanDB;  -- Hoặc tên database sau khi restore
GO

-- Tạo thư mục export (cần quyền admin trên SQL Server)
-- EXEC xp_cmdshell 'mkdir C:\temp\tinhkhoan_export'

-- ==================================================
-- 1. EXPORT UNITS (46 đơn vị)
-- ==================================================
PRINT '📤 Exporting Units...';
SELECT
    Id,
    Code,
    Name,
    Type,
    ParentUnitId,
    IsActive,
    CreatedAt,
    UpdatedAt
FROM Units
ORDER BY Id;

-- Export to CSV (nếu có quyền)
-- EXEC xp_cmdshell 'bcp "SELECT Id,Code,Name,Type,ParentUnitId,IsActive,CreatedAt,UpdatedAt FROM TinhKhoanDB.dbo.Units ORDER BY Id" queryout "C:\temp\tinhkhoan_export\units.csv" -c -t"," -T -S localhost'

-- ==================================================
-- 2. EXPORT POSITIONS (Chức vụ)
-- ==================================================
PRINT '📤 Exporting Positions...';
SELECT
    Id,
    Name,
    Description,
    IsActive,
    CreatedAt,
    UpdatedAt
FROM Positions
ORDER BY Id;

-- ==================================================
-- 3. EXPORT EMPLOYEES (Nhân viên)
-- ==================================================
PRINT '📤 Exporting Employees...';
SELECT
    Id,
    EmployeeCode,
    CbCode,
    FullName,
    Username,
    Email,
    PhoneNumber,
    IsActive,
    UnitId,
    PositionId,
    CreatedAt,
    UpdatedAt
FROM Employees
WHERE Username != 'admin'  -- Loại trừ admin vì đã có
ORDER BY Id;

-- ==================================================
-- 4. EXPORT KPI DEFINITIONS
-- ==================================================
PRINT '📤 Exporting KPIDefinitions...';
SELECT
    Id,
    Code,
    Name,
    Description,
    UnitOfMeasure,
    TargetType,
    IsActive,
    CreatedAt,
    UpdatedAt
FROM KPIDefinitions
ORDER BY Id;

-- ==================================================
-- 5. EXPORT KPI INDICATORS
-- ==================================================
PRINT '📤 Exporting KpiIndicators...';
SELECT
    Id,
    KPIDefinitionId,
    Name,
    Description,
    Weight,
    IsActive,
    CreatedAt,
    UpdatedAt
FROM KpiIndicators
ORDER BY Id;

-- ==================================================
-- 6. EXPORT KPI SCORING RULES
-- ==================================================
PRINT '📤 Exporting KpiScoringRules...';
SELECT
    Id,
    KpiIndicatorId,
    MinValue,
    MaxValue,
    Score,
    Description,
    IsActive,
    CreatedAt,
    UpdatedAt
FROM KpiScoringRules
ORDER BY Id;

-- ==================================================
-- 7. EXPORT BUSINESS PLAN TARGETS
-- ==================================================
PRINT '📤 Exporting BusinessPlanTargets...';
SELECT
    Id,
    Year,
    Quarter,
    Month,
    UnitId,
    KPIDefinitionId,
    TargetValue,
    IsActive,
    CreatedAt,
    UpdatedAt
FROM BusinessPlanTargets
ORDER BY Year, Quarter, Month, UnitId, KPIDefinitionId;

-- ==================================================
-- THỐNG KÊ DỮ LIỆU
-- ==================================================
PRINT '';
PRINT '📊 THỐNG KÊ DỮ LIỆU CẦN EXPORT:';
SELECT 'Units' as TableName, COUNT(*) as RecordCount FROM Units
UNION ALL
SELECT 'Positions' as TableName, COUNT(*) as RecordCount FROM Positions
UNION ALL
SELECT 'Employees' as TableName, COUNT(*) as RecordCount FROM Employees
UNION ALL
SELECT 'KPIDefinitions' as TableName, COUNT(*) as RecordCount FROM KPIDefinitions
UNION ALL
SELECT 'KpiIndicators' as TableName, COUNT(*) as RecordCount FROM KpiIndicators
UNION ALL
SELECT 'KpiScoringRules' as TableName, COUNT(*) as RecordCount FROM KpiScoringRules
UNION ALL
SELECT 'BusinessPlanTargets' as TableName, COUNT(*) as RecordCount FROM BusinessPlanTargets
ORDER BY TableName;

PRINT '';
PRINT '✅ HOÀN THÀNH EXTRACT. Copy kết quả và chuyển sang Azure SQL Edge.';

GO
EOF

echo "✅ Đã tạo script extract: $OUTPUT_DIR/extract_data.sql"
echo ""

echo "🔹 BƯỚC 3: CHUẨN BỊ SCRIPT IMPORT"
echo "================================="

# Tạo script template để import dữ liệu
cat > "$OUTPUT_DIR/import_template.sql" << 'EOF'
-- ==================================================
-- SCRIPT IMPORT DỮ LIỆU VÀO AZURE SQL EDGE
-- Chạy sau khi đã có dữ liệu từ extract
-- ==================================================

USE TinhKhoanDB;
GO

-- ==================================================
-- BƯỚC 1: BACKUP DỮ LIỆU HIỆN TẠI (TẠM THỜI)
-- ==================================================
PRINT '🔄 Backup dữ liệu admin hiện tại...';

-- Lưu thông tin admin để khôi phục sau
DECLARE @AdminUserId INT = (SELECT Id FROM Users WHERE Username = 'admin');
DECLARE @AdminUnitId INT = (SELECT UnitId FROM Employees WHERE Username = 'admin');
DECLARE @AdminPositionId INT = (SELECT PositionId FROM Employees WHERE Username = 'admin');

PRINT '  📝 Admin User ID: ' + CAST(@AdminUserId AS VARCHAR);
PRINT '  🏢 Admin Unit ID: ' + CAST(@AdminUnitId AS VARCHAR);
PRINT '  👤 Admin Position ID: ' + CAST(@AdminPositionId AS VARCHAR);

-- ==================================================
-- BƯỚC 2: XÓA DỮ LIỆU TẠM THỜI (GIỮ LẠI ADMIN)
-- ==================================================
PRINT '';
PRINT '🗑️  Xóa dữ liệu tạm thời...';

-- Xóa theo thứ tự dependency
DELETE FROM BusinessPlanTargets;
DELETE FROM KpiScoringRules;
DELETE FROM KpiIndicators;
DELETE FROM KPIDefinitions;
DELETE FROM Employees WHERE Username != 'admin';
-- Giữ lại Position và Unit của admin

PRINT '  ✅ Đã xóa dữ liệu tạm thời, giữ lại admin';

-- ==================================================
-- BƯỚC 3: INSERT DỮ LIỆU THỰC TẾ
-- ==================================================
PRINT '';
PRINT '📥 Bắt đầu import dữ liệu thực tế...';

-- 3.1. IMPORT UNITS (46 đơn vị)
PRINT '  📤 Importing Units...';
-- TODO: INSERT statements từ kết quả extract
-- INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsActive, CreatedAt, UpdatedAt) VALUES
-- (...data từ extract...)

-- 3.2. IMPORT POSITIONS (Chức vụ)
PRINT '  📤 Importing Positions...';
-- TODO: INSERT statements từ kết quả extract
-- INSERT INTO Positions (Id, Name, Description, IsActive, CreatedAt, UpdatedAt) VALUES
-- (...data từ extract...)

-- 3.3. IMPORT EMPLOYEES (Nhân viên)
PRINT '  📤 Importing Employees...';
-- TODO: INSERT statements từ kết quả extract
-- INSERT INTO Employees (...) VALUES (...)

-- 3.4. IMPORT KPI DEFINITIONS
PRINT '  📤 Importing KPIDefinitions...';
-- TODO: INSERT statements

-- 3.5. IMPORT KPI INDICATORS
PRINT '  📤 Importing KpiIndicators...';
-- TODO: INSERT statements

-- 3.6. IMPORT KPI SCORING RULES
PRINT '  📤 Importing KpiScoringRules...';
-- TODO: INSERT statements

-- 3.7. IMPORT BUSINESS PLAN TARGETS
PRINT '  📤 Importing BusinessPlanTargets...';
-- TODO: INSERT statements

-- ==================================================
-- BƯỚC 4: VERIFY DỮ LIỆU SAU IMPORT
-- ==================================================
PRINT '';
PRINT '✅ VERIFY DỮ LIỆU SAU IMPORT:';
SELECT 'Units' as TableName, COUNT(*) as RecordCount FROM Units
UNION ALL
SELECT 'Positions' as TableName, COUNT(*) as RecordCount FROM Positions
UNION ALL
SELECT 'Employees' as TableName, COUNT(*) as RecordCount FROM Employees
UNION ALL
SELECT 'KPIDefinitions' as TableName, COUNT(*) as RecordCount FROM KPIDefinitions
UNION ALL
SELECT 'KpiIndicators' as TableName, COUNT(*) as RecordCount FROM KpiIndicators
UNION ALL
SELECT 'KpiScoringRules' as TableName, COUNT(*) as RecordCount FROM KpiScoringRules
UNION ALL
SELECT 'BusinessPlanTargets' as TableName, COUNT(*) as RecordCount FROM BusinessPlanTargets
ORDER BY TableName;

-- Kiểm tra admin vẫn hoạt động
SELECT 'Admin User Exists' as CheckName, COUNT(*) as Result FROM Users WHERE Username = 'admin';

PRINT '';
PRINT '🎯 MỤC TIÊU ĐẠT ĐƯỢC:';
PRINT '  ✅ 46 Units (Đơn vị)';
PRINT '  ✅ Đầy đủ Positions (Chức vụ)';
PRINT '  ✅ Đầy đủ Employees (Nhân viên)';
PRINT '  ✅ Đầy đủ KPI Definitions';
PRINT '  ✅ Admin vẫn hoạt động bình thường';
PRINT '';
PRINT '🚀 DỮ LIỆU ĐÃ SẴN SÀNG CHO SẢN XUẤT!';

GO
EOF

echo "✅ Đã tạo template import: $OUTPUT_DIR/import_template.sql"
echo ""

echo "🔹 BƯỚC 4: HƯỚNG DẪN THỰC HIỆN"
echo "=============================="
echo ""
echo "📋 CÁC BƯỚC TIẾP THEO:"
echo "1. 🐳 Khởi động SQL Server Docker x64 (cổng 1434)"
echo "2. 🔄 Restore backup vào SQL Server"
echo "3. 📤 Chạy script extract_data.sql"
echo "4. 📋 Copy kết quả vào import_template.sql"
echo "5. 📥 Chạy script import vào Azure SQL Edge"
echo "6. ✅ Verify dữ liệu qua API"
echo ""
echo "💡 TIP: Có thể dùng Azure Data Studio để dễ dàng copy/paste data"
echo ""
echo "🚀 HOÀN THÀNH SCRIPT CHUẨN BỊ!"

# Hiển thị thông tin file
echo ""
echo "📁 FILES ĐÃ TẠO:"
echo "  - $OUTPUT_DIR/extract_data.sql"
echo "  - $OUTPUT_DIR/import_template.sql"
echo ""
echo "🔍 Xem chi tiết: ls -la $OUTPUT_DIR"
