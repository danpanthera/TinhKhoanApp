# Hướng dẫn triển khai cập nhật kiểu dữ liệu RR01

## Chuẩn bị
1. Tạo bản backup cơ sở dữ liệu trước khi thực hiện thay đổi
2. Đảm bảo đã cập nhật mã nguồn với kiểu dữ liệu mới (Model và DTO)
3. Chuẩn bị script SQL để cập nhật cấu trúc bảng

## Các bước thực hiện

### 1. Cập nhật cấu trúc cơ sở dữ liệu
```sql
-- Thực hiện script update_rr01_datatypes.sql trong SQL Server Management Studio
```

### 2. Kiểm thử import dữ liệu
1. Tạo một tập tin CSV mẫu để kiểm thử (đã có sample_rr01_test.csv)
2. Sử dụng API hoặc giao diện import để nhập liệu
3. Kiểm tra log để xác nhận không có lỗi trong quá trình chuyển đổi kiểu dữ liệu

### 3. Xác minh dữ liệu đã import
1. Thực hiện script verify_rr01_imported_data.sql để kiểm tra dữ liệu
2. Xác nhận các giá trị số và ngày tháng được lưu trữ chính xác
3. Xác nhận có thể thực hiện các phép tính trên dữ liệu số

### 4. Kiểm thử toàn diện
1. Kiểm tra các chức năng khác liên quan đến RR01
2. Kiểm tra báo cáo và phân tích dữ liệu RR01
3. Xác nhận không có lỗi khi hiển thị, xuất báo cáo

## Xử lý sự cố
Nếu gặp vấn đề trong quá trình triển khai, có thể:
1. Khôi phục cơ sở dữ liệu từ bản backup
2. Kiểm tra logs để xác định lỗi
3. Xem xét chạy một phần script (chỉ cập nhật một số cột)

## Xác nhận hoàn tất
Sau khi hoàn tất triển khai, kiểm tra:
1. Cấu trúc bảng RR01 đã được cập nhật
2. Dữ liệu đã được chuyển đổi đúng kiểu
3. Các chức năng liên quan hoạt động bình thường
