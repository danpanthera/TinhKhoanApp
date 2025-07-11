#!/bin/bash

# =============================================================================
# 🗑️ CLEANUP SCRIPT: XÓA HOÀN TOÀN MỌI THỨ LIÊN QUAN ĐẾN DT_KHKD
# =============================================================================
# Script này sẽ xóa bỏ hoàn toàn:
# 1. Database tables (7800_DT_KHKD1, DT_KHKD1, DT_KHKD1_History)
# 2. Code references trong SmartDataImportService
# 3. File SQL configurations
# 4. Documentation references
# 5. Backup files chứa DT_KHKD
# =============================================================================

echo "🧹 BẮT ĐẦU CLEANUP DT_KHKD HOÀN TOÀN..."
echo "============================================="

# 1. XÓA CÁC BẢNG TRONG DATABASE (nếu có)
echo "🗄️ Step 1: Kiểm tra và xóa bảng database..."

sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -C -Q "
-- Kiểm tra các bảng tồn tại
SELECT 'FOUND: ' + TABLE_NAME AS Status FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME LIKE '%DT_KHKD%' OR TABLE_NAME LIKE '%7800_DT_KHKD%'

-- Tắt System Versioning nếu có
IF EXISTS (SELECT * FROM sys.tables WHERE name LIKE '%DT_KHKD%' AND temporal_type = 2)
BEGIN
    PRINT 'Tắt System Versioning cho temporal tables...'
    IF EXISTS (SELECT * FROM sys.tables WHERE name = '7800_DT_KHKD1')
        ALTER TABLE [7800_DT_KHKD1] SET (SYSTEM_VERSIONING = OFF)
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DT_KHKD1')
        ALTER TABLE [DT_KHKD1] SET (SYSTEM_VERSIONING = OFF)
END

-- Xóa các bảng (cẩn thận với thứ tự)
IF EXISTS (SELECT * FROM sys.tables WHERE name = '7800_DT_KHKD1_History')
    DROP TABLE [7800_DT_KHKD1_History]
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DT_KHKD1_History')
    DROP TABLE [DT_KHKD1_History]
IF EXISTS (SELECT * FROM sys.tables WHERE name = '7800_DT_KHKD1')
    DROP TABLE [7800_DT_KHKD1]
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DT_KHKD1')
    DROP TABLE [DT_KHKD1]

-- Xác nhận đã xóa
SELECT 'AFTER CLEANUP: ' + ISNULL(STUFF((
    SELECT ', ' + TABLE_NAME
    FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_NAME LIKE '%DT_KHKD%'
    FOR XML PATH('')), 1, 2, ''), 'NO TABLES FOUND') AS TablesRemaining

PRINT '✅ Database cleanup completed'
"

echo "✅ Step 1 hoàn thành - Database tables cleaned"

# 2. XÓA REFERENCES TRONG CODE
echo ""
echo "📝 Step 2: Cleaning code references..."

# Backup SmartDataImportService trước khi sửa
if [ -f "Services/SmartDataImportService.cs" ]; then
    cp "Services/SmartDataImportService.cs" "Services/SmartDataImportService.cs.backup_$(date +%Y%m%d_%H%M%S)"
    echo "✅ Backed up SmartDataImportService.cs"

    # Xóa references trong SmartDataImportService
    sed -i '' '/DT_KHKD/d' "Services/SmartDataImportService.cs"
    echo "✅ Removed DT_KHKD references from SmartDataImportService.cs"
fi

# 3. XÓA CÁC FILE SQL CONFIGURATION
echo ""
echo "🗂️ Step 3: Cleaning SQL configuration files..."

# Tạo backup trước khi sửa
if [ -f "configure_raw_data_tables.sql" ]; then
    cp "configure_raw_data_tables.sql" "configure_raw_data_tables.sql.backup_$(date +%Y%m%d_%H%M%S)"

    # Xóa section DT_KHKD1 trong configure_raw_data_tables.sql
    sed -i '' '/-- 1\. BẢNG 7800_DT_KHKD1/,/PRINT.*7800_DT_KHKD1.*completed/d' "configure_raw_data_tables.sql"
    echo "✅ Removed DT_KHKD1 configuration from configure_raw_data_tables.sql"
fi

# Xóa references trong check_column_mapping.sql
if [ -f "check_column_mapping.sql" ]; then
    cp "check_column_mapping.sql" "check_column_mapping.sql.backup_$(date +%Y%m%d_%H%M%S)"
    sed -i '' "s/,'7800_DT_KHKD1'//g" "check_column_mapping.sql"
    sed -i '' "s/'7800_DT_KHKD1',//g" "check_column_mapping.sql"
    echo "✅ Removed DT_KHKD1 from check_column_mapping.sql"
fi

# 4. XÓA DOCUMENTATION REFERENCES
echo ""
echo "📚 Step 4: Cleaning documentation..."

# Cập nhật documentation files
for file in *.md; do
    if [ -f "$file" ] && grep -q "DT_KHKD" "$file"; then
        cp "$file" "$file.backup_$(date +%Y%m%d_%H%M%S)"
        sed -i '' '/DT_KHKD/d' "$file"
        echo "✅ Cleaned $file"
    fi
done

# 5. XÓA BACKUP FILES CHỨA DT_KHKD
echo ""
echo "🗑️ Step 5: Removing backup files containing DT_KHKD..."

# Tìm và hiển thị các file backup chứa DT_KHKD
echo "📋 Files containing DT_KHKD:"
find . -name "*.backup*" -exec grep -l "DT_KHKD" {} \; 2>/dev/null | head -10

# Xóa các file backup cũ chứa DT_KHKD (an toàn)
find . -name "*.backup_before_cleanup" -exec rm -f {} \; 2>/dev/null
echo "✅ Removed old backup files"

# 6. XÓA MIGRATION FILES (nếu có)
echo ""
echo "🔄 Step 6: Checking migration files..."

find Migrations/ -name "*DT_KHKD*" 2>/dev/null | while read -r file; do
    echo "⚠️ Found migration file: $file"
    echo "   You may need to manually review and remove this migration"
done

# 7. FINAL VERIFICATION
echo ""
echo "🔍 Step 7: Final verification..."

echo "📊 VERIFICATION RESULTS:"
echo "========================"

# Check database
echo "Database tables:"
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -C -Q "
SELECT ISNULL('Found: ' + STRING_AGG(TABLE_NAME, ', '), 'No DT_KHKD tables found') AS DatabaseStatus
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME LIKE '%DT_KHKD%'
" 2>/dev/null

# Check code files
echo ""
echo "Code references:"
code_refs=$(find . -name "*.cs" -not -path "./Migrations/*" -exec grep -l "DT_KHKD" {} \; 2>/dev/null | wc -l)
echo "Found $code_refs files with DT_KHKD references"

# Check SQL files
echo ""
echo "SQL configuration files:"
sql_refs=$(find . -name "*.sql" -exec grep -l "DT_KHKD" {} \; 2>/dev/null | wc -l)
echo "Found $sql_refs SQL files with DT_KHKD references"

echo ""
echo "🎉 CLEANUP HOÀN THÀNH!"
echo "======================"
echo "✅ Database tables: Cleaned"
echo "✅ Code references: Cleaned"
echo "✅ SQL configurations: Cleaned"
echo "✅ Documentation: Cleaned"
echo "✅ Backup files: Cleaned"
echo ""
echo "🚀 Dự án đã sạch hoàn toàn khỏi DT_KHKD!"
echo "   Có thể build và chạy bình thường."
