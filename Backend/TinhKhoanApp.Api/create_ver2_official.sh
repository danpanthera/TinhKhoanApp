#!/bin/bash

# Script tạo cấu trúc đơn vị Ver2 hoàn chỉnh theo danh sách chính thức
API_BASE="http://localhost:5055/api"
LOG_FILE="create_ver2_official_structure_$(date +%Y%m%d_%H%M%S).log"

echo "🎯 TẠO CẤU TRÚC ĐƠN VỊ VER2 CHÍNH THỨC - 46 ĐƠN VỊ" | tee -a $LOG_FILE
echo "📅 Bắt đầu: $(date)" | tee -a $LOG_FILE

# Hàm xóa tất cả dữ liệu
delete_all_data() {
    echo "🧹 XÓA TOÀN BỘ DỮ LIỆU CŨ..." | tee -a $LOG_FILE

    # Xóa employees trước (foreign key)
    EMPLOYEES=$(curl -s "$API_BASE/Employees")
    EMP_IDS=$(echo "$EMPLOYEES" | grep -o '"Id": *[0-9]*' | sed 's/"Id": *//')
    EMP_COUNT=0
    for ID in $EMP_IDS; do
        curl -s -X DELETE "$API_BASE/Employees/$ID" > /dev/null
        EMP_COUNT=$((EMP_COUNT + 1))
    done
    echo "✅ Đã xóa $EMP_COUNT nhân viên" | tee -a $LOG_FILE

    # Xóa units
    UNITS=$(curl -s "$API_BASE/Units")
    UNIT_IDS=$(echo "$UNITS" | grep -o '"Id": *[0-9]*' | sed 's/"Id": *//')
    UNIT_COUNT=0
    for ID in $UNIT_IDS; do
        curl -s -X DELETE "$API_BASE/Units/$ID" > /dev/null
        UNIT_COUNT=$((UNIT_COUNT + 1))
    done
    echo "✅ Đã xóa $UNIT_COUNT đơn vị" | tee -a $LOG_FILE

    # Xóa positions
    POSITIONS=$(curl -s "$API_BASE/Positions")
    POS_IDS=$(echo "$POSITIONS" | grep -o '"Id": *[0-9]*' | sed 's/"Id": *//')
    POS_COUNT=0
    for ID in $POS_IDS; do
        curl -s -X DELETE "$API_BASE/Positions/$ID" > /dev/null
        POS_COUNT=$((POS_COUNT + 1))
    done
    echo "✅ Đã xóa $POS_COUNT chức vụ" | tee -a $LOG_FILE

    sleep 3
}

# Hàm tạo đơn vị
create_unit() {
    local code="$1"
    local name="$2"
    local type="$3"
    local parent_id="$4"

    local parent_json=""
    if [ "$parent_id" = "null" ]; then
        parent_json=', "ParentUnitId": null'
    else
        parent_json=', "ParentUnitId": $parent_id'
    fi

    local json='{"Code": "$code", "Name": "$name", "Type": "$type"$parent_json}'

    log "⏳ Đang tạo: $name ($type)"
    log "📝 JSON: $json"

    local response=$(curl -s -X POST "$API_BASE/Units" \
        -H "Content-Type: application/json" \
        -d "$json")

    log "📥 Response: $response"

    local unit_id=""
    if [ -n "$response" ] && [[ "$response" == *"Id"* ]]; then
        unit_id=$(echo "$response" | jq -r '.Id // empty' 2>/dev/null)
    fi

    if [ -n "$unit_id" ] && [ "$unit_id" != "null" ]; then
        log "✅ Thành công: $name - ID: $unit_id"
        echo "$unit_id"
    else
        log "❌ Thất bại: $name - Response: $response"
        echo ""
    fi
}

# BƯỚC 1: Xóa toàn bộ dữ liệu cũ
delete_all_data

# BƯỚC 2: Tạo cấu trúc Ver2 chính thức
echo "" | tee -a $LOG_FILE
echo "🏗️ BƯỚC 2: TẠO CẤU TRÚC VER2 CHÍNH THỨC" | tee -a $LOG_FILE

