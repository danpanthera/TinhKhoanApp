#!/bin/bash

echo "🎯 TẠO CHỈ TIÊU KPI - PHIÊN BẢN ĐÚNG TABLE ID"
echo "============================================="

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

# Xóa tất cả KPI indicators cũ
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

echo ""
echo "📊 SUMMARY:"
echo "✅ Đã tạo thành công: $success_count/40 chỉ tiêu KPI (5 bảng đầu tiên)"
echo "🎯 Tiếp tục tạo các bảng còn lại..."
