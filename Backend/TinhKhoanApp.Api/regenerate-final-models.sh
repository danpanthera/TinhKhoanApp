#!/bin/bash

# ===================================================================
# SCRIPT TẠO LẠI HOÀN TOÀN TẤT CẢ MODEL THEO HEADER CSV GỐC
# Đảm bảo 100% chính xác với header CSV
# ===================================================================

echo "🔧 Tạo lại HOÀN TOÀN tất cả model theo header CSV gốc..."

MODELS_DIR="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/Models/DataTables"

# Backup toàn bộ
mkdir -p "${MODELS_DIR}_backup_final_$(date +%Y%m%d_%H%M%S)"
cp -r $MODELS_DIR/* "${MODELS_DIR}_backup_final_$(date +%Y%m%d_%H%M%S)/" 2>/dev/null || true

# === 1. DPDA MODEL - CHÍNH XÁC 13 CỘT ===
echo "🔧 Tạo DPDA.cs với chính xác 13 cột theo header..."
cat > $MODELS_DIR/DPDA.cs << 'EOF'
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng DPDA - 13 cột theo header_7800_dpda_20250430.csv
    /// MA_CHI_NHANH,MA_KHACH_HANG,TEN_KHACH_HANG,SO_TAI_KHOAN,LOAI_THE,SO_THE,NGAY_NOP_DON,NGAY_PHAT_HANH,USER_PHAT_HANH,TRANG_THAI,PHAN_LOAI,GIAO_THE,LOAI_PHAT_HANH
    /// </summary>
    [Table("DPDA")]
    public class DPDA
    {
        [Key]
        public int Id { get; set; }

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        // === 13 CỘT THEO HEADER CSV GỐC ===
        [Column("MA_CHI_NHANH")]
        [StringLength(50)]
        public string? MA_CHI_NHANH { get; set; }

        [Column("MA_KHACH_HANG")]
        [StringLength(50)]
        public string? MA_KHACH_HANG { get; set; }

        [Column("TEN_KHACH_HANG")]
        [StringLength(255)]
        public string? TEN_KHACH_HANG { get; set; }

        [Column("SO_TAI_KHOAN")]
        [StringLength(50)]
        public string? SO_TAI_KHOAN { get; set; }

        [Column("LOAI_THE")]
        [StringLength(50)]
        public string? LOAI_THE { get; set; }

        [Column("SO_THE")]
        [StringLength(50)]
        public string? SO_THE { get; set; }

        [Column("NGAY_NOP_DON")]
        [StringLength(20)]
        public string? NGAY_NOP_DON { get; set; }

        [Column("NGAY_PHAT_HANH")]
        [StringLength(20)]
        public string? NGAY_PHAT_HANH { get; set; }

        [Column("USER_PHAT_HANH")]
        [StringLength(100)]
        public string? USER_PHAT_HANH { get; set; }

        [Column("TRANG_THAI")]
        [StringLength(50)]
        public string? TRANG_THAI { get; set; }

        [Column("PHAN_LOAI")]
        [StringLength(50)]
        public string? PHAN_LOAI { get; set; }

        [Column("GIAO_THE")]
        [StringLength(50)]
        public string? GIAO_THE { get; set; }

        [Column("LOAI_PHAT_HANH")]
        [StringLength(50)]
        public string? LOAI_PHAT_HANH { get; set; }

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

# === 2. EI01 MODEL - CHÍNH XÁC 24 CỘT ===
echo "🔧 Tạo EI01.cs với chính xác 24 cột theo header..."
cat > $MODELS_DIR/EI01.cs << 'EOF'
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng EI01 - 24 cột theo header_7800_ei01_20250430.csv
    /// MA_CN,MA_KH,TEN_KH,LOAI_KH,SDT_EMB,TRANG_THAI_EMB,NGAY_DK_EMB,SDT_OTT,TRANG_THAI_OTT,NGAY_DK_OTT,SDT_SMS,TRANG_THAI_SMS,NGAY_DK_SMS,SDT_SAV,TRANG_THAI_SAV,NGAY_DK_SAV,SDT_LN,TRANG_THAI_LN,NGAY_DK_LN,USER_EMB,USER_OTT,USER_SMS,USER_SAV,USER_LN
    /// </summary>
    [Table("EI01")]
    public class EI01
    {
        [Key]
        public int Id { get; set; }

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        // === 24 CỘT THEO HEADER CSV GỐC ===
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

        [Column("SDT_EMB")]
        [StringLength(20)]
        public string? SDT_EMB { get; set; }

        [Column("TRANG_THAI_EMB")]
        [StringLength(50)]
        public string? TRANG_THAI_EMB { get; set; }

        [Column("NGAY_DK_EMB")]
        [StringLength(20)]
        public string? NGAY_DK_EMB { get; set; }

        [Column("SDT_OTT")]
        [StringLength(20)]
        public string? SDT_OTT { get; set; }

        [Column("TRANG_THAI_OTT")]
        [StringLength(50)]
        public string? TRANG_THAI_OTT { get; set; }

        [Column("NGAY_DK_OTT")]
        [StringLength(20)]
        public string? NGAY_DK_OTT { get; set; }

        [Column("SDT_SMS")]
        [StringLength(20)]
        public string? SDT_SMS { get; set; }

        [Column("TRANG_THAI_SMS")]
        [StringLength(50)]
        public string? TRANG_THAI_SMS { get; set; }

        [Column("NGAY_DK_SMS")]
        [StringLength(20)]
        public string? NGAY_DK_SMS { get; set; }

        [Column("SDT_SAV")]
        [StringLength(20)]
        public string? SDT_SAV { get; set; }

        [Column("TRANG_THAI_SAV")]
        [StringLength(50)]
        public string? TRANG_THAI_SAV { get; set; }

        [Column("NGAY_DK_SAV")]
        [StringLength(20)]
        public string? NGAY_DK_SAV { get; set; }

        [Column("SDT_LN")]
        [StringLength(20)]
        public string? SDT_LN { get; set; }

        [Column("TRANG_THAI_LN")]
        [StringLength(50)]
        public string? TRANG_THAI_LN { get; set; }

        [Column("NGAY_DK_LN")]
        [StringLength(20)]
        public string? NGAY_DK_LN { get; set; }

        [Column("USER_EMB")]
        [StringLength(100)]
        public string? USER_EMB { get; set; }

        [Column("USER_OTT")]
        [StringLength(100)]
        public string? USER_OTT { get; set; }

        [Column("USER_SMS")]
        [StringLength(100)]
        public string? USER_SMS { get; set; }

        [Column("USER_SAV")]
        [StringLength(100)]
        public string? USER_SAV { get; set; }

        [Column("USER_LN")]
        [StringLength(100)]
        public string? USER_LN { get; set; }

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

# === 3. GL01 MODEL - CHÍNH XÁC 27 CỘT ===
echo "🔧 Tạo GL01.cs với chính xác 27 cột theo header..."
cat > $MODELS_DIR/GL01.cs << 'EOF'
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng GL01 - 27 cột theo header_7800_gl01_2025050120250531.csv
    /// STS,NGAY_GD,NGUOI_TAO,DYSEQ,TR_TYPE,DT_SEQ,TAI_KHOAN,TEN_TK,SO_TIEN_GD,POST_BR,LOAI_TIEN,DR_CR,MA_KH,TEN_KH,CCA_USRID,TR_EX_RT,REMARK,BUS_CODE,UNIT_BUS_CODE,TR_CODE,TR_NAME,REFERENCE,VALUE_DATE,DEPT_CODE,TR_TIME,COMFIRM,TRDT_TIME
    /// </summary>
    [Table("GL01")]
    public class GL01
    {
        [Key]
        public int Id { get; set; }

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        // === 27 CỘT THEO HEADER CSV GỐC ===
        [Column("STS")]
        [StringLength(20)]
        public string? STS { get; set; }

        [Column("NGAY_GD")]
        [StringLength(20)]
        public string? NGAY_GD { get; set; }

        [Column("NGUOI_TAO")]
        [StringLength(100)]
        public string? NGUOI_TAO { get; set; }

        [Column("DYSEQ")]
        [StringLength(50)]
        public string? DYSEQ { get; set; }

        [Column("TR_TYPE")]
        [StringLength(20)]
        public string? TR_TYPE { get; set; }

        [Column("DT_SEQ")]
        [StringLength(50)]
        public string? DT_SEQ { get; set; }

        [Column("TAI_KHOAN")]
        [StringLength(50)]
        public string? TAI_KHOAN { get; set; }

        [Column("TEN_TK")]
        [StringLength(255)]
        public string? TEN_TK { get; set; }

        [Column("SO_TIEN_GD")]
        public decimal? SO_TIEN_GD { get; set; }

        [Column("POST_BR")]
        [StringLength(20)]
        public string? POST_BR { get; set; }

        [Column("LOAI_TIEN")]
        [StringLength(10)]
        public string? LOAI_TIEN { get; set; }

        [Column("DR_CR")]
        [StringLength(10)]
        public string? DR_CR { get; set; }

        [Column("MA_KH")]
        [StringLength(50)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH")]
        [StringLength(255)]
        public string? TEN_KH { get; set; }

        [Column("CCA_USRID")]
        [StringLength(50)]
        public string? CCA_USRID { get; set; }

        [Column("TR_EX_RT")]
        public decimal? TR_EX_RT { get; set; }

        [Column("REMARK")]
        [StringLength(500)]
        public string? REMARK { get; set; }

        [Column("BUS_CODE")]
        [StringLength(20)]
        public string? BUS_CODE { get; set; }

        [Column("UNIT_BUS_CODE")]
        [StringLength(20)]
        public string? UNIT_BUS_CODE { get; set; }

        [Column("TR_CODE")]
        [StringLength(20)]
        public string? TR_CODE { get; set; }

        [Column("TR_NAME")]
        [StringLength(255)]
        public string? TR_NAME { get; set; }

        [Column("REFERENCE")]
        [StringLength(100)]
        public string? REFERENCE { get; set; }

        [Column("VALUE_DATE")]
        [StringLength(20)]
        public string? VALUE_DATE { get; set; }

        [Column("DEPT_CODE")]
        [StringLength(20)]
        public string? DEPT_CODE { get; set; }

        [Column("TR_TIME")]
        [StringLength(20)]
        public string? TR_TIME { get; set; }

        [Column("COMFIRM")]
        [StringLength(20)]
        public string? COMFIRM { get; set; }

        [Column("TRDT_TIME")]
        [StringLength(20)]
        public string? TRDT_TIME { get; set; }

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

echo "✅ Đã tạo xong DPDA (13+3), EI01 (24+3), GL01 (27+3) columns"
echo "🔍 Verify số cột:"
echo "DPDA: $(grep -c "public.*{.*get.*set.*}" $MODELS_DIR/DPDA.cs) columns"
echo "EI01: $(grep -c "public.*{.*get.*set.*}" $MODELS_DIR/EI01.cs) columns"
echo "GL01: $(grep -c "public.*{.*get.*set.*}" $MODELS_DIR/GL01.cs) columns"

echo "🎉 Hoàn thành tạo lại model chính xác!"
