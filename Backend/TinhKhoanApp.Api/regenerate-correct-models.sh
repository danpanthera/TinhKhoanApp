#!/bin/bash

# ===================================================================
# SCRIPT TẠO LẠI MODEL THEO ĐÚNG HEADER CSV GỐC - CHÍNH XÁC 100%
# Chỉ giữ lại các cột từ header CSV + temporal columns
# ===================================================================

echo "🔧 Tạo lại model theo CHÍNH XÁC header CSV gốc..."

MODELS_DIR="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/Models/DataTables"

# Backup trước khi regenerate
mkdir -p "${MODELS_DIR}_backup_correct_$(date +%Y%m%d_%H%M%S)"
cp -r $MODELS_DIR/* "${MODELS_DIR}_backup_correct_$(date +%Y%m%d_%H%M%S)/" 2>/dev/null || true

# === LN02 MODEL - CHÍNH XÁC 11 CỘT ===
echo "🔧 Tạo LN02.cs với chính xác 11 cột theo header..."
cat > $MODELS_DIR/LN02.cs << 'EOF'
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng LN02 - 11 cột theo header_7800_ln02_20250430.csv
    /// TENCHINHANH,MAKHACHHANG,TENKHACHHANG,NGAYCHUYENNHOMNO,DUNO,NHOMNOMOI,NHOMNOCU,NGUYENNHAN,MACANBO,TENCANBO,MANGANH
    /// </summary>
    [Table("LN02")]
    public class LN02
    {
        [Key]
        public int Id { get; set; }

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        // === 11 CỘT THEO HEADER CSV GỐC ===
        [Column("TENCHINHANH")]
        [StringLength(255)]
        public string? TENCHINHANH { get; set; }

        [Column("MAKHACHHANG")]
        [StringLength(50)]
        public string? MAKHACHHANG { get; set; }

        [Column("TENKHACHHANG")]
        [StringLength(255)]
        public string? TENKHACHHANG { get; set; }

        [Column("NGAYCHUYENNHOMNO")]
        [StringLength(20)]
        public string? NGAYCHUYENNHOMNO { get; set; }

        [Column("DUNO")]
        public decimal? DUNO { get; set; }

        [Column("NHOMNOMOI")]
        [StringLength(20)]
        public string? NHOMNOMOI { get; set; }

        [Column("NHOMNOCU")]
        [StringLength(20)]
        public string? NHOMNOCU { get; set; }

        [Column("NGUYENNHAN")]
        [StringLength(500)]
        public string? NGUYENNHAN { get; set; }

        [Column("MACANBO")]
        [StringLength(50)]
        public string? MACANBO { get; set; }

        [Column("TENCANBO")]
        [StringLength(255)]
        public string? TENCANBO { get; set; }

        [Column("MANGANH")]
        [StringLength(50)]
        public string? MANGANH { get; set; }

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

# === LN03 MODEL - CHÍNH XÁC 17 CỘT ===
echo "🔧 Tạo LN03.cs với chính xác 17 cột theo header..."
cat > $MODELS_DIR/LN03.cs << 'EOF'
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng LN03 - 17 cột theo header_7800_ln03_20250430.csv
    /// MACHINHANH,TENCHINHANH,MAKH,TENKH,SOHOPDONG,SOTIENXLRR,NGAYPHATSINHXL,THUNOSAUXL,CONLAINGOAIBANG,DUNONOIBANG,NHOMNO,MACBTD,TENCBTD,MAPGD,TAIKHOANHACHTOAN,REFNO,LOAINGUONVON
    /// </summary>
    [Table("LN03")]
    public class LN03
    {
        [Key]
        public int Id { get; set; }

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        // === 17 CỘT THEO HEADER CSV GỐC ===
        [Column("MACHINHANH")]
        [StringLength(50)]
        public string? MACHINHANH { get; set; }

        [Column("TENCHINHANH")]
        [StringLength(255)]
        public string? TENCHINHANH { get; set; }

        [Column("MAKH")]
        [StringLength(50)]
        public string? MAKH { get; set; }

        [Column("TENKH")]
        [StringLength(255)]
        public string? TENKH { get; set; }

        [Column("SOHOPDONG")]
        [StringLength(50)]
        public string? SOHOPDONG { get; set; }

        [Column("SOTIENXLRR")]
        public decimal? SOTIENXLRR { get; set; }

        [Column("NGAYPHATSINHXL")]
        [StringLength(20)]
        public string? NGAYPHATSINHXL { get; set; }

        [Column("THUNOSAUXL")]
        public decimal? THUNOSAUXL { get; set; }

        [Column("CONLAINGOAIBANG")]
        public decimal? CONLAINGOAIBANG { get; set; }

        [Column("DUNONOIBANG")]
        public decimal? DUNONOIBANG { get; set; }

        [Column("NHOMNO")]
        [StringLength(20)]
        public string? NHOMNO { get; set; }

        [Column("MACBTD")]
        [StringLength(50)]
        public string? MACBTD { get; set; }

        [Column("TENCBTD")]
        [StringLength(255)]
        public string? TENCBTD { get; set; }

        [Column("MAPGD")]
        [StringLength(50)]
        public string? MAPGD { get; set; }

        [Column("TAIKHOANHACHTOAN")]
        [StringLength(50)]
        public string? TAIKHOANHACHTOAN { get; set; }

        [Column("REFNO")]
        [StringLength(50)]
        public string? REFNO { get; set; }

        [Column("LOAINGUONVON")]
        [StringLength(50)]
        public string? LOAINGUONVON { get; set; }

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

# === RR01 MODEL - CHÍNH XÁC 25 CỘT ===
echo "🔧 Tạo RR01.cs với chính xác 25 cột theo header..."
cat > $MODELS_DIR/RR01.cs << 'EOF'
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng RR01 - 25 cột theo header_7800_rr01_20250430.csv
    /// CN_LOAI_I,BRCD,MA_KH,TEN_KH,SO_LDS,CCY,SO_LAV,LOAI_KH,NGAY_GIAI_NGAN,NGAY_DEN_HAN,VAMC_FLG,NGAY_XLRR,DUNO_GOC_BAN_DAU,DUNO_LAI_TICHLUY_BD,DOC_DAUKY_DA_THU_HT,DUNO_GOC_HIENTAI,DUNO_LAI_HIENTAI,DUNO_NGAN_HAN,DUNO_TRUNG_HAN,DUNO_DAI_HAN,THU_GOC,THU_LAI,BDS,DS,TSK
    /// </summary>
    [Table("RR01")]
    public class RR01
    {
        [Key]
        public int Id { get; set; }

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        // === 25 CỘT THEO HEADER CSV GỐC ===
        [Column("CN_LOAI_I")]
        [StringLength(50)]
        public string? CN_LOAI_I { get; set; }

        [Column("BRCD")]
        [StringLength(50)]
        public string? BRCD { get; set; }

        [Column("MA_KH")]
        [StringLength(50)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH")]
        [StringLength(255)]
        public string? TEN_KH { get; set; }

        [Column("SO_LDS")]
        [StringLength(50)]
        public string? SO_LDS { get; set; }

        [Column("CCY")]
        [StringLength(10)]
        public string? CCY { get; set; }

        [Column("SO_LAV")]
        [StringLength(50)]
        public string? SO_LAV { get; set; }

        [Column("LOAI_KH")]
        [StringLength(50)]
        public string? LOAI_KH { get; set; }

        [Column("NGAY_GIAI_NGAN")]
        [StringLength(20)]
        public string? NGAY_GIAI_NGAN { get; set; }

        [Column("NGAY_DEN_HAN")]
        [StringLength(20)]
        public string? NGAY_DEN_HAN { get; set; }

        [Column("VAMC_FLG")]
        [StringLength(10)]
        public string? VAMC_FLG { get; set; }

        [Column("NGAY_XLRR")]
        [StringLength(20)]
        public string? NGAY_XLRR { get; set; }

        [Column("DUNO_GOC_BAN_DAU")]
        public decimal? DUNO_GOC_BAN_DAU { get; set; }

        [Column("DUNO_LAI_TICHLUY_BD")]
        public decimal? DUNO_LAI_TICHLUY_BD { get; set; }

        [Column("DOC_DAUKY_DA_THU_HT")]
        public decimal? DOC_DAUKY_DA_THU_HT { get; set; }

        [Column("DUNO_GOC_HIENTAI")]
        public decimal? DUNO_GOC_HIENTAI { get; set; }

        [Column("DUNO_LAI_HIENTAI")]
        public decimal? DUNO_LAI_HIENTAI { get; set; }

        [Column("DUNO_NGAN_HAN")]
        public decimal? DUNO_NGAN_HAN { get; set; }

        [Column("DUNO_TRUNG_HAN")]
        public decimal? DUNO_TRUNG_HAN { get; set; }

        [Column("DUNO_DAI_HAN")]
        public decimal? DUNO_DAI_HAN { get; set; }

        [Column("THU_GOC")]
        public decimal? THU_GOC { get; set; }

        [Column("THU_LAI")]
        public decimal? THU_LAI { get; set; }

        [Column("BDS")]
        public decimal? BDS { get; set; }

        [Column("DS")]
        public decimal? DS { get; set; }

        [Column("TSK")]
        public decimal? TSK { get; set; }

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

# === TSDB01 MODEL - CHÍNH XÁC 16 CỘT ===
echo "🔧 Tạo TSDB01.cs với chính xác 16 cột theo header..."
cat > $MODELS_DIR/TSDB01.cs << 'EOF'
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng TSDB01 - 16 cột theo header_7800_tsdb01_20250430.csv
    /// MA_CN,MA_KH,TEN_KH,LOAI_KH,TONG_DU_NO,VAY_NGAN_HAN,VAY_TRUNG_HAN,VAY_DAI_HAN,DU_NO_KHONG_TSDB,TONG_TSDB,BDS,MAY_MOC,GIAY_TO_CO_GIA,TSDB_KHAC,MA_NGANH_KINH_TE,CHO_VAY_NNNT
    /// </summary>
    [Table("TSDB01")]
    public class TSDB01
    {
        [Key]
        public int Id { get; set; }

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        // === 16 CỘT THEO HEADER CSV GỐC ===
        [Column("MA_CN")]
        [StringLength(50)]
        public string? MA_CN { get; set; }

        [Column("MA_KH")]
        [StringLength(50)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH")]
        [StringLength(255)]
        public string? TEN_KH { get; set; }

        [Column("LOAI_KH")]
        [StringLength(50)]
        public string? LOAI_KH { get; set; }

        [Column("TONG_DU_NO")]
        public decimal? TONG_DU_NO { get; set; }

        [Column("VAY_NGAN_HAN")]
        public decimal? VAY_NGAN_HAN { get; set; }

        [Column("VAY_TRUNG_HAN")]
        public decimal? VAY_TRUNG_HAN { get; set; }

        [Column("VAY_DAI_HAN")]
        public decimal? VAY_DAI_HAN { get; set; }

        [Column("DU_NO_KHONG_TSDB")]
        public decimal? DU_NO_KHONG_TSDB { get; set; }

        [Column("TONG_TSDB")]
        public decimal? TONG_TSDB { get; set; }

        [Column("BDS")]
        public decimal? BDS { get; set; }

        [Column("MAY_MOC")]
        public decimal? MAY_MOC { get; set; }

        [Column("GIAY_TO_CO_GIA")]
        public decimal? GIAY_TO_CO_GIA { get; set; }

        [Column("TSDB_KHAC")]
        public decimal? TSDB_KHAC { get; set; }

        [Column("MA_NGANH_KINH_TE")]
        [StringLength(50)]
        public string? MA_NGANH_KINH_TE { get; set; }

        [Column("CHO_VAY_NNNT")]
        public decimal? CHO_VAY_NNNT { get; set; }

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

echo "✅ Đã tạo xong các model với số cột chính xác theo header CSV gốc:"
echo "   - LN02: 11 cột data + 3 temporal = 14 cột"
echo "   - LN03: 17 cột data + 3 temporal = 20 cột"
echo "   - RR01: 25 cột data + 3 temporal = 28 cột"
echo "   - TSDB01: 16 cột data + 3 temporal = 19 cột"

echo "🔍 Kiểm tra kết quả:"
grep -c "public.*{.*get.*set.*}" $MODELS_DIR/LN02.cs
grep -c "public.*{.*get.*set.*}" $MODELS_DIR/LN03.cs
grep -c "public.*{.*get.*set.*}" $MODELS_DIR/RR01.cs
grep -c "public.*{.*get.*set.*}" $MODELS_DIR/TSDB01.cs

echo "🎉 Hoàn thành regenerate model chính xác!"
