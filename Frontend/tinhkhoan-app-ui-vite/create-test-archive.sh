#!/bin/bash

# Script tạo file test archive để kiểm tra import

echo "🗂️ Tạo file archive để test import..."

# Tạo thư mục tạm
mkdir -p temp_archive

# Tạo file CSV test với dữ liệu LN01
cat > temp_archive/LN01_20240115_test.csv << 'EOF'
Ma_Khach_Hang,Ten_Khach_Hang,So_Tai_Khoan,So_Tien,Ngay_Giao_Dich,Loai_Giao_Dich
KH001,Nguyen Van A,123456789,1000000,2024-01-15,VAY_VON
KH002,Tran Thi B,987654321,2000000,2024-01-15,VAY_VON
KH003,Le Van C,456789123,1500000,2024-01-15,VAY_VON
KH004,Pham Thi D,789123456,3000000,2024-01-15,VAY_VON
KH005,Hoang Van E,321654987,2500000,2024-01-15,VAY_VON
EOF

# Tạo file Excel test với dữ liệu DP01
cat > temp_archive/DP01_20240115_test.csv << 'EOF'
Ma_Khach_Hang,Ten_Khach_Hang,So_Tai_Khoan,So_Du,Loai_Tien_Gui,Ngay_Mo
KH101,Vo Van F,111222333,5000000,TIET_KIEM,2024-01-15
KH102,Bui Thi G,444555666,8000000,TIET_KIEM,2024-01-15
KH103,Do Van H,777888999,12000000,CO_KY_HAN,2024-01-15
KH104,Ngo Thi I,123987456,6000000,TIET_KIEM,2024-01-15
KH105,Dang Van J,789456123,9000000,CO_KY_HAN,2024-01-15
EOF

# Tạo file ZIP
cd temp_archive
zip -r ../test_archive_LN01_20240115.zip . > /dev/null 2>&1
cd ..

echo "✅ Đã tạo file test_archive_LN01_20240115.zip"
echo "📋 Nội dung:"
unzip -l test_archive_LN01_20240115.zip

# Dọn dẹp
rm -rf temp_archive

echo ""
echo "🎯 Sử dụng file này để test import archive:"
echo "   - Chọn loại dữ liệu: LN01"
echo "   - Upload file: test_archive_LN01_20240115.zip"
echo "   - Backend sẽ tự động extract và import cả 2 file CSV"
echo ""
