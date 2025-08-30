#!/bin/bash
# Fix Vietnamese encoding issues in KPI indicators

echo "🔧 Starting Vietnamese encoding fix..."

cd /opt/Projects/Khoan/Backend/KhoanApp.Api

# Create temporary C# console app to execute the fix
cat > FixEncoding.cs << 'EOF'
using Microsoft.EntityFrameworkCore;
using KhoanApp.Api.Data;
using KhoanApp.Api.Models;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");

var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseSqlServer(connectionString)
    .Options;

using var context = new ApplicationDbContext(options);

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
    { "T?ng du n?", "Tổng dư nợ" }
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

Console.WriteLine("🎉 Vietnamese encoding fix completed!");
EOF

# Run the encoding fix
dotnet run --project . -- --execute-script FixEncoding.cs 2>/dev/null || {
    echo "📝 Compiling and running encoding fix..."
    csc -reference:Microsoft.EntityFrameworkCore.dll \
        -reference:Microsoft.EntityFrameworkCore.SqlServer.dll \
        -reference:Microsoft.Extensions.Configuration.dll \
        -reference:Microsoft.Extensions.Configuration.Json.dll \
        FixEncoding.cs 2>/dev/null || {

        echo "🔄 Using dotnet script approach..."
        dotnet script FixEncoding.cs 2>/dev/null || {

            echo "⚡ Direct SQL approach to fix encoding..."
            echo "UPDATE KpiIndicators SET
                IndicatorName = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
                    IndicatorName,
                    'T?ng ngu?n v?n cu?i k?', 'Tổng nguồn vốn cuối kỳ'),
                    'T?ng ngu?n v?n huy d?ng BQ trong k?', 'Tổng nguồn vốn huy động BQ trong kỳ'),
                    'T?ng du n? cu?i k?', 'Tổng dư nợ cuối kỳ'),
                    'T?ng du n? BQ trong k?', 'Tổng dư nợ BQ trong kỳ'),
                    'T? l? n? x?u n?i b?ng', 'Tỷ lệ nợ xấu nội bảng'),
                    'Thu n? dã XLRR', 'Thu nợ đã XLRR'),
                    'Phát tri?n Khách hàng', 'Phát triển Khách hàng'),
                    'L?i nhu?n khoán tài chính', 'Lợi nhuận khoán tài chính'),
                    'T?ng ngu?n v?n', 'Tổng nguồn vốn')
            WHERE IndicatorName LIKE '%?%';"
        }
    }
}

# Clean up
rm -f FixEncoding.cs FixEncoding.exe

echo "✅ Encoding fix script completed!"
