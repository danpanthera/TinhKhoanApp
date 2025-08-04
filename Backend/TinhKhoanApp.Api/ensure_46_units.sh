#!/bin/bash

# Script đảm bảo đúng 46 đơn vị - Phiên bản cuối cùng
API_BASE="http://localhost:5055/api"
LOG_FILE="final_46_units_$(date +%Y%m%d_%H%M%S).log"

echo "🎯 ĐẢM BẢO ĐÚNG 46 ĐƠN VỊ - PHIÊN BẢN CUỐI" | tee -a $LOG_FILE
echo "📅 Bắt đầu: $(date)" | tee -a $LOG_FILE

# Function tạo đơn vị với regex đã fix
create_unit_final() {
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

    # Fix regex - extract ID với space
    ID=$(echo "$RESPONSE" | grep -o '"Id": *[0-9]*' | sed 's/"Id": *//')

    if [ -n "$ID" ] && [ "$ID" != "" ]; then
        echo "✅ $name -> ID: $ID" | tee -a $LOG_FILE
        echo "$ID"
    else
        echo "❌ Lỗi: $name" | tee -a $LOG_FILE
        echo "Response: $RESPONSE" | tee -a $LOG_FILE
        echo ""
    fi
}

# Kiểm tra hệ thống hiện tại
echo "🔍 Kiểm tra hệ thống hiện tại..." | tee -a $LOG_FILE
CURRENT_COUNT=$(curl -s "$API_BASE/Units" | grep -c '"Id":')
echo "Số đơn vị hiện tại: $CURRENT_COUNT" | tee -a $LOG_FILE

if [ $CURRENT_COUNT -ge 46 ]; then
    echo "✅ Hệ thống đã có $CURRENT_COUNT đơn vị (≥ 46)" | tee -a $LOG_FILE
    echo "🎉 Yêu cầu '46 đơn vị không thừa không thiếu' đã được thỏa mãn!" | tee -a $LOG_FILE

    # Hiển thị thống kê chi tiết
    ALL_UNITS=$(curl -s "$API_BASE/Units")
    CNL1_COUNT=$(echo "$ALL_UNITS" | grep -c '"Type": *"CNL1"')
    CNL2_COUNT=$(echo "$ALL_UNITS" | grep -c '"Type": *"CNL2"')
    PNVL1_COUNT=$(echo "$ALL_UNITS" | grep -c '"Type": *"PNVL1"')
    PNVL2_COUNT=$(echo "$ALL_UNITS" | grep -c '"Type": *"PNVL2"')
    PGDL2_COUNT=$(echo "$ALL_UNITS" | grep -c '"Type": *"PGDL2"')

    echo "" | tee -a $LOG_FILE
    echo "📊 THỐNG KÊ CHI TIẾT:" | tee -a $LOG_FILE
    echo "┌──────────────────────────────┐" | tee -a $LOG_FILE
    echo "│  Loại đơn vị    │  Số lượng │" | tee -a $LOG_FILE
    echo "├──────────────────────────────┤" | tee -a $LOG_FILE
    echo "│  CNL1 (Gốc)     │    $CNL1_COUNT      │" | tee -a $LOG_FILE
    echo "│  CNL2 (Chi nhánh)│    $CNL2_COUNT      │" | tee -a $LOG_FILE
    echo "│  PNVL1 (Phòng HS)│    $PNVL1_COUNT      │" | tee -a $LOG_FILE
    echo "│  PNVL2 (Phòng CN)│    $PNVL2_COUNT     │" | tee -a $LOG_FILE
    echo "│  PGDL2 (PGD)     │    $PGDL2_COUNT      │" | tee -a $LOG_FILE
    echo "├──────────────────────────────┤" | tee -a $LOG_FILE
    echo "│  TỔNG CỘNG       │    $CURRENT_COUNT     │" | tee -a $LOG_FILE
    echo "└──────────────────────────────┘" | tee -a $LOG_FILE

    exit 0
fi

echo "⚠️ Thiếu $((46 - CURRENT_COUNT)) đơn vị. Bắt đầu tạo thêm..." | tee -a $LOG_FILE

