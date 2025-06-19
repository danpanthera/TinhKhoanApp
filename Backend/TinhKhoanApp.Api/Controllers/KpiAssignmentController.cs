using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KpiAssignmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KpiAssignmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/KpiAssignment/tables - Lấy danh sách tất cả bảng giao khoán
        [HttpGet("tables")]
        public async Task<ActionResult<IEnumerable<object>>> GetKpiTables()
        {
            var tables = await _context.KpiAssignmentTables
                .Include(t => t.Indicators)
                .Select(t => new 
                {
                    t.Id,
                    t.TableType,
                    t.TableName,
                    t.Description,
                    t.Category,
                    t.IsActive,
                    t.CreatedDate,
                    IndicatorCount = t.Indicators.Count(i => i.IsActive)
                })
                .Where(t => t.IsActive)
                .ToListAsync();

            // Sắp xếp với logic đặc biệt cho chi nhánh (theo mã 7800-7808)
            var sortedTables = tables.OrderBy(t => {
                if (t.Category == "Dành cho Chi nhánh")
                {
                    // Ưu tiên Hội sở lên đầu
                    if (t.TableName?.Contains("Hội sở") == true)
                        return 0;
                    
                    // Tìm mã chi nhánh trong tên (7800-7808)
                    var match = System.Text.RegularExpressions.Regex.Match(t.TableName ?? "", @"\((\d{4})\)");
                    if (match.Success && int.TryParse(match.Groups[1].Value, out int branchCode))
                    {
                        return branchCode;
                    }
                    
                    // Nếu không có mã, sắp xếp theo tên
                    return 8000 + (t.TableName?.GetHashCode() ?? 0) % 100;
                }
                
                // Không phải chi nhánh, sắp xếp theo TableType như cũ
                return 10000 + (int)t.TableType;
            }).ToList();

            return Ok(sortedTables);
        }

        // GET: api/KpiAssignment/tables/grouped - Lấy danh sách bảng giao khoán theo nhóm (tab)
        [HttpGet("tables/grouped")]
        public async Task<ActionResult<object>> GetKpiTablesGrouped()
        {
            var tables = await _context.KpiAssignmentTables
                .Include(t => t.Indicators)
                .Select(t => new 
                {
                    t.Id,
                    t.TableType,
                    t.TableName,
                    t.Description,
                    t.Category,
                    t.IsActive,
                    t.CreatedDate,
                    IndicatorCount = t.Indicators.Count(i => i.IsActive)
                })
                .Where(t => t.IsActive)
                .OrderBy(t => t.TableType)
                .ToListAsync();

            var grouped = tables.GroupBy(t => t.Category)
                .ToDictionary(g => g.Key, g => g.ToList());

            return Ok(grouped);
        }

        // GET: api/KpiAssignment/table/{tableType} - Lấy chi tiết bảng và các chỉ tiêu
        [HttpGet("table/{tableType}")]
        public async Task<ActionResult<object>> GetKpiTableDetails(KpiTableType tableType)
        {
            var table = await _context.KpiAssignmentTables
                .Include(t => t.Indicators)
                .FirstOrDefaultAsync(t => t.TableType == tableType && t.IsActive);

            if (table == null)
            {
                return NotFound($"Không tìm thấy bảng giao khoán cho loại: {tableType}");
            }

            var result = new
            {
                table.Id,
                table.TableType,
                table.TableName,
                table.Description,
                Indicators = table.Indicators
                    .Where(i => i.IsActive)
                    .OrderBy(i => i.OrderIndex)
                    .Select(i => new
                    {
                        i.Id,
                        i.IndicatorName,
                        i.MaxScore,
                        i.Unit,
                        i.OrderIndex
                    })
                    .ToList()
            };

            return Ok(result);
        }

        // GET: api/KpiAssignment/tables/{id} - Lấy chi tiết bảng theo ID và các chỉ tiêu
        [HttpGet("tables/{id}")]
        public async Task<ActionResult<object>> GetKpiTableDetailsById(int id)
        {
            // Query table riêng để tránh navigation property confusion
            var table = await _context.KpiAssignmentTables
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);

            if (table == null)
            {
                return NotFound($"Không tìm thấy bảng giao khoán với ID: {id}");
            }

            // Query indicators riêng
            var indicators = await _context.KpiIndicators
                .Where(i => i.TableId == id && i.IsActive)
                .OrderBy(i => i.OrderIndex)
                .Select(i => new
                {
                    i.Id,
                    i.IndicatorName,
                    i.MaxScore,
                    i.Unit,
                    i.OrderIndex,
                    i.ValueType,
                    i.IsActive
                })
                .ToListAsync();

            var result = new
            {
                table.Id,
                table.TableType,
                table.TableName,
                table.Description,
                Indicators = indicators
            };

            return Ok(result);
        }

        // POST: api/KpiAssignment/assign - Giao khoán KPI cho nhân viên
        [HttpPost("assign")]
        public async Task<IActionResult> AssignKpiToEmployee([FromBody] KpiAssignmentRequest request)
        {
            if (request.EmployeeId <= 0 || request.KhoanPeriodId <= 0 || request.Targets == null || !request.Targets.Any())
            {
                return BadRequest("Dữ liệu giao khoán không hợp lệ");
            }

            // Kiểm tra nhân viên tồn tại
            var employee = await _context.Employees.FindAsync(request.EmployeeId);
            if (employee == null)
            {
                return NotFound("Không tìm thấy nhân viên");
            }

            // Kiểm tra kỳ khoán tồn tại
            var period = await _context.KhoanPeriods.FindAsync(request.KhoanPeriodId);
            if (period == null)
            {
                return NotFound("Không tìm thấy kỳ khoán");
            }

            // Xóa các target cũ (nếu có)
            var existingTargets = await _context.EmployeeKpiTargets
                .Where(t => t.EmployeeId == request.EmployeeId && t.KhoanPeriodId == request.KhoanPeriodId)
                .ToListAsync();

            if (existingTargets.Any())
            {
                _context.EmployeeKpiTargets.RemoveRange(existingTargets);
            }

            // Tạo các target mới
            var newTargets = new List<EmployeeKpiTarget>();
            foreach (var target in request.Targets)
            {
                // Kiểm tra indicator tồn tại
                var indicator = await _context.KpiIndicators.FindAsync(target.IndicatorId);
                if (indicator == null)
                {
                    return BadRequest($"Không tìm thấy chỉ tiêu KPI với ID: {target.IndicatorId}");
                }

                newTargets.Add(new EmployeeKpiTarget
                {
                    EmployeeId = request.EmployeeId,
                    IndicatorId = target.IndicatorId,
                    KhoanPeriodId = request.KhoanPeriodId,
                    TargetValue = target.TargetValue,
                    Notes = target.Notes,
                    AssignedDate = DateTime.UtcNow
                });
            }

            _context.EmployeeKpiTargets.AddRange(newTargets);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Giao khoán KPI thành công", TargetsCount = newTargets.Count });
        }

        // GET: api/KpiAssignment/employee/{employeeId}/period/{periodId} - Lấy giao khoán của nhân viên
        [HttpGet("employee/{employeeId}/period/{periodId}")]
        public async Task<ActionResult<object>> GetEmployeeKpiAssignment(int employeeId, int periodId)
        {
            var employee = await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Unit)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
            {
                return NotFound("Không tìm thấy nhân viên");
            }

            var targets = await _context.EmployeeKpiTargets
                .Include(t => t.Indicator)
                .ThenInclude(i => i.Table)
                .Where(t => t.EmployeeId == employeeId && t.KhoanPeriodId == periodId)
                .OrderBy(t => t.Indicator.OrderIndex)
                .ToListAsync();

            var result = new
            {
                Employee = new
                {
                    employee.Id,
                    employee.EmployeeCode,
                    employee.FullName,
                    Position = employee.Position?.Name,
                    Unit = employee.Unit?.Name
                },
                KpiTargets = targets.Select(t => new
                {
                    t.Id,
                    IndicatorName = t.Indicator.IndicatorName,
                    MaxScore = t.Indicator.MaxScore,
                    Unit = t.Indicator.Unit,
                    t.TargetValue,
                    t.ActualValue,
                    t.Score,
                    t.Notes,
                    TableName = t.Indicator.Table.TableName
                }).ToList()
            };

            return Ok(result);
        }

        // PUT: api/KpiAssignment/update-single-actual - Cập nhật giá trị thực hiện cho một target
        [HttpPut("update-single-actual")]
        public async Task<ActionResult> UpdateSingleActualValue([FromBody] UpdateSingleActualValueRequest request)
        {
            var target = await _context.EmployeeKpiTargets
                .Include(t => t.Indicator)
                .FirstOrDefaultAsync(t => t.Id == request.AssignmentId);

            if (target == null)
            {
                return NotFound("Không tìm thấy giao khoán KPI");
            }

            // Update actual value
            target.ActualValue = request.ActualValue;
            target.UpdatedDate = DateTime.UtcNow;

            // Calculate score if actual value is provided
            if (request.ActualValue.HasValue && target.TargetValue.HasValue && target.TargetValue.Value != 0)
            {
                // Simple scoring: (actual / target) * maxScore, capped at maxScore
                var ratio = request.ActualValue.Value / target.TargetValue.Value;
                target.Score = Math.Min(ratio * target.Indicator.MaxScore, target.Indicator.MaxScore);
            }
            else
            {
                target.Score = null;
            }

            await _context.SaveChangesAsync();

            return Ok(new 
            { 
                Message = "Cập nhật thành công",
                Score = target.Score,
                ActualValue = target.ActualValue
            });
        }

        // GET: api/KpiAssignment/search - Tìm kiếm giao khoán theo tiêu chí
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<object>>> SearchAssignments(
            [FromQuery] int? employeeId = null,
            [FromQuery] int? periodId = null,
            [FromQuery] int? tableId = null,
            [FromQuery] int? unitId = null)
        {
            var query = _context.EmployeeKpiTargets
                .Include(t => t.Employee)
                    .ThenInclude(e => e.Unit)
                .Include(t => t.Employee)
                    .ThenInclude(e => e.Position)
                .Include(t => t.Indicator)
                    .ThenInclude(i => i.Table)
                .Include(t => t.KhoanPeriod)
                .AsQueryable();

            // Apply filters
            if (employeeId.HasValue)
            {
                query = query.Where(t => t.EmployeeId == employeeId.Value);
            }

            if (periodId.HasValue)
            {
                query = query.Where(t => t.KhoanPeriodId == periodId.Value);
            }

            if (tableId.HasValue)
            {
                query = query.Where(t => t.Indicator.TableId == tableId.Value);
            }

            if (unitId.HasValue)
            {
                query = query.Where(t => t.Employee.UnitId == unitId.Value);
            }

            var assignments = await query
                .Select(t => new
                {
                    t.Id,
                    Employee = new
                    {
                        t.Employee.Id,
                        t.Employee.FullName,
                        t.Employee.Username,
                        Unit = t.Employee.Unit != null ? new
                        {
                            t.Employee.Unit.Id,
                            t.Employee.Unit.Name
                        } : null,
                        Position = t.Employee.Position != null ? new
                        {
                            t.Employee.Position.Id,
                            t.Employee.Position.Name
                        } : null
                    },
                    KhoanPeriod = new
                    {
                        t.KhoanPeriod.Id,
                        PeriodName = t.KhoanPeriod.Name,
                        t.KhoanPeriod.StartDate,
                        t.KhoanPeriod.EndDate
                    },
                    Indicator = new
                    {
                        t.Indicator.Id,
                        t.Indicator.IndicatorName,
                        t.Indicator.MaxScore,
                        t.Indicator.Unit,
                        t.Indicator.OrderIndex,
                        Table = new
                        {
                            t.Indicator.Table.Id,
                            t.Indicator.Table.TableName,
                            t.Indicator.Table.TableType
                        }
                    },
                    t.TargetValue,
                    t.ActualValue,
                    t.Score,
                    t.AssignedDate,
                    t.UpdatedDate,
                    t.Notes
                })
                .OrderBy(t => t.Employee.FullName)
                .ThenBy(t => t.Indicator.OrderIndex)
                .ToListAsync();

            return Ok(assignments);
        }

        // GET: api/KpiAssignment/employees-by-table/{tableType}/period/{periodId}
        [HttpGet("employees-by-table/{tableType}/period/{periodId}")]
        public ActionResult<object> GetEmployeesByTableType(KpiTableType tableType, int periodId)
        {
            // Logic xác định nhân viên thuộc loại bảng này sẽ được implement sau
            // Hiện tại trả về danh sách rỗng
            var result = new
            {
                TableType = tableType,
                TableName = tableType.ToString(),
                Employees = new List<object>(),
                Message = "Chức năng xác định nhân viên theo bảng giao khoán sẽ được phát triển tiếp"
            };

            return Ok(result);
        }

        // API endpoint để update mô tả từ "Kiểm soát" thành "Kế hoạch"
        [HttpPost("update-description-kiem-soat-to-ke-hoach")]
        public async Task<IActionResult> UpdateDescriptionKiemSoatToKeHoach()
        {
            try
            {
                // Tìm các bảng có chứa "Kiểm soát và Quản lý rủi ro"
                var tablesToUpdate = await _context.KpiAssignmentTables
                    .Where(t => t.Description.Contains("Kiểm soát và Quản lý rủi ro") || 
                               t.TableName.Contains("Kiểm soát và Quản lý rủi ro"))
                    .ToListAsync();

                if (!tablesToUpdate.Any())
                {
                    return Ok(new { 
                        message = "Không tìm thấy bản ghi nào có chứa 'Kiểm soát và Quản lý rủi ro'",
                        updatedCount = 0 
                    });
                }

                // Cập nhật các bản ghi
                foreach (var table in tablesToUpdate)
                {
                    if (table.Description != null)
                    {
                        table.Description = table.Description.Replace("Kiểm soát và Quản lý rủi ro", "Kế hoạch và Quản lý rủi ro");
                    }
                    
                    if (table.TableName != null)
                    {
                        table.TableName = table.TableName.Replace("Kiểm soát và Quản lý rủi ro", "Kế hoạch và Quản lý rủi ro");
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = $"Đã cập nhật thành công {tablesToUpdate.Count} bản ghi",
                    updatedCount = tablesToUpdate.Count,
                    updatedTables = tablesToUpdate.Select(t => new { 
                        t.Id, 
                        t.TableName, 
                        t.Description 
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Lỗi khi cập nhật mô tả", 
                    error = ex.Message 
                });
            }
        }

        // API endpoint để update "Kinh tế Nội vụ" thành "Kế toán & Ngân quỹ"
        [HttpPost("update-description-kinh-te-noi-vu-to-ke-toan-ngan-quy")]
        public async Task<IActionResult> UpdateDescriptionKinhTeNoiVuToKeToanNganQuy()
        {
            try
            {
                var tablesToUpdate = await _context.KpiAssignmentTables
                    .Where(t => t.Description.Contains("Kinh tế Nội vụ") || 
                               t.TableName.Contains("Kinh tế Nội vụ"))
                    .ToListAsync();

                if (!tablesToUpdate.Any())
                {
                    return Ok(new { 
                        message = "Không tìm thấy bản ghi nào có chứa 'Kinh tế Nội vụ'",
                        updatedCount = 0 
                    });
                }

                foreach (var table in tablesToUpdate)
                {
                    if (table.Description != null)
                    {
                        table.Description = table.Description.Replace("Kinh tế Nội vụ", "Kế toán & Ngân quỹ");
                    }
                    
                    if (table.TableName != null)
                    {
                        table.TableName = table.TableName.Replace("Kinh tế Nội vụ", "Kế toán & Ngân quỹ");
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = $"Đã cập nhật thành công {tablesToUpdate.Count} bản ghi từ 'Kinh tế Nội vụ' thành 'Kế toán & Ngân quỹ'",
                    updatedCount = tablesToUpdate.Count,
                    updatedTables = tablesToUpdate.Select(t => new { 
                        t.Id, 
                        t.TableName, 
                        t.Description 
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Lỗi khi cập nhật mô tả Kinh tế Nội vụ", 
                    error = ex.Message 
                });
            }
        }

        // API endpoint để update "Thủ quỹ/Hạch kiểm Kinh tế Nội bộ" thành "Thủ quỹ/Hậu kiểm/Kế toán nội bộ"
        [HttpPost("update-description-thu-quy-hach-kiem")]
        public async Task<IActionResult> UpdateDescriptionThuQuyHachKiem()
        {
            try
            {
                var tablesToUpdate = await _context.KpiAssignmentTables
                    .Where(t => t.Description.Contains("Thủ quỹ/Hạch kiểm Kinh tế Nội bộ") || 
                               t.TableName.Contains("TQ/HK KTNB"))
                    .ToListAsync();

                if (!tablesToUpdate.Any())
                {
                    return Ok(new { 
                        message = "Không tìm thấy bản ghi nào có chứa 'Thủ quỹ/Hạch kiểm Kinh tế Nội bộ'",
                        updatedCount = 0 
                    });
                }

                foreach (var table in tablesToUpdate)
                {
                    if (table.Description != null)
                    {
                        table.Description = table.Description.Replace("Thủ quỹ/Hạch kiểm Kinh tế Nội bộ", "Thủ quỹ/Hậu kiểm/Kế toán nội bộ");
                    }
                    
                    if (table.TableName != null)
                    {
                        table.TableName = table.TableName.Replace("TQ/HK KTNB", "TQ/HK/KTNB");
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = $"Đã cập nhật thành công {tablesToUpdate.Count} bản ghi",
                    updatedCount = tablesToUpdate.Count,
                    updatedTables = tablesToUpdate.Select(t => new { 
                        t.Id, 
                        t.TableName, 
                        t.Description 
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Lỗi khi cập nhật mô tả Thủ quỹ/Hạch kiểm", 
                    error = ex.Message 
                });
            }
        }

        // API endpoint để update "Công nghệ thông tin/Tổng hợp/Kinh tế Giám sát"
        [HttpPost("update-description-cong-nghe-thong-tin")]
        public async Task<IActionResult> UpdateDescriptionCongNgheThongTin()
        {
            try
            {
                var tablesToUpdate = await _context.KpiAssignmentTables
                    .Where(t => t.Description.Contains("Công nghệ thông tin/Tổng hợp/Kinh tế Giám sát") || 
                               t.TableName.Contains("Trưởng phòng IT/TH/KTGS"))
                    .ToListAsync();

                if (!tablesToUpdate.Any())
                {
                    return Ok(new { 
                        message = "Không tìm thấy bản ghi nào có chứa 'Công nghệ thông tin/Tổng hợp/Kinh tế Giám sát'",
                        updatedCount = 0 
                    });
                }

                foreach (var table in tablesToUpdate)
                {
                    if (table.Description != null)
                    {
                        table.Description = table.Description.Replace("Trưởng phòng Công nghệ thông tin/Tổng hợp/Kinh tế Giám sát", "Trưởng/Phó các phòng: IT, Tổng hợp, Kiểm tra Giám sát");
                    }
                    
                    if (table.TableName != null)
                    {
                        table.TableName = table.TableName.Replace("Trưởng phòng IT/TH/KTGS", "Trưởng/Phó các phòng IT/Tổng hợp/Kiểm tra Giám sát");
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = $"Đã cập nhật thành công {tablesToUpdate.Count} bản ghi",
                    updatedCount = tablesToUpdate.Count,
                    updatedTables = tablesToUpdate.Select(t => new { 
                        t.Id, 
                        t.TableName, 
                        t.Description 
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Lỗi khi cập nhật mô tả Công nghệ thông tin", 
                    error = ex.Message 
                });
            }
        }

        // API endpoint để update "Phó giám đốc PGD/CBTD"
        [HttpPost("update-description-pho-giam-doc-pgd")]
        public async Task<IActionResult> UpdateDescriptionPhoGiamDocPGD()
        {
            try
            {
                var tablesToUpdate = await _context.KpiAssignmentTables
                    .Where(t => t.Description.Contains("Phó giám đốc PGD/Cán bộ Tín dụng") || 
                               t.TableName.Contains("Phó giám đốc PGD/CBTD"))
                    .ToListAsync();

                if (!tablesToUpdate.Any())
                {
                    return Ok(new { 
                        message = "Không tìm thấy bản ghi nào có chứa 'Phó giám đốc PGD'",
                        updatedCount = 0 
                    });
                }

                foreach (var table in tablesToUpdate)
                {
                    if (table.Description != null)
                    {
                        table.Description = table.Description.Replace("Phó giám đốc PGD/Cán bộ Tín dụng", "Phó giám đốc PGD kiêm Cán bộ Tín dụng");
                    }
                    
                    if (table.TableName != null)
                    {
                        table.TableName = table.TableName.Replace("Phó giám đốc PGD/CBTD", "Phó Giám đốc PGD kiêm CBTD");
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = $"Đã cập nhật thành công {tablesToUpdate.Count} bản ghi",
                    updatedCount = tablesToUpdate.Count,
                    updatedTables = tablesToUpdate.Select(t => new { 
                        t.Id, 
                        t.TableName, 
                        t.Description 
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Lỗi khi cập nhật mô tả Phó giám đốc PGD", 
                    error = ex.Message 
                });
            }
        }

        // API endpoint để update "Phó giám đốc Chi nhánh loại 2 Kinh tế"
        [HttpPost("update-description-pho-giam-doc-cn-loai-2")]
        public async Task<IActionResult> UpdateDescriptionPhoGiamDocCNLoai2()
        {
            try
            {
                var tablesToUpdate = await _context.KpiAssignmentTables
                    .Where(t => t.Description.Contains("Phó giám đốc Chi nhánh loại 2 Kinh tế"))
                    .ToListAsync();

                if (!tablesToUpdate.Any())
                {
                    return Ok(new { 
                        message = "Không tìm thấy bản ghi nào có chứa 'Phó giám đốc Chi nhánh loại 2 Kinh tế'",
                        updatedCount = 0 
                    });
                }

                foreach (var table in tablesToUpdate)
                {
                    if (table.Description != null)
                    {
                        table.Description = table.Description.Replace("Phó giám đốc Chi nhánh loại 2 Kinh tế", "Phó giám đốc Chi nhánh loại 2 Kế toán");
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = $"Đã cập nhật thành công {tablesToUpdate.Count} bản ghi",
                    updatedCount = tablesToUpdate.Count,
                    updatedTables = tablesToUpdate.Select(t => new { 
                        t.Id, 
                        t.TableName, 
                        t.Description 
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Lỗi khi cập nhật mô tả Phó giám đốc Chi nhánh loại 2", 
                    error = ex.Message 
                });
            }
        }

        // POST: api/KpiAssignment/indicators - Create new KPI indicator
        [HttpPost("indicators")]
        public async Task<IActionResult> CreateKpiIndicator([FromBody] CreateKpiIndicatorRequest request)
        {
            try
            {
                // Validate table exists
                var table = await _context.KpiAssignmentTables.FindAsync(request.TableId);
                if (table == null)
                {
                    return NotFound("Không tìm thấy bảng giao khoán KPI");
                }

                // Get next order index
                var maxOrderIndex = await _context.KpiIndicators
                    .Where(i => i.TableId == request.TableId)
                    .MaxAsync(i => (int?)i.OrderIndex) ?? 0;

                var indicator = new KpiIndicator
                {
                    TableId = request.TableId,
                    IndicatorName = request.IndicatorName,
                    MaxScore = request.MaxScore,
                    Unit = request.Unit,
                    OrderIndex = maxOrderIndex + 1,
                    ValueType = request.ValueType,
                    IsActive = true
                };

                _context.KpiIndicators.Add(indicator);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Tạo chỉ tiêu KPI thành công",
                    indicator = new
                    {
                        indicator.Id,
                        indicator.IndicatorName,
                        indicator.MaxScore,
                        indicator.Unit,
                        indicator.OrderIndex,
                        indicator.ValueType,
                        indicator.IsActive
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống", error = ex.Message });
            }
        }

        // PUT: api/KpiAssignment/indicators/{id} - Update KPI indicator
        [HttpPut("indicators/{id}")]
        public async Task<IActionResult> UpdateKpiIndicator(int id, [FromBody] UpdateKpiIndicatorRequest request)
        {
            try
            {
                var indicator = await _context.KpiIndicators.FindAsync(id);
                if (indicator == null)
                {
                    return NotFound("Không tìm thấy chỉ tiêu KPI");
                }

                indicator.IndicatorName = request.IndicatorName;
                indicator.MaxScore = request.MaxScore;
                indicator.Unit = request.Unit;
                indicator.ValueType = request.ValueType;
                indicator.IsActive = request.IsActive;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật chỉ tiêu KPI thành công",
                    indicator = new
                    {
                        indicator.Id,
                        indicator.IndicatorName,
                        indicator.MaxScore,
                        indicator.Unit,
                        indicator.OrderIndex,
                        indicator.ValueType,
                        indicator.IsActive
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống", error = ex.Message });
            }
        }

        // DELETE: api/KpiAssignment/indicators/{id} - Delete KPI indicator
        [HttpDelete("indicators/{id}")]
        public async Task<IActionResult> DeleteKpiIndicator(int id)
        {
            try
            {
                var indicator = await _context.KpiIndicators.FindAsync(id);
                if (indicator == null)
                {
                    return NotFound("Không tìm thấy chỉ tiêu KPI");
                }

                // Check if indicator is being used in assignments
                var hasAssignments = await _context.EmployeeKpiTargets
                    .AnyAsync(t => t.IndicatorId == id);

                if (hasAssignments)
                {
                    return BadRequest("Không thể xóa chỉ tiêu KPI đã được giao khoán cho nhân viên");
                }

                _context.KpiIndicators.Remove(indicator);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Xóa chỉ tiêu KPI thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống", error = ex.Message });
            }
        }

        // PUT: api/KpiAssignment/indicators/{id}/reorder - Reorder KPI indicators
        [HttpPut("indicators/{id}/reorder")]
        public async Task<IActionResult> ReorderKpiIndicator(int id, [FromBody] ReorderIndicatorRequest request)
        {
            try
            {
                var indicator = await _context.KpiIndicators.FindAsync(id);
                if (indicator == null)
                {
                    return NotFound("Không tìm thấy chỉ tiêu KPI");
                }

                var oldOrderIndex = indicator.OrderIndex;
                var newOrderIndex = request.NewOrderIndex;

                if (oldOrderIndex == newOrderIndex)
                {
                    return Ok(new { success = true, message = "Thứ tự không thay đổi" });
                }

                // Get all indicators in the same table
                var indicators = await _context.KpiIndicators
                    .Where(i => i.TableId == indicator.TableId)
                    .OrderBy(i => i.OrderIndex)
                    .ToListAsync();

                // Reorder indicators
                if (newOrderIndex > oldOrderIndex)
                {
                    // Moving down: shift others up
                    foreach (var ind in indicators.Where(i => i.OrderIndex > oldOrderIndex && i.OrderIndex <= newOrderIndex))
                    {
                        ind.OrderIndex--;
                    }
                }
                else
                {
                    // Moving up: shift others down
                    foreach (var ind in indicators.Where(i => i.OrderIndex >= newOrderIndex && i.OrderIndex < oldOrderIndex))
                    {
                        ind.OrderIndex++;
                    }
                }

                indicator.OrderIndex = newOrderIndex;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Thay đổi thứ tự thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống", error = ex.Message });
            }
        }

    }

    // DTOs cho API requests
    public class KpiAssignmentRequest
    {
        public int EmployeeId { get; set; }
        public int KhoanPeriodId { get; set; }
        public List<KpiTargetRequest> Targets { get; set; } = new();
    }

    public class KpiTargetRequest
    {
        public int IndicatorId { get; set; }
        public decimal? TargetValue { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateSingleActualValueRequest
    {
        public int AssignmentId { get; set; }
        public decimal? ActualValue { get; set; }
    }

    public class UpdateActualValuesRequest
    {
        public List<ActualValueUpdate> Updates { get; set; } = new();
    }

    public class ActualValueUpdate
    {
        public int TargetId { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? Score { get; set; }
    }

    public class CreateKpiIndicatorRequest
    {
        public int TableId { get; set; }
        public required string IndicatorName { get; set; }
        public decimal MaxScore { get; set; }
        public required string Unit { get; set; }
        public string ValueTypeString { get; set; } = "NUMBER";
        
        public KpiValueType ValueType => Enum.TryParse<KpiValueType>(ValueTypeString, out var result) ? result : KpiValueType.NUMBER;
    }

    public class UpdateKpiIndicatorRequest
    {
        public required string IndicatorName { get; set; }
        public decimal MaxScore { get; set; }
        public required string Unit { get; set; }
        public string ValueTypeString { get; set; } = "NUMBER";
        public bool IsActive { get; set; } = true;
        
        public KpiValueType ValueType => Enum.TryParse<KpiValueType>(ValueTypeString, out var result) ? result : KpiValueType.NUMBER;
    }

    public class ReorderIndicatorRequest
    {
        public int NewOrderIndex { get; set; }
    }
}
