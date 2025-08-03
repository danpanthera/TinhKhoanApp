using System.Globalization;
using TinhKhoanApp.Api.Models.DataTables;

namespace TinhKhoanApp.Api.Services;

/// <summary>
/// LN03 CSV parser để xử lý file CSV có 20 cột (17 cột có header + 3 cột không có header)
/// </summary>
public static class LN03CsvParser
{
    /// <summary>
    /// Parse CSV content thành danh sách LN03 objects
    /// CSV Structure: 17 columns with headers + 3 columns without headers
    /// Headers: MACHINHANH,MAHOPDONG,SOTIENGOC,DUSAUHANTHANHTOAN,DUQUAHAN,DUQUAHAN1_30NGAY,DUQUAHAN31_90NGAY,DUQUAHAN91_180NGAY,DUQUAHAN181_360NGAY,DUQUAHAN361_720NGAY,DUQUAHAN720NGAY,SOLANSAINHOM,TONGNOHIENTAI,DUPHONGRUTRUITUONGUNG,DUPHONGRISKKEEPKEMTHEO,DUPHONGRUTRUITINHKHOAN,LOAINGUONVON
    /// No headers: MALOAI,LOAIKHACHHANG,HANMUCPHEDUYET
    /// </summary>
    public static List<LN03> ParseCsv(string csvContent, DateTime ngayDl)
    {
        var results = new List<LN03>();

        if (string.IsNullOrWhiteSpace(csvContent))
            return results;

        var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        // Skip header line
        var dataLines = lines.Skip(1);

        foreach (var line in dataLines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            try
            {
                var record = ParseLine(line.Trim(), ngayDl);
                if (record != null)
                {
                    results.Add(record);
                }
            }
            catch (Exception ex)
            {
                // Log error but continue processing other lines
                Console.WriteLine($"Error parsing line: {line}. Error: {ex.Message}");
            }
        }

        return results;
    }

    /// <summary>
    /// Parse một dòng CSV thành LN03 object
    /// </summary>
    private static LN03? ParseLine(string line, DateTime ngayDl)
    {
        if (string.IsNullOrWhiteSpace(line)) return null;

        // Split by comma, but handle potential quoted values
        var fields = SplitCsvLine(line);

        // Expect exactly 20 fields
        if (fields.Length != 20)
        {
            Console.WriteLine($"Expected 20 fields but got {fields.Length} in line: {line}");
            return null;
        }

        try
        {
            return new LN03
            {
                // 17 cột có header theo CSV thực tế (indexes 0-16)
                MACHINHANH = fields[0]?.Trim(),
                TENCHINHANH = fields[1]?.Trim(),
                MAKH = fields[2]?.Trim(),
                TENKH = fields[3]?.Trim(),
                SOHOPDONG = fields[4]?.Trim(),
                SOTIENXLRR = ParseNullableDecimal(fields[5]),
                NGAYPHATSINHXL = ParseNullableDateTime(fields[6]),
                THUNOSAUXL = ParseNullableDecimal(fields[7]),
                CONLAINGOAIBANG = ParseNullableDecimal(fields[8]),
                DUNONOIBANG = ParseNullableDecimal(fields[9]),
                NHOMNO = fields[10]?.Trim(),
                MACBTD = fields[11]?.Trim(),
                TENCBTD = fields[12]?.Trim(),
                MAPGD = fields[13]?.Trim(),
                TAIKHOANHACHTOAN = fields[14]?.Trim(),
                REFNO = fields[15]?.Trim(),
                LOAINGUONVON = fields[16]?.Trim(),

                // 3 cột không có header (indexes 17-19)
                MALOAI = fields[17]?.Trim(),
                LOAIKHACHHANG = fields[18]?.Trim(),
                SOTIEN = ParseNullableDecimal(fields[19]),

                // System fields
                NGAY_DL = ngayDl,
                CREATED_DATE = DateTime.Now,
                CREATED_BY = "LN03_IMPORT"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating LN03 object from line: {line}. Error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Split CSV line theo comma nhưng handle quoted values
    /// </summary>
    private static string[] SplitCsvLine(string line)
    {
        var result = new List<string>();
        var current = new System.Text.StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        // Add the last field
        result.Add(current.ToString());

        return result.ToArray();
    }

    /// <summary>
    /// Parse string thành nullable decimal
    /// </summary>
    private static decimal? ParseNullableDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim() == "0" || value.Trim().ToLower() == "null")
            return null;

        value = value.Trim().Replace("\"", "");

        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result))
            return result;

        return null;
    }

    /// <summary>
    /// Parse string thành nullable int
    /// </summary>
    private static int? ParseNullableInt(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim() == "0" || value.Trim().ToLower() == "null")
            return null;

        value = value.Trim().Replace("\"", "");

        if (int.TryParse(value, out int result))
            return result;

        return null;
    }

    /// <summary>
    /// Parse string thành nullable DateTime cho cột ngày
    /// Format: yyyyMMdd (ví dụ: 20190628)
    /// </summary>
    private static DateTime? ParseNullableDateTime(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "null")
            return null;

        value = value.Trim().Replace("\"", "");

        // Format yyyyMMdd -> DateTime
        if (value.Length == 8 && DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            return result;

        // Thử parse format khác nếu cần
        if (DateTime.TryParse(value, out DateTime result2))
            return result2;

        return null;
    }
}
