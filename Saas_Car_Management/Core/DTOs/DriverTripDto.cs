using System;

namespace Saas_Car_Management.Core.DTOs
{
    public class DriverTripDto
    {
        public int BookingId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string PickupLocation { get; set; } = string.Empty;
        public string DropLocation { get; set; } = string.Empty;
        public DateTime ScheduledStart { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string CarDetails { get; set; } = string.Empty;
        public string AssignmentStatus { get; set; } = string.Empty;
        public int BookingVehicleId { get; set; }
    }

    public class DriverProfileDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public DateTime LicenseExpiry { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string VehicleModel { get; set; } = string.Empty;
    }

    public class UpdateDriverProfileDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class EndTripRequestDto
    {
        public decimal EndOdo { get; set; }
        public decimal Tolls { get; set; }
    }

    public class DriverAttendanceRecordDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public string WorkingHours { get; set; } = string.Empty;
    }

    public class DriverAttendanceSummaryDto
    {
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int TotalHours { get; set; }
        public List<DriverAttendanceRecordDto> Records { get; set; } = new List<DriverAttendanceRecordDto>();
    }
}
