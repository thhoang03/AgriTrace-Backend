using System.ComponentModel.DataAnnotations;

namespace AgriTrace.API.Models
{
    /// <summary>
    /// Tham số phân trang Client gửi lên (thường qua query string: ?pageNumber=1&pageSize=10).
    /// </summary>
    public class PaginationRequest
    {
        private const int MaxPageSize = 100;
        private int _pageSize = 10;

        [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be at least 1.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, MaxPageSize)]
        public int PageSize
        {
            get => _pageSize;
            // Chặn trên để tránh Client yêu cầu trang quá lớn.
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
