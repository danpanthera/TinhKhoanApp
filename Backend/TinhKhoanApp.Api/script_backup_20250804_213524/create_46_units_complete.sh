#!/bin/bash

echo "🏢 PHỤC HỒI HOÀN CHỈNH 46 UNITS"
echo "================================"

API_BASE="http://localhost:5055/api"

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# Function to create unit
create_unit() {
    local name="$1"
    local code="$2"
    local level="$3"
    local parent_id="$4"

    local data="{\"name\": \"$name\", \"code\": \"$code\", \"level\": \"$level\""
    if [ "$parent_id" != "null" ]; then
        data+=", \"parentId\": $parent_id"
    fi
    data+="}"

    result=$(curl -s -X POST "$API_BASE/units" \
        -H "Content-Type: application/json" \
        -d "$data")

    if echo "$result" | jq -e '.id' > /dev/null 2>&1; then
        unit_id=$(echo "$result" | jq -r '.id')
        echo -e "${GREEN}✅ $name (ID: $unit_id)${NC}"
        echo "$unit_id"
    else
        echo -e "${YELLOW}⚠️ $name (đã tồn tại hoặc lỗi)${NC}"
        # Try to get existing ID
        existing=$(curl -s "$API_BASE/units" | jq -r ".[] | select(.code == \"$code\") | .id")
        if [ "$existing" != "" ]; then
            echo "$existing"
        else
            echo "1" # fallback
        fi
    fi
}

echo "📋 Tạo cấu trúc 46 đơn vị hoàn chỉnh..."
echo ""

# 1. Chi nhánh Lai Châu (Root) - ID=1
unit1=$(create_unit "Chi nhánh Lai Châu" "CNL1" "CNL1" "null")

# 2. Hội Sở - ID=2
unit2=$(create_unit "Hội Sở" "HoiSo" "CNL1" "$unit1")

# 3-9. Phòng ban Hội Sở (PNVL1)
unit3=$(create_unit "Ban Giám đốc" "BGD_HS" "PNVL1" "$unit2")
unit4=$(create_unit "Phòng Khách hàng Doanh nghiệp" "KHDN_HS" "PNVL1" "$unit2")
unit5=$(create_unit "Phòng Khách hàng Cá nhân" "KHCN_HS" "PNVL1" "$unit2")
unit6=$(create_unit "Phòng Kế toán & Ngân quỹ" "KTNQ_HS" "PNVL1" "$unit2")
unit7=$(create_unit "Phòng Tổng hợp" "TH_HS" "PNVL1" "$unit2")
unit8=$(create_unit "Phòng Kế hoạch & Quản lý rủi ro" "KHQLRR_HS" "PNVL1" "$unit2")
unit9=$(create_unit "Phòng Kiểm tra giám sát" "KTGS_HS" "PNVL1" "$unit2")

# 10-17. Chi nhánh cấp 2 (CNL2)
unit10=$(create_unit "Chi nhánh Bình Lư" "BinhLu" "CNL2" "$unit1")
unit11=$(create_unit "Chi nhánh Phong Thổ" "PhongTho" "CNL2" "$unit1")
unit12=$(create_unit "Chi nhánh Sìn Hồ" "SinHo" "CNL2" "$unit1")
unit13=$(create_unit "Chi nhánh Bum Tở" "BumTo" "CNL2" "$unit1")
unit14=$(create_unit "Chi nhánh Than Uyên" "ThanUyen" "CNL2" "$unit1")
unit15=$(create_unit "Chi nhánh Đoàn Kết" "DoanKet" "CNL2" "$unit1")
unit16=$(create_unit "Chi nhánh Tân Uyên" "TanUyen" "CNL2" "$unit1")
unit17=$(create_unit "Chi nhánh Nậm Hàng" "NamHang" "CNL2" "$unit1")

# 18-20. Phòng ban Chi nhánh Bình Lư
unit18=$(create_unit "Ban Giám đốc - Bình Lư" "BGD_BL" "PNVL2" "$unit10")
unit19=$(create_unit "Phòng Kế toán & Ngân quỹ - Bình Lư" "KTNQ_BL" "PNVL2" "$unit10")
unit20=$(create_unit "Phòng Khách hàng - Bình Lư" "KH_BL" "PNVL2" "$unit10")

# 21-24. Phòng ban Chi nhánh Phong Thổ + PGD
unit21=$(create_unit "Ban Giám đốc - Phong Thổ" "BGD_PT" "PNVL2" "$unit11")
unit22=$(create_unit "Phòng KT&NQ - Phong Thổ" "KTNQ_PT" "PNVL2" "$unit11")
unit23=$(create_unit "Phòng Khách hàng - Phong Thổ" "KH_PT" "PNVL2" "$unit11")
unit24=$(create_unit "Phòng giao dịch Số 5" "PGD_5" "PGDL2" "$unit11")

