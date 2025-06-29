using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Data;
using System.Text;
using System.Globalization;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// Service để đọc file Excel đời cũ (.xls, Excel 97-2003) bằng NPOI
    /// Lấy sheet đầu tiên, dòng 1 làm header, từ dòng 2 trở đi là dữ liệu
    /// </summary>
    public interface ILegacyExcelReaderService
    {
        Task<ExcelReadResult> ReadExcelFileAsync(Stream stream, string fileName);
        bool CanReadFile(string fileName);
    }

    public class ExcelReadResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Headers { get; set; } = new();
        public List<Dictionary<string, object>> Data { get; set; } = new();
        public int TotalRows { get; set; }
        public string WorksheetName { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
    }

    public class LegacyExcelReaderService : ILegacyExcelReaderService
    {
        private readonly ILogger<LegacyExcelReaderService> _logger;

        public LegacyExcelReaderService(ILogger<LegacyExcelReaderService> logger)
        {
            _logger = logger;
        }

        public bool CanReadFile(string fileName)
        {
            return fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<ExcelReadResult> ReadExcelFileAsync(Stream stream, string fileName)
        {
            var result = new ExcelReadResult();

            try
            {
                _logger.LogInformation("📊 Starting to read legacy Excel file: {FileName}", fileName);

                // Reset stream position
                stream.Position = 0;

                // Tạo workbook từ stream bằng NPOI HSSF (cho .xls)
                HSSFWorkbook workbook;
                try
                {
                    workbook = new HSSFWorkbook(stream);
                    _logger.LogInformation("✅ Successfully opened .xls workbook with NPOI");
                }
                catch (Exception ex)
                {
                    result.Message = $"Cannot read .xls file: {ex.Message}";
                    result.Errors.Add($"NPOI Error: {ex.Message}");
                    _logger.LogError(ex, "❌ Failed to open .xls file with NPOI: {FileName}", fileName);
                    return result;
                }

                using (workbook)
                {
                    // Kiểm tra có worksheet không
                    if (workbook.NumberOfSheets == 0)
                    {
                        result.Message = "Excel file contains no worksheets";
                        return result;
                    }

                    // Lấy sheet đầu tiên
                    var worksheet = workbook.GetSheetAt(0);
                    result.WorksheetName = worksheet.SheetName;

                    _logger.LogInformation("📋 Reading worksheet: {WorksheetName}", result.WorksheetName);

                    // Kiểm tra có dữ liệu không
                    if (worksheet.LastRowNum < 0)
                    {
                        result.Message = "Worksheet is empty";
                        return result;
                    }

                    // Đọc header từ dòng đầu tiên (row index 0)
                    var headerRow = worksheet.GetRow(0);
                    if (headerRow == null)
                    {
                        result.Message = "Header row is empty";
                        return result;
                    }

                    // Lấy headers từ dòng đầu tiên
                    result.Headers = GetRowValues(headerRow);
                    _logger.LogInformation("📋 Headers found: {HeaderCount} columns - [{Headers}]",
                        result.Headers.Count, string.Join("] [", result.Headers.Take(5)));

                    if (!result.Headers.Any())
                    {
                        result.Message = "No headers found in first row";
                        return result;
                    }

                    // Đọc dữ liệu từ dòng thứ 2 trở đi (row index 1+)
                    int dataRowsProcessed = 0;
                    int totalDataRows = worksheet.LastRowNum; // 0-based, so if LastRowNum = 5, we have rows 0,1,2,3,4,5

                    _logger.LogInformation("📊 Processing data rows from 1 to {LastRow}", totalDataRows);

                    for (int rowIndex = 1; rowIndex <= totalDataRows; rowIndex++)
                    {
                        var row = worksheet.GetRow(rowIndex);
                        if (row == null) continue; // Skip null rows

                        var rowValues = GetRowValues(row);

                        // Skip completely empty rows
                        if (rowValues.All(v => string.IsNullOrWhiteSpace(v?.ToString())))
                            continue;

                        // Tạo dictionary cho dữ liệu row
                        var dataRecord = new Dictionary<string, object>();

                        // Map values to headers
                        for (int colIndex = 0; colIndex < result.Headers.Count; colIndex++)
                        {
                            var headerName = result.Headers[colIndex];
                            var cellValue = colIndex < rowValues.Count ? rowValues[colIndex] : string.Empty;

                            // Fix Vietnamese encoding if needed
                            if (cellValue is string strValue && !string.IsNullOrEmpty(strValue))
                            {
                                cellValue = FixVietnameseEncoding(strValue);
                            }

                            dataRecord[headerName] = cellValue ?? string.Empty;
                        }

                        result.Data.Add(dataRecord);
                        dataRowsProcessed++;

                        // Log progress for large files
                        if (dataRowsProcessed % 1000 == 0)
                        {
                            _logger.LogInformation("⚡ Processed {Processed} rows...", dataRowsProcessed);
                        }
                    }

                    result.TotalRows = dataRowsProcessed;
                    result.Success = true;
                    result.Message = $"Successfully read {dataRowsProcessed} data rows from .xls file";

                    _logger.LogInformation("✅ Legacy Excel file read completed: {Rows} rows, {Columns} columns",
                        dataRowsProcessed, result.Headers.Count);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error reading legacy Excel file: {FileName}", fileName);
                result.Message = $"Error reading .xls file: {ex.Message}";
                result.Errors.Add(ex.Message);
                return result;
            }
        }

        /// <summary>
        /// Lấy tất cả giá trị từ một row, bao gồm cả empty cells
        /// </summary>
        private List<string> GetRowValues(IRow row)
        {
            var values = new List<string>();

            if (row == null) return values;

            // Lấy số cột tối đa trong row này
            var lastCellNum = row.LastCellNum; // 1-based

            for (int cellIndex = 0; cellIndex < lastCellNum; cellIndex++)
            {
                var cell = row.GetCell(cellIndex);
                var cellValue = GetCellValueAsString(cell);
                values.Add(cellValue);
            }

            return values;
        }

        /// <summary>
        /// Chuyển đổi cell value thành string, xử lý các loại dữ liệu khác nhau
        /// </summary>
        private string GetCellValueAsString(ICell cell)
        {
            if (cell == null) return string.Empty;

            try
            {
                switch (cell.CellType)
                {
                    case CellType.String:
                        return cell.StringCellValue?.Trim() ?? string.Empty;

                    case CellType.Numeric:
                        if (DateUtil.IsCellDateFormatted(cell))
                        {
                            var dateValue = cell.DateCellValue;
                            return dateValue.HasValue ?
                                $"{dateValue.Value.Year:0000}-{dateValue.Value.Month:00}-{dateValue.Value.Day:00}" :
                                string.Empty;
                        }
                        return cell.NumericCellValue.ToString();

                    case CellType.Boolean:
                        return cell.BooleanCellValue.ToString();

                    case CellType.Formula:
                        try
                        {
                            // Try to get the calculated value
                            switch (cell.CachedFormulaResultType)
                            {
                                case CellType.String:
                                    return cell.StringCellValue?.Trim() ?? string.Empty;
                                case CellType.Numeric:
                                    if (DateUtil.IsCellDateFormatted(cell))
                                    {
                                        var dateValue = cell.DateCellValue;
                                        return dateValue.HasValue ?
                                            $"{dateValue.Value.Year:0000}-{dateValue.Value.Month:00}-{dateValue.Value.Day:00}" :
                                            string.Empty;
                                    }
                                    return cell.NumericCellValue.ToString();
                                case CellType.Boolean:
                                    return cell.BooleanCellValue.ToString();
                                default:
                                    return cell.ToString() ?? string.Empty;
                            }
                        }
                        catch
                        {
                            return cell.ToString() ?? string.Empty;
                        }

                    case CellType.Error:
                        _logger.LogWarning("⚠️ Error cell encountered: {ErrorCode}", cell.ErrorCellValue);
                        return "#ERROR#";

                    case CellType.Blank:
                    default:
                        return string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ Error reading cell value, returning empty string");
                return string.Empty;
            }
        }

        /// <summary>
        /// Sửa encoding tiếng Việt bị lỗi
        /// </summary>
        private string FixVietnameseEncoding(string input)
        {
            try
            {
                if (string.IsNullOrEmpty(input)) return input;

                // Kiểm tra và fix các ký tự encoding bị lỗi thường gặp
                if (input.Contains("â€") || input.Contains("Ä") || input.Contains("Ã"))
                {
                    // Thử chuyển đổi từ UTF-8 bị lỗi thành Unicode đúng
                    var bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(input);
                    return Encoding.UTF8.GetString(bytes);
                }

                return input;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "❌ Error fixing encoding for text: {Input}",
                    input.Substring(0, Math.Min(input.Length, 50)));
                return input; // Fallback to original
            }
        }
    }
}
