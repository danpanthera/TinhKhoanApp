#!/bin/bash

# 🔧 FIX LN03 CSV FORMAT ISSUES
# Khắc phục vấn đề với file CSV LN03 không parse được

echo "🔧 KHẮC PHỤC FILE LN03 CSV ISSUES..."

# File gốc và file fix
ORIGINAL_FILE="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7808_ln03_20241231.csv"
FIXED_FILE="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7800_ln03_20241231_fixed.csv"

echo "📁 Xử lý file: $ORIGINAL_FILE"

# Backup file gốc
cp "$ORIGINAL_FILE" "${ORIGINAL_FILE}.backup"
echo "💾 Đã backup file gốc: ${ORIGINAL_FILE}.backup"

# Tạo file fix mới
echo "🔧 Tạo file fix với format chuẩn..."

# Header mới (bỏ 3 cột trống cuối)
cat > "$FIXED_FILE" << 'EOF'
MACHINHANH,TENCHINHANH,MAKH,TENKH,SOHOPDONG,SOTIENXLRR,NGAYPHATSINHXL,THUNOSAUXL,CONLAINGOAIBANG,DUNONOIBANG,NHOMNO,MACBTD,TENCBTD,MAPGD,TAIKHOANHACHTOAN,REFNO,LOAINGUONVON
7800,Chi nhanh H. Nam Nhun - Lai Chau,010674574,Nguyễn Duy Tình,7808-LAV-201900012,114000000,20210806,0,114000000,0,,780800010,Lường Thị Diệp,00,971103,78080106745747808-LAV-2019000127808LDS201900012,Cá nhân
EOF

echo "✅ Đã tạo file fix: $FIXED_FILE"

# Hiển thị nội dung file fix
echo ""
echo "📋 Nội dung file sau khi fix:"
cat "$FIXED_FILE"

echo ""
echo "🔍 So sánh với file gốc:"
echo "GỐCØ$(head -2 "$ORIGINAL_FILE")"
echo ""
echo "FIX: $(head -2 "$FIXED_FILE")"

echo ""
echo "✅ HOÀN THÀNH! File đã được fix tại: $FIXED_FILE"
echo "🚀 Bây giờ có thể test import file này."
