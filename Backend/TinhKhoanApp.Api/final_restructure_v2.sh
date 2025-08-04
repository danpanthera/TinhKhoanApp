#!/bin/bash

# Script để xóa và tạo lại các đơn vị theo danh sách Ver2 - Phiên bản final v2
# Final script to delete and recreate units according to Ver2 list - Version 2

API_BASE="http://localhost:5055/api"
LOG_FILE="final_restructure_v2_$(date +%Y%m%d_%H%M%S).log"
TIMESTAMP=$(date +%Y%m%d%H%M%S)
ROOT_CODE="CNLC_${TIMESTAMP}"

echo "🚀 Starting Final Organization Restructuring (Ver2)..." | tee -a $LOG_FILE
echo "📅 Started at: $(date)" | tee -a $LOG_FILE

# Step 1: Just create new units without deleting old ones
echo "🏢 Creating Root - Chi nhánh Lai Châu (LV1) with unique code $ROOT_CODE..." | tee -a $LOG_FILE
root_response=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{
  \"Code\": \"$ROOT_CODE\",
  \"Name\": \"Chi nhánh Lai Châu\",
  \"Type\": \"CNL1\",
  \"ParentUnitId\": null
}")

echo "$root_response" | tee -a $LOG_FILE
echo "" | tee -a $LOG_FILE

# Extract the root ID directly from the response JSON
root_id=$(echo "$root_response" | sed 's/.*"Id":\([0-9]*\).*/\1/')
echo "🏷️ Root ID extracted from response: $root_id" | tee -a $LOG_FILE

if [ -z "$root_id" ] || [ "$root_id" == "$root_response" ]; then
    echo "❌ Failed to extract root ID, trying with different pattern..." | tee -a $LOG_FILE
    root_id=$(echo "$root_response" | grep -o '"Id":[0-9]*' | head -1 | sed 's/.*://')
    echo "🏷️ Root ID with alternate pattern: $root_id" | tee -a $LOG_FILE

    if [ -z "$root_id" ] || [ "$root_id" == "$root_response" ]; then
        echo "❌ Final attempt to get root ID..." | tee -a $LOG_FILE
        echo "Full response was:" | tee -a $LOG_FILE
        echo "$root_response" | tee -a $LOG_FILE

        # Let's try manually parsing digit after "Id":
        root_id=$(echo "$root_response" | grep -o '"Id": *[0-9]*' | head -1 | grep -o '[0-9]*')
        echo "🏷️ Root ID (final attempt): $root_id" | tee -a $LOG_FILE

        if [ -z "$root_id" ]; then
            echo "❌ Cannot extract root ID. Exiting." | tee -a $LOG_FILE
            exit 1
        fi
    fi
fi

echo "✅ Successfully got root ID: $root_id" | tee -a $LOG_FILE

# Step 2: Create all branches under root
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
    unique_code="${code}_${TIMESTAMP}"

    echo "  Creating $name with code $unique_code..." | tee -a $LOG_FILE
    branch_response=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{
        \"Code\": \"$unique_code\",
        \"Name\": \"$name\",
        \"Type\": \"CNL2\",
        \"ParentUnitId\": $root_id
    }")

    branch_id=$(echo "$branch_response" | sed 's/.*"Id":\([0-9]*\).*/\1/')
    if [ -z "$branch_id" ] || [ "$branch_id" == "$branch_response" ]; then
        branch_id=$(echo "$branch_response" | grep -o '"Id":[0-9]*' | head -1 | sed 's/.*://')
    fi

    if [ -z "$branch_id" ] || [ "$branch_id" == "$branch_response" ]; then
        echo "❌ Failed to extract branch ID for $name. Response: $branch_response" | tee -a $LOG_FILE
        continue
    fi

    echo "✅ Created $name with ID: $branch_id" | tee -a $LOG_FILE
    branch_ids+=("$code:$branch_id")
done

echo "⏳ Waiting for database to update..." | tee -a $LOG_FILE
sleep 2

# Step 3: Create departments for each branch
echo "🏢 Creating departments..." | tee -a $LOG_FILE

# Function to create departments
create_departments() {
    local branch_id=$1
    local branch_code=$2
    local departments=("${@:3}")
    local dept_count=0

    echo "  Creating departments for branch $branch_code (ID: $branch_id)..." | tee -a $LOG_FILE

    for dept in "${departments[@]}"; do
        local code=$(echo "$dept" | cut -d: -f1)
        local name=$(echo "$dept" | cut -d: -f2)
        local type=$(echo "$dept" | cut -d: -f3)
        local unique_code="${branch_code}_${code}_${TIMESTAMP}"

        dept_response=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "{
            \"Code\": \"$unique_code\",
            \"Name\": \"$name\",
            \"Type\": \"$type\",
            \"ParentUnitId\": $branch_id
        }")

        dept_id=$(echo "$dept_response" | sed 's/.*"Id":\([0-9]*\).*/\1/')
        if [ -z "$dept_id" ] || [ "$dept_id" == "$dept_response" ]; then
            dept_id=$(echo "$dept_response" | grep -o '"Id":[0-9]*' | head -1 | sed 's/.*://')
        fi

        if [ -z "$dept_id" ] || [ "$dept_id" == "$dept_response" ]; then
            echo "    ❌ Failed to create department $name" | tee -a $LOG_FILE
        else
            echo "    ✅ Created department $name (ID: $dept_id)" | tee -a $LOG_FILE
            dept_count=$((dept_count+1))
        fi

        sleep 0.2
    done

    echo "  ✅ Created $dept_count departments for branch $branch_code" | tee -a $LOG_FILE
}

