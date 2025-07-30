# Câu hỏi thường gặp khi cập nhật kiểu dữ liệu bảng RR01

## Câu hỏi chung

### 1. Tại sao cần thay đổi kiểu dữ liệu của bảng RR01?
Việc thay đổi kiểu dữ liệu nhằm đảm bảo tính nhất quán và chính xác của dữ liệu. Khi sử dụng kiểu dữ liệu đúng (decimal cho số và datetime2 cho ngày), chúng ta có thể:
- Thực hiện các phép tính số học chính xác
- So sánh và tìm kiếm dữ liệu ngày tháng hiệu quả hơn
- Tăng hiệu năng truy vấn và tính toán
- Giảm khả năng xảy ra lỗi khi chuyển đổi kiểu dữ liệu

### 2. Thay đổi này sẽ ảnh hưởng đến dữ liệu hiện có không?
Nếu áp dụng đúng quy trình, dữ liệu hiện có sẽ được bảo toàn. Tuy nhiên, có thể xảy ra lỗi nếu dữ liệu hiện có không thể chuyển đổi sang kiểu dữ liệu mới (ví dụ: chuỗi không phải số hợp lệ hoặc ngày không đúng định dạng).

### 3. Liệu có cần dừng hệ thống trong quá trình cập nhật không?
Có, việc cập nhật cơ sở dữ liệu đòi hỏi phải tạm dừng các hoạt động liên quan đến bảng RR01, đặc biệt là import dữ liệu. Tuy nhiên, các chức năng khác của hệ thống vẫn có thể hoạt động bình thường.

## Câu hỏi kỹ thuật

### 4. Temporal Table sẽ bị ảnh hưởng như thế nào?
Quá trình cập nhật đòi hỏi phải tắt tạm thời tính năng temporal table, thực hiện thay đổi kiểu dữ liệu, sau đó bật lại tính năng này. Lịch sử thay đổi trước đó vẫn được bảo toàn, nhưng cần lưu ý rằng việc thay đổi kiểu dữ liệu sẽ không được ghi lại trong lịch sử thay đổi.

### 5. Việc thay đổi kiểu dữ liệu có ảnh hưởng đến Columnstore Index không?
Có, Columnstore Index sẽ cần được tái tạo sau khi thay đổi kiểu dữ liệu. Việc này được thực hiện tự động khi bật lại tính năng temporal table, nhưng có thể ảnh hưởng đến hiệu năng trong quá trình tái tạo.

### 6. Làm thế nào để xử lý dữ liệu không thể chuyển đổi?
Trước khi thực hiện thay đổi, cần kiểm tra và xử lý dữ liệu không thể chuyển đổi:
```sql
-- Kiểm tra các giá trị không phải số
SELECT * FROM RR01 WHERE ISNUMERIC(SO_LDS) = 0 AND SO_LDS IS NOT NULL;

-- Kiểm tra các giá trị ngày không hợp lệ
SELECT * FROM RR01 WHERE ISDATE(NGAY_GIAI_NGAN) = 0 AND NGAY_GIAI_NGAN IS NOT NULL;
```

Nếu phát hiện dữ liệu không hợp lệ, cần xử lý bằng cách:
- Sửa lại giá trị để đúng định dạng
- Đặt giá trị NULL nếu không thể xác định giá trị đúng
- Ghi lại các trường hợp đặc biệt để theo dõi

## Câu hỏi về import dữ liệu

### 7. Việc import dữ liệu từ file CSV sẽ thay đổi như thế nào?
Phương thức import dữ liệu không cần thay đổi nhiều, vì phương thức `ConvertCsvValue` đã hỗ trợ chuyển đổi sang kiểu decimal và DateTime. Tuy nhiên, cần kiểm tra kỹ file CSV đầu vào để đảm bảo dữ liệu có thể chuyển đổi chính xác.

### 8. Định dạng nào được chấp nhận cho các cột số và ngày trong file CSV?
- Cột số: Có thể sử dụng định dạng số thông thường (123456.78) hoặc định dạng với dấu phân cách hàng nghìn (123,456.78)
- Cột ngày: Nên sử dụng định dạng ISO (YYYY-MM-DD), nhưng các định dạng khác như DD/MM/YYYY hoặc MM/DD/YYYY cũng được hỗ trợ