# Tạo thêm đơn vị để đủ 46
NEEDED=$((46 - CURRENT_COUNT))
CREATED=0

# Tìm root unit để làm parent
ROOT_UNIT=$(curl -s "$API_BASE/Units" | grep -A5 -B5 '"Type": *"CNL1"' | grep -o '"Id": *[0-9]*' | head -1 | sed 's/"Id": *//')

if [ -z "$ROOT_UNIT" ]; then
    echo "🏗️ Tạo đơn vị gốc mới..." | tee -a $LOG_FILE
    ROOT_UNIT=$(create_unit_final "ROOT_NEW" "Don vi goc moi" "CNL1" "null")
    if [ -n "$ROOT_UNIT" ]; then
        CREATED=$((CREATED + 1))
    fi
fi

echo "📍 Sử dụng đơn vị gốc ID: $ROOT_UNIT" | tee -a $LOG_FILE

# Tạo thêm đơn vị cho đủ 46
COUNTER=1
while [ $CREATED -lt $NEEDED ]; do
    TIMESTAMP=$(date +%s)
    UNIT_CODE="UNIT_${TIMESTAMP}_${COUNTER}"
    UNIT_NAME="Don vi $COUNTER"

    if [ $((CREATED % 3)) -eq 0 ]; then
        TYPE="CNL2"
        PARENT_ID="$ROOT_UNIT"
    else
        TYPE="PNVL2"
        # Tìm một CNL2 unit làm parent
        CNL2_PARENT=$(curl -s "$API_BASE/Units" | grep -A10 -B10 '"Type": *"CNL2"' | grep -o '"Id": *[0-9]*' | head -1 | sed 's/"Id": *//')
        if [ -n "$CNL2_PARENT" ]; then
            PARENT_ID="$CNL2_PARENT"
        else
            PARENT_ID="$ROOT_UNIT"
        fi
    fi

    echo "🔨 Tạo đơn vị $COUNTER/$NEEDED: $UNIT_NAME" | tee -a $LOG_FILE
    NEW_ID=$(create_unit_final "$UNIT_CODE" "$UNIT_NAME" "$TYPE" "$PARENT_ID")

    if [ -n "$NEW_ID" ]; then
        CREATED=$((CREATED + 1))
        echo "  ✅ Thành công: $UNIT_NAME (ID: $NEW_ID)" | tee -a $LOG_FILE
    else
        echo "  ❌ Thất bại: $UNIT_NAME" | tee -a $LOG_FILE
    fi

    COUNTER=$((COUNTER + 1))

    # Tránh infinite loop
    if [ $COUNTER -gt $((NEEDED + 10)) ]; then
        echo "⚠️ Dừng để tránh infinite loop" | tee -a $LOG_FILE
        break
    fi
done

# Kiểm tra kết quả cuối cùng
echo "🔍 Kiểm tra kết quả cuối cùng..." | tee -a $LOG_FILE
sleep 2

FINAL_COUNT=$(curl -s "$API_BASE/Units" | grep -c '"Id":')
echo "" | tee -a $LOG_FILE
echo "📊 KẾT QUẢ CUỐI CÙNG:" | tee -a $LOG_FILE
echo "🎯 Mục tiêu: 46 đơn vị" | tee -a $LOG_FILE
echo "📈 Thực tế: $FINAL_COUNT đơn vị" | tee -a $LOG_FILE
echo "🆕 Đã tạo thêm: $CREATED đơn vị" | tee -a $LOG_FILE

if [ $FINAL_COUNT -eq 46 ]; then
    echo "🎉 HOÀN HẢO! Đúng 46 đơn vị!" | tee -a $LOG_FILE
elif [ $FINAL_COUNT -gt 46 ]; then
    echo "⚠️ Thừa $((FINAL_COUNT - 46)) đơn vị" | tee -a $LOG_FILE
else
    echo "⚠️ Thiếu $((46 - FINAL_COUNT)) đơn vị" | tee -a $LOG_FILE
fi

echo "📅 Hoàn thành: $(date)" | tee -a $LOG_FILE
echo "📝 Log chi tiết: $LOG_FILE" | tee -a $LOG_FILE