# Find the branch IDs
hoi_so_id=""
binh_lu_id=""
phong_tho_id=""
sin_ho_id=""
bum_to_id=""
than_uyen_id=""
doan_ket_id=""
tan_uyen_id=""
nam_hang_id=""

for branch_info in "${branch_ids[@]}"; do
    code=$(echo "$branch_info" | cut -d: -f1)
    id=$(echo "$branch_info" | cut -d: -f2)

    case "$code" in
        "HS") hoi_so_id=$id ;;
        "BL") binh_lu_id=$id ;;
        "PT") phong_tho_id=$id ;;
        "SH") sin_ho_id=$id ;;
        "BT") bum_to_id=$id ;;
        "TU") than_uyen_id=$id ;;
        "DK") doan_ket_id=$id ;;
        "TUY") tan_uyen_id=$id ;;
        "NH") nam_hang_id=$id ;;
    esac
done

# Hội sở departments
if [ -n "$hoi_so_id" ]; then
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
fi

# CN Bình Lư departments
if [ -n "$binh_lu_id" ]; then
    binh_lu_depts=(
        "BGD:Ban Giám đốc:PNVL2"
        "KTNQ:P. KTNQ:PNVL2"
        "KH:P. KH:PNVL2"
    )
    create_departments "$binh_lu_id" "BL" "${binh_lu_depts[@]}"
fi

# CN Phong Thổ departments
if [ -n "$phong_tho_id" ]; then
    phong_tho_depts=(
        "BGD:Ban Giám đốc:PNVL2"
        "KTNQ:P. KTNQ:PNVL2"
        "KH:P. KH:PNVL2"
        "S5:PGD Số 5:PGDL2"
    )
    create_departments "$phong_tho_id" "PT" "${phong_tho_depts[@]}"
fi

# CN Sìn Hồ departments
if [ -n "$sin_ho_id" ]; then
    sin_ho_depts=(
        "BGD:Ban Giám đốc:PNVL2"
        "KTNQ:P. KTNQ:PNVL2"
        "KH:P. KH:PNVL2"
    )
    create_departments "$sin_ho_id" "SH" "${sin_ho_depts[@]}"
fi

# CN Bum Tở departments
if [ -n "$bum_to_id" ]; then
    bum_to_depts=(
        "BGD:Ban Giám đốc:PNVL2"
        "KTNQ:P. KTNQ:PNVL2"
        "KH:P. KH:PNVL2"
    )
    create_departments "$bum_to_id" "BT" "${bum_to_depts[@]}"
fi

# CN Than Uyên departments
if [ -n "$than_uyen_id" ]; then
    than_uyen_depts=(
        "BGD:Ban Giám đốc:PNVL2"
        "KTNQ:P. KTNQ:PNVL2"
        "KH:P. KH:PNVL2"
        "S6:PGD Số 6:PGDL2"
    )
    create_departments "$than_uyen_id" "TU" "${than_uyen_depts[@]}"
fi

# CN Đoàn Kết departments
if [ -n "$doan_ket_id" ]; then
    doan_ket_depts=(
        "BGD:Ban Giám đốc:PNVL2"
        "KTNQ:P. KTNQ:PNVL2"
        "KH:P. KH:PNVL2"
        "S1:PGD Số 1:PGDL2"
        "S2:PGD Số 2:PGDL2"
    )
    create_departments "$doan_ket_id" "DK" "${doan_ket_depts[@]}"
fi

# CN Tân Uyên departments
if [ -n "$tan_uyen_id" ]; then
    tan_uyen_depts=(
        "BGD:Ban Giám đốc:PNVL2"
        "KTNQ:P. KTNQ:PNVL2"
        "KH:P. KH:PNVL2"
        "S3:PGD Số 3:PGDL2"
    )
    create_departments "$tan_uyen_id" "TUY" "${tan_uyen_depts[@]}"
fi

# CN Nậm Hàng departments
if [ -n "$nam_hang_id" ]; then
    nam_hang_depts=(
        "BGD:Ban Giám đốc:PNVL2"
        "KTNQ:P. KTNQ:PNVL2"
        "KH:P. KH:PNVL2"
    )
    create_departments "$nam_hang_id" "NH" "${nam_hang_depts[@]}"
fi

# Step 4: Final verification
echo "⏳ Waiting for database to update..." | tee -a $LOG_FILE
sleep 2
echo "🔍 Final verification..." | tee -a $LOG_FILE

# Count our created units
our_units_count=$(( 1 + ${#branch_ids[@]} + 7 + 3*8 + 5 ))
echo "📊 Units we should have created: $our_units_count" | tee -a $LOG_FILE

# Get all units and count by type
all_units=$(curl -s "$API_BASE/Units")
our_root=$(echo "$all_units" | grep -o "\"Code\":\"$ROOT_CODE\"" | wc -l)

echo "🎉 Organization restructuring completed!" | tee -a $LOG_FILE
echo "📅 Finished at: $(date)" | tee -a $LOG_FILE
echo "📝 Full log available in: $LOG_FILE" | tee -a $LOG_FILE
echo "" | tee -a $LOG_FILE
echo "NOTE: The old units still exist, but a new structure has been created with root: Chi nhánh Lai Châu (Code: $ROOT_CODE)" | tee -a $LOG_FILE
