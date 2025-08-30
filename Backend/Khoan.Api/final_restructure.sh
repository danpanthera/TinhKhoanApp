#!/bin/bash

# Script để xóa và tạo lại các đơn vị theo danh sách Ver2 - Phiên bản final
# Final script to delete and recreate units according to Ver2 list

API_BASE="http://localhost:5055/api"
LOG_FILE="final_restructure_$(date +%Y%m%d_%H%M%S).log"
TIMESTAMP=$(date +%Y%m%d%H%M%S)

echo "🚀 Starting Final Organization Restructuring (Ver2)..." | tee -a $LOG_FILE
echo "📅 Started at: $(date)" | tee -a $LOG_FILE

# Step 1: Get all units and their codes first
echo "🔍 Getting list of all existing units..." | tee -a $LOG_FILE
all_units=$(curl -s "$API_BASE/Units")
echo "$all_units" > all_units_before.json
unit_count=$(echo "$all_units" | grep -o '"Id"' | wc -l)
echo "📊 Found $unit_count units" | tee -a $LOG_FILE

# Step 2: Delete all existing units - try special endpoint
echo "☢️ Attempting to delete all units using hard delete..." | tee -a $LOG_FILE
curl -s -X DELETE "$API_BASE/Units/HardDeleteAll" | tee -a $LOG_FILE
echo "" | tee -a $LOG_FILE

# Wait for deletion
echo "⏳ Waiting for database to stabilize..." | tee -a $LOG_FILE
sleep 3

# Step 3: Check if units are gone
all_units=$(curl -s "$API_BASE/Units")
unit_count=$(echo "$all_units" | grep -o '"Id"' | wc -l)
echo "📊 Units remaining after hard delete: $unit_count" | tee -a $LOG_FILE

# If units still exist, try manual deletion
if [ "$unit_count" -gt 0 ]; then
    echo "⚠️ Hard delete failed. Trying manual deletion..." | tee -a $LOG_FILE
    unit_ids=$(echo "$all_units" | grep -o '"Id":[0-9]*' | grep -o '[0-9]*')
    for id in $unit_ids; do
      echo "  Deleting unit ID: $id" | tee -a $LOG_FILE
      curl -s -X DELETE "$API_BASE/Units/$id" > /dev/null
      sleep 0.2
    done

    # Check again
    sleep 2
    all_units=$(curl -s "$API_BASE/Units")
    unit_count=$(echo "$all_units" | grep -o '"Id"' | wc -l)
    echo "📊 Units remaining after manual delete: $unit_count" | tee -a $LOG_FILE
fi

# Step 4: Create Root node with timestamp in code to ensure uniqueness
ROOT_CODE="CNLC_${TIMESTAMP}"
echo "🏢 Creating Root - Chi nhánh Lai Châu (LV1) with unique code $ROOT_CODE..." | tee -a $LOG_FILE
response=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{
  \"Code\": \"$ROOT_CODE\",
  \"Name\": \"Chi nhánh Lai Châu\",
  \"Type\": \"CNL1\",
  \"ParentUnitId\": null
}")
echo "$response" | tee -a $LOG_FILE
echo "" | tee -a $LOG_FILE

# Extract the root ID from the response
root_id=$(echo "$response" | grep -o '"Id":[0-9]*' | head -1 | grep -o '[0-9]*')
if [ -z "$root_id" ]; then
    echo "❌ Failed to get root ID from response, trying from all units..." | tee -a $LOG_FILE

    # Wait and try to get from all units
    sleep 2
    all_units=$(curl -s "$API_BASE/Units")
    root_id=$(echo "$all_units" | grep -o "\"Id\":[0-9]*,\"Code\":\"$ROOT_CODE\"" | grep -o '[0-9]*')

    if [ -z "$root_id" ]; then
        echo "❌ Still failed to get root ID" | tee -a $LOG_FILE
        exit 1
    fi
fi
echo "🏷️ Root ID: $root_id" | tee -a $LOG_FILE# Step 5: Create all branches under root
echo "🏢 Creating branches..." | tee -a $LOG_FILE

# Create all 9 branches in a loop
branches=(
    "HS:Hội sở"
    "BL:CN Bình Lư"
    "PT:CN Phong Thổ"
    "SH:CN Sìn Hồ"
    "BT:CN Bum Tở"
    "TU:CN Than Uyên"
    "DK:CN Đoàn Kết"
    "TUY:CN Tân Uyên"
    "NH:CN Nậm Hàng"
)

