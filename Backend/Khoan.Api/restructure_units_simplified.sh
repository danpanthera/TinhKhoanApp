#!/bin/bash

# Script để xóa tất cả đơn vị và tạo lại theo danh sách chính xác (Ver2) - Phiên bản đơn giản hơn
# Complete unit restructuring according to Ver2 specifications - Simplified version

set -e

API_BASE="http://localhost:5055/api"
LOG_FILE="restructure_ver2_simplified_$(date +%Y%m%d_%H%M%S).log"

echo "🚀 Starting Organization Restructuring (Ver2) - Simplified..." | tee -a $LOG_FILE
echo "📅 Started at: $(date)" | tee -a $LOG_FILE

# Step 1: Delete ALL existing units
echo "☢️ Deleting all existing units..." | tee -a $LOG_FILE
curl -s -X POST "$API_BASE/Units/DeleteAllUnits" > /dev/null
echo "✅ All existing units deleted" | tee -a $LOG_FILE

echo "⏳ Waiting for database to stabilize..." | tee -a $LOG_FILE
sleep 2

# Step 2: Create new structure - Chi nhánh Lai Châu (LV1)
echo "🏢 Creating Chi nhánh Lai Châu (LV1)..." | tee -a $LOG_FILE
create_root=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d '{
    "Code": "CNLV1_LaiChau",
    "Name": "Chi nhánh Lai Châu",
    "Type": "CNL1",
    "ParentUnitId": null
}')
echo "$create_root" | tee -a $LOG_FILE
echo "✅ Created Chi nhánh Lai Châu (LV1)" | tee -a $LOG_FILE

# Get latest ID for root unit
echo "🔍 Fetching units to get IDs..." | tee -a $LOG_FILE
all_units=$(curl -s "$API_BASE/Units")
root_id=$(echo "$all_units" | grep -o '"Id":[0-9]*,"Code":"CNLV1_LaiChau"' | grep -o '[0-9]*')
echo "🏷️ Chi nhánh Lai Châu ID: $root_id" | tee -a $LOG_FILE

# Step 3: Create branches
echo "🏢 Creating branches (CNL2)..." | tee -a $LOG_FILE

# Create Hội sở
echo "  Creating Hội sở..." | tee -a $LOG_FILE
create_hoi_so=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{
    \"Code\": \"CNLV2_HoiSo\",
    \"Name\": \"Hội sở\",
    \"Type\": \"CNL2\",
    \"ParentUnitId\": $root_id
}")
echo "✅ Created Hội sở" | tee -a $LOG_FILE

# Create CN Bình Lư
echo "  Creating CN Bình Lư..." | tee -a $LOG_FILE
create_binh_lu=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{
    \"Code\": \"CNLV2_BinhLu\",
    \"Name\": \"CN Bình Lư\",
    \"Type\": \"CNL2\",
    \"ParentUnitId\": $root_id
}")
echo "✅ Created CN Bình Lư" | tee -a $LOG_FILE

# Create CN Phong Thổ
echo "  Creating CN Phong Thổ..." | tee -a $LOG_FILE
create_phong_tho=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{
    \"Code\": \"CNLV2_PhongTho\",
    \"Name\": \"CN Phong Thổ\",
    \"Type\": \"CNL2\",
    \"ParentUnitId\": $root_id
}")
echo "✅ Created CN Phong Thổ" | tee -a $LOG_FILE

# Create CN Sìn Hồ
echo "  Creating CN Sìn Hồ..." | tee -a $LOG_FILE
create_sin_ho=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{
    \"Code\": \"CNLV2_SinHo\",
    \"Name\": \"CN Sìn Hồ\",
    \"Type\": \"CNL2\",
    \"ParentUnitId\": $root_id
}")
echo "✅ Created CN Sìn Hồ" | tee -a $LOG_FILE

# Create CN Bum Tở
echo "  Creating CN Bum Tở..." | tee -a $LOG_FILE
create_bum_to=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{
    \"Code\": \"CNLV2_BumTo\",
    \"Name\": \"CN Bum Tở\",
    \"Type\": \"CNL2\",
    \"ParentUnitId\": $root_id
}")
echo "✅ Created CN Bum Tở" | tee -a $LOG_FILE

# Create CN Than Uyên
echo "  Creating CN Than Uyên..." | tee -a $LOG_FILE
create_than_uyen=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{
    \"Code\": \"CNLV2_ThanUyen\",
    \"Name\": \"CN Than Uyên\",
    \"Type\": \"CNL2\",
    \"ParentUnitId\": $root_id
}")
echo "✅ Created CN Than Uyên" | tee -a $LOG_FILE

