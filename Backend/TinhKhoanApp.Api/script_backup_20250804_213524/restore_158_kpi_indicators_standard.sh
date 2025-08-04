#!/bin/bash

echo "🎯 PHỤC HỒI 158 CHỈ TIÊU KPI CHÍNH XÁC THEO DANH SÁCH ANH CUNG CẤP"
echo "==============================================================="

API_BASE="http://localhost:5055/api"

# Function tạo KPI indicator với đúng value type
create_indicator() {
    local table_id=$1
    local name=$2
    local max_score=$3
    local unit=$4
    local order_index=$5

    # Xác định value type từ unit
    local value_type=1  # NUMBER mặc định
    case "$unit" in
        "%") value_type=2 ;;     # PERCENTAGE
        "Điểm") value_type=3 ;;  # POINTS
        "Triệu VND") value_type=4 ;; # CURRENCY
        "Khách hàng") value_type=1 ;; # NUMBER
        "BT") value_type=1 ;;    # NUMBER (bút toán)
        "cái") value_type=1 ;;   # NUMBER
        *) value_type=1 ;;       # NUMBER cho các trường hợp khác
    esac

    curl -s -X POST "$API_BASE/KpiIndicators" \
        -H "Content-Type: application/json" \
        -d "{
            \"tableId\": $table_id,
            \"indicatorName\": \"$name\",
            \"maxScore\": $max_score,
            \"unit\": \"$unit\",
            \"orderIndex\": $order_index,
            \"valueType\": $value_type,
            \"isActive\": true
        }" > /dev/null
    echo "      $order_index. $name ($max_score điểm, $unit)"
}

echo "🔧 Tạo 158 chỉ tiêu KPI theo danh sách CHÍNH XÁC từ anh..."

# BẢNG 1: TruongphongKhdn (ID: 33) - 8 chỉ tiêu
echo ""
echo "📋 1. TruongphongKhdn (8 chỉ tiêu):"
create_indicator 33 "Tổng Dư nợ KHDN" 20 "Triệu VND" 1
create_indicator 33 "Tỷ lệ nợ xấu KHDN" 10 "%" 2
create_indicator 33 "Thu nợ đã XLRR KHDN" 10 "Triệu VND" 3
create_indicator 33 "Lợi nhuận khoán tài chính" 10 "Triệu VND" 4
create_indicator 33 "Phát triển Khách hàng Doanh nghiệp" 10 "Khách hàng" 5
create_indicator 33 "Điều hành theo chương trình công tác" 20 "%" 6
create_indicator 33 "Chấp hành quy chế, quy trình nghiệp vụ" 10 "%" 7
create_indicator 33 "BQ kết quả thực hiện CB trong phòng mình phụ trách" 10 "%" 8

# BẢNG 2: TruongphongKhcn (ID: 34) - 8 chỉ tiêu
echo ""
echo "📋 2. TruongphongKhcn (8 chỉ tiêu):"
create_indicator 34 "Tổng Dư nợ KHCN" 20 "Triệu VND" 1
create_indicator 34 "Tỷ lệ nợ xấu KHCN" 10 "%" 2
create_indicator 34 "Thu nợ đã XLRR KHCN" 10 "Triệu VND" 3
create_indicator 34 "Lợi nhuận khoán tài chính" 10 "Triệu VND" 4
create_indicator 34 "Phát triển Khách hàng Cá nhân" 10 "Khách hàng" 5
create_indicator 34 "Điều hành theo chương trình công tác" 20 "%" 6
create_indicator 34 "Chấp hành quy chế, quy trình nghiệp vụ" 10 "%" 7
create_indicator 34 "BQ kết quả thực hiện CB trong phòng mình phụ trách" 10 "%" 8

