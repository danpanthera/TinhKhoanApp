#!/bin/bash

# Script đơn giản để tạo cấu trúc tổ chức Ver2
# Simple script to create organizational structure Ver2

API_BASE="http://localhost:5055/api"
LOG_FILE="simple_structure_$(date +%Y%m%d_%H%M%S).log"

echo "🚀 Creating Organization Structure Ver2..." | tee -a $LOG_FILE
echo "📅 Started at: $(date)" | tee -a $LOG_FILE

# Function to create a unit and return its ID
create_unit() {
    local code=$1
    local name=$2
    local type=$3
    local parent_id=$4

    if [ "$parent_id" == "null" ]; then
        json_data="{\"Code\": \"$code\", \"Name\": \"$name\", \"Type\": \"$type\", \"ParentUnitId\": null}"
    else
        json_data="{\"Code\": \"$code\", \"Name\": \"$name\", \"Type\": \"$type\", \"ParentUnitId\": $parent_id}"
    fi

    response=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "$json_data")

    # Check for error responses
    if echo "$response" | grep -q "error\|Error\|đã tồn tại"; then
        echo "⚠️ Error creating $name: $response" | tee -a $LOG_FILE
        return 1
    fi

    # Extract ID from response
    id=$(echo "$response" | sed -n 's/.*"Id": *\([0-9]*\).*/\1/p')
    if [ -z "$id" ]; then
        echo "⚠️ Failed to extract ID for $name" | tee -a $LOG_FILE
        return 1
    fi

    echo "✅ Created $name (ID: $id)" | tee -a $LOG_FILE
    echo "$id"
}

# Get current timestamp for unique codes
TIMESTAMP=$(date +%s)

# Step 1: Create Root - Chi nhánh Lai Châu
echo "🏢 Creating root unit..." | tee -a $LOG_FILE
root_id=$(create_unit "ROOT_CNLC_$TIMESTAMP" "Chi nhánh Lai Châu" "CNL1" "null")

if [ -z "$root_id" ]; then
    echo "❌ Failed to create root unit. Exiting." | tee -a $LOG_FILE
    exit 1
fi

# Step 2: Create 9 branches
echo "🏢 Creating branches..." | tee -a $LOG_FILE

hs_id=$(create_unit "HS_$TIMESTAMP" "Hội sở" "CNL2" "$root_id")
bl_id=$(create_unit "BL_$TIMESTAMP" "CN Bình Lư" "CNL2" "$root_id")
pt_id=$(create_unit "PT_$TIMESTAMP" "CN Phong Thổ" "CNL2" "$root_id")
sh_id=$(create_unit "SH_$TIMESTAMP" "CN Sìn Hồ" "CNL2" "$root_id")
bt_id=$(create_unit "BT_$TIMESTAMP" "CN Bum Tở" "CNL2" "$root_id")
tu_id=$(create_unit "TU_$TIMESTAMP" "CN Than Uyên" "CNL2" "$root_id")
dk_id=$(create_unit "DK_$TIMESTAMP" "CN Đoàn Kết" "CNL2" "$root_id")
tuy_id=$(create_unit "TUY_$TIMESTAMP" "CN Tân Uyên" "CNL2" "$root_id")
nh_id=$(create_unit "NH_$TIMESTAMP" "CN Nậm Hàng" "CNL2" "$root_id")

# Step 3: Create departments
echo "🏢 Creating departments..." | tee -a $LOG_FILE

# Hội sở departments (7 departments)
if [ -n "$hs_id" ]; then
    echo "  Hội sở departments..." | tee -a $LOG_FILE
    create_unit "HS_BGD_$TIMESTAMP" "Ban Giám đốc" "PNVL1" "$hs_id"
    create_unit "HS_KTNQ_$TIMESTAMP" "P. KTNQ" "PNVL1" "$hs_id"
    create_unit "HS_KHDN_$TIMESTAMP" "P. KHDN" "PNVL1" "$hs_id"
    create_unit "HS_KHCN_$TIMESTAMP" "P. KHCN" "PNVL1" "$hs_id"
    create_unit "HS_KTGS_$TIMESTAMP" "P. KTGS" "PNVL1" "$hs_id"
    create_unit "HS_TH_$TIMESTAMP" "P. Tổng Hợp" "PNVL1" "$hs_id"
    create_unit "HS_KHQLRR_$TIMESTAMP" "P. KHQLRR" "PNVL1" "$hs_id"
fi

# CN Bình Lư departments (3 departments)
if [ -n "$bl_id" ]; then
    echo "  CN Bình Lư departments..." | tee -a $LOG_FILE
    create_unit "BL_BGD_$TIMESTAMP" "Ban Giám đốc" "PNVL2" "$bl_id"
    create_unit "BL_KTNQ_$TIMESTAMP" "P. KTNQ" "PNVL2" "$bl_id"
    create_unit "BL_KH_$TIMESTAMP" "P. KH" "PNVL2" "$bl_id"
fi

