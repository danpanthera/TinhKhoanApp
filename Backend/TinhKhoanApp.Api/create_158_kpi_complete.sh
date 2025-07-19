#!/bin/bash

echo "🎯 TẠO HOÀN CHỈNH 158 CHỈ TIÊU KPI CHO 23 BẢNG CÁN BỘ"
echo "===================================================="

API_BASE="http://localhost:5055/api"

# Function tạo KPI indicator
create_kpi() {
    local table_id=$1
    local name=$2
    local max_score=$3
    local unit=$4
    local order_index=$5

    # Xác định value type từ unit
    local value_type=1  # NUMBER
    case "$unit" in
        "%") value_type=2 ;;         # PERCENTAGE
        "Điểm") value_type=3 ;;      # POINTS
        "Triệu VND") value_type=4 ;; # CURRENCY
        *) value_type=1 ;;           # NUMBER
    esac

    http_code=$(curl -s -w "%{http_code}" -o /dev/null -X POST "$API_BASE/KpiIndicators/CreateFromDto" \
        -H "Content-Type: application/json" \
        -d "{
            \"tableId\": $table_id,
            \"indicatorName\": \"$name\",
            \"maxScore\": $max_score,
            \"unit\": \"$unit\",
            \"orderIndex\": $order_index,
            \"valueType\": $value_type,
            \"isActive\": true
        }")

    if [[ "$http_code" == "201" ]]; then
        echo "      ✅ $order_index. $name ($max_score điểm, $unit)"
        return 0
    else
        echo "      ❌ $order_index. $name - HTTP $http_code"
        return 1
    fi
}

# Kiểm tra API
echo "🔧 Kiểm tra kết nối API..."
response=$(curl -s -w "%{http_code}" "$API_BASE/KpiAssignmentTables")
if [[ "${response: -3}" == "200" ]]; then
    echo "✅ API đã sẵn sàng"
else
    echo "❌ API không phản hồi. Đảm bảo backend đang chạy trên localhost:5055"
    exit 1
fi

# Xóa tất cả KPI indicators cũ
echo ""
echo "🗑️  Xóa tất cả KPI indicators cũ..."
curl -s -X DELETE "$API_BASE/KpiIndicators/DeleteAll" > /dev/null

success_count=0

# BẢNG 1: TruongphongKhdn (ID: 1) - 8 chỉ tiêu
echo ""
echo "📋 1. TruongphongKhdn (8 chỉ tiêu):"
create_kpi 1 "Tổng Dư nợ KHDN" 20 "Triệu VND" 1 && ((success_count++))
create_kpi 1 "Tỷ lệ nợ xấu KHDN" 10 "%" 2 && ((success_count++))
create_kpi 1 "Thu nợ đã XLRR KHDN" 10 "Triệu VND" 3 && ((success_count++))
create_kpi 1 "Lợi nhuận khoán tài chính" 10 "Triệu VND" 4 && ((success_count++))
create_kpi 1 "Phát triển Khách hàng Doanh nghiệp" 10 "Khách hàng" 5 && ((success_count++))
create_kpi 1 "Điều hành theo chương trình công tác" 20 "%" 6 && ((success_count++))
create_kpi 1 "Chấp hành quy chế, quy trình nghiệp vụ" 10 "%" 7 && ((success_count++))
create_kpi 1 "BQ kết quả thực hiện CB trong phòng mình phụ trách" 10 "%" 8 && ((success_count++))

# BẢNG 2: TruongphongKhcn (ID: 2) - 8 chỉ tiêu
echo ""
echo "📋 2. TruongphongKhcn (8 chỉ tiêu):"
create_kpi 2 "Tổng Dư nợ KHCN" 20 "Triệu VND" 1 && ((success_count++))
create_kpi 2 "Tỷ lệ nợ xấu KHCN" 10 "%" 2 && ((success_count++))
create_kpi 2 "Thu nợ đã XLRR KHCN" 10 "Triệu VND" 3 && ((success_count++))
create_kpi 2 "Lợi nhuận khoán tài chính" 10 "Triệu VND" 4 && ((success_count++))
create_kpi 2 "Phát triển Khách hàng Cá nhân" 10 "Khách hàng" 5 && ((success_count++))
create_kpi 2 "Điều hành theo chương trình công tác" 20 "%" 6 && ((success_count++))
create_kpi 2 "Chấp hành quy chế, quy trình nghiệp vụ" 10 "%" 7 && ((success_count++))
create_kpi 2 "BQ kết quả thực hiện CB trong phòng mình phụ trách" 10 "%" 8 && ((success_count++))

