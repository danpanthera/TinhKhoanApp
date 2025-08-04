#!/bin/bash

# Script để xóa tất cả đơn vị và tạo lại theo danh sách chính xác (Ver2)
# Complete unit restructuring according to Ver2 specifications

set -e

API_BASE="http://localhost:5055/api"
LOG_FILE="complete_restructure_ver2_$(date +%Y%m%d_%H%M%S).log"

echo "🚀 Starting Complete Organization Restructuring (Ver2)..." | tee -a $LOG_FILE
echo "📅 Started at: $(date)" | tee -a $LOG_FILE
echo "📝 Log file: $LOG_FILE" | tee -a $LOG_FILE

# Function to call API with error handling
call_api() {
    local method=$1
    local endpoint=$2
    local data=$3
    local description=$4

    echo "🔄 $description..." | tee -a $LOG_FILE

    if [ "$method" = "DELETE" ]; then
        response=$(curl -s -w "\nHTTP_STATUS:%{http_code}" -X DELETE "$API_BASE$endpoint")
    elif [ "$method" = "POST" ]; then
        response=$(curl -s -w "\nHTTP_STATUS:%{http_code}" -X POST "$API_BASE$endpoint" \
            -H "Content-Type: application/json" \
            -d "$data")
    elif [ "$method" = "GET" ]; then
        response=$(curl -s -w "\nHTTP_STATUS:%{http_code}" "$API_BASE$endpoint")
    fi

    http_status=$(echo "$response" | grep "HTTP_STATUS:" | cut -d: -f2)
    response_body=$(echo "$response" | sed '/HTTP_STATUS:/d')

    if [ "$http_status" -ge 200 ] && [ "$http_status" -lt 300 ]; then
        echo "✅ Success: $description (HTTP $http_status)" | tee -a $LOG_FILE
        echo "$response_body"
        return 0
    else
        echo "❌ Failed: $description (HTTP $http_status)" | tee -a $LOG_FILE
        echo "Response: $response_body" | tee -a $LOG_FILE
        return 1
    fi
}

# Step 1: Delete ALL existing units - NUCLEAR OPTION
echo "☢️ Using nuclear option to delete all units at once..." | tee -a $LOG_FILE
curl -s -X POST "$API_BASE/Units/DeleteAllUnits" | tee -a $LOG_FILE
echo "✅ All existing units deleted" | tee -a $LOG_FILE

# Step 2: Create Chi nhánh Lai Châu (LV1) - Root node
echo "🏢 Creating Chi nhánh Lai Châu (LV1) - Root node..." | tee -a $LOG_FILE
lai_chau_data='{"Code":"CNLV1_LaiChau","Name":"Chi nhánh Lai Châu","Type":"CNL1","ParentUnitId":null}'
lai_chau_response=$(call_api "POST" "/Units" "$lai_chau_data" "Create Chi nhánh Lai Châu (LV1)")
lai_chau_id=$(echo "$lai_chau_response" | jq -r '.Id // empty')

if [ -z "$lai_chau_id" ] || [ "$lai_chau_id" = "null" ]; then
    echo "❌ Failed to create Chi nhánh Lai Châu - cannot continue" | tee -a $LOG_FILE
    exit 1
fi

echo "✅ Chi nhánh Lai Châu created with ID: $lai_chau_id" | tee -a $LOG_FILE

# Step 3: Create Hội sở (LV2) with 7 departments
echo "🏢 Creating Hội sở (LV2)..." | tee -a $LOG_FILE
hoi_so_data="{\"Code\":\"CNLV2_HoiSo\",\"Name\":\"Hội sở\",\"Type\":\"CNL2\",\"ParentUnitId\":$lai_chau_id}"
hoi_so_response=$(call_api "POST" "/Units" "$hoi_so_data" "Create Hội sở (LV2)")
hoi_so_id=$(echo "$hoi_so_response" | jq -r '.Id // empty')

if [ -z "$hoi_so_id" ] || [ "$hoi_so_id" = "null" ]; then
    echo "❌ Failed to create Hội sở" | tee -a $LOG_FILE
