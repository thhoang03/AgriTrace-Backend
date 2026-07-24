using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AgriTrace.API.Models;
using AgriTrace.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AgriTrace.API.Common
{
    /// <summary>
    /// Bắt MỌI exception chưa được xử lý và trả về đúng envelope <see cref="ErrorResponse"/>.
    /// Nhờ đó client luôn nhận cùng một hình dạng { success, data, message, errors[], timestamp } kể cả khi có lỗi.
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (statusCode, message, errors) = MapException(exception);

            if (statusCode == HttpStatusCode.InternalServerError)
            {
                // Chỉ log chi tiết cho lỗi ngoài dự kiến; không rò rỉ thông tin ra client.
                _logger.LogError(exception, "Unhandled exception while processing {Path}", httpContext.Request.Path);
            }

            var response = ErrorResponse.Fail(message, errors);
            httpContext.Response.StatusCode = (int)statusCode;
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true;
        }

        private static (HttpStatusCode statusCode, string message, List<FieldError> errors) MapException(Exception exception)
        {
            switch (exception)
            {
                case ValidationException ve:
                    var fieldErrors = ve.Errors
                        .Select(e => new FieldError
                        {
                            Field = e.PropertyName,
                            Code = string.IsNullOrEmpty(e.ErrorCode) ? "VALIDATION_ERROR" : e.ErrorCode,
                            Message = e.ErrorMessage
                        })
                        .ToList();
                    return (HttpStatusCode.BadRequest, "Validation failed", fieldErrors);

                case NotFoundException:
                    return (HttpStatusCode.NotFound, exception.Message, SingleError(exception.Message));

                case ConflictException:
                    return (HttpStatusCode.Conflict, exception.Message, SingleError(exception.Message));

                case ForbiddenException:
                    return (HttpStatusCode.Forbidden, exception.Message, SingleError(exception.Message));

                case UnauthorizedAccessException:
                    return (HttpStatusCode.Unauthorized, exception.Message, SingleError(exception.Message));

                case ArgumentException:
                    return (HttpStatusCode.BadRequest, exception.Message, SingleError(exception.Message));

                default:
                    const string genericMessage = "An unexpected error occurred.";
                    return (HttpStatusCode.InternalServerError, genericMessage, SingleError(genericMessage));
            }
        }

        private static List<FieldError> SingleError(string message) => new()
        {
            new FieldError { Field = "", Code = "ERROR", Message = message }
        };
    }
}
