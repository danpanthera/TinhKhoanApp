#!/bin/bash

# Enhanced script để hoàn thành việc xóa các đơn vị còn lại và tạo mới cấu trúc tổ chức
# Complete cleanup and restructure organizational units

set -e

API_BASE="http://localhost:5055/api"
LOG_FILE="complete_restructure_$(date +%Y%m%d_%H%M%S).log"

echo "🚀 Completing Organizational Units Restructuring..." | tee -a $LOG_FILE
echo "📅 Started at: $(date)" | tee -a $LOG_FILE

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

# Step 1: Delete remaining problematic units in correct order
echo "🗑️ Cleaning up remaining units..." | tee -a $LOG_FILE

# Delete PGDL2 units first
pgd_units=(17 30 35 36 41)
for unit_id in "${pgd_units[@]}"; do
    call_api "DELETE" "/Units/$unit_id" "" "Delete PGDL2 unit ID $unit_id" || true
    sleep 0.5
done

# Delete PNVL2 units (Ban Giám đốc)
pnvl2_units=(10 14 19 23 27 32 38 43)
for unit_id in "${pnvl2_units[@]}"; do
    call_api "DELETE" "/Units/$unit_id" "" "Delete PNVL2 unit ID $unit_id" || true
    sleep 0.5
done

# Delete PNVL1 unit
call_api "DELETE" "/Units/2" "" "Delete PNVL1 unit ID 2" || true

# Delete CNL1 unit (Hội Sở)
call_api "DELETE" "/Units/1" "" "Delete CNL1 unit ID 1" || true

echo "✅ Cleanup completed" | tee -a $LOG_FILE

# Step 2: Get the Chi nhánh Lai Châu ID (should be 46)
lai_chau_id=46
echo "🔍 Using Chi nhánh Lai Châu ID: $lai_chau_id" | tee -a $LOG_FILE

# Step 3: Create new organizational structure
echo "🏗️ Creating complete organizational structure..." | tee -a $LOG_FILE

# 2. Hội sở (CNL2) + 7 Phòng NVL1
echo "🏢 Creating Hội sở (CNL2)..." | tee -a $LOG_FILE
hoi_so_data="{\"Code\":\"CNL2_HoiSo\",\"Name\":\"Hội sở\",\"Type\":\"CNL2\",\"ParentUnitId\":$lai_chau_id}"
hoi_so_response=$(call_api "POST" "/Units" "$hoi_so_data" "Create Hội sở")
hoi_so_id=$(echo "$hoi_so_response" | jq -r '.Id // .id // empty' 2>/dev/null || echo "")

if [ -z "$hoi_so_id" ] || [ "$hoi_so_id" = "null" ]; then
    echo "❌ Failed to create Hội sở" | tee -a $LOG_FILE
else
    echo "✅ Hội sở created with ID: $hoi_so_id" | tee -a $LOG_FILE
    
    # Create 7 Phòng NVL1 under Hội sở
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
        call_api "POST" "/Units" "$dept_data" "Create $name" || true
        sleep 0.3
    done
fi

# 3. CN Bình Lư (CNL2) + 3 Phòng NVL2
echo "🏢 Creating CN Bình Lư (CNL2)..." | tee -a $LOG_FILE
binh_lu_data="{\"Code\":\"CNL2_BinhLu\",\"Name\":\"Chi nhánh Bình Lư\",\"Type\":\"CNL2\",\"ParentUnitId\":$lai_chau_id}"
binh_lu_response=$(call_api "POST" "/Units" "$binh_lu_data" "Create CN Bình Lư")
binh_lu_id=$(echo "$binh_lu_response" | jq -r '.Id // .id // empty' 2>/dev/null || echo "")

if [ -z "$binh_lu_id" ] || [ "$binh_lu_id" = "null" ]; then
    echo "❌ Failed to create CN Bình Lư" | tee -a $LOG_FILE
else
    echo "✅ CN Bình Lư created with ID: $binh_lu_id" | tee -a $LOG_FILE
    
    # Create 3 Phòng NVL2 under CN Bình Lư
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
fi

# 4. CN Phong Thổ (CNL2) + 4 Phòng NVL2
echo "🏢 Creating CN Phong Thổ (CNL2)..." | tee -a $LOG_FILE
phong_tho_data="{\"Code\":\"CNL2_PhongTho\",\"Name\":\"Chi nhánh Phong Thổ\",\"Type\":\"CNL2\",\"ParentUnitId\":$lai_chau_id}"
phong_tho_response=$(call_api "POST" "/Units" "$phong_tho_data" "Create CN Phong Thổ")
phong_tho_id=$(echo "$phong_tho_response" | jq -r '.Id // .id // empty' 2>/dev/null || echo "")

