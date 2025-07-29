using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Utilities;

namespace TinhKhoanApp.Api.Services.DataServices
{
    /// <summary>
    /// DPDA Service - triển khai IDPDADataService
    /// </summary>
    public class DPDADataService : IDPDADataService
    {
        private readonly IDPDARepository _dpdaRepository;
        private readonly ILogger<DPDADataService> _logger;

        public DPDADataService(IDPDARepository dpdaRepository, ILogger<DPDADataService> logger)
        {
            _dpdaRepository = dpdaRepository;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DPDAPreviewDto>> GetDPDAPreviewAsync(int count = 10)
        {
            try
            {
                var records = await _dpdaRepository.GetRecentAsync(count);
                return MapToDPDAPreviewDtos(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DPDA preview data");
                return Enumerable.Empty<DPDAPreviewDto>();
            }
        }

        /// <inheritdoc/>
        public async Task<DPDADetailDto?> GetDPDADetailAsync(int id)
        {
            try
            {
                var record = await _dpdaRepository.GetByIdAsync(id);
                if (record == null)
                    return null;

                return MapToDPDADetailDto(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DPDA detail for ID {Id}", id);
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DPDAPreviewDto>> GetDPDAByDateAsync(DateTime date, int maxResults = 100)
        {
            try
            {
                var records = await _dpdaRepository.GetByDateAsync(date);
                return MapToDPDAPreviewDtos(records.Take(maxResults));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DPDA data for date {Date}", date);
                return Enumerable.Empty<DPDAPreviewDto>();
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DPDAPreviewDto>> GetDPDAByBranchAsync(string branchCode, int maxResults = 100)
        {
            try
            {
                var records = await _dpdaRepository.GetByBranchCodeAsync(branchCode, maxResults);
                return MapToDPDAPreviewDtos(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DPDA data for branch {BranchCode}", branchCode);
                return Enumerable.Empty<DPDAPreviewDto>();
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DPDAPreviewDto>> GetDPDAByCustomerAsync(string customerCode, int maxResults = 100)
        {
            try
            {
                var records = await _dpdaRepository.GetByCustomerCodeAsync(customerCode, maxResults);
                return MapToDPDAPreviewDtos(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DPDA data for customer {CustomerCode}", customerCode);
                return Enumerable.Empty<DPDAPreviewDto>();
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DPDAPreviewDto>> GetDPDAByAccountNumberAsync(string accountNumber, int maxResults = 100)
        {
            try
            {
                var records = await _dpdaRepository.GetByAccountNumberAsync(accountNumber, maxResults);
                return MapToDPDAPreviewDtos(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DPDA data for account {AccountNumber}", accountNumber);
                return Enumerable.Empty<DPDAPreviewDto>();
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DPDAPreviewDto>> GetDPDAByCardNumberAsync(string cardNumber, int maxResults = 100)
        {
            try
            {
                var records = await _dpdaRepository.GetByCardNumberAsync(cardNumber, maxResults);
                return MapToDPDAPreviewDtos(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DPDA data for card {CardNumber}", cardNumber);
                return Enumerable.Empty<DPDAPreviewDto>();
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DPDAPreviewDto>> GetDPDAByStatusAsync(string status, int maxResults = 100)
        {
            try
            {
                var records = await _dpdaRepository.GetByStatusAsync(status, maxResults);
                return MapToDPDAPreviewDtos(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DPDA data for status {Status}", status);
                return Enumerable.Empty<DPDAPreviewDto>();
            }
        }

        /// <inheritdoc/>
        public async Task<DPDASummaryDto> GetDPDASummaryByBranchAsync(string branchCode, DateTime? date = null)
        {
            try
            {
                Expression<Func<DPDA, bool>> predicate = dpda => dpda.MA_CHI_NHANH == branchCode;
                if (date.HasValue)
                {
                    predicate = dpda => dpda.MA_CHI_NHANH == branchCode && dpda.NGAY_DL.Date == date.Value.Date;
                }

                var allRecords = await _dpdaRepository.FindAsync(predicate);

                // Tính toán các thống kê
                var activeCount = allRecords.Count(dpda => dpda.TRANG_THAI == "ACTIVE" || dpda.TRANG_THAI == "HOẠT ĐỘNG");
                var issuedCount = allRecords.Count(dpda => dpda.NGAY_PHAT_HANH.HasValue);
                var pendingCount = allRecords.Count(dpda => dpda.TRANG_THAI == "PENDING" || dpda.TRANG_THAI == "CHỜ DUYỆT");
                var rejectedCount = allRecords.Count(dpda => dpda.TRANG_THAI == "REJECTED" || dpda.TRANG_THAI == "TỪ CHỐI" || dpda.TRANG_THAI == "HỦY");

                // Tính thời gian xử lý trung bình
                double? avgProcessingDays = null;
                var processedCards = allRecords.Where(dpda => dpda.NGAY_NOP_DON.HasValue && dpda.NGAY_PHAT_HANH.HasValue).ToList();
                if (processedCards.Any())
                {
                    avgProcessingDays = processedCards.Average(dpda =>
                        (dpda.NGAY_PHAT_HANH!.Value - dpda.NGAY_NOP_DON!.Value).TotalDays);
                }

                // Phân loại theo loại thẻ
                var cardTypeDistribution = allRecords
                    .GroupBy(dpda => dpda.LOAI_THE)
                    .ToDictionary(g => g.Key, g => g.Count());

                return new DPDASummaryDto
                {
                    TotalCards = allRecords.Count(),
                    ActiveCards = activeCount,
                    IssuedCards = issuedCount,
                    PendingCards = pendingCount,
                    RejectedCards = rejectedCount,
                    AverageProcessingDays = avgProcessingDays,
                    CardTypeDistribution = cardTypeDistribution,
                    BranchCode = branchCode,
                    Date = date
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DPDA summary for branch {BranchCode}", branchCode);
                return new DPDASummaryDto { BranchCode = branchCode, Date = date };
            }
        }

        /// <inheritdoc/>
        public async Task<DPDASummaryDto> GetDPDASummaryByDateAsync(DateTime date)
        {
            try
            {
                Expression<Func<DPDA, bool>> predicate = dpda => dpda.NGAY_DL.Date == date.Date;
                var allRecords = await _dpdaRepository.FindAsync(predicate);

                // Tính toán các thống kê
                var activeCount = allRecords.Count(dpda => dpda.TRANG_THAI == "ACTIVE" || dpda.TRANG_THAI == "HOẠT ĐỘNG");
                var issuedCount = allRecords.Count(dpda => dpda.NGAY_PHAT_HANH.HasValue);
                var pendingCount = allRecords.Count(dpda => dpda.TRANG_THAI == "PENDING" || dpda.TRANG_THAI == "CHỜ DUYỆT");
                var rejectedCount = allRecords.Count(dpda => dpda.TRANG_THAI == "REJECTED" || dpda.TRANG_THAI == "TỪ CHỐI" || dpda.TRANG_THAI == "HỦY");

                // Tính thời gian xử lý trung bình
                double? avgProcessingDays = null;
                var processedCards = allRecords.Where(dpda => dpda.NGAY_NOP_DON.HasValue && dpda.NGAY_PHAT_HANH.HasValue).ToList();
                if (processedCards.Any())
                {
                    avgProcessingDays = processedCards.Average(dpda =>
                        (dpda.NGAY_PHAT_HANH!.Value - dpda.NGAY_NOP_DON!.Value).TotalDays);
                }

                // Phân loại theo loại thẻ
                var cardTypeDistribution = allRecords
                    .GroupBy(dpda => dpda.LOAI_THE)
                    .ToDictionary(g => g.Key, g => g.Count());

                return new DPDASummaryDto
                {
                    TotalCards = allRecords.Count(),
                    ActiveCards = activeCount,
                    IssuedCards = issuedCount,
                    PendingCards = pendingCount,
                    RejectedCards = rejectedCount,
                    AverageProcessingDays = avgProcessingDays,
                    CardTypeDistribution = cardTypeDistribution,
                    Date = date
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DPDA summary for date {Date}", date);
                return new DPDASummaryDto { Date = date };
            }
        }

        /// <inheritdoc/>
        public async Task<PagedApiResponse<DPDAPreviewDto>> SearchDPDAAsync(
            string? keyword,
            string? branchCode,
            string? customerCode,
            string? accountNumber,
            string? cardNumber,
            string? status,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 20)
        {
            try
            {
                Expression<Func<DPDA, bool>> predicate = PreparePredicate(
                    keyword, branchCode, customerCode, accountNumber, cardNumber, status, fromDate, toDate);

                var totalCount = await _dpdaRepository.CountAsync(predicate);
                var records = await _dpdaRepository.GetPagedAsync(
                    predicate,
                    page,
                    pageSize,
                    q => q.OrderByDescending(dpda => dpda.NGAY_DL));

                return new PagedApiResponse<DPDAPreviewDto>
                {
                    Data = MapToDPDAPreviewDtos(records),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching DPDA data");
                return new PagedApiResponse<DPDAPreviewDto>
                {
                    Data = Enumerable.Empty<DPDAPreviewDto>(),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = 0
                };
            }
        }

        #region Private Methods

        private static Expression<Func<DPDA, bool>> PreparePredicate(
            string? keyword,
            string? branchCode,
            string? customerCode,
            string? accountNumber,
            string? cardNumber,
            string? status,
            DateTime? fromDate,
            DateTime? toDate)
        {
            Expression<Func<DPDA, bool>> predicate = dpda => true;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                predicate = ExpressionHelper.And(predicate, dpda =>
                    dpda.TEN_KHACH_HANG.Contains(keyword) ||
                    dpda.MA_KHACH_HANG.Contains(keyword) ||
                    dpda.SO_THE.Contains(keyword) ||
                    dpda.SO_TAI_KHOAN.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(branchCode))
            {
                predicate = ExpressionHelper.And(predicate, dpda => dpda.MA_CHI_NHANH == branchCode);
            }

            if (!string.IsNullOrWhiteSpace(customerCode))
            {
                predicate = ExpressionHelper.And(predicate, dpda => dpda.MA_KHACH_HANG == customerCode);
            }

            if (!string.IsNullOrWhiteSpace(accountNumber))
            {
                predicate = ExpressionHelper.And(predicate, dpda => dpda.SO_TAI_KHOAN == accountNumber);
            }

            if (!string.IsNullOrWhiteSpace(cardNumber))
            {
                predicate = ExpressionHelper.And(predicate, dpda => dpda.SO_THE == cardNumber);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                predicate = ExpressionHelper.And(predicate, dpda => dpda.TRANG_THAI == status);
            }

            if (fromDate.HasValue)
            {
                predicate = ExpressionHelper.And(predicate, dpda => dpda.NGAY_DL.Date >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                predicate = ExpressionHelper.And(predicate, dpda => dpda.NGAY_DL.Date <= toDate.Value.Date);
            }

            return predicate;
        }

        private static IEnumerable<DPDAPreviewDto> MapToDPDAPreviewDtos(IEnumerable<DPDA> records)
        {
            return records.Select(record => new DPDAPreviewDto
            {
                Id = record.Id,
                NGAY_DL = record.NGAY_DL,
                MA_CHI_NHANH = record.MA_CHI_NHANH,
                MA_KHACH_HANG = record.MA_KHACH_HANG,
                TEN_KHACH_HANG = record.TEN_KHACH_HANG,
                SO_TAI_KHOAN = record.SO_TAI_KHOAN,
                LOAI_THE = record.LOAI_THE,
                SO_THE = record.SO_THE,
                TRANG_THAI = record.TRANG_THAI,
                NGAY_PHAT_HANH = record.NGAY_PHAT_HANH
            });
        }

        private static DPDADetailDto MapToDPDADetailDto(DPDA record)
        {
            return new DPDADetailDto
            {
                Id = record.Id,
                NGAY_DL = record.NGAY_DL,
                MA_CHI_NHANH = record.MA_CHI_NHANH,
                MA_KHACH_HANG = record.MA_KHACH_HANG,
                TEN_KHACH_HANG = record.TEN_KHACH_HANG,
                SO_TAI_KHOAN = record.SO_TAI_KHOAN,
                LOAI_THE = record.LOAI_THE,
                SO_THE = record.SO_THE,
                TRANG_THAI = record.TRANG_THAI,
                NGAY_PHAT_HANH = record.NGAY_PHAT_HANH,
                NGAY_NOP_DON = record.NGAY_NOP_DON,
                USER_PHAT_HANH = record.USER_PHAT_HANH,
                PHAN_LOAI = record.PHAN_LOAI,
                GIAO_THE = record.GIAO_THE,
                LOAI_PHAT_HANH = record.LOAI_PHAT_HANH,
                CREATED_DATE = record.CREATED_DATE,
                UPDATED_DATE = record.UPDATED_DATE
            };
        }

        #endregion
    }

    /// <summary>
    /// Helper class để kết hợp LINQ expressions
    /// </summary>
    public static class ExpressionHelper
    {
        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            var parameter = Expression.Parameter(typeof(T));
            var body = Expression.AndAlso(
                Expression.Invoke(first, parameter),
                Expression.Invoke(second, parameter)
            );
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}
