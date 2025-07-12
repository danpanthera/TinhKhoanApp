using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer("Server=localhost,1433;Database=TinhKhoanDB;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=true"));

var app = builder.Build();

// Tạo KPI schema
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Tạo bảng KpiAssignmentTables nếu chưa có
    await context.Database.ExecuteSqlRawAsync(@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'KpiAssignmentTables')
        BEGIN
            CREATE TABLE KpiAssignmentTables (
                Id int IDENTITY(1,1) PRIMARY KEY,
                TableType nvarchar(50) NOT NULL,
                TableName nvarchar(100) NOT NULL,
                Description nvarchar(max),
                Category nvarchar(50) NOT NULL,
                CreatedAt datetime2 DEFAULT GETDATE(),
                UpdatedAt datetime2 DEFAULT GETDATE()
            );
        END");

    // Tạo bảng KpiIndicators nếu chưa có
    await context.Database.ExecuteSqlRawAsync(@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'KpiIndicators')
        BEGIN
            CREATE TABLE KpiIndicators (
                Id int IDENTITY(1,1) PRIMARY KEY,
                TableId int NOT NULL,
                IndicatorCode nvarchar(50) NOT NULL,
                IndicatorName nvarchar(200) NOT NULL,
                Description nvarchar(max),
                Unit nvarchar(50),
                IsActive bit DEFAULT 1,
                CreatedAt datetime2 DEFAULT GETDATE(),
                UpdatedAt datetime2 DEFAULT GETDATE(),
                FOREIGN KEY (TableId) REFERENCES KpiAssignmentTables(Id)
            );
        END");

    Console.WriteLine("✅ KPI Schema created successfully!");
}

app.Run();
