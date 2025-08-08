using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models.NguonVon;
using TinhKhoanApp.Api.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// Controller xử lý API tính toán nguồn vốn từ dữ liệu thô DP01
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NguonVonController : ControllerBase
    {
        private readonly IRawDataService _rawDataService;
        private readonly ILogger<NguonVonController> _logger;

        public NguonVonController(
            IRawDataService rawDataService,
            ILogger<NguonVonController> logger)
        {
            _rawDataService = rawDataService;
            _logger = logger;
        }

        /// <summary>
        /// Tính toán nguồn vốn từ bảng DP01 theo đơn vị và ngày
        /// </summary>
        /// <param name="request">Thông tin request: đơn vị, ngày, loại ngày</param>
        /// <returns>Kết quả tính toán nguồn vốn</returns>
        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateNguonVon([FromBody] NguonVonRequest request)
        {
            try
            {
                // Validate input
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Request không hợp lệ" });
                }

                if (string.IsNullOrEmpty(request.UnitCode))
                {
                    return BadRequest(new { success = false, message = "Mã đơn vị không được để trống" });
                }

                _logger.LogInformation("🔍 API: Bắt đầu tính toán nguồn vốn cho đơn vị: {Unit}, ngày: {Date}, loại: {Type}",
                    request.UnitCode, request.TargetDate.ToString("dd/MM/yyyy"), request.DateType);

                // Tính toán nguồn vốn từ dữ liệu thô
                var result = await _rawDataService.CalculateNguonVonFromRawDataAsync(request);

                if (result == null || !result.HasData)
                {
                    _logger.LogWarning("⚠️ Không tìm thấy dữ liệu cho đơn vị {Unit} ngày {Date}", request.UnitCode, request.TargetDate.ToString("dd/MM/yyyy"));

                    return NotFound(new
                    {
                        success = false,
                        message = result?.Message ?? "Chưa tìm thấy dữ liệu theo ngày chỉ định",
                        requestedDate = request.TargetDate.ToString("dd/MM/yyyy"),
                        unitCode = request.UnitCode
                    });
                }

                _logger.LogInformation("✅ Tính toán thành công: {Balance:N0} VND cho {Unit}", result.Summary.TotalBalance, result.Summary.UnitCode);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        unitCode = result.Summary.UnitCode,
                        unitName = GetUnitName(result.Summary.UnitCode),
                        totalBalance = result.Summary.TotalBalance,
                        recordCount = result.Summary.RecordCount,
                        calculatedDate = result.Summary.CalculatedDate,
                        topAccounts = result.TopAccounts.Take(10).ToList()
                    },
                    calculatedDate = request.TargetDate.ToString("dd/MM/yyyy"),
                    message = result.Message
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "❌ Lỗi tham số không hợp lệ: {Message}", ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi hệ thống khi tính toán nguồn vốn cho {Unit}", request?.UnitCode);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi hệ thống, vui lòng thử lại sau",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy chi tiết nguồn vốn theo đơn vị để debug và kiểm tra
        /// </summary>
        /// <param name="unitCode">Mã đơn vị</param>
        /// <param name="date">Ngày cần kiểm tra</param>
        /// <returns>Chi tiết các tài khoản có số dư lớn nhất</returns>
        [HttpGet("details")]
        public async Task<IActionResult> GetNguonVonDetails(
            [FromQuery] string unitCode,
            [FromQuery] DateTime date)
        {
            try
            {
                if (string.IsNullOrEmpty(unitCode))
                {
                    return BadRequest(new { success = false, message = "Mã đơn vị không được để trống" });
                }

                _logger.LogInformation("🔍 API: Lấy chi tiết nguồn vốn cho {Unit} ngày {Date}", unitCode, date.ToString("dd/MM/yyyy"));

                var details = await _rawDataService.CalculateNguonVonFromRawDataAsync(new NguonVonRequest
                {
                    UnitCode = unitCode,
                    TargetDate = date,
                    DateType = "day"
                });

                return Ok(new
                {
                    success = true,
                    data = details,
                    message = details.HasData ? "Lấy chi tiết thành công" : "Không có dữ liệu"
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "❌ Lỗi tham số không hợp lệ: {Message}", ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi lấy chi tiết nguồn vốn cho {Unit}", unitCode);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi hệ thống, vui lòng thử lại sau",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy danh sách đơn vị có thể tính toán
        /// </summary>
        /// <returns>Danh sách các đơn vị (chi nhánh + PGD)</returns>
        [HttpGet("units")]
        public IActionResult GetAvailableUnits()
        {
            try
            {
                var units = new
                {
                    branches = new[]
                    {
                        new { code = "HoiSo", name = "Hội sở", maCN = "7800" },
                        new { code = "CnBinhLu", name = "Chi nhánh Bình Lư", maCN = "7801" },
                        new { code = "CnPhongTho", name = "Chi nhánh Phong Thổ", maCN = "7802" },
                        new { code = "CnSinHo", name = "Chi nhánh Sìn Hồ", maCN = "7803" },
                        new { code = "CnBumTo", name = "Chi nhánh Bum Tở", maCN = "7804" },
                        new { code = "CnThanUyen", name = "Chi nhánh Than Uyên", maCN = "7805" },
                        new { code = "CnDoanKet", name = "Chi nhánh Đoàn Kết", maCN = "7806" },
                        new { code = "CnTanUyen", name = "Chi nhánh Tân Uyên", maCN = "7807" },
                        new { code = "CnNamHang", name = "Chi nhánh Nậm Hàng", maCN = "7808" }
                    },
                    pgds = new[]
                    {
                        new { code = "CnPhongThoPgdSo5", name = "CN Phong Thổ - PGD Số 5", maCN = "7802", maPGD = "01" },
                        new { code = "CnThanUyenPgdSo6", name = "CN Than Uyên - PGD Số 6", maCN = "7805", maPGD = "01" },
                        new { code = "CnDoanKetPgdSo1", name = "CN Đoàn Kết - PGD Số 1", maCN = "7806", maPGD = "01" },
                        new { code = "CnDoanKetPgdSo2", name = "CN Đoàn Kết - PGD Số 2", maCN = "7806", maPGD = "02" },
                        new { code = "CnTanUyenPgdSo3", name = "CN Tân Uyên - PGD Số 3", maCN = "7807", maPGD = "01" }
                    },
                    all = new { code = "ALL", name = "Tất cả đơn vị (Toàn tỉnh)" }
                };

                return Ok(new
                {
                    success = true,
                    data = units,
                    message = "Lấy danh sách đơn vị thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi lấy danh sách đơn vị");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi hệ thống",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Mapping từ mã đơn vị sang tên đơn vị
        /// </summary>
        /// <param name="unitCode">Mã đơn vị</param>
        /// <returns>Tên đơn vị</returns>
        private static string GetUnitName(string unitCode)
        {
            var mapping = new Dictionary<string, string>
            {
                { "7800", "🏢 Hội sở" },
                { "7801", "🏦 Chi nhánh Bình Lư" },
                { "7802", "🏦 Chi nhánh Phong Thổ" },
                { "7803", "🏦 Chi nhánh Sìn Hồ" },
                { "7804", "🏦 Chi nhánh Bum Tở" },
                { "7805", "🏦 Chi nhánh Than Uyên" },
                { "7806", "🏦 Chi nhánh Đoàn Kết" },
                { "7807", "🏦 Chi nhánh Tân Uyên" },
                { "7808", "🏦 Chi nhánh Nậm Hàng" },
                { "ALL", "🏛️ Toàn tỉnh (Tổng hợp)" },
                { "", "🏛️ Toàn tỉnh (Tổng hợp)" }
            };

            return mapping.ContainsKey(unitCode) ? mapping[unitCode] : unitCode;
        }
    }
}