branch_ids=()
for branch in "${branches[@]}"; do
    code=$(echo "$branch" | cut -d: -f1)
    name=$(echo "$branch" | cut -d: -f2)

    echo "  Creating $name..." | tee -a $LOG_FILE
    curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{
        \"Code\": \"CNLV2_$code\",
        \"Name\": \"$name\",
        \"Type\": \"CNL2\",
        \"ParentUnitId\": $root_id
    }" > /dev/null

    echo "✅ Created $name" | tee -a $LOG_FILE
done

echo "⏳ Waiting for database to update..." | tee -a $LOG_FILE
sleep 2

# Step 6: Get all branch IDs
all_units=$(curl -s "$API_BASE/Units")
hoi_so_id=$(echo "$all_units" | grep -o '"Id":[0-9]*,"Code":"CNLV2_HS"' | grep -o '[0-9]*')
binh_lu_id=$(echo "$all_units" | grep -o '"Id":[0-9]*,"Code":"CNLV2_BL"' | grep -o '[0-9]*')
phong_tho_id=$(echo "$all_units" | grep -o '"Id":[0-9]*,"Code":"CNLV2_PT"' | grep -o '[0-9]*')
sin_ho_id=$(echo "$all_units" | grep -o '"Id":[0-9]*,"Code":"CNLV2_SH"' | grep -o '[0-9]*')
bum_to_id=$(echo "$all_units" | grep -o '"Id":[0-9]*,"Code":"CNLV2_BT"' | grep -o '[0-9]*')
than_uyen_id=$(echo "$all_units" | grep -o '"Id":[0-9]*,"Code":"CNLV2_TU"' | grep -o '[0-9]*')
doan_ket_id=$(echo "$all_units" | grep -o '"Id":[0-9]*,"Code":"CNLV2_DK"' | grep -o '[0-9]*')
tan_uyen_id=$(echo "$all_units" | grep -o '"Id":[0-9]*,"Code":"CNLV2_TUY"' | grep -o '[0-9]*')
nam_hang_id=$(echo "$all_units" | grep -o '"Id":[0-9]*,"Code":"CNLV2_NH"' | grep -o '[0-9]*')

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

# Step 7: Create departments for each branch
echo "🏢 Creating departments..." | tee -a $LOG_FILE

# Function to create departments
create_departments() {
    local branch_id=$1
    local branch_code=$2
    local departments=("${@:3}")

    for dept in "${departments[@]}"; do
        local code=$(echo "$dept" | cut -d: -f1)
        local name=$(echo "$dept" | cut -d: -f2)
        local type=$(echo "$dept" | cut -d: -f3)

        curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{
            \"Code\": \"${branch_code}_${code}\",
            \"Name\": \"$name\",
            \"Type\": \"$type\",
            \"ParentUnitId\": $branch_id
        }" > /dev/null
    done

    echo "✅ Created departments for branch $branch_code" | tee -a $LOG_FILE
}

# Hội sở departments
hoi_so_depts=(
    "BGD:Ban Giám đốc:PNVL1"
    "KTNQ:P. KTNQ:PNVL1"
    "KHDN:P. KHDN:PNVL1"
    "KHCN:P. KHCN:PNVL1"
    "KTGS:P. KTGS:PNVL1"
    "TH:P. Tổng Hợp:PNVL1"
    "KHQLRR:P. KHQLRR:PNVL1"
)
create_departments "$hoi_so_id" "HS" "${hoi_so_depts[@]}"

# CN Bình Lư departments
binh_lu_depts=(
    "BGD:Ban Giám đốc:PNVL2"
    "KTNQ:P. KTNQ:PNVL2"
    "KH:P. KH:PNVL2"
)
create_departments "$binh_lu_id" "BL" "${binh_lu_depts[@]}"

# CN Phong Thổ departments
phong_tho_depts=(
    "BGD:Ban Giám đốc:PNVL2"
    "KTNQ:P. KTNQ:PNVL2"
    "KH:P. KH:PNVL2"
    "S5:PGD Số 5:PGDL2"
)
create_departments "$phong_tho_id" "PT" "${phong_tho_depts[@]}"

