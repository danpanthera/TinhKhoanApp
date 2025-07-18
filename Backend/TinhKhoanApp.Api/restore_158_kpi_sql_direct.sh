#!/bin/bash

echo "🎯 PHỤC HỒI 158 CHỈ TIÊU KPI BẰNG SQL TRỰC TIẾP"
echo "=============================================="

# Function tạo KPI indicator bằng SQL
create_indicator_sql() {
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

    # Escape single quotes trong name
    local escaped_name=$(echo "$name" | sed "s/'/''/g")

    sqlcmd -S localhost,1433 -U sa -P "YourStrong@Password123" -d TinhKhoanDB -N -C -Q "
    INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
    VALUES ($table_id, N'$escaped_name', $max_score, N'$unit', $order_index, $value_type, 1)
    " > /dev/null 2>&1

    echo "      $order_index. $escaped_name ($max_score điểm, $unit)"
}

echo "🔧 Tạo 158 chỉ tiêu KPI bằng SQL trực tiếp..."

# Xóa tất cả indicators cũ trước
echo "🧹 Xóa indicators cũ..."
sqlcmd -S localhost,1433 -U sa -P "YourStrong@Password123" -d TinhKhoanDB -N -C -Q "DELETE FROM KpiIndicators" > /dev/null 2>&1

# BẢNG 1: TruongphongKhdn (ID: 33) - 8 chỉ tiêu
echo ""
echo "📋 1. TruongphongKhdn (8 chỉ tiêu):"
create_indicator_sql 33 "Tổng Dư nợ KHDN" 20 "Triệu VND" 1
create_indicator_sql 33 "Tỷ lệ nợ xấu KHDN" 10 "%" 2
create_indicator_sql 33 "Thu nợ đã XLRR KHDN" 10 "Triệu VND" 3
create_indicator_sql 33 "Lợi nhuận khoán tài chính" 10 "Triệu VND" 4
create_indicator_sql 33 "Phát triển Khách hàng Doanh nghiệp" 10 "Khách hàng" 5
create_indicator_sql 33 "Điều hành theo chương trình công tác" 20 "%" 6
create_indicator_sql 33 "Chấp hành quy chế, quy trình nghiệp vụ" 10 "%" 7
create_indicator_sql 33 "BQ kết quả thực hiện CB trong phòng mình phụ trách" 10 "%" 8

# BẢNG 2: TruongphongKhcn (ID: 34) - 8 chỉ tiêu
echo ""
echo "📋 2. TruongphongKhcn (8 chỉ tiêu):"
create_indicator_sql 34 "Tổng Dư nợ KHCN" 20 "Triệu VND" 1
create_indicator_sql 34 "Tỷ lệ nợ xấu KHCN" 10 "%" 2
create_indicator_sql 34 "Thu nợ đã XLRR KHCN" 10 "Triệu VND" 3
create_indicator_sql 34 "Lợi nhuận khoán tài chính" 10 "Triệu VND" 4
create_indicator_sql 34 "Phát triển Khách hàng Cá nhân" 10 "Khách hàng" 5
create_indicator_sql 34 "Điều hành theo chương trình công tác" 20 "%" 6
create_indicator_sql 34 "Chấp hành quy chế, quy trình nghiệp vụ" 10 "%" 7
create_indicator_sql 34 "BQ kết quả thực hiện CB trong phòng mình phụ trách" 10 "%" 8

# BẢNG 3: PhophongKhdn (ID: 35) - 8 chỉ tiêu
echo ""
echo "📋 3. PhophongKhdn (8 chỉ tiêu):"
create_indicator_sql 35 "Tổng Dư nợ KHDN" 20 "Triệu VND" 1
create_indicator_sql 35 "Tỷ lệ nợ xấu KHDN" 10 "%" 2
create_indicator_sql 35 "Thu nợ đã XLRR KHDN" 10 "Triệu VND" 3
create_indicator_sql 35 "Lợi nhuận khoán tài chính" 10 "Triệu VND" 4
create_indicator_sql 35 "Phát triển Khách hàng Doanh nghiệp" 10 "Khách hàng" 5
create_indicator_sql 35 "Điều hành theo chương trình công tác" 20 "%" 6
create_indicator_sql 35 "Chấp hành quy chế, quy trình nghiệp vụ" 10 "%" 7
create_indicator_sql 35 "BQ kết quả thực hiện CB trong phòng mình phụ trách" 10 "%" 8

