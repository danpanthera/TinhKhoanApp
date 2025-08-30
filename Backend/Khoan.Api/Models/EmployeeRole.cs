using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
// Không cần [Key] ở đây vì khóa chính là cặp (EmployeeId, RoleId)
// và sẽ được cấu hình bằng Fluent API trong DbContext.

namespace Khoan.Api.Models
{
    [Table("EmployeeRoles")] // Tên bảng trong database
    public class EmployeeRole
    {
        // Foreign Key đến bảng Employees
        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        [JsonIgnore] // Tránh circular reference
        public virtual Employee? Employee { get; set; }

        // Foreign Key đến bảng Roles
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        [JsonIgnore] // Tránh circular reference
        public virtual Role? Role { get; set; }

        // Sếp có thể thêm các thuộc tính khác cho mối quan hệ này nếu cần, ví dụ:
        // public DateTime DateAssigned { get; set; } // Ngày gán quyền

        /// <summary>
        /// Indicates if this role assignment is active
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