# CN Sìn Hồ departments
sin_ho_depts=(
    "BGD:Ban Giám đốc:PNVL2"
    "KTNQ:P. KTNQ:PNVL2"
    "KH:P. KH:PNVL2"
)
create_departments "$sin_ho_id" "SH" "${sin_ho_depts[@]}"

# CN Bum Tở departments
bum_to_depts=(
    "BGD:Ban Giám đốc:PNVL2"
    "KTNQ:P. KTNQ:PNVL2"
    "KH:P. KH:PNVL2"
)
create_departments "$bum_to_id" "BT" "${bum_to_depts[@]}"

# CN Than Uyên departments
than_uyen_depts=(
    "BGD:Ban Giám đốc:PNVL2"
    "KTNQ:P. KTNQ:PNVL2"
    "KH:P. KH:PNVL2"
    "S6:PGD Số 6:PGDL2"
)
create_departments "$than_uyen_id" "TU" "${than_uyen_depts[@]}"

# CN Đoàn Kết departments
doan_ket_depts=(
    "BGD:Ban Giám đốc:PNVL2"
    "KTNQ:P. KTNQ:PNVL2"
    "KH:P. KH:PNVL2"
    "S1:PGD Số 1:PGDL2"
    "S2:PGD Số 2:PGDL2"
)
create_departments "$doan_ket_id" "DK" "${doan_ket_depts[@]}"

# CN Tân Uyên departments
tan_uyen_depts=(
    "BGD:Ban Giám đốc:PNVL2"
    "KTNQ:P. KTNQ:PNVL2"
    "KH:P. KH:PNVL2"
    "S3:PGD Số 3:PGDL2"
)
create_departments "$tan_uyen_id" "TUY" "${tan_uyen_depts[@]}"

# CN Nậm Hàng departments
nam_hang_depts=(
    "BGD:Ban Giám đốc:PNVL2"
    "KTNQ:P. KTNQ:PNVL2"
    "KH:P. KH:PNVL2"
)
create_departments "$nam_hang_id" "NH" "${nam_hang_depts[@]}"

# Step 8: Final verification
echo "⏳ Waiting for database to update..." | tee -a $LOG_FILE
sleep 2
echo "🔍 Final verification..." | tee -a $LOG_FILE

# Count total units and by type
all_units=$(curl -s "$API_BASE/Units")
unit_count=$(echo "$all_units" | grep -o '"Id"' | wc -l)
cnl1_count=$(echo "$all_units" | grep -o '"Type":"CNL1"' | wc -l)
cnl2_count=$(echo "$all_units" | grep -o '"Type":"CNL2"' | wc -l)
pnvl1_count=$(echo "$all_units" | grep -o '"Type":"PNVL1"' | wc -l)
pnvl2_count=$(echo "$all_units" | grep -o '"Type":"PNVL2"' | wc -l)
pgdl2_count=$(echo "$all_units" | grep -o '"Type":"PGDL2"' | wc -l)

echo "📊 Final Unit Count:" | tee -a $LOG_FILE
echo "  - Total Units: $unit_count" | tee -a $LOG_FILE
echo "  - CNL1: $cnl1_count (Expected: 1)" | tee -a $LOG_FILE
echo "  - CNL2: $cnl2_count (Expected: 9)" | tee -a $LOG_FILE
echo "  - PNVL1: $pnvl1_count (Expected: 7)" | tee -a $LOG_FILE
echo "  - PNVL2: $pnvl2_count (Expected: 24)" | tee -a $LOG_FILE
echo "  - PGDL2: $pgdl2_count (Expected: 5)" | tee -a $LOG_FILE
echo "  - Expected Total: 46, Actual: $unit_count" | tee -a $LOG_FILE

if [ "$unit_count" -eq 46 ] && [ "$cnl1_count" -eq 1 ] && [ "$cnl2_count" -eq 9 ] &&
   [ "$pnvl1_count" -eq 7 ] && [ "$pnvl2_count" -eq 24 ] && [ "$pgdl2_count" -eq 5 ]; then
    echo "✅ RESTRUCTURING SUCCESSFUL! All units created correctly." | tee -a $LOG_FILE
else
    echo "⚠️ VERIFICATION WARNING: Unit counts don't match expectations." | tee -a $LOG_FILE
fi

echo "🎉 Organization restructuring completed!" | tee -a $LOG_FILE
echo "📅 Finished at: $(date)" | tee -a $LOG_FILE
echo "📝 Full log available in: $LOG_FILE" | tee -a $LOG_FILE
