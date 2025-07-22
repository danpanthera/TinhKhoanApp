using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    [Table("GL01")]
    public class GL01
    {
        [Key]
        public long Id { get; set; }

        public DateTime ImportedAt { get; set; }

        public string? StatementDate { get; set; }

        // Business columns - all string type as required
        public string? STS { get; set; }
        public string? NGAY_GD { get; set; }
        public string? NGUOI_TAO { get; set; }
        public string? DYSEQ { get; set; }
        public string? TR_TYPE { get; set; }
        public string? DT_SEQ { get; set; }
        public string? TAI_KHOAN { get; set; }
        public string? TEN_TK { get; set; }
        public string? SO_TIEN_GD { get; set; }
        public string? POST_BR { get; set; }
        public string? LOAI_TIEN { get; set; }
        public string? DR_CR { get; set; }
        public string? MA_KH { get; set; }
        public string? TEN_KH { get; set; }
        public string? CCA_USRID { get; set; }
        public string? TR_EX_RT { get; set; }
        public string? REMARK { get; set; }
        public string? BUS_CODE { get; set; }
        public string? UNIT_BUS_CODE { get; set; }
        public string? TR_CODE { get; set; }
        public string? TR_NAME { get; set; }
        public string? REFERENCE { get; set; }
        public string? VALUE_DATE { get; set; }
        public string? DEPT_CODE { get; set; }
        public string? TR_TIME { get; set; }
        public string? COMFIRM { get; set; }
        public string? TRDT_TIME { get; set; }

        // Common fields
        public string? MA_CN { get; set; }
        public string? NGAY_DL { get; set; }
        public string? FILE_NAME { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