# BẢNG 3: PhophongKhdn (ID: 3) - 8 chỉ tiêu
echo ""
echo "📋 3. PhophongKhdn (8 chỉ tiêu):"
create_kpi 3 "Tổng Dư nợ KHDN" 20 "Triệu VND" 1 && ((success_count++))
create_kpi 3 "Tỷ lệ nợ xấu KHDN" 10 "%" 2 && ((success_count++))
create_kpi 3 "Thu nợ đã XLRR KHDN" 10 "Triệu VND" 3 && ((success_count++))
create_kpi 3 "Lợi nhuận khoán tài chính" 10 "Triệu VND" 4 && ((success_count++))
create_kpi 3 "Phát triển Khách hàng Doanh nghiệp" 10 "Khách hàng" 5 && ((success_count++))
create_kpi 3 "Điều hành theo chương trình công tác" 20 "%" 6 && ((success_count++))
create_kpi 3 "Chấp hành quy chế, quy trình nghiệp vụ" 10 "%" 7 && ((success_count++))
create_kpi 3 "BQ kết quả thực hiện CB trong phòng mình phụ trách" 10 "%" 8 && ((success_count++))

# BẢNG 4: PhophongKhcn (ID: 4) - 8 chỉ tiêu
echo ""
echo "📋 4. PhophongKhcn (8 chỉ tiêu):"
create_kpi 4 "Tổng Dư nợ KHCN" 20 "Triệu VND" 1 && ((success_count++))
create_kpi 4 "Tỷ lệ nợ xấu KHCN" 10 "%" 2 && ((success_count++))
create_kpi 4 "Thu nợ đã XLRR KHCN" 10 "Triệu VND" 3 && ((success_count++))
create_kpi 4 "Lợi nhuận khoán tài chính" 10 "Triệu VND" 4 && ((success_count++))
create_kpi 4 "Phát triển Khách hàng Cá nhân" 10 "Khách hàng" 5 && ((success_count++))
create_kpi 4 "Điều hành theo chương trình công tác" 20 "%" 6 && ((success_count++))
create_kpi 4 "Chấp hành quy chế, quy trình nghiệp vụ" 10 "%" 7 && ((success_count++))
create_kpi 4 "BQ kết quả thực hiện CB trong phòng mình phụ trách" 10 "%" 8 && ((success_count++))

# BẢNG 5: TruongphongKhqlrr (ID: 5) - 6 chỉ tiêu
echo ""
echo "📋 5. TruongphongKhqlrr (6 chỉ tiêu):"
create_kpi 5 "Tổng nguồn vốn" 20 "Triệu VND" 1 && ((success_count++))
create_kpi 5 "Tổng dư nợ" 20 "Triệu VND" 2 && ((success_count++))
create_kpi 5 "Lợi nhuận khoán tài chính" 15 "Triệu VND" 3 && ((success_count++))
create_kpi 5 "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng" 20 "%" 4 && ((success_count++))
create_kpi 5 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 15 "%" 5 && ((success_count++))
create_kpi 5 "Kết quả thực hiện BQ của CB trong phòng" 10 "%" 6 && ((success_count++))

# BẢNG 6: PhophongKhqlrr (ID: 6) - 6 chỉ tiêu
echo ""
echo "📋 6. PhophongKhqlrr (6 chỉ tiêu):"
create_kpi 6 "Tổng nguồn vốn" 20 "Triệu VND" 1 && ((success_count++))
create_kpi 6 "Tổng dư nợ" 20 "Triệu VND" 2 && ((success_count++))
create_kpi 6 "Lợi nhuận khoán tài chính" 15 "Triệu VND" 3 && ((success_count++))
create_kpi 6 "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng" 20 "%" 4 && ((success_count++))
create_kpi 6 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 15 "%" 5 && ((success_count++))
create_kpi 6 "Kết quả thực hiện BQ của CB trong phòng mình phụ trách" 10 "%" 6 && ((success_count++))

