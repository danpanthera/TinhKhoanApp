using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    [Table("EI01")]
    public class EI01
    {
        [Key]
        public long Id { get; set; }

        public DateTime ImportedAt { get; set; }

        public string? StatementDate { get; set; }

        // Business columns - all string type as required
        public string? MA_CN { get; set; }
        public string? MA_KH { get; set; }
        public string? TEN_KH { get; set; }
        public string? LOAI_KH { get; set; }
        public string? SDT_EMB { get; set; }
        public string? TRANG_THAI_EMB { get; set; }
        public string? NGAY_DK_EMB { get; set; }
        public string? SDT_OTT { get; set; }
        public string? TRANG_THAI_OTT { get; set; }
        public string? NGAY_DK_OTT { get; set; }
        public string? SDT_SMS { get; set; }
        public string? TRANG_THAI_SMS { get; set; }
        public string? NGAY_DK_SMS { get; set; }
        public string? SDT_SAV { get; set; }
        public string? TRANG_THAI_SAV { get; set; }
        public string? NGAY_DK_SAV { get; set; }
        public string? SDT_LN { get; set; }
        public string? TRANG_THAI_LN { get; set; }
        public string? NGAY_DK_LN { get; set; }
        public string? USER_EMB { get; set; }
        public string? USER_OTT { get; set; }
        public string? USER_SMS { get; set; }
        public string? USER_SAV { get; set; }
        public string? USER_LN { get; set; }

        // Common fields
        public string? NGAY_DL { get; set; }
        public string? FILE_NAME { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
