using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Models.Dtos;

namespace TinhKhoanApp.Api.Services
{
    public class EmployeeKpiAssignmentService : IEmployeeKpiAssignmentService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeKpiAssignmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeKpiAssignmentDto>> GetAllAsync()
        {
            var assignments = await _context.EmployeeKpiAssignments
                .Include(e => e.Employee)
                .Include(e => e.KpiDefinition)
                .Include(e => e.KhoanPeriod)
                .Select(e => new EmployeeKpiAssignmentDto
                {
                    Id = e.Id,
                    EmployeeId = e.EmployeeId,
                    EmployeeName = e.Employee.FullName,
                    KpiDefinitionId = e.KpiDefinitionId,
                    KpiName = e.KpiDefinition.KpiName,
                    KhoanPeriodId = e.KhoanPeriodId,
                    KhoanPeriodName = e.KhoanPeriod.Name,
                    TargetValue = e.TargetValue,
                    ActualValue = e.ActualValue,
                    Score = e.Score,
                    CreatedDate = e.CreatedDate,
                    UpdatedDate = e.UpdatedDate,
                    Notes = e.Notes
                })
                .ToListAsync();

            return assignments;
        }

        public async Task<EmployeeKpiAssignmentDto?> GetByIdAsync(int id)
        {
            var assignment = await _context.EmployeeKpiAssignments
                .Include(e => e.Employee)
                .Include(e => e.KpiDefinition)
                .Include(e => e.KhoanPeriod)
                .Where(e => e.Id == id)
                .Select(e => new EmployeeKpiAssignmentDto
                {
                    Id = e.Id,
                    EmployeeId = e.EmployeeId,
                    EmployeeName = e.Employee.FullName,
                    KpiDefinitionId = e.KpiDefinitionId,
                    KpiName = e.KpiDefinition.KpiName,
                    KhoanPeriodId = e.KhoanPeriodId,
                    KhoanPeriodName = e.KhoanPeriod.Name,
                    TargetValue = e.TargetValue,
                    ActualValue = e.ActualValue,
                    Score = e.Score,
                    CreatedDate = e.CreatedDate,
                    UpdatedDate = e.UpdatedDate,
                    Notes = e.Notes
                })
                .FirstOrDefaultAsync();

            return assignment;
        }

        public async Task<IEnumerable<EmployeeKpiAssignmentDto>> GetByEmployeeIdAsync(int employeeId)
        {
            var assignments = await _context.EmployeeKpiAssignments
                .Include(e => e.Employee)
                .Include(e => e.KpiDefinition)
                .Include(e => e.KhoanPeriod)
                .Where(e => e.EmployeeId == employeeId)
                .Select(e => new EmployeeKpiAssignmentDto
                {
                    Id = e.Id,
                    EmployeeId = e.EmployeeId,
                    EmployeeName = e.Employee.FullName,
                    KpiDefinitionId = e.KpiDefinitionId,
                    KpiName = e.KpiDefinition.KpiName,
                    KhoanPeriodId = e.KhoanPeriodId,
                    KhoanPeriodName = e.KhoanPeriod.Name,
                    TargetValue = e.TargetValue,
                    ActualValue = e.ActualValue,
                    Score = e.Score,
                    CreatedDate = e.CreatedDate,
                    UpdatedDate = e.UpdatedDate,
                    Notes = e.Notes
                })
                .ToListAsync();

            return assignments;
        }

        public async Task<IEnumerable<EmployeeKpiAssignmentDto>> GetByKhoanPeriodIdAsync(int khoanPeriodId)
        {
            var assignments = await _context.EmployeeKpiAssignments
                .Include(e => e.Employee)
                .Include(e => e.KpiDefinition)
                .Include(e => e.KhoanPeriod)
                .Where(e => e.KhoanPeriodId == khoanPeriodId)
                .Select(e => new EmployeeKpiAssignmentDto
                {
                    Id = e.Id,
                    EmployeeId = e.EmployeeId,
                    EmployeeName = e.Employee.FullName,
                    KpiDefinitionId = e.KpiDefinitionId,
                    KpiName = e.KpiDefinition.KpiName,
                    KhoanPeriodId = e.KhoanPeriodId,
                    KhoanPeriodName = e.KhoanPeriod.Name,
                    TargetValue = e.TargetValue,
                    ActualValue = e.ActualValue,
                    Score = e.Score,
                    CreatedDate = e.CreatedDate,
                    UpdatedDate = e.UpdatedDate,
                    Notes = e.Notes
                })
                .ToListAsync();

            return assignments;
        }

        public async Task<EmployeeKpiAssignmentDto> CreateAsync(CreateEmployeeKpiAssignmentDto createDto)
        {
            var assignment = new EmployeeKpiAssignment
            {
                EmployeeId = createDto.EmployeeId,
                KpiDefinitionId = createDto.KpiDefinitionId,
                KhoanPeriodId = createDto.KhoanPeriodId,
                TargetValue = createDto.TargetValue,
                ActualValue = createDto.ActualValue,
                Notes = createDto.Notes,
                CreatedDate = DateTime.UtcNow
            };

            _context.EmployeeKpiAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(assignment.Id) ?? throw new InvalidOperationException("Failed to retrieve created assignment");
        }

