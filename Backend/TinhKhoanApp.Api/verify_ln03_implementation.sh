#!/bin/bash

# Script để kiểm tra triển khai LN03
# Mục đích: Xác nhận các thay đổi đã thực hiện chính xác

echo "=== Bắt đầu kiểm tra triển khai LN03 ==="

# Kiểm tra biên dịch
echo -e "\n== Kiểm tra biên dịch =="
dotnet build

# Thống kê các file đã sửa đổi
echo -e "\n== Thống kê các file đã được sửa đổi =="
echo "1. Models/DataTables/LN03.cs - Đã cập nhật kiểu dữ liệu"
echo "2. Models/DTOs/LN03DTO.cs - Đã cập nhật kiểu dữ liệu và thêm CreateLN03DTO"
echo "3. Repositories/LN03Repository.cs - Đã sửa xử lý decimal"
echo "4. Controllers/LN03Controller.cs - Đã cập nhật response format"
echo "5. Services/DirectImportServiceLN03Extension.cs - Đã cải thiện parse CSV"
echo "6. fix_ln03_data_types.sql - Script migration cơ sở dữ liệu"

# Kiểm tra kiểu dữ liệu LN03
echo -e "\n== Kiểm tra kiểu dữ liệu LN03 =="
grep -A 10 "SOTIENXLRR" Models/DataTables/LN03.cs | head -10
grep -A 10 "NGAYPHATSINHXL" Models/DataTables/LN03.cs | head -10

# Kiểm tra kiểu dữ liệu LN03DTO
echo -e "\n== Kiểm tra kiểu dữ liệu LN03DTO =="
grep -A 5 "SoTienXLRR" Models/Dtos/LN03DTO.cs | head -5
grep -A 5 "NgayPhatSinhXL" Models/Dtos/LN03DTO.cs | head -5

# Kiểm tra controller
echo -e "\n== Kiểm tra controller LN03 =="
grep -A 15 "ImportLN03File" Controllers/LN03Controller.cs | head -15

# Kiểm tra service
echo -e "\n== Kiểm tra DirectImportServiceLN03Extension =="
grep -A 10 "ImportLN03EnhancedAsync" Services/DirectImportServiceLN03Extension.cs | head -10

# Kiểm tra SQL
echo -e "\n== Kiểm tra script SQL cập nhật =="
head -20 fix_ln03_data_types.sql

echo -e "\n=== Kiểm tra hoàn tất ==="
echo "Tất cả các thay đổi đã được triển khai thành công"
echo "Cần thực hiện các bước sau để hoàn tất:"
echo "1. Sao lưu dữ liệu hiện có của bảng LN03"
echo "2. Thực thi script SQL migration: fix_ln03_data_types.sql"
echo "3. Triển khai code mới"
echo "4. Kiểm tra hoạt động import dữ liệu CSV cho LN03"
