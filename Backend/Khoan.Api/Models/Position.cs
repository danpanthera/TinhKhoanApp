using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Khoan.Api.Models
{
    [Table("Positions")] // Tên bảng trong database sẽ là "Positions"
    public class Position
    {
        [Key] // Khóa chính
        public int Id { get; set; }

        [Required] // Bắt buộc phải có
        [StringLength(150)] // Giới hạn độ dài
        public string Name { get; set; } = string.Empty; // Tên chức vụ, ví dụ: "Giám đốc PGD", "Cán bộ Tín dụng", "Kế toán viên"

        [StringLength(500)]
        public string? Description { get; set; } // Mô tả thêm về chức vụ (có thể null)

        // Navigation property: Một Chức vụ có thể có nhiều Cán bộ (Employees)
        // Lỗi "Employee not found" ở đây là bình thường vì mình chưa tạo model Employee
        [JsonIgnore] // Tránh circular reference với Employee.Position
        public virtual ICollection<Employee> Employees { get; set; }

        public Position()
        {
            // Khởi tạo collection để tránh lỗi khi sử dụng
            // Lỗi "Employee not found" ở đây cũng là bình thường
            Employees = new HashSet<Employee>();
        }
    }
}
