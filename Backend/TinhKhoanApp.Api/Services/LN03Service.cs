using Khoan.Api.Models.Common;
using Khoan.Api.Models.DataTables;
using Khoan.Api.Models.DTOs.LN03;
using Khoan.Api.Repositories;
using Khoan.Api.Services.Interfaces;

namespace Khoan.Api.Services
{
    /// <summary>
    /// LN03 Service - business logic for LN03 CSV-first table
    /// </summary>
    public class LN03Service : ILN03Service
    {
        private readonly ILN03DataRepository _repository;
        private readonly ILogger<LN03Service> _logger;

        public LN03Service(ILN03DataRepository repository, ILogger<LN03Service> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiResponse<PagedResult<LN03PreviewDto>>> GetPreviewAsync(int pageNumber, int pageSize, DateTime? ngayDL = null)
        {
            try
            {
                IEnumerable<LN03> source;
                if (ngayDL.HasValue)
                {
                    var d = ngayDL.Value.Date;
                    source = await _repository.FindAsync(x => x.NGAY_DL.Date == d);
                }
                else
                {
                    source = await _repository.GetAllAsync();
                }

                var ordered = source
                    .OrderByDescending(x => x.NGAY_DL)
                    .ThenByDescending(x => x.CREATED_DATE);

                var total = ordered.Count();
                var items = ordered
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var dto = new PagedResult<LN03PreviewDto>
                {
                    Items = items.Select(ToPreview).ToList(),
                    TotalCount = total,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return ApiResponse<PagedResult<LN03PreviewDto>>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Service.GetPreviewAsync");
                return ApiResponse<PagedResult<LN03PreviewDto>>.Error($"Lỗi lấy danh sách LN03: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LN03DetailsDto>> GetByIdAsync(long id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync((int)id);
                if (entity == null)
                    return ApiResponse<LN03DetailsDto>.Error("Không tìm thấy LN03", 404);

                return ApiResponse<LN03DetailsDto>.Ok(ToDetails(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Service.GetByIdAsync for ID {Id}", id);
                return ApiResponse<LN03DetailsDto>.Error($"Lỗi lấy LN03 ID {id}: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LN03DetailsDto>> CreateAsync(LN03CreateDto dto)
        {
            try
            {
                var entity = ToEntity(dto);
                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                return ApiResponse<LN03DetailsDto>.Ok(ToDetails(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Service.CreateAsync");
                return ApiResponse<LN03DetailsDto>.Error($"Lỗi tạo LN03: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LN03DetailsDto>> UpdateAsync(long id, LN03UpdateDto dto)
        {
            try
            {
                var entity = await _repository.GetByIdAsync((int)id);
                if (entity == null)
                    return ApiResponse<LN03DetailsDto>.Error("Không tìm thấy LN03", 404);

                UpdateEntity(entity, dto);
                _repository.Update(entity);
                await _repository.SaveChangesAsync();

                return ApiResponse<LN03DetailsDto>.Ok(ToDetails(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Service.UpdateAsync for ID {Id}", id);
                return ApiResponse<LN03DetailsDto>.Error($"Lỗi cập nhật LN03 ID {id}: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(long id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync((int)id);
                if (entity == null)
                    return ApiResponse<bool>.Error("Không tìm thấy LN03", 404);

                _repository.Remove(entity);
                await _repository.SaveChangesAsync();
                return ApiResponse<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Service.DeleteAsync for ID {Id}", id);
                return ApiResponse<bool>.Error($"Lỗi xóa LN03 ID {id}: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN03PreviewDto>>> GetByDateAsync(DateTime date, int maxResults = 100)
        {
            try
            {
                var items = await _repository.GetByDateAsync(date, maxResults);
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Ok(items.Select(ToPreview));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Service.GetByDateAsync for date {Date}", date);
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Error($"Lỗi lấy LN03 theo ngày {date:dd/MM/yyyy}: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN03PreviewDto>>> GetByBranchAsync(string branchCode, int maxResults = 100)
        {
            try
            {
                var items = await _repository.GetByBranchAsync(branchCode, maxResults);
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Ok(items.Select(ToPreview));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Service.GetByBranchAsync for branch {Branch}", branchCode);
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Error($"Lỗi lấy LN03 theo chi nhánh {branchCode}: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN03PreviewDto>>> GetByCustomerAsync(string customerCode, int maxResults = 100)
        {
            try
            {
                var items = await _repository.GetByCustomerAsync(customerCode, maxResults);
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Ok(items.Select(ToPreview));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Service.GetByCustomerAsync for customer {Customer}", customerCode);
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Error($"Lỗi lấy LN03 theo khách hàng {customerCode}: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LN03ProcessingSummaryDto>> GetProcessingSummaryAsync(DateTime? ngayDL = null)
        {
            try
            {
                IEnumerable<LN03> list;
                if (ngayDL.HasValue)
                {
                    var d = ngayDL.Value.Date;
                    list = await _repository.FindAsync(x => x.NGAY_DL.Date == d);
                }
                else
                {
                    list = await _repository.GetAllAsync();
                }

                var summary = new LN03ProcessingSummaryDto
                {
                    TotalContracts = list.Count(),
                    TotalProcessingAmount = list.Sum(x => x.SOTIENXLRR ?? 0),
                    AverageProcessingAmount = list.Any() ? list.Average(x => x.SOTIENXLRR ?? 0) : 0,
                    ContractsByBranch = list.GroupBy(x => x.MACHINHANH ?? "Unknown").ToDictionary(g => g.Key, g => g.Count()),
                    ContractsByOfficer = list.GroupBy(x => x.MACBTD ?? "Unknown").ToDictionary(g => g.Key, g => g.Count())
                };

                return ApiResponse<LN03ProcessingSummaryDto>.Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Service.GetProcessingSummaryAsync");
                return ApiResponse<LN03ProcessingSummaryDto>.Error($"Lỗi lấy tóm tắt LN03: {ex.Message}");
            }
        }

        private static LN03PreviewDto ToPreview(LN03 e)
        {
            return new LN03PreviewDto
            {
                Id = e.Id,
                NGAY_DL = e.NGAY_DL,
                MACHINHANH = e.MACHINHANH,
                TENCHINHANH = e.TENCHINHANH,
                MAKH = e.MAKH,
                TENKH = e.TENKH,
                SOHOPDONG = e.SOHOPDONG,
                SOTIENXLRR = e.SOTIENXLRR,
                NGAYPHATSINHXL = e.NGAYPHATSINHXL,
                THUNOSAUXL = e.THUNOSAUXL,
                CONLAINGOAIBANG = e.CONLAINGOAIBANG,
                DUNONOIBANG = e.DUNONOIBANG,
                NHOMNO = e.NHOMNO,
                MACBTD = e.MACBTD,
                TENCBTD = e.TENCBTD,
                MAPGD = e.MAPGD,
                TAIKHOANHACHTOAN = e.TAIKHOANHACHTOAN,
                REFNO = e.REFNO,
                LOAINGUONVON = e.LOAINGUONVON,
                MALOAI = e.Column18,
                LOAIKHACHHANG = e.Column19,
                SOTIEN = e.Column20,
                CreatedAt = e.CREATED_DATE,
                FileName = e.FILE_ORIGIN
            };
        }

        private static LN03DetailsDto ToDetails(LN03 e)
        {
            return new LN03DetailsDto
            {
                Id = e.Id,
                NGAY_DL = e.NGAY_DL,
                MACHINHANH = e.MACHINHANH,
                TENCHINHANH = e.TENCHINHANH,
                MAKH = e.MAKH,
                TENKH = e.TENKH,
                SOHOPDONG = e.SOHOPDONG,
                SOTIENXLRR = e.SOTIENXLRR,
                NGAYPHATSINHXL = e.NGAYPHATSINHXL,
                THUNOSAUXL = e.THUNOSAUXL,
                CONLAINGOAIBANG = e.CONLAINGOAIBANG,
                DUNONOIBANG = e.DUNONOIBANG,
                NHOMNO = e.NHOMNO,
                MACBTD = e.MACBTD,
                TENCBTD = e.TENCBTD,
                MAPGD = e.MAPGD,
                TAIKHOANHACHTOAN = e.TAIKHOANHACHTOAN,
                REFNO = e.REFNO,
                LOAINGUONVON = e.LOAINGUONVON,
                MALOAI = e.Column18,
                LOAIKHACHHANG = e.Column19,
                SOTIEN = e.Column20,
                FILE_NAME = e.FILE_ORIGIN,
                CreatedAt = e.CREATED_DATE,
                UpdatedAt = e.CREATED_DATE
            };
        }

        private static LN03 ToEntity(LN03CreateDto d)
        {
            return new LN03
            {
                NGAY_DL = d.NGAY_DL,
                MACHINHANH = d.MACHINHANH,
                TENCHINHANH = d.TENCHINHANH,
                MAKH = d.MAKH,
                TENKH = d.TENKH,
                SOHOPDONG = d.SOHOPDONG,
                SOTIENXLRR = d.SOTIENXLRR,
                NGAYPHATSINHXL = d.NGAYPHATSINHXL,
                THUNOSAUXL = d.THUNOSAUXL,
                CONLAINGOAIBANG = d.CONLAINGOAIBANG,
                DUNONOIBANG = d.DUNONOIBANG,
                NHOMNO = d.NHOMNO,
                MACBTD = d.MACBTD,
                TENCBTD = d.TENCBTD,
                MAPGD = d.MAPGD,
                TAIKHOANHACHTOAN = d.TAIKHOANHACHTOAN,
                REFNO = d.REFNO,
                LOAINGUONVON = d.LOAINGUONVON,
                Column18 = d.MALOAI,
                Column19 = d.LOAIKHACHHANG,
                Column20 = d.SOTIEN,
                FILE_ORIGIN = d.FILE_NAME,
                CREATED_DATE = DateTime.UtcNow
            };
        }

        private static void UpdateEntity(LN03 e, LN03UpdateDto d)
        {
            e.NGAY_DL = d.NGAY_DL;
            e.MACHINHANH = d.MACHINHANH;
            e.TENCHINHANH = d.TENCHINHANH;
            e.MAKH = d.MAKH;
            e.TENKH = d.TENKH;
            e.SOHOPDONG = d.SOHOPDONG;
            e.SOTIENXLRR = d.SOTIENXLRR;
            e.NGAYPHATSINHXL = d.NGAYPHATSINHXL;
            e.THUNOSAUXL = d.THUNOSAUXL;
            e.CONLAINGOAIBANG = d.CONLAINGOAIBANG;
            e.DUNONOIBANG = d.DUNONOIBANG;
            e.NHOMNO = d.NHOMNO;
            e.MACBTD = d.MACBTD;
            e.TENCBTD = d.TENCBTD;
            e.MAPGD = d.MAPGD;
            e.TAIKHOANHACHTOAN = d.TAIKHOANHACHTOAN;
            e.REFNO = d.REFNO;
            e.LOAINGUONVON = d.LOAINGUONVON;
            e.Column18 = d.MALOAI;
            e.Column19 = d.LOAIKHACHHANG;
            e.Column20 = d.SOTIEN;
            e.FILE_ORIGIN = d.FILE_NAME;
            // CREATED_DATE left as original creation timestamp
        }
    }
}
