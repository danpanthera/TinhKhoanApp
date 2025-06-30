# Tóm tắt chuẩn hóa logic tính toán KPI lũy kế

## 📋 Mục tiêu đã hoàn thành

Đã chuẩn hóa thành công logic tính toán các chỉ tiêu KPI lũy kế trên Dashboard/2 (Cập nhật tình hình thực hiện) theo đúng ngày dữ liệu thực tế.

## 🎯 Nguyên tắc chuẩn hóa

### Logic chọn file dữ liệu theo tháng/năm:
- **Tháng 4/2025**: chỉ lấy file ngày **20250430**
- **Tháng 12/2024 hoặc năm 2024**: chỉ lấy file ngày **20241231**  
- **Các tháng/năm khác**: báo lỗi "Chưa có dữ liệu cho tháng/năm này"

### Quy trình tính toán chuẩn:
1. Xác định ngày file cần lấy bằng `GetTargetStatementDate(DateTime)`
2. Nếu không có dữ liệu cho tháng/năm đó → báo lỗi bằng `GetDataNotAvailableMessage(DateTime)`
3. Tìm file import có `StatementDate` chính xác
4. Chỉ lấy dữ liệu thuộc chi nhánh được chọn (`BRANCH_CODE`)
5. Không cộng dồn nhiều file - chỉ dùng 1 file đúng ngày

## ✅ Các chỉ tiêu đã chuẩn hóa

### 1. **Nguồn vốn** (`CalculateNguonVon`)
- **File dữ liệu**: DP01
- **Công thức**: Tổng `CURRENT_BALANCE` - loại trừ TK(2,40,41,427)
- **Đơn vị**: Triệu VND
- **Status**: ✅ Đã hoàn thành

### 2. **Dư nợ** (`CalculateDuNo`)  
- **File dữ liệu**: LN01
- **Công thức**: Tổng `DISBURSEMENT_AMOUNT` theo chi nhánh + breakdown nhóm nợ
- **Đơn vị**: Triệu VND
- **Status**: ✅ Đã hoàn thành

### 3. **Tỷ lệ nợ xấu** (`CalculateTyLeNoXau`)
- **File dữ liệu**: LN01  
- **Công thức**: (Nhóm nợ 3,4,5 / Tổng dư nợ) × 100%
- **Đơn vị**: Phần trăm (%)
- **Status**: ✅ Đã hoàn thành

### 4. **Thu hồi XLRR** (`CalculateThuHoiXLRR`)
- **File dữ liệu**: LN01
- **Công thức**: Tổng `RECOVERED_AMOUNT` từ nợ đã xử lý rủi ro
- **Đơn vị**: Triệu VND  
- **Status**: ✅ Đã hoàn thành

### 5. **Lợi nhuận** (`CalculateLoiNhuan`)
- **File dữ liệu**: GLCB41
- **Công thức**: (TK 7+790001+8511) - (TK 8+882)
- **Đơn vị**: Triệu VND
- **Status**: ✅ Đã hoàn thành

### 6. **Thu dịch vụ** (`CalculateThuDichVu`)
- **File dữ liệu**: Chờ xác định
- **Công thức**: Chờ công thức cụ thể
- **Đơn vị**: Triệu VND
- **Status**: ⏳ Chờ công thức chi tiết

## 🔧 Helper Methods đã tạo

### `GetTargetStatementDate(DateTime selectedDate)`
```csharp
private DateTime? GetTargetStatementDate(DateTime selectedDate)
{
    // Chỉ hỗ trợ tháng 4/2025 và 12/2024
    if (selectedDate.Year == 2025 && selectedDate.Month == 4)
        return new DateTime(2025, 4, 30);
    
    if (selectedDate.Year == 2024 && selectedDate.Month == 12)
        return new DateTime(2024, 12, 31);
        
    return null; // Các tháng khác chưa có dữ liệu
}
```

### `GetDataNotAvailableMessage(DateTime selectedDate)`
```csharp
private string GetDataNotAvailableMessage(DateTime selectedDate)
{
    return $"Chưa có dữ liệu cho tháng {selectedDate.Month}/{selectedDate.Year}. " +
           "Hiện tại chỉ hỗ trợ dữ liệu tháng 4/2025 và 12/2024.";
}
```

## 🧹 Code cleanup đã thực hiện

- ✅ Loại bỏ `GenerateSampleNguonVonData` (không sử dụng)
- ✅ Loại bỏ `GenerateSampleDuNoData` (không sử dụng)  
- ✅ Loại bỏ `GenerateSampleGLCB41Data` (không sử dụng)
- ✅ Xóa logic fallback dữ liệu mẫu cũ
- ✅ Chuẩn hóa error handling cho tất cả methods

## 🔄 Git commits đã thực hiện

1. **feat: add helper methods for cumulative KPI date logic**
2. **feat: standardize CalculateNguonVon with exact date logic**  
3. **feat: standardize CalculateDuNo with exact date logic**
4. **feat: standardize CalculateTyLeNoXau with exact date logic**
5. **feat: standardize cumulative KPI logic for ThuHoiXLRR and LoiNhuan**
6. **refactor: remove unused sample data generation methods**

## 🧪 Testing Guidelines

### Test cases cần kiểm tra:
1. **Tháng 4/2025**: Phải lấy đúng file ngày 30/4/2025
2. **Tháng 12/2024**: Phải lấy đúng file ngày 31/12/2024  
3. **Tháng khác**: Phải hiển thị thông báo lỗi rõ ràng
4. **Chi nhánh cụ thể**: Chỉ tính dữ liệu của chi nhánh đó
5. **Không có file**: Hiển thị lỗi "Không tìm thấy file import"

### Frontend error handling:
- ✅ Errors được hiển thị qua `errorMessage.value`
- ✅ Success messages qua `successMessage.value`  
- ✅ Loading states qua `calculating.value`

## 📝 Lưu ý cho tương lai

1. **Mở rộng dữ liệu**: Khi có thêm file cho các tháng khác, chỉ cần cập nhật `GetTargetStatementDate()`
2. **Thu dịch vụ**: Cần cập nhật khi có công thức cụ thể
3. **Performance**: Có thể cache kết quả tính toán để tối ưu
4. **Monitoring**: Theo dõi logs để đảm bảo tính chính xác

---
**📅 Hoàn thành vào**: {{ new Date().toLocaleDateString('vi-VN') }}  
**👨‍💻 Thực hiện bởi**: GitHub Copilot  
**🎯 Mục tiêu**: Chuẩn hóa logic KPI lũy kế theo đúng ngày dữ liệu thực tế
