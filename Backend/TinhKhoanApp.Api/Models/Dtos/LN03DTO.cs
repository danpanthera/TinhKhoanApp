using System.ComponentModel.DataAnnotations;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Models.DTOs
{
    /// <summary>
    /// DTO cho dữ liệu LN03 (Dữ liệu phục hồi khoản vay)
    /// </summary>
    public class LN03DTO
    {
        /// <summary>
        /// Id của bản ghi
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Ngày dữ liệu
        /// </summary>
        public DateTime NgayDL { get; set; }

        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        public string? MaChiNhanh { get; set; }

        /// <summary>
        /// Tên chi nhánh
        /// </summary>
        public string? TenChiNhanh { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string? MaKH { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string? TenKH { get; set; }

        /// <summary>
        /// Số hợp đồng
        /// </summary>
        public string? SoHopDong { get; set; }

        /// <summary>
        /// Số tiền xử lý rủi ro
        /// </summary>
        public decimal? SoTienXLRR { get; set; }

        /// <summary>
        /// Ngày phát sinh xử lý
        /// </summary>
        public DateTime? NgayPhatSinhXL { get; set; }

        /// <summary>
        /// Thu nợ sau xử lý
        /// </summary>
        public decimal? ThuNoSauXL { get; set; }

        /// <summary>
        /// Còn lại ngoài bảng
        /// </summary>
        public decimal? ConLaiNgoaiBang { get; set; }

        /// <summary>
        /// Dư nợ nội bảng
        /// </summary>
        public decimal? DuNoNoiBang { get; set; }

        /// <summary>
        /// Nhóm nợ
        /// </summary>
        public string? NhomNo { get; set; }

        /// <summary>
        /// Mã cán bộ tín dụng
        /// </summary>
        public string? MaCBTD { get; set; }

        /// <summary>
        /// Tên cán bộ tín dụng
        /// </summary>
        public string? TenCBTD { get; set; }

        /// <summary>
        /// Mã phòng giao dịch
        /// </summary>
        public string? MaPGD { get; set; }

        /// <summary>
        /// Tài khoản hạch toán
        /// </summary>
        public string? TaiKhoanHachToan { get; set; }

        /// <summary>
        /// Số tham chiếu
        /// </summary>
        public string? RefNo { get; set; }

        /// <summary>
        /// Loại nguồn vốn
        /// </summary>
        public string? LoaiNguonVon { get; set; }

        /// <summary>
        /// Cột 18 - Không có header trong CSV
        /// </summary>
        public string? Column18 { get; set; }

        /// <summary>
        /// Cột 19 - Không có header trong CSV
        /// </summary>
        public string? Column19 { get; set; }

        /// <summary>
        /// Cột 20 - Không có header trong CSV (số tiền)
        /// </summary>
        public decimal? Column20 { get; set; }

        /// <summary>
        /// Ngày tạo bản ghi
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Ngày cập nhật bản ghi
        /// </summary>
        public DateTime UpdatedDate { get; set; }

        /// <summary>
        /// Tên file nguồn
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Map từ entity sang DTO
        /// </summary>
        public static LN03DTO FromEntity(LN03 entity)
        {
            return new LN03DTO
            {
                Id = entity.Id,
                NgayDL = entity.NGAY_DL,
                MaChiNhanh = entity.MACHINHANH,
                TenChiNhanh = entity.TENCHINHANH,
                MaKH = entity.MAKH,
                TenKH = entity.TENKH,
                SoHopDong = entity.SOHOPDONG,
                SoTienXLRR = entity.SOTIENXLRR,
                NgayPhatSinhXL = entity.NGAYPHATSINHXL,
                ThuNoSauXL = entity.THUNOSAUXL,
                ConLaiNgoaiBang = entity.CONLAINGOAIBANG,
                DuNoNoiBang = entity.DUNONOIBANG,
                NhomNo = entity.NHOMNO,
                MaCBTD = entity.MACBTD,
                TenCBTD = entity.TENCBTD,
                MaPGD = entity.MAPGD,
                TaiKhoanHachToan = entity.TAIKHOANHACHTOAN,
                RefNo = entity.REFNO,
                LoaiNguonVon = entity.LOAINGUONVON,
                Column18 = entity.COLUMN_18,
                Column19 = entity.COLUMN_19,
                Column20 = entity.COLUMN_20,
                CreatedDate = entity.CREATED_DATE,
                UpdatedDate = entity.UPDATED_DATE,
                FileName = entity.FILE_NAME
            };
        }

        /// <summary>
        /// Chuyển đổi sang entity
        /// </summary>
        public LN03 ToEntity()
        {
            return new LN03
            {
                NGAY_DL = this.NgayDL,
                MACHINHANH = this.MaChiNhanh ?? string.Empty,
                TENCHINHANH = this.TenChiNhanh ?? string.Empty,
                MAKH = this.MaKH ?? string.Empty,
                TENKH = this.TenKH ?? string.Empty,
                SOHOPDONG = this.SoHopDong ?? string.Empty,
                SOTIENXLRR = this.SoTienXLRR,
                NGAYPHATSINHXL = this.NgayPhatSinhXL,
                THUNOSAUXL = this.ThuNoSauXL,
                CONLAINGOAIBANG = this.ConLaiNgoaiBang,
                DUNONOIBANG = this.DuNoNoiBang,
                NHOMNO = this.NhomNo ?? string.Empty,
                MACBTD = this.MaCBTD ?? string.Empty,
                TENCBTD = this.TenCBTD ?? string.Empty,
                MAPGD = this.MaPGD ?? string.Empty,
                TAIKHOANHACHTOAN = this.TaiKhoanHachToan ?? string.Empty,
                REFNO = this.RefNo ?? string.Empty,
                LOAINGUONVON = this.LoaiNguonVon ?? string.Empty,
                CREATED_DATE = DateTime.Now,
                UPDATED_DATE = DateTime.Now,
                FILE_NAME = $"MANUAL_CREATION_{DateTime.Now:yyyyMMdd}"
            };
        }
    }

    /// <summary>
    /// DTO cho việc cập nhật LN03
    /// </summary>
    public class UpdateLN03DTO
    {
        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        [StringLength(200)]
        public string? MaChiNhanh { get; set; }

        /// <summary>
        /// Tên chi nhánh
        /// </summary>
        [StringLength(200)]
        public string? TenChiNhanh { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        [StringLength(200)]
        public string? MaKH { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        [StringLength(200)]
        public string? TenKH { get; set; }

        /// <summary>
        /// Số hợp đồng
        /// </summary>
        [StringLength(200)]
        public string? SoHopDong { get; set; }

        /// <summary>
        /// Số tiền xử lý rủi ro
        /// </summary>
        public decimal? SoTienXLRR { get; set; }

        /// <summary>
        /// Ngày phát sinh xử lý
        /// </summary>
        public DateTime? NgayPhatSinhXL { get; set; }

        /// <summary>
        /// Thu nợ sau xử lý
        /// </summary>
        public decimal? ThuNoSauXL { get; set; }

        /// <summary>
        /// Còn lại ngoài bảng
        /// </summary>
        public decimal? ConLaiNgoaiBang { get; set; }

        /// <summary>
        /// Dư nợ nội bảng
        /// </summary>
        public decimal? DuNoNoiBang { get; set; }

        /// <summary>
        /// Nhóm nợ
        /// </summary>
        [StringLength(200)]
        public string? NhomNo { get; set; }

        /// <summary>
        /// Mã cán bộ tín dụng
        /// </summary>
        [StringLength(200)]
        public string? MaCBTD { get; set; }

        /// <summary>
        /// Tên cán bộ tín dụng
        /// </summary>
        [StringLength(200)]
        public string? TenCBTD { get; set; }

        /// <summary>
        /// Mã phòng giao dịch
        /// </summary>
        [StringLength(200)]
        public string? MaPGD { get; set; }

        /// <summary>
        /// Tài khoản hạch toán
        /// </summary>
        [StringLength(200)]
        public string? TaiKhoanHachToan { get; set; }

        /// <summary>
        /// Số tham chiếu
        /// </summary>
        [StringLength(200)]
        public string? RefNo { get; set; }

        /// <summary>
        /// Loại nguồn vốn
        /// </summary>
        [StringLength(200)]
        public string? LoaiNguonVon { get; set; }

        /// <summary>
        /// Cập nhật entity hiện có
        /// </summary>
        public void UpdateEntity(LN03 entity)
        {
            if (!string.IsNullOrEmpty(MaChiNhanh)) entity.MACHINHANH = MaChiNhanh;
            if (!string.IsNullOrEmpty(TenChiNhanh)) entity.TENCHINHANH = TenChiNhanh;
            if (!string.IsNullOrEmpty(MaKH)) entity.MAKH = MaKH;
            if (!string.IsNullOrEmpty(TenKH)) entity.TENKH = TenKH;
            if (!string.IsNullOrEmpty(SoHopDong)) entity.SOHOPDONG = SoHopDong;

            // Các trường kiểu decimal
            entity.SOTIENXLRR = SoTienXLRR;
            entity.THUNOSAUXL = ThuNoSauXL;
            entity.CONLAINGOAIBANG = ConLaiNgoaiBang;
            entity.DUNONOIBANG = DuNoNoiBang;

            // Trường kiểu DateTime
            entity.NGAYPHATSINHXL = NgayPhatSinhXL;

            if (!string.IsNullOrEmpty(NhomNo)) entity.NHOMNO = NhomNo;
            if (!string.IsNullOrEmpty(MaCBTD)) entity.MACBTD = MaCBTD;
            if (!string.IsNullOrEmpty(TenCBTD)) entity.TENCBTD = TenCBTD;
            if (!string.IsNullOrEmpty(MaPGD)) entity.MAPGD = MaPGD;
            if (!string.IsNullOrEmpty(TaiKhoanHachToan)) entity.TAIKHOANHACHTOAN = TaiKhoanHachToan;
            if (!string.IsNullOrEmpty(RefNo)) entity.REFNO = RefNo;
            if (!string.IsNullOrEmpty(LoaiNguonVon)) entity.LOAINGUONVON = LoaiNguonVon;

            entity.UPDATED_DATE = DateTime.Now;
        }
    }
}

/// <summary>
/// DTO cho việc tạo mới LN03
/// </summary>
public class CreateLN03DTO
{
    /// <summary>
    /// Ngày dữ liệu
    /// </summary>
    [Required]
    public DateTime NgayDL { get; set; } = DateTime.Now;

    /// <summary>
    /// Mã chi nhánh
    /// </summary>
    [Required]
    [StringLength(200)]
    public string MaChiNhanh { get; set; } = string.Empty;

    /// <summary>
    /// Tên chi nhánh
    /// </summary>
    [StringLength(200)]
    public string TenChiNhanh { get; set; } = string.Empty;

    /// <summary>
    /// Mã khách hàng
    /// </summary>
    [Required]
    [StringLength(200)]
    public string MaKH { get; set; } = string.Empty;

    /// <summary>
    /// Tên khách hàng
    /// </summary>
    [StringLength(200)]
    public string TenKH { get; set; } = string.Empty;

    /// <summary>
    /// Số hợp đồng
    /// </summary>
    [Required]
    [StringLength(200)]
    public string SoHopDong { get; set; } = string.Empty;

    /// <summary>
    /// Số tiền xử lý rủi ro
    /// </summary>
    public decimal? SoTienXLRR { get; set; }

    /// <summary>
    /// Ngày phát sinh xử lý
    /// </summary>
    public DateTime? NgayPhatSinhXL { get; set; }

    /// <summary>
    /// Thu nợ sau xử lý
    /// </summary>
    public decimal? ThuNoSauXL { get; set; }

    /// <summary>
    /// Còn lại ngoài bảng
    /// </summary>
    public decimal? ConLaiNgoaiBang { get; set; }

    /// <summary>
    /// Dư nợ nội bảng
    /// </summary>
    public decimal? DuNoNoiBang { get; set; }

    /// <summary>
    /// Nhóm nợ
    /// </summary>
    [StringLength(200)]
    public string? NhomNo { get; set; }

    /// <summary>
    /// Mã cán bộ tín dụng
    /// </summary>
    [StringLength(200)]
    public string? MaCBTD { get; set; }

    /// <summary>
    /// Tên cán bộ tín dụng
    /// </summary>
    [StringLength(200)]
    public string? TenCBTD { get; set; }

    /// <summary>
    /// Mã phòng giao dịch
    /// </summary>
    [StringLength(200)]
    public string? MaPGD { get; set; }

    /// <summary>
    /// Tài khoản hạch toán
    /// </summary>
    [StringLength(200)]
    public string? TaiKhoanHachToan { get; set; }

    /// <summary>
    /// Số tham chiếu
    /// </summary>
    [StringLength(200)]
    public string? RefNo { get; set; }

    /// <summary>
    /// Loại nguồn vốn
    /// </summary>
    [StringLength(200)]
    public string? LoaiNguonVon { get; set; }

    /// <summary>
    /// Chuyển đổi sang entity
    /// </summary>
    public LN03 ToEntity()
    {
        return new LN03
        {
            NGAY_DL = this.NgayDL,
            MACHINHANH = this.MaChiNhanh,
            TENCHINHANH = this.TenChiNhanh,
            MAKH = this.MaKH,
            TENKH = this.TenKH ?? string.Empty,
            SOHOPDONG = this.SoHopDong,
            SOTIENXLRR = this.SoTienXLRR,
            NGAYPHATSINHXL = this.NgayPhatSinhXL,
            THUNOSAUXL = this.ThuNoSauXL,
            CONLAINGOAIBANG = this.ConLaiNgoaiBang,
            DUNONOIBANG = this.DuNoNoiBang,
            NHOMNO = this.NhomNo ?? string.Empty,
            MACBTD = this.MaCBTD ?? string.Empty,
            TENCBTD = this.TenCBTD ?? string.Empty,
            MAPGD = this.MaPGD ?? string.Empty,
            TAIKHOANHACHTOAN = this.TaiKhoanHachToan ?? string.Empty,
            REFNO = this.RefNo ?? string.Empty,
            LOAINGUONVON = this.LoaiNguonVon ?? string.Empty,
            CREATED_DATE = DateTime.Now,
            UPDATED_DATE = DateTime.Now,
            FILE_NAME = $"MANUAL_CREATION_{DateTime.Now:yyyyMMdd}"
        };
    }
}