# Create CN Đoàn Kết
echo "  Creating CN Đoàn Kết..." | tee -a $LOG_FILE
create_doan_ket=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{
    \"Code\": \"CNLV2_DoanKet\",
    \"Name\": \"CN Đoàn Kết\",
    \"Type\": \"CNL2\",
    \"ParentUnitId\": $root_id
}")
echo "✅ Created CN Đoàn Kết" | tee -a $LOG_FILE

# Create CN Tân Uyên
echo "  Creating CN Tân Uyên..." | tee -a $LOG_FILE
create_tan_uyen=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{
    \"Code\": \"CNLV2_TanUyen\",
    \"Name\": \"CN Tân Uyên\",
    \"Type\": \"CNL2\",
    \"ParentUnitId\": $root_id
}")
echo "✅ Created CN Tân Uyên" | tee -a $LOG_FILE

# Create CN Nậm Hàng
echo "  Creating CN Nậm Hàng..." | tee -a $LOG_FILE
create_nam_hang=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{
    \"Code\": \"CNLV2_NamHang\",
    \"Name\": \"CN Nậm Hàng\",
    \"Type\": \"CNL2\",
    \"ParentUnitId\": $root_id
}")
echo "✅ Created CN Nậm Hàng" | tee -a $LOG_FILE

# Step 4: Get branch IDs
echo "⏳ Waiting for database to update..." | tee -a $LOG_FILE
sleep 2
echo "🔍 Getting branch IDs..." | tee -a $LOG_FILE
all_units=$(curl -s "$API_BASE/Units")

hoi_so_id=$(echo "$all_units" | grep -o '"Id":[0-9]*,"Code":"CNLV2_HoiSo"' | grep -o '[0-9]*')
binh_lu_id=$(echo "$all_units" | grep -o '"Id":[0-9]*,"Code":"CNLV2_BinhLu"' | grep -o '[0-9]*')
phong_tho_id=$(echo "$all_units" | grep -o '"Id":[0-9]*,"Code":"CNLV2_PhongTho"' | grep -o '[0-9]*')
sin_ho_id=$(echo "$all_units" | grep -o '"Id":[0-9]*,"Code":"CNLV2_SinHo"' | grep -o '[0-9]*')
bum_to_id=$(echo "$all_units" | grep -o '"Id":[0-9]*,"Code":"CNLV2_BumTo"' | grep -o '[0-9]*')
than_uyen_id=$(echo "$all_units" | grep -o '"Id":[0-9]*,"Code":"CNLV2_ThanUyen"' | grep -o '[0-9]*')
doan_ket_id=$(echo "$all_units" | grep -o '"Id":[0-9]*,"Code":"CNLV2_DoanKet"' | grep -o '[0-9]*')
tan_uyen_id=$(echo "$all_units" | grep -o '"Id":[0-9]*,"Code":"CNLV2_TanUyen"' | grep -o '[0-9]*')
nam_hang_id=$(echo "$all_units" | grep -o '"Id":[0-9]*,"Code":"CNLV2_NamHang"' | grep -o '[0-9]*')

echo "🏷️ Branch IDs:" | tee -a $LOG_FILE
echo "  - Hội sở: $hoi_so_id" | tee -a $LOG_FILE
echo "  - CN Bình Lư: $binh_lu_id" | tee -a $LOG_FILE
echo "  - CN Phong Thổ: $phong_tho_id" | tee -a $LOG_FILE
echo "  - CN Sìn Hồ: $sin_ho_id" | tee -a $LOG_FILE
echo "  - CN Bum Tở: $bum_to_id" | tee -a $LOG_FILE
echo "  - CN Than Uyên: $than_uyen_id" | tee -a $LOG_FILE
echo "  - CN Đoàn Kết: $doan_ket_id" | tee -a $LOG_FILE
echo "  - CN Tân Uyên: $tan_uyen_id" | tee -a $LOG_FILE
echo "  - CN Nậm Hàng: $nam_hang_id" | tee -a $LOG_FILE

# Step 5: Create departments for each branch
echo "🏢 Creating departments for branches..." | tee -a $LOG_FILE

