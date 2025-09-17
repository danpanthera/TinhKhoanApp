using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khoan.Api.Models.Entities
{
    [Table("RR01")]
    public class RR01Entity : ITemporalEntity
    {
        [Key]
        public long Id { get; set; }

        // Business columns from RR01 structure
        [Column("CN_LOAI_I")]
        [StringLength(50)]
        public string? CnLoaiI { get; set; }

        [Column("BRCD")]
        [StringLength(20)]
        public string? Brcd { get; set; }

        [Column("MA_KH")]
        [StringLength(50)]
        public string? MaKh { get; set; }

        [Column("TEN_KH")]
        [StringLength(255)]
        public string? TenKh { get; set; }

        [Column("SO_LDS")]
        [StringLength(50)]
        public string? SoLds { get; set; }

        [Column("CCY")]
        [StringLength(10)]
        public string? Ccy { get; set; }

        [Column("SO_LAV")]
        [StringLength(50)]
        public string? SoLav { get; set; }

        [Column("LOAI_KH")]
        [StringLength(20)]
        public string? LoaiKh { get; set; }

        [Column("NGAY_GIAI_NGAN")]
        [StringLength(20)]
        public string? NgayGiaiNgan { get; set; }

        [Column("NGAY_DAO_HAN")]
        [StringLength(20)]
        public string? NgayDaoHan { get; set; }

        [Column("SO_DU_NO")]
        public decimal? SoDuNo { get; set; }

        [Column("SO_DU_NO_VND")]
        public decimal? SoDuNoVnd { get; set; }

        [Column("LOAI_TS")]
        [StringLength(20)]
        public string? LoaiTs { get; set; }

        [Column("GIA_TRI_TS_DAM_BAO")]
        public decimal? GiaTriTsDamBao { get; set; }

        [Column("GIA_TRI_TS_DAM_BAO_VND")]
        public decimal? GiaTriTsDamBaoVnd { get; set; }

        [Column("TY_LE_DAM_BAO")]
        public decimal? TyLeDamBao { get; set; }

        [Column("LOAI_TS_KHAC")]
        [StringLength(50)]
        public string? LoaiTsKhac { get; set; }

        [Column("GIA_TRI_TS_KHAC")]
        public decimal? GiaTriTsKhac { get; set; }

        [Column("GIA_TRI_TS_KHAC_VND")]
        public decimal? GiaTriTsKhacVnd { get; set; }

        [Column("TY_LE_DAM_BAO_KHAC")]
        public decimal? TyLeDamBaoKhac { get; set; }

        [Column("MUC_PHAN_LOAI")]
        [StringLength(20)]
        public string? MucPhanLoai { get; set; }

        [Column("SO_TIEN_DU_PHONG")]
        public decimal? SoTienDuPhong { get; set; }

        [Column("SO_TIEN_DU_PHONG_VND")]
        public decimal? SoTienDuPhongVnd { get; set; }

        [Column("GHI_CHU")]
        [StringLength(500)]
        public string? GhiChu { get; set; }

        [Column("GIA_TRI_TAI_SAN_THE_CHAP")]
        public decimal? GiaTriTaiSanTheChap { get; set; }

        // Temporal columns (ITemporalEntity)
        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }

        [Column("SysStartTime")]
        public DateTime SysStartTime { get; set; }

        [Column("SysEndTime")]
        public DateTime SysEndTime { get; set; }
    }
}
