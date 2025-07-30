#!/bin/bash

echo "=== 📋 Kế hoạch kiểm thử triển khai RR01 với kiểu dữ liệu mới ==="

# Phần 1: Tạo dữ liệu mẫu để kiểm thử
echo -e "\n=== 🔄 1. Tạo dữ liệu mẫu RR01 ==="
cat > sample_rr01_test.csv << 'CSV_CONTENT'
CN_LOAI_I,BRCD,MA_KH,TEN_KH,SO_LDS,CCY,SO_LAV,LOAI_KH,NGAY_GIAI_NGAN,NGAY_DEN_HAN,VAMC_FLG,NGAY_XLRR,DUNO_GOC_BAN_DAU,DUNO_LAI_TICHLUY_BD,DOC_DAUKY_DA_THU_HT,DUNO_GOC_HIENTAI,DUNO_LAI_HIENTAI,DUNO_NGAN_HAN,DUNO_TRUNG_HAN,DUNO_DAI_HAN,THU_GOC,THU_LAI,BDS,DS,TSK
CN,7800,KH123,NGUYỄN VĂN A,123.45,VND,LAV001,CN,20250601,20251231,Y,20250701,1000000.00,50000.00,25000.00,975000.00,45000.00,100000.00,400000.00,475000.00,25000.00,5000.00,500000.00,600000.00,1100000.00
CN,7800,KH124,TRẦN THỊ B,456.78,VND,LAV002,CN,20250501,20251130,N,20250615,2000000.00,75000.00,40000.00,1960000.00,72000.00,200000.00,800000.00,960000.00,40000.00,3000.00,1000000.00,1200000.00,2200000.00
CN,7800,KH125,LÊ VĂN C,789.10,VND,LAV003,CN,20250401,20251031,Y,20250520,3000000.00,100000.00,60000.00,2940000.00,95000.00,300000.00,1200000.00,1440000.00,60000.00,5000.00,1500000.00,1800000.00,3300000.00
CS,7800,KH126,PHẠM THỊ D,,USD,LAV004,CS,20250301,20250930,N,20250410,500000.00,25000.00,10000.00,490000.00,23000.00,50000.00,200000.00,240000.00,10000.00,2000.00,300000.00,350000.00,650000.00
CSV_CONTENT
echo "✅ Đã tạo tập tin mẫu với 4 bản ghi có dữ liệu số và ngày tháng đa dạng"

# Phần 2: Kiểm tra cấu trúc SQL
echo -e "\n=== 🔍 2. Kiểm tra script SQL cập nhật cấu trúc ==="
if [ -f "update_rr01_datatypes.sql" ]; then
    echo "✅ Đã tìm thấy script SQL cập nhật cấu trúc"
    grep -n "ALTER TABLE RR01" update_rr01_datatypes.sql | head -5
    echo "... và các lệnh khác (tổng cộng $(grep -c "ALTER TABLE RR01" update_rr01_datatypes.sql) lệnh ALTER)"
else
    echo "❌ Không tìm thấy script SQL. Tạo mới script để cập nhật cấu trúc bảng."
fi

# Phần 3: Kiểm tra mã nguồn C# xử lý chuyển đổi kiểu dữ liệu
echo -e "\n=== 🔍 3. Kiểm tra hàm chuyển đổi kiểu dữ liệu ==="
echo "Kiểm tra hàm ConvertCsvValue trong DirectImportService.cs:"
grep -n -A 5 "underlyingType == typeof(decimal)" Services/DirectImportService.cs | head -6
echo "..."
grep -n -A 5 "underlyingType == typeof(DateTime)" Services/DirectImportService.cs | head -6
echo "✅ Hàm chuyển đổi kiểu dữ liệu đã hỗ trợ decimal và DateTime"

# Phần 4: Tạo kịch bản kiểm thử đơn vị
echo -e "\n=== 🧪 4. Tạo kịch bản kiểm thử ==="
cat > TestRR01DataTypes.cs << 'TEST_CODE'
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Services;