# BẢNG 7: Cbtd (ID: 7) - 8 chỉ tiêu
echo ""
echo "📋 7. Cbtd (8 chỉ tiêu):"
create_kpi 7 "Tổng dư nợ BQ" 20 "Triệu VND" 1 && ((success_count++))
create_kpi 7 "Tỷ lệ nợ xấu" 10 "%" 2 && ((success_count++))
create_kpi 7 "Phát triển Khách hàng" 10 "Khách hàng" 3 && ((success_count++))
create_kpi 7 "Thu nợ đã XLRR (nếu không có nợ XLRR thì cộng vào chỉ tiêu Dư nợ)" 10 "Triệu VND" 4 && ((success_count++))
create_kpi 7 "Thực hiện nhiệm vụ theo chương trình công tác" 20 "%" 5 && ((success_count++))
create_kpi 7 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 6 && ((success_count++))
create_kpi 7 "Tổng nguồn vốn huy động BQ" 10 "Triệu VND" 7 && ((success_count++))
create_kpi 7 "Hoàn thành chỉ tiêu giao khoán SPDV" 10 "%" 8 && ((success_count++))

# BẢNG 8: TruongphongKtnqCnl1 (ID: 8) - 6 chỉ tiêu
echo ""
echo "📋 8. TruongphongKtnqCnl1 (6 chỉ tiêu):"
create_kpi 8 "Tổng nguồn vốn" 20 "Triệu VND" 1 && ((success_count++))
create_kpi 8 "Lợi nhuận khoán tài chính" 20 "Triệu VND" 2 && ((success_count++))
create_kpi 8 "Thu dịch vụ thanh toán trong nước" 15 "Triệu VND" 3 && ((success_count++))
create_kpi 8 "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng" 20 "%" 4 && ((success_count++))
create_kpi 8 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 15 "%" 5 && ((success_count++))
create_kpi 8 "Kết quả thực hiện BQ của CB trong phòng" 10 "%" 6 && ((success_count++))

# BẢNG 9: PhophongKtnqCnl1 (ID: 9) - 6 chỉ tiêu
echo ""
echo "📋 9. PhophongKtnqCnl1 (6 chỉ tiêu):"
create_kpi 9 "Tổng nguồn vốn" 20 "Triệu VND" 1 && ((success_count++))
create_kpi 9 "Lợi nhuận khoán tài chính" 20 "Triệu VND" 2 && ((success_count++))
create_kpi 9 "Thu dịch vụ thanh toán trong nước" 15 "Triệu VND" 3 && ((success_count++))
create_kpi 9 "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng" 20 "%" 4 && ((success_count++))
create_kpi 9 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 15 "%" 5 && ((success_count++))
create_kpi 9 "Kết quả thực hiện BQ của CB thuộc mình phụ trách" 10 "%" 6 && ((success_count++))

# BẢNG 10: Gdv (ID: 10) - 6 chỉ tiêu
echo ""
echo "📋 10. Gdv (6 chỉ tiêu):"
create_kpi 10 "Số bút toán giao dịch BQ" 20 "BT" 1 && ((success_count++))
create_kpi 10 "Số bút toán hủy" 10 "BT" 2 && ((success_count++))
create_kpi 10 "Thực hiện chức năng, nhiệm vụ được giao" 25 "%" 3 && ((success_count++))
create_kpi 10 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 15 "%" 4 && ((success_count++))
create_kpi 10 "Tổng nguồn vốn huy động BQ" 15 "Triệu VND" 5 && ((success_count++))
create_kpi 10 "Hoàn thành chỉ tiêu giao khoán SPDV" 15 "%" 6 && ((success_count++))

# BẢNG 11: TqHkKtnb (ID: 11) - Skip (đợi giao sau)
echo ""
echo "📋 11. TqHkKtnb - ⏳ Đợi TP KTNQ/Giám đốc CN loại 2 trực tiếp giao sau"

# BẢNG 12: TruongphoItThKtgs (ID: 12) - 5 chỉ tiêu
echo ""
echo "📋 12. TruongphoItThKtgs (5 chỉ tiêu):"
create_kpi 12 "Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng" 30 "%" 1 && ((success_count++))
create_kpi 12 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 20 "%" 2 && ((success_count++))
create_kpi 12 "Tổng nguồn vốn huy động BQ" 20 "Triệu VND" 3 && ((success_count++))
create_kpi 12 "Hoàn thành chỉ tiêu giao khoán SPDV" 20 "%" 4 && ((success_count++))
create_kpi 12 "Kết quả thực hiện BQ của cán bộ trong phòng" 10 "%" 5 && ((success_count++))

