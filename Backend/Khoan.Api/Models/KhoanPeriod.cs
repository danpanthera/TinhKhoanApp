using System;
using System.Collections.Generic; // Sẽ cần cho ICollection sau này
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models
{
    // Định nghĩa Enum cho Loại Kỳ Khoán
    public enum PeriodType
    {
        MONTHLY,    // Khoán theo tháng
        QUARTERLY,  // Khoán theo quý
        ANNUAL      // Khoán theo năm
    }

    // Định nghĩa Enum cho Trạng Thái Kỳ Khoán
    public enum PeriodStatus
    {
        DRAFT,              // Kỳ nháp, chưa chính thức
        OPEN,               // Kỳ đang mở để nhập liệu/giao khoán
        PROCESSING,         // Kỳ đang trong quá trình tính toán, chấm điểm
        PENDINGAPPROVAL,   // Kỳ chờ duyệt kết quả
        CLOSED,             // Kỳ đã đóng và chốt số liệu
        ARCHIVED            // Kỳ đã lưu trữ (không còn sử dụng thường xuyên)
    }

    [Table("KhoanPeriods")] // Tên bảng trong database sẽ là "KhoanPeriods"
    public class KhoanPeriod
    {
        [Key] // Khóa chính
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty; // Tên của kỳ khoán, ví dụ: "Tháng 01/2025", "Quý I/2025", "Năm 2025"

        [Required]
        public PeriodType Type { get; set; } // Loại kỳ khoán (Tháng, Quý, Năm) - sử dụng Enum đã định nghĩa ở trên

        [Required]
        public DateTime StartDate { get; set; } // Ngày bắt đầu kỳ khoán

        [Required]
        public DateTime EndDate { get; set; } // Ngày kết thúc kỳ khoán

        [Required]
        public PeriodStatus Status { get; set; } = PeriodStatus.DRAFT; // Trạng thái của kỳ khoán - sử dụng Enum, mặc định là DRAFT

        // Navigation properties - sẽ được thêm sau khi các model liên quan (như UnitKhoanAssignment, EmployeeKhoanAssignment) được tạo
        // Ví dụ:
        // public virtual ICollection<UnitKhoanAssignment> UnitAssignments { get; set; }
        // public virtual ICollection<EmployeeKhoanAssignment> EmployeeAssignments { get; set; }

        public KhoanPeriod()
        {
            // Khởi tạo các collection nếu có, ví dụ:
            // UnitAssignments = new HashSet<UnitKhoanAssignment>();
            // EmployeeAssignments = new HashSet<EmployeeKhoanAssignment>();
        }
    }
}