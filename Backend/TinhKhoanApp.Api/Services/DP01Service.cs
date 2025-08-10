using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.DTOs.DP01;
using TinhKhoanApp.Api.Repositories;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// DP01 Service - xử lý business logic cho bảng DP01 (Tiền gửi có kỳ hạn)
    /// 63 business columns theo CSV structure + system/temporal columns
    /// </summary>
    public class DP01Service : IDP01Service
    {
        private readonly IDP01Repository _dp01Repository;

        public DP01Service(IDP01Repository dp01Repository)
        {
            _dp01Repository = dp01Repository;
        }

        public async Task<IEnumerable<DP01PreviewDto>> GetAllAsync(int pageNumber = 1, int pageSize = 100)
        {
            var dp01Records = await _dp01Repository.GetAllAsync();
            var pagedResults = dp01Records
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return MapToPreviewDto(pagedResults);
        }

        public async Task<DP01DetailsDto?> GetByIdAsync(int id)
        {
            var dp01 = await _dp01Repository.GetByIdAsync(id);
            return dp01 != null ? MapToDetailsDto(dp01) : null;
        }

        public async Task<IEnumerable<DP01PreviewDto>> GetRecentAsync(int count = 10)
        {
            var dp01Records = await _dp01Repository.GetRecentAsync(count);
            return MapToPreviewDto(dp01Records);
        }

        public async Task<IEnumerable<DP01PreviewDto>> GetByDateAsync(DateTime date)
        {
            var dp01Records = await _dp01Repository.GetByDateAsync(date);
            return MapToPreviewDto(dp01Records);
        }

        public async Task<IEnumerable<DP01PreviewDto>> GetByBranchCodeAsync(string branchCode, int maxResults = 100)
        {
            var dp01Records = await _dp01Repository.GetByBranchCodeAsync(branchCode, maxResults);
            return MapToPreviewDto(dp01Records);
        }

        public async Task<IEnumerable<DP01PreviewDto>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100)
        {
            var dp01Records = await _dp01Repository.GetByCustomerCodeAsync(customerCode, maxResults);
            return MapToPreviewDto(dp01Records);
        }

        public async Task<IEnumerable<DP01PreviewDto>> GetByAccountNumberAsync(string accountNumber, int maxResults = 100)
        {
            var dp01Records = await _dp01Repository.GetByAccountNumberAsync(accountNumber, maxResults);
            return MapToPreviewDto(dp01Records);
        }

        public async Task<decimal> GetTotalBalanceByBranchAsync(string branchCode, DateTime? date = null)
        {
            return await _dp01Repository.GetTotalBalanceByBranchAsync(branchCode, date);
        }

        public async Task<DP01SummaryDto> GetStatisticsAsync(DateTime? date = null)
        {
            var allRecords = date.HasValue
                ? await _dp01Repository.GetByDateAsync(date.Value)
                : await _dp01Repository.GetAllAsync();

            var dp01List = allRecords.ToList();

            return new DP01SummaryDto
            {
                TotalRecords = dp01List.Count,
                TotalBalance = dp01List.Where(x => x.CURRENT_BALANCE.HasValue).Sum(x => x.CURRENT_BALANCE ?? 0),
                AverageBalance = dp01List.Where(x => x.CURRENT_BALANCE.HasValue).Any()
                    ? dp01List.Where(x => x.CURRENT_BALANCE.HasValue).Average(x => x.CURRENT_BALANCE ?? 0)
                    : 0
            };
        }

        public async Task<DP01DetailsDto> CreateAsync(DP01CreateDto createDto)
        {
            var dp01 = MapFromCreateDto(createDto);
            dp01.NGAY_DL = DateTime.Now; // Set current date
            dp01.CreatedAt = DateTime.Now;
            dp01.UpdatedAt = DateTime.Now;

            await _dp01Repository.AddAsync(dp01);
            await _dp01Repository.SaveChangesAsync();

            return MapToDetailsDto(dp01);
        }

        public async Task<DP01DetailsDto?> UpdateAsync(int id, DP01UpdateDto updateDto)
        {
            var existingDp01 = await _dp01Repository.GetByIdAsync(id);
            if (existingDp01 == null)
            {
                return null;
            }

            UpdateFromDto(existingDp01, updateDto);
            existingDp01.UpdatedAt = DateTime.Now;

            _dp01Repository.Update(existingDp01);
            await _dp01Repository.SaveChangesAsync();

            return MapToDetailsDto(existingDp01);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var dp01 = await _dp01Repository.GetByIdAsync(id);
            if (dp01 == null)
            {
                return false;
            }

            _dp01Repository.Remove(dp01);
            await _dp01Repository.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _dp01Repository.CountAsync();
        }

        // === PRIVATE MAPPING METHODS ===

        private IEnumerable<DP01PreviewDto> MapToPreviewDto(IEnumerable<DP01> dp01Records)
        {
            return dp01Records.Select(dp01 => new DP01PreviewDto
            {
                Id = dp01.Id,
                NGAY_DL = dp01.NGAY_DL ?? DateTime.MinValue,
                MA_CN = dp01.MA_CN,
                TAI_KHOAN_HACH_TOAN = dp01.TAI_KHOAN_HACH_TOAN,
                MA_KH = dp01.MA_KH,
                TEN_KH = dp01.TEN_KH,
                DP_TYPE_NAME = dp01.DP_TYPE_NAME,
                CCY = dp01.CCY,
                CURRENT_BALANCE = dp01.CURRENT_BALANCE,
                RATE = dp01.RATE,
                SO_TAI_KHOAN = dp01.SO_TAI_KHOAN,
                OPENING_DATE = dp01.OPENING_DATE,
                MATURITY_DATE = dp01.MATURITY_DATE,
                ADDRESS = dp01.ADDRESS,
                NOTENO = dp01.NOTENO,
                MONTH_TERM = dp01.MONTH_TERM,
                TERM_DP_NAME = dp01.TERM_DP_NAME,
                TIME_DP_NAME = dp01.TIME_DP_NAME,
                MA_PGD = dp01.MA_PGD,
                TEN_PGD = dp01.TEN_PGD,
                DP_TYPE_CODE = dp01.DP_TYPE_CODE,
                RENEW_DATE = dp01.RENEW_DATE,
                CUST_TYPE = dp01.CUST_TYPE,
                CUST_TYPE_NAME = dp01.CUST_TYPE_NAME,
                CUST_TYPE_DETAIL = dp01.CUST_TYPE_DETAIL,
                CUST_DETAIL_NAME = dp01.CUST_DETAIL_NAME,
                PREVIOUS_DP_CAP_DATE = dp01.PREVIOUS_DP_CAP_DATE,
                NEXT_DP_CAP_DATE = dp01.NEXT_DP_CAP_DATE,
                ID_NUMBER = dp01.ID_NUMBER,
                ISSUED_BY = dp01.ISSUED_BY,
                ISSUE_DATE = dp01.ISSUE_DATE,
                SEX_TYPE = dp01.SEX_TYPE,
                BIRTH_DATE = dp01.BIRTH_DATE,
                TELEPHONE = dp01.TELEPHONE,
                ACRUAL_AMOUNT = dp01.ACRUAL_AMOUNT,
                ACRUAL_AMOUNT_END = dp01.ACRUAL_AMOUNT_END,
                ACCOUNT_STATUS = dp01.ACCOUNT_STATUS
                // Add other 63 business columns as needed
            });
        }

        private DP01DetailsDto MapToDetailsDto(DP01 dp01)
        {
            return new DP01DetailsDto
            {
                Id = dp01.Id,
                NGAY_DL = dp01.NGAY_DL ?? DateTime.MinValue,
                MA_CN = dp01.MA_CN,
                TAI_KHOAN_HACH_TOAN = dp01.TAI_KHOAN_HACH_TOAN,
                MA_KH = dp01.MA_KH,
                TEN_KH = dp01.TEN_KH,
                // Map all 63 business columns
                CURRENT_BALANCE = dp01.CURRENT_BALANCE,
                SO_TAI_KHOAN = dp01.SO_TAI_KHOAN
                // Add remaining columns as needed
            };
        }

        private DP01 MapFromCreateDto(DP01CreateDto createDto)
        {
            return new DP01
            {
                MA_CN = createDto.MA_CN,
                TAI_KHOAN_HACH_TOAN = createDto.TAI_KHOAN_HACH_TOAN,
                MA_KH = createDto.MA_KH,
                TEN_KH = createDto.TEN_KH,
                CURRENT_BALANCE = createDto.CURRENT_BALANCE,
                SO_TAI_KHOAN = createDto.SO_TAI_KHOAN
                // Map all 63 business columns
            };
        }

        private void UpdateFromDto(DP01 dp01, DP01UpdateDto updateDto)
        {
            dp01.MA_CN = updateDto.MA_CN;
            dp01.TAI_KHOAN_HACH_TOAN = updateDto.TAI_KHOAN_HACH_TOAN;
            dp01.MA_KH = updateDto.MA_KH;
            dp01.TEN_KH = updateDto.TEN_KH;
            dp01.CURRENT_BALANCE = updateDto.CURRENT_BALANCE;
            dp01.SO_TAI_KHOAN = updateDto.SO_TAI_KHOAN;
            // Update all 63 business columns
        }
    }
}
