#!/bin/bash

# 🎯 SCRIPT VERIFICATION - Kiểm tra tổng thể dự án sau rà soát
# Ngày: $(date '+%d/%m/%Y %H:%M:%S')

echo "🔍 ===== RÀ SOÁT TỔNG THỂ DỰ ÁN TINHKHOANAPP ====="
echo "📅 Thời gian: $(date '+%d/%m/%Y %H:%M:%S')"
echo ""

echo "1️⃣ KIỂM TRA KHO DỮ LIỆU THÔ - DIRECT IMPORT"
echo "============================================="

# Check frontend rawDataService uses DirectImport
if grep -q "DirectImport/smart" Frontend/tinhkhoan-app-ui-vite/src/services/rawDataService.js; then
    echo "✅ rawDataService.js sử dụng DirectImport/smart API"
else
    echo "❌ rawDataService.js chưa sử dụng DirectImport API"
fi

# Check smartImportService uses DirectImport  
if grep -q "DirectImport/smart" Frontend/tinhkhoan-app-ui-vite/src/services/smartImportService.js; then
    echo "✅ smartImportService.js sử dụng DirectImport/smart API"
else
    echo "❌ smartImportService.js chưa sử dụng DirectImport API"
fi

# Check DataImportViewFull uses both services
if grep -q "rawDataService.*smartImportService" Frontend/tinhkhoan-app-ui-vite/src/views/DataImportViewFull.vue; then
    echo "✅ DataImportViewFull.vue sử dụng cả rawDataService và smartImportService"
else
    echo "❌ DataImportViewFull.vue chưa import đúng services"
fi

echo ""

echo "2️⃣ KIỂM TRA BACKEND API DIRECTIMPORT"
echo "===================================="

# Check DirectImport API health
DIRECTIMPORT_STATUS=$(curl -s http://localhost:5055/api/DirectImport/status 2>/dev/null)
if [[ $DIRECTIMPORT_STATUS == *"Direct Import System Online"* ]]; then
    echo "✅ DirectImport API đang online"
    # Extract supported data types count
    DATA_TYPES_COUNT=$(echo $DIRECTIMPORT_STATUS | grep -o '"DP01\|LN01\|DB01\|GL01\|GL41\|DPDA\|EI01\|KH03\|RR01\|DT_KHKD1"' | wc -l)
    echo "✅ Hỗ trợ $DATA_TYPES_COUNT loại dữ liệu"
else
    echo "❌ DirectImport API không phản hồi hoặc offline"
fi

echo ""

echo "3️⃣ KIỂM TRA DATABASE - TEMPORAL TABLES"
echo "======================================="

# Check if we can connect to database
if command -v sqlcmd &> /dev/null; then
    # Count temporal tables
    TEMPORAL_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -C -h -1 -Q "SELECT COUNT(*) FROM sys.tables WHERE temporal_type = 2;" 2>/dev/null | grep -E '^[0-9]+$' | head -1)
    
    if [[ $TEMPORAL_COUNT =~ ^[0-9]+$ ]] && [[ $TEMPORAL_COUNT -gt 0 ]]; then
        echo "✅ Database có $TEMPORAL_COUNT bảng Temporal Tables"
        
        # Check specific raw data tables
        RAW_TABLES=("DP01_New" "LN01" "DB01" "GL01" "GL41" "DPDA" "EI01" "KH03" "LN02" "LN03" "RR01" "7800_DT_KHKD1")
        FOUND_TABLES=0
        
        for table in "${RAW_TABLES[@]}"; do
            TABLE_EXISTS=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -C -h -1 -Q "SELECT COUNT(*) FROM sys.tables WHERE name = '$table' AND temporal_type = 2;" 2>/dev/null | grep -E '^[0-9]+$' | head -1)
            if [[ $TABLE_EXISTS == "1" ]]; then
                ((FOUND_TABLES++))
            fi
        done
        
        echo "✅ $FOUND_TABLES/12 bảng dữ liệu thô là Temporal Tables"
    else
        echo "❌ Không thể kiểm tra Temporal Tables hoặc không có bảng nào"
    fi
else
    echo "⚠️ sqlcmd không khả dụng - không thể kiểm tra database"
fi

echo ""

echo "4️⃣ KIỂM TRA COLUMNSTORE INDEXES"
echo "==============================="

if command -v sqlcmd &> /dev/null; then
    # Count columnstore indexes
    COLUMNSTORE_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -C -h -1 -Q "SELECT COUNT(*) FROM sys.indexes WHERE type_desc LIKE '%COLUMNSTORE%';" 2>/dev/null | grep -E '^[0-9]+$' | head -1)
    
    if [[ $COLUMNSTORE_COUNT =~ ^[0-9]+$ ]] && [[ $COLUMNSTORE_COUNT -gt 0 ]]; then
        echo "✅ Database có $COLUMNSTORE_COUNT Columnstore Indexes"
        
        # Check which tables have columnstore indexes
        TABLES_WITH_COLUMNSTORE=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -C -h -1 -Q "SELECT COUNT(DISTINCT t.name) FROM sys.indexes i INNER JOIN sys.tables t ON i.object_id = t.object_id WHERE i.type_desc LIKE '%COLUMNSTORE%' AND t.name IN ('DP01_New','LN01','DB01','GL01','GL41','DPDA','EI01','KH03','LN02','LN03','RR01','7800_DT_KHKD1');" 2>/dev/null | grep -E '^[0-9]+$' | head -1)
        
        echo "✅ $TABLES_WITH_COLUMNSTORE/12 bảng dữ liệu thô có Columnstore Indexes"
    else
        echo "❌ Không có Columnstore Indexes hoặc không thể kiểm tra"
    fi
