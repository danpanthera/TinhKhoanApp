#!/bin/bash

# Script triển khai LN03 cho môi trường staging
# Tạo tự động vào: $(date +"%d/%m/%Y %H:%M:%S")

echo "=== Triển khai LN03 trên môi trường staging ==="

# Thực hiện build cho môi trường staging
echo "Build ứng dụng cho môi trường staging..."
cd /opt/tinhkhoanapp/staging/src
git pull
dotnet build -c Release

# Dừng dịch vụ staging
echo "Dừng dịch vụ staging..."
systemctl stop tinhkhoanapp-staging

# Sao lưu cơ sở dữ liệu
echo "Sao lưu cơ sở dữ liệu staging..."
BACKUP_FILE="/var/backups/tinhkhoanapp/staging_db_backup_$(date +"%Y%m%d_%H%M%S").bak"
sqlcmd -S localhost -d TinhKhoanAppStaging -U sa -P "$DB_PASSWORD" -Q "BACKUP DATABASE [TinhKhoanAppStaging] TO DISK = N'$BACKUP_FILE' WITH COMPRESSION, INIT"

# Thực hiện migration
echo "Thực hiện migration SQL..."
sqlcmd -S localhost -d TinhKhoanAppStaging -U sa -P "$DB_PASSWORD" -i ln03_migration.sql

# Sao chép build mới
echo "Cập nhật files..."
cp -r /opt/tinhkhoanapp/staging/src/bin/Release/net8.0/publish/* /opt/tinhkhoanapp/staging/

# Khởi động lại dịch vụ
echo "Khởi động lại dịch vụ staging..."
systemctl start tinhkhoanapp-staging

# Kiểm tra trạng thái dịch vụ
echo "Kiểm tra trạng thái dịch vụ..."
systemctl status tinhkhoanapp-staging

echo "=== Triển khai staging hoàn tất ==="
