# Trình tự triển khai cập nhật kiểu dữ liệu cho bảng RR01

## A. Chuẩn bị trước triển khai

### 1. Sao lưu dữ liệu
```sql
-- Tạo bảng sao lưu dữ liệu
SELECT * INTO RR01_Backup_$(date +%Y%m%d) FROM RR01;
```

### 2. Xác minh tính nhất quán của mã nguồn
- Kiểm tra class model `RR01.cs` đã được cập nhật
- Kiểm tra class DTO `RR01DTO.cs` đã được cập nhật
- Biên dịch dự án để đảm bảo không có lỗi

### 3. Tạo môi trường kiểm thử
- Chuẩn bị tập dữ liệu mẫu `sample_rr01_test.csv` để kiểm tra import
- Sao chép kịch bản kiểm thử `TestRR01DataTypes.cs` vào thư mục test

### 4. Thông báo cho người dùng
- Gửi thông báo cho người dùng về thời gian bảo trì
- Đảm bảo không có người dùng đang import dữ liệu RR01

## B. Quy trình triển khai

### 1. Cập nhật cơ sở dữ liệu
```sql
-- Tắt tạm thời tính năng temporal table
ALTER TABLE RR01 SET (SYSTEM_VERSIONING = OFF);
ALTER TABLE RR01 DROP PERIOD FOR SYSTEM_TIME;

-- Cập nhật kiểu dữ liệu cho các cột số
ALTER TABLE RR01 ALTER COLUMN SO_LDS decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_GOC_BAN_DAU decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_LAI_TICHLUY_BD decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DOC_DAUKY_DA_THU_HT decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_GOC_HIENTAI decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_LAI_HIENTAI decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_NGAN_HAN decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_TRUNG_HAN decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_DAI_HAN decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN THU_GOC decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN THU_LAI decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN BDS decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DS decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN TSK decimal(18,2) NULL;

-- Cập nhật kiểu dữ liệu cho các cột ngày
ALTER TABLE RR01 ALTER COLUMN NGAY_GIAI_NGAN datetime2 NULL;
ALTER TABLE RR01 ALTER COLUMN NGAY_DEN_HAN datetime2 NULL;
ALTER TABLE RR01 ALTER COLUMN NGAY_XLRR datetime2 NULL;

-- Bật lại tính năng temporal table
ALTER TABLE RR01 ADD PERIOD FOR SYSTEM_TIME(ValidFrom, ValidTo);
ALTER TABLE RR01 SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.RR01History));
```

### 2. Triển khai mã nguồn
- Sao chép các file đã cập nhật vào môi trường sản xuất:
  - `RR01.cs`
  - `RR01DTO.cs`
- Biên dịch và triển khai lại ứng dụng

### 3. Kiểm tra tính năng import
- Sử dụng tập dữ liệu mẫu để kiểm tra import
- Xác minh dữ liệu được import chính xác với kiểu dữ liệu mới

## C. Kiểm tra sau triển khai

### 1. Kiểm tra tính toàn vẹn dữ liệu
```sql
-- Kiểm tra số lượng bản ghi
SELECT COUNT(*) FROM RR01;
SELECT COUNT(*) FROM RR01_Backup_$(date +%Y%m%d);

-- Kiểm tra kiểu dữ liệu
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'RR01';

-- Kiểm tra tính nhất quán dữ liệu
SELECT TOP 100 * FROM RR01 ORDER BY NGAY_DL DESC;
```

### 2. Kiểm tra tính năng của ứng dụng
- Thực hiện import dữ liệu RR01 từ file CSV
- Kiểm tra API trả về dữ liệu chính xác
- Kiểm tra các tính năng liên quan đến RR01

### 3. Kiểm tra hiệu năng
- So sánh thời gian truy vấn trước và sau khi thay đổi
- Theo dõi sử dụng CPU và bộ nhớ của ứng dụng

## D. Xử lý sự cố

### 1. Kịch bản khôi phục
```sql
-- Khôi phục dữ liệu từ bảng backup
TRUNCATE TABLE RR01;
INSERT INTO RR01 SELECT * FROM RR01_Backup_$(date +%Y%m%d);
```

### 2. Xử lý lỗi chuyển đổi dữ liệu
- Nếu phát hiện lỗi chuyển đổi, xác định các bản ghi có vấn đề:
```sql
-- Tìm các bản ghi có dữ liệu không thể chuyển đổi
SELECT * FROM RR01_Backup_$(date +%Y%m%d)
WHERE ISDATE(NGAY_GIAI_NGAN) = 0 AND NGAY_GIAI_NGAN IS NOT NULL;

SELECT * FROM RR01_Backup_$(date +%Y%m%d)
WHERE ISNUMERIC(SO_LDS) = 0 AND SO_LDS IS NOT NULL;
```

### 3. Xử lý lỗi import
- Kiểm tra format của file CSV
- Kiểm tra phương thức `ConvertCsvValue` trong `DirectImportService.cs`
- Xem log lỗi import và xử lý từng trường hợp cụ thể

## E. Xác nhận hoàn thành

### 1. Tiêu chí xác nhận
- Tất cả kiểu dữ liệu đã được cập nhật chính xác
- Dữ liệu được bảo toàn sau khi chuyển đổi
- Tính năng import hoạt động bình thường
- Không có lỗi được báo cáo từ người dùng

### 2. Tài liệu hóa
- Cập nhật tài liệu về cấu trúc cơ sở dữ liệu
- Ghi chép các vấn đề gặp phải và giải pháp
- Lưu trữ kịch bản SQL đã sử dụng

### 3. Thông báo hoàn thành
- Thông báo cho người dùng về việc hoàn thành cập nhật
- Hướng dẫn người dùng về các thay đổi (nếu cần)

## F. Theo dõi sau triển khai

### 1. Giám sát hệ thống
- Theo dõi log ứng dụng trong 48 giờ đầu
- Giám sát hiệu năng cơ sở dữ liệu
- Theo dõi báo cáo lỗi từ người dùng

### 2. Đánh giá hiệu quả
- So sánh hiệu năng trước và sau khi cập nhật
- Đánh giá tác động đến tính năng khác của hệ thống
- Tổng hợp báo cáo về quá trình triển khai
