using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Khoan.Api.Data;
using Khoan.Api.Models.Configuration;
using Khoan.Api.Services;

// Simple console runner to exercise import service with in-memory DB and print metrics
var services = new ServiceCollection();
services.AddLogging(b => b.AddConsole());
services.AddDbContext<ApplicationDbContext>(o => o.UseInMemoryDatabase("ImportRunnerDB"));
services.AddSingleton<IOptions<DirectImportSettings>>(Options.Create(new DirectImportSettings()));
services.AddSingleton<InMemoryImportMetrics>();
services.AddTransient<DirectImportService>(sp => new DirectImportService(
    sp.GetRequiredService<ApplicationDbContext>(),
    sp.GetRequiredService<ILogger<DirectImportService>>(),
    sp.GetRequiredService<IOptions<DirectImportSettings>>(),
    sp.GetRequiredService<InMemoryImportMetrics>()
));

var sp = services.BuildServiceProvider();
var svc = sp.GetRequiredService<DirectImportService>();
var metrics = sp.GetRequiredService<InMemoryImportMetrics>();

// Build a small LN03 sample CSV in-memory (>=1 but < threshold so EF path is used)
string ln03Csv = "MACHINHANH,TENCHINHANH,MAKH,TENKH,SOHOPDONG,SOTIENXLRR,NGAYPHATSINHXL,THUNOSAUXL,CONLAINGOAIBANG,DUNONOIBANG,NHOMNO,MACBTD,TENCBTD,MAPGD,TAIKHOANHACHTOAN,REFNO,LOAINGUONVON\n" +
                 "001,Branch A,KH1,Nguyen A,HD1,1000,01/01/2024,200,300,400,N1,CB01,Ten CB,PGD01,111,REF1,VON1,extra18,extra19,500\n";

IFormFile MakeCsv(string fileName, string csv)
{
    var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
    var stream = new MemoryStream(bytes);
    return new FormFile(stream, 0, bytes.Length, "file", fileName);
}

var ln03File = MakeCsv("7800_ln03_20240101.csv", ln03Csv);
var res = await svc.ImportLN03EnhancedAsync(ln03File);
Console.WriteLine($"LN03 import success={res.Success}, records={res.ProcessedRecords}, date={res.NgayDL}");

// Print prometheus text
Console.WriteLine("----- /metrics (Prometheus) -----");
Console.WriteLine(metrics.ToPrometheus());
