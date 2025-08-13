using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.Dtos.DP01;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Services.Interfaces;

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
                NGAY_DL = dp01.NGAY_DL,
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
                ACCOUNT_STATUS = dp01.ACCOUNT_STATUS,
                DRAMT = dp01.DRAMT,
                CRAMT = dp01.CRAMT,
                EMPLOYEE_NUMBER = dp01.EMPLOYEE_NUMBER,
                EMPLOYEE_NAME = dp01.EMPLOYEE_NAME,
                SPECIAL_RATE = dp01.SPECIAL_RATE,
                AUTO_RENEWAL = dp01.AUTO_RENEWAL,
                CLOSE_DATE = dp01.CLOSE_DATE,
                LOCAL_PROVIN_NAME = dp01.LOCAL_PROVIN_NAME,
                LOCAL_DISTRICT_NAME = dp01.LOCAL_DISTRICT_NAME,
                LOCAL_WARD_NAME = dp01.LOCAL_WARD_NAME,
                TERM_DP_TYPE = dp01.TERM_DP_TYPE,
                TIME_DP_TYPE = dp01.TIME_DP_TYPE,
                STATES_CODE = dp01.STATES_CODE,
                ZIP_CODE = dp01.ZIP_CODE,
                COUNTRY_CODE = dp01.COUNTRY_CODE,
                TAX_CODE_LOCATION = dp01.TAX_CODE_LOCATION,
                MA_CAN_BO_PT = dp01.MA_CAN_BO_PT,
                TEN_CAN_BO_PT = dp01.TEN_CAN_BO_PT,
                PHONG_CAN_BO_PT = dp01.PHONG_CAN_BO_PT,
                NGUOI_NUOC_NGOAI = dp01.NGUOI_NUOC_NGOAI,
                QUOC_TICH = dp01.QUOC_TICH,
                MA_CAN_BO_AGRIBANK = dp01.MA_CAN_BO_AGRIBANK,
                NGUOI_GIOI_THIEU = dp01.NGUOI_GIOI_THIEU,
                TEN_NGUOI_GIOI_THIEU = dp01.TEN_NGUOI_GIOI_THIEU,
                CONTRACT_COUTS_DAY = dp01.CONTRACT_COUTS_DAY,
                SO_KY_AD_LSDB = dp01.SO_KY_AD_LSDB,
                UNTBUSCD = dp01.UNTBUSCD,
                TYGIA = dp01.TYGIA,

                // System fields
                CreatedDate = dp01.CreatedAt,
                UpdatedDate = dp01.UpdatedAt,
                FileName = dp01.FILE_NAME
            });
        }

        private DP01DetailsDto MapToDetailsDto(DP01 dp01)
        {
            return new DP01DetailsDto
            {
                Id = dp01.Id,
                NGAY_DL = dp01.NGAY_DL,
                CreatedDate = dp01.CreatedAt,
                UpdatedDate = dp01.UpdatedAt,
                FileName = dp01.FILE_NAME,

                // All 63 business columns
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
                ACCOUNT_STATUS = dp01.ACCOUNT_STATUS,
                DRAMT = dp01.DRAMT,
                CRAMT = dp01.CRAMT,
                EMPLOYEE_NUMBER = dp01.EMPLOYEE_NUMBER,
                EMPLOYEE_NAME = dp01.EMPLOYEE_NAME,
                SPECIAL_RATE = dp01.SPECIAL_RATE,
                AUTO_RENEWAL = dp01.AUTO_RENEWAL,
                CLOSE_DATE = dp01.CLOSE_DATE,
                LOCAL_PROVIN_NAME = dp01.LOCAL_PROVIN_NAME,
                LOCAL_DISTRICT_NAME = dp01.LOCAL_DISTRICT_NAME,
                LOCAL_WARD_NAME = dp01.LOCAL_WARD_NAME,
                TERM_DP_TYPE = dp01.TERM_DP_TYPE,
                TIME_DP_TYPE = dp01.TIME_DP_TYPE,
                STATES_CODE = dp01.STATES_CODE,
                ZIP_CODE = dp01.ZIP_CODE,
                COUNTRY_CODE = dp01.COUNTRY_CODE,
                TAX_CODE_LOCATION = dp01.TAX_CODE_LOCATION,
                MA_CAN_BO_PT = dp01.MA_CAN_BO_PT,
                TEN_CAN_BO_PT = dp01.TEN_CAN_BO_PT,
                PHONG_CAN_BO_PT = dp01.PHONG_CAN_BO_PT,
                NGUOI_NUOC_NGOAI = dp01.NGUOI_NUOC_NGOAI,
                QUOC_TICH = dp01.QUOC_TICH,
                MA_CAN_BO_AGRIBANK = dp01.MA_CAN_BO_AGRIBANK,
                NGUOI_GIOI_THIEU = dp01.NGUOI_GIOI_THIEU,
                TEN_NGUOI_GIOI_THIEU = dp01.TEN_NGUOI_GIOI_THIEU,
                CONTRACT_COUTS_DAY = dp01.CONTRACT_COUTS_DAY,
                SO_KY_AD_LSDB = dp01.SO_KY_AD_LSDB,
                UNTBUSCD = dp01.UNTBUSCD,
                TYGIA = dp01.TYGIA
            };
        }

        private DP01 MapFromCreateDto(DP01CreateDto createDto)
        {
            return new DP01
            {
                NGAY_DL = createDto.NGAY_DL ?? DateTime.Now,

                // All 63 business columns
                MA_CN = createDto.MA_CN,
                TAI_KHOAN_HACH_TOAN = createDto.TAI_KHOAN_HACH_TOAN,
                MA_KH = createDto.MA_KH,
                TEN_KH = createDto.TEN_KH,
                DP_TYPE_NAME = createDto.DP_TYPE_NAME,
                CCY = createDto.CCY,
                CURRENT_BALANCE = createDto.CURRENT_BALANCE,
                RATE = createDto.RATE,
                SO_TAI_KHOAN = createDto.SO_TAI_KHOAN,
                OPENING_DATE = createDto.OPENING_DATE,
                MATURITY_DATE = createDto.MATURITY_DATE,
                ADDRESS = createDto.ADDRESS,
                NOTENO = createDto.NOTENO,
                MONTH_TERM = createDto.MONTH_TERM,
                TERM_DP_NAME = createDto.TERM_DP_NAME,
                TIME_DP_NAME = createDto.TIME_DP_NAME,
                MA_PGD = createDto.MA_PGD,
                TEN_PGD = createDto.TEN_PGD,
                DP_TYPE_CODE = createDto.DP_TYPE_CODE,
                RENEW_DATE = createDto.RENEW_DATE,
                CUST_TYPE = createDto.CUST_TYPE,
                CUST_TYPE_NAME = createDto.CUST_TYPE_NAME,
                CUST_TYPE_DETAIL = createDto.CUST_TYPE_DETAIL,
                CUST_DETAIL_NAME = createDto.CUST_DETAIL_NAME,
                PREVIOUS_DP_CAP_DATE = createDto.PREVIOUS_DP_CAP_DATE,
                NEXT_DP_CAP_DATE = createDto.NEXT_DP_CAP_DATE,
                ID_NUMBER = createDto.ID_NUMBER,
                ISSUED_BY = createDto.ISSUED_BY,
                ISSUE_DATE = createDto.ISSUE_DATE,
                SEX_TYPE = createDto.SEX_TYPE,
                BIRTH_DATE = createDto.BIRTH_DATE,
                TELEPHONE = createDto.TELEPHONE,
                ACRUAL_AMOUNT = createDto.ACRUAL_AMOUNT,
                ACRUAL_AMOUNT_END = createDto.ACRUAL_AMOUNT_END,
                ACCOUNT_STATUS = createDto.ACCOUNT_STATUS,
                DRAMT = createDto.DRAMT,
                CRAMT = createDto.CRAMT,
                EMPLOYEE_NUMBER = createDto.EMPLOYEE_NUMBER,
                EMPLOYEE_NAME = createDto.EMPLOYEE_NAME,
                SPECIAL_RATE = createDto.SPECIAL_RATE,
                AUTO_RENEWAL = createDto.AUTO_RENEWAL,
                CLOSE_DATE = createDto.CLOSE_DATE,
                LOCAL_PROVIN_NAME = createDto.LOCAL_PROVIN_NAME,
                LOCAL_DISTRICT_NAME = createDto.LOCAL_DISTRICT_NAME,
                LOCAL_WARD_NAME = createDto.LOCAL_WARD_NAME,
                TERM_DP_TYPE = createDto.TERM_DP_TYPE,
                TIME_DP_TYPE = createDto.TIME_DP_TYPE,
                STATES_CODE = createDto.STATES_CODE,
                ZIP_CODE = createDto.ZIP_CODE,
                COUNTRY_CODE = createDto.COUNTRY_CODE,
                TAX_CODE_LOCATION = createDto.TAX_CODE_LOCATION,
                MA_CAN_BO_PT = createDto.MA_CAN_BO_PT,
                TEN_CAN_BO_PT = createDto.TEN_CAN_BO_PT,
                PHONG_CAN_BO_PT = createDto.PHONG_CAN_BO_PT,
                NGUOI_NUOC_NGOAI = createDto.NGUOI_NUOC_NGOAI,
                QUOC_TICH = createDto.QUOC_TICH,
                MA_CAN_BO_AGRIBANK = createDto.MA_CAN_BO_AGRIBANK,
                NGUOI_GIOI_THIEU = createDto.NGUOI_GIOI_THIEU,
                TEN_NGUOI_GIOI_THIEU = createDto.TEN_NGUOI_GIOI_THIEU,
                CONTRACT_COUTS_DAY = createDto.CONTRACT_COUTS_DAY,
                SO_KY_AD_LSDB = createDto.SO_KY_AD_LSDB,
                UNTBUSCD = createDto.UNTBUSCD,
                TYGIA = createDto.TYGIA
            };
        }

        private void UpdateFromDto(DP01 dp01, DP01UpdateDto updateDto)
        {
            // Update all 63 business columns
            dp01.MA_CN = updateDto.MA_CN;
            dp01.TAI_KHOAN_HACH_TOAN = updateDto.TAI_KHOAN_HACH_TOAN;
            dp01.MA_KH = updateDto.MA_KH;
            dp01.TEN_KH = updateDto.TEN_KH;
            dp01.DP_TYPE_NAME = updateDto.DP_TYPE_NAME;
            dp01.CCY = updateDto.CCY;
            dp01.CURRENT_BALANCE = updateDto.CURRENT_BALANCE;
            dp01.RATE = updateDto.RATE;
            dp01.SO_TAI_KHOAN = updateDto.SO_TAI_KHOAN;
            dp01.OPENING_DATE = updateDto.OPENING_DATE;
            dp01.MATURITY_DATE = updateDto.MATURITY_DATE;
            dp01.ADDRESS = updateDto.ADDRESS;
            dp01.NOTENO = updateDto.NOTENO;
            dp01.MONTH_TERM = updateDto.MONTH_TERM;
            dp01.TERM_DP_NAME = updateDto.TERM_DP_NAME;
            dp01.TIME_DP_NAME = updateDto.TIME_DP_NAME;
            dp01.MA_PGD = updateDto.MA_PGD;
            dp01.TEN_PGD = updateDto.TEN_PGD;
            dp01.DP_TYPE_CODE = updateDto.DP_TYPE_CODE;
            dp01.RENEW_DATE = updateDto.RENEW_DATE;
            dp01.CUST_TYPE = updateDto.CUST_TYPE;
            dp01.CUST_TYPE_NAME = updateDto.CUST_TYPE_NAME;
            dp01.CUST_TYPE_DETAIL = updateDto.CUST_TYPE_DETAIL;
            dp01.CUST_DETAIL_NAME = updateDto.CUST_DETAIL_NAME;
            dp01.PREVIOUS_DP_CAP_DATE = updateDto.PREVIOUS_DP_CAP_DATE;
            dp01.NEXT_DP_CAP_DATE = updateDto.NEXT_DP_CAP_DATE;
            dp01.ID_NUMBER = updateDto.ID_NUMBER;
            dp01.ISSUED_BY = updateDto.ISSUED_BY;
            dp01.ISSUE_DATE = updateDto.ISSUE_DATE;
            dp01.SEX_TYPE = updateDto.SEX_TYPE;
            dp01.BIRTH_DATE = updateDto.BIRTH_DATE;
            dp01.TELEPHONE = updateDto.TELEPHONE;
            dp01.ACRUAL_AMOUNT = updateDto.ACRUAL_AMOUNT;
            dp01.ACRUAL_AMOUNT_END = updateDto.ACRUAL_AMOUNT_END;
            dp01.ACCOUNT_STATUS = updateDto.ACCOUNT_STATUS;
            dp01.DRAMT = updateDto.DRAMT;
            dp01.CRAMT = updateDto.CRAMT;
            dp01.EMPLOYEE_NUMBER = updateDto.EMPLOYEE_NUMBER;
            dp01.EMPLOYEE_NAME = updateDto.EMPLOYEE_NAME;
            dp01.SPECIAL_RATE = updateDto.SPECIAL_RATE;
            dp01.AUTO_RENEWAL = updateDto.AUTO_RENEWAL;
            dp01.CLOSE_DATE = updateDto.CLOSE_DATE;
            dp01.LOCAL_PROVIN_NAME = updateDto.LOCAL_PROVIN_NAME;
            dp01.LOCAL_DISTRICT_NAME = updateDto.LOCAL_DISTRICT_NAME;
            dp01.LOCAL_WARD_NAME = updateDto.LOCAL_WARD_NAME;
            dp01.TERM_DP_TYPE = updateDto.TERM_DP_TYPE;
            dp01.TIME_DP_TYPE = updateDto.TIME_DP_TYPE;
            dp01.STATES_CODE = updateDto.STATES_CODE;
            dp01.ZIP_CODE = updateDto.ZIP_CODE;
            dp01.COUNTRY_CODE = updateDto.COUNTRY_CODE;
            dp01.TAX_CODE_LOCATION = updateDto.TAX_CODE_LOCATION;
            dp01.MA_CAN_BO_PT = updateDto.MA_CAN_BO_PT;
            dp01.TEN_CAN_BO_PT = updateDto.TEN_CAN_BO_PT;
            dp01.PHONG_CAN_BO_PT = updateDto.PHONG_CAN_BO_PT;
            dp01.NGUOI_NUOC_NGOAI = updateDto.NGUOI_NUOC_NGOAI;
            dp01.QUOC_TICH = updateDto.QUOC_TICH;
            dp01.MA_CAN_BO_AGRIBANK = updateDto.MA_CAN_BO_AGRIBANK;
            dp01.NGUOI_GIOI_THIEU = updateDto.NGUOI_GIOI_THIEU;
            dp01.TEN_NGUOI_GIOI_THIEU = updateDto.TEN_NGUOI_GIOI_THIEU;
            dp01.CONTRACT_COUTS_DAY = updateDto.CONTRACT_COUTS_DAY;
            dp01.SO_KY_AD_LSDB = updateDto.SO_KY_AD_LSDB;
            dp01.UNTBUSCD = updateDto.UNTBUSCD;
            dp01.TYGIA = updateDto.TYGIA;
        }
    }
}
