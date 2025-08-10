using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.Dtos
{
    /// <summary>
    /// DPDA Preview DTO - Light view cho danh sách và trang chính
    /// Contains key identification và status fields cho quick reference
    /// Used in: GetAll, GetByDate, GetByAccountNumber paging results
    /// </summary>
    public class DPDAPreviewDto
    {
        /// <summary>Record ID</summary>
        public int Id { get; set; }

        /// <summary>Ngày dữ liệu</summary>
        [Display(Name = "Ngày Dữ Liệu")]
        public DateTime NgayDl { get; set; }

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

        /// <summary>Trạng thái thẻ</summary>
        [Display(Name = "Trạng Thái")]
        public string TrangThai { get; set; } = "";

        /// <summary>Ngày tạo record</summary>
        [Display(Name = "Ngày Tạo")]
        public DateTime CreatedDate { get; set; }
    }
}
