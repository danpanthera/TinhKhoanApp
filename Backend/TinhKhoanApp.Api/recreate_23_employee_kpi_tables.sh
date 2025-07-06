#!/bin/bash
set -e

echo "🏗️ RECREATE 23 EMPLOYEE KPI TABLES - Tạo lại 23 bảng KPI cán bộ"
echo "==============================================================="

API_BASE="http://localhost:5055/api"

# Danh sách 23 bảng KPI cán bộ theo đúng tên anh cung cấp
echo "📋 Tạo 23 bảng KPI cán bộ..."

# Function to create table
create_kpi_table() {
    local table_name="$1"
    local description="$2"
    local count="$3"

    echo "  $count. Tạo bảng: $table_name - $description"

    # JSON payload cho tạo bảng KPI
    JSON_PAYLOAD=$(cat <<EOF
{
    "TableType": "$table_name",
    "TableName": "$table_name",
    "Description": "$description",
    "Category": "CANBO",
    "IsActive": true
}
EOF
)

    # Gọi API tạo bảng
    RESPONSE=$(curl -s -X POST "$API_BASE/KpiAssignment/tables" \
        -H "Content-Type: application/json" \
        -d "$JSON_PAYLOAD")

    echo "    Response: $RESPONSE"
}

# Tạo từng bảng
create_kpi_table "TruongphongKhdn" "Bảng KPI Trưởng phòng Khách hàng Doanh nghiệp" 1
create_kpi_table "TruongphongKhcn" "Bảng KPI Trưởng phòng Khách hàng Cá nhân" 2
create_kpi_table "PhophongKhdn" "Bảng KPI Phó phòng Khách hàng Doanh nghiệp" 3
create_kpi_table "PhophongKhcn" "Bảng KPI Phó phòng Khách hàng Cá nhân" 4
create_kpi_table "TruongphongKhqlrr" "Bảng KPI Trưởng phòng Kế hoạch & Quản lý rủi ro" 5
create_kpi_table "PhophongKhqlrr" "Bảng KPI Phó phòng Kế hoạch & Quản lý rủi ro" 6
create_kpi_table "Cbtd" "Bảng KPI Cán bộ tín dụng" 7
create_kpi_table "TruongphongKtnqCnl1" "Bảng KPI Trưởng phòng Kế toán & Ngân quỹ CNL1" 8
create_kpi_table "PhophongKtnqCnl1" "Bảng KPI Phó phòng Kế toán & Ngân quỹ CNL1" 9
create_kpi_table "Gdv" "Bảng KPI Giao dịch viên" 10
create_kpi_table "TqHkKtnb" "Bảng KPI Tổ quản lý Hành chính Kế toán Nhân bàn" 11
create_kpi_table "TruongphoItThKtgs" "Bảng KPI Trưởng phòng IT/TH/KTGS" 12
create_kpi_table "CBItThKtgsKhqlrr" "Bảng KPI CB IT/TH/KTGS & KHQLRR" 13
create_kpi_table "GiamdocPgd" "Bảng KPI Giám đốc Phòng giao dịch" 14
create_kpi_table "PhogiamdocPgd" "Bảng KPI Phó Giám đốc Phòng giao dịch" 15
create_kpi_table "PhogiamdocPgdCbtd" "Bảng KPI Phó Giám đốc PGD Cán bộ tín dụng" 16
create_kpi_table "GiamdocCnl2" "Bảng KPI Giám đốc Chi nhánh cấp 2" 17
create_kpi_table "PhogiamdocCnl2Td" "Bảng KPI Phó Giám đốc CNL2 Tín dụng" 18
create_kpi_table "PhogiamdocCnl2Kt" "Bảng KPI Phó Giám đốc CNL2 Kế toán" 19
create_kpi_table "TruongphongKhCnl2" "Bảng KPI Trưởng phòng Khách hàng CNL2" 20
create_kpi_table "PhophongKhCnl2" "Bảng KPI Phó phòng Khách hàng CNL2" 21
create_kpi_table "TruongphongKtnqCnl2" "Bảng KPI Trưởng phòng Kế toán & Ngân quỹ CNL2" 22
create_kpi_table "PhophongKtnqCnl2" "Bảng KPI Phó phòng Kế toán & Ngân quỹ CNL2" 23

    echo "  $count. Tạo bảng: $table_name - $description"

    # JSON payload cho tạo bảng KPI
    JSON_PAYLOAD=$(cat <<EOF
{
    "TableType": "$table_name",
    "TableName": "$table_name",
    "Description": "$description",
    "Category": "CANBO",
    "IsActive": true
}
EOF
)

    # Gọi API tạo bảng
    RESPONSE=$(curl -s -X POST "$API_BASE/KpiAssignment/tables" \
        -H "Content-Type: application/json" \
        -d "$JSON_PAYLOAD")


echo ""
echo "✅ Hoàn thành tạo 23 bảng KPI cán bộ!"
echo ""
echo "📊 Kiểm tra kết quả:"
EMPLOYEE_COUNT=$(curl -s "$API_BASE/KpiAssignment/tables" | jq '[.[] | select(.Category == "CANBO")] | length')
TOTAL_COUNT=$(curl -s "$API_BASE/KpiAssignment/tables" | jq 'length')

echo "Số bảng KPI cán bộ: $EMPLOYEE_COUNT/23"
echo "Tổng số bảng KPI: $TOTAL_COUNT"

if [ "$EMPLOYEE_COUNT" = "23" ]; then
    echo "🎉 THÀNH CÔNG: Đã có đủ 23 bảng KPI cán bộ!"
else
    echo "⚠️  CẢNH BÁO: Chỉ có $EMPLOYEE_COUNT/23 bảng KPI cán bộ"
fi

echo ""
echo "🎯 Sẵn sàng populate KPI indicators cho từng bảng!"
