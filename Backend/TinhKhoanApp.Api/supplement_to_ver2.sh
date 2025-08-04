#!/bin/bash

# Script bổ sung để đạt cấu trúc Ver2: 46 đơn vị chính xác
API_BASE="http://localhost:5055/api"
LOG_FILE="supplement_to_ver2_$(date +%Y%m%d_%H%M%S).log"

echo "🎯 BỔ SUNG ĐỂ ĐẠT CẤU TRÚC VER2: 46 ĐƠN VỊ" | tee -a $LOG_FILE
echo "📅 Bắt đầu: $(date)" | tee -a $LOG_FILE

# Hàm tạo đơn vị
create_unit() {
    local code=$1
    local name=$2
    local type=$3
    local parent_id=$4

    if [ "$parent_id" = "null" ]; then
        JSON='{"Code":"'$code'","Name":"'$name'","Type":"'$type'","ParentUnitId":null}'
    else
        JSON='{"Code":"'$code'","Name":"'$name'","Type":"'$type'","ParentUnitId":'$parent_id'}'
    fi

    RESPONSE=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "$JSON")
    ID=$(echo "$RESPONSE" | grep -o '"Id": *[0-9]*' | sed 's/"Id": *//')

    if [ -n "$ID" ]; then
        echo "✅ $name -> ID: $ID" | tee -a $LOG_FILE
        echo "$ID"
    else
        echo "❌ Lỗi: $name" | tee -a $LOG_FILE
        echo "Response: $RESPONSE" | tee -a $LOG_FILE
        echo ""
    fi
}

# Kiểm tra trạng thái hiện tại
echo "🔍 KIỂM TRA TRẠNG THÁI HIỆN TẠI" | tee -a $LOG_FILE

ALL_UNITS=$(curl -s "$API_BASE/Units")
CURRENT_COUNT=$(echo "$ALL_UNITS" | grep -c '"Id":')
CNL1_CURRENT=$(echo "$ALL_UNITS" | grep -c '"Type": *"CNL1"')
CNL2_CURRENT=$(echo "$ALL_UNITS" | grep -c '"Type": *"CNL2"')
PNVL1_CURRENT=$(echo "$ALL_UNITS" | grep -c '"Type": *"PNVL1"')
PNVL2_CURRENT=$(echo "$ALL_UNITS" | grep -c '"Type": *"PNVL2"')
PGDL2_CURRENT=$(echo "$ALL_UNITS" | grep -c '"Type": *"PGDL2"')

echo "Hiện tại: CNL1=$CNL1_CURRENT, CNL2=$CNL2_CURRENT, PNVL1=$PNVL1_CURRENT, PNVL2=$PNVL2_CURRENT, PGDL2=$PGDL2_CURRENT" | tee -a $LOG_FILE
echo "Tổng: $CURRENT_COUNT đơn vị" | tee -a $LOG_FILE

# Tính toán cần bổ sung
CNL1_NEED=$((1 - CNL1_CURRENT))
CNL2_NEED=$((9 - CNL2_CURRENT))
PNVL1_NEED=$((7 - PNVL1_CURRENT))
PNVL2_NEED=$((24 - PNVL2_CURRENT))
PGDL2_NEED=$((5 - PGDL2_CURRENT))

echo "Cần bổ sung: CNL1=$CNL1_NEED, CNL2=$CNL2_NEED, PNVL1=$PNVL1_NEED, PNVL2=$PNVL2_NEED, PGDL2=$PGDL2_NEED" | tee -a $LOG_FILE

# Lấy ID của đơn vị gốc hiện tại để làm parent
ROOT_UNITS=$(echo "$ALL_UNITS" | grep -A5 -B5 '"Type": *"CNL1"')
ROOT_ID=$(echo "$ROOT_UNITS" | grep -o '"Id": *[0-9]*' | head -1 | sed 's/"Id": *//')
echo "Sử dụng đơn vị gốc ID: $ROOT_ID" | tee -a $LOG_FILE

# Bổ sung CNL2 nếu thiếu
if [ $CNL2_NEED -gt 0 ]; then
    echo "📍 Bổ sung $CNL2_NEED chi nhánh CNL2..." | tee -a $LOG_FILE
    for i in $(seq 1 $CNL2_NEED); do
        TIMESTAMP=$(date +%s)
        create_unit "CN_BO_SUNG_${i}_${TIMESTAMP}" "Chi nhanh bo sung $i" "CNL2" "$ROOT_ID"
    done
fi

# Lấy danh sách CNL2 units để làm parent cho PNVL1, PNVL2, PGDL2
CNL2_UNITS=$(curl -s "$API_BASE/Units" | grep -A5 -B5 '"Type": *"CNL2"')
CNL2_IDS=$(echo "$CNL2_UNITS" | grep -o '"Id": *[0-9]*' | sed 's/"Id": *//')

