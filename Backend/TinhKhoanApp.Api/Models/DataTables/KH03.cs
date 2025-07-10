using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng KH03 - Thông tin khách hàng (38 cột theo header)
    /// Cấu trúc theo header_7800_kh03_20250430.csv
    /// </summary>
    [Table("KH03")]
    public class KH03
    {
        [Key]
        public int Id { get; set; }

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        // === CỘT THEO HEADER CSV GỐC ===
        [Column("MA_CN")]
        [StringLength(20)]
        public string? MA_CN { get; set; }

        [Column("MA_KH")]
        [StringLength(50)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH")]
        [StringLength(255)]
        public string? TEN_KH { get; set; }

        [Column("SOHD")]
        [StringLength(50)]
        public string? SOHD { get; set; }

        [Column("LDS")]
        [StringLength(50)]
        public string? LDS { get; set; }

        [Column("SO_TIEN_GIAI_NGAN")]
        public decimal? SO_TIEN_GIAI_NGAN { get; set; }

        [Column("NGAY_GIAI_NGAN")]
        [StringLength(20)]
        public string? NGAY_GIAI_NGAN { get; set; }

        [Column("NGAY_DEN_HAN_GN")]
        [StringLength(20)]
        public string? NGAY_DEN_HAN_GN { get; set; }

        [Column("ACCOUNT_NAME")]
        [StringLength(255)]
        public string? ACCOUNT_NAME { get; set; }

        [Column("TY_GIA")]
        public decimal? TY_GIA { get; set; }

        [Column("SOTIEN_PHEDUYET")]
        public decimal? SOTIEN_PHEDUYET { get; set; }

        [Column("DUNO_NGAN")]
        public decimal? DUNO_NGAN { get; set; }

        [Column("DUNO_TRUNGDAI")]
        public decimal? DUNO_TRUNGDAI { get; set; }

        [Column("BAOLANH")]
        public decimal? BAOLANH { get; set; }

        [Column("NHOMNO")]
        [StringLength(20)]
        public string? NHOMNO { get; set; }

        [Column("XLRR")]
        [StringLength(20)]
        public string? XLRR { get; set; }

        [Column("MANGANH")]
        [StringLength(20)]
        public string? MANGANH { get; set; }

        [Column("BSRT")]
        public decimal? BSRT { get; set; }

        [Column("SPRDRT")]
        public decimal? SPRDRT { get; set; }

        [Column("INTRT")]
        public decimal? INTRT { get; set; }

        [Column("XEP_HANG")]
        [StringLength(50)]
        public string? XEP_HANG { get; set; }

        [Column("TONG_TSDB")]
        public decimal? TONG_TSDB { get; set; }

        [Column("BDS")]
        public decimal? BDS { get; set; }

        [Column("DS")]
        public decimal? DS { get; set; }

        [Column("CCTG")]
        public decimal? CCTG { get; set; }

        [Column("CK")]
        public decimal? CK { get; set; }

        [Column("TAI_SAN_BAO_LANH")]
        public decimal? TAI_SAN_BAO_LANH { get; set; }

        [Column("TAI_SAN_KHAC")]
        public decimal? TAI_SAN_KHAC { get; set; }

        [Column("KI_HAN")]
        [StringLength(20)]
        public string? KI_HAN { get; set; }

        [Column("LOAI_KH")]
        [StringLength(50)]
        public string? LOAI_KH { get; set; }

        [Column("QUY_MO_THEO_CHAM_DIEM_KH")]
        [StringLength(50)]
        public string? QUY_MO_THEO_CHAM_DIEM_KH { get; set; }

        [Column("CCYCD")]
        [StringLength(10)]
        public string? CCYCD { get; set; }

        [Column("ACCOUNT_LAV")]
        [StringLength(50)]
        public string? ACCOUNT_LAV { get; set; }

        [Column("QUY_MO_THEO_TT41")]
        [StringLength(50)]
        public string? QUY_MO_THEO_TT41 { get; set; }

        [Column("LOAI_TIEN_LAV")]
        [StringLength(10)]
        public string? LOAI_TIEN_LAV { get; set; }

        [Column("THANG_KY_HAN")]
        public int? THANG_KY_HAN { get; set; }

        [Column("NGAY_PHE_DUYET")]
        [StringLength(20)]
        public string? NGAY_PHE_DUYET { get; set; }

        [Column("NGAY_DEN_HAN_LAV")]
        [StringLength(20)]
        public string? NGAY_DEN_HAN_LAV { get; set; }

        // === TEMPORAL COLUMNS ===
        [Column("CREATED_DATE")]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        [Column("UPDATED_DATE")]
        public DateTime? UPDATED_DATE { get; set; }

        [Column("FILE_NAME")]
        [StringLength(255)]
        public string? FILE_NAME { get; set; }
    }
}
