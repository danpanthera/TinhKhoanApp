#!/bin/bash

echo "=== VERIFICATION TSDB01 - TÀI SẢN ĐẢM BẢO ==="
echo "Date: $(date)"
echo "File: 7808_tsdb01_20241231.csv"
echo ""

# Kiểm tra file CSV
CSV_FILE="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7808_tsdb01_20241231.csv"

if [ ! -f "$CSV_FILE" ]; then
    echo "❌ File CSV không tồn tại: $CSV_FILE"
    exit 1
fi

echo "📄 Kiểm tra file CSV:"
CSV_HEADERS=$(head -1 "$CSV_FILE" | tr ',' '\n' | wc -l | tr -d ' ')
echo "📊 Số cột CSV header: $CSV_HEADERS"

echo ""
echo "🔍 HEADER CSV CHI TIẾT (16 cột):"
head -1 "$CSV_FILE" | tr ',' '\n' | sed 's/﻿//g' | nl

echo ""
echo "📄 Kiểm tra model TSDB01:"
MODEL_FILE="Models/DataTables/TSDB01.cs"

# Đếm business columns (loại trừ system columns)
BUSINESS_COLS=$(grep "\[Column(" "$MODEL_FILE" | grep -v "NGAY_DL\|CREATED_DATE\|UPDATED_DATE\|FILE_NAME" | wc -l | tr -d ' ')
TOTAL_COLS=$(grep -c "\[Column(" "$MODEL_FILE")
SYSTEM_COLS=$((TOTAL_COLS - BUSINESS_COLS))

echo "📊 Business columns trong model: $BUSINESS_COLS"
echo "📊 System/Temporal columns: $SYSTEM_COLS (NGAY_DL, CREATED_DATE, UPDATED_DATE, FILE_NAME)"
echo "📊 Tổng [Column] attributes: $TOTAL_COLS"

echo ""
echo "🔍 BUSINESS COLUMNS TRONG MODEL (16 cột):"
grep "\[Column(" "$MODEL_FILE" | grep -v "NGAY_DL\|CREATED_DATE\|UPDATED_DATE\|FILE_NAME" | grep -o '"[^"]*"' | sed 's/"//g' | nl

echo ""
echo "🎯 KẾT QUẢ SO SÁNH:"
if [ "$CSV_HEADERS" -eq "$BUSINESS_COLS" ]; then
    echo "✅ HOÀN HẢO: Business columns model ($BUSINESS_COLS) = CSV headers ($CSV_HEADERS)"
    echo "✅ Model TSDB01 đã chính xác 100%"
else
    echo "❌ SAI LỆCH: Business columns model ($BUSINESS_COLS) ≠ CSV headers ($CSV_HEADERS)"
    echo "🔧 CẦN CẬP NHẬT MODEL"
fi

echo ""
echo "📋 SO SÁNH TỪNG CỘT:"
echo "CSV HEADERS vs MODEL COLUMNS:"

# Tạo file tạm để so sánh
CSV_TEMP="/tmp/csv_headers_tsdb01.txt"
MODEL_TEMP="/tmp/model_columns_tsdb01.txt"

head -1 "$CSV_FILE" | tr ',' '\n' | sed 's/﻿//g' | sort > "$CSV_TEMP"
grep "\[Column(" "$MODEL_FILE" | grep -v "NGAY_DL\|CREATED_DATE\|UPDATED_DATE\|FILE_NAME" | grep -o '"[^"]*"' | sed 's/"//g' | sort > "$MODEL_TEMP"

if diff "$CSV_TEMP" "$MODEL_TEMP" > /dev/null; then
    echo "✅ TẤT CẢ TÊN CỘT KHỚP HOÀN TOÀN"
else
    echo "❌ CÓ SAI LỆCH TÊN CỘT:"
    diff "$CSV_TEMP" "$MODEL_TEMP"
fi

# Cleanup
rm -f "$CSV_TEMP" "$MODEL_TEMP"

echo ""
echo "=== KẾT THÚC VERIFICATION TSDB01 ==="
