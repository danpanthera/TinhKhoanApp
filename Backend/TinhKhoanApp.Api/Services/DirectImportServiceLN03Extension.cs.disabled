using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// Extension cho DirectImportService tập trung vào LN03
    /// </summary>
    public partial class DirectImportService
    {
        /// <summary>
        /// Phương thức import LN03 được cải tiến để xử lý đúng các trường số và ngày
        /// </summary>
        public async Task<DirectImportResult> ImportLN03EnhancedAsync(IFormFile file, string? statementDate = null)
        {
            _logger.LogInformation("🚀 [LN03_ENHANCED] Import vào bảng LN03 với xử lý cải tiến cho số và ngày tháng");

            var result = new DirectImportResult
            {
                FileName = file.FileName,
                DataType = "LN03",
                TargetTable = "LN03",
                FileSizeBytes = file.Length,
                StartTime = DateTime.UtcNow
            };

            try
            {
                // Extract NgayDL từ filename
                var ngayDL = ExtractNgayDLFromFileName(file.FileName);
                result.NgayDL = ngayDL;

                // Create ImportedDataRecord for tracking (chỉ metadata)
                var importRecord = await CreateImportedDataRecordAsync(file, "LN03", 0);
                result.ImportedDataRecordId = importRecord.Id;

                // Parse CSV với xử lý đặc biệt cho LN03
                var records = await ParseLN03EnhancedAsync(file, statementDate);
                _logger.LogInformation("📊 [LN03_ENHANCED] Đã parse {Count} bản ghi từ CSV", records.Count);

                if (records.Any())
                {
                    _logger.LogInformation("📊 [LN03_ENHANCED] Bắt đầu bulk insert cho {Count} bản ghi", records.Count);
                    var insertedCount = await BulkInsertGenericAsync(records, "LN03");
                    result.ProcessedRecords = insertedCount;

                    // Update record count
                    importRecord.RecordsCount = insertedCount;
                    await _context.SaveChangesAsync();
                }

                result.Success = true;
                result.BatchId = Guid.NewGuid().ToString();
                result.EndTime = DateTime.UtcNow;

                _logger.LogInformation("✅ [LN03_ENHANCED] Import thành công: {Count} bản ghi trong {Duration}ms",
                    result.ProcessedRecords, (result.EndTime - result.StartTime).TotalMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [LN03_ENHANCED] Lỗi khi import dữ liệu LN03: {Error}", ex.Message);
                result.Success = false;
                result.ErrorMessage = $"Lỗi khi import: {ex.Message}";
                result.EndTime = DateTime.UtcNow;
                return result;
            }
        }

        /// <summary>
        /// Phương thức parse CSV cho LN03 với xử lý đặc biệt cho các trường số và ngày
        /// </summary>
        private async Task<List<LN03>> ParseLN03EnhancedAsync(IFormFile file, string? statementDate = null)
        {
            var records = new List<LN03>();
            var ngayDL = DateTime.Parse(ExtractNgayDLFromFileName(file.FileName));

            _logger.LogInformation("🔍 [LN03_PARSE] Bắt đầu parse CSV với xử lý cải tiến: {FileName}", file.FileName);

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            // Cấu hình CSV để xử lý định dạng phức tạp
            csv.Context.Configuration.MissingFieldFound = null;
            csv.Context.Configuration.HeaderValidated = null;
            csv.Context.Configuration.PrepareHeaderForMatch = args => args.Header.ToUpper();
            csv.Context.Configuration.BadDataFound = null;
            csv.Context.Configuration.Quote = '"';
            csv.Context.Configuration.Escape = '"';
            csv.Context.Configuration.Mode = CsvMode.RFC4180;
            csv.Context.Configuration.IgnoreBlankLines = true;
            csv.Context.Configuration.AllowComments = false;

            csv.Read();
            csv.ReadHeader();

            // Log headers để debug
            var headers = csv.HeaderRecord;
            _logger.LogInformation("📊 [LN03_PARSE] Các cột trong CSV: {Headers}", string.Join(", ", headers ?? new string[0]));

            int totalRows = 0;
            int successRows = 0;

            while (csv.Read())
            {
                totalRows++;
                try
                {
                    var record = new LN03
                    {
                        NGAY_DL = ngayDL,
                        FILE_NAME = file.FileName,
                        CREATED_DATE = DateTime.Now,
                        UPDATED_DATE = DateTime.Now
                    };

                    // Các trường chuỗi
                    record.MACHINHANH = csv.GetField("MACHINHANH") ?? "";
                    record.TENCHINHANH = csv.GetField("TENCHINHANH") ?? "";
                    record.MAKH = csv.GetField("MAKH") ?? "";
                    record.TENKH = csv.GetField("TENKH") ?? "";
                    record.SOHOPDONG = csv.GetField("SOHOPDONG") ?? "";
                    record.NHOMNO = csv.GetField("NHOMNO") ?? "";
                    record.MACBTD = csv.GetField("MACBTD") ?? "";
                    record.TENCBTD = csv.GetField("TENCBTD") ?? "";
                    record.MAPGD = csv.GetField("MAPGD") ?? "";
                    record.TAIKHOANHACHTOAN = csv.GetField("TAIKHOANHACHTOAN") ?? "";
                    record.REFNO = csv.GetField("REFNO") ?? "";
                    record.LOAINGUONVON = csv.GetField("LOAINGUONVON") ?? "";

                    // Các trường số
                    var sotienXLRR = csv.GetField("SOTIENXLRR") ?? "";
                    if (!string.IsNullOrWhiteSpace(sotienXLRR))
                    {
                        record.SOTIENXLRR = (decimal?)ConvertCsvValue(sotienXLRR, typeof(decimal?));
                    }

                    var thuNoSauXL = csv.GetField("THUNOSAUXL") ?? "";
                    if (!string.IsNullOrWhiteSpace(thuNoSauXL))
                    {
                        record.THUNOSAUXL = (decimal?)ConvertCsvValue(thuNoSauXL, typeof(decimal?));
                    }

                    var conLaiNgoaiBang = csv.GetField("CONLAINGOAIBANG") ?? "";
                    if (!string.IsNullOrWhiteSpace(conLaiNgoaiBang))
                    {
                        record.CONLAINGOAIBANG = (decimal?)ConvertCsvValue(conLaiNgoaiBang, typeof(decimal?));
                    }

                    var duNoNoiBang = csv.GetField("DUNONOIBANG") ?? "";
                    if (!string.IsNullOrWhiteSpace(duNoNoiBang))
                    {
                        record.DUNONOIBANG = (decimal?)ConvertCsvValue(duNoNoiBang, typeof(decimal?));
                    }

                    // Trường ngày tháng
                    var ngayPhatSinhXL = csv.GetField("NGAYPHATSINHXL") ?? "";
                    if (!string.IsNullOrWhiteSpace(ngayPhatSinhXL))
                    {
                        record.NGAYPHATSINHXL = (DateTime?)ConvertCsvValue(ngayPhatSinhXL, typeof(DateTime?));
                    }

                    records.Add(record);
                    successRows++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("⚠️ [LN03_PARSE] Lỗi khi parse dòng {Row}: {Error}", totalRows, ex.Message);
                }
            }

            _logger.LogInformation("✅ [LN03_PARSE] Đã parse thành công {Success}/{Total} dòng từ CSV", successRows, totalRows);
            return records;
        }
    }
}
