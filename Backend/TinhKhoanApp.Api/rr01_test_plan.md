# Kế hoạch kiểm thử RR01 sau khi cập nhật kiểu dữ liệu

## 1. Kiểm thử đơn vị

### 1.1. Kiểm thử model và DTO
- Xác nhận model `RR01` có các kiểu dữ liệu đúng
- Xác nhận DTO `RR01DTO` có các kiểu dữ liệu đúng
- Xác nhận phương thức chuyển đổi `FromEntity` hoạt động chính xác

### 1.2. Kiểm thử hàm chuyển đổi kiểu dữ liệu
- Xác nhận hàm `ConvertCsvValue` xử lý đúng kiểu `decimal`
- Xác nhận hàm `ConvertCsvValue` xử lý đúng kiểu `DateTime`
- Thử nghiệm với các định dạng số và ngày tháng khác nhau

### 1.3. Kiểm thử phương thức parse CSV
- Xác nhận phương thức `ParseRR01SpecialFormatAsync` hoạt động chính xác
- Thử nghiệm với các định dạng CSV khác nhau
- Kiểm tra xử lý lỗi khi dữ liệu không hợp lệ

## 2. Kiểm thử tích hợp

### 2.1. Kiểm thử import dữ liệu
- Import tập tin CSV mẫu thông qua API
- Xác nhận dữ liệu được lưu với kiểu dữ liệu đúng
- Kiểm tra log để đảm bảo không có lỗi

### 2.2. Kiểm thử truy vấn và tính toán
- Thực hiện các truy vấn có phép tính số học
- Thực hiện các truy vấn có điều kiện về ngày tháng
- Xác nhận kết quả truy vấn chính xác

### 2.3. Kiểm thử các API liên quan
- Thử nghiệm API lấy dữ liệu RR01
- Thử nghiệm API cập nhật dữ liệu RR01
- Thử nghiệm API xóa dữ liệu RR01

## 3. Kiểm thử hiệu năng

### 3.1. Kiểm thử tốc độ import
- So sánh tốc độ import trước và sau khi thay đổi
- Đo thời gian import với các kích thước tập tin khác nhau
- Đo thời gian chuyển đổi kiểu dữ liệu

### 3.2. Kiểm thử tốc độ truy vấn
- So sánh tốc độ truy vấn trước và sau khi thay đổi
- Đo thời gian thực hiện các truy vấn phức tạp
- Xác nhận index hoạt động hiệu quả

### 3.3. Kiểm thử tốc độ báo cáo
- Đo thời gian tạo báo cáo liên quan đến RR01
- So sánh thời gian xử lý báo cáo trước và sau khi thay đổi
- Xác nhận hiệu năng không bị giảm sút

## 4. Kiểm thử chấp nhận

### 4.1. Xác nhận chức năng cốt lõi
- Kiểm tra tất cả các chức năng liên quan đến RR01
- Xác nhận không có lỗi nào sau khi thay đổi
- Xác nhận tất cả các yêu cầu nghiệp vụ vẫn được đáp ứng

### 4.2. Xác nhận tính nhất quán dữ liệu
- Kiểm tra tính nhất quán của dữ liệu sau khi chuyển đổi
- So sánh số lượng bản ghi trước và sau khi thay đổi
- Kiểm tra giá trị dữ liệu có bị thay đổi không mong muốn

### 4.3. Xác nhận khả năng tương thích
- Kiểm tra khả năng tương thích với các ứng dụng khác
- Kiểm tra khả năng tương thích với các báo cáo hiện có
- Kiểm tra khả năng tương thích với các API bên ngoài

## 5. Kịch bản kiểm thử cụ thể

### 5.1. Kịch bản import dữ liệu
1. Import tập tin `sample_rr01_test.csv`
2. Kiểm tra dữ liệu trong database
3. Xác nhận kiểu dữ liệu và giá trị đúng

### 5.2. Kịch bản truy vấn dữ liệu
1. Thực hiện truy vấn lấy tất cả bản ghi RR01
2. Thực hiện truy vấn có điều kiện về số tiền
3. Thực hiện truy vấn có điều kiện về ngày tháng
4. Thực hiện truy vấn có phép tính số học

### 5.3. Kịch bản xuất báo cáo
1. Tạo báo cáo tổng hợp RR01 theo kỳ hạn
2. Tạo báo cáo phân tích nợ quá hạn
3. Tạo báo cáo so sánh các giai đoạn

## 6. Báo cáo kết quả kiểm thử

### 6.1. Mẫu báo cáo lỗi
- Mô tả lỗi: [mô tả chi tiết]
- Bước tái hiện: [các bước để tái hiện lỗi]
- Mức độ nghiêm trọng: [thấp/trung bình/cao/nghiêm trọng]
- Tác động: [tác động của lỗi]
- Giải pháp đề xuất: [giải pháp]

### 6.2. Mẫu báo cáo kiểm thử
- Tên kiểm thử: [tên]
- Mô tả: [mô tả]
- Kết quả mong đợi: [kết quả mong đợi]
- Kết quả thực tế: [kết quả thực tế]
- Trạng thái: [đạt/không đạt]
- Ghi chú: [ghi chú bổ sung]

### 6.3. Danh sách kiểm tra hoàn tất
- [ ] Tất cả kiểm thử đơn vị đã được thực hiện và đạt yêu cầu
- [ ] Tất cả kiểm thử tích hợp đã được thực hiện và đạt yêu cầu
- [ ] Tất cả kiểm thử hiệu năng đã được thực hiện và đạt yêu cầu
- [ ] Tất cả kiểm thử chấp nhận đã được thực hiện và đạt yêu cầu
- [ ] Tất cả lỗi đã được sửa và kiểm tra lại
- [ ] Tài liệu đã được cập nhật
