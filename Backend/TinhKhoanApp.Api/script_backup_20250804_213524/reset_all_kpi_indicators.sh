#!/bin/bash
set -e

echo "🗑️  XÓA TOÀN BỘ CHỈ TIÊU CŨ VÀ RESET CLEAN"
echo "=========================================="

API_BASE="http://localhost:5055/api"

# Function to backup current indicators
backup_indicators() {
    echo "📦 Backup toàn bộ chỉ tiêu hiện tại..."
    local backup_file="backup_all_indicators_$(date +%Y%m%d_%H%M%S).json"

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

# Function to delete ALL indicators via SQL
reset_all_indicators_sql() {
    echo ""
    echo "🗑️  Xóa TOÀN BỘ chỉ tiêu qua SQL..."

    # Create SQL script to delete all KpiIndicators
    local sql_file="delete_all_kpi_indicators.sql"

    cat > "$sql_file" << 'EOF'
-- Reset toàn bộ KpiIndicators cho 23 bảng KPI cán bộ
USE TinhKhoanDB;

-- Backup count trước khi xóa
SELECT 'Tổng chỉ tiêu trước khi xóa:' as Status, COUNT(*) as Count
FROM KpiIndicators ki
JOIN KpiAssignmentTables kat ON ki.TableId = kat.Id
WHERE kat.Category = 'CANBO';

-- Xóa tất cả KpiIndicators của 23 bảng KPI cán bộ
DELETE ki
FROM KpiIndicators ki
JOIN KpiAssignmentTables kat ON ki.TableId = kat.Id
WHERE kat.Category = 'CANBO';

-- Reset IDENTITY seed về 1
DBCC CHECKIDENT ('KpiIndicators', RESEED, 0);

-- Verify kết quả
SELECT 'Tổng chỉ tiêu sau khi xóa:' as Status, COUNT(*) as Count
FROM KpiIndicators ki
JOIN KpiAssignmentTables kat ON ki.TableId = kat.Id
WHERE kat.Category = 'CANBO';

PRINT 'Đã xóa toàn bộ KpiIndicators cho 23 bảng KPI cán bộ!';
EOF

    echo "   📝 Tạo SQL script: $sql_file"
    echo "   🔄 Thực thi SQL qua sqlcmd..."

    # Execute SQL via sqlcmd
    sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -i "$sql_file" -C

    if [ $? -eq 0 ]; then
        echo "   ✅ Đã xóa toàn bộ chỉ tiêu thành công!"
    else
        echo "   ❌ Lỗi khi thực thi SQL!"
        exit 1
    fi
}

# Function to verify cleanup
verify_cleanup() {
    echo ""
    echo "✅ VERIFY KẾT QUẢ SAU KHI XÓA"
    echo "============================="

    local total_indicators=0

    for table_id in {1..23}; do
        local table_info=$(curl -s "$API_BASE/KpiAssignment/tables/$table_id")
        local table_name=$(echo "$table_info" | jq -r '.TableName // "N/A"')
        local current_count=$(echo "$table_info" | jq '.Indicators | length // 0')

        if [ "$current_count" -eq 0 ]; then
            printf "  🗑️  %-25s | %2s chỉ tiêu (đã xóa sạch)\n" "$table_name" "$current_count"
        else
            printf "  ⚠️  %-25s | %2s chỉ tiêu (còn lại)\n" "$table_name" "$current_count"
        fi

        total_indicators=$((total_indicators + current_count))
    done

    echo ""
    echo "📊 KẾT QUẢ:"
    echo "   📋 Tổng chỉ tiêu còn lại: $total_indicators"

    if [ "$total_indicators" -eq 0 ]; then
        echo "🎉 THÀNH CÔNG! Đã xóa sạch toàn bộ chỉ tiêu cũ!"
        echo "✨ Sẵn sàng để populate lại 158 chỉ tiêu mới!"
    else
        echo "⚠️  VẪN CÒN $total_indicators chỉ tiêu! Cần kiểm tra."
    fi
}

# Main execution
echo "🚀 Bắt đầu quá trình reset toàn bộ chỉ tiêu..."
echo ""

# 1. Backup trước khi xóa
backup_indicators

# 2. Confirm với user
echo ""
echo "⚠️  CẢNH BÁO: Script này sẽ XÓA TOÀN BỘ chỉ tiêu của 23 bảng KPI cán bộ!"
echo "📦 Dữ liệu đã được backup, nhưng hãy chắc chắn trước khi tiếp tục."
echo ""
read -p "🤔 Bạn có chắc chắn muốn XÓA TOÀN BỘ chỉ tiêu? (yes/NO): " confirm

if [[ $confirm == "yes" ]]; then
    # 3. Thực hiện reset via SQL
    reset_all_indicators_sql

    # 4. Verify kết quả
    verify_cleanup

    echo ""
    echo "🎯 BƯỚC TIẾP THEO:"
    echo "   1. Chạy script: ./populate_exact_158_kpi_indicators.sh"
    echo "   2. Verify có đúng 158 chỉ tiêu mới"
    echo "   3. Kiểm tra frontend dropdown"
    echo "   4. Tạo EmployeeKpiAssignments"
else
    echo "❌ Hủy bỏ quá trình reset."
fi

echo ""
echo "🏁 Hoàn tất script reset indicators!"