        public async Task<EmployeeKpiAssignmentDto?> UpdateAsync(int id, UpdateEmployeeKpiAssignmentDto updateDto)
        {
            var assignment = await _context.EmployeeKpiAssignments.FindAsync(id);
            if (assignment == null)
                return null;

            assignment.TargetValue = updateDto.TargetValue;
            assignment.ActualValue = updateDto.ActualValue;
            assignment.Notes = updateDto.Notes;
            assignment.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(assignment.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var assignment = await _context.EmployeeKpiAssignments.FindAsync(id);
            if (assignment == null)
                return false;

            _context.EmployeeKpiAssignments.Remove(assignment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EmployeeKpiAssignmentDto>> CreateBulkAsync(IEnumerable<CreateEmployeeKpiAssignmentDto> createDtos)
        {
            var assignments = createDtos.Select(dto => new EmployeeKpiAssignment
            {
                EmployeeId = dto.EmployeeId,
                KpiDefinitionId = dto.KpiDefinitionId,
                KhoanPeriodId = dto.KhoanPeriodId,
                TargetValue = dto.TargetValue,
                ActualValue = dto.ActualValue,
                Notes = dto.Notes,
                CreatedDate = DateTime.UtcNow
            }).ToList();

            _context.EmployeeKpiAssignments.AddRange(assignments);
            await _context.SaveChangesAsync();

            var assignmentIds = assignments.Select(a => a.Id).ToList();
            return await _context.EmployeeKpiAssignments
                .Include(e => e.Employee)
                .Include(e => e.KpiDefinition)
                .Include(e => e.KhoanPeriod)
                .Where(e => assignmentIds.Contains(e.Id))
                .Select(e => new EmployeeKpiAssignmentDto
                {
                    Id = e.Id,
                    EmployeeId = e.EmployeeId,
                    EmployeeName = e.Employee.FullName,
                    KpiDefinitionId = e.KpiDefinitionId,
                    KpiName = e.KpiDefinition.KpiName,
                    KhoanPeriodId = e.KhoanPeriodId,
                    KhoanPeriodName = e.KhoanPeriod.Name,
                    TargetValue = e.TargetValue,
                    ActualValue = e.ActualValue,
                    Score = e.Score,
                    CreatedDate = e.CreatedDate,
                    UpdatedDate = e.UpdatedDate,
                    Notes = e.Notes
                })
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int employeeId, int kpiDefinitionId, int khoanPeriodId)
        {
            return await _context.EmployeeKpiAssignments
                .AnyAsync(e => e.EmployeeId == employeeId && 
                              e.KpiDefinitionId == kpiDefinitionId && 
                              e.KhoanPeriodId == khoanPeriodId);
        }

        // 🔥 Bulk Operations Implementation

        public async Task<BulkAssignmentResultDto> BulkAssignKpisAsync(BulkKpiAssignmentDto request)
        {
            var result = new BulkAssignmentResultDto
            {
                TotalCount = request.EmployeeIds.Count * request.KpiIds.Count
            };

            var assignmentsToCreate = new List<EmployeeKpiAssignment>();

            foreach (var employeeId in request.EmployeeIds)
            {
                foreach (var kpiId in request.KpiIds)
                {
                    // Check if assignment already exists
                    var exists = await ExistsAsync(employeeId, kpiId, request.KhoanPeriodId);
                    if (exists)
                    {
                        result.FailureCount++;
                        result.ErrorMessages.Add($"Assignment already exists for Employee ID {employeeId} and KPI ID {kpiId}");
                        continue;
                    }

                    var assignment = new EmployeeKpiAssignment
                    {
                        EmployeeId = employeeId,
                        KpiDefinitionId = kpiId,
                        KhoanPeriodId = request.KhoanPeriodId,
                        TargetValue = request.TargetValue,
                        Notes = request.Notes,
                        CreatedDate = DateTime.UtcNow
                    };

                    assignmentsToCreate.Add(assignment);
                }
            }

            if (assignmentsToCreate.Any())
            {
                try
                {
                    _context.EmployeeKpiAssignments.AddRange(assignmentsToCreate);
                    await _context.SaveChangesAsync();

                    result.SuccessCount = assignmentsToCreate.Count;
                    result.SuccessMessages.Add($"Successfully created {assignmentsToCreate.Count} KPI assignments");

                    // Load created assignments with navigation properties
                    var assignmentIds = assignmentsToCreate.Select(a => a.Id).ToList();
                    result.CreatedAssignments = await _context.EmployeeKpiAssignments
                        .Include(e => e.Employee)
                        .Include(e => e.KpiDefinition)
                        .Include(e => e.KhoanPeriod)
                        .Where(e => assignmentIds.Contains(e.Id))
                        .Select(e => new EmployeeKpiAssignmentDto
                        {
                            Id = e.Id,
                            EmployeeId = e.EmployeeId,
                            EmployeeName = e.Employee.FullName,
                            KpiDefinitionId = e.KpiDefinitionId,
                            KpiName = e.KpiDefinition.KpiName,
                            KhoanPeriodId = e.KhoanPeriodId,
                            KhoanPeriodName = e.KhoanPeriod.Name,
                            TargetValue = e.TargetValue,
                            ActualValue = e.ActualValue,
                            Score = e.Score,
                            CreatedDate = e.CreatedDate,
                            UpdatedDate = e.UpdatedDate,
                            Notes = e.Notes
                        })
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    result.FailureCount += assignmentsToCreate.Count;
                    result.ErrorMessages.Add($"Database error: {ex.Message}");
                }
            }

            return result;
        }

        public async Task<BulkUpdateResultDto> BulkUpdateScoresAsync(BulkScoreUpdateDto request)
        {
            var result = new BulkUpdateResultDto
            {
                TotalCount = request.ScoreUpdates.Count
            };

            var assignmentIds = request.ScoreUpdates.Select(u => u.AssignmentId).ToList();
            var assignments = await _context.EmployeeKpiAssignments
                .Where(a => assignmentIds.Contains(a.Id))
                .ToListAsync();

            foreach (var updateItem in request.ScoreUpdates)
            {
                var assignment = assignments.FirstOrDefault(a => a.Id == updateItem.AssignmentId);
                if (assignment == null)
                {
                    result.FailureCount++;
                    result.ErrorMessages.Add($"Assignment with ID {updateItem.AssignmentId} not found");
                    continue;
                }

                try
                {
                    assignment.ActualValue = updateItem.ActualValue;
                    assignment.Notes = updateItem.Notes ?? assignment.Notes;
                    assignment.UpdatedDate = DateTime.UtcNow;

                    result.SuccessCount++;
                    result.SuccessMessages.Add($"Updated assignment ID {updateItem.AssignmentId}");
                }
                catch (Exception ex)
                {
                    result.FailureCount++;
                    result.ErrorMessages.Add($"Error updating assignment ID {updateItem.AssignmentId}: {ex.Message}");
                }
            }

            if (result.SuccessCount > 0)
            {
                try
                {
                    await _context.SaveChangesAsync();

                    // Load updated assignments with navigation properties
                    var updatedAssignmentIds = request.ScoreUpdates
                        .Where(u => assignments.Any(a => a.Id == u.AssignmentId))
                        .Select(u => u.AssignmentId)
                        .ToList();

                    result.UpdatedAssignments = await _context.EmployeeKpiAssignments
                        .Include(e => e.Employee)
                        .Include(e => e.KpiDefinition)
                        .Include(e => e.KhoanPeriod)
                        .Where(e => updatedAssignmentIds.Contains(e.Id))
                        .Select(e => new EmployeeKpiAssignmentDto
                        {
                            Id = e.Id,
                            EmployeeId = e.EmployeeId,
                            EmployeeName = e.Employee.FullName,
                            KpiDefinitionId = e.KpiDefinitionId,
                            KpiName = e.KpiDefinition.KpiName,
                            KhoanPeriodId = e.KhoanPeriodId,
                            KhoanPeriodName = e.KhoanPeriod.Name,
                            TargetValue = e.TargetValue,
                            ActualValue = e.ActualValue,
                            Score = e.Score,
                            CreatedDate = e.CreatedDate,
                            UpdatedDate = e.UpdatedDate,
                            Notes = e.Notes
                        })
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    result.ErrorMessages.Add($"Database save error: {ex.Message}");
                }
            }

            return result;
        }

        public async Task<BulkDeleteResultDto> BulkDeleteAssignmentsAsync(BulkDeleteAssignmentDto request)
        {
            var result = new BulkDeleteResultDto
            {
                TotalCount = request.AssignmentIds.Count
            };

            var assignments = await _context.EmployeeKpiAssignments
                .Where(a => request.AssignmentIds.Contains(a.Id))
                .ToListAsync();

            foreach (var assignmentId in request.AssignmentIds)
            {
                var assignment = assignments.FirstOrDefault(a => a.Id == assignmentId);
                if (assignment == null)
                {
                    result.FailureCount++;
                    result.ErrorMessages.Add($"Assignment with ID {assignmentId} not found");
                    continue;
                }

                try
                {
                    _context.EmployeeKpiAssignments.Remove(assignment);
                    result.SuccessCount++;
                    result.DeletedAssignmentIds.Add(assignmentId);
                    result.SuccessMessages.Add($"Deleted assignment ID {assignmentId}");
                }
                catch (Exception ex)
                {
                    result.FailureCount++;
                    result.ErrorMessages.Add($"Error deleting assignment ID {assignmentId}: {ex.Message}");
                }
            }

            if (result.SuccessCount > 0)
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    result.ErrorMessages.Add($"Database save error: {ex.Message}");
                }
            }

            return result;
        }
    }
}
