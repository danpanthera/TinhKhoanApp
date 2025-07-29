using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TinhKhoanApp.Api.Models.DTOs;

namespace TinhKhoanApp.Api.Filters
{
    /// <summary>
    /// Filter để validate model state tự động và trả về ApiResponse chuẩn
    /// </summary>
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Xử lý validation trước khi action được thực thi
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                // Collect all validation errors
                var errors = context.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .Select(e => new ValidationError
                    {
                        Field = e.Key,
                        Messages = e.Value.Errors.Select(er => er.ErrorMessage).ToArray()
                    }).ToArray();

                // Return a standardized API response with validation errors
                var response = ApiResponse<object>.Error("Dữ liệu không hợp lệ", "VALIDATION_ERROR");
                response.ValidationErrors = errors;

                context.Result = new BadRequestObjectResult(response);
            }
        }
    }

    /// <summary>
    /// Cấu trúc lỗi validation
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Tên trường có lỗi
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Danh sách thông báo lỗi
        /// </summary>
        public string[] Messages { get; set; }
    }
}
