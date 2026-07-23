using System;
using System.Net;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models
{
    /// <summary>
    /// Envelope chuẩn cho response thành công của API, khớp schema <c>ApiResponse</c> trong swagger.yaml.
    /// Hình dạng: { success, data, message, timestamp }.
    /// </summary>
    public class ApiResponse
    {
        [JsonPropertyName("success")]
        public bool IsSuccess { get; set; } = true;

        [JsonPropertyName("data")]
        public object? Data { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // ---- Factory helpers cho gọn ở controller ----

        public static ApiResponse Success(object? data, string message = "Thực hiện thành công") => new()
        {
            IsSuccess = true,
            Data = data,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        public static ApiResponse Fail(HttpStatusCode statusCode, string message = "Thất bại") => new()
        {
            IsSuccess = false,
            Data = null,
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }
}
