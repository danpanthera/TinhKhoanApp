using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Utilities;

namespace TinhKhoanApp.Api.Services.DataServices
{
    /// <summary>
    /// GL01 Data Service - triển khai IGL01DataService
    /// </summary>
    public class GL01DataService : IGL01DataService
    {
        private readonly IGL01Repository _gl01Repository;
        private readonly ILogger<GL01DataService> _logger;

        public GL01DataService(
            IGL01Repository gl01Repository,
            ILogger<GL01DataService> logger)
        {
            _gl01Repository = gl01Repository;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GL01PreviewDto>> GetGL01PreviewAsync(int count = 10)
        {
            try
            {
                var records = await _gl01Repository.GetRecentAsync(count);
                return MapToGL01PreviewDtos(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL01 preview data");
                return Enumerable.Empty<GL01PreviewDto>();
            }
        }

        /// <inheritdoc/>
        public async Task<GL01DetailDto?> GetGL01DetailAsync(long id)
        {
            try
            {
                var record = await _gl01Repository.GetByIdAsync((int)id);
                return record != null ? MapToGL01DetailDto(record) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL01 detail with ID {Id}", id);
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GL01PreviewDto>> GetGL01ByDateAsync(DateTime date, int maxResults = 100)
        {
            try
            {
                var records = await _gl01Repository.GetByDateAsync(date);
                return records.Take(maxResults).Select(MapToGL01PreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL01 data by date {Date}", date);
                return Enumerable.Empty<GL01PreviewDto>();
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GL01PreviewDto>> GetGL01ByUnitCodeAsync(string unitCode, int maxResults = 100)
        {
            try
            {
                var records = await _gl01Repository.GetByUnitCodeAsync(unitCode, maxResults);
                return records.Select(MapToGL01PreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL01 data by unit code {UnitCode}", unitCode);
                return Enumerable.Empty<GL01PreviewDto>();
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GL01PreviewDto>> GetGL01ByAccountCodeAsync(string accountCode, int maxResults = 100)
        {
            try
            {
                var records = await _gl01Repository.GetByAccountCodeAsync(accountCode, maxResults);
                return records.Select(MapToGL01PreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL01 data by account code {AccountCode}", accountCode);
                return Enumerable.Empty<GL01PreviewDto>();
            }
        }

        /// <inheritdoc/>
        public async Task<GL01SummaryDto> GetGL01SummaryByUnitAsync(string unitCode, DateTime? date = null)
        {
            try
            {
                decimal totalDebit = await _gl01Repository.GetTotalTransactionsByUnitAsync(unitCode, "DR", date);
                decimal totalCredit = await _gl01Repository.GetTotalTransactionsByUnitAsync(unitCode, "CR", date);

                Expression<Func<GL01, bool>> predicate = gl01 => gl01.POST_BR == unitCode;
                if (date.HasValue)
                {
                    predicate = gl01 => gl01.POST_BR == unitCode && gl01.NGAY_DL.Date == date.Value.Date;
                }

                int totalRecords = await _gl01Repository.CountAsync(predicate);
                int debitCount = await _gl01Repository.CountAsync(predicate.And(gl01 => gl01.DR_CR == "DR"));
                int creditCount = await _gl01Repository.CountAsync(predicate.And(gl01 => gl01.DR_CR == "CR"));

                return new GL01SummaryDto
                {
                    UnitCode = unitCode,
                    Date = date,
                    TotalTransactions = totalRecords,
                    TotalDebitTransactions = debitCount,
                    TotalCreditTransactions = creditCount,
                    TotalDebitAmount = totalDebit,
                    TotalCreditAmount = totalCredit
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL01 summary by unit {UnitCode}", unitCode);
                return new GL01SummaryDto
                {
                    UnitCode = unitCode,
                    Date = date,
                    TotalTransactions = 0,
                    TotalDebitTransactions = 0,
                    TotalCreditTransactions = 0,
                    TotalDebitAmount = 0,
                    TotalCreditAmount = 0
                };
            }
        }

        /// <inheritdoc/>
        public async Task<GL01SummaryDto> GetGL01SummaryByDateAsync(DateTime date)
        {
            try
            {
                decimal totalDebit = await _gl01Repository.GetTotalTransactionsByDateAsync(date, "DR");
                decimal totalCredit = await _gl01Repository.GetTotalTransactionsByDateAsync(date, "CR");

                Expression<Func<GL01, bool>> predicate = gl01 => gl01.NGAY_DL.Date == date.Date;
                int totalRecords = await _gl01Repository.CountAsync(predicate);
                int debitCount = await _gl01Repository.CountAsync(predicate.And(gl01 => gl01.DR_CR == "DR"));
                int creditCount = await _gl01Repository.CountAsync(predicate.And(gl01 => gl01.DR_CR == "CR"));

                return new GL01SummaryDto
                {
                    Date = date,
                    TotalTransactions = totalRecords,
                    TotalDebitTransactions = debitCount,
                    TotalCreditTransactions = creditCount,
                    TotalDebitAmount = totalDebit,
                    TotalCreditAmount = totalCredit
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GL01 summary by date {Date}", date);
                return new GL01SummaryDto
                {
                    Date = date,
                    TotalTransactions = 0,
                    TotalDebitTransactions = 0,
                    TotalCreditTransactions = 0,
                    TotalDebitAmount = 0,
                    TotalCreditAmount = 0
                };
            }
        }

        /// <inheritdoc/>
        public async Task<PagedApiResponse<GL01PreviewDto>> SearchGL01Async(
            string? keyword,
            string? unitCode,
            string? accountCode,
            string? transactionType,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 20)
        {
            try
            {
                Expression<Func<GL01, bool>> predicate = PreparePredicate(keyword, unitCode, accountCode, transactionType, fromDate, toDate);

                var totalCount = await _gl01Repository.CountAsync(predicate);
                var records = await _gl01Repository.GetPagedAsync(
                    predicate,
                    (page - 1) * pageSize,
                    pageSize,
                    q => q.OrderByDescending(gl01 => gl01.CREATED_DATE));

                var response = new PagedApiResponse<GL01PreviewDto>
                {
                    Success = true,
                    Message = "Success",
                    Data = MapToGL01PreviewDtos(records),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching GL01 data");
                return new PagedApiResponse<GL01PreviewDto>
                {
                    Success = false,
                    Message = "Error searching GL01 data",
                    Data = Enumerable.Empty<GL01PreviewDto>(),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = 0
                };
            }
        }

        private static Expression<Func<GL01, bool>> PreparePredicate(
            string? keyword,
            string? unitCode,
            string? accountCode,
            string? transactionType,
            DateTime? fromDate,
            DateTime? toDate)
        {
            Expression<Func<GL01, bool>> predicate = gl01 => true;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                predicate = predicate.And(gl01 =>
                    gl01.TR_NAME!.Contains(keyword) ||
                    gl01.REFERENCE!.Contains(keyword) ||
                    gl01.STS!.Contains(keyword) ||
                    gl01.TR_CODE!.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(unitCode))
            {
                predicate = predicate.And(gl01 => gl01.POST_BR == unitCode);
            }

            if (!string.IsNullOrWhiteSpace(accountCode))
            {
                predicate = predicate.And(gl01 => gl01.TAI_KHOAN == accountCode);
            }

            if (!string.IsNullOrWhiteSpace(transactionType))
            {
                predicate = predicate.And(gl01 => gl01.DR_CR == transactionType);
            }

            if (fromDate.HasValue)
            {
                predicate = PredicateBuilder.And(predicate, gl01 => gl01.NGAY_DL.Date >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                predicate = PredicateBuilder.And(predicate, gl01 => gl01.NGAY_DL.Date <= toDate.Value.Date);
            }

            return predicate;
        }

        private static IEnumerable<GL01PreviewDto> MapToGL01PreviewDtos(IEnumerable<GL01> records)
        {
            return records.Select(MapToGL01PreviewDto);
        }

        private static GL01PreviewDto MapToGL01PreviewDto(GL01 record)
        {
            return new GL01PreviewDto
            {
                Id = record.Id,
                NGAY_DL = record.NGAY_DL,
                POST_BR = record.POST_BR,
                DEPT_CODE = record.DEPT_CODE,
                TAI_KHOAN = record.TAI_KHOAN,
                SO_TIEN_GD = record.SO_TIEN_GD,
                DR_CR = record.DR_CR,
                LOAI_TIEN = record.LOAI_TIEN,
                TR_NAME = record.TR_NAME,
                TR_CODE = record.TR_CODE,
                CREATED_DATE = record.CREATED_DATE
            };
        }

        private static GL01DetailDto MapToGL01DetailDto(GL01 record)
        {
            return new GL01DetailDto
            {
                Id = record.Id,
                NGAY_DL = record.NGAY_DL,
                NGAY_GD = record.NGAY_GD,
                POST_BR = record.POST_BR,
                DEPT_CODE = record.DEPT_CODE,
                TAI_KHOAN = record.TAI_KHOAN,
                TEN_TK = record.TEN_TK,
                SO_TIEN_GD = record.SO_TIEN_GD,
                DR_CR = record.DR_CR,
                LOAI_TIEN = record.LOAI_TIEN,
                TR_NAME = record.TR_NAME,
                TR_CODE = record.TR_CODE,
                STS = record.STS,
                NGUOI_TAO = record.NGUOI_TAO,
                TR_TYPE = record.TR_TYPE,
                MA_KH = record.MA_KH,
                TEN_KH = record.TEN_KH,
                TR_EX_RT = record.TR_EX_RT,
                REMARK = record.REMARK,
                BUS_CODE = record.BUS_CODE,
                UNIT_BUS_CODE = record.UNIT_BUS_CODE,
                REFERENCE = record.REFERENCE,
                VALUE_DATE = record.VALUE_DATE,
                TR_TIME = record.TR_TIME,
                COMFIRM = record.COMFIRM,
                TRDT_TIME = record.TRDT_TIME,
                CREATED_DATE = record.CREATED_DATE,
                UPDATED_DATE = record.UPDATED_DATE,
                FILE_NAME = record.FILE_NAME
            };
        }
    }
}
