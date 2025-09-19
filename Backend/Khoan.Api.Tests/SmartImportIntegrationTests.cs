using System.Net;
using System.Net.Http.Headers;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Khoan.Api.Tests;

public class SmartImportIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SmartImportIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(_ => { /* use defaults */ });
    }

    [Fact]
    public async Task SmartImport_InvalidFilename_ReturnsBadRequest()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using var content = new MultipartFormDataContent();
        var csv = new ByteArrayContent(Encoding.UTF8.GetBytes("a,b\n1,2\n"));
        csv.Headers.ContentType = MediaTypeHeaderValue.Parse("text/csv");
        // invalid filename (does not contain dp01/dpda/ei01/gl01/gl02/gl41/ln03)
        content.Add(csv, "file", "some_random_20240101.csv");

        var res = await client.PostAsync("/api/DirectImport/smart", content);
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SmartImport_LN03_Smoke_WhenAllowed()
    {
        // This test only runs when ALLOW_DB_TESTS=true to avoid DB requirements by default
        if (!string.Equals(Environment.GetEnvironmentVariable("ALLOW_DB_TESTS"), "true", StringComparison.OrdinalIgnoreCase))
        {
            return; // skip silently
        }

        var client = _factory.CreateClient();
        using var content = new MultipartFormDataContent();
        var header = "MACHINHANH,TENCHINHANH,MAKH,TENKH,SOHOPDONG,SOTIENXLRR,NGAYPHATSINHXL,THUNOSAUXL,CONLAINGOAIBANG,DUNONOIBANG,NHOMNO,MACBTD,TENCBTD,MAPGD,TAIKHOANHACHTOAN,REFNO,LOAINGUONVON";
        var row = "001,Branch,KH1,Ten,HD1,1000,01/01/2024,10,20,30,N1,CB1,TenCB,PGD,111,REF,VON";
        var extra = ",C18,C19,999";
        var data = Encoding.UTF8.GetBytes(header + "\n" + row + extra + "\n");
        var csv = new ByteArrayContent(data);
        csv.Headers.ContentType = MediaTypeHeaderValue.Parse("text/csv");
        content.Add(csv, "file", "7800_ln03_20240101.csv");

        var res = await client.PostAsync("/api/DirectImport/smart", content);
        res.EnsureSuccessStatusCode();
    }
}
