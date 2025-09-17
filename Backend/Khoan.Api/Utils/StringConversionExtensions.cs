using System;
using System.Globalization;

namespace Khoan.Api.Utils
{
    public static class StringConversionExtensions
    {
        /// <summary>
        /// Safely parse string to decimal, return 0 if null or invalid
        /// </summary>
        public static decimal GetDecimalOrDefault(this string? value, decimal defaultValue = 0m)
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;

            return defaultValue;
        }

        /// <summary>
        /// Safely parse string to int, return 0 if null or invalid
        /// </summary>
        public static int GetIntOrDefault(this string? value, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
                return result;

            return defaultValue;
        }

        /// <summary>
        /// Check if string has value (not null or whitespace)
        /// </summary>
        public static bool HasValue(this string? value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Get string or default value
        /// </summary>
        public static string GetStringOrDefault(this string? value, string defaultValue = "")
        {
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        /// <summary>
        /// Parse decimal safely - legacy method name
        /// </summary>
        public static decimal ParseDecimalSafe(this string? value)
        {
            return GetDecimalOrDefault(value);
        }

        /// <summary>
        /// Parse int safely - legacy method name
        /// </summary>
        public static int ParseIntSafe(this string? value)
        {
            return GetIntOrDefault(value);
        }

        /// <summary>
        /// Parse DateTime safely from string
        /// </summary>
        public static DateTime? ParseDateTimeSafe(this string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                return result;

            return null;
        }
    }
}
