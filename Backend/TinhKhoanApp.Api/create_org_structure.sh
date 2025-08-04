#!/bin/bash

# Script để tạo cấu trúc tổ chức Ver2 - Phiên bản cuối cùng
# Final script to create organizational structure Ver2 - Final version

API_BASE="http://localhost:5055/api"
LOG_FILE="final_structure_$(date +%Y%m%d_%H%M%S).log"
TIMESTAMP=$(date +%Y%m%d%H%M%S)

echo "🚀 Starting Final Organization Structure Creation (Ver2)..." | tee -a $LOG_FILE
echo "📅 Started at: $(date)" | tee -a $LOG_FILE

# Step 1: Create Root node - Chi nhánh Lai Châu
echo "🏢 Creating Root - Chi nhánh Lai Châu (LV1)..." | tee -a $LOG_FILE
root_response=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d '{
  "Code": "CNLC_ROOT",
  "Name": "Chi nhánh Lai Châu",
  "Type": "CNL1",
  "ParentUnitId": null
}')

echo "$root_response" | tee -a $LOG_FILE
echo "" | tee -a $LOG_FILE

# Check if root creation failed due to duplicate code
if echo "$root_response" | grep -q "Mã đơn vị đã tồn tại"; then
    echo "⚠️ Root unit code already exists. Using a unique code..." | tee -a $LOG_FILE
    root_response=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{
      \"Code\": \"CNLC_ROOT_$TIMESTAMP\",
      \"Name\": \"Chi nhánh Lai Châu\",
      \"Type\": \"CNL1\",
      \"ParentUnitId\": null
    }")
    echo "$root_response" | tee -a $LOG_FILE
    echo "" | tee -a $LOG_FILE
fi

# Extract the root ID using grep and cut
root_id=$(echo "$root_response" | grep -o '"Id":[0-9]*' | head -1 | cut -d':' -f2)
if [ -z "$root_id" ]; then
    echo "❌ Failed to get root ID. Exiting." | tee -a $LOG_FILE
    exit 1
fi

echo "✅ Root ID: $root_id" | tee -a $LOG_FILE

# Step 2: Create all branches under root
echo "🏢 Creating branches..." | tee -a $LOG_FILE

# Array to store branch info: code:name:id
branch_info=()

# Create all 9 branches one by one
create_branch() {
    local code=$1
    local name=$2
    local parent_id=$3

    echo "  Creating $name..." | tee -a $LOG_FILE
    branch_response=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{
        \"Code\": \"${code}\",
        \"Name\": \"${name}\",
        \"Type\": \"CNL2\",
        \"ParentUnitId\": ${parent_id}
    }")

    # Check if branch creation failed due to duplicate code
    if echo "$branch_response" | grep -q "Mã đơn vị đã tồn tại"; then
        echo "  ⚠️ Branch code $code already exists. Using unique code..." | tee -a $LOG_FILE
        branch_response=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{
            \"Code\": \"${code}_${TIMESTAMP}\",
            \"Name\": \"${name}\",
            \"Type\": \"CNL2\",
            \"ParentUnitId\": ${parent_id}
        }")
    fi

    branch_id=$(echo "$branch_response" | grep -o '"Id":[0-9]*' | head -1 | cut -d':' -f2)

    if [ -z "$branch_id" ]; then
        echo "  ❌ Failed to create branch $name" | tee -a $LOG_FILE
        echo "  Response: $branch_response" | tee -a $LOG_FILE
        return
    fi

    echo "  ✅ Created branch $name with ID: $branch_id" | tee -a $LOG_FILE
    echo "$code:$name:$branch_id"
}

# Create each branch
HS_info=$(create_branch "CNLC_HS" "Hội sở" $root_id)
BL_info=$(create_branch "CNLC_BL" "CN Bình Lư" $root_id)
PT_info=$(create_branch "CNLC_PT" "CN Phong Thổ" $root_id)
SH_info=$(create_branch "CNLC_SH" "CN Sìn Hồ" $root_id)
BT_info=$(create_branch "CNLC_BT" "CN Bum Tở" $root_id)
TU_info=$(create_branch "CNLC_TU" "CN Than Uyên" $root_id)
DK_info=$(create_branch "CNLC_DK" "CN Đoàn Kết" $root_id)
TUY_info=$(create_branch "CNLC_TUY" "CN Tân Uyên" $root_id)
NH_info=$(create_branch "CNLC_NH" "CN Nậm Hàng" $root_id)

# Extract branch IDs
HS_id=$(echo "$HS_info" | cut -d':' -f3)
BL_id=$(echo "$BL_info" | cut -d':' -f3)
PT_id=$(echo "$PT_info" | cut -d':' -f3)
SH_id=$(echo "$SH_info" | cut -d':' -f3)
BT_id=$(echo "$BT_info" | cut -d':' -f3)
TU_id=$(echo "$TU_info" | cut -d':' -f3)
DK_id=$(echo "$DK_info" | cut -d':' -f3)
TUY_id=$(echo "$TUY_info" | cut -d':' -f3)
NH_id=$(echo "$NH_info" | cut -d':' -f3)

