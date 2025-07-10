#!/bin/bash

# ===================================================================
# SCRIPT TẠO LẠI CÁC MODEL CÒN LẠI THEO HEADER CSV GỐC
# LN02, LN03, RR01, TSDB01 (không có DB01 trong header)
# ===================================================================

echo "🔧 Tạo lại các model còn lại theo header CSV gốc..."

MODELS_DIR="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/Models/DataTables"

# === 1. LN02 MODEL (11 cột) ===
echo "🔧 Tạo LN02.cs với 11 cột..."
cat > $MODELS_DIR/LN02.cs << 'EOF'
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng LN02 - Thông tin cho vay (11 cột theo header)
    /// Cấu trúc theo header_7800_ln02_20250430.csv
    /// </summary>
    [Table("LN02")]
    public class LN02
    {
        [Key]
        public int Id { get; set; }

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        // === CỘT THEO HEADER CSV GỐC ===
        [Column("MA_CN")]
        [StringLength(20)]
        public string? MA_CN { get; set; }

        [Column("MA_KH")]
        [StringLength(50)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH")]
        [StringLength(255)]
        public string? TEN_KH { get; set; }

        [Column("SOHD")]
        [StringLength(50)]
        public string? SOHD { get; set; }

        [Column("NGAY_KY")]
        [StringLength(20)]
        public string? NGAY_KY { get; set; }

        [Column("NGAY_DH")]
        [StringLength(20)]
        public string? NGAY_DH { get; set; }

        [Column("SOTIENVAY")]
        public decimal? SOTIENVAY { get; set; }

        [Column("SOTIENVAY_USD")]
        public decimal? SOTIENVAY_USD { get; set; }

        [Column("TIENGOCOS")]
        public decimal? TIENGOCOS { get; set; }

        [Column("TIENGOCOS_USD")]
        public decimal? TIENGOCOS_USD { get; set; }

        [Column("SOTIENNO")]
        public decimal? SOTIENNO { get; set; }

        // === TEMPORAL COLUMNS ===
        [Column("CREATED_DATE")]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        [Column("UPDATED_DATE")]
        public DateTime? UPDATED_DATE { get; set; }

        [Column("FILE_NAME")]
        [StringLength(255)]
        public string? FILE_NAME { get; set; }
    }
}
EOF

# === 2. LN03 MODEL (17 cột) ===
echo "🔧 Tạo LN03.cs với 17 cột..."
cat > $MODELS_DIR/LN03.cs << 'EOF'
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng LN03 - Thông tin cho vay chi tiết (17 cột theo header)
    /// Cấu trúc theo header_7800_ln03_20250430.csv
    /// </summary>
    [Table("LN03")]
    public class LN03
    {
        [Key]
        public int Id { get; set; }

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        // === CỘT THEO HEADER CSV GỐC ===
        [Column("MA_CN")]
        [StringLength(20)]
        public string? MA_CN { get; set; }

        [Column("MA_KH")]
        [StringLength(50)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH")]
        [StringLength(255)]
        public string? TEN_KH { get; set; }

        [Column("SOHD")]
        [StringLength(50)]
        public string? SOHD { get; set; }

        [Column("NGAY_KY")]
        [StringLength(20)]
        public string? NGAY_KY { get; set; }

        [Column("NGAY_DH")]
        [StringLength(20)]
        public string? NGAY_DH { get; set; }

        [Column("SOTIENVAY")]
        public decimal? SOTIENVAY { get; set; }

        [Column("SOTIENVAY_USD")]
        public decimal? SOTIENVAY_USD { get; set; }

        [Column("TIENGOCOS")]
        public decimal? TIENGOCOS { get; set; }

        [Column("TIENGOCOS_USD")]
        public decimal? TIENGOCOS_USD { get; set; }

        [Column("SOTIENNO")]
        public decimal? SOTIENNO { get; set; }

        [Column("LOAI_TIEN")]
        [StringLength(10)]
        public string? LOAI_TIEN { get; set; }

        [Column("TSDB")]
        [StringLength(255)]
        public string? TSDB { get; set; }

        [Column("TSDB2")]
        [StringLength(255)]
        public string? TSDB2 { get; set; }

        [Column("NGAY_TSDB")]
        [StringLength(20)]
        public string? NGAY_TSDB { get; set; }

        [Column("GIATRI_TSDB")]
        public decimal? GIATRI_TSDB { get; set; }

        [Column("GIATRI_TSDB_USD")]
        public decimal? GIATRI_TSDB_USD { get; set; }

        // === TEMPORAL COLUMNS ===
        [Column("CREATED_DATE")]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        [Column("UPDATED_DATE")]
        public DateTime? UPDATED_DATE { get; set; }

        [Column("FILE_NAME")]
        [StringLength(255)]
        public string? FILE_NAME { get; set; }
    }
}
EOF

