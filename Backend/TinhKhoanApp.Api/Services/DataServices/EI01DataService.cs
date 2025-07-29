using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Repositories;

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
                var entity = await _repository.GetByIdAsync(id);
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
                var entities = await _repository.GetByDateAsync(date, maxResults);
                return entities.Select(MapToPreviewDto);
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
                var entities = await _repository.GetByServiceStatusAsync(serviceType, status, maxResults);
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

                var allRecords = await _repository.GetAsync(predicate);

                // Tính toán các số liệu tổng hợp
                return new EI01SummaryDto
                {
                    BranchCode = branchCode,
                    Date = date,
                    TotalRecords = allRecords.Count(),
                    TotalActiveServices = allRecords.Count(e => e.TRANG_THAI == "1"),
                    TotalInactiveServices = allRecords.Count(e => e.TRANG_THAI == "0"),
                    InternetBankingCount = allRecords.Count(e => e.LOAI_DV == "IB" || e.LOAI_DV?.Contains("Internet", StringComparison.OrdinalIgnoreCase) == true),
                    MobileBankingCount = allRecords.Count(e => e.LOAI_DV == "MB" || e.LOAI_DV?.Contains("Mobile", StringComparison.OrdinalIgnoreCase) == true),
                    SMSBankingCount = allRecords.Count(e => e.LOAI_DV == "SMS" || e.LOAI_DV?.Contains("SMS", StringComparison.OrdinalIgnoreCase) == true),
                    CorporateCustomersCount = allRecords.Count(e => e.LOAI_KH == "1"),
                    IndividualCustomersCount = allRecords.Count(e => e.LOAI_KH == "2"),
                    GroupedByRegistrationDate = allRecords
                        .GroupBy(e => e.NGAY_DK.Date)
                        .OrderByDescending(g => g.Key)
                        .Take(10)
                        .ToDictionary(g => g.Key, g => g.Count())
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
                    TotalRecords = allRecords.Count(),
                    TotalActiveServices = allRecords.Count(e => e.TRANG_THAI == "1"),
                    TotalInactiveServices = allRecords.Count(e => e.TRANG_THAI == "0"),
                    InternetBankingCount = allRecords.Count(e => e.LOAI_DV == "IB" || e.LOAI_DV?.Contains("Internet", StringComparison.OrdinalIgnoreCase) == true),
                    MobileBankingCount = allRecords.Count(e => e.LOAI_DV == "MB" || e.LOAI_DV?.Contains("Mobile", StringComparison.OrdinalIgnoreCase) == true),
                    SMSBankingCount = allRecords.Count(e => e.LOAI_DV == "SMS" || e.LOAI_DV?.Contains("SMS", StringComparison.OrdinalIgnoreCase) == true),
                    CorporateCustomersCount = allRecords.Count(e => e.LOAI_KH == "1"),
                    IndividualCustomersCount = allRecords.Count(e => e.LOAI_KH == "2"),
                    GroupedByBranch = allRecords
                        .GroupBy(e => e.MA_CN)
                        .OrderByDescending(g => g.Count())
                        .Take(10)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    GroupedByServiceType = allRecords
                        .GroupBy(e => e.LOAI_DV)
                        .OrderByDescending(g => g.Count())
                        .Take(5)
                        .ToDictionary(g => g.Key ?? "Unknown", g => g.Count())
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
                        e.MA_KH.Contains(keyword) ||
                        e.TEN_KH.Contains(keyword) ||
                        e.SO_DT.Contains(keyword);
                }

                if (!string.IsNullOrWhiteSpace(branchCode))
                {
                    predicate = predicate.And(e => e.MA_CN == branchCode);
                }

                if (!string.IsNullOrWhiteSpace(customerCode))
                {
                    predicate = predicate.And(e => e.MA_KH == customerCode);
                }

                if (!string.IsNullOrWhiteSpace(customerType))
                {
                    predicate = predicate.And(e => e.LOAI_KH == customerType);
                }

                if (!string.IsNullOrWhiteSpace(phoneNumber))
                {
                    predicate = predicate.And(e => e.SO_DT == phoneNumber);
                }

                if (!string.IsNullOrWhiteSpace(serviceType))
                {
                    predicate = predicate.And(e => e.LOAI_DV == serviceType);
                }

                if (!string.IsNullOrWhiteSpace(serviceStatus))
                {
                    predicate = predicate.And(e => e.TRANG_THAI == serviceStatus);
                }

                if (fromDate.HasValue)
                {
                    var from = fromDate.Value.Date;
                    predicate = predicate.And(e => e.NGAY_DL >= from);
                }

                if (toDate.HasValue)
                {
                    var to = toDate.Value.Date.AddDays(1).AddSeconds(-1);
                    predicate = predicate.And(e => e.NGAY_DL <= to);
                }

                // Thực hiện tìm kiếm với phân trang
                var (totalCount, items) = await _repository.GetPagedAsync(
                    predicate,
                    page,
                    pageSize,
                    e => e.OrderByDescending(x => x.NGAY_DL).ThenBy(x => x.MA_KH)
                );

                // Chuyển đổi kết quả sang DTO
                return new PagedApiResponse<EI01PreviewDto>
                {
                    TotalCount = totalCount,
                    PageSize = pageSize,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    Items = items.Select(MapToPreviewDto).ToList()
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
                NgayDL = entity.NGAY_DL,
                MaCN = entity.MA_CN,
                MaKH = entity.MA_KH,
                TenKH = entity.TEN_KH,
                LoaiKH = entity.LOAI_KH,
                SoDT = entity.SO_DT,
                LoaiDV = entity.LOAI_DV,
                NgayDK = entity.NGAY_DK,
                TrangThai = entity.TRANG_THAI
            };
        }

        private static EI01DetailDto MapToDetailDto(EI01 entity)
        {
            return new EI01DetailDto
            {
                Id = entity.Id,
                NgayDL = entity.NGAY_DL,
                MaCN = entity.MA_CN,
                MaKH = entity.MA_KH,
                TenKH = entity.TEN_KH,
                LoaiKH = entity.LOAI_KH,
                SoDT = entity.SO_DT,
                Email = entity.EMAIL,
                LoaiDV = entity.LOAI_DV,
                NgayDK = entity.NGAY_DK,
                NgayHH = entity.NGAY_HH,
                MaQlDv = entity.MA_QL_DV,
                TenQlDv = entity.TEN_QL_DV,
                HanMucGd = entity.HAN_MUC_GD,
                HanMucNgay = entity.HAN_MUC_NGAY,
                TrangThai = entity.TRANG_THAI,
                MoTaTt = entity.MO_TA_TT,
                NgayCapNhat = entity.NGAY_CAP_NHAT,
                NguoiCapNhat = entity.NGUOI_CAP_NHAT,
                GhiChu = entity.GHI_CHU,
                SoLanDn = entity.SO_LAN_DN,
                LanDnCuoi = entity.LAN_DN_CUOI,
                TgDungDv = entity.TG_DUNG_DV,
                LyDoKhoa = entity.LY_DO_KHOA
            };
        }

        #endregion
    }

    // Extension method để kết hợp các điều kiện predicate
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        {
            var parameter = Expression.Parameter(typeof(T));

            var visitor = new ReplaceParameterVisitor(b.Parameters[0], parameter);
            var bBody = visitor.Visit(b.Body);

            visitor = new ReplaceParameterVisitor(a.Parameters[0], parameter);
            var aBody = visitor.Visit(a.Body);

            var body = Expression.AndAlso(aBody, bBody);

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _old;
            private readonly ParameterExpression _new;

            public ReplaceParameterVisitor(ParameterExpression old, ParameterExpression @new)
            {
                _old = old;
                _new = @new;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _old ? _new : base.VisitParameter(node);
            }
        }
    }
}
