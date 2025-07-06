#!/bin/bash
set -e

echo "📝 POPULATE KPI INDICATORS - Tạo đầy đủ chỉ tiêu KPI cho 23 bảng cán bộ"
echo "======================================================================"

API_BASE="http://localhost:5055/api"

# Function to create KPI indicator
create_kpi_indicator() {
    local table_name="$1"
    local name="$2"
    local score="$3"
    local unit="$4"

    # Get TableId từ table name
    local table_id=$(curl -s "$API_BASE/KpiAssignment/tables" | jq -r ".[] | select(.TableName == \"$table_name\") | .Id")

    if [ -z "$table_id" ]; then
        echo "    ❌ Không tìm thấy TableId cho $table_name"
        return 1
    fi

    echo "    + $name ($score điểm, $unit)"

    # JSON payload cho tạo KPI indicator
    JSON_PAYLOAD=$(cat <<EOF
{
    "TableId": $table_id,
    "IndicatorName": "$name",
    "MaxScore": $score,
    "Unit": "$unit",
    "ValueTypeString": "NUMBER"
}
EOF
)

    RESPONSE=$(curl -s -X POST "$API_BASE/KpiAssignment/indicators" \
        -H "Content-Type: application/json" \
        -d "$JSON_PAYLOAD")

    if [[ "$RESPONSE" == *"error"* || "$RESPONSE" == *"Error"* ]]; then
        echo "      ❌ Lỗi: $RESPONSE"
    fi
}

echo "📋 1. TruongphongKhdn - Trưởng phòng Khách hàng Doanh nghiệp (8 chỉ tiêu)"
create_kpi_indicator "TruongphongKhdn" "Tổng Dư nợ KHDN" 20 "Triệu VND"
create_kpi_indicator "TruongphongKhdn" "Tỷ lệ nợ xấu KHDN" 10 "%"
create_kpi_indicator "TruongphongKhdn" "Thu nợ đã XLRR KHDN" 10 "Triệu VND"
create_kpi_indicator "TruongphongKhdn" "Lợi nhuận khoán tài chính" 10 "Triệu VND"
create_kpi_indicator "TruongphongKhdn" "Phát triển Khách hàng Doanh nghiệp" 10 "Khách hàng"
create_kpi_indicator "TruongphongKhdn" "Điều hành theo chương trình công tác" 20 "%"
create_kpi_indicator "TruongphongKhdn" "Chấp hành quy chế, quy trình nghiệp vụ" 10 "%"
create_kpi_indicator "TruongphongKhdn" "BQ kết quả thực hiện CB trong phòng mình phụ trách" 10 "%"

echo "📋 2. TruongphongKhcn - Trưởng phòng Khách hàng Cá nhân (8 chỉ tiêu)"
create_kpi_indicator "TruongphongKhcn" "Tổng Dư nợ KHCN" 20 "Triệu VND"
create_kpi_indicator "TruongphongKhcn" "Tỷ lệ nợ xấu KHCN" 10 "%"
create_kpi_indicator "TruongphongKhcn" "Thu nợ đã XLRR KHCN" 10 "Triệu VND"
create_kpi_indicator "TruongphongKhcn" "Lợi nhuận khoán tài chính" 10 "Triệu VND"
create_kpi_indicator "TruongphongKhcn" "Phát triển Khách hàng Cá nhân" 10 "Khách hàng"
create_kpi_indicator "TruongphongKhcn" "Điều hành theo chương trình công tác" 20 "%"
create_kpi_indicator "TruongphongKhcn" "Chấp hành quy chế, quy trình nghiệp vụ" 10 "%"
create_kpi_indicator "TruongphongKhcn" "BQ kết quả thực hiện CB trong phòng mình phụ trách" 10 "%"

