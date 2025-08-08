using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Models.DTOs.GL41;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Repositories;

namespace TinhKhoanApp.Api.Services.DataServices
{
    /// <summary>
    /// GL41 Data Service - triển khai IGL41DataService
    /// </summary>
    public class GL41DataService : IGL41DataService
    {
        private readonly IGL41Repository _gl41Repository;
        private readonly ILogger<GL41DataService> _logger;

        public GL41DataService(
            IGL41Repository gl41Repository,
            ILogger<GL41DataService> logger)
        {
            _gl41Repository = gl41Repository;
            _logger = logger;
        }

        public async Task<IEnumerable<GL41PreviewDto>> GetGL41PreviewAsync(int count = 10)
        {
            try
            {
                var records = await _gl41Repository.GetRecentAsync(count);
                return MapToGL41PreviewDtos(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL41 preview data");
                return Enumerable.Empty<GL41PreviewDto>();
            }
        }

        public async Task<GL41DetailsDto?> GetGL41DetailAsync(long id)
        {
            try
            {
                var record = await _gl41Repository.GetByIdAsync((int)id);
                if (record == null)
                    return null;

                return MapToGL41DetailsDto(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL41 detail for ID {Id}", id);
                return null;
            }
        }

        public async Task<IEnumerable<GL41PreviewDto>> GetGL41ByUnitAsync(string unitCode, int maxResults = 100)
        {
            try
            {
                var records = await _gl41Repository.GetByUnitCodeAsync(unitCode, maxResults);
                return MapToGL41PreviewDtos(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL41 data for unit {UnitCode}", unitCode);
                return Enumerable.Empty<GL41PreviewDto>();
            }
        }

        public async Task<IEnumerable<GL41PreviewDto>> GetGL41ByAccountAsync(string accountCode, int maxResults = 100)
        {
            try
            {
                var records = await _gl41Repository.GetByAccountCodeAsync(accountCode, maxResults);
                return MapToGL41PreviewDtos(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL41 data for account {AccountCode}", accountCode);
                return Enumerable.Empty<GL41PreviewDto>();
            }
        }

        public async Task<GL41SummaryDto> GetGL41SummaryByUnitAsync(string unitCode, DateTime? date = null)
        {
            try
            {
                // Get all records for the unit and date
                var query = _gl41Repository.FindAsync(gl41 => gl41.MA_CN == unitCode);
                var records = await query;

                if (date.HasValue)
                {
                    records = records.Where(gl41 => gl41.NGAY_DL.Date == date.Value.Date);
                }
                else if (records.Any())
                {
                    // If no date specified, use the most recent date
                    var mostRecentDate = records.Max(r => r.NGAY_DL.Date);
                    records = records.Where(r => r.NGAY_DL.Date == mostRecentDate);
                }

                var summary = new GL41SummaryDto
                {
                    MA_CN = unitCode,
                    NGAY_DL = records.Any() ? records.First().NGAY_DL : DateTime.Now,
                    TotalDebitOpening = records.Sum(r => r.DN_DAUKY ?? 0),
                    TotalCreditOpening = records.Sum(r => r.DC_DAUKY ?? 0),
                    TotalDebitTransaction = records.Sum(r => r.ST_GHINO ?? 0),
                    TotalCreditTransaction = records.Sum(r => r.ST_GHICO ?? 0),
                    TotalDebitClosing = records.Sum(r => r.DN_CUOIKY ?? 0),
                    TotalCreditClosing = records.Sum(r => r.DC_CUOIKY ?? 0),
                    AccountCount = records.Select(r => r.MA_TK).Distinct().Count()
                };

                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating GL41 summary for unit {UnitCode}", unitCode);
                return new GL41SummaryDto
                {
                    MA_CN = unitCode,
                    NGAY_DL = date ?? DateTime.Now
                };
            }
        }

        public async Task<ApiResponse<PagedResult<GL41PreviewDto>>> SearchGL41Async(
            string? keyword,
            string? unitCode,
            string? accountCode,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 20)
        {
            try
            {
                // Build predicate
                Expression<Func<GL41, bool>> predicate = gl41 => true;

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    predicate = gl41 =>
                        (gl41.MA_TK != null && gl41.MA_TK.Contains(keyword)) ||
                        (gl41.TEN_TK != null && gl41.TEN_TK.Contains(keyword));
                }

                if (!string.IsNullOrWhiteSpace(unitCode))
                {
                    var unitPredicate = (Expression<Func<GL41, bool>>)(gl41 => gl41.MA_CN == unitCode);
                    predicate = CombinePredicates(predicate, unitPredicate);
                }

                if (!string.IsNullOrWhiteSpace(accountCode))
                {
                    var accountPredicate = (Expression<Func<GL41, bool>>)(gl41 => gl41.MA_TK == accountCode);
                    predicate = CombinePredicates(predicate, accountPredicate);
                }

                if (fromDate.HasValue)
                {
                    var fromDatePredicate = (Expression<Func<GL41, bool>>)(gl41 => gl41.NGAY_DL >= fromDate.Value);
                    predicate = CombinePredicates(predicate, fromDatePredicate);
                }

                if (toDate.HasValue)
                {
                    var toDatePredicate = (Expression<Func<GL41, bool>>)(gl41 => gl41.NGAY_DL <= toDate.Value);
                    predicate = CombinePredicates(predicate, toDatePredicate);
                }

                // Get total count
                var records = await _gl41Repository.FindAsync(predicate);
                var totalCount = records.Count();

                // Get paged data
                var pagedRecords = records
                    .OrderByDescending(gl41 => gl41.CREATED_DATE)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                // Map to DTOs
                var dtos = MapToGL41PreviewDtos(pagedRecords);

                // Create paged response
                return ApiResponse < PagedResult<GL41PreviewDto>.Ok(
                    dtos,
                    page,
                    pageSize,
                    totalCount,
                    "GL41 search results retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching GL41 data");
                return ApiResponse < PagedResult<GL41PreviewDto>.Error(
                    "Failed to search GL41 data",
                    "GL41_SEARCH_ERROR");
            }
        }

        #region Helper Methods

        private IEnumerable<GL41PreviewDto> MapToGL41PreviewDtos(IEnumerable<GL41> records)
        {
            return records.Select(MapToGL41PreviewDto);
        }

        private GL41PreviewDto MapToGL41PreviewDto(GL41 record)
        {
            return new GL41PreviewDto
            {
                ID = record.ID,
                NGAY_DL = record.NGAY_DL,
                MA_CN = record.MA_CN,
                LOAI_TIEN = record.LOAI_TIEN,
                MA_TK = record.MA_TK,
                TEN_TK = record.TEN_TK,
                LOAI_BT = record.LOAI_BT,
                DN_DAUKY = record.DN_DAUKY,
                DC_DAUKY = record.DC_DAUKY,
                SBT_NO = record.SBT_NO,
                ST_GHINO = record.ST_GHINO,
                SBT_CO = record.SBT_CO,
                ST_GHICO = record.ST_GHICO,
                DN_CUOIKY = record.DN_CUOIKY,
                DC_CUOIKY = record.DC_CUOIKY,
                CREATED_DATE = record.CREATED_DATE
            };
        }

        private GL41DetailsDto MapToGL41DetailsDto(GL41 record)
        {
            return new GL41DetailsDto
            {
                // Base properties from GL41PreviewDto
                ID = record.ID,
                NGAY_DL = record.NGAY_DL,
                MA_CN = record.MA_CN,
                LOAI_TIEN = record.LOAI_TIEN,
                MA_TK = record.MA_TK,
                TEN_TK = record.TEN_TK,
                LOAI_BT = record.LOAI_BT,
                DN_DAUKY = record.DN_DAUKY,
                DC_DAUKY = record.DC_DAUKY,
                SBT_NO = record.SBT_NO,
                ST_GHINO = record.ST_GHINO,
                SBT_CO = record.SBT_CO,
                ST_GHICO = record.ST_GHICO,
                DN_CUOIKY = record.DN_CUOIKY,
                DC_CUOIKY = record.DC_CUOIKY,
                CREATED_DATE = record.CREATED_DATE,

                // Additional detail properties
                FILE_NAME = record.FILE_NAME,
                BATCH_ID = record.BATCH_ID,
                IMPORT_SESSION_ID = record.IMPORT_SESSION_ID
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