# === 3. RR01 MODEL (25 cột) ===
echo "🔧 Tạo RR01.cs với 25 cột..."
cat > $MODELS_DIR/RR01.cs << 'EOF'
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng RR01 - Báo cáo rủi ro (25 cột theo header)
    /// Cấu trúc theo header_7800_rr01_20250430.csv
    /// </summary>
    [Table("RR01")]
    public class RR01
    {
        [Key]
        public int Id { get; set; }

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        // === CỘT THEO HEADER CSV GỐC ===
        [Column("MA_CN")]
        [StringLength(20)]
        public string? MA_CN { get; set; }

        [Column("MA_KH")]
        [StringLength(50)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH")]
        [StringLength(255)]
        public string? TEN_KH { get; set; }

        [Column("SOHD")]
        [StringLength(50)]
        public string? SOHD { get; set; }

        [Column("NGAY_KY")]
        [StringLength(20)]
        public string? NGAY_KY { get; set; }

        [Column("NGAY_DH")]
        [StringLength(20)]
        public string? NGAY_DH { get; set; }

        [Column("SOTIENVAY")]
        public decimal? SOTIENVAY { get; set; }

        [Column("SOTIENVAY_USD")]
        public decimal? SOTIENVAY_USD { get; set; }

        [Column("TIENGOCOS")]
        public decimal? TIENGOCOS { get; set; }

        [Column("TIENGOCOS_USD")]
        public decimal? TIENGOCOS_USD { get; set; }

        [Column("SOTIENNO")]
        public decimal? SOTIENNO { get; set; }

        [Column("LOAI_TIEN")]
        [StringLength(10)]
        public string? LOAI_TIEN { get; set; }

        [Column("TSDB")]
        [StringLength(255)]
        public string? TSDB { get; set; }

        [Column("TSDB2")]
        [StringLength(255)]
        public string? TSDB2 { get; set; }

        [Column("NGAY_TSDB")]
        [StringLength(20)]
        public string? NGAY_TSDB { get; set; }

        [Column("GIATRI_TSDB")]
        public decimal? GIATRI_TSDB { get; set; }

        [Column("GIATRI_TSDB_USD")]
        public decimal? GIATRI_TSDB_USD { get; set; }

        [Column("NHOM_NO")]
        [StringLength(10)]
        public string? NHOM_NO { get; set; }

        [Column("PHAN_LOAI")]
        [StringLength(50)]
        public string? PHAN_LOAI { get; set; }

        [Column("LOAI_HINH")]
        [StringLength(50)]
        public string? LOAI_HINH { get; set; }

        [Column("MUCDO_RUIRO")]
        [StringLength(20)]
        public string? MUCDO_RUIRO { get; set; }

        [Column("TYLAI_SUAT")]
        public decimal? TYLAI_SUAT { get; set; }

        [Column("DANHGIA_KHACHHANG")]
        [StringLength(50)]
        public string? DANHGIA_KHACHHANG { get; set; }

        [Column("GHICHU")]
        [StringLength(500)]
        public string? GHICHU { get; set; }

        [Column("NGAYBAOCAO")]
        [StringLength(20)]
        public string? NGAYBAOCAO { get; set; }

        // === TEMPORAL COLUMNS ===
        [Column("CREATED_DATE")]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        [Column("UPDATED_DATE")]
        public DateTime? UPDATED_DATE { get; set; }

        [Column("FILE_NAME")]
        [StringLength(255)]
        public string? FILE_NAME { get; set; }
    }
}
EOF

