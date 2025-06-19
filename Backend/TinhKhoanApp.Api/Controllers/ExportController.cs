using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using ClosedXML.Excel;
using System.IO;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Export/employees?unitId={unitId}&format=excel
        [HttpGet("employees")]
        public async Task<IActionResult> ExportEmployees([FromQuery] int? unitId, [FromQuery] string format = "excel")
        {
            try
            {
                var query = _context.Employees
                    .Include(e => e.Unit)
                    .Include(e => e.Position)
                    .Include(e => e.EmployeeRoles)
                        .ThenInclude(er => er.Role)
                    .AsQueryable();

                if (unitId.HasValue)
                {
                    query = query.Where(e => e.UnitId == unitId.Value);
                }

                var employees = await query
                    .OrderBy(e => e.FullName)
                    .ToListAsync();

                if (format.ToLower() == "excel")
                {
                    return ExportEmployeesToExcel(employees);
                }

                return BadRequest("Định dạng không được hỗ trợ. Chỉ hỗ trợ: excel");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xuất dữ liệu: {ex.Message}");
            }
        }

        private IActionResult ExportEmployeesToExcel(List<Employee> employees)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Danh sách nhân viên");

            // Tạo header
            var headers = new[] { "STT", "Mã NV", "Họ tên", "Đơn vị", "Chức vụ", "Vai trò", "Username", "Email", "Số điện thoại" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
            }

            // Điền dữ liệu
            for (int i = 0; i < employees.Count; i++)
            {
                var employee = employees[i];
                var row = i + 2;

                worksheet.Cell(row, 1).Value = i + 1;
                worksheet.Cell(row, 2).Value = employee.EmployeeCode ?? "";
                worksheet.Cell(row, 3).Value = employee.FullName ?? "";
                worksheet.Cell(row, 4).Value = employee.Unit?.Name ?? "";
                worksheet.Cell(row, 5).Value = employee.Position?.Name ?? "";
                worksheet.Cell(row, 6).Value = string.Join(", ", employee.EmployeeRoles?.Select(er => er.Role?.Name) ?? new List<string>());
                worksheet.Cell(row, 7).Value = employee.Username ?? "";
                worksheet.Cell(row, 8).Value = employee.Email ?? "";
                worksheet.Cell(row, 9).Value = employee.PhoneNumber ?? "";
            }

            // Tự động điều chỉnh độ rộng cột
            worksheet.Columns().AdjustToContents();

            // Tạo file và trả về
            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"DanhSach_NhanVien_{DateTime.Now:ddMMyyyy_HHmm}.xlsx";
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
