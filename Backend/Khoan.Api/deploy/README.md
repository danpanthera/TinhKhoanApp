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
