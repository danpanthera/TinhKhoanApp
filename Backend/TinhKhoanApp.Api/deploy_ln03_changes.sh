#!/bin/bash

# Script triển khai code mới cho LN03
# Tạo bởi: Copilot
# Ngày: $(date +"%d/%m/%Y")

echo "=== Bắt đầu triển khai code mới cho LN03 ==="

# Kiểm tra đã build thành công chưa
echo "Kiểm tra build trước khi triển khai..."
dotnet build

if [ $? -ne 0 ]; then
    echo "Build thất bại. Vui lòng sửa lỗi trước khi triển khai."
    exit 1
fi

# Tạo thư mục sao lưu code nếu chưa tồn tại
BACKUP_DIR="./code_backups"
mkdir -p $BACKUP_DIR

# Tạo bản sao lưu code
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
BACKUP_CODE="$BACKUP_DIR/ln03_code_backup_$TIMESTAMP.tar.gz"

echo "Đang sao lưu code hiện tại vào $BACKUP_CODE..."
tar -czf $BACKUP_CODE Models/DataTables/LN03.cs Models/Dtos/LN03DTO.cs Repositories/LN03Repository.cs Controllers/LN03Controller.cs Services/DirectImportServiceLN03Extension.cs 2>/dev/null

if [ $? -ne 0 ]; then
    echo "Cảnh báo: Không thể sao lưu một số file. Tiếp tục triển khai."
fi

# Tạo thư mục triển khai
DEPLOY_DIR="./deploy"
mkdir -p $DEPLOY_DIR

echo "Chuẩn bị các file cho triển khai..."

# Tạo script SQL cho môi trường sản xuất
cat fix_ln03_data_types.sql > $DEPLOY_DIR/ln03_migration.sql

# Tạo script triển khai để chạy trên máy chủ
cat > $DEPLOY_DIR/deploy_ln03_production.sh << 'DEPLOY_EOF'
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
DEPLOY_EOF

chmod +x $DEPLOY_DIR/deploy_ln03_production.sh

# Tạo file triển khai trên môi trường staging
cat > $DEPLOY_DIR/deploy_ln03_staging.sh << 'STAGING_EOF'
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
STAGING_EOF

chmod +x $DEPLOY_DIR/deploy_ln03_staging.sh

# Tạo file README hướng dẫn triển khai
cat > $DEPLOY_DIR/README.md << 'README_EOF'
# Hướng dẫn triển khai LN03

Các file trong thư mục này cung cấp những công cụ cần thiết để triển khai các thay đổi về mô hình LN03 
lên các môi trường staging và production.

## Các file trong gói triển khai

1. `ln03_migration.sql` - Script SQL để cập nhật cấu trúc bảng LN03
2. `deploy_ln03_staging.sh` - Script triển khai cho môi trường staging
3. `deploy_ln03_production.sh` - Script triển khai cho môi trường production

## Quy trình triển khai khuyến nghị

### 1. Triển khai trên môi trường staging

```bash
# Đặt mật khẩu cơ sở dữ liệu
export DB_PASSWORD="your_db_password"

# Chạy script triển khai staging
./deploy_ln03_staging.sh
```

### 2. Kiểm tra trên môi trường staging

- Kiểm tra API bằng Postman hoặc công cụ tương tự
- Thử upload một file LN03 mẫu để xác nhận quá trình import hoạt động đúng
- Kiểm tra dữ liệu trong cơ sở dữ liệu để đảm bảo các kiểu dữ liệu đã được chuyển đổi chính xác

### 3. Triển khai trên môi trường production

```bash
# Đặt mật khẩu cơ sở dữ liệu
export DB_PASSWORD="your_db_password"

# Chạy script triển khai production
./deploy_ln03_production.sh
```

### 4. Xác nhận sau khi triển khai

- Thực hiện kiểm tra smoke test
- Xác nhận API hoạt động bình thường
- Thử import một file LN03 mẫu

## Khôi phục trong trường hợp có vấn đề

Nếu gặp vấn đề sau khi triển khai:

1. Khôi phục cơ sở dữ liệu từ bản sao lưu:
```sql
RESTORE DATABASE [TinhKhoanAppDb] FROM DISK = N'/path/to/backup.bak' WITH REPLACE
```

2. Triển khai lại phiên bản code trước đó

## Liên hệ hỗ trợ

