using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Repositories.Interfaces
{
    /// <summary>
    /// Interface cho DPDA Repository - Thẻ nộp đơn gửi tiết kiệm
    /// Extend IRepository<DPDA> với các business methods cho DPDA data
    /// CSV Structure: 13 business columns - MA_CHI_NHANH, MA_KHACH_HANG, TEN_KHACH_HANG, SO_TAI_KHOAN, LOAI_THE, SO_THE, NGAY_NOP_DON, NGAY_PHAT_HANH, USER_PHAT_HANH, TRANG_THAI, PHAN_LOAI, GIAO_THE, LOAI_PHAT_HANH
    /// </summary>
    public interface IDPDARepository : IRepository<DPDA>
    {
        /// <summary>
        /// Lấy danh sách DPDA theo ngày dữ liệu
        /// </summary>
        /// <param name="ngayDl">Ngày dữ liệu (YYYY-MM-DD)</param>
        /// <returns>Danh sách DPDA records</returns>
        Task<IEnumerable<DPDA>> GetByDateAsync(DateTime ngayDl);

        /// <summary>
        /// Lấy danh sách DPDA theo mã chi nhánh
        /// </summary>
        /// <param name="maChiNhanh">Mã chi nhánh</param>
        /// <returns>Danh sách DPDA theo chi nhánh</returns>
        Task<IEnumerable<DPDA>> GetByBranchCodeAsync(string maChiNhanh);

        /// <summary>
        /// Lấy danh sách DPDA theo mã khách hàng
        /// </summary>
        /// <param name="maKhachHang">Mã khách hàng</param>
        /// <returns>Danh sách DPDA theo khách hàng</returns>
        Task<IEnumerable<DPDA>> GetByCustomerCodeAsync(string maKhachHang);

        /// <summary>
        /// Lấy danh sách DPDA theo số tài khoản
        /// </summary>
        /// <param name="soTaiKhoan">Số tài khoản</param>
        /// <returns>Danh sách DPDA theo tài khoản</returns>
        Task<IEnumerable<DPDA>> GetByAccountNumberAsync(string soTaiKhoan);

        /// <summary>
        /// Lấy danh sách DPDA theo trạng thái thẻ
        /// </summary>
        /// <param name="trangThai">Trạng thái thẻ</param>
        /// <returns>Danh sách DPDA theo trạng thái</returns>
        Task<IEnumerable<DPDA>> GetByCardStatusAsync(string trangThai);

        /// <summary>
        /// Lấy danh sách DPDA theo loại thẻ
        /// </summary>
        /// <param name="loaiThe">Loại thẻ</param>
        /// <returns>Danh sách DPDA theo loại thẻ</returns>
        Task<IEnumerable<DPDA>> GetByCardTypeAsync(string loaiThe);

        /// <summary>
        /// Lấy thống kê DPDA theo ngày
        /// </summary>
        /// <param name="ngayDl">Ngày dữ liệu</param>
        /// <returns>Số lượng records theo ngày</returns>
        Task<int> GetCountByDateAsync(DateTime ngayDl);
    }
}
