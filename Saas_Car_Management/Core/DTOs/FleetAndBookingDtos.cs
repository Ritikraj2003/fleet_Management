using System;
using System.Collections.Generic;

namespace Saas_Car_Management.Core.DTOs
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CreateCustomerDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

    public class CarDto
    {
        public int Id { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public DateTime? InsuranceExpiry { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public bool IsOwnVehicle { get; set; }
        public decimal BaseRate { get; set; }
    }

    public class CreateCarDto
    {
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Status { get; set; } = "Available";
        public string? ImagePath { get; set; }
        public DateTime? InsuranceExpiry { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public bool IsOwnVehicle { get; set; } = true;
        public decimal BaseRate { get; set; }
    }

    public class DriverDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public DateTime LicenseExpiry { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PhotoPath { get; set; }
        public int? UserId { get; set; }
    }

    public class CreateDriverDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public DateTime LicenseExpiry { get; set; }
        public string Status { get; set; } = "Available";
        public string? PhotoPath { get; set; }
        public bool CreateLogin { get; set; } = false; // Whether to create login user for the driver
    }

    public class BookingDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string PickupLocation { get; set; } = string.Empty;
        public string DropLocation { get; set; } = string.Empty;
        public int? DutyTypeId { get; set; }
        public string? DutyTypeName { get; set; }
        public decimal? ActualDistance { get; set; }
        public decimal? ActualHours { get; set; }
        public decimal ExtraKmCharge { get; set; }
        public decimal ExtraHourCharge { get; set; }
        public List<BookingVehicleDto> BookingVehicles { get; set; } = new();
    }

    public class BookingVehicleDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int? CarId { get; set; }
        public string? CarName { get; set; }
        public int? DriverId { get; set; }
        public string? DriverName { get; set; }
        public int? PartnerVehicleId { get; set; }
        public string? PartnerVehicleName { get; set; }
        public int? PartnerDriverId { get; set; }
        public string? PartnerDriverName { get; set; }
        public string AssignmentType { get; set; } = string.Empty; // Own, Vendor, Marketplace
        public string Status { get; set; } = string.Empty;
        public DateTime? ActualStart { get; set; }
        public DateTime? ActualEnd { get; set; }
        public int Quantity { get; set; }
        public string RateType { get; set; } = string.Empty;
        public decimal BaseRate { get; set; }
        public decimal? Distance { get; set; }
        public decimal? Hours { get; set; }
        public decimal Fare { get; set; }
        public string? MagicToken { get; set; }
    }

    public class CreateBookingDto
    {
        public int CustomerId { get; set; }
        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string PickupLocation { get; set; } = string.Empty;
        public string DropLocation { get; set; } = string.Empty;
        public int? DutyTypeId { get; set; }
        public List<CreateBookingVehicleRequestDto> Vehicles { get; set; } = new();
    }

    public class CreateBookingVehicleRequestDto
    {
        public string AssignmentType { get; set; } = "Own"; // Own, Vendor, Marketplace
        public int? CarId { get; set; }
        public int? DriverId { get; set; }
        public int? PartnerId { get; set; } // Vendor id
        public int? PartnerVehicleId { get; set; }
        public int? PartnerDriverId { get; set; }
        
        // Marketplace assignment params
        public int? MarketplaceOfferId { get; set; }

        public int Quantity { get; set; } = 1;
        public string RateType { get; set; } = "Fixed";
        public decimal BaseRate { get; set; }
        public decimal? Distance { get; set; }
        public decimal? Hours { get; set; }
        public decimal Fare { get; set; }
    }

    public class AssignVehicleDriverDto
    {
        public int BookingVehicleId { get; set; }
        public int? CarId { get; set; }
        public int? DriverId { get; set; }
        public int? PartnerVehicleId { get; set; }
        public int? PartnerDriverId { get; set; }
    }

    public class PagedBookingResponseDto
    {
        public IEnumerable<BookingDto> Data { get; set; } = new List<BookingDto>();
        public int TotalCount { get; set; }
    }

    public class CompleteBookingDto
    {
        public decimal? ActualDistance { get; set; }
        public decimal? ActualHours { get; set; }
    }

    public class DriverAppHomeDto
    {
        public int ActiveTrips { get; set; }
        public int CompletedTrips { get; set; }
        public bool IsCheckedIn { get; set; }
        public bool IsCheckedOut { get; set; }
        public IEnumerable<DriverAppLiveRideDto> UpcomingRides { get; set; } = new List<DriverAppLiveRideDto>();
    }

    public class DriverAppLiveRideDto
    {
        public int BookingId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string Pickup { get; set; } = string.Empty;
        public string Drop { get; set; } = string.Empty;
        public DateTime ScheduledStart { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CarDetails { get; set; } = string.Empty;
        public int BookingVehicleId { get; set; }
        public string? MagicToken { get; set; }
    }

    public class DriverAppHistoryDto
    {
        public int BookingId { get; set; }
        public string Pickup { get; set; } = string.Empty;
        public string Drop { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Fare { get; set; }
    }

    public class DriverPunchResultDto
    {
        public string Message { get; set; } = string.Empty;
        public TimeSpan Time { get; set; }
    }
}
