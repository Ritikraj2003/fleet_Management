using System;
using System.Collections.Generic;

namespace Saas_Car_Management.Core.DTOs
{
    public class PartnerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Vendor, MarketplaceCompany
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public string MagicToken { get; set; } = string.Empty;
    }

    public class CreatePartnerDto
    {
        public string Name { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Type { get; set; } = "Vendor";
    }

    public class PartnerVehicleDto
    {
        public int Id { get; set; }
        public int PartnerId { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Status { get; set; } = "Available";
        public bool IsActive { get; set; }
        public string PartnerName { get; set; } = string.Empty;
    }

    public class CreatePartnerVehicleDto
    {
        public int PartnerId { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }

    public class PartnerDriverDto
    {
        public int Id { get; set; }
        public int PartnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string PartnerName { get; set; } = string.Empty;
    }

    public class CreatePartnerDriverDto
    {
        public int PartnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
    }

    public class MarketplaceRequestDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string BuyerCompanyName { get; set; } = string.Empty;
        public int RequiredQty { get; set; }
        public string VehicleTypeRequired { get; set; } = string.Empty;
        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }
        public decimal TargetPrice { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class CreateMarketplaceRequestDto
    {
        public int RequiredQty { get; set; }
        public string VehicleTypeRequired { get; set; } = string.Empty;
        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }
        public decimal TargetPrice { get; set; }
    }

    public class MarketplaceOfferDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int BuyerTenantId { get; set; }
        public string SellerCompanyName { get; set; } = string.Empty;
        public int MarketplaceRequestId { get; set; }
        public decimal OfferPrice { get; set; }
        public int QuantityOffered { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class CreateMarketplaceOfferDto
    {
        public int MarketplaceRequestId { get; set; }
        public decimal OfferPrice { get; set; }
        public int QuantityOffered { get; set; }
    }

    public class MarketplaceAssignmentDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int MarketplaceOfferId { get; set; }
        public int ProviderCompanyId { get; set; }
        public string ProviderCompanyName { get; set; } = string.Empty;
        public string PlateNumber { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public string DriverPhone { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public string SettlementStatus { get; set; } = string.Empty;
    }

    public class AssignMarketplaceVehicleDto
    {
        public int CarId { get; set; }
        public int DriverId { get; set; }
    }

    public class VendorPaymentDto
    {
        public int Id { get; set; }
        public int PartnerId { get; set; }
        public string PartnerName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    public class CreateVendorPaymentDto
    {
        public int PartnerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    public class PagedVendorPaymentResponseDto
    {
        public IEnumerable<VendorPaymentDto> Data { get; set; } = new List<VendorPaymentDto>();
        public int TotalCount { get; set; }
    }

    public class MarketplaceTransactionDto
    {
        public int Id { get; set; }
        public int BuyerTenantId { get; set; }
        public string BuyerCompanyName { get; set; } = string.Empty;
        public int SellerTenantId { get; set; }
        public string SellerCompanyName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class CreateMarketplaceTransactionDto
    {
        public int MarketplaceAssignmentId { get; set; }
        public decimal Amount { get; set; }
    }

    public class PagedMarketplaceTransactionResponseDto
    {
        public IEnumerable<MarketplaceTransactionDto> Data { get; set; } = new List<MarketplaceTransactionDto>();
        public int TotalCount { get; set; }
    }
}
