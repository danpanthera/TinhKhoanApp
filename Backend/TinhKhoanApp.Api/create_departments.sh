#!/bin/bash

# Script để tạo các phòng nghiệp vụ dưới các chi nhánh mới
# Create departments under the new branches

set -e

API_BASE="http://localhost:5055/api"
LOG_FILE="create_departments_$(date +%Y%m%d_%H%M%S).log"

echo "🚀 Creating departments under new branch structure..." | tee -a $LOG_FILE
echo "📅 Started at: $(date)" | tee -a $LOG_FILE

# Function to call API with error handling
call_api() {
    local method=$1
    local endpoint=$2
    local data=$3
    local description=$4

    echo "🔄 $description..." | tee -a $LOG_FILE

    if [ "$method" = "POST" ]; then
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

# Get all the branch IDs
echo "🔍 Retrieving branch information..." | tee -a $LOG_FILE
units_json=$(curl -s "$API_BASE/Units")

# 1. Hội sở (CNL2) - ID 47
hoi_so_id=47
echo "🏢 Creating departments under Hội sở (ID: $hoi_so_id)..." | tee -a $LOG_FILE
departments_hoi_so=(
    "PNVL1_KinhDoanh:Phòng Kinh doanh"
    "PNVL1_TinDung:Phòng Tín dụng"
    "PNVL1_KeToan:Phòng Kế toán"
    "PNVL1_NhanSu:Phòng Nhân sự"
    "PNVL1_CongNghe:Phòng Công nghệ thông tin"
    "PNVL1_KiemSoat:Phòng Kiểm soát rủi ro"
    "PNVL1_HanhChinh:Phòng Hành chính"
)

for dept in "${departments_hoi_so[@]}"; do
    code=$(echo "$dept" | cut -d: -f1)
    name=$(echo "$dept" | cut -d: -f2)
    dept_data="{\"Code\":\"$code\",\"Name\":\"$name\",\"Type\":\"PNVL1\",\"ParentUnitId\":$hoi_so_id}"
    call_api "POST" "/Units" "$dept_data" "Create $name under Hội sở" || true
    sleep 0.3
done

# 2. CN Bình Lư (CNL2) - ID 48
binh_lu_id=48
echo "🏢 Creating departments under Chi nhánh Bình Lư (ID: $binh_lu_id)..." | tee -a $LOG_FILE
departments_binh_lu=(
    "PNVL2_BinhLu_KinhDoanh:Phòng Kinh doanh Bình Lư"
    "PNVL2_BinhLu_TinDung:Phòng Tín dụng Bình Lư"
    "PNVL2_BinhLu_HanhChinh:Phòng Hành chính Bình Lư"
)

for dept in "${departments_binh_lu[@]}"; do
    code=$(echo "$dept" | cut -d: -f1)
    name=$(echo "$dept" | cut -d: -f2)
    dept_data="{\"Code\":\"$code\",\"Name\":\"$name\",\"Type\":\"PNVL2\",\"ParentUnitId\":$binh_lu_id}"
    call_api "POST" "/Units" "$dept_data" "Create $name" || true
    sleep 0.3
done

# 3. CN Phong Thổ (CNL2) - ID 49
phong_tho_id=49
echo "🏢 Creating departments under Chi nhánh Phong Thổ (ID: $phong_tho_id)..." | tee -a $LOG_FILE
departments_phong_tho=(
    "PNVL2_PhongTho_KinhDoanh:Phòng Kinh doanh Phong Thổ"
    "PNVL2_PhongTho_TinDung:Phòng Tín dụng Phong Thổ"
    "PNVL2_PhongTho_KeToan:Phòng Kế toán Phong Thổ"
    "PNVL2_PhongTho_HanhChinh:Phòng Hành chính Phong Thổ"
)

for dept in "${departments_phong_tho[@]}"; do
    code=$(echo "$dept" | cut -d: -f1)
    name=$(echo "$dept" | cut -d: -f2)
    dept_data="{\"Code\":\"$code\",\"Name\":\"$name\",\"Type\":\"PNVL2\",\"ParentUnitId\":$phong_tho_id}"
    call_api "POST" "/Units" "$dept_data" "Create $name" || true
    sleep 0.3
done

# 4-9. Rest of the branches (ID 50-55)
additional_branches=(
    "50:SinHo:Chi nhánh Sìn Hồ"
    "51:BumTo:Chi nhánh Bum Tở"
    "52:ThanUyen:Chi nhánh Than Uyên"
    "53:DoanKet:Chi nhánh Đoàn Kết"
    "54:TanUyen:Chi nhánh Tân Uyên"
    "55:NamHang:Chi nhánh Nậm Hàng"
)

for branch in "${additional_branches[@]}"; do
    branch_id=$(echo "$branch" | cut -d: -f1)
    code=$(echo "$branch" | cut -d: -f2)
    name=$(echo "$branch" | cut -d: -f3)

    echo "🏢 Creating departments under $name (ID: $branch_id)..." | tee -a $LOG_FILE

    # Create 2 basic departments for each branch
    dept_kinh_doanh_data="{\"Code\":\"PNVL2_${code}_KinhDoanh\",\"Name\":\"Phòng Kinh doanh ${name#Chi nhánh }\",\"Type\":\"PNVL2\",\"ParentUnitId\":$branch_id}"
    dept_hanh_chinh_data="{\"Code\":\"PNVL2_${code}_HanhChinh\",\"Name\":\"Phòng Hành chính ${name#Chi nhánh }\",\"Type\":\"PNVL2\",\"ParentUnitId\":$branch_id}"

    call_api "POST" "/Units" "$dept_kinh_doanh_data" "Create Phòng Kinh doanh ${name#Chi nhánh }" || true
    sleep 0.3
    call_api "POST" "/Units" "$dept_hanh_chinh_data" "Create Phòng Hành chính ${name#Chi nhánh }" || true
    sleep 0.3
done

# Final verification
echo "" | tee -a $LOG_FILE
echo "🔍 Final verification of organizational structure..." | tee -a $LOG_FILE

final_units_count=$(curl -s "$API_BASE/Units" | jq 'length')
echo "📊 Total units in organizational structure: $final_units_count" | tee -a $LOG_FILE

# Show the complete tree structure
echo "🌳 Complete organizational tree structure:" | tee -a $LOG_FILE
curl -s "$API_BASE/Units" | jq -r '.[] | "\(.Type) - \(.Name) (ID: \(.Id), Parent: \(.ParentUnitId // "None"))"' | sort | tee -a $LOG_FILE

echo "" | tee -a $LOG_FILE
echo "🎉 Department creation completed!" | tee -a $LOG_FILE
echo "📅 Finished at: $(date)" | tee -a $LOG_FILE
echo "📝 Full log available in: $LOG_FILE" | tee -a $LOG_FILE
