#!/bin/bash

# Script tạo 46 đơn vị theo cấu trúc 3 cấp mới
API_BASE="http://localhost:5055/api"

echo "🎯 TẠO 46 ĐƠN VỊ THEO CẤU TRÚC 3 CẤP MỚI"
echo "📅 Bắt đầu: $(date)"

# Xóa tất cả dữ liệu cũ
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

echo ""
echo "🏗️ TẠO CẤU TRÚC 46 ĐƠN VỊ THEO SƠ ĐỒ 3 CẤP"

# Cấp 1: Chi nhánh Lai Châu (Root)
echo "1️⃣ Tạo Chi nhánh Lai Châu (Cấp 1)..."
curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 1, "Name": "Chi nhánh Lai Châu", "Code": "CnLaiChau", "Type": "CNL1", "ParentUnitId": null}'
echo "✅ ID: 1 - Chi nhánh Lai Châu"

echo ""
echo "2️⃣ Tạo 9 đơn vị Cấp 2 (Hội sở + 8 Chi nhánh)..."

# Cấp 2: Hội sở
curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 2, "Name": "Hội Sở", "Code": "HoiSo", "Type": "CNL2", "ParentUnitId": 1}'
echo "✅ ID: 2 - Hội Sở"

# Cấp 2: 8 Chi nhánh
curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 10, "Name": "Chi nhánh Bình Lư", "Code": "CnBinhLu", "Type": "CNL2", "ParentUnitId": 1}'
echo "✅ ID: 10 - Chi nhánh Bình Lư"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 14, "Name": "Chi nhánh Phong Thổ", "Code": "CnPhongTho", "Type": "CNL2", "ParentUnitId": 1}'
echo "✅ ID: 14 - Chi nhánh Phong Thổ"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 19, "Name": "Chi nhánh Sìn Hồ", "Code": "CnSinHo", "Type": "CNL2", "ParentUnitId": 1}'
echo "✅ ID: 19 - Chi nhánh Sìn Hồ"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 23, "Name": "Chi nhánh Bum Tở", "Code": "CnBumTo", "Type": "CNL2", "ParentUnitId": 1}'
echo "✅ ID: 23 - Chi nhánh Bum Tở"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 27, "Name": "Chi nhánh Than Uyên", "Code": "CnThanUyen", "Type": "CNL2", "ParentUnitId": 1}'
echo "✅ ID: 27 - Chi nhánh Than Uyên"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 32, "Name": "Chi nhánh Đoàn Kết", "Code": "CnDoanKet", "Type": "CNL2", "ParentUnitId": 1}'
echo "✅ ID: 32 - Chi nhánh Đoàn Kết"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 38, "Name": "Chi nhánh Tân Uyên", "Code": "CnTanUyen", "Type": "CNL2", "ParentUnitId": 1}'
echo "✅ ID: 38 - Chi nhánh Tân Uyên"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 43, "Name": "Chi nhánh Nậm Hàng", "Code": "CnNamHang", "Type": "CNL2", "ParentUnitId": 1}'
echo "✅ ID: 43 - Chi nhánh Nậm Hàng"

echo ""
echo "3️⃣ Tạo 36 đơn vị Cấp 3 (Phòng ban + PGD)..."

# Cấp 3: Hội sở - 7 phòng ban
curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 3, "Name": "Ban Giám đốc", "Code": "HoiSoBgd", "Type": "PNVL1", "ParentUnitId": 2}'
echo "✅ ID: 3 - Ban Giám đốc (Hội sở)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 4, "Name": "Phòng Khách hàng Doanh nghiệp", "Code": "HoiSoKhdn", "Type": "PNVL1", "ParentUnitId": 2}'
echo "✅ ID: 4 - Phòng Khách hàng Doanh nghiệp"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 5, "Name": "Phòng Khách hàng Cá nhân", "Code": "HoiSoKhcn", "Type": "PNVL1", "ParentUnitId": 2}'
echo "✅ ID: 5 - Phòng Khách hàng Cá nhân"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 6, "Name": "Phòng Kế toán & Ngân quỹ", "Code": "HoiSoKtnq", "Type": "PNVL1", "ParentUnitId": 2}'
echo "✅ ID: 6 - Phòng Kế toán & Ngân quỹ"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 7, "Name": "Phòng Tổng hợp", "Code": "HoiSoTonghop", "Type": "PNVL1", "ParentUnitId": 2}'
echo "✅ ID: 7 - Phòng Tổng hợp"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 8, "Name": "Phòng Kế hoạch & Quản lý rủi ro", "Code": "HoiSoKhqlrr", "Type": "PNVL1", "ParentUnitId": 2}'
echo "✅ ID: 8 - Phòng Kế hoạch & Quản lý rủi ro"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 9, "Name": "Phòng Kiểm tra giám sát", "Code": "HoiSoKtgs", "Type": "PNVL1", "ParentUnitId": 2}'
echo "✅ ID: 9 - Phòng Kiểm tra giám sát"