echo "📋 3. PhophongKhdn - Phó phòng Khách hàng Doanh nghiệp (8 chỉ tiêu)"
create_kpi_indicator "PhophongKhdn" "Tổng Dư nợ KHDN" 20 "Triệu VND"
create_kpi_indicator "PhophongKhdn" "Tỷ lệ nợ xấu KHDN" 10 "%"
create_kpi_indicator "PhophongKhdn" "Thu nợ đã XLRR KHDN" 10 "Triệu VND"
create_kpi_indicator "PhophongKhdn" "Lợi nhuận khoán tài chính" 10 "Triệu VND"
create_kpi_indicator "PhophongKhdn" "Phát triển Khách hàng Doanh nghiệp" 10 "Khách hàng"
create_kpi_indicator "PhophongKhdn" "Điều hành theo chương trình công tác" 20 "%"
create_kpi_indicator "PhophongKhdn" "Chấp hành quy chế, quy trình nghiệp vụ" 10 "%"
create_kpi_indicator "PhophongKhdn" "BQ kết quả thực hiện CB trong phòng mình phụ trách" 10 "%"

echo "📋 4. PhophongKhcn - Phó phòng Khách hàng Cá nhân (8 chỉ tiêu)"
create_kpi_indicator "PhophongKhcn" "Tổng Dư nợ KHCN" 20 "Triệu VND"
create_kpi_indicator "PhophongKhcn" "Tỷ lệ nợ xấu KHCN" 10 "%"
create_kpi_indicator "PhophongKhcn" "Thu nợ đã XLRR KHCN" 10 "Triệu VND"
create_kpi_indicator "PhophongKhcn" "Lợi nhuận khoán tài chính" 10 "Triệu VND"
create_kpi_indicator "PhophongKhcn" "Phát triển Khách hàng Cá nhân" 10 "Khách hàng"
create_kpi_indicator "PhophongKhcn" "Điều hành theo chương trình công tác" 20 "%"
create_kpi_indicator "PhophongKhcn" "Chấp hành quy chế, quy trình nghiệp vụ" 10 "%"
create_kpi_indicator "PhophongKhcn" "BQ kết quả thực hiện CB trong phòng mình phụ trách" 10 "%"

echo "📋 5. TruongphongKhqlrr - Trưởng phòng Kế hoạch & Quản lý rủi ro (6 chỉ tiêu)"
create_kpi_indicator "TruongphongKhqlrr" "Tổng nguồn vốn" 10 "Triệu VND"
create_kpi_indicator "TruongphongKhqlrr" "Tổng dư nợ" 10 "Triệu VND"
create_kpi_indicator "TruongphongKhqlrr" "Lợi nhuận khoán tài chính" 10 "Triệu VND"
create_kpi_indicator "TruongphongKhqlrr" "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng" 50 "%"
create_kpi_indicator "TruongphongKhqlrr" "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%"
create_kpi_indicator "TruongphongKhqlrr" "Kết quả thực hiện BQ của CB trong phòng" 10 "%"

echo "📋 6. PhophongKhqlrr - Phó phòng Kế hoạch & Quản lý rủi ro (6 chỉ tiêu)"
create_kpi_indicator "PhophongKhqlrr" "Tổng nguồn vốn" 10 "Triệu VND"
create_kpi_indicator "PhophongKhqlrr" "Tổng dư nợ" 10 "Triệu VND"
create_kpi_indicator "PhophongKhqlrr" "Lợi nhuận khoán tài chính" 10 "Triệu VND"
create_kpi_indicator "PhophongKhqlrr" "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng" 50 "%"
create_kpi_indicator "PhophongKhqlrr" "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%"
create_kpi_indicator "PhophongKhqlrr" "Kết quả thực hiện BQ của CB trong phòng mình phụ trách" 10 "%"

echo "📋 7. Cbtd - Cán bộ tín dụng (8 chỉ tiêu)"
create_kpi_indicator "Cbtd" "Tổng dư nợ BQ" 30 "Triệu VND"
create_kpi_indicator "Cbtd" "Tỷ lệ nợ xấu" 15 "%"
create_kpi_indicator "Cbtd" "Phát triển Khách hàng" 10 "Khách hàng"
create_kpi_indicator "Cbtd" "Thu nợ đã XLRR (nếu không có nợ XLRR thì cộng vào chỉ tiêu Dư nợ)" 10 "Triệu VND"
create_kpi_indicator "Cbtd" "Thực hiện nhiệm vụ theo chương trình công tác" 10 "%"
create_kpi_indicator "Cbtd" "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%"
create_kpi_indicator "Cbtd" "Tổng nguồn vốn huy động BQ" 10 "Triệu VND"
create_kpi_indicator "Cbtd" "Hoàn thành chỉ tiêu giao khoán SPDV" 5 "%"

