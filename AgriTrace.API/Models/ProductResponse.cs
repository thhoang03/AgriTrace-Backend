public class ProductResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid OrganizationId { get; set; }

    public Guid? CategoryId { get; set; }

    public Guid? UnitId { get; set; }

    public string? CategoryName { get; set; }

    public string? UnitName { get; set; }
}