echo "Branch IDs:" | tee -a $LOG_FILE
echo "  - Hội sở: $HS_id" | tee -a $LOG_FILE
echo "  - CN Bình Lư: $BL_id" | tee -a $LOG_FILE
echo "  - CN Phong Thổ: $PT_id" | tee -a $LOG_FILE
echo "  - CN Sìn Hồ: $SH_id" | tee -a $LOG_FILE
echo "  - CN Bum Tở: $BT_id" | tee -a $LOG_FILE
echo "  - CN Than Uyên: $TU_id" | tee -a $LOG_FILE
echo "  - CN Đoàn Kết: $DK_id" | tee -a $LOG_FILE
echo "  - CN Tân Uyên: $TUY_id" | tee -a $LOG_FILE
echo "  - CN Nậm Hàng: $NH_id" | tee -a $LOG_FILE

echo "⏳ Waiting for database to update..." | tee -a $LOG_FILE
sleep 2

# Step 3: Create departments for each branch
echo "🏢 Creating departments..." | tee -a $LOG_FILE

# Function to create departments
create_department() {
    local parent_id=$1
    local code=$2
    local name=$3
    local type=$4

    dept_response=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{
        \"Code\": \"${code}\",
        \"Name\": \"${name}\",
        \"Type\": \"${type}\",
        \"ParentUnitId\": ${parent_id}
    }")

    # Check if department creation failed due to duplicate code
    if echo "$dept_response" | grep -q "Mã đơn vị đã tồn tại"; then
        echo "    ⚠️ Department code $code already exists. Using unique code..." | tee -a $LOG_FILE
        dept_response=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{
            \"Code\": \"${code}_${TIMESTAMP}\",
            \"Name\": \"${name}\",
            \"Type\": \"${type}\",
            \"ParentUnitId\": ${parent_id}
        }")
    fi

    dept_id=$(echo "$dept_response" | grep -o '"Id":[0-9]*' | head -1 | cut -d':' -f2)

    if [ -z "$dept_id" ]; then
        echo "    ❌ Failed to create department $name" | tee -a $LOG_FILE
        echo "    Response: $dept_response" | tee -a $LOG_FILE
        return 1
    else
        echo "    ✅ Created department $name with ID: $dept_id" | tee -a $LOG_FILE
        return 0
    fi
}

# Create departments for Hội sở
if [ -n "$HS_id" ]; then
    echo "  Creating departments for Hội sở..." | tee -a $LOG_FILE
    create_department $HS_id "CNLC_HS_BGD" "Ban Giám đốc" "PNVL1"
    create_department $HS_id "CNLC_HS_KTNQ" "P. KTNQ" "PNVL1"
    create_department $HS_id "CNLC_HS_KHDN" "P. KHDN" "PNVL1"
    create_department $HS_id "CNLC_HS_KHCN" "P. KHCN" "PNVL1"
    create_department $HS_id "CNLC_HS_KTGS" "P. KTGS" "PNVL1"
    create_department $HS_id "CNLC_HS_TH" "P. Tổng Hợp" "PNVL1"
    create_department $HS_id "CNLC_HS_KHQLRR" "P. KHQLRR" "PNVL1"
fi

# Create departments for CN Bình Lư
if [ -n "$BL_id" ]; then
    echo "  Creating departments for CN Bình Lư..." | tee -a $LOG_FILE
    create_department $BL_id "CNLC_BL_BGD" "Ban Giám đốc" "PNVL2"
    create_department $BL_id "CNLC_BL_KTNQ" "P. KTNQ" "PNVL2"
    create_department $BL_id "CNLC_BL_KH" "P. KH" "PNVL2"
fi

# Create departments for CN Phong Thổ
if [ -n "$PT_id" ]; then
    echo "  Creating departments for CN Phong Thổ..." | tee -a $LOG_FILE
    create_department $PT_id "CNLC_PT_BGD" "Ban Giám đốc" "PNVL2"
    create_department $PT_id "CNLC_PT_KTNQ" "P. KTNQ" "PNVL2"
    create_department $PT_id "CNLC_PT_KH" "P. KH" "PNVL2"
    create_department $PT_id "CNLC_PT_S5" "PGD Số 5" "PGDL2"
fi

# Create departments for CN Sìn Hồ
if [ -n "$SH_id" ]; then
    echo "  Creating departments for CN Sìn Hồ..." | tee -a $LOG_FILE
    create_department $SH_id "CNLC_SH_BGD" "Ban Giám đốc" "PNVL2"
    create_department $SH_id "CNLC_SH_KTNQ" "P. KTNQ" "PNVL2"
    create_department $SH_id "CNLC_SH_KH" "P. KH" "PNVL2"