# Cấp 3: Chi nhánh Bình Lư - 3 phòng
curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 11, "Name": "Ban Giám đốc", "Code": "CnBinhLuBgd", "Type": "PNVL2", "ParentUnitId": 10}'
echo "✅ ID: 11 - Ban Giám đốc (CN Bình Lư)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 12, "Name": "Phòng Kế toán & Ngân quỹ", "Code": "CnBinhLuKtnq", "Type": "PNVL2", "ParentUnitId": 10}'
echo "✅ ID: 12 - Phòng Kế toán & Ngân quỹ (CN Bình Lư)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 13, "Name": "Phòng Khách hàng", "Code": "CnBinhLuKh", "Type": "PNVL2", "ParentUnitId": 10}'
echo "✅ ID: 13 - Phòng Khách hàng (CN Bình Lư)"

# Cấp 3: Chi nhánh Phong Thổ - 3 phòng + 1 PGD
curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 15, "Name": "Ban Giám đốc", "Code": "CnPhongThoBgd", "Type": "PNVL2", "ParentUnitId": 14}'
echo "✅ ID: 15 - Ban Giám đốc (CN Phong Thổ)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 16, "Name": "Phòng Kế toán & Ngân quỹ", "Code": "CnPhongThoKtnq", "Type": "PNVL2", "ParentUnitId": 14}'
echo "✅ ID: 16 - Phòng Kế toán & Ngân quỹ (CN Phong Thổ)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 17, "Name": "Phòng Khách hàng", "Code": "CnPhongThoKh", "Type": "PNVL2", "ParentUnitId": 14}'
echo "✅ ID: 17 - Phòng Khách hàng (CN Phong Thổ)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 18, "Name": "Phòng giao dịch Số 5", "Code": "CnPhongThoPgdSo5", "Type": "PGDL2", "ParentUnitId": 14}'
echo "✅ ID: 18 - Phòng giao dịch Số 5 (CN Phong Thổ)"

# Cấp 3: Chi nhánh Sìn Hồ - 3 phòng
curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 20, "Name": "Ban Giám đốc", "Code": "CnSinHoBgd", "Type": "PNVL2", "ParentUnitId": 19}'
echo "✅ ID: 20 - Ban Giám đốc (CN Sìn Hồ)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 21, "Name": "Phòng Kế toán & Ngân quỹ", "Code": "CnSinHoKtnq", "Type": "PNVL2", "ParentUnitId": 19}'
echo "✅ ID: 21 - Phòng Kế toán & Ngân quỹ (CN Sìn Hồ)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 22, "Name": "Phòng Khách hàng", "Code": "CnSinHoKh", "Type": "PNVL2", "ParentUnitId": 19}'
echo "✅ ID: 22 - Phòng Khách hàng (CN Sìn Hồ)"

# Cấp 3: Chi nhánh Bum Tở - 3 phòng
curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 24, "Name": "Ban Giám đốc", "Code": "CnBumToBgd", "Type": "PNVL2", "ParentUnitId": 23}'
echo "✅ ID: 24 - Ban Giám đốc (CN Bum Tở)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 25, "Name": "Phòng Kế toán & Ngân quỹ", "Code": "CnBumToKtnq", "Type": "PNVL2", "ParentUnitId": 23}'
echo "✅ ID: 25 - Phòng Kế toán & Ngân quỹ (CN Bum Tở)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 26, "Name": "Phòng Khách hàng", "Code": "CnBumToKh", "Type": "PNVL2", "ParentUnitId": 23}'
echo "✅ ID: 26 - Phòng Khách hàng (CN Bum Tở)"

# Cấp 3: Chi nhánh Than Uyên - 3 phòng + 1 PGD
curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 28, "Name": "Ban Giám đốc", "Code": "CnThanUyenBgd", "Type": "PNVL2", "ParentUnitId": 27}'
echo "✅ ID: 28 - Ban Giám đốc (CN Than Uyên)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 29, "Name": "Phòng Kế toán & Ngân quỹ", "Code": "CnThanUyenKtnq", "Type": "PNVL2", "ParentUnitId": 27}'
echo "✅ ID: 29 - Phòng Kế toán & Ngân quỹ (CN Than Uyên)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 30, "Name": "Phòng Khách hàng", "Code": "CnThanUyenKh", "Type": "PNVL2", "ParentUnitId": 27}'
echo "✅ ID: 30 - Phòng Khách hàng (CN Than Uyên)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 31, "Name": "Phòng giao dịch số 6", "Code": "CnThanUyenPgdSo6", "Type": "PGDL2", "ParentUnitId": 27}'
echo "✅ ID: 31 - Phòng giao dịch số 6 (CN Than Uyên)"

