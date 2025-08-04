#!/bin/bash

# Script tạo cấu trúc đơn vị Ver2 đơn giản
API_BASE="http://localhost:5055/api"

echo "🎯 TẠO CẤU TRÚC ĐƠN VỊ VER2 - 46 ĐƠN VỊ"
echo "📅 Bắt đầu: $(date)"

# Xóa tất cả dữ liệu
echo "🧹 XÓA TOÀN BỘ DỮ LIỆU CŨ..."

# Xóa employees
EMPLOYEES=$(curl -s "$API_BASE/Employees")
EMP_IDS=$(echo "$EMPLOYEES" | jq -r '.[].Id // empty' 2>/dev/null)
EMP_COUNT=0
for ID in $EMP_IDS; do
    curl -s -X DELETE "$API_BASE/Employees/$ID" > /dev/null
    EMP_COUNT=$((EMP_COUNT + 1))
done
echo "✅ Đã xóa $EMP_COUNT nhân viên"

# Xóa units
UNITS=$(curl -s "$API_BASE/Units")
UNIT_IDS=$(echo "$UNITS" | jq -r '.[].Id // empty' 2>/dev/null)
UNIT_COUNT=0
for ID in $UNIT_IDS; do
    curl -s -X DELETE "$API_BASE/Units/$ID" > /dev/null
    UNIT_COUNT=$((UNIT_COUNT + 1))
done
echo "✅ Đã xóa $UNIT_COUNT đơn vị"

# Xóa positions
POSITIONS=$(curl -s "$API_BASE/Positions")
POS_IDS=$(echo "$POSITIONS" | jq -r '.[].Id // empty' 2>/dev/null)
POS_COUNT=0
for ID in $POS_IDS; do
    curl -s -X DELETE "$API_BASE/Positions/$ID" > /dev/null
    POS_COUNT=$((POS_COUNT + 1))
done
echo "✅ Đã xóa $POS_COUNT chức vụ"

# Hàm tạo đơn vị
create_unit() {
    local name="$1"
    local code="$2"
    local type="$3"
    local parent_id="$4"

    local json="{\"Name\": \"$name\", \"Code\": \"$code\", \"Type\": \"$type\""
    if [ -n "$parent_id" ] && [ "$parent_id" != "null" ]; then
        json="$json, \"ParentUnitId\": $parent_id"
    fi
    json="$json}"

    echo "⏳ Đang tạo: $name ($type)"

    local response=$(curl -s -X POST "$API_BASE/Units" \
        -H "Content-Type: application/json" \
        -d "$json")

    local unit_id=""
    if [ -n "$response" ] && [[ "$response" == *"Id"* ]]; then
        unit_id=$(echo "$response" | jq -r '.Id // empty' 2>/dev/null)
    fi

    if [ -n "$unit_id" ] && [ "$unit_id" != "null" ]; then
        echo "✅ Thành công: $name - ID: $unit_id"
        echo "$unit_id"
    else
        echo "❌ Thất bại: $name"
        echo ""
    fi
}

echo ""
echo "🏗️ TẠO CẤU TRÚC VER2 CHÍNH THỨC"

# 1. Tạo Chi nhánh Lai Châu (LV1)
echo "1️⃣ Tạo Chi nhánh Lai Châu (LV1)..."
ROOT_RESPONSE=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d '{"Name": "Chi nhánh Lai Châu", "Code": "CNL1_LaiChau", "Type": "CNL1"}')
ROOT_ID=$(echo "$ROOT_RESPONSE" | jq -r '.Id // empty')
echo "✅ Root ID: $ROOT_ID"

if [ -z "$ROOT_ID" ] || [ "$ROOT_ID" = "null" ]; then
    echo "❌ Không thể tạo đơn vị gốc! Response: $ROOT_RESPONSE"
    exit 1
fi

# 2. Tạo 9 đơn vị CNL2 (Hội sở + 8 Chi nhánh)
echo ""
echo "2️⃣ Tạo 9 đơn vị CNL2..."

