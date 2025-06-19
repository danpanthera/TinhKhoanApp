using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models
{
    [Table("Roles")] // Tên bảng trong database
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        // Sau này mình sẽ cấu hình IsUnique cho Name trong DbContext bằng Fluent API
        public string Name { get; set; } = string.Empty; // Tên của vai trò, ví dụ: "Administrator", "Manager", "Staff"

        [StringLength(255)]
        public string? Description { get; set; } // Mô tả chi tiết về vai trò

        // Navigation property cho bảng trung gian EmployeeRole
        // Một Role có thể được gán cho nhiều Employee (thông qua EmployeeRole)
        public virtual ICollection<EmployeeRole> EmployeeRoles { get; set; }

        public Role()
        {
            EmployeeRoles = new HashSet<EmployeeRole>();
        }
    }
}