else
    echo "✅ Hội sở created with ID: $hoi_so_id" | tee -a $LOG_FILE

    # Create 7 NVL1 departments under Hội sở
    departments_hoi_so=(
        "PNVL1_BGD:Ban Giám đốc"
        "PNVL1_KTNQ:Phòng KTNQ"
        "PNVL1_KHDN:Phòng KHDN"
        "PNVL1_KHCN:Phòng KHCN"
        "PNVL1_KTGS:Phòng KTGS"
        "PNVL1_TH:Phòng Tổng Hợp"
        "PNVL1_KHQLRR:Phòng KHQLRR"
    )

    for dept in "${departments_hoi_so[@]}"; do
        code=$(echo "$dept" | cut -d: -f1)
        name=$(echo "$dept" | cut -d: -f2)
        dept_data="{\"Code\":\"$code\",\"Name\":\"$name\",\"Type\":\"PNVL1\",\"ParentUnitId\":$hoi_so_id}"
        call_api "POST" "/Units" "$dept_data" "Create $name" || true
        sleep 0.3
    done
fi

# Step 4: Create CN Bình Lư (LV2) with 3 departments
echo "🏢 Creating CN Bình Lư (LV2)..." | tee -a $LOG_FILE
binh_lu_data="{\"Code\":\"CNLV2_BinhLu\",\"Name\":\"CN Bình Lư\",\"Type\":\"CNL2\",\"ParentUnitId\":$lai_chau_id}"
binh_lu_response=$(call_api "POST" "/Units" "$binh_lu_data" "Create CN Bình Lư (LV2)")
binh_lu_id=$(echo "$binh_lu_response" | jq -r '.Id // empty')

if [ -z "$binh_lu_id" ] || [ "$binh_lu_id" = "null" ]; then
    echo "❌ Failed to create CN Bình Lư" | tee -a $LOG_FILE
else
    echo "✅ CN Bình Lư created with ID: $binh_lu_id" | tee -a $LOG_FILE

    # Create 3 departments under CN Bình Lư
    departments_binh_lu=(
        "PNVL3_BL_BGD:Ban Giám đốc"
        "PNVL3_BL_KTNQ:Phòng KTNQ"
        "PNVL3_BL_KH:Phòng KH"
    )

    for dept in "${departments_binh_lu[@]}"; do
        code=$(echo "$dept" | cut -d: -f1)
        name=$(echo "$dept" | cut -d: -f2)
        dept_data="{\"Code\":\"$code\",\"Name\":\"$name\",\"Type\":\"PNVL2\",\"ParentUnitId\":$binh_lu_id}"
        call_api "POST" "/Units" "$dept_data" "Create $name" || true
        sleep 0.3
    done
fi

# Step 5: Create CN Phong Thổ (LV2) with 4 departments
echo "🏢 Creating CN Phong Thổ (LV2)..." | tee -a $LOG_FILE
phong_tho_data="{\"Code\":\"CNLV2_PhongTho\",\"Name\":\"CN Phong Thổ\",\"Type\":\"CNL2\",\"ParentUnitId\":$lai_chau_id}"
phong_tho_response=$(call_api "POST" "/Units" "$phong_tho_data" "Create CN Phong Thổ (LV2)")
phong_tho_id=$(echo "$phong_tho_response" | jq -r '.Id // empty')

if [ -z "$phong_tho_id" ] || [ "$phong_tho_id" = "null" ]; then
    echo "❌ Failed to create CN Phong Thổ" | tee -a $LOG_FILE
else
    echo "✅ CN Phong Thổ created with ID: $phong_tho_id" | tee -a $LOG_FILE

    # Create 4 departments under CN Phong Thổ
    departments_phong_tho=(
        "PNVL3_PT_BGD:Ban Giám đốc"
        "PNVL3_PT_KTNQ:Phòng KTNQ"
        "PNVL3_PT_KH:Phòng KH"
        "PGDL3_PT_S5:PGD Số 5"
    )

    for dept in "${departments_phong_tho[@]}"; do
        code=$(echo "$dept" | cut -d: -f1)
        name=$(echo "$dept" | cut -d: -f2)
        dept_type=$(echo "$code" | cut -d_ -f1 | grep -q "PGD" && echo "PGDL2" || echo "PNVL2")
        dept_data="{\"Code\":\"$code\",\"Name\":\"$name\",\"Type\":\"$dept_type\",\"ParentUnitId\":$phong_tho_id}"
        call_api "POST" "/Units" "$dept_data" "Create $name" || true
        sleep 0.3
    done
