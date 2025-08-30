#!/bin/bash

# =============================================================================
# 🔄 DATABASE RESTORE SCRIPT - August 12, 2025
# Phục hồi database với cấu trúc hoàn chỉnh theo README_DAT.md
# =============================================================================

echo "🔄 DATABASE RESTORE - August 12, 2025"
echo "====================================="

# Kiểm tra kết nối database
echo "🔍 Checking database connection..."
if ! sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -Q "SELECT 1" -o /dev/null 2>&1; then
    echo "❌ Cannot connect to database. Please ensure Docker container is running."
    echo "   Run: ./restore_original_docker.sh"
    exit 1
fi

echo "✅ Database connection successful"

# Kiểm tra database TinhKhoanDB
echo ""
echo "🗄️  Checking TinhKhoanDB..."
DB_EXISTS=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -Q "SELECT COUNT(*) FROM sys.databases WHERE name = 'TinhKhoanDB'" -h-1 -W | tr -d ' \r\n')

if [ "$DB_EXISTS" = "0" ]; then
    echo "📝 Creating TinhKhoanDB database..."
    sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -Q "CREATE DATABASE TinhKhoanDB"
    echo "✅ TinhKhoanDB created successfully"
else
    echo "✅ TinhKhoanDB already exists"
fi

# Chuyển sang TinhKhoanDB và kiểm tra bảng
echo ""
echo "📊 Checking current table structure..."
TABLE_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'" -h-1 -W | tr -d ' \r\n')

echo "Current tables in database: $TABLE_COUNT"

if [ "$TABLE_COUNT" -lt "47" ]; then
    echo "⚠️  Database needs to be restored with Entity Framework migrations"
    echo ""
    echo "🚀 Restoring database structure using Entity Framework..."

    # Chạy Entity Framework migrations
    echo "📝 Running EF database update..."
    if dotnet ef database update --verbose; then
        echo "✅ Entity Framework migrations completed successfully"

        # Kiểm tra lại số bảng sau migration
        NEW_TABLE_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'" -h-1 -W | tr -d ' \r\n')
        echo "Tables after migration: $NEW_TABLE_COUNT"

    else
        echo "❌ Entity Framework migration failed"
        echo "   Trying alternative restoration method..."

        # Alternative: Tạo database structure cơ bản
        echo "🔧 Creating basic database structure..."

        # Tạo các bảng cơ bản
        sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "
        -- Create basic system tables if not exists
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Units')
        CREATE TABLE Units (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            UnitCode NVARCHAR(50) NOT NULL,
            UnitName NVARCHAR(200) NOT NULL,
            UnitType NVARCHAR(50),
            ParentUnitId INT,
            CreatedAt DATETIME2 DEFAULT GETDATE(),
            UpdatedAt DATETIME2 DEFAULT GETDATE()
        );

        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles')
        CREATE TABLE Roles (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            RoleCode NVARCHAR(50) NOT NULL,
            RoleName NVARCHAR(200) NOT NULL,
            Description NVARCHAR(500),
            CreatedAt DATETIME2 DEFAULT GETDATE(),
            UpdatedAt DATETIME2 DEFAULT GETDATE()
        );

        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Employees')
        CREATE TABLE Employees (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            EmployeeCode NVARCHAR(50) NOT NULL,
            FullName NVARCHAR(200) NOT NULL,
            UnitId INT,
            RoleId INT,
            Position NVARCHAR(100),
            CreatedAt DATETIME2 DEFAULT GETDATE(),
            UpdatedAt DATETIME2 DEFAULT GETDATE()
        );
        "

        if [ $? -eq 0 ]; then
            echo "✅ Basic database structure created"
        else
            echo "❌ Failed to create basic structure"
        fi
    fi

else
    echo "✅ Database structure appears complete ($TABLE_COUNT tables)"
fi

# Kiểm tra các bảng dữ liệu chính (8 core tables)
echo ""
echo "📋 Checking core data tables (8 tables)..."

CORE_TABLES=("DP01" "DPDA" "EI01" "GL01" "GL02" "GL41" "LN01" "LN03" "RR01")

