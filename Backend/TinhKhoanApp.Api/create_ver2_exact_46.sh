#!/bin/bash

# Script tạo chính xác cấu trúc Ver2: 46 đơn vị (1+9+7+24+5)
API_BASE="http://localhost:5055/api"
LOG_FILE="create_exact_ver2_structure_$(date +%Y%m%d_%H%M%S).log"

echo "🎯 TẠO CẤU TRÚC VER2 CHÍNH XÁC: 46 ĐƠN VỊ (1+9+7+24+5)" | tee -a $LOG_FILE
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
        echo ""
    fi
}

# BƯỚC 1: Xóa toàn bộ đơn vị hiện tại
echo "🧹 BƯỚC 1: XÓA TOÀN BỘ ĐƠN VỊ HIỆN TẠI" | tee -a $LOG_FILE

ALL_UNITS=$(curl -s "$API_BASE/Units")
UNIT_IDS=$(echo "$ALL_UNITS" | grep -o '"Id": *[0-9]*' | sed 's/"Id": *//')

DELETED_COUNT=0
for ID in $UNIT_IDS; do
    echo "Xóa đơn vị ID: $ID" >> $LOG_FILE
    DELETE_RESPONSE=$(curl -s -X DELETE "$API_BASE/Units/$ID")
    DELETED_COUNT=$((DELETED_COUNT + 1))
done

echo "✅ Đã xóa $DELETED_COUNT đơn vị" | tee -a $LOG_FILE
sleep 3

# Kiểm tra sau khi xóa
REMAINING=$(curl -s "$API_BASE/Units" | grep -c '"Id":')
echo "Còn lại: $REMAINING đơn vị" | tee -a $LOG_FILE

# BƯỚC 2: Tạo cấu trúc Ver2 chính xác
echo "" | tee -a $LOG_FILE
echo "🏗️ BƯỚC 2: TẠO CẤU TRÚC VER2 CHÍNH XÁC" | tee -a $LOG_FILE

# 1. Tạo 1 đơn vị gốc CNL1
echo "1️⃣ Tạo 1 đơn vị gốc CNL1..." | tee -a $LOG_FILE
ROOT_ID=$(create_unit "CNLC_VER2_ROOT" "Chi nhanh Lai Chau Ver2" "CNL1" "null")

if [ -z "$ROOT_ID" ]; then
    echo "❌ Không thể tạo đơn vị gốc!" | tee -a $LOG_FILE
    exit 1
fi

# 2. Tạo 9 chi nhánh CNL2
echo "2️⃣ Tạo 9 chi nhánh CNL2..." | tee -a $LOG_FILE
HS_ID=$(create_unit "HS_VER2" "Hoi So" "CNL2" "$ROOT_ID")
BL_ID=$(create_unit "BL_VER2" "CN Binh Lu" "CNL2" "$ROOT_ID")
PT_ID=$(create_unit "PT_VER2" "CN Phong Tho" "CNL2" "$ROOT_ID")
SH_ID=$(create_unit "SH_VER2" "CN Sin Ho" "CNL2" "$ROOT_ID")
BT_ID=$(create_unit "BT_VER2" "CN Bum To" "CNL2" "$ROOT_ID")
TU_ID=$(create_unit "TU_VER2" "CN Than Uyen" "CNL2" "$ROOT_ID")
DK_ID=$(create_unit "DK_VER2" "CN Doan Ket" "CNL2" "$ROOT_ID")
TUY_ID=$(create_unit "TUY_VER2" "CN Tan Uyen" "CNL2" "$ROOT_ID")
NH_ID=$(create_unit "NH_VER2" "CN Nam Hang" "CNL2" "$ROOT_ID")

# 3. Tạo 7 phòng Hội sở PNVL1
echo "3️⃣ Tạo 7 phòng Hội sở PNVL1..." | tee -a $LOG_FILE
if [ -n "$HS_ID" ]; then
    create_unit "HS_BGD" "Ban Giam Doc" "PNVL1" "$HS_ID"
    create_unit "HS_KTNQ" "P KTNQ" "PNVL1" "$HS_ID"
    create_unit "HS_KHDN" "P KHDN" "PNVL1" "$HS_ID"
    create_unit "HS_KHCN" "P KHCN" "PNVL1" "$HS_ID"
    create_unit "HS_KTGS" "P KTGS" "PNVL1" "$HS_ID"
    create_unit "HS_TH" "P Tong Hop" "PNVL1" "$HS_ID"
    create_unit "HS_KHQLRR" "P KHQLRR" "PNVL1" "$HS_ID"
