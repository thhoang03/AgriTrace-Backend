namespace AgriTrace.API.Models
{
    public class OrgranizationResponse
    {
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; } = null!;
        public string Address { get; set; } = null!;

        public Guid OrganizationTypeId { get; set; }

        public string OrganizationStatus { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
