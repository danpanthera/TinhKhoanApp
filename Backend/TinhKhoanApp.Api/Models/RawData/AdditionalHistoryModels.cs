using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.RawData
{
    // =======================================
    // 📊 LN03 - Dữ liệu Nợ XLRR History Model
    // =======================================
    [Table("LN03_History")]
    public class LN03History : IExtendedHistoryModel
    {
        [Key]
        public int Id { get; set; }

        // 🔑 SCD Type 2 Fields
        [Required]
        [StringLength(500)]
        public string BusinessKey { get; set; } = null!;

        [Required]
        public DateTime EffectiveDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Required]
        public bool IsCurrent { get; set; } = true;

        [Required]
        public int RowVersion { get; set; } = 1;

        // 🗃️ Metadata Fields
        [Required]
        [StringLength(100)]
        public string ImportId { get; set; } = null!;

        [Required]
        public DateTime StatementDate { get; set; }

        [Required]
        public DateTime ProcessedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(64)]
        public string DataHash { get; set; } = null!;

        // 📊 Business Data Fields for LN03 (Nợ XLRR)
        [StringLength(50)]
        public string? MaKhachHang { get; set; }

        [StringLength(500)]
        public string? TenKhachHang { get; set; }

        [StringLength(50)]
        public string? MaHopDong { get; set; }

        [StringLength(100)]
        public string? LoaiHopDong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SoTienGoc { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SoTienLai { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TongNo { get; set; }

        public DateTime? NgayDaoHan { get; set; }

        [StringLength(100)]
        public string? TinhTrangNo { get; set; }

        public int? NhomNo { get; set; }

        [StringLength(20)]
        public string? MaChiNhanh { get; set; }

        [StringLength(200)]
        public string? TenChiNhanh { get; set; }

        public DateTime? NgayTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        // 📝 Additional JSON field for flexible data
        public string? AdditionalData { get; set; }
    }

    // =======================================
    // 📊 EI01 - Dữ liệu Mobile Banking History Model
    // =======================================
    [Table("EI01_History")]
    public class EI01History : IExtendedHistoryModel
    {
        [Key]
        public int Id { get; set; }

        // 🔑 SCD Type 2 Fields
        [Required]
        [StringLength(500)]
        public string BusinessKey { get; set; } = null!;

        [Required]
        public DateTime EffectiveDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Required]
        public bool IsCurrent { get; set; } = true;

        [Required]
        public int RowVersion { get; set; } = 1;

        // 🗃️ Metadata Fields
        [Required]
        [StringLength(100)]
        public string ImportId { get; set; } = null!;

        [Required]
        public DateTime StatementDate { get; set; }

        [Required]
        public DateTime ProcessedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(64)]
        public string DataHash { get; set; } = null!;

        // 📊 Business Data Fields for EI01 (Mobile Banking)
        [StringLength(50)]
        public string? MaKhachHang { get; set; }

        [StringLength(500)]
        public string? TenKhachHang { get; set; }

        [StringLength(50)]
        public string? SoTaiKhoan { get; set; }

        [StringLength(100)]
        public string? LoaiGiaoDich { get; set; }

        [StringLength(100)]
        public string? MaGiaoDich { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SoTien { get; set; }

        public DateTime? NgayGiaoDich { get; set; }

        public DateTime? ThoiGianGiaoDich { get; set; }

        [StringLength(50)]
        public string? TrangThaiGiaoDich { get; set; }

        [StringLength(1000)]
        public string? NoiDungGiaoDich { get; set; }

        [StringLength(20)]
        public string? MaChiNhanh { get; set; }

        [StringLength(200)]
        public string? TenChiNhanh { get; set; }

        [StringLength(50)]
        public string? Channel { get; set; }

        [StringLength(500)]
        public string? DeviceInfo { get; set; }

        // 📝 Additional JSON field for flexible data
        public string? AdditionalData { get; set; }
    }

    // =======================================
    // 📊 DPDA - Dữ liệu Phát hành thẻ History Model
    // =======================================
    [Table("DPDA_History")]
    public class DPDAHistory : IExtendedHistoryModel
    {
        [Key]
        public int Id { get; set; }

        // 🔑 SCD Type 2 Fields
        [Required]
        [StringLength(500)]
        public string BusinessKey { get; set; } = null!;

        [Required]
        public DateTime EffectiveDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Required]
        public bool IsCurrent { get; set; } = true;

        [Required]
        public int RowVersion { get; set; } = 1;

        // 🗃️ Metadata Fields
        [Required]
        [StringLength(100)]
        public string ImportId { get; set; } = null!;

        [Required]
        public DateTime StatementDate { get; set; }

        [Required]
        public DateTime ProcessedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(64)]
        public string DataHash { get; set; } = null!;

        // 📊 Business Data Fields for DPDA (Phát hành thẻ)
        [StringLength(50)]
        public string? MaKhachHang { get; set; }

        [StringLength(500)]
        public string? TenKhachHang { get; set; }

        [StringLength(50)]
        public string? SoThe { get; set; }

        [StringLength(100)]
        public string? LoaiThe { get; set; }

        [StringLength(50)]
        public string? TrangThaiThe { get; set; }

        public DateTime? NgayPhatHanh { get; set; }

        public DateTime? NgayHetHan { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? HanMucThe { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SoDuHienTai { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SoTienDaSD { get; set; }

        [StringLength(20)]
        public string? MaChiNhanh { get; set; }

        [StringLength(200)]
        public string? TenChiNhanh { get; set; }

        public DateTime? NgayTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        // 📝 Additional JSON field for flexible data
        public string? AdditionalData { get; set; }
    }

    // =======================================
    // 📊 DB01 - Sao kê TSDB History Model
    // =======================================
    [Table("DB01_History")]
    public class DB01History : IExtendedHistoryModel
    {
        [Key]
        public int Id { get; set; }

        // 🔑 SCD Type 2 Fields
        [Required]
        [StringLength(500)]
        public string BusinessKey { get; set; } = null!;

        [Required]
        public DateTime EffectiveDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Required]
        public bool IsCurrent { get; set; } = true;

        [Required]
        public int RowVersion { get; set; } = 1;

        // 🗃️ Metadata Fields
        [Required]
        [StringLength(100)]
        public string ImportId { get; set; } = null!;

        [Required]
        public DateTime StatementDate { get; set; }

        [Required]
        public DateTime ProcessedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(64)]
        public string DataHash { get; set; } = null!;

        // 📊 Business Data Fields for DB01 (TSDB)
        [StringLength(50)]
        public string? MaKhachHang { get; set; }

        [StringLength(500)]
        public string? TenKhachHang { get; set; }

        [StringLength(50)]
        public string? SoTaiKhoan { get; set; }

        [StringLength(100)]
        public string? LoaiTaiKhoan { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SoDu { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SoDuKhaDung { get; set; }

        public DateTime? NgayMoTK { get; set; }

        [StringLength(50)]
        public string? TrangThaiTK { get; set; }

        [Column(TypeName = "decimal(5,4)")]
        public decimal? LaiSuat { get; set; }

        public int? KyHan { get; set; }

        public DateTime? NgayDaoHan { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SoTienGocGuy { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TienLaiDuThu { get; set; }

        [StringLength(20)]
        public string? MaChiNhanh { get; set; }

        [StringLength(200)]
        public string? TenChiNhanh { get; set; }

        [StringLength(50)]
        public string? LoaiHinhDB { get; set; } // TSDB/Không TSDB

        // 📝 Additional JSON field for flexible data
        public string? AdditionalData { get; set; }
    }

    // =======================================

    // =======================================
    // 📊 BC57 - Lãi dự thu History Model
    // =======================================
    [Table("BC57_History")]
    public class BC57History : IExtendedHistoryModel
    {
        [Key]
        public int Id { get; set; }

        // 🔑 SCD Type 2 Fields
        [Required]
        [StringLength(500)]
        public string BusinessKey { get; set; } = null!;

        [Required]
        public DateTime EffectiveDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Required]
        public bool IsCurrent { get; set; } = true;

        [Required]
        public int RowVersion { get; set; } = 1;

        // 🗃️ Metadata Fields
        [Required]
        [StringLength(100)]
        public string ImportId { get; set; } = null!;

        [Required]
        public DateTime StatementDate { get; set; }

        [Required]
        public DateTime ProcessedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(64)]
        public string DataHash { get; set; } = null!;

        // 📊 Business Data Fields for BC57 (Lãi dự thu)
        [StringLength(50)]
        public string? MaKhachHang { get; set; }

        [StringLength(500)]
        public string? TenKhachHang { get; set; }

        [StringLength(50)]
        public string? SoTaiKhoan { get; set; }

        [StringLength(50)]
        public string? MaHopDong { get; set; }

        [StringLength(100)]
        public string? LoaiSanPham { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SoTienGoc { get; set; }

        [Column(TypeName = "decimal(5,4)")]
        public decimal? LaiSuat { get; set; }

        public int? SoNgayTinhLai { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TienLaiDuThu { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TienLaiQuaHan { get; set; }

        public DateTime? NgayBatDau { get; set; }

        public DateTime? NgayKetThuc { get; set; }

        [StringLength(50)]
        public string? TrangThai { get; set; }

        [StringLength(20)]
        public string? MaChiNhanh { get; set; }

        [StringLength(200)]
        public string? TenChiNhanh { get; set; }

        public DateTime? NgayTinhLai { get; set; }

        // 📝 Additional JSON field for flexible data
        public string? AdditionalData { get; set; }
    }
}
