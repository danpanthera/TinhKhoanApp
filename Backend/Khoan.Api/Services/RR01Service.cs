using Microsoft.EntityFrameworkCore;
using System.Globalization;
using CsvHelper;
using Khoan.Api.Models.DataTables;
using Khoan.Api.Data;
using Khoan.Api.Dtos.RR01;
using Khoan.Api.Services.Interfaces;

namespace Khoan.Api.Services
{
    /// <summary>
    /// RR01 Service - Risk Report processing với CSV import logic
    /// Tuân thủ business column names, không transformation tiếng Việt
    /// </summary>
    public class RR01Service : IRR01Service
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RR01Service> _logger;

        public RR01Service(ApplicationDbContext context, ILogger<RR01Service> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Extract NGAY_DL từ filename theo pattern: [anything]_rr01_YYYYMMDD.[ext]
        /// </summary>
        public DateTime? ExtractDateFromFilename(string fileName)
        {
            try
            {
                var fileNameUpper = fileName.ToUpper();
                if (!fileNameUpper.Contains("RR01"))
                {
                    _logger.LogWarning($"Filename '{fileName}' không chứa 'RR01' identifier");
                    return null;
                }

                // Pattern: xxxx_rr01_YYYYMMDD.csv
                var parts = fileName.Split('_');
                var datePart = parts.LastOrDefault()?.Split('.').FirstOrDefault();

                if (datePart != null && datePart.Length == 8)
                {
                    if (DateTime.TryParseExact(datePart, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                    {
                        _logger.LogInformation($"Extracted NGAY_DL: {result:yyyy-MM-dd} từ filename: {fileName}");
                        return result;
                    }
                }

                _logger.LogWarning($"Không thể extract date từ filename: {fileName}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi extract date từ filename: {fileName}");
                return null;
            }
        }

        /// <summary>
        /// Validate filename theo RR01 business rules
        /// </summary>
        public bool ValidateFileName(string fileName)
        {
            var fileNameUpper = fileName.ToUpper();
            return fileNameUpper.Contains("RR01") && 
                   (fileNameUpper.EndsWith(".CSV") || fileNameUpper.EndsWith(".TXT"));
        }

        /// <summary>
        /// Parse CSV với 25 business columns theo exact header names
        /// </summary>
        public async Task<List<RR01>> ParseGenericCSVAsync(string csvFilePath, DateTime ngayDl)
        {
            var entities = new List<RR01>();

            try
            {
                using var reader = new StreamReader(csvFilePath);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                // Read header để verify columns
                csv.Read();
                csv.ReadHeader();
                var headerRecord = csv.HeaderRecord;

                if (headerRecord == null)
                {
                    throw new InvalidOperationException("CSV file không có header row");
                }

                _logger.LogInformation($"CSV Headers found: {string.Join(", ", headerRecord)}");

                // Verify required columns tồn tại
                var requiredColumns = new[] { 
                    "CN_LOAI_I", "BRCD", "MA_KH", "TEN_KH", "SO_LDS", "CCY", 
                    "DUNO_GOC_HIENTAI", "DUNO_LAI_HIENTAI", "THU_GOC", "THU_LAI"
                };

                foreach (var col in requiredColumns)
                {
                    if (!headerRecord.Contains(col))
                    {
                        _logger.LogWarning($"Required column '{col}' không tìm thấy trong CSV");
                    }
                }

                // Parse từng row
                var rowNumber = 1;
                while (csv.Read())
                {
                    try
                    {
                        var entity = new RR01
                        {
                            NGAY_DL = ngayDl,
                            
                            // Parse 25 business columns với safe casting
                            CN_LOAI_I = GetCsvValue(csv, "CN_LOAI_I"),
                            BRCD = GetCsvValue(csv, "BRCD"),
                            MA_KH = GetCsvValue(csv, "MA_KH"),
                            TEN_KH = GetCsvValue(csv, "TEN_KH"),
                            SO_LDS = GetCsvValue(csv, "SO_LDS"),
                            CCY = GetCsvValue(csv, "CCY"),
                            SO_LAV = GetCsvValue(csv, "SO_LAV"),
                            LOAI_KH = GetCsvValue(csv, "LOAI_KH"),
                            NGAY_GIAI_NGAN = GetCsvDateTime(csv, "NGAY_GIAI_NGAN"),
                            NGAY_DEN_HAN = GetCsvDateTime(csv, "NGAY_DEN_HAN"),
                            VAMC_FLG = GetCsvValue(csv, "VAMC_FLG"),
                            NGAY_XLRR = GetCsvDateTime(csv, "NGAY_XLRR"),
                            DUNO_GOC_BAN_DAU = GetCsvDecimal(csv, "DUNO_GOC_BAN_DAU"),
                            DUNO_LAI_TICHLUY_BD = GetCsvDecimal(csv, "DUNO_LAI_TICHLUY_BD"),
                            DOC_DAUKY_DA_THU_HT = GetCsvDecimal(csv, "DOC_DAUKY_DA_THU_HT"),
                            DUNO_GOC_HIENTAI = GetCsvDecimal(csv, "DUNO_GOC_HIENTAI"),
                            DUNO_LAI_HIENTAI = GetCsvDecimal(csv, "DUNO_LAI_HIENTAI"),
                            DUNO_NGAN_HAN = GetCsvDecimal(csv, "DUNO_NGAN_HAN"),
                            DUNO_TRUNG_HAN = GetCsvDecimal(csv, "DUNO_TRUNG_HAN"),
                            DUNO_DAI_HAN = GetCsvDecimal(csv, "DUNO_DAI_HAN"),
                            THU_GOC = GetCsvDecimal(csv, "THU_GOC"),
                            THU_LAI = GetCsvDecimal(csv, "THU_LAI"),
                            BDS = GetCsvDecimal(csv, "BDS"),
                            DS = GetCsvDecimal(csv, "DS"),
                            TSK = GetCsvDecimal(csv, "TSK"),

                            // System fields
                            CREATED_DATE = DateTime.UtcNow
                        };

                        entities.Add(entity);
                        rowNumber++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Lỗi parse row {rowNumber}: {ex.Message}");
                        rowNumber++;
                        continue;
                    }
                }

                _logger.LogInformation($"Parsed {entities.Count} RR01 records từ CSV");
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi parse CSV file: {csvFilePath}");
                throw;
            }
        }

        /// <summary>
        /// Bulk insert với performance optimization
        /// </summary>
        public async Task<RR01ImportResultDto> BulkInsertGenericAsync(List<RR01> entities, string fileName)
        {
            var result = new RR01ImportResultDto
            {
                TotalRows = entities.Count,
                ProcessingStartTime = DateTime.UtcNow,
                FileName = fileName,
                ExtractedNGAY_DL = entities.FirstOrDefault()?.NGAY_DL
            };

            try
            {
                if (!entities.Any())
                {
                    result.Errors.Add("Không có records để import");
                    result.ProcessingEndTime = DateTime.UtcNow;
                    return result;
                }

                // Delete data cũ cùng NGAY_DL (nếu re-import)
                var ngayDl = entities.First().NGAY_DL;
                var existingRecords = await _context.RR01
                    .Where(x => x.NGAY_DL.Date == ngayDl.Date)
                    .ToListAsync();

                if (existingRecords.Any())
                {
                    _logger.LogInformation($"Hard delete {existingRecords.Count} existing records cho ngày {ngayDl:yyyy-MM-dd}");
                    _context.RR01.RemoveRange(existingRecords);
                    await _context.SaveChangesAsync();
                }

                // Bulk insert với batches
                const int batchSize = 1000;
                var batches = entities.Chunk(batchSize);
                var successCount = 0;
                var failCount = 0;

                foreach (var batch in batches)
                {
                    try
                    {
                        await _context.RR01.AddRangeAsync(batch);
                        await _context.SaveChangesAsync();
                        successCount += batch.Count();
                        result.BatchCount++;

                        _logger.LogDebug($"Inserted batch {result.BatchCount} - {batch.Count()} records");
                    }
                    catch (Exception ex)
                    {
                        failCount += batch.Count();
                        result.Errors.Add($"Batch {result.BatchCount + 1} failed: {ex.Message}");
                        _logger.LogError(ex, $"Batch insert failed cho batch {result.BatchCount + 1}");
                    }
                }

                result.SuccessfulRows = successCount;
                result.FailedRows = failCount;
                result.ProcessingEndTime = DateTime.UtcNow;

                // Calculate business metrics
                var processedEntities = entities.Take(successCount);
                result.RecordsWithVAMCFlag = processedEntities.Count(x => !string.IsNullOrEmpty(x.VAMC_FLG) && x.VAMC_FLG.ToUpper() == "Y");
                result.RecordsWithRecovery = processedEntities.Count(x => (x.THU_GOC ?? 0) > 0 || (x.THU_LAI ?? 0) > 0);
                result.TotalProcessedAmount = processedEntities.Sum(x => (x.DUNO_GOC_HIENTAI ?? 0) + (x.DUNO_LAI_HIENTAI ?? 0));

                _logger.LogInformation($"RR01 Import completed: {successCount}/{entities.Count} successful, Duration: {result.ProcessingDuration.TotalSeconds:F2}s");

                return result;
            }
            catch (Exception ex)
            {
                result.ProcessingEndTime = DateTime.UtcNow;
                result.Errors.Add($"Critical error: {ex.Message}");
                _logger.LogError(ex, "Critical error trong RR01 bulk insert");
                return result;
            }
        }

        /// <summary>
        /// Get RR01 records với filtering và pagination
        /// </summary>
        public async Task<IEnumerable<RR01PreviewDto>> GetRR01PreviewAsync(DateTime? ngayDl = null, string? branchCode = null, int skip = 0, int take = 100)
        {
            var query = _context.RR01.AsQueryable();

            if (ngayDl.HasValue)
                query = query.Where(x => x.NGAY_DL.Date == ngayDl.Value.Date);

            if (!string.IsNullOrEmpty(branchCode))
                query = query.Where(x => x.BRCD == branchCode);

            return await query
                .OrderByDescending(x => x.NGAY_DL)
                .ThenBy(x => x.BRCD)
                .Skip(skip)
                .Take(take)
                .Select(x => new RR01PreviewDto
                {
                    Id = x.Id,
                    NGAY_DL = x.NGAY_DL,
                    CN_LOAI_I = x.CN_LOAI_I,
                    BRCD = x.BRCD,
                    MA_KH = x.MA_KH,
                    TEN_KH = x.TEN_KH,
                    SO_LDS = x.SO_LDS,
                    CCY = x.CCY,
                    DUNO_GOC_HIENTAI = x.DUNO_GOC_HIENTAI,
                    DUNO_LAI_HIENTAI = x.DUNO_LAI_HIENTAI,
                    THU_GOC = x.THU_GOC,
                    THU_LAI = x.THU_LAI,
                    BDS = x.BDS,
                    DS = x.DS,
                    CREATED_DATE = x.CREATED_DATE
                })
                .ToListAsync();
        }

        /// <summary>
        /// Get RR01 details by ID
        /// </summary>
        public async Task<RR01DetailsDto?> GetRR01ByIdAsync(int id)
        {
            var entity = await _context.RR01
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (entity == null) return null;

            return new RR01DetailsDto
            {
                Id = entity.Id,
                NGAY_DL = entity.NGAY_DL,
                CN_LOAI_I = entity.CN_LOAI_I,
                BRCD = entity.BRCD,
                MA_KH = entity.MA_KH,
                TEN_KH = entity.TEN_KH,
                SO_LDS = entity.SO_LDS,
                CCY = entity.CCY,
                SO_LAV = entity.SO_LAV,
                LOAI_KH = entity.LOAI_KH,
                NGAY_GIAI_NGAN = entity.NGAY_GIAI_NGAN,
                NGAY_DEN_HAN = entity.NGAY_DEN_HAN,
                VAMC_FLG = entity.VAMC_FLG,
                NGAY_XLRR = entity.NGAY_XLRR,
                DUNO_GOC_BAN_DAU = entity.DUNO_GOC_BAN_DAU,
                DUNO_LAI_TICHLUY_BD = entity.DUNO_LAI_TICHLUY_BD,
                DOC_DAUKY_DA_THU_HT = entity.DOC_DAUKY_DA_THU_HT,
                DUNO_GOC_HIENTAI = entity.DUNO_GOC_HIENTAI,
                DUNO_LAI_HIENTAI = entity.DUNO_LAI_HIENTAI,
                DUNO_NGAN_HAN = entity.DUNO_NGAN_HAN,
                DUNO_TRUNG_HAN = entity.DUNO_TRUNG_HAN,
                DUNO_DAI_HAN = entity.DUNO_DAI_HAN,
                THU_GOC = entity.THU_GOC,
                THU_LAI = entity.THU_LAI,
                BDS = entity.BDS,
                DS = entity.DS,
                TSK = entity.TSK,
                CREATED_DATE = entity.CREATED_DATE,
                UPDATED_DATE = entity.UPDATED_DATE
            };
        }

        /// <summary>
        /// Generate RR01 summary analytics
        /// </summary>
        public async Task<RR01SummaryDto> GetRR01SummaryAsync(DateTime ngayDl)
        {
            var records = await _context.RR01
                .Where(x => x.NGAY_DL.Date == ngayDl.Date)
                .ToListAsync();

            return new RR01SummaryDto
            {
                NGAY_DL = ngayDl,
                TotalRecords = records.Count,
                UniqueBranches = records.Select(x => x.BRCD).Distinct().Count(),
                UniqueCustomers = records.Select(x => x.MA_KH).Distinct().Count(),
                TotalDUNO_GOC_HIENTAI = records.Sum(x => x.DUNO_GOC_HIENTAI ?? 0),
                TotalDUNO_LAI_HIENTAI = records.Sum(x => x.DUNO_LAI_HIENTAI ?? 0),
                TotalTHU_GOC = records.Sum(x => x.THU_GOC ?? 0),
                TotalTHU_LAI = records.Sum(x => x.THU_LAI ?? 0),
                TotalBDS = records.Sum(x => x.BDS ?? 0),
                TotalDS = records.Sum(x => x.DS ?? 0),
                TotalTSK = records.Sum(x => x.TSK ?? 0)
            };
        }

        // === HELPER METHODS ===
        private string? GetCsvValue(CsvReader csv, string fieldName)
        {
            return csv.TryGetField(fieldName, out string? value) ? value?.Trim() : null;
        }

        private DateTime? GetCsvDateTime(CsvReader csv, string fieldName)
        {
            if (!csv.TryGetField(fieldName, out string? value) || string.IsNullOrWhiteSpace(value))
                return null;

            // Try multiple date formats
            var formats = new[] { "yyyyMMdd", "yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy" };
            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(value.Trim(), format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                    return result;
            }

            if (DateTime.TryParse(value.Trim(), out DateTime fallbackResult))
                return fallbackResult;

            return null;
        }

        private decimal? GetCsvDecimal(CsvReader csv, string fieldName)
        {
            if (!csv.TryGetField(fieldName, out string? value) || string.IsNullOrWhiteSpace(value))
                return null;

            // Remove commas and parse
            var cleanValue = value.Trim().Replace(",", "");
            return decimal.TryParse(cleanValue, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result) ? result : null;
        }
    }
}
