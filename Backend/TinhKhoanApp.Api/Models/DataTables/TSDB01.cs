using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng TSDB01 - 16 cột theo header_7800_tsdb01_20250430.csv
    /// MA_CN,MA_KH,TEN_KH,LOAI_KH,TONG_DU_NO,VAY_NGAN_HAN,VAY_TRUNG_HAN,VAY_DAI_HAN,DU_NO_KHONG_TSDB,TONG_TSDB,BDS,MAY_MOC,GIAY_TO_CO_GIA,TSDB_KHAC,MA_NGANH_KINH_TE,CHO_VAY_NNNT
    /// </summary>
    [Table("TSDB01")]
    public class TSDB01
    {
        [Key]
        public int Id { get; set; }

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        // === 16 CỘT THEO HEADER CSV GỐC ===
        [Column("MA_CN")]
        [StringLength(50)]
        public string? MA_CN { get; set; }

        [Column("MA_KH")]
        [StringLength(50)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH")]
        [StringLength(255)]
        public string? TEN_KH { get; set; }

        [Column("LOAI_KH")]
        [StringLength(50)]
        public string? LOAI_KH { get; set; }

        [Column("TONG_DU_NO")]
        public decimal? TONG_DU_NO { get; set; }

        [Column("VAY_NGAN_HAN")]
        public decimal? VAY_NGAN_HAN { get; set; }

        [Column("VAY_TRUNG_HAN")]
        public decimal? VAY_TRUNG_HAN { get; set; }

        [Column("VAY_DAI_HAN")]
        public decimal? VAY_DAI_HAN { get; set; }

        [Column("DU_NO_KHONG_TSDB")]
        public decimal? DU_NO_KHONG_TSDB { get; set; }

        [Column("TONG_TSDB")]
        public decimal? TONG_TSDB { get; set; }

        [Column("BDS")]
        public decimal? BDS { get; set; }

        [Column("MAY_MOC")]
        public decimal? MAY_MOC { get; set; }

        [Column("GIAY_TO_CO_GIA")]
        public decimal? GIAY_TO_CO_GIA { get; set; }

        [Column("TSDB_KHAC")]
        public decimal? TSDB_KHAC { get; set; }

        [Column("MA_NGANH_KINH_TE")]
        [StringLength(50)]
        public string? MA_NGANH_KINH_TE { get; set; }

        [Column("CHO_VAY_NNNT")]
        public decimal? CHO_VAY_NNNT { get; set; }

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
