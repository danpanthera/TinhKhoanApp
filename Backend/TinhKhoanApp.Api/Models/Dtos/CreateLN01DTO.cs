using System;
using System.ComponentModel.DataAnnotations;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Models.DTOs
{
    /// <summary>
    /// DTO for creating a new LN01 record
    /// </summary>
    public class CreateLN01DTO
    {
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
        /// Tên file nguồn
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Tạo entity từ DTO
        /// </summary>
        public TinhKhoanApp.Api.Models.DataTables.LN01 ToEntity()
        {
            return new TinhKhoanApp.Api.Models.DataTables.LN01
            {
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
                FILE_NAME = this.FileName,
                CREATED_DATE = DateTime.Now
            };
        }
    }
}
