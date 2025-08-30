#!/bin/bash

# =============================================================================
# 🎯 DATABASE RESTORE COMPLETION SCRIPT - August 12, 2025
# Hoàn tất phục hồi database với dữ liệu cơ bản
# =============================================================================

echo "🎯 DATABASE RESTORE COMPLETION - August 12, 2025"
echo "================================================"

# Kiểm tra database đã tồn tại - simplified approach
if sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -Q "SELECT name FROM sys.databases WHERE name = 'KhoanDB'" -h-1 | grep -q "KhoanDB"; then
    echo "✅ KhoanDB found"
else
    echo "❌ KhoanDB not found"
    exit 1
fi

# Kiểm tra cấu trúc bảng
TABLE_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d KhoanDB -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'" -h-1 -W | tr -d ' \r\n')
echo "📊 Total tables: $TABLE_COUNT"

# Populate dữ liệu cơ bản cho Units
echo ""
echo "📝 Populating basic data..."

# Units data
echo "   • Adding Units..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d KhoanDB -Q "
IF (SELECT COUNT(*) FROM Units) = 0
BEGIN
    SET IDENTITY_INSERT Units ON;
    INSERT INTO Units (Id, UnitCode, UnitName, UnitType, ParentUnitId, CreatedAt, UpdatedAt) VALUES
    (1, 'CNL1', 'Lai Châu', 'CNL1', NULL, GETDATE(), GETDATE()),
    (2, 'HOISO', 'Hội Sở', 'CNL1', 1, GETDATE(), GETDATE()),
    (3, 'BINHLU', 'Chi nhánh Bình Lư', 'CNL2', 1, GETDATE(), GETDATE()),
    (4, 'PHONGTHO', 'Chi nhánh Phong Thổ', 'CNL2', 1, GETDATE(), GETDATE()),
    (5, 'SINHO', 'Chi nhánh Sìn Hồ', 'CNL2', 1, GETDATE(), GETDATE()),
    (6, 'BUMTO', 'Chi nhánh Bum Tở', 'CNL2', 1, GETDATE(), GETDATE());
    SET IDENTITY_INSERT Units OFF;
    PRINT '✅ Units populated: 6 records';
END
ELSE
    PRINT '⚠️  Units already has data';
"

# Roles data
echo "   • Adding Roles..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d KhoanDB -Q "
IF (SELECT COUNT(*) FROM Roles) = 0
BEGIN
    SET IDENTITY_INSERT Roles ON;
    INSERT INTO Roles (Id, RoleCode, RoleName, Description, CreatedAt, UpdatedAt) VALUES
    (1, 'GiamdocCnl2', 'Giám đốc CNL2', 'Giám đốc chi nhánh cấp 2', GETDATE(), GETDATE()),
    (2, 'TruongphongKhdn', 'Trưởng phòng KHDN', 'Trưởng phòng Khách hàng doanh nghiệp', GETDATE(), GETDATE()),
    (3, 'TruongphongKhcn', 'Trưởng phòng KHCN', 'Trưởng phòng Khách hàng cá nhân', GETDATE(), GETDATE()),
    (4, 'PhophongKhdn', 'Phó phòng KHDN', 'Phó phòng Khách hàng doanh nghiệp', GETDATE(), GETDATE()),
    (5, 'PhophongKhcn', 'Phó phòng KHCN', 'Phó phòng Khách hàng cá nhân', GETDATE(), GETDATE()),
    (6, 'Cbtd', 'Cán bộ tín dụng', 'Cán bộ tín dụng', GETDATE(), GETDATE()),
    (7, 'Gdv', 'Giao dịch viên', 'Giao dịch viên', GETDATE(), GETDATE());
    SET IDENTITY_INSERT Roles OFF;
    PRINT '✅ Roles populated: 7 records';
END
ELSE
    PRINT '⚠️  Roles already has data';
"

# Employees data
echo "   • Adding Employees..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d KhoanDB -Q "
IF (SELECT COUNT(*) FROM Employees) = 0
BEGIN
    SET IDENTITY_INSERT Employees ON;
    INSERT INTO Employees (Id, EmployeeCode, FullName, UnitId, RoleId, Position, CreatedAt, UpdatedAt) VALUES
    (1, 'EMP001', 'Nguyễn Văn A', 1, 1, 'Giám đốc', GETDATE(), GETDATE()),
    (2, 'EMP002', 'Trần Thị B', 2, 2, 'Trưởng phòng', GETDATE(), GETDATE()),
    (3, 'EMP003', 'Lê Văn C', 3, 3, 'Trưởng phòng', GETDATE(), GETDATE()),
    (4, 'EMP004', 'Phạm Thị D', 4, 4, 'Phó phòng', GETDATE(), GETDATE()),
    (5, 'EMP005', 'Hoàng Văn E', 5, 5, 'Phó phòng', GETDATE(), GETDATE());
    SET IDENTITY_INSERT Employees OFF;
    PRINT '✅ Employees populated: 5 records';
END
ELSE
    PRINT '⚠️  Employees already has data';
"

# Final verification
echo ""
echo "🔍 Final verification..."

UNITS_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d KhoanDB -Q "SELECT COUNT(*) FROM Units" -h-1 -W | tr -d ' \r\n')
ROLES_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d KhoanDB -Q "SELECT COUNT(*) FROM Roles" -h-1 -W | tr -d ' \r\n')
EMP_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d KhoanDB -Q "SELECT COUNT(*) FROM Employees" -h-1 -W | tr -d ' \r\n')

# Check core data tables
echo "📋 Core data tables status:"
CORE_TABLES=("DP01" "DPDA" "EI01" "GL01" "GL02" "GL41" "LN01" "LN03" "RR01")

for table in "${CORE_TABLES[@]}"; do
    TABLE_EXISTS=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d KhoanDB -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '$table'" -h-1 -W 2>/dev/null | tr -d ' \r\n')

    if [ "${TABLE_EXISTS:-0}" = "1" ]; then
        RECORD_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d KhoanDB -Q "SELECT COUNT(*) FROM [$table]" -h-1 -W 2>/dev/null | tr -d ' \r\n')
        echo "   • $table: ✅ (${RECORD_COUNT:-0} records)"
    else
        echo "   • $table: ❌ (missing)"
    fi
done

# Check temporal tables
TEMPORAL_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d KhoanDB -Q "SELECT COUNT(*) FROM sys.tables WHERE temporal_type = 2" -h-1 -W 2>/dev/null | tr -d ' \r\n')

echo ""
echo "🎯 RESTORE SUMMARY - August 12, 2025"
echo "===================================="
echo "✅ Database: KhoanDB (restored from backup)"
echo "📊 Total tables: ${TABLE_COUNT:-0}"
echo "🕒 Temporal tables: ${TEMPORAL_COUNT:-0}"
echo "👥 Basic data:"
echo "   • Units: ${UNITS_COUNT:-0}"
echo "   • Roles: ${ROLES_COUNT:-0}"
echo "   • Employees: ${EMP_COUNT:-0}"
echo ""
echo "📝 Source backup: azure_sql_backup_20250812_112407.tar.gz"
echo "⏰ Restore completed: $(date)"
echo ""
echo "🚀 Database is ready for development!"
echo "   • Backend: dotnet run"
echo "   • Connection: localhost:1433"
echo "   • Database: KhoanDB"
echo "   • Password: Dientoan@303"
echo ""
echo "📋 Next steps:"
echo "   1. Start backend server"
echo "   2. Test EI01 implementation"
echo "   3. Import CSV data as needed"
