namespace TinhKhoanApp.Api.Models.DTOs
{
    /// <summary>
    /// DTO cho preview dữ liệu GL02 - Giao dịch sổ cái
    /// </summary>
    public class GL02PreviewDto
    {
        /// <summary>
        /// ID bản ghi
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Ngày dữ liệu
        /// </summary>
        public DateTime NGAY_DL { get; set; }

        /// <summary>
        /// Mã chi nhánh giao dịch
        /// </summary>
        public string? TRBRCD { get; set; }

        /// <summary>
        /// Mã người dùng
        /// </summary>
        public string? USERID { get; set; }

        /// <summary>
        /// Số thứ tự nhật ký
        /// </summary>
        public string? JOURSEQ { get; set; }

        /// <summary>
        /// Số thứ tự giao dịch trong ngày
        /// </summary>
        public string? DYTRSEQ { get; set; }

        /// <summary>
        /// Mã tài khoản
        /// </summary>
        public string? LOCAC { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string? MaKhachHang { get; set; }

        /// <summary>
        /// Số tiền ghi nợ
        /// </summary>
        public decimal? SoTienNo { get; set; }

        /// <summary>
        /// Số tiền ghi có
        /// </summary>
        public decimal? SoTienCo { get; set; }

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime? ThoiGianTao { get; set; }
    }

    /// <summary>
    /// DTO cho chi tiết dữ liệu GL02 - Giao dịch sổ cái
    /// </summary>
    public class GL02DetailDto
    {
        /// <summary>
        /// ID bản ghi
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Ngày dữ liệu
        /// </summary>
        public DateTime NgayDL { get; set; }

        /// <summary>
        /// Mã chi nhánh giao dịch
        /// </summary>
        public string? MaChiNhanh { get; set; }

        /// <summary>
        /// Mã người dùng
        /// </summary>
        public string? MaNguoiDung { get; set; }

        /// <summary>
        /// Sequence nhật ký
        /// </summary>
        public string? JournalSeq { get; set; }

        /// <summary>
        /// Sequence giao dịch ngày
        /// </summary>
        public string? DayTransactionSeq { get; set; }

        /// <summary>
        /// Mã tài khoản
        /// </summary>
        public string? MaTaiKhoan { get; set; }

        /// <summary>
        /// Loại tiền tệ
        /// </summary>
        public string? LoaiTien { get; set; }

        /// <summary>
        /// Mã nghiệp vụ
        /// </summary>
        public string? MaNghiepVu { get; set; }

        /// <summary>
        /// Mã đơn vị
        /// </summary>
        public string? MaDonVi { get; set; }

        /// <summary>
        /// Mã giao dịch
        /// </summary>
        public string? MaGiaoDich { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string? MaKhachHang { get; set; }

        /// <summary>
        /// Loại giao dịch
        /// </summary>
        public string? LoaiGiaoDich { get; set; }

        /// <summary>
        /// Tham chiếu
        /// </summary>
        public string? ThamChieu { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? GhiChu { get; set; }

        /// <summary>
        /// Số tiền ghi nợ
        /// </summary>
        public decimal? SoTienNo { get; set; }

        /// <summary>
        /// Số tiền ghi có
        /// </summary>
        public decimal? SoTienCo { get; set; }

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime? ThoiGianTao { get; set; }

        /// <summary>
        /// Tên file nguồn
        /// </summary>
        public string? TenFileNguon { get; set; }

        /// <summary>
        /// Ngày tạo bản ghi
        /// </summary>
        public DateTime NgayTao { get; set; }

        /// <summary>
        /// Ngày cập nhật bản ghi
        /// </summary>
        public DateTime NgayCapNhat { get; set; }
    }

    /// <summary>
    /// DTO cho tổng hợp dữ liệu GL02 - Giao dịch sổ cái
    /// </summary>
    public class GL02SummaryDto
    {
        /// <summary>
        /// Mã đơn vị
        /// </summary>
        public string? MaDonVi { get; set; }

        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        public string? MaChiNhanh { get; set; }

        /// <summary>
        /// Ngày dữ liệu
        /// </summary>
        public DateTime? NgayDL { get; set; }

        /// <summary>
        /// Tổng số bản ghi
        /// </summary>
        public int TongSoBanGhi { get; set; }

        /// <summary>
        /// Tổng số tiền ghi nợ
        /// </summary>
        public decimal TongTienNo { get; set; }

        /// <summary>
        /// Tổng số tiền ghi có
        /// </summary>
        public decimal TongTienCo { get; set; }

        /// <summary>
        /// Chênh lệch (Nợ - Có)
        /// </summary>
        public decimal ChenhLech => TongTienNo - TongTienCo;

        /// <summary>
        /// Số lượng giao dịch theo chi nhánh
        /// </summary>
        public Dictionary<string, int>? SoLuongTheoChiNhanh { get; set; }

        /// <summary>
        /// Số lượng giao dịch theo đơn vị
        /// </summary>
        public Dictionary<string, int>? SoLuongTheoDonVi { get; set; }

        /// <summary>
        /// Số lượng giao dịch theo loại giao dịch
        /// </summary>
        public Dictionary<string, int>? SoLuongTheoLoaiGiaoDich { get; set; }

        /// <summary>
        /// Tổng tiền theo chi nhánh
        /// </summary>
        public Dictionary<string, decimal>? TongTienTheoChiNhanh { get; set; }

        /// <summary>
        /// Tổng tiền theo đơn vị
        /// </summary>
        public Dictionary<string, decimal>? TongTienTheoDonVi { get; set; }
    }

    /// <summary>
    /// DTO cho tìm kiếm dữ liệu GL02 - Giao dịch sổ cái
    /// </summary>
    public class GL02SearchDto
    {
        /// <summary>
        /// Từ khóa tìm kiếm
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        public string? MaChiNhanh { get; set; }

        /// <summary>
        /// Mã đơn vị
        /// </summary>
        public string? MaDonVi { get; set; }

        /// <summary>
        /// Mã tài khoản
        /// </summary>
        public string? MaTaiKhoan { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string? MaKhachHang { get; set; }

        /// <summary>
        /// Từ ngày
        /// </summary>
        public DateTime? TuNgay { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        public DateTime? DenNgay { get; set; }

        /// <summary>
        /// Loại giao dịch (Nợ/Có)
        /// </summary>
        public string? LoaiGiaoDich { get; set; }

        /// <summary>
        /// Số trang
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Số bản ghi mỗi trang
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
}
