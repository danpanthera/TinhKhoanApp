using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.Dtos
{
    /// <summary>
    /// DPDA Create DTO - Input DTO cho tạo mới DPDA record
    /// Contains all business fields required cho creation (13 business columns)
    /// Used in: POST /api/dpda endpoint, CreateAsync service method
    /// Validation: Required fields, data formats, business rules
    /// </summary>
    public class DPDACreateDto
    {
        /// <summary>Ngày dữ liệu - Required system field</summary>
        [Required(ErrorMessage = "Ngày dữ liệu không được để trống")]
        [Display(Name = "Ngày Dữ Liệu")]
        public DateTime NgayDl { get; set; }

        // 13 Business Columns - Input validation
        /// <summary>Mã chi nhánh</summary>
        [Required(ErrorMessage = "Mã chi nhánh không được để trống")]
        [StringLength(200, ErrorMessage = "Mã chi nhánh không được vượt quá 200 ký tự")]
        [Display(Name = "Mã Chi Nhánh")]
        public string MaChiNhanh { get; set; } = "";

        /// <summary>Mã khách hàng</summary>
        [Required(ErrorMessage = "Mã khách hàng không được để trống")]
        [StringLength(200, ErrorMessage = "Mã khách hàng không được vượt quá 200 ký tự")]
        [Display(Name = "Mã Khách Hàng")]
        public string MaKhachHang { get; set; } = "";

        /// <summary>Tên khách hàng</summary>
        [Required(ErrorMessage = "Tên khách hàng không được để trống")]
        [StringLength(200, ErrorMessage = "Tên khách hàng không được vượt quá 200 ký tự")]
        [Display(Name = "Tên Khách Hàng")]
        public string TenKhachHang { get; set; } = "";

        /// <summary>Số tài khoản</summary>
        [Required(ErrorMessage = "Số tài khoản không được để trống")]
        [StringLength(200, ErrorMessage = "Số tài khoản không được vượt quá 200 ký tự")]
        [Display(Name = "Số Tài Khoản")]
        public string SoTaiKhoan { get; set; } = "";

        /// <summary>Loại thẻ</summary>
        [StringLength(200, ErrorMessage = "Loại thẻ không được vượt quá 200 ký tự")]
        [Display(Name = "Loại Thẻ")]
        public string LoaiThe { get; set; } = "";

        /// <summary>Số thẻ</summary>
        [StringLength(200, ErrorMessage = "Số thẻ không được vượt quá 200 ký tự")]
        [Display(Name = "Số Thẻ")]
        public string SoThe { get; set; } = "";

        /// <summary>Ngày nộp đơn</summary>
        [Display(Name = "Ngày Nộp Đơn")]
        public DateTime? NgayNopDon { get; set; }

        /// <summary>Ngày phát hành thẻ</summary>
        [Display(Name = "Ngày Phát Hành")]
        public DateTime? NgayPhatHanh { get; set; }

        /// <summary>User phát hành thẻ</summary>
        [StringLength(200, ErrorMessage = "User phát hành không được vượt quá 200 ký tự")]
        [Display(Name = "User Phát Hành")]
        public string UserPhatHanh { get; set; } = "";

        /// <summary>Trạng thái thẻ</summary>
        [StringLength(200, ErrorMessage = "Trạng thái không được vượt quá 200 ký tự")]
        [Display(Name = "Trạng Thái")]
        public string TrangThai { get; set; } = "";

        /// <summary>Phân loại thẻ</summary>
        [StringLength(200, ErrorMessage = "Phân loại không được vượt quá 200 ký tự")]
        [Display(Name = "Phân Loại")]
        public string PhanLoai { get; set; } = "";

        /// <summary>Giao thẻ</summary>
        [StringLength(200, ErrorMessage = "Giao thẻ không được vượt quá 200 ký tự")]
        [Display(Name = "Giao Thẻ")]
        public string GiaoThe { get; set; } = "";

        /// <summary>Loại phát hành</summary>
        [StringLength(200, ErrorMessage = "Loại phát hành không được vượt quá 200 ký tự")]
        [Display(Name = "Loại Phát Hành")]
        public string LoaiPhatHanh { get; set; } = "";
    }
}
