using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.Dtos
{
    /// <summary>
    /// DPDA Details DTO - Complete view với tất cả 13 business columns + system fields
    /// Contains all CSV business columns + temporal data cho detailed views
    /// Used in: GetById, Create result, Update result, GetByCustomer, GetByCardNumber
    /// </summary>
    public class DPDADetailsDto
    {
        /// <summary>Record ID</summary>
        public int Id { get; set; }

        /// <summary>Ngày dữ liệu - System column</summary>
        [Display(Name = "Ngày Dữ Liệu")]
        public DateTime NgayDl { get; set; }

        // 13 Business Columns - Exact CSV order
        /// <summary>Mã chi nhánh</summary>
        [Display(Name = "Mã Chi Nhánh")]
        public string MaChiNhanh { get; set; } = "";

        /// <summary>Mã khách hàng</summary>
        [Display(Name = "Mã Khách Hàng")]
        public string MaKhachHang { get; set; } = "";

        /// <summary>Tên khách hàng</summary>
        [Display(Name = "Tên Khách Hàng")]
        public string TenKhachHang { get; set; } = "";

        /// <summary>Số tài khoản</summary>
        [Display(Name = "Số Tài Khoản")]
        public string SoTaiKhoan { get; set; } = "";

        /// <summary>Loại thẻ</summary>
        [Display(Name = "Loại Thẻ")]
        public string LoaiThe { get; set; } = "";

        /// <summary>Số thẻ</summary>
        [Display(Name = "Số Thẻ")]
        public string SoThe { get; set; } = "";

        /// <summary>Ngày nộp đơn</summary>
        [Display(Name = "Ngày Nộp Đơn")]
        public DateTime? NgayNopDon { get; set; }

        /// <summary>Ngày phát hành thẻ</summary>
        [Display(Name = "Ngày Phát Hành")]
        public DateTime? NgayPhatHanh { get; set; }

        /// <summary>User phát hành thẻ</summary>
        [Display(Name = "User Phát Hành")]
        public string UserPhatHanh { get; set; } = "";

        /// <summary>Trạng thái thẻ</summary>
        [Display(Name = "Trạng Thái")]
        public string TrangThai { get; set; } = "";

        /// <summary>Phân loại thẻ</summary>
        [Display(Name = "Phân Loại")]
        public string PhanLoai { get; set; } = "";

        /// <summary>Giao thẻ</summary>
        [Display(Name = "Giao Thẻ")]
        public string GiaoThe { get; set; } = "";

        /// <summary>Loại phát hành</summary>
        [Display(Name = "Loại Phát Hành")]
        public string LoaiPhatHanh { get; set; } = "";

        // System columns cho auditing
        /// <summary>Ngày tạo record</summary>
        [Display(Name = "Ngày Tạo")]
        public DateTime CreatedDate { get; set; }

        /// <summary>Ngày cập nhật record</summary>
        [Display(Name = "Ngày Cập Nhật")]
        public DateTime UpdatedDate { get; set; }
    }
}
