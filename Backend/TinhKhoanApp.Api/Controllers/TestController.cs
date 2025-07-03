using Microsoft.AspNetCore.Mvc;

namespace TinhKhoanApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "OK", timestamp = DateTime.Now });
        }

        [HttpGet("nguon-von-test")]
        public IActionResult NguonVonTest()
        {
            return Ok(new
            {
                message = "Backend đã sẵn sàng",
                timestamp = DateTime.Now,
                version = "Updated with ImportedDataItems"
            });
        }
    }
}