# BẢNG 3: PhophongKhdn (ID: 35) - 8 chỉ tiêu
echo ""
echo "📋 3. PhophongKhdn (8 chỉ tiêu):"
create_indicator 35 "Tổng Dư nợ KHDN" 20 "Triệu VND" 1
create_indicator 35 "Tỷ lệ nợ xấu KHDN" 10 "%" 2
create_indicator 35 "Thu nợ đã XLRR KHDN" 10 "Triệu VND" 3
create_indicator 35 "Lợi nhuận khoán tài chính" 10 "Triệu VND" 4
create_indicator 35 "Phát triển Khách hàng Doanh nghiệp" 10 "Khách hàng" 5
create_indicator 35 "Điều hành theo chương trình công tác" 20 "%" 6
create_indicator 35 "Chấp hành quy chế, quy trình nghiệp vụ" 10 "%" 7
create_indicator 35 "BQ kết quả thực hiện CB trong phòng mình phụ trách" 10 "%" 8

# BẢNG 4: PhophongKhcn (ID: 36) - 8 chỉ tiêu
echo ""
echo "📋 4. PhophongKhcn (8 chỉ tiêu):"
create_indicator 36 "Tổng Dư nợ KHCN" 20 "Triệu VND" 1
create_indicator 36 "Tỷ lệ nợ xấu KHCN" 10 "%" 2
create_indicator 36 "Thu nợ đã XLRR KHCN" 10 "Triệu VND" 3
create_indicator 36 "Lợi nhuận khoán tài chính" 10 "Triệu VND" 4
create_indicator 36 "Phát triển Khách hàng Cá nhân" 10 "Khách hàng" 5
create_indicator 36 "Điều hành theo chương trình công tác" 20 "%" 6
create_indicator 36 "Chấp hành quy chế, quy trình nghiệp vụ" 10 "%" 7
create_indicator 36 "BQ kết quả thực hiện CB trong phòng mình phụ trách" 10 "%" 8

# BẢNG 5: TruongphongKhqlrr (ID: 37) - 6 chỉ tiêu
echo ""
echo "📋 5. TruongphongKhqlrr (6 chỉ tiêu):"
create_indicator 37 "Tổng nguồn vốn" 10 "Triệu VND" 1
create_indicator 37 "Tổng dư nợ" 10 "Triệu VND" 2
create_indicator 37 "Lợi nhuận khoán tài chính" 10 "Triệu VND" 3
create_indicator 37 "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng" 50 "%" 4
create_indicator 37 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 5
create_indicator 37 "Kết quả thực hiện BQ của CB trong phòng" 10 "%" 6

# BẢNG 6: PhophongKhqlrr (ID: 38) - 6 chỉ tiêu
echo ""
echo "📋 6. PhophongKhqlrr (6 chỉ tiêu):"
create_indicator 38 "Tổng nguồn vốn" 10 "Triệu VND" 1
create_indicator 38 "Tổng dư nợ" 10 "Triệu VND" 2
create_indicator 38 "Lợi nhuận khoán tài chính" 10 "Triệu VND" 3
create_indicator 38 "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng" 50 "%" 4
create_indicator 38 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 5
create_indicator 38 "Kết quả thực hiện BQ của CB trong phòng mình phụ trách" 10 "%" 6

# BẢNG 7: Cbtd (ID: 39) - 8 chỉ tiêu
echo ""
echo "📋 7. Cbtd (8 chỉ tiêu):"
create_indicator 39 "Tổng dư nợ BQ" 30 "Triệu VND" 1
create_indicator 39 "Tỷ lệ nợ xấu" 15 "%" 2
create_indicator 39 "Phát triển Khách hàng" 10 "Khách hàng" 3
create_indicator 39 "Thu nợ đã XLRR (nếu không có nợ XLRR thì cộng vào chỉ tiêu Dư nợ)" 10 "Triệu VND" 4
create_indicator 39 "Thực hiện nhiệm vụ theo chương trình công tác" 10 "%" 5
create_indicator 39 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 6
create_indicator 39 "Tổng nguồn vốn huy động BQ" 10 "Triệu VND" 7
create_indicator 39 "Hoàn thành chỉ tiêu giao khoán SPDV" 5 "%" 8