# Hội sở: Gồm 7 Phòng NVL1
echo "  Creating departments for Hội sở..." | tee -a $LOG_FILE
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL1_BGD\",\"Name\":\"Ban Giám đốc\",\"Type\":\"PNVL1\",\"ParentUnitId\":$hoi_so_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL1_KTNQ\",\"Name\":\"P. KTNQ\",\"Type\":\"PNVL1\",\"ParentUnitId\":$hoi_so_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL1_KHDN\",\"Name\":\"P. KHDN\",\"Type\":\"PNVL1\",\"ParentUnitId\":$hoi_so_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL1_KHCN\",\"Name\":\"P. KHCN\",\"Type\":\"PNVL1\",\"ParentUnitId\":$hoi_so_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL1_KTGS\",\"Name\":\"P. KTGS\",\"Type\":\"PNVL1\",\"ParentUnitId\":$hoi_so_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL1_TH\",\"Name\":\"P. Tổng Hợp\",\"Type\":\"PNVL1\",\"ParentUnitId\":$hoi_so_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL1_KHQLRR\",\"Name\":\"P. KHQLRR\",\"Type\":\"PNVL1\",\"ParentUnitId\":$hoi_so_id}" > /dev/null
echo "✅ Created 7 departments for Hội sở" | tee -a $LOG_FILE

# CN Bình Lư: Gồm 3 Phòng NVL2
echo "  Creating departments for CN Bình Lư..." | tee -a $LOG_FILE
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_BL_BGD\",\"Name\":\"Ban Giám đốc\",\"Type\":\"PNVL2\",\"ParentUnitId\":$binh_lu_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_BL_KTNQ\",\"Name\":\"P. KTNQ\",\"Type\":\"PNVL2\",\"ParentUnitId\":$binh_lu_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_BL_KH\",\"Name\":\"P. KH\",\"Type\":\"PNVL2\",\"ParentUnitId\":$binh_lu_id}" > /dev/null
echo "✅ Created 3 departments for CN Bình Lư" | tee -a $LOG_FILE

# CN Phong Thổ: Gồm 4 Phòng NVL2
echo "  Creating departments for CN Phong Thổ..." | tee -a $LOG_FILE
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_PT_BGD\",\"Name\":\"Ban Giám đốc\",\"Type\":\"PNVL2\",\"ParentUnitId\":$phong_tho_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_PT_KTNQ\",\"Name\":\"P. KTNQ\",\"Type\":\"PNVL2\",\"ParentUnitId\":$phong_tho_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_PT_KH\",\"Name\":\"P. KH\",\"Type\":\"PNVL2\",\"ParentUnitId\":$phong_tho_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PGDL3_PT_S5\",\"Name\":\"PGD Số 5\",\"Type\":\"PGDL2\",\"ParentUnitId\":$phong_tho_id}" > /dev/null
echo "✅ Created 4 departments for CN Phong Thổ" | tee -a $LOG_FILE

# CN Sìn Hồ: Gồm 3 Phòng NVL2
echo "  Creating departments for CN Sìn Hồ..." | tee -a $LOG_FILE
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_SH_BGD\",\"Name\":\"Ban Giám đốc\",\"Type\":\"PNVL2\",\"ParentUnitId\":$sin_ho_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_SH_KTNQ\",\"Name\":\"P. KTNQ\",\"Type\":\"PNVL2\",\"ParentUnitId\":$sin_ho_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_SH_KH\",\"Name\":\"P. KH\",\"Type\":\"PNVL2\",\"ParentUnitId\":$sin_ho_id}" > /dev/null
echo "✅ Created 3 departments for CN Sìn Hồ" | tee -a $LOG_FILE

# CN Bum Tở: Gồm 3 Phòng NVL2
echo "  Creating departments for CN Bum Tở..." | tee -a $LOG_FILE
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_BT_BGD\",\"Name\":\"Ban Giám đốc\",\"Type\":\"PNVL2\",\"ParentUnitId\":$bum_to_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_BT_KTNQ\",\"Name\":\"P. KTNQ\",\"Type\":\"PNVL2\",\"ParentUnitId\":$bum_to_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_BT_KH\",\"Name\":\"P. KH\",\"Type\":\"PNVL2\",\"ParentUnitId\":$bum_to_id}" > /dev/null
echo "✅ Created 3 departments for CN Bum Tở" | tee -a $LOG_FILE

# CN Than Uyên: Gồm 4 Phòng NVL2
echo "  Creating departments for CN Than Uyên..." | tee -a $LOG_FILE
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_TU_BGD\",\"Name\":\"Ban Giám đốc\",\"Type\":\"PNVL2\",\"ParentUnitId\":$than_uyen_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_TU_KTNQ\",\"Name\":\"P. KTNQ\",\"Type\":\"PNVL2\",\"ParentUnitId\":$than_uyen_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_TU_KH\",\"Name\":\"P. KH\",\"Type\":\"PNVL2\",\"ParentUnitId\":$than_uyen_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PGDL3_TU_S6\",\"Name\":\"PGD Số 6\",\"Type\":\"PGDL2\",\"ParentUnitId\":$than_uyen_id}" > /dev/null
echo "✅ Created 4 departments for CN Than Uyên" | tee -a $LOG_FILE

