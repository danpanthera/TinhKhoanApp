using System;
using System.ComponentModel.DataAnnotations;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Models.DTOs
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
        /// Số tài khoản khoản vay
        /// </summary>
        public string? TaiKhoan { get; set; }

        /// <summary>
        /// Loại tiền
        /// </summary>
        public string? Currency { get; set; }

        /// <summary>
        /// Dư nợ thực tế
        /// </summary>
        public decimal? DuNoThucTe { get; set; }

        /// <summary>
        /// Hạn mức
        /// </summary>
        public decimal? HanMuc { get; set; }

        /// <summary>
        /// Ngày giải ngân
        /// </summary>
        public DateTime? NgayGiaiNgan { get; set; }

        /// <summary>
        /// Ngày đến hạn
        /// </summary>
        public DateTime? NgayDenHan { get; set; }

        /// <summary>
        /// Lãi suất
        /// </summary>
        public decimal? LaiSuat { get; set; }

        /// <summary>
        /// Nhóm nợ
        /// </summary>
        public string? NhomNo { get; set; }

        /// <summary>
        /// Loại hình cho vay
        /// </summary>
        public string? LoaiHinhChoVay { get; set; }

        /// <summary>
        /// Thông tin cán bộ quản lý
        /// </summary>
        public string? CanBoQL { get; set; }

        /// <summary>
        /// Ngày tạo bản ghi
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Map từ entity sang DTO
        /// </summary>
        public static LN01DTO FromEntity(LN01 entity)
        {
            return new LN01DTO
            {
                Id = entity.Id,
                NgayDL = entity.NGAY_DL,
                BranchCode = entity.BRCD,
                CustomerCode = entity.CUSTSEQ,
                TaiKhoan = entity.TAI_KHOAN,
                Currency = entity.CCY,
                DuNoThucTe = decimal.TryParse(entity.DU_NO, out decimal duNo) ? duNo : 0,
                HanMuc = 0, // Mapping to default value since HAN_MUC doesn't exist
                NgayGiaiNgan = DateTime.TryParse(entity.DSBSDT, out DateTime giaiNgan) ? giaiNgan : null,
                NgayDenHan = DateTime.TryParse(entity.DSBSMATDT, out DateTime denHan) ? denHan : null,
                LaiSuat = decimal.TryParse(entity.INTEREST_RATE, out decimal laiSuat) ? laiSuat : 0,
                NhomNo = entity.NHOM_NO,
                LoaiHinhChoVay = entity.LOAN_TYPE,
                CanBoQL = entity.OFFICER_IPCAS,
                CreatedDate = entity.CREATED_DATE
            };
        }

        /// <summary>
        /// Map từ DTO sang entity
        /// </summary>
        public LN01 ToEntity()
        {
            return new LN01
            {
                Id = this.Id,
                NGAY_DL = this.NgayDL.HasValue ? this.NgayDL.Value : DateTime.Now,
                BRCD = this.BranchCode,
                CUSTSEQ = this.CustomerCode,
                TAI_KHOAN = this.TaiKhoan,
                CCY = this.Currency,
                DU_NO = this.DuNoThucTe?.ToString() ?? "0",
                DSBSDT = this.NgayGiaiNgan?.ToString("yyyy-MM-dd") ?? "",
                DSBSMATDT = this.NgayDenHan?.ToString("yyyy-MM-dd") ?? "",
                INTEREST_RATE = this.LaiSuat?.ToString() ?? "0",
                NHOM_NO = this.NhomNo,
                LOAN_TYPE = this.LoaiHinhChoVay,
                OFFICER_IPCAS = this.CanBoQL,
                CREATED_DATE = this.CreatedDate.HasValue ? this.CreatedDate.Value : DateTime.Now
            };
        }
    }

    /// <summary>
    /// DTO cho việc tạo mới LN01
    /// </summary>
    public class CreateLN01DTO
    {
        /// <summary>
        /// Ngày dữ liệu
        /// </summary>
        [Required]
        public DateTime NgayDL { get; set; }

        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        [Required, MaxLength(10)]
        public string BranchCode { get; set; } = string.Empty;

        /// <summary>
        /// Mã số khách hàng
        /// </summary>
        [Required, MaxLength(50)]
        public string CustomerCode { get; set; } = string.Empty;

        /// <summary>
        /// Số tài khoản khoản vay
        /// </summary>
        [Required, MaxLength(50)]
        public string TaiKhoan { get; set; } = string.Empty;

        /// <summary>
        /// Loại tiền
        /// </summary>
        [Required, MaxLength(5)]
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Dư nợ thực tế
        /// </summary>
        public decimal? DuNoThucTe { get; set; }

        /// <summary>
        /// Hạn mức
        /// </summary>
        public decimal? HanMuc { get; set; }

        /// <summary>
        /// Ngày giải ngân
        /// </summary>
        public DateTime? NgayGiaiNgan { get; set; }

        /// <summary>
        /// Ngày đến hạn
        /// </summary>
        public DateTime? NgayDenHan { get; set; }

        /// <summary>
        /// Lãi suất
        /// </summary>
        public decimal? LaiSuat { get; set; }

        /// <summary>
        /// Nhóm nợ
        /// </summary>
        [MaxLength(5)]
        public string? NhomNo { get; set; }

        /// <summary>
        /// Loại hình cho vay
        /// </summary>
        [MaxLength(50)]
        public string? LoaiHinhChoVay { get; set; }

        /// <summary>
        /// Thông tin cán bộ quản lý
        /// </summary>
        [MaxLength(100)]
        public string? CanBoQL { get; set; }

        /// <summary>
        /// Chuyển đổi sang entity
        /// </summary>
        public LN01 ToEntity()
        {
            return new LN01
            {
                NGAY_DL = this.NgayDL != null ? this.NgayDL : DateTime.Now,
                BRCD = this.BranchCode ?? "",
                CUSTSEQ = this.CustomerCode ?? "",
                TAI_KHOAN = this.TaiKhoan ?? "",
                CCY = this.Currency ?? "",
                DU_NO = this.DuNoThucTe?.ToString() ?? "0",
                DSBSDT = this.NgayGiaiNgan?.ToString("yyyy-MM-dd") ?? "",
                DSBSMATDT = this.NgayDenHan?.ToString("yyyy-MM-dd") ?? "",
                INTEREST_RATE = this.LaiSuat?.ToString() ?? "0",
                NHOM_NO = this.NhomNo ?? "",
                LOAN_TYPE = this.LoaiHinhChoVay ?? "",
                OFFICER_IPCAS = this.CanBoQL ?? "",
                CREATED_DATE = DateTime.Now
            };
        }
    }

    /// <summary>
    /// DTO cho việc cập nhật LN01
    /// </summary>
    public class UpdateLN01DTO
    {
        /// <summary>
        /// Ngày dữ liệu
        /// </summary>
        public DateTime? NgayDL { get; set; }

        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        [MaxLength(10)]
        public string? BranchCode { get; set; }

        /// <summary>
        /// Mã số khách hàng
        /// </summary>
        [MaxLength(50)]
        public string? CustomerCode { get; set; }

        /// <summary>
        /// Số tài khoản khoản vay
        /// </summary>
        [MaxLength(50)]
        public string? TaiKhoan { get; set; }

        /// <summary>
        /// Loại tiền
        /// </summary>
        [MaxLength(5)]
        public string? Currency { get; set; }

        /// <summary>
        /// Dư nợ thực tế
        /// </summary>
        public decimal? DuNoThucTe { get; set; }

        /// <summary>
        /// Hạn mức
        /// </summary>
        public decimal? HanMuc { get; set; }

        /// <summary>
        /// Ngày giải ngân
        /// </summary>
        public DateTime? NgayGiaiNgan { get; set; }

        /// <summary>
        /// Ngày đến hạn
        /// </summary>
        public DateTime? NgayDenHan { get; set; }

        /// <summary>
        /// Lãi suất
        /// </summary>
        public decimal? LaiSuat { get; set; }

        /// <summary>
        /// Nhóm nợ
        /// </summary>
        [MaxLength(5)]
        public string? NhomNo { get; set; }

        /// <summary>
        /// Loại hình cho vay
        /// </summary>
        [MaxLength(50)]
        public string? LoaiHinhChoVay { get; set; }

        /// <summary>
        /// Thông tin cán bộ quản lý
        /// </summary>
        [MaxLength(100)]
        public string? CanBoQL { get; set; }

        /// <summary>
        /// Cập nhật entity hiện có
        /// </summary>
        public void UpdateEntity(LN01 entity)
        {
            if (NgayDL.HasValue) entity.NGAY_DL = NgayDL.Value;
            if (!string.IsNullOrEmpty(BranchCode)) entity.BRCD = BranchCode;
            if (!string.IsNullOrEmpty(CustomerCode)) entity.CUSTSEQ = CustomerCode;
            if (!string.IsNullOrEmpty(TaiKhoan)) entity.TAI_KHOAN = TaiKhoan;
            if (!string.IsNullOrEmpty(Currency)) entity.CCY = Currency;
            if (DuNoThucTe.HasValue) entity.DU_NO = DuNoThucTe.ToString();
            if (NgayGiaiNgan.HasValue) entity.DSBSDT = NgayGiaiNgan.Value.ToString("yyyy-MM-dd");
            if (NgayDenHan.HasValue) entity.DSBSMATDT = NgayDenHan.Value.ToString("yyyy-MM-dd");
            if (LaiSuat.HasValue) entity.INTEREST_RATE = LaiSuat.ToString();
            if (!string.IsNullOrEmpty(NhomNo)) entity.NHOM_NO = NhomNo;
            if (!string.IsNullOrEmpty(LoaiHinhChoVay)) entity.LOAN_TYPE = LoaiHinhChoVay;
            if (!string.IsNullOrEmpty(CanBoQL)) entity.OFFICER_IPCAS = CanBoQL;
        }
    }
}