# 1. Tạo Chi nhánh Lai Châu (LV1) - Root
echo "1️⃣ Tạo Chi nhánh Lai Châu (LV1)..." | tee -a $LOG_FILE
ROOT_ID=$(create_unit "CNLC" "Chi nhanh Lai Chau" "CNL1" "null")

if [ -z "$ROOT_ID" ]; then
    echo "❌ Không thể tạo đơn vị gốc!" | tee -a $LOG_FILE
    exit 1
fi

# 2. Tạo Hội sở (LV2)
echo "2️⃣ Tạo Hội sở (LV2)..." | tee -a $LOG_FILE
HS_ID=$(create_unit "HS" "Hoi So" "CNL2" "$ROOT_ID")

# 3. Tạo 8 Chi nhánh cấp 2 (LV2)
echo "3️⃣ Tạo 8 Chi nhánh cấp 2 (LV2)..." | tee -a $LOG_FILE
BL_ID=$(create_unit "BL" "CN Binh Lu" "CNL2" "$ROOT_ID")
PT_ID=$(create_unit "PT" "CN Phong Tho" "CNL2" "$ROOT_ID")
SH_ID=$(create_unit "SH" "CN Sin Ho" "CNL2" "$ROOT_ID")
BT_ID=$(create_unit "BT" "CN Bum To" "CNL2" "$ROOT_ID")
TU_ID=$(create_unit "TU" "CN Than Uyen" "CNL2" "$ROOT_ID")
DK_ID=$(create_unit "DK" "CN Doan Ket" "CNL2" "$ROOT_ID")
TUY_ID=$(create_unit "TUY" "CN Tan Uyen" "CNL2" "$ROOT_ID")
NH_ID=$(create_unit "NH" "CN Nam Hang" "CNL2" "$ROOT_ID")

# 4. Tạo 7 Phòng NVL1 cho Hội sở
echo "4️⃣ Tạo 7 Phòng NVL1 cho Hội sở..." | tee -a $LOG_FILE
if [ -n "$HS_ID" ]; then
    create_unit "HS_BGD" "Ban Giam doc" "PNVL1" "$HS_ID"
    create_unit "HS_KTNQ" "P KTNQ" "PNVL1" "$HS_ID"
    create_unit "HS_KHDN" "P KHDN" "PNVL1" "$HS_ID"
    create_unit "HS_KHCN" "P KHCN" "PNVL1" "$HS_ID"
    create_unit "HS_KTGS" "P KTGS" "PNVL1" "$HS_ID"
    create_unit "HS_TH" "P Tong Hop" "PNVL1" "$HS_ID"
    create_unit "HS_KHQLRR" "P KHQLRR" "PNVL1" "$HS_ID"
fi

# 5. Tạo Phòng NVL2 cho CN Bình Lư (3 phòng)
echo "5️⃣ Tạo 3 Phòng NVL2 cho CN Bình Lư..." | tee -a $LOG_FILE
if [ -n "$BL_ID" ]; then
    create_unit "BL_BGD" "Ban Giam doc" "PNVL2" "$BL_ID"
    create_unit "BL_KTNQ" "P KTNQ" "PNVL2" "$BL_ID"
    create_unit "BL_KH" "P KH" "PNVL2" "$BL_ID"
fi

# 6. Tạo Phòng NVL2 cho CN Phong Thổ (4 phòng: 3 PNVL2 + 1 PGDL2)
echo "6️⃣ Tạo 4 Phòng cho CN Phong Thổ..." | tee -a $LOG_FILE
if [ -n "$PT_ID" ]; then
    create_unit "PT_BGD" "Ban Giam doc" "PNVL2" "$PT_ID"
    create_unit "PT_KTNQ" "P KTNQ" "PNVL2" "$PT_ID"
    create_unit "PT_KH" "P KH" "PNVL2" "$PT_ID"
    create_unit "PT_PGD5" "PGD So 5" "PGDL2" "$PT_ID"
fi