# CN Đoàn Kết: Gồm 5 Phòng NVL2
echo "  Creating departments for CN Đoàn Kết..." | tee -a $LOG_FILE
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_DK_BGD\",\"Name\":\"Ban Giám đốc\",\"Type\":\"PNVL2\",\"ParentUnitId\":$doan_ket_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_DK_KTNQ\",\"Name\":\"P. KTNQ\",\"Type\":\"PNVL2\",\"ParentUnitId\":$doan_ket_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_DK_KH\",\"Name\":\"P. KH\",\"Type\":\"PNVL2\",\"ParentUnitId\":$doan_ket_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PGDL3_DK_S1\",\"Name\":\"PGD Số 1\",\"Type\":\"PGDL2\",\"ParentUnitId\":$doan_ket_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PGDL3_DK_S2\",\"Name\":\"PGD Số 2\",\"Type\":\"PGDL2\",\"ParentUnitId\":$doan_ket_id}" > /dev/null
echo "✅ Created 5 departments for CN Đoàn Kết" | tee -a $LOG_FILE

# CN Tân Uyên: Gồm 4 Phòng NVL2
echo "  Creating departments for CN Tân Uyên..." | tee -a $LOG_FILE
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_TUY_BGD\",\"Name\":\"Ban Giám đốc\",\"Type\":\"PNVL2\",\"ParentUnitId\":$tan_uyen_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_TUY_KTNQ\",\"Name\":\"P. KTNQ\",\"Type\":\"PNVL2\",\"ParentUnitId\":$tan_uyen_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_TUY_KH\",\"Name\":\"P. KH\",\"Type\":\"PNVL2\",\"ParentUnitId\":$tan_uyen_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PGDL3_TUY_S3\",\"Name\":\"PGD Số 3\",\"Type\":\"PGDL2\",\"ParentUnitId\":$tan_uyen_id}" > /dev/null
echo "✅ Created 4 departments for CN Tân Uyên" | tee -a $LOG_FILE

# CN Nậm Hàng: Gồm 3 Phòng NVL2
echo "  Creating departments for CN Nậm Hàng..." | tee -a $LOG_FILE
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_NH_BGD\",\"Name\":\"Ban Giám đốc\",\"Type\":\"PNVL2\",\"ParentUnitId\":$nam_hang_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_NH_KTNQ\",\"Name\":\"P. KTNQ\",\"Type\":\"PNVL2\",\"ParentUnitId\":$nam_hang_id}" > /dev/null
curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{\"Code\":\"PNVL3_NH_KH\",\"Name\":\"P. KH\",\"Type\":\"PNVL2\",\"ParentUnitId\":$nam_hang_id}" > /dev/null
echo "✅ Created 3 departments for CN Nậm Hàng" | tee -a $LOG_FILE

# Step 6: Final verification
echo "⏳ Waiting for database to update..." | tee -a $LOG_FILE
sleep 2
echo "🔍 Verifying final structure..." | tee -a $LOG_FILE

# Count units by type
all_units=$(curl -s "$API_BASE/Units")
unit_count=$(echo "$all_units" | grep -o '"Id"' | wc -l)
cnl1_count=$(echo "$all_units" | grep -o '"Type":"CNL1"' | wc -l)
cnl2_count=$(echo "$all_units" | grep -o '"Type":"CNL2"' | wc -l)
pnvl1_count=$(echo "$all_units" | grep -o '"Type":"PNVL1"' | wc -l)
pnvl2_count=$(echo "$all_units" | grep -o '"Type":"PNVL2"' | wc -l)
pgdl2_count=$(echo "$all_units" | grep -o '"Type":"PGDL2"' | wc -l)

echo "📊 Final Unit Count:" | tee -a $LOG_FILE
echo "  - Total Units: $unit_count" | tee -a $LOG_FILE
echo "  - CNL1: $cnl1_count" | tee -a $LOG_FILE
echo "  - CNL2: $cnl2_count" | tee -a $LOG_FILE
echo "  - PNVL1: $pnvl1_count" | tee -a $LOG_FILE
echo "  - PNVL2: $pnvl2_count" | tee -a $LOG_FILE
echo "  - PGDL2: $pgdl2_count" | tee -a $LOG_FILE

echo "🎉 Organization restructuring completed!" | tee -a $LOG_FILE
echo "📅 Finished at: $(date)" | tee -a $LOG_FILE
echo "📝 Log file: $LOG_FILE" | tee -a $LOG_FILE