if [ -z "$phong_tho_id" ] || [ "$phong_tho_id" = "null" ]; then
    echo "❌ Failed to create CN Phong Thổ" | tee -a $LOG_FILE
else
    echo "✅ CN Phong Thổ created with ID: $phong_tho_id" | tee -a $LOG_FILE
    
    # Create 4 Phòng NVL2 under CN Phong Thổ
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
fi

# 5-10. Additional 6 CNL2 branches
additional_branches=(
    "CNL2_SinHo:Chi nhánh Sìn Hồ"
    "CNL2_BumTo:Chi nhánh Bum Tở" 
    "CNL2_ThanUyen:Chi nhánh Than Uyên"
    "CNL2_DoanKet:Chi nhánh Đoàn Kết"
    "CNL2_TanUyen:Chi nhánh Tân Uyên"
    "CNL2_NamHang:Chi nhánh Nậm Hàng"
)

for branch in "${additional_branches[@]}"; do
    code=$(echo "$branch" | cut -d: -f1)
    name=$(echo "$branch" | cut -d: -f2)
    
    echo "🏢 Creating $name..." | tee -a $LOG_FILE
    branch_data="{\"Code\":\"$code\",\"Name\":\"$name\",\"Type\":\"CNL2\",\"ParentUnitId\":$lai_chau_id}"
    branch_response=$(call_api "POST" "/Units" "$branch_data" "Create $name")
    branch_id=$(echo "$branch_response" | jq -r '.Id // .id // empty' 2>/dev/null || echo "")
    
    if [ -z "$branch_id" ] || [ "$branch_id" = "null" ]; then
        echo "❌ Failed to create $name" | tee -a $LOG_FILE
    else
        echo "✅ $name created with ID: $branch_id" | tee -a $LOG_FILE
        
        # Create 2 basic departments for each additional branch
        dept_kinh_doanh_data="{\"Code\":\"PNVL2_${code#CNL2_}_KinhDoanh\",\"Name\":\"Phòng Kinh doanh ${name#Chi nhánh }\",\"Type\":\"PNVL2\",\"ParentUnitId\":$branch_id}"
        dept_hanh_chinh_data="{\"Code\":\"PNVL2_${code#CNL2_}_HanhChinh\",\"Name\":\"Phòng Hành chính ${name#Chi nhánh }\",\"Type\":\"PNVL2\",\"ParentUnitId\":$branch_id}"
        
        call_api "POST" "/Units" "$dept_kinh_doanh_data" "Create Phòng Kinh doanh ${name#Chi nhánh }" || true
        sleep 0.3
        call_api "POST" "/Units" "$dept_hanh_chinh_data" "Create Phòng Hành chính ${name#Chi nhánh }" || true
        sleep 0.3
    fi
done

# Step 4: Final verification
echo "" | tee -a $LOG_FILE
echo "🔍 Final verification..." | tee -a $LOG_FILE

final_units_count=$(curl -s "$API_BASE/Units" | jq 'length')
echo "📊 Total units in new structure: $final_units_count" | tee -a $LOG_FILE

# Show complete tree structure organized by type
echo "🌳 Complete new organizational tree structure:" | tee -a $LOG_FILE
echo "📋 CNL1 (Root):" | tee -a $LOG_FILE
curl -s "$API_BASE/Units" | jq -r '.[] | select(.Type == "CNL1") | "  - \(.Name) (ID: \(.Id))"' | tee -a $LOG_FILE

echo "📋 CNL2 (Branches):" | tee -a $LOG_FILE
curl -s "$API_BASE/Units" | jq -r '.[] | select(.Type == "CNL2") | "  - \(.Name) (ID: \(.Id), Parent: \(.ParentUnitId))"' | tee -a $LOG_FILE

echo "📋 PNVL1 (Level 1 Departments):" | tee -a $LOG_FILE
curl -s "$API_BASE/Units" | jq -r '.[] | select(.Type == "PNVL1") | "  - \(.Name) (ID: \(.Id), Parent: \(.ParentUnitId))"' | tee -a $LOG_FILE

echo "📋 PNVL2 (Level 2 Departments):" | tee -a $LOG_FILE
curl -s "$API_BASE/Units" | jq -r '.[] | select(.Type == "PNVL2") | "  - \(.Name) (ID: \(.Id), Parent: \(.ParentUnitId))"' | tee -a $LOG_FILE

echo "" | tee -a $LOG_FILE
echo "🎉 Complete organizational restructuring finished!" | tee -a $LOG_FILE
echo "📅 Completed at: $(date)" | tee -a $LOG_FILE
echo "📝 Full log available in: $LOG_FILE" | tee -a $LOG_FILE
