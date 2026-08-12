using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AgriTrace.API.Models
{
    /// <summary>
    /// Envelope chuẩn cho response lỗi của API, khớp schema <c>ErrorResponse</c> trong swagger.yaml.
    /// Hình dạng: { success, data, message, errors[], timestamp }.
    /// </summary>
    public class ErrorResponse
    {
        [JsonPropertyName("success")]
        public bool IsSuccess { get; set; } = false;

        [JsonPropertyName("data")]
        public object? Data { get; set; } = null;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("errors")]
        public List<FieldError> Errors { get; set; } = new();

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // ---- Factory helpers ----

        public static ErrorResponse Fail(string message, List<FieldError>? errors = null) => new()
        {
            IsSuccess = false,
            Data = null,
            Message = message,
            Errors = errors ?? new List<FieldError>(),
            Timestamp = DateTime.UtcNow
        };
    }
}
