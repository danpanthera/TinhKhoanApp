#!/bin/bash

# Script kiểm tra và so sánh cấu trúc bảng EI01 với CSV file
# Created: July 12, 2025

echo "🔍 KIỂM TRA CẤU TRÚC BẢNG EI01 vs CSV FILE"
echo "=========================================="

CSV_FILE="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7808_ei01_20241231.csv"
MODEL_FILE="Models/DataTables/EI01.cs"

echo "📁 CSV File: $CSV_FILE"
echo "📁 Model File: $MODEL_FILE"
echo ""

# Đếm số cột trong CSV (xử lý BOM)
echo "📊 ĐẾM SỐ CỘT:"
CSV_COUNT=$(head -1 "$CSV_FILE" | sed 's/^\xEF\xBB\xBF//' | tr ',' '\n' | wc -l)
echo "   CSV file: $CSV_COUNT cột"

# Đếm business columns trong model
MODEL_COUNT=$(grep -E "^\s*\[Column\(" "$MODEL_FILE" | grep -v -E "(NGAY_DL|CREATED_DATE|UPDATED_DATE|FILE_NAME)" | wc -l)
echo "   Model business columns: $MODEL_COUNT cột"

# Đếm tổng columns trong model
TOTAL_MODEL_COUNT=$(grep -E "^\s*\[Column\(" "$MODEL_FILE" | wc -l)
echo "   Model total columns: $TOTAL_MODEL_COUNT cột (bao gồm system + temporal)"

echo ""

# Lấy danh sách cột từ CSV (loại bỏ BOM)
echo "📋 DANH SÁCH CỘT TRONG CSV:"
head -1 "$CSV_FILE" | sed 's/^\xEF\xBB\xBF//' | tr ',' '\n' | nl

echo ""

# Lấy danh sách business columns từ model
echo "📋 DANH SÁCH BUSINESS COLUMNS TRONG MODEL:"
grep -E "^\s*\[Column\(" "$MODEL_FILE" | grep -v -E "(NGAY_DL|CREATED_DATE|UPDATED_DATE|FILE_NAME)" | sed 's/.*"\(.*\)".*/\1/' | nl

echo ""

# So sánh từng cột
echo "🔍 SO SÁNH CHI TIẾT:"
CSV_COLS=$(head -1 "$CSV_FILE" | sed 's/^\xEF\xBB\xBF//' | tr ',' '\n')
MODEL_COLS=$(grep -E "^\s*\[Column\(" "$MODEL_FILE" | grep -v -E "(NGAY_DL|CREATED_DATE|UPDATED_DATE|FILE_NAME)" | sed 's/.*"\(.*\)".*/\1/')

# Tạo file tạm để so sánh
echo "$CSV_COLS" > /tmp/csv_cols_ei01.txt
echo "$MODEL_COLS" > /tmp/model_cols_ei01.txt

if diff /tmp/csv_cols_ei01.txt /tmp/model_cols_ei01.txt > /dev/null; then
    echo "✅ PERFECT: Tất cả cột business trong model khớp với CSV!"
else
    echo "❌ KHÁC BIỆT được phát hiện:"
    echo "   CSV có nhưng Model không có:"
    comm -23 /tmp/csv_cols_ei01.txt /tmp/model_cols_ei01.txt | sed 's/^/   - /'
    echo "   Model có nhưng CSV không có:"
    comm -13 /tmp/csv_cols_ei01.txt /tmp/model_cols_ei01.txt | sed 's/^/   - /'
fi

# Dọn dẹp
rm -f /tmp/csv_cols_ei01.txt /tmp/model_cols_ei01.txt

echo ""
echo "📊 KẾT QUẢ CUỐI CÙNG:"
if [ "$CSV_COUNT" -eq "$MODEL_COUNT" ]; then
    echo "✅ PERFECT: Bảng EI01 có đủ $MODEL_COUNT cột business theo file CSV!"
    echo "✅ Cấu trúc: $MODEL_COUNT business + 4 system/temporal = $TOTAL_MODEL_COUNT total columns"
else
    echo "❌ CẢNH BÁO: Số cột không khớp!"
    echo "   CSV: $CSV_COUNT cột"
    echo "   Model business: $MODEL_COUNT cột"
    echo "   Cần sửa model để khớp với CSV"
fi

echo ""
echo "🏗️ SYSTEM & TEMPORAL COLUMNS trong model:"
echo "   - Id (Primary Key)"
echo "   - NGAY_DL (Data Date)"
echo "   - CREATED_DATE (System)"
echo "   - UPDATED_DATE (System)"
echo "   - FILE_NAME (System)"
