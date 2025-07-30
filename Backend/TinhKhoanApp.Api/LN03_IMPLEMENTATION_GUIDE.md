# Hướng dẫn triển khai LN03 (Dữ liệu phục hồi khoản vay)

## Tổng quan về các thay đổi

Đã thực hiện các thay đổi sau để đảm bảo LN03 tuân thủ đầy đủ các yêu cầu:

1. **Cập nhật mô hình LN03**:
   - Thay đổi kiểu dữ liệu cho các trường số (SOTIENXLRR, THUNOSAUXL, CONLAINGOAIBANG, DUNONOIBANG) từ string sang decimal(18,2)
   - Thay đổi kiểu dữ liệu cho trường ngày (NGAYPHATSINHXL) từ string sang datetime2
   - Thêm XML comments đầy đủ cho tất cả các thuộc tính

2. **Cải tiến quá trình xử lý import**:
   - Tạo phương thức import mới `ImportLN03EnhancedAsync` để xử lý chính xác định dạng số và ngày
   - Thêm phương thức parse CSV đặc biệt cho LN03 (`ParseLN03EnhancedAsync`) để xử lý chính xác chuyển đổi kiểu dữ liệu
   - Sử dụng `ConvertCsvValue` hiện có để chuyển đổi dữ liệu một cách chính xác

3. **Cập nhật LN03Controller**:
   - Kích hoạt lại controller đã bị vô hiệu hóa trước đó
   - Cập nhật endpoint import để sử dụng phương thức import cải tiến
   - Thêm XML comments đầy đủ cho controller và các phương thức

4. **Cập nhật cấu trúc cơ sở dữ liệu**:
   - Tạo script SQL để cập nhật kiểu dữ liệu các cột trong bảng LN03
   - Duy trì cấu hình temporal table và columnstore index

## Hướng dẫn triển khai

### Bước 1: Xây dựng lại ứng dụng

```bash
dotnet build
```

### Bước 2: Cập nhật cơ sở dữ liệu

```bash
# Chạy script cập nhật kiểu dữ liệu
./run_ln03_migration.sh
```

### Bước 3: Kiểm tra quá trình import

Sử dụng Swagger hoặc Postman để kiểm tra API endpoint:
- `POST /api/LN03/import`

Kiểm tra xem dữ liệu được nhập có chính xác không, đặc biệt là các trường số và ngày tháng.

## Lưu ý quan trọng

1. **Dữ liệu hiện có**: Dữ liệu hiện có sẽ không bị ảnh hưởng bởi thay đổi này.

2. **Temporal table**: Bảng LN03 vẫn duy trì cấu hình temporal table và có thể truy vấn lịch sử dữ liệu.

3. **Columnstore index**: Đã tạo lại columnstore index sau khi thay đổi kiểu dữ liệu để đảm bảo hiệu suất.

4. **Xử lý lỗi**: Quá trình import đã được cải tiến để xử lý nhiều định dạng số và ngày tháng, nhưng vẫn nên kiểm tra dữ liệu nguồn để đảm bảo chất lượng.

5. **Logs**: Quá trình import ghi nhận log chi tiết vào console, có thể sử dụng để theo dõi và gỡ lỗi.

## Xác minh triển khai

Để xác minh việc triển khai thành công:

1. Kiểm tra cấu trúc bảng trong cơ sở dữ liệu:
```sql
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'LN03'
ORDER BY ORDINAL_POSITION;
```

2. Kiểm tra xem temporal table đã được kích hoạt:
```sql
SELECT name, temporal_type_desc 
FROM sys.tables 
WHERE name = 'LN03';
```

3. Kiểm tra xem columnstore index đã được tạo:
```sql
SELECT name, type_desc 
FROM sys.indexes 
WHERE object_id = OBJECT_ID('LN03') 
AND type = 5; -- Type 5 là Columnstore
```

4. Import file CSV mẫu và kiểm tra dữ liệu:
```sql
SELECT TOP 10 * FROM LN03 ORDER BY CREATED_DATE DESC;
```
