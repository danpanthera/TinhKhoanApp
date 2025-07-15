#!/bin/bash

echo "📊 PHÂN TÍCH CHI TIẾT CSV HEADERS"
echo "================================="
echo "$(date '+%Y-%m-%d %H:%M:%S') - Analyzing CSV headers từ DuLieuMau"
echo ""

# Đường dẫn đến thư mục chứa CSV gốc
csv_dir="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau"

# Function để phân tích header của file CSV
analyze_csv_header() {
    local file_path=$1
    local table_name=$2
    local expected_cols=$3

    if [ -f "$file_path" ]; then
        echo "📄 $table_name Table - File: $(basename "$file_path")"
        echo "Expected columns: $expected_cols"

        # Lấy header line và xử lý BOM
        header_line=$(head -1 "$file_path" | sed 's/^\xEF\xBB\xBF//')

        # Đếm số cột
        actual_cols=$(echo "$header_line" | tr ',' '\n' | wc -l | tr -d ' ')
        echo "Actual columns: $actual_cols"

        # So sánh
        if [ "$actual_cols" -eq "$expected_cols" ]; then
            echo "✅ Column count MATCHES!"
        else
            echo "❌ Column count MISMATCH! ($actual_cols vs $expected_cols)"
        fi

        echo ""
        echo "🔍 Column Headers:"
        echo "$header_line" | tr ',' '\n' | nl -v0 | while IFS=$'\t' read num col; do
            printf "%2d: %s\n" "$num" "$col"
        done

        echo ""
        echo "📝 SQL CREATE TABLE suggestion:"
        echo "DROP TABLE IF EXISTS ${table_name}_NEW;"
        echo "CREATE TABLE ${table_name}_NEW ("
        echo "    Id INT IDENTITY(1,1) PRIMARY KEY,"

        # Generate column definitions
        echo "$header_line" | tr ',' '\n' | while read col; do
            clean_col=$(echo "$col" | sed 's/[^a-zA-Z0-9_]//g')
            if [ ! -z "$clean_col" ]; then
                echo "    [$clean_col] NVARCHAR(500),"
            fi
        done

        echo "    NGAY_DL DATE DEFAULT GETDATE(),"
        echo "    CREATED_DATE DATETIME2 DEFAULT GETDATE(),"
        echo "    UPDATED_DATE DATETIME2 DEFAULT GETDATE(),"
        echo "    FILE_NAME NVARCHAR(255),"
        echo "    ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START,"
        echo "    ValidTo DATETIME2 GENERATED ALWAYS AS ROW END,"
        echo "    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)"
        echo ") WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.${table_name}_History));"
        echo ""
        echo "-- Create Columnstore Index"
        echo "CREATE NONCLUSTERED COLUMNSTORE INDEX IX_${table_name}_Columnstore"
        echo "ON ${table_name}_NEW ([NGAY_DL], [CREATED_DATE]);"
        echo ""
        echo "=" x 60
    else
        echo "❌ File not found: $file_path"
    fi
    echo ""
}

# Phân tích các file chính
echo "🎯 ANALYZING MAIN CSV FILES FROM DuLieuMau:"
echo "==========================================="

analyze_csv_header "$csv_dir/7808_dp01_20241231.csv" "DP01" 63
analyze_csv_header "$csv_dir/7808_dpda_20250331.csv" "DPDA" 13
analyze_csv_header "$csv_dir/7808_ei01_20241231.csv" "EI01" 24
analyze_csv_header "$csv_dir/7808_gl01_2025030120250331.csv" "GL01" 27
analyze_csv_header "$csv_dir/7808_gl41_20250630.csv" "GL41" 13
analyze_csv_header "$csv_dir/7808_ln01_20241231.csv" "LN01" 79
analyze_csv_header "$csv_dir/7808_ln03_20241231.csv" "LN03" 17
analyze_csv_header "$csv_dir/7800_rr01_20250531.csv" "RR01" 25

echo "✅ Analysis completed $(date '+%H:%M:%S')"
