using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DTOs.LN03;
using TinhKhoanApp.Api.Services.Interfaces;
using TinhKhoanApp.Api.Repositories;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LN03Controller : ControllerBase
    {
        private readonly ILN03Service _service;
        private readonly ILogger<LN03Controller> _logger;

        public LN03Controller(ILN03Service service, ILogger<LN03Controller> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("preview")]
        public async Task<IActionResult> Preview([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50, [FromQuery] DateTime? ngayDL = null)
        {
            var response = await _service.GetPreviewAsync(pageNumber, pageSize, ngayDL);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var response = await _service.GetByIdAsync(id);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LN03CreateDto dto)
        {
            var response = await _service.CreateAsync(dto);
            return StatusCode(response.StatusCode ?? 201, response);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] LN03UpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<object>.Error("ID không khớp", "ID_MISMATCH"));

            var response = await _service.UpdateAsync(id, dto);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await _service.DeleteAsync(id);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpGet("by-date/{date}")]
        public async Task<IActionResult> GetByDate(DateTime date, [FromQuery] int maxResults = 100)
        {
            var response = await _service.GetByDateAsync(date, maxResults);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpGet("by-branch/{branchCode}")]
        public async Task<IActionResult> GetByBranch(string branchCode, [FromQuery] int maxResults = 100)
        {
            var response = await _service.GetByBranchAsync(branchCode, maxResults);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpGet("by-customer/{customerCode}")]
        public async Task<IActionResult> GetByCustomer(string customerCode, [FromQuery] int maxResults = 100)
        {
            var response = await _service.GetByCustomerAsync(customerCode, maxResults);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetProcessingSummary([FromQuery] DateTime? ngayDL = null)
        {
            var response = await _service.GetProcessingSummaryAsync(ngayDL);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        // Development-only: minimal self-test for LN03 service
        [HttpGet("_selftest")]
        public async Task<IActionResult> SelfTest()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (!string.Equals(env, "Development", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(403, ApiResponse<object>.Error("Forbidden outside Development", 403));
            }

            // In-memory stub repo
            var repo = new InMemoryLN03Repo();
            var svc = new Services.LN03Service(repo, _logger as ILogger<Services.LN03Service> ??
                this.HttpContext.RequestServices.GetRequiredService<ILogger<Services.LN03Service>>());

            await repo.AddRangeAsync(new[]
            {
                new Models.DataTables.LN03 { Id = 1, NGAY_DL = new DateTime(2025,8,1), MACHINHANH = "7800", MAKH = "KH1", SOTIENXLRR = 100m, CREATED_DATE = DateTime.UtcNow.AddMinutes(-10) },
                new Models.DataTables.LN03 { Id = 2, NGAY_DL = new DateTime(2025,8,1), MACHINHANH = "7800", MAKH = "KH2", SOTIENXLRR = 200m, CREATED_DATE = DateTime.UtcNow.AddMinutes(-5) },
                new Models.DataTables.LN03 { Id = 3, NGAY_DL = new DateTime(2025,8,2), MACHINHANH = "7801", MAKH = "KH3", SOTIENXLRR = 300m, CREATED_DATE = DateTime.UtcNow },
            });

            var preview = await svc.GetPreviewAsync(1, 2, new DateTime(2025, 8, 1));
            var summary = await svc.GetProcessingSummaryAsync(new DateTime(2025, 8, 1));

            var ok = preview.Success && preview.Data!.Items.Count == 2 && summary.Success && summary.Data!.TotalContracts == 2 && summary.Data!.TotalProcessingAmount == 300m;

            return Ok(ApiResponse<object>.Ok(new
            {
                success = ok,
                previewCount = preview.Data!.Items.Count,
                totalContracts = summary.Data!.TotalContracts,
                totalAmount = summary.Data!.TotalProcessingAmount
            }));
        }
    }

    // Minimal in-memory repository for self-test
    internal class InMemoryLN03Repo : ILN03DataRepository
    {
        private readonly List<Models.DataTables.LN03> _data = new();
        public Task AddAsync(Models.DataTables.LN03 entity) { _data.Add(entity); return Task.CompletedTask; }
        public Task AddRangeAsync(IEnumerable<Models.DataTables.LN03> entities) { _data.AddRange(entities); return Task.CompletedTask; }
        public Task<int> CountAsync() => Task.FromResult(_data.Count);
        public Task<int> CountAsync(System.Linq.Expressions.Expression<Func<Models.DataTables.LN03, bool>> predicate) => Task.FromResult(_data.AsQueryable().Count(predicate));
        public Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<Models.DataTables.LN03, bool>> predicate) => Task.FromResult(_data.AsQueryable().Any(predicate));
        public Task<IEnumerable<Models.DataTables.LN03>> FindAsync(System.Linq.Expressions.Expression<Func<Models.DataTables.LN03, bool>> predicate) => Task.FromResult(_data.AsQueryable().Where(predicate).AsEnumerable());
        public Task<IEnumerable<Models.DataTables.LN03>> GetAllAsync() => Task.FromResult(_data.AsEnumerable());
        public Task<Models.DataTables.LN03?> GetByIdAsync(int id) => Task.FromResult(_data.FirstOrDefault(x => x.Id == id));
        public Task<IEnumerable<Models.DataTables.LN03>> GetRecentAsync(int count) => Task.FromResult(_data.OrderByDescending(x => x.NGAY_DL).ThenByDescending(x => x.CREATED_DATE).Take(count).AsEnumerable());
        public void Remove(Models.DataTables.LN03 entity) { _data.Remove(entity); }
        public void RemoveRange(IEnumerable<Models.DataTables.LN03> entities) { foreach (var e in entities) _data.Remove(e); }
        public Task<int> SaveChangesAsync() => Task.FromResult(1);
        public void Update(Models.DataTables.LN03 entity) { /* no-op */ }
        public Task<IEnumerable<Models.DataTables.LN03>> GetByBranchAsync(string branchCode, int maxResults = 100) => Task.FromResult(_data.Where(x => x.MACHINHANH == branchCode).OrderByDescending(x => x.NGAY_DL).ThenByDescending(x => x.CREATED_DATE).Take(maxResults).AsEnumerable());
        public Task<IEnumerable<Models.DataTables.LN03>> GetByCustomerAsync(string customerCode, int maxResults = 100) => Task.FromResult(_data.Where(x => x.MAKH == customerCode).OrderByDescending(x => x.NGAY_DL).ThenByDescending(x => x.CREATED_DATE).Take(maxResults).AsEnumerable());
        public Task<IEnumerable<Models.DataTables.LN03>> GetByDateAsync(DateTime date, int maxResults = 100) => Task.FromResult(_data.Where(x => x.NGAY_DL.Date == date.Date).OrderByDescending(x => x.NGAY_DL).ThenByDescending(x => x.CREATED_DATE).Take(maxResults).AsEnumerable());
    }
}
