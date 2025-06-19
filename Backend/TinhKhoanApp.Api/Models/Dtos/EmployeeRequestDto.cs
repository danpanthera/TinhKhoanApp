using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.Dtos
{
    public class EmployeeRequestDto
    {
        public int? Id { get; set; }
        
        [Required]
        public string EmployeeCode { get; set; } = string.Empty;
        
        [Required]
        public string CBCode { get; set; } = string.Empty;
        
        [Required]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        public string Username { get; set; } = string.Empty;
        
        public string? PasswordHash { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public string? PhoneNumber { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [Required]
        public int UnitId { get; set; }
        
        [Required]
        public int PositionId { get; set; }
        
        // Role assignments
        public List<int> RoleIds { get; set; } = new List<int>();
    }
}
