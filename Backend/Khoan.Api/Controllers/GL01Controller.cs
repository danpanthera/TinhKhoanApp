using Microsoft.AspNetCore.Mvc;
using Khoan.Api.Models.Dtos.GL01;
using Khoan.Api.Services.Interfaces;

namespace Khoan.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GL01Controller : ControllerBase
    {
        private readonly IGL01Service _service;

        public GL01Controller(IGL01Service service)
        {
            _service = service;
        }

        [HttpGet("preview")]
        public async Task<IActionResult> Preview([FromQuery] int count = 10)
            => Ok(await _service.GetPreviewAsync(count));

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
            => Ok(await _service.GetByIdAsync(id));

        [HttpGet("by-date")]
        public async Task<IActionResult> GetByDate([FromQuery] DateTime date, [FromQuery] int maxResults = 100)
            => Ok(await _service.GetByDateAsync(date, maxResults));

        [HttpGet("by-unit/{unitCode}")]
        public async Task<IActionResult> GetByUnit(string unitCode, [FromQuery] int maxResults = 100)
            => Ok(await _service.GetByUnitAsync(unitCode, maxResults));

        [HttpGet("by-account/{accountCode}")]
        public async Task<IActionResult> GetByAccount(string accountCode, [FromQuery] int maxResults = 100)
            => Ok(await _service.GetByAccountAsync(accountCode, maxResults));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GL01CreateDto dto)
            => Ok(await _service.CreateAsync(dto));

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] GL01UpdateDto dto)
            => Ok(await _service.UpdateAsync(id, dto));

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
            => Ok(await _service.DeleteAsync(id));

        [HttpGet("summary/by-unit/{unitCode}")]
        public async Task<IActionResult> SummaryByUnit(string unitCode, [FromQuery] DateTime? date = null)
            => Ok(await _service.GetSummaryByUnitAsync(unitCode, date));
    }
}