echo "📋 8. TruongphongKtnqCnl1 - Trưởng phòng Kế toán & Ngân quỹ CNL1 (6 chỉ tiêu)"
create_kpi_indicator "TruongphongKtnqCnl1" "Tổng nguồn vốn" 10 "Triệu VND"
create_kpi_indicator "TruongphongKtnqCnl1" "Lợi nhuận khoán tài chính" 20 "Triệu VND"
create_kpi_indicator "TruongphongKtnqCnl1" "Thu dịch vụ thanh toán trong nước" 10 "Triệu VND"
create_kpi_indicator "TruongphongKtnqCnl1" "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng" 40 "%"
create_kpi_indicator "TruongphongKtnqCnl1" "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%"
create_kpi_indicator "TruongphongKtnqCnl1" "Kết quả thực hiện BQ của CB trong phòng" 10 "%"

echo "📋 9. PhophongKtnqCnl1 - Phó phòng Kế toán & Ngân quỹ CNL1 (6 chỉ tiêu)"
create_kpi_indicator "PhophongKtnqCnl1" "Tổng nguồn vốn" 10 "Triệu VND"
create_kpi_indicator "PhophongKtnqCnl1" "Lợi nhuận khoán tài chính" 20 "Triệu VND"
create_kpi_indicator "PhophongKtnqCnl1" "Thu dịch vụ thanh toán trong nước" 10 "Triệu VND"
create_kpi_indicator "PhophongKtnqCnl1" "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng" 40 "%"
create_kpi_indicator "PhophongKtnqCnl1" "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%"
create_kpi_indicator "PhophongKtnqCnl1" "Kết quả thực hiện BQ của CB thuộc mình phụ trách" 10 "%"

echo "📋 10. Gdv - Giao dịch viên (6 chỉ tiêu)"
create_kpi_indicator "Gdv" "Số bút toán giao dịch BQ" 50 "BT"
create_kpi_indicator "Gdv" "Số bút toán hủy" 15 "BT"
create_kpi_indicator "Gdv" "Thực hiện chức năng, nhiệm vụ được giao" 10 "%"
create_kpi_indicator "Gdv" "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%"
create_kpi_indicator "Gdv" "Tổng nguồn vốn huy động BQ" 10 "Triệu VND"
create_kpi_indicator "Gdv" "Hoàn thành chỉ tiêu giao khoán SPDV" 5 "%"

echo "📋 11. TqHkKtnb - Thủ quỹ | Hậu kiểm | KTNB (chưa có chỉ tiêu cụ thể)"
echo "    ⚠️ Đợi TP KTNQ/Giám đốc CN loại 2 trực tiếp giao sau (chưa có cụ thể trong 186)"

echo "📋 12. TruongphongItThKtgs - Trưởng phòng IT | Tổng hợp | KTGS (5 chỉ tiêu)"
create_kpi_indicator "TruongphongItThKtgs" "Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng" 65 "%"
create_kpi_indicator "TruongphongItThKtgs" "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%"
create_kpi_indicator "TruongphongItThKtgs" "Tổng nguồn vốn huy động BQ" 10 "Triệu VND"
create_kpi_indicator "TruongphongItThKtgs" "Hoàn thành chỉ tiêu giao khoán SPDV" 5 "%"
create_kpi_indicator "TruongphongItThKtgs" "Kết quả thực hiện BQ của cán bộ trong phòng" 10 "%"

echo "📋 13. CbItThKtgsKhqlrr - Cán bộ IT | Tổng hợp | KTGS | KH&QLRR (4 chỉ tiêu)"
create_kpi_indicator "CbItThKtgsKhqlrr" "Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ được giao" 75 "%"
create_kpi_indicator "CbItThKtgsKhqlrr" "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%"
create_kpi_indicator "CbItThKtgsKhqlrr" "Tổng nguồn vốn huy động BQ" 10 "Triệu VND"
create_kpi_indicator "CbItThKtgsKhqlrr" "Hoàn thành chỉ tiêu giao khoán SPDV" 5 "%"