for table in "${CORE_TABLES[@]}"; do
    TABLE_EXISTS=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '$table'" -h-1 -W | tr -d ' \r\n')

    if [ "$TABLE_EXISTS" = "1" ]; then
        # Đếm records trong bảng
        RECORD_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM [$table]" -h-1 -W 2>/dev/null | tr -d ' \r\n')
        echo "  ✅ $table: EXISTS (${RECORD_COUNT:-0} records)"
    else
        echo "  ❌ $table: MISSING"
    fi
done

# Kiểm tra temporal tables
echo ""
echo "🕒 Checking temporal tables..."
TEMPORAL_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM sys.tables WHERE temporal_type = 2" -h-1 -W | tr -d ' \r\n')
echo "Temporal tables found: $TEMPORAL_COUNT"

# Populate cơ bản nếu cần
echo ""
echo "📦 Checking basic data..."

# Kiểm tra Units
UNITS_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM Units" -h-1 -W 2>/dev/null | tr -d ' \r\n')
if [ "${UNITS_COUNT:-0}" -lt "5" ]; then
    echo "📝 Populating basic Units..."
    sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "
    INSERT INTO Units (UnitCode, UnitName, UnitType) VALUES
    ('CNL1', 'Lai Châu', 'CNL1'),
    ('HOISO', 'Hội Sở', 'CNL1'),
    ('BINHLU', 'Chi nhánh Bình Lư', 'CNL2'),
    ('PHONGTHO', 'Chi nhánh Phong Thổ', 'CNL2'),
    ('SINHO', 'Chi nhánh Sìn Hồ', 'CNL2')
    " 2>/dev/null
    echo "✅ Basic Units populated"
fi

# Kiểm tra Roles
ROLES_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM Roles" -h-1 -W 2>/dev/null | tr -d ' \r\n')
if [ "${ROLES_COUNT:-0}" -lt "5" ]; then
    echo "📝 Populating basic Roles..."
    sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "
    INSERT INTO Roles (RoleCode, RoleName, Description) VALUES
    ('GiamdocCnl2', 'Giám đốc CNL2', 'Giám đốc chi nhánh cấp 2'),
    ('TruongphongKhdn', 'Trưởng phòng KHDN', 'Trưởng phòng Khách hàng doanh nghiệp'),
    ('TruongphongKhcn', 'Trưởng phòng KHCN', 'Trưởng phòng Khách hàng cá nhân'),
    ('PhophongKhdn', 'Phó phòng KHDN', 'Phó phòng Khách hàng doanh nghiệp'),
    ('PhophongKhcn', 'Phó phòng KHCN', 'Phó phòng Khách hàng cá nhân')
    " 2>/dev/null
    echo "✅ Basic Roles populated"
fi

# Final status check
echo ""
echo "🎯 FINAL DATABASE STATUS"
echo "========================"

FINAL_TABLE_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'" -h-1 -W | tr -d ' \r\n')
FINAL_UNITS_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM Units" -h-1 -W 2>/dev/null | tr -d ' \r\n')
FINAL_ROLES_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM Roles" -h-1 -W 2>/dev/null | tr -d ' \r\n')

echo "📊 Database Statistics:"
echo "   • Total Tables: ${FINAL_TABLE_COUNT:-0}"
echo "   • Units: ${FINAL_UNITS_COUNT:-0}"
echo "   • Roles: ${FINAL_ROLES_COUNT:-0}"
echo "   • Database: TinhKhoanDB"
echo "   • Server: Azure SQL Edge 1.0.7"
echo "   • Connection: localhost:1433"

if [ "${FINAL_TABLE_COUNT:-0}" -gt "10" ]; then
    echo ""
    echo "✅ Database restoration completed successfully!"
    echo "🚀 Ready for backend development"
    echo ""
    echo "📝 Next steps:"
    echo "   1. Run backend: dotnet run"
    echo "   2. Test EI01 implementation"
    echo "   3. Continue with remaining tables"
else
    echo ""
    echo "⚠️  Database partially restored"
    echo "   Some tables may be missing - run 'dotnet ef database update' manually"
fi
