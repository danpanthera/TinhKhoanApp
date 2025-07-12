using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// 🔧 SERVICE CHUẨN HÓA: Extract thông tin từ filename theo format MaCN_LoaiFile_Ngay.ext
    /// Đảm bảo đồng nhất cách lấy thông tin cho tất cả bảng dữ liệu thô
    /// </summary>
    public interface IFileNameParsingService
    {
        /// <summary>
        /// Extract mã chi nhánh từ filename - Format: 7800_LN01_20241231.csv
        /// </summary>
        string? ExtractBranchCode(string fileName);

        /// <summary>
        /// Extract loại dữ liệu từ filename - Format: 7800_LN01_20241231.csv → LN01
        /// </summary>
        string? ExtractDataTypeFromFilename(string fileName);

        /// <summary>
        /// Extract ngày sao kê từ filename - Format: 7800_LN01_20241231.csv → 2024-12-31
        /// </summary>
        DateTime? ExtractStatementDate(string fileName);

        /// <summary>
        /// Validate filename có đúng format chuẩn hay không
        /// </summary>
        bool IsValidFormat(string fileName);

        /// <summary>
        /// Parse toàn bộ thông tin từ filename
        /// </summary>
        FileNameParseResult ParseFileName(string fileName);
    }

    public class FileNameParseResult
    {
        public bool IsValid { get; set; }
        public string? BranchCode { get; set; }
        public string? DataType { get; set; }
        public DateTime? StatementDate { get; set; }
        public string? Extension { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class FileNameParsingService : IFileNameParsingService
    {
        private readonly ILogger<FileNameParsingService> _logger;

        // 📋 Danh sách định nghĩa loại dữ liệu - ĐỒNG BỘ TẤT CẢ LOẠI
        private static readonly Dictionary<string, string> DataTypeDefinitions = new()
        {
            { "LN01", "Dữ liệu LOAN - Danh mục tín dụng" },

            { "LN03", "Dữ liệu Nợ XLRR - Nợ xử lý rủi ro" },
            { "DP01", "Dữ liệu Tiền gửi - Huy động vốn" },
            { "EI01", "Dữ liệu mobile banking - Giao dịch điện tử" },
            { "GL01", "Dữ liệu bút toán GDV - Giao dịch viên" },
            { "DPDA", "Dữ liệu sao kê phát hành thẻ - Thẻ tín dụng/ghi nợ" },

            { "BC57", "Sao kê Lãi dự thu - Dự phòng lãi" },
            { "RR01", "Sao kê dư nợ gốc, lãi XLRR - Rủi ro tín dụng" },
            { "GL41", "Bảng cân đối - Báo cáo tài chính" }
        };

        public FileNameParsingService(ILogger<FileNameParsingService> logger)
        {
            _logger = logger;
        }

        public string? ExtractBranchCode(string fileName)
        {
            try
            {
                _logger.LogInformation("🔍 Extracting branch code from filename: {FileName}", fileName);

                // Strategy 1: Format chuẩn MaCN_LoaiFile_Ngay.ext (7800_LN01_20241231.csv)
                var standardMatch = Regex.Match(fileName, @"^(78\d{2})_[A-Z0-9_]+_\d{8}\.(csv|xlsx?)", RegexOptions.IgnoreCase);
                if (standardMatch.Success)
                {
                    var branchCode = standardMatch.Groups[1].Value;
                    _logger.LogInformation("✅ Standard format - Branch code: {BranchCode}", branchCode);
                    return branchCode;
                }

                // Strategy 2: Fallback - tìm mã chi nhánh bất kỳ đâu trong filename (78xx)
                var fallbackMatch = Regex.Match(fileName, @"(78\d{2})");
                if (fallbackMatch.Success)
                {
                    var branchCode = fallbackMatch.Groups[1].Value;
                    _logger.LogWarning("⚠️ Non-standard format but found branch code: {BranchCode}", branchCode);
                    return branchCode;
                }

                _logger.LogWarning("❌ Không tìm thấy mã chi nhánh trong: {FileName}, sử dụng default 7800", fileName);
                return "7800";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi extract branch code từ: {FileName}", fileName);
                return "7800";
            }
        }

        public string? ExtractDataTypeFromFilename(string fileName)
        {
            try
            {
                _logger.LogInformation("🔍 Extracting data type from filename: {FileName}", fileName);

                // Strategy 1: Format chuẩn MaCN_LoaiFile_Ngay.ext
                var standardMatch = Regex.Match(fileName, @"^78\d{2}_([A-Z0-9_]+)_\d{8}\.(csv|xlsx?)", RegexOptions.IgnoreCase);
                if (standardMatch.Success)
                {
                    var dataType = standardMatch.Groups[1].Value.ToUpper();
                    _logger.LogInformation("✅ Standard format - Data type: {DataType}", dataType);
                    return dataType;
                }

                // Strategy 2: Fallback - tìm trong các loại đã định nghĩa
                var definedTypes = DataTypeDefinitions.Keys.ToArray();
                foreach (var type in definedTypes)
                {
                    if (fileName.Contains(type, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogWarning("⚠️ Non-standard format but found data type: {DataType}", type);
                        return type;
                    }
                }

                _logger.LogWarning("❌ Không tìm thấy loại dữ liệu trong: {FileName}", fileName);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi extract data type từ: {FileName}", fileName);
                return null;
            }
        }

        public DateTime? ExtractStatementDate(string fileName)
        {
            try
            {
                _logger.LogInformation("🔍 Extracting statement date from filename: {FileName}", fileName);

                // Strategy 1: Format chuẩn MaCN_LoaiFile_Ngay.ext (20241231)
                var standardMatch = Regex.Match(fileName, @"^78\d{2}_[A-Z0-9_]+_(\d{8})\.(csv|xlsx?)", RegexOptions.IgnoreCase);
                if (standardMatch.Success)
                {
                    var dateStr = standardMatch.Groups[1].Value;
                    if (DateTime.TryParseExact(dateStr, "yyyyMMdd", null, DateTimeStyles.None, out DateTime date))
                    {
                        _logger.LogInformation("✅ Standard format - Statement date: {Date}", date.ToString("yyyy-MM-dd"));
                        return date;
                    }
                }

                // Strategy 2: Fallback - tìm pattern yyyyMMdd bất kỳ đâu
                var fallbackMatch = Regex.Match(fileName, @"(\d{8})");
                if (fallbackMatch.Success)
                {
                    var dateStr = fallbackMatch.Groups[1].Value;
                    if (DateTime.TryParseExact(dateStr, "yyyyMMdd", null, DateTimeStyles.None, out DateTime date))
                    {
                        _logger.LogWarning("⚠️ Non-standard format but found date: {Date}", date.ToString("yyyy-MM-dd"));
                        return date;
                    }
                }

                _logger.LogWarning("❌ Không tìm thấy ngày hợp lệ trong: {FileName}, sử dụng ngày hiện tại", fileName);
                return DateTime.Now.Date;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi extract statement date từ: {FileName}", fileName);
                return DateTime.Now.Date;
            }
        }

        public bool IsValidFormat(string fileName)
        {
            try
            {
                // Kiểm tra format chuẩn: MaCN_LoaiFile_Ngay.ext
                var pattern = @"^(78\d{2})_([A-Z0-9_]+)_(\d{8})\.(csv|xlsx?)$";
                var match = Regex.Match(fileName, pattern, RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    // Kiểm tra mã chi nhánh hợp lệ (7800-7808)
                    var branchCode = match.Groups[1].Value;
                    if (int.TryParse(branchCode, out int branchNum) && branchNum >= 7800 && branchNum <= 7808)
                    {
                        // Kiểm tra loại dữ liệu có trong danh sách định nghĩa
                        var dataType = match.Groups[2].Value.ToUpper();
                        if (DataTypeDefinitions.ContainsKey(dataType))
                        {
                            // Kiểm tra ngày hợp lệ
                            var dateStr = match.Groups[3].Value;
                            if (DateTime.TryParseExact(dateStr, "yyyyMMdd", null, DateTimeStyles.None, out _))
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public FileNameParseResult ParseFileName(string fileName)
        {
            var result = new FileNameParseResult();

            try
            {
                _logger.LogInformation("🔍 Parsing complete filename: {FileName}", fileName);

                // Kiểm tra format chuẩn trước
                result.IsValid = IsValidFormat(fileName);

                // Extract thông tin dù có hợp lệ hay không
                result.BranchCode = ExtractBranchCode(fileName);
                result.DataType = ExtractDataTypeFromFilename(fileName);
                result.StatementDate = ExtractStatementDate(fileName);
                result.Extension = Path.GetExtension(fileName).TrimStart('.').ToLower();

                if (!result.IsValid)
                {
                    result.ErrorMessage = "File name không đúng format chuẩn MaCN_LoaiFile_Ngay.ext";
                    _logger.LogWarning("⚠️ Non-standard filename format: {FileName}", fileName);
                }
                else
                {
                    _logger.LogInformation("✅ Valid filename format: {FileName}", fileName);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi parse filename: {FileName}", fileName);
                result.IsValid = false;
                result.ErrorMessage = $"Lỗi parse filename: {ex.Message}";
                return result;
            }
        }
    }
}
