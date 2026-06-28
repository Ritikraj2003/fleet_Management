using System;

namespace Saas_Car_Management.Core.Entities
{
    public class Invoice : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int BookingId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string Status { get; set; } = "Unpaid"; // Unpaid, Paid, Overdue

        // Navigation
        public Booking Booking { get; set; } = null!;
    }

    public class VendorPayment : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int PartnerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        // Navigation
        public Partner Partner { get; set; } = null!;
    }

    public class Expense : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string Category { get; set; } = string.Empty; // Fuel, Maintenance, Salary, Rent, Other
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;
        public string Description { get; set; } = string.Empty;
        public int? CarId { get; set; }

        // Navigation
        public Car? Car { get; set; }
    }

    public class VehicleMaintenance : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int CarId { get; set; }
        public string MaintenanceType { get; set; } = string.Empty; // Routine, Repair, Accident, etc.
        public decimal Cost { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed

        // Navigation
        public Car Car { get; set; } = null!;
    }

    public class DriverAttendance : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int DriverId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public string Status { get; set; } = "Present"; // Present, Absent, Leave

        // Navigation
        public Driver Driver { get; set; } = null!;
    }

    public class AuditLog : BaseEntity
    {
        public int? TenantId { get; set; } // Nullable for Super Admin actions
        public int? UserId { get; set; }
        public string Action { get; set; } = string.Empty; // e.g. "Create", "Update", "Delete"
        public string EntityName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string Changes { get; set; } = string.Empty; // JSON string of changes
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class File : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
    }

    public class EntityFile : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string EntityName { get; set; } = string.Empty; // e.g. "Car", "Driver", "Partner"
        public int EntityId { get; set; }
        public int FileId { get; set; }

        // Navigation
        public File File { get; set; } = null!;
    }
}
