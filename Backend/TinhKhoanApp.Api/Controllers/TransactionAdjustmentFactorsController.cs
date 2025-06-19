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
    public class TransactionAdjustmentFactorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TransactionAdjustmentFactorsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionAdjustmentFactor>>> GetAll()
        {
            return await _context.TransactionAdjustmentFactors.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionAdjustmentFactor>> Get(int id)
        {
            var factor = await _context.TransactionAdjustmentFactors.FirstOrDefaultAsync(x => x.Id == id);
            if (factor == null) return NotFound();
            return factor;
        }
        [HttpPost]
        public async Task<ActionResult<TransactionAdjustmentFactor>> Post(TransactionAdjustmentFactor factor)
        {
            _context.TransactionAdjustmentFactors.Add(factor);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = factor.Id }, factor);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, TransactionAdjustmentFactor factor)
        {
            if (id != factor.Id) return BadRequest();
            _context.Entry(factor).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.TransactionAdjustmentFactors.Any(x => x.Id == id)) return NotFound();
                else throw;
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var factor = await _context.TransactionAdjustmentFactors.FindAsync(id);
            if (factor == null) return NotFound();
            _context.TransactionAdjustmentFactors.Remove(factor);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
