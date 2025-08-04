#!/bin/bash

# 🏢 TẠO ĐẦY ĐỦ 46 UNITS THEO CẤU TRÚC README_DAT

echo "🏢 ĐANG TẠO ĐẦY ĐỦ 46 UNITS..."

# Xóa tất cả units hiện tại
curl -X DELETE http://localhost:5055/api/units/clear-all 2>/dev/null

# Tạo từng unit theo đúng cấu trúc
echo "📋 Tạo ROOT: Chi nhánh Lai Châu (CNL1)"
curl -X POST http://localhost:5055/api/units -H "Content-Type: application/json" -d '{
  "name": "Chi nhánh Lai Châu",
  "code": "CNL1",
  "unitType": "CNL1",
  "level": 1,
  "parentId": null
}' 2>/dev/null

echo "📋 Tạo Hội Sở (CNL1)"
curl -X POST http://localhost:5055/api/units -H "Content-Type: application/json" -d '{
  "name": "Hội Sở",
  "code": "HS",
  "unitType": "CNL1",
  "level": 2,
  "parentId": 1
}' 2>/dev/null

# Tạo 7 phòng ban Hội Sở (PNVL1)
units=(
  "Ban Giám đốc:BGD"
  "Phòng Khách hàng Doanh nghiệp:PKHDN"
  "Phòng Khách hàng Cá nhân:PKHCN"
  "Phòng Kế toán & Ngân quỹ:PKTNQ"
  "Phòng Tổng hợp:PTH"
  "Phòng Kế hoạch & Quản lý rủi ro:PKHQLRR"
  "Phòng Kiểm tra giám sát:PKTGS"
)

parent_id=2
for unit in "${units[@]}"; do
  name=$(echo $unit | cut -d: -f1)
  code=$(echo $unit | cut -d: -f2)
  echo "📋 Tạo: $name ($code)"
  curl -X POST http://localhost:5055/api/units -H "Content-Type: application/json" -d "{
    \"name\": \"$name\",
    \"code\": \"$code\",
    \"unitType\": \"PNVL1\",
    \"level\": 3,
    \"parentId\": $parent_id
  }" 2>/dev/null
done

# Tạo 8 chi nhánh CNL2
branches=(
  "Chi nhánh Bình Lư:CNBL"
  "Chi nhánh Phong Thổ:CNPT"
  "Chi nhánh Sìn Hồ:CNSH"
  "Chi nhánh Bum Tở:CNBT"
  "Chi nhánh Than Uyên:CNTU"
  "Chi nhánh Đoàn Kết:CNDK"
  "Chi nhánh Tân Uyên:CNTUY"
  "Chi nhánh Nậm Hàng:CNNH"
)

for branch in "${branches[@]}"; do
  name=$(echo $branch | cut -d: -f1)
  code=$(echo $branch | cut -d: -f2)
  echo "📋 Tạo: $name ($code)"
  result=$(curl -X POST http://localhost:5055/api/units -H "Content-Type: application/json" -d "{
    \"name\": \"$name\",
    \"code\": \"$code\",
    \"unitType\": \"CNL2\",
    \"level\": 2,
    \"parentId\": 1
  }" 2>/dev/null)

  # Lấy ID của chi nhánh vừa tạo
  branch_id=$(echo $result | jq -r '.id // empty')
  if [ -n "$branch_id" ] && [ "$branch_id" != "null" ]; then
    # Tạo 3 phòng ban cho mỗi chi nhánh
    depts=(
      "Ban Giám đốc:BGD$code"
      "Phòng Kế toán & Ngân quỹ:PKTNQ$code"
      "Phòng Khách hàng:PKH$code"
    )

    for dept in "${depts[@]}"; do
      dept_name=$(echo $dept | cut -d: -f1)
      dept_code=$(echo $dept | cut -d: -f2)
      echo "  📋 Tạo: $dept_name ($dept_code)"
      curl -X POST http://localhost:5055/api/units -H "Content-Type: application/json" -d "{
        \"name\": \"$dept_name\",
        \"code\": \"$dept_code\",
        \"unitType\": \"PNVL2\",
        \"level\": 3,
        \"parentId\": $branch_id
      }" 2>/dev/null
    done
  fi
done

# Tạo các phòng giao dịch đặc biệt
echo "📋 Tạo: Phòng giao dịch Số 5 (PGD5)"
curl -X POST http://localhost:5055/api/units -H "Content-Type: application/json" -d '{
  "name": "Phòng giao dịch Số 5",
  "code": "PGD5",
  "unitType": "PGDL2",
  "level": 4,
  "parentId": 11
}' 2>/dev/null

echo "📋 Tạo: Phòng giao dịch số 6 (PGD6)"
curl -X POST http://localhost:5055/api/units -H "Content-Type: application/json" -d '{
  "name": "Phòng giao dịch số 6",
  "code": "PGD6",
  "unitType": "PGDL2",
  "level": 4,
  "parentId": 14
}' 2>/dev/null

echo "📋 Tạo: Phòng giao dịch số 1 (PGD1)"
curl -X POST http://localhost:5055/api/units -H "Content-Type: application/json" -d '{
  "name": "Phòng giao dịch số 1",
  "code": "PGD1",
  "unitType": "PGDL2",
  "level": 4,
  "parentId": 15
}' 2>/dev/null

echo "📋 Tạo: Phòng giao dịch số 2 (PGD2)"
curl -X POST http://localhost:5055/api/units -H "Content-Type: application/json" -d '{
  "name": "Phòng giao dịch số 2",
  "code": "PGD2",
  "unitType": "PGDL2",
  "level": 4,
  "parentId": 15
}' 2>/dev/null

echo "📋 Tạo: Phòng giao dịch số 3 (PGD3)"
curl -X POST http://localhost:5055/api/units -H "Content-Type: application/json" -d '{
  "name": "Phòng giao dịch số 3",
  "code": "PGD3",
  "unitType": "PGDL2",
  "level": 4,
  "parentId": 16
}' 2>/dev/null

echo ""
echo "🎉 HOÀN THÀNH TẠO UNITS!"

# Kiểm tra kết quả
total=$(curl -s http://localhost:5055/api/units | jq '. | length')
echo "📊 Tổng số units: $total/46"

if [ "$total" -eq 46 ]; then
  echo "✅ THÀNH CÔNG: Đã tạo đủ 46 units!"
else
  echo "⚠️ Cảnh báo: Chỉ tạo được $total/46 units"
fi

echo ""
echo "📋 Cấu trúc hoàn chỉnh:"
echo "  - CNL1: 2 đơn vị (Lai Châu, Hội Sở)"
echo "  - CNL2: 8 chi nhánh cấp 2"
echo "  - PNVL1: 7 phòng ban Hội Sở"
echo "  - PNVL2: 24 phòng ban chi nhánh"
echo "  - PGDL2: 5 phòng giao dịch"
echo "  - TỔNG: 46 đơn vị ✅"
