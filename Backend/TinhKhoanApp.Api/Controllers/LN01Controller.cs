using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.RawData;
using System.Text;
using System.Text.Json;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// Controller xử lý dữ liệu bảng LN01 - Khoản vay cá nhân
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LN01Controller : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LN01Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách thay đổi của chi nhánh 7808 từ 30/4/2025 đến 31/5/2025 từ ImportedDataRecords
        /// </summary>
        [HttpGet("changes/branch-7808")]
        public async Task<IActionResult> GetBranch7808Changes()
        {
            try
            {
                // Truy vấn từ ImportedDataRecords và ImportedDataItems
                var importRecords = await _context.ImportedDataRecords
                    .Where(x => x.Category == "LN01" &&
                               x.FileName.Contains("7808") &&
                               x.StatementDate >= new DateTime(2025, 4, 30) &&
                               x.StatementDate <= new DateTime(2025, 5, 31))
                    .OrderByDescending(x => x.StatementDate)
                    .ToListAsync();

                if (!importRecords.Any())
                {
                    return Ok(new
                    {
                        Summary = new
                        {
                            TotalChanges = 0,
                            TimeRange = "30/04/2025 - 31/05/2025",
                            BranchCode = "7808",
                            QueryTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                            Message = "Không tìm thấy dữ liệu LN01 cho chi nhánh 7808 trong khoảng thời gian này"
                        },
                        Changes = new List<object>()
                    });
                }

                // Lấy chi tiết dữ liệu cho mỗi file import
                var detailedChanges = new List<object>();

                foreach (var record in importRecords)
                {
                    // Lấy một số mẫu dữ liệu từ ImportedDataItems
                    var sampleItems = await _context.ImportedDataItems
                        .Where(x => x.ImportedDataRecordId == record.Id)
                        .Take(10) // Lấy 10 mẫu đầu tiên
                        .Select(x => new
                        {
                            Id = x.Id,
                            RawData = x.RawData,
                            ProcessedDate = x.ProcessedDate.ToString("dd/MM/yyyy HH:mm:ss")
                        })
                        .ToListAsync();

                    detailedChanges.Add(new
                    {
                        ImportId = record.Id,
                        FileName = record.FileName,
                        StatementDate = record.StatementDate?.ToString("dd/MM/yyyy") ?? "N/A",
                        ImportDate = record.ImportDate.ToString("dd/MM/yyyy HH:mm:ss"),
                        ImportedBy = record.ImportedBy ?? "System",
                        Status = record.Status ?? "Unknown",
                        RecordsCount = record.RecordsCount,
                        Category = record.Category,
                        FileType = record.FileType,
                        Notes = record.Notes,
                        BranchCode = "7808",
                        SampleData = sampleItems,
                        ChangeType = record.StatementDate.HasValue && record.StatementDate.Value.Date == new DateTime(2025, 4, 30).Date
                            ? "Dữ liệu đầu kỳ"
                            : record.StatementDate.HasValue && record.StatementDate.Value.Date == new DateTime(2025, 5, 31).Date
                            ? "Dữ liệu cuối kỳ"
                            : "Dữ liệu giữa kỳ"
                    });
                }

                var summary = new
                {
                    TotalChanges = importRecords.Count,
                    TotalRecords = importRecords.Sum(x => x.RecordsCount),
                    TimeRange = "30/04/2025 - 31/05/2025",
                    BranchCode = "7808",
                    QueryTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    DateRange = new
                    {
                        FromDate = importRecords.Min(r => r.StatementDate)?.ToString("dd/MM/yyyy") ?? "N/A",
                        ToDate = importRecords.Max(r => r.StatementDate)?.ToString("dd/MM/yyyy") ?? "N/A"
                    },
                    FileInfo = importRecords.Select(r => new
                    {
                        FileName = r.FileName,
                        Records = r.RecordsCount,
                        Date = r.StatementDate?.ToString("dd/MM/yyyy")
                    }).ToList()
                };

                return Ok(new
                {
                    Summary = summary,
                    Changes = detailedChanges
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Lỗi khi lấy dữ liệu thay đổi",
                    Message = ex.Message,
                    Details = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Trích xuất mã chi nhánh từ SourceId
        /// </summary>
        private static string ExtractBranchCode(string sourceId)
        {
            // Tìm mã chi nhánh 4 số trong SourceId
            var match = System.Text.RegularExpressions.Regex.Match(sourceId, @"\d{4}");
            return match.Success ? match.Value : "N/A";
        }

        /// <summary>
        /// Lấy thống kê tổng quan thay đổi LN01
        /// </summary>
        [HttpGet("changes/summary")]
        public async Task<IActionResult> GetChangesSummary()
        {
            try
            {
                // Lấy thống kê từ LN01_History
                var stats = await _context.LN01History
                    .GroupBy(x => x.SourceID.Contains("7808") ? "7808" : "Other")
                    .Select(g => new
                    {
                        BranchGroup = g.Key,
                        TotalRecords = g.Count(),
                        ChangesInPeriod = g.Count(x => x.ValidFrom >= new DateTime(2025, 4, 30) &&
                                                      x.ValidFrom <= new DateTime(2025, 5, 31)),
                        EarliestRecord = g.Min(x => x.ValidFrom),
                        LatestRecord = g.Max(x => x.ValidFrom),
                        EarliestRecordFormatted = g.Min(x => x.ValidFrom).ToString("dd/MM/yyyy"),
                        LatestRecordFormatted = g.Max(x => x.ValidFrom).ToString("dd/MM/yyyy"),
                        CurrentRecords = g.Count(x => x.IsCurrent),
                        HistoryRecords = g.Count(x => !x.IsCurrent)
                    })
                    .ToListAsync();

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Lỗi khi lấy thống kê",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy tất cả dữ liệu LN01 hiện có trong hệ thống
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllRecords()
        {
            try
            {
                var allRecords = await _context.LN01History
                    .OrderByDescending(x => x.ValidFrom)
                    .Take(100) // Giới hạn 100 bản ghi để tránh quá tải
                    .Select(r => new
                    {
                        HistoryID = r.HistoryID,
                        SourceID = r.SourceID,
                        ValidFrom = r.ValidFrom.ToString("dd/MM/yyyy HH:mm:ss"),
                        ValidTo = r.ValidTo.ToString("dd/MM/yyyy HH:mm:ss"),
                        IsCurrent = r.IsCurrent,
                        VersionNumber = r.VersionNumber,
                        CreatedDate = r.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss"),
                        BranchCode = "N/A",
                        MANDT = r.MANDT,
                        BUKRS = r.BUKRS,
                        LAND1 = r.LAND1,
                        WAERS = r.WAERS
                    })
                    .ToListAsync();

                return Ok(new
                {
                    TotalFound = allRecords.Count,
                    Message = allRecords.Any() ? "Đã tìm thấy dữ liệu LN01" : "Chưa có dữ liệu LN01 trong hệ thống",
                    Records = allRecords
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Lỗi khi lấy tất cả dữ liệu",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Tạo dữ liệu mẫu cho chi nhánh 7808 để test
        /// </summary>
        [HttpPost("create-sample-data")]
        public async Task<IActionResult> CreateSampleData()
        {
            try
            {
                var sampleRecords = new List<LN01History>();

                // Tạo 5 bản ghi mẫu cho chi nhánh 7808
                for (int i = 1; i <= 5; i++)
                {
                    var record = new LN01History
                    {
                        SourceID = $"LN01_7808_{i:000}",
                        ValidFrom = new DateTime(2025, 4, 30).AddDays(i),
                        ValidTo = new DateTime(9999, 12, 31, 23, 59, 59),
                        IsCurrent = true,
                        VersionNumber = 1,
                        RecordHash = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,

                        // Dữ liệu LN01 mẫu
                        MANDT = "100",
                        BUKRS = "7808",
                        LAND1 = "VN",
                        WAERS = "VND",
                        SPRAS = "V",
                        KTOPL = "AGR1",
                        WAABW = "01",
                        PERIV = "K4",
                        KOKFI = "X",
                        RCOMP = "100000",
                        ADRNR = $"100000{i:00}",
                        STCEG = $"0123456789{i}",
                        FIKRS = "AGR1",
                        XFMCO = "X",
                        XFMCB = "X",
                        XFMCA = "X",
                        TXJCD = "V1"
                    };

                    sampleRecords.Add(record);
                }

                await _context.LN01History.AddRangeAsync(sampleRecords);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Message = $"Đã tạo {sampleRecords.Count} bản ghi mẫu cho chi nhánh 7808",
                    CreatedRecords = sampleRecords.Select(r => new
                    {
                        SourceID = r.SourceID,
                        ValidFrom = r.ValidFrom.ToString("dd/MM/yyyy"),
                        BUKRS = r.BUKRS
                    })
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Lỗi khi tạo dữ liệu mẫu",
                    Message = ex.Message,
                    Details = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Debug: Lấy thông tin tất cả file LN01 của chi nhánh 7808
        /// </summary>
        [HttpGet("debug/branch-7808-files")]
        public async Task<IActionResult> GetBranch7808Files()
        {
            try
            {
                var allLN01Files = await _context.ImportedDataRecords
                    .Where(x => x.Category == "LN01" && x.FileName.Contains("7808"))
                    .OrderByDescending(x => x.ImportDate)
                    .ToListAsync();

                var result = allLN01Files.Select(x => new
                {
                    Id = x.Id,
                    FileName = x.FileName,
                    StatementDate = x.StatementDate?.ToString("dd/MM/yyyy") ?? "NULL",
                    ImportDate = x.ImportDate.ToString("dd/MM/yyyy HH:mm:ss"),
                    RecordsCount = x.RecordsCount,
                    Status = x.Status,
                    ImportedBy = x.ImportedBy,
                    BranchCode = "7808"
                }).ToList();

                return Ok(new
                {
                    Message = $"Tìm thấy {result.Count} file LN01 của chi nhánh 7808",
                    Files = result,
                    FilterInfo = new
                    {
                        RequestedDateRange = "30/04/2025 - 31/05/2025",
                        ActualDatesFound = result.Select(f => f.StatementDate).Distinct().ToList()
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Lỗi khi debug files",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Đơn giản: Lấy tất cả file LN01 chi nhánh 7808
        /// </summary>
        [HttpGet("simple-files")]
        public async Task<IActionResult> GetSimpleFiles()
        {
            try
            {
                var files = await _context.ImportedDataRecords
                    .Where(x => x.Category == "LN01" && x.FileName.Contains("7808"))
                    .ToListAsync();

                return Ok(files);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// So sánh chi tiết dữ liệu LN01 giữa hai ngày cho chi nhánh 7808
        /// </summary>
        [HttpGet("detailed-comparison/branch-7808")]
        public async Task<IActionResult> GetDetailedComparison()
        {
            try
            {
                // Lấy dữ liệu từ file ngày 30/4/2025
                var april30Record = await _context.ImportedDataRecords
                    .Where(x => x.Category == "LN01" &&
                               x.FileName.Contains("7808") &&
                               x.StatementDate.HasValue &&
                               x.StatementDate.Value.Date == new DateTime(2025, 4, 30).Date)
                    .FirstOrDefaultAsync();

                // Lấy dữ liệu từ file ngày 31/5/2025
                var may31Record = await _context.ImportedDataRecords
                    .Where(x => x.Category == "LN01" &&
                               x.FileName.Contains("7808") &&
                               x.StatementDate.HasValue &&
                               x.StatementDate.Value.Date == new DateTime(2025, 5, 31).Date)
                    .FirstOrDefaultAsync();

                if (april30Record == null && may31Record == null)
                {
                    return Ok(new
                    {
                        Summary = new
                        {
                            Status = "NoData",
                            Message = "Không tìm thấy dữ liệu cho cả hai ngày",
                            QueryTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                        }
                    });
                }

                var comparison = new
                {
                    Summary = new
                    {
                        BranchCode = "7808",
                        ComparisonDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                        April30File = april30Record?.FileName ?? "Không có",
                        May31File = may31Record?.FileName ?? "Không có",
                        April30Records = april30Record?.RecordsCount ?? 0,
                        May31Records = may31Record?.RecordsCount ?? 0,
                        RecordsDifference = (may31Record?.RecordsCount ?? 0) - (april30Record?.RecordsCount ?? 0),
                        HasBothFiles = april30Record != null && may31Record != null
                    },
                    Files = new
                    {
                        April30 = april30Record != null ? new
                        {
                            Id = april30Record.Id,
                            FileName = april30Record.FileName,
                            StatementDate = april30Record.StatementDate?.ToString("dd/MM/yyyy"),
                            ImportDate = april30Record.ImportDate.ToString("dd/MM/yyyy HH:mm:ss"),
                            RecordsCount = april30Record.RecordsCount,
                            Status = april30Record.Status,
                            ImportedBy = april30Record.ImportedBy
                        } : null,
                        May31 = may31Record != null ? new
                        {
                            Id = may31Record.Id,
                            FileName = may31Record.FileName,
                            StatementDate = may31Record.StatementDate?.ToString("dd/MM/yyyy"),
                            ImportDate = may31Record.ImportDate.ToString("dd/MM/yyyy HH:mm:ss"),
                            RecordsCount = may31Record.RecordsCount,
                            Status = may31Record.Status,
                            ImportedBy = may31Record.ImportedBy
                        } : null
                    }
                };

                // Nếu có cả hai file, lấy sample data để so sánh
                if (april30Record != null && may31Record != null)
                {
                    var april30Data = await _context.ImportedDataItems
                        .Where(x => x.ImportedDataRecordId == april30Record.Id)
                        .Take(20)
                        .Select(x => new
                        {
                            Id = x.Id,
                            RawData = x.RawData,
                            ProcessedDate = x.ProcessedDate.ToString("dd/MM/yyyy HH:mm:ss")
                        })
                        .ToListAsync();

                    var may31Data = await _context.ImportedDataItems
                        .Where(x => x.ImportedDataRecordId == may31Record.Id)
                        .Take(20)
                        .Select(x => new
                        {
                            Id = x.Id,
                            RawData = x.RawData,
                            ProcessedDate = x.ProcessedDate.ToString("dd/MM/yyyy HH:mm:ss")
                        })
                        .ToListAsync();

                    return Ok(new
                    {
                        Summary = comparison.Summary,
                        Files = comparison.Files,
                        SampleComparison = new
                        {
                            April30Sample = april30Data,
                            May31Sample = may31Data,
                            ComparisonNote = "Hiển thị 20 bản ghi đầu tiên từ mỗi file để so sánh"
                        }
                    });
                }

                return Ok(comparison);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Lỗi khi so sánh dữ liệu chi tiết",
                    Message = ex.Message,
                    Details = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Lấy tất cả thay đổi LN01 chi nhánh 7808 với định dạng đơn giản
        /// </summary>
        [HttpGet("all-changes/branch-7808")]
        public async Task<IActionResult> GetAllBranch7808Changes()
        {
            try
            {
                // Lấy tất cả file LN01 của chi nhánh 7808
                var allFiles = await _context.ImportedDataRecords
                    .Where(x => x.Category == "LN01" && x.FileName.Contains("7808"))
                    .OrderBy(x => x.StatementDate)
                    .ToListAsync();

                var changes = new List<object>();

                foreach (var file in allFiles)
                {
                    // Lấy một vài bản ghi mẫu từ mỗi file
                    var sampleData = await _context.ImportedDataItems
                        .Where(x => x.ImportedDataRecordId == file.Id)
                        .Take(5)
                        .Select(x => new
                        {
                            RawData = x.RawData.Length > 100 ? x.RawData.Substring(0, 100) + "..." : x.RawData,
                            ProcessedDate = x.ProcessedDate.ToString("dd/MM/yyyy HH:mm:ss")
                        })
                        .ToListAsync();

                    changes.Add(new
                    {
                        FileName = file.FileName,
                        StatementDate = file.StatementDate?.ToString("dd/MM/yyyy") ?? "N/A",
                        ImportDate = file.ImportDate.ToString("dd/MM/yyyy HH:mm:ss"),
                        RecordsCount = file.RecordsCount,
                        Status = file.Status,
                        ImportedBy = file.ImportedBy ?? "System",
                        SampleRecords = sampleData
                    });
                }

                return Ok(new
                {
                    BranchCode = "7808",
                    TotalFiles = allFiles.Count,
                    TotalRecords = allFiles.Sum(x => x.RecordsCount),
                    TimeRange = allFiles.Any() ?
                        $"{allFiles.Min(x => x.StatementDate)?.ToString("dd/MM/yyyy")} - {allFiles.Max(x => x.StatementDate)?.ToString("dd/MM/yyyy")}" :
                        "N/A",
                    QueryTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    Changes = changes
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Lỗi khi lấy tất cả thay đổi",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Xuất tất cả thay đổi LN01 chi nhánh 7808 ra file CSV
        /// </summary>
        [HttpGet("export/csv/branch-7808")]
        public async Task<IActionResult> ExportBranch7808ToCSV()
        {
            try
            {
                // Lấy tất cả file LN01 của chi nhánh 7808
                var allFiles = await _context.ImportedDataRecords
                    .Where(x => x.Category == "LN01" && x.FileName.Contains("7808"))
                    .OrderBy(x => x.StatementDate)
                    .ToListAsync();

                var csvData = new StringBuilder();
                
                // Header cho CSV
                csvData.AppendLine("STT,FileName,StatementDate,ImportDate,RecordsCount,Status,ImportedBy,CustomerSeq,CustomerName,AccountNumber,Currency,DebtAmount,LoanType,InterestRate,OfficerName,NextRepayDate,Province,District,LastRepayDate");

                int stt = 1;
                foreach (var file in allFiles)
                {
                    // Lấy tất cả dữ liệu chi tiết từ file này
                    var detailData = await _context.ImportedDataItems
                        .Where(x => x.ImportedDataRecordId == file.Id)
                        .Select(x => x.RawData)
                        .ToListAsync();

                    foreach (var rawData in detailData)
                    {
                        try
                        {
                            // Parse JSON để lấy thông tin chi tiết
                            var jsonDoc = JsonDocument.Parse(rawData);
                            var root = jsonDoc.RootElement;

                            var custSeq = root.TryGetProperty("CUSTSEQ", out var custSeqProp) ? custSeqProp.GetString() : "";
                            var custName = root.TryGetProperty("CUSTNM", out var custNameProp) ? custNameProp.GetString() : "";
                            var account = root.TryGetProperty("TAI_KHOAN", out var accountProp) ? accountProp.GetString() : "";
                            var currency = root.TryGetProperty("CCY", out var ccyProp) ? ccyProp.GetString() : "";
                            var debtAmount = root.TryGetProperty("DU_NO", out var debtProp) ? debtProp.GetString() : "";
                            var loanType = root.TryGetProperty("LOAN_TYPE", out var loanTypeProp) ? loanTypeProp.GetString() : "";
                            var interestRate = root.TryGetProperty("INTEREST_RATE", out var rateProp) ? rateProp.GetString() : "";
                            var officerName = root.TryGetProperty("OFFICER_NAME", out var officerProp) ? officerProp.GetString() : "";
                            var nextRepayDate = root.TryGetProperty("NEXT_REPAY_DATE", out var nextRepayProp) ? nextRepayProp.GetString() : "";
                            var province = root.TryGetProperty("LCLPROVINNM", out var provinceProp) ? provinceProp.GetString() : "";
                            var district = root.TryGetProperty("LCLDISTNM", out var districtProp) ? districtProp.GetString() : "";
                            var lastRepayDate = root.TryGetProperty("LAST_REPAY_DATE", out var lastRepayProp) ? lastRepayProp.GetString() : "";

                            // Escape CSV special characters
                            custName = EscapeCSVField(custName);
                            loanType = EscapeCSVField(loanType);
                            officerName = EscapeCSVField(officerName);
                            province = EscapeCSVField(province);
                            district = EscapeCSVField(district);

                            csvData.AppendLine($"{stt},{file.FileName},{file.StatementDate?.ToString("dd/MM/yyyy")},{file.ImportDate:dd/MM/yyyy HH:mm:ss},{file.RecordsCount},{file.Status},{file.ImportedBy},{custSeq},{custName},{account},{currency},{debtAmount},{loanType},{interestRate},{officerName},{nextRepayDate},{province},{district},{lastRepayDate}");
                            stt++;
                        }
                        catch (JsonException)
                        {
                            // Nếu không parse được JSON, thêm dòng với thông tin cơ bản
                            csvData.AppendLine($"{stt},{file.FileName},{file.StatementDate?.ToString("dd/MM/yyyy")},{file.ImportDate:dd/MM/yyyy HH:mm:ss},{file.RecordsCount},{file.Status},{file.ImportedBy},,,,,,,,,,,");
                            stt++;
                        }
                    }
                }

                var csvBytes = Encoding.UTF8.GetBytes(csvData.ToString());
                var fileName = $"LN01_Branch_7808_Changes_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                return File(csvBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Lỗi khi xuất file CSV",
                    Message = ex.Message,
                    Details = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Xuất tóm tắt thay đổi LN01 chi nhánh 7808 ra file CSV (chỉ thông tin file)
        /// </summary>
        [HttpGet("export/summary-csv/branch-7808")]
        public async Task<IActionResult> ExportBranch7808SummaryToCSV()
        {
            try
            {
                // Lấy tất cả file LN01 của chi nhánh 7808
                var allFiles = await _context.ImportedDataRecords
                    .Where(x => x.Category == "LN01" && x.FileName.Contains("7808"))
                    .OrderBy(x => x.StatementDate)
                    .ToListAsync();

                var csvData = new StringBuilder();
                
                // Header cho CSV tóm tắt
                csvData.AppendLine("STT,FileName,StatementDate,ImportDate,RecordsCount,Status,ImportedBy,FileType,Category,Notes");

                int stt = 1;
                foreach (var file in allFiles)
                {
                    var notes = EscapeCSVField(file.Notes ?? "");
                    csvData.AppendLine($"{stt},{file.FileName},{file.StatementDate?.ToString("dd/MM/yyyy")},{file.ImportDate:dd/MM/yyyy HH:mm:ss},{file.RecordsCount},{file.Status},{file.ImportedBy},{file.FileType},{file.Category},{notes}");
                    stt++;
                }

                var csvBytes = Encoding.UTF8.GetBytes(csvData.ToString());
                var fileName = $"LN01_Branch_7808_Summary_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                return File(csvBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Lỗi khi xuất file CSV tóm tắt",
                    Message = ex.Message,
                    Details = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Xuất so sánh chi tiết giữa hai ngày ra file CSV
        /// </summary>
        [HttpGet("export/comparison-csv/branch-7808")]
        public async Task<IActionResult> ExportComparisonToCSV()
        {
            try
            {
                // Lấy dữ liệu từ file ngày 30/4/2025
                var april30Record = await _context.ImportedDataRecords
                    .Where(x => x.Category == "LN01" &&
                               x.FileName.Contains("7808") &&
                               x.StatementDate.HasValue &&
                               x.StatementDate.Value.Date == new DateTime(2025, 4, 30).Date)
                    .FirstOrDefaultAsync();

                // Lấy dữ liệu từ file ngày 31/5/2025
                var may31Record = await _context.ImportedDataRecords
                    .Where(x => x.Category == "LN01" &&
                               x.FileName.Contains("7808") &&
                               x.StatementDate.HasValue &&
                               x.StatementDate.Value.Date == new DateTime(2025, 5, 31).Date)
                    .FirstOrDefaultAsync();

                if (april30Record == null || may31Record == null)
                {
                    return BadRequest(new
                    {
                        Error = "Không tìm thấy đủ dữ liệu để so sánh",
                        Message = $"File 30/4: {(april30Record != null ? "Có" : "Không có")}, File 31/5: {(may31Record != null ? "Có" : "Không có")}"
                    });
                }

                var csvData = new StringBuilder();
                
                // Header cho CSV so sánh
                csvData.AppendLine("Type,FileName,StatementDate,CustomerSeq,CustomerName,AccountNumber,Currency,DebtAmount,LoanType,InterestRate,OfficerName,Province,District,LastRepayDate");

                // Lấy dữ liệu từ file tháng 4
                var april30Data = await _context.ImportedDataItems
                    .Where(x => x.ImportedDataRecordId == april30Record.Id)
                    .Select(x => x.RawData)
                    .ToListAsync();

                // Lấy dữ liệu từ file tháng 5
                var may31Data = await _context.ImportedDataItems
                    .Where(x => x.ImportedDataRecordId == may31Record.Id)
                    .Select(x => x.RawData)
                    .ToListAsync();

                // Xuất dữ liệu tháng 4
                foreach (var rawData in april30Data.Take(100)) // Giới hạn 100 bản ghi đầu tiên
                {
                    AddDataRowToCSV(csvData, "30/04/2025", april30Record.FileName, rawData);
                }

                // Xuất dữ liệu tháng 5
                foreach (var rawData in may31Data.Take(100)) // Giới hạn 100 bản ghi đầu tiên
                {
                    AddDataRowToCSV(csvData, "31/05/2025", may31Record.FileName, rawData);
                }

                var csvBytes = Encoding.UTF8.GetBytes(csvData.ToString());
                var fileName = $"LN01_Branch_7808_Comparison_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                return File(csvBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Lỗi khi xuất file CSV so sánh",
                    Message = ex.Message,
                    Details = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Helper method để thêm dòng dữ liệu vào CSV
        /// </summary>
        private void AddDataRowToCSV(StringBuilder csvData, string type, string fileName, string rawData)
        {
            try
            {
                var jsonDoc = JsonDocument.Parse(rawData);
                var root = jsonDoc.RootElement;

                var custSeq = root.TryGetProperty("CUSTSEQ", out var custSeqProp) ? custSeqProp.GetString() : "";
                var custName = root.TryGetProperty("CUSTNM", out var custNameProp) ? custNameProp.GetString() : "";
                var account = root.TryGetProperty("TAI_KHOAN", out var accountProp) ? accountProp.GetString() : "";
                var currency = root.TryGetProperty("CCY", out var ccyProp) ? ccyProp.GetString() : "";
                var debtAmount = root.TryGetProperty("DU_NO", out var debtProp) ? debtProp.GetString() : "";
                var loanType = root.TryGetProperty("LOAN_TYPE", out var loanTypeProp) ? loanTypeProp.GetString() : "";
                var interestRate = root.TryGetProperty("INTEREST_RATE", out var rateProp) ? rateProp.GetString() : "";
                var officerName = root.TryGetProperty("OFFICER_NAME", out var officerProp) ? officerProp.GetString() : "";
                var province = root.TryGetProperty("LCLPROVINNM", out var provinceProp) ? provinceProp.GetString() : "";
                var district = root.TryGetProperty("LCLDISTNM", out var districtProp) ? districtProp.GetString() : "";
                var lastRepayDate = root.TryGetProperty("LAST_REPAY_DATE", out var lastRepayProp) ? lastRepayProp.GetString() : "";

                // Escape CSV special characters
                custName = EscapeCSVField(custName);
                loanType = EscapeCSVField(loanType);
                officerName = EscapeCSVField(officerName);
                province = EscapeCSVField(province);
                district = EscapeCSVField(district);

                csvData.AppendLine($"{type},{fileName},{type},{custSeq},{custName},{account},{currency},{debtAmount},{loanType},{interestRate},{officerName},{province},{district},{lastRepayDate}");
            }
            catch (JsonException)
            {
                csvData.AppendLine($"{type},{fileName},{type},,,,,,,,,,,");
            }
        }

        /// <summary>
        /// Helper method để escape các ký tự đặc biệt trong CSV
        /// </summary>
        private string EscapeCSVField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";

            // Nếu có dấu phẩy, dấu ngoặc kép, hoặc xuống dòng thì bọc trong dấu ngoặc kép
            if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            {
                // Escape dấu ngoặc kép bằng cách nhân đôi
                field = field.Replace("\"", "\"\"");
                return $"\"{field}\"";
            }

            return field;
        }
    }
}
