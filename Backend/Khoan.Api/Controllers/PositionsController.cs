using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Khoan.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionsController : ControllerBase
    {
        private readonly string _connStr;

        public PositionsController(IConfiguration configuration)
        {
            _connStr = configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Missing DefaultConnection");
        }

        public record PositionDto(int Id, string Name, string? Description);

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = new List<PositionDto>();
            await using var conn = new SqlConnection(_connStr);
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Description FROM Positions ORDER BY Name";
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                items.Add(new PositionDto(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.IsDBNull(2) ? null : reader.GetString(2)
                ));
            }

            return Ok(items);
        }
    }
}