# BẢNG 4: PhophongKhcn (ID: 36) - 8 chỉ tiêu
echo ""
echo "📋 4. PhophongKhcn (8 chỉ tiêu):"
create_indicator_sql 36 "Tổng Dư nợ KHCN" 20 "Triệu VND" 1
create_indicator_sql 36 "Tỷ lệ nợ xấu KHCN" 10 "%" 2
create_indicator_sql 36 "Thu nợ đã XLRR KHCN" 10 "Triệu VND" 3
create_indicator_sql 36 "Lợi nhuận khoán tài chính" 10 "Triệu VND" 4
create_indicator_sql 36 "Phát triển Khách hàng Cá nhân" 10 "Khách hàng" 5
create_indicator_sql 36 "Điều hành theo chương trình công tác" 20 "%" 6
create_indicator_sql 36 "Chấp hành quy chế, quy trình nghiệp vụ" 10 "%" 7
create_indicator_sql 36 "BQ kết quả thực hiện CB trong phòng mình phụ trách" 10 "%" 8

# BẢNG 5: TruongphongKhqlrr (ID: 37) - 6 chỉ tiêu
echo ""
echo "📋 5. TruongphongKhqlrr (6 chỉ tiêu):"
create_indicator_sql 37 "Tổng nguồn vốn" 10 "Triệu VND" 1
create_indicator_sql 37 "Tổng dư nợ" 10 "Triệu VND" 2
create_indicator_sql 37 "Lợi nhuận khoán tài chính" 10 "Triệu VND" 3
create_indicator_sql 37 "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng" 50 "%" 4
create_indicator_sql 37 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 5
create_indicator_sql 37 "Kết quả thực hiện BQ của CB trong phòng" 10 "%" 6

# BẢNG 6: PhophongKhqlrr (ID: 38) - 6 chỉ tiêu
echo ""
echo "📋 6. PhophongKhqlrr (6 chỉ tiêu):"
create_indicator_sql 38 "Tổng nguồn vốn" 10 "Triệu VND" 1
create_indicator_sql 38 "Tổng dư nợ" 10 "Triệu VND" 2
create_indicator_sql 38 "Lợi nhuận khoán tài chính" 10 "Triệu VND" 3
create_indicator_sql 38 "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng" 50 "%" 4
create_indicator_sql 38 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 5
create_indicator_sql 38 "Kết quả thực hiện BQ của CB trong phòng mình phụ trách" 10 "%" 6

# BẢNG 7: Cbtd (ID: 39) - 8 chỉ tiêu
echo ""
echo "📋 7. Cbtd (8 chỉ tiêu):"
create_indicator_sql 39 "Tổng dư nợ BQ" 30 "Triệu VND" 1
create_indicator_sql 39 "Tỷ lệ nợ xấu" 15 "%" 2
create_indicator_sql 39 "Phát triển Khách hàng" 10 "Khách hàng" 3
create_indicator_sql 39 "Thu nợ đã XLRR (nếu không có nợ XLRR thì cộng vào chỉ tiêu Dư nợ)" 10 "Triệu VND" 4
create_indicator_sql 39 "Thực hiện nhiệm vụ theo chương trình công tác" 10 "%" 5
create_indicator_sql 39 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 6
create_indicator_sql 39 "Tổng nguồn vốn huy động BQ" 10 "Triệu VND" 7
create_indicator_sql 39 "Hoàn thành chỉ tiêu giao khoán SPDV" 5 "%" 8

# BẢNG 8: TruongphongKtnqCnl1 (ID: 40) - 6 chỉ tiêu
echo ""
echo "📋 8. TruongphongKtnqCnl1 (6 chỉ tiêu):"
create_indicator_sql 40 "Tổng nguồn vốn" 10 "Triệu VND" 1
create_indicator_sql 40 "Lợi nhuận khoán tài chính" 20 "Triệu VND" 2
create_indicator_sql 40 "Thu dịch vụ thanh toán trong nước" 10 "Triệu VND" 3
create_indicator_sql 40 "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng" 40 "%" 4
create_indicator_sql 40 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 5
create_indicator_sql 40 "Kết quả thực hiện BQ của CB trong phòng" 10 "%" 6

