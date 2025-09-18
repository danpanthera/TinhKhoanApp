using Microsoft.Net.Http.Headers;
using System.Text;

namespace Khoan.Api.Helpers
{
    /// <summary>
    /// Helper class để xử lý multipart requests cho streaming upload
    /// </summary>
    public static class MultipartRequestHelper
    {
        /// <summary>
        /// Kiểm tra xem request có phải multipart content type không
        /// </summary>
        public static bool IsMultipartContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType) &&
                   contentType.Contains("multipart/", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Lấy boundary từ content type header
        /// </summary>
        public static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

            if (string.IsNullOrWhiteSpace(boundary))
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }

            if (boundary.Length > lengthLimit)
            {
                throw new InvalidDataException($"Multipart boundary length limit {lengthLimit} exceeded.");
            }

            return boundary;
        }

        /// <summary>
        /// Kiểm tra xem có phải file section không
        /// </summary>
        public static bool HasFileContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            return contentDisposition != null &&
                   contentDisposition.DispositionType.Equals("form-data") &&
                   (!string.IsNullOrEmpty(contentDisposition.FileName.Value) ||
                    !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value));
        }

        /// <summary>
        /// Lấy encoding từ content type
        /// </summary>
        public static Encoding GetEncoding(MediaTypeHeaderValue contentType)
        {
            var requestEncoding = contentType?.Charset.Value;

            if (string.IsNullOrEmpty(requestEncoding))
            {
                return Encoding.UTF8;
            }

            try
            {
                return Encoding.GetEncoding(requestEncoding);
            }
            catch
            {
                return Encoding.UTF8;
            }
        }
    }
}