fi

# 4. Tạo 24 phòng chi nhánh PNVL2 (3 phòng mỗi chi nhánh x 8 chi nhánh)
echo "4️⃣ Tạo 24 phòng chi nhánh PNVL2..." | tee -a $LOG_FILE

# CN Bình Lư (3 phòng)
if [ -n "$BL_ID" ]; then
    create_unit "BL_BGD" "Ban Giam Doc" "PNVL2" "$BL_ID"
    create_unit "BL_KTNQ" "P KTNQ" "PNVL2" "$BL_ID"
    create_unit "BL_KH" "P KH" "PNVL2" "$BL_ID"
fi

# CN Phong Thổ (3 phòng)
if [ -n "$PT_ID" ]; then
    create_unit "PT_BGD" "Ban Giam Doc" "PNVL2" "$PT_ID"
    create_unit "PT_KTNQ" "P KTNQ" "PNVL2" "$PT_ID"
    create_unit "PT_KH" "P KH" "PNVL2" "$PT_ID"
fi

# CN Sìn Hồ (3 phòng)
if [ -n "$SH_ID" ]; then
    create_unit "SH_BGD" "Ban Giam Doc" "PNVL2" "$SH_ID"
    create_unit "SH_KTNQ" "P KTNQ" "PNVL2" "$SH_ID"
    create_unit "SH_KH" "P KH" "PNVL2" "$SH_ID"
fi

# CN Bum Tở (3 phòng)
if [ -n "$BT_ID" ]; then
    create_unit "BT_BGD" "Ban Giam Doc" "PNVL2" "$BT_ID"
    create_unit "BT_KTNQ" "P KTNQ" "PNVL2" "$BT_ID"
    create_unit "BT_KH" "P KH" "PNVL2" "$BT_ID"
fi

# CN Than Uyên (3 phòng)
if [ -n "$TU_ID" ]; then
    create_unit "TU_BGD" "Ban Giam Doc" "PNVL2" "$TU_ID"
    create_unit "TU_KTNQ" "P KTNQ" "PNVL2" "$TU_ID"
    create_unit "TU_KH" "P KH" "PNVL2" "$TU_ID"
fi

# CN Đoàn Kết (3 phòng)
if [ -n "$DK_ID" ]; then
    create_unit "DK_BGD" "Ban Giam Doc" "PNVL2" "$DK_ID"
    create_unit "DK_KTNQ" "P KTNQ" "PNVL2" "$DK_ID"
    create_unit "DK_KH" "P KH" "PNVL2" "$DK_ID"
fi

# CN Tân Uyên (3 phòng)
if [ -n "$TUY_ID" ]; then
    create_unit "TUY_BGD" "Ban Giam Doc" "PNVL2" "$TUY_ID"
    create_unit "TUY_KTNQ" "P KTNQ" "PNVL2" "$TUY_ID"
    create_unit "TUY_KH" "P KH" "PNVL2" "$TUY_ID"
fi

# CN Nậm Hàng (3 phòng)
if [ -n "$NH_ID" ]; then
    create_unit "NH_BGD" "Ban Giam Doc" "PNVL2" "$NH_ID"
    create_unit "NH_KTNQ" "P KTNQ" "PNVL2" "$NH_ID"
    create_unit "NH_KH" "P KH" "PNVL2" "$NH_ID"
fi

# 5. Tạo 5 phòng giao dịch PGDL2
echo "5️⃣ Tạo 5 phòng giao dịch PGDL2..." | tee -a $LOG_FILE

# PGD tại CN Phong Thổ
if [ -n "$PT_ID" ]; then
    create_unit "PT_PGD5" "PGD So 5" "PGDL2" "$PT_ID"
fi

# PGD tại CN Than Uyên
if [ -n "$TU_ID" ]; then
    create_unit "TU_PGD6" "PGD So 6" "PGDL2" "$TU_ID"
fi

# PGD tại CN Đoàn Kết (2 PGD)
if [ -n "$DK_ID" ]; then
    create_unit "DK_PGD1" "PGD So 1" "PGDL2" "$DK_ID"
    create_unit "DK_PGD2" "PGD So 2" "PGDL2" "$DK_ID"
fi

# PGD tại CN Tân Uyên
if [ -n "$TUY_ID" ]; then
    create_unit "TUY_PGD3" "PGD So 3" "PGDL2" "$TUY_ID"