# BẢNG 8: TruongphongKtnqCnl1 (ID: 40) - 6 chỉ tiêu
echo ""
echo "📋 8. TruongphongKtnqCnl1 (6 chỉ tiêu):"
create_indicator 40 "Tổng nguồn vốn" 10 "Triệu VND" 1
create_indicator 40 "Lợi nhuận khoán tài chính" 20 "Triệu VND" 2
create_indicator 40 "Thu dịch vụ thanh toán trong nước" 10 "Triệu VND" 3
create_indicator 40 "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng" 40 "%" 4
create_indicator 40 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 5
create_indicator 40 "Kết quả thực hiện BQ của CB trong phòng" 10 "%" 6

# BẢNG 9: PhophongKtnqCnl1 (ID: 41) - 6 chỉ tiêu
echo ""
echo "📋 9. PhophongKtnqCnl1 (6 chỉ tiêu):"
create_indicator 41 "Tổng nguồn vốn" 10 "Triệu VND" 1
create_indicator 41 "Lợi nhuận khoán tài chính" 20 "Triệu VND" 2
create_indicator 41 "Thu dịch vụ thanh toán trong nước" 10 "Triệu VND" 3
create_indicator 41 "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng" 40 "%" 4
create_indicator 41 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 5
create_indicator 41 "Kết quả thực hiện BQ của CB thuộc mình phụ trách" 10 "%" 6

# BẢNG 10: Gdv (ID: 42) - 6 chỉ tiêu
echo ""
echo "📋 10. Gdv (6 chỉ tiêu):"
create_indicator 42 "Số bút toán giao dịch BQ" 50 "BT" 1
create_indicator 42 "Số bút toán hủy" 15 "BT" 2
create_indicator 42 "Thực hiện chức năng, nhiệm vụ được giao" 10 "%" 3
create_indicator 42 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 4
create_indicator 42 "Tổng nguồn vốn huy động BQ" 10 "Triệu VND" 5
create_indicator 42 "Hoàn thành chỉ tiêu giao khoán SPDV" 5 "%" 6

# BẢNG 11: TqHkKtnb (ID: 43) - 0 chỉ tiêu (theo anh chưa có cụ thể)
echo ""
echo "📋 11. TqHkKtnb - Đợi TP KTNQ/Giám đốc CN loại 2 trực tiếp giao sau (chưa có cụ thể)"

# BẢNG 12: TruongphoItThKtgs (ID: 44) - 5 chỉ tiêu
echo ""
echo "📋 12. TruongphoItThKtgs (5 chỉ tiêu):"
create_indicator 44 "Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng" 65 "%" 1
create_indicator 44 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 2
create_indicator 44 "Tổng nguồn vốn huy động BQ" 10 "Triệu VND" 3
create_indicator 44 "Hoàn thành chỉ tiêu giao khoán SPDV" 5 "%" 4
create_indicator 44 "Kết quả thực hiện BQ của cán bộ trong phòng" 10 "%" 5

# BẢNG 13: CBItThKtgsKhqlrr (ID: 45) - 4 chỉ tiêu
echo ""
echo "📋 13. CBItThKtgsKhqlrr (4 chỉ tiêu):"
create_indicator 45 "Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ được giao" 75 "%" 1
create_indicator 45 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 2
create_indicator 45 "Tổng nguồn vốn huy động BQ" 10 "Triệu VND" 3
create_indicator 45 "Hoàn thành chỉ tiêu giao khoán SPDV" 5 "%" 4

