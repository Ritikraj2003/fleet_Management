using System;

namespace Saas_Car_Management.Core.Entities
{
    public class Partner : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Type { get; set; } = "Vendor"; // Vendor, MarketplaceCompany
        public decimal Balance { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public string MagicToken { get; set; } = Guid.NewGuid().ToString();
    }

    public class PartnerVehicle : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int PartnerId { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Status { get; set; } = "Available"; // Available, Busy
        public bool IsActive { get; set; } = true;

        // Navigation
        public Partner Partner { get; set; } = null!;
    }

    public class PartnerDriver : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int PartnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Navigation
        public Partner Partner { get; set; } = null!;
    }

    public class MarketplaceRequest : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; } // Buyer Company
        public int RequiredQty { get; set; }
        public string VehicleTypeRequired { get; set; } = string.Empty; // Sedan, SUV, Van, etc.
        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }
        public decimal TargetPrice { get; set; }
        public string Status { get; set; } = "Open"; // Open, Fulfilled, Expired, Cancelled
    }

    public class MarketplaceOffer : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; } // Seller Company
        public int MarketplaceRequestId { get; set; }
        public decimal OfferPrice { get; set; }
        public int QuantityOffered { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Accepted, Rejected

        // Navigation
        public MarketplaceRequest MarketplaceRequest { get; set; } = null!;
    }

    public class MarketplaceAssignment : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; } // Buyer Company
        public int MarketplaceOfferId { get; set; }
        public int ProviderCompanyId { get; set; } // Seller Company
        
        // References to Seller's vehicles (if any) or stored details
        public int? CarId { get; set; }
        public int? DriverId { get; set; }
        
        public string PlateNumber { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public string DriverPhone { get; set; } = string.Empty;
        
        public decimal Price { get; set; }
        public string Status { get; set; } = "Assigned"; // Assigned, Active, Completed, Cancelled
        public string SettlementStatus { get; set; } = "Pending"; // Pending, Settled

        // Navigation
        public MarketplaceOffer MarketplaceOffer { get; set; } = null!;
    }

    public class MarketplaceVehicleAvailability : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string VehicleType { get; set; } = string.Empty;
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }
        public string Status { get; set; } = "Available"; // Available, Booked
    }

    public class MarketplaceTransaction : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; } // Entity's own view of transaction
        public int BuyerTenantId { get; set; }
        public int SellerTenantId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = "Settlement"; // Settlement, Commission
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending"; // Pending, Completed
    }
}
