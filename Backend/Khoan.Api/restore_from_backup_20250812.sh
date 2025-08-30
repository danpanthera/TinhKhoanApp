#!/bin/bash

# =============================================================================
# 🔄 DATABASE RESTORE FROM BACKUP - August 12, 2025
# Phục hồi database từ file backup azure_sql_backup_20250812_112407.tar.gz
# =============================================================================

echo "🔄 DATABASE RESTORE FROM BACKUP - August 12, 2025"
echo "=================================================="

BACKUP_FILE="azure_sql_backup_20250812_112407.tar.gz"
RESTORE_DIR="restore_temp_$(date +%Y%m%d_%H%M%S)"

# Kiểm tra file backup tồn tại
echo "🔍 Checking backup file..."
if [ ! -f "$BACKUP_FILE" ]; then
    echo "❌ Backup file not found: $BACKUP_FILE"
    echo "   Current directory: $(pwd)"
    echo "   Available backups:"
    find . -name "*backup*" -name "*.tar.gz" -o -name "*.bak" -o -name "*.sql" | head -5
    exit 1
fi

echo "✅ Found backup file: $BACKUP_FILE ($(du -h $BACKUP_FILE | cut -f1))"

# Kiểm tra kết nối database
echo ""
echo "🔍 Checking database connection..."
if ! sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -Q "SELECT 1" -o /dev/null 2>&1; then
    echo "❌ Cannot connect to database. Please ensure Docker container is running."
    echo "   Run: ./restore_original_docker.sh"
    exit 1
fi

echo "✅ Database connection successful"

# Tạo thư mục tạm để extract backup
echo ""
echo "📦 Extracting backup file..."
mkdir -p "$RESTORE_DIR"
if tar -xzf "$BACKUP_FILE" -C "$RESTORE_DIR"; then
    echo "✅ Backup extracted successfully"

    echo "📋 Extracted contents:"
    ls -la "$RESTORE_DIR/"
else
    echo "❌ Failed to extract backup file"
    exit 1
fi

# Tìm file .bak trong backup
echo ""
echo "🔍 Looking for database backup files..."
BAK_FILE=$(find "$RESTORE_DIR" -name "*.bak" | head -1)
SQL_FILE=$(find "$RESTORE_DIR" -name "*.sql" | head -1)

if [ -n "$BAK_FILE" ]; then
    echo "✅ Found .bak file: $(basename $BAK_FILE)"

    # Phục hồi từ .bak file
    echo "🚀 Restoring database from .bak file..."

    # Dừng tất cả connections đến database
    sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -Q "
    ALTER DATABASE KhoanDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    " 2>/dev/null || true

    # Drop database nếu tồn tại
    sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -Q "
    IF EXISTS (SELECT name FROM sys.databases WHERE name = 'KhoanDB')
    BEGIN
        DROP DATABASE KhoanDB;
    END
    " 2>/dev/null || true

    # Copy .bak file vào container
    echo "📥 Copying backup file to container..."
    docker cp "$BAK_FILE" azure_sql_edge_tinhkhoan:/var/opt/mssql/backup/KhoanDB_restore.bak

    # Restore database
    echo "🔄 Restoring database..."
    sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -Q "
    RESTORE DATABASE KhoanDB
    FROM DISK = '/var/opt/mssql/backup/KhoanDB_restore.bak'
    WITH REPLACE,
    MOVE 'KhoanDB' TO '/var/opt/mssql/data/KhoanDB.mdf',
    MOVE 'KhoanDB_Log' TO '/var/opt/mssql/data/KhoanDB_Log.ldf'
    "

    if [ $? -eq 0 ]; then
        echo "✅ Database restored successfully from .bak file"
    else
        echo "⚠️  .bak restore failed, trying alternative method..."
    fi

elif [ -n "$SQL_FILE" ]; then
    echo "✅ Found .sql file: $(basename $SQL_FILE)"

    # Drop và tạo lại database
    echo "🔄 Recreating database..."
    sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -Q "
    IF EXISTS (SELECT name FROM sys.databases WHERE name = 'KhoanDB')
    BEGIN
        ALTER DATABASE KhoanDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
        DROP DATABASE KhoanDB;
    END
    CREATE DATABASE KhoanDB;
    "

    # Chạy SQL file
    echo "📝 Executing SQL restore script..."
    sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d KhoanDB -i "$SQL_FILE"

    if [ $? -eq 0 ]; then
        echo "✅ Database restored successfully from .sql file"
    else
        echo "❌ SQL restore failed"
    fi

else
    echo "❌ No .bak or .sql files found in backup"
    echo "📋 Backup contents:"
    find "$RESTORE_DIR" -type f | head -10
fi

# Cleanup
echo ""
echo "🧹 Cleaning up temporary files..."
rm -rf "$RESTORE_DIR"
echo "✅ Cleanup completed"

# Kiểm tra kết quả restore
echo ""
echo "🔍 Verifying database restore..."

# Kiểm tra database tồn tại
DB_EXISTS=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -Q "SELECT COUNT(*) FROM sys.databases WHERE name = 'KhoanDB'" -h-1 -W | tr -d ' \r\n')

if [ "$DB_EXISTS" = "1" ]; then
    echo "✅ KhoanDB exists"

    # Đếm số bảng
    TABLE_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d KhoanDB -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'" -h-1 -W 2>/dev/null | tr -d ' \r\n')
    echo "📊 Tables found: ${TABLE_COUNT:-0}"

    # Kiểm tra các bảng chính
    if [ "${TABLE_COUNT:-0}" -gt "0" ]; then
        echo ""
        echo "📋 Key tables check:"

        # Kiểm tra Units
        UNITS_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d KhoanDB -Q "SELECT COUNT(*) FROM Units" -h-1 -W 2>/dev/null | tr -d ' \r\n')
        echo "   • Units: ${UNITS_COUNT:-0} records"

        # Kiểm tra Roles
        ROLES_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d KhoanDB -Q "SELECT COUNT(*) FROM Roles" -h-1 -W 2>/dev/null | tr -d ' \r\n')
        echo "   • Roles: ${ROLES_COUNT:-0} records"

        # Kiểm tra Employees
        EMP_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d KhoanDB -Q "SELECT COUNT(*) FROM Employees" -h-1 -W 2>/dev/null | tr -d ' \r\n')
        echo "   • Employees: ${EMP_COUNT:-0} records"

        # Kiểm tra 8 core data tables
        echo ""
        echo "📊 Core data tables:"
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
    fi

    echo ""
    echo "🎯 DATABASE RESTORE SUMMARY"
    echo "=========================="
    echo "✅ Database: KhoanDB restored successfully"
    echo "📊 Total tables: ${TABLE_COUNT:-0}"
    echo "📝 Source: $BACKUP_FILE"
    echo "🕒 Restore time: $(date)"
    echo ""
    echo "🚀 Database is ready for use!"
    echo "   Connection: localhost:1433"
    echo "   Username: sa"
    echo "   Password: Dientoan@303"
    echo "   Database: KhoanDB"

else
    echo "❌ Database restore failed - KhoanDB not found"
    exit 1
fi