fi

# Step 6: Create CN Sìn Hồ (LV2) with 3 departments
echo "🏢 Creating CN Sìn Hồ (LV2)..." | tee -a $LOG_FILE
sin_ho_data="{\"Code\":\"CNLV2_SinHo\",\"Name\":\"CN Sìn Hồ\",\"Type\":\"CNL2\",\"ParentUnitId\":$lai_chau_id}"
sin_ho_response=$(call_api "POST" "/Units" "$sin_ho_data" "Create CN Sìn Hồ (LV2)")
sin_ho_id=$(echo "$sin_ho_response" | jq -r '.Id // empty')

if [ -z "$sin_ho_id" ] || [ "$sin_ho_id" = "null" ]; then
    echo "❌ Failed to create CN Sìn Hồ" | tee -a $LOG_FILE
else
    echo "✅ CN Sìn Hồ created with ID: $sin_ho_id" | tee -a $LOG_FILE

    # Create 3 departments under CN Sìn Hồ
    departments_sin_ho=(
        "PNVL3_SH_BGD:Ban Giám đốc"
        "PNVL3_SH_KTNQ:Phòng KTNQ"
        "PNVL3_SH_KH:Phòng KH"
    )

    for dept in "${departments_sin_ho[@]}"; do
        code=$(echo "$dept" | cut -d: -f1)
        name=$(echo "$dept" | cut -d: -f2)
        dept_data="{\"Code\":\"$code\",\"Name\":\"$name\",\"Type\":\"PNVL2\",\"ParentUnitId\":$sin_ho_id}"
        call_api "POST" "/Units" "$dept_data" "Create $name" || true
        sleep 0.3
    done
fi

# Step 7: Create CN Bum Tở (LV2) with 3 departments
echo "🏢 Creating CN Bum Tở (LV2)..." | tee -a $LOG_FILE
bum_to_data="{\"Code\":\"CNLV2_BumTo\",\"Name\":\"CN Bum Tở\",\"Type\":\"CNL2\",\"ParentUnitId\":$lai_chau_id}"
bum_to_response=$(call_api "POST" "/Units" "$bum_to_data" "Create CN Bum Tở (LV2)")
bum_to_id=$(echo "$bum_to_response" | jq -r '.Id // empty')

if [ -z "$bum_to_id" ] || [ "$bum_to_id" = "null" ]; then
    echo "❌ Failed to create CN Bum Tở" | tee -a $LOG_FILE
else
    echo "✅ CN Bum Tở created with ID: $bum_to_id" | tee -a $LOG_FILE

    # Create 3 departments under CN Bum Tở
    departments_bum_to=(
        "PNVL3_BT_BGD:Ban Giám đốc"
        "PNVL3_BT_KTNQ:Phòng KTNQ"
        "PNVL3_BT_KH:Phòng KH"
    )

    for dept in "${departments_bum_to[@]}"; do
        code=$(echo "$dept" | cut -d: -f1)
        name=$(echo "$dept" | cut -d: -f2)
        dept_data="{\"Code\":\"$code\",\"Name\":\"$name\",\"Type\":\"PNVL2\",\"ParentUnitId\":$bum_to_id}"
        call_api "POST" "/Units" "$dept_data" "Create $name" || true
        sleep 0.3
    done
fi

# Step 8: Create CN Than Uyên (LV2) with 4 departments
echo "🏢 Creating CN Than Uyên (LV2)..." | tee -a $LOG_FILE
than_uyen_data="{\"Code\":\"CNLV2_ThanUyen\",\"Name\":\"CN Than Uyên\",\"Type\":\"CNL2\",\"ParentUnitId\":$lai_chau_id}"
than_uyen_response=$(call_api "POST" "/Units" "$than_uyen_data" "Create CN Than Uyên (LV2)")
than_uyen_id=$(echo "$than_uyen_response" | jq -r '.Id // empty')

if [ -z "$than_uyen_id" ] || [ "$than_uyen_id" = "null" ]; then
    echo "❌ Failed to create CN Than Uyên" | tee -a $LOG_FILE
