using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.Dtos;
using TinhKhoanApp.Api.Models.Dtos.DP01;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Repositories;

namespace TinhKhoanApp.Api.Services.DataServices
{
    /// <summary>
    /// Data Preview Service - triển khai IDataPreviewService
    /// </summary>
    public class DataPreviewService : IDataPreviewService
    {
        private readonly IDP01Repository _dp01Repository;
        private readonly ILogger<DataPreviewService> _logger;

        public DataPreviewService(
            IDP01Repository dp01Repository,
            ILogger<DataPreviewService> logger)
        {
            _dp01Repository = dp01Repository;
            _logger = logger;
        }

        public async Task<IEnumerable<DP01PreviewDto>> GetDP01PreviewAsync(int count = 10)
        {
            try
            {
                var records = await _dp01Repository.GetRecentAsync(count);
                return MapToDP01PreviewDtos(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DP01 preview data");
                return Enumerable.Empty<DP01PreviewDto>();
            }
        }

        public async Task<DP01DetailsDto?> GetDP01DetailAsync(int id)
        {
            try
            {
                var record = await _dp01Repository.GetByIdAsync(id);
                if (record == null)
                    return null;

                return MapToDP01DetailsDto(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DP01 detail for ID {Id}", id);
                return null;
            }
        }

        public async Task<IEnumerable<DP01PreviewDto>> GetDP01ByBranchAsync(string branchCode, int maxResults = 100)
        {
            try
            {
                var records = await _dp01Repository.GetByBranchCodeAsync(branchCode, maxResults);
                return MapToDP01PreviewDtos(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DP01 data for branch {BranchCode}", branchCode);
                return Enumerable.Empty<DP01PreviewDto>();
            }
        }

        public async Task<IEnumerable<DP01PreviewDto>> GetDP01ByCustomerAsync(string customerCode, int maxResults = 100)
        {
            try
            {
                var records = await _dp01Repository.GetByCustomerCodeAsync(customerCode, maxResults);
                return MapToDP01PreviewDtos(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DP01 data for customer {CustomerCode}", customerCode);
                return Enumerable.Empty<DP01PreviewDto>();
            }
        }

        public async Task<IEnumerable<DP01PreviewDto>> GetDP01ByAccountNumberAsync(string accountNumber, int maxResults = 100)
        {
            try
            {
                var records = await _dp01Repository.GetByAccountNumberAsync(accountNumber, maxResults);
                return MapToDP01PreviewDtos(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DP01 data for account {AccountNumber}", accountNumber);
                return Enumerable.Empty<DP01PreviewDto>();
            }
        }

        public async Task<ApiResponse<PagedResult<DP01PreviewDto>>> SearchDP01Async(
            string? keyword,
            string? branchCode,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 20)
        {
            try
            {
                // Build predicate
                Expression<Func<DP01, bool>> predicate = dp01 => true;

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    // Note: This might need optimization for performance
                    predicate = dp01 =>
                        (dp01.MA_KH != null && dp01.MA_KH.Contains(keyword)) ||
                        (dp01.TEN_KH != null && dp01.TEN_KH.Contains(keyword)) ||
                        (dp01.SO_TAI_KHOAN != null && dp01.SO_TAI_KHOAN.Contains(keyword));
                }

                if (!string.IsNullOrWhiteSpace(branchCode))
                {
                    var branchPredicate = (Expression<Func<DP01, bool>>)(dp01 => dp01.MA_CN == branchCode);
                    predicate = CombinePredicates(predicate, branchPredicate);
                }

                if (fromDate.HasValue)
                {
                    var fromDatePredicate = (Expression<Func<DP01, bool>>)(dp01 =>
                        dp01.NGAY_DL != null && dp01.NGAY_DL >= fromDate.Value);
                    predicate = CombinePredicates(predicate, fromDatePredicate);
                }

                if (toDate.HasValue)
                {
                    var toDatePredicate = (Expression<Func<DP01, bool>>)(dp01 =>
                        dp01.NGAY_DL != null && dp01.NGAY_DL <= toDate.Value);
                    predicate = CombinePredicates(predicate, toDatePredicate);
                }

                // Get total count
                var totalCount = await _dp01Repository.CountAsync();

                // Get paged data
                var records = await _dp01Repository.FindAsync(predicate);
                var pagedRecords = records
                    .OrderByDescending(dp01 => dp01.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                // Map to DTOs
                var dtos = MapToDP01PreviewDtos(pagedRecords);

                // Create paged response
                return ApiResponse < PagedResult<DP01PreviewDto>.Create(
                    dtos,
                    totalCount,
                    page,
                    pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching DP01 data");
                return ApiResponse < PagedResult<DP01PreviewDto>.Create(
                    Enumerable.Empty<DP01PreviewDto>(),
                    0,
                    page,
                    pageSize);
            }
        }

        #region Helper Methods

        private IEnumerable<DP01PreviewDto> MapToDP01PreviewDtos(IEnumerable<DP01> records)
        {
            return records.Select(MapToDP01PreviewDto);
        }

        private DP01PreviewDto MapToDP01PreviewDto(DP01 record)
        {
            return new DP01PreviewDto
            {
                Id = record.Id,
                NGAY_DL = record.NGAY_DL,
                MA_CN = record.MA_CN,
                MA_KH = record.MA_KH,
                TEN_KH = record.TEN_KH,
                CCY = record.CCY,
                CURRENT_BALANCE = record.CURRENT_BALANCE,
                RATE = record.RATE,
                SO_TAI_KHOAN = record.SO_TAI_KHOAN,
                OPENING_DATE = record.OPENING_DATE,
                MATURITY_DATE = record.MATURITY_DATE,
                CreatedAt = record.CreatedAt,
                UpdatedAt = record.UpdatedAt
            };
        }

        private DP01DetailsDto MapToDP01DetailsDto(DP01 record)
        {
            return new DP01DetailsDto
            {
                // Base properties from DP01PreviewDto
                Id = record.Id,
                NGAY_DL = record.NGAY_DL,
                MA_CN = record.MA_CN,
                MA_KH = record.MA_KH,
                TEN_KH = record.TEN_KH,
                CCY = record.CCY,
                CURRENT_BALANCE = record.CURRENT_BALANCE,
                RATE = record.RATE,
                SO_TAI_KHOAN = record.SO_TAI_KHOAN,
                OPENING_DATE = record.OPENING_DATE,
                MATURITY_DATE = record.MATURITY_DATE,
                CreatedAt = record.CreatedAt,
                UpdatedAt = record.UpdatedAt,

                // Additional detail properties
                ADDRESS = record.ADDRESS,
                NOTENO = record.NOTENO,
                MONTH_TERM = record.MONTH_TERM,
                TERM_DP_NAME = record.TERM_DP_NAME,
                TIME_DP_NAME = record.TIME_DP_NAME,
                MA_PGD = record.MA_PGD,
                TEN_PGD = record.TEN_PGD,
                DataSource = record.DataSource,
                ImportDateTime = record.ImportDateTime
            };
        }

        private Expression<Func<T, bool>> CombinePredicates<T>(
            Expression<Func<T, bool>> predicate1,
            Expression<Func<T, bool>> predicate2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(
                predicate1.Parameters[0], parameter);
            var left = leftVisitor.Visit(predicate1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(
                predicate2.Parameters[0], parameter);
            var right = rightVisitor.Visit(predicate2.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(left!, right!), parameter);
        }

        private class ReplaceExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression? Visit(Expression? node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }

        #endregion
    }
}
