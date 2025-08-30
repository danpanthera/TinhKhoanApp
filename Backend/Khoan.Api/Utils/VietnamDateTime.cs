namespace TinhKhoanApp.Api.Utils
{
    /// <summary>
    /// Utility class để xử lý DateTime với timezone Vietnam (UTC+7)
    /// </summary>
    public static class VietnamDateTime
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

        /// <summary>
        /// Lấy thời gian hiện tại theo timezone Vietnam (UTC+7)
        /// </summary>
        public static DateTime Now => TimeZoneInfo.ConvertTime(DateTime.UtcNow, VietnamTimeZone);

        /// <summary>
        /// Lấy ngày hiện tại theo timezone Vietnam (UTC+7)
        /// </summary>
        public static DateTime Today => Now.Date;

        /// <summary>
        /// Chuyển đổi UTC time sang Vietnam time
        /// </summary>
        public static DateTime FromUtc(DateTime utcDateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, VietnamTimeZone);
        }

        /// <summary>
        /// Chuyển đổi Vietnam time sang UTC
        /// </summary>
        public static DateTime ToUtc(DateTime vietnamDateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(vietnamDateTime, VietnamTimeZone);
        }

        /// <summary>
        /// Format Vietnam time thành string
        /// </summary>
        public static string ToString(DateTime dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return dateTime.ToString(format);
        }

        /// <summary>
        /// Lấy thời gian hiện tại dạng string theo format Vietnam
        /// </summary>
        public static string NowString(string format = "yyyy-MM-dd HH:mm:ss")
        {
            return Now.ToString(format);
        }

        /// <summary>
        /// Lấy ngày hiện tại dạng string theo format Vietnam
        /// </summary>
        public static string TodayString(string format = "yyyy-MM-dd")
        {
            return Today.ToString(format);
        }

        /// <summary>
        /// Format ngày theo chuẩn Việt Nam dd/mm/yyyy
        /// </summary>
        public static string ToVietnameseDateString(DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Format ngày giờ theo chuẩn Việt Nam dd/mm/yyyy HH:mm:ss
        /// </summary>
        public static string ToVietnameseDateTimeString(DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
        }

        /// <summary>
        /// Lấy ngày hiện tại theo format Việt Nam dd/mm/yyyy
        /// </summary>
        public static string TodayVietnameseString()
        {
            return ToVietnameseDateString(Today);
        }

        /// <summary>
        /// Lấy ngày giờ hiện tại theo format Việt Nam dd/mm/yyyy HH:mm:ss
        /// </summary>
        public static string NowVietnameseString()
        {
            return ToVietnameseDateTimeString(Now);
        }
    }
}
