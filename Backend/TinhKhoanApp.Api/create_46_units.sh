#!/bin/bash

# Script tạo 46 đơn vị theo cấu trúc hierarchical
# Sử dụng API Units thông thường

echo "🚀 Bắt đầu tạo cấu trúc 46 đơn vị..."
BASE_URL="http://localhost:5055/api/units"

# 1. Chi nhánh Lai Châu (root)
echo "1. Tạo Chi nhánh Lai Châu..."
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{
  "name": "Chi nhánh Lai Châu",
  "code": "CnLaiChau",
  "type": "CNL1",
  "parentUnitId": null
}' > /dev/null

# 2. Hội Sở
echo "2. Tạo Hội Sở..."
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{
  "name": "Hội Sở",
  "code": "HoiSo",
  "type": "CNL1",
  "parentUnitId": 1
}' > /dev/null

# 3-9. Các phòng ban Hội Sở
echo "3-9. Tạo các phòng ban Hội Sở..."
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Ban Giám đốc", "code": "HoiSoBgd", "type": "PNVL1", "parentUnitId": 2}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Khách hàng Doanh nghiệp", "code": "HoiSoKhdn", "type": "PNVL1", "parentUnitId": 2}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Khách hàng Cá nhân", "code": "HoiSoKhcn", "type": "PNVL1", "parentUnitId": 2}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Kế toán & Ngân quỹ", "code": "HoiSoKtnq", "type": "PNVL1", "parentUnitId": 2}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Tổng hợp", "code": "HoiSoTonghop", "type": "PNVL1", "parentUnitId": 2}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Kế hoạch & Quản lý rủi ro", "code": "HoiSoKhqlrr", "type": "PNVL1", "parentUnitId": 2}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Kiểm tra giám sát", "code": "HoiSoKtgs", "type": "PNVL1", "parentUnitId": 2}' > /dev/null

# 10-18. Các chi nhánh cấp 2
echo "10-18. Tạo các chi nhánh cấp 2..."
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Chi nhánh Bình Lư", "code": "CnBinhLu", "type": "CNL2", "parentUnitId": 1}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Chi nhánh Phong Thổ", "code": "CnPhongTho", "type": "CNL2", "parentUnitId": 1}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Chi nhánh Sìn Hồ", "code": "CnSinHo", "type": "CNL2", "parentUnitId": 1}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Chi nhánh Bum Tở", "code": "CnBumTo", "type": "CNL2", "parentUnitId": 1}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Chi nhánh Than Uyên", "code": "CnThanUyen", "type": "CNL2", "parentUnitId": 1}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Chi nhánh Đoàn Kết", "code": "CnDoanKet", "type": "CNL2", "parentUnitId": 1}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Chi nhánh Tân Uyên", "code": "CnTanUyen", "type": "CNL2", "parentUnitId": 1}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Chi nhánh Nậm Hàng", "code": "CnNamHang", "type": "CNL2", "parentUnitId": 1}' > /dev/null

# Nghỉ 1 giây để đảm bảo data đã được tạo
sleep 1

# Tạo phòng ban cho từng chi nhánh
echo "19+. Tạo phòng ban cho các chi nhánh..."

# Chi nhánh Bình Lư (ID=10)
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Ban Giám đốc", "code": "CnBinhLuBgd", "type": "PNVL2", "parentUnitId": 10}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Kế toán & Ngân quỹ", "code": "CnBinhLuKtnq", "type": "PNVL2", "parentUnitId": 10}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Khách hàng", "code": "CnBinhLuKh", "type": "PNVL2", "parentUnitId": 10}' > /dev/null

# Chi nhánh Phong Thổ (ID=11)
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Ban Giám đốc", "code": "CnPhongThoBgd", "type": "PNVL2", "parentUnitId": 11}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Kế toán & Ngân quỹ", "code": "CnPhongThoKtnq", "type": "PNVL2", "parentUnitId": 11}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Khách hàng", "code": "CnPhongThoKh", "type": "PNVL2", "parentUnitId": 11}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng giao dịch Số 5", "code": "CnPhongThoPgdSo5", "type": "PGDL2", "parentUnitId": 11}' > /dev/null

# Chi nhánh Sìn Hồ (ID=12)
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Ban Giám đốc", "code": "CnSinHoBgd", "type": "PNVL2", "parentUnitId": 12}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Kế toán & Ngân quỹ", "code": "CnSinHoKtnq", "type": "PNVL2", "parentUnitId": 12}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Khách hàng", "code": "CnSinHoKh", "type": "PNVL2", "parentUnitId": 12}' > /dev/null

# Chi nhánh Bum Tở (ID=13)
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Ban Giám đốc", "code": "CnBumToBgd", "type": "PNVL2", "parentUnitId": 13}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Kế toán & Ngân quỹ", "code": "CnBumToKtnq", "type": "PNVL2", "parentUnitId": 13}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Khách hàng", "code": "CnBumToKh", "type": "PNVL2", "parentUnitId": 13}' > /dev/null

# Chi nhánh Than Uyên (ID=14)
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Ban Giám đốc", "code": "CnThanUyenBgd", "type": "PNVL2", "parentUnitId": 14}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Kế toán & Ngân quỹ", "code": "CnThanUyenKtnq", "type": "PNVL2", "parentUnitId": 14}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Khách hàng", "code": "CnThanUyenKh", "type": "PNVL2", "parentUnitId": 14}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng giao dịch số 6", "code": "CnThanUyenPgdSo6", "type": "PGDL2", "parentUnitId": 14}' > /dev/null

# Chi nhánh Đoàn Kết (ID=15)
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Ban Giám đốc", "code": "CnDoanKetBgd", "type": "PNVL2", "parentUnitId": 15}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Kế toán & Ngân quỹ", "code": "CnDoanKetKtnq", "type": "PNVL2", "parentUnitId": 15}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Khách hàng", "code": "CnDoanKetKh", "type": "PNVL2", "parentUnitId": 15}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng giao dịch số 1", "code": "CnDoanKetPgdso1", "type": "PGDL2", "parentUnitId": 15}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng giao dịch số 2", "code": "CnDoanKetPgdso2", "type": "PGDL2", "parentUnitId": 15}' > /dev/null

# Chi nhánh Tân Uyên (ID=16)
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Ban Giám đốc", "code": "CnTanUyenBgd", "type": "PNVL2", "parentUnitId": 16}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Kế toán & Ngân quỹ", "code": "CnTanUyenKtnq", "type": "PNVL2", "parentUnitId": 16}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Khách hàng", "code": "CnTanUyenKh", "type": "PNVL2", "parentUnitId": 16}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng giao dịch số 3", "code": "CnTanUyenPgdso3", "type": "PGDL2", "parentUnitId": 16}' > /dev/null

# Chi nhánh Nậm Hàng (ID=17)
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Ban Giám đốc", "code": "CnNamHangBgd", "type": "PNVL2", "parentUnitId": 17}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Kế toán & Ngân quỹ", "code": "CnNamHangKtnq", "type": "PNVL2", "parentUnitId": 17}' > /dev/null
curl -s -X POST "$BASE_URL" -H "Content-Type: application/json" -d '{"name": "Phòng Khách hàng", "code": "CnNamHangKh", "type": "PNVL2", "parentUnitId": 17}' > /dev/null

# Kiểm tra kết quả
echo "✅ Hoàn thành! Kiểm tra kết quả..."
TOTAL=$(curl -s "$BASE_URL" | jq 'length')
echo "📊 Tổng số đơn vị đã tạo: $TOTAL"

echo "🎯 Cấu trúc 46 đơn vị đã được tạo thành công!"
