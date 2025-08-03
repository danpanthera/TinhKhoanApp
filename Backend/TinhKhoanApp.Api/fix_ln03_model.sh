#!/bin/bash
# fix_ln03_model.sh
# Sửa model LN03 để hỗ trợ đầy đủ 20 cột

echo "🔧 FIXING LN03 MODEL - 20 COLUMNS SUPPORT"
echo "========================================="

echo "1. Backup model LN03 hiện tại..."
cp Models/DataTables/LN03.cs Models/DataTables/LN03.cs.backup

echo "2. Cập nhật model LN03 với 20 cột đầy đủ..."

cat > Models/DataTables/LN03.cs << 'EOF'
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables;

/// <summary>
/// LN03 - Báo cáo nợ xấu (20 cột business + 4 cột system)
/// Fixed: Hỗ trợ đầy đủ 20 cột từ file CSV
/// </summary>
[Table("LN03")]
public class LN03
{
    // System columns (tự động)
    [Key]
    public int Id { get; set; }

    [Column(Order = 1)]
    public DateTime NGAY_DL { get; set; }

    [Column(Order = 2)]
    public DateTime? CreatedAt { get; set; }

    [Column(Order = 3)]
    public DateTime? UpdatedAt { get; set; }

    [Column(Order = 4)]
    public int? ImportId { get; set; }

    // Business columns (20 cột từ CSV)

    [Column(Order = 5)]
    [StringLength(50)]
    public string? MACHINHANH { get; set; }

    [Column(Order = 6)]
    [StringLength(255)]
    public string? TENCHINHANH { get; set; }

    [Column(Order = 7)]
    [StringLength(50)]
    public string? MAKH { get; set; }

    [Column(Order = 8)]
    [StringLength(255)]
    public string? TENKH { get; set; }

    [Column(Order = 9)]
    [StringLength(100)]
    public string? SOHOPDONG { get; set; }

    [Column(Order = 10)]
    public decimal? SOTIENXLRR { get; set; }

    [Column(Order = 11)]
    [StringLength(50)]
    public string? NGAYPHATSINHXL { get; set; }

    [Column(Order = 12)]
    public decimal? THUNOSAUXL { get; set; }

    [Column(Order = 13)]
    public decimal? CONLAINGOAIBANG { get; set; }

    [Column(Order = 14)]
    public decimal? DUNONOIBANG { get; set; }

    [Column(Order = 15)]
    [StringLength(50)]
    public string? NHOMNO { get; set; }

    [Column(Order = 16)]
    [StringLength(50)]
    public string? MACBTD { get; set; }

    [Column(Order = 17)]
    [StringLength(255)]
    public string? TENCBTD { get; set; }

    [Column(Order = 18)]
    [StringLength(50)]
    public string? MAPGD { get; set; }

    [Column(Order = 19)]
    [StringLength(100)]
    public string? TAIKHOANHACHTOAN { get; set; }

    [Column(Order = 20)]
    [StringLength(255)]
    public string? REFNO { get; set; }

    [Column(Order = 21)]
    [StringLength(100)]
    public string? LOAINGUONVON { get; set; }

    // 3 cột bổ sung không có header trong CSV
    [Column(Order = 22)]
    [StringLength(50)]
    public string? MALOAI { get; set; }  // Cột 18: "100"

    [Column(Order = 23)]
    [StringLength(100)]
    public string? LOAIKHACHHANG { get; set; }  // Cột 19: "Cá nhân"

    [Column(Order = 24)]
    public decimal? HANMUCPHEDUYET { get; set; }  // Cột 20: số tiền lớn
}
EOF

echo "3. Cập nhật LN03Repository để xử lý 20 cột..."

# Kiểm tra xem LN03Repository có tồn tại không
if [ ! -f "Repositories/LN03Repository.cs" ]; then
    echo "Tạo LN03Repository mới..."

    cat > Repositories/LN03Repository.cs << 'REPO_EOF'
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.DataTables;
using Microsoft.EntityFrameworkCore;

namespace TinhKhoanApp.Api.Repositories;

public interface ILN03Repository : IRepository<LN03>
{
    Task<IEnumerable<LN03>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
    Task<IEnumerable<LN03>> GetByBranchAsync(string branchCode);
    Task<int> GetTotalRecordsAsync();
}

