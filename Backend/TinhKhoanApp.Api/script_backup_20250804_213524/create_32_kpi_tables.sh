#!/bin/bash

echo "=== TẠO 32 BẢNG KPI ASSIGNMENT TABLES ==="
echo "23 bảng cho Cán bộ + 9 bảng cho Chi nhánh"

API_BASE="http://localhost:5055/api/KpiAssignmentTables"

# Xóa toàn bộ dữ liệu cũ nếu có
echo "Cleaning up existing data..."
curl -X DELETE "$API_BASE/cleanup" -H "Content-Type: application/json" 2>/dev/null

echo ""
echo "=== TẠO 23 BẢNG CHO CÁN BỘ ==="

# 23 bảng cho cán bộ (Category = "CANBO")
declare -a canbo_tables=(
    "1:TruongphongKhdn:Trưởng phòng KHDN:Trưởng phòng Khách hàng Doanh nghiệp"
    "2:TruongphongKhcn:Trưởng phòng KHCN:Trưởng phòng Khách hàng Cá nhân"
    "3:PhophongKhdn:Phó phòng KHDN:Phó phòng Khách hàng Doanh nghiệp"
    "4:PhophongKhcn:Phó phòng KHCN:Phó phòng Khách hàng Cá nhân"
    "5:TruongphongKhqlrr:Trưởng phòng KH&QLRR:Trưởng phòng Kế hoạch & Quản lý rủi ro"
    "6:PhophongKhqlrr:Phó phòng KH&QLRR:Phó phòng Kế hoạch & Quản lý rủi ro"
    "7:Cbtd:Cán bộ tín dụng:Cán bộ tín dụng"
    "8:TruongphongKtnqCnl1:Trưởng phòng KTNQ CNL1:Trưởng phòng Kế toán & Ngân quỹ CNL1"
    "9:PhophongKtnqCnl1:Phó phòng KTNQ CNL1:Phó phòng Kế toán & Ngân quỹ CNL1"
    "10:Gdv:GDV:Giao dịch viên"
    "11:TqHkKtnb:Thủ quỹ | Hậu kiểm | KTNB:Thủ quỹ | Hậu kiểm | Kế toán nghiệp vụ"
    "12:TruongphoItThKtgs:Trưởng phó IT | Tổng hợp | KTGS:Trưởng phó IT | Tổng hợp | Kiểm tra giám sát"
    "13:CBItThKtgsKhqlrr:Cán bộ IT | Tổng hợp | KTGS | KH&QLRR:Cán bộ IT | Tổng hợp | KTGS | KH&QLRR"
    "14:GiamdocPgd:Giám đốc Phòng giao dịch:Giám đốc Phòng giao dịch"
    "15:PhogiamdocPgd:Phó giám đốc Phòng giao dịch:Phó giám đốc Phòng giao dịch"
    "16:PhogiamdocPgdCbtd:Phó giám đốc PGD kiêm CBTD:Phó giám đốc Phòng giao dịch kiêm CBTD"
    "17:GiamdocCnl2:Giám đốc CNL2:Giám đốc Chi nhánh cấp 2"
    "18:PhogiamdocCnl2Td:Phó giám đốc CNL2 phụ trách TD:Phó giám đốc CNL2 phụ trách Tín dụng"
    "19:PhogiamdocCnl2Kt:Phó giám đốc CNL2 phụ trách KT:Phó giám đốc CNL2 phụ trách Kế toán"
    "20:TruongphongKhCnl2:Trưởng phòng KH CNL2:Trưởng phòng Khách hàng CNL2"
    "21:PhophongKhCnl2:Phó phòng KH CNL2:Phó phòng Khách hàng CNL2"
    "22:TruongphongKtnqCnl2:Trưởng phòng KTNQ CNL2:Trưởng phòng Kế toán & Ngân quỹ CNL2"
    "23:PhophongKtnqCnl2:Phó phòng KTNQ CNL2:Phó phòng Kế toán & Ngân quỹ CNL2"
)

for table_info in "${canbo_tables[@]}"; do
    IFS=':' read -r table_type table_name display_name description <<< "$table_info"

    echo "Creating table $table_type: $display_name"

    curl -X POST "$API_BASE" \
        -H "Content-Type: application/json" \
        -d "{
            \"TableType\": $table_type,
            \"TableName\": \"$display_name\",
            \"Description\": \"$description\",
            \"Category\": \"CANBO\",
            \"IsActive\": true
        }" \
        -s > /dev/null

    if [ $? -eq 0 ]; then
        echo "✅ Created: $display_name"
    else
        echo "❌ Failed: $display_name"
    fi
done

echo ""
echo "=== TẠO 9 BẢNG CHO CHI NHÁNH ==="

# 9 bảng cho chi nhánh (Category = "CHINHANH")
declare -a chinhanh_tables=(
    "200:HoiSo:KPI Hội Sở:Bảng KPI cho Hội Sở"
    "201:CnTamDuong:KPI Chi nhánh Bình Lư:Bảng KPI cho Chi nhánh Bình Lư"
    "202:CnPhongTho:KPI Chi nhánh Phong Thổ:Bảng KPI cho Chi nhánh Phong Thổ"
    "203:CnSinHo:KPI Chi nhánh Sìn Hồ:Bảng KPI cho Chi nhánh Sìn Hồ"
    "204:CnMuongTe:KPI Chi nhánh Bum Tở:Bảng KPI cho Chi nhánh Bum Tở"
    "205:CnThanUyen:KPI Chi nhánh Than Uyên:Bảng KPI cho Chi nhánh Than Uyên"
    "206:CnThanhPho:KPI Chi nhánh Đoàn Kết:Bảng KPI cho Chi nhánh Đoàn Kết"
    "207:CnTanUyen:KPI Chi nhánh Tân Uyên:Bảng KPI cho Chi nhánh Tân Uyên"
    "208:CnNamNhun:KPI Chi nhánh Nậm Hàng:Bảng KPI cho Chi nhánh Nậm Hàng"
)

for table_info in "${chinhanh_tables[@]}"; do
    IFS=':' read -r table_type table_name display_name description <<< "$table_info"

    echo "Creating table $table_type: $display_name"

    curl -X POST "$API_BASE" \
        -H "Content-Type: application/json" \
        -d "{
            \"TableType\": $table_type,
            \"TableName\": \"$display_name\",
            \"Description\": \"$description\",
            \"Category\": \"CHINHANH\",
            \"IsActive\": true
        }" \
        -s > /dev/null

    if [ $? -eq 0 ]; then
        echo "✅ Created: $display_name"
    else
        echo "❌ Failed: $display_name"
    fi
done

echo ""
echo "=== VERIFICATION ==="
echo "Checking total count..."
total_count=$(curl -s "$API_BASE" | jq '. | length')
canbo_count=$(curl -s "$API_BASE" | jq '[.[] | select(.Category == "CANBO")] | length')
chinhanh_count=$(curl -s "$API_BASE" | jq '[.[] | select(.Category == "CHINHANH")] | length')

echo "📊 RESULTS:"
echo "- Total KPI tables: $total_count"
echo "- Cán bộ tables: $canbo_count"
echo "- Chi nhánh tables: $chinhanh_count"

if [ "$total_count" = "32" ] && [ "$canbo_count" = "23" ] && [ "$chinhanh_count" = "9" ]; then
    echo "✅ SUCCESS: All 32 KPI tables created correctly!"
else
    echo "❌ MISMATCH: Expected 32 total (23 CANBO + 9 CHINHANH)"
fi

echo "=== COMPLETED ==="
