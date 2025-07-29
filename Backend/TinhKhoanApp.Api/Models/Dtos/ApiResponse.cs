namespace TinhKhoanApp.Api.Models.DTOs
{
    /// <summary>
    /// Standard API Response - chuẩn hóa response format cho tất cả API
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu trả về</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Trạng thái thành công
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Thông báo
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Mã lỗi (nếu có)
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Dữ liệu trả về
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Thời gian xử lý (milliseconds)
        /// </summary>
        public long? ProcessingTimeMs { get; set; }

        /// <summary>
        /// Metadata bổ sung
        /// </summary>
        public object? Metadata { get; set; }

        /// <summary>
        /// Danh sách lỗi validation (nếu có)
        /// </summary>
        public object? ValidationErrors { get; set; }

        /// <summary>
        /// Tạo response thành công
        /// </summary>
        public static ApiResponse<T> Ok(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Tạo response lỗi
        /// </summary>
        public static ApiResponse<T> Error(string message, string? errorCode = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode
            };
        }
    }

    /// <summary>
    /// API Response với pagination
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu trả về</typeparam>
    public class PagedApiResponse<T> : ApiResponse<IEnumerable<T>>
    {
        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Kích thước trang
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Tổng số bản ghi
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        /// <summary>
        /// Có trang tiếp theo không
        /// </summary>
        public bool HasNextPage => Page < TotalPages;

        /// <summary>
        /// Có trang trước không
        /// </summary>
        public bool HasPreviousPage => Page > 1;

        /// <summary>
        /// Tạo paged response
        /// </summary>
        public static PagedApiResponse<T> Create(IEnumerable<T> data, int count, int page, int pageSize)
        {
            return new PagedApiResponse<T>
            {
                Success = true,
                Message = "Success",
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = count
            };
        }

        /// <summary>
        /// Tạo response thành công với data và message
        /// </summary>
        public static PagedApiResponse<T> Ok(
            IEnumerable<T> data,
            int page,
            int pageSize,
            int totalCount,
            string message = "Successful")
        {
            return new PagedApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// Tạo response lỗi với message và errorCode
        /// </summary>
        public static new PagedApiResponse<T> Error(string message, string? errorCode = null)
        {
            return new PagedApiResponse<T>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Data = new List<T>(),
                Page = 0,
                PageSize = 0,
                TotalCount = 0
            };
        }
    }
}
