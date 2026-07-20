using System.Text.Json.Serialization;

namespace AgriTrace.API.Models
{
    /// <summary>
    /// Mô tả một lỗi ở cấp field, khớp schema <c>FieldError</c> trong swagger.yaml.
    /// Hình dạng: { field, code, message }.
    /// </summary>
    public class FieldError
    {
        [JsonPropertyName("field")]
        public string Field { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}
