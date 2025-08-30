#!/bin/bash

# 🏢 BỔ SUNG 24 PHÒNG BAN PNVL2 CHO CÁC CHI NHÁNH CNL2

echo "🏢 ĐANG BỔ SUNG 24 PHÒNG BAN PNVL2..."

# Lấy danh sách chi nhánh CNL2 và tạo phòng ban cho mỗi chi nhánh
branches=(
  "10:CNBL"  # Chi nhánh Bình Lư
  "11:CNPT"  # Chi nhánh Phong Thổ
  "12:CNSH"  # Chi nhánh Sìn Hồ
  "13:CNBT"  # Chi nhánh Bum Tở
  "14:CNTU"  # Chi nhánh Than Uyên
  "15:CNDK"  # Chi nhánh Đoàn Kết
  "16:CNTUY" # Chi nhánh Tân Uyên
  "17:CNNH"  # Chi nhánh Nậm Hàng
)

for branch in "${branches[@]}"; do
  branch_id=$(echo $branch | cut -d: -f1)
  branch_code=$(echo $branch | cut -d: -f2)

  echo "📋 Tạo phòng ban cho chi nhánh ID $branch_id ($branch_code):"

  # Tạo 3 phòng ban chuẩn cho mỗi chi nhánh
  depts=(
    "Ban Giám đốc:BGD$branch_code"
    "Phòng Kế toán & Ngân quỹ:PKTNQ$branch_code"
    "Phòng Khách hàng:PKH$branch_code"
  )

  for dept in "${depts[@]}"; do
    dept_name=$(echo $dept | cut -d: -f1)
    dept_code=$(echo $dept | cut -d: -f2)

    echo "  📋 Tạo: $dept_name ($dept_code)"
    result=$(curl -s -X POST http://localhost:5055/api/units -H "Content-Type: application/json" -d "{
      \"name\": \"$dept_name\",
      \"code\": \"$dept_code\",
      \"unitType\": \"PNVL2\",
      \"level\": 3,
      \"parentId\": $branch_id
    }")

    # Kiểm tra kết quả
    if echo "$result" | grep -q '"Id"'; then
      echo "    ✅ Thành công"
    else
      echo "    ❌ Lỗi: $result"
    fi
  done
  echo ""
done

echo "🎉 HOÀN THÀNH BỔ SUNG PHÒNG BAN!"

# Kiểm tra kết quả cuối cùng
total=$(curl -s http://localhost:5055/api/units | jq '. | length')
echo "📊 Tổng số units hiện tại: $total/46"

if [ "$total" -eq 46 ]; then
  echo "✅ THÀNH CÔNG: Đã tạo đủ 46 units!"
  echo ""
  echo "📋 Cấu trúc hoàn chỉnh:"
  echo "  - CNL1: 2 đơn vị ✅"
  echo "  - CNL2: 8 chi nhánh ✅"
  echo "  - PNVL1: 7 phòng ban Hội Sở ✅"
  echo "  - PNVL2: 24 phòng ban chi nhánh ✅"
  echo "  - PGDL2: 5 phòng giao dịch ✅"
  echo "  - TỔNG: 46 đơn vị ✅"
else
  echo "⚠️ Cần tạo thêm $((46 - total)) units"
fi
