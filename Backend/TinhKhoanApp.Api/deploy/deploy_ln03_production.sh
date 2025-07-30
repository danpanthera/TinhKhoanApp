#!/bin/bash

# Script triển khai LN03 cho môi trường sản xuất
# Tạo tự động vào: $(date +"%d/%m/%Y %H:%M:%S")

echo "=== Triển khai LN03 trên môi trường sản xuất ==="

# Dừng dịch vụ trước khi cập nhật
echo "Dừng dịch vụ API..."
systemctl stop tinhkhoanapp-api

# Sao lưu cơ sở dữ liệu
echo "Sao lưu cơ sở dữ liệu..."
BACKUP_FILE="/var/backups/tinhkhoanapp/db_backup_$(date +"%Y%m%d_%H%M%S").bak"
sqlcmd -S localhost -d TinhKhoanAppDb -U sa -P "$DB_PASSWORD" -Q "BACKUP DATABASE [TinhKhoanAppDb] TO DISK = N'$BACKUP_FILE' WITH COMPRESSION, INIT"

# Thực hiện migration
echo "Thực hiện migration SQL..."
sqlcmd -S localhost -d TinhKhoanAppDb -U sa -P "$DB_PASSWORD" -i ln03_migration.sql

if [ $? -ne 0 ]; then
    echo "Migration thất bại. Hủy triển khai và khôi phục dịch vụ."
    systemctl start tinhkhoanapp-api
    exit 1
fi

# Sao chép file mới
echo "Sao chép các file đã cập nhật..."
cp -f /tmp/deploy/TinhKhoanApp.Api.dll /opt/tinhkhoanapp/api/

# Khởi động lại dịch vụ
echo "Khởi động lại dịch vụ..."
systemctl start tinhkhoanapp-api

# Kiểm tra trạng thái dịch vụ
echo "Kiểm tra trạng thái dịch vụ..."
systemctl status tinhkhoanapp-api

echo "=== Triển khai hoàn tất ==="
