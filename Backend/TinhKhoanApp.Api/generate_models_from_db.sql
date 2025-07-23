-- Script to generate C# models from database with exact structure
-- Structure: NGAY_DL -> Business Columns -> Temporal/System Columns

-- Generate DP01 Model
SELECT
'using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// DP01 - Deposit Data Model (Auto-generated from database)
    /// Structure: NGAY_DL -> Business Columns (CSV order) -> Temporal/System Columns
    /// </summary>
    [Table("DP01")]
    public class DP01
    {' as model_header

UNION ALL

SELECT
    CASE
        WHEN COLUMN_NAME = 'Id' THEN
            '        [Key]
        [Column("Id", Order = ' + CAST(ORDINAL_POSITION-1 as VARCHAR) + ')]
        public long Id { get; set; }'
        WHEN COLUMN_NAME = 'NGAY_DL' THEN
            '        [Column("NGAY_DL", Order = ' + CAST(ORDINAL_POSITION-1 as VARCHAR) + ')]
        [StringLength(20)]
        [Required]
        public string NGAY_DL { get; set; } = "";'
        WHEN DATA_TYPE = 'nvarchar' AND COLUMN_NAME NOT IN ('FILE_NAME') THEN
            '        [Column("' + COLUMN_NAME + '", Order = ' + CAST(ORDINAL_POSITION-1 as VARCHAR) + ')]
        [StringLength(' + CAST(CHARACTER_MAXIMUM_LENGTH as VARCHAR) + ')]
        public string ' + COLUMN_NAME + ' { get; set; } = "";'
        WHEN DATA_TYPE = 'datetime2' THEN
            '        [Column("' + COLUMN_NAME + '", Order = ' + CAST(ORDINAL_POSITION-1 as VARCHAR) + ')]
        public DateTime' + CASE WHEN IS_NULLABLE = 'YES' THEN '?' ELSE '' END + ' ' + COLUMN_NAME + ' { get; set; }' +
        CASE WHEN COLUMN_NAME = 'CREATED_DATE' THEN ' = DateTime.Now;'
             WHEN COLUMN_NAME = 'UPDATED_DATE' THEN ' = DateTime.Now;'
             ELSE ';' END
        WHEN DATA_TYPE = 'bigint' AND COLUMN_NAME = 'Id' THEN ''
        ELSE '        // TODO: Handle ' + COLUMN_NAME + ' (' + DATA_TYPE + ')'
    END as property_definition
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'DP01' AND COLUMN_NAME != 'Id'
ORDER BY ORDINAL_POSITION

UNION ALL
SELECT '    }
}' as model_footer;
