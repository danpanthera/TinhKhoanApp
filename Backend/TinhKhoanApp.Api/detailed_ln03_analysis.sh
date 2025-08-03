#!/bin/bash
# detailed_ln03_analysis.sh
# Phân tích chi tiết file LN03 thực tế

echo "🔬 PHÂN TÍCH CHI TIẾT LN03"
echo "========================="

LN03_FILE="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7800_ln03_20241231.csv"

echo "1. Đếm số cột trong header vs dữ liệu..."
echo "Header có số cột:"
head -1 "$LN03_FILE" | tr ',' '\n' | wc -l

echo ""
echo "Dòng dữ liệu đầu tiên có số cột:"
sed -n '2p' "$LN03_FILE" | tr ',' '\n' | wc -l

echo ""
echo "Dòng dữ liệu thứ hai có số cột:"
sed -n '3p' "$LN03_FILE" | tr ',' '\n' | wc -l

echo ""
echo "2. So sánh cột header vs dữ liệu..."
echo "=== HEADER (17 cột) ==="
head -1 "$LN03_FILE" | tr ',' '\n' | nl

echo ""
echo "=== DÒNG DỮ LIỆU 1 (chia theo comma) ==="
sed -n '2p' "$LN03_FILE" | tr ',' '\n' | nl

echo ""
echo "3. Tìm 3 cột không có header..."
echo "Cột 18:"
sed -n '2p' "$LN03_FILE" | cut -d',' -f18

echo "Cột 19:"
sed -n '2p' "$LN03_FILE" | cut -d',' -f19

echo "Cột 20:"
sed -n '2p' "$LN03_FILE" | cut -d',' -f20

echo ""
echo "4. Kiểm tra mẫu các cột cuối..."
echo "Cột 16-20 của dòng đầu:"
sed -n '2p' "$LN03_FILE" | cut -d',' -f16-20

echo ""
echo "Cột 16-20 của dòng thứ hai:"
sed -n '3p' "$LN03_FILE" | cut -d',' -f16-20

echo ""
echo "5. Phân tích pattern dữ liệu..."
echo "Kiểm tra 5 dòng đầu (chỉ 3 cột cuối):"
head -5 "$LN03_FILE" | cut -d',' -f18-20 | nl

echo ""
echo "6. Tạo header đầy đủ với 20 cột..."
echo "Header gốc (17 cột):"
head -1 "$LN03_FILE" | tr ',' '\n' | paste -s -d','

echo ""
echo "Đề xuất header đầy đủ (20 cột):"
echo "$(head -1 "$LN03_FILE" | tr -d '\n'),COL18_UNKNOWN,COL19_UNKNOWN,COL20_UNKNOWN"
