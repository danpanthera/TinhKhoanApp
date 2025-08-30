# Các bước triển khai thay đổi LN03

## 1. Tổng quan

Tài liệu này mô tả các bước cần thực hiện để triển khai thay đổi mô hình dữ liệu LN03, chuyển từ sử dụng kiểu `string` sang kiểu dữ liệu thích hợp (`decimal?` cho các trường số tiền, `DateTime?` cho các trường ngày tháng).

## 2. Chuẩn bị

### 2.1 Sao lưu dữ liệu

```bash
# Thực hiện backup dữ liệu trước khi thay đổi
./backup_ln03_data.sh
```

### 2.2 Kiểm tra môi trường

- Xác nhận phiên bản .NET Core/SQL Server
- Kiểm tra quyền truy cập cơ sở dữ liệu
- Xác nhận không có người dùng đang sử dụng hệ thống (nếu có thể)

## 3. Triển khai

### 3.1 Triển khai thay đổi cơ sở dữ liệu

```bash
# Thực hiện migration cơ sở dữ liệu
./run_ln03_migration.sh
```

Script sẽ thực hiện:

1. Thêm cột mới với kiểu dữ liệu mới
2. Chuyển đổi dữ liệu từ cột cũ sang cột mới
3. Xóa cột cũ
4. Đổi tên cột mới thành tên cột cũ
5. Cập nhật các bảng liên quan nếu cần

### 3.2 Triển khai thay đổi code

```bash
# Triển khai code mới
./deploy_ln03_changes.sh
```

Các thay đổi bao gồm:

1. Cập nhật model LN03.cs với kiểu dữ liệu mới
2. Cập nhật DirectImportServiceLN03Extension.cs để xử lý chuyển đổi kiểu dữ liệu
3. Bật lại controller LN03 đã bị tắt trước đó

## 4. Kiểm tra sau triển khai

### 4.1 Kiểm tra tự động

```bash
# Kiểm tra thay đổi sau triển khai
./verify_ln03_changes.sh
```

### 4.2 Kiểm tra import CSV

```bash
# Kiểm tra quá trình import CSV
cd test_data
./run_ln03_tests.sh
```

### 4.3 Kiểm tra thủ công

1. Kiểm tra API trả về dữ liệu với kiểu đúng
2. Kiểm tra khả năng thêm/sửa/xóa dữ liệu
3. Kiểm tra hiển thị dữ liệu trên giao diện

## 5. Xử lý sự cố

### 5.1 Phục hồi dữ liệu

Nếu gặp sự cố:

```bash
# Phục hồi dữ liệu từ bản sao lưu
./restore_ln03_backup.sh
```

### 5.2 Hồi phục code

Nếu cần hồi phục code:

```bash
# Quay lại phiên bản trước đó
git checkout [previous-commit-id]
```

## 6. Theo dõi

Theo dõi hệ thống sau khi triển khai:

- Kiểm tra log lỗi
- Giám sát hiệu suất import
- Xác nhận với người dùng

## 7. Tài liệu liên quan

- [README.md](./test_data/README.md) - Hướng dẫn kiểm tra import CSV
- [fix_ln03_data_types.sql](./Migrations/fix_ln03_data_types.sql) - Script SQL migration
- [backup_ln03_data.sh](./backup_ln03_data.sh) - Script sao lưu dữ liệu
- [run_ln03_migration.sh](./run_ln03_migration.sh) - Script thực hiện migration
- [deploy_ln03_changes.sh](./deploy_ln03_changes.sh) - Script triển khai code

## 8. Người thực hiện

- Người phát triển: [Tên nhà phát triển]
- Người kiểm tra: [Tên người kiểm tra]
- Người phê duyệt: [Tên người phê duyệt]

## 9. Lịch sử thay đổi

| Ngày   | Phiên bản | Mô tả             | Người thực hiện |
| ------ | --------- | ----------------- | --------------- |
| [Ngày] | 1.0       | Phiên bản ban đầu | [Tên]           |
