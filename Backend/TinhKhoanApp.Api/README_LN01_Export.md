# 📊 Hướng Dẫn Xuất Dữ Liệu LN01 Chi Nhánh 7808

## 🎯 Tổng Quan
Hệ thống cung cấp các API endpoint để xuất dữ liệu thay đổi LN01 (Khoản vay cá nhân) của chi nhánh 7808 trong khoảng thời gian từ 30/04/2025 đến 31/05/2025.

## 📈 Thống Kê Dữ Liệu
- **Tổng số file**: 4 file đã import
- **Tổng bản ghi**: 3,382 bản ghi
- **File 30/04/2025**: 848 bản ghi  
- **File 31/05/2025**: 843 bản ghi
- **Chênh lệch**: -5 bản ghi (giảm)

## 🔗 API Endpoints

### 1. Tóm Tắt Thay Đổi File (CSV)
```
GET http://localhost:5055/api/LN01/export/summary-csv/branch-7808
```
- **Mô tả**: Xuất thông tin tổng quan về các file đã import
- **Kích thước**: ~0.5KB
- **Dữ liệu**: Tên file, ngày, số bản ghi, trạng thái

### 2. So Sánh Chi Tiết 2 Ngày (CSV)  
```
GET http://localhost:5055/api/LN01/export/comparison-csv/branch-7808
```
- **Mô tả**: So sánh dữ liệu chi tiết giữa 30/04/2025 và 31/05/2025
- **Kích thước**: ~37KB  
- **Dữ liệu**: 200 bản ghi (100 từ mỗi ngày)

### 3. Dữ Liệu Chi Tiết Đầy Đủ (CSV)
```
GET http://localhost:5055/api/LN01/export/csv/branch-7808
```
- **Mô tả**: Xuất toàn bộ dữ liệu chi tiết của tất cả các bản ghi
- **Kích thước**: ~772KB
- **Dữ liệu**: 3,382 bản ghi với đầy đủ thông tin

### 4. API JSON Endpoints

#### So Sánh Chi Tiết (JSON)
```
GET http://localhost:5055/api/LN01/detailed-comparison/branch-7808
```

#### Tất Cả Thay Đổi (JSON)
```
GET http://localhost:5055/api/LN01/all-changes/branch-7808
```

#### Thay Đổi Cơ Bản (JSON)
```
GET http://localhost:5055/api/LN01/changes/branch-7808
```

## 📋 Cấu Trúc Dữ Liệu CSV

### Tóm Tắt File:
```csv
STT,FileName,StatementDate,ImportDate,RecordsCount,Status,ImportedBy,FileType,Category,Notes
```

### So Sánh Chi Tiết:
```csv
Type,FileName,StatementDate,CustomerSeq,CustomerName,AccountNumber,Currency,DebtAmount,LoanType,InterestRate,OfficerName,Province,District,LastRepayDate
```

### Dữ Liệu Đầy Đủ:
```csv
STT,FileName,StatementDate,ImportDate,RecordsCount,Status,ImportedBy,CustomerSeq,CustomerName,AccountNumber,Currency,DebtAmount,LoanType,InterestRate,OfficerName,NextRepayDate,Province,District,LastRepayDate
```

## 🛠️ Cách Sử Dụng

### 1. Qua Trình Duyệt Web
Mở file `export_ln01.html` trong trình duyệt để có giao diện thân thiện.

### 2. Qua Command Line (cURL)
```bash
# Tải file tóm tắt
curl -o "summary.csv" "http://localhost:5055/api/LN01/export/summary-csv/branch-7808"

# Tải file so sánh
curl -o "comparison.csv" "http://localhost:5055/api/LN01/export/comparison-csv/branch-7808"

# Tải file chi tiết đầy đủ
curl -o "details.csv" "http://localhost:5055/api/LN01/export/csv/branch-7808"
```

### 3. Qua HTTP Client (Postman, Insomnia...)
Sử dụng HTTP GET request đến các endpoint trên.

## 📝 Lưu Ý Quan Trọng

1. **Encoding**: Tất cả file CSV sử dụng UTF-8 encoding
2. **Excel**: Có thể mở trực tiếp bằng Excel (chọn UTF-8 khi import)
3. **Tốc độ**: File lớn có thể mất vài giây để tải
4. **Thời gian thực**: Dữ liệu được xuất trực tiếp từ database
5. **Giới hạn**: File so sánh giới hạn 100 bản ghi đầu tiên mỗi ngày để tối ưu tốc độ

## 🔧 Khởi Động Backend

```bash
cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api
dotnet run --urls=http://localhost:5055
```

Backend sẽ chạy tại: `http://localhost:5055`

## 📞 Hỗ Trợ

Nếu có vấn đề với việc xuất dữ liệu, kiểm tra:
1. Backend có đang chạy tại port 5055 không
2. Database có kết nối được không  
3. Dữ liệu LN01 chi nhánh 7808 có tồn tại không

---
*Tạo ngày: 26/06/2025*
*Phiên bản: 1.0*
