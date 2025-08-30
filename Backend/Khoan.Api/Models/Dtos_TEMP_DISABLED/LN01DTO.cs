using System;
using System.ComponentModel.DataAnnotations;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Models.Dtos
{
    /// <summary>
    /// DTO cho dữ liệu LN01 (Thông tin khoản vay)
    /// </summary>
    public class LN01DTO
    {
        /// <summary>
        /// Id của bản ghi
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Ngày dữ liệu
        /// </summary>
        public DateTime? NgayDL { get; set; }

        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        public string? BranchCode { get; set; }

        /// <summary>
        /// Mã số khách hàng
        /// </summary>
        public string? CustomerCode { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string? CustomerName { get; set; }

        /// <summary>
        /// Số tài khoản khoản vay
        /// </summary>
        public string? TaiKhoan { get; set; }

        /// <summary>
        /// Loại tiền
        /// </summary>
        public string? Currency { get; set; }

        /// <summary>
        /// Dư nợ
        /// </summary>
        public decimal? DuNo { get; set; }

        /// <summary>
        /// Mã giải ngân
        /// </summary>
        public string? DisbursementSeq { get; set; }

        /// <summary>
        /// Ngày giao dịch
        /// </summary>
        public DateTime? TransactionDate { get; set; }

        /// <summary>
        /// Ngày giải ngân
        /// </summary>
        public DateTime? NgayGiaiNgan { get; set; }

        /// <summary>
        /// Loại tiền giải ngân
        /// </summary>
        public string? DisbursementCurrency { get; set; }

        /// <summary>
        /// Số tiền giải ngân
        /// </summary>
        public decimal? DisbursementAmount { get; set; }

        /// <summary>
        /// Ngày đáo hạn
        /// </summary>
        public DateTime? NgayDaoHan { get; set; }

        /// <summary>
        /// Mã lãi suất
        /// </summary>
        public string? InterestRateCode { get; set; }

        /// <summary>
        /// Lãi suất
        /// </summary>
        public decimal? LaiSuat { get; set; }

        /// <summary>
        /// Mã phê duyệt
        /// </summary>
        public string? ApprovalSeq { get; set; }

        /// <summary>
        /// Ngày phê duyệt
        /// </summary>
        public DateTime? ApprovalDate { get; set; }

        /// <summary>
        /// Loại tiền phê duyệt
        /// </summary>
        public string? ApprovalCurrency { get; set; }

        /// <summary>
        /// Số tiền phê duyệt
        /// </summary>
        public decimal? ApprovalAmount { get; set; }

        /// <summary>
        /// Ngày đáo hạn phê duyệt
        /// </summary>
        public DateTime? ApprovalMaturityDate { get; set; }

        /// <summary>
        /// Loại khoản vay
        /// </summary>
        public string? LoanType { get; set; }

        /// <summary>
        /// Mã nguồn vốn
        /// </summary>
        public string? FundResourceCode { get; set; }

        /// <summary>
        /// Mã mục đích vay
        /// </summary>
        public string? FundPurposeCode { get; set; }

        /// <summary>
        /// Số tiền trả
        /// </summary>
        public decimal? RepaymentAmount { get; set; }

        /// <summary>
        /// Ngày trả tiếp theo
        /// </summary>
        public DateTime? NextRepayDate { get; set; }

        /// <summary>
        /// Số tiền trả tiếp theo
        /// </summary>
        public decimal? NextRepayAmount { get; set; }

        /// <summary>
        /// Ngày trả lãi tiếp theo
        /// </summary>
        public DateTime? NextInterestRepayDate { get; set; }

        /// <summary>
        /// ID cán bộ
        /// </summary>
        public string? OfficerId { get; set; }

        /// <summary>
        /// Tên cán bộ
        /// </summary>
        public string? OfficerName { get; set; }

        /// <summary>
        /// Nhóm nợ
        /// </summary>
        public string? NhomNo { get; set; }

        /// <summary>
        /// Ngày tạo bản ghi
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Ngày cập nhật bản ghi
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Tên file nguồn
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Map từ entity sang DTO
        /// </summary>
        public static LN01DTO FromEntity(DataTables.LN01 entity)
        {
            if (entity == null)
                return null;

            return new DataTables.LN01DTO
            {
                Id = entity.Id,
                NgayDL = entity.NGAY_DL,
                BranchCode = entity.BRCD,
                CustomerCode = entity.CUSTSEQ,
                CustomerName = entity.CUSTNM,
                TaiKhoan = entity.TAI_KHOAN,
                Currency = entity.CCY,
                DuNo = entity.DU_NO,
                DisbursementSeq = entity.DSBSSEQ,
                TransactionDate = entity.TRANSACTION_DATE,
                NgayGiaiNgan = entity.DSBSDT,
                DisbursementCurrency = entity.DISBUR_CCY,
                DisbursementAmount = entity.DISBURSEMENT_AMOUNT,
                NgayDaoHan = entity.DSBSMATDT,
                InterestRateCode = entity.BSRTCD,
                LaiSuat = entity.INTEREST_RATE,
                ApprovalSeq = entity.APPRSEQ,
                ApprovalDate = entity.APPRDT,
                ApprovalCurrency = entity.APPR_CCY,
                ApprovalAmount = entity.APPRAMT,
                ApprovalMaturityDate = entity.APPRMATDT,
                LoanType = entity.LOAN_TYPE,
                FundResourceCode = entity.FUND_RESOURCE_CODE,
                FundPurposeCode = entity.FUND_PURPOSE_CODE,
                RepaymentAmount = entity.REPAYMENT_AMOUNT,
                NextRepayDate = entity.NEXT_REPAY_DATE,
                NextRepayAmount = entity.NEXT_REPAY_AMOUNT,
                NextInterestRepayDate = entity.NEXT_INT_REPAY_DATE,
                OfficerId = entity.OFFICER_ID,
                OfficerName = entity.OFFICER_NAME,
                NhomNo = entity.NHOM_NO,
                CreatedDate = entity.CREATED_DATE,
                UpdatedDate = entity.UPDATED_DATE,
                FileName = entity.FILE_NAME
            };
        }

        /// <summary>
        /// Tạo entity từ DTO
        /// </summary>
        public DataTables.LN01 ToEntity()
        {
            return new DataTables.LN01
            {
                Id = this.Id,
                NGAY_DL = this.NgayDL,
                BRCD = this.BranchCode,
                CUSTSEQ = this.CustomerCode,
                CUSTNM = this.CustomerName,
                TAI_KHOAN = this.TaiKhoan,
                CCY = this.Currency,
                DU_NO = this.DuNo,
                DSBSSEQ = this.DisbursementSeq,
                TRANSACTION_DATE = this.TransactionDate,
                DSBSDT = this.NgayGiaiNgan,
                DISBUR_CCY = this.DisbursementCurrency,
                DISBURSEMENT_AMOUNT = this.DisbursementAmount,
                DSBSMATDT = this.NgayDaoHan,
                BSRTCD = this.InterestRateCode,
                INTEREST_RATE = this.LaiSuat,
                APPRSEQ = this.ApprovalSeq,
                APPRDT = this.ApprovalDate,
                APPR_CCY = this.ApprovalCurrency,
                APPRAMT = this.ApprovalAmount,
                APPRMATDT = this.ApprovalMaturityDate,
                LOAN_TYPE = this.LoanType,
                FUND_RESOURCE_CODE = this.FundResourceCode,
                FUND_PURPOSE_CODE = this.FundPurposeCode,
                REPAYMENT_AMOUNT = this.RepaymentAmount,
                NEXT_REPAY_DATE = this.NextRepayDate,
                NEXT_REPAY_AMOUNT = this.NextRepayAmount,
                NEXT_INT_REPAY_DATE = this.NextInterestRepayDate,
                OFFICER_ID = this.OfficerId,
                OFFICER_NAME = this.OfficerName,
                NHOM_NO = this.NhomNo,
                CREATED_DATE = this.CreatedDate ?? DateTime.Now,
                FILE_NAME = this.FileName
            };
        }

        /// <summary>
        /// Cập nhật entity từ DTO (chỉ cập nhật các trường không null)
        /// </summary>
        public void UpdateEntity(Models.DataTables.LN01 entity)
        {
            if (entity == null) return;

            // Chỉ cập nhật các trường có giá trị
            if (NgayDL.HasValue) entity.NGAY_DL = NgayDL;
            if (!string.IsNullOrEmpty(BranchCode)) entity.BRCD = BranchCode;
            if (!string.IsNullOrEmpty(CustomerCode)) entity.CUSTSEQ = CustomerCode;
            if (!string.IsNullOrEmpty(CustomerName)) entity.CUSTNM = CustomerName;
            if (!string.IsNullOrEmpty(TaiKhoan)) entity.TAI_KHOAN = TaiKhoan;
            if (!string.IsNullOrEmpty(Currency)) entity.CCY = Currency;
            if (DuNo.HasValue) entity.DU_NO = DuNo;
            if (!string.IsNullOrEmpty(DisbursementSeq)) entity.DSBSSEQ = DisbursementSeq;
            if (TransactionDate.HasValue) entity.TRANSACTION_DATE = TransactionDate;
            if (NgayGiaiNgan.HasValue) entity.DSBSDT = NgayGiaiNgan;
            if (!string.IsNullOrEmpty(DisbursementCurrency)) entity.DISBUR_CCY = DisbursementCurrency;
            if (DisbursementAmount.HasValue) entity.DISBURSEMENT_AMOUNT = DisbursementAmount;
            if (NgayDaoHan.HasValue) entity.DSBSMATDT = NgayDaoHan;
            if (!string.IsNullOrEmpty(InterestRateCode)) entity.BSRTCD = InterestRateCode;
            if (LaiSuat.HasValue) entity.INTEREST_RATE = LaiSuat;
            if (!string.IsNullOrEmpty(ApprovalSeq)) entity.APPRSEQ = ApprovalSeq;
            if (ApprovalDate.HasValue) entity.APPRDT = ApprovalDate;
            if (!string.IsNullOrEmpty(ApprovalCurrency)) entity.APPR_CCY = ApprovalCurrency;
            if (ApprovalAmount.HasValue) entity.APPRAMT = ApprovalAmount;
            if (ApprovalMaturityDate.HasValue) entity.APPRMATDT = ApprovalMaturityDate;
            if (!string.IsNullOrEmpty(LoanType)) entity.LOAN_TYPE = LoanType;
            if (!string.IsNullOrEmpty(FundResourceCode)) entity.FUND_RESOURCE_CODE = FundResourceCode;
            if (!string.IsNullOrEmpty(FundPurposeCode)) entity.FUND_PURPOSE_CODE = FundPurposeCode;
            if (RepaymentAmount.HasValue) entity.REPAYMENT_AMOUNT = RepaymentAmount;
            if (NextRepayDate.HasValue) entity.NEXT_REPAY_DATE = NextRepayDate;
            if (NextRepayAmount.HasValue) entity.NEXT_REPAY_AMOUNT = NextRepayAmount;
            if (NextInterestRepayDate.HasValue) entity.NEXT_INT_REPAY_DATE = NextInterestRepayDate;
            if (!string.IsNullOrEmpty(OfficerId)) entity.OFFICER_ID = OfficerId;
            if (!string.IsNullOrEmpty(OfficerName)) entity.OFFICER_NAME = OfficerName;
            if (!string.IsNullOrEmpty(NhomNo)) entity.NHOM_NO = NhomNo;
            if (!string.IsNullOrEmpty(FileName)) entity.FILE_NAME = FileName;

            // Cập nhật thời gian cập nhật
            entity.UPDATED_DATE = DateTime.Now;
        }
    }
}