# BẢNG 14: GiamdocPgd (ID: 46) - 9 chỉ tiêu
echo ""
echo "📋 14. GiamdocPgd (9 chỉ tiêu):"
create_indicator 46 "Tổng nguồn vốn BQ" 15 "Triệu VND" 1
create_indicator 46 "Tổng dư nợ BQ" 15 "Triệu VND" 2
create_indicator 46 "Tỷ lệ nợ xấu" 10 "%" 3
create_indicator 46 "Phát triển Khách hàng" 10 "Khách hàng" 4
create_indicator 46 "Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)" 5 "Triệu VND" 5
create_indicator 46 "Thu dịch vụ" 10 "Triệu VND" 6
create_indicator 46 "Lợi nhuận khoán tài chính" 15 "Triệu VND" 7
create_indicator 46 "Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 8
create_indicator 46 "BQ kết quả thực hiện của CB trong phòng" 10 "%" 9

# BẢNG 15: PhogiamdocPgd (ID: 47) - 9 chỉ tiêu
echo ""
echo "📋 15. PhogiamdocPgd (9 chỉ tiêu):"
create_indicator 47 "Tổng nguồn vốn BQ" 15 "Triệu VND" 1
create_indicator 47 "Tổng dư nợ BQ" 15 "Triệu VND" 2
create_indicator 47 "Tỷ lệ nợ xấu" 10 "%" 3
create_indicator 47 "Phát triển Khách hàng" 10 "Khách hàng" 4
create_indicator 47 "Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)" 5 "Triệu VND" 5
create_indicator 47 "Thu dịch vụ" 10 "Triệu VND" 6
create_indicator 47 "Lợi nhuận khoán tài chính" 15 "Triệu VND" 7
create_indicator 47 "Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 8
create_indicator 47 "BQ kết quả thực hiện của CB trong phòng" 10 "%" 9

# BẢNG 16: PhogiamdocPgdCbtd (ID: 48) - 8 chỉ tiêu
echo ""
echo "📋 16. PhogiamdocPgdCbtd (8 chỉ tiêu):"
create_indicator 48 "Tổng dư nợ BQ" 30 "Triệu VND" 1
create_indicator 48 "Tỷ lệ nợ xấu" 15 "%" 2
create_indicator 48 "Phát triển Khách hàng" 10 "Khách hàng" 3
create_indicator 48 "Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)" 10 "Triệu VND" 4
create_indicator 48 "Thực hiện nhiệm vụ theo chương trình công tác" 10 "%" 5
create_indicator 48 "Tổng nguồn vốn huy động BQ" 10 "Triệu VND" 6
create_indicator 48 "Hoàn thành chỉ tiêu giao khoán SPDV" 5 "%" 7
create_indicator 48 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 8

# BẢNG 17: GiamdocCnl2 (ID: 49) - 11 chỉ tiêu
echo ""
echo "📋 17. GiamdocCnl2 (11 chỉ tiêu):"
create_indicator 49 "Tổng nguồn vốn cuối kỳ" 5 "Triệu VND" 1
create_indicator 49 "Tổng nguồn vốn huy động BQ trong kỳ" 10 "Triệu VND" 2
create_indicator 49 "Tổng dư nợ cuối kỳ" 5 "Triệu VND" 3
create_indicator 49 "Tổng dư nợ BQ trong kỳ" 10 "Triệu VND" 4
create_indicator 49 "Tổng dư nợ HSX&CN" 5 "Triệu VND" 5
create_indicator 49 "Tỷ lệ nợ xấu nội bảng" 10 "%" 6
create_indicator 49 "Thu nợ đã XLRR" 5 "Triệu VND" 7
create_indicator 49 "Phát triển Khách hàng" 10 "Khách hàng" 8
create_indicator 49 "Lợi nhuận khoán tài chính" 20 "Triệu VND" 9
create_indicator 49 "Thu dịch vụ" 10 "Triệu VND" 10
create_indicator 49 "Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank" 10 "%" 11

