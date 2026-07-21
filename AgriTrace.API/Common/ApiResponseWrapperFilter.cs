using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgriTrace.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AgriTrace.API.Common
{
    /// <summary>
    /// Tự động bọc MỌI kết quả trả về của controller vào envelope chuẩn của swagger.
    /// - Status 2xx: bọc thành <see cref="ApiResponse"/> ({ success, data, message, timestamp }).
    /// - Status khác: bọc thành <see cref="ErrorResponse"/> ({ success, data, message, errors[], timestamp }).
    /// Nhờ đó controller chỉ cần trả về dữ liệu thô (Ok(data), Created(...), v.v.).
    /// </summary>
    public class ApiResponseWrapperFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            switch (context.Result)
            {
                // Đã là envelope chuẩn thì giữ nguyên (ví dụ output của GlobalExceptionHandler hoặc controller tự dựng).
                case ObjectResult { Value: ApiResponse }:
                case ObjectResult { Value: ErrorResponse }:
                    break;

                // Có body: bọc lại Value, giữ nguyên loại result (để không mất header Location của Created...).
                case ObjectResult objectResult:
                    objectResult.Value = Build(objectResult.Value, objectResult.StatusCode ?? StatusCodes.Status200OK);
                    break;

                // RFC 9110 §15.3.5: 204/205/304 KHÔNG được có message body — giữ nguyên, không wrap.
                case StatusCodeResult { StatusCode: 204 or 205 or 304 }:
                    break;

                // Không có body (StatusCode(...), NotFound() không kèm dữ liệu).
                case StatusCodeResult statusCodeResult:
                    context.Result = new ObjectResult(Build(null, statusCodeResult.StatusCode))
                    {
                        StatusCode = statusCodeResult.StatusCode
                    };
                    break;

                case EmptyResult:
                    context.Result = new ObjectResult(Build(null, StatusCodes.Status200OK))
                    {
                        StatusCode = StatusCodes.Status200OK
                    };
                    break;
            }

            await next();
        }

        private static object Build(object? value, int statusCode)
        {
            if (statusCode is >= 200 and < 300)
            {
                return ApiResponse.Success(value);
            }

            var errors = ExtractMessages(value, statusCode);
            var message = errors.Count > 0 ? errors[0].Message : DefaultMessage(statusCode);
            return ErrorResponse.Fail(message, errors);
        }

        private static List<FieldError> ExtractMessages(object? value, int statusCode)
        {
            switch (value)
            {
                // Lỗi validation tự động của [ApiController] (ModelState).
                case ValidationProblemDetails vpd:
                    return vpd.Errors
                        .SelectMany(kvp => kvp.Value.Select(msg => new FieldError
                        {
                            Field = kvp.Key,
                            Code = "VALIDATION_ERROR",
                            Message = msg
                        }))
                        .ToList();

                case ProblemDetails pd:
                    return new List<FieldError>
                    {
                        new() { Field = "", Code = "ERROR", Message = pd.Detail ?? pd.Title ?? DefaultMessage(statusCode) }
                    };

                case string s:
                    return new List<FieldError>
                    {
                        new() { Field = "", Code = "ERROR", Message = s }
                    };

                case null:
                    return new List<FieldError>
                    {
                        new() { Field = "", Code = "ERROR", Message = DefaultMessage(statusCode) }
                    };

                default:
                    return new List<FieldError>
                    {
                        new() { Field = "", Code = "ERROR", Message = value.ToString()! }
                    };
            }
        }

        private static string DefaultMessage(int statusCode) =>
            ((System.Net.HttpStatusCode)statusCode).ToString();
    }
}
