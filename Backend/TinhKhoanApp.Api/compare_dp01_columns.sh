#!/bin/bash

# Script so sánh các cột giữa file CSV và model DP01
CSV_FILE="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7808_dp01_20241231.csv"
MODEL_FILE="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/Models/DataTables/DP01.cs"

echo "🔍 SO SÁNH CỘT DP01: FILE CSV vs MODEL"
echo "======================================"

# Lấy danh sách cột từ CSV (loại bỏ BOM)
echo "📄 Cột từ file CSV:"
head -1 "$CSV_FILE" | sed 's/﻿//' | tr ',' '\n' | nl

echo ""
echo "🏗️ Cột từ model DP01 (business columns only):"
cd "/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api"
grep -E "^\s*\[Column\(" "$MODEL_FILE" | grep -v -E "(NGAY_DL|CREATED_DATE|UPDATED_DATE|FILE_NAME)" | sed 's/.*\[Column("\([^"]*\)").*/\1/' | nl

echo ""
echo "📊 TỔNG KẾT:"
CSV_COUNT=$(head -1 "$CSV_FILE" | sed 's/﻿//' | tr ',' '\n' | wc -l | tr -d ' ')
MODEL_COUNT=$(grep -E "^\s*\[Column\(" "$MODEL_FILE" | grep -v -E "(NGAY_DL|CREATED_DATE|UPDATED_DATE|FILE_NAME)" | wc -l | tr -d ' ')

echo "- File CSV: $CSV_COUNT cột"
echo "- Model DP01: $MODEL_COUNT cột business"

if [ "$CSV_COUNT" -eq "$MODEL_COUNT" ]; then
    echo "✅ SỐ LƯỢNG CỘT: KHỚP HOÀN TOÀN ($CSV_COUNT cột)"
else
    echo "❌ SỐ LƯỢNG CỘT: KHÔNG KHỚP (CSV: $CSV_COUNT, Model: $MODEL_COUNT)"
fi

echo ""
echo "🔍 KIỂM TRA CHI TIẾT TỪNG CỘT:"

# Tạo temp files để so sánh
CSV_COLS=$(mktemp)
MODEL_COLS=$(mktemp)

head -1 "$CSV_FILE" | sed 's/﻿//' | tr ',' '\n' > "$CSV_COLS"
grep -E "^\s*\[Column\(" "$MODEL_FILE" | grep -v -E "(NGAY_DL|CREATED_DATE|UPDATED_DATE|FILE_NAME)" | sed 's/.*\[Column("\([^"]*\)").*/\1/' > "$MODEL_COLS"

# So sánh từng cột
diff_result=$(diff "$CSV_COLS" "$MODEL_COLS")
if [ -z "$diff_result" ]; then
    echo "✅ TÊN CỘT: KHỚP HOÀN TOÀN (tất cả 63 cột)"
    echo "✅ THỨ TỰ CỘT: ĐÚNG"
else
    echo "❌ CÓ SỰ KHÁC BIỆT:"
    echo "$diff_result"
fi

# Cleanup
rm "$CSV_COLS" "$MODEL_COLS"

echo ""
echo "🎯 KẾT LUẬN CUỐI CÙNG:"
if [ "$CSV_COUNT" -eq "$MODEL_COUNT" ] && [ -z "$diff_result" ]; then
    echo "✅ HOÀN HẢO: Model DP01 có đủ và đúng 63 cột business theo file CSV!"
    echo "✅ SAMEPLE DATA: Sẵn sàng import file 7808_dp01_20241231.csv"
else
    echo "❌ CẦN SỬA CHỮA: Model chưa khớp hoàn toàn với file CSV"
fi
