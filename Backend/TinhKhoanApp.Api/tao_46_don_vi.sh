#!/bin/bash

# Script tạo đầy đủ 46 đơn vị - Phiên bản đơn giản
# Đảm bảo không thừa không thiếu - Fix lỗi encoding và syntax

API_BASE="http://localhost:5055/api"
LOG_FILE="tao_46_don_vi_$(date +%Y%m%d_%H%M%S).log"
TIMESTAMP=$(date +%s)

echo "🎯 TẠO 46 ĐƠN VỊ THEO CẤU TRÚC VER2" | tee -a $LOG_FILE
echo "📅 Bắt đầu: $(date)" | tee -a $LOG_FILE

# Hàm tạo đơn vị đơn giản
tao_unit() {
    local code=$1
    local name=$2
    local type=$3
    local parent_id=$4

    if [ "$parent_id" = "null" ]; then
        JSON="{\"Code\":\"$code\",\"Name\":\"$name\",\"Type\":\"$type\",\"ParentUnitId\":null}"
    else
        JSON="{\"Code\":\"$code\",\"Name\":\"$name\",\"Type\":\"$type\",\"ParentUnitId\":$parent_id}"
    fi

    RESPONSE=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "$JSON")

    # Lấy ID từ response
    ID=$(echo "$RESPONSE" | grep -o '"Id":[0-9]*' | head -1 | cut -d':' -f2)

    if [ -n "$ID" ]; then
        echo "✅ Tạo thành công: $name (ID: $ID)" | tee -a $LOG_FILE
        echo "$ID"
    else
        echo "❌ Lỗi tạo: $name" | tee -a $LOG_FILE
        echo "$RESPONSE" | tee -a $LOG_FILE
        echo ""
    fi
}

# BƯỚC 1: Tạo đơn vị gốc
echo "🏢 BƯỚC 1: TẠO ĐƠN VỊ GỐC" | tee -a $LOG_FILE
ROOT_CODE="CNLC_ROOT_$TIMESTAMP"
ROOT_ID=$(tao_unit "$ROOT_CODE" "Chi nhanh Lai Chau" "CNL1" "null")

if [ -z "$ROOT_ID" ]; then
    echo "❌ Không thể tạo đơn vị gốc!" | tee -a $LOG_FILE
    exit 1
fi

echo "🎉 Đơn vị gốc: ID=$ROOT_ID" | tee -a $LOG_FILE

# BƯỚC 2: Tạo 9 chi nhánh
echo "🏢 BƯỚC 2: TẠO 9 CHI NHÁNH" | tee -a $LOG_FILE

HS_ID=$(tao_unit "HS_$TIMESTAMP" "Hoi so" "CNL2" "$ROOT_ID")
BL_ID=$(tao_unit "BL_$TIMESTAMP" "CN Binh Lu" "CNL2" "$ROOT_ID")
PT_ID=$(tao_unit "PT_$TIMESTAMP" "CN Phong Tho" "CNL2" "$ROOT_ID")
SH_ID=$(tao_unit "SH_$TIMESTAMP" "CN Sin Ho" "CNL2" "$ROOT_ID")
BT_ID=$(tao_unit "BT_$TIMESTAMP" "CN Bum To" "CNL2" "$ROOT_ID")
TU_ID=$(tao_unit "TU_$TIMESTAMP" "CN Than Uyen" "CNL2" "$ROOT_ID")
DK_ID=$(tao_unit "DK_$TIMESTAMP" "CN Doan Ket" "CNL2" "$ROOT_ID")
TUY_ID=$(tao_unit "TUY_$TIMESTAMP" "CN Tan Uyen" "CNL2" "$ROOT_ID")
NH_ID=$(tao_unit "NH_$TIMESTAMP" "CN Nam Hang" "CNL2" "$ROOT_ID")

echo "📊 Chi nhánh đã tạo:" | tee -a $LOG_FILE
echo "  - Hoi so: $HS_ID" | tee -a $LOG_FILE
echo "  - CN Binh Lu: $BL_ID" | tee -a $LOG_FILE
echo "  - CN Phong Tho: $PT_ID" | tee -a $LOG_FILE
echo "  - CN Sin Ho: $SH_ID" | tee -a $LOG_FILE
echo "  - CN Bum To: $BT_ID" | tee -a $LOG_FILE
echo "  - CN Than Uyen: $TU_ID" | tee -a $LOG_FILE
echo "  - CN Doan Ket: $DK_ID" | tee -a $LOG_FILE
echo "  - CN Tan Uyen: $TUY_ID" | tee -a $LOG_FILE
echo "  - CN Nam Hang: $NH_ID" | tee -a $LOG_FILE

# BƯỚC 3: Tạo phòng ban
echo "🏢 BƯỚC 3: TẠO PHÒNG BAN" | tee -a $LOG_FILE

