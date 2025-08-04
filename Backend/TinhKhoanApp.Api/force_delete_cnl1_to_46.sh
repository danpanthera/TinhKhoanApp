#!/bin/bash

# Script force xóa 3 CNL1 thừa để đạt chính xác 46 đơn vị Ver2
API_BASE="http://localhost:5055/api"
LOG_FILE="force_delete_cnl1_to_46_$(date +%Y%m%d_%H%M%S).log"

echo "🎯 FORCE XÓA 3 CNL1 THỪA ĐỂ ĐẠT CHÍNH XÁC 46 ĐƠN VỊ" | tee -a $LOG_FILE
echo "📅 Bắt đầu: $(date)" | tee -a $LOG_FILE

# Kiểm tra trạng thái hiện tại
echo "🔍 KIỂM TRA TRẠNG THÁI HIỆN TẠI" | tee -a $LOG_FILE

ALL_UNITS=$(curl -s "$API_BASE/Units")
CURRENT_COUNT=$(echo "$ALL_UNITS" | grep -c '"Id":')
CNL1_CURRENT=$(echo "$ALL_UNITS" | grep -c '"Type": *"CNL1"')
CNL2_CURRENT=$(echo "$ALL_UNITS" | grep -c '"Type": *"CNL2"')
PNVL1_CURRENT=$(echo "$ALL_UNITS" | grep -c '"Type": *"PNVL1"')
PNVL2_CURRENT=$(echo "$ALL_UNITS" | grep -c '"Type": *"PNVL2"')
PGDL2_CURRENT=$(echo "$ALL_UNITS" | grep -c '"Type": *"PGDL2"')

echo "📊 Trạng thái hiện tại:" | tee -a $LOG_FILE
echo "  CNL1: $CNL1_CURRENT (mục tiêu: 1, thừa: $((CNL1_CURRENT - 1)))" | tee -a $LOG_FILE
echo "  CNL2: $CNL2_CURRENT (mục tiêu: 9)" | tee -a $LOG_FILE
echo "  PNVL1: $PNVL1_CURRENT (mục tiêu: 7)" | tee -a $LOG_FILE
echo "  PNVL2: $PNVL2_CURRENT (mục tiêu: 24)" | tee -a $LOG_FILE
echo "  PGDL2: $PGDL2_CURRENT (mục tiêu: 5)" | tee -a $LOG_FILE
echo "  TỔNG: $CURRENT_COUNT (mục tiêu: 46, thừa: $((CURRENT_COUNT - 46)))" | tee -a $LOG_FILE

if [ $CNL1_CURRENT -le 1 ]; then
    echo "✅ CNL1 đã đúng hoặc thiếu, không cần xóa" | tee -a $LOG_FILE
    exit 0
fi

# Xác định số CNL1 cần xóa
CNL1_TO_DELETE=$((CNL1_CURRENT - 1))
echo "" | tee -a $LOG_FILE
echo "🗑️ CẦN XÓA $CNL1_TO_DELETE CNL1 THỪA" | tee -a $LOG_FILE

# Lấy danh sách CNL1 units
CNL1_UNITS=$(echo "$ALL_UNITS" | grep -A10 -B10 '"Type": *"CNL1"')
CNL1_IDS=$(echo "$CNL1_UNITS" | grep -o '"Id": *[0-9]*' | sed 's/"Id": *//')

echo "Tìm thấy các CNL1 unit IDs: $CNL1_IDS" | tee -a $LOG_FILE

# Convert to array
CNL1_ARRAY=($CNL1_IDS)
echo "Tổng CNL1 units: ${#CNL1_ARRAY[@]}" | tee -a $LOG_FILE