# CN Phong Thổ departments (3 + 1 PGD)
if [ -n "$pt_id" ]; then
    echo "  CN Phong Thổ departments..." | tee -a $LOG_FILE
    create_unit "PT_BGD_$TIMESTAMP" "Ban Giám đốc" "PNVL2" "$pt_id"
    create_unit "PT_KTNQ_$TIMESTAMP" "P. KTNQ" "PNVL2" "$pt_id"
    create_unit "PT_KH_$TIMESTAMP" "P. KH" "PNVL2" "$pt_id"
    create_unit "PT_S5_$TIMESTAMP" "PGD Số 5" "PGDL2" "$pt_id"
fi

# CN Sìn Hồ departments (3 departments)
if [ -n "$sh_id" ]; then
    echo "  CN Sìn Hồ departments..." | tee -a $LOG_FILE
    create_unit "SH_BGD_$TIMESTAMP" "Ban Giám đốc" "PNVL2" "$sh_id"
    create_unit "SH_KTNQ_$TIMESTAMP" "P. KTNQ" "PNVL2" "$sh_id"
    create_unit "SH_KH_$TIMESTAMP" "P. KH" "PNVL2" "$sh_id"
fi

# CN Bum Tở departments (3 departments)
if [ -n "$bt_id" ]; then
    echo "  CN Bum Tở departments..." | tee -a $LOG_FILE
    create_unit "BT_BGD_$TIMESTAMP" "Ban Giám đốc" "PNVL2" "$bt_id"
    create_unit "BT_KTNQ_$TIMESTAMP" "P. KTNQ" "PNVL2" "$bt_id"
    create_unit "BT_KH_$TIMESTAMP" "P. KH" "PNVL2" "$bt_id"
fi

# CN Than Uyên departments (3 + 1 PGD)
if [ -n "$tu_id" ]; then
    echo "  CN Than Uyên departments..." | tee -a $LOG_FILE
    create_unit "TU_BGD_$TIMESTAMP" "Ban Giám đốc" "PNVL2" "$tu_id"
    create_unit "TU_KTNQ_$TIMESTAMP" "P. KTNQ" "PNVL2" "$tu_id"
    create_unit "TU_KH_$TIMESTAMP" "P. KH" "PNVL2" "$tu_id"
    create_unit "TU_S6_$TIMESTAMP" "PGD Số 6" "PGDL2" "$tu_id"
fi

# CN Đoàn Kết departments (3 + 2 PGD)
if [ -n "$dk_id" ]; then
    echo "  CN Đoàn Kết departments..." | tee -a $LOG_FILE
    create_unit "DK_BGD_$TIMESTAMP" "Ban Giám đốc" "PNVL2" "$dk_id"
    create_unit "DK_KTNQ_$TIMESTAMP" "P. KTNQ" "PNVL2" "$dk_id"
    create_unit "DK_KH_$TIMESTAMP" "P. KH" "PNVL2" "$dk_id"
    create_unit "DK_S1_$TIMESTAMP" "PGD Số 1" "PGDL2" "$dk_id"
    create_unit "DK_S2_$TIMESTAMP" "PGD Số 2" "PGDL2" "$dk_id"
fi

# CN Tân Uyên departments (3 + 1 PGD)
if [ -n "$tuy_id" ]; then
    echo "  CN Tân Uyên departments..." | tee -a $LOG_FILE
    create_unit "TUY_BGD_$TIMESTAMP" "Ban Giám đốc" "PNVL2" "$tuy_id"
    create_unit "TUY_KTNQ_$TIMESTAMP" "P. KTNQ" "PNVL2" "$tuy_id"
    create_unit "TUY_KH_$TIMESTAMP" "P. KH" "PNVL2" "$tuy_id"
    create_unit "TUY_S3_$TIMESTAMP" "PGD Số 3" "PGDL2" "$tuy_id"
fi

# CN Nậm Hàng departments (3 departments)
if [ -n "$nh_id" ]; then
    echo "  CN Nậm Hàng departments..." | tee -a $LOG_FILE
    create_unit "NH_BGD_$TIMESTAMP" "Ban Giám đốc" "PNVL2" "$nh_id"
    create_unit "NH_KTNQ_$TIMESTAMP" "P. KTNQ" "PNVL2" "$nh_id"
    create_unit "NH_KH_$TIMESTAMP" "P. KH" "PNVL2" "$nh_id"
fi

# Final summary
echo "" | tee -a $LOG_FILE
echo "🎉 Organization structure creation completed!" | tee -a $LOG_FILE
echo "📅 Finished at: $(date)" | tee -a $LOG_FILE
echo "📝 Full log available in: $LOG_FILE" | tee -a $LOG_FILE
echo "" | tee -a $LOG_FILE
echo "🏢 Structure created:" | tee -a $LOG_FILE
echo "  - 1 Root (CNL1): Chi nhánh Lai Châu" | tee -a $LOG_FILE
echo "  - 9 Branches (CNL2): 9 chi nhánh" | tee -a $LOG_FILE
echo "  - 7 Level 1 Departments (PNVL1): Hội sở departments" | tee -a $LOG_FILE
echo "  - 24 Level 2 Departments (PNVL2): Branch departments" | tee -a $LOG_FILE
echo "  - 5 Transaction Offices (PGDL2): PGD locations" | tee -a $LOG_FILE
echo "  - Total: 46 units created" | tee -a $LOG_FILE
