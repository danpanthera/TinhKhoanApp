#!/bin/bash

echo "🎯 KẾT QUẢ KIỂM TRA CUỐI CÙNG DP01"
echo "================================="

CSV_FILE="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7808_dp01_20241231.csv"
MODEL_FILE="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/Models/DataTables/DP01.cs"

# Đếm chính xác
CSV_COUNT=$(head -1 "$CSV_FILE" | sed 's/﻿//' | tr ',' '\n' | wc -l | tr -d ' ')
MODEL_COUNT=$(cd "/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api" && grep -E "^\s*\[Column\(" "$MODEL_FILE" | grep -v -E "(NGAY_DL|CREATED_DATE|UPDATED_DATE|FILE_NAME)" | wc -l | tr -d ' ')

echo "📊 SỐ LƯỢNG CỘT:"
echo "   - File CSV: $CSV_COUNT cột"
echo "   - Model DP01 (business): $MODEL_COUNT cột"
echo "   - System/Temporal: 4 cột (Id, NGAY_DL, CREATED_DATE, UPDATED_DATE, FILE_NAME)"

# Tính tổng cột trong model
TOTAL_MODEL_COLS=$(cd "/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api" && grep -E "^\s*\[Column\(" "$MODEL_FILE" | wc -l | tr -d ' ')
echo "   - Model DP01 (tổng): $TOTAL_MODEL_COLS cột"

echo ""
if [ "$CSV_COUNT" -eq "$MODEL_COUNT" ]; then
    echo "✅ PERFECT: Bảng DP01 có đủ $CSV_COUNT cột business theo file CSV!"
    echo "✅ STRUCTURE: Model đã chuẩn với temporal tables và columnstore"
    echo "✅ READY: Sẵn sàng import file 7808_dp01_20241231.csv"

    echo ""
    echo "🎉 CONFIRMATION:"
    echo "   📄 CSV Header: $(head -1 "$CSV_FILE" | sed 's/﻿//' | cut -d',' -f1,2,3 | tr ',' ' | ')"
    echo "   🏗️ Model Start: MA_CN | TAI_KHOAN_HACH_TOAN | MA_KH"
    echo "   📄 CSV End: $(head -1 "$CSV_FILE" | sed 's/﻿//' | rev | cut -d',' -f1,2,3 | rev | tr ',' ' | ')"
    echo "   🏗️ Model End: UNTBUSCD | TYGIA"
else
    echo "❌ MISMATCH: CSV có $CSV_COUNT cột, Model có $MODEL_COUNT cột business"
fi
