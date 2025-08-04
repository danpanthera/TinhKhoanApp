#!/bin/bash

# Script tạo 23 vai trò cho hệ thống TinhKhoanApp
# Ngày tạo: 06/07/2025
# Tác giả: Assistant

echo "🎯 BẮT ĐẦU TẠO 23 VAI TRÒ THEO DANH SÁCH CHUẨN"
echo "================================================"

BASE_URL="http://localhost:5055/api/roles"

# Hàm tạo role
create_role() {
    local code="$1"
    local name="$2"
    local description="$3"

    echo "Đang tạo vai trò: $name ($code)"

    response=$(curl -s -X POST "$BASE_URL" \
        -H "Content-Type: application/json" \
        -d "{
            \"Name\": \"$name\",
            \"Description\": \"$description\"
        }")

    if echo "$response" | jq -e '.id' > /dev/null 2>&1; then
        id=$(echo "$response" | jq -r '.id')
        echo "✅ Tạo thành công - ID: $id"
    else
        echo "❌ Lỗi tạo vai trò: $response"
    fi

    sleep 0.5
}

# Tạo 23 vai trò theo danh sách chuẩn
echo ""
echo "📋 DANH SÁCH 23 VAI TRÒ:"

create_role "TruongphongKhdn" "Trưởng phòng KHDN" "Trưởng phòng Khách hàng Doanh nghiệp"
create_role "TruongphongKhcn" "Trưởng phòng KHCN" "Trưởng phòng Khách hàng Cá nhân"
create_role "PhophongKhdn" "Phó phòng KHDN" "Phó phòng Khách hàng Doanh nghiệp"
create_role "PhophongKhcn" "Phó phòng KHCN" "Phó phòng Khách hàng Cá nhân"
create_role "TruongphongKhqlrr" "Trưởng phòng KH&QLRR" "Trưởng phòng Kế hoạch & Quản lý rủi ro"
create_role "PhophongKhqlrr" "Phó phòng KH&QLRR" "Phó phòng Kế hoạch & Quản lý rủi ro"
create_role "Cbtd" "Cán bộ tín dụng" "Cán bộ tín dụng"
create_role "TruongphongKtnqCnl1" "Trưởng phòng KTNQ CNL1" "Trưởng phòng Kế toán & Ngân quỹ CNL1"
create_role "PhophongKtnqCnl1" "Phó phòng KTNQ CNL1" "Phó phòng Kế toán & Ngân quỹ CNL1"
create_role "Gdv" "GDV" "Giao dịch viên"
create_role "TqHkKtnb" "Thủ quỹ | Hậu kiểm | KTNB" "Thủ quỹ | Hậu kiểm | Kế toán nghiệp vụ"
create_role "TruongphoItThKtgs" "Trưởng phó IT | Tổng hợp | KTGS" "Trưởng phó IT | Tổng hợp | Kiểm tra giám sát"
create_role "CBItThKtgsKhqlrr" "Cán bộ IT | Tổng hợp | KTGS | KH&QLRR" "Cán bộ IT | Tổng hợp | KTGS | KH&QLRR"
create_role "GiamdocPgd" "Giám đốc Phòng giao dịch" "Giám đốc Phòng giao dịch"
create_role "PhogiamdocPgd" "Phó giám đốc Phòng giao dịch" "Phó giám đốc Phòng giao dịch"
create_role "PhogiamdocPgdCbtd" "Phó giám đốc PGD kiêm CBTD" "Phó giám đốc Phòng giao dịch kiêm CBTD"
create_role "GiamdocCnl2" "Giám đốc CNL2" "Giám đốc Chi nhánh cấp 2"
create_role "PhogiamdocCnl2Td" "Phó giám đốc CNL2 phụ trách TD" "Phó giám đốc CNL2 phụ trách Tín dụng"
create_role "PhogiamdocCnl2Kt" "Phó giám đốc CNL2 phụ trách KT" "Phó giám đốc CNL2 phụ trách Kế toán"
create_role "TruongphongKhCnl2" "Trưởng phòng KH CNL2" "Trưởng phòng Khách hàng CNL2"
create_role "PhophongKhCnl2" "Phó phòng KH CNL2" "Phó phòng Khách hàng CNL2"
create_role "TruongphongKtnqCnl2" "Trưởng phòng KTNQ CNL2" "Trưởng phòng Kế toán & Ngân quỹ CNL2"
create_role "PhophongKtnqCnl2" "Phó phòng KTNQ CNL2" "Phó phòng Kế toán & Ngân quỹ CNL2"

echo ""
echo "🏁 HOÀN THÀNH TẠO 23 VAI TRÒ"
echo "=========================="

# Kiểm tra kết quả
echo ""
echo "📊 KIỂM TRA KẾT QUẢ:"
total_roles=$(curl -s "$BASE_URL" | jq 'length')
echo "Tổng số vai trò đã tạo: $total_roles"

if [ "$total_roles" -eq 23 ]; then
    echo "✅ THÀNH CÔNG: Đã tạo đủ 23 vai trò!"
else
    echo "⚠️ CẢNH BÁO: Số lượng vai trò không đúng ($total_roles/23)"
fi

echo ""
echo "📋 DANH SÁCH VAI TRÒ ĐÃ TẠO:"
curl -s "$BASE_URL" | jq -r '.[] | "ID: \(.id) | Code: \(.roleCode) | Name: \(.roleName)"'
