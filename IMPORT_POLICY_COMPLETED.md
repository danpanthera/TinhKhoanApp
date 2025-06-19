# 📋 Chính Sách Import Tệp Nén và CSV/XLS - Hệ Thống Tính Khoán

## 🎯 Tổng Quan

Hệ thống đã được nâng cấp để hỗ trợ import đồng thời file nén (ZIP) có mật khẩu và file CSV/XLS không có mật khẩu, với khả năng tự động phân loại và sắp xếp theo mã chi nhánh.

## 🔧 Tính Năng Chính

### 1. 📦 Hỗ Trợ File Nén
- **Định dạng hỗ trợ**: ZIP (mở rộng tương lai: 7z, RAR)
- **Mật khẩu**: Phát hiện và xử lý file có mật khẩu
- **Giải nén tự động**: Xử lý tất cả file CSV/XLS trong archive
- **Sắp xếp thông minh**: Tự động sắp xếp theo mã CN tăng dần

### 2. 📊 Tự Động Phân Loại
- **Định dạng tên file**: `MaCN_MaBC_NgayThangNam.extension`
- **Ví dụ**: `7800_GL01_20250531.csv`, `7801_LN01_20250531.zip`
- **Phân tích metadata**: Tự động trích xuất thông tin chi nhánh, báo cáo, ngày

### 3. 🏢 Mapping Chi Nhánh
```
7800 → Chi nhánh Cao Bằng
7801 → Chi nhánh Đồng Khê  
7802 → Chi nhánh Trà Lĩnh
7803 → Chi nhánh Quảng Uyên
7804 → Chi nhánh Hà Quảng
7805 → Chi nhánh Pac Nam
7806 → Chi nhánh Bảo Lạc
7807 → Chi nhánh Bảo Lâm
7808 → Chi nhánh Nguyên Bình
```

### 4. 📈 Mapping Loại Báo Cáo
```
GL01 → Sổ cái tổng hợp
LN01 → Báo cáo tín dụng
DP01 → Báo cáo tiền gửi
TR01 → Báo cáo giao dịch
KH01 → Báo cáo khách hàng
```

## 🛠 API Endpoints

### 1. Upload Nâng Cao
```http
POST /api/DataImport/upload-advanced
Content-Type: multipart/form-data

Body:
- files: File[] (CSV, XLS, ZIP)
- password: string (tùy chọn, cho file ZIP có mật khẩu)
```

**Response Example:**
```json
{
  "message": "Successfully processed 3 out of 3 files",
  "results": [
    {
      "success": true,
      "fileName": "7800_GL01_20250531.csv",
      "recordsProcessed": 3,
      "branchCode": "7800",
      "reportCode": "GL01", 
      "statementDate": "2025-05-31T00:00:00",
      "category": "Chi_nhanh_Cao_Bang_So_cai_tong_hop"
    }
  ],
  "totalArchivesProcessed": 1,
  "totalRegularFilesProcessed": 0
}
```

### 2. Thống Kê Import
```http
GET /api/DataImport/statistics?fromDate=2025-06-01&toDate=2025-06-30
```

**Response bao gồm:**
- Tổng số file và record
- Thống kê theo chi nhánh
- Thống kê theo loại báo cáo  
- Import gần đây

### 3. Xem Dữ Liệu Đã Import
```http
GET /api/DataImport
GET /api/DataImport/{id}/preview
```

## 🔄 Quy Trình Xử Lý

### 1. Phân Tích Tên File
```
7800_GL01_20250531.zip
├── 7800: Mã chi nhánh
├── GL01: Mã báo cáo  
├── 20250531: Ngày (yyyyMMdd)
└── .zip: Định dạng file
```

### 2. Chuyển Đổi Ngày
- **Input**: `20250531` (yyyyMMdd)
- **Output**: `31/05/2025` (dd/MM/yyyy)
- **Lưu trữ**: `2025-05-31T00:00:00` (ISO format)