Nếu cần hỗ trợ, vui lòng liên hệ:
- Email: it-support@agribank.com.vn
- Phone: XXXX-XXX-XXX
README_EOF

# Tạo file test sau khi triển khai
cat > $DEPLOY_DIR/test_ln03_deployment.sh << 'TEST_EOF'
#!/bin/bash

# Script kiểm tra triển khai LN03
# Tạo tự động vào: $(date +"%d/%m/%Y %H:%M:%S")

echo "=== Kiểm tra triển khai LN03 ==="

# Cấu hình API endpoint
API_BASE_URL="https://api.tinhkhoanapp.com"  # Thay thế bằng URL thực tế
API_TOKEN="YOUR_API_TOKEN"  # Thay thế bằng token thực tế

# Kiểm tra API hoạt động
echo "Kiểm tra kết nối API..."
curl -s -o /dev/null -w "%{http_code}" $API_BASE_URL/api/health

if [ $? -ne 0 ]; then
    echo "Không thể kết nối đến API. Kiểm tra lại kết nối mạng và trạng thái dịch vụ."
    exit 1
fi

# Kiểm tra endpoint LN03
echo "Kiểm tra endpoint LN03..."
curl -s -H "Authorization: Bearer $API_TOKEN" $API_BASE_URL/api/LN03/recent?count=1

if [ $? -ne 0 ]; then
    echo "Endpoint LN03 không phản hồi đúng. Kiểm tra logs để biết thêm chi tiết."
    exit 1
fi

# Thử upload file LN03 mẫu
echo "Thử upload file LN03 mẫu..."
curl -s -X POST -H "Authorization: Bearer $API_TOKEN" \
     -H "Content-Type: multipart/form-data" \
     -F "file=@./test_data/ln03_sample.csv" \
     $API_BASE_URL/api/LN03/import

if [ $? -ne 0 ]; then
    echo "Không thể upload file LN03 mẫu. Kiểm tra logs để biết thêm chi tiết."
    exit 1
fi

echo "=== Kiểm tra hoàn tất thành công! ==="
TEST_EOF

chmod +x $DEPLOY_DIR/test_ln03_deployment.sh

# Tạo thư mục cho dữ liệu test
mkdir -p $DEPLOY_DIR/test_data

# Tạo file LN03 mẫu để test
cat > $DEPLOY_DIR/test_data/ln03_sample.csv << 'CSV_EOF'
MACHINHANH,TENCHINHANH,MAKH,TENKH,SOHOPDONG,SOTIENXLRR,NGAYPHATSINHXL,THUNOSAUXL,CONLAINGOAIBANG,DUNONOIBANG,NHOMNO,MACBTD,TENCBTD,MAPGD,TAIKHOANHACHTOAN,REFNO,LOAINGUONVON
7800,CN Hà Nội,KH001,Nguyễn Văn A,HD001,1000000.50,2025-01-15,500000.25,500000.25,0,N1,CB001,Trần Văn B,PGD001,TK001,REF001,NV1
7800,CN Hà Nội,KH002,Nguyễn Thị C,HD002,2000000.75,2025-01-20,750000.50,1250000.25,0,N2,CB002,Lê Thị D,PGD002,TK002,REF002,NV2
7800,CN Hà Nội,KH003,Trần Văn E,HD003,3000000.25,2025-01-25,1000000.75,2000000.50,0,N3,CB003,Phạm Văn F,PGD003,TK003,REF003,NV3
CSV_EOF

echo "Tạo package triển khai..."
DEPLOY_PACKAGE="ln03_deployment_package_$TIMESTAMP.tar.gz"
tar -czf $DEPLOY_PACKAGE -C $DEPLOY_DIR .

echo "=== Triển khai code mới cho LN03 đã sẵn sàng ==="
echo "Package triển khai: $DEPLOY_PACKAGE"
echo ""
echo "Các bước tiếp theo:"
echo "1. Sao lưu dữ liệu bảng LN03 hiện tại bằng script: ./backup_ln03_data.sh"
echo "2. Thực hiện migration bằng script: ./run_ln03_migration.sh"
echo "3. Triển khai code mới lên môi trường staging bằng package: $DEPLOY_PACKAGE"
echo "4. Kiểm tra trên môi trường staging"
echo "5. Triển khai lên môi trường production khi mọi thứ hoạt động tốt"