# BẢNG 18: PhogiamdocCnl2Td (ID: 50) - 8 chỉ tiêu
echo ""
echo "📋 18. PhogiamdocCnl2Td (8 chỉ tiêu):"
create_indicator 50 "Tổng dư nợ cho vay" 20 "Triệu VND" 1
create_indicator 50 "Tổng dư nợ cho vay HSX&CN" 10 "Triệu VND" 2
create_indicator 50 "Thu nợ đã xử lý" 10 "Triệu VND" 3
create_indicator 50 "Lợi nhuận khoán tài chính" 20 "Triệu VND" 4
create_indicator 50 "Tỷ lệ nợ xấu" 10 "%" 5
create_indicator 50 "Phát triển Khách hàng" 10 "Khách hàng" 6
create_indicator 50 "Điều hành theo chương trình công tác, nhiệm vụ được giao" 10 "%" 7
create_indicator 50 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 8

# BẢNG 19: PhogiamdocCnl2Kt (ID: 51) - 6 chỉ tiêu
echo ""
echo "📋 19. PhogiamdocCnl2Kt (6 chỉ tiêu):"
create_indicator 51 "Tổng nguồn vốn" 20 "Triệu VND" 1
create_indicator 51 "Lợi nhuận khoán tài chính" 30 "Triệu VND" 2
create_indicator 51 "Tổng doanh thu phí dịch vụ" 20 "Triệu VND" 3
create_indicator 51 "Số thẻ phát hành" 10 "cái" 4
create_indicator 51 "Điều hành theo chương trình công tác, nhiệm vụ được giao" 10 "%" 5
create_indicator 51 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 6

# BẢNG 20: TruongphongKhCnl2 (ID: 52) - 9 chỉ tiêu
echo ""
echo "📋 20. TruongphongKhCnl2 (9 chỉ tiêu):"
create_indicator 52 "Tổng dư nợ" 20 "Triệu VND" 1
create_indicator 52 "Tỷ lệ nợ xấu" 15 "%" 2
create_indicator 52 "Phát triển Khách hàng" 10 "Khách hàng" 3
create_indicator 52 "Thu nợ đã XLRR" 10 "Triệu VND" 4
create_indicator 52 "Điều hành theo chương trình công tác" 10 "%" 5
create_indicator 52 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 6
create_indicator 52 "Tổng nguồn vốn huy động BQ" 10 "Triệu VND" 7
create_indicator 52 "Hoàn thành chỉ tiêu giao khoán SPDV" 5 "%" 8
create_indicator 52 "Kết quả thực hiện BQ của CB trong phòng" 10 "%" 9

# BẢNG 21: PhophongKhCnl2 (ID: 53) - 8 chỉ tiêu
echo ""
echo "📋 21. PhophongKhCnl2 (8 chỉ tiêu):"
create_indicator 53 "Tổng dư nợ BQ" 30 "Triệu VND" 1
create_indicator 53 "Tỷ lệ nợ xấu" 15 "%" 2
create_indicator 53 "Phát triển Khách hàng" 10 "Khách hàng" 3
create_indicator 53 "Thu nợ đã XLRR" 10 "Triệu VND" 4
create_indicator 53 "Thực hiện nhiệm vụ theo chương trình công tác" 10 "%" 5
create_indicator 53 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 6
create_indicator 53 "Tổng nguồn vốn huy động BQ" 10 "Triệu VND" 7
create_indicator 53 "Hoàn thành chỉ tiêu giao khoán SPDV" 5 "%" 8

# BẢNG 22: TruongphongKtnqCnl2 (ID: 54) - 6 chỉ tiêu
echo ""
echo "📋 22. TruongphongKtnqCnl2 (6 chỉ tiêu):"
create_indicator 54 "Tổng nguồn vốn" 10 "Triệu VND" 1
create_indicator 54 "Lợi nhuận khoán tài chính" 20 "Triệu VND" 2
create_indicator 54 "Thu dịch vụ thanh toán trong nước" 10 "Triệu VND" 3
create_indicator 54 "Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng" 40 "%" 4
create_indicator 54 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 5
create_indicator 54 "Kết quả thực hiện BQ của CB trong phòng" 10 "%" 6

