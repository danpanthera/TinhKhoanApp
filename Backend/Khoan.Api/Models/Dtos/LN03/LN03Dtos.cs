using System.ComponentModel.DataAnnotations;

namespace Khoan.Api.Models.DTOs.LN03
{
    // Preview DTO for listing LN03
    public class LN03PreviewDto
    {
        public long Id { get; set; }
        public DateTime NGAY_DL { get; set; }
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
        public string? MALOAI { get; set; }
        public string? LOAIKHACHHANG { get; set; }
        public decimal? SOTIEN { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? FileName { get; set; }
    }

    // Details DTO for full record
    public class LN03DetailsDto
    {
        public long Id { get; set; }
        public DateTime NGAY_DL { get; set; }
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
        public string? MALOAI { get; set; }
        public string? LOAIKHACHHANG { get; set; }
        public decimal? SOTIEN { get; set; }
        public string? FILE_NAME { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    // Create DTO
    public class LN03CreateDto
    {
        [Required]
        public DateTime NGAY_DL { get; set; }
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
        public string? MALOAI { get; set; }
        public string? LOAIKHACHHANG { get; set; }
        public decimal? SOTIEN { get; set; }
        public string? FILE_NAME { get; set; }
    }

    // Update DTO
    public class LN03UpdateDto : LN03CreateDto
    {
        [Required]
        public long Id { get; set; }
    }

    // Simple processing summary DTO
    public class LN03ProcessingSummaryDto
    {
        public int TotalContracts { get; set; }
        public decimal TotalProcessingAmount { get; set; }
        public decimal AverageProcessingAmount { get; set; }
        public Dictionary<string, int> ContractsByBranch { get; set; } = new();
        public Dictionary<string, int> ContractsByOfficer { get; set; } = new();
    }
}