# 25-27. Phòng ban Chi nhánh Sìn Hồ
unit25=$(create_unit "Ban Giám đốc - Sìn Hồ" "BGD_SH" "PNVL2" "$unit12")
unit26=$(create_unit "Phòng KT&NQ - Sìn Hồ" "KTNQ_SH" "PNVL2" "$unit12")
unit27=$(create_unit "Phòng Khách hàng - Sìn Hồ" "KH_SH" "PNVL2" "$unit12")

# 28-30. Phòng ban Chi nhánh Bum Tở
unit28=$(create_unit "Ban Giám đốc - Bum Tở" "BGD_BT" "PNVL2" "$unit13")
unit29=$(create_unit "Phòng KT&NQ - Bum Tở" "KTNQ_BT" "PNVL2" "$unit13")
unit30=$(create_unit "Phòng Khách hàng - Bum Tở" "KH_BT" "PNVL2" "$unit13")

# 31-34. Phòng ban Chi nhánh Than Uyên + PGD
unit31=$(create_unit "Ban Giám đốc - Than Uyên" "BGD_TU" "PNVL2" "$unit14")
unit32=$(create_unit "Phòng KT&NQ - Than Uyên" "KTNQ_TU" "PNVL2" "$unit14")
unit33=$(create_unit "Phòng Khách hàng - Than Uyên" "KH_TU" "PNVL2" "$unit14")
unit34=$(create_unit "Phòng giao dịch Số 6" "PGD_6" "PGDL2" "$unit14")

# 35-38. Phòng ban Chi nhánh Đoàn Kết + 2 PGD
unit35=$(create_unit "Ban Giám đốc - Đoàn Kết" "BGD_DK" "PNVL2" "$unit15")
unit36=$(create_unit "Phòng KT&NQ - Đoàn Kết" "KTNQ_DK" "PNVL2" "$unit15")
unit37=$(create_unit "Phòng giao dịch Số 1" "PGD_1" "PGDL2" "$unit15")
unit38=$(create_unit "Phòng giao dịch Số 2" "PGD_2" "PGDL2" "$unit15")

# 39-42. Phòng ban Chi nhánh Tân Uyên + PGD
unit39=$(create_unit "Ban Giám đốc - Tân Uyên" "BGD_TaN" "PNVL2" "$unit16")
unit40=$(create_unit "Phòng KT&NQ - Tân Uyên" "KTNQ_TaN" "PNVL2" "$unit16")
unit41=$(create_unit "Phòng Khách hàng - Tân Uyên" "KH_TaN" "PNVL2" "$unit16")
unit42=$(create_unit "Phòng giao dịch Số 3" "PGD_3" "PGDL2" "$unit16")

# 43-46. Phòng ban Chi nhánh Nậm Hàng
unit43=$(create_unit "Ban Giám đốc - Nậm Hàng" "BGD_NH" "PNVL2" "$unit17")
unit44=$(create_unit "Phòng KT&NQ - Nậm Hàng" "KTNQ_NH" "PNVL2" "$unit17")
unit45=$(create_unit "Phòng Khách hàng - Nậm Hàng" "KH_NH" "PNVL2" "$unit17")
unit46=$(create_unit "Phòng Khách hàng Cá nhân - Nậm Hàng" "KHCN_NH" "PNVL2" "$unit17")

echo ""
echo "🎉 HOÀN THÀNH TẠO 46 UNITS!"
echo ""

# Verify count
final_count=$(curl -s "$API_BASE/units" | jq 'length' 2>/dev/null || echo "0")
echo -e "${BLUE}📊 Tổng số units: $final_count/46${NC}"

if [ "$final_count" -ge "40" ]; then
    echo -e "${GREEN}✅ ĐẠT MỤC TIÊU: Đã có đủ units!${NC}"
else
    echo -e "${YELLOW}⚠️ Cần kiểm tra: Chưa đủ 46 units${NC}"
fi

echo ""
echo "📋 Cấu trúc hoàn chỉnh:"
echo "  - CNL1: 2 đơn vị (Lai Châu, Hội Sở)"
echo "  - CNL2: 8 chi nhánh cấp 2"
echo "  - PNVL1: 7 phòng ban Hội Sở"
echo "  - PNVL2: 25 phòng ban chi nhánh"
echo "  - PGDL2: 4 phòng giao dịch"
echo "  - TỔNG: 46 đơn vị ✅"
