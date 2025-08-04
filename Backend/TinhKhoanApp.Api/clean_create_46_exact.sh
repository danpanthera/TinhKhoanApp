#!/bin/bash

# Script tạo CHÍNH XÁC 46 đơn vị - Clean & Rebuild
API_BASE="http://localhost:5055/api"
LOG_FILE="clean_create_46_exact.log"

echo "🎯 TẠO CHÍNH XÁC 46 ĐƠN VỊ - CLEAN & REBUILD" | tee -a $LOG_FILE
echo "📅 Bắt đầu: $(date)" | tee -a $LOG_FILE

# BƯỚC 1: Xóa toàn bộ đơn vị hiện tại
echo "🧹 BƯỚC 1: XÓA TOÀN BỘ ĐƠN VỊ CŨ" | tee -a $LOG_FILE

# Get all units
ALL_UNITS=$(curl -s "$API_BASE/Units")
UNIT_IDS=$(echo "$ALL_UNITS" | grep -o '"Id":[0-9]*' | cut -d':' -f2)

DELETE_COUNT=0
for ID in $UNIT_IDS; do
    echo "Đang xóa đơn vị ID: $ID" | tee -a $LOG_FILE
    curl -s -X DELETE "$API_BASE/Units/$ID" > /dev/null
    DELETE_COUNT=$((DELETE_COUNT + 1))
done

echo "✅ Đã xóa $DELETE_COUNT đơn vị" | tee -a $LOG_FILE

# Wait for deletion to complete
sleep 2

# Verify deletion
REMAINING=$(curl -s "$API_BASE/Units" | grep -c '"Id":')
echo "Còn lại trong hệ thống: $REMAINING đơn vị" | tee -a $LOG_FILE

# BƯỚC 2: Tạo cấu trúc mới với chính xác 46 đơn vị
echo "🏗️ BƯỚC 2: TẠO CẤU TRÚC MỚI 46 ĐƠN VỊ" | tee -a $LOG_FILE

# Function to create unit and return ID
create_unit_clean() {
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
    ID=$(echo "$RESPONSE" | grep -o '"Id":[0-9]*' | head -1 | cut -d':' -f2)

    if [ -n "$ID" ]; then
        echo "✅ Tạo: $name (ID: $ID)" | tee -a $LOG_FILE
        echo "$ID"
    else
        echo "❌ Lỗi tạo: $name" | tee -a $LOG_FILE
        echo ""
    fi
}

# 1. Tạo đơn vị gốc (1 đơn vị)
echo "1️⃣ Tạo đơn vị gốc..." | tee -a $LOG_FILE
ROOT_ID=$(create_unit_clean "CNLC_VER2" "Chi nhanh Lai Chau Ver2" "CNL1" "null")

if [ -z "$ROOT_ID" ]; then
    echo "❌ Không thể tạo đơn vị gốc!" | tee -a $LOG_FILE
    exit 1
fi

# 2. Tạo 9 chi nhánh
echo "2️⃣ Tạo 9 chi nhánh..." | tee -a $LOG_FILE
HS_ID=$(create_unit_clean "HS_VER2" "Hoi So" "CNL2" "$ROOT_ID")
BL_ID=$(create_unit_clean "BL_VER2" "CN Binh Lu" "CNL2" "$ROOT_ID")
PT_ID=$(create_unit_clean "PT_VER2" "CN Phong Tho" "CNL2" "$ROOT_ID")
SH_ID=$(create_unit_clean "SH_VER2" "CN Sin Ho" "CNL2" "$ROOT_ID")
BT_ID=$(create_unit_clean "BT_VER2" "CN Bum To" "CNL2" "$ROOT_ID")
TU_ID=$(create_unit_clean "TU_VER2" "CN Than Uyen" "CNL2" "$ROOT_ID")
DK_ID=$(create_unit_clean "DK_VER2" "CN Doan Ket" "CNL2" "$ROOT_ID")
TUY_ID=$(create_unit_clean "TUY_VER2" "CN Tan Uyen" "CNL2" "$ROOT_ID")
NH_ID=$(create_unit_clean "NH_VER2" "CN Nam Hang" "CNL2" "$ROOT_ID")