# Cấp 3: Chi nhánh Đoàn Kết - 3 phòng + 2 PGD
curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 33, "Name": "Ban Giám đốc", "Code": "CnDoanKetBgd", "Type": "PNVL2", "ParentUnitId": 32}'
echo "✅ ID: 33 - Ban Giám đốc (CN Đoàn Kết)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 34, "Name": "Phòng Kế toán & Ngân quỹ", "Code": "CnDoanKetKtnq", "Type": "PNVL2", "ParentUnitId": 32}'
echo "✅ ID: 34 - Phòng Kế toán & Ngân quỹ (CN Đoàn Kết)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 35, "Name": "Phòng Khách hàng", "Code": "CnDoanKetKh", "Type": "PNVL2", "ParentUnitId": 32}'
echo "✅ ID: 35 - Phòng Khách hàng (CN Đoàn Kết)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 36, "Name": "Phòng giao dịch số 1", "Code": "CnDoanKetPgdso1", "Type": "PGDL2", "ParentUnitId": 32}'
echo "✅ ID: 36 - Phòng giao dịch số 1 (CN Đoàn Kết)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 37, "Name": "Phòng giao dịch số 2", "Code": "CnDoanKetPgdso2", "Type": "PGDL2", "ParentUnitId": 32}'
echo "✅ ID: 37 - Phòng giao dịch số 2 (CN Đoàn Kết)"

# Cấp 3: Chi nhánh Tân Uyên - 3 phòng + 1 PGD
curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 39, "Name": "Ban Giám đốc", "Code": "CnTanUyenBgd", "Type": "PNVL2", "ParentUnitId": 38}'
echo "✅ ID: 39 - Ban Giám đốc (CN Tân Uyên)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 40, "Name": "Phòng Kế toán & Ngân quỹ", "Code": "CnTanUyenKtnq", "Type": "PNVL2", "ParentUnitId": 38}'
echo "✅ ID: 40 - Phòng Kế toán & Ngân quỹ (CN Tân Uyên)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 41, "Name": "Phòng Khách hàng", "Code": "CnTanUyenKh", "Type": "PNVL2", "ParentUnitId": 38}'
echo "✅ ID: 41 - Phòng Khách hàng (CN Tân Uyên)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 42, "Name": "Phòng giao dịch số 3", "Code": "CnTanUyenPgdso3", "Type": "PGDL2", "ParentUnitId": 38}'
echo "✅ ID: 42 - Phòng giao dịch số 3 (CN Tân Uyên)"

# Cấp 3: Chi nhánh Nậm Hàng - 3 phòng
curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 44, "Name": "Ban Giám đốc", "Code": "CnNamHangBgd", "Type": "PNVL2", "ParentUnitId": 43}'
echo "✅ ID: 44 - Ban Giám đốc (CN Nậm Hàng)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 45, "Name": "Phòng Kế toán & Ngân quỹ", "Code": "CnNamHangKtnq", "Type": "PNVL2", "ParentUnitId": 43}'
echo "✅ ID: 45 - Phòng Kế toán & Ngân quỹ (CN Nậm Hàng)"

curl -s -X POST "$API_BASE/Units" \
    -H "Content-Type: application/json" \
    -d '{"Id": 46, "Name": "Phòng Khách hàng", "Code": "CnNamHangKh", "Type": "PNVL2", "ParentUnitId": 43}'
echo "✅ ID: 46 - Phòng Khách hàng (CN Nậm Hàng)"

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
    echo "🎉 THÀNH CÔNG! Đã tạo đủ 46 đơn vị theo cấu trúc 3 cấp mới"
else
    echo "⚠️ Kết quả chưa đạt cấu trúc mong muốn"
fi

echo ""
echo "🏗️ DANH SÁCH ĐƠN VỊ ĐÃ TẠO:"
echo "📊 Cấp 1: Chi nhánh Lai Châu"
echo "📊 Cấp 2: Hội sở + 8 Chi nhánh (Bình Lư, Phong Thổ, Sìn Hồ, Bum Tở, Than Uyên, Đoàn Kết, Tân Uyên, Nậm Hàng)"
echo "📊 Cấp 3: 7 Phòng Hội sở + 24 Phòng Chi nhánh + 5 PGD"

echo "📅 Hoàn thành: $(date)"
