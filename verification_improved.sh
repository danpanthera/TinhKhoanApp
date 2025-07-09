#!/bin/bash

# 🎯 SCRIPT VERIFICATION IMPROVED - Kiểm tra tổng thể dự án với logic chính xác
# Ngày: $(date '+%d/%m/%Y %H:%M:%S')

echo "🔍 ===== RÀ SOÁT TỔNG THỂ DỰ ÁN TINHKHOANAPP (IMPROVED) ====="
echo "📅 Thời gian: $(date '+%d/%m/%Y %H:%M:%S')"
echo ""

# Initialize counters
TOTAL_CHECKS=10
PASSED_CHECKS=0
CHECK_RESULTS=()

echo "1️⃣ KIỂM TRA KHO DỮ LIỆU THÔ - DIRECT IMPORT"
echo "============================================="

# Check frontend rawDataService uses DirectImport
if grep -q "DirectImport/smart" Frontend/tinhkhoan-app-ui-vite/src/services/rawDataService.js; then
    echo "✅ rawDataService.js sử dụng DirectImport/smart API"
    ((PASSED_CHECKS++))
    CHECK_RESULTS+=("✅ rawDataService DirectImport")
else
    echo "❌ rawDataService.js chưa sử dụng DirectImport API"
    CHECK_RESULTS+=("❌ rawDataService DirectImport")
fi

# Check smartImportService uses DirectImport  
if grep -q "DirectImport/smart" Frontend/tinhkhoan-app-ui-vite/src/services/smartImportService.js; then
    echo "✅ smartImportService.js sử dụng DirectImport/smart API"
    ((PASSED_CHECKS++))
    CHECK_RESULTS+=("✅ smartImportService DirectImport")
else
    echo "❌ smartImportService.js chưa sử dụng DirectImport API"
    CHECK_RESULTS+=("❌ smartImportService DirectImport")
fi

# Check DataImportViewFull uses both services (improved logic)
RAWDATA_IMPORT=$(grep -c "rawDataService" Frontend/tinhkhoan-app-ui-vite/src/views/DataImportViewFull.vue)
SMART_IMPORT=$(grep -c "smartImportService" Frontend/tinhkhoan-app-ui-vite/src/views/DataImportViewFull.vue)

if [[ $RAWDATA_IMPORT -gt 0 ]] && [[ $SMART_IMPORT -gt 0 ]]; then
    echo "✅ DataImportViewFull.vue sử dụng cả rawDataService và smartImportService"
    ((PASSED_CHECKS++))
    CHECK_RESULTS+=("✅ DataImportViewFull services")
else
    echo "❌ DataImportViewFull.vue chưa import đúng services"
    CHECK_RESULTS+=("❌ DataImportViewFull services")
fi

echo ""

echo "2️⃣ KIỂM TRA BACKEND API DIRECTIMPORT"
echo "===================================="

# Check DirectImport API health
DIRECTIMPORT_STATUS=$(curl -s http://localhost:5055/api/DirectImport/status 2>/dev/null)
if [[ $DIRECTIMPORT_STATUS == *"Direct Import System Online"* ]]; then
    echo "✅ DirectImport API đang online"
    
    # Extract supported data types count (improved)
    DATA_TYPES_COUNT=$(echo "$DIRECTIMPORT_STATUS" | grep -o '"DP01\|LN01\|DB01\|GL01\|GL41\|DPDA\|EI01\|KH03\|RR01\|DT_KHKD1"' | wc -l | tr -d ' ')
    echo "✅ Hỗ trợ $DATA_TYPES_COUNT loại dữ liệu"
    
    if [[ $DATA_TYPES_COUNT -ge 9 ]]; then
        ((PASSED_CHECKS++))
        CHECK_RESULTS+=("✅ DirectImport API Online")
    else
        CHECK_RESULTS+=("⚠️ DirectImport API - ít data types")
    fi
else
    echo "❌ DirectImport API không phản hồi hoặc offline"
    CHECK_RESULTS+=("❌ DirectImport API Offline")
fi

echo ""

echo "3️⃣ KIỂM TRA DATABASE - TEMPORAL TABLES"
echo "======================================="

# Check if we can connect to database (improved logic)
if command -v sqlcmd &> /dev/null; then
    # Count temporal tables (improved with better parsing)
    TEMPORAL_RAW=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -C -h -1 -Q "SELECT COUNT(*) FROM sys.tables WHERE temporal_type = 2;" 2>/dev/null)
    TEMPORAL_COUNT=$(echo "$TEMPORAL_RAW" | grep -E '^[[:space:]]*[0-9]+[[:space:]]*$' | tr -d ' \t' | head -1)
    
    if [[ $TEMPORAL_COUNT =~ ^[0-9]+$ ]] && [[ $TEMPORAL_COUNT -gt 0 ]]; then
        echo "✅ Database có $TEMPORAL_COUNT bảng Temporal Tables"
        
        # Check specific raw data tables (improved)
        RAW_TABLES=("DP01_New" "LN01" "DB01" "GL01" "GL41" "DPDA" "EI01" "KH03" "LN02" "LN03" "RR01" "7800_DT_KHKD1")
        FOUND_TABLES=0
        
        for table in "${RAW_TABLES[@]}"; do
            TABLE_RAW=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -C -h -1 -Q "SELECT COUNT(*) FROM sys.tables WHERE name = '$table' AND temporal_type = 2;" 2>/dev/null)
            TABLE_EXISTS=$(echo "$TABLE_RAW" | grep -E '^[[:space:]]*[0-9]+[[:space:]]*$' | tr -d ' \t' | head -1)
            if [[ $TABLE_EXISTS == "1" ]]; then
                ((FOUND_TABLES++))
            fi
        done
        
        echo "✅ $FOUND_TABLES/12 bảng dữ liệu thô là Temporal Tables"
        
        # If we have 10+ temporal tables, consider it passed
        if [[ $TEMPORAL_COUNT -ge 10 ]]; then
            ((PASSED_CHECKS++))
            CHECK_RESULTS+=("✅ Temporal Tables ($TEMPORAL_COUNT)")
        else
            CHECK_RESULTS+=("⚠️ Temporal Tables ($TEMPORAL_COUNT < 10)")
        fi
    else
        echo "❌ Không thể kiểm tra Temporal Tables hoặc không có bảng nào"
        CHECK_RESULTS+=("❌ Temporal Tables (không kết nối được)")
    fi
