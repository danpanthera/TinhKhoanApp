using Microsoft.AspNetCore.Mvc;
using Khoan.Api.Models.Common;
using Khoan.Api.Models.DTOs.GL02;
using Khoan.Api.Services.Interfaces;

namespace Khoan.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GL02Controller : ControllerBase
    {
        private readonly IGL02Service _service;

        public GL02Controller(IGL02Service service)
        {
            _service = service;
        }

        [HttpGet("preview")]
        public async Task<IActionResult> Preview([FromQuery] int take = 20)
        {
            var res = await _service.PreviewAsync(take);
            return StatusCode(res.StatusCode ?? 200, res);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var res = await _service.GetByIdAsync(id);
            return StatusCode(res.StatusCode ?? 200, res);
        }

        [HttpGet("by-date/{date}")]
        public async Task<IActionResult> GetByDate(DateTime date, [FromQuery] int maxResults = 100)
        {
            var res = await _service.GetByDateAsync(date, maxResults);
            return StatusCode(res.StatusCode ?? 200, res);
        }

        [HttpGet("by-unit/{unit}")]
        public async Task<IActionResult> GetByUnit(string unit, [FromQuery] int maxResults = 100)
        {
            var res = await _service.GetByUnitAsync(unit, maxResults);
            return StatusCode(res.StatusCode ?? 200, res);
        }

        [HttpGet("by-trcd/{trcd}")]
        public async Task<IActionResult> GetByTransactionCode(string trcd, [FromQuery] int maxResults = 100)
        {
            var res = await _service.GetByTransactionCodeAsync(trcd, maxResults);
            return StatusCode(res.StatusCode ?? 200, res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GL02CreateDto dto)
        {
            var res = await _service.CreateAsync(dto);
            return StatusCode(res.StatusCode ?? 200, res);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] GL02UpdateDto dto)
        {
            var res = await _service.UpdateAsync(dto);
            return StatusCode(res.StatusCode ?? 200, res);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var res = await _service.DeleteAsync(id);
            return StatusCode(res.StatusCode ?? 200, res);
        }

        [HttpGet("summary/unit/{unit}")]
        public async Task<IActionResult> SummaryByUnit(string unit)
        {
            var res = await _service.SummaryByUnitAsync(unit);
            return StatusCode(res.StatusCode ?? 200, res);
        }
    }
}
