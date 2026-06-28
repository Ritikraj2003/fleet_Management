using System;

namespace Saas_Car_Management.Core.Entities
{
    public class Plan : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int MaxCars { get; set; }
        public int MaxUsers { get; set; }
        public string EnabledModules { get; set; } = string.Empty; // Comma-separated module names (e.g. Cars,Drivers,Bookings,VendorManagement,Marketplace,Reports)
        public bool IsActive { get; set; } = true;
    }

    public class Subscription : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Active"; // Active, Expired, Cancelled, Suspended
        
        // Navigation properties
        public Plan Plan { get; set; } = null!;
    }
}