### 9. Có cần thay đổi cấu trúc file CSV mẫu không?
Không cần thay đổi cấu trúc file CSV. Các cột vẫn giữ nguyên thứ tự và tên, chỉ có kiểu dữ liệu ở phía cơ sở dữ liệu thay đổi.

## Câu hỏi về phát triển

### 10. Làm thế nào để truy cập và sử dụng các cột số trong code?
Sau khi thay đổi, các cột số có thể được sử dụng trực tiếp trong các phép tính:
```csharp
// Trước khi thay đổi
decimal soLds = decimal.Parse(rr01.SO_LDS ?? "0");
decimal tongDuno = decimal.Parse(rr01.DUNO_NGAN_HAN ?? "0") 
    + decimal.Parse(rr01.DUNO_TRUNG_HAN ?? "0") 
    + decimal.Parse(rr01.DUNO_DAI_HAN ?? "0");

// Sau khi thay đổi
decimal soLds = rr01.SO_LDS ?? 0;
decimal tongDuno = (rr01.DUNO_NGAN_HAN ?? 0) 
    + (rr01.DUNO_TRUNG_HAN ?? 0) 
    + (rr01.DUNO_DAI_HAN ?? 0);
```

### 11. Làm thế nào để truy cập và sử dụng các cột ngày trong code?
Sau khi thay đổi, các cột ngày có thể được sử dụng trực tiếp trong các phép so sánh và tính toán ngày:
```csharp
// Trước khi thay đổi
DateTime ngayGiaiNgan = DateTime.Parse(rr01.NGAY_GIAI_NGAN ?? DateTime.MinValue.ToString());
int soNgayQuaHan = (DateTime.Now - DateTime.Parse(rr01.NGAY_DEN_HAN ?? DateTime.Now.ToString())).Days;

// Sau khi thay đổi
DateTime ngayGiaiNgan = rr01.NGAY_GIAI_NGAN ?? DateTime.MinValue;
int soNgayQuaHan = (DateTime.Now - (rr01.NGAY_DEN_HAN ?? DateTime.Now)).Days;
```

### 12. Có cần cập nhật API để phản ánh thay đổi kiểu dữ liệu không?
API không cần thay đổi về cấu trúc hoặc endpoint, nhưng các DTO đã được cập nhật để sử dụng kiểu dữ liệu mới. Nếu có code phía client dựa vào kiểu dữ liệu cụ thể (như xử lý chuỗi), có thể cần cập nhật.

## Câu hỏi về vận hành

### 13. Có kế hoạch dự phòng nếu cập nhật gặp vấn đề không?
Có, chúng ta có kế hoạch dự phòng:
- Sao lưu đầy đủ dữ liệu trước khi thực hiện thay đổi
- Chuẩn bị kịch bản khôi phục để quay lại trạng thái trước khi thay đổi
- Giám sát hệ thống sau khi cập nhật để phát hiện sớm các vấn đề

### 14. Làm thế nào để xác nhận cập nhật đã thành công?
Các bước kiểm tra sau khi cập nhật:
- Kiểm tra kiểu dữ liệu của các cột đã thay đổi
- Xác minh tính toàn vẹn dữ liệu bằng cách so sánh số lượng bản ghi
- Thực hiện import dữ liệu mẫu và kiểm tra kết quả
- Kiểm tra các tính năng liên quan đến RR01 (truy vấn, báo cáo, v.v.)

### 15. Ai cần biết về thay đổi này?
Những người cần được thông báo về thay đổi này bao gồm:
- Đội phát triển (để cập nhật code nếu cần)
- Người quản trị cơ sở dữ liệu (để hỗ trợ quá trình cập nhật)
- Người dùng cuối (để biết về thời gian bảo trì và các thay đổi có thể ảnh hưởng)
- Đội hỗ trợ (để xử lý các câu hỏi hoặc vấn đề phát sinh)