else
    echo "✅ CN Than Uyên created with ID: $than_uyen_id" | tee -a $LOG_FILE

    # Create 4 departments under CN Than Uyên
    departments_than_uyen=(
        "PNVL3_TU_BGD:Ban Giám đốc"
        "PNVL3_TU_KTNQ:Phòng KTNQ"
        "PNVL3_TU_KH:Phòng KH"
        "PGDL3_TU_S6:PGD Số 6"
    )

    for dept in "${departments_than_uyen[@]}"; do
        code=$(echo "$dept" | cut -d: -f1)
        name=$(echo "$dept" | cut -d: -f2)
        dept_type=$(echo "$code" | cut -d_ -f1 | grep -q "PGD" && echo "PGDL2" || echo "PNVL2")
        dept_data="{\"Code\":\"$code\",\"Name\":\"$name\",\"Type\":\"$dept_type\",\"ParentUnitId\":$than_uyen_id}"
        call_api "POST" "/Units" "$dept_data" "Create $name" || true
        sleep 0.3
    done
fi

# Step 9: Create CN Đoàn Kết (LV2) with 5 departments
echo "🏢 Creating CN Đoàn Kết (LV2)..." | tee -a $LOG_FILE
doan_ket_data="{\"Code\":\"CNLV2_DoanKet\",\"Name\":\"CN Đoàn Kết\",\"Type\":\"CNL2\",\"ParentUnitId\":$lai_chau_id}"
doan_ket_response=$(call_api "POST" "/Units" "$doan_ket_data" "Create CN Đoàn Kết (LV2)")
doan_ket_id=$(echo "$doan_ket_response" | jq -r '.Id // empty')

if [ -z "$doan_ket_id" ] || [ "$doan_ket_id" = "null" ]; then
    echo "❌ Failed to create CN Đoàn Kết" | tee -a $LOG_FILE
else
    echo "✅ CN Đoàn Kết created with ID: $doan_ket_id" | tee -a $LOG_FILE

    # Create 5 departments under CN Đoàn Kết
    departments_doan_ket=(
        "PNVL3_DK_BGD:Ban Giám đốc"
        "PNVL3_DK_KTNQ:Phòng KTNQ"
        "PNVL3_DK_KH:Phòng KH"
        "PGDL3_DK_S1:PGD Số 1"
        "PGDL3_DK_S2:PGD Số 2"
    )

    for dept in "${departments_doan_ket[@]}"; do
        code=$(echo "$dept" | cut -d: -f1)
        name=$(echo "$dept" | cut -d: -f2)
        dept_type=$(echo "$code" | cut -d_ -f1 | grep -q "PGD" && echo "PGDL2" || echo "PNVL2")
        dept_data="{\"Code\":\"$code\",\"Name\":\"$name\",\"Type\":\"$dept_type\",\"ParentUnitId\":$doan_ket_id}"
        call_api "POST" "/Units" "$dept_data" "Create $name" || true
        sleep 0.3
    done
fi

# Step 10: Create CN Tân Uyên (LV2) with 4 departments
echo "🏢 Creating CN Tân Uyên (LV2)..." | tee -a $LOG_FILE
tan_uyen_data="{\"Code\":\"CNLV2_TanUyen\",\"Name\":\"CN Tân Uyên\",\"Type\":\"CNL2\",\"ParentUnitId\":$lai_chau_id}"
tan_uyen_response=$(call_api "POST" "/Units" "$tan_uyen_data" "Create CN Tân Uyên (LV2)")
tan_uyen_id=$(echo "$tan_uyen_response" | jq -r '.Id // empty')

if [ -z "$tan_uyen_id" ] || [ "$tan_uyen_id" = "null" ]; then
    echo "❌ Failed to create CN Tân Uyên" | tee -a $LOG_FILE
else
    echo "✅ CN Tân Uyên created with ID: $tan_uyen_id" | tee -a $LOG_FILE

    # Create 4 departments under CN Tân Uyên
    departments_tan_uyen=(
        "PNVL3_TUY_BGD:Ban Giám đốc"
        "PNVL3_TUY_KTNQ:Phòng KTNQ"
        "PNVL3_TUY_KH:Phòng KH"
        "PGDL3_TUY_S3:PGD Số 3"
    )

    for dept in "${departments_tan_uyen[@]}"; do
        code=$(echo "$dept" | cut -d: -f1)
        name=$(echo "$dept" | cut -d: -f2)
        dept_type=$(echo "$code" | cut -d_ -f1 | grep -q "PGD" && echo "PGDL2" || echo "PNVL2")
        dept_data="{\"Code\":\"$code\",\"Name\":\"$name\",\"Type\":\"$dept_type\",\"ParentUnitId\":$tan_uyen_id}"
        call_api "POST" "/Units" "$dept_data" "Create $name" || true
        sleep 0.3
    done
