#!/bin/bash

# Script kiểm tra toàn diện: Models vs Database vs CSV Headers
# Đảm bảo synchronization hoàn hảo cho Direct Import

echo "🔍 KIỂM TRA TOÀN DIỆN: MODELS vs DATABASE vs CSV HEADERS"
echo "============================================================"

CSV_DIR="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau"

# Màu sắc
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Danh sách bảng và file CSV tương ứng
declare -A CSV_FILES=(
    ["DP01"]="7808_dp01_20241231.csv"
    ["EI01"]="7808_ei01_20241231.csv"
    ["GL01"]="7808_gl01_2025030120250331.csv"
    ["GL41"]="7808_gl41_20250630.csv"
    ["LN01"]="7808_ln01_20241231.csv"
    ["LN03"]="7800_ln03_20241231_fixed.csv"
    ["RR01"]="7800_rr01_20250531.csv"
    ["DPDA"]="7808_dpda_20250331.csv"
)

# Function: Kiểm tra CSV headers
check_csv_headers() {
    local table=$1
    local csv_file=$2

    echo -e "${BLUE}📋 BẢNG: $table${NC}"
    echo "   📁 CSV File: $csv_file"

    if [ ! -f "$CSV_DIR/$csv_file" ]; then
        echo -e "   ${RED}❌ CSV file not found: $CSV_DIR/$csv_file${NC}"
        return 1
    fi

    # Đếm số cột trong CSV
    csv_columns=$(head -1 "$CSV_DIR/$csv_file" | tr ',' '\n' | wc -l | tr -d ' ')
    echo "   📊 CSV Columns: $csv_columns"

    return 0
}

# Function: Kiểm tra database structure
check_database_structure() {
    local table=$1

    echo "   🗄️  Database Structure:"

    # Kiểm tra bảng có tồn tại không
    table_exists=$(sqlcmd -S localhost,1433 -U sa -P "YourStrong@Password123" -d TinhKhoanDB -C -h -1 -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '$table'" 2>/dev/null | tr -d ' ')

    if [ "$table_exists" != "1" ]; then
        echo -e "      ${RED}❌ Table $table does not exist in database${NC}"
        return 1
    fi

    # Đếm tổng số cột
    total_columns=$(sqlcmd -S localhost,1433 -U sa -P "YourStrong@Password123" -d TinhKhoanDB -C -h -1 -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$table'" 2>/dev/null | tr -d ' ')
    echo "      📊 Total Columns: $total_columns"

    # Đếm business columns (những cột không phải system/temporal)
    business_columns=$(sqlcmd -S localhost,1433 -U sa -P "YourStrong@Password123" -d TinhKhoanDB -C -h -1 -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$table' AND COLUMN_NAME NOT IN ('Id', 'NGAY_DL', 'CREATED_DATE', 'UPDATED_DATE', 'FILE_NAME')" 2>/dev/null | tr -d ' ')
    echo "      📈 Business Columns: $business_columns"

    # Kiểm tra temporal table
    is_temporal=$(sqlcmd -S localhost,1433 -U sa -P "YourStrong@Password123" -d TinhKhoanDB -C -h -1 -Q "SELECT COUNT(*) FROM sys.tables WHERE name = '$table' AND temporal_type = 2" 2>/dev/null | tr -d ' ')
    if [ "$is_temporal" = "1" ]; then
        echo -e "      ${GREEN}✅ Temporal Table: YES${NC}"
    else
        echo -e "      ${YELLOW}⚠️  Temporal Table: NO${NC}"
    fi

    # Kiểm tra columnstore index
    has_columnstore=$(sqlcmd -S localhost,1433 -U sa -P "YourStrong@Password123" -d TinhKhoanDB -C -h -1 -Q "SELECT COUNT(*) FROM sys.indexes WHERE object_id = OBJECT_ID('$table') AND type = 5" 2>/dev/null | tr -d ' ')
    if [ "$has_columnstore" -gt "0" ]; then
        echo -e "      ${GREEN}✅ Columnstore Index: YES${NC}"
    else
        echo -e "      ${YELLOW}⚠️  Columnstore Index: NO${NC}"
    fi

    return 0
}

# Function: Kiểm tra model file
check_model_file() {
    local table=$1

    echo "   🏗️  Model File:"

    model_file="Models/DataTables/$table.cs"
    if [ ! -f "$model_file" ]; then
        echo -e "      ${RED}❌ Model file not found: $model_file${NC}"
        return 1
    fi

    echo -e "      ${GREEN}✅ Model file exists: $model_file${NC}"

    # Kiểm tra số lượng properties trong model (excluding navigation/computed properties)
    model_properties=$(grep -c "\[Column(" "$model_file" 2>/dev/null || echo "0")
    echo "      📊 Model Properties: $model_properties"

    return 0
}

# Function: So sánh consistency
check_consistency() {
    local table=$1
    local csv_file=$2

    echo "   🔍 Consistency Check:"

    # Lấy số cột từ CSV
    if [ -f "$CSV_DIR/$csv_file" ]; then
        csv_columns=$(head -1 "$CSV_DIR/$csv_file" | tr ',' '\n' | wc -l | tr -d ' ')
    else
        csv_columns=0
    fi

    # Lấy số business columns từ database
    business_columns=$(sqlcmd -S localhost,1433 -U sa -P "YourStrong@Password123" -d TinhKhoanDB -C -h -1 -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$table' AND COLUMN_NAME NOT IN ('Id', 'NGAY_DL', 'CREATED_DATE', 'UPDATED_DATE', 'FILE_NAME')" 2>/dev/null | tr -d ' ')

    # So sánh
    if [ "$csv_columns" = "$business_columns" ]; then
        echo -e "      ${GREEN}✅ CSV ($csv_columns) = DB Business Columns ($business_columns)${NC}"
    else
        echo -e "      ${RED}❌ MISMATCH: CSV ($csv_columns) ≠ DB Business Columns ($business_columns)${NC}"
    fi

    return 0
}

# Main execution
echo ""
for table in "${!CSV_FILES[@]}"; do
    csv_file="${CSV_FILES[$table]}"

    check_csv_headers "$table" "$csv_file"
    check_database_structure "$table"
    check_model_file "$table"
    check_consistency "$table" "$csv_file"

    echo "   --------------------------------------------------------"
    echo ""
done

echo "🎯 KIỂM TRA MIGRATION ISSUES:"
echo "============================================"

# Kiểm tra migration files
echo "📂 Latest Migration Files:"
ls -la Migrations/*.cs | tail -5

echo ""
echo "🔄 Kiểm tra migration status:"
dotnet ef migrations list --no-build 2>/dev/null | tail -5

echo ""
echo "✅ KIỂM TRA HOÀN TẤT!"
echo "Xem kết quả phía trên để xác định các vấn đề cần khắc phục."