# 7. Tạo Phòng NVL2 cho CN Sìn Hồ (3 phòng)
echo "7️⃣ Tạo 3 Phòng NVL2 cho CN Sìn Hồ..." | tee -a $LOG_FILE
if [ -n "$SH_ID" ]; then
    create_unit "SH_BGD" "Ban Giam doc" "PNVL2" "$SH_ID"
    create_unit "SH_KTNQ" "P KTNQ" "PNVL2" "$SH_ID"
    create_unit "SH_KH" "P KH" "PNVL2" "$SH_ID"
fi

# 8. Tạo Phòng NVL2 cho CN Bum Tở (3 phòng)
echo "8️⃣ Tạo 3 Phòng NVL2 cho CN Bum Tở..." | tee -a $LOG_FILE
if [ -n "$BT_ID" ]; then
    create_unit "BT_BGD" "Ban Giam doc" "PNVL2" "$BT_ID"
    create_unit "BT_KTNQ" "P KTNQ" "PNVL2" "$BT_ID"
    create_unit "BT_KH" "P KH" "PNVL2" "$BT_ID"
fi

# 9. Tạo Phòng NVL2 cho CN Than Uyên (4 phòng: 3 PNVL2 + 1 PGDL2)
echo "9️⃣ Tạo 4 Phòng cho CN Than Uyên..." | tee -a $LOG_FILE
if [ -n "$TU_ID" ]; then
    create_unit "TU_BGD" "Ban Giam doc" "PNVL2" "$TU_ID"
    create_unit "TU_KTNQ" "P KTNQ" "PNVL2" "$TU_ID"
    create_unit "TU_KH" "P KH" "PNVL2" "$TU_ID"
    create_unit "TU_PGD6" "PGD So 6" "PGDL2" "$TU_ID"
fi

# 10. Tạo Phòng NVL2 cho CN Đoàn Kết (5 phòng: 3 PNVL2 + 2 PGDL2)
echo "🔟 Tạo 5 Phòng cho CN Đoàn Kết..." | tee -a $LOG_FILE
if [ -n "$DK_ID" ]; then
    create_unit "DK_BGD" "Ban Giam doc" "PNVL2" "$DK_ID"
    create_unit "DK_KTNQ" "P KTNQ" "PNVL2" "$DK_ID"
    create_unit "DK_KH" "P KH" "PNVL2" "$DK_ID"
    create_unit "DK_PGD1" "PGD So 1" "PGDL2" "$DK_ID"
    create_unit "DK_PGD2" "PGD So 2" "PGDL2" "$DK_ID"
fi

# 11. Tạo Phòng NVL2 cho CN Tân Uyên (4 phòng: 3 PNVL2 + 1 PGDL2)
echo "1️⃣1️⃣ Tạo 4 Phòng cho CN Tân Uyên..." | tee -a $LOG_FILE
if [ -n "$TUY_ID" ]; then
    create_unit "TUY_BGD" "Ban Giam doc" "PNVL2" "$TUY_ID"
    create_unit "TUY_KTNQ" "P KTNQ" "PNVL2" "$TUY_ID"
    create_unit "TUY_KH" "P KH" "PNVL2" "$TUY_ID"
    create_unit "TUY_PGD3" "PGD So 3" "PGDL2" "$TUY_ID"
fi

# 12. Tạo Phòng NVL2 cho CN Nậm Hàng (3 phòng)
echo "1️⃣2️⃣ Tạo 3 Phòng NVL2 cho CN Nậm Hàng..." | tee -a $LOG_FILE
if [ -n "$NH_ID" ]; then
    create_unit "NH_BGD" "Ban Giam doc" "PNVL2" "$NH_ID"
    create_unit "NH_KTNQ" "P KTNQ" "PNVL2" "$NH_ID"
    create_unit "NH_KH" "P KH" "PNVL2" "$NH_ID"
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