# Convert to array
CNL2_ARRAY=($CNL2_IDS)
echo "Tìm thấy ${#CNL2_ARRAY[@]} chi nhánh CNL2: ${CNL2_ARRAY[*]}" | tee -a $LOG_FILE

# Bổ sung PNVL1 nếu thiếu (gắn vào CNL2 đầu tiên - giả sử là Hội sở)
if [ $PNVL1_NEED -gt 0 ] && [ ${#CNL2_ARRAY[@]} -gt 0 ]; then
    echo "📍 Bổ sung $PNVL1_NEED phòng PNVL1..." | tee -a $LOG_FILE
    HOISSO_ID=${CNL2_ARRAY[0]}
    for i in $(seq 1 $PNVL1_NEED); do
        TIMESTAMP=$(date +%s)
        create_unit "PNVL1_BO_SUNG_${i}_${TIMESTAMP}" "Phong Hoi So bo sung $i" "PNVL1" "$HOISSO_ID"
    done
fi

# Bổ sung PNVL2 nếu thiếu (phân bổ đều cho các CNL2)
if [ $PNVL2_NEED -gt 0 ] && [ ${#CNL2_ARRAY[@]} -gt 0 ]; then
    echo "📍 Bổ sung $PNVL2_NEED phòng PNVL2..." | tee -a $LOG_FILE
    CNL2_COUNT=${#CNL2_ARRAY[@]}
    for i in $(seq 1 $PNVL2_NEED); do
        # Phân bổ round-robin
        CNL2_INDEX=$(((i - 1) % CNL2_COUNT))
        PARENT_CNL2=${CNL2_ARRAY[$CNL2_INDEX]}
        TIMESTAMP=$(date +%s)
        create_unit "PNVL2_BO_SUNG_${i}_${TIMESTAMP}" "Phong Chi nhanh bo sung $i" "PNVL2" "$PARENT_CNL2"
    done
fi

# Bổ sung PGDL2 nếu thiếu (gắn vào CNL2 cuối)
if [ $PGDL2_NEED -gt 0 ] && [ ${#CNL2_ARRAY[@]} -gt 0 ]; then
    echo "📍 Bổ sung $PGDL2_NEED phòng giao dịch PGDL2..." | tee -a $LOG_FILE
    for i in $(seq 1 $PGDL2_NEED); do
        # Phân bổ cho các CNL2 cuối
        CNL2_INDEX=$(((i - 1) % ${#CNL2_ARRAY[@]}))
        PARENT_CNL2=${CNL2_ARRAY[$CNL2_INDEX]}
        TIMESTAMP=$(date +%s)
        create_unit "PGDL2_BO_SUNG_${i}_${TIMESTAMP}" "PGD bo sung $i" "PGDL2" "$PARENT_CNL2"
    done
fi

# Kiểm tra kết quả cuối cùng
echo "" | tee -a $LOG_FILE
echo "🔍 KIỂM TRA KẾT QUẢ CUỐI CÙNG" | tee -a $LOG_FILE
sleep 3

ALL_UNITS_FINAL=$(curl -s "$API_BASE/Units")
FINAL_COUNT=$(echo "$ALL_UNITS_FINAL" | grep -c '"Id":')

CNL1_FINAL=$(echo "$ALL_UNITS_FINAL" | grep -c '"Type": *"CNL1"')
CNL2_FINAL=$(echo "$ALL_UNITS_FINAL" | grep -c '"Type": *"CNL2"')
PNVL1_FINAL=$(echo "$ALL_UNITS_FINAL" | grep -c '"Type": *"PNVL1"')
PNVL2_FINAL=$(echo "$ALL_UNITS_FINAL" | grep -c '"Type": *"PNVL2"')
PGDL2_FINAL=$(echo "$ALL_UNITS_FINAL" | grep -c '"Type": *"PGDL2"')

echo "📊 KẾT QUẢ SAU BỔ SUNG:" | tee -a $LOG_FILE
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
if [ $FINAL_COUNT -eq 46 ] && [ $CNL1_FINAL -eq 1 ] && [ $CNL2_FINAL -eq 9 ] && [ $PNVL1_FINAL -eq 7 ] && [ $PNVL2_FINAL -eq 24 ] && [ $PGDL2_FINAL -eq 5 ]; then
    echo "🎉 THÀNH CÔNG! Đã đạt cấu trúc Ver2 chính xác!" | tee -a $LOG_FILE
elif [ $FINAL_COUNT -eq 46 ]; then
    echo "✅ Đã đạt 46 đơn vị nhưng phân bổ chưa hoàn hảo" | tee -a $LOG_FILE
else
    echo "⚠️ Tổng số đơn vị: $FINAL_COUNT (mục tiêu: 46)" | tee -a $LOG_FILE
fi

echo "📅 Hoàn thành: $(date)" | tee -a $LOG_FILE
echo "📝 Log chi tiết: $LOG_FILE" | tee -a $LOG_FILE
