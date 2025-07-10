#!/bin/bash

# ===================================================================
# SCRIPT TẠO LẠI CẤU TRÚC BẢNG THEO HEADER CSV GỐC
# Đảm bảo 100% khớp với cột CSV gốc
# ===================================================================

echo "🔧 Tạo lại cấu trúc model EI01 theo header CSV gốc..."

# Tạo EI01.cs với cấu trúc đúng header_7800_ei01_20250430.csv
cat > /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/Models/DataTables/EI01.cs << 'EOF'
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng EI01 - Thu nhập khác (24 cột theo header)
    /// Lưu trữ dữ liệu từ các file CSV có filename chứa "EI01"
    /// Cấu trúc theo header_7800_ei01_20250430.csv
    /// Tuân thủ Temporal Tables + Columnstore Indexes
    /// </summary>
    [Table("EI01")]
    public class EI01
    {
        /// <summary>
        /// Khóa chính tự tăng
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Ngày dữ liệu theo định dạng dd/MM/yyyy
        /// Được parse từ tên file *yyyymmdd.csv
        /// </summary>
        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

        // === TẤT CẢ CỘT THEO HEADER CSV GỐC (24 cột) ===

        [Column("MA_CN")]
        [StringLength(20)]
        public string? MA_CN { get; set; }

        [Column("MA_KH")]
        [StringLength(50)]
        public string? MA_KH { get; set; }

        [Column("TEN_KH")]
        [StringLength(255)]
        public string? TEN_KH { get; set; }

        [Column("LOAI_KH")]
        [StringLength(50)]
        public string? LOAI_KH { get; set; }

        [Column("SDT_EMB")]
        [StringLength(20)]
        public string? SDT_EMB { get; set; }

        [Column("TRANG_THAI_EMB")]
        [StringLength(50)]
        public string? TRANG_THAI_EMB { get; set; }

        [Column("NGAY_DK_EMB")]
        [StringLength(20)]
        public string? NGAY_DK_EMB { get; set; }

        [Column("SDT_OTT")]
        [StringLength(20)]
        public string? SDT_OTT { get; set; }

        [Column("TRANG_THAI_OTT")]
        [StringLength(50)]
        public string? TRANG_THAI_OTT { get; set; }

        [Column("NGAY_DK_OTT")]
        [StringLength(20)]
        public string? NGAY_DK_OTT { get; set; }

        [Column("SDT_SMS")]
        [StringLength(20)]
        public string? SDT_SMS { get; set; }

        [Column("TRANG_THAI_SMS")]
        [StringLength(50)]
        public string? TRANG_THAI_SMS { get; set; }

        [Column("NGAY_DK_SMS")]
        [StringLength(20)]
        public string? NGAY_DK_SMS { get; set; }

        [Column("SDT_SAV")]
        [StringLength(20)]
        public string? SDT_SAV { get; set; }

        [Column("TRANG_THAI_SAV")]
        [StringLength(50)]
        public string? TRANG_THAI_SAV { get; set; }

        [Column("NGAY_DK_SAV")]
        [StringLength(20)]
        public string? NGAY_DK_SAV { get; set; }

        [Column("SDT_LN")]
        [StringLength(20)]
        public string? SDT_LN { get; set; }

        [Column("TRANG_THAI_LN")]
        [StringLength(50)]
        public string? TRANG_THAI_LN { get; set; }

        [Column("NGAY_DK_LN")]
        [StringLength(20)]
        public string? NGAY_DK_LN { get; set; }

        [Column("USER_EMB")]
        [StringLength(100)]
        public string? USER_EMB { get; set; }

        [Column("USER_OTT")]
        [StringLength(100)]
        public string? USER_OTT { get; set; }

        [Column("USER_SMS")]
        [StringLength(100)]
        public string? USER_SMS { get; set; }

        [Column("USER_SAV")]
        [StringLength(100)]
        public string? USER_SAV { get; set; }

        [Column("USER_LN")]
        [StringLength(100)]
        public string? USER_LN { get; set; }

        // === TEMPORAL TABLES STANDARD COLUMNS ===

        /// <summary>
        /// Ngày tạo bản ghi
        /// </summary>
        [Column("CREATED_DATE")]
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;

        /// <summary>
        /// Ngày cập nhật bản ghi
        /// </summary>
        [Column("UPDATED_DATE")]
        public DateTime? UPDATED_DATE { get; set; }

        /// <summary>
        /// Tên file import
        /// </summary>
        [Column("FILE_NAME")]
        [StringLength(255)]
        public string? FILE_NAME { get; set; }
    }
}
EOF

echo "✅ Đã tạo EI01.cs với 24 cột theo header CSV gốc"
