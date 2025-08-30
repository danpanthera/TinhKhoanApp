using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Khoan.Api.Converters
{
    /// <summary>
    /// Custom DateTime converter để hiển thị thời gian theo timezone Vietnam (UTC+7)
    /// </summary>
    public class VietnamDateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly TimeZoneInfo VietnamTimeZone = GetVietnamTimeZone();

        private static TimeZoneInfo GetVietnamTimeZone()
        {
            try
            {
                // Thử Windows timezone id trước
                return TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                try
                {
                    // Fallback cho Linux/macOS
                    return TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
                }
                catch (TimeZoneNotFoundException)
                {
                    // Tạo custom timezone nếu không tìm thấy
                    return TimeZoneInfo.CreateCustomTimeZone(
                        "Vietnam Standard Time",
                        TimeSpan.FromHours(7),
                        "Vietnam Standard Time",
                        "Vietnam Standard Time"
                    );
                }
            }
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateTimeString = reader.GetString();
            if (DateTime.TryParse(dateTimeString, out var dateTime))
            {
                // Chuyển đổi về UTC nếu không có timezone info
                if (dateTime.Kind == DateTimeKind.Unspecified)
                {
                    // Giả sử input là Vietnam time, chuyển về UTC
                    return TimeZoneInfo.ConvertTimeToUtc(dateTime, VietnamTimeZone);
                }
                return dateTime;
            }
            throw new JsonException($"Unable to parse DateTime: {dateTimeString}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // Chuyển đổi từ UTC về Vietnam time để hiển thị
            DateTime vietnamTime;

            if (value.Kind == DateTimeKind.Utc)
            {
                vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(value, VietnamTimeZone);
            }
            else if (value.Kind == DateTimeKind.Unspecified)
            {
                // Giả sử đây đã là Vietnam time
                vietnamTime = value;
            }
            else
            {
                // Local time - chuyển về Vietnam time
                vietnamTime = TimeZoneInfo.ConvertTime(value, VietnamTimeZone);
            }
            // Guard invalid min values and out-of-range offsets
            if (vietnamTime.Year <= 1)
            {
                writer.WriteNullValue();
                return;
            }

            try
            {
                // Serialize với format ISO 8601 + timezone offset
                var vietnamTimeWithOffset = new DateTimeOffset(vietnamTime, VietnamTimeZone.GetUtcOffset(vietnamTime));
                writer.WriteStringValue(vietnamTimeWithOffset.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"));
            }
            catch
            {
                // Fallback: write without offset if offset application would be out of range
                writer.WriteStringValue(vietnamTime.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
            }
        }
    }

    /// <summary>
    /// Custom DateTime? converter cho nullable DateTime
    /// </summary>
    public class VietnamNullableDateTimeConverter : JsonConverter<DateTime?>
    {
        private readonly VietnamDateTimeConverter _dateTimeConverter = new();

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            return _dateTimeConverter.Read(ref reader, typeof(DateTime), options);
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                _dateTimeConverter.Write(writer, value.Value, options);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
