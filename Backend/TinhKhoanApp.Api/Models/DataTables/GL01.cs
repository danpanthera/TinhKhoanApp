using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng GL01 - Partitioned Columnstore (Monthly Data Pattern)
    /// ARCHITECTURE: Partitioned by IMPORT_MONTH + Clustered Columnstore Index
    /// OPTIMIZATION: Tiết kiệm 10-20x dung lượng, truy vấn nhanh cho monthly batch data
    /// STRUCTURE: [27 Business Columns] + [System Columns] + [Partition Key]
    /// HEADERS: STS,NGAY_GD,NGUOI_TAO,DYSEQ,TR_TYPE,DT_SEQ,TAI_KHOAN,TEN_TK,SO_TIEN_GD,POST_BR,LOAI_TIEN,DR_CR,MA_KH,TEN_KH,CCA_USRID,TR_EX_RT,REMARK,BUS_CODE,UNIT_BUS_CODE,TR_CODE,TR_NAME,REFERENCE,VALUE_DATE,DEPT_CODE,TR_TIME,COMFIRM,TRDT_TIME
    /// </summary>
    [Table("GL01")]
    public class GL01
    {
        // === AUTO-INCREMENT PRIMARY KEY ===
        [Key]
        [Column("Id")]
        public long Id { get; set; } // Changed to BIGINT for large monthly datasets

        // === 27 CỘT BUSINESS DATA THEO CSV GỐC (Positions 2-28) ===
        [Column("STS")]
        [StringLength(20)]
        public string? STS { get; set; }

        [Column("NGAY_GD")]
        [StringLength(20)]
        public string? NGAY_GD { get; set; }

        [Column("NGUOI_TAO")]
        [StringLength(100)]
        public string? NGUOI_TAO { get; set; }

        [Column("DYSEQ")]
        [StringLength(50)]
        public string? DYSEQ { get; set; }

        [Column("TR_TYPE")]
        [StringLength(20)]
        public string? TR_TYPE { get; set; }

        [Column("DT_SEQ")]
        [StringLength(50)]
        public string? DT_SEQ { get; set; }

        [Column("TAI_KHOAN")]
        [StringLength(50)]
        public string? TAI_KHOAN { get; set; }

        [Column("TEN_TK")]
        [StringLength(255)]
        public string? TEN_TK { get; set; }

        [Column("SO_TIEN_GD")]
        public decimal? SO_TIEN_GD { get; set; }

        [Column("POST_BR")]
        [StringLength(20)]
        public string? POST_BR { get; set; }

        [Column("LOAI_TIEN")]
        [StringLength(10)]
        public string? LOAI_TIEN { get; set; }

        [Column("DR_CR")]
        [StringLength(10)]
        public string? DR_CR { get; set; }

        [Column("MA_KH")]
        [StringLength(50)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH")]
        [StringLength(255)]
        public string? TEN_KH { get; set; }

        [Column("CCA_USRID")]
        [StringLength(50)]
        public string? CCA_USRID { get; set; }

        [Column("TR_EX_RT")]
        public decimal? TR_EX_RT { get; set; }

        [Column("REMARK")]
        [StringLength(500)]
        public string? REMARK { get; set; }

        [Column("BUS_CODE")]
        [StringLength(20)]
        public string? BUS_CODE { get; set; }

        [Column("UNIT_BUS_CODE")]
        [StringLength(20)]
        public string? UNIT_BUS_CODE { get; set; }

        [Column("TR_CODE")]
        [StringLength(20)]
        public string? TR_CODE { get; set; }

        [Column("TR_NAME")]
        [StringLength(255)]
        public string? TR_NAME { get; set; }

        [Column("REFERENCE")]
        [StringLength(100)]
        public string? REFERENCE { get; set; }

        [Column("VALUE_DATE")]
        [StringLength(20)]
        public string? VALUE_DATE { get; set; }

        [Column("DEPT_CODE")]
        [StringLength(20)]
        public string? DEPT_CODE { get; set; }

        [Column("TR_TIME")]
        [StringLength(20)]
        public string? TR_TIME { get; set; }

        [Column("COMFIRM")]
        [StringLength(20)]
        public string? COMFIRM { get; set; }

        [Column("TRDT_TIME")]
        [StringLength(20)]
        public string? TRDT_TIME { get; set; }

        // === SYSTEM/TEMPORAL COLUMNS (Positions 29+) ===

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        [Column("CREATED_DATE")]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        [Column("UPDATED_DATE")]
        public DateTime? UPDATED_DATE { get; set; }

        [Column("FILE_NAME")]
        [StringLength(255)]
        public string? FILE_NAME { get; set; }

        // === PARTITION KEY (Computed Column) ===
        /// <summary>
        /// Partition key được tính tự động từ NGAY_DL để phân chia dữ liệu theo tháng
        /// Tối ưu cho Columnstore compression và query performance
        /// </summary>
        [Column("IMPORT_MONTH")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime IMPORT_MONTH { get; set; }
    }
}
