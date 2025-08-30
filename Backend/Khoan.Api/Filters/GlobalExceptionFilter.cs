using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace TinhKhoanApp.Api.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var requestId = Guid.NewGuid().ToString();

            // Log the exception with context
            _logger.LogError(exception,
                "🔥 Global Exception Handler - RequestId: {RequestId}, Method: {Method}, Path: {Path}, User: {User}",
                requestId,
                context.HttpContext.Request.Method,
                context.HttpContext.Request.Path,
                context.HttpContext.User?.Identity?.Name ?? "Anonymous"
            );

            var response = new ApiErrorResponse
            {
                RequestId = requestId,
                Timestamp = DateTime.UtcNow,
                Path = context.HttpContext.Request.Path,
                Method = context.HttpContext.Request.Method
            };

            // Handle different types of exceptions
            switch (exception)
            {
                case ArgumentException argEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Error = "Invalid Request";
                    response.Message = argEx.Message;
                    response.Details = GetExceptionDetails(argEx);
                    break;

                case UnauthorizedAccessException unauthorizedEx:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Error = "Unauthorized";
                    response.Message = "Access denied";
                    response.Details = GetExceptionDetails(unauthorizedEx);
                    break;

                case TimeoutException timeoutEx:
                    response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    response.Error = "Request Timeout";
                    response.Message = "The request took too long to process";
                    response.Details = GetExceptionDetails(timeoutEx);
                    break;

                case InvalidOperationException invalidOpEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Error = "Invalid Operation";
                    response.Message = invalidOpEx.Message;
                    response.Details = GetExceptionDetails(invalidOpEx);
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Error = "Internal Server Error";
                    response.Message = "An unexpected error occurred";
                    response.Details = GetExceptionDetails(exception);
                    break;
            }

            context.Result = new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };

            context.ExceptionHandled = true;
        }

        private object GetExceptionDetails(Exception exception)
        {
            return new
            {
                Type = exception.GetType().Name,
                StackTrace = exception.StackTrace?.Split('\n').Take(5).ToArray(), // First 5 lines only
                InnerException = exception.InnerException?.Message,
                Data = exception.Data.Count > 0 ? exception.Data : null
            };
        }
    }

    public class ApiErrorResponse
    {
        public string RequestId { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public string Path { get; set; } = "";
        public string Method { get; set; } = "";
        public int StatusCode { get; set; }
        public string Error { get; set; } = "";
        public string Message { get; set; } = "";
        public object Details { get; set; } = new { };
    }
}
