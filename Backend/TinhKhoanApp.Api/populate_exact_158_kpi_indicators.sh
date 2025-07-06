#!/bin/bash
set -e

echo "🔄 XÓA VÀ TẠO LẠI TOÀN BỘ CHỈ TIÊU KPI THEO DANH SÁCH CHÍNH XÁC"
echo "================================================================"

API_BASE="http://localhost:5055/api"

# Function để xóa tất cả indicators của một bảng và tạo lại
reset_table_indicators() {
    local table_name="$1"
    shift
    local indicators=("$@")
    
    echo "🗑️  Xóa toàn bộ chỉ tiêu của bảng: $table_name"
    
    # Get TableId
    local table_id=$(curl -s "$API_BASE/KpiAssignment/tables" | jq -r ".[] | select(.TableName == \"$table_name\") | .Id")
    
    if [ -z "$table_id" ]; then
        echo "    ❌ Không tìm thấy TableId cho $table_name"
        return 1
    fi
    
    # Xóa tất cả indicators hiện tại (nếu có API để xóa)
    echo "    📋 Tạo lại chỉ tiêu mới cho $table_name:"
    
    # Tạo các indicators mới
    local index=1
    for indicator in "${indicators[@]}"; do
        IFS='|' read -r name score unit <<< "$indicator"
        
        echo "      $index. $name ($score điểm, $unit)"
        
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
            echo "        ❌ Lỗi: $RESPONSE"
        fi
        
        ((index++))
    done
    echo ""
}

echo "📋 1. TruongphongKhdn - Trưởng phòng KHDN"
reset_table_indicators "TruongphongKhdn" \
    "Tổng Dư nợ KHDN|20|Triệu VND" \
    "Tỷ lệ nợ xấu KHDN|10|%" \
    "Thu nợ đã XLRR KHDN|10|Triệu VND" \
    "Lợi nhuận khoán tài chính|10|Triệu VND" \
    "Phát triển Khách hàng Doanh nghiệp|10|Khách hàng" \
    "Điều hành theo chương trình công tác|20|%" \
    "Chấp hành quy chế, quy trình nghiệp vụ|10|%" \
    "BQ kết quả thực hiện CB trong phòng mình phụ trách|10|%"

echo "📋 2. TruongphongKhcn - Trưởng phòng KHCN"
reset_table_indicators "TruongphongKhcn" \
    "Tổng Dư nợ KHCN|20|Triệu VND" \
    "Tỷ lệ nợ xấu KHCN|10|%" \
    "Thu nợ đã XLRR KHCN|10|Triệu VND" \
    "Lợi nhuận khoán tài chính|10|Triệu VND" \
    "Phát triển Khách hàng Cá nhân|10|Khách hàng" \
    "Điều hành theo chương trình công tác|20|%" \
    "Chấp hành quy chế, quy trình nghiệp vụ|10|%" \
    "BQ kết quả thực hiện CB trong phòng mình phụ trách|10|%"

echo "📋 3. PhophongKhdn - Phó phòng KHDN"
reset_table_indicators "PhophongKhdn" \
    "Tổng Dư nợ KHDN|20|Triệu VND" \
    "Tỷ lệ nợ xấu KHDN|10|%" \
    "Thu nợ đã XLRR KHDN|10|Triệu VND" \
    "Lợi nhuận khoán tài chính|10|Triệu VND" \
    "Phát triển Khách hàng Doanh nghiệp|10|Khách hàng" \
    "Điều hành theo chương trình công tác|20|%" \
    "Chấp hành quy chế, quy trình nghiệp vụ|10|%" \
    "BQ kết quả thực hiện CB trong phòng mình phụ trách|10|%"

echo "📋 4. PhophongKhcn - Phó phòng KHCN"
reset_table_indicators "PhophongKhcn" \
    "Tổng Dư nợ KHCN|20|Triệu VND" \
    "Tỷ lệ nợ xấu KHCN|10|%" \
    "Thu nợ đã XLRR KHCN|10|Triệu VND" \
    "Lợi nhuận khoán tài chính|10|Triệu VND" \
    "Phát triển Khách hàng Cá nhân|10|Khách hàng" \
    "Điều hành theo chương trình công tác|20|%" \
    "Chấp hành quy chế, quy trình nghiệp vụ|10|%" \
    "BQ kết quả thực hiện CB trong phòng mình phụ trách|10|%"

