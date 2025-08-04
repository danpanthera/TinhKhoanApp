using System;
using System.Collections.Generic;

namespace TinhKhoanApp.Api.Models.Dtos
{
    public class EmployeeListItemDto
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string CBCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }

        public int UnitId { get; set; }
        public string? UnitName { get; set; }

        public int PositionId { get; set; }
        public string? PositionName { get; set; }

        // Role information
        public List<RoleDto> Roles { get; set; } = new List<RoleDto>();
    }

    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
