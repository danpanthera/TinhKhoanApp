using System.Globalization;

namespace TinhKhoanApp.Api.Utils
{
    /// <summary>
    /// 🔢 Utility class cho định dạng số theo chuẩn Việt Nam
    /// Quy ước: Dấu "," cho ngăn cách hàng nghìn, dấu "." cho thập phân
    /// </summary>
    public static class NumberFormatter
    {
        // CultureInfo cho định dạng Việt Nam
        private static readonly CultureInfo VietnameseCulture = new CultureInfo("vi-VN");

        /// <summary>
        /// Format số thành chuỗi với định dạng Việt Nam (dấu phẩy ngăn cách nghìn, dấu chấm thập phân)
        /// </summary>
        /// <param name="value">Giá trị số cần format</param>
        /// <param name="decimalPlaces">Số chữ số sau dấu thập phân (mặc định: 0)</param>
        /// <returns>Chuỗi đã format theo chuẩn Việt Nam</returns>
        public static string FormatNumber(decimal value, int decimalPlaces = 0)
        {
            string format = decimalPlaces > 0 ? $"N{decimalPlaces}" : "N0";
            return value.ToString(format, VietnameseCulture);
        }

        /// <summary>
        /// Format số thành chuỗi với định dạng Việt Nam
        /// </summary>
        /// <param name="value">Giá trị số cần format</param>
        /// <param name="decimalPlaces">Số chữ số sau dấu thập phân (mặc định: 0)</param>
        /// <returns>Chuỗi đã format theo chuẩn Việt Nam</returns>
        public static string FormatNumber(double value, int decimalPlaces = 0)
        {
            string format = decimalPlaces > 0 ? $"N{decimalPlaces}" : "N0";
            return value.ToString(format, VietnameseCulture);
        }

        /// <summary>
        /// Format số tiền với đơn vị (triệu VND, tỷ VND)
        /// </summary>
        /// <param name="value">Giá trị tiền tệ (VND)</param>
        /// <param name="showUnit">Có hiển thị đơn vị không</param>
        /// <returns>Chuỗi tiền tệ đã format</returns>
        public static string FormatCurrency(decimal value, bool showUnit = true)
        {
            string formattedValue = "";
            string unit = "";

            if (Math.Abs(value) >= 1_000_000_000) // Tỷ VND
            {
                formattedValue = FormatNumber(value / 1_000_000_000m, 2);
                unit = " tỷ VND";
            }
            else if (Math.Abs(value) >= 1_000_000) // Triệu VND
            {
                formattedValue = FormatNumber(value / 1_000_000m, 2);
                unit = " triệu VND";
            }
            else if (Math.Abs(value) >= 1_000) // Nghìn VND
            {
                formattedValue = FormatNumber(value / 1_000m, 2);
                unit = " nghìn VND";
            }
            else // VND
            {
                formattedValue = FormatNumber(value, 0);
                unit = " VND";
            }

            return showUnit ? formattedValue + unit : formattedValue;
        }

        /// <summary>
        /// Format phần trăm theo chuẩn Việt Nam
        /// </summary>
        /// <param name="value">Giá trị phần trăm (0-100)</param>
        /// <param name="decimalPlaces">Số chữ số sau dấu thập phân (mặc định: 2)</param>
        /// <returns>Chuỗi phần trăm đã format</returns>
        public static string FormatPercentage(decimal value, int decimalPlaces = 2)
        {
            string format = $"N{decimalPlaces}";
            return value.ToString(format, VietnameseCulture) + "%";
        }

        /// <summary>
        /// Parse chuỗi đã format thành số
        /// </summary>
        /// <param name="formattedValue">Chuỗi đã format theo chuẩn Việt Nam</param>
        /// <returns>Giá trị số</returns>
        public static decimal ParseFormattedNumber(string formattedValue)
        {
            if (string.IsNullOrWhiteSpace(formattedValue))
                return 0;

            // Loại bỏ tất cả ký tự không phải số, dấu phẩy và dấu chấm
            string cleanValue = formattedValue.Trim();

            // Loại bỏ các đơn vị tiền tệ
            cleanValue = cleanValue.Replace(" VND", "")
                                  .Replace(" triệu", "")
                                  .Replace(" tỷ", "")
                                  .Replace(" nghìn", "")
                                  .Replace("%", "")
                                  .Trim();

            if (decimal.TryParse(cleanValue, NumberStyles.Number, VietnameseCulture, out decimal result))
            {
                return result;
            }

            return 0;
        }

        /// <summary>
        /// Validate input chỉ cho phép ký tự số, dấu phẩy và dấu chấm
        /// </summary>
        /// <param name="input">Chuỗi input</param>
        /// <returns>Chuỗi đã clean</returns>
        public static string SanitizeNumberInput(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            // Chỉ cho phép: số (0-9), dấu phẩy (,), dấu chấm (.), dấu âm (-)
            return System.Text.RegularExpressions.Regex.Replace(input, @"[^\d,.\-]", "");
        }

        /// <summary>
        /// Lấy CultureInfo Việt Nam
        /// </summary>
        public static CultureInfo GetVietnameseCulture()
        {
            return VietnameseCulture;
        }
    }
}
