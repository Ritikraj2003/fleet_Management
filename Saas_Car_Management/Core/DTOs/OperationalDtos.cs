using System;
using System.Collections.Generic;

namespace Saas_Car_Management.Core.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
    }

    public class CreateUserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }

    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsSystemRole { get; set; }
        public List<string> Permissions { get; set; } = new();
    }

    public class CreateRoleDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
    }

    public class ExpenseDto
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public int? CarId { get; set; }
        public string? CarPlateNumber { get; set; }
    }

    public class CreateExpenseDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public int? CarId { get; set; }
    }

    public class UpdateExpenseDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public int? CarId { get; set; }
    }

    public class MaintenanceDto
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string CarPlateNumber { get; set; } = string.Empty;
        public string MaintenanceType { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class CreateMaintenanceDto
    {
        public int CarId { get; set; }
        public string MaintenanceType { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public DateTime StartDate { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class AttendanceDto
    {
        public int Id { get; set; }
        public int DriverId { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class CreateAttendanceDto
    {
        public int DriverId { get; set; }
        public DateTime Date { get; set; }
        public string? CheckInTime { get; set; } // "HH:mm"
        public string? CheckOutTime { get; set; } // "HH:mm"
        public string Status { get; set; } = "Present";
    }

    public class InvoiceDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class DashboardDto
    {
        public int TotalCars { get; set; }
        public int AvailableCars { get; set; }
        public int CarsOnRide { get; set; }
        public int TotalDrivers { get; set; }
        public int AvailableDrivers { get; set; }
        public int TodayBookings { get; set; }
        public int CompletedBookings { get; set; }
        public decimal Revenue { get; set; }
        public decimal PendingVendorPayments { get; set; }
        public int MarketplaceTransactions { get; set; }
        public int ActiveTrips { get; set; }
    }
}
