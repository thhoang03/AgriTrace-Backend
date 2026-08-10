namespace AgriTrace.Domain.Entities.Certificates;

public enum CertificateStatus
{
    Pending = 0,
    UnderReview = 1,
    Approved = 2,
    Active = 3,
    Expired = 4,
    Suspended = 5,
    Rejected = 6
}
