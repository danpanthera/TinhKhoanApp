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
                DuNoThucTe = entity.DU_NO_THUC_TE,
                HanMuc = entity.HAN_MUC,
                NgayGiaiNgan = entity.NGAY_GIAI_NGAN,
                NgayDenHan = entity.NGAY_DEN_HAN,
                LaiSuat = entity.LAI_SUAT,
                NhomNo = entity.NHOM_NO,
                LoaiHinhChoVay = entity.LOAI_HINH_CHO_VAY,
                CanBoQL = entity.CAN_BO_QL,
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
                NGAY_DL = this.NgayDL,
                BRCD = this.BranchCode,
                CUSTSEQ = this.CustomerCode,
                TAI_KHOAN = this.TaiKhoan,
                CCY = this.Currency,
                DU_NO_THUC_TE = this.DuNoThucTe,
                HAN_MUC = this.HanMuc,
                NGAY_GIAI_NGAN = this.NgayGiaiNgan,
                NGAY_DEN_HAN = this.NgayDenHan,
                LAI_SUAT = this.LaiSuat,
                NHOM_NO = this.NhomNo,
                LOAI_HINH_CHO_VAY = this.LoaiHinhChoVay,
                CAN_BO_QL = this.CanBoQL,
                CREATED_DATE = this.CreatedDate ?? DateTime.Now
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
                NGAY_DL = this.NgayDL,
                BRCD = this.BranchCode,
                CUSTSEQ = this.CustomerCode,
                TAI_KHOAN = this.TaiKhoan,
                CCY = this.Currency,
                DU_NO_THUC_TE = this.DuNoThucTe,
                HAN_MUC = this.HanMuc,
                NGAY_GIAI_NGAN = this.NgayGiaiNgan,
                NGAY_DEN_HAN = this.NgayDenHan,
                LAI_SUAT = this.LaiSuat,
                NHOM_NO = this.NhomNo,
                LOAI_HINH_CHO_VAY = this.LoaiHinhChoVay,
                CAN_BO_QL = this.CanBoQL,
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
            if (NgayDL.HasValue) entity.NGAY_DL = NgayDL;
            if (!string.IsNullOrEmpty(BranchCode)) entity.BRCD = BranchCode;
            if (!string.IsNullOrEmpty(CustomerCode)) entity.CUSTSEQ = CustomerCode;
            if (!string.IsNullOrEmpty(TaiKhoan)) entity.TAI_KHOAN = TaiKhoan;
            if (!string.IsNullOrEmpty(Currency)) entity.CCY = Currency;
            if (DuNoThucTe.HasValue) entity.DU_NO_THUC_TE = DuNoThucTe;
            if (HanMuc.HasValue) entity.HAN_MUC = HanMuc;
            if (NgayGiaiNgan.HasValue) entity.NGAY_GIAI_NGAN = NgayGiaiNgan;
            if (NgayDenHan.HasValue) entity.NGAY_DEN_HAN = NgayDenHan;
            if (LaiSuat.HasValue) entity.LAI_SUAT = LaiSuat;
            if (!string.IsNullOrEmpty(NhomNo)) entity.NHOM_NO = NhomNo;
            if (!string.IsNullOrEmpty(LoaiHinhChoVay)) entity.LOAI_HINH_CHO_VAY = LoaiHinhChoVay;
            if (!string.IsNullOrEmpty(CanBoQL)) entity.CAN_BO_QL = CanBoQL;
        }
    }
}
