#!/bin/bash

# 🔍 Comprehensive CSV Headers Analysis for 8 Core DataTables
# Phân tích headers và NGAY_DL của 8 file CSV gốc để đối chiếu với database structure

echo "🔍 ANALYZING CSV HEADERS FOR 8 CORE DATATABLES"
echo "=============================================="
echo ""

CSV_DIR="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau"

echo "📂 CSV Files Directory: $CSV_DIR"
echo ""

# Function to analyze CSV headers
analyze_csv_file() {
    local file=$1
    local table_name=$2

    if [ -f "$file" ]; then
        echo "📋 Table: $table_name"
        echo "📁 File: $(basename $file)"

        # Extract date from filename
        local filename=$(basename $file)
        local file_date=""
        if [[ $filename =~ [0-9]{8} ]]; then
            file_date=$(echo $filename | grep -oE '[0-9]{8}' | head -1)
            if [ ${#file_date} -eq 8 ]; then
                # Convert YYYYMMDD to DD/MM/YYYY
                local day=${file_date:6:2}
                local month=${file_date:4:2}
                local year=${file_date:0:4}
                file_date="$day/$month/$year"
            fi
        fi
        echo "📅 NGAY_DL from filename: $file_date"

        # Get headers
        echo "📊 CSV Headers:"
        head -1 "$file" | sed 's/\xEF\xBB\xBF//' | tr ',' '\n' | nl

        # Get column count
        local col_count=$(head -1 "$file" | sed 's/\xEF\xBB\xBF//' | tr ',' '\n' | wc -l)
        echo "📈 Total Columns: $col_count"

        # Special handling for GL01 - check TR_TIME column
        if [ "$table_name" = "GL01" ]; then
            echo "🕐 Special check for GL01 TR_TIME column:"
            head -1 "$file" | sed 's/\xEF\xBB\xBF//' | tr ',' '\n' | grep -n "TR_TIME" || echo "   TR_TIME column not found!"
        fi

        echo "----------------------------------------"
        echo ""
    else
        echo "❌ File not found: $file"
        echo ""
    fi
}

# Analyze each CSV file
echo "🔍 Step 1: Analyzing GL01 CSV..."
analyze_csv_file "$CSV_DIR/7808_gl01_2025030120250331.csv" "GL01"

echo "🔍 Step 2: Analyzing DP01 CSV..."
analyze_csv_file "$CSV_DIR/7808_dp01_20241231.csv" "DP01"

echo "🔍 Step 3: Analyzing DPDA CSV..."
analyze_csv_file "$CSV_DIR/7808_dpda_20250331.csv" "DPDA"

echo "🔍 Step 4: Analyzing EI01 CSV..."
analyze_csv_file "$CSV_DIR/7808_ei01_20241231.csv" "EI01"

echo "🔍 Step 5: Analyzing GL41 CSV..."
analyze_csv_file "$CSV_DIR/7808_gl41_20250630.csv" "GL41"

echo "🔍 Step 6: Analyzing LN01 CSV..."
analyze_csv_file "$CSV_DIR/7808_ln01_20241231.csv" "LN01"

echo "🔍 Step 7: Analyzing LN03 CSV..."
analyze_csv_file "$CSV_DIR/7800_ln03_20241231_fixed.csv" "LN03"

echo "🔍 Step 8: Analyzing RR01 CSV..."
analyze_csv_file "$CSV_DIR/7800_rr01_20250531.csv" "RR01"

echo "📊 NGAY_DL RULES SUMMARY:"
echo "========================"
echo "✅ GL01: NGAY_DL = TR_TIME column from CSV (format: date)"
echo "✅ Other 7 tables: NGAY_DL = date from filename (format: dd/mm/yyyy)"
echo "✅ All NGAY_DL fields should be DATE type, not STRING"
echo ""

echo "📋 NEXT STEPS:"
echo "=============="
echo "1. Compare CSV headers with current database table structures"
echo "2. Verify business columns are ordered first"
echo "3. Ensure NGAY_DL field is properly mapped and typed as DATE"
echo "4. Update table structures if needed"
echo "5. Test import functionality"