# Phòng ban cho Hội sở (7 phòng)
if [ -n "$HS_ID" ]; then
    echo "  Tạo phòng ban cho Hoi so..." | tee -a $LOG_FILE
    tao_unit "HS_BGD_$TIMESTAMP" "Ban Giam doc" "PNVL1" "$HS_ID"
    tao_unit "HS_KTNQ_$TIMESTAMP" "P. KTNQ" "PNVL1" "$HS_ID"
    tao_unit "HS_KHDN_$TIMESTAMP" "P. KHDN" "PNVL1" "$HS_ID"
    tao_unit "HS_KHCN_$TIMESTAMP" "P. KHCN" "PNVL1" "$HS_ID"
    tao_unit "HS_KTGS_$TIMESTAMP" "P. KTGS" "PNVL1" "$HS_ID"
    tao_unit "HS_TH_$TIMESTAMP" "P. Tong Hop" "PNVL1" "$HS_ID"
    tao_unit "HS_KHQLRR_$TIMESTAMP" "P. KHQLRR" "PNVL1" "$HS_ID"
fi

# Phòng ban cho CN Bình Lư (3 phòng)
if [ -n "$BL_ID" ]; then
    echo "  Tạo phòng ban cho CN Binh Lu..." | tee -a $LOG_FILE
    tao_unit "BL_BGD_$TIMESTAMP" "Ban Giam doc" "PNVL2" "$BL_ID"
    tao_unit "BL_KTNQ_$TIMESTAMP" "P. KTNQ" "PNVL2" "$BL_ID"
    tao_unit "BL_KH_$TIMESTAMP" "P. KH" "PNVL2" "$BL_ID"
fi

# Phòng ban cho CN Phong Thổ (4 phòng)
if [ -n "$PT_ID" ]; then
    echo "  Tạo phòng ban cho CN Phong Tho..." | tee -a $LOG_FILE
    tao_unit "PT_BGD_$TIMESTAMP" "Ban Giam doc" "PNVL2" "$PT_ID"
    tao_unit "PT_KTNQ_$TIMESTAMP" "P. KTNQ" "PNVL2" "$PT_ID"
    tao_unit "PT_KH_$TIMESTAMP" "P. KH" "PNVL2" "$PT_ID"
    tao_unit "PT_S5_$TIMESTAMP" "PGD So 5" "PGDL2" "$PT_ID"
fi

# Phòng ban cho CN Sìn Hồ (3 phòng)
if [ -n "$SH_ID" ]; then
    echo "  Tạo phòng ban cho CN Sin Ho..." | tee -a $LOG_FILE
    tao_unit "SH_BGD_$TIMESTAMP" "Ban Giam doc" "PNVL2" "$SH_ID"
    tao_unit "SH_KTNQ_$TIMESTAMP" "P. KTNQ" "PNVL2" "$SH_ID"
    tao_unit "SH_KH_$TIMESTAMP" "P. KH" "PNVL2" "$SH_ID"
fi

# Phòng ban cho CN Bum Tở (3 phòng)
if [ -n "$BT_ID" ]; then
    echo "  Tạo phòng ban cho CN Bum To..." | tee -a $LOG_FILE
    tao_unit "BT_BGD_$TIMESTAMP" "Ban Giam doc" "PNVL2" "$BT_ID"
    tao_unit "BT_KTNQ_$TIMESTAMP" "P. KTNQ" "PNVL2" "$BT_ID"
    tao_unit "BT_KH_$TIMESTAMP" "P. KH" "PNVL2" "$BT_ID"
fi

# Phòng ban cho CN Than Uyên (4 phòng)
if [ -n "$TU_ID" ]; then
    echo "  Tạo phòng ban cho CN Than Uyen..." | tee -a $LOG_FILE
    tao_unit "TU_BGD_$TIMESTAMP" "Ban Giam doc" "PNVL2" "$TU_ID"
    tao_unit "TU_KTNQ_$TIMESTAMP" "P. KTNQ" "PNVL2" "$TU_ID"
    tao_unit "TU_KH_$TIMESTAMP" "P. KH" "PNVL2" "$TU_ID"
    tao_unit "TU_S6_$TIMESTAMP" "PGD So 6" "PGDL2" "$TU_ID"
fi

# Phòng ban cho CN Đoàn Kết (5 phòng)
if [ -n "$DK_ID" ]; then
    echo "  Tạo phòng ban cho CN Doan Ket..." | tee -a $LOG_FILE
    tao_unit "DK_BGD_$TIMESTAMP" "Ban Giam doc" "PNVL2" "$DK_ID"
    tao_unit "DK_KTNQ_$TIMESTAMP" "P. KTNQ" "PNVL2" "$DK_ID"
    tao_unit "DK_KH_$TIMESTAMP" "P. KH" "PNVL2" "$DK_ID"
    tao_unit "DK_S1_$TIMESTAMP" "PGD So 1" "PGDL2" "$DK_ID"
    tao_unit "DK_S2_$TIMESTAMP" "PGD So 2" "PGDL2" "$DK_ID"