# Hội sở
HS_RESPONSE=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Name\": \"Hội sở\", \"Code\": \"CNL2_HoiSo\", \"Type\": \"CNL2\", \"ParentUnitId\": $ROOT_ID}")
HS_ID=$(echo "$HS_RESPONSE" | jq -r '.Id // empty')
echo "✅ Hội sở ID: $HS_ID"

# 8 Chi nhánh
BL_RESPONSE=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Name\": \"CN Bình Lư\", \"Code\": \"CNL2_BinhLu\", \"Type\": \"CNL2\", \"ParentUnitId\": $ROOT_ID}")
BL_ID=$(echo "$BL_RESPONSE" | jq -r '.Id // empty')
echo "✅ CN Bình Lư ID: $BL_ID"

PT_RESPONSE=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Name\": \"CN Phong Thổ\", \"Code\": \"CNL2_PhongTho\", \"Type\": \"CNL2\", \"ParentUnitId\": $ROOT_ID}")
PT_ID=$(echo "$PT_RESPONSE" | jq -r '.Id // empty')
echo "✅ CN Phong Thổ ID: $PT_ID"

SH_RESPONSE=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Name\": \"CN Sìn Hồ\", \"Code\": \"CNL2_SinHo\", \"Type\": \"CNL2\", \"ParentUnitId\": $ROOT_ID}")
SH_ID=$(echo "$SH_RESPONSE" | jq -r '.Id // empty')
echo "✅ CN Sìn Hồ ID: $SH_ID"

BT_RESPONSE=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Name\": \"CN Bum Tở\", \"Code\": \"CNL2_BumTo\", \"Type\": \"CNL2\", \"ParentUnitId\": $ROOT_ID}")
BT_ID=$(echo "$BT_RESPONSE" | jq -r '.Id // empty')
echo "✅ CN Bum Tở ID: $BT_ID"

TU_RESPONSE=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Name\": \"CN Than Uyên\", \"Code\": \"CNL2_ThanUyen\", \"Type\": \"CNL2\", \"ParentUnitId\": $ROOT_ID}")
TU_ID=$(echo "$TU_RESPONSE" | jq -r '.Id // empty')
echo "✅ CN Than Uyên ID: $TU_ID"

DK_RESPONSE=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Name\": \"CN Đoàn Kết\", \"Code\": \"CNL2_DoanKet\", \"Type\": \"CNL2\", \"ParentUnitId\": $ROOT_ID}")
DK_ID=$(echo "$DK_RESPONSE" | jq -r '.Id // empty')
echo "✅ CN Đoàn Kết ID: $DK_ID"

TAN_RESPONSE=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Name\": \"CN Tân Uyên\", \"Code\": \"CNL2_TanUyen\", \"Type\": \"CNL2\", \"ParentUnitId\": $ROOT_ID}")
TAN_ID=$(echo "$TAN_RESPONSE" | jq -r '.Id // empty')
echo "✅ CN Tân Uyên ID: $TAN_ID"

NH_RESPONSE=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Name\": \"CN Nậm Hàng\", \"Code\": \"CNL2_NamHang\", \"Type\": \"CNL2\", \"ParentUnitId\": $ROOT_ID}")
NH_ID=$(echo "$NH_RESPONSE" | jq -r '.Id // empty')
echo "✅ CN Nậm Hàng ID: $NH_ID"

# 3. Tạo 7 Phòng PNVL1 thuộc Hội sở
echo ""
echo "3️⃣ Tạo 7 Phòng PNVL1 thuộc Hội sở..."

if [ -n "$HS_ID" ]; then
    create_unit "Phòng Tín dụng" "PNVL1_TinDung" "PNVL1" "$HS_ID"
    create_unit "Phòng Huy động vốn" "PNVL1_HuyDongVon" "PNVL1" "$HS_ID"
    create_unit "Phòng Kế toán" "PNVL1_KeToan" "PNVL1" "$HS_ID"
    create_unit "Phòng Quản lý rủi ro" "PNVL1_QuanLyRuiRo" "PNVL1" "$HS_ID"
    create_unit "Phòng Nhân sự" "PNVL1_NhanSu" "PNVL1" "$HS_ID"
    create_unit "Phòng Công nghệ thông tin" "PNVL1_CNTT" "PNVL1" "$HS_ID"
    create_unit "Phòng Tuân thủ" "PNVL1_TuanThu" "PNVL1" "$HS_ID"
