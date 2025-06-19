using System.Globalization;
using System.Text.RegularExpressions;

namespace TinhKhoanApp.Api.Services
{
    public interface IStatementDateService
    {
        DateTime? ExtractStatementDateFromFileName(string fileName);
        string GenerateFileKey(string fileType, DateTime statementDate);
    }

    public class StatementDateService : IStatementDateService
    {
        private readonly ILogger<StatementDateService> _logger;

        public StatementDateService(ILogger<StatementDateService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Trích xuất ngày sao kê từ tên file
        /// Patterns: 7800_GL01_20250531.csv, 7801_LN01_20250531_test.csv
        /// </summary>
        public DateTime? ExtractStatementDateFromFileName(string fileName)
        {
            try
            {
                // Pattern để match các format ngày phổ biến trong tên file
                var patterns = new[]
                {
                    @"_(\d{8})(?:_|\.)", // Pattern: _YYYYMMDD_ hoặc _YYYYMMDD.
                    @"_(\d{4})(\d{2})(\d{2})(?:_|\.)", // Pattern: _YYYY_MM_DD
                    @"(\d{8})", // Pattern: YYYYMMDD standalone
                    @"(\d{4})-(\d{2})-(\d{2})", // Pattern: YYYY-MM-DD
                    @"(\d{2})/(\d{2})/(\d{4})", // Pattern: MM/DD/YYYY
                    @"(\d{2})-(\d{2})-(\d{4})" // Pattern: MM-DD-YYYY
                };

                foreach (var pattern in patterns)
                {
                    var match = Regex.Match(fileName, pattern);
                    if (match.Success)
                    {
                        DateTime date;
                        
                        if (match.Groups.Count == 2) // Single group (YYYYMMDD)
                        {
                            var dateStr = match.Groups[1].Value;
                            if (dateStr.Length == 8 && 
                                DateTime.TryParseExact(dateStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                            {
                                _logger.LogInformation($"Extracted statement date {date:yyyy-MM-dd} from filename: {fileName}");
                                return date;
                            }
                        }
                        else if (match.Groups.Count == 4) // Three groups (YYYY, MM, DD)
                        {
                            if (int.TryParse(match.Groups[1].Value, out int year) &&
                                int.TryParse(match.Groups[2].Value, out int month) &&
                                int.TryParse(match.Groups[3].Value, out int day))
                            {
                                try
                                {
                                    date = new DateTime(year, month, day);
                                    _logger.LogInformation($"Extracted statement date {date:yyyy-MM-dd} from filename: {fileName}");
                                    return date;
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    // Invalid date, continue to next pattern
                                    continue;
                                }
                            }
                        }
                    }
                }

                _logger.LogWarning($"Could not extract statement date from filename: {fileName}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error extracting statement date from filename: {fileName}");
                return null;
            }
        }

        /// <summary>
        /// Tạo key để phân biệt các file cùng loại nhưng khác ngày
        /// </summary>
        public string GenerateFileKey(string fileType, DateTime statementDate)
        {
            return $"{fileType}_{statementDate:yyyyMMdd}";
        }
    }
}