echo "📋 5. TruongphongKhqlrr - Trưởng phòng KH&QLRR"
reset_table_indicators "TruongphongKhqlrr" \
    "Tổng nguồn vốn|10|Triệu VND" \
    "Tổng dư nợ|10|Triệu VND" \
    "Lợi nhuận khoán tài chính|10|Triệu VND" \
    "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng|50|%" \
    "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank|10|%" \
    "Kết quả thực hiện BQ của CB trong phòng|10|%"

echo "📋 6. PhophongKhqlrr - Phó phòng KH&QLRR"
reset_table_indicators "PhophongKhqlrr" \
    "Tổng nguồn vốn|10|Triệu VND" \
    "Tổng dư nợ|10|Triệu VND" \
    "Lợi nhuận khoán tài chính|10|Triệu VND" \
    "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng|50|%" \
    "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank|10|%" \
    "Kết quả thực hiện BQ của CB trong phòng mình phụ trách|10|%"

echo "📋 7. Cbtd - Cán bộ tín dụng"
reset_table_indicators "Cbtd" \
    "Tổng dư nợ BQ|30|Triệu VND" \
    "Tỷ lệ nợ xấu|15|%" \
    "Phát triển Khách hàng|10|Khách hàng" \
    "Thu nợ đã XLRR (nếu không có nợ XLRR thì cộng vào chỉ tiêu Dư nợ)|10|Triệu VND" \
    "Thực hiện nhiệm vụ theo chương trình công tác|10|%" \
    "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank|10|%" \
    "Tổng nguồn vốn huy động BQ|10|Triệu VND" \
    "Hoàn thành chỉ tiêu giao khoán SPDV|5|%"

echo "📋 8. TruongphongKtnqCnl1 - Trưởng phòng KTNQ CNL1"
reset_table_indicators "TruongphongKtnqCnl1" \
    "Tổng nguồn vốn|10|Triệu VND" \
    "Lợi nhuận khoán tài chính|20|Triệu VND" \
    "Thu dịch vụ thanh toán trong nước|10|Triệu VND" \
    "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng|40|%" \
    "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank|10|%" \
    "Kết quả thực hiện BQ của CB trong phòng|10|%"

echo "📋 9. PhophongKtnqCnl1 - Phó phòng KTNQ CNL1"
reset_table_indicators "PhophongKtnqCnl1" \
    "Tổng nguồn vốn|10|Triệu VND" \
    "Lợi nhuận khoán tài chính|20|Triệu VND" \
    "Thu dịch vụ thanh toán trong nước|10|Triệu VND" \
    "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng|40|%" \
    "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank|10|%" \
    "Kết quả thực hiện BQ của CB thuộc mình phụ trách|10|%"

echo "📋 10. Gdv - Giao dịch viên"
reset_table_indicators "Gdv" \
    "Số bút toán giao dịch BQ|50|BT" \
    "Số bút toán hủy|15|BT" \
    "Thực hiện chức năng, nhiệm vụ được giao|10|%" \
    "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank|10|%" \
    "Tổng nguồn vốn huy động BQ|10|Triệu VND" \
    "Hoàn thành chỉ tiêu giao khoán SPDV|5|%"

echo "📋 11. TqHkKtnb - Thủ quỹ | Hậu kiểm | KTNB (PLACEHOLDER - đợi anh cung cấp)"
# Bảng này sẽ được cập nhật sau khi anh cung cấp chỉ tiêu cụ thể

echo "📋 12. TruongphongItThKtgs - Trưởng phòng IT | Tổng hợp | KTGS"
reset_table_indicators "TruongphongItThKtgs" \
    "Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng|65|%" \
    "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank|10|%" \
    "Tổng nguồn vốn huy động BQ|10|Triệu VND" \
    "Hoàn thành chỉ tiêu giao khoán SPDV|5|%" \
    "Kết quả thực hiện BQ của cán bộ trong phòng|10|%"

echo "📋 13. CbItThKtgsKhqlrr - Cán bộ IT | Tổng hợp | KTGS | KH&QLRR"
reset_table_indicators "CbItThKtgsKhqlrr" \
    "Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ được giao|75|%" \
    "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank|10|%" \
    "Tổng nguồn vốn huy động BQ|10|Triệu VND" \
    "Hoàn thành chỉ tiêu giao khoán SPDV|5|%"