else
    echo "⚠️ sqlcmd không khả dụng - không thể kiểm tra database"
    CHECK_RESULTS+=("⚠️ Temporal Tables (không có sqlcmd)")
fi

echo ""

echo "4️⃣ KIỂM TRA COLUMNSTORE INDEXES"
echo "==============================="

if command -v sqlcmd &> /dev/null; then
    # Count columnstore indexes (improved with better parsing)
    COLUMNSTORE_RAW=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -C -h -1 -Q "SELECT COUNT(*) FROM sys.indexes WHERE type_desc LIKE '%COLUMNSTORE%';" 2>/dev/null)
    COLUMNSTORE_COUNT=$(echo "$COLUMNSTORE_RAW" | grep -E '^[[:space:]]*[0-9]+[[:space:]]*$' | tr -d ' \t' | head -1)
    
    if [[ $COLUMNSTORE_COUNT =~ ^[0-9]+$ ]] && [[ $COLUMNSTORE_COUNT -gt 0 ]]; then
        echo "✅ Database có $COLUMNSTORE_COUNT Columnstore Indexes"
        
        # Check which tables have columnstore indexes (improved with better parsing)
        TABLES_RAW=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -C -h -1 -Q "SELECT COUNT(DISTINCT t.name) FROM sys.indexes i INNER JOIN sys.tables t ON i.object_id = t.object_id WHERE i.type_desc LIKE '%COLUMNSTORE%' AND t.name IN ('DP01_New','LN01','DB01','GL01','GL41','DPDA','EI01','KH03','LN02','LN03','RR01','7800_DT_KHKD1');" 2>/dev/null)
        TABLES_WITH_COLUMNSTORE=$(echo "$TABLES_RAW" | grep -E '^[[:space:]]*[0-9]+[[:space:]]*$' | tr -d ' \t' | head -1)
        
        echo "✅ $TABLES_WITH_COLUMNSTORE/12 bảng dữ liệu thô có Columnstore Indexes"
        
        # If we have 10+ columnstore indexes, consider it passed
        if [[ $COLUMNSTORE_COUNT -ge 10 ]]; then
            ((PASSED_CHECKS++))
            CHECK_RESULTS+=("✅ Columnstore Indexes ($COLUMNSTORE_COUNT)")
        else
            CHECK_RESULTS+=("⚠️ Columnstore Indexes ($COLUMNSTORE_COUNT < 10)")
        fi
    else
        echo "❌ Không có Columnstore Indexes hoặc không thể kiểm tra"
        CHECK_RESULTS+=("❌ Columnstore Indexes (không có)")
    fi
fi

echo ""

echo "5️⃣ KIỂM TRA LEGACY CLEANUP" 
echo "=========================="

# Check if ImportedDataItems is removed from models (improved)
IMPORTEDDATAITEMS_USAGE=$(grep -v "// .*CLEANED.*ImportedDataItems" Backend/TinhKhoanApp.Api/Models/ImportedDataRecord.cs | grep -c "ImportedDataItems" 2>/dev/null)
if [[ -z "$IMPORTEDDATAITEMS_USAGE" ]]; then
    IMPORTEDDATAITEMS_USAGE=0
