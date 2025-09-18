using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Khoan.Api.Services;
using Khoan.Api.Models.Configuration;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;

namespace Khoan.Api.Tests;

public class CsvParsingAndSamplingTests
{
    private static IOptions<DirectImportSettings> Settings() => Options.Create(new DirectImportSettings());

    private static IFormFile MakeCsv(string fileName, string csv)
    {
        var bytes = Encoding.UTF8.GetBytes(csv);
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "file", fileName);
    }

    private static ApplicationDbContext InMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var ctx = new ApplicationDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    [Fact]
    public async Task Parse_LN03_Enhanced_Works_With_Mixed_Formats()
    {
        var svc = new DirectImportService(
            context: InMemoryContext(),
            logger: NullLogger<DirectImportService>.Instance,
            settings: Settings(),
            metrics: new InMemoryImportMetrics()
        );

        var csv = "MACHINHANH,TENCHINHANH,MAKH,TENKH,SOHOPDONG,SOTIENXLRR,NGAYPHATSINHXL,THUNOSAUXL,CONLAINGOAIBANG,DUNONOIBANG,NHOMNO,MACBTD,TENCBTD,MAPGD,TAIKHOANHACHTOAN,REFNO,LOAINGUONVON\n"
                + "001,Branch A,KH1,Nguyen A,HD1,1000,01/01/2024,200,300,400,N1,CB01,Ten CB,PGD01,111,REF1,VON1,extra18,extra19,500\n";

        var file = MakeCsv("7800_ln03_20240101.csv", csv);
        var result = await svc.ImportLN03EnhancedAsync(file);

        Assert.True(result.Success);
        Assert.Equal("2024-01-01", result.NgayDL);
        Assert.Equal(1, result.ProcessedRecords);
    }

    [Fact]
    public async Task Parse_RR01_Works_With_yyyyMMdd_Dates()
    {
        var svc = new DirectImportService(
            context: InMemoryContext(),
            logger: NullLogger<DirectImportService>.Instance,
            settings: Settings(),
            metrics: new InMemoryImportMetrics()
        );

        var header = "CN_LOAI_I,BRCD,MA_KH,TEN_KH,SO_LDS,CCY,SO_LAV,LOAI_KH,NGAY_GIAI_NGAN,NGAY_DEN_HAN,VAMC_FLG,NGAY_XLRR,DUNO_GOC_BAN_DAU,DUNO_LAI_TICHLUY_BD,DOC_DAUKY_DA_THU_HT,DUNO_GOC_HIENTAI,DUNO_LAI_HIENTAI,DUNO_NGAN_HAN,DUNO_TRUNG_HAN,DUNO_DAI_HAN,THU_GOC,THU_LAI,BDS,DS,TSK";
        var row =  "I,001,KH1,Ten KH,SO1,VND,LAV1,IND,20240101,20241231,Y,20240202,100,10,5,90,9,50,20,20,1,2,3,4,5";
        var csv = header + "\n" + row + "\n";

        var file = MakeCsv("7800_rr01_20240101.csv", csv);
        var result = await svc.ImportRR01Async(file);

        Assert.True(result.Success);
        Assert.Equal("2024-01-01", result.NgayDL);
        Assert.Equal(1, result.ProcessedRecords);
    }

    [Fact]
    public void ParseError_Sampling_Max5_PerFile()
    {
        var svc = new DirectImportService(
            context: null!,
            logger: NullLogger<DirectImportService>.Instance,
            settings: Settings(),
            metrics: new InMemoryImportMetrics()
        );

        // dùng reflection để gọi RecordParseError private method
        var mi = typeof(DirectImportService).GetMethod("RecordParseError", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(mi);

        for (int i = 0; i < 10; i++)
        {
            mi!.Invoke(svc, new object[] { "GL01", $"badline-{i}", "parse-failed" });
        }

        var errors = svc.GetRecentParseErrors("GL01");
        Assert.True(errors.Count <= 5);

        DirectImportService.ClearRuntimeParseErrors();
        Assert.Empty(svc.GetRecentParseErrors("GL01"));
    }
}