# === 4. TSDB01 MODEL (16 cột) ===
echo "🔧 Tạo TSDB01.cs với 16 cột..."
cat > $MODELS_DIR/TSDB01.cs << 'EOF'
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng TSDB01 - Tài sản đảm bảo (16 cột theo header)
    /// Cấu trúc theo header_7800_tsdb01_20250430.csv
    /// </summary>
    [Table("TSDB01")]
    public class TSDB01
    {
        [Key]
        public int Id { get; set; }

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        // === CỘT THEO HEADER CSV GỐC ===
        [Column("MA_CN")]
        [StringLength(20)]
        public string? MA_CN { get; set; }

        [Column("MA_KH")]
        [StringLength(50)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH")]
        [StringLength(255)]
        public string? TEN_KH { get; set; }

        [Column("SOHD")]
        [StringLength(50)]
        public string? SOHD { get; set; }

        [Column("LOAI_TSDB")]
        [StringLength(50)]
        public string? LOAI_TSDB { get; set; }

        [Column("TEN_TSDB")]
        [StringLength(255)]
        public string? TEN_TSDB { get; set; }

        [Column("DIACHI_TSDB")]
        [StringLength(500)]
        public string? DIACHI_TSDB { get; set; }

        [Column("NGAY_DINHGIA")]
        [StringLength(20)]
        public string? NGAY_DINHGIA { get; set; }

        [Column("GIATRI_DINHGIA")]
        public decimal? GIATRI_DINHGIA { get; set; }

        [Column("GIATRI_DINHGIA_USD")]
        public decimal? GIATRI_DINHGIA_USD { get; set; }

        [Column("TYLE_CHODAM")]
        public decimal? TYLE_CHODAM { get; set; }

        [Column("GIATRI_CHODAM")]
        public decimal? GIATRI_CHODAM { get; set; }

        [Column("GIATRI_CHODAM_USD")]
        public decimal? GIATRI_CHODAM_USD { get; set; }

        [Column("TRANGBAI")]
        [StringLength(50)]
        public string? TRANGBAI { get; set; }

        [Column("GHICHU")]
        [StringLength(500)]
        public string? GHICHU { get; set; }

        [Column("NGAYBAOCAO")]
        [StringLength(20)]
        public string? NGAYBAOCAO { get; set; }

        // === TEMPORAL COLUMNS ===
        [Column("CREATED_DATE")]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        [Column("UPDATED_DATE")]
        public DateTime? UPDATED_DATE { get; set; }

        [Column("FILE_NAME")]
        [StringLength(255)]
        public string? FILE_NAME { get; set; }
    }
}
EOF

echo "✅ Đã tạo thành công các model còn lại:"
echo "   - LN02: 11 cột + temporal"
echo "   - LN03: 17 cột + temporal"
echo "   - RR01: 25 cột + temporal"
echo "   - TSDB01: 16 cột + temporal"

echo "📋 Kiểm tra file đã tạo:"
ls -la $MODELS_DIR/LN02.cs $MODELS_DIR/LN03.cs $MODELS_DIR/RR01.cs $MODELS_DIR/TSDB01.cs

echo "🔍 Đếm số dòng model (để verify):"
wc -l $MODELS_DIR/LN02.cs $MODELS_DIR/LN03.cs $MODELS_DIR/RR01.cs $MODELS_DIR/TSDB01.cs

echo "🎉 Hoàn thành tạo tất cả model theo header CSV gốc!"
