using TinhKhoanApp.Api.Models.NguonVon;

namespace TinhKhoanApp.Api.Services.Interfaces
{
    public interface IRawDataService
    {
        /// <summary>
        /// Tính toán nguồn vốn từ dữ liệu thô (Direct Import Tables)
        /// </summary>
        /// <param name="request">Thông tin yêu cầu tính toán</param>
        /// <returns>Kết quả tính toán nguồn vốn</returns>
        Task<NguonVonDetails> CalculateNguonVonFromRawDataAsync(NguonVonRequest request);
    }
}
