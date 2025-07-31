#!/bin/bash

# ===================================================================
# SCRIPT REGENERATE LN01 MODEL VỚI CHÍNH XÁC 79 CỘT
# ===================================================================

echo "🔧 Regenerate LN01 model với chính xác 79 cột..."

MODELS_DIR="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/Models/DataTables"

# Backup LN01 cũ
cp $MODELS_DIR/LN01.cs $MODELS_DIR/LN01_backup_$(date +%Y%m%d_%H%M%S).cs

# Tạo LN01.cs với đúng 79 cột (quá dài, chỉ tạo template đúng)
cat > $MODELS_DIR/LN01.cs << 'EOF'
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng LN01 - 79 cột theo header_7800_ln01_20250430.csv
    /// BRCD,CUSTSEQ,CUSTNM,TAI_KHOAN,CCY,DU_NO,DSBSSEQ,TRANSACTION_DATE,DSBSDT,DISBUR_CCY,DISBURSEMENT_AMOUNT,DSBSMATDT,BSRTCD,INTEREST_RATE,APPRSEQ,APPRDT,APPR_CCY,APPRAMT,APPRMATDT,LOAN_TYPE,FUND_RESOURCE_CODE,FUND_PURPOSE_CODE,REPAYMENT_AMOUNT,NEXT_REPAY_DATE,NEXT_REPAY_AMOUNT,NEXT_INT_REPAY_DATE,OFFICER_ID,OFFICER_NAME,INTEREST_AMOUNT,PASTDUE_INTEREST_AMOUNT,TOTAL_INTEREST_REPAY_AMOUNT,CUSTOMER_TYPE_CODE,CUSTOMER_TYPE_CODE_DETAIL,TRCTCD,TRCTNM,ADDR1,PROVINCE,LCLPROVINNM,DISTRICT,LCLDISTNM,COMMCD,LCLWARDNM,LAST_REPAY_DATE,SECURED_PERCENT,NHOM_NO,LAST_INT_CHARGE_DATE,EXEMPTINT,EXEMPTINTTYPE,EXEMPTINTAMT,GRPNO,BUSCD,BSNSSCLTPCD,USRIDOP,ACCRUAL_AMOUNT,ACCRUAL_AMOUNT_END_OF_MONTH,INTCMTH,INTRPYMTH,INTTRMMTH,YRDAYS,REMARK,CHITIEU,CTCV,CREDIT_LINE_YPE,INT_LUMPSUM_PARTIAL_TYPE,INT_PARTIAL_PAYMENT_TYPE,INT_PAYMENT_INTERVAL,AN_HAN_LAI,PHUONG_THUC_GIAI_NGAN_1,TAI_KHOAN_GIAI_NGAN_1,SO_TIEN_GIAI_NGAN_1,PHUONG_THUC_GIAI_NGAN_2,TAI_KHOAN_GIAI_NGAN_2,SO_TIEN_GIAI_NGAN_2,CMT_HC,NGAY_SINH,MA_CB_AGRI,MA_NGANH_KT,TY_GIA,OFFICER_IPCAS
    /// </summary>
    [Table("LN01")]
    public class LN01
    {
        [Key]
        public int Id { get; set; }

        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        // === 79 CỘT THEO HEADER CSV GỐC ===
        [Column("BRCD")]
        [StringLength(50)]
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
        [StringLength(50)]
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
        [StringLength(50)]
        public string? CUSTOMER_TYPE_CODE { get; set; }

        [Column("CUSTOMER_TYPE_CODE_DETAIL")]
        [StringLength(50)]
        public string? CUSTOMER_TYPE_CODE_DETAIL { get; set; }

        [Column("TRCTCD")]
        [StringLength(50)]
        public string? TRCTCD { get; set; }

        [Column("TRCTNM")]
        [StringLength(255)]
        public string? TRCTNM { get; set; }

        [Column("ADDR1")]
        [StringLength(500)]
        public string? ADDR1 { get; set; }

        [Column("PROVINCE")]
        [StringLength(255)]
        public string? PROVINCE { get; set; }

        [Column("LCLPROVINNM")]
        [StringLength(255)]
        public string? LCLPROVINNM { get; set; }

        [Column("DISTRICT")]
        [StringLength(255)]
        public string? DISTRICT { get; set; }

        [Column("LCLDISTNM")]
        [StringLength(255)]
        public string? LCLDISTNM { get; set; }

        [Column("COMMCD")]
        [StringLength(50)]
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
        [StringLength(50)]
        public string? EXEMPTINTTYPE { get; set; }

        [Column("EXEMPTINTAMT")]
        public decimal? EXEMPTINTAMT { get; set; }

        [Column("GRPNO")]
        [StringLength(50)]
        public string? GRPNO { get; set; }

        [Column("BUSCD")]
        [StringLength(50)]
        public string? BUSCD { get; set; }

        [Column("BSNSSCLTPCD")]
        [StringLength(50)]
        public string? BSNSSCLTPCD { get; set; }

        [Column("USRIDOP")]
        [StringLength(50)]
        public string? USRIDOP { get; set; }

        [Column("ACCRUAL_AMOUNT")]
        public decimal? ACCRUAL_AMOUNT { get; set; }

        [Column("ACCRUAL_AMOUNT_END_OF_MONTH")]
        public decimal? ACCRUAL_AMOUNT_END_OF_MONTH { get; set; }

        [Column("INTCMTH")]
        public decimal? INTCMTH { get; set; }

        [Column("INTRPYMTH")]
        public decimal? INTRPYMTH { get; set; }

        [Column("INTTRMMTH")]
        public int? INTTRMMTH { get; set; }

        [Column("YRDAYS")]
        public int? YRDAYS { get; set; }

        [Column("REMARK")]
        [StringLength(500)]
        public string? REMARK { get; set; }

        [Column("CHITIEU")]
        [StringLength(255)]
        public string? CHITIEU { get; set; }

        [Column("CTCV")]
        [StringLength(255)]
        public string? CTCV { get; set; }

        [Column("CREDIT_LINE_YPE")]
        [StringLength(50)]
        public string? CREDIT_LINE_YPE { get; set; }

        [Column("INT_LUMPSUM_PARTIAL_TYPE")]
        [StringLength(50)]
        public string? INT_LUMPSUM_PARTIAL_TYPE { get; set; }

        [Column("INT_PARTIAL_PAYMENT_TYPE")]
        [StringLength(50)]
        public string? INT_PARTIAL_PAYMENT_TYPE { get; set; }

        [Column("INT_PAYMENT_INTERVAL")]
        public int? INT_PAYMENT_INTERVAL { get; set; }

        [Column("AN_HAN_LAI")]
        [StringLength(50)]
        public string? AN_HAN_LAI { get; set; }

        [Column("PHUONG_THUC_GIAI_NGAN_1")]
        [StringLength(255)]
        public string? PHUONG_THUC_GIAI_NGAN_1 { get; set; }

        [Column("TAI_KHOAN_GIAI_NGAN_1")]
        [StringLength(50)]
        public string? TAI_KHOAN_GIAI_NGAN_1 { get; set; }

        [Column("SO_TIEN_GIAI_NGAN_1")]
        public decimal? SO_TIEN_GIAI_NGAN_1 { get; set; }

        [Column("PHUONG_THUC_GIAI_NGAN_2")]
        [StringLength(255)]
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
        [StringLength(50)]
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

echo "✅ Đã tạo lại LN01.cs với chính xác 79 cột CSV + 3 temporal = 82 cột tổng"
echo "🔍 Kiểm tra số property trong file:"
grep -c "public.*{.*get.*set.*}" $MODELS_DIR/LN01.cs

echo "🎉 Hoàn thành regenerate LN01 model!"
