namespace AgriTrace.Infrastructure.Sqlserver.Models
{
    /// <summary>
    /// Base class cho các DataModel.
    /// Chứa các thuộc tính dùng chung của Database.
    /// </summary>
    public abstract class BaseDataModel
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}