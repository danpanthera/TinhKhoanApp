#!/bin/bash

# Script cleanup legacy ImportedDataItems code
# Tạo backup và dọn dẹp từng bước

echo "🧹 CLEANUP LEGACY IMPORTEDDATAITEMS CODE"
echo "========================================"

# Backup trước khi cleanup
BACKUP_DIR="backup_importeddataitems_$(date +%Y%m%d_%H%M%S)"
mkdir -p "$BACKUP_DIR"

echo "📋 Đang tạo backup..."

# Files cần cleanup (top priority)
LEGACY_FILES=(
    "Controllers/LN01Controller.cs"
    "Controllers/NguonVonCalculationController.cs"
    "Controllers/BranchIndicatorsController.cs"
    "Controllers/RawDataController.cs"
    "Services/DashboardCalculationService.cs"
    "Services/BranchCalculationService.cs"
    "Services/RawDataProcessingService.cs"
    "Services/RawDataService.cs"
    "Services/SmartDataImportService.cs"
)

# Backup files
for file in "${LEGACY_FILES[@]}"; do
    if [ -f "$file" ]; then
        echo "  - $file"
        cp "$file" "$BACKUP_DIR/"
    fi
done

echo "✅ Backup hoàn thành: $BACKUP_DIR"
echo ""

echo "📊 Thống kê sử dụng ImportedDataItems:"
echo "======================================="

# Count ImportedDataItems usage
for file in "${LEGACY_FILES[@]}"; do
    if [ -f "$file" ]; then
        count=$(grep -c "ImportedDataItems" "$file" 2>/dev/null || echo "0")
        if [ "$count" -gt 0 ]; then
            echo "  $file: $count occurrences"
            grep -n "ImportedDataItems" "$file" | head -3 | sed 's/^/    /'
            echo ""
        fi
    fi
done

echo "🎯 CÁC BƯỚC CLEANUP TIẾP THEO:"
echo "=========================="
echo "1. Sửa DashboardCalculationService - thay ImportedDataItems bằng direct table queries"
echo "2. Sửa BranchIndicatorsController - thay ImportedDataItems bằng direct table queries"
echo "3. Sửa NguonVonCalculationController - thay ImportedDataItems bằng direct table queries"
echo "4. Sửa LN01Controller - thay ImportedDataItems bằng direct table queries"
echo "5. Disable RawDataProcessingService - thay bằng DirectImportService"
echo "6. Update SmartDataImportService - chuyển sang DirectImportService"
echo "7. Xóa ImportedDataItem model và references"
echo ""

echo "💡 KHUYẾN NGHỊ:"
echo "=============="
echo "- Test từng service sau khi cleanup"
echo "- Chỉ cleanup khi Direct Import đã implement đầy đủ"
echo "- Giữ backup để rollback nếu cần"
echo ""