fi

# 4. Tạo các PNVL2 cho từng Chi nhánh
echo ""
echo "4️⃣ Tạo các Phòng PNVL2 cho 8 Chi nhánh..."

# CN Bình Lư - 3 phòng
if [ -n "$BL_ID" ]; then
    create_unit "Phòng Tín dụng" "PNVL2_BL_TinDung" "PNVL2" "$BL_ID"
    create_unit "Phòng Huy động vốn" "PNVL2_BL_HuyDongVon" "PNVL2" "$BL_ID"
    create_unit "Phòng Kế toán" "PNVL2_BL_KeToan" "PNVL2" "$BL_ID"
fi

# CN Phong Thổ - 3 phòng
if [ -n "$PT_ID" ]; then
    create_unit "Phòng Tín dụng" "PNVL2_PT_TinDung" "PNVL2" "$PT_ID"
    create_unit "Phòng Huy động vốn" "PNVL2_PT_HuyDongVon" "PNVL2" "$PT_ID"
    create_unit "Phòng Kế toán" "PNVL2_PT_KeToan" "PNVL2" "$PT_ID"
fi

# CN Sìn Hồ - 3 phòng
if [ -n "$SH_ID" ]; then
    create_unit "Phòng Tín dụng" "PNVL2_SH_TinDung" "PNVL2" "$SH_ID"
    create_unit "Phòng Huy động vốn" "PNVL2_SH_HuyDongVon" "PNVL2" "$SH_ID"
    create_unit "Phòng Kế toán" "PNVL2_SH_KeToan" "PNVL2" "$SH_ID"
fi

# CN Bum Tở - 3 phòng
if [ -n "$BT_ID" ]; then
    create_unit "Phòng Tín dụng" "PNVL2_BT_TinDung" "PNVL2" "$BT_ID"
    create_unit "Phòng Huy động vốn" "PNVL2_BT_HuyDongVon" "PNVL2" "$BT_ID"
    create_unit "Phòng Kế toán" "PNVL2_BT_KeToan" "PNVL2" "$BT_ID"
fi

# CN Than Uyên - 3 phòng
if [ -n "$TU_ID" ]; then
    create_unit "Phòng Tín dụng" "PNVL2_TU_TinDung" "PNVL2" "$TU_ID"
    create_unit "Phòng Huy động vốn" "PNVL2_TU_HuyDongVon" "PNVL2" "$TU_ID"
    create_unit "Phòng Kế toán" "PNVL2_TU_KeToan" "PNVL2" "$TU_ID"
fi

# CN Đoàn Kết - 3 phòng
if [ -n "$DK_ID" ]; then
    create_unit "Phòng Tín dụng" "PNVL2_DK_TinDung" "PNVL2" "$DK_ID"
    create_unit "Phòng Huy động vốn" "PNVL2_DK_HuyDongVon" "PNVL2" "$DK_ID"
    create_unit "Phòng Kế toán" "PNVL2_DK_KeToan" "PNVL2" "$DK_ID"
fi

# CN Tân Uyên - 3 phòng
if [ -n "$TAN_ID" ]; then
    create_unit "Phòng Tín dụng" "PNVL2_TAN_TinDung" "PNVL2" "$TAN_ID"
    create_unit "Phòng Huy động vốn" "PNVL2_TAN_HuyDongVon" "PNVL2" "$TAN_ID"
    create_unit "Phòng Kế toán" "PNVL2_TAN_KeToan" "PNVL2" "$TAN_ID"
fi

