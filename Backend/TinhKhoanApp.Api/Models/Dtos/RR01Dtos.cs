using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.DTOs
{
    // DTO for detailed RR01 information (all business columns)
    public class RR01DetailsDto
    {
        public long Id { get; set; }

        [StringLength(50, ErrorMessage = "CN_LOAI_I không được vượt quá 50 ký tự")]
        public string? CnLoaiI { get; set; }

        [StringLength(20, ErrorMessage = "BRCD không được vượt quá 20 ký tự")]
        public string? Brcd { get; set; }

        [StringLength(50, ErrorMessage = "MA_KH không được vượt quá 50 ký tự")]
        public string? MaKh { get; set; }

        [StringLength(255, ErrorMessage = "TEN_KH không được vượt quá 255 ký tự")]
        public string? TenKh { get; set; }

        [StringLength(50, ErrorMessage = "SO_LDS không được vượt quá 50 ký tự")]
        public string? SoLds { get; set; }

        [StringLength(10, ErrorMessage = "CCY không được vượt quá 10 ký tự")]
        public string? Ccy { get; set; }

        [StringLength(50, ErrorMessage = "SO_LAV không được vượt quá 50 ký tự")]
        public string? SoLav { get; set; }

        [StringLength(20, ErrorMessage = "LOAI_KH không được vượt quá 20 ký tự")]
        public string? LoaiKh { get; set; }

        [StringLength(20, ErrorMessage = "NGAY_GIAI_NGAN không được vượt quá 20 ký tự")]
        public string? NgayGiaiNgan { get; set; }

        [StringLength(20, ErrorMessage = "NGAY_DAO_HAN không được vượt quá 20 ký tự")]
        public string? NgayDaoHan { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "SO_DU_NO phải là số dương")]
        public decimal? SoDuNo { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "SO_DU_NO_VND phải là số dương")]
        public decimal? SoDuNoVnd { get; set; }

        [StringLength(20, ErrorMessage = "LOAI_TS không được vượt quá 20 ký tự")]
        public string? LoaiTs { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "GIA_TRI_TS_DAM_BAO phải là số dương")]
        public decimal? GiaTriTsDamBao { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "GIA_TRI_TS_DAM_BAO_VND phải là số dương")]
        public decimal? GiaTriTsDamBaoVnd { get; set; }

        [Range(0, 100, ErrorMessage = "TY_LE_DAM_BAO phải từ 0 đến 100")]
        public decimal? TyLeDamBao { get; set; }

        [StringLength(50, ErrorMessage = "LOAI_TS_KHAC không được vượt quá 50 ký tự")]
        public string? LoaiTsKhac { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "GIA_TRI_TS_KHAC phải là số dương")]
        public decimal? GiaTriTsKhac { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "GIA_TRI_TS_KHAC_VND phải là số dương")]
        public decimal? GiaTriTsKhacVnd { get; set; }

        [Range(0, 100, ErrorMessage = "TY_LE_DAM_BAO_KHAC phải từ 0 đến 100")]
        public decimal? TyLeDamBaoKhac { get; set; }

        [StringLength(20, ErrorMessage = "MUC_PHAN_LOAI không được vượt quá 20 ký tự")]
        public string? MucPhanLoai { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "SO_TIEN_DU_PHONG phải là số dương")]
        public decimal? SoTienDuPhong { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "SO_TIEN_DU_PHONG_VND phải là số dương")]
        public decimal? SoTienDuPhongVnd { get; set; }

        [StringLength(500, ErrorMessage = "GHI_CHU không được vượt quá 500 ký tự")]
        public string? GhiChu { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "GIA_TRI_TAI_SAN_THE_CHAP phải là số dương")]
        public decimal? GiaTriTaiSanTheChap { get; set; }

        // System fields
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    // DTO for creating new RR01 (business columns only)
    public class RR01CreateDto
    {
        [StringLength(50, ErrorMessage = "CN_LOAI_I không được vượt quá 50 ký tự")]
        public string? CnLoaiI { get; set; }

        [StringLength(20, ErrorMessage = "BRCD không được vượt quá 20 ký tự")]
        public string? Brcd { get; set; }

        [StringLength(50, ErrorMessage = "MA_KH không được vượt quá 50 ký tự")]
        public string? MaKh { get; set; }

        [StringLength(255, ErrorMessage = "TEN_KH không được vượt quá 255 ký tự")]
        public string? TenKh { get; set; }

        [StringLength(50, ErrorMessage = "SO_LDS không được vượt quá 50 ký tự")]
        public string? SoLds { get; set; }

        [StringLength(10, ErrorMessage = "CCY không được vượt quá 10 ký tự")]
        public string? Ccy { get; set; }

        [StringLength(50, ErrorMessage = "SO_LAV không được vượt quá 50 ký tự")]
        public string? SoLav { get; set; }

        [StringLength(20, ErrorMessage = "LOAI_KH không được vượt quá 20 ký tự")]
        public string? LoaiKh { get; set; }

        [StringLength(20, ErrorMessage = "NGAY_GIAI_NGAN không được vượt quá 20 ký tự")]
        public string? NgayGiaiNgan { get; set; }

        [StringLength(20, ErrorMessage = "NGAY_DAO_HAN không được vượt quá 20 ký tự")]
        public string? NgayDaoHan { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "SO_DU_NO phải là số dương")]
        public decimal? SoDuNo { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "SO_DU_NO_VND phải là số dương")]
        public decimal? SoDuNoVnd { get; set; }

        [StringLength(20, ErrorMessage = "LOAI_TS không được vượt quá 20 ký tự")]
        public string? LoaiTs { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "GIA_TRI_TS_DAM_BAO phải là số dương")]
        public decimal? GiaTriTsDamBao { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "GIA_TRI_TS_DAM_BAO_VND phải là số dương")]
        public decimal? GiaTriTsDamBaoVnd { get; set; }

        [Range(0, 100, ErrorMessage = "TY_LE_DAM_BAO phải từ 0 đến 100")]
        public decimal? TyLeDamBao { get; set; }

        [StringLength(50, ErrorMessage = "LOAI_TS_KHAC không được vượt quá 50 ký tự")]
        public string? LoaiTsKhac { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "GIA_TRI_TS_KHAC phải là số dương")]
        public decimal? GiaTriTsKhac { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "GIA_TRI_TS_KHAC_VND phải là số dương")]
        public decimal? GiaTriTsKhacVnd { get; set; }

        [Range(0, 100, ErrorMessage = "TY_LE_DAM_BAO_KHAC phải từ 0 đến 100")]
        public decimal? TyLeDamBaoKhac { get; set; }

        [StringLength(20, ErrorMessage = "MUC_PHAN_LOAI không được vượt quá 20 ký tự")]
        public string? MucPhanLoai { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "SO_TIEN_DU_PHONG phải là số dương")]
        public decimal? SoTienDuPhong { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "SO_TIEN_DU_PHONG_VND phải là số dương")]
        public decimal? SoTienDuPhongVnd { get; set; }

        [StringLength(500, ErrorMessage = "GHI_CHU không được vượt quá 500 ký tự")]
        public string? GhiChu { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "GIA_TRI_TAI_SAN_THE_CHAP phải là số dương")]
        public decimal? GiaTriTaiSanTheChap { get; set; }
    }

    // DTO for updating existing RR01 (partial)
    public class RR01UpdateDto
    {
        [StringLength(50, ErrorMessage = "CN_LOAI_I không được vượt quá 50 ký tự")]
        public string? CnLoaiI { get; set; }

        [StringLength(20, ErrorMessage = "BRCD không được vượt quá 20 ký tự")]
        public string? Brcd { get; set; }

        [StringLength(50, ErrorMessage = "MA_KH không được vượt quá 50 ký tự")]
        public string? MaKh { get; set; }

        [StringLength(255, ErrorMessage = "TEN_KH không được vượt quá 255 ký tự")]
        public string? TenKh { get; set; }

        [StringLength(50, ErrorMessage = "SO_LDS không được vượt quá 50 ký tự")]
        public string? SoLds { get; set; }

        [StringLength(10, ErrorMessage = "CCY không được vượt quá 10 ký tự")]
        public string? Ccy { get; set; }

        [StringLength(50, ErrorMessage = "SO_LAV không được vượt quá 50 ký tự")]
        public string? SoLav { get; set; }

        [StringLength(20, ErrorMessage = "LOAI_KH không được vượt quá 20 ký tự")]
        public string? LoaiKh { get; set; }

        [StringLength(20, ErrorMessage = "NGAY_GIAI_NGAN không được vượt quá 20 ký tự")]
        public string? NgayGiaiNgan { get; set; }

        [StringLength(20, ErrorMessage = "NGAY_DAO_HAN không được vượt quá 20 ký tự")]
        public string? NgayDaoHan { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "SO_DU_NO phải là số dương")]
        public decimal? SoDuNo { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "SO_DU_NO_VND phải là số dương")]
        public decimal? SoDuNoVnd { get; set; }

        [StringLength(20, ErrorMessage = "LOAI_TS không được vượt quá 20 ký tự")]
        public string? LoaiTs { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "GIA_TRI_TS_DAM_BAO phải là số dương")]
        public decimal? GiaTriTsDamBao { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "GIA_TRI_TS_DAM_BAO_VND phải là số dương")]
        public decimal? GiaTriTsDamBaoVnd { get; set; }

        [Range(0, 100, ErrorMessage = "TY_LE_DAM_BAO phải từ 0 đến 100")]
        public decimal? TyLeDamBao { get; set; }

        [StringLength(50, ErrorMessage = "LOAI_TS_KHAC không được vượt quá 50 ký tự")]
        public string? LoaiTsKhac { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "GIA_TRI_TS_KHAC phải là số dương")]
        public decimal? GiaTriTsKhac { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "GIA_TRI_TS_KHAC_VND phải là số dương")]
        public decimal? GiaTriTsKhacVnd { get; set; }

        [Range(0, 100, ErrorMessage = "TY_LE_DAM_BAO_KHAC phải từ 0 đến 100")]
        public decimal? TyLeDamBaoKhac { get; set; }

        [StringLength(20, ErrorMessage = "MUC_PHAN_LOAI không được vượt quá 20 ký tự")]
        public string? MucPhanLoai { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "SO_TIEN_DU_PHONG phải là số dương")]
        public decimal? SoTienDuPhong { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "SO_TIEN_DU_PHONG_VND phải là số dương")]
        public decimal? SoTienDuPhongVnd { get; set; }

        [StringLength(500, ErrorMessage = "GHI_CHU không được vượt quá 500 ký tự")]
        public string? GhiChu { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "GIA_TRI_TAI_SAN_THE_CHAP phải là số dương")]
        public decimal? GiaTriTaiSanTheChap { get; set; }
    }

    // DTO for preview/list display (key fields only)
    public class RR01PreviewDto
    {
        public long Id { get; set; }
        public string? Brcd { get; set; }
        public string? MaKh { get; set; }
        public string? TenKh { get; set; }
        public string? LoaiKh { get; set; }
        public decimal? SoDuNo { get; set; }
        public decimal? SoDuNoVnd { get; set; }
        public string? MucPhanLoai { get; set; }
        public decimal? SoTienDuPhong { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // DTO for summary statistics
    public class RR01SummaryDto
    {
        public int TotalCount { get; set; }
        public decimal TotalSoDuNo { get; set; }
        public decimal TotalSoDuNoVnd { get; set; }
        public decimal TotalSoTienDuPhong { get; set; }
        public decimal TotalSoTienDuPhongVnd { get; set; }
        public int CountByLoaiKH_CaNhan { get; set; }
        public int CountByLoaiKH_ToChuc { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
