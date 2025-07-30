#!/bin/bash

# Script để cập nhật thông tin kết nối trong các script triển khai
# Tạo bởi: Copilot
# Ngày: $(date +"%d/%m/%Y")

echo "=== Cập nhật thông tin kết nối trong các script ==="

# Thông tin kết nối cũ
OLD_DB_SERVER="localhost"
OLD_DB_NAME="TinhKhoanApp"
OLD_DB_USER="sa"
OLD_DB_PASSWORD="YourStrong!Passw0rd"

# Thông tin kết nối mới
NEW_DB_SERVER="localhost,1433"
NEW_DB_NAME="TinhKhoanDB"
NEW_DB_USER="sa"
NEW_DB_PASSWORD="Dientoan@303"

# Danh sách các file cần cập nhật
FILES=(
  "backup_ln03_data.sh"
  "run_ln03_migration.sh"
  "verify_ln03_changes.sh"
  "test_data/run_ln03_tests.sh"
)

# Cập nhật từng file
for file in "${FILES[@]}"; do
  if [ -f "$file" ]; then
    echo "Cập nhật file $file..."
    
    # Tạo bản sao
    cp "$file" "${file}.bak"
    
    # Thay thế thông tin kết nối
    sed -i.tmp "s|$OLD_DB_SERVER|$NEW_DB_SERVER|g" "$file"
    sed -i.tmp "s|$OLD_DB_NAME|$NEW_DB_NAME|g" "$file"
    sed -i.tmp "s|$OLD_DB_USER|$NEW_DB_USER|g" "$file"
    sed -i.tmp "s|$OLD_DB_PASSWORD|$NEW_DB_PASSWORD|g" "$file"
    
    # Xóa file tạm
    rm -f "${file}.tmp"
    
    echo "Đã cập nhật file $file thành công."
  else
    echo "File $file không tồn tại, bỏ qua."
  fi
done

echo "=== Hoàn tất cập nhật thông tin kết nối ==="
