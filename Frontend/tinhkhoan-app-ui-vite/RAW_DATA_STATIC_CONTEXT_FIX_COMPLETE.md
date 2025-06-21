## KHO DỮ LIỆU THÔ (RAW DATA) - STATIC CONTEXT FIX COMPLETED

### ✅ Hoàn thành sửa lỗi build do static context

#### Vấn đề
- Lỗi build do sử dụng các phương thức và trường không phải static từ context static trong method `AddNewImportItem`
- Cụ thể là các phương thức `ExtractStatementDate`, `IsArchiveFile` và trường `_logger` được gọi từ static method

#### Giải pháp
1. Tạo phiên bản static của các phương thức sử dụng:
   - `ExtractStatementDateStatic`: Phiên bản static của `ExtractStatementDate`
   - `IsArchiveFileStatic`: Phiên bản static của `IsArchiveFile`
   
2. Thêm static logger để sử dụng trong static method:
   ```csharp
   private static readonly ILogger _staticLogger = LoggerFactory.Create(builder => 
       builder.AddConsole()).CreateLogger("RawDataControllerStatic");
   ```

3. Cập nhật method `AddNewImportItem` để sử dụng các static method và logger

4. Cập nhật các điểm gọi `IsArchiveFile` và xử lý null safety cho các parameters

#### Lợi ích
- Sửa triệt để lỗi build do static context
- Duy trì tính nhất quán trong code
- Đảm bảo xử lý null safety đúng cách cho các parameters khi gọi `ProcessArchiveFile` và `ProcessSingleFile`

#### Kiểm thử
- Đã build thành công backend
- Các chức năng xóa dữ liệu import và import dữ liệu mới vẫn hoạt động như mong đợi
- Sau khi import file, dữ liệu mới sẽ được thêm vào mock data và hiển thị trong danh sách
- Sau khi xóa dữ liệu import, item đã xóa sẽ không xuất hiện trong danh sách trả về

### 🔄 Các tính năng Raw Data đã hoàn thiện
1. Cải thiện quản lý mock data để đảm bảo:
   - Các item mới import sẽ được thêm vào danh sách hiển thị
   - Các item đã xóa sẽ không xuất hiện trong danh sách trả về
   - Người dùng nhìn thấy UI/UX nhất quán khi xóa và import dữ liệu

2. Tất cả các endpoint luôn trả về response hợp lệ:
   - `/api/rawdata` - Lấy danh sách item không bị xóa
   - `/api/rawdata/{id}` - Xóa item theo ID và mark đã xóa
   - `/api/rawdata/import/{dataType}` - Import và thêm vào mock data

3. Build thành công không có lỗi, chỉ còn warnings không liên quan

### 📋 Các hành động tiếp theo
1. Xem xét refactor lại các warning còn lại
2. Xem xét chuyển đổi hoàn toàn sang temporal table khi schema đã đồng bộ
3. Xem xét thêm unit test cho các chức năng này

--------------
Hoàn thành: 2025-06-22
