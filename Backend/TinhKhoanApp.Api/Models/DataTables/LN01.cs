using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng LN01 - Auto-generated from database structure
    /// Generated: $(date '+%Y-%m-%d %H:%M:%S')
    /// Temporal Table with History tracking
    /// </summary>
    [Table("LN01")]
    public class LN01
    {
        // Column: Id, Type: bigint
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        // Column: NGAY_DL, Type: date
        [Column("NGAY_DL")]
        public DateTime NGAY_DL { get; set; }

        // Column: MA_CN, Type: nvarchar
        [Column("MA_CN")]
        public string? MA_CN { get; set; }

        // Column: MA_KH, Type: nvarchar
        [Column("MA_KH")]
        public string? MA_KH { get; set; }

        // Column: TEN_KH, Type: nvarchar
        [Column("TEN_KH")]
        public string? TEN_KH { get; set; }

        // Column: LOAI_KH, Type: nvarchar
        [Column("LOAI_KH")]
        public string? LOAI_KH { get; set; }

        // Column: SO_HSV, Type: nvarchar
        [Column("SO_HSV")]
        public string? SO_HSV { get; set; }

        // Column: SO_TD, Type: nvarchar
        [Column("SO_TD")]
        public string? SO_TD { get; set; }

        // Column: TEN_SP, Type: nvarchar
        [Column("TEN_SP")]
        public string? TEN_SP { get; set; }

        // Column: LOAI_TIEN, Type: nvarchar
        [Column("LOAI_TIEN")]
        public string? LOAI_TIEN { get; set; }

        // Column: SO_TIEN_VAY, Type: decimal
        [Column("SO_TIEN_VAY")]
        public decimal? SO_TIEN_VAY { get; set; }

        // Column: LS_VAY, Type: decimal
        [Column("LS_VAY")]
        public decimal? LS_VAY { get; set; }

        // Column: NGAY_TT_GOC, Type: nvarchar
        [Column("NGAY_TT_GOC")]
        public string? NGAY_TT_GOC { get; set; }

        // Column: NGAY_TT_LAI, Type: nvarchar
        [Column("NGAY_TT_LAI")]
        public string? NGAY_TT_LAI { get; set; }

        // Column: DU_NO_GOC, Type: decimal
        [Column("DU_NO_GOC")]
        public decimal? DU_NO_GOC { get; set; }

        // Column: DU_NO_LAI, Type: decimal
        [Column("DU_NO_LAI")]
        public decimal? DU_NO_LAI { get; set; }

        // Column: DU_NO_PHI, Type: decimal
        [Column("DU_NO_PHI")]
        public decimal? DU_NO_PHI { get; set; }

        // Column: NGAY_HIEU_LUC, Type: nvarchar
        [Column("NGAY_HIEU_LUC")]
        public string? NGAY_HIEU_LUC { get; set; }

        // Column: NGAY_DAO_HAN, Type: nvarchar
        [Column("NGAY_DAO_HAN")]
        public string? NGAY_DAO_HAN { get; set; }

        // Column: MHANG_BAO_DAM, Type: nvarchar
        [Column("MHANG_BAO_DAM")]
        public string? MHANG_BAO_DAM { get; set; }

        // Column: LOAI_BD, Type: nvarchar
        [Column("LOAI_BD")]
        public string? LOAI_BD { get; set; }

        // Column: TH_CVAY, Type: nvarchar
        [Column("TH_CVAY")]
        public string? TH_CVAY { get; set; }

        // Column: KH_NO_NHOM, Type: nvarchar
        [Column("KH_NO_NHOM")]
        public string? KH_NO_NHOM { get; set; }

        // Column: PHAN_LOAI, Type: nvarchar
        [Column("PHAN_LOAI")]
        public string? PHAN_LOAI { get; set; }

        // Column: LY_DO_PHAN_LOAI, Type: nvarchar
        [Column("LY_DO_PHAN_LOAI")]
        public string? LY_DO_PHAN_LOAI { get; set; }

        // Column: TRANG_THAI_HD, Type: nvarchar
        [Column("TRANG_THAI_HD")]
        public string? TRANG_THAI_HD { get; set; }

        // Column: MUCTICH_VAY, Type: nvarchar
        [Column("MUCTICH_VAY")]
        public string? MUCTICH_VAY { get; set; }

        // Column: NGANH_KT, Type: nvarchar
        [Column("NGANH_KT")]
        public string? NGANH_KT { get; set; }

        // Column: MA_CAN_BO, Type: nvarchar
        [Column("MA_CAN_BO")]
        public string? MA_CAN_BO { get; set; }

        // Column: TEN_CAN_BO, Type: nvarchar
        [Column("TEN_CAN_BO")]
        public string? TEN_CAN_BO { get; set; }

        // Column: MA_NV, Type: nvarchar
        [Column("MA_NV")]
        public string? MA_NV { get; set; }

        // Column: TEN_NV, Type: nvarchar
        [Column("TEN_NV")]
        public string? TEN_NV { get; set; }

        // Column: MA_PHONG, Type: nvarchar
        [Column("MA_PHONG")]
        public string? MA_PHONG { get; set; }

        // Column: TEN_PHONG, Type: nvarchar
        [Column("TEN_PHONG")]
        public string? TEN_PHONG { get; set; }

        // Column: SO_CMT, Type: nvarchar
        [Column("SO_CMT")]
        public string? SO_CMT { get; set; }

        // Column: NGAY_CAP_CMT, Type: nvarchar
        [Column("NGAY_CAP_CMT")]
        public string? NGAY_CAP_CMT { get; set; }

        // Column: NOI_CAP_CMT, Type: nvarchar
        [Column("NOI_CAP_CMT")]
        public string? NOI_CAP_CMT { get; set; }

        // Column: NGAY_SINH, Type: nvarchar
        [Column("NGAY_SINH")]
        public string? NGAY_SINH { get; set; }

        // Column: GIOI_TINH, Type: nvarchar
        [Column("GIOI_TINH")]
        public string? GIOI_TINH { get; set; }

        // Column: DIA_CHI, Type: nvarchar
        [Column("DIA_CHI")]
        public string? DIA_CHI { get; set; }

        // Column: SO_DT, Type: nvarchar
        [Column("SO_DT")]
        public string? SO_DT { get; set; }

        // Column: EMAIL, Type: nvarchar
        [Column("EMAIL")]
        public string? EMAIL { get; set; }

        // Column: NGHE_NGHIEP, Type: nvarchar
        [Column("NGHE_NGHIEP")]
        public string? NGHE_NGHIEP { get; set; }

        // Column: LOAI_HO, Type: nvarchar
        [Column("LOAI_HO")]
        public string? LOAI_HO { get; set; }

        // Column: QUAN_HE_VOU, Type: nvarchar
        [Column("QUAN_HE_VOU")]
        public string? QUAN_HE_VOU { get; set; }

        // Column: HON_NHAN, Type: nvarchar
        [Column("HON_NHAN")]
        public string? HON_NHAN { get; set; }

        // Column: LOAI_CTVAY, Type: nvarchar
        [Column("LOAI_CTVAY")]
        public string? LOAI_CTVAY { get; set; }

        // Column: TEN_CTVAY, Type: nvarchar
        [Column("TEN_CTVAY")]
        public string? TEN_CTVAY { get; set; }

        // Column: SO_CMT_CTVAY, Type: nvarchar
        [Column("SO_CMT_CTVAY")]
        public string? SO_CMT_CTVAY { get; set; }

        // Column: NGAY_CAP_CMT_CTVAY, Type: nvarchar
        [Column("NGAY_CAP_CMT_CTVAY")]
        public string? NGAY_CAP_CMT_CTVAY { get; set; }

        // Column: NOI_CAP_CMT_CTVAY, Type: nvarchar
        [Column("NOI_CAP_CMT_CTVAY")]
        public string? NOI_CAP_CMT_CTVAY { get; set; }

        // Column: NGAY_SINH_CTVAY, Type: nvarchar
        [Column("NGAY_SINH_CTVAY")]
        public string? NGAY_SINH_CTVAY { get; set; }

        // Column: GIOI_TINH_CTVAY, Type: nvarchar
        [Column("GIOI_TINH_CTVAY")]
        public string? GIOI_TINH_CTVAY { get; set; }

        // Column: DIA_CHI_CTVAY, Type: nvarchar
        [Column("DIA_CHI_CTVAY")]
        public string? DIA_CHI_CTVAY { get; set; }

        // Column: SO_DT_CTVAY, Type: nvarchar
        [Column("SO_DT_CTVAY")]
        public string? SO_DT_CTVAY { get; set; }

        // Column: EMAIL_CTVAY, Type: nvarchar
        [Column("EMAIL_CTVAY")]
        public string? EMAIL_CTVAY { get; set; }

        // Column: NGHE_NGHIEP_CTVAY, Type: nvarchar
        [Column("NGHE_NGHIEP_CTVAY")]
        public string? NGHE_NGHIEP_CTVAY { get; set; }

        // Column: QUAN_HE_VOU_CTVAY, Type: nvarchar
        [Column("QUAN_HE_VOU_CTVAY")]
        public string? QUAN_HE_VOU_CTVAY { get; set; }

        // Column: HON_NHAN_CTVAY, Type: nvarchar
        [Column("HON_NHAN_CTVAY")]
        public string? HON_NHAN_CTVAY { get; set; }

        // Column: KY_HAN_VAY, Type: nvarchar
        [Column("KY_HAN_VAY")]
        public string? KY_HAN_VAY { get; set; }

        // Column: HINH_THUC_TT, Type: nvarchar
        [Column("HINH_THUC_TT")]
        public string? HINH_THUC_TT { get; set; }

        // Column: SO_KY_TT, Type: nvarchar
        [Column("SO_KY_TT")]
        public string? SO_KY_TT { get; set; }

        // Column: SO_NGAY_QUA_HAN, Type: nvarchar
        [Column("SO_NGAY_QUA_HAN")]
        public string? SO_NGAY_QUA_HAN { get; set; }

        // Column: NGAY_QUA_HAN, Type: nvarchar
        [Column("NGAY_QUA_HAN")]
        public string? NGAY_QUA_HAN { get; set; }

        // Column: TYLE_BD, Type: decimal
        [Column("TYLE_BD")]
        public decimal? TYLE_BD { get; set; }

        // Column: GIATRI_BD, Type: decimal
        [Column("GIATRI_BD")]
        public decimal? GIATRI_BD { get; set; }

        // Column: NGAY_DANH_GIA_BD, Type: nvarchar
        [Column("NGAY_DANH_GIA_BD")]
        public string? NGAY_DANH_GIA_BD { get; set; }

        // Column: LS_QUA_HAN, Type: decimal
        [Column("LS_QUA_HAN")]
        public decimal? LS_QUA_HAN { get; set; }

        // Column: PHI_TD, Type: decimal
        [Column("PHI_TD")]
        public decimal? PHI_TD { get; set; }

        // Column: TK_TIEN_VAY, Type: nvarchar
        [Column("TK_TIEN_VAY")]
        public string? TK_TIEN_VAY { get; set; }

        // Column: TK_TT_LAI, Type: nvarchar
        [Column("TK_TT_LAI")]
        public string? TK_TT_LAI { get; set; }

        // Column: TK_TT_GOC, Type: nvarchar
        [Column("TK_TT_GOC")]
        public string? TK_TT_GOC { get; set; }

        // Column: TK_TONG_THU, Type: nvarchar
        [Column("TK_TONG_THU")]
        public string? TK_TONG_THU { get; set; }

        // Column: MA_KV, Type: nvarchar
        [Column("MA_KV")]
        public string? MA_KV { get; set; }

        // Column: TEN_KV, Type: nvarchar
        [Column("TEN_KV")]
        public string? TEN_KV { get; set; }

        // Column: TINH_TP, Type: nvarchar
        [Column("TINH_TP")]
        public string? TINH_TP { get; set; }

        // Column: QUAN_HUYEN, Type: nvarchar
        [Column("QUAN_HUYEN")]
        public string? QUAN_HUYEN { get; set; }

        // Column: PHUONG_XA, Type: nvarchar
        [Column("PHUONG_XA")]
        public string? PHUONG_XA { get; set; }

        // Column: LOAI_HINH_DN, Type: nvarchar
        [Column("LOAI_HINH_DN")]
        public string? LOAI_HINH_DN { get; set; }

        // Column: LOAI_HINH_DN_CHI_TIET, Type: nvarchar
        [Column("LOAI_HINH_DN_CHI_TIET")]
        public string? LOAI_HINH_DN_CHI_TIET { get; set; }

        // Column: QUY_MO_DN, Type: nvarchar
        [Column("QUY_MO_DN")]
        public string? QUY_MO_DN { get; set; }

        // Column: NGANH_CHINH, Type: nvarchar
        [Column("NGANH_CHINH")]
        public string? NGANH_CHINH { get; set; }

        // Column: NGANH_PHU, Type: nvarchar
        [Column("NGANH_PHU")]
        public string? NGANH_PHU { get; set; }

        // Column: CreatedAt, Type: datetime2
        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        // Column: UpdatedAt, Type: datetime2
        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }

        // Column: IsDeleted, Type: bit
        [Column("IsDeleted")]
        public bool IsDeleted { get; set; }

        // Column: SysStartTime, Type: datetime2
        [Column("SysStartTime")]
        public DateTime SysStartTime { get; set; }

        // Column: SysEndTime, Type: datetime2
        [Column("SysEndTime")]
        public DateTime SysEndTime { get; set; }

        // Column: (88rowsaffected), Type: 
        [Column("(88rowsaffected)")]
        public string? (88rowsaffected) { get; set; }

    }
}
