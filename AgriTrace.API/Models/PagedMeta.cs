using System.Text.Json.Serialization;

namespace AgriTrace.API.Models
{
    /// <summary>
    /// Metadata phân trang dùng chung, khớp schema <c>PagedMeta</c> trong swagger.yaml.
    /// Hình dạng: { totalCount, page, pageSize, totalPages }.
    /// </summary>
    public class PagedMeta
    {
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }
    }
}