echo "📋 14. GiamdocPgd - Giám đốc Phòng giao dịch (9 chỉ tiêu)"
create_kpi_indicator "GiamdocPgd" "Tổng doanh thu" 25 "Triệu đồng"
create_kpi_indicator "GiamdocPgd" "Lợi nhuận trước thuế" 20 "Triệu đồng"
create_kpi_indicator "GiamdocPgd" "Tăng trưởng dư nợ tín dụng" 15 "%"
create_kpi_indicator "GiamdocPgd" "Tăng trưởng huy động vốn" 15 "%"
create_kpi_indicator "GiamdocPgd" "Tỷ lệ nợ xấu" 10 "%"
create_kpi_indicator "GiamdocPgd" "Thu nhập dịch vụ" 5 "Triệu đồng"
create_kpi_indicator "GiamdocPgd" "Quản lý chi phí" 5 "Triệu đồng"
create_kpi_indicator "GiamdocPgd" "Phát triển khách hàng" 3 "Khách hàng"
create_kpi_indicator "GiamdocPgd" "Đánh giá chất lượng công việc" 2 "Điểm"

echo "📋 15. PhogiamdocPgd - Phó Giám đốc Phòng giao dịch (9 chỉ tiêu)"
create_kpi_indicator "PhogiamdocPgd" "Tổng doanh thu" 25 "Triệu đồng"
create_kpi_indicator "PhogiamdocPgd" "Lợi nhuận trước thuế" 20 "Triệu đồng"
create_kpi_indicator "PhogiamdocPgd" "Tăng trưởng dư nợ tín dụng" 15 "%"
create_kpi_indicator "PhogiamdocPgd" "Tăng trưởng huy động vốn" 15 "%"
create_kpi_indicator "PhogiamdocPgd" "Tỷ lệ nợ xấu" 10 "%"
create_kpi_indicator "PhogiamdocPgd" "Thu nhập dịch vụ" 5 "Triệu đồng"
create_kpi_indicator "PhogiamdocPgd" "Quản lý chi phí" 5 "Triệu đồng"
create_kpi_indicator "PhogiamdocPgd" "Phát triển khách hàng" 3 "Khách hàng"
create_kpi_indicator "PhogiamdocPgd" "Đánh giá chất lượng công việc" 2 "Điểm"

echo "📋 16. PhogiamdocPgdCbtd - Phó Giám đốc PGD Cán bộ tín dụng (8 chỉ tiêu)"
create_kpi_indicator "PhogiamdocPgdCbtd" "Doanh số cho vay mới" 25 "Tỷ đồng"
create_kpi_indicator "PhogiamdocPgdCbtd" "Chất lượng hồ sơ tín dụng" 20 "Điểm"
create_kpi_indicator "PhogiamdocPgdCbtd" "Tỷ lệ nợ xấu" 15 "%"
create_kpi_indicator "PhogiamdocPgdCbtd" "Thu hồi nợ" 15 "Triệu đồng"
create_kpi_indicator "PhogiamdocPgdCbtd" "Phát triển khách hàng" 10 "Khách hàng"
create_kpi_indicator "PhogiamdocPgdCbtd" "Tuân thủ quy trình" 10 "Điểm"
create_kpi_indicator "PhogiamdocPgdCbtd" "Quản lý nhóm" 3 "Điểm"
create_kpi_indicator "PhogiamdocPgdCbtd" "Đánh giá chất lượng công việc" 2 "Điểm"

