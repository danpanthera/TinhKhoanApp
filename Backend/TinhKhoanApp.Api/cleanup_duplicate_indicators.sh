#!/bin/bash
set -e

echo "🗑️  XÓA DUPLICATE CHỈ TIÊU CŨ - Chỉ giữ lại 158 chỉ tiêu mới"
echo "============================================================="

API_BASE="http://localhost:5055/api"

# Function to backup current indicators before cleanup
backup_indicators() {
    echo "📦 Backup toàn bộ chỉ tiêu hiện tại..."
    local backup_file="backup_all_indicators_before_cleanup_$(date +%Y%m%d_%H%M%S).json"

    # Backup tất cả indicators từ tất cả bảng
    echo "[" > "$backup_file"
    local first=true

    for table_id in {1..23}; do
        local table_data=$(curl -s "$API_BASE/KpiAssignment/tables/$table_id")
        local indicators=$(echo "$table_data" | jq '.Indicators // []')

        if [ "$indicators" != "[]" ]; then
            if [ "$first" = false ]; then
                echo "," >> "$backup_file"
            fi
            echo "$table_data" >> "$backup_file"
            first=false
        fi
    done

    echo "]" >> "$backup_file"
    echo "   ✅ Backup saved: $backup_file"
}

# Function to identify old vs new indicators
analyze_indicators() {
    echo ""
    echo "🔍 Phân tích chỉ tiêu cũ vs mới..."

    for table_id in {1..23}; do
        local table_info=$(curl -s "$API_BASE/KpiAssignment/tables/$table_id")
        local table_name=$(echo "$table_info" | jq -r '.TableName // "N/A"')
        local total_indicators=$(echo "$table_info" | jq '.Indicators | length // 0')

        if [ "$total_indicators" -gt 0 ]; then
            local old_count=$(echo "$table_info" | jq '[.Indicators[] | select(.OrderIndex < 17)] | length // 0')
            local new_count=$(echo "$table_info" | jq '[.Indicators[] | select(.OrderIndex >= 17)] | length // 0')

            printf "  📋 %-25s | Total: %2s | Cũ: %2s | Mới: %2s\n" "$table_name" "$total_indicators" "$old_count" "$new_count"

            if [ "$old_count" -gt 0 ]; then
                echo "     🗑️  Sẽ xóa $old_count chỉ tiêu cũ (OrderIndex < 17)"
            fi
        fi
    done
}

# Function to delete old indicators (OrderIndex < 17)
cleanup_old_indicators() {
    echo ""
    echo "🗑️  Bắt đầu xóa chỉ tiêu cũ (OrderIndex < 17)..."

    local total_deleted=0

    for table_id in {1..23}; do
        local table_info=$(curl -s "$API_BASE/KpiAssignment/tables/$table_id")
        local table_name=$(echo "$table_info" | jq -r '.TableName // "N/A"')

        # Get old indicators (OrderIndex < 17)
        local old_indicators=$(echo "$table_info" | jq -r '.Indicators[] | select(.OrderIndex < 17) | .Id')

        if [ ! -z "$old_indicators" ]; then
            echo "  🗑️  Xóa chỉ tiêu cũ từ bảng: $table_name"

            local deleted_count=0
            while IFS= read -r indicator_id; do
                if [ ! -z "$indicator_id" ] && [ "$indicator_id" != "null" ]; then
                    echo "     - Xóa Indicator ID: $indicator_id"

                    # Call DELETE API (if exists)
                    # Note: Cần kiểm tra xem backend có API DELETE /api/KpiAssignment/indicators/{id} không
                    local response=$(curl -s -X DELETE "$API_BASE/KpiAssignment/indicators/$indicator_id" 2>/dev/null || echo "API_NOT_FOUND")

                    if [[ "$response" == *"error"* ]] || [[ "$response" == "API_NOT_FOUND" ]]; then
                        echo "       ⚠️  DELETE API không có sẵn cho indicator ID: $indicator_id"
                        echo "       💡 Cần tạo DELETE endpoint hoặc xóa qua SQL"
                    else
                        ((deleted_count++))
                        ((total_deleted++))
                    fi
                fi
            done <<< "$old_indicators"

            echo "     ✅ Đã xóa $deleted_count chỉ tiêu cũ từ $table_name"
        fi
    done

    echo ""
    echo "📊 Tổng số chỉ tiêu cũ đã xóa: $total_deleted"
}

# Function to verify final result
verify_cleanup() {
    echo ""
    echo "✅ VERIFY KẾT QUẢ SAU KHI CLEANUP"
    echo "================================"

    local total_indicators=0
    local new_indicators=0

    for table_id in {1..23}; do
        local table_info=$(curl -s "$API_BASE/KpiAssignment/tables/$table_id")
        local table_name=$(echo "$table_info" | jq -r '.TableName // "N/A"')
        local current_count=$(echo "$table_info" | jq '.Indicators | length // 0')
        local new_count=$(echo "$table_info" | jq '[.Indicators[] | select(.OrderIndex >= 17)] | length // 0')

        if [ "$current_count" -gt 0 ]; then
            printf "  ✅ %-25s | %2s chỉ tiêu (tất cả mới)\n" "$table_name" "$current_count"
            total_indicators=$((total_indicators + current_count))
            new_indicators=$((new_indicators + new_count))
        fi
    done

    echo ""
    echo "📊 KẾT QUẢ CUỐI CÙNG:"
    echo "   📋 Tổng chỉ tiêu còn lại: $total_indicators"
    echo "   ✨ Chỉ tiêu mới (OrderIndex >= 17): $new_indicators"
    echo ""

    if [ "$total_indicators" -eq 158 ]; then
        echo "🎉 THÀNH CÔNG! Đã cleanup về đúng 158 chỉ tiêu mới!"
    elif [ "$total_indicators" -eq "$new_indicators" ]; then
        echo "✅ CLEANUP HOÀN TẤT! Tất cả chỉ tiêu hiện tại đều là mới."
        echo "📝 Lưu ý: Có $total_indicators chỉ tiêu thay vì 158 (có thể do TqHkKtnb chưa có chỉ tiêu)"
    else
        echo "⚠️  VẪN CÓN DUPLICATE! Cần kiểm tra và xóa thêm."
    fi
}

# Main execution
echo "🚀 Bắt đầu quá trình cleanup duplicate chỉ tiêu..."
echo ""

# 1. Backup trước khi xóa
backup_indicators

# 2. Phân tích tình trạng hiện tại
analyze_indicators

# 3. Confirm với user trước khi xóa
echo ""
read -p "🤔 Bạn có muốn tiếp tục xóa chỉ tiêu cũ? (y/N): " confirm
if [[ $confirm =~ ^[Yy]$ ]]; then
    # 4. Thực hiện cleanup
    cleanup_old_indicators

    # 5. Verify kết quả
    verify_cleanup
else
    echo "❌ Hủy bỏ quá trình cleanup."
fi

echo ""
echo "🏁 Hoàn tất script cleanup duplicate indicators!"
