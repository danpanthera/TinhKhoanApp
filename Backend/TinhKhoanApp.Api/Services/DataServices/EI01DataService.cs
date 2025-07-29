using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Utilities;

namespace TinhKhoanApp.Api.Services.DataServices
{
    /// <summary>
    /// Service xử lý business logic liên quan đến dữ liệu EI01 (Thông tin dịch vụ ngân hàng điện tử)
    /// </summary>
    public class EI01DataService : IEI01DataService
    {
        private readonly IEI01Repository _repository;
        private readonly ILogger<EI01DataService> _logger;

        public EI01DataService(IEI01Repository repository, ILogger<EI01DataService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<EI01PreviewDto>> GetEI01PreviewAsync(int count = 10)
        {
            try
            {
                var entities = await _repository.GetAsync(take: count);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy preview dữ liệu EI01");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<EI01DetailDto?> GetEI01DetailAsync(long id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync((int)id);
                return entity != null ? MapToDetailDto(entity) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết bản ghi EI01 với ID {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<EI01PreviewDto>> GetEI01ByDateAsync(DateTime date, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByDateAsync(date);
                return entities.Take(maxResults).Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu EI01 theo ngày {Date}", date);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<EI01PreviewDto>> GetEI01ByBranchAsync(string branchCode, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByBranchCodeAsync(branchCode, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu EI01 theo chi nhánh {BranchCode}", branchCode);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<EI01PreviewDto>> GetEI01ByCustomerAsync(string customerCode, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByCustomerCodeAsync(customerCode, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu EI01 theo mã khách hàng {CustomerCode}", customerCode);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<EI01PreviewDto>> GetEI01ByCustomerTypeAsync(string customerType, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByCustomerTypeAsync(customerType, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu EI01 theo loại khách hàng {CustomerType}", customerType);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<EI01PreviewDto>> GetEI01ByPhoneNumberAsync(string phoneNumber, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByPhoneNumberAsync(phoneNumber, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu EI01 theo số điện thoại {PhoneNumber}", phoneNumber);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<EI01PreviewDto>> GetEI01ByServiceStatusAsync(string serviceType, string status, int maxResults = 100)
        {
            try
            {
                // Combine service type and status into a single status string, for example: "EMB-ACTIVE"
                string combinedStatus = $"{serviceType}-{status}";
                var entities = await _repository.GetByServiceStatusAsync(combinedStatus, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu EI01 theo trạng thái dịch vụ {ServiceType} - {Status}", serviceType, status);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<EI01PreviewDto>> GetEI01ByRegistrationDateRangeAsync(
            DateTime fromDate,
            DateTime toDate,
            string serviceType,
            int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByRegistrationDateRangeAsync(fromDate, toDate, serviceType, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu EI01 theo khoảng thời gian đăng ký {FromDate} - {ToDate}, dịch vụ {ServiceType}",
                    fromDate, toDate, serviceType);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<EI01SummaryDto> GetEI01SummaryByBranchAsync(string branchCode, DateTime? date = null)
        {
            try
            {
                // Tìm kiếm các bản ghi theo chi nhánh và ngày (nếu có)
                Expression<Func<EI01, bool>> predicate = e => e.MA_CN == branchCode;
                if (date.HasValue)
                {
                    var dateValue = date.Value;
                    predicate = e => e.MA_CN == branchCode && e.NGAY_DL.Date == dateValue.Date;
                }

                var allRecords = await _repository.FindAsync(predicate);

                // Tính toán các số liệu tổng hợp
                return new EI01SummaryDto
                {
                    BranchCode = branchCode,
                    Date = date,
                    TotalCustomers = allRecords.Count(),
                    EMBRegistrations = allRecords.Count(e => !string.IsNullOrEmpty(e.SDT_EMB)),
                    OTTRegistrations = allRecords.Count(e => !string.IsNullOrEmpty(e.SDT_OTT)),
                    SMSRegistrations = allRecords.Count(e => !string.IsNullOrEmpty(e.SDT_SMS)),
                    SAVRegistrations = allRecords.Count(e => !string.IsNullOrEmpty(e.SDT_SAV)),
                    LNRegistrations = allRecords.Count(e => !string.IsNullOrEmpty(e.SDT_LN))
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê tổng hợp EI01 theo chi nhánh {BranchCode}", branchCode);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<EI01SummaryDto> GetEI01SummaryByDateAsync(DateTime date)
        {
            try
            {
                var allRecords = await _repository.GetByDateAsync(date);

                // Tính toán các số liệu tổng hợp
                return new EI01SummaryDto
                {
                    Date = date,
                    TotalCustomers = allRecords.Count(),
                    EMBRegistrations = allRecords.Count(e => !string.IsNullOrEmpty(e.SDT_EMB)),
                    OTTRegistrations = allRecords.Count(e => !string.IsNullOrEmpty(e.SDT_OTT)),
                    SMSRegistrations = allRecords.Count(e => !string.IsNullOrEmpty(e.SDT_SMS)),
                    SAVRegistrations = allRecords.Count(e => !string.IsNullOrEmpty(e.SDT_SAV)),
                    LNRegistrations = allRecords.Count(e => !string.IsNullOrEmpty(e.SDT_LN))
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê tổng hợp EI01 theo ngày {Date}", date);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<PagedApiResponse<EI01PreviewDto>> SearchEI01Async(
            string? keyword,
            string? branchCode,
            string? customerCode,
            string? customerType,
            string? phoneNumber,
            string? serviceType,
            string? serviceStatus,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 20)
        {
            try
            {
                // Xây dựng điều kiện tìm kiếm
                Expression<Func<EI01, bool>> predicate = e => true;

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    predicate = e =>
                        (e.MA_KH != null && e.MA_KH.Contains(keyword)) ||
                        (e.TEN_KH != null && e.TEN_KH.Contains(keyword)) ||
                        (e.SDT_EMB != null && e.SDT_EMB.Contains(keyword)) ||
                        (e.SDT_OTT != null && e.SDT_OTT.Contains(keyword)) ||
                        (e.SDT_SMS != null && e.SDT_SMS.Contains(keyword));
                }

                if (!string.IsNullOrWhiteSpace(branchCode))
                {
                    predicate = PredicateBuilder.And(predicate, e => e.MA_CN == branchCode);
                }

                if (!string.IsNullOrWhiteSpace(customerCode))
                {
                    predicate = PredicateBuilder.And(predicate, e => e.MA_KH == customerCode);
                }

                if (!string.IsNullOrWhiteSpace(customerType))
                {
                    predicate = PredicateBuilder.And(predicate, e => e.LOAI_KH == customerType);
                }

                if (!string.IsNullOrWhiteSpace(phoneNumber))
                {
                    predicate = PredicateBuilder.And(predicate, e =>
                        (e.SDT_EMB != null && e.SDT_EMB == phoneNumber) ||
                        (e.SDT_OTT != null && e.SDT_OTT == phoneNumber) ||
                        (e.SDT_SMS != null && e.SDT_SMS == phoneNumber));
                }

                if (!string.IsNullOrWhiteSpace(serviceType))
                {
                    if (serviceType == "EMB")
                        predicate = PredicateBuilder.And(predicate, e => e.SDT_EMB != null && !string.IsNullOrEmpty(e.SDT_EMB));
                    else if (serviceType == "OTT")
                        predicate = PredicateBuilder.And(predicate, e => e.SDT_OTT != null && !string.IsNullOrEmpty(e.SDT_OTT));
                    else if (serviceType == "SMS")
                        predicate = PredicateBuilder.And(predicate, e => e.SDT_SMS != null && !string.IsNullOrEmpty(e.SDT_SMS));
                }

                if (!string.IsNullOrWhiteSpace(serviceStatus))
                {
                    predicate = PredicateBuilder.And(predicate, e =>
                        (e.TRANG_THAI_EMB != null && e.TRANG_THAI_EMB == serviceStatus) ||
                        (e.TRANG_THAI_OTT != null && e.TRANG_THAI_OTT == serviceStatus) ||
                        (e.TRANG_THAI_SMS != null && e.TRANG_THAI_SMS == serviceStatus));
                }

                if (fromDate.HasValue)
                {
                    var from = fromDate.Value.Date;
                    predicate = PredicateBuilder.And(predicate, e => e.NGAY_DL >= from);
                }

                if (toDate.HasValue)
                {
                    var to = toDate.Value.Date.AddDays(1).AddSeconds(-1);
                    predicate = PredicateBuilder.And(predicate, e => e.NGAY_DL <= to);
                }

                // Thực hiện tìm kiếm với phân trang
                int totalCount = await _repository.CountAsync(predicate);
                var skip = (page - 1) * pageSize;
                var items = await _repository.GetPagedAsync(
                    predicate,
                    skip,
                    pageSize,
                    e => e.OrderByDescending(x => x.NGAY_DL).ThenBy(x => x.MA_KH)
                );

                // Chuyển đổi kết quả sang DTO
                return new PagedApiResponse<EI01PreviewDto>
                {
                    TotalCount = totalCount,
                    PageSize = pageSize,
                    Page = page,
                    Data = items.Select(MapToPreviewDto).ToList(),
                    Success = true,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm kiếm dữ liệu EI01");
                throw;
            }
        }

        #region Mapping Methods

        private static EI01PreviewDto MapToPreviewDto(EI01 entity)
        {
            return new EI01PreviewDto
            {
                Id = entity.Id,
                NGAY_DL = entity.NGAY_DL,
                MA_CN = entity.MA_CN,
                MA_KH = entity.MA_KH,
                TEN_KH = entity.TEN_KH,
                LOAI_KH = entity.LOAI_KH,
                SDT_EMB = entity.SDT_EMB,
                TRANG_THAI_EMB = entity.TRANG_THAI_EMB,
                SDT_OTT = entity.SDT_OTT,
                TRANG_THAI_OTT = entity.TRANG_THAI_OTT,
                SDT_SMS = entity.SDT_SMS,
                TRANG_THAI_SMS = entity.TRANG_THAI_SMS
            };
        }

        private static EI01DetailDto MapToDetailDto(EI01 entity)
        {
            return new EI01DetailDto
            {
                Id = entity.Id,
                NGAY_DL = entity.NGAY_DL,
                MA_CN = entity.MA_CN,
                MA_KH = entity.MA_KH,
                TEN_KH = entity.TEN_KH,
                LOAI_KH = entity.LOAI_KH,
                SDT_EMB = entity.SDT_EMB,
                TRANG_THAI_EMB = entity.TRANG_THAI_EMB,
                NGAY_DK_EMB = entity.NGAY_DK_EMB,
                SDT_OTT = entity.SDT_OTT,
                TRANG_THAI_OTT = entity.TRANG_THAI_OTT,
                NGAY_DK_OTT = entity.NGAY_DK_OTT,
                SDT_SMS = entity.SDT_SMS,
                TRANG_THAI_SMS = entity.TRANG_THAI_SMS,
                NGAY_DK_SMS = entity.NGAY_DK_SMS,
                SDT_SAV = entity.SDT_SAV,
                TRANG_THAI_SAV = entity.TRANG_THAI_SAV,
                NGAY_DK_SAV = entity.NGAY_DK_SAV,
                SDT_LN = entity.SDT_LN,
                TRANG_THAI_LN = entity.TRANG_THAI_LN,
                NGAY_DK_LN = entity.NGAY_DK_LN,
                USER_EMB = entity.USER_EMB,
                USER_OTT = entity.USER_OTT,
                USER_SMS = entity.USER_SMS,
                USER_SAV = entity.USER_SAV,
                USER_LN = entity.USER_LN,
                CREATED_DATE = entity.CREATED_DATE
            };
        }

        #endregion
    }
}
