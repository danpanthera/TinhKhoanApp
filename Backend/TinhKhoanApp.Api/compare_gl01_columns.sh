#!/bin/bash

# Script so sánh chi tiết và chính xác cấu trúc GL01
# Created: July 12, 2025

echo "🔍 KIỂM TRA CHI TIẾT BẢNG GL01"
echo "=============================="

CSV_FILE="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7808_gl01_2025030120250331.csv"

echo "📋 CSV Header chính xác (loại bỏ BOM):"
head -1 "$CSV_FILE" | sed 's/^\xEF\xBB\xBF//' | tr ',' '\n' | sed 's/\r$//' | nl

echo ""
echo "📋 Model Business Columns:"
grep -E "^\s*\[Column\(" Models/DataTables/GL01.cs | grep -v -E "(NGAY_DL|CREATED_DATE|UPDATED_DATE|FILE_NAME)" | sed 's/.*"\(.*\)".*/\1/' | nl

echo ""
echo "🔢 So sánh số lượng:"
CSV_COUNT=$(head -1 "$CSV_FILE" | sed 's/^\xEF\xBB\xBF//' | tr ',' '\n' | sed 's/\r$//' | wc -l)
MODEL_COUNT=$(grep -E "^\s*\[Column\(" Models/DataTables/GL01.cs | grep -v -E "(NGAY_DL|CREATED_DATE|UPDATED_DATE|FILE_NAME)" | wc -l)

echo "CSV: $CSV_COUNT cột"
echo "Model business: $MODEL_COUNT cột"

# So sánh từng cột một cách chính xác
echo ""
echo "🔍 Kiểm tra từng cột:"

# Tạo list cột CSV clean
CSV_COLS=$(head -1 "$CSV_FILE" | sed 's/^\xEF\xBB\xBF//' | tr ',' '\n' | sed 's/\r$//')
MODEL_COLS=$(grep -E "^\s*\[Column\(" Models/DataTables/GL01.cs | grep -v -E "(NGAY_DL|CREATED_DATE|UPDATED_DATE|FILE_NAME)" | sed 's/.*"\(.*\)".*/\1/')

counter=1
echo "$CSV_COLS" | while read csv_col; do
    model_col=$(echo "$MODEL_COLS" | sed -n "${counter}p")
    if [ "$csv_col" = "$model_col" ]; then
        echo "✅ Cột $counter: $csv_col"
    else
        echo "❌ Cột $counter: CSV='$csv_col' vs Model='$model_col'"
    fi
    counter=$((counter + 1))
done

echo ""
if [ "$CSV_COUNT" -eq "$MODEL_COUNT" ]; then
    echo "✅ KẾT LUẬN: Bảng GL01 hoàn hảo với $MODEL_COUNT business columns khớp CSV!"
else
    echo "❌ KẾT LUẬN: Cần sửa model GL01 để khớp CSV ($CSV_COUNT cột)"
fi
