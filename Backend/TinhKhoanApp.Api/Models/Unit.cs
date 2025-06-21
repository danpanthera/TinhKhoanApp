using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models
{
    [Table("Units")]
    public class Unit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Code")]
        public string Code { get; set; } = string.Empty; // Khởi tạo giá trị mặc định

        [Required]
        [StringLength(255)]
        [Column("Name")]
        public string Name { get; set; } = string.Empty; // Khởi tạo giá trị mặc định

        [StringLength(100)]
        [Column("Type")]
        public string? Type { get; set; } // Đã sửa: Thêm dấu ? để cho phép null

        public int? ParentUnitId { get; set; }

        // Add alias for ParentId property expected by controllers
        public int? ParentId => ParentUnitId;

        // Add IsDeleted property for soft delete pattern
        public bool IsDeleted { get; set; } = false;

        // [Column("SortOrder")]
        // public int? SortOrder { get; set; } // Thứ tự hiển thị cho chi nhánh

        [ForeignKey("ParentUnitId")]
        public virtual Unit? ParentUnit { get; set; } // Đã sửa: Thêm dấu ? để cho phép null

        public virtual ICollection<Unit> ChildUnits { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }

        public Unit()
        {
            ChildUnits = new HashSet<Unit>();
            Employees = new HashSet<Employee>();
            // Không cần khởi tạo Code và Name ở đây nữa vì đã khởi tạo trực tiếp khi khai báo
            // Type và ParentUnit có thể null nên không cần khởi tạo trong constructor
        }
    }
}