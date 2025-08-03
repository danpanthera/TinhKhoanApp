#!/bin/bash
# analyze_ln03_sample.sh
# Phân tích file mẫu LN03 từ DuLieuMau

echo "🔍 PHÂN TÍCH FILE LN03 MẪU"
echo "=========================="

SAMPLE_PATH="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau"

echo "1. Tìm file LN03 trong DuLieuMau..."
find "$SAMPLE_PATH" -name "*ln03*" -o -name "*LN03*" 2>/dev/null | head -10

echo ""
echo "2. Liệt kê tất cả file CSV trong thư mục..."
ls -la "$SAMPLE_PATH"/*.csv 2>/dev/null | head -20

echo ""
echo "3. Tìm file có pattern LN03..."
ls -la "$SAMPLE_PATH"/*ln03* 2>/dev/null
ls -la "$SAMPLE_PATH"/*LN03* 2>/dev/null

echo ""
echo "4. Kiểm tra file có chứa 'ln03' trong tên..."
find "$SAMPLE_PATH" -type f -iname "*ln03*" 2>/dev/null

echo ""
echo "5. Liệt kê tất cả file trong thư mục..."
ls -la "$SAMPLE_PATH" 2>/dev/null | grep -i csv

echo ""
echo "6. Nếu tìm thấy file LN03, phân tích header..."
LN03_FILE=$(find "$SAMPLE_PATH" -type f -iname "*ln03*.csv" | head -1)

if [ -n "$LN03_FILE" ]; then
    echo "🎯 Tìm thấy file LN03: $LN03_FILE"
    echo ""
    echo "Header (dòng đầu):"
    head -1 "$LN03_FILE" | sed 's/,/\n/g' | nl

    echo ""
    echo "Tổng số cột:"
    head -1 "$LN03_FILE" | tr ',' '\n' | wc -l

    echo ""
    echo "Mẫu dữ liệu (3 dòng đầu):"
    head -3 "$LN03_FILE"

    echo ""
    echo "Kiểm tra cột trống (không có header):"
    head -1 "$LN03_FILE" | grep -o ',,' && echo "Có cột trống!" || echo "Không có cột trống trong header"

else
    echo "❌ Không tìm thấy file LN03 trong $SAMPLE_PATH"
    echo "Các file có sẵn:"
    ls -la "$SAMPLE_PATH" 2>/dev/null
fi
