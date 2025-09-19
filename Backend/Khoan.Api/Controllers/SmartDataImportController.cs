using Microsoft.AspNetCore.Mvc;

namespace Khoan.Api.Controllers
{
    /// <summary>
    /// SmartDataImport controller (temporary compatibility layer)
    /// Provides lightweight endpoints expected by the current frontend.
    /// TODO: Wire these to real services/persistence if needed.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SmartDataImportController : ControllerBase
    {
        private static readonly string[] Supported = new[]
        {
            "DP01", "LN01", "LN03", "GL01", "GL02", "GL41", "DPDA", "EI01", "RR01"
        };

        /// <summary>
        /// Returns supported categories for smart import.
        /// </summary>
        [HttpGet("supported-categories")]
        public IActionResult GetSupportedCategories()
        {
            return Ok(Supported);
        }

        /// <summary>
        /// Returns imported records (stub: empty list). Frontend expects an array or object.
        /// </summary>
        [HttpGet("imported-records")]
        public IActionResult GetImportedRecords()
        {
            return Ok(new { items = Array.Empty<object>(), total = 0 });
        }

        /// <summary>
        /// Returns detail of a specific imported record (stub: not found).
        /// </summary>
        [HttpGet("imported-records/{id:int}")]
        public IActionResult GetImportedRecordDetail(int id)
        {
            return NotFound(new { message = $"Imported record {id} not found (stub)", id });
        }

        /// <summary>
        /// Processes imported data to history tables (stub: acknowledges request only).
        /// </summary>
        [HttpPost("process-to-history")]
        public IActionResult ProcessToHistory([FromBody] ProcessToHistoryRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Category))
            {
                return BadRequest(new { ok = false, message = "Invalid request" });
            }

            return Ok(new
            {
                ok = true,
                processed = false,
                message = "Stub endpoint: processing to history not implemented yet",
                request
            });
        }

        public class ProcessToHistoryRequest
        {
            public int ImportedDataRecordId { get; set; }
            public string Category { get; set; } = string.Empty;
            public DateTime? StatementDate { get; set; }
        }
    }
}
