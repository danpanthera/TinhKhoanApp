# Hướng dẫn kiểm tra import CSV cho LN03

Thư mục này chứa các file và script để kiểm tra quá trình import CSV cho LN03 sau khi thực hiện 
các thay đổi về mô hình dữ liệu.

## Các file test

1. `ln03_test_sample.csv` - File CSV mẫu với dữ liệu hợp lệ
2. `ln03_test_invalid.csv` - File CSV mẫu với dữ liệu không hợp lệ để kiểm tra xử lý lỗi
3. `ln03_test_header_only.csv` - File CSV chỉ có header để kiểm tra trường hợp đặc biệt
4. `run_ln03_tests.sh` - Script tự động hóa kiểm tra import

## Cách sử dụng

### Kiểm tra thủ công

1. Đảm bảo API đang chạy trên môi trường phát triển hoặc staging
2. Sử dụng Postman hoặc công cụ tương tự để import các file CSV mẫu
3. Kiểm tra kết quả trả về và dữ liệu trong cơ sở dữ liệu

### Kiểm tra tự động

1. Chỉnh sửa thông tin kết nối API trong file `run_ln03_tests.sh`
2. Chạy script kiểm tra:

```bash
cd test_data
./run_ln03_tests.sh
```

## Kịch bản kiểm tra

Script kiểm tra sẽ thực hiện các kịch bản sau:

1. **Test case 1**: Import file hợp lệ (ln03_test_sample.csv)
   - Kỳ vọng: Import thành công với các kiểu dữ liệu đúng

2. **Test case 2**: Import file không hợp lệ (ln03_test_invalid.csv)
   - Kỳ vọng: API xử lý đúng dữ liệu không hợp lệ (bỏ qua hoặc hiển thị lỗi)

3. **Test case 3**: Import file chỉ có header (ln03_test_header_only.csv)
   - Kỳ vọng: API xử lý đúng file chỉ có header (không có bản ghi nào được import)

4. **Test case 4**: Kiểm tra dữ liệu đã import
   - Kỳ vọng: Dữ liệu được hiển thị với kiểu dữ liệu đúng (decimal, datetime)

## Kiểm tra thủ công bổ sung

Ngoài các kiểm tra tự động, hãy thực hiện thêm các kiểm tra thủ công sau:

1. Kiểm tra API response với các trường decimal và datetime
2. Kiểm tra trực tiếp trong cơ sở dữ liệu các kiểu dữ liệu của bảng LN03
3. Thử thêm/sửa/xóa các bản ghi LN03 thông qua API

## Vấn đề đã biết

- Định dạng ngày tháng trong file CSV cần phải là yyyy-MM-dd
- Định dạng số trong file CSV không nên có dấu phân cách hàng nghìn
