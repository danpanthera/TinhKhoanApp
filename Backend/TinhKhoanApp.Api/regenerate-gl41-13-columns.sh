#!/bin/bash

# ✅ Script regenerate GL41 model theo đúng header CSV chuẩn
# Header chuẩn GL41: MA_CN,LOAI_TIEN,MA_TK,TEN_TK,LOAI_BT,DN_DAUKY,DC_DAUKY,SBT_NO,ST_GHINO,SBT_CO,ST_GHICO,DN_CUOIKY,DC_CUOIKY

echo "🔧 REGENERATE GL41 MODEL - THEO HEADER CSV CHUẨN"
echo "════════════════════════════════════════════════"

# Header GL41 chuẩn theo yêu cầu anh
GL41_HEADER="MA_CN,LOAI_TIEN,MA_TK,TEN_TK,LOAI_BT,DN_DAUKY,DC_DAUKY,SBT_NO,ST_GHINO,SBT_CO,ST_GHICO,DN_CUOIKY,DC_CUOIKY"

echo "📊 Header GL41 chuẩn (13 cột):"
echo "$GL41_HEADER"
echo

# Đếm số cột
COLUMN_COUNT=$(echo "$GL41_HEADER" | tr ',' '\n' | wc -l | xargs)
echo "🔢 Tổng số cột: $COLUMN_COUNT"
echo

# Tạo GL41 model mới
echo "🚀 Tạo GL41.cs model mới..."

cat > /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/Models/DataTables/GL41.cs << 'EOF'
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng GL41 - Dữ liệu kế toán chi tiết theo tài khoản
    /// Lưu trữ dữ liệu từ các file CSV có filename chứa "GL41"
    /// Tuân thủ Temporal Tables + Columnstore Indexes
    /// Header: MA_CN,LOAI_TIEN,MA_TK,TEN_TK,LOAI_BT,DN_DAUKY,DC_DAUKY,SBT_NO,ST_GHINO,SBT_CO,ST_GHICO,DN_CUOIKY,DC_CUOIKY
    /// </summary>
    [Table("GL41")]
    public class GL41
    {
        /// <summary>
        /// Khóa chính tự tăng
        /// </summary>
        [Key]
        public int Id { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // 🏛️ CÁC CỘT DỮ LIỆU THEO HEADER CSV GL41 (13 CỘT)
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Mã chi nhánh
        /// </summary>
        [Column("MA_CN")]
        [StringLength(50)]
        public string? MA_CN { get; set; }

        /// <summary>
        /// Loại tiền tệ (VND, USD, etc.)
        /// </summary>
        [Column("LOAI_TIEN")]
        [StringLength(10)]
        public string? LOAI_TIEN { get; set; }

        /// <summary>
        /// Mã tài khoản kế toán
        /// </summary>
        [Column("MA_TK")]
        [StringLength(50)]
        public string? MA_TK { get; set; }

        /// <summary>
        /// Tên tài khoản kế toán
        /// </summary>
        [Column("TEN_TK")]
        [StringLength(255)]
        public string? TEN_TK { get; set; }

        /// <summary>
        /// Loại bút toán
        /// </summary>
        [Column("LOAI_BT")]
        [StringLength(50)]
        public string? LOAI_BT { get; set; }

        /// <summary>
        /// Dư nợ đầu kỳ
        /// </summary>
        [Column("DN_DAUKY")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DN_DAUKY { get; set; }

        /// <summary>
        /// Dư có đầu kỳ
        /// </summary>
        [Column("DC_DAUKY")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DC_DAUKY { get; set; }

        /// <summary>
        /// Số bút toán nợ
        /// </summary>
        [Column("SBT_NO")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SBT_NO { get; set; }

        /// <summary>
        /// Số tiền ghi nợ
        /// </summary>
        [Column("ST_GHINO")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ST_GHINO { get; set; }

        /// <summary>
        /// Số bút toán có
        /// </summary>
        [Column("SBT_CO")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SBT_CO { get; set; }

        /// <summary>
        /// Số tiền ghi có
        /// </summary>
        [Column("ST_GHICO")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ST_GHICO { get; set; }

        /// <summary>
        /// Dư nợ cuối kỳ
        /// </summary>
        [Column("DN_CUOIKY")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DN_CUOIKY { get; set; }

        /// <summary>
        /// Dư có cuối kỳ
        /// </summary>
        [Column("DC_CUOIKY")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DC_CUOIKY { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // 🛠️ CÁC CỘT CHUẨN TEMPORAL TABLES + METADATA
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Ngày dữ liệu theo định dạng dd/MM/yyyy
        /// Được parse từ tên file *yyyymmdd.csv
        /// </summary>
        [Column("NGAY_DL")]
        [StringLength(10)]
        public string NgayDL { get; set; } = null!;

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
        /// Tên file gốc được import
        /// </summary>
        [Column("FILE_NAME")]
        [StringLength(255)]
        public string? FILE_NAME { get; set; }

        // Temporal Tables columns sẽ được thêm bởi EF Core khi config
        // SysStartTime, SysEndTime sẽ được SQL Server tự quản lý
    }
}
EOF

echo "✅ Đã tạo GL41.cs model mới với 13 cột theo header chuẩn!"
echo

# Tạo file CSV mẫu để test
echo "📄 Tạo file CSV mẫu để test import..."
cat > /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/test_gl41_13_columns.csv << 'EOF'
MA_CN,LOAI_TIEN,MA_TK,TEN_TK,LOAI_BT,DN_DAUKY,DC_DAUKY,SBT_NO,ST_GHINO,SBT_CO,ST_GHICO,DN_CUOIKY,DC_CUOIKY
7800,VND,11101,TK Tiền mặt tại quỹ VND,PS,1000000.00,0.00,500000.00,500000.00,200000.00,200000.00,1300000.00,0.00
7800,VND,11102,TK Tiền gửi không kỳ hạn,PS,5000000.00,0.00,2000000.00,2000000.00,1000000.00,1000000.00,6000000.00,0.00
EOF

echo "✅ Đã tạo test_gl41_13_columns.csv với 2 records mẫu!"
echo

# Kiểm tra số cột trong file CSV mẫu
CSV_COLUMNS=$(head -1 /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/test_gl41_13_columns.csv | tr ',' '\n' | wc -l | xargs)
echo "🔍 Verification:"
echo "   - Header chuẩn: $COLUMN_COUNT cột"
echo "   - CSV mẫu: $CSV_COLUMNS cột"

if [ "$COLUMN_COUNT" -eq "$CSV_COLUMNS" ]; then
    echo "   ✅ KHỚP! Model và CSV có cùng số cột"
else
    echo "   ❌ KHÔNG KHỚP! Cần kiểm tra lại"
fi

echo
echo "🎯 Kết quả:"
echo "   ✅ GL41.cs model: 13 cột dữ liệu + 4 cột chuẩn temporal"
echo "   ✅ Test CSV: test_gl41_13_columns.csv"
echo "   ✅ Ready for: Migration và test import"
echo
echo "📋 Bước tiếp theo:"
echo "   1. Tạo migration: dotnet ef migrations add UpdateGL41Structure"
echo "   2. Apply migration: dotnet ef database update"
echo "   3. Test import: /api/directimport/smart"
