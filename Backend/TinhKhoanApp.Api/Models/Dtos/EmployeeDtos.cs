using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.Dtos
{
    public class CreateEmployeeDto
    {
        [Required]
        [StringLength(20)]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required]
        [StringLength(9, MinimumLength = 9)]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Mã CB phải là dạng số và có đúng 9 chữ số")]
        public string CBCode { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        public int UnitId { get; set; }

        [Required]
        public int PositionId { get; set; }
    }

    public class UpdateEmployeeDto
    {
        [Required]
        [StringLength(255)]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public int UnitId { get; set; }

        [Required]
        public int PositionId { get; set; }

        [StringLength(100, MinimumLength = 6)]
        public string? Password { get; set; } // Optional for updates
    }

    public class EmployeeDetailDto : EmployeeListItemDto
    {
        // Inherits all properties from EmployeeListItemDto
        // Can add additional details here if needed
    }

    public class EmployeeStatsDto
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int InactiveEmployees { get; set; }
        public List<UnitEmployeeCountDto> UnitStats { get; set; } = new List<UnitEmployeeCountDto>();
    }

    public class UnitEmployeeCountDto
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public int ActiveCount { get; set; }
        public int InactiveCount => EmployeeCount - ActiveCount;
    }
}