fi

# Step 11: Create CN Nậm Hàng (LV2) with 3 departments
echo "🏢 Creating CN Nậm Hàng (LV2)..." | tee -a $LOG_FILE
nam_hang_data="{\"Code\":\"CNLV2_NamHang\",\"Name\":\"CN Nậm Hàng\",\"Type\":\"CNL2\",\"ParentUnitId\":$lai_chau_id}"
nam_hang_response=$(call_api "POST" "/Units" "$nam_hang_data" "Create CN Nậm Hàng (LV2)")
nam_hang_id=$(echo "$nam_hang_response" | jq -r '.Id // empty')

if [ -z "$nam_hang_id" ] || [ "$nam_hang_id" = "null" ]; then
    echo "❌ Failed to create CN Nậm Hàng" | tee -a $LOG_FILE
else
    echo "✅ CN Nậm Hàng created with ID: $nam_hang_id" | tee -a $LOG_FILE

    # Create 3 departments under CN Nậm Hàng
    departments_nam_hang=(
        "PNVL3_NH_BGD:Ban Giám đốc"
        "PNVL3_NH_KTNQ:Phòng KTNQ"
        "PNVL3_NH_KH:Phòng KH"
    )

    for dept in "${departments_nam_hang[@]}"; do
        code=$(echo "$dept" | cut -d: -f1)
        name=$(echo "$dept" | cut -d: -f2)
        dept_data="{\"Code\":\"$code\",\"Name\":\"$name\",\"Type\":\"PNVL2\",\"ParentUnitId\":$nam_hang_id}"
        call_api "POST" "/Units" "$dept_data" "Create $name" || true
        sleep 0.3
    done
fi

# Step 12: Final verification
echo "" | tee -a $LOG_FILE
echo "🔍 Verifying organizational structure..." | tee -a $LOG_FILE

final_units_count=$(curl -s "$API_BASE/Units" | jq 'length')
echo "📊 Total units created: $final_units_count" | tee -a $LOG_FILE

# Display tree structure
echo "🌳 Complete organizational tree structure:" | tee -a $LOG_FILE
echo "📋 CNL1 (Root):" | tee -a $LOG_FILE
curl -s "$API_BASE/Units" | jq -r '.[] | select(.Type == "CNL1") | "  - \(.Name) (ID: \(.Id))"' | tee -a $LOG_FILE

echo "📋 CNL2 Branches:" | tee -a $LOG_FILE
curl -s "$API_BASE/Units" | jq -r '.[] | select(.Type == "CNL2") | "  - \(.Name) (ID: \(.Id))"' | tee -a $LOG_FILE

echo "📋 PNVL1 Departments:" | tee -a $LOG_FILE
curl -s "$API_BASE/Units" | jq -r '.[] | select(.Type == "PNVL1") | "  - \(.Name) (Parent: \(.ParentUnitId))"' | tee -a $LOG_FILE

echo "📋 PNVL2 Departments:" | tee -a $LOG_FILE
curl -s "$API_BASE/Units" | jq -r '.[] | select(.Type == "PNVL2") | "  - \(.Name) (Parent: \(.ParentUnitId))"' | tee -a $LOG_FILE

echo "📋 PGD Units:" | tee -a $LOG_FILE
curl -s "$API_BASE/Units" | jq -r '.[] | select(.Type == "PGDL2") | "  - \(.Name) (Parent: \(.ParentUnitId))"' | tee -a $LOG_FILE

echo "" | tee -a $LOG_FILE
echo "🎉 Complete organizational restructuring finished!" | tee -a $LOG_FILE
echo "📅 Completed at: $(date)" | tee -a $LOG_FILE
echo "📝 Full log available in: $LOG_FILE" | tee -a $LOG_FILE
