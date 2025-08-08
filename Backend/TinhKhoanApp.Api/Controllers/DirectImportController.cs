using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Threading.Channels;
using TinhKhoanApp.Api.Helpers;
using TinhKhoanApp.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// Controller cho Direct Import - Import trực tiếp vào bảng riêng biệt
    /// Hiệu năng cao với SqlBulkCopy, bỏ qua các bước xử lý trung gian
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DirectImportController : ControllerBase
    {
        private readonly IDirectImportService _directImportService;
        private readonly ILogger<DirectImportController> _logger;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        public DirectImportController(
            IDirectImportService directImportService,
            ILogger<DirectImportController> logger,
            ApplicationDbContext context)
        {
            _directImportService = directImportService;
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Import thông minh - tự động phát hiện loại file và import trực tiếp
        /// </summary>
        /// <param name="file">File dữ liệu CSV/Excel</param>
        /// <param name="statementDate">Ngày báo cáo (optional)</param>
        /// <returns>Kết quả import với thông tin chi tiết</returns>
        [HttpPost("smart")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<object>> ImportSmartDirect(
            IFormFile file,
            [FromQuery] string? statementDate = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("File không được để trống");
                }

                // 🎯 AUTO-ROUTE GL01 to OPTIMIZED WORKFLOW
                var dataType = _directImportService.DetectDataTypeFromFileName(file.FileName);
                if (dataType == "GL01")
                {
                    _logger.LogInformation("🔄 [SMART_ROUTING] Auto-routing GL01 to optimized workflow: {FileName}", file.FileName);
                    var optimizedResult = await OptimizedGL01Import(file, statementDate);
                    return optimizedResult;
                }

                _logger.LogInformation("🚀 [DIRECT_IMPORT_API] Smart import start: {FileName}, Size: {FileSize}MB",
                    file.FileName, file.Length / 1024.0 / 1024.0);

                var result = await _directImportService.ImportSmartDirectAsync(file, statementDate);

                if (result.Success)
                {
                    _logger.LogInformation("✅ [DIRECT_IMPORT_API] Smart import success: {Records} records in {Duration}ms",
                        result.ProcessedRecords, result.Duration.TotalMilliseconds);
                    return Ok(result);
                }
                else
                {
                    _logger.LogError("❌ [DIRECT_IMPORT_API] Smart import failed: {Error}", result.ErrorMessage);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 [DIRECT_IMPORT_API] Smart import exception: {FileName}", file?.FileName);
                return StatusCode(500, new DirectImportResult
                {
                    Success = false,
                    ErrorMessage = $"Lỗi hệ thống: {ex.Message}",
                    FileName = file?.FileName ?? "Unknown"
                });
            }
        }

        /// <summary>
        /// 🚀 SMART GL01 IMPORT - Tự động quản lý columnstore index cho optimal performance
        /// Workflow: Disable columnstore → Import → Re-enable columnstore
        /// </summary>
        /// <param name="file">File GL01 CSV</param>
        /// <param name="statementDate">Ngày báo cáo (optional)</param>
        /// <param name="skipColumnstoreManagement">Bỏ qua tự động quản lý columnstore index</param>
        /// <returns>Kết quả import với thông tin chi tiết về performance</returns>
        [HttpPost("smart-gl01")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<object>> SmartGL01Import(
            IFormFile file,
            [FromQuery] string? statementDate = null,
            [FromQuery] bool skipColumnstoreManagement = false)
        {
            var performanceLog = new List<string>();
            var startTime = DateTime.Now;

            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("File không được để trống");
                }

                // Detect data type
                var dataType = _directImportService.DetectDataTypeFromFileName(file.FileName);
                if (dataType != "GL01")
                {
                    return BadRequest($"File không phải GL01. Detected type: {dataType}. Sử dụng endpoint /smart cho các loại file khác.");
                }

                var fileSizeMB = file.Length / 1024.0 / 1024.0;
                var isLargeFile = fileSizeMB > 10; // Files > 10MB được coi là lớn

                _logger.LogInformation("🚀 [SMART_GL01] Starting smart GL01 import: {FileName}, Size: {FileSize}MB, Large: {IsLarge}",
                    file.FileName, fileSizeMB, isLargeFile);

                performanceLog.Add($"⏱️ Start time: {startTime:HH:mm:ss.fff}");
                performanceLog.Add($"📂 File: {file.FileName} ({fileSizeMB:F2} MB)");
                performanceLog.Add($"📊 Large file mode: {isLargeFile}");

                bool columnstoreWasDisabled = false;
                bool columnstoreExisted = false;

                // STEP 1: Check và disable columnstore index nếu cần
                if (!skipColumnstoreManagement && isLargeFile)
                {
                    var checkIndexTime = DateTime.Now;
                    var indexInfo = await CheckGL01IndexesInternal();
                    columnstoreExisted = indexInfo.Any(i => i.IndexType?.Contains("COLUMNSTORE") == true);

                    if (columnstoreExisted)
                    {
                        performanceLog.Add($"🔍 Found columnstore index at {checkIndexTime:HH:mm:ss.fff}");

                        var disableTime = DateTime.Now;
                        await DisableColumnstoreIndexInternal();
                        columnstoreWasDisabled = true;

                        performanceLog.Add($"🔧 Disabled columnstore at {disableTime:HH:mm:ss.fff} (took {(DateTime.Now - disableTime).TotalMilliseconds:F0}ms)");
                        _logger.LogInformation("🔧 [SMART_GL01] Disabled columnstore index for large file import");
                    }
                    else
                    {
                        performanceLog.Add($"✅ No columnstore index found - proceeding with normal import");
                    }
                }

                // STEP 2: Perform the actual import
                var importStartTime = DateTime.Now;
                performanceLog.Add($"🚀 Starting import at {importStartTime:HH:mm:ss.fff}");

                var result = await _directImportService.ImportSmartDirectAsync(file, statementDate);

                var importEndTime = DateTime.Now;
                var importDuration = importEndTime - importStartTime;
                performanceLog.Add($"✅ Import completed at {importEndTime:HH:mm:ss.fff} (took {importDuration.TotalSeconds:F2}s)");

                // STEP 3: Re-enable columnstore index nếu đã disable
                if (columnstoreWasDisabled && !skipColumnstoreManagement)
                {
                    var enableStartTime = DateTime.Now;
                    performanceLog.Add($"🔄 Re-enabling columnstore at {enableStartTime:HH:mm:ss.fff}");

                    // Enable columnstore trong background để không block response
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await EnableColumnstoreIndexInternal();
                            var enableEndTime = DateTime.Now;
                            _logger.LogInformation("🔧 [SMART_GL01] Re-enabled columnstore index in background (took {Duration}ms)",
                                (enableEndTime - enableStartTime).TotalMilliseconds);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "❌ [SMART_GL01] Failed to re-enable columnstore index in background");
                        }
                    });

                    performanceLog.Add($"🔄 Columnstore re-enable started in background");
                }

                var totalTime = DateTime.Now - startTime;
                performanceLog.Add($"🏁 Total workflow time: {totalTime.TotalSeconds:F2}s");

                if (result.Success)
                {
                    var response = new
                    {
                        Success = result.Success,
                        FileName = result.FileName,
                        DataType = result.DataType,
                        TargetTable = result.TargetTable,
                        FileSizeBytes = result.FileSizeBytes,
                        FileSizeMB = fileSizeMB,
                        ProcessedRecords = result.ProcessedRecords,
                        ErrorRecords = result.ErrorRecords,
                        NgayDL = result.NgayDL,
                        BatchId = result.BatchId,
                        ImportedDataRecordId = result.ImportedDataRecordId,
                        StartTime = result.StartTime,
                        EndTime = result.EndTime,
                        Duration = result.Duration,
                        RecordsPerSecond = result.RecordsPerSecond,
                        MBPerSecond = result.MBPerSecond,

                        // Smart workflow specific info
                        SmartWorkflow = new
                        {
                            IsLargeFile = isLargeFile,
                            ColumnstoreManagement = new
                            {
                                Enabled = !skipColumnstoreManagement,
                                ColumnstoreExisted = columnstoreExisted,
                                WasDisabled = columnstoreWasDisabled,
                                ReEnabledInBackground = columnstoreWasDisabled
                            },
                            TotalWorkflowTime = totalTime,
                            ImportOnlyTime = importDuration,
                            PerformanceLog = performanceLog
                        }
                    };

                    _logger.LogInformation("✅ [SMART_GL01] Workflow completed successfully: {Records} records in {Duration}ms",
                        result.ProcessedRecords, totalTime.TotalMilliseconds);

                    return Ok(response);
                }
                else
                {
                    _logger.LogError("❌ [SMART_GL01] Import failed: {Error}", result.ErrorMessage);
                    return BadRequest(new
                    {
                        Success = false,
                        Error = result.ErrorMessage,
                        SmartWorkflow = new { PerformanceLog = performanceLog }
                    });
                }
            }
            catch (Exception ex)
            {
                var errorTime = DateTime.Now;
                performanceLog.Add($"💥 Error at {errorTime:HH:mm:ss.fff}: {ex.Message}");

                _logger.LogError(ex, "💥 [SMART_GL01] Smart workflow exception: {FileName}", file?.FileName);
                return StatusCode(500, new
                {
                    Success = false,
                    Error = $"Lỗi hệ thống: {ex.Message}",
                    FileName = file?.FileName ?? "Unknown",
                    SmartWorkflow = new { PerformanceLog = performanceLog }
                });
            }
        }

        /// <summary>
        /// Import trực tiếp file DP01 vào bảng DP01
        /// </summary>
        [HttpPost("dp01")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<DirectImportResult>> ImportDP01Direct(
            IFormFile file,
            [FromQuery] string? statementDate = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("File không được để trống");
                }

                _logger.LogInformation("🚀 [DP01_DIRECT_API] Import start: {FileName}", file.FileName);

                var result = await _directImportService.ImportDP01DirectAsync(file, statementDate);

                if (result.Success)
                {
                    _logger.LogInformation("✅ [DP01_DIRECT_API] Import success: {Records} records", result.ProcessedRecords);
                    return Ok(result);
                }
                else
                {
                    _logger.LogError("❌ [DP01_DIRECT_API] Import failed: {Error}", result.ErrorMessage);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 [DP01_DIRECT_API] Import exception: {FileName}", file?.FileName);
                return StatusCode(500, new DirectImportResult
                {
                    Success = false,
                    ErrorMessage = $"Lỗi hệ thống: {ex.Message}",
                    FileName = file?.FileName ?? "Unknown"
                });
            }
        }

        /// <summary>
        /// Import trực tiếp file LN01 vào bảng LN01
        /// </summary>
        [HttpPost("ln01")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<DirectImportResult>> ImportLN01Direct(
            IFormFile file,
            [FromQuery] string? statementDate = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("File không được để trống");
                }

                var result = await _directImportService.ImportLN01DirectAsync(file, statementDate);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 [LN01_DIRECT_API] Import exception: {FileName}", file?.FileName);
                return StatusCode(500, new DirectImportResult
                {
                    Success = false,
                    ErrorMessage = $"Lỗi hệ thống: {ex.Message}",
                    FileName = file?.FileName ?? "Unknown"
                });
            }
        }

        /// <summary>
        /// Lấy thông tin trạng thái import
        /// </summary>
        [HttpGet("status")]
        public async Task<ActionResult<object>> GetImportStatus()
        {
            try
            {
                // TODO: Implement status tracking
                return Ok(new
                {
                    Status = "Direct Import System Online",
                    Version = "1.0.0",
                    Features = new[]
                    {
                        "✅ Smart Detection - Tự động phát hiện loại file",
                        "✅ SqlBulkCopy - Tăng tốc import 2-5x",
                        "✅ Temporal Tables - Lịch sử thay đổi tự động",
                        "✅ Columnstore Indexes - Hiệu năng query 10-100x",
                        "✅ Metadata Tracking - Chỉ lưu metadata, không lưu raw data"
                    },
                    SupportedDataTypes = new[]
                    {
                        "DP01 - Báo cáo tài chính",
                        "LN01 - Dữ liệu cho vay",
                        "GL01 - Dữ liệu sổ cái",
                        "GL41 - Dữ liệu giao dịch",
                        "DPDA - Dữ liệu phân tích",
                        "EI01 - Dữ liệu lãi suất",
                        "RR01 - Dữ liệu rủi ro",
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 [DIRECT_IMPORT_API] Status check exception");
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy số lượng records thực tế từ database tables (không phải metadata)
        /// </summary>
        [HttpGet("table-counts")]
        public async Task<ActionResult<Dictionary<string, int>>> GetTableCounts()
        {
            try
            {
                _logger.LogInformation("📊 Getting actual table record counts");

                var counts = await _directImportService.GetTableRecordCountsAsync();

                return Ok(counts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting table counts");
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        /// <summary>
        /// 🚀 STREAMING UPLOAD - Import file lớn bằng streaming (hàng trăm MB)
        /// Không load toàn bộ file vào memory, stream trực tiếp vào database
        /// </summary>
        [HttpPost("stream")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = long.MaxValue)]
        public async Task<IActionResult> StreamImport()
        {
            try
            {
                _logger.LogInformation("🚀 [STREAM_IMPORT] Starting streaming upload");

                if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
                {
                    return BadRequest("Request must be multipart/form-data");
                }

                var boundary = MultipartRequestHelper.GetBoundary(
                    MediaTypeHeaderValue.Parse(Request.ContentType),
                    70);

                var reader = new MultipartReader(boundary, HttpContext.Request.Body);
                var section = await reader.ReadNextSectionAsync();

                while (section != null)
                {
                    var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(
                        section.ContentDisposition, out var contentDisposition);

                    if (hasContentDispositionHeader &&
                        contentDisposition.DispositionType.Equals("form-data") &&
                        !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                    {
                        var fileName = contentDisposition.FileName.Value.Trim('"');
                        var dataType = _directImportService.DetectDataTypeFromFileName(fileName);

                        _logger.LogInformation("📂 [STREAM_IMPORT] Processing file: {FileName}, Type: {DataType}",
                            fileName, dataType);

                        // Stream trực tiếp vào database
                        var result = await _directImportService.StreamImportAsync(
                            section.Body, fileName, dataType);

                        return Ok(result);
                    }

                    section = await reader.ReadNextSectionAsync();
                }

                return BadRequest("No file found in request");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [STREAM_IMPORT] Stream import error");
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    type = "STREAM_IMPORT_ERROR"
                });
            }
        }

        /// <summary>
        /// 🆔 Tạo session cho chunked upload
        /// </summary>
        [HttpPost("create-session")]
        public async Task<IActionResult> CreateUploadSession([FromBody] CreateSessionRequest request)
        {
            try
            {
                var sessionId = Guid.NewGuid().ToString();

                // Store session info (có thể dùng Redis hoặc database)
                var sessionInfo = new UploadSession
                {
                    SessionId = sessionId,
                    FileName = request.FileName,
                    FileSize = request.FileSize,
                    TotalChunks = request.TotalChunks,
                    DataType = request.DataType,
                    CreatedAt = DateTime.UtcNow,
                    UploadedChunks = new List<int>()
                };

                // Lưu session vào cache/database
                await _directImportService.CreateUploadSessionAsync(sessionInfo);

                _logger.LogInformation("🆔 [CREATE_SESSION] Created session {SessionId} for file {FileName}",
                    sessionId, request.FileName);

                return Ok(new { sessionId, success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [CREATE_SESSION] Error creating upload session");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// 📤 Upload chunk
        /// </summary>
        [HttpPost("upload-chunk")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadChunk([FromForm] UploadChunkRequest request)
        {
            try
            {
                var result = await _directImportService.UploadChunkAsync(
                    request.SessionId,
                    request.ChunkIndex,
                    request.Chunk);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [UPLOAD_CHUNK] Error uploading chunk {ChunkIndex} for session {SessionId}",
                    request.ChunkIndex, request.SessionId);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// ✅ Finalize chunked upload và process file
        /// </summary>
        [HttpPost("finalize/{sessionId}")]
        public async Task<IActionResult> FinalizeUpload(string sessionId)
        {
            try
            {
                var result = await _directImportService.FinalizeUploadAsync(sessionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [FINALIZE_UPLOAD] Error finalizing upload for session {SessionId}", sessionId);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// 📊 Get upload info (for resume functionality)
        /// </summary>
        [HttpGet("upload-info/{sessionId}")]
        public async Task<IActionResult> GetUploadInfo(string sessionId)
        {
            try
            {
                var info = await _directImportService.GetUploadInfoAsync(sessionId);
                return Ok(info);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [UPLOAD_INFO] Error getting upload info for session {SessionId}", sessionId);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// 🚫 Cancel upload session
        /// </summary>
        [HttpDelete("cancel/{sessionId}")]
        public async Task<IActionResult> CancelUpload(string sessionId)
        {
            try
            {
                await _directImportService.CancelUploadAsync(sessionId);
                return Ok(new { success = true, message = "Upload cancelled" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [CANCEL_UPLOAD] Error cancelling upload for session {SessionId}", sessionId);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// 🔄 PARALLEL CHUNKED UPLOAD - Xử lý nhiều chunks song song
        /// </summary>
        [HttpPost("parallel")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> ParallelImport(IFormFile file, [FromQuery] int chunkSize = 50000)
        {
            try
            {
                var dataType = _directImportService.DetectDataTypeFromFileName(file.FileName);

                var result = await _directImportService.ParallelImportAsync(
                    file.OpenReadStream(),
                    file.FileName,
                    dataType,
                    chunkSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [PARALLEL_IMPORT] Error in parallel import");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Kiểm tra dữ liệu GL02 - CRTDTM parsing validation
        /// </summary>
        [HttpGet("gl02/validate")]
        public async Task<ActionResult> ValidateGL02Data()
        {
            try
            {
                var validationResult = await _directImportService.ValidateGL02DataAsync();
                return Ok(validationResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [GL02_VALIDATE] Error validating GL02 data");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Check GL01 table indexes - Debug performance issues
        /// </summary>
        [HttpGet("debug/gl01-indexes")]
        public async Task<ActionResult> CheckGL01Indexes()
        {
            try
            {
                var sql = @"
                    SELECT
                        i.name as IndexName,
                        i.type_desc as IndexType,
                        i.is_unique as IsUnique,
                        i.is_primary_key as IsPrimaryKey,
                        STRING_AGG(c.name, ', ') as Columns
                    FROM sys.indexes i
                    JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                    JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                    WHERE i.object_id = OBJECT_ID('GL01')
                    GROUP BY i.name, i.type_desc, i.is_unique, i.is_primary_key, i.index_id
                    ORDER BY i.index_id;";

                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = sql;

                var indexes = new List<object>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    indexes.Add(new
                    {
                        IndexName = reader["IndexName"]?.ToString(),
                        IndexType = reader["IndexType"]?.ToString(),
                        IsUnique = reader["IsUnique"],
                        IsPrimaryKey = reader["IsPrimaryKey"],
                        Columns = reader["Columns"]?.ToString()
                    });
                }

                return Ok(new
                {
                    success = true,
                    indexCount = indexes.Count,
                    indexes = indexes
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [DEBUG] Error checking GL01 indexes");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Disable columnstore index for bulk insert performance
        /// </summary>
        [HttpPost("debug/disable-columnstore")]
        public async Task<ActionResult> DisableColumnstoreIndex()
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "DROP INDEX NCCI_GL01_Analytics ON GL01;";

                await command.ExecuteNonQueryAsync();

                _logger.LogInformation("🔧 [DEBUG] Disabled columnstore index NCCI_GL01_Analytics");

                return Ok(new
                {
                    success = true,
                    message = "Columnstore index disabled for bulk insert performance",
                    warning = "Remember to re-enable after bulk import!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [DEBUG] Error disabling columnstore index");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Re-enable columnstore index after bulk insert
        /// </summary>
        [HttpPost("debug/enable-columnstore")]
        public async Task<ActionResult> EnableColumnstoreIndex()
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_GL01_Analytics
                    ON GL01 (
                        NGAY_DL, STS, NGAY_GD, NGUOI_TAO, DYSEQ, TR_TYPE, DT_SEQ, TAI_KHOAN, TEN_TK,
                        SO_TIEN_GD, POST_BR, LOAI_TIEN, DR_CR, MA_KH, TEN_KH, CCA_USRID, TR_EX_RT,
                        REMARK, BUS_CODE, UNIT_BUS_CODE, TR_CODE, TR_NAME, REFERENCE, VALUE_DATE,
                        DEPT_CODE, TR_TIME, COMFIRM, TRDT_TIME, CREATED_DATE, UPDATED_DATE, FILE_NAME
                    );";

                await command.ExecuteNonQueryAsync();

                _logger.LogInformation("🔧 [DEBUG] Re-enabled columnstore index NCCI_GL01_Analytics");

                return Ok(new
                {
                    success = true,
                    message = "Columnstore index re-enabled for analytics performance"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [DEBUG] Error enabling columnstore index");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// 🎯 OPTIMIZED GL01 BULK IMPORT - Using SQL Server bulk optimization hints
        /// Faster alternative to dropping/recreating columnstore index
        /// </summary>
        [HttpPost("optimized-gl01")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<object>> OptimizedGL01Import(
            IFormFile file,
            [FromQuery] string? statementDate = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("File không được để trống");
                }

                // Detect data type
                var dataType = _directImportService.DetectDataTypeFromFileName(file.FileName);
                if (dataType != "GL01")
                {
                    return BadRequest($"File không phải GL01. Detected type: {dataType}");
                }

                var fileSizeMB = file.Length / 1024.0 / 1024.0;
                _logger.LogInformation("🎯 [OPTIMIZED_GL01] Starting optimized bulk import: {FileName}, Size: {FileSize}MB",
                    file.FileName, fileSizeMB);

                var startTime = DateTime.Now;

                // OPTIMIZATION 1: Disable auto-update statistics during bulk import
                await ExecuteOptimizationCommand("ALTER INDEX ALL ON GL01 SET (STATISTICS_NORECOMPUTE = ON);");

                // OPTIMIZATION 2: Set bulk import options
                await ExecuteOptimizationCommand("ALTER DATABASE TinhKhoanDB SET AUTO_UPDATE_STATISTICS OFF;");

                // Perform the import with optimized settings
                var result = await _directImportService.ImportSmartDirectAsync(file, statementDate);

                // OPTIMIZATION 3: Re-enable statistics (in background)
                _ = Task.Run(async () =>
                {
                    await Task.Delay(1000); // Wait for import to complete
                    await ExecuteOptimizationCommand("ALTER DATABASE TinhKhoanDB SET AUTO_UPDATE_STATISTICS ON;");
                    await ExecuteOptimizationCommand("ALTER INDEX ALL ON GL01 SET (STATISTICS_NORECOMPUTE = OFF);");
                    await ExecuteOptimizationCommand("UPDATE STATISTICS GL01;");
                });

                var endTime = DateTime.Now;
                var duration = endTime - startTime;

                if (result.Success)
                {
                    var response = new
                    {
                        Success = result.Success,
                        FileName = result.FileName,
                        DataType = result.DataType,
                        TargetTable = result.TargetTable,
                        FileSizeBytes = result.FileSizeBytes,
                        FileSizeMB = fileSizeMB,
                        ProcessedRecords = result.ProcessedRecords,
                        ErrorRecords = result.ErrorRecords,
                        NgayDL = result.NgayDL,
                        BatchId = result.BatchId,
                        ImportedDataRecordId = result.ImportedDataRecordId,
                        StartTime = result.StartTime,
                        EndTime = result.EndTime,
                        Duration = result.Duration,
                        RecordsPerSecond = result.RecordsPerSecond,
                        MBPerSecond = result.MBPerSecond,

                        OptimizationInfo = new
                        {
                            Method = "SQL Server Bulk Optimization Hints",
                            StatisticsDisabled = true,
                            ColumnstoreIntact = true,
                            Note = "Statistics re-enabled in background"
                        }
                    };

                    _logger.LogInformation("✅ [OPTIMIZED_GL01] Import completed: {Records} records in {Duration}s",
                        result.ProcessedRecords, duration.TotalSeconds);

                    return Ok(response);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [OPTIMIZED_GL01] Error in optimized import");
                return StatusCode(500, new { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// Helper method to execute optimization commands
        /// </summary>
        private async Task ExecuteOptimizationCommand(string sql)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = sql;
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ [OPTIMIZATION] Failed to execute: {SQL}", sql);
            }
        }

        /// <summary>
        /// Internal method to check GL01 indexes
        /// </summary>
        private async Task<List<dynamic>> CheckGL01IndexesInternal()
        {
            var sql = @"
                SELECT
                    i.name as IndexName,
                    i.type_desc as IndexType,
                    i.is_unique as IsUnique,
                    i.is_primary_key as IsPrimaryKey,
                    STRING_AGG(c.name, ', ') as Columns
                FROM sys.indexes i
                JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                WHERE i.object_id = OBJECT_ID('GL01')
                GROUP BY i.name, i.type_desc, i.is_unique, i.is_primary_key, i.index_id
                ORDER BY i.index_id;";

            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var indexes = new List<dynamic>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                indexes.Add(new
                {
                    IndexName = reader["IndexName"]?.ToString(),
                    IndexType = reader["IndexType"]?.ToString(),
                    IsUnique = reader["IsUnique"],
                    IsPrimaryKey = reader["IsPrimaryKey"],
                    Columns = reader["Columns"]?.ToString()
                });
            }

            return indexes;
        }

        /// <summary>
        /// Internal method to disable columnstore index
        /// </summary>
        private async Task DisableColumnstoreIndexInternal()
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "DROP INDEX NCCI_GL01_Analytics ON GL01;";

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Internal method to enable columnstore index
        /// </summary>
        private async Task EnableColumnstoreIndexInternal()
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_GL01_Analytics
                ON GL01 (
                    NGAY_DL, STS, NGAY_GD, NGUOI_TAO, DYSEQ, TR_TYPE, DT_SEQ, TAI_KHOAN, TEN_TK,
                    SO_TIEN_GD, POST_BR, LOAI_TIEN, DR_CR, MA_KH, TEN_KH, CCA_USRID, TR_EX_RT,
                    REMARK, BUS_CODE, UNIT_BUS_CODE, TR_CODE, TR_NAME, REFERENCE, VALUE_DATE,
                    DEPT_CODE, TR_TIME, COMFIRM, TRDT_TIME, CREATED_DATE, UPDATED_DATE, FILE_NAME
                );";

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// LN03 Direct Import - Chuyên biệt cho Bad debt data với 20 cột
        /// Luôn import trực tiếp vào bảng dữ liệu theo cấu hình AlwaysDirectImport
        /// </summary>
        /// <param name="file">File LN03 CSV với 20 cột (17 có header + 3 không header)</param>
        /// <param name="statementDate">Ngày báo cáo (optional)</param>
        /// <returns>Kết quả import với thông tin chi tiết về LN03 direct import</returns>
        [HttpPost("ln03-direct")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<object>> LN03DirectImport(
            IFormFile file,
            [FromQuery] string? statementDate = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("File LN03 không được để trống");
                }

                // Validate file là LN03
                var dataType = _directImportService.DetectDataTypeFromFileName(file.FileName);
                if (dataType != "LN03")
                {
                    return BadRequest($"File không phải LN03. Detected type: {dataType}. Endpoint này chỉ dành riêng cho LN03.");
                }

                var fileSizeMB = file.Length / 1024.0 / 1024.0;
                _logger.LogInformation("🚀 [LN03_DIRECT_API] Starting LN03 direct import: {FileName}, Size: {FileSize}MB",
                    file.FileName, fileSizeMB);

                // ✅ DIRECT IMPORT - Luôn bypass mọi xử lý trung gian
                var result = await _directImportService.ImportLN03DirectAsync(file, statementDate);

                if (result.Success)
                {
                    var recordsPerSecond = result.ProcessedRecords / Math.Max(result.Duration.TotalSeconds, 0.001);
                    _logger.LogInformation("✅ [LN03_DIRECT_API] LN03 direct import SUCCESS: {Records} records in {Duration}ms ({Rate:F0} records/sec)",
                        result.ProcessedRecords, result.Duration.TotalMilliseconds, recordsPerSecond);

                    return Ok(new
                    {
                        success = true,
                        message = "LN03 import trực tiếp thành công",
                        dataType = "LN03",
                        targetTable = "LN03",
                        fileName = result.FileName,
                        processedRecords = result.ProcessedRecords,
                        durationMs = result.Duration.TotalMilliseconds,
                        recordsPerSecond = recordsPerSecond,
                        ngayDL = result.NgayDL,
                        importMode = "DIRECT_IMPORT_ONLY",
                        configurationApplied = "AlwaysDirectImport=true, UseCustomParser=true",
                        result
                    });
                }
                else
                {
                    _logger.LogError("❌ [LN03_DIRECT_API] LN03 direct import FAILED: {Error}", result.ErrorMessage);
                    return BadRequest(new
                    {
                        success = false,
                        message = "LN03 import trực tiếp thất bại",
                        error = result.ErrorMessage,
                        fileName = result.FileName,
                        dataType = "LN03",
                        result
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 [LN03_DIRECT_API] LN03 direct import EXCEPTION: {FileName}", file?.FileName);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi hệ thống khi import LN03 trực tiếp",
                    error = ex.Message,
                    fileName = file?.FileName ?? "Unknown",
                    dataType = "LN03"
                });
            }
        }
    }
}
