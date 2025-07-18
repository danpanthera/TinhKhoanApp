#!/bin/bash

# Script phân tích headers của các file CSV trong DuLieuMau
# Để xác định cấu trúc cột business cho từng bảng dữ liệu

echo "🔍 PHÂN TÍCH HEADERS CỦA CÁC FILE CSV TRONG DULIEUMAU"
echo "================================================================"

CSV_DIR="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau"

# Danh sách các file và bảng tương ứng (format: TABLE:FILE)
FILES=(
    "DP01:7808_dp01_20241231.csv"
    "EI01:7808_ei01_20241231.csv"
    "GL01:7808_gl01_2025030120250331.csv"
    "GL41:7808_gl41_20250630.csv"
    "LN01:7808_ln01_20241231.csv"
    "LN03:7800_ln03_20241231_fixed.csv"
    "RR01:7800_rr01_20250531.csv"
    "DPDA:7808_dpda_20250331.csv"
)

# Phân tích từng file
for entry in "${FILES[@]}"; do
    table="${entry%%:*}"
    file="${entry##*:}"
    filepath="$CSV_DIR/$file"

    echo ""
    echo "📊 BẢNG $table - FILE: $file"
    echo "----------------------------------------"

    if [ -f "$filepath" ]; then
        # Lấy header (dòng đầu tiên)
        header=$(head -n 1 "$filepath")

        # Đếm số cột
        col_count=$(echo "$header" | tr ',' '\n' | wc -l | tr -d ' ')
        echo "Số cột: $col_count"

        echo "Headers:"
        echo "$header" | tr ',' '\n' | nl -nln

        # Lưu header vào file riêng để dễ xử lý
        echo "$header" > "${table}_headers.txt"
        echo "✅ Đã lưu headers vào ${table}_headers.txt"
    else
        echo "❌ File không tồn tại: $filepath"
    fi
done

echo ""
echo "🎯 TỔNG KẾT:"
echo "- Đã phân tích headers của 8 bảng dữ liệu"
echo "- Headers được lưu vào files: *_headers.txt"
echo "- Sử dụng thông tin này để tạo cấu trúc bảng với business columns ở đầu"
