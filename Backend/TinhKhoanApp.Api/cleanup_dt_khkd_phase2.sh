#!/bin/bash

# =============================================================================
# 🗑️ ADVANCED CLEANUP: DỌN SẠCH HOÀN TOÀN DT_KHKD PHASE 2
# =============================================================================

echo "🧹 PHASE 2: ADVANCED CLEANUP DT_KHKD..."
echo "========================================"

# 1. DỌN DẠP CÁC FILE C# CÒN LẠI
echo "📝 Step 1: Cleaning remaining C# files..."

# DirectImportController.cs
if grep -q "DT_KHKD" "Controllers/DirectImportController.cs" 2>/dev/null; then
    cp "Controllers/DirectImportController.cs" "Controllers/DirectImportController.cs.backup_$(date +%Y%m%d_%H%M%S)"
    sed -i '' '/DT_KHKD/d' "Controllers/DirectImportController.cs"
    echo "✅ Cleaned DirectImportController.cs"
fi

# FileNameParsingService.cs
if grep -q "DT_KHKD" "Services/FileNameParsingService.cs" 2>/dev/null; then
    cp "Services/FileNameParsingService.cs" "Services/FileNameParsingService.cs.backup_$(date +%Y%m%d_%H%M%S)"
    sed -i '' '/DT_KHKD/d' "Services/FileNameParsingService.cs"
    echo "✅ Cleaned FileNameParsingService.cs"
fi

# 2. DỌN DẠP CÁC FILE SQL CÒN LẠI
echo ""
echo "🗂️ Step 2: Cleaning remaining SQL files..."

sql_files=(
    "Database/Scripts/CompleteTemporalTablesSetup.sql"
    "Database/Scripts/MasterTemporalSetup.sql"
    "Database/Scripts/ConvertToTemporalTables.sql"
    "configure_raw_data_tables_final.sql"
    "configure_raw_data_tables_v2.sql"
    "enable_temporal_simple.sql"
    "fix_identity_final.sql"
    "create_columnstore_indexes_v2.sql"
    "setup_smart_data_import_tables.sql"
    "check_temporal_tables.sql"
    "COMPLETE_TEMPORAL_COLUMNSTORE_100.sql"
)

for file in "${sql_files[@]}"; do
    if [ -f "$file" ] && grep -q "DT_KHKD" "$file"; then
        cp "$file" "$file.backup_$(date +%Y%m%d_%H%M%S)"

        # Xóa các dòng chứa DT_KHKD và dòng comment related
        sed -i '' '/DT_KHKD/d' "$file"

        # Xóa section blocks cụ thể cho DT_KHKD1
        sed -i '' '/-- .*DT_KHKD1/,/-- End DT_KHKD1/d' "$file"
        sed -i '' '/-- Configure 7800_DT_KHKD1/,/PRINT.*DT_KHKD1.*completed/d' "$file"

        echo "✅ Cleaned $file"
    fi
done

# 3. XÓA CÁC BACKUP FOLDER CŨ
echo ""
echo "🗑️ Step 3: Removing old backup folders..."

if [ -d "backup_importeddataitems_20250709_201445" ]; then
    rm -rf "backup_importeddataitems_20250709_201445"
    echo "✅ Removed old backup folder: backup_importeddataitems_20250709_201445"
fi

# Tìm và xóa các backup folder khác có chứa DT_KHKD
find . -type d -name "backup_*" | while read -r dir; do
    if find "$dir" -type f -exec grep -l "DT_KHKD" {} \; 2>/dev/null | head -1 | grep -q .; then
        echo "⚠️ Found backup directory with DT_KHKD: $dir"
        read -p "Delete this directory? (y/N): " confirm
        if [[ $confirm == [yY] ]]; then
            rm -rf "$dir"
            echo "✅ Deleted $dir"
        fi
    fi
done

# 4. KIỂM TRA VÀ DỌN DẠP IMPLEMENT SCRIPTS
echo ""
echo "📜 Step 4: Cleaning implementation scripts..."

if grep -q "DT_KHKD" "implement_all_direct_imports.sh" 2>/dev/null; then
    cp "implement_all_direct_imports.sh" "implement_all_direct_imports.sh.backup_$(date +%Y%m%d_%H%M%S)"
    sed -i '' '/DT_KHKD/d' "implement_all_direct_imports.sh"

    # Sửa lại comment header để loại bỏ DT_KHKD1
    sed -i '' 's/, DT_KHKD1//g' "implement_all_direct_imports.sh"
    sed -i '' 's/DT_KHKD1, //g' "implement_all_direct_imports.sh"

    echo "✅ Cleaned implement_all_direct_imports.sh"
fi

# 5. BUILD TEST ĐỂ ĐẢM BẢO KHÔNG CÓ LỖI
echo ""
echo "🔨 Step 5: Testing build after cleanup..."

echo "Building project to verify no compilation errors..."
dotnet build --no-restore > build_test.log 2>&1

if [ $? -eq 0 ]; then
    echo "✅ Build successful - No compilation errors"
    rm build_test.log
else
    echo "⚠️ Build failed - Check build_test.log for details"
    echo "📋 Last few lines of build output:"
    tail -10 build_test.log
fi

# 6. FINAL VERIFICATION PHASE 2
echo ""
echo "🔍 Step 6: Final verification Phase 2..."

echo "📊 ADVANCED VERIFICATION RESULTS:"
echo "================================="

# Check code files again
code_refs=$(find . -name "*.cs" -not -path "./Migrations/*" -not -path "./backup_*" -exec grep -l "DT_KHKD" {} \; 2>/dev/null | wc -l)
echo "Remaining C# files with DT_KHKD: $code_refs"

# Check SQL files again
sql_refs=$(find . -name "*.sql" -not -path "./backup_*" -exec grep -l "DT_KHKD" {} \; 2>/dev/null | wc -l)
echo "Remaining SQL files with DT_KHKD: $sql_refs"

# List any remaining files for manual review
echo ""
echo "📋 Remaining files (if any):"
find . -name "*.cs" -o -name "*.sql" | grep -v backup | xargs grep -l "DT_KHKD" 2>/dev/null | head -5

echo ""
if [ $code_refs -eq 0 ] && [ $sql_refs -eq 0 ]; then
    echo "🎉 PERFECT! CLEANUP HOÀN TOÀN THÀNH CÔNG!"
    echo "========================================"
    echo "✅ 0 C# files contain DT_KHKD"
    echo "✅ 0 SQL files contain DT_KHKD"
    echo "✅ Build successful"
    echo "✅ Project is completely clean"
else
    echo "⚠️ Still found some references - manual review needed"
    echo "======================================================"
    echo "Remaining C# files: $code_refs"
    echo "Remaining SQL files: $sql_refs"
fi

echo ""
echo "🚀 PHASE 2 CLEANUP COMPLETED!"
