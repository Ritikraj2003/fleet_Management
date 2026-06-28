using System;
using System.Collections.Generic;

namespace Saas_Car_Management.Core.Entities
{
    public class Booking : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int CustomerId { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Assigned, InProgress, Completed, Cancelled
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string PickupLocation { get; set; } = string.Empty;
        public string DropLocation { get; set; } = string.Empty;
        public int? DutyTypeId { get; set; }
        
        public decimal? ActualDistance { get; set; }
        public decimal? ActualHours { get; set; }
        public decimal ExtraKmCharge { get; set; }
        public decimal ExtraHourCharge { get; set; }

        // Navigation
        public Customer Customer { get; set; } = null!;
        public DutyType? DutyType { get; set; }
        public ICollection<BookingVehicle> BookingVehicles { get; set; } = new List<BookingVehicle>();
    }

    public class BookingVehicle : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int BookingId { get; set; }
        
        // Own fleet assignment (nullable if vendor/marketplace)
        public int? CarId { get; set; }
        public int? DriverId { get; set; }

        // Vendor assignment details (nullable)
        public int? PartnerVehicleId { get; set; }
        public int? PartnerDriverId { get; set; }

        public string AssignmentType { get; set; } = "Own"; // Own, Vendor, Marketplace
        public string Status { get; set; } = "Assigned"; // Assigned, Active, Completed, Cancelled
        public DateTime? ActualStart { get; set; }
        public DateTime? ActualEnd { get; set; }
        
        public int? StartOdometer { get; set; }
        public int? EndOdometer { get; set; }
        
        public int Quantity { get; set; } = 1;
        public string RateType { get; set; } = "Fixed"; // Fixed, PerKM, Hourly
        public decimal BaseRate { get; set; }
        public decimal? Distance { get; set; }
        public decimal? Hours { get; set; }
        public decimal Fare { get; set; }

        // Navigation
        public Booking Booking { get; set; } = null!;
        public Car? Car { get; set; }
        public Driver? Driver { get; set; }
        
        // These can reference PartnerVehicle / PartnerDriver
        public PartnerVehicle? PartnerVehicle { get; set; }
        public PartnerDriver? PartnerDriver { get; set; }

        public string? MagicToken { get; set; } = Guid.NewGuid().ToString();
    }

    public class RideTracking : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int BookingVehicleId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
        public double Speed { get; set; }

        // Navigation
        public BookingVehicle BookingVehicle { get; set; } = null!;
    }
}
