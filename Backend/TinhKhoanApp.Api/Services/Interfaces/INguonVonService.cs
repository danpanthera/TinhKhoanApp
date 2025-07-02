using System;
using System.Threading.Tasks;
using TinhKhoanApp.Api.Models.NguonVon;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    /// <summary>
    /// Interface cho service tính toán nguồn vốn từ bảng DP01
    /// </summary>
    public interface INguonVonService
    {
        /// <summary>
        /// Tính toán nguồn vốn theo đơn vị và ngày
        /// </summary>
        /// <param name="unitCode">Mã đơn vị</param>
        /// <param name="targetDate">Ngày cần tính</param>
        /// <returns>Kết quả tính toán hoặc null nếu không có dữ liệu</returns>
        Task<NguonVonResult> CalculateNguonVonAsync(string unitCode, DateTime targetDate);

        /// <summary>
        /// Lấy chi tiết nguồn vốn theo đơn vị để debug
        /// </summary>
        /// <param name="unitCode">Mã đơn vị</param>
        /// <param name="targetDate">Ngày cần tính</param>
        /// <returns>Chi tiết kết quả tính toán</returns>
        Task<NguonVonDetails> GetNguonVonDetailsAsync(string unitCode, DateTime targetDate);

        /// <summary>
        /// Xác định ngày target dựa trên loại input
        /// </summary>
        /// <param name="inputDate">Ngày input</param>
        /// <param name="dateType">Loại ngày: year/month/day</param>
        /// <returns>Ngày cần tìm dữ liệu</returns>
        DateTime DetermineTargetDate(DateTime inputDate, string dateType);
    }
}
