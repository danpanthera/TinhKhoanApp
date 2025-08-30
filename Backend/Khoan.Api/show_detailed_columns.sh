#!/bin/bash

echo "🔍 CHI TIẾT CỘT ĐẦU TIÊN CỦA TỪNG BẢNG"
echo "======================================"
echo "$(date '+%Y-%m-%d %H:%M:%S') - Hiển thị 10 cột business đầu của mỗi bảng"
echo ""

show_table_columns() {
    local table_name=$1
    echo "📊 Bảng: $table_name"
    echo "-------------------"

    sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d KhoanDB -Q "
    SELECT TOP 10
        COLUMN_NAME,
        ORDINAL_POSITION,
        DATA_TYPE,
        CHARACTER_MAXIMUM_LENGTH
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = '$table_name'
    AND COLUMN_NAME NOT IN ('Id', 'NGAY_DL', 'CREATED_DATE', 'UPDATED_DATE', 'FILE_NAME', 'ValidFrom', 'ValidTo')
    ORDER BY ORDINAL_POSITION
    " 2>/dev/null

    echo ""
}

# Hiển thị tất cả bảng
for table in DP01 DPDA EI01 GL01 GL41 LN01 LN03 RR01; do
    show_table_columns $table
done

echo "🚨 VẤNĐỀ PHÁT HIỆN:"
echo "==================="
echo "❌ Tất cả các bảng đang sử dụng generic column names (Col1, Col2, Col3, etc.)"
echo "❌ Không có tên cột thực tế từ CSV headers"
echo "❌ Một số bảng có số lượng cột không đúng với documented"
echo ""
echo "💡 CẦN LÀME:"
echo "============="
echo "1. Lấy CSV files gốc để xem đúng column headers"
echo "2. Rebuild table schema với tên cột thực tế"
echo "3. Re-import data từ CSV với đúng mapping"
echo "4. Validate lại cấu trúc sau khi fix"
