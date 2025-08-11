#!/bin/bash
# SCRIPT: verify_9_tables_consistency.sh
# MỤC ĐÍCH: Kiểm tra sự thống nhất giữa CSV ↔ Models ↔ Database cho 9 bảng

echo "🔍 KIỂM TRA THỐNG NHẤT 9 BẢNG DỮ LIỆU"
echo "======================================"

# Đường dẫn Backend
BACKEND_DIR="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api"
cd "$BACKEND_DIR" || exit 1

# Danh sách 9 bảng cần kiểm tra
declare -a TABLES=("DP01" "DPDA" "EI01" "GL01" "GL02" "GL41" "LN01" "LN03" "RR01")

# Function kiểm tra CSV columns
check_csv_columns() {
    local table=$1
    local csv_file=""

    # Tìm file CSV tương ứng
    case $table in
        "DP01") csv_file="7800_dp01_20241231.csv" ;;
        "DPDA") csv_file="7800_dpda_20250331.csv" ;;
        "EI01") csv_file="7800_ei01_20241231.csv" ;;
        "GL01") csv_file="7800_gl01_2024120120241231.csv" ;;
        "GL02") csv_file="7800_gl02_2024120120241231.csv" ;;
        "GL41") csv_file="7800_gl41_20241231.csv" ;;
        "LN01") csv_file="7800_ln01_20241231.csv" ;;
        "LN03") csv_file="7800_ln03_20241231.csv" ;;
        "RR01") csv_file="7800_rr01_20241231.csv" ;;
    esac

    if [ -f "$csv_file" ]; then
        echo "📄 CSV File: $csv_file"
        local column_count=$(head -1 "$csv_file" | tr ',' '\n' | wc -l | tr -d ' ')
        echo "📊 Business Columns: $column_count"
        echo "🏷️  CSV Headers:"
        head -1 "$csv_file" | tr ',' '\n' | nl
        echo ""
    else
        echo "❌ CSV File không tìm thấy: $csv_file"
        echo ""
        return 1
    fi
}

# Function kiểm tra Model structure
check_model_structure() {
    local table=$1
    local model_file="Models/DataTables/${table}.cs"

    if [ -f "$model_file" ]; then
        echo "📋 Model File: $model_file"

        # Đếm business columns (trừ system columns)
        local business_cols=$(grep -E "^\s*\[Column\(" "$model_file" | grep -v "NGAY_DL\|CreatedAt\|UpdatedAt\|DataSource\|ImportDateTime\|CreatedBy\|UpdatedBy\|Id" | wc -l | tr -d ' ')
        echo "📊 Business Columns trong Model: $business_cols"

        # Hiển thị business column names
        echo "🏷️  Model Business Columns:"
        grep -E "^\s*\[Column\(" "$model_file" | grep -v "NGAY_DL\|CreatedAt\|UpdatedAt\|DataSource\|ImportDateTime\|CreatedBy\|UpdatedBy\|Id" | sed -E 's/.*\[Column\("([^"]+)"\).*/\1/' | nl
        echo ""
    else
        echo "❌ Model File không tìm thấy: $model_file"
        echo ""
        return 1
    fi
}

# Function kiểm tra Database table structure
check_database_structure() {
    local table=$1
    echo "🗄️  Database Table: $table"

    # Sử dụng sqlcmd để kiểm tra table structure
    sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "
    SELECT
        COLUMN_NAME,
        DATA_TYPE,
        CHARACTER_MAXIMUM_LENGTH,
        IS_NULLABLE
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = '$table'
    AND COLUMN_NAME NOT IN ('Id', 'NGAY_DL', 'CreatedAt', 'UpdatedAt', 'DataSource', 'ImportDateTime', 'CreatedBy', 'UpdatedBy', 'ValidFrom', 'ValidTo')
    ORDER BY ORDINAL_POSITION;" 2>/dev/null
    echo ""
}

# Function so sánh consistency
check_consistency() {
    local table=$1
    echo "🔍 CONSISTENCY CHECK FOR $table"
    echo "================================"

    check_csv_columns "$table"
    check_model_structure "$table"
    check_database_structure "$table"

    echo "⚖️  VERIFICATION SUMMARY:"
    echo "• CSV headers should match Model business columns"
    echo "• Model business columns should match Database columns"
    echo "• All should preserve original CSV column names"
    echo ""
    echo "-------------------------------------------"
    echo ""
}

# Main execution
echo "📅 Date: $(date)"
echo "📍 Location: $BACKEND_DIR"
echo ""

# Kiểm tra từng bảng
for table in "${TABLES[@]}"; do
    check_consistency "$table"
done

echo "✅ COMPLETED: 9 Tables Consistency Check"
echo "📋 Next Steps:"
echo "   1. Review column mismatches"
echo "   2. Fix Model definitions if needed"
echo "   3. Update Database migrations if needed"
echo "   4. Ensure Services/Repository use correct column names"
