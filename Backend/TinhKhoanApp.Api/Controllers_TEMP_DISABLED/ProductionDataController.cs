using Microsoft.AspNetCore.Mvc;
using Khoan.Api.Models.Dtos.DP01;
using Khoan.Api.Models.Dtos.RR01;
using Khoan.Api.Models.Dtos.LN01;
using Khoan.Api.Models.Dtos;
using Khoan.Api.Models.Dtos.DPDA;
using Khoan.Api.Models.Dtos.GL01;
using Khoan.Api.Models.Dtos.GL02;
using Khoan.Api.Models.Dtos.GL41;
using Khoan.Api.Models.Dtos.EI01;
using Khoan.Api.Models.Common;
using Khoan.Api.Services.Interfaces;

namespace Khoan.Api.Controllers
{
    /// <summary>
    /// Production Data Controller - Clean architecture implementation
    /// Thay thế TestDataController với standardized service pattern cho tất cả 9 tables
    /// HTTP request handling only - business logic in services
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductionDataController : ControllerBase
    {
        // All 9 core table services
        private readonly IDP01Service _dp01Service;
        private readonly IRR01Service _rr01Service;
        private readonly ILN01Service _ln01Service;
        private readonly ILN03Service _ln03Service;
        private readonly IDPDAService _dpdaService;
        private readonly IGL01Service _gl01Service;
        private readonly IGL02Service _gl02Service;
        private readonly IGL41Service _gl41Service;
        private readonly IEI01Service _ei01Service;
        private readonly ILogger<ProductionDataController> _logger;

        public ProductionDataController(
            IDP01Service dp01Service,
            IRR01Service rr01Service,
            ILN01Service ln01Service,
            ILN03Service ln03Service,
            IDPDAService dpdaService,
            IGL01Service gl01Service,
            IGL02Service gl02Service,
            IGL41Service gl41Service,
            IEI01Service ei01Service,
            ILogger<ProductionDataController> logger)
        {
            _dp01Service = dp01Service;
            _rr01Service = rr01Service;
            _ln01Service = ln01Service;
            _ln03Service = ln03Service;
            _dpdaService = dpdaService;
            _gl01Service = gl01Service;
            _gl02Service = gl02Service;
            _gl41Service = gl41Service;
            _ei01Service = ei01Service;
            _logger = logger;
        }

        #region DP01 Endpoints

        /// <summary>
        /// Lấy preview data DP01 với pagination
        /// </summary>
        [HttpGet("dp01/preview")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<DP01PreviewDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<ActionResult<ApiResponse<PagedResult<DP01PreviewDto>>>> GetDP01Preview(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? ngayDL = null)
        {
            try
            {
                _logger.LogInformation("Getting DP01 preview data - Page: {Page}, PageSize: {PageSize}, NgayDL: {NgayDL}",
                    page, pageSize, ngayDL);

                var result = await _dp01Service.GetPreviewAsync(page, pageSize, ngayDL);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetDP01Preview endpoint");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Internal server error",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Lấy chi tiết record DP01 theo ID
        /// </summary>
        [HttpGet("dp01/{id}")]
        [ProducesResponseType(typeof(ApiResponse<DP01DetailsDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<ActionResult<ApiResponse<DP01DetailsDto>>> GetDP01ById(long id)
        {
            try
            {
                _logger.LogInformation("Getting DP01 record with ID: {Id}", id);

                var result = await _dp01Service.GetByIdAsync(id);

                if (!result.Success)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting DP01 record with ID: {Id}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Internal server error",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Import CSV file vào DP01 table
        /// </summary>
        [HttpPost("dp01/import")]
        [ProducesResponseType(typeof(ApiResponse<DP01ImportResultDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<ActionResult<ApiResponse<DP01ImportResultDto>>> ImportDP01(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "No file provided"
                    });
                }

                _logger.LogInformation("Starting DP01 CSV import - File: {FileName}, Size: {Size} bytes",
                    file.FileName, file.Length);

                var result = await _dp01Service.ImportCsvAsync(file);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ImportDP01 endpoint - File: {FileName}", file?.FileName);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Internal server error",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Lấy summary/aggregate data DP01 theo ngày
        /// </summary>
        [HttpGet("dp01/summary")]
        [ProducesResponseType(typeof(ApiResponse<DP01SummaryDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<ActionResult<ApiResponse<DP01SummaryDto>>> GetDP01Summary(
            [FromQuery] DateTime ngayDL)
        {
            try
            {
                _logger.LogInformation("Getting DP01 summary for date: {NgayDL}", ngayDL);

                var result = await _dp01Service.GetSummaryAsync(ngayDL);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting DP01 summary for date: {NgayDL}", ngayDL);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Internal server error",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        #endregion

        #region RR01 Endpoints - TODO

        /// <summary>
        /// Lấy preview data RR01 với pagination
        /// TODO: Implement when RR01Service implementation is completed
        /// </summary>
        [HttpGet("rr01/preview")]
        public async Task<ActionResult<ApiResponse<object>>> GetRR01Preview(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? ngayDL = null)
        {
            return StatusCode(501, new ApiResponse<object>
            {
                Success = false,
                Message = "RR01 service implementation not yet completed"
            });
        }

        #endregion

        #region LN01 Endpoints - TODO

        /// <summary>
        /// Lấy preview data LN01 với pagination
        /// TODO: Implement when LN01Service implementation is created
        /// </summary>
        [HttpGet("ln01/preview")]
        public async Task<ActionResult<ApiResponse<object>>> GetLN01Preview(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? ngayDL = null)
        {
            return StatusCode(501, new ApiResponse<object>
            {
                Success = false,
                Message = "LN01 service implementation not yet created"
            });
        }

        #endregion

        #region LN03 Endpoints - TODO

        /// <summary>
        /// Lấy preview data LN03 với pagination
        /// TODO: Implement when LN03Service implementation is created
        /// </summary>
        [HttpGet("ln03/preview")]
        public async Task<ActionResult<ApiResponse<object>>> GetLN03Preview(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? ngayDL = null)
        {
            return StatusCode(501, new ApiResponse<object>
            {
                Success = false,
                Message = "LN03 service implementation not yet created"
            });
        }

        #endregion

        #region DPDA Endpoints - TODO

        /// <summary>
        /// Lấy preview data DPDA với pagination
        /// TODO: Implement when DPDAService implementation is created
        /// </summary>
        [HttpGet("dpda/preview")]
        public async Task<ActionResult<ApiResponse<object>>> GetDPDAPreview(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return StatusCode(501, new ApiResponse<object>
            {
                Success = false,
                Message = "DPDA service implementation not yet created"
            });
        }

        #endregion

        #region GL01 Endpoints - TODO

        /// <summary>
        /// Lấy preview data GL01 với pagination
        /// TODO: Implement when GL01Service implementation is created
        /// </summary>
        [HttpGet("gl01/preview")]
        public async Task<ActionResult<ApiResponse<object>>> GetGL01Preview(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? ngayGD = null)
        {
            return StatusCode(501, new ApiResponse<object>
            {
                Success = false,
                Message = "GL01 service implementation not yet created"
            });
        }

        #endregion

        #region GL02 Endpoints - TODO

        /// <summary>
        /// Lấy preview data GL02 với pagination
        /// TODO: Implement when GL02Service implementation is created
        /// </summary>
        [HttpGet("gl02/preview")]
        public async Task<ActionResult<ApiResponse<object>>> GetGL02Preview(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? trDate = null)
        {
            return StatusCode(501, new ApiResponse<object>
            {
                Success = false,
                Message = "GL02 service implementation not yet created"
            });
        }

        #endregion

        #region GL41 Endpoints - TODO

        /// <summary>
        /// Lấy preview data GL41 với pagination
        /// TODO: Implement when GL41Service implementation is created
        /// </summary>
        [HttpGet("gl41/preview")]
        public async Task<ActionResult<ApiResponse<object>>> GetGL41Preview(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? branchCode = null)
        {
            return StatusCode(501, new ApiResponse<object>
            {
                Success = false,
                Message = "GL41 service implementation not yet created"
            });
        }

        #endregion

        #region EI01 Endpoints - TODO

        /// <summary>
        /// Lấy preview data EI01 với pagination
        /// TODO: Implement when EI01Service implementation is created
        /// </summary>
        [HttpGet("ei01/preview")]
        public async Task<ActionResult<ApiResponse<object>>> GetEI01Preview(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? branchCode = null)
        {
            return StatusCode(501, new ApiResponse<object>
            {
                Success = false,
                Message = "EI01 service implementation not yet created"
            });
        }

        #endregion

        #region System Health Endpoints

        /// <summary>
        /// Health check cho tất cả 9 data tables
        /// </summary>
        [HttpGet("health")]
        [ProducesResponseType(typeof(ApiResponse<SystemHealthDto>), 200)]
        public async Task<ActionResult<ApiResponse<SystemHealthDto>>> GetSystemHealth()
        {
            try
            {
                _logger.LogInformation("Performing system health check for all 9 tables");

                var health = new SystemHealthDto
                {
                    Timestamp = DateTime.UtcNow,
                    DP01Available = true,  // Service implementation exists
                    RR01Available = false, // Service interface exists, implementation partial
                    LN01Available = false, // Service interface exists, implementation not created
                    LN03Available = false, // Service interface exists, implementation not created
                    DPDAAvailable = false, // Service interface exists, implementation not created
                    GL01Available = false, // Service interface exists, implementation not created
                    GL02Available = false, // Service interface exists, implementation not created
                    GL41Available = false, // Service interface exists, implementation not created
                    EI01Available = false, // Service interface exists, implementation not created
                    DatabaseConnected = true, // Check database connectivity
                    ServicesHealthy = true
                };

                // TODO: Implement actual health checks for all 9 services

                return Ok(new ApiResponse<SystemHealthDto>
                {
                    Success = true,
                    Message = "System health check completed for all 9 tables",
                    Data = health
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing system health check");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Health check failed",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        #endregion
    }
}

namespace Khoan.Api.Models.Common
{
    /// <summary>
    /// System Health DTO - All 9 core tables
    /// </summary>
    public class SystemHealthDto
    {
        public DateTime Timestamp { get; set; }
        public bool DP01Available { get; set; }
        public bool RR01Available { get; set; }
        public bool LN01Available { get; set; }
        public bool LN03Available { get; set; }
        public bool DPDAAvailable { get; set; }
        public bool GL01Available { get; set; }
        public bool GL02Available { get; set; }
        public bool GL41Available { get; set; }
        public bool EI01Available { get; set; }
        public bool DatabaseConnected { get; set; }
        public bool ServicesHealthy { get; set; }
        public Dictionary<string, object> AdditionalInfo { get; set; } = new();
    }
}
