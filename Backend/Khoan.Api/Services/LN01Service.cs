using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.DTOs.LN01;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Services.Interfaces;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// LN01 Service - xử lý business logic cho LN01 với manual mapping
    /// </summary>
    public class LN01Service : ILN01Service
    {
        private readonly ILN01Repository _repository;
        private readonly ILogger<LN01Service> _logger;

        public LN01Service(ILN01Repository repository, ILogger<LN01Service> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<LN01PreviewDto>>> PreviewAsync(int take = 20)
        {
            try
            {
                var entities = await _repository.GetRecentAsync(take);
                var previews = entities.Select(ToPreview);
                return ApiResponse<IEnumerable<LN01PreviewDto>>.Ok(previews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN01Service.PreviewAsync");
                return ApiResponse<IEnumerable<LN01PreviewDto>>.Error($"Lỗi lấy preview LN01: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LN01DetailsDto>> GetByIdAsync(long id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync((int)id);
                if (entity == null)
                {
                    return ApiResponse<LN01DetailsDto>.Error("Không tìm thấy LN01", 404);
                }

                var details = ToDetails(entity);
                return ApiResponse<LN01DetailsDto>.Ok(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN01Service.GetByIdAsync for ID: {Id}", id);
                return ApiResponse<LN01DetailsDto>.Error($"Lỗi lấy LN01 ID {id}: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LN01DetailsDto>> CreateAsync(LN01CreateDto createDto)
        {
            try
            {
                var entity = ToEntity(createDto);
                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                var details = ToDetails(entity);
                return ApiResponse<LN01DetailsDto>.Ok(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN01Service.CreateAsync");
                return ApiResponse<LN01DetailsDto>.Error($"Lỗi tạo LN01 mới: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LN01DetailsDto>> UpdateAsync(LN01UpdateDto updateDto)
        {
            try
            {
                var existingEntity = await _repository.GetByIdAsync((int)updateDto.Id);
                if (existingEntity == null)
                {
                    return ApiResponse<LN01DetailsDto>.Error("Không tìm thấy LN01", 404);
                }

                UpdateEntity(existingEntity, updateDto);
                _repository.Update(existingEntity);
                await _repository.SaveChangesAsync();

                var details = ToDetails(existingEntity);
                return ApiResponse<LN01DetailsDto>.Ok(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN01Service.UpdateAsync for ID: {Id}", updateDto.Id);
                return ApiResponse<LN01DetailsDto>.Error($"Lỗi cập nhật LN01 ID {updateDto.Id}: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(long id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync((int)id);
                if (entity == null)
                {
                    return ApiResponse<bool>.Error("Không tìm thấy LN01", 404);
                }

                _repository.Remove(entity);
                await _repository.SaveChangesAsync();
                return ApiResponse<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN01Service.DeleteAsync for ID: {Id}", id);
                return ApiResponse<bool>.Error($"Lỗi xóa LN01 ID {id}: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN01PreviewDto>>> GetByDateAsync(DateTime date, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByDateAsync(date);
                var previews = entities.Take(maxResults).Select(ToPreview);
                return ApiResponse<IEnumerable<LN01PreviewDto>>.Ok(previews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN01Service.GetByDateAsync for date: {Date}", date);
                return ApiResponse<IEnumerable<LN01PreviewDto>>.Error($"Lỗi lấy LN01 theo ngày {date:dd/MM/yyyy}: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN01PreviewDto>>> GetByBranchCodeAsync(string branchCode, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByBranchCodeAsync(branchCode);
                var previews = entities.Take(maxResults).Select(ToPreview);
                return ApiResponse<IEnumerable<LN01PreviewDto>>.Ok(previews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN01Service.GetByBranchCodeAsync for branch: {BranchCode}", branchCode);
                return ApiResponse<IEnumerable<LN01PreviewDto>>.Error($"Lỗi lấy LN01 theo chi nhánh {branchCode}: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN01PreviewDto>>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByCustomerCodeAsync(customerCode);
                var previews = entities.Take(maxResults).Select(ToPreview);
                return ApiResponse<IEnumerable<LN01PreviewDto>>.Ok(previews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN01Service.GetByCustomerCodeAsync for customer: {CustomerCode}", customerCode);
                return ApiResponse<IEnumerable<LN01PreviewDto>>.Error($"Lỗi lấy LN01 theo khách hàng {customerCode}: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN01PreviewDto>>> GetByAccountNumberAsync(string accountNumber, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.FindAsync(x => x.TAI_KHOAN == accountNumber);
                var previews = entities.Take(maxResults).Select(ToPreview);
                return ApiResponse<IEnumerable<LN01PreviewDto>>.Ok(previews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN01Service.GetByAccountNumberAsync for account: {AccountNumber}", accountNumber);
                return ApiResponse<IEnumerable<LN01PreviewDto>>.Error($"Lỗi lấy LN01 theo tài khoản {accountNumber}: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN01PreviewDto>>> GetByDebtGroupAsync(string debtGroup, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.FindAsync(x => x.NHOM_NO == debtGroup);
                var previews = entities.Take(maxResults).Select(ToPreview);
                return ApiResponse<IEnumerable<LN01PreviewDto>>.Ok(previews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN01Service.GetByDebtGroupAsync for debt group: {DebtGroup}", debtGroup);
                return ApiResponse<IEnumerable<LN01PreviewDto>>.Error($"Lỗi lấy LN01 theo nhóm nợ {debtGroup}: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN01PreviewDto>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.FindAsync(x => x.NGAY_DL >= fromDate && x.NGAY_DL <= toDate);
                var previews = entities.Take(maxResults).Select(ToPreview);
                return ApiResponse<IEnumerable<LN01PreviewDto>>.Ok(previews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN01Service.GetByDateRangeAsync for range: {FromDate} - {ToDate}", fromDate, toDate);
                return ApiResponse<IEnumerable<LN01PreviewDto>>.Error($"Lỗi lấy LN01 từ {fromDate:dd/MM/yyyy} đến {toDate:dd/MM/yyyy}: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LN01SummaryDto>> GetSummaryAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var entities = await _repository.GetAllAsync();

                if (fromDate.HasValue || toDate.HasValue)
                {
                    entities = entities.Where(x =>
                        (!fromDate.HasValue || x.NGAY_DL >= fromDate.Value) &&
                        (!toDate.HasValue || x.NGAY_DL <= toDate.Value));
                }

                var summary = new LN01SummaryDto
                {
                    TotalRecords = entities.Count(),
                    TotalLoanAmount = entities.Sum(x => x.DU_NO ?? 0),
                    TotalDisbursementAmount = entities.Sum(x => x.DISBURSEMENT_AMOUNT ?? 0),
                    TotalInterestAmount = entities.Sum(x => x.INTEREST_AMOUNT ?? 0),
                    TotalBranches = entities.Select(x => x.BRCD).Distinct().Count(),
                    TotalCustomers = entities.Select(x => x.CUSTSEQ).Distinct().Count(),
                    EarliestDate = entities.Min(x => x.NGAY_DL),
                    LatestDate = entities.Max(x => x.NGAY_DL)
                };

                return ApiResponse<LN01SummaryDto>.Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN01Service.GetSummaryAsync");
                return ApiResponse<LN01SummaryDto>.Error($"Lỗi lấy tóm tắt LN01: {ex.Message}");
            }
        }

        public async Task<ApiResponse<decimal>> GetTotalLoanAmountByBranchAsync(string branchCode, DateTime? date = null)
        {
            try
            {
                var entities = await _repository.GetByBranchCodeAsync(branchCode);
                if (date.HasValue)
                {
                    entities = entities.Where(x => x.NGAY_DL?.Date == date.Value.Date);
                }
                var total = entities.Sum(x => x.DU_NO ?? 0);
                return ApiResponse<decimal>.Ok(total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN01Service.GetTotalLoanAmountByBranchAsync for branch: {BranchCode}", branchCode);
                return ApiResponse<decimal>.Error($"Lỗi lấy tổng số tiền vay chi nhánh {branchCode}: {ex.Message}");
            }
        }

        public async Task<ApiResponse<decimal>> GetTotalDisbursementByDateAsync(DateTime date)
        {
            try
            {
                var entities = await _repository.GetByDateAsync(date);
                var total = entities.Sum(x => x.DISBURSEMENT_AMOUNT ?? 0);
                return ApiResponse<decimal>.Ok(total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN01Service.GetTotalDisbursementByDateAsync for date: {Date}", date);
                return ApiResponse<decimal>.Error($"Lỗi lấy tổng giải ngân ngày {date:dd/MM/yyyy}: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Dictionary<string, int>>> GetLoanCountByDebtGroupAsync(DateTime? date = null)
        {
            try
            {
                var entities = await _repository.GetAllAsync();
                if (date.HasValue)
                {
                    entities = entities.Where(x => x.NGAY_DL?.Date == date.Value.Date);
                }

                var counts = entities
                    .GroupBy(x => x.NHOM_NO ?? "Unknown")
                    .ToDictionary(g => g.Key, g => g.Count());

                return ApiResponse<Dictionary<string, int>>.Ok(counts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN01Service.GetLoanCountByDebtGroupAsync");
                return ApiResponse<Dictionary<string, int>>.Error($"Lỗi lấy số lượng vay theo nhóm nợ: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN01PreviewDto>>> GetTopCustomersByLoanAmountAsync(int topCount = 10, DateTime? date = null)
        {
            try
            {
                var entities = await _repository.GetAllAsync();
                if (date.HasValue)
                {
                    entities = entities.Where(x => x.NGAY_DL?.Date == date.Value.Date);
                }

                var topEntities = entities
                    .OrderByDescending(x => x.DU_NO ?? 0)
                    .Take(topCount);

                var previews = topEntities.Select(ToPreview);
                return ApiResponse<IEnumerable<LN01PreviewDto>>.Ok(previews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN01Service.GetTopCustomersByLoanAmountAsync");
                return ApiResponse<IEnumerable<LN01PreviewDto>>.Error($"Lỗi lấy top khách hàng theo số tiền vay: {ex.Message}");
            }
        }

        #region Private Mapping Methods

        private static LN01PreviewDto ToPreview(LN01 entity)
        {
            return new LN01PreviewDto
            {
                Id = entity.Id,
                NGAY_DL = entity.NGAY_DL,
                BRCD = entity.BRCD,
                CUSTSEQ = entity.CUSTSEQ,
                CUSTNM = entity.CUSTNM,
                TAI_KHOAN = entity.TAI_KHOAN,
                CCY = entity.CCY,
                DU_NO = entity.DU_NO,
                DSBSDT = entity.DSBSDT,
                DISBURSEMENT_AMOUNT = entity.DISBURSEMENT_AMOUNT,
                APPRDT = entity.APPRDT,
                APPRAMT = entity.APPRAMT,
                LOAN_TYPE = entity.LOAN_TYPE,
                NHOM_NO = entity.NHOM_NO,
                CreatedAt = entity.CreatedAt
            };
        }

        private static LN01DetailsDto ToDetails(LN01 entity)
        {
            return new LN01DetailsDto
            {
                Id = entity.Id,
                NGAY_DL = entity.NGAY_DL ?? DateTime.MinValue,
                BRCD = entity.BRCD,
                CUSTSEQ = entity.CUSTSEQ,
                CUSTNM = entity.CUSTNM,
                TAI_KHOAN = entity.TAI_KHOAN,
                CCY = entity.CCY,
                DU_NO = entity.DU_NO,
                DSBSSEQ = entity.DSBSSEQ,
                TRANSACTION_DATE = entity.TRANSACTION_DATE,
                DSBSDT = entity.DSBSDT,
                DISBUR_CCY = entity.DISBUR_CCY,
                DISBURSEMENT_AMOUNT = entity.DISBURSEMENT_AMOUNT,
                DSBSMATDT = entity.DSBSMATDT,
                BSRTCD = entity.BSRTCD,
                INTEREST_RATE = entity.INTEREST_RATE,
                APPRSEQ = entity.APPRSEQ,
                APPRDT = entity.APPRDT,
                APPR_CCY = entity.APPR_CCY,
                APPRAMT = entity.APPRAMT,
                APPRMATDT = entity.APPRMATDT,
                LOAN_TYPE = entity.LOAN_TYPE,
                FUND_RESOURCE_CODE = entity.FUND_RESOURCE_CODE,
                FUND_PURPOSE_CODE = entity.FUND_PURPOSE_CODE,
                REPAYMENT_AMOUNT = entity.REPAYMENT_AMOUNT,
                NEXT_REPAY_DATE = entity.NEXT_REPAY_DATE,
                NEXT_REPAY_AMOUNT = entity.NEXT_REPAY_AMOUNT,
                NEXT_INT_REPAY_DATE = entity.NEXT_INT_REPAY_DATE,
                OFFICER_ID = entity.OFFICER_ID,
                OFFICER_NAME = entity.OFFICER_NAME,
                INTEREST_AMOUNT = entity.INTEREST_AMOUNT,
                PASTDUE_INTEREST_AMOUNT = entity.PASTDUE_INTEREST_AMOUNT,
                TOTAL_INTEREST_REPAY_AMOUNT = entity.TOTAL_INTEREST_REPAY_AMOUNT,
                CUSTOMER_TYPE_CODE = entity.CUSTOMER_TYPE_CODE,
                CUSTOMER_TYPE_CODE_DETAIL = entity.CUSTOMER_TYPE_CODE_DETAIL,
                TRCTCD = entity.TRCTCD,
                TRCTNM = entity.TRCTNM,
                ADDR1 = entity.ADDR1,
                PROVINCE = entity.PROVINCE,
                LCLPROVINNM = entity.LCLPROVINNM,
                DISTRICT = entity.DISTRICT,
                LCLDISTNM = entity.LCLDISTNM,
                COMMCD = entity.COMMCD,
                LCLWARDNM = entity.LCLWARDNM,
                LAST_REPAY_DATE = entity.LAST_REPAY_DATE,
                SECURED_PERCENT = entity.SECURED_PERCENT,
                NHOM_NO = entity.NHOM_NO,
                LAST_INT_CHARGE_DATE = entity.LAST_INT_CHARGE_DATE,
                EXEMPTINT = entity.EXEMPTINT,
                EXEMPTINTTYPE = entity.EXEMPTINTTYPE,
                EXEMPTINTAMT = entity.EXEMPTINTAMT,
                GRPNO = entity.GRPNO,
                BUSCD = entity.BUSCD,
                BSNSSCLTPCD = entity.BSNSSCLTPCD,
                USRIDOP = entity.USRIDOP,
                ACCRUAL_AMOUNT = entity.ACCRUAL_AMOUNT,
                ACCRUAL_AMOUNT_END_OF_MONTH = entity.ACCRUAL_AMOUNT_END_OF_MONTH,
                INTCMTH = entity.INTCMTH,
                INTRPYMTH = entity.INTRPYMTH,
                INTTRMMTH = entity.INTTRMMTH,
                YRDAYS = entity.YRDAYS,
                REMARK = entity.REMARK,
                CHITIEU = entity.CHITIEU,
                CTCV = entity.CTCV,
                CREDIT_LINE_YPE = entity.CREDIT_LINE_YPE,
                INT_LUMPSUM_PARTIAL_TYPE = entity.INT_LUMPSUM_PARTIAL_TYPE,
                INT_PARTIAL_PAYMENT_TYPE = entity.INT_PARTIAL_PAYMENT_TYPE,
                INT_PAYMENT_INTERVAL = entity.INT_PAYMENT_INTERVAL,
                AN_HAN_LAI = entity.AN_HAN_LAI,
                PHUONG_THUC_GIAI_NGAN_1 = entity.PHUONG_THUC_GIAI_NGAN_1,
                TAI_KHOAN_GIAI_NGAN_1 = entity.TAI_KHOAN_GIAI_NGAN_1,
                SO_TIEN_GIAI_NGAN_1 = entity.SO_TIEN_GIAI_NGAN_1,
                PHUONG_THUC_GIAI_NGAN_2 = entity.PHUONG_THUC_GIAI_NGAN_2,
                TAI_KHOAN_GIAI_NGAN_2 = entity.TAI_KHOAN_GIAI_NGAN_2,
                SO_TIEN_GIAI_NGAN_2 = entity.SO_TIEN_GIAI_NGAN_2,
                CMT_HC = entity.CMT_HC,
                NGAY_SINH = entity.NGAY_SINH,
                MA_CB_AGRI = entity.MA_CB_AGRI,
                MA_NGANH_KT = entity.MA_NGANH_KT,
                TY_GIA = entity.TY_GIA,
                OFFICER_IPCAS = entity.OFFICER_IPCAS,
                FILE_NAME = entity.FILE_NAME,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
                // Note: SysStartTime and SysEndTime are temporal shadow properties managed by EF Core
            };
        }

        private static LN01 ToEntity(LN01CreateDto createDto)
        {
            return new LN01
            {
                NGAY_DL = createDto.NGAY_DL,
                BRCD = createDto.BRCD,
                CUSTSEQ = createDto.CUSTSEQ,
                CUSTNM = createDto.CUSTNM,
                TAI_KHOAN = createDto.TAI_KHOAN,
                CCY = createDto.CCY,
                DU_NO = createDto.DU_NO,
                DSBSSEQ = createDto.DSBSSEQ,
                TRANSACTION_DATE = createDto.TRANSACTION_DATE,
                DSBSDT = createDto.DSBSDT,
                DISBUR_CCY = createDto.DISBUR_CCY,
                DISBURSEMENT_AMOUNT = createDto.DISBURSEMENT_AMOUNT,
                DSBSMATDT = createDto.DSBSMATDT,
                BSRTCD = createDto.BSRTCD,
                INTEREST_RATE = createDto.INTEREST_RATE,
                APPRSEQ = createDto.APPRSEQ,
                APPRDT = createDto.APPRDT,
                APPR_CCY = createDto.APPR_CCY,
                APPRAMT = createDto.APPRAMT,
                APPRMATDT = createDto.APPRMATDT,
                LOAN_TYPE = createDto.LOAN_TYPE,
                FUND_RESOURCE_CODE = createDto.FUND_RESOURCE_CODE,
                FUND_PURPOSE_CODE = createDto.FUND_PURPOSE_CODE,
                REPAYMENT_AMOUNT = createDto.REPAYMENT_AMOUNT,
                NEXT_REPAY_DATE = createDto.NEXT_REPAY_DATE,
                NEXT_REPAY_AMOUNT = createDto.NEXT_REPAY_AMOUNT,
                NEXT_INT_REPAY_DATE = createDto.NEXT_INT_REPAY_DATE,
                OFFICER_ID = createDto.OFFICER_ID,
                OFFICER_NAME = createDto.OFFICER_NAME,
                INTEREST_AMOUNT = createDto.INTEREST_AMOUNT,
                PASTDUE_INTEREST_AMOUNT = createDto.PASTDUE_INTEREST_AMOUNT,
                TOTAL_INTEREST_REPAY_AMOUNT = createDto.TOTAL_INTEREST_REPAY_AMOUNT,
                CUSTOMER_TYPE_CODE = createDto.CUSTOMER_TYPE_CODE,
                CUSTOMER_TYPE_CODE_DETAIL = createDto.CUSTOMER_TYPE_CODE_DETAIL,
                TRCTCD = createDto.TRCTCD,
                TRCTNM = createDto.TRCTNM,
                ADDR1 = createDto.ADDR1,
                PROVINCE = createDto.PROVINCE,
                LCLPROVINNM = createDto.LCLPROVINNM,
                DISTRICT = createDto.DISTRICT,
                LCLDISTNM = createDto.LCLDISTNM,
                COMMCD = createDto.COMMCD,
                LCLWARDNM = createDto.LCLWARDNM,
                LAST_REPAY_DATE = createDto.LAST_REPAY_DATE,
                SECURED_PERCENT = createDto.SECURED_PERCENT,
                NHOM_NO = createDto.NHOM_NO,
                LAST_INT_CHARGE_DATE = createDto.LAST_INT_CHARGE_DATE,
                EXEMPTINT = createDto.EXEMPTINT,
                EXEMPTINTTYPE = createDto.EXEMPTINTTYPE,
                EXEMPTINTAMT = createDto.EXEMPTINTAMT,
                GRPNO = createDto.GRPNO,
                BUSCD = createDto.BUSCD,
                BSNSSCLTPCD = createDto.BSNSSCLTPCD,
                USRIDOP = createDto.USRIDOP,
                ACCRUAL_AMOUNT = createDto.ACCRUAL_AMOUNT,
                ACCRUAL_AMOUNT_END_OF_MONTH = createDto.ACCRUAL_AMOUNT_END_OF_MONTH,
                INTCMTH = createDto.INTCMTH,
                INTRPYMTH = createDto.INTRPYMTH,
                INTTRMMTH = createDto.INTTRMMTH,
                YRDAYS = createDto.YRDAYS,
                REMARK = createDto.REMARK,
                CHITIEU = createDto.CHITIEU,
                CTCV = createDto.CTCV,
                CREDIT_LINE_YPE = createDto.CREDIT_LINE_YPE,
                INT_LUMPSUM_PARTIAL_TYPE = createDto.INT_LUMPSUM_PARTIAL_TYPE,
                INT_PARTIAL_PAYMENT_TYPE = createDto.INT_PARTIAL_PAYMENT_TYPE,
                INT_PAYMENT_INTERVAL = createDto.INT_PAYMENT_INTERVAL,
                AN_HAN_LAI = createDto.AN_HAN_LAI,
                PHUONG_THUC_GIAI_NGAN_1 = createDto.PHUONG_THUC_GIAI_NGAN_1,
                TAI_KHOAN_GIAI_NGAN_1 = createDto.TAI_KHOAN_GIAI_NGAN_1,
                SO_TIEN_GIAI_NGAN_1 = createDto.SO_TIEN_GIAI_NGAN_1,
                PHUONG_THUC_GIAI_NGAN_2 = createDto.PHUONG_THUC_GIAI_NGAN_2,
                TAI_KHOAN_GIAI_NGAN_2 = createDto.TAI_KHOAN_GIAI_NGAN_2,
                SO_TIEN_GIAI_NGAN_2 = createDto.SO_TIEN_GIAI_NGAN_2,
                CMT_HC = createDto.CMT_HC,
                NGAY_SINH = createDto.NGAY_SINH,
                MA_CB_AGRI = createDto.MA_CB_AGRI,
                MA_NGANH_KT = createDto.MA_NGANH_KT,
                TY_GIA = createDto.TY_GIA,
                OFFICER_IPCAS = createDto.OFFICER_IPCAS,
                FILE_NAME = createDto.FILE_NAME,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        private static void UpdateEntity(LN01 entity, LN01UpdateDto updateDto)
        {
            entity.BRCD = updateDto.BRCD;
            entity.CUSTSEQ = updateDto.CUSTSEQ;
            entity.CUSTNM = updateDto.CUSTNM;
            entity.TAI_KHOAN = updateDto.TAI_KHOAN;
            entity.CCY = updateDto.CCY;
            entity.DU_NO = updateDto.DU_NO;
            entity.DSBSSEQ = updateDto.DSBSSEQ;
            entity.TRANSACTION_DATE = updateDto.TRANSACTION_DATE;
            entity.DSBSDT = updateDto.DSBSDT;
            entity.DISBUR_CCY = updateDto.DISBUR_CCY;
            entity.DISBURSEMENT_AMOUNT = updateDto.DISBURSEMENT_AMOUNT;
            entity.DSBSMATDT = updateDto.DSBSMATDT;
            entity.BSRTCD = updateDto.BSRTCD;
            entity.INTEREST_RATE = updateDto.INTEREST_RATE;
            entity.APPRSEQ = updateDto.APPRSEQ;
            entity.APPRDT = updateDto.APPRDT;
            entity.APPR_CCY = updateDto.APPR_CCY;
            entity.APPRAMT = updateDto.APPRAMT;
            entity.APPRMATDT = updateDto.APPRMATDT;
            entity.LOAN_TYPE = updateDto.LOAN_TYPE;
            entity.FUND_RESOURCE_CODE = updateDto.FUND_RESOURCE_CODE;
            entity.FUND_PURPOSE_CODE = updateDto.FUND_PURPOSE_CODE;
            entity.REPAYMENT_AMOUNT = updateDto.REPAYMENT_AMOUNT;
            entity.NEXT_REPAY_DATE = updateDto.NEXT_REPAY_DATE;
            entity.NEXT_REPAY_AMOUNT = updateDto.NEXT_REPAY_AMOUNT;
            entity.NEXT_INT_REPAY_DATE = updateDto.NEXT_INT_REPAY_DATE;
            entity.OFFICER_ID = updateDto.OFFICER_ID;
            entity.OFFICER_NAME = updateDto.OFFICER_NAME;
            entity.INTEREST_AMOUNT = updateDto.INTEREST_AMOUNT;
            entity.PASTDUE_INTEREST_AMOUNT = updateDto.PASTDUE_INTEREST_AMOUNT;
            entity.TOTAL_INTEREST_REPAY_AMOUNT = updateDto.TOTAL_INTEREST_REPAY_AMOUNT;
            entity.CUSTOMER_TYPE_CODE = updateDto.CUSTOMER_TYPE_CODE;
            entity.CUSTOMER_TYPE_CODE_DETAIL = updateDto.CUSTOMER_TYPE_CODE_DETAIL;
            entity.TRCTCD = updateDto.TRCTCD;
            entity.TRCTNM = updateDto.TRCTNM;
            entity.ADDR1 = updateDto.ADDR1;
            entity.PROVINCE = updateDto.PROVINCE;
            entity.LCLPROVINNM = updateDto.LCLPROVINNM;
            entity.DISTRICT = updateDto.DISTRICT;
            entity.LCLDISTNM = updateDto.LCLDISTNM;
            entity.COMMCD = updateDto.COMMCD;
            entity.LCLWARDNM = updateDto.LCLWARDNM;
            entity.LAST_REPAY_DATE = updateDto.LAST_REPAY_DATE;
            entity.SECURED_PERCENT = updateDto.SECURED_PERCENT;
            entity.NHOM_NO = updateDto.NHOM_NO;
            entity.LAST_INT_CHARGE_DATE = updateDto.LAST_INT_CHARGE_DATE;
            entity.EXEMPTINT = updateDto.EXEMPTINT;
            entity.EXEMPTINTTYPE = updateDto.EXEMPTINTTYPE;
            entity.EXEMPTINTAMT = updateDto.EXEMPTINTAMT;
            entity.GRPNO = updateDto.GRPNO;
            entity.BUSCD = updateDto.BUSCD;
            entity.BSNSSCLTPCD = updateDto.BSNSSCLTPCD;
            entity.USRIDOP = updateDto.USRIDOP;
            entity.ACCRUAL_AMOUNT = updateDto.ACCRUAL_AMOUNT;
            entity.ACCRUAL_AMOUNT_END_OF_MONTH = updateDto.ACCRUAL_AMOUNT_END_OF_MONTH;
            entity.INTCMTH = updateDto.INTCMTH;
            entity.INTRPYMTH = updateDto.INTRPYMTH;
            entity.INTTRMMTH = updateDto.INTTRMMTH;
            entity.YRDAYS = updateDto.YRDAYS;
            entity.REMARK = updateDto.REMARK;
            entity.CHITIEU = updateDto.CHITIEU;
            entity.CTCV = updateDto.CTCV;
            entity.CREDIT_LINE_YPE = updateDto.CREDIT_LINE_YPE;
            entity.INT_LUMPSUM_PARTIAL_TYPE = updateDto.INT_LUMPSUM_PARTIAL_TYPE;
            entity.INT_PARTIAL_PAYMENT_TYPE = updateDto.INT_PARTIAL_PAYMENT_TYPE;
            entity.INT_PAYMENT_INTERVAL = updateDto.INT_PAYMENT_INTERVAL;
            entity.AN_HAN_LAI = updateDto.AN_HAN_LAI;
            entity.PHUONG_THUC_GIAI_NGAN_1 = updateDto.PHUONG_THUC_GIAI_NGAN_1;
            entity.TAI_KHOAN_GIAI_NGAN_1 = updateDto.TAI_KHOAN_GIAI_NGAN_1;
            entity.SO_TIEN_GIAI_NGAN_1 = updateDto.SO_TIEN_GIAI_NGAN_1;
            entity.PHUONG_THUC_GIAI_NGAN_2 = updateDto.PHUONG_THUC_GIAI_NGAN_2;
            entity.TAI_KHOAN_GIAI_NGAN_2 = updateDto.TAI_KHOAN_GIAI_NGAN_2;
            entity.SO_TIEN_GIAI_NGAN_2 = updateDto.SO_TIEN_GIAI_NGAN_2;
            entity.CMT_HC = updateDto.CMT_HC;
            entity.NGAY_SINH = updateDto.NGAY_SINH;
            entity.MA_CB_AGRI = updateDto.MA_CB_AGRI;
            entity.MA_NGANH_KT = updateDto.MA_NGANH_KT;
            entity.TY_GIA = updateDto.TY_GIA;
            entity.OFFICER_IPCAS = updateDto.OFFICER_IPCAS;
            entity.FILE_NAME = updateDto.FILE_NAME;
            entity.UpdatedAt = DateTime.Now;
        }

        #endregion
    }
}
