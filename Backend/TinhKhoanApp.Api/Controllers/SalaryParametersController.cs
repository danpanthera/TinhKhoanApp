using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryParametersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SalaryParametersController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalaryParameter>>> GetAll()
        {
            return await _context.SalaryParameters.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<SalaryParameter>> Get(int id)
        {
            var param = await _context.SalaryParameters.FindAsync(id);
            if (param == null) return NotFound();
            return param;
        }
        [HttpPost]
        public async Task<ActionResult<SalaryParameter>> Post(SalaryParameter param)
        {
            _context.SalaryParameters.Add(param);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = param.Id }, param);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, SalaryParameter param)
        {
            if (id != param.Id) return BadRequest();
            _context.Entry(param).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.SalaryParameters.Any(x => x.Id == id)) return NotFound();
                else throw;
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var param = await _context.SalaryParameters.FindAsync(id);
            if (param == null) return NotFound();
            _context.SalaryParameters.Remove(param);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