fi

# Create departments for CN Bum Tở
if [ -n "$BT_id" ]; then
    echo "  Creating departments for CN Bum Tở..." | tee -a $LOG_FILE
    create_department $BT_id "CNLC_BT_BGD" "Ban Giám đốc" "PNVL2"
    create_department $BT_id "CNLC_BT_KTNQ" "P. KTNQ" "PNVL2"
    create_department $BT_id "CNLC_BT_KH" "P. KH" "PNVL2"
fi

# Create departments for CN Than Uyên
if [ -n "$TU_id" ]; then
    echo "  Creating departments for CN Than Uyên..." | tee -a $LOG_FILE
    create_department $TU_id "CNLC_TU_BGD" "Ban Giám đốc" "PNVL2"
    create_department $TU_id "CNLC_TU_KTNQ" "P. KTNQ" "PNVL2"
    create_department $TU_id "CNLC_TU_KH" "P. KH" "PNVL2"
    create_department $TU_id "CNLC_TU_S6" "PGD Số 6" "PGDL2"
fi

# Create departments for CN Đoàn Kết
if [ -n "$DK_id" ]; then
    echo "  Creating departments for CN Đoàn Kết..." | tee -a $LOG_FILE
    create_department $DK_id "CNLC_DK_BGD" "Ban Giám đốc" "PNVL2"
    create_department $DK_id "CNLC_DK_KTNQ" "P. KTNQ" "PNVL2"
    create_department $DK_id "CNLC_DK_KH" "P. KH" "PNVL2"
    create_department $DK_id "CNLC_DK_S1" "PGD Số 1" "PGDL2"
    create_department $DK_id "CNLC_DK_S2" "PGD Số 2" "PGDL2"
fi

# Create departments for CN Tân Uyên
if [ -n "$TUY_id" ]; then
    echo "  Creating departments for CN Tân Uyên..." | tee -a $LOG_FILE
    create_department $TUY_id "CNLC_TUY_BGD" "Ban Giám đốc" "PNVL2"
    create_department $TUY_id "CNLC_TUY_KTNQ" "P. KTNQ" "PNVL2"
    create_department $TUY_id "CNLC_TUY_KH" "P. KH" "PNVL2"
    create_department $TUY_id "CNLC_TUY_S3" "PGD Số 3" "PGDL2"
fi

# Create departments for CN Nậm Hàng
if [ -n "$NH_id" ]; then
    echo "  Creating departments for CN Nậm Hàng..." | tee -a $LOG_FILE
    create_department $NH_id "CNLC_NH_BGD" "Ban Giám đốc" "PNVL2"
    create_department $NH_id "CNLC_NH_KTNQ" "P. KTNQ" "PNVL2"
    create_department $NH_id "CNLC_NH_KH" "P. KH" "PNVL2"
fi

# Step 4: Final verification
echo "⏳ Waiting for database to update..." | tee -a $LOG_FILE
sleep 2
echo "🔍 Verifying the structure..." | tee -a $LOG_FILE

# Get all units
all_units=$(curl -s "$API_BASE/Units")
our_root_count=$(echo "$all_units" | grep -c "\"ParentUnitId\":null.*\"Code\":\"CNLC_ROOT")

# Count units by type
cnl1_count=$(echo "$all_units" | grep -c "\"Type\":\"CNL1\"")
cnl2_count=$(echo "$all_units" | grep -c "\"Type\":\"CNL2\"")
pnvl1_count=$(echo "$all_units" | grep -c "\"Type\":\"PNVL1\"")
pnvl2_count=$(echo "$all_units" | grep -c "\"Type\":\"PNVL2\"")
pgdl2_count=$(echo "$all_units" | grep -c "\"Type\":\"PGDL2\"")

echo "📊 Structure Verification:" | tee -a $LOG_FILE
echo "  - CNL1 (Chi nhánh Lai Châu): $cnl1_count" | tee -a $LOG_FILE
echo "  - CNL2 (9 chi nhánh): $cnl2_count" | tee -a $LOG_FILE
echo "  - PNVL1 (7 phòng cấp 1): $pnvl1_count" | tee -a $LOG_FILE
echo "  - PNVL2 (24 phòng cấp 2): $pnvl2_count" | tee -a $LOG_FILE
echo "  - PGDL2 (5 phòng giao dịch): $pgdl2_count" | tee -a $LOG_FILE

# Print root code for reference
echo "" | tee -a $LOG_FILE
echo "🏢 Root unit created with code: CNLC_ROOT or CNLC_ROOT_$TIMESTAMP" | tee -a $LOG_FILE
echo "🎉 Organization structure creation completed!" | tee -a $LOG_FILE
echo "📅 Finished at: $(date)" | tee -a $LOG_FILE
echo "📝 Full log available in: $LOG_FILE" | tee -a $LOG_FILE
