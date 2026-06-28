using System;

namespace Saas_Car_Management.Core.Entities
{
    public class Customer : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class Car : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Status { get; set; } = "Available"; // Available, OnRide, Maintenance
        public string? ImagePath { get; set; }
        public DateTime? InsuranceExpiry { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public bool IsOwnVehicle { get; set; } = true;
        public decimal BaseRate { get; set; }
    }

    public class Driver : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public DateTime LicenseExpiry { get; set; }
        public string Status { get; set; } = "Available"; // Available, Active, OffDuty
        public string? PhotoPath { get; set; }
        public int? UserId { get; set; } // Link to User login details

        // Navigation
        public User? User { get; set; }
    }

    public class DriverVehicleAssignment : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int DriverId { get; set; }
        public int CarId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReleasedAt { get; set; }

        // Navigation
        public Driver Driver { get; set; } = null!;
        public Car Car { get; set; } = null!;
    }
}
