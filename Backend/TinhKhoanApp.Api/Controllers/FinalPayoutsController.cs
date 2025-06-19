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
    public class FinalPayoutsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public FinalPayoutsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FinalPayout>>> GetAll()
        {
            return await _context.FinalPayouts.Include(x => x.Employee).Include(x => x.KhoanPeriod).ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<FinalPayout>> Get(int id)
        {
            var payout = await _context.FinalPayouts.Include(x => x.Employee).Include(x => x.KhoanPeriod).FirstOrDefaultAsync(x => x.Id == id);
            if (payout == null) return NotFound();
            return payout;
        }
        [HttpPost]
        public async Task<ActionResult<FinalPayout>> Post(FinalPayout payout)
        {
            _context.FinalPayouts.Add(payout);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = payout.Id }, payout);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, FinalPayout payout)
        {
            if (id != payout.Id) return BadRequest();
            _context.Entry(payout).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.FinalPayouts.Any(x => x.Id == id)) return NotFound();
                else throw;
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var payout = await _context.FinalPayouts.FindAsync(id);
            if (payout == null) return NotFound();
            _context.FinalPayouts.Remove(payout);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
