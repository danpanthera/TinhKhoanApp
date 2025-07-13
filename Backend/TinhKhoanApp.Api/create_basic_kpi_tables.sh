#!/bin/bash

echo "🎯 TẠO CƠ BẢN 32 BẢNG KPI ASSIGNMENT TABLES"
echo "==========================================="

API_BASE="http://localhost:5055/api"

# Tạo 23 bảng cho cán bộ (vai trò)
echo "📋 1. Tạo 23 bảng KPI cho cán bộ (vai trò)..."

ROLES=(
    "TruongphongKhdn"
    "TruongphongKhcn"
    "PhophongKhdn"
    "PhophongKhcn"
    "TruongphongKhqlrr"
    "PhophongKhqlrr"
    "Cbtd"
    "TruongphongKtnqCnl1"
    "PhophongKtnqCnl1"
    "Gdv"
    "TqHkKtnb"
    "TruongphoItThKtgs"
    "CBItThKtgsKhqlrr"
    "GiamdocPgd"
    "PhogiamdocPgd"
    "PhogiamdocPgdCbtd"
    "GiamdocCnl2"
    "PhogiamdocCnl2Td"
    "PhogiamdocCnl2Kt"
    "TruongphongKhCnl2"
    "PhophongKhCnl2"
    "TruongphongKtnqCnl2"
    "PhophongKtnqCnl2"
)

ROLE_DESCRIPTIONS=(
    "Trưởng phòng KHDN"
    "Trưởng phòng KHCN"
    "Phó phòng KHDN"
    "Phó phòng KHCN"
    "Trưởng phòng KH&QLRR"
    "Phó phòng KH&QLRR"
    "Cán bộ tín dụng"
    "Trưởng phòng KTNQ CNL1"
    "Phó phòng KTNQ CNL1"
    "GDV"
    "Thủ quỹ | Hậu kiểm | KTNB"
    "Trưởng phó IT | Tổng hợp | KTGS"
    "Cán bộ IT | Tổng hợp | KTGS | KH&QLRR"
    "Giám đốc Phòng giao dịch"
    "Phó giám đốc Phòng giao dịch"
    "Phó giám đốc PGD kiêm CBTD"
    "Giám đốc CNL2"
    "Phó giám đốc CNL2 phụ trách TD"
    "Phó giám đốc CNL2 phụ trách KT"
    "Trưởng phòng KH CNL2"
    "Phó phòng KH CNL2"
    "Trưởng phòng KTNQ CNL2"
    "Phó phòng KTNQ CNL2"
)

for i in "${!ROLES[@]}"; do
    ROLE_CODE="${ROLES[$i]}"
    DESCRIPTION="${ROLE_DESCRIPTIONS[$i]}"

    echo "Tạo bảng KPI: $ROLE_CODE"

    curl -s -X POST "$API_BASE/KpiAssignmentTables" \
        -H "Content-Type: application/json" \
        -d "{
            \"TableName\": \"$ROLE_CODE\",
            \"Description\": \"KPI cho $DESCRIPTION\",
            \"Category\": \"CANBO\"
        }" > /dev/null
done

# Tạo 9 bảng cho chi nhánh
echo "🏢 2. Tạo 9 bảng KPI cho chi nhánh..."

BRANCHES=(
    "HoiSo"
    "BinhLu"
    "PhongTho"
    "SinHo"
    "BumTo"
    "ThanUyen"
    "DoanKet"
    "TanUyen"
    "NamHang"
)

BRANCH_DESCRIPTIONS=(
    "KPI cho Hội Sở"
    "KPI cho Chi nhánh Bình Lư"
    "KPI cho Chi nhánh Phong Thổ"
    "KPI cho Chi nhánh Sìn Hồ"
    "KPI cho Chi nhánh Bum Tở"
    "KPI cho Chi nhánh Than Uyên"
    "KPI cho Chi nhánh Đoàn Kết"
    "KPI cho Chi nhánh Tân Uyên"
    "KPI cho Chi nhánh Nậm Hàng"
)

for i in "${!BRANCHES[@]}"; do
    BRANCH_CODE="${BRANCHES[$i]}"
    DESCRIPTION="${BRANCH_DESCRIPTIONS[$i]}"

    echo "Tạo bảng KPI: $BRANCH_CODE"

    curl -s -X POST "$API_BASE/KpiAssignmentTables" \
        -H "Content-Type: application/json" \
        -d "{
            \"TableName\": \"$BRANCH_CODE\",
            \"Description\": \"$DESCRIPTION\",
            \"Category\": \"CHINHANH\"
        }" > /dev/null
done

echo ""
echo "📊 3. KIỂM TRA KẾT QUẢ:"
CANBO_COUNT=$(curl -s "$API_BASE/KpiAssignmentTables" | jq '[.[] | select(.Category == "CANBO")] | length')
CHINHANH_COUNT=$(curl -s "$API_BASE/KpiAssignmentTables" | jq '[.[] | select(.Category == "CHINHANH")] | length')
TOTAL_COUNT=$(curl -s "$API_BASE/KpiAssignmentTables" | jq 'length')

echo "   ✅ Cán bộ: $CANBO_COUNT/23"
echo "   ✅ Chi nhánh: $CHINHANH_COUNT/9"
echo "   ✅ Tổng: $TOTAL_COUNT/32"

if [ "$TOTAL_COUNT" -eq 32 ]; then
    echo "🎉 THÀNH CÔNG: Đã tạo đủ 32 bảng KPI Assignment Tables!"
else
    echo "⚠️  Cần kiểm tra lại: Chỉ tạo được $TOTAL_COUNT/32 bảng"
fi