# BẢNG 13: CBItThKtgsKhqlrr (ID: 13) - 4 chỉ tiêu
echo ""
echo "📋 13. CBItThKtgsKhqlrr (4 chỉ tiêu):"
create_kpi 13 "Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ được giao" 40 "%" 1 && ((success_count++))
create_kpi 13 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 25 "%" 2 && ((success_count++))
create_kpi 13 "Tổng nguồn vốn huy động BQ" 20 "Triệu VND" 3 && ((success_count++))
create_kpi 13 "Hoàn thành chỉ tiêu giao khoán SPDV" 15 "%" 4 && ((success_count++))

# BẢNG 14: GiamdocPgd (ID: 14) - 9 chỉ tiêu
echo ""
echo "📋 14. GiamdocPgd (9 chỉ tiêu):"
create_kpi 14 "Tổng nguồn vốn BQ" 15 "Triệu VND" 1 && ((success_count++))
create_kpi 14 "Tổng dư nợ BQ" 15 "Triệu VND" 2 && ((success_count++))
create_kpi 14 "Tỷ lệ nợ xấu" 10 "%" 3 && ((success_count++))
create_kpi 14 "Phát triển Khách hàng" 10 "Khách hàng" 4 && ((success_count++))
create_kpi 14 "Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)" 10 "Triệu VND" 5 && ((success_count++))
create_kpi 14 "Thu dịch vụ" 10 "Triệu VND" 6 && ((success_count++))
create_kpi 14 "Lợi nhuận khoán tài chính" 10 "Triệu VND" 7 && ((success_count++))
create_kpi 14 "Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 20 "%" 8 && ((success_count++))
create_kpi 14 "BQ kết quả thực hiện của CB trong phòng" 10 "%" 9 && ((success_count++))

# BẢNG 15: PhogiamdocPgd (ID: 15) - 9 chỉ tiêu
echo ""
echo "📋 15. PhogiamdocPgd (9 chỉ tiêu):"
create_kpi 15 "Tổng nguồn vốn BQ" 15 "Triệu VND" 1 && ((success_count++))
create_kpi 15 "Tổng dư nợ BQ" 15 "Triệu VND" 2 && ((success_count++))
create_kpi 15 "Tỷ lệ nợ xấu" 10 "%" 3 && ((success_count++))
create_kpi 15 "Phát triển Khách hàng" 10 "Khách hàng" 4 && ((success_count++))
create_kpi 15 "Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)" 10 "Triệu VND" 5 && ((success_count++))
create_kpi 15 "Thu dịch vụ" 10 "Triệu VND" 6 && ((success_count++))
create_kpi 15 "Lợi nhuận khoán tài chính" 10 "Triệu VND" 7 && ((success_count++))
create_kpi 15 "Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 20 "%" 8 && ((success_count++))
create_kpi 15 "BQ kết quả thực hiện của CB trong phòng" 10 "%" 9 && ((success_count++))

# BẢNG 16: PhogiamdocPgdCbtd (ID: 16) - 8 chỉ tiêu
echo ""
echo "📋 16. PhogiamdocPgdCbtd (8 chỉ tiêu):"
create_kpi 16 "Tổng dư nợ BQ" 20 "Triệu VND" 1 && ((success_count++))
create_kpi 16 "Tỷ lệ nợ xấu" 10 "%" 2 && ((success_count++))
create_kpi 16 "Phát triển Khách hàng" 10 "Khách hàng" 3 && ((success_count++))
create_kpi 16 "Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)" 10 "Triệu VND" 4 && ((success_count++))
create_kpi 16 "Thực hiện nhiệm vụ theo chương trình công tác" 20 "%" 5 && ((success_count++))
create_kpi 16 "Tổng nguồn vốn huy động BQ" 10 "Triệu VND" 6 && ((success_count++))
create_kpi 16 "Hoàn thành chỉ tiêu giao khoán SPDV" 10 "%" 7 && ((success_count++))
create_kpi 16 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 8 && ((success_count++))