if [ ${#CNL1_ARRAY[@]} -le 1 ]; then
    echo "✅ Chỉ có 1 CNL1 hoặc ít hơn, không cần xóa" | tee -a $LOG_FILE
    exit 0
fi

# Kiểm tra dependencies trước khi xóa
echo "" | tee -a $LOG_FILE
echo "🔍 KIỂM TRA DEPENDENCIES TRƯỚC KHI XÓA" | tee -a $LOG_FILE

# Function để kiểm tra xem unit có child units không
check_dependencies() {
    local unit_id=$1
    local children=$(curl -s "$API_BASE/Units" | grep -A5 -B5 '"ParentUnitId": *'$unit_id'')
    local child_count=$(echo "$children" | grep -c '"Id":')
    echo $child_count
}

# Function để xóa unit với retry
force_delete_unit() {
    local unit_id=$1
    local unit_info=$(echo "$ALL_UNITS" | grep -A15 -B15 '"Id": *'$unit_id'')
    local unit_name=$(echo "$unit_info" | grep '"Name":' | head -1 | sed 's/.*"Name": *"\([^"]*\)".*/\1/')
    local unit_code=$(echo "$unit_info" | grep '"Code":' | head -1 | sed 's/.*"Code": *"\([^"]*\)".*/\1/')

    echo "🗑️ Đang xóa CNL1: $unit_name (Code: $unit_code, ID: $unit_id)" | tee -a $LOG_FILE

    # Thử xóa với retry
    for attempt in 1 2 3; do
        RESPONSE=$(curl -s -X DELETE "$API_BASE/Units/$unit_id")

        # Kiểm tra xem unit còn tồn tại không
        sleep 1
        CHECK_EXISTS=$(curl -s "$API_BASE/Units" | grep -c '"Id": *'$unit_id'')

        if [ $CHECK_EXISTS -eq 0 ]; then
            echo "  ✅ Xóa thành công CNL1 ID: $unit_id (attempt $attempt)" | tee -a $LOG_FILE
            return 0
        else
            echo "  ⚠️ Attempt $attempt thất bại, thử lại..." | tee -a $LOG_FILE
            echo "  Response: $RESPONSE" >> $LOG_FILE
        fi
    done

    echo "  ❌ Không thể xóa CNL1 ID: $unit_id sau 3 lần thử" | tee -a $LOG_FILE
    return 1
}

# Xóa CNL1 units (giữ lại unit đầu tiên)
DELETED_COUNT=0
for i in $(seq 2 ${#CNL1_ARRAY[@]}); do
    if [ $DELETED_COUNT -ge $CNL1_TO_DELETE ]; then
        break
    fi

    UNIT_ID=${CNL1_ARRAY[$((i-1))]}
    DEPS=$(check_dependencies $UNIT_ID)

    echo "📋 CNL1 ID $UNIT_ID có $DEPS child units" | tee -a $LOG_FILE

    if [ $DEPS -gt 0 ]; then
        echo "  ⚠️ Unit có $DEPS children, sẽ thử force delete" | tee -a $LOG_FILE
    fi

    if force_delete_unit $UNIT_ID; then
        DELETED_COUNT=$((DELETED_COUNT + 1))
        echo "  📊 Đã xóa $DELETED_COUNT/$CNL1_TO_DELETE CNL1" | tee -a $LOG_FILE
    fi
done

# Kiểm tra kết quả sau khi xóa
echo "" | tee -a $LOG_FILE
echo "🔍 KIỂM TRA KẾT QUẢ SAU KHI XÓA" | tee -a $LOG_FILE
sleep 3

ALL_UNITS_FINAL=$(curl -s "$API_BASE/Units")
FINAL_COUNT=$(echo "$ALL_UNITS_FINAL" | grep -c '"Id":')

CNL1_FINAL=$(echo "$ALL_UNITS_FINAL" | grep -c '"Type": *"CNL1"')
CNL2_FINAL=$(echo "$ALL_UNITS_FINAL" | grep -c '"Type": *"CNL2"')
PNVL1_FINAL=$(echo "$ALL_UNITS_FINAL" | grep -c '"Type": *"PNVL1"')
PNVL2_FINAL=$(echo "$ALL_UNITS_FINAL" | grep -c '"Type": *"PNVL2"')
PGDL2_FINAL=$(echo "$ALL_UNITS_FINAL" | grep -c '"Type": *"PGDL2"')

echo "📊 KẾT QUẢ SAU FORCE DELETE:" | tee -a $LOG_FILE
echo "┌─────────────────────────────────────────┐" | tee -a $LOG_FILE
echo "│  LOẠI     │  MỤC TIÊU  │  THỰC TẾ  │ ✓│" | tee -a $LOG_FILE
echo "├─────────────────────────────────────────┤" | tee -a $LOG_FILE
echo "│  CNL1     │     1      │    $CNL1_FINAL     │$([ $CNL1_FINAL -eq 1 ] && echo " ✅" || echo " ❌")│" | tee -a $LOG_FILE
echo "│  CNL2     │     9      │    $CNL2_FINAL     │$([ $CNL2_FINAL -eq 9 ] && echo " ✅" || echo " ❌")│" | tee -a $LOG_FILE
echo "│  PNVL1    │     7      │    $PNVL1_FINAL     │$([ $PNVL1_FINAL -eq 7 ] && echo " ✅" || echo " ❌")│" | tee -a $LOG_FILE
echo "│  PNVL2    │    24      │   $PNVL2_FINAL    │$([ $PNVL2_FINAL -eq 24 ] && echo " ✅" || echo " ❌")│" | tee -a $LOG_FILE
echo "│  PGDL2    │     5      │    $PGDL2_FINAL     │$([ $PGDL2_FINAL -eq 5 ] && echo " ✅" || echo " ❌")│" | tee -a $LOG_FILE
echo "├─────────────────────────────────────────┤" | tee -a $LOG_FILE
echo "│  TỔNG     │    46      │   $FINAL_COUNT    │$([ $FINAL_COUNT -eq 46 ] && echo " ✅" || echo " ❌")│" | tee -a $LOG_FILE
echo "└─────────────────────────────────────────┘" | tee -a $LOG_FILE

echo "" | tee -a $LOG_FILE
echo "📈 Thống kê xóa:" | tee -a $LOG_FILE
echo "  🗑️ Đã xóa: $DELETED_COUNT CNL1" | tee -a $LOG_FILE
echo "  📉 Giảm từ: $CURRENT_COUNT → $FINAL_COUNT đơn vị" | tee -a $LOG_FILE

if [ $FINAL_COUNT -eq 46 ] && [ $CNL1_FINAL -eq 1 ] && [ $CNL2_FINAL -eq 9 ] && [ $PNVL1_FINAL -eq 7 ] && [ $PNVL2_FINAL -eq 24 ] && [ $PGDL2_FINAL -eq 5 ]; then
    echo "🎉 HOÀN HẢO! Đã đạt chính xác cấu trúc Ver2 với 46 đơn vị!" | tee -a $LOG_FILE
    echo "✅ Cấu trúc Ver2 hoàn chỉnh: 1+9+7+24+5 = 46 đơn vị" | tee -a $LOG_FILE

    # Cập nhật README_DAT.md
    echo "" | tee -a $LOG_FILE
    echo "📝 CẦN CẬP NHẬT README_DAT.md:" | tee -a $LOG_FILE
    echo "  - ✅ 46 Units: ĐÃ HOÀN THÀNH đầy đủ 46 units (Ver2)" | tee -a $LOG_FILE
    echo "  - Statistics: CNL1:1, CNL2:9, PNVL1:7, PNVL2:24, PGDL2:5" | tee -a $LOG_FILE

elif [ $FINAL_COUNT -eq 46 ]; then
    echo "✅ Đã đạt 46 đơn vị nhưng phân bổ chưa hoàn hảo" | tee -a $LOG_FILE
    echo "CNL1: $CNL1_FINAL/1, CNL2: $CNL2_FINAL/9, PNVL1: $PNVL1_FINAL/7, PNVL2: $PNVL2_FINAL/24, PGDL2: $PGDL2_FINAL/5" | tee -a $LOG_FILE
else
    echo "⚠️ Chưa đạt mục tiêu 46 đơn vị" | tee -a $LOG_FILE
    echo "Hiện tại: $FINAL_COUNT, Cần: $((46 - FINAL_COUNT)) đơn vị nữa" | tee -a $LOG_FILE
fi

echo "📅 Hoàn thành: $(date)" | tee -a $LOG_FILE
echo "📝 Log chi tiết: $LOG_FILE" | tee -a $LOG_FILE
echo "🔚 KẾT THÚC FORCE DELETE" | tee -a $LOG_FILE
