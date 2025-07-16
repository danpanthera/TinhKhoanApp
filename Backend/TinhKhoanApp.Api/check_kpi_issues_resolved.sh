#!/bin/bash

# ====================================
# KIỂM TRA KẾT QUẢ SAU KHẮC PHỤC 4 VẤN ĐỀ KPI
# ====================================

echo "🔧 KIỂM TRA KẾT QUẢ SAU KHẮC PHỤC 4 VẤN ĐỀ KPI"
echo "============================================="

BASE_URL="http://localhost:5055/api"

echo "📊 1. KIỂM TRA CẤU HÌNH KPI:"
echo "----------------------------"

# KPI Assignment Tables
TABLES_COUNT=$(curl -s "${BASE_URL}/KpiAssignmentTables" | jq '. | length')
echo "✅ Tổng KPI Assignment Tables: $TABLES_COUNT/32"

# Phân loại theo category
EMPLOYEE_TABLES=$(curl -s "${BASE_URL}/KpiAssignmentTables" | jq '[.[] | select(.Category == "VAI TRÒ CÁN BỘ")] | length')
BRANCH_TABLES=$(curl -s "${BASE_URL}/KpiAssignmentTables" | jq '[.[] | select(.Category == "CHI NHÁNH")] | length')

echo "   📋 Bảng cho cán bộ: $EMPLOYEE_TABLES/23"
echo "   🏢 Bảng cho chi nhánh: $BRANCH_TABLES/9"

# KPI Indicators
INDICATORS_COUNT=$(curl -s "${BASE_URL}/KpiIndicators" | jq '. | length')
echo "✅ Tổng KPI Indicators: $INDICATORS_COUNT"

if [ "$INDICATORS_COUNT" -gt 0 ]; then
    echo ""
    echo "📋 Mẫu indicators theo bảng:"
    # Lấy một số indicators đầu tiên và group theo TableId
    curl -s "${BASE_URL}/KpiIndicators" | jq -r 'group_by(.TableId)[:3] | .[] | "   Table \(.[0].TableId): \(length) indicators"'
fi

echo ""
echo "🎯 2. KIỂM TRA GIAO KHOÁN KPI:"
echo "-----------------------------"

# Test API endpoints cho giao khoán
echo "🔗 API Test - Lấy indicators cho bảng ID 1:"
SAMPLE_INDICATORS=$(curl -s "${BASE_URL}/KpiIndicators/table/1" | jq '. | length')
echo "   ✅ Bảng ID 1 có: $SAMPLE_INDICATORS indicators"

echo ""
echo "🔗 API Test - Lấy indicators cho bảng ID 14 (GiamdocPgd):"
PGD_INDICATORS=$(curl -s "${BASE_URL}/KpiIndicators/table/14" | jq '. | length')
echo "   ✅ Bảng GiamdocPgd có: $PGD_INDICATORS indicators"

echo ""
echo "🔗 API Test - Lấy indicators cho bảng ID 24 (HoiSo - Chi nhánh):"
HOISHO_INDICATORS=$(curl -s "${BASE_URL}/KpiIndicators/table/24" | jq '. | length')
echo "   ✅ Bảng HoiSo có: $HOISHO_INDICATORS indicators"

echo ""
echo "📈 3. KIỂM TRA FRONTEND READINESS:"
echo "---------------------------------"

echo "🎯 Kiểm tra các endpoint cần thiết cho frontend:"

endpoints=("KpiAssignmentTables" "KpiIndicators" "KhoanPeriods" "Employees" "Roles")

for endpoint in "${endpoints[@]}"; do
    status_code=$(curl -s -o /dev/null -w "%{http_code}" "${BASE_URL}/$endpoint")
    if [ "$status_code" -eq 200 ]; then
        echo "   ✅ $endpoint: OK ($status_code)"
    else
        echo "   ❌ $endpoint: ERROR ($status_code)"
    fi
done

echo ""
echo "🏆 4. TỔNG KẾT KHẮC PHỤC 4 VẤN ĐỀ:"
echo "===================================="