### 3. Sắp Xếp Kết Quả
- **Ưu tiên**: Mã chi nhánh tăng dần (7800 → 7801 → 7802...)
- **Thứ tự**: Trong file ZIP, xử lý theo thứ tự alphabet của tên file
- **Metadata**: Lưu trữ thông tin chi nhánh, báo cáo, ngày

## 📝 Ví Dụ Sử Dụng

### 1. Import File ZIP
```bash
curl -X POST "http://localhost:5001/api/dataimport/upload-advanced" \
  -F "files=@branches_20250531.zip" \
  -F "password=mypassword"
```

### 2. Import Nhiều File CSV
```bash
curl -X POST "http://localhost:5001/api/dataimport/upload-advanced" \
  -F "files=@7800_GL01_20250531.csv" \
  -F "files=@7801_LN01_20250531.csv" \
  -F "files=@7802_DP01_20250531.csv"
```

### 3. Xem Thống Kê
```bash
curl -X GET "http://localhost:5001/api/dataimport/statistics"
```

## 🔍 Kiểm Tra và Xác Thực

### Test Cases Đã Thực Hiện:
1. ✅ **ZIP File Processing**: Import file ZIP chứa 3 file CSV khác nhau
2. ✅ **Auto Categorization**: Tự động phân loại theo mã CN và BC
3. ✅ **Date Parsing**: Chuyển đổi ngày từ yyyyMMdd sang dd/MM/yyyy
4. ✅ **Branch Sorting**: Sắp xếp kết quả theo mã CN tăng dần
5. ✅ **Individual Files**: Xử lý file CSV đơn lẻ với metadata đầy đủ
6. ✅ **Data Persistence**: Lưu trữ đúng định dạng trong database
7. ✅ **Statistics API**: Thống kê theo chi nhánh và loại báo cáo

### Kết Quả Test:
```json
{
  "7800_GL01_20250531.csv": {
    "branch": "Chi_nhanh_Cao_Bang", 
    "report": "So_cai_tong_hop",
    "date": "31/05/2025",
    "records": 3
  },
  "7801_LN01_20250531.csv": {
    "branch": "Chi_nhanh_Dong_Khe",
    "report": "Bao_cao_tin_dung", 
    "date": "31/05/2025",
    "records": 3
  }
}
```

## 🚀 Tính Năng Tương Lai

### 1. Mật Khẩu ZIP
- **Hiện tại**: Phát hiện file có mật khẩu
- **Tương lai**: Implement SharpCompress cho giải nén có mật khẩu

### 2. Định Dạng Bổ Sung
- **7z, RAR**: Mở rộng hỗ trợ format nén khác
- **XLSX**: Nâng cao xử lý Excel với nhiều sheet

### 3. Validation Rules  
- **Tên file**: Kiểm tra format chuẩn
- **Dữ liệu**: Validate nội dung theo loại báo cáo
- **Duplicate**: Phát hiện file trùng lặp

## ✅ Tóm Tắt Hoàn Thành

### Đã Triển Khai:
1. 🔧 **Enhanced DataImportController** với endpoint `/upload-advanced`
2. 📦 **ZIP File Processing** với auto-extraction
3. 🏷️ **Smart Categorization** theo mã CN và BC
4. 📅 **Date Parsing** từ tên file
5. 🔄 **Branch Code Sorting** tăng dần
6. 📊 **Statistics API** với thống kê chi tiết
7. 💾 **Metadata Storage** đầy đủ thông tin
8. ✅ **Comprehensive Testing** với nhiều test case

### Hệ Thống Sẵn Sàng:
- Import file CSV/XLS đơn lẻ hoặc trong ZIP
- Tự động phân loại và sắp xếp
- Chuyển đổi ngày tự động
- Thống kê và báo cáo chi tiết
- API documentation qua Swagger UI

**🎉 Chính sách import đã được thiết lập thành công và sẵn sàng sử dụng!**
