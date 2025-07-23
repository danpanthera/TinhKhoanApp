using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// DPDA - Deposit Card Application Data Model with exact CSV column structure
    /// Structure: NGAY_DL -> Business Columns (CSV order) -> Temporal Columns
    /// </summary>
    [Table("DPDA")]
    public class DPDA
    {
        // System Column - NGAY_DL first (extracted from filename)
        [Column("NGAY_DL", Order = 0)]
        [StringLength(10)]
        public string NGAY_DL { get; set; } = "";

        // Business Columns - Exact CSV order, all NVARCHAR(50) format
        [Column("MA_CHI_NHANH", Order = 1)]
        [StringLength(50)]
        public string MA_CHI_NHANH { get; set; } = "";

        [Column("MA_KHACH_HANG", Order = 2)]
        [StringLength(50)]
        public string MA_KHACH_HANG { get; set; } = "";

        [Column("TEN_KHACH_HANG", Order = 3)]
        [StringLength(50)]
        public string TEN_KHACH_HANG { get; set; } = "";

        [Column("SO_TAI_KHOAN", Order = 4)]
        [StringLength(50)]
        public string SO_TAI_KHOAN { get; set; } = "";

        [Column("LOAI_THE", Order = 5)]
        [StringLength(50)]
        public string LOAI_THE { get; set; } = "";

        [Column("SO_THE", Order = 6)]
        [StringLength(50)]
        public string SO_THE { get; set; } = "";

        [Column("NGAY_NOP_DON", Order = 7)]
        [StringLength(50)]
        public string NGAY_NOP_DON { get; set; } = "";

        [Column("NGAY_PHAT_HANH", Order = 8)]
        [StringLength(50)]
        public string NGAY_PHAT_HANH { get; set; } = "";

        [Column("USER_PHAT_HANH", Order = 9)]
        [StringLength(50)]
        public string USER_PHAT_HANH { get; set; } = "";

        [Column("TRANG_THAI", Order = 10)]
        [StringLength(50)]
        public string TRANG_THAI { get; set; } = "";

        [Column("PHAN_LOAI", Order = 11)]
        [StringLength(50)]
        public string PHAN_LOAI { get; set; } = "";

        [Column("GIAO_THE", Order = 12)]
        [StringLength(50)]
        public string GIAO_THE { get; set; } = "";

        [Column("LOAI_PHAT_HANH", Order = 13)]
        [StringLength(50)]
        public string LOAI_PHAT_HANH { get; set; } = "";

        // Temporal/System Columns - Always last
        [Key]
        [Column("Id", Order = 14)]
        public int Id { get; set; }

        [Column("CREATED_DATE", Order = 15)]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        [Column("UPDATED_DATE", Order = 16)]
        public DateTime UPDATED_DATE { get; set; } = DateTime.Now;

        [Column("FILE_NAME", Order = 17)]
        [StringLength(255)]
        public string FILE_NAME { get; set; } = "";
    }
}
