using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Khoan.Api.Models
{
    [Table("Employees")]
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        // Removed EmployeeCode per CBCode-first migration

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
        public string PasswordHash { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        // 🆕 Thông tin bổ sung từ GAHR26 / yêu cầu import
        [StringLength(100)]
        public string? UserAD { get; set; }

        [StringLength(100)]
        public string? UserIPCAS { get; set; }

        [StringLength(50)]
        public string? MaCBTD { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public int UnitId { get; set; }
        [ForeignKey("UnitId")]
        [JsonIgnore] // Tránh circular reference với Unit.Employees
        public virtual Unit? Unit { get; set; }

        [Required]
        public int PositionId { get; set; }
        [ForeignKey("PositionId")]
        public virtual Position? Position { get; set; }

        // --- THAY ĐỔI Ở ĐÂY ---
        // 1. Đã XÓA thuộc tính: public string? SystemRole { get; set; }

        // 2. THÊM navigation property cho mối quan hệ nhiều-nhiều với Role thông qua EmployeeRole
        [JsonIgnore] // Tránh circular reference với EmployeeRole.Employee
        public virtual ICollection<EmployeeRole> EmployeeRoles { get; set; }
        // --- KẾT THÚC THAY ĐỔI ---


        // Các navigation property cho các bảng giao khoán, trừ điểm... sẽ được thêm vào sau
        // Ví dụ:
        // public virtual ICollection<EmployeeKhoanAssignment> KhoanAssignments { get; set; }
        // public virtual ICollection<EmployeeDeduction> Deductions { get; set; }

        public Employee()
        {
            // --- THAY ĐỔI Ở ĐÂY ---
            // 3. Khởi tạo EmployeeRoles
            EmployeeRoles = new HashSet<EmployeeRole>();
            // --- KẾT THÚC THAY ĐỔI ---

            // Khởi tạo các collection khác nếu có, ví dụ:
            // KhoanAssignments = new HashSet<EmployeeKhoanAssignment>();
            // Deductions = new HashSet<EmployeeDeduction>();
        }
    }
}
