using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;
using Khoan.Api.Models;

namespace Khoan.Api.Scripts;

public static class FixVietnameseEncoding
{
    public static void FixKpiIndicatorNames(ApplicationDbContext context)
    {
        Console.WriteLine("🔧 Starting Vietnamese encoding fix for KPI Indicators...");

        var indicators = context.KpiIndicators.ToList();
        int fixedCount = 0;

        // Dictionary to fix common corrupted Vietnamese characters
        var encodingFixes = new Dictionary<string, string>
        {
            { "T?ng ngu?n v?n cu?i k?", "Tổng nguồn vốn cuối kỳ" },
            { "T?ng ngu?n v?n huy d?ng BQ trong k?", "Tổng nguồn vốn huy động BQ trong kỳ" },
            { "T?ng du n? cu?i k?", "Tổng dư nợ cuối kỳ" },
            { "T?ng du n? BQ trong k?", "Tổng dư nợ BQ trong kỳ" },
            { "T?ng du n? HSX&CN", "Tổng dư nợ HSX&CN" },
            { "T? l? n? x?u n?i b?ng", "Tỷ lệ nợ xấu nội bảng" },
            { "Thu n? dã XLRR", "Thu nợ đã XLRR" },
            { "Phát tri?n Khách hàng", "Phát triển Khách hàng" },
            { "L?i nhu?n khoán tài chính", "Lợi nhuận khoán tài chính" },
            { "T?ng ngu?n v?n huy d?ng BQ", "Tổng nguồn vốn huy động BQ" },
            { "T?ng ngu?n v?n BQ", "Tổng nguồn vốn BQ" },
            { "T?ng ngu?n v?n", "Tổng nguồn vốn" },
            { "T?ng du n?", "Tổng dư nợ" },
            { "L?i nhu?n", "Lợi nhuận" },
            { "Thu n?", "Thu nợ" },
            { "N? x?u", "Nợ xấu" },
            { "Phát tri?n", "Phát triển" },
            { "Qu?n l?", "Quản lý" },
            { "Chi ti?t", "Chi tiết" },
            { "T?ng h?p", "Tổng hợp" },
            { "B?o cáo", "Báo cáo" },
            { "Cán b?", "Cán bộ" },
            { "Qu? trình", "Quá trình" },
            { "Gi?i pháp", "Giải pháp" },
            { "Chi nhánh", "Chi nhánh" }, // This should already be correct
            { "K? ho?ch", "Kế hoạch" },
            { "Th?c hi?n", "Thực hiện" },
            { "Doanh s?", "Doanh số" },
            { "Thu nh?p", "Thu nhập" },
            { "Ch?t l??ng", "Chất lượng" },
            { "Hi?u qu?", "Hiệu quả" },
            // Additional common fixes
            { "?", "ố" }, // Handle individual character fixes if needed
            { "?", "ợ" },
            { "?", "ủ" },
            { "?", "ề" },
            { "?", "ị" },
            { "?", "ữ" },
            { "?", "ậ" },
            { "?", "ẹ" },
            { "?", "ũ" },
            { "?", "ả" },
        };

        foreach (var indicator in indicators)
        {
            var originalName = indicator.IndicatorName;
            var fixedName = originalName;

            // Apply encoding fixes
            foreach (var fix in encodingFixes)
            {
                if (fixedName.Contains(fix.Key))
                {
                    fixedName = fixedName.Replace(fix.Key, fix.Value);
                }
            }

            if (fixedName != originalName)
            {
                Console.WriteLine($"  🔤 Fixing: '{originalName}' → '{fixedName}'");
                indicator.IndicatorName = fixedName;
                fixedCount++;
            }
        }

        if (fixedCount > 0)
        {
            context.SaveChanges();
            Console.WriteLine($"✅ Fixed {fixedCount} indicator names with Vietnamese encoding issues");
        }
        else
        {
            Console.WriteLine("ℹ️ No encoding issues found in indicator names");
        }
    }
}