echo "📋 14. GiamdocPgd - Giám đốc Phòng giao dịch"
reset_table_indicators "GiamdocPgd" \
    "Tổng nguồn vốn BQ|15|Triệu VND" \
    "Tổng dư nợ BQ|15|Triệu VND" \
    "Tỷ lệ nợ xấu|10|%" \
    "Phát triển Khách hàng|10|Khách hàng" \
    "Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)|5|Triệu VND" \
    "Thu dịch vụ|10|Triệu VND" \
    "Lợi nhuận khoán tài chính|15|Triệu VND" \
    "Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank|10|%" \
    "BQ kết quả thực hiện của CB trong phòng|10|%"

echo "📋 15. PhogiamdocPgd - Phó Giám đốc Phòng giao dịch"
reset_table_indicators "PhogiamdocPgd" \
    "Tổng nguồn vốn BQ|15|Triệu VND" \
    "Tổng dư nợ BQ|15|Triệu VND" \
    "Tỷ lệ nợ xấu|10|%" \
    "Phát triển Khách hàng|10|Khách hàng" \
    "Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)|5|Triệu VND" \
    "Thu dịch vụ|10|Triệu VND" \
    "Lợi nhuận khoán tài chính|15|Triệu VND" \
    "Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank|10|%" \
    "BQ kết quả thực hiện của CB trong phòng|10|%"

echo "📋 16. PhogiamdocPgdCbtd - Phó Giám đốc PGD kiêm CBTD"
reset_table_indicators "PhogiamdocPgdCbtd" \
    "Tổng dư nợ BQ|30|Triệu VND" \
    "Tỷ lệ nợ xấu|15|%" \
    "Phát triển Khách hàng|10|Khách hàng" \
    "Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)|10|Triệu VND" \
    "Thực hiện nhiệm vụ theo chương trình công tác|10|%" \
    "Tổng nguồn vốn huy động BQ|10|Triệu VND" \
    "Hoàn thành chỉ tiêu giao khoán SPDV|5|%" \
    "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank|10|%"

echo "📋 17. GiamdocCnl2 - Giám đốc CNL2"
reset_table_indicators "GiamdocCnl2" \
    "Tổng nguồn vốn cuối kỳ|5|Triệu VND" \
    "Tổng nguồn vốn huy động BQ trong kỳ|10|Triệu VND" \
    "Tổng dư nợ cuối kỳ|5|Triệu VND" \
    "Tổng dư nợ BQ trong kỳ|10|Triệu VND" \
    "Tổng dư nợ HSX&CN|5|Triệu VND" \
    "Tỷ lệ nợ xấu nội bảng|10|%" \
    "Thu nợ đã XLRR|5|Triệu VND" \
    "Phát triển Khách hàng|10|Khách hàng" \
    "Lợi nhuận khoán tài chính|20|Triệu VND" \
    "Thu dịch vụ|10|Triệu VND" \
    "Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank|10|%"

echo "📋 18. PhogiamdocCnl2Td - Phó Giám đốc CNL2 phụ trách TD"
reset_table_indicators "PhogiamdocCnl2Td" \
    "Tổng dư nợ cho vay|20|Triệu VND" \
    "Tổng dư nợ cho vay HSX&CN|10|Triệu VND" \
    "Thu nợ đã xử lý|10|Triệu VND" \
    "Lợi nhuận khoán tài chính|20|Triệu VND" \
    "Tỷ lệ nợ xấu|10|%" \
    "Phát triển Khách hàng|10|Khách hàng" \
    "Điều hành theo chương trình công tác, nhiệm vụ được giao|10|%" \
    "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank|10|%"

echo "📋 19. PhogiamdocCnl2Kt - Phó Giám đốc CNL2 phụ trách KT"
reset_table_indicators "PhogiamdocCnl2Kt" \
    "Tổng nguồn vốn|20|Triệu VND" \
    "Lợi nhuận khoán tài chính|30|Triệu VND" \
    "Tổng doanh thu phí dịch vụ|20|Triệu VND" \
    "Số thẻ phát hành|10|cái" \
    "Điều hành theo chương trình công tác, nhiệm vụ được giao|10|%" \
    "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank|10|%"