fi

echo ""

echo "5️⃣ KIỂM TRA LEGACY CLEANUP" 
echo "=========================="

# Check if ImportedDataItems is removed from models
if ! grep -q "ImportedDataItems" Backend/TinhKhoanApp.Api/Models/ImportedDataRecord.cs 2>/dev/null; then
    echo "✅ ImportedDataItems navigation property đã được xóa khỏi models"
else
    echo "❌ ImportedDataItems navigation property vẫn còn trong models"
fi

# Check if legacy controllers are disabled
DISABLED_CONTROLLERS=$(ls Backend/TinhKhoanApp.Api/Controllers/Legacy_Disabled/*.disabled 2>/dev/null | wc -l)
if [[ $DISABLED_CONTROLLERS -gt 0 ]]; then
    echo "✅ $DISABLED_CONTROLLERS legacy controllers đã được disabled"
else
    echo "⚠️ Không có legacy controllers nào được disabled"
fi

# Check if migration exists
if ls Backend/TinhKhoanApp.Api/Migrations/*DropImportedDataItemsTable* 1> /dev/null 2>&1; then
    echo "✅ Migration xóa ImportedDataItems table đã tồn tại"
else
    echo "❌ Migration xóa ImportedDataItems table chưa có"
fi

echo ""

echo "6️⃣ KIỂM TRA BACKEND BUILD STATUS"
echo "================================"

cd Backend/TinhKhoanApp.Api
BUILD_RESULT=$(dotnet build --verbosity quiet 2>&1)
BUILD_EXIT_CODE=$?

if [[ $BUILD_EXIT_CODE -eq 0 ]]; then
    echo "✅ Backend build thành công"
    
    # Count warnings
    WARNING_COUNT=$(echo "$BUILD_RESULT" | grep -c "warning" || echo "0")
    if [[ $WARNING_COUNT -gt 0 ]]; then
        echo "⚠️ Build có $WARNING_COUNT warnings"
    else
        echo "✅ Build không có warnings"
    fi
else
    echo "❌ Backend build thất bại"
    echo "Lỗi build: $BUILD_RESULT"
fi

cd - > /dev/null

echo ""

echo "7️⃣ TÓM TẮT ĐÁNH GIÁ"
echo "==================="

# Calculate overall score
TOTAL_CHECKS=10
PASSED_CHECKS=0

# Count passed checks (simplified)
if grep -q "DirectImport/smart" Frontend/tinhkhoan-app-ui-vite/src/services/rawDataService.js; then ((PASSED_CHECKS++)); fi
if grep -q "DirectImport/smart" Frontend/tinhkhoan-app-ui-vite/src/services/smartImportService.js; then ((PASSED_CHECKS++)); fi
if [[ $DIRECTIMPORT_STATUS == *"Direct Import System Online"* ]]; then ((PASSED_CHECKS++)); fi
if [[ $TEMPORAL_COUNT =~ ^[0-9]+$ ]] && [[ $TEMPORAL_COUNT -gt 10 ]]; then ((PASSED_CHECKS++)); fi
if [[ $COLUMNSTORE_COUNT =~ ^[0-9]+$ ]] && [[ $COLUMNSTORE_COUNT -gt 10 ]]; then ((PASSED_CHECKS++)); fi
if ! grep -q "ImportedDataItems" Backend/TinhKhoanApp.Api/Models/ImportedDataRecord.cs 2>/dev/null; then ((PASSED_CHECKS++)); fi
if [[ $DISABLED_CONTROLLERS -gt 0 ]]; then ((PASSED_CHECKS++)); fi
if ls Backend/TinhKhoanApp.Api/Migrations/*DropImportedDataItemsTable* 1> /dev/null 2>&1; then ((PASSED_CHECKS++)); fi
if [[ $BUILD_EXIT_CODE -eq 0 ]]; then ((PASSED_CHECKS++)); fi
if [[ $DATA_TYPES_COUNT -ge 10 ]]; then ((PASSED_CHECKS++)); fi

PASS_PERCENTAGE=$((PASSED_CHECKS * 100 / TOTAL_CHECKS))

echo "📊 Kết quả tổng thể: $PASSED_CHECKS/$TOTAL_CHECKS checks passed ($PASS_PERCENTAGE%)"

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
echo "📝 Báo cáo này được tạo bởi script tự động verification"
echo "📅 $(date '+%d/%m/%Y %H:%M:%S')"
echo "==========================================="