echo "📊 KẾT QUẢ CẤU TRÚC VER2 CHÍNH THỨC:" | tee -a $LOG_FILE
echo "┌─────────────────────────────────────────────────────────┐" | tee -a $LOG_FILE
echo "│  LOẠI           │  MỤC TIÊU  │  THỰC TẾ  │     MÔ TẢ    │" | tee -a $LOG_FILE
echo "├─────────────────────────────────────────────────────────┤" | tee -a $LOG_FILE
echo "│  CNL1 (Root)     │     1      │    $CNL1_COUNT     │ Lai Châu     │" | tee -a $LOG_FILE
echo "│  CNL2 (CN)       │     9      │    $CNL2_COUNT     │ HS + 8 CN    │" | tee -a $LOG_FILE
echo "│  PNVL1 (P.HS)    │     7      │    $PNVL1_COUNT     │ Phòng HS     │" | tee -a $LOG_FILE
echo "│  PNVL2 (P.CN)    │    24      │   $PNVL2_COUNT    │ Phòng CN     │" | tee -a $LOG_FILE
echo "│  PGDL2 (PGD)     │     5      │    $PGDL2_COUNT     │ PGD          │" | tee -a $LOG_FILE
echo "├─────────────────────────────────────────────────────────┤" | tee -a $LOG_FILE
echo "│  TỔNG CỘNG       │    46      │   $FINAL_COUNT    │ Ver2 Total   │" | tee -a $LOG_FILE
echo "└─────────────────────────────────────────────────────────┘" | tee -a $LOG_FILE

echo "" | tee -a $LOG_FILE
if [ $FINAL_COUNT -eq 46 ] && [ $CNL1_COUNT -eq 1 ] && [ $CNL2_COUNT -eq 9 ] && [ $PNVL1_COUNT -eq 7 ] && [ $PNVL2_COUNT -eq 24 ] && [ $PGDL2_COUNT -eq 5 ]; then
    echo "🎉 HOÀN HẢO! Cấu trúc Ver2 chính thức hoàn thành!" | tee -a $LOG_FILE
    echo "✅ Đúng 46 đơn vị theo đúng phân bổ chính thức" | tee -a $LOG_FILE
else
    echo "⚠️ Kết quả chưa đạt cấu trúc Ver2 mong muốn" | tee -a $LOG_FILE
    echo "📊 CNL1: $CNL1_COUNT/1, CNL2: $CNL2_COUNT/9, PNVL1: $PNVL1_COUNT/7, PNVL2: $PNVL2_COUNT/24, PGDL2: $PGDL2_COUNT/5" | tee -a $LOG_FILE
fi

echo "" | tee -a $LOG_FILE
echo "🎯 DANH SÁCH ĐƠN VỊ VER2 ĐÃ TẠO:" | tee -a $LOG_FILE
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" | tee -a $LOG_FILE
echo "🏢 Chi nhánh Lai Châu (LV1)" | tee -a $LOG_FILE
echo "├── 🏛️ Hội sở (LV2) → 7 Phòng PNVL1" | tee -a $LOG_FILE
echo "├── 🏢 CN Bình Lư (LV2) → 3 Phòng PNVL2" | tee -a $LOG_FILE
echo "├── 🏢 CN Phong Thổ (LV2) → 3 Phòng PNVL2 + 1 PGD" | tee -a $LOG_FILE
echo "├── 🏢 CN Sìn Hồ (LV2) → 3 Phòng PNVL2" | tee -a $LOG_FILE
echo "├── 🏢 CN Bum Tở (LV2) → 3 Phòng PNVL2" | tee -a $LOG_FILE
echo "├── 🏢 CN Than Uyên (LV2) → 3 Phòng PNVL2 + 1 PGD" | tee -a $LOG_FILE
echo "├── 🏢 CN Đoàn Kết (LV2) → 3 Phòng PNVL2 + 2 PGD" | tee -a $LOG_FILE
echo "├── 🏢 CN Tân Uyên (LV2) → 3 Phòng PNVL2 + 1 PGD" | tee -a $LOG_FILE
echo "└── 🏢 CN Nậm Hàng (LV2) → 3 Phòng PNVL2" | tee -a $LOG_FILE

echo "📅 Hoàn thành: $(date)" | tee -a $LOG_FILE
echo "📝 Log chi tiết: $LOG_FILE" | tee -a $LOG_FILE
echo "🔚 KẾT THÚC" | tee -a $LOG_FILE