# 3. Tạo phòng ban cho Hội sở (7 phòng)
echo "3️⃣ Tạo 7 phòng ban Hội sở..." | tee -a $LOG_FILE
if [ -n "$HS_ID" ]; then
    create_unit_clean "HS_BGD" "Ban Giam Doc" "PNVL1" "$HS_ID"
    create_unit_clean "HS_KTNQ" "P KTNQ" "PNVL1" "$HS_ID"
    create_unit_clean "HS_KHDN" "P KHDN" "PNVL1" "$HS_ID"
    create_unit_clean "HS_KHCN" "P KHCN" "PNVL1" "$HS_ID"
    create_unit_clean "HS_KTGS" "P KTGS" "PNVL1" "$HS_ID"
    create_unit_clean "HS_TH" "P Tong Hop" "PNVL1" "$HS_ID"
    create_unit_clean "HS_KHQLRR" "P KHQLRR" "PNVL1" "$HS_ID"
fi

# 4. Tạo phòng ban cho 8 chi nhánh còn lại (29 phòng)
echo "4️⃣ Tạo phòng ban cho 8 chi nhánh..." | tee -a $LOG_FILE

# CN Bình Lư (3 phòng)
if [ -n "$BL_ID" ]; then
    create_unit_clean "BL_BGD" "Ban Giam Doc" "PNVL2" "$BL_ID"
    create_unit_clean "BL_KTNQ" "P KTNQ" "PNVL2" "$BL_ID"
    create_unit_clean "BL_KH" "P KH" "PNVL2" "$BL_ID"
fi

# CN Phong Thổ (4 phòng)
if [ -n "$PT_ID" ]; then
    create_unit_clean "PT_BGD" "Ban Giam Doc" "PNVL2" "$PT_ID"
    create_unit_clean "PT_KTNQ" "P KTNQ" "PNVL2" "$PT_ID"
    create_unit_clean "PT_KH" "P KH" "PNVL2" "$PT_ID"
    create_unit_clean "PT_PGD5" "PGD So 5" "PGDL2" "$PT_ID"
fi

# CN Sìn Hồ (3 phòng)
if [ -n "$SH_ID" ]; then
    create_unit_clean "SH_BGD" "Ban Giam Doc" "PNVL2" "$SH_ID"
    create_unit_clean "SH_KTNQ" "P KTNQ" "PNVL2" "$SH_ID"
    create_unit_clean "SH_KH" "P KH" "PNVL2" "$SH_ID"
fi

# CN Bum Tở (3 phòng)
if [ -n "$BT_ID" ]; then
    create_unit_clean "BT_BGD" "Ban Giam Doc" "PNVL2" "$BT_ID"
    create_unit_clean "BT_KTNQ" "P KTNQ" "PNVL2" "$BT_ID"
    create_unit_clean "BT_KH" "P KH" "PNVL2" "$BT_ID"
fi

# CN Than Uyên (4 phòng)
if [ -n "$TU_ID" ]; then
    create_unit_clean "TU_BGD" "Ban Giam Doc" "PNVL2" "$TU_ID"
    create_unit_clean "TU_KTNQ" "P KTNQ" "PNVL2" "$TU_ID"
    create_unit_clean "TU_KH" "P KH" "PNVL2" "$TU_ID"
    create_unit_clean "TU_PGD6" "PGD So 6" "PGDL2" "$TU_ID"
fi

# CN Đoàn Kết (5 phòng)
if [ -n "$DK_ID" ]; then
    create_unit_clean "DK_BGD" "Ban Giam Doc" "PNVL2" "$DK_ID"
    create_unit_clean "DK_KTNQ" "P KTNQ" "PNVL2" "$DK_ID"
    create_unit_clean "DK_KH" "P KH" "PNVL2" "$DK_ID"
    create_unit_clean "DK_PGD1" "PGD So 1" "PGDL2" "$DK_ID"
    create_unit_clean "DK_PGD2" "PGD So 2" "PGDL2" "$DK_ID"
