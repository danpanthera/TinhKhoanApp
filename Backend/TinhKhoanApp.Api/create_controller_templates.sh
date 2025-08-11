#!/bin/bash

echo "🚀 ==============================================="
echo "   TẠO STANDARDIZED CONTROLLERS TEMPLATES       "
echo "==============================================="
echo ""

# Array of tables that need controllers
declare -a empty_controllers=("DPDA" "EI01" "GL01" "GL02" "GL41")

for table in "${empty_controllers[@]}"; do
    controller_file="Controllers/${table}Controller.cs"

    echo "📝 Creating ${table}Controller.cs..."

    # Create basic controller template
    cat > "$controller_file" << EOF
using Microsoft.AspNetCore.Mvc;
using TinhKhoanApp.Api.Services.Interfaces;
using TinhKhoanApp.Api.Models.DTOs.${table};
using TinhKhoanApp.Api.Models.Common;

namespace TinhKhoanApp.Api.Controllers
{
    /// <summary>
    /// ${table} Controller - API endpoints cho bảng ${table}
    /// Business columns từ CSV là single source of truth
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ${table}Controller : ControllerBase
    {
        private readonly I${table}Service _${table,}Service;
        private readonly ILogger<${table}Controller> _logger;

        public ${table}Controller(I${table}Service ${table,}Service, ILogger<${table}Controller> logger)
        {
            _${table,}Service = ${table,}Service;
            _logger = logger;
        }

        /// <summary>
        /// Lấy ${table} records với paging và search
        /// </summary>
        [HttpGet("preview")]
        public async Task<ActionResult<ApiResponse<PagedResult<${table}PreviewDto>>>> GetPreview(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                _logger.LogInformation("Getting ${table} preview - Page: {PageNumber}, Size: {PageSize}, Search: {SearchTerm}",
                    pageNumber, pageSize, searchTerm);

                var result = await _${table,}Service.GetPreviewAsync(pageNumber, pageSize, searchTerm);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ${table} preview");
                return StatusCode(500, ApiResponse<PagedResult<${table}PreviewDto>>.Error("Lỗi server khi lấy dữ liệu ${table}"));
            }
        }

        /// <summary>
        /// Import CSV file vào ${table} table
        /// </summary>
        [HttpPost("import")]
        public async Task<ActionResult<ApiResponse<${table}ImportResultDto>>> ImportCsv(IFormFile file, [FromQuery] string uploadedBy = "System")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(ApiResponse<${table}ImportResultDto>.Error("File CSV không hợp lệ"));
                }

                _logger.LogInformation("Starting ${table} CSV import - File: {FileName}, Size: {FileSize}, UploadedBy: {UploadedBy}",
                    file.FileName, file.Length, uploadedBy);

                var result = await _${table,}Service.ImportCsvAsync(file, uploadedBy);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing ${table} CSV - File: {FileName}", file?.FileName);
                return StatusCode(500, ApiResponse<${table}ImportResultDto>.Error("Lỗi server khi import dữ liệu CSV"));
            }
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new {
                Status = "Healthy",
                Service = "${table}Controller",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
EOF

    # Fix the case-sensitive variable substitution
    sed -i '' "s/\${table,}/${table,,}/g" "$controller_file"

    echo "✅ Created ${controller_file}"
done

echo ""
echo "🎯 COMPLETED: Created standardized controller templates"
echo "==============================================="
