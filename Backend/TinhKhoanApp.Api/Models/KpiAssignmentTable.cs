using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Models
{
    // Enum định nghĩa 23 loại bảng giao khoán KPI
    public enum KpiTableType
    {
        TruongphongKhdn = 1,
        TruongphongKhcn = 2,
        PhophongKhdn = 3,
        PhophongKhcn = 4,
        TruongphongKhqlrr = 5,
        PhophongKhqlrr = 6,
        Cbtd = 7,
        TruongphongKtnqCnl1 = 8,
        PhophongKtnqCnl1 = 9,
        Gdv = 10,
        TqHkKtnb = 11,
        TruongphoItThKtgs = 12,
        CBItThKtgsKhqlrr = 13,
        GiamdocPgd = 14,
        PhogiamdocPgd = 15,
        PhogiamdocPgdCbtd = 16,
        GiamdocCnl2 = 17,
        PhogiamdocCnl2Td = 18,
        PhogiamdocCnl2Kt = 19,
        TruongphongKhCnl2 = 20,
        PhophongKhCnl2 = 21,
        TruongphongKtnqCnl2 = 22,
        PhophongKtnqCnl2 = 23,
        // Chi nhánh - sắp xếp theo mã 7800-7808
        HoiSo = 200,           // 7800
        CnTamDuong = 201,      // 7801
        CnPhongTho = 202,      // 7802
        CnSinHo = 203,         // 7803
        CnMuongTe = 204,       // 7804
        CnThanUyen = 205,      // 7805
        CnThanhPho = 206,      // 7806
        CnTanUyen = 207,       // 7807
        CnNamNhun = 208        // 7808
        // Đã bỏ CnTinhLaiChau theo yêu cầu
    }

    // Model định nghĩa template cho từng bảng giao khoán
    [Table("KpiAssignmentTables")]
    public class KpiAssignmentTable
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public KpiTableType TableType { get; set; }

        [Required]
        [StringLength(200)]
        public string TableName { get; set; } = string.Empty; // Tên hiển thị của bảng

        [StringLength(500)]
        public string? Description { get; set; } // Mô tả bảng giao khoán

        [StringLength(100)]
        public string Category { get; set; } = "Dành cho Cán bộ"; // Tab grouping: "Dành cho Chi nhánh" hoặc "Dành cho Cán bộ"

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property - danh sách các chỉ tiêu trong bảng
        public virtual ICollection<KpiIndicator> Indicators { get; set; } = new List<KpiIndicator>();
    }

    // Model định nghĩa các chỉ tiêu KPI trong từng bảng
    [Table("KpiIndicators")]
    public class KpiIndicator
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TableId { get; set; } // FK tới KpiAssignmentTable

        [Required]
        [StringLength(300)]
        public string IndicatorName { get; set; } = string.Empty; // Tên chỉ tiêu

        [Required]
        public decimal MaxScore { get; set; } // Điểm tối đa

        [Required]
        [StringLength(50)]
        public string Unit { get; set; } = string.Empty; // Đơn vị tính (%, Triệu VND, BT, cái)

        [Required]
        public int OrderIndex { get; set; } // Thứ tự hiển thị

        [Required]
        public KpiValueType ValueType { get; set; } = KpiValueType.NUMBER; // Loại dữ liệu KPI

        public bool IsActive { get; set; } = true;

        // Navigation property
        [ForeignKey("TableId")]
        public virtual KpiAssignmentTable Table { get; set; } = null!;

        // Navigation property - danh sách giao khoán cho nhân viên
        public virtual ICollection<EmployeeKpiTarget> EmployeeTargets { get; set; } = new List<EmployeeKpiTarget>();
    }

    // Model lưu chỉ tiêu giao khoán cho từng nhân viên
    [Table("EmployeeKpiTargets")]
    public class EmployeeKpiTarget
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; } // FK tới Employee

        [Required]
        public int IndicatorId { get; set; } // FK tới KpiIndicator

        [Required]
        public int KhoanPeriodId { get; set; } // FK tới KhoanPeriod

        public decimal? TargetValue { get; set; } // Chỉ tiêu được giao

        public decimal? ActualValue { get; set; } // Giá trị thực hiện

        public decimal? Score { get; set; } // Điểm đạt được

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; } // Ghi chú

        // Navigation properties
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey("IndicatorId")]
        public virtual KpiIndicator Indicator { get; set; } = null!;

        [ForeignKey("KhoanPeriodId")]
        public virtual KhoanPeriod KhoanPeriod { get; set; } = null!;
    }
}