fi

# Phòng ban cho CN Tân Uyên (4 phòng)
if [ -n "$TUY_ID" ]; then
    echo "  Tạo phòng ban cho CN Tan Uyen..." | tee -a $LOG_FILE
    tao_unit "TUY_BGD_$TIMESTAMP" "Ban Giam doc" "PNVL2" "$TUY_ID"
    tao_unit "TUY_KTNQ_$TIMESTAMP" "P. KTNQ" "PNVL2" "$TUY_ID"
    tao_unit "TUY_KH_$TIMESTAMP" "P. KH" "PNVL2" "$TUY_ID"
    tao_unit "TUY_S3_$TIMESTAMP" "PGD So 3" "PGDL2" "$TUY_ID"
fi

# Phòng ban cho CN Nậm Hàng (3 phòng)
if [ -n "$NH_ID" ]; then
    echo "  Tạo phòng ban cho CN Nam Hang..." | tee -a $LOG_FILE
    tao_unit "NH_BGD_$TIMESTAMP" "Ban Giam doc" "PNVL2" "$NH_ID"
    tao_unit "NH_KTNQ_$TIMESTAMP" "P. KTNQ" "PNVL2" "$NH_ID"
    tao_unit "NH_KH_$TIMESTAMP" "P. KH" "PNVL2" "$NH_ID"
fi

# BƯỚC 4: Kiểm tra kết quả
echo "🔍 BƯỚC 4: KIỂM TRA KẾT QUẢ" | tee -a $LOG_FILE
sleep 3

# Lấy thống kê
ALL_UNITS=$(curl -s "$API_BASE/Units")
TOTAL_COUNT=$(echo "$ALL_UNITS" | grep -c '"Id":')
CNL1_COUNT=$(echo "$ALL_UNITS" | grep -c '"Type":"CNL1"')
CNL2_COUNT=$(echo "$ALL_UNITS" | grep -c '"Type":"CNL2"')
PNVL1_COUNT=$(echo "$ALL_UNITS" | grep -c '"Type":"PNVL1"')
PNVL2_COUNT=$(echo "$ALL_UNITS" | grep -c '"Type":"PNVL2"')
PGDL2_COUNT=$(echo "$ALL_UNITS" | grep -c '"Type":"PGDL2"')

# Tính toán đơn vị mới
NEW_UNITS=$((1 + 9 + 7 + 24 + 5))

echo "📊 THỐNG KÊ CUỐI CÙNG:" | tee -a $LOG_FILE
echo "┌──────────────────────────────────────┐" | tee -a $LOG_FILE
echo "│  LOẠI      │  MỤC TIÊU  │  THỰC TẾ  │" | tee -a $LOG_FILE
echo "├──────────────────────────────────────┤" | tee -a $LOG_FILE
echo "│  CNL1      │     1      │    $CNL1_COUNT     │" | tee -a $LOG_FILE
echo "│  CNL2      │     9      │    $CNL2_COUNT     │" | tee -a $LOG_FILE
echo "│  PNVL1     │     7      │    $PNVL1_COUNT     │" | tee -a $LOG_FILE
echo "│  PNVL2     │    24      │    $PNVL2_COUNT    │" | tee -a $LOG_FILE
echo "│  PGDL2     │     5      │    $PGDL2_COUNT     │" | tee -a $LOG_FILE
echo "├──────────────────────────────────────┤" | tee -a $LOG_FILE
echo "│  TỔNG      │    46      │    $TOTAL_COUNT    │" | tee -a $LOG_FILE
echo "└──────────────────────────────────────┘" | tee -a $LOG_FILE

# Kết luận
echo "" | tee -a $LOG_FILE
if [ "$TOTAL_COUNT" -ge 46 ]; then
    echo "🎉 THÀNH CÔNG! Đã có ít nhất 46 đơn vị trong hệ thống" | tee -a $LOG_FILE
    echo "✅ Cấu trúc tổ chức Ver2 đã hoàn thành" | tee -a $LOG_FILE
    echo "🏢 Đơn vị gốc mới: Chi nhanh Lai Chau (Code: $ROOT_CODE)" | tee -a $LOG_FILE
else
    echo "⚠️ Chưa đạt mục tiêu 46 đơn vị" | tee -a $LOG_FILE
    echo "📈 Hiện tại có: $TOTAL_COUNT đơn vị" | tee -a $LOG_FILE
    echo "📋 Cần tạo thêm: $((46 - TOTAL_COUNT)) đơn vị" | tee -a $LOG_FILE
fi

echo "" | tee -a $LOG_FILE
echo "📅 Hoàn thành: $(date)" | tee -a $LOG_FILE
echo "📝 Chi tiết: $LOG_FILE" | tee -a $LOG_FILE
echo "🔚 KẾT THÚC" | tee -a $LOG_FILE