fi

# BƯỚC 3: Kiểm tra kết quả Ver2
echo "" | tee -a $LOG_FILE
echo "🔍 BƯỚC 3: KIỂM TRA KẾT QUẢ VER2" | tee -a $LOG_FILE
sleep 3

ALL_UNITS_FINAL=$(curl -s "$API_BASE/Units")
FINAL_COUNT=$(echo "$ALL_UNITS_FINAL" | grep -c '"Id":')

CNL1_COUNT=$(echo "$ALL_UNITS_FINAL" | grep -c '"Type": *"CNL1"')
CNL2_COUNT=$(echo "$ALL_UNITS_FINAL" | grep -c '"Type": *"CNL2"')
PNVL1_COUNT=$(echo "$ALL_UNITS_FINAL" | grep -c '"Type": *"PNVL1"')
PNVL2_COUNT=$(echo "$ALL_UNITS_FINAL" | grep -c '"Type": *"PNVL2"')
PGDL2_COUNT=$(echo "$ALL_UNITS_FINAL" | grep -c '"Type": *"PGDL2"')

echo "📊 KẾT QUẢ CẤU TRÚC VER2:" | tee -a $LOG_FILE
echo "┌─────────────────────────────────────────┐" | tee -a $LOG_FILE
echo "│  LOẠI     │  MỤC TIÊU  │  THỰC TẾ  │ ✓│" | tee -a $LOG_FILE
echo "├─────────────────────────────────────────┤" | tee -a $LOG_FILE
echo "│  CNL1     │     1      │    $CNL1_COUNT     │$([ $CNL1_COUNT -eq 1 ] && echo " ✅" || echo " ❌")│" | tee -a $LOG_FILE
echo "│  CNL2     │     9      │    $CNL2_COUNT     │$([ $CNL2_COUNT -eq 9 ] && echo " ✅" || echo " ❌")│" | tee -a $LOG_FILE
echo "│  PNVL1    │     7      │    $PNVL1_COUNT     │$([ $PNVL1_COUNT -eq 7 ] && echo " ✅" || echo " ❌")│" | tee -a $LOG_FILE
echo "│  PNVL2    │    24      │   $PNVL2_COUNT    │$([ $PNVL2_COUNT -eq 24 ] && echo " ✅" || echo " ❌")│" | tee -a $LOG_FILE
echo "│  PGDL2    │     5      │    $PGDL2_COUNT     │$([ $PGDL2_COUNT -eq 5 ] && echo " ✅" || echo " ❌")│" | tee -a $LOG_FILE
echo "├─────────────────────────────────────────┤" | tee -a $LOG_FILE
echo "│  TỔNG     │    46      │   $FINAL_COUNT    │$([ $FINAL_COUNT -eq 46 ] && echo " ✅" || echo " ❌")│" | tee -a $LOG_FILE
echo "└─────────────────────────────────────────┘" | tee -a $LOG_FILE

echo "" | tee -a $LOG_FILE
if [ $FINAL_COUNT -eq 46 ] && [ $CNL1_COUNT -eq 1 ] && [ $CNL2_COUNT -eq 9 ] && [ $PNVL1_COUNT -eq 7 ] && [ $PNVL2_COUNT -eq 24 ] && [ $PGDL2_COUNT -eq 5 ]; then
    echo "🎉 HOÀN HẢO! Cấu trúc Ver2 chính xác 46 đơn vị!" | tee -a $LOG_FILE
    echo "✅ CNL1: $CNL1_COUNT/1 ✅ CNL2: $CNL2_COUNT/9 ✅ PNVL1: $PNVL1_COUNT/7 ✅ PNVL2: $PNVL2_COUNT/24 ✅ PGDL2: $PGDL2_COUNT/5" | tee -a $LOG_FILE
else
    echo "⚠️ Chưa đạt cấu trúc Ver2 mong muốn" | tee -a $LOG_FILE
    echo "CNL1: $CNL1_COUNT/1, CNL2: $CNL2_COUNT/9, PNVL1: $PNVL1_COUNT/7, PNVL2: $PNVL2_COUNT/24, PGDL2: $PGDL2_COUNT/5" | tee -a $LOG_FILE
fi

echo "📅 Hoàn thành: $(date)" | tee -a $LOG_FILE
echo "📝 Log chi tiết: $LOG_FILE" | tee -a $LOG_FILE
echo "🔚 KẾT THÚC" | tee -a $LOG_FILE