# BẢNG 17: GiamdocCnl2 (ID: 17) - 11 chỉ tiêu
echo ""
echo "📋 17. GiamdocCnl2 (11 chỉ tiêu):"
create_kpi 17 "Tổng nguồn vốn cuối kỳ" 10 "Triệu VND" 1 && ((success_count++))
create_kpi 17 "Tổng nguồn vốn huy động BQ trong kỳ" 10 "Triệu VND" 2 && ((success_count++))
create_kpi 17 "Tổng dư nợ cuối kỳ" 10 "Triệu VND" 3 && ((success_count++))
create_kpi 17 "Tổng dư nợ BQ trong kỳ" 10 "Triệu VND" 4 && ((success_count++))
create_kpi 17 "Tổng dư nợ HSX&CN" 10 "Triệu VND" 5 && ((success_count++))
create_kpi 17 "Tỷ lệ nợ xấu nội bảng" 10 "%" 6 && ((success_count++))
create_kpi 17 "Thu nợ đã XLRR" 5 "Triệu VND" 7 && ((success_count++))
create_kpi 17 "Phát triển Khách hàng" 5 "Khách hàng" 8 && ((success_count++))
create_kpi 17 "Lợi nhuận khoán tài chính" 10 "Triệu VND" 9 && ((success_count++))
create_kpi 17 "Thu dịch vụ" 10 "Triệu VND" 10 && ((success_count++))
create_kpi 17 "Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank" 20 "%" 11 && ((success_count++))

# BẢNG 18: PhogiamdocCnl2Td (ID: 18) - 8 chỉ tiêu
echo ""
echo "📋 18. PhogiamdocCnl2Td (8 chỉ tiêu):"
create_kpi 18 "Tổng dư nợ cho vay" 20 "Triệu VND" 1 && ((success_count++))
create_kpi 18 "Tổng dư nợ cho vay HSX&CN" 15 "Triệu VND" 2 && ((success_count++))
create_kpi 18 "Thu nợ đã xử lý" 10 "Triệu VND" 3 && ((success_count++))
create_kpi 18 "Lợi nhuận khoán tài chính" 15 "Triệu VND" 4 && ((success_count++))
create_kpi 18 "Tỷ lệ nợ xấu" 10 "%" 5 && ((success_count++))
create_kpi 18 "Phát triển Khách hàng" 10 "Khách hàng" 6 && ((success_count++))
create_kpi 18 "Điều hành theo chương trình công tác, nhiệm vụ được giao" 15 "%" 7 && ((success_count++))
create_kpi 18 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 15 "%" 8 && ((success_count++))

# BẢNG 19: PhogiamdocCnl2Kt (ID: 19) - 6 chỉ tiêu
echo ""
echo "📋 19. PhogiamdocCnl2Kt (6 chỉ tiêu):"
create_kpi 19 "Tổng nguồn vốn" 25 "Triệu VND" 1 && ((success_count++))
create_kpi 19 "Lợi nhuận khoán tài chính" 20 "Triệu VND" 2 && ((success_count++))
create_kpi 19 "Tổng doanh thu phí dịch vụ" 15 "Triệu VND" 3 && ((success_count++))
create_kpi 19 "Số thẻ phát hành" 10 "cái" 4 && ((success_count++))
create_kpi 19 "Điều hành theo chương trình công tác, nhiệm vụ được giao" 15 "%" 5 && ((success_count++))
create_kpi 19 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 15 "%" 6 && ((success_count++))

# BẢNG 20: TruongphongKhCnl2 (ID: 20) - 9 chỉ tiêu
echo ""
echo "📋 20. TruongphongKhCnl2 (9 chỉ tiêu):"
create_kpi 20 "Tổng dư nợ" 20 "Triệu VND" 1 && ((success_count++))
create_kpi 20 "Tỷ lệ nợ xấu" 10 "%" 2 && ((success_count++))
create_kpi 20 "Phát triển Khách hàng" 10 "Khách hàng" 3 && ((success_count++))
create_kpi 20 "Thu nợ đã XLRR" 10 "Triệu VND" 4 && ((success_count++))
create_kpi 20 "Điều hành theo chương trình công tác" 15 "%" 5 && ((success_count++))
create_kpi 20 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 6 && ((success_count++))
create_kpi 20 "Tổng nguồn vốn huy động BQ" 10 "Triệu VND" 7 && ((success_count++))
create_kpi 20 "Hoàn thành chỉ tiêu giao khoán SPDV" 10 "%" 8 && ((success_count++))
create_kpi 20 "Kết quả thực hiện BQ của CB trong phòng" 5 "%" 9 && ((success_count++))

