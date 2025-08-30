using System;
using System.Collections.Generic;

namespace TinhKhoanApp.Api.Models.NguonVon
{
    /// <summary>
    /// Request model cho API tính toán nguồn vốn
    /// </summary>
    public class NguonVonRequest
    {
        /// <summary>
        /// Mã đơn vị: "ALL", "HoiSo", "CnBinhLu"... hoặc "CnPhongThoPgdSo5"...
        /// </summary>
        public string UnitCode { get; set; }

        /// <summary>
        /// Ngày cần tính toán
        /// </summary>
        public DateTime TargetDate { get; set; }

        /// <summary>
        /// Loại ngày: "year", "month", "day"
        /// </summary>
        public string DateType { get; set; }
    }

    /// <summary>
    /// Kết quả tính toán nguồn vốn
    /// </summary>
    public class NguonVonResult
    {
        /// <summary>
        /// Mã đơn vị
        /// </summary>
        public string UnitCode { get; set; }

        /// <summary>
        /// Tên đơn vị
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// Tổng số dư (nguồn vốn)
        /// </summary>
        public decimal TotalBalance { get; set; }

        /// <summary>
        /// Ngày được tính toán
        /// </summary>
        public DateTime CalculatedDate { get; set; }

        /// <summary>
        /// Số lượng bản ghi tham gia tính toán
        /// </summary>
        public int RecordCount { get; set; }
    }

    /// <summary>
    /// Chi tiết nguồn vốn cho mục đích debug
    /// </summary>
    public class NguonVonDetails
    {
        /// <summary>
        /// Thông tin tổng kết
        /// </summary>
        public NguonVonResult Summary { get; set; }

        /// <summary>
        /// Top 20 tài khoản có số dư lớn nhất
        /// </summary>
        public List<AccountDetail> TopAccounts { get; set; }

        /// <summary>
        /// Có dữ liệu hay không
        /// </summary>
        public bool HasData { get; set; }

        /// <summary>
        /// Thông báo kết quả
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Chi tiết một tài khoản
    /// </summary>
    public class AccountDetail
    {
        /// <summary>
        /// Mã tài khoản hạch toán
        /// </summary>
        public string AccountCode { get; set; }

        /// <summary>
        /// Tổng số dư của tài khoản này
        /// </summary>
        public decimal TotalBalance { get; set; }

        /// <summary>
        /// Số lượng bản ghi của tài khoản này
        /// </summary>
        public int RecordCount { get; set; }
    }
}
