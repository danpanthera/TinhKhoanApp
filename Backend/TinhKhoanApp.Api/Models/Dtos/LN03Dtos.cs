using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.DTOs
{
    /// <summary>
    /// DTO chính cho hiển thị chi tiết LN03
    /// </summary>
    public class LN03DetailsDto
    {
        public long Id { get; set; }
        
        // System field đầu tiên
        public DateTime NGAY_DL { get; set; }
        
        // 17 business columns (có header)
        public string? MACHINHANH { get; set; }
        public string? TENCHINHANH { get; set; }
        public string? MAKH { get; set; }
        public string? TENKH { get; set; }
        public string? SOHOPDONG { get; set; }
        public decimal? SOTIENXLRR { get; set; }
        public DateTime? NGAYPHATSINHXL { get; set; }
        public decimal? THUNOSAUXL { get; set; }
        public decimal? CONLAINGOAIBANG { get; set; }
        public decimal? DUNONOIBANG { get; set; }
        public string? NHOMNO { get; set; }
        public string? MACBTD { get; set; }
        public string? TENCBTD { get; set; }
        public string? MAPGD { get; set; }
        public string? TAIKHOANHACHTOAN { get; set; }
        public string? REFNO { get; set; }
        public string? LOAINGUONVON { get; set; }
        
        // 3 columns không có header (Column18-20)
        public string? Column18 { get; set; }
        public string? Column19 { get; set; }
        public decimal? Column20 { get; set; }
        
        // System fields
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? FileName { get; set; }
        
        // Additional system fields từ entity
        public DateTime? ValidFromDate { get; set; }
        public DateTime? ValidToDate { get; set; }
        public string? ImportBatch { get; set; }
        public string? DataSource { get; set; }
        public int? RecordVersion { get; set; }
        public bool IsActive { get; set; }
        public string? LastModifiedBy { get; set; }
    }

    /// <summary>
    /// DTO cho tạo mới LN03
    /// </summary>
    public class LN03CreateDto
    {
        public DateTime NGAY_DL { get; set; }
        
        [Required]
        public string? MACHINHANH { get; set; }
        
        public string? TENCHINHANH { get; set; }
        public string? MAKH { get; set; }
        public string? TENKH { get; set; }
        public string? SOHOPDONG { get; set; }
        public decimal? SOTIENXLRR { get; set; }
        public DateTime? NGAYPHATSINHXL { get; set; }
        public decimal? THUNOSAUXL { get; set; }
        public decimal? CONLAINGOAIBANG { get; set; }
        public decimal? DUNONOIBANG { get; set; }
        public string? NHOMNO { get; set; }
        public string? MACBTD { get; set; }
        public string? TENCBTD { get; set; }
        public string? MAPGD { get; set; }
        public string? TAIKHOANHACHTOAN { get; set; }
        public string? REFNO { get; set; }
        public string? LOAINGUONVON { get; set; }
        
        // 3 columns không có header (Column18-20)
        public string? Column18 { get; set; }
        public string? Column19 { get; set; }
        public decimal? Column20 { get; set; }
        
        // System field
        public string? FileName { get; set; }
    }

    /// <summary>
    /// DTO cho cập nhật LN03
    /// </summary>
    public class LN03UpdateDto
    {
        public string? MACHINHANH { get; set; }
        public string? TENCHINHANH { get; set; }
        public string? MAKH { get; set; }
        public string? TENKH { get; set; }
        public string? SOHOPDONG { get; set; }
        public decimal? SOTIENXLRR { get; set; }
        public DateTime? NGAYPHATSINHXL { get; set; }
        public decimal? THUNOSAUXL { get; set; }
        public decimal? CONLAINGOAIBANG { get; set; }
        public decimal? DUNONOIBANG { get; set; }
        public string? NHOMNO { get; set; }
        public string? MACBTD { get; set; }
        public string? TENCBTD { get; set; }
        public string? MAPGD { get; set; }
        public string? TAIKHOANHACHTOAN { get; set; }
        public string? REFNO { get; set; }
        public string? LOAINGUONVON { get; set; }
        
        // 3 columns không có header (Column18-20)
        public string? Column18 { get; set; }
        public string? Column19 { get; set; }
        public decimal? Column20 { get; set; }
    }

    /// <summary>
    /// DTO cho preview/listing LN03
    /// </summary>
    public class LN03PreviewDto
    {
        public long Id { get; set; }
        public DateTime NGAY_DL { get; set; }
        
        // Business columns
        public string? MACHINHANH { get; set; }
        public string? TENCHINHANH { get; set; }
        public string? MAKH { get; set; }
        public string? TENKH { get; set; }
        public string? SOHOPDONG { get; set; }
        public decimal SOTIENXLRR { get; set; }
        public DateTime? NGAYPHATSINHXL { get; set; }
        public decimal THUNOSAUXL { get; set; }
        public decimal CONLAINGOAIBANG { get; set; }
        public decimal DUNONOIBANG { get; set; }
        public string? NHOMNO { get; set; }
        public string? MACBTD { get; set; }
        public string? TENCBTD { get; set; }
        public string? MAPGD { get; set; }
        public string? TAIKHOANHACHTOAN { get; set; }
        public string? REFNO { get; set; }
        public string? LOAINGUONVON { get; set; }
        
        // 3 columns không có header
        public string? Column18 { get; set; }
        public string? Column19 { get; set; }
        public decimal Column20 { get; set; }
        
        // System fields  
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? FileName { get; set; }
    }

    /// <summary>
    /// DTO cho summary/dashboard LN03
    /// </summary>
    public class LN03SummaryDto
    {
        public DateTime NgayDL { get; set; }
        public int TotalRecords { get; set; }
        public int TotalBranches { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalContracts { get; set; }
        public decimal TotalSoTienXLRR { get; set; }
        public decimal TotalThuNoSauXL { get; set; }
        public decimal TotalConLaiNgoaiBang { get; set; }
        public decimal TotalDuNoNoiBang { get; set; }
        public DateTime? LastImportDate { get; set; }
        public string? FileName { get; set; }
    }

    // Type aliases for backwards compatibility
    public class LN03DTO : LN03DetailsDto { }
    public class CreateLN03DTO : LN03CreateDto { }
    public class UpdateLN03DTO : LN03UpdateDto { }
}
