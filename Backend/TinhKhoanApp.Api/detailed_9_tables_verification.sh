#!/bin/bash
# SCRIPT: detailed_9_tables_verification.sh
# MỤC ĐÍCH: Kiểm tra chi tiết và chính xác hơn cho 9 bảng

echo "🔍 KIỂM TRA CHI TIẾT 9 BẢNG DỮ LIỆU"
echo "====================================="

BACKEND_DIR="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api"
cd "$BACKEND_DIR" || exit 1

# Function chi tiết cho từng bảng
detailed_check() {
    local table=$1
    echo "🔍 DETAILED CHECK: $table"
    echo "========================="

    case $table in
        "DP01")
            echo "📄 CSV: 7800_dp01_20241231.csv"
            csv_cols=$(head -1 7800_dp01_20241231.csv | tr ',' '\n' | wc -l | tr -d ' ')
            echo "📊 CSV Columns: $csv_cols"

            model_cols=$(grep -E "public.*{.*set;" Models/DataTables/DP01.cs | grep -v "Id\|NGAY_DL\|DataSource\|ImportDateTime\|CreatedAt\|UpdatedAt\|CreatedBy\|UpdatedBy" | wc -l | tr -d ' ')
            echo "📋 Model Business Columns: $model_cols"

            if [ "$csv_cols" -eq "$model_cols" ]; then
                echo "✅ STATUS: PERFECT MATCH"
            else
                echo "❌ STATUS: MISMATCH"
            fi
            ;;

        "DPDA")
            echo "📄 CSV: 7800_dpda_20250331.csv"
            csv_cols=$(head -1 7800_dpda_20250331.csv | tr ',' '\n' | wc -l | tr -d ' ')
            echo "📊 CSV Columns: $csv_cols"

            model_cols=$(grep -E "public.*{.*set;" Models/DataTables/DPDA.cs | grep -v "Id\|NGAY_DL\|CreatedAt\|UpdatedAt\|FILE_NAME" | wc -l | tr -d ' ')
            echo "📋 Model Business Columns: $model_cols"

            if [ "$csv_cols" -eq "$model_cols" ]; then
                echo "✅ STATUS: PERFECT MATCH"
            else
                echo "❌ STATUS: MISMATCH"
            fi
            ;;

        "EI01")
            echo "📄 CSV: 7800_ei01_20241231.csv"
            csv_cols=$(head -1 7800_ei01_20241231.csv | tr ',' '\n' | wc -l | tr -d ' ')
            echo "📊 CSV Columns: $csv_cols"

            model_cols=$(grep -E "public.*{.*set;" Models/DataTables/EI01.cs | grep -v "Id\|NGAY_DL\|CreatedAt\|UpdatedAt\|FILE_NAME" | wc -l | tr -d ' ')
            echo "📋 Model Business Columns: $model_cols"

            if [ "$csv_cols" -eq "$model_cols" ]; then
                echo "✅ STATUS: PERFECT MATCH"
            else
                echo "❌ STATUS: MISMATCH"
            fi
            ;;

        "GL01")
            echo "📄 CSV: 7800_gl01_2024120120241231.csv"
            csv_cols=$(head -1 7800_gl01_2024120120241231.csv | tr ',' '\n' | wc -l | tr -d ' ')
            echo "📊 CSV Columns: $csv_cols"

            model_cols=$(grep -E "public.*{.*set;" Models/DataTables/GL01.cs | grep -v "Id\|NGAY_DL\|CreatedAt\|UpdatedAt\|FILE_NAME" | wc -l | tr -d ' ')
            echo "📋 Model Business Columns: $model_cols"

            if [ "$csv_cols" -eq "$model_cols" ]; then
                echo "✅ STATUS: PERFECT MATCH"
            else
                echo "❌ STATUS: MISMATCH"
            fi
            ;;

        "GL02")
            echo "📄 CSV: 7800_gl02_2024120120241231.csv"
            csv_cols=$(head -1 7800_gl02_2024120120241231.csv | tr ',' '\n' | wc -l | tr -d ' ')
            echo "📊 CSV Columns: $csv_cols"

            model_cols=$(grep -E "public.*{.*set;" Models/DataTables/GL02.cs | grep -v "Id\|NGAY_DL\|CREATED_DATE\|UPDATED_DATE\|FILE_NAME" | wc -l | tr -d ' ')
            echo "📋 Model Business Columns: $model_cols"

            if [ "$csv_cols" -eq "$model_cols" ]; then
                echo "✅ STATUS: PERFECT MATCH"
            else
                echo "❌ STATUS: MISMATCH"
                echo "🔍 CSV Headers:"
                head -1 7800_gl02_2024120120241231.csv | tr ',' '\n' | nl
                echo "🔍 Model Properties:"
                grep -E "public.*{.*set;" Models/DataTables/GL02.cs | grep -v "Id\|NGAY_DL\|CREATED_DATE\|UPDATED_DATE\|FILE_NAME" | nl
            fi
            ;;

        "GL41")
            echo "📄 CSV: 7800_gl41_20241231.csv"
            if [ ! -f "7800_gl41_20241231.csv" ]; then
                echo "❌ CSV FILE MISSING"
                echo "📋 Model has business columns but no CSV to verify"
            fi
            ;;

        "LN01")
            echo "📄 CSV: 7800_ln01_20241231.csv"
            csv_cols=$(head -1 7800_ln01_20241231.csv | tr ',' '\n' | wc -l | tr -d ' ')
            echo "📊 CSV Columns: $csv_cols"

            model_cols=$(grep -E "public.*{.*set;" Models/DataTables/LN01.cs | grep -v "Id\|NGAY_DL\|FILE_NAME\|CREATED_DATE\|UPDATED_DATE" | wc -l | tr -d ' ')
            echo "📋 Model Business Columns: $model_cols"

            if [ "$csv_cols" -eq "$model_cols" ]; then
                echo "✅ STATUS: PERFECT MATCH"
            else
                echo "❌ STATUS: MISMATCH - Model có thêm $(($model_cols - $csv_cols)) columns"
            fi
            ;;

        "LN03")
            echo "📄 CSV: 7800_ln03_20241231.csv"
            csv_cols=$(head -1 7800_ln03_20241231.csv | tr ',' '\n' | wc -l | tr -d ' ')
            echo "📊 CSV Columns (có header): $csv_cols"

            # LN03 đặc biệt: có 17 columns có header + 3 columns không header = 20 total
            echo "📊 Theo spec: 17 có header + 3 không header = 20 business columns"

            model_cols=$(grep -E "public.*{.*set;" Models/DataTables/LN03.cs | grep -v "Id\|NGAY_DL\|CREATED_BY\|CREATED_DATE\|FILE_ORIGIN" | wc -l | tr -d ' ')
            echo "📋 Model Business Columns: $model_cols"

            if [ "$model_cols" -eq 20 ]; then
                echo "✅ STATUS: CORRECT (20 business columns as per spec)"
            else
                echo "❌ STATUS: MISMATCH - Expected 20, got $model_cols"
            fi
            ;;

        "RR01")
            echo "📄 CSV: 7800_rr01_20241231.csv"
            csv_cols=$(head -1 7800_rr01_20241231.csv | tr ',' '\n' | wc -l | tr -d ' ')
            echo "📊 CSV Columns: $csv_cols"

            model_cols=$(grep -E "public.*{.*set;" Models/DataTables/RR01.cs | grep -v "Id\|NGAY_DL\|CREATED_DATE\|UPDATED_DATE\|FILE_NAME\|IMPORT_BATCH_ID\|DATA_SOURCE\|PROCESSING_STATUS\|ERROR_MESSAGE\|ROW_HASH" | wc -l | tr -d ' ')
            echo "📋 Model Business Columns: $model_cols"

            if [ "$csv_cols" -eq "$model_cols" ]; then
                echo "✅ STATUS: PERFECT MATCH"
            else
                echo "❌ STATUS: MISMATCH - Model có thêm $(($model_cols - $csv_cols)) columns"
            fi
            ;;
    esac
    echo ""
    echo "-------------------------------------------"
    echo ""
}

# Kiểm tra từng bảng
for table in "DP01" "DPDA" "EI01" "GL01" "GL02" "GL41" "LN01" "LN03" "RR01"; do
    detailed_check "$table"
done

echo "✅ COMPLETED: Detailed 9 Tables Verification"
