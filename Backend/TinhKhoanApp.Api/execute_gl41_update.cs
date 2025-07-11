using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddJsonFile("appsettings.json", optional: false);

var configuration = builder.Build();
var connectionString = configuration.GetConnectionString("DefaultConnection");

var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionsBuilder.UseSqlServer(connectionString);

using (var context = new ApplicationDbContext(optionsBuilder.Options))
{
    Console.WriteLine("🔗 Connected to database");
    
    // Rename SO_TK to MA_TK if exists
    try 
    {
        await context.Database.ExecuteSqlRawAsync(@"
            IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GL41') AND name = 'SO_TK')
            BEGIN
                EXEC sp_rename 'GL41.SO_TK', 'MA_TK', 'COLUMN'
                PRINT 'Renamed SO_TK to MA_TK'
            END");
        Console.WriteLine("✅ SO_TK renamed to MA_TK");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ℹ️ SO_TK rename: {ex.Message}");
    }
    
    // Add LOAI_TIEN column
    try 
    {
        await context.Database.ExecuteSqlRawAsync(@"
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GL41') AND name = 'LOAI_TIEN')
            BEGIN
                ALTER TABLE GL41 ADD LOAI_TIEN NVARCHAR(50)
            END");
        Console.WriteLine("✅ LOAI_TIEN column added");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ℹ️ LOAI_TIEN: {ex.Message}");
    }
    
    // Add LOAI_BT column
    try 
    {
        await context.Database.ExecuteSqlRawAsync(@"
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('GL41') AND name = 'LOAI_BT')
            BEGIN
                ALTER TABLE GL41 ADD LOAI_BT NVARCHAR(50)
            END");
        Console.WriteLine("✅ LOAI_BT column added");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ℹ️ LOAI_BT: {ex.Message}");
    }
    
    Console.WriteLine("\n🎉 GL41 structure update completed!");
}