# BẢNG 21: PhophongKhCnl2 (ID: 21) - 8 chỉ tiêu
echo ""
echo "📋 21. PhophongKhCnl2 (8 chỉ tiêu):"
create_kpi 21 "Tổng dư nợ BQ" 20 "Triệu VND" 1 && ((success_count++))
create_kpi 21 "Tỷ lệ nợ xấu" 10 "%" 2 && ((success_count++))
create_kpi 21 "Phát triển Khách hàng" 10 "Khách hàng" 3 && ((success_count++))
create_kpi 21 "Thu nợ đã XLRR" 10 "Triệu VND" 4 && ((success_count++))
create_kpi 21 "Thực hiện nhiệm vụ theo chương trình công tác" 20 "%" 5 && ((success_count++))
create_kpi 21 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 6 && ((success_count++))
create_kpi 21 "Tổng nguồn vốn huy động BQ" 10 "Triệu VND" 7 && ((success_count++))
create_kpi 21 "Hoàn thành chỉ tiêu giao khoán SPDV" 10 "%" 8 && ((success_count++))

# BẢNG 22: TruongphongKtnqCnl2 (ID: 22) - 6 chỉ tiêu
echo ""
echo "📋 22. TruongphongKtnqCnl2 (6 chỉ tiêu):"
create_kpi 22 "Tổng nguồn vốn" 25 "Triệu VND" 1 && ((success_count++))
create_kpi 22 "Lợi nhuận khoán tài chính" 20 "Triệu VND" 2 && ((success_count++))
create_kpi 22 "Thu dịch vụ thanh toán trong nước" 15 "Triệu VND" 3 && ((success_count++))
create_kpi 22 "Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng" 20 "%" 4 && ((success_count++))
create_kpi 22 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 15 "%" 5 && ((success_count++))
create_kpi 22 "Kết quả thực hiện BQ của CB trong phòng" 5 "%" 6 && ((success_count++))

# BẢNG 23: PhophongKtnqCnl2 (ID: 23) - 5 chỉ tiêu
echo ""
echo "📋 23. PhophongKtnqCnl2 (5 chỉ tiêu):"
create_kpi 23 "Số bút toán giao dịch BQ" 30 "BT" 1 && ((success_count++))
create_kpi 23 "Số bút toán hủy" 15 "BT" 2 && ((success_count++))
create_kpi 23 "Thực hiện nhiệm vụ theo chương trình công tác" 25 "%" 3 && ((success_count++))
create_kpi 23 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 20 "%" 4 && ((success_count++))
create_kpi 23 "Hoàn thành chỉ tiêu giao khoán SPDV" 10 "%" 5 && ((success_count++))

echo ""
echo "🎉 HOÀN THÀNH TẠO KPI INDICATORS!"
echo "================================="

# Kiểm tra kết quả cuối cùng
total_indicators=$(curl -s "$API_BASE/KpiIndicators" | jq length 2>/dev/null || echo "0")

echo ""
echo "📊 KẾT QUẢ CUỐI CÙNG:"
echo "✅ Đã tạo thành công: $success_count KPI indicators"
echo "📈 Tổng KPI trong database: $total_indicators"
echo "🎯 Mục tiêu: 158 chỉ tiêu (trừ TqHkKtnb chưa có)"

if [[ "$success_count" -ge 150 ]]; then
    echo "🎉 THÀNH CÔNG HOÀN TOÀN! Hệ thống KPI đã sẵn sàng!"
else
    echo "⚠️  Một số chỉ tiêu chưa được tạo. Kiểm tra lại API hoặc dữ liệu."
fi

echo ""
echo "🚀 CÁC BƯỚC TIẾP THEO:"
echo "1. ✅ Kiểm tra frontend dropdown hiển thị đúng tên bảng KPI"
echo "2. 🔄 Tạo Employee KPI Assignments"
echo "3. 🔄 Tạo Unit KPI Scorings cho chi nhánh"
echo "4. 🔄 Tạo module frontend 'Cấu hình KPI'"
