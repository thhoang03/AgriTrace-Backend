using AgriTrace.Domain.Entities.Batches;
using AgriTrace.Domain.Entities.Categories;
using AgriTrace.Domain.Entities.Certificates;
using AgriTrace.Domain.Entities.Events;
using AgriTrace.Domain.Entities.Notifications;
using AgriTrace.Domain.Entities.Organizations;
using AgriTrace.Domain.Entities.Products;
using AgriTrace.Domain.Entities.QualityInspections;
using AgriTrace.Domain.Entities.Recalls;
using AgriTrace.Domain.Entities.Units;
using AgriTrace.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgriTrace.Application.Contracts
{
    public class OrganizationDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Address { get; set; }

        public OrganizationStatus Status { get; set; }

        public Guid OrganizationTypeId { get; set; }

        public string? OrganizationTypeName { get; set; }

        public string? OrganizationTypeCode { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}

