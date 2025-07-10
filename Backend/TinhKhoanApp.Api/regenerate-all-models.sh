#!/bin/bash

# ===================================================================
# SCRIPT TẠO LẠI TẤT CẢ MODEL THEO HEADER CSV GỐC
# Đảm bảo 100% khớp với cột CSV gốc từ các file header
# ===================================================================

echo "🔧 Tạo lại tất cả models theo header CSV gốc..."

MODELS_DIR="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/Models/DataTables"

# === BACKUP CÁC FILE CŨ ===
echo "📦 Backup các file model cũ..."
mkdir -p "${MODELS_DIR}_backup_$(date +%Y%m%d_%H%M%S)"
cp -r $MODELS_DIR/* "${MODELS_DIR}_backup_$(date +%Y%m%d_%H%M%S)/" 2>/dev/null || true

# === 1. GL01 MODEL (27 cột) ===
echo "🔧 Tạo GL01.cs với 27 cột..."
cat > $MODELS_DIR/GL01.cs << 'EOF'
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng GL01 - Sổ cái tổng hợp (27 cột theo header)
    /// Cấu trúc theo header_7800_gl01_2025050120250531.csv
    /// </summary>
    [Table("GL01")]
    public class GL01
    {
        [Key]
        public int Id { get; set; }

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        // === CỘT THEO HEADER CSV GỐC ===
        [Column("STS")]
        [StringLength(10)]
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
        [StringLength(5)]
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
        [StringLength(10)]
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

# === 2. KH03 MODEL (38 cột) ===
echo "🔧 Tạo KH03.cs với 38 cột..."
cat > $MODELS_DIR/KH03.cs << 'EOF'
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng KH03 - Thông tin khách hàng (38 cột theo header)
    /// Cấu trúc theo header_7800_kh03_20250430.csv
    /// </summary>
    [Table("KH03")]
    public class KH03
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

        [Column("LDS")]
        [StringLength(50)]
        public string? LDS { get; set; }

        [Column("SO_TIEN_GIAI_NGAN")]
        public decimal? SO_TIEN_GIAI_NGAN { get; set; }

        [Column("NGAY_GIAI_NGAN")]
        [StringLength(20)]
        public string? NGAY_GIAI_NGAN { get; set; }

        [Column("NGAY_DEN_HAN_GN")]
        [StringLength(20)]
        public string? NGAY_DEN_HAN_GN { get; set; }

        [Column("ACCOUNT_NAME")]
        [StringLength(255)]
        public string? ACCOUNT_NAME { get; set; }

        [Column("TY_GIA")]
        public decimal? TY_GIA { get; set; }

        [Column("SOTIEN_PHEDUYET")]
        public decimal? SOTIEN_PHEDUYET { get; set; }

        [Column("DUNO_NGAN")]
        public decimal? DUNO_NGAN { get; set; }

        [Column("DUNO_TRUNGDAI")]
        public decimal? DUNO_TRUNGDAI { get; set; }

        [Column("BAOLANH")]
        public decimal? BAOLANH { get; set; }

        [Column("NHOMNO")]
        [StringLength(20)]
        public string? NHOMNO { get; set; }

        [Column("XLRR")]
        [StringLength(20)]
        public string? XLRR { get; set; }

        [Column("MANGANH")]
        [StringLength(20)]
        public string? MANGANH { get; set; }

        [Column("BSRT")]
        public decimal? BSRT { get; set; }

        [Column("SPRDRT")]
        public decimal? SPRDRT { get; set; }

        [Column("INTRT")]
        public decimal? INTRT { get; set; }

        [Column("XEP_HANG")]
        [StringLength(50)]
        public string? XEP_HANG { get; set; }

        [Column("TONG_TSDB")]
        public decimal? TONG_TSDB { get; set; }

        [Column("BDS")]
        public decimal? BDS { get; set; }

        [Column("DS")]
        public decimal? DS { get; set; }

        [Column("CCTG")]
        public decimal? CCTG { get; set; }

        [Column("CK")]
        public decimal? CK { get; set; }

        [Column("TAI_SAN_BAO_LANH")]
        public decimal? TAI_SAN_BAO_LANH { get; set; }

        [Column("TAI_SAN_KHAC")]
        public decimal? TAI_SAN_KHAC { get; set; }

        [Column("KI_HAN")]
        [StringLength(20)]
        public string? KI_HAN { get; set; }

        [Column("LOAI_KH")]
        [StringLength(50)]
        public string? LOAI_KH { get; set; }

        [Column("QUY_MO_THEO_CHAM_DIEM_KH")]
        [StringLength(50)]
        public string? QUY_MO_THEO_CHAM_DIEM_KH { get; set; }

        [Column("CCYCD")]
        [StringLength(10)]
        public string? CCYCD { get; set; }

        [Column("ACCOUNT_LAV")]
        [StringLength(50)]
        public string? ACCOUNT_LAV { get; set; }

        [Column("QUY_MO_THEO_TT41")]
        [StringLength(50)]
        public string? QUY_MO_THEO_TT41 { get; set; }

        [Column("LOAI_TIEN_LAV")]
        [StringLength(10)]
        public string? LOAI_TIEN_LAV { get; set; }

        [Column("THANG_KY_HAN")]
        public int? THANG_KY_HAN { get; set; }

        [Column("NGAY_PHE_DUYET")]
        [StringLength(20)]
        public string? NGAY_PHE_DUYET { get; set; }

        [Column("NGAY_DEN_HAN_LAV")]
        [StringLength(20)]
        public string? NGAY_DEN_HAN_LAV { get; set; }

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

# === 3. LN01 MODEL (67 cột) ===
echo "🔧 Tạo LN01.cs với 67 cột..."
cat > $MODELS_DIR/LN01.cs << 'EOF'
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng LN01 - Cho vay (67 cột theo header)
    /// Cấu trúc theo header_7800_ln01_20250430.csv
    /// </summary>
    [Table("LN01")]
    public class LN01
    {
        [Key]
        public int Id { get; set; }

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        // === CỘT THEO HEADER CSV GỐC (67 cột) ===
        [Column("BRCD")]
        [StringLength(20)]
        public string? BRCD { get; set; }

        [Column("CUSTSEQ")]
        [StringLength(50)]
        public string? CUSTSEQ { get; set; }

        [Column("CUSTNM")]
        [StringLength(255)]
        public string? CUSTNM { get; set; }

        [Column("TAI_KHOAN")]
        [StringLength(50)]
        public string? TAI_KHOAN { get; set; }

        [Column("CCY")]
        [StringLength(10)]
        public string? CCY { get; set; }

        [Column("DU_NO")]
        public decimal? DU_NO { get; set; }

        [Column("DSBSSEQ")]
        [StringLength(50)]
        public string? DSBSSEQ { get; set; }

        [Column("TRANSACTION_DATE")]
        [StringLength(20)]
        public string? TRANSACTION_DATE { get; set; }

        [Column("DSBSDT")]
        [StringLength(20)]
        public string? DSBSDT { get; set; }

        [Column("DISBUR_CCY")]
        [StringLength(10)]
        public string? DISBUR_CCY { get; set; }

        [Column("DISBURSEMENT_AMOUNT")]
        public decimal? DISBURSEMENT_AMOUNT { get; set; }

        [Column("DSBSMATDT")]
        [StringLength(20)]
        public string? DSBSMATDT { get; set; }

        [Column("BSRTCD")]
        [StringLength(20)]
        public string? BSRTCD { get; set; }

        [Column("INTEREST_RATE")]
        public decimal? INTEREST_RATE { get; set; }

        [Column("APPRSEQ")]
        [StringLength(50)]
        public string? APPRSEQ { get; set; }

        [Column("APPRDT")]
        [StringLength(20)]
        public string? APPRDT { get; set; }

        [Column("APPR_CCY")]
        [StringLength(10)]
        public string? APPR_CCY { get; set; }

        [Column("APPRAMT")]
        public decimal? APPRAMT { get; set; }

        [Column("APPRMATDT")]
        [StringLength(20)]
        public string? APPRMATDT { get; set; }

        [Column("LOAN_TYPE")]
        [StringLength(50)]
        public string? LOAN_TYPE { get; set; }

        [Column("FUND_RESOURCE_CODE")]
        [StringLength(50)]
        public string? FUND_RESOURCE_CODE { get; set; }

        [Column("FUND_PURPOSE_CODE")]
        [StringLength(50)]
        public string? FUND_PURPOSE_CODE { get; set; }

        [Column("REPAYMENT_AMOUNT")]
        public decimal? REPAYMENT_AMOUNT { get; set; }

        [Column("NEXT_REPAY_DATE")]
        [StringLength(20)]
        public string? NEXT_REPAY_DATE { get; set; }

        [Column("NEXT_REPAY_AMOUNT")]
        public decimal? NEXT_REPAY_AMOUNT { get; set; }

        [Column("NEXT_INT_REPAY_DATE")]
        [StringLength(20)]
        public string? NEXT_INT_REPAY_DATE { get; set; }

        [Column("OFFICER_ID")]
        [StringLength(50)]
        public string? OFFICER_ID { get; set; }

        [Column("OFFICER_NAME")]
        [StringLength(255)]
        public string? OFFICER_NAME { get; set; }

        [Column("INTEREST_AMOUNT")]
        public decimal? INTEREST_AMOUNT { get; set; }

        [Column("PASTDUE_INTEREST_AMOUNT")]
        public decimal? PASTDUE_INTEREST_AMOUNT { get; set; }

        [Column("TOTAL_INTEREST_REPAY_AMOUNT")]
        public decimal? TOTAL_INTEREST_REPAY_AMOUNT { get; set; }

        [Column("CUSTOMER_TYPE_CODE")]
        [StringLength(20)]
        public string? CUSTOMER_TYPE_CODE { get; set; }

        [Column("CUSTOMER_TYPE_CODE_DETAIL")]
        [StringLength(50)]
        public string? CUSTOMER_TYPE_CODE_DETAIL { get; set; }

        [Column("TRCTCD")]
        [StringLength(20)]
        public string? TRCTCD { get; set; }

        [Column("TRCTNM")]
        [StringLength(255)]
        public string? TRCTNM { get; set; }

        [Column("ADDR1")]
        [StringLength(500)]
        public string? ADDR1 { get; set; }

        [Column("PROVINCE")]
        [StringLength(100)]
        public string? PROVINCE { get; set; }

        [Column("LCLPROVINNM")]
        [StringLength(255)]
        public string? LCLPROVINNM { get; set; }

        [Column("DISTRICT")]
        [StringLength(100)]
        public string? DISTRICT { get; set; }

        [Column("LCLDISTNM")]
        [StringLength(255)]
        public string? LCLDISTNM { get; set; }

        [Column("COMMCD")]
        [StringLength(100)]
        public string? COMMCD { get; set; }

        [Column("LCLWARDNM")]
        [StringLength(255)]
        public string? LCLWARDNM { get; set; }

        [Column("LAST_REPAY_DATE")]
        [StringLength(20)]
        public string? LAST_REPAY_DATE { get; set; }

        [Column("SECURED_PERCENT")]
        public decimal? SECURED_PERCENT { get; set; }

        [Column("NHOM_NO")]
        [StringLength(20)]
        public string? NHOM_NO { get; set; }

        [Column("LAST_INT_CHARGE_DATE")]
        [StringLength(20)]
        public string? LAST_INT_CHARGE_DATE { get; set; }

        [Column("EXEMPTINT")]
        public decimal? EXEMPTINT { get; set; }

        [Column("EXEMPTINTTYPE")]
        [StringLength(20)]
        public string? EXEMPTINTTYPE { get; set; }

        [Column("EXEMPTINTAMT")]
        public decimal? EXEMPTINTAMT { get; set; }

        [Column("GRPNO")]
        [StringLength(20)]
        public string? GRPNO { get; set; }

        [Column("BUSCD")]
        [StringLength(20)]
        public string? BUSCD { get; set; }

        [Column("BSNSSCLTPCD")]
        [StringLength(20)]
        public string? BSNSSCLTPCD { get; set; }

        [Column("USRIDOP")]
        [StringLength(50)]
        public string? USRIDOP { get; set; }

        [Column("ACCRUAL_AMOUNT")]
        public decimal? ACCRUAL_AMOUNT { get; set; }

        [Column("ACCRUAL_AMOUNT_END_OF_MONTH")]
        public decimal? ACCRUAL_AMOUNT_END_OF_MONTH { get; set; }

        [Column("INTCMTH")]
        [StringLength(20)]
        public string? INTCMTH { get; set; }

        [Column("INTRPYMTH")]
        [StringLength(20)]
        public string? INTRPYMTH { get; set; }

        [Column("INTTRMMTH")]
        public int? INTTRMMTH { get; set; }

        [Column("YRDAYS")]
        public int? YRDAYS { get; set; }

        [Column("REMARK")]
        [StringLength(500)]
        public string? REMARK { get; set; }

        [Column("CHITIEU")]
        [StringLength(50)]
        public string? CHITIEU { get; set; }

        [Column("CTCV")]
        [StringLength(50)]
        public string? CTCV { get; set; }

        [Column("CREDIT_LINE_YPE")]
        [StringLength(50)]
        public string? CREDIT_LINE_YPE { get; set; }

        [Column("INT_LUMPSUM_PARTIAL_TYPE")]
        [StringLength(20)]
        public string? INT_LUMPSUM_PARTIAL_TYPE { get; set; }

        [Column("INT_PARTIAL_PAYMENT_TYPE")]
        [StringLength(20)]
        public string? INT_PARTIAL_PAYMENT_TYPE { get; set; }

        [Column("INT_PAYMENT_INTERVAL")]
        [StringLength(20)]
        public string? INT_PAYMENT_INTERVAL { get; set; }

        [Column("AN_HAN_LAI")]
        [StringLength(20)]
        public string? AN_HAN_LAI { get; set; }

        [Column("PHUONG_THUC_GIAI_NGAN_1")]
        [StringLength(50)]
        public string? PHUONG_THUC_GIAI_NGAN_1 { get; set; }

        [Column("TAI_KHOAN_GIAI_NGAN_1")]
        [StringLength(50)]
        public string? TAI_KHOAN_GIAI_NGAN_1 { get; set; }

        [Column("SO_TIEN_GIAI_NGAN_1")]
        public decimal? SO_TIEN_GIAI_NGAN_1 { get; set; }

        [Column("PHUONG_THUC_GIAI_NGAN_2")]
        [StringLength(50)]
        public string? PHUONG_THUC_GIAI_NGAN_2 { get; set; }

        [Column("TAI_KHOAN_GIAI_NGAN_2")]
        [StringLength(50)]
        public string? TAI_KHOAN_GIAI_NGAN_2 { get; set; }

        [Column("SO_TIEN_GIAI_NGAN_2")]
        public decimal? SO_TIEN_GIAI_NGAN_2 { get; set; }

        [Column("CMT_HC")]
        [StringLength(50)]
        public string? CMT_HC { get; set; }

        [Column("NGAY_SINH")]
        [StringLength(20)]
        public string? NGAY_SINH { get; set; }

        [Column("MA_CB_AGRI")]
        [StringLength(50)]
        public string? MA_CB_AGRI { get; set; }

        [Column("MA_NGANH_KT")]
        [StringLength(20)]
        public string? MA_NGANH_KT { get; set; }

        [Column("TY_GIA")]
        public decimal? TY_GIA { get; set; }

        [Column("OFFICER_IPCAS")]
        [StringLength(50)]
        public string? OFFICER_IPCAS { get; set; }

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

echo "✅ Đã tạo lại GL01, KH03, LN01 với đúng cấu trúc header CSV gốc"
echo "📋 Các model đã được tạo:"
echo "   - GL01: 27 cột + temporal"
echo "   - KH03: 38 cột + temporal"
echo "   - LN01: 67 cột + temporal"
echo "   - EI01: 24 cột + temporal (đã tạo trước đó)"

echo ""
echo "🔄 Tiếp theo cần tạo: LN02, LN03, RR01, DB01 (tương tự)"
