namespace Khoan.Api.Models.DTOs.GL02
{
    public class GL02PreviewDto
    {
        public DateTime NGAY_DL { get; set; }
        public string? UNIT { get; set; }
        public string? TRCD { get; set; }
        public string? CCY { get; set; }
        public decimal? DRAMOUNT { get; set; }
        public decimal? CRAMOUNT { get; set; }
        public DateTime? CRTDTM { get; set; }
    }

    public class GL02DetailsDto
    {
        public long Id { get; set; }
        public DateTime NGAY_DL { get; set; }
        public string? TRBRCD { get; set; }
        public string? USERID { get; set; }
        public string? JOURSEQ { get; set; }
        public string? DYTRSEQ { get; set; }
        public string? LOCAC { get; set; }
        public string? CCY { get; set; }
        public string? BUSCD { get; set; }
        public string? UNIT { get; set; }
        public string? TRCD { get; set; }
        public string? CUSTOMER { get; set; }
        public string? TRTP { get; set; }
        public string? REFERENCE { get; set; }
        public string? REMARK { get; set; }
        public decimal? DRAMOUNT { get; set; }
        public decimal? CRAMOUNT { get; set; }
        public DateTime? CRTDTM { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public string? FILE_NAME { get; set; }
    }

    public class GL02CreateDto
    {
        public DateTime NGAY_DL { get; set; }
        public string? TRBRCD { get; set; }
        public string? USERID { get; set; }
        public string? JOURSEQ { get; set; }
        public string? DYTRSEQ { get; set; }
        public string? LOCAC { get; set; }
        public string? CCY { get; set; }
        public string? BUSCD { get; set; }
        public string? UNIT { get; set; }
        public string? TRCD { get; set; }
        public string? CUSTOMER { get; set; }
        public string? TRTP { get; set; }
        public string? REFERENCE { get; set; }
        public string? REMARK { get; set; }
        public decimal? DRAMOUNT { get; set; }
        public decimal? CRAMOUNT { get; set; }
        public DateTime? CRTDTM { get; set; }
    }

    public class GL02UpdateDto : GL02CreateDto
    {
        public long Id { get; set; }
    }

    public class GL02SummaryByUnitDto
    {
        public string? UNIT { get; set; }
        public decimal TotalDR { get; set; }
        public decimal TotalCR { get; set; }
    }

    public class GL02ImportResultDto
    {
        public string? FileName { get; set; }
        public int Processed { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