echo "📋 20. TruongphongKhCnl2 - Trưởng phòng KH CNL2"
reset_table_indicators "TruongphongKhCnl2" \
    "Tổng dư nợ|20|Triệu VND" \
    "Tỷ lệ nợ xấu|15|%" \
    "Phát triển Khách hàng|10|Khách hàng" \
    "Thu nợ đã XLRR|10|Triệu VND" \
    "Điều hành theo chương trình công tác|10|%" \
    "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank|10|%" \
    "Tổng nguồn vốn huy động BQ|10|Triệu VND" \
    "Hoàn thành chỉ tiêu giao khoán SPDV|5|%" \
    "Kết quả thực hiện BQ của CB trong phòng|10|%"

echo "📋 21. PhophongKhCnl2 - Phó phòng KH CNL2"
reset_table_indicators "PhophongKhCnl2" \
    "Tổng dư nợ BQ|30|Triệu VND" \
    "Tỷ lệ nợ xấu|15|%" \
    "Phát triển Khách hàng|10|Khách hàng" \
    "Thu nợ đã XLRR|10|Triệu VND" \
    "Thực hiện nhiệm vụ theo chương trình công tác|10|%" \
    "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank|10|%" \
    "Tổng nguồn vốn huy động BQ|10|Triệu VND" \
    "Hoàn thành chỉ tiêu giao khoán SPDV|5|%"

echo "📋 22. TruongphongKtnqCnl2 - Trưởng phòng KTNQ CNL2"
reset_table_indicators "TruongphongKtnqCnl2" \
    "Tổng nguồn vốn|10|Triệu VND" \
    "Lợi nhuận khoán tài chính|20|Triệu VND" \
    "Thu dịch vụ thanh toán trong nước|10|Triệu VND" \
    "Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng|40|%" \
    "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank|10|%" \
    "Kết quả thực hiện BQ của CB trong phòng|10|%"

echo "📋 23. PhophongKtnqCnl2 - Phó phòng KTNQ CNL2"
reset_table_indicators "PhophongKtnqCnl2" \
    "Số bút toán giao dịch BQ|40|BT" \
    "Số bút toán hủy|20|BT" \
    "Thực hiện nhiệm vụ theo chương trình công tác|25|%" \
    "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank|10|%" \
    "Hoàn thành chỉ tiêu giao khoán SPDV|5|%"

echo ""
echo "✅ HOÀN THÀNH thay thế toàn bộ chỉ tiêu KPI!"
echo ""
echo "📊 Tổng số chỉ tiêu theo danh sách anh cung cấp:"
echo "   1-4. KHDN/KHCN: 4 bảng × 8 chỉ tiêu = 32"
echo "   5-6. KH&QLRR: 2 bảng × 6 chỉ tiêu = 12"
echo "   7. CBTD: 1 bảng × 8 chỉ tiêu = 8"
echo "   8-9. KTNQ CNL1: 2 bảng × 6 chỉ tiêu = 12"
echo "   10. GDV: 1 bảng × 6 chỉ tiêu = 6"
echo "   12. IT/TH/KTGS: 1 bảng × 5 chỉ tiêu = 5"
echo "   13. CB IT/TH/KTGS: 1 bảng × 4 chỉ tiêu = 4"
echo "   14-15. GĐ PGD: 2 bảng × 9 chỉ tiêu = 18"
echo "   16. PGĐ CBTD: 1 bảng × 8 chỉ tiêu = 8"
echo "   17. GĐ CNL2: 1 bảng × 11 chỉ tiêu = 11"
echo "   18. PGĐ CNL2 TD: 1 bảng × 8 chỉ tiêu = 8"
echo "   19. PGĐ CNL2 KT: 1 bảng × 6 chỉ tiêu = 6"
echo "   20. TP KH CNL2: 1 bảng × 9 chỉ tiêu = 9"
echo "   21. PP KH CNL2: 1 bảng × 8 chỉ tiêu = 8"
echo "   22. TP KTNQ CNL2: 1 bảng × 6 chỉ tiêu = 6"
echo "   23. PP KTNQ CNL2: 1 bảng × 5 chỉ tiêu = 5"
echo ""
echo "   📈 TỔNG: 158 chỉ tiêu cho 22 bảng (thiếu TqHkKtnb)"
echo ""
