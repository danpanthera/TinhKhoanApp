using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Threading.Channels;
using TinhKhoanApp.Api.Helpers;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// Controller cho Direct Import - Import trực tiếp vào bảng riêng biệt
    /// Bỏ hoàn toàn ImportedDataItems để tăng hiệu năng 2-5x
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DirectImportController : ControllerBase
    {
        private readonly IDirectImportService _directImportService;
        private readonly ILogger<DirectImportController> _logger;

        public DirectImportController(
            IDirectImportService directImportService,
            ILogger<DirectImportController> logger)
        {
            _directImportService = directImportService;
            _logger = logger;
        }

        /// <summary>
        /// Import thông minh - tự động phát hiện loại file và import trực tiếp
        /// </summary>
        /// <param name="file">File dữ liệu CSV/Excel</param>
        /// <param name="statementDate">Ngày báo cáo (optional)</param>
        /// <returns>Kết quả import với thông tin chi tiết</returns>
        [HttpPost("smart")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<DirectImportResult>> ImportSmartDirect(
            IFormFile file,
            [FromQuery] string? statementDate = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("File không được để trống");
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
    }
}
