using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.Validation
{
    // Request validation attributes
    public class PaginationValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is OptimizedQueryRequest request)
            {
                if (request.Page < 1)
                {
                    ErrorMessage = "Page must be greater than 0";
                    return false;
                }

                if (request.PageSize < 1 || request.PageSize > 1000)
                {
                    ErrorMessage = "PageSize must be between 1 and 1000";
                    return false;
                }

                return true;
            }

            return false;
        }
    }

    public class SearchTermValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return true; // Optional field

            var searchTerm = value.ToString();

            if (searchTerm?.Length > 500)
            {
                ErrorMessage = "Search term cannot exceed 500 characters";
                return false;
            }

            // Check for SQL injection patterns
            var sqlInjectionPatterns = new[] { "'", "--", "/*", "*/", "xp_", "sp_", "DROP", "DELETE", "INSERT", "UPDATE" };
            if (searchTerm != null && sqlInjectionPatterns.Any(pattern => searchTerm.ToUpper().Contains(pattern.ToUpper())))
            {
                ErrorMessage = "Search term contains invalid characters";
                return false;
            }

            return true;
        }
    }

    public class DataTypeValidationAttribute : ValidationAttribute
    {
        private readonly string[] _validDataTypes = { "LN01", "LN03", "DP01", "EI01", "GL01", "DPDA", "DB01", "BC57" };

        public override bool IsValid(object? value)
        {
            if (value == null) return true; // Optional field

            var dataType = value.ToString();

            if (dataType != null && !_validDataTypes.Contains(dataType.ToUpper()))
            {
                ErrorMessage = $"Invalid data type. Valid types are: {string.Join(", ", _validDataTypes)}";
                return false;
            }

            return true;
        }
    }

    public class DateRangeValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is OptimizedQueryRequest request)
            {
                if (request.FromDate.HasValue && request.ToDate.HasValue)
                {
                    if (request.FromDate > request.ToDate)
                    {
                        ErrorMessage = "FromDate cannot be greater than ToDate";
                        return false;
                    }

                    var dateRange = request.ToDate.Value - request.FromDate.Value;
                    if (dateRange.TotalDays > 365)
                    {
                        ErrorMessage = "Date range cannot exceed 365 days";
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }

    public class VirtualScrollValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is VirtualScrollRequest request)
            {
                if (request.StartIndex < 0)
                {
                    ErrorMessage = "StartIndex must be non-negative";
                    return false;
                }

                if (request.ViewportSize < 1 || request.ViewportSize > 500)
                {
                    ErrorMessage = "ViewportSize must be between 1 and 500";
                    return false;
                }

                return true;
            }

            return false;
        }
    }

    // Enhanced request models with validation
    public class ValidatedOptimizedQueryRequest : OptimizedQueryRequest
    {
        // Remove 'new' keyword since we can't override non-virtual properties
    }

    public class ValidatedVirtualScrollRequest : VirtualScrollRequest
    {
        // Remove 'new' keyword since we can't override non-virtual properties
    }

    // Request rate limiting model
    public class RateLimitConfig
    {
        public int RequestsPerMinute { get; set; } = 100;
        public int RequestsPerHour { get; set; } = 1000;
        public int RequestsPerDay { get; set; } = 10000;
        public bool EnableRateLimit { get; set; } = true;
    }
}