public class LN03Repository : Repository<LN03>, ILN03Repository
{
    public LN03Repository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<LN03>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        return await _context.LN03
            .Where(x => x.NGAY_DL >= fromDate && x.NGAY_DL <= toDate)
            .OrderBy(x => x.NGAY_DL)
            .ToListAsync();
    }

    public async Task<IEnumerable<LN03>> GetByBranchAsync(string branchCode)
    {
        return await _context.LN03
            .Where(x => x.MACHINHANH == branchCode)
            .OrderBy(x => x.NGAY_DL)
            .ToListAsync();
    }

    public async Task<int> GetTotalRecordsAsync()
    {
        return await _context.LN03.CountAsync();
    }
}
REPO_EOF
fi

echo "4. Tạo CSV parser cho LN03 với 20 cột..."

cat > Services/LN03CsvParser.cs << 'PARSER_EOF'
using TinhKhoanApp.Api.Models.DataTables;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace TinhKhoanApp.Api.Services;

public class LN03CsvParser
{
    public static List<LN03> ParseCsv(string csvContent, DateTime ngayDl)
    {
        var records = new List<LN03>();

        using var reader = new StringReader(csvContent);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null,
            TrimOptions = TrimOptions.Trim
        });

        // Skip header
        csv.Read();
        csv.ReadHeader();

        while (csv.Read())
        {
            try
            {
                var record = new LN03
                {
                    NGAY_DL = ngayDl,
                    CreatedAt = DateTime.UtcNow,

                    // 17 cột có header
                    MACHINHANH = csv.GetField(0)?.Trim('"'),
                    TENCHINHANH = csv.GetField(1)?.Trim('"'),
                    MAKH = csv.GetField(2)?.Trim('"'),
                    TENKH = csv.GetField(3)?.Trim('"'),
                    SOHOPDONG = csv.GetField(4)?.Trim('"'),
                    SOTIENXLRR = ParseDecimal(csv.GetField(5)),
                    NGAYPHATSINHXL = csv.GetField(6)?.Trim('"'),
                    THUNOSAUXL = ParseDecimal(csv.GetField(7)),
                    CONLAINGOAIBANG = ParseDecimal(csv.GetField(8)),
                    DUNONOIBANG = ParseDecimal(csv.GetField(9)),
                    NHOMNO = csv.GetField(10)?.Trim('"'),
                    MACBTD = csv.GetField(11)?.Trim('"'),
                    TENCBTD = csv.GetField(12)?.Trim('"'),
                    MAPGD = csv.GetField(13)?.Trim('"'),
                    TAIKHOANHACHTOAN = csv.GetField(14)?.Trim('"'),
                    REFNO = csv.GetField(15)?.Trim('"'),
                    LOAINGUONVON = csv.GetField(16)?.Trim('"'),

                    // 3 cột không có header (17, 18, 19)
                    MALOAI = csv.GetField(17)?.Trim('"'),
                    LOAIKHACHHANG = csv.GetField(18)?.Trim('"'),
                    HANMUCPHEDUYET = ParseDecimal(csv.GetField(19))
                };

                records.Add(record);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing LN03 row: {ex.Message}");
                // Continue with next row
            }
        }

        return records;
    }

    private static decimal? ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        var cleanValue = value.Trim('"').Trim();
        if (decimal.TryParse(cleanValue, out var result))
            return result;

        return null;
    }
}
PARSER_EOF

echo "5. Tạo migration cho LN03 với 20 cột..."

cat > create_ln03_migration.sh << 'MIG_EOF'
#!/bin/bash
echo "Tạo migration cho LN03 với 20 cột..."
dotnet ef migrations add "UpdateLN03With20Columns" --context ApplicationDbContext
echo "Migration created! Run 'dotnet ef database update' to apply."
MIG_EOF

chmod +x create_ln03_migration.sh

echo "✅ LN03 MODEL FIX COMPLETED!"
echo "=========================="
echo "🎯 Cập nhật hoàn thành:"
echo "   • LN03.cs - 20 cột business + 4 system"
echo "   • LN03Repository.cs - Repository pattern"
echo "   • LN03CsvParser.cs - Parse CSV với 20 cột"
echo "   • create_ln03_migration.sh - Tạo migration"
echo ""
echo "📋 Các bước tiếp theo:"
echo "   1. ./create_ln03_migration.sh"
echo "   2. dotnet ef database update"
echo "   3. Test import LN03 file"
