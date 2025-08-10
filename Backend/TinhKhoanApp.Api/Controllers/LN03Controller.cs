using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models.DTOs.LN03;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Repositories.Interfaces;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LN03Controller : ControllerBase
    {
        private readonly ILN03Repository _repository;
        private readonly ILogger<LN03Controller> _logger;

        public LN03Controller(ILN03Repository repository, ILogger<LN03Controller> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<LN03PreviewDto>>> GetPaged(int page = 1, int pageSize = 10, DateTime? ngayDL = null)
        {
            try
            {
                var result = await _repository.GetPagedAsync(page, pageSize, ngayDL);

                var previewDtos = result.Items.Select(entity => new LN03PreviewDto
                {
                    Id = entity.Id,
                    NGAY_DL = entity.NGAY_DL,
                    MACHINHANH = entity.MACHINHANH,
                    TENCHINHANH = entity.TENCHINHANH,
                    MAKH = entity.MAKH,
                    TENKH = entity.TENKH,
                    SOHOPDONG = entity.SOHOPDONG,
                    SOTIENXLRR = entity.SOTIENXLRR ?? 0,
                    NGAYPHATSINHXL = entity.NGAYPHATSINHXL,
                    THUNOSAUXL = entity.THUNOSAUXL ?? 0,
                    CONLAINGOAIBANG = entity.CONLAINGOAIBANG ?? 0,
                    DUNONOIBANG = entity.DUNONOIBANG ?? 0,
                    NHOMNO = entity.NHOMNO,
                    MACBTD = entity.MACBTD,
                    TENCBTD = entity.TENCBTD,
                    MAPGD = entity.MAPGD,
                    TAIKHOANHACHTOAN = entity.TAIKHOANHACHTOAN,
                    REFNO = entity.REFNO,
                    LOAINGUONVON = entity.LOAINGUONVON,
                    Column18 = entity.Column18,
                    Column19 = entity.Column19,
                    Column20 = entity.Column20 ?? 0,
                    CreatedDate = entity.CreatedAt,
                    UpdatedDate = entity.UpdatedAt,
                    FileName = entity.FileName
                }).ToList();

                return Ok(new PagedResult<LN03PreviewDto>
                {
                    Items = previewDtos,
                    TotalCount = result.TotalCount,
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged LN03 data");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LN03DetailsDto>> GetById(long id)
        {
            try
            {
                var entity = await _repository.GetEntityByIdAsync(id);
                if (entity == null)
                    return NotFound($"LN03 record with ID {id} not found");

                var detailsDto = new LN03DetailsDto
                {
                    Id = entity.Id,
                    NGAY_DL = entity.NGAY_DL,
                    MACHINHANH = entity.MACHINHANH,
                    TENCHINHANH = entity.TENCHINHANH,
                    MAKH = entity.MAKH,
                    TENKH = entity.TENKH,
                    SOHOPDONG = entity.SOHOPDONG,
                    SOTIENXLRR = entity.SOTIENXLRR ?? 0,
                    NGAYPHATSINHXL = entity.NGAYPHATSINHXL,
                    THUNOSAUXL = entity.THUNOSAUXL ?? 0,
                    CONLAINGOAIBANG = entity.CONLAINGOAIBANG ?? 0,
                    DUNONOIBANG = entity.DUNONOIBANG ?? 0,
                    NHOMNO = entity.NHOMNO,
                    MACBTD = entity.MACBTD,
                    TENCBTD = entity.TENCBTD,
                    MAPGD = entity.MAPGD,
                    TAIKHOANHACHTOAN = entity.TAIKHOANHACHTOAN,
                    REFNO = entity.REFNO,
                    LOAINGUONVON = entity.LOAINGUONVON,
                    Column18 = entity.Column18,
                    Column19 = entity.Column19,
                    Column20 = entity.Column20 ?? 0,
                    CreatedDate = entity.CreatedAt,
                    UpdatedDate = entity.UpdatedAt,
                    FileName = entity.FileName,
                    ValidFromDate = entity.SysStartTime,
                    ValidToDate = entity.SysEndTime,
                    ImportBatch = entity.ImportId?.ToString(),
                    DataSource = "CSV Import",
                    RecordVersion = 1,
                    IsActive = true,
                    LastModifiedBy = "System"
                };

                return Ok(detailsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting LN03 by ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("summary")]
        public async Task<ActionResult<LN03SummaryDto>> GetSummary(DateTime ngayDL)
        {
            try
            {
                var entities = await _repository.GetByDateAsync(ngayDL);

                var summary = new LN03SummaryDto
                {
                    NgayDL = ngayDL,
                    TotalRecords = entities.Count,
                    TotalBranches = entities.Select(x => x.MACHINHANH).Distinct().Count(),
                    TotalCustomers = entities.Select(x => x.MAKH).Distinct().Count(),
                    TotalContracts = entities.Select(x => x.SOHOPDONG).Distinct().Count(),
                    TotalSoTienXLRR = entities.Sum(x => x.SOTIENXLRR ?? 0),
                    TotalThuNoSauXL = entities.Sum(x => x.THUNOSAUXL ?? 0),
                    TotalConLaiNgoaiBang = entities.Sum(x => x.CONLAINGOAIBANG ?? 0),
                    TotalDuNoNoiBang = entities.Sum(x => x.DUNONOIBANG ?? 0),
                    LastImportDate = entities.Max(x => x.CreatedAt),
                    FileName = entities.FirstOrDefault()?.FileName ?? ""
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting LN03 summary for date {NgayDL}", ngayDL);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