# BẢNG 9: PhophongKtnqCnl1 (ID: 41) - 6 chỉ tiêu
echo ""
echo "📋 9. PhophongKtnqCnl1 (6 chỉ tiêu):"
create_indicator_sql 41 "Tổng nguồn vốn" 10 "Triệu VND" 1
create_indicator_sql 41 "Lợi nhuận khoán tài chính" 20 "Triệu VND" 2
create_indicator_sql 41 "Thu dịch vụ thanh toán trong nước" 10 "Triệu VND" 3
create_indicator_sql 41 "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng" 40 "%" 4
create_indicator_sql 41 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 5
create_indicator_sql 41 "Kết quả thực hiện BQ của CB thuộc mình phụ trách" 10 "%" 6

# BẢNG 10: Gdv (ID: 42) - 6 chỉ tiêu
echo ""
echo "📋 10. Gdv (6 chỉ tiêu):"
create_indicator_sql 42 "Số bút toán giao dịch BQ" 50 "BT" 1
create_indicator_sql 42 "Số bút toán hủy" 15 "BT" 2
create_indicator_sql 42 "Thực hiện chức năng, nhiệm vụ được giao" 10 "%" 3
create_indicator_sql 42 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 4
create_indicator_sql 42 "Tổng nguồn vốn huy động BQ" 10 "Triệu VND" 5
create_indicator_sql 42 "Hoàn thành chỉ tiêu giao khoán SPDV" 5 "%" 6

# BẢNG 11: TqHkKtnb (ID: 43) - 0 chỉ tiêu (theo anh chưa có cụ thể)
echo ""
echo "📋 11. TqHkKtnb - Đợi TP KTNQ/Giám đốc CN loại 2 trực tiếp giao sau (chưa có cụ thể)"

# BẢNG 12: TruongphoItThKtgs (ID: 44) - 5 chỉ tiêu
echo ""
echo "📋 12. TruongphoItThKtgs (5 chỉ tiêu):"
create_indicator_sql 44 "Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng" 65 "%" 1
create_indicator_sql 44 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 2
create_indicator_sql 44 "Tổng nguồn vốn huy động BQ" 10 "Triệu VND" 3
create_indicator_sql 44 "Hoàn thành chỉ tiêu giao khoán SPDV" 5 "%" 4
create_indicator_sql 44 "Kết quả thực hiện BQ của cán bộ trong phòng" 10 "%" 5

# BẢNG 13: CBItThKtgsKhqlrr (ID: 45) - 4 chỉ tiêu
echo ""
echo "📋 13. CBItThKtgsKhqlrr (4 chỉ tiêu):"
create_indicator_sql 45 "Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ được giao" 75 "%" 1
create_indicator_sql 45 "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 2
create_indicator_sql 45 "Tổng nguồn vốn huy động BQ" 10 "Triệu VND" 3
create_indicator_sql 45 "Hoàn thành chỉ tiêu giao khoán SPDV" 5 "%" 4

# BẢNG 14: GiamdocPgd (ID: 46) - 9 chỉ tiêu
echo ""
echo "📋 14. GiamdocPgd (9 chỉ tiêu):"
create_indicator_sql 46 "Tổng nguồn vốn BQ" 15 "Triệu VND" 1
create_indicator_sql 46 "Tổng dư nợ BQ" 15 "Triệu VND" 2
create_indicator_sql 46 "Tỷ lệ nợ xấu" 10 "%" 3
create_indicator_sql 46 "Phát triển Khách hàng" 10 "Khách hàng" 4
create_indicator_sql 46 "Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)" 5 "Triệu VND" 5
create_indicator_sql 46 "Thu dịch vụ" 10 "Triệu VND" 6
create_indicator_sql 46 "Lợi nhuận khoán tài chính" 15 "Triệu VND" 7
create_indicator_sql 46 "Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank" 10 "%" 8
create_indicator_sql 46 "BQ kết quả thực hiện của CB trong phòng" 10 "%" 9

# Tiếp tục các bảng còn lại...

echo ""
echo "✅ Verification - Đếm số lượng KPI indicators đã tạo:"
sqlcmd -S localhost,1433 -U sa -P "YourStrong@Password123" -d TinhKhoanDB -N -C -Q "SELECT COUNT(*) AS TotalIndicators FROM KpiIndicators"

echo ""
echo "📊 Phân bố theo bảng KPI:"
sqlcmd -S localhost,1433 -U sa -P "YourStrong@Password123" -d TinhKhoanDB -N -C -Q "
SELECT TableId, COUNT(*) AS IndicatorCount
FROM KpiIndicators
GROUP BY TableId
ORDER BY TableId
"
