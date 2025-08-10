using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models.DTOs.LN01;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Repositories.Interfaces;
using TinhKhoanApp.Api.Models.Entities;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LN01Controller : ControllerBase
    {
        private readonly IBaseRepository<LN01Entity> _repository;
        private readonly ILogger<LN01Controller> _logger;

        public LN01Controller(IBaseRepository<LN01Entity> repository, ILogger<LN01Controller> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<LN01PreviewDto>>> GetPaged(int page = 1, int pageSize = 10)
        {
            try
            {
                var result = await _repository.GetPagedAsync(page, pageSize);

                var previewDtos = result.Items.Select(entity => new LN01PreviewDto
                {
                    Id = entity.Id,
                    BRCD = entity.BRCD,
                    CUSTSEQ = entity.CUSTSEQ,
                    CUSTNM = entity.CUSTNM ?? "",
                    TAI_KHOAN = entity.TAI_KHOAN,
                    CCY = entity.CCY ?? "",
                    DU_NO = entity.DU_NO ?? 0,
                    TRANSACTION_DATE = entity.TRANSACTION_DATE,
                    LOAN_TYPE = entity.LOAN_TYPE ?? "",
                    DISBURSEMENT_AMOUNT = entity.DISBURSEMENT_AMOUNT ?? 0,
                    INTEREST_RATE = entity.INTEREST_RATE ?? 0,
                    OFFICER_NAME = entity.OFFICER_NAME ?? "",
                    CreatedAt = entity.CreatedAt,
                    UpdatedAt = entity.UpdatedAt
                }).ToList();

                return Ok(new PagedResult<LN01PreviewDto>
                {
                    Items = previewDtos,
                    TotalCount = result.TotalCount,
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged LN01 data");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LN01DetailsDto>> GetById(long id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                    return NotFound($"LN01 record with ID {id} not found");

                var detailsDto = new LN01DetailsDto
                {
                    Id = entity.Id,
                    BRCD = entity.BRCD,
                    CUSTSEQ = entity.CUSTSEQ,
                    CUSTNM = entity.CUSTNM ?? "",
                    TAI_KHOAN = entity.TAI_KHOAN,
                    CCY = entity.CCY ?? "",
                    DU_NO = entity.DU_NO ?? 0,
                    DSBSSEQ = entity.DSBSSEQ ?? "",
                    TRANSACTION_DATE = entity.TRANSACTION_DATE,
                    DSBSDT = entity.DSBSDT,
                    DISBUR_CCY = entity.DISBUR_CCY ?? "",
                    DISBURSEMENT_AMOUNT = entity.DISBURSEMENT_AMOUNT ?? 0,
                    DSBSMATDT = entity.DSBSMATDT,
                    BSRTCD = entity.BSRTCD ?? "",
                    INTEREST_RATE = entity.INTEREST_RATE ?? 0,
                    APPRSEQ = entity.APPRSEQ ?? "",
                    APPRDT = entity.APPRDT,
                    APPR_CCY = entity.APPR_CCY ?? "",
                    APPRAMT = entity.APPRAMT ?? 0,
                    APPRMATDT = entity.APPRMATDT,
                    LOAN_TYPE = entity.LOAN_TYPE ?? "",
                    FUND_RESOURCE_CODE = entity.FUND_RESOURCE_CODE ?? "",
                    FUND_PURPOSE_CODE = entity.FUND_PURPOSE_CODE ?? "",
                    REPAYMENT_AMOUNT = entity.REPAYMENT_AMOUNT ?? 0,
                    NEXT_REPAY_DATE = entity.NEXT_REPAY_DATE,
                    NEXT_REPAY_AMOUNT = entity.NEXT_REPAY_AMOUNT ?? 0,
                    NEXT_INT_REPAY_DATE = entity.NEXT_INT_REPAY_DATE,
                    OFFICER_ID = entity.OFFICER_ID ?? "",
                    OFFICER_NAME = entity.OFFICER_NAME ?? "",
                    INTEREST_AMOUNT = entity.INTEREST_AMOUNT ?? 0,
                    PASTDUE_INTEREST_AMOUNT = entity.PASTDUE_INTEREST_AMOUNT ?? 0,
                    TOTAL_INTEREST_REPAY_AMOUNT = entity.TOTAL_INTEREST_REPAY_AMOUNT ?? 0,
                    CUSTOMER_TYPE_CODE = entity.CUSTOMER_TYPE_CODE ?? "",
                    CUSTOMER_TYPE_CODE_DETAIL = entity.CUSTOMER_TYPE_CODE_DETAIL ?? "",
                    CreatedDate = entity.CreatedAt,
                    UpdatedDate = entity.UpdatedAt
                };

                return Ok(detailsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting LN01 by ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("summary")]
        public async Task<ActionResult<LN01SummaryDto>> GetSummary()
        {
            try
            {
                var response = await _repository.GetAllAsync();
                if (!response.Success || response.Data == null)
                {
                    return StatusCode(500, "Failed to retrieve data");
                }

                var entities = response.Data.ToList();

                var summary = new LN01SummaryDto
                {
                    NgayDL = entities.FirstOrDefault()?.TRANSACTION_DATE ?? DateTime.Now,
                    TotalRecords = entities.Count(),
                    TotalLoanAmount = entities.Sum(x => x.DISBURSEMENT_AMOUNT ?? 0),
                    TotalOutstandingAmount = entities.Sum(x => x.DU_NO ?? 0),
                    TotalOverdueAmount = entities.Sum(x => x.PASTDUE_INTEREST_AMOUNT ?? 0),
                    LastUpdated = DateTime.UtcNow
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting LN01 summary");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