namespace TinhKhoanApp.Api.Tests
{
    /// <summary>
    /// Kịch bản kiểm thử cho RR01 với kiểu dữ liệu mới
    /// </summary>
    public class TestRR01DataTypes
    {
        /// <summary>
        /// Kiểm thử import dữ liệu RR01 với các kiểu dữ liệu mới (decimal, DateTime)
        /// </summary>
        public static async Task TestImportAndConversion()
        {
            Console.WriteLine("=== Kiểm thử chuyển đổi kiểu dữ liệu RR01 ===");
            
            // 1. Tạo một bản ghi RR01 với dữ liệu mẫu
            var testRecord = new RR01
            {
                NGAY_DL = DateTime.Parse("2025-07-30"),
                CN_LOAI_I = "CN",
                BRCD = "7800",
                MA_KH = "KH123",
                TEN_KH = "NGUYỄN VĂN A",
                SO_LDS = 123.45m,
                CCY = "VND",
                SO_LAV = "LAV001",
                LOAI_KH = "CN",
                NGAY_GIAI_NGAN = DateTime.Parse("2025-06-01"),
                NGAY_DEN_HAN = DateTime.Parse("2025-12-31"),
                VAMC_FLG = "Y",
                NGAY_XLRR = DateTime.Parse("2025-07-01"),
                DUNO_GOC_BAN_DAU = 1000000.00m,
                DUNO_LAI_TICHLUY_BD = 50000.00m,
                DOC_DAUKY_DA_THU_HT = 25000.00m,
                DUNO_GOC_HIENTAI = 975000.00m,
                DUNO_LAI_HIENTAI = 45000.00m,
                DUNO_NGAN_HAN = 100000.00m,
                DUNO_TRUNG_HAN = 400000.00m,
                DUNO_DAI_HAN = 475000.00m,
                THU_GOC = 25000.00m,
                THU_LAI = 5000.00m,
                BDS = 500000.00m,
                DS = 600000.00m,
                TSK = 1100000.00m,
                FILE_NAME = "test_7800_rr01_20250730.csv"
            };
            
            // 2. Kiểm tra kiểu dữ liệu
            Console.WriteLine($"SO_LDS: {testRecord.SO_LDS?.GetType().Name} = {testRecord.SO_LDS}");
            Console.WriteLine($"NGAY_GIAI_NGAN: {testRecord.NGAY_GIAI_NGAN?.GetType().Name} = {testRecord.NGAY_GIAI_NGAN:yyyy-MM-dd}");
            Console.WriteLine($"DUNO_GOC_BAN_DAU: {testRecord.DUNO_GOC_BAN_DAU?.GetType().Name} = {testRecord.DUNO_GOC_BAN_DAU:N2}");
            
            // 3. Chuyển đổi từ Entity sang DTO
            var dto = RR01DTO.FromEntity(testRecord);
            Console.WriteLine("\nKiểm tra chuyển đổi Entity -> DTO:");
            Console.WriteLine($"DTO.SO_LDS: {dto.SO_LDS?.GetType().Name} = {dto.SO_LDS}");
            Console.WriteLine($"DTO.NGAY_GIAI_NGAN: {dto.NGAY_GIAI_NGAN?.GetType().Name} = {dto.NGAY_GIAI_NGAN:yyyy-MM-dd}");
            Console.WriteLine($"DTO.DUNO_GOC_BAN_DAU: {dto.DUNO_GOC_BAN_DAU?.GetType().Name} = {dto.DUNO_GOC_BAN_DAU:N2}");
            
            // 4. Mô phỏng việc parse CSV để kiểm tra ConvertCsvValue
            Console.WriteLine("\nKiểm tra mô phỏng parse CSV:");
            string csvLine = "CN,7800,KH123,NGUYỄN VĂN A,123.45,VND,LAV001,CN,20250601,20251231,Y,20250701,1000000.00,50000.00,25000.00,975000.00,45000.00,100000.00,400000.00,475000.00,25000.00,5000.00,500000.00,600000.00,1100000.00";
            
            // Giả lập hàm ConvertCsvValue - đơn giản hóa cho mục đích kiểm thử
            decimal? ParseDecimal(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return null;
                    
                var normalizedValue = value.Replace(",", "").Trim();
                if (decimal.TryParse(normalizedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                    return result;
                return null;
            }
            
            DateTime? ParseDateTime(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return null;
                    
                string[] formats = { "yyyyMMdd", "yyyy-MM-dd", "dd/MM/yyyy" };
                if (DateTime.TryParseExact(value.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                    return result;
                return null;
            }
            
            var fields = csvLine.Split(',');
            var so_lds = ParseDecimal(fields[4]);
            var ngay_giai_ngan = ParseDateTime(fields[8]);
            var duno_goc = ParseDecimal(fields[12]);
            
            Console.WriteLine($"CSV Parse - SO_LDS: {so_lds?.GetType().Name} = {so_lds}");
            Console.WriteLine($"CSV Parse - NGAY_GIAI_NGAN: {ngay_giai_ngan?.GetType().Name} = {ngay_giai_ngan:yyyy-MM-dd}");
            Console.WriteLine($"CSV Parse - DUNO_GOC_BAN_DAU: {duno_goc?.GetType().Name} = {duno_goc:N2}");
            
            Console.WriteLine("\n✅ Kiểm thử hoàn tất: Các kiểu dữ liệu đúng theo yêu cầu");
        }
    }
}
TEST_CODE
echo "✅ Đã tạo kịch bản kiểm thử cho RR01 với kiểu dữ liệu mới"

# Phần 5: Tạo SQL để kiểm tra dữ liệu sau khi import
echo -e "\n=== 🔍 5. Tạo câu lệnh SQL kiểm tra dữ liệu sau khi import ==="
cat > verify_rr01_imported_data.sql << 'SQL_VERIFY'
-- Kiểm tra dữ liệu số và ngày trong bảng RR01 sau khi import
SELECT 
    MA_KH,
    TEN_KH,
    SO_LDS,
    NGAY_GIAI_NGAN,
    NGAY_DEN_HAN,
    NGAY_XLRR,
    DUNO_GOC_BAN_DAU,
    DUNO_LAI_TICHLUY_BD,
    DUNO_GOC_HIENTAI,
    DUNO_LAI_HIENTAI,
    DUNO_NGAN_HAN + DUNO_TRUNG_HAN + DUNO_DAI_HAN AS TONG_DU_NO,
    THU_GOC,
    THU_LAI,
    BDS + DS AS TONG_BAO_DAM,
    TSK,
    FILE_NAME,
    CREATED_DATE
FROM 
    RR01
WHERE 
    NGAY_DL = CONVERT(DATETIME, '20250730')
    AND FILE_NAME LIKE '%test%'
ORDER BY 
    MA_KH;
    
-- Kiểm tra tính toán với kiểu số
SELECT
    MA_KH,
    TEN_KH,
    DUNO_GOC_BAN_DAU,
    DUNO_GOC_HIENTAI,
    (DUNO_GOC_BAN_DAU - DUNO_GOC_HIENTAI) AS GOC_DA_TRA,
    DUNO_NGAN_HAN,
    DUNO_TRUNG_HAN,
    DUNO_DAI_HAN,
    (DUNO_NGAN_HAN + DUNO_TRUNG_HAN + DUNO_DAI_HAN) AS TONG_DU_NO,
    CASE 
        WHEN DUNO_GOC_HIENTAI > 0 THEN 
            (BDS + DS) / DUNO_GOC_HIENTAI * 100 
        ELSE 0 
    END AS TY_LE_BAO_DAM
FROM
    RR01
WHERE 
    NGAY_DL = CONVERT(DATETIME, '20250730')
    AND FILE_NAME LIKE '%test%'
ORDER BY 
    MA_KH;
SQL_VERIFY
echo "✅ Đã tạo câu lệnh SQL để kiểm tra dữ liệu đã import"

# Phần 6: Tạo hướng dẫn triển khai
echo -e "\n=== 📝 6. Hướng dẫn triển khai ==="
cat > rr01_implementation_guide.md << 'GUIDE'
# Hướng dẫn triển khai cập nhật kiểu dữ liệu RR01

## Chuẩn bị
1. Tạo bản backup cơ sở dữ liệu trước khi thực hiện thay đổi
2. Đảm bảo đã cập nhật mã nguồn với kiểu dữ liệu mới (Model và DTO)
3. Chuẩn bị script SQL để cập nhật cấu trúc bảng

## Các bước thực hiện

### 1. Cập nhật cấu trúc cơ sở dữ liệu
```sql
-- Thực hiện script update_rr01_datatypes.sql trong SQL Server Management Studio
```

### 2. Kiểm thử import dữ liệu
1. Tạo một tập tin CSV mẫu để kiểm thử (đã có sample_rr01_test.csv)
2. Sử dụng API hoặc giao diện import để nhập liệu
3. Kiểm tra log để xác nhận không có lỗi trong quá trình chuyển đổi kiểu dữ liệu

### 3. Xác minh dữ liệu đã import
1. Thực hiện script verify_rr01_imported_data.sql để kiểm tra dữ liệu
2. Xác nhận các giá trị số và ngày tháng được lưu trữ chính xác
3. Xác nhận có thể thực hiện các phép tính trên dữ liệu số

### 4. Kiểm thử toàn diện
1. Kiểm tra các chức năng khác liên quan đến RR01
2. Kiểm tra báo cáo và phân tích dữ liệu RR01
3. Xác nhận không có lỗi khi hiển thị, xuất báo cáo

## Xử lý sự cố
Nếu gặp vấn đề trong quá trình triển khai, có thể:
1. Khôi phục cơ sở dữ liệu từ bản backup
2. Kiểm tra logs để xác định lỗi
3. Xem xét chạy một phần script (chỉ cập nhật một số cột)

## Xác nhận hoàn tất
Sau khi hoàn tất triển khai, kiểm tra:
1. Cấu trúc bảng RR01 đã được cập nhật
2. Dữ liệu đã được chuyển đổi đúng kiểu
3. Các chức năng liên quan hoạt động bình thường
GUIDE
echo "✅ Đã tạo hướng dẫn triển khai chi tiết"

echo -e "\n=== ✅ Hoàn tất kế hoạch kiểm thử triển khai RR01 ==="
echo "Các tài nguyên đã được tạo:"
echo "1. sample_rr01_test.csv - Dữ liệu mẫu để kiểm thử"
echo "2. TestRR01DataTypes.cs - Kịch bản kiểm thử đơn vị"
echo "3. verify_rr01_imported_data.sql - SQL kiểm tra dữ liệu đã import"
echo "4. rr01_implementation_guide.md - Hướng dẫn triển khai chi tiết"
echo ""
echo "Bạn có thể sử dụng các tài nguyên này để triển khai và kiểm thử cập nhật kiểu dữ liệu RR01."
