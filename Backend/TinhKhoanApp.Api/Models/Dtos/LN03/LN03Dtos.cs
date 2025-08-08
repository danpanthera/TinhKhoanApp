using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.DTOs.LN03
{
    /// <summary>
    /// LN03 Preview DTO - để hiển thị preview data từ bảng LN03
    /// Chứa 20 business columns (17 có header + 3 không có header) theo CSV structure + system columns
    /// </summary>
    public class LN03PreviewDto
    {
        // === SYSTEM COLUMNS ===
        public long Id { get; set; }
        public DateTime NGAY_DL { get; set; }

        // === BUSINESS COLUMNS WITH HEADERS (17 columns) ===
        public string? MACHINHANH { get; set; }
        public string? TENCHINHANH { get; set; }
        public string? MAKH { get; set; }
        public string? TENKH { get; set; }
        public string? SOHOPDONG { get; set; }
        public decimal? SOTIENXLRR { get; set; }          // SOTIEN field - decimal
        public DateTime? NGAYPHATSINHXL { get; set; }     // DATE field - datetime2
        public decimal? THUNOSAUXL { get; set; }          // THUNO field - decimal
        public decimal? CONLAINGOAIBANG { get; set; }     // AMT field - decimal
        public decimal? DUNONOIBANG { get; set; }         // AMT field - decimal
        public string? NHOMNO { get; set; }
        public string? MACBTD { get; set; }
        public string? TENCBTD { get; set; }
        public string? MAPGD { get; set; }
        public string? TAIKHOANHACHTOAN { get; set; }
        public string? REFNO { get; set; }
        public string? LOAINGUONVON { get; set; }

        // === BUSINESS COLUMNS WITHOUT HEADERS (3 columns - positions 18, 19, 20) ===
        public string? Column18 { get; set; }  // No header column position 18
        public string? Column19 { get; set; }  // No header column position 19
        public decimal? Column20 { get; set; } // No header column position 20 - numeric

        // === TEMPORAL/SYSTEM METADATA ===
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? FileName { get; set; }
    }

    /// <summary>
    /// LN03 Create DTO - để tạo record mới LN03
    /// </summary>
    public class LN03CreateDto
    {
        [Required]
        public DateTime NGAY_DL { get; set; }

        // === BUSINESS COLUMNS WITH HEADERS (17 columns) ===
        [MaxLength(200)]
        public string? MACHINHANH { get; set; }

        [MaxLength(200)]
        public string? TENCHINHANH { get; set; }

        [MaxLength(200)]
        public string? MAKH { get; set; }

        [MaxLength(200)]
        public string? TENKH { get; set; }

        [MaxLength(200)]
        public string? SOHOPDONG { get; set; }

        public decimal? SOTIENXLRR { get; set; }          // SOTIEN field - decimal
        public DateTime? NGAYPHATSINHXL { get; set; }     // DATE field - datetime2
        public decimal? THUNOSAUXL { get; set; }          // THUNO field - decimal
        public decimal? CONLAINGOAIBANG { get; set; }     // AMT field - decimal
        public decimal? DUNONOIBANG { get; set; }         // AMT field - decimal

        [MaxLength(200)]
        public string? NHOMNO { get; set; }

        [MaxLength(200)]
        public string? MACBTD { get; set; }

        [MaxLength(200)]
        public string? TENCBTD { get; set; }

        [MaxLength(200)]
        public string? MAPGD { get; set; }

        [MaxLength(200)]
        public string? TAIKHOANHACHTOAN { get; set; }

        [MaxLength(200)]
        public string? REFNO { get; set; }

        [MaxLength(200)]
        public string? LOAINGUONVON { get; set; }

        // === BUSINESS COLUMNS WITHOUT HEADERS (3 columns) ===
        [MaxLength(200)]
        public string? Column18 { get; set; }  // No header column position 18

        [MaxLength(200)]
        public string? Column19 { get; set; }  // No header column position 19

        public decimal? Column20 { get; set; } // No header column position 20 - numeric

        // === SYSTEM METADATA ===
        [MaxLength(400)]
        public string? FileName { get; set; }
    }

    /// <summary>
    /// LN03 Update DTO - để cập nhật record LN03
    /// </summary>
    public class LN03UpdateDto : LN03CreateDto
    {
        [Required]
        public long Id { get; set; }
    }

    /// <summary>
    /// LN03 Details DTO - để hiển thị chi tiết record LN03
    /// Bao gồm temporal tracking information
    /// </summary>
    public class LN03DetailsDto : LN03PreviewDto
    {
        // === TEMPORAL TRACKING ===
        public DateTime ValidFromDate { get; set; }
        public DateTime ValidToDate { get; set; }

        // === AUDIT INFORMATION ===
        public string? ImportBatch { get; set; }
        public string? DataSource { get; set; }

        // === METADATA ===
        public int RecordVersion { get; set; }
        public bool IsActive { get; set; }
        public string? LastModifiedBy { get; set; }
    }

    /// <summary>
    /// LN03 Summary DTO - để hiển thị tóm tắt/aggregate data
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
        public DateTime LastImportDate { get; set; }
        public string? FileName { get; set; }
    }

    /// <summary>
    /// LN03 Import Result DTO - kết quả import CSV
    /// </summary>
    public class LN03ImportResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalRecords { get; set; }
        public int SuccessRecords { get; set; }
        public int FailedRecords { get; set; }
        public DateTime ImportDate { get; set; }
        public DateTime NgayDL { get; set; }
        public string FileName { get; set; } = string.Empty;
        public TimeSpan ProcessingTime { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