fi

# CN Tân Uyên (4 phòng)
if [ -n "$TUY_ID" ]; then
    create_unit_clean "TUY_BGD" "Ban Giam Doc" "PNVL2" "$TUY_ID"
    create_unit_clean "TUY_KTNQ" "P KTNQ" "PNVL2" "$TUY_ID"
    create_unit_clean "TUY_KH" "P KH" "PNVL2" "$TUY_ID"
    create_unit_clean "TUY_PGD3" "PGD So 3" "PGDL2" "$TUY_ID"
fi

# CN Nậm Hàng (3 phòng)
if [ -n "$NH_ID" ]; then
    create_unit_clean "NH_BGD" "Ban Giam Doc" "PNVL2" "$NH_ID"
    create_unit_clean "NH_KTNQ" "P KTNQ" "PNVL2" "$NH_ID"
    create_unit_clean "NH_KH" "P KH" "PNVL2" "$NH_ID"
fi

# BƯỚC 3: Kiểm tra kết quả cuối cùng
echo "🔍 BƯỚC 3: KIỂM TRA KẾT QUẢ" | tee -a $LOG_FILE
sleep 3

ALL_UNITS_FINAL=$(curl -s "$API_BASE/Units")
FINAL_COUNT=$(echo "$ALL_UNITS_FINAL" | grep -c '"Id":')

CNL1_COUNT=$(echo "$ALL_UNITS_FINAL" | grep -c '"Type":"CNL1"')
CNL2_COUNT=$(echo "$ALL_UNITS_FINAL" | grep -c '"Type":"CNL2"')
PNVL1_COUNT=$(echo "$ALL_UNITS_FINAL" | grep -c '"Type":"PNVL1"')
PNVL2_COUNT=$(echo "$ALL_UNITS_FINAL" | grep -c '"Type":"PNVL2"')
PGDL2_COUNT=$(echo "$ALL_UNITS_FINAL" | grep -c '"Type":"PGDL2"')

echo "📊 KẾT QUẢ CUỐI CÙNG:" | tee -a $LOG_FILE
echo "┌──────────────────────────────────────┐" | tee -a $LOG_FILE
echo "│  LOẠI      │  MỤC TIÊU  │  THỰC TẾ  │" | tee -a $LOG_FILE
echo "├──────────────────────────────────────┤" | tee -a $LOG_FILE
echo "│  CNL1      │     1      │    $CNL1_COUNT     │" | tee -a $LOG_FILE
echo "│  CNL2      │     9      │    $CNL2_COUNT     │" | tee -a $LOG_FILE
echo "│  PNVL1     │     7      │    $PNVL1_COUNT     │" | tee -a $LOG_FILE
echo "│  PNVL2     │    24      │    $PNVL2_COUNT    │" | tee -a $LOG_FILE
echo "│  PGDL2     │     5      │    $PGDL2_COUNT     │" | tee -a $LOG_FILE
echo "├──────────────────────────────────────┤" | tee -a $LOG_FILE
echo "│  TỔNG      │    46      │    $FINAL_COUNT    │" | tee -a $LOG_FILE
echo "└──────────────────────────────────────┘" | tee -a $LOG_FILE

if [ "$FINAL_COUNT" -eq 46 ]; then
    echo "🎉 HOÀN HẢO! Đã tạo chính xác 46 đơn vị!" | tee -a $LOG_FILE
    echo "✅ Cấu trúc tổ chức Ver2 hoàn thành" | tee -a $LOG_FILE
elif [ "$FINAL_COUNT" -lt 46 ]; then
    echo "⚠️ Thiếu $((46 - FINAL_COUNT)) đơn vị" | tee -a $LOG_FILE
else
    echo "⚠️ Thừa $((FINAL_COUNT - 46)) đơn vị" | tee -a $LOG_FILE
fi

echo "📅 Hoàn thành: $(date)" | tee -a $LOG_FILE
echo "🔚 KẾT THÚC" | tee -a $LOG_FILE