# Vấn đề 1: Cấu hình KPI dropdown
if [ "$EMPLOYEE_TABLES" -eq 23 ] && [ "$BRANCH_TABLES" -eq 9 ]; then
    echo "✅ VẤN ĐỀ 1: Cấu hình KPI đã có dropdown 23 bảng cán bộ + 9 bảng chi nhánh"
else
    echo "❌ VẤN ĐỀ 1: Cấu hình KPI chưa đủ bảng ($EMPLOYEE_TABLES/23 cán bộ, $BRANCH_TABLES/9 chi nhánh)"
fi

# Vấn đề 2: Giao khoán KPI theo cán bộ có indicators
if [ "$INDICATORS_COUNT" -gt 0 ] && [ "$SAMPLE_INDICATORS" -gt 0 ]; then
    echo "✅ VẤN ĐỀ 2: Giao khoán KPI theo cán bộ đã có chỉ tiêu trong bảng"
else
    echo "❌ VẤN ĐỀ 2: Giao khoán KPI theo cán bộ chưa có chỉ tiêu"
fi

# Vấn đề 3: Giao khoán KPI theo chi nhánh
if [ "$HOISHO_INDICATORS" -gt 0 ]; then
    echo "✅ VẤN ĐỀ 3: Giao khoán KPI theo chi nhánh đã có dropdown và chỉ tiêu"
else
    echo "❌ VẤN ĐỀ 3: Giao khoán KPI theo chi nhánh chưa có chỉ tiêu"
fi

# Vấn đề 4: Dependency giữa Cấu hình và Giao khoán
if [ "$INDICATORS_COUNT" -eq 257 ] && [ "$TABLES_COUNT" -eq 32 ]; then
    echo "✅ VẤN ĐỀ 4: Hệ thống KPI có tính nhất quán (257 indicators cho 32 bảng: 158 cán bộ + 99 chi nhánh)"
else
    echo "❌ VẤN ĐỀ 4: Hệ thống KPI chưa nhất quán ($INDICATORS_COUNT/257 indicators, $TABLES_COUNT/32 bảng)"
fi

echo ""
echo "📊 ĐIỂM TỔNG KẾT:"
total_issues=4
resolved_issues=0

if [ "$EMPLOYEE_TABLES" -eq 23 ] && [ "$BRANCH_TABLES" -eq 9 ]; then
    ((resolved_issues++))
fi

if [ "$INDICATORS_COUNT" -gt 0 ] && [ "$SAMPLE_INDICATORS" -gt 0 ]; then
    ((resolved_issues++))
fi

if [ "$HOISHO_INDICATORS" -gt 0 ]; then
    ((resolved_issues++))
fi

if [ "$INDICATORS_COUNT" -eq 257 ] && [ "$TABLES_COUNT" -eq 32 ]; then
    ((resolved_issues++))
fi

percentage=$((resolved_issues * 100 / total_issues))
echo "🎯 Đã khắc phục: $resolved_issues/$total_issues vấn đề ($percentage%)"

if [ "$percentage" -eq 100 ]; then
    echo "🎉 HOÀN THÀNH TẤT CẢ! Hệ thống KPI đã sẵn sàng hoạt động."
elif [ "$percentage" -ge 75 ]; then
    echo "👍 RẤT TỐT! Chỉ còn một số chi tiết nhỏ cần điều chỉnh."
elif [ "$percentage" -ge 50 ]; then
    echo "⚠️ KHỞI SẮC! Đã giải quyết được một nửa, cần tiếp tục."
else
    echo "❌ CẦN TIẾP TỤC! Vẫn còn nhiều vấn đề cần khắc phục."
fi

echo ""
echo "🚀 CÁC BƯỚC TIẾP THEO CHO FRONTEND:"
echo "- Kiểm tra lại dropdown hiển thị 23 bảng cán bộ"
echo "- Kiểm tra dropdown hiển thị 9 bảng chi nhánh"
echo "- Test chức năng giao khoán với indicators"
echo "- Xác nhận thay đổi cấu hình ảnh hưởng đến giao khoán"
