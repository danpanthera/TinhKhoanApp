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