echo "📋 17. GiamdocCnl2 - Giám đốc Chi nhánh cấp 2 (11 chỉ tiêu)"
create_kpi_indicator "GiamdocCnl2" "Tổng doanh thu" 20 "Triệu đồng"
create_kpi_indicator "GiamdocCnl2" "Lợi nhuận trước thuế" 18 "Triệu đồng"
create_kpi_indicator "GiamdocCnl2" "Tăng trưởng dư nợ tín dụng" 15 "%"
create_kpi_indicator "GiamdocCnl2" "Tăng trưởng huy động vốn" 15 "%"
create_kpi_indicator "GiamdocCnl2" "Tỷ lệ nợ xấu" 10 "%"
create_kpi_indicator "GiamdocCnl2" "Thu nhập dịch vụ" 8 "Triệu đồng"
create_kpi_indicator "GiamdocCnl2" "Quản lý chi phí" 5 "Triệu đồng"
create_kpi_indicator "GiamdocCnl2" "Phát triển khách hàng" 4 "Khách hàng"
create_kpi_indicator "GiamdocCnl2" "Quản lý nhân sự" 3 "Điểm"
create_kpi_indicator "GiamdocCnl2" "Tuân thủ quy định" 1 "Điểm"
create_kpi_indicator "GiamdocCnl2" "Đánh giá chất lượng công việc" 1 "Điểm"

echo "📋 18. PhogiamdocCnl2Td - Phó Giám đốc CNL2 Tín dụng (8 chỉ tiêu)"
create_kpi_indicator "PhogiamdocCnl2Td" "Doanh số cho vay mới" 25 "Tỷ đồng"
create_kpi_indicator "PhogiamdocCnl2Td" "Chất lượng hồ sơ tín dụng" 20 "Điểm"
create_kpi_indicator "PhogiamdocCnl2Td" "Tỷ lệ nợ xấu" 20 "%"
create_kpi_indicator "PhogiamdocCnl2Td" "Thu hồi nợ" 15 "Triệu đồng"
create_kpi_indicator "PhogiamdocCnl2Td" "Phát triển khách hàng" 10 "Khách hàng"
create_kpi_indicator "PhogiamdocCnl2Td" "Tuân thủ quy trình" 5 "Điểm"
create_kpi_indicator "PhogiamdocCnl2Td" "Quản lý nhóm tín dụng" 3 "Điểm"
create_kpi_indicator "PhogiamdocCnl2Td" "Đánh giá chất lượng công việc" 2 "Điểm"

echo "📋 19. PhogiamdocCnl2Kt - Phó Giám đốc CNL2 Kế toán (6 chỉ tiêu)"
create_kpi_indicator "PhogiamdocCnl2Kt" "Quản lý thanh khoản" 30 "Điểm"
create_kpi_indicator "PhogiamdocCnl2Kt" "Báo cáo tài chính" 25 "Điểm"
create_kpi_indicator "PhogiamdocCnl2Kt" "Tuân thủ quy định" 20 "Điểm"
create_kpi_indicator "PhogiamdocCnl2Kt" "Quản lý chi phí" 15 "Điểm"
create_kpi_indicator "PhogiamdocCnl2Kt" "Hỗ trợ kinh doanh" 8 "Điểm"
create_kpi_indicator "PhogiamdocCnl2Kt" "Đánh giá chất lượng công việc" 2 "Điểm"

echo "📋 20. TruongphongKhCnl2 - Trưởng phòng Khách hàng CNL2 (9 chỉ tiêu)"
create_kpi_indicator "TruongphongKhCnl2" "Tăng trưởng dư nợ cho vay" 20 "%"
create_kpi_indicator "TruongphongKhCnl2" "Tăng trưởng huy động vốn" 20 "%"
create_kpi_indicator "TruongphongKhCnl2" "Tỷ lệ nợ xấu" 15 "%"
create_kpi_indicator "TruongphongKhCnl2" "Thu nhập dịch vụ" 15 "Triệu đồng"
create_kpi_indicator "TruongphongKhCnl2" "Phát triển khách hàng mới" 10 "Khách hàng"
create_kpi_indicator "TruongphongKhCnl2" "Chất lượng dịch vụ" 10 "Điểm"
create_kpi_indicator "TruongphongKhCnl2" "Tuân thủ quy trình" 5 "Điểm"
create_kpi_indicator "TruongphongKhCnl2" "Quản lý nhóm" 3 "Điểm"
create_kpi_indicator "TruongphongKhCnl2" "Đánh giá chất lượng công việc" 2 "Điểm"

