namespace AgriTrace.Infrastructure.Sqlserver.Models
{

    /// <summary>
    /// Base class cho tất cả DataModel.
    /// Mapping với các cột dùng chung trong Database.
    /// </summary>
    public abstract class BaseDataModel
    {

        /// <summary>
        /// Primary Key
        /// </summary>
        public Guid Id { get; set; }



        /// <summary>
        /// Thời gian tạo record
        /// </summary>
        public DateTime CreatedAt { get; set; }



        /// <summary>
        /// Thời gian cập nhật record gần nhất
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

    }

}