# CN Nậm Hàng - 3 phòng
if [ -n "$NH_ID" ]; then
    create_unit "Phòng Tín dụng" "PNVL2_NH_TinDung" "PNVL2" "$NH_ID"
    create_unit "Phòng Huy động vốn" "PNVL2_NH_HuyDongVon" "PNVL2" "$NH_ID"
    create_unit "Phòng Kế toán" "PNVL2_NH_KeToan" "PNVL2" "$NH_ID"
fi

# 5. Tạo 5 PGDL2 (PGD)
echo ""
echo "5️⃣ Tạo 5 Phòng giao dịch PGDL2..."

# Phong Thổ có 1 PGD
if [ -n "$PT_ID" ]; then
    create_unit "PGD Mường So" "PGDL2_PT_MuongSo" "PGDL2" "$PT_ID"
fi

# Than Uyên có 1 PGD
if [ -n "$TU_ID" ]; then
    create_unit "PGD Noong Hét" "PGDL2_TU_NoongHet" "PGDL2" "$TU_ID"
fi

# Đoàn Kết có 2 PGD
if [ -n "$DK_ID" ]; then
    create_unit "PGD Pá Tần" "PGDL2_DK_PaTan" "PGDL2" "$DK_ID"
    create_unit "PGD Ka Lăng" "PGDL2_DK_KaLang" "PGDL2" "$DK_ID"
fi

# Tân Uyên có 1 PGD
if [ -n "$TAN_ID" ]; then
    create_unit "PGD Tà Mung" "PGDL2_TAN_TaMung" "PGDL2" "$TAN_ID"
fi

# Kiểm tra kết quả
echo ""
echo "🎯 KIỂM TRA KẾT QUẢ CUỐI CÙNG"

# Đếm các loại đơn vị
FINAL_UNITS=$(curl -s "$API_BASE/Units")
CNL1_COUNT=$(echo "$FINAL_UNITS" | jq -r '.[] | select(.Type == "CNL1") | .Name' | wc -l)
CNL2_COUNT=$(echo "$FINAL_UNITS" | jq -r '.[] | select(.Type == "CNL2") | .Name' | wc -l)
PNVL1_COUNT=$(echo "$FINAL_UNITS" | jq -r '.[] | select(.Type == "PNVL1") | .Name' | wc -l)
PNVL2_COUNT=$(echo "$FINAL_UNITS" | jq -r '.[] | select(.Type == "PNVL2") | .Name' | wc -l)
PGDL2_COUNT=$(echo "$FINAL_UNITS" | jq -r '.[] | select(.Type == "PGDL2") | .Name' | wc -l)
TOTAL_COUNT=$(echo "$FINAL_UNITS" | jq length)

echo "┌─────────────────────────────────────────────────────────┐"
echo "│  LOẠI           │  MỤC TIÊU  │  THỰC TẾ  │     MÔ TẢ    │"
echo "├─────────────────────────────────────────────────────────┤"
echo "│  CNL1 (Root)     │     1      │    $CNL1_COUNT     │ Lai Châu     │"
echo "│  CNL2 (CN)       │     9      │    $CNL2_COUNT     │ HS + 8 CN    │"
echo "│  PNVL1 (P.HS)    │     7      │    $PNVL1_COUNT     │ Phòng HS     │"
echo "│  PNVL2 (P.CN)    │    24      │   $PNVL2_COUNT    │ Phòng CN     │"
echo "│  PGDL2 (PGD)     │     5      │    $PGDL2_COUNT     │ PGD          │"
echo "├─────────────────────────────────────────────────────────┤"
echo "│  TỔNG CỘNG       │    46      │   $TOTAL_COUNT    │ Ver2 Total   │"
echo "└─────────────────────────────────────────────────────────┘"

if [ "$TOTAL_COUNT" -eq 46 ]; then
    echo "🎉 THÀNH CÔNG! Đã tạo đủ 46 đơn vị theo cấu trúc Ver2"
else
    echo "⚠️ Kết quả chưa đạt cấu trúc Ver2 mong muốn"
fi

echo "📅 Hoàn thành: $(date)"