echo "📋 21. PhophongKhCnl2 - Phó phòng Khách hàng CNL2 (8 chỉ tiêu)"
create_kpi_indicator "PhophongKhCnl2" "Tăng trưởng dư nợ cho vay" 25 "%"
create_kpi_indicator "PhophongKhCnl2" "Tăng trưởng huy động vốn" 20 "%"
create_kpi_indicator "PhophongKhCnl2" "Tỷ lệ nợ xấu" 15 "%"
create_kpi_indicator "PhophongKhCnl2" "Thu nhập dịch vụ" 15 "Triệu đồng"
create_kpi_indicator "PhophongKhCnl2" "Phát triển khách hàng mới" 10 "Khách hàng"
create_kpi_indicator "PhophongKhCnl2" "Chất lượng dịch vụ" 10 "Điểm"
create_kpi_indicator "PhophongKhCnl2" "Tuân thủ quy trình" 3 "Điểm"
create_kpi_indicator "PhophongKhCnl2" "Đánh giá chất lượng công việc" 2 "Điểm"

echo "📋 22. TruongphongKtnqCnl2 - Trưởng phòng Kế toán & Ngân quỹ CNL2 (6 chỉ tiêu)"
create_kpi_indicator "TruongphongKtnqCnl2" "Quản lý thanh khoản" 30 "Điểm"
create_kpi_indicator "TruongphongKtnqCnl2" "Báo cáo tài chính" 25 "Điểm"
create_kpi_indicator "TruongphongKtnqCnl2" "Tuân thủ quy định" 20 "Điểm"
create_kpi_indicator "TruongphongKtnqCnl2" "Quản lý chi phí" 15 "Điểm"
create_kpi_indicator "TruongphongKtnqCnl2" "Hỗ trợ kinh doanh" 8 "Điểm"
create_kpi_indicator "TruongphongKtnqCnl2" "Đánh giá chất lượng công việc" 2 "Điểm"

echo "📋 23. PhophongKtnqCnl2 - Phó phòng Kế toán & Ngân quỹ CNL2 (5 chỉ tiêu)"
create_kpi_indicator "PhophongKtnqCnl2" "Quản lý thanh khoản" 35 "Điểm"
create_kpi_indicator "PhophongKtnqCnl2" "Báo cáo tài chính" 30 "Điểm"
create_kpi_indicator "PhophongKtnqCnl2" "Tuân thủ quy định" 20 "Điểm"
create_kpi_indicator "PhophongKtnqCnl2" "Quản lý chi phí" 13 "Điểm"
create_kpi_indicator "PhophongKtnqCnl2" "Đánh giá chất lượng công việc" 2 "Điểm"

echo "📋 24. CanBoNghiepVuKhac - Cán bộ nghiệp vụ khác (5 chỉ tiêu)"
create_kpi_indicator "CanBoNghiepVuKhac" "Hoàn thành công việc được giao" 60 "%"
create_kpi_indicator "CanBoNghiepVuKhac" "Chấp hành quy chế, quy trình nghiệp vụ" 15 "%"
create_kpi_indicator "CanBoNghiepVuKhac" "Hỗ trợ phát triển kinh doanh" 10 "%"
create_kpi_indicator "CanBoNghiepVuKhac" "Ứng dụng công nghệ" 10 "%"
create_kpi_indicator "CanBoNghiepVuKhac" "Đánh giá tổng hợp" 5 "%"

echo ""
echo "✅ HOÀN THÀNH populate toàn bộ chỉ tiêu KPI!"
echo ""
echo "📊 Kiểm tra kết quả cuối cùng:"
echo "Số bảng KPI cán bộ: $(curl -s "$API_BASE/KpiAssignment/tables" | jq '[.[] | select(.Category == "CANBO")] | length')/23"
echo "Tổng số chỉ tiêu: $(curl -s "$API_BASE/KpiAssignment/indicators" | jq 'length') indicators"

echo ""
echo "🎯 Hoàn tất tạo lại 23 bảng KPI cán bộ với đầy đủ chỉ tiêu theo danh sách anh cung cấp!"
