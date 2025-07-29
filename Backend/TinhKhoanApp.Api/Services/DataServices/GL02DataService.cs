using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Utilities;

namespace TinhKhoanApp.Api.Services.DataServices
{
    /// <summary>
    /// Service xử lý business logic liên quan đến dữ liệu GL02 (Giao dịch sổ cái)
    /// </summary>
    public class GL02DataService : IGL02DataService
    {
        private readonly IGL02Repository _repository;
        private readonly ILogger<GL02DataService> _logger;
        private readonly ApplicationDbContext _context;

        public GL02DataService(IGL02Repository repository, ILogger<GL02DataService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = _repository.GetDbContext();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GL02PreviewDto>> GetGL02PreviewAsync(int count = 10)
        {
            try
            {
                var entities = await _repository.GetRecentAsync(count);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy preview dữ liệu GL02");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<GL02DetailDto?> GetGL02DetailAsync(long id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                return entity != null ? MapToDetailDto(entity) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết bản ghi GL02 với ID {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GL02PreviewDto>> GetGL02ByDateAsync(DateTime date, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByDateAsync(date, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu GL02 theo ngày {Date}", date);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GL02PreviewDto>> GetGL02ByDateRangeAsync(DateTime fromDate, DateTime toDate, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByDateRangeAsync(fromDate, toDate, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu GL02 theo khoảng thời gian {FromDate} - {ToDate}", fromDate, toDate);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GL02PreviewDto>> GetGL02ByBranchCodeAsync(string branchCode, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByBranchCodeAsync(branchCode, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu GL02 theo chi nhánh {BranchCode}", branchCode);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GL02PreviewDto>> GetGL02ByUnitAsync(string unit, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByUnitAsync(unit, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu GL02 theo đơn vị {Unit}", unit);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GL02PreviewDto>> GetGL02ByAccountCodeAsync(string accountCode, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByAccountCodeAsync(accountCode, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu GL02 theo tài khoản {AccountCode}", accountCode);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GL02PreviewDto>> GetGL02ByCustomerAsync(string customer, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByCustomerAsync(customer, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu GL02 theo khách hàng {Customer}", customer);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GL02PreviewDto>> GetGL02ByTransactionTypeAsync(string transactionType, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByTransactionTypeAsync(transactionType, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu GL02 theo loại giao dịch {TransactionType}", transactionType);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<GL02SummaryDto> GetGL02SummaryByDateAsync(DateTime date)
        {
            try
            {
                var entities = await _repository.GetByDateAsync(date, 1000);
                var totalDr = await _repository.GetTotalTransactionsByDateAsync(date, "DR");
                var totalCr = await _repository.GetTotalTransactionsByDateAsync(date, "CR");

                return new GL02SummaryDto
                {
                    NgayDL = date,
                    TongSoBanGhi = entities.Count(),
                    TongTienNo = totalDr,
                    TongTienCo = totalCr,
                    SoLuongTheoChiNhanh = entities
                        .GroupBy(e => e.TRBRCD ?? "Unknown")
                        .OrderByDescending(g => g.Count())
                        .Take(10)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    SoLuongTheoDonVi = entities
                        .GroupBy(e => e.UNIT ?? "Unknown")
                        .OrderByDescending(g => g.Count())
                        .Take(10)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    SoLuongTheoLoaiGiaoDich = entities
                        .GroupBy(e => e.TRTP ?? "Unknown")
                        .OrderByDescending(g => g.Count())
                        .Take(5)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    TongTienTheoChiNhanh = entities
                        .GroupBy(e => e.TRBRCD ?? "Unknown")
                        .OrderByDescending(g => g.Sum(x => (x.DRAMOUNT ?? 0) + (x.CRAMOUNT ?? 0)))
                        .Take(10)
                        .ToDictionary(g => g.Key, g => g.Sum(x => (x.DRAMOUNT ?? 0) + (x.CRAMOUNT ?? 0))),
                    TongTienTheoDonVi = entities
                        .GroupBy(e => e.UNIT ?? "Unknown")
                        .OrderByDescending(g => g.Sum(x => (x.DRAMOUNT ?? 0) + (x.CRAMOUNT ?? 0)))
                        .Take(10)
                        .ToDictionary(g => g.Key, g => g.Sum(x => (x.DRAMOUNT ?? 0) + (x.CRAMOUNT ?? 0)))
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tổng hợp GL02 theo ngày {Date}", date);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<GL02SummaryDto> GetGL02SummaryByBranchAsync(string branchCode, DateTime? date = null)
        {
            try
            {
                IEnumerable<GL02> entities;
                decimal totalDr, totalCr;

                if (date.HasValue)
                {
                    entities = await _context.GL02
                        .Where(x => x.TRBRCD == branchCode && x.NGAY_DL.Date == date.Value.Date)
                        .ToListAsync();
                    totalDr = await _repository.GetTotalTransactionsByBranchAsync(branchCode, "DR");
                    totalCr = await _repository.GetTotalTransactionsByBranchAsync(branchCode, "CR");
                }
                else
                {
                    entities = await _repository.GetByBranchCodeAsync(branchCode, 1000);
                    totalDr = entities.Where(x => x.DRAMOUNT > 0).Sum(x => x.DRAMOUNT ?? 0);
                    totalCr = entities.Where(x => x.CRAMOUNT > 0).Sum(x => x.CRAMOUNT ?? 0);
                }

                return new GL02SummaryDto
                {
                    MaChiNhanh = branchCode,
                    NgayDL = date,
                    TongSoBanGhi = entities.Count(),
                    TongTienNo = totalDr,
                    TongTienCo = totalCr,
                    SoLuongTheoDonVi = entities
                        .GroupBy(e => e.UNIT ?? "Unknown")
                        .OrderByDescending(g => g.Count())
                        .Take(10)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    SoLuongTheoLoaiGiaoDich = entities
                        .GroupBy(e => e.TRTP ?? "Unknown")
                        .OrderByDescending(g => g.Count())
                        .Take(5)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    TongTienTheoDonVi = entities
                        .GroupBy(e => e.UNIT ?? "Unknown")
                        .OrderByDescending(g => g.Sum(x => (x.DRAMOUNT ?? 0) + (x.CRAMOUNT ?? 0)))
                        .Take(10)
                        .ToDictionary(g => g.Key, g => g.Sum(x => (x.DRAMOUNT ?? 0) + (x.CRAMOUNT ?? 0)))
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tổng hợp GL02 theo chi nhánh {BranchCode}", branchCode);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<GL02SummaryDto> GetGL02SummaryByUnitAsync(string unit, DateTime? date = null)
        {
            try
            {
                IEnumerable<GL02> entities;
                decimal totalDr, totalCr;

                if (date.HasValue)
                {
                    entities = await _context.GL02
                        .Where(x => x.UNIT == unit && x.NGAY_DL.Date == date.Value.Date)
                        .ToListAsync();
                    totalDr = await _repository.GetTotalTransactionsByUnitAsync(unit, "DR");
                    totalCr = await _repository.GetTotalTransactionsByUnitAsync(unit, "CR");
                }
                else
                {
                    entities = await _repository.GetByUnitAsync(unit, 1000);
                    totalDr = entities.Where(x => x.DRAMOUNT > 0).Sum(x => x.DRAMOUNT ?? 0);
                    totalCr = entities.Where(x => x.CRAMOUNT > 0).Sum(x => x.CRAMOUNT ?? 0);
                }

                return new GL02SummaryDto
                {
                    MaDonVi = unit,
                    NgayDL = date,
                    TongSoBanGhi = entities.Count(),
                    TongTienNo = totalDr,
                    TongTienCo = totalCr,
                    SoLuongTheoChiNhanh = entities
                        .GroupBy(e => e.TRBRCD ?? "Unknown")
                        .OrderByDescending(g => g.Count())
                        .Take(10)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    SoLuongTheoLoaiGiaoDich = entities
                        .GroupBy(e => e.TRTP ?? "Unknown")
                        .OrderByDescending(g => g.Count())
                        .Take(5)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    TongTienTheoChiNhanh = entities
                        .GroupBy(e => e.TRBRCD ?? "Unknown")
                        .OrderByDescending(g => g.Sum(x => (x.DRAMOUNT ?? 0) + (x.CRAMOUNT ?? 0)))
                        .Take(10)
                        .ToDictionary(g => g.Key, g => g.Sum(x => (x.DRAMOUNT ?? 0) + (x.CRAMOUNT ?? 0)))
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tổng hợp GL02 theo đơn vị {Unit}", unit);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<PagedApiResponse<GL02PreviewDto>> SearchGL02Async(
            string? keyword,
            string? branchCode,
            string? unit,
            string? accountCode,
            string? customer,
            DateTime? fromDate,
            DateTime? toDate,
            string? transactionType,
            int page = 1,
            int pageSize = 20)
        {
            try
            {
                // Xây dựng điều kiện tìm kiếm
                Expression<Func<GL02, bool>> predicate = e => true;

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    predicate = e =>
                        (e.TRBRCD != null && e.TRBRCD.Contains(keyword)) ||
                        (e.LOCAC != null && e.LOCAC.Contains(keyword)) ||
                        (e.CUSTOMER != null && e.CUSTOMER.Contains(keyword)) ||
                        (e.REFERENCE != null && e.REFERENCE.Contains(keyword)) ||
                        (e.REMARK != null && e.REMARK.Contains(keyword));
                }

                if (!string.IsNullOrEmpty(branchCode))
                {
                    predicate = PredicateBuilder.And(predicate, e => e.TRBRCD == branchCode);
                }

                if (!string.IsNullOrEmpty(unit))
                {
                    predicate = PredicateBuilder.And(predicate, e => e.UNIT == unit);
                }

                if (!string.IsNullOrEmpty(accountCode))
                {
                    predicate = PredicateBuilder.And(predicate, e => e.LOCAC == accountCode);
                }

                if (!string.IsNullOrEmpty(customer))
                {
                    predicate = PredicateBuilder.And(predicate, e => e.CUSTOMER == customer);
                }

                if (!string.IsNullOrEmpty(transactionType))
                {
                    predicate = PredicateBuilder.And(predicate, e => e.TRTP == transactionType);
                }

                if (fromDate.HasValue)
                {
                    predicate = PredicateBuilder.And(predicate, e => e.NGAY_DL >= fromDate);
                }

                if (toDate.HasValue)
                {
                    predicate = PredicateBuilder.And(predicate, e => e.NGAY_DL <= toDate);
                }

                // Thực hiện tìm kiếm với phân trang
                // Lấy dữ liệu phân trang
                var (totalCount, items) = await _repository.GetPagedAsync(
                    predicate,
                    page,
                    pageSize,
                    "NGAY_DL",
                    false
                );

                // Chuyển đổi kết quả sang DTO
                var response = new PagedApiResponse<GL02PreviewDto>
                {
                    TotalCount = totalCount,
                    PageSize = pageSize,
                    Page = page,
                    Data = items.Select(MapToPreviewDto).ToList()
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm kiếm dữ liệu GL02");
                throw;
            }
        }

        #region Mapping Methods

        private static GL02PreviewDto MapToPreviewDto(GL02 entity)
        {
            return new GL02PreviewDto
            {
                Id = entity.Id,
                NgayDL = entity.NGAY_DL,
                MaChiNhanh = entity.TRBRCD,
                MaNguoiDung = entity.USERID,
                MaTaiKhoan = entity.LOCAC,
                LoaiTien = entity.CCY,
                MaDonVi = entity.UNIT,
                MaKhachHang = entity.CUSTOMER,
                SoTienNo = entity.DRAMOUNT,
                SoTienCo = entity.CRAMOUNT,
                ThoiGianTao = entity.CRTDTM
            };
        }

        private static GL02DetailDto MapToDetailDto(GL02 entity)
        {
            return new GL02DetailDto
            {
                Id = entity.Id,
                NgayDL = entity.NGAY_DL,
                MaChiNhanh = entity.TRBRCD,
                MaNguoiDung = entity.USERID,
                JournalSeq = entity.JOURSEQ,
                DayTransactionSeq = entity.DYTRSEQ,
                MaTaiKhoan = entity.LOCAC,
                LoaiTien = entity.CCY,
                MaNghiepVu = entity.BUSCD,
                MaDonVi = entity.UNIT,
                MaGiaoDich = entity.TRCD,
                MaKhachHang = entity.CUSTOMER,
                LoaiGiaoDich = entity.TRTP,
                ThamChieu = entity.REFERENCE,
                GhiChu = entity.REMARK,
                SoTienNo = entity.DRAMOUNT,
                SoTienCo = entity.CRAMOUNT,
                ThoiGianTao = entity.CRTDTM,
                TenFileNguon = entity.FILE_NAME,
                NgayTao = entity.CREATED_DATE,
                NgayCapNhat = entity.UPDATED_DATE
            };
        }

        #endregion

        #region Private methods and properties

        // Not needed - we're already using the ApplicationDbContext from the constructor

        #endregion
    }
}
