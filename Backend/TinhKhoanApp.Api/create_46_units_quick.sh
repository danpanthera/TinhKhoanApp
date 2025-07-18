#!/bin/bash

echo "🏢 ĐANG TẠO 46 ĐƠN VỊ..."

# Function để tạo unit
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

    echo "📋 Tạo: $name ($code)"
    curl -s -X POST "http://localhost:5055/api/units" \
        -H "Content-Type: application/json" \
        -d "$data" | jq -r '.id // "ERROR"'
}

# 1. Chi nhánh Lai Châu (Root)
unit1=$(create_unit "Chi nhánh Lai Châu" "CNL1" "CNL1" "null")
echo "✅ Unit 1: $unit1"

# 2. Hội Sở
unit2=$(create_unit "Hội Sở" "CNL1" "CNL1" "$unit1")
echo "✅ Unit 2: $unit2"

# 3-9. Phòng ban Hội Sở
unit3=$(create_unit "Ban Giám đốc" "PNVL1" "PNVL1" "$unit2")
unit4=$(create_unit "Phòng Khách hàng Doanh nghiệp" "PNVL1" "PNVL1" "$unit2")
unit5=$(create_unit "Phòng Khách hàng Cá nhân" "PNVL1" "PNVL1" "$unit2")
unit6=$(create_unit "Phòng Kế toán & Ngân quỹ" "PNVL1" "PNVL1" "$unit2")
unit7=$(create_unit "Phòng Tổng hợp" "PNVL1" "PNVL1" "$unit2")
unit8=$(create_unit "Phòng Kế hoạch & Quản lý rủi ro" "PNVL1" "PNVL1" "$unit2")
unit9=$(create_unit "Phòng Kiểm tra giám sát" "PNVL1" "PNVL1" "$unit2")

# 10-17. Chi nhánh cấp 2
unit10=$(create_unit "Chi nhánh Bình Lư" "CNL2" "CNL2" "$unit1")
unit11=$(create_unit "Chi nhánh Phong Thổ" "CNL2" "CNL2" "$unit1")
unit12=$(create_unit "Chi nhánh Sìn Hồ" "CNL2" "CNL2" "$unit1")
unit13=$(create_unit "Chi nhánh Bum Tở" "CNL2" "CNL2" "$unit1")
unit14=$(create_unit "Chi nhánh Than Uyên" "CNL2" "CNL2" "$unit1")
unit15=$(create_unit "Chi nhánh Đoàn Kết" "CNL2" "CNL2" "$unit1")
unit16=$(create_unit "Chi nhánh Tân Uyên" "CNL2" "CNL2" "$unit1")
unit17=$(create_unit "Chi nhánh Nậm Hàng" "CNL2" "CNL2" "$unit1")

echo ""
echo "🎉 HOÀN THÀNH TẠO 17 UNITS CƠ BẢN!"
echo ""
echo "📊 Kiểm tra kết quả:"
curl -s http://localhost:5055/api/units | jq '. | length'