fi

if [[ $IMPORTEDDATAITEMS_USAGE -eq 0 ]]; then
    echo "✅ ImportedDataItems navigation property đã được xóa khỏi models"
    ((PASSED_CHECKS++))
    CHECK_RESULTS+=("✅ ImportedDataItems cleanup")
else
    echo "❌ ImportedDataItems navigation property vẫn còn trong models ($IMPORTEDDATAITEMS_USAGE usages)"
    CHECK_RESULTS+=("❌ ImportedDataItems vẫn còn ($IMPORTEDDATAITEMS_USAGE)")
fi

# Check if legacy controllers are disabled (improved)
DISABLED_CONTROLLERS=$(find Backend/TinhKhoanApp.Api/Controllers -name "*.disabled" 2>/dev/null | wc -l | tr -d ' ')
if [[ $DISABLED_CONTROLLERS -gt 0 ]]; then
    echo "✅ $DISABLED_CONTROLLERS legacy controllers đã được disabled"
    ((PASSED_CHECKS++))
    CHECK_RESULTS+=("✅ Legacy controllers disabled ($DISABLED_CONTROLLERS)")
else
    echo "⚠️ Không có legacy controllers nào được disabled"
    CHECK_RESULTS+=("⚠️ Legacy controllers chưa disabled")
fi

# Check if migration exists (improved)
if ls Backend/TinhKhoanApp.Api/Migrations/*DropImportedDataItemsTable* 1> /dev/null 2>&1; then
    echo "✅ Migration xóa ImportedDataItems table đã tồn tại"
    ((PASSED_CHECKS++))
    CHECK_RESULTS+=("✅ Migration exists")
else
    echo "❌ Migration xóa ImportedDataItems table chưa có"
    CHECK_RESULTS+=("❌ Migration missing")
fi

echo ""

echo "6️⃣ KIỂM TRA BACKEND BUILD STATUS"
echo "================================"

cd Backend/TinhKhoanApp.Api
BUILD_RESULT=$(dotnet build --verbosity quiet 2>&1)
BUILD_EXIT_CODE=$?

if [[ $BUILD_EXIT_CODE -eq 0 ]]; then
    echo "✅ Backend build thành công"
    ((PASSED_CHECKS++))
    CHECK_RESULTS+=("✅ Backend build success")
    
    # Count warnings (fixed syntax error)
    WARNING_COUNT=0
    if echo "$BUILD_RESULT" | grep -q "warning"; then
        WARNING_COUNT=$(echo "$BUILD_RESULT" | grep -c "warning")
    fi
    
    if [[ $WARNING_COUNT -gt 0 ]]; then
        echo "⚠️ Build có $WARNING_COUNT warnings"
    else
        echo "✅ Build không có warnings"
    fi
else
    echo "❌ Backend build thất bại"
    echo "Lỗi build: $BUILD_RESULT"
    CHECK_RESULTS+=("❌ Backend build failed")
fi

cd - > /dev/null

echo ""

echo "7️⃣ TÓM TẮT ĐÁNH GIÁ"
echo "==================="

PASS_PERCENTAGE=$((PASSED_CHECKS * 100 / TOTAL_CHECKS))

echo "📊 Kết quả tổng thể: $PASSED_CHECKS/$TOTAL_CHECKS checks passed ($PASS_PERCENTAGE%)"
echo ""
echo "📋 Chi tiết kết quả:"
for result in "${CHECK_RESULTS[@]}"; do
    echo "   $result"
done
echo ""

if [[ $PASS_PERCENTAGE -ge 90 ]]; then
    echo "🎉 TUYỆT VỜI! Dự án đã hoàn thiện và sẵn sàng production"
elif [[ $PASS_PERCENTAGE -ge 70 ]]; then
    echo "✅ TỐT! Dự án cơ bản hoàn thiện, có thể cần điều chỉnh nhỏ"
elif [[ $PASS_PERCENTAGE -ge 50 ]]; then
    echo "⚠️ TRUNG BÌNH! Cần khắc phục một số vấn đề"
else
    echo "❌ CẦN KHẮC PHỤC! Nhiều vấn đề cần được giải quyết"
fi

echo ""
echo "🛠️ Gợi ý khắc phục:"
if [[ $PASS_PERCENTAGE -lt 100 ]]; then
    echo "   • Kiểm tra kết nối database nếu có lỗi sqlcmd"
    echo "   • Đảm bảo backend đang chạy để test API"
    echo "   • Logic script có thể cần tinh chỉnh cho edge cases"
fi

echo ""
echo "📝 Báo cáo này được tạo bởi script tự động verification (improved)"
echo "📅 $(date '+%d/%m/%Y %H:%M:%S')"
echo "==========================================="
