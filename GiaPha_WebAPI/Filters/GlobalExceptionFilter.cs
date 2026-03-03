using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FluentValidation;

namespace GiaPha_WebAPI.Filters
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
            //  FluentValidation
            if (context.Exception is ValidationException validationException)
            {
                var errors = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                _logger.LogWarning("Validation failed: {@Errors}", errors);

                context.Result = new BadRequestObjectResult(new
                {
                    status = StatusCodes.Status400BadRequest,
                    errorCode = "VALIDATION_ERROR",
                    message = "Dữ liệu không hợp lệ",
                    errors = errors,
                    traceId = context.HttpContext.TraceIdentifier
                });

                context.ExceptionHandled = true;
                return;
            }

            //  Business Logic Errors (Email/Username trùng, etc.)
            if (context.Exception is InvalidOperationException invalidOpException)
            {
                _logger.LogWarning("Business logic error: {Message}", invalidOpException.Message);

                context.Result = new BadRequestObjectResult(new
                {
                    status = StatusCodes.Status400BadRequest,
                    errorCode = "BUSINESS_LOGIC_ERROR",
                    message = invalidOpException.Message,
                    traceId = context.HttpContext.TraceIdentifier
                });

                context.ExceptionHandled = true;
                return;
            }

            //  Unauthorized (Login failed, Invalid token, etc.)
            if (context.Exception is UnauthorizedAccessException unauthorizedException)
            {
                _logger.LogWarning("Unauthorized access: {Message}", unauthorizedException.Message);

                context.Result = new ObjectResult(new
                {
                    status = StatusCodes.Status401Unauthorized,
                    errorCode = "UNAUTHORIZED",
                    message = unauthorizedException.Message,
                    traceId = context.HttpContext.TraceIdentifier
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };

                context.ExceptionHandled = true;
                return;
            }

            //  Lỗi không xác định
            _logger.LogError(context.Exception, "Unhandled exception");

            context.Result = new ObjectResult(new
            {
                status = StatusCodes.Status500InternalServerError,
                errorCode = "INTERNAL_SERVER_ERROR",
                message = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.",
                traceId = context.HttpContext.TraceIdentifier
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            context.ExceptionHandled = true;
        }
    }
}
