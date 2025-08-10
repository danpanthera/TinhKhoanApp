using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models.DTOs.RR01;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Repositories.Interfaces;
using TinhKhoanApp.Api.Models.Entities;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RR01Controller : ControllerBase
    {
        private readonly IBaseRepository<RR01Entity> _repository;
        private readonly ILogger<RR01Controller> _logger;

        public RR01Controller(IBaseRepository<RR01Entity> repository, ILogger<RR01Controller> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<RR01PreviewDto>>> GetPaged(int page = 1, int pageSize = 10)
        {
            try
            {
                var result = await _repository.GetPagedAsync(page, pageSize);

                var previewDtos = result.Items.Select(entity => new RR01PreviewDto
                {
                    Id = entity.Id,
                    NGAY_DL = entity.NGAY_DL,
                    CN_LOAI_I = entity.CN_LOAI_I,
                    BRCD = entity.BRCD,
                    MA_KH = entity.MA_KH,
                    TEN_KH = entity.TEN_KH,
                    SO_LDS = entity.SO_LDS ?? 0,
                    CCY = entity.CCY,
                    SO_LAV = entity.SO_LAV,
                    LOAI_KH = entity.LOAI_KH,
                    NGAY_GIAI_NGAN = entity.NGAY_GIAI_NGAN,
                    CreatedDate = entity.CreatedAt,
                    UpdatedDate = entity.UpdatedAt
                }).ToList();

                return Ok(new PagedResult<RR01PreviewDto>
                {
                    Items = previewDtos,
                    TotalCount = result.TotalCount,
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged RR01 data");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<RR01DetailsDto>> GetById(long id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                    return NotFound($"RR01 record with ID {id} not found");

                var detailsDto = new RR01DetailsDto
                {
                    Id = entity.Id,
                    NGAY_DL = entity.NGAY_DL,
                    CN_LOAI_I = entity.CN_LOAI_I,
                    BRCD = entity.BRCD,
                    MA_KH = entity.MA_KH,
                    TEN_KH = entity.TEN_KH,
                    SO_LDS = entity.SO_LDS ?? 0,
                    CCY = entity.CCY,
                    SO_LAV = entity.SO_LAV,
                    LOAI_KH = entity.LOAI_KH,
                    NGAY_GIAI_NGAN = entity.NGAY_GIAI_NGAN,
                    NGAY_DEN_HAN = entity.NGAY_DEN_HAN,
                    VAMC_FLG = entity.VAMC_FLG,
                    NGAY_XLRR = entity.NGAY_XLRR,
                    DUNO_GOC_BAN_DAU = entity.DUNO_GOC_BAN_DAU ?? 0,
                    DUNO_LAI_TICHLUY_BD = entity.DUNO_LAI_TICHLUY_BD ?? 0,
                    DOC_DAUKY_DA_THU_HT = entity.DOC_DAUKY_DA_THU_HT ?? 0,
                    DUNO_GOC_HIENTAI = entity.DUNO_GOC_HIENTAI ?? 0,
                    DUNO_LAI_HIENTAI = entity.DUNO_LAI_HIENTAI ?? 0,
                    DUNO_NGAN_HAN = entity.DUNO_NGAN_HAN ?? 0,
                    DUNO_TRUNG_HAN = entity.DUNO_TRUNG_HAN ?? 0,
                    DUNO_DAI_HAN = entity.DUNO_DAI_HAN ?? 0,
                    THU_GOC = entity.THU_GOC ?? 0,
                    THU_LAI = entity.THU_LAI ?? 0,
                    BDS = entity.BDS ?? 0,
                    DS = entity.DS ?? 0,
                    TSK = entity.TSK ?? 0,
                    CreatedDate = entity.CreatedAt,
                    UpdatedDate = entity.UpdatedAt
                };

                return Ok(detailsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting RR01 by ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("summary")]
        public async Task<ActionResult<RR01SummaryDto>> GetSummary()
        {
            try
            {
                var response = await _repository.GetAllAsync();
                if (!response.Success || response.Data == null)
                {
                    return StatusCode(500, "Failed to retrieve data");
                }

                var entities = response.Data.ToList();

                var summary = new RR01SummaryDto
                {
                    NgayDL = entities.FirstOrDefault()?.NGAY_DL ?? DateTime.Now,
                    TotalRecords = entities.Count(),
                    TotalBranches = entities.Select(x => x.BRCD).Distinct().Count(),
                    TotalCustomers = entities.Select(x => x.MA_KH).Distinct().Count(),
                    TotalOutstandingPrincipal = entities.Sum(x => x.DUNO_GOC_HIENTAI ?? 0),
                    TotalAccumulatedInterest = entities.Sum(x => x.DUNO_LAI_HIENTAI ?? 0),
                    TotalInterestRepayments = entities.Sum(x => x.THU_LAI ?? 0),
                    TotalPrincipalRepayments = entities.Sum(x => x.THU_GOC ?? 0),
                    LastUpdated = entities.Any() ? entities.Max(x => x.CreatedAt) : DateTime.UtcNow
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting RR01 summary");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
