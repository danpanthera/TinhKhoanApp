using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Reports/EmployeeList
        [HttpGet("EmployeeList")]
        public async Task<IActionResult> GetEmployeeListReport()
        {
            var employees = await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Unit)
                .ToListAsync();

            var reportData = employees.Select(e => new
            {
                e.EmployeeCode,
                e.FullName,
                Position = e.Position?.Name ?? "Chưa có chức vụ",
                Unit = e.Unit?.Name ?? "Chưa có đơn vị",
                e.Email,
                e.PhoneNumber
            }).ToList();

            return Ok(new { employees = reportData, total = employees.Count });
        }

        // GET: api/Reports/EmployeesByUnit/{unitId}
        [HttpGet("EmployeesByUnit/{unitId}")]
        public async Task<IActionResult> GetEmployeesByUnit(int unitId)
        {
            var unit = await _context.Units.FindAsync(unitId);
            if (unit == null) return NotFound("Đơn vị không tồn tại");

            var employees = await _context.Employees
                .Include(e => e.Position)
                .Where(e => e.UnitId == unitId)
                .ToListAsync();

            var reportData = employees.Select(e => new
            {
                e.EmployeeCode,
                e.FullName,
                Position = e.Position?.Name ?? "Chưa có chức vụ",
                e.Email,
                e.PhoneNumber
            }).ToList();

            return Ok(new 
            { 
                unitName = unit.Name,
                employees = reportData, 
                total = employees.Count 
            });
        }

        // GET: api/Reports/KhoanPeriods
        [HttpGet("KhoanPeriods")]
        public async Task<IActionResult> GetKhoanPeriodsReport()
        {
            var periods = await _context.KhoanPeriods
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();

            var reportData = periods.Select(p => new
            {
                p.Id,
                periodName = p.Name,
                p.StartDate,
                p.EndDate,
                isActive = p.Status == Models.PeriodStatus.OPEN,
                status = p.Status.ToString()
            }).ToList();

            return Ok(new { periods = reportData, total = periods.Count });
        }

        // Placeholder for future KPI reports that will be implemented with the new 22 table structure
        [HttpGet("KpiReports/Placeholder")]
        public IActionResult GetKpiReportsPlaceholder()
        {
            return Ok(new
            {
                message = "KPI reporting functionality will be implemented after the new 22 KPI assignment table structure is created",
                availableReports = new[]
                {
                    "Employee List Report",
                    "Employees by Unit Report", 
                    "Khoan Periods Report"
                },
                futureKpiReports = new[]
                {
                    "Employee KPI Performance by CbType",
                    "Unit KPI Summary",
                    "KPI Achievement Analysis",
                    "Period Comparison Reports"
                }
            });
        }
    }
}
