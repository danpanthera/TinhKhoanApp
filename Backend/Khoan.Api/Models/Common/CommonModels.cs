using System.Collections.Generic;

namespace Khoan.Api.Models.Common
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    public class PagedRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; } = "asc";
    }

    public class FilterRequest : PagedRequest
    {
        public Dictionary<string, object> Filters { get; set; } = new Dictionary<string, object>();
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public int? StatusCode { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? ErrorCode { get; set; }

        // Static helper methods
        public static ApiResponse<T> Ok(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message ?? "Success",
                StatusCode = 200
            };
        }

        public static ApiResponse<T> Error(string message, int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                ErrorCode = statusCode.ToString(),
                StatusCode = statusCode
            };
        }

        public static ApiResponse<T> Error(string message, string errorCode, int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                StatusCode = statusCode
            };
        }

        public static ApiResponse<T> Failure(string message, int statusCode = 500)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                ErrorCode = statusCode.ToString(),
                StatusCode = statusCode
            };
        }

        public static ApiResponse<T> Failure(string message, string errorCode, int statusCode = 500)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                StatusCode = statusCode
            };
        }
    }

    public class BulkOperationResult
    {
        public int TotalProcessed { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public TimeSpan ProcessingTime { get; set; }
    }

    public class CachedResponse
    {
        public int StatusCode { get; set; }
        public string? ContentType { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public string Body { get; set; } = string.Empty;
        public DateTime CachedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// CSV Validation Result
    /// </summary>
    public class CsvValidationResult
    {
        public bool IsValid { get; set; }
        public int ExpectedColumns { get; set; }
        public int ActualColumns { get; set; }
        public List<string> MissingColumns { get; set; } = new();
        public List<string> ExtraColumns { get; set; } = new();
        public List<string> ValidationErrors { get; set; } = new();
        public List<string> ValidationWarnings { get; set; } = new();
    }
}
