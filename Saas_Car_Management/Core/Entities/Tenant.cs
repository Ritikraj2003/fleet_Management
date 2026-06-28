using System;

namespace Saas_Car_Management.Core.Entities
{
    public class Tenant : BaseEntity
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty; // Unique company identifier/subdomain
        public int PlanId { get; set; }
        public string Status { get; set; } = "Active"; // Active, Suspended, Expired
        public bool IsActive { get; set; } = true;

        // Navigation
        public Plan Plan { get; set; } = null!;
    }
}