# BẢNG 23: PhophongKtnqCnl2 (ID: 55) - 5 chỉ tiêu
echo ""
echo "📋 23. PhophongKtnqCnl2 (5 chỉ tiêu):"
create_indicator 55 "Số bút toán giao dịch BQ" 40 "BT" 1
create_indicator 55 "Số bút toán hủy" 20 "BT" 2
create_indicator 55 "Thực hiện nhiệm vụ theo chương trình công tác" 25 "%" 3
create_indicator 55 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 4
create_indicator 55 "Hoàn thành chỉ tiêu giao khoán SPDV" 5 "%" 5

echo ""
echo "✅ HOÀN THÀNH PHỤC HỒI 158 CHỈ TIÊU KPI CHÍNH XÁC!"
echo "=============================================="

# Verification
total_indicators=$(curl -s http://localhost:5055/api/KpiIndicators | jq '. | length')
echo "📊 Tổng số KPI Indicators: $total_indicators/158"

echo ""
echo "🎯 PHÂN BỐ THEO BẢNG KPI (CHÍNH XÁC):"
echo "====================================="
echo "1. TruongphongKhdn: 8 chỉ tiêu"
echo "2. TruongphongKhcn: 8 chỉ tiêu"
echo "3. PhophongKhdn: 8 chỉ tiêu"
echo "4. PhophongKhcn: 8 chỉ tiêu"
echo "5. TruongphongKhqlrr: 6 chỉ tiêu"
echo "6. PhophongKhqlrr: 6 chỉ tiêu"
echo "7. Cbtd: 8 chỉ tiêu"
echo "8. TruongphongKtnqCnl1: 6 chỉ tiêu"
echo "9. PhophongKtnqCnl1: 6 chỉ tiêu"
echo "10. Gdv: 6 chỉ tiêu"
echo "11. TqHkKtnb: 0 chỉ tiêu (đợi giao sau)"
echo "12. TruongphoItThKtgs: 5 chỉ tiêu"
echo "13. CBItThKtgsKhqlrr: 4 chỉ tiêu"
echo "14. GiamdocPgd: 9 chỉ tiêu"
echo "15. PhogiamdocPgd: 9 chỉ tiêu"
echo "16. PhogiamdocPgdCbtd: 8 chỉ tiêu"
echo "17. GiamdocCnl2: 11 chỉ tiêu"
echo "18. PhogiamdocCnl2Td: 8 chỉ tiêu"
echo "19. PhogiamdocCnl2Kt: 6 chỉ tiêu"
echo "20. TruongphongKhCnl2: 9 chỉ tiêu"
echo "21. PhophongKhCnl2: 8 chỉ tiêu"
echo "22. TruongphongKtnqCnl2: 6 chỉ tiêu"
echo "23. PhophongKtnqCnl2: 5 chỉ tiêu"
echo "────────────────────────────────────────────"
echo "TỔNG: 158 chỉ tiêu cho 23 bảng cán bộ ✅"
echo "(Ghi chú: TqHkKtnb chưa có chỉ tiêu cụ thể)"
echo ""

if [ "$total_indicators" = "158" ]; then
    echo "🎉 THÀNH CÔNG! Đã phục hồi đúng 158 chỉ tiêu KPI chính xác từ anh!"
else
    echo "⚠️ Chỉ có $total_indicators/158 chỉ tiêu. Cần kiểm tra lại!"
fi

echo ""
echo "🚀 CÁC BƯỚC TIẾP THEO:"
echo "====================="
echo "1. ✅ Kiểm tra frontend dropdown hiển thị đúng tên bảng KPI"
echo "2. 🔄 Tạo Employee KPI Assignments"
echo "3. 🔄 Tạo Unit KPI Scorings cho chi nhánh"
echo "4. 🔄 Import dữ liệu CSV cho 8 bảng core"
echo "5. 🔄 Tạo thêm roles/units nếu cần"
