#!/bin/bash

# Script kiểm tra thay đổi LN03 sau khi triển khai
# Tạo bởi: Copilot
# Ngày: $(date +"%d/%m/%Y")

echo "=== Kiểm tra thay đổi LN03 sau khi triển khai ==="

# Thiết lập các biến
DB_SERVER="localhost,1433"
DB_NAME="TinhKhoanDB"
DB_USER="sa"
DB_PASSWORD="Dientoan@303"

# Kiểm tra cấu trúc bảng LN03 trong cơ sở dữ liệu
check_database_structure() {
  echo -e "\n=== Kiểm tra cấu trúc bảng LN03 trong cơ sở dữ liệu ==="
  
  # Sử dụng sqlcmd để kiểm tra cấu trúc bảng
  sqlcmd -S $DB_SERVER -d $DB_NAME -U $DB_USER -P $DB_PASSWORD -Q "
  SELECT 
    c.name AS ColumnName,
    t.name AS DataType,
    c.max_length,
    c.precision,
    c.scale,
    c.is_nullable
  FROM 
    sys.columns c
    JOIN sys.types t ON c.user_type_id = t.user_type_id
  WHERE 
    c.object_id = OBJECT_ID('dbo.LN03')
  ORDER BY 
    c.column_id;
  " -o database_check_output.txt -C -N -b -t 30 -y 30
  
  # Hiển thị kết quả
  echo "Kết quả kiểm tra cấu trúc bảng LN03:"
  cat database_check_output.txt
  
  # Kiểm tra các cột quan trọng cần phải có kiểu dữ liệu chính xác
  echo -e "\nKiểm tra kiểu dữ liệu của các trường quan trọng:"
  
  # Kiểm tra trường số tiền
  sqlcmd -S $DB_SERVER -d $DB_NAME -U $DB_USER -P $DB_PASSWORD -Q "
  SELECT 
    name, 
    system_type_id,
    CASE 
      WHEN system_type_id IN (106, 108, 122) THEN 'PASS: Numeric type'
      ELSE 'FAIL: Not a numeric type'
    END AS Check_Result
  FROM 
    sys.columns 
  WHERE 
    object_id = OBJECT_ID('dbo.LN03') 
    AND name IN ('SoTienXLRR', 'ThuNoSauXL', 'ConLaiNgoaiBang', 'DuNoNoiBang');
  " -o numeric_check.txt -C -N -b -t 30 -y 30
  
  cat numeric_check.txt
  
  # Kiểm tra trường ngày tháng
  sqlcmd -S $DB_SERVER -d $DB_NAME -U $DB_USER -P $DB_PASSWORD -Q "
  SELECT 
    name, 
    system_type_id,
    CASE 
      WHEN system_type_id IN (40, 42, 43, 58, 61) THEN 'PASS: Date type'
      ELSE 'FAIL: Not a date type'
    END AS Check_Result
  FROM 
    sys.columns 
  WHERE 
    object_id = OBJECT_ID('dbo.LN03') 
    AND name IN ('NgayPhatSinhXL');
  " -o date_check.txt -C -N -b -t 30 -y 30
  
  cat date_check.txt
}

# Kiểm tra khả năng import CSV mới
check_csv_import() {
  echo -e "\n=== Kiểm tra khả năng import CSV mới ==="
  
  # Kiểm tra API endpoint LN03
  echo "Kiểm tra API endpoint LN03:"
  if [ -f ./bin/Debug/net8.0/TinhKhoanApp.Api.dll ]; then
    echo "✅ Ứng dụng API đã được build thành công"
  else
    echo "❌ Không tìm thấy file ứng dụng API đã build"
    return 1
  fi
  
  echo "Các test file CSV đã được chuẩn bị trong thư mục test_data/"
  echo "Hãy thực hiện kiểm tra import thủ công hoặc sử dụng script test_data/run_ln03_tests.sh"
}

# Kiểm tra code thay đổi
check_code_changes() {
  echo -e "\n=== Kiểm tra code thay đổi ==="
  
  # Tìm file LN03.cs
  LN03_MODEL_FILE=$(find . -name "LN03.cs" | head -n 1)
  
  # Kiểm tra file LN03.cs
  echo "Kiểm tra file model LN03:"
  if [ -n "$LN03_MODEL_FILE" ]; then
    echo "Tìm thấy file model tại: $LN03_MODEL_FILE"
    if grep -q "decimal?" "$LN03_MODEL_FILE" && grep -q "DateTime?" "$LN03_MODEL_FILE"; then
      echo "✅ Model LN03 sử dụng kiểu dữ liệu decimal? và DateTime? đúng"
    else
      echo "❌ Model LN03 không sử dụng kiểu dữ liệu đúng"
    fi
  else
    echo "❌ Không tìm thấy file model LN03.cs"
  fi
  
  # Tìm DirectImportService
  DIRECT_IMPORT_SERVICE=$(find . -name "DirectImportService*.cs" | head -n 1)
  
  # Kiểm tra DirectImportService
  echo -e "\nKiểm tra DirectImportService:"
  if [ -n "$DIRECT_IMPORT_SERVICE" ]; then
    echo "Tìm thấy file service tại: $DIRECT_IMPORT_SERVICE"
    if grep -q "ConvertCsvValue" "$DIRECT_IMPORT_SERVICE" || grep -q "ParseLN03" "$DIRECT_IMPORT_SERVICE"; then
      echo "✅ DirectImportService có phương thức chuyển đổi kiểu dữ liệu"
    else
      echo "❌ DirectImportService không có phương thức chuyển đổi kiểu dữ liệu"
    fi
  else
    echo "❌ Không tìm thấy file DirectImportService"
  fi
  
  # Tìm LN03Controller
  LN03_CONTROLLER=$(find . -name "LN03Controller.cs" | head -n 1)
  
  # Kiểm tra LN03Controller
  echo -e "\nKiểm tra LN03Controller:"
  if [ -n "$LN03_CONTROLLER" ]; then
    echo "Tìm thấy file controller tại: $LN03_CONTROLLER"
    if grep -q "Import" "$LN03_CONTROLLER" && ! grep -q "// Disabled" "$LN03_CONTROLLER"; then
      echo "✅ LN03Controller có phương thức Import và không bị disable"
    else
      echo "❌ LN03Controller không có phương thức Import hoặc bị disable"
    fi
  else
    echo "❌ Không tìm thấy file LN03Controller.cs"
  fi
}

# Chạy các chức năng kiểm tra
main() {
  echo "Bắt đầu kiểm tra toàn diện thay đổi LN03..."
  
  # Kiểm tra code
  check_code_changes
  
  # Kiểm tra cấu trúc DB
  check_database_structure
  
  # Kiểm tra import CSV
  check_csv_import
  
  echo -e "\n=== Tóm tắt kết quả kiểm tra ==="
  echo "1. Kiểm tra code: Xem kết quả bên trên"
  echo "2. Kiểm tra cơ sở dữ liệu: Xem kết quả trong file database_check_output.txt"
  echo "3. Kiểm tra API: Xem kết quả trong file api_health.txt"
  echo ""
  echo "Để kiểm tra đầy đủ quy trình import CSV, hãy chạy script test_data/run_ln03_tests.sh"
  echo "Hướng dẫn chi tiết được lưu trong file test_data/README.md"
}

# Thực hiện kiểm tra
main
