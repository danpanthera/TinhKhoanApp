using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.RawData
{
    /// <summary>
    /// Base class cho tất cả các History tables với SCD Type 2
    /// </summary>
    public abstract class BaseHistoryModel
    {
        [Key]
        public long HistoryID { get; set; }

        [Required]
        [StringLength(50)]
        public string SourceID { get; set; }

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidTo { get; set; } = new DateTime(9999, 12, 31, 23, 59, 59);

        [Required]
        public bool IsCurrent { get; set; } = true;

        [Required]
        public int VersionNumber { get; set; } = 1;

        public string RecordHash { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// LN01 History Model
    /// </summary>
    [Table("LN01_History")]
    public class LN01History : BaseHistoryModel
    {
        [StringLength(3)]
        public string MANDT { get; set; }

        [StringLength(4)]
        public string BUKRS { get; set; }

        [StringLength(3)]
        public string LAND1 { get; set; }

        [StringLength(5)]
        public string WAERS { get; set; }

        [StringLength(1)]
        public string SPRAS { get; set; }

        [StringLength(4)]
        public string KTOPL { get; set; }

        [StringLength(2)]
        public string WAABW { get; set; }

        [StringLength(2)]
        public string PERIV { get; set; }

        [StringLength(1)]
        public string KOKFI { get; set; }

        [StringLength(6)]
        public string RCOMP { get; set; }

        [StringLength(10)]
        public string ADRNR { get; set; }

        [StringLength(20)]
        public string STCEG { get; set; }

        [StringLength(4)]
        public string FIKRS { get; set; }

        [StringLength(1)]
        public string XFMCO { get; set; }

        [StringLength(1)]
        public string XFMCB { get; set; }

        [StringLength(1)]
        public string XFMCA { get; set; }

        [StringLength(15)]
        public string TXJCD { get; set; }
    }

    /// <summary>
    /// GL01 History Model
    /// </summary>
    [Table("GL01_History")]
    public class GL01History : BaseHistoryModel
    {
        [StringLength(3)]
        public string MANDT { get; set; }

        [StringLength(4)]
        public string BUKRS { get; set; }

        [StringLength(4)]
        public string GJAHR { get; set; }

        [StringLength(10)]
        public string BELNR { get; set; }

        [StringLength(3)]
        public string BUZEI { get; set; }

        [StringLength(8)]
        public string AUGDT { get; set; }

        [StringLength(6)]
        public string AUGCP { get; set; }

        [StringLength(10)]
        public string AUGBL { get; set; }

        [StringLength(2)]
        public string BSCHL { get; set; }

        [StringLength(1)]
        public string KOART { get; set; }

        [StringLength(1)]
        public string UMSKZ { get; set; }

        [StringLength(1)]
        public string UMSKS { get; set; }

        [StringLength(1)]
        public string ZUMSK { get; set; }

        [StringLength(1)]
        public string SHKZG { get; set; }

        [StringLength(4)]
        public string GSBER { get; set; }

        [StringLength(4)]
        public string PARGB { get; set; }

        [StringLength(2)]
        public string MWSKZ { get; set; }

        [StringLength(2)]
        public string QSSKZ { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public decimal? DMBTR { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public decimal? WRBTR { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public decimal? KZBTR { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public decimal? PSWBT { get; set; }

        [StringLength(5)]
        public string PSWSL { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public decimal? TXBHW { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public decimal? TXBFW { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public decimal? MWSTS { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public decimal? MWSTV { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public decimal? HWBAS { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public decimal? FWBAS { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public decimal? HWSTE { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public decimal? FWSTE { get; set; }

        [StringLength(10)]
        public string STBLG { get; set; }

        [StringLength(2)]
        public string STGRD { get; set; }

        [StringLength(8)]
        public string VALUT { get; set; }

        [StringLength(18)]
        public string ZUONR { get; set; }

        [StringLength(50)]
        public string SGTXT { get; set; }

        [StringLength(1)]
        public string ZINKZ { get; set; }

        [StringLength(6)]
        public string VBUND { get; set; }

        [StringLength(3)]
        public string BEWAR { get; set; }

        [StringLength(10)]
        public string ALTKT { get; set; }

        [StringLength(4)]
        public string VORGN { get; set; }

        [StringLength(2)]
        public string FDLEV { get; set; }

        [StringLength(10)]
        public string FDGRP { get; set; }

        [StringLength(10)]
        public string HKONT { get; set; }

        [StringLength(10)]
        public string KUNNR { get; set; }

        [StringLength(10)]
        public string LIFNR { get; set; }

        [StringLength(10)]
        public string FILKD { get; set; }

        [StringLength(1)]
        public string XBILK { get; set; }

        [StringLength(2)]
        public string GVTYP { get; set; }

        [StringLength(18)]
        public string HZUON { get; set; }

        [StringLength(8)]
        public string ZFBDT { get; set; }

        [StringLength(4)]
        public string ZTERM { get; set; }

        public int? ZBD1T { get; set; }

        public int? ZBD2T { get; set; }

        public int? ZBD3T { get; set; }

        [Column(TypeName = "decimal(5,3)")]
        public decimal? ZBD1P { get; set; }

        [Column(TypeName = "decimal(5,3)")]
        public decimal? ZBD2P { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public decimal? SKFBT { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public decimal? SKNTO { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public decimal? WSKTO { get; set; }

        [StringLength(1)]
        public string ZLSCH { get; set; }

        [StringLength(1)]
        public string ZLSPR { get; set; }

        [StringLength(1)]
        public string ZBFIX { get; set; }

        [StringLength(5)]
        public string HBKID { get; set; }

        [StringLength(4)]
        public string BVTYP { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public decimal? NEBTR { get; set; }

        [StringLength(1)]
        public string MWART { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public decimal? DMBE2 { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        public decimal? DMBE3 { get; set; }

        [StringLength(3)]
        public string PPRCT { get; set; }

        [StringLength(12)]
        public string XREF1 { get; set; }

        [StringLength(12)]
        public string XREF2 { get; set; }

        [StringLength(12)]
        public string KOST1 { get; set; }

        [StringLength(12)]
        public string KOST2 { get; set; }

        [StringLength(10)]
        public string VBEL2 { get; set; }

        [StringLength(6)]
        public string POSN2 { get; set; }

        [StringLength(4)]
        public string KKBER { get; set; }

        [StringLength(12)]
        public string EMPFB { get; set; }
    }

    /// <summary>
    /// Import Log Model
    /// </summary>
    [Table("ImportLog")]
    public class ImportLog
    {
        [Key]
        public long LogID { get; set; }

        [Required]
        [StringLength(50)]
        public string BatchId { get; set; }

        [Required]
        [StringLength(50)]
        public string TableName { get; set; }

        [Required]
        public DateTime ImportDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "PENDING";

        public int TotalRecords { get; set; } = 0;
        public int ProcessedRecords { get; set; } = 0;
        public int NewRecords { get; set; } = 0;
        public int UpdatedRecords { get; set; } = 0;
        public int DeletedRecords { get; set; } = 0;

        public string ErrorMessage { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Duration { get; set; } // seconds

        [StringLength(50)]
        public string CreatedBy { get; set; } = "SYSTEM";
    }

    /// <summary>
    /// Import Statistics DTO
    /// </summary>
    public class ImportStatisticsDto
    {
        public string TableName { get; set; }
        public int TotalImports { get; set; }
        public int SuccessfulImports { get; set; }
        public int FailedImports { get; set; }
        public int ProcessingImports { get; set; }
        public DateTime? LastImportDate { get; set; }
        public long TotalRecordsProcessed { get; set; }
        public long TotalNewRecords { get; set; }
        public long TotalUpdatedRecords { get; set; }
        public long TotalDeletedRecords { get; set; }
        public double AvgDurationSeconds { get; set; }
        public double SuccessRate { get; set; }
    }

    /// <summary>
    /// Import Request DTO
    /// </summary>
    public class ImportRequestDto
    {
        [Required]
        public string TableName { get; set; }

        public string BatchId { get; set; }
        public DateTime? ImportDate { get; set; }
        public string CreatedBy { get; set; } = "USER";
        public bool DetectChangesOnly { get; set; } = true;
        public bool BackupBeforeImport { get; set; } = false;
    }

    /// <summary>
    /// Import Response DTO
    /// </summary>
    public class ImportResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string BatchId { get; set; }
        public ImportStatisticsDto Statistics { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }
}
