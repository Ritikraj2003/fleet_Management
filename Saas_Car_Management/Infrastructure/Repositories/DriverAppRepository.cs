using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saas_Car_Management.Core.DTOs;
using Saas_Car_Management.Core.Entities;
using Saas_Car_Management.Core.Interfaces;
using Saas_Car_Management.Infrastructure.Persistence;

namespace Saas_Car_Management.Infrastructure.Repositories
{
    public class DriverAppRepository : IDriverAppRepository
    {
        private readonly ApplicationDbContext _context;

        public DriverAppRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<int?> GetDriverIdAsync(int userId)
        {
            var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.UserId == userId);
            return driver?.Id;
        }

        public async Task<DriverAppHomeDto?> GetHomeDataAsync(int userId)
        {
            var driverId = await GetDriverIdAsync(userId);
            if (driverId == null) return null;

            var today = DateTime.UtcNow.Date;

            var activeTripsCount = await _context.BookingVehicles
                .Where(bv => bv.DriverId == driverId && bv.Status == "Active")
                .CountAsync();

            var completedTripsCount = await _context.BookingVehicles
                .Where(bv => bv.DriverId == driverId && bv.Status == "Completed")
                .CountAsync();

            var attendance = await _context.DriverAttendances
                .FirstOrDefaultAsync(a => a.DriverId == driverId && a.Date.Date == today);

            var upcomingRides = await _context.BookingVehicles
                .Include(bv => bv.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(bv => bv.Car)
                .Where(bv => bv.DriverId == driverId && bv.Status == "Assigned")
                .Select(bv => new DriverAppLiveRideDto
                {
                    BookingId = bv.BookingId,
                    CustomerName = bv.Booking.Customer != null ? bv.Booking.Customer.Name : "Unknown",
                    CustomerPhone = bv.Booking.Customer != null ? bv.Booking.Customer.Phone : "Unknown",
                    Pickup = bv.Booking.PickupLocation,
                    Drop = bv.Booking.DropLocation,
                    ScheduledStart = bv.Booking.ScheduledStart,
                    Status = bv.Status,
                    CarDetails = bv.Car != null ? $"{bv.Car.Make} {bv.Car.Model}" : "Assigned Car",
                    BookingVehicleId = bv.Id,
                    MagicToken = bv.MagicToken
                })
                .ToListAsync();

            return new DriverAppHomeDto
            {
                ActiveTrips = activeTripsCount,
                CompletedTrips = completedTripsCount,
                IsCheckedIn = attendance != null && attendance.CheckInTime != null,
                IsCheckedOut = attendance != null && attendance.CheckOutTime != null,
                UpcomingRides = upcomingRides
            };
        }

        public async Task<IEnumerable<DriverAppLiveRideDto>> GetLiveRidesAsync(int userId)
        {
            var driverId = await GetDriverIdAsync(userId);
            if (driverId == null) return new List<DriverAppLiveRideDto>();

            var liveRides = await _context.BookingVehicles
                .Include(bv => bv.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(bv => bv.Car)
                .Where(bv => bv.DriverId == driverId && (bv.Status == "Assigned" || bv.Status == "Active" || bv.Status == "InProgress"))
                .Select(bv => new DriverAppLiveRideDto
                {
                    BookingId = bv.BookingId,
                    CustomerName = bv.Booking.Customer != null ? bv.Booking.Customer.Name : "Unknown",
                    CustomerPhone = bv.Booking.Customer != null ? bv.Booking.Customer.Phone : "Unknown",
                    Pickup = bv.Booking.PickupLocation,
                    Drop = bv.Booking.DropLocation,
                    ScheduledStart = bv.Booking.ScheduledStart,
                    Status = bv.Status,
                    CarDetails = bv.Car != null ? $"{bv.Car.Make} {bv.Car.Model}" : "Assigned Car",
                    BookingVehicleId = bv.Id,
                    MagicToken = bv.MagicToken
                })
                .ToListAsync();

            return liveRides;
        }

        public async Task<IEnumerable<DriverAppHistoryDto>> GetHistoryRidesAsync(int userId)
        {
            var driverId = await GetDriverIdAsync(userId);
            if (driverId == null) return new List<DriverAppHistoryDto>();

            var history = await _context.BookingVehicles
                .Include(bv => bv.Booking)
                .Where(bv => bv.DriverId == driverId)
                .OrderByDescending(bv => bv.Booking.ScheduledStart)
                .Select(bv => new DriverAppHistoryDto
                {
                    BookingId = bv.BookingId,
                    Pickup = bv.Booking.PickupLocation,
                    Drop = bv.Booking.DropLocation,
                    Date = bv.Booking.ScheduledStart,
                    Status = bv.Status,
                    Fare = bv.Fare
                })
                .Take(20)
                .ToListAsync();

            return history;
        }

        public async Task<DriverPunchResultDto?> PunchAttendanceAsync(int userId)
        {
            var driverId = await GetDriverIdAsync(userId);
            if (driverId == null) return null;

            var today = DateTime.UtcNow.Date;
            var currentTime = DateTime.UtcNow.TimeOfDay;

            var attendance = await _context.DriverAttendances
                .FirstOrDefaultAsync(a => a.DriverId == driverId && a.Date.Date == today);

            if (attendance == null)
            {
                attendance = new DriverAttendance
                {
                    DriverId = driverId.Value,
                    Date = today,
                    CheckInTime = currentTime,
                    Status = "Present"
                };
                _context.DriverAttendances.Add(attendance);
                await _context.SaveChangesAsync();
                return new DriverPunchResultDto { Message = "Punched In Successfully", Time = currentTime };
            }
            else if (attendance.CheckOutTime == null)
            {
                attendance.CheckOutTime = currentTime;
                await _context.SaveChangesAsync();
                return new DriverPunchResultDto { Message = "Punched Out Successfully", Time = currentTime };
            }

            return new DriverPunchResultDto { Message = "Already punched out for today.", Time = currentTime };
        }

        public async Task<DriverProfileDto?> GetProfileAsync(int userId)
        {
            var driver = await _context.Drivers
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (driver == null) return null;

            var assignment = await _context.DriverVehicleAssignments
                .Include(a => a.Car)
                .Where(a => a.DriverId == driver.Id && a.ReleasedAt == null)
                .FirstOrDefaultAsync();

            return new DriverProfileDto
            {
                Id = driver.Id,
                FirstName = driver.FirstName,
                LastName = driver.LastName,
                Email = driver.Email,
                Phone = driver.Phone,
                LicenseNumber = driver.LicenseNumber,
                LicenseExpiry = driver.LicenseExpiry,
                VehicleNumber = assignment?.Car?.PlateNumber ?? "Not Assigned",
                VehicleModel = assignment?.Car != null ? $"{assignment.Car.Make} {assignment.Car.Model}" : "N/A"
            };
        }

        public async Task<bool> UpdateProfileAsync(int userId, UpdateDriverProfileDto dto)
        {
            var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.UserId == userId);
            if (driver == null) return false;

            driver.FirstName = dto.FirstName;
            driver.LastName = dto.LastName;
            driver.Phone = dto.Phone;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> StartTripAsync(int userId, int bookingVehicleId)
        {
            var driverId = await GetDriverIdAsync(userId);
            if (driverId == null) return false;

            var bookingVehicle = await _context.BookingVehicles
                .FirstOrDefaultAsync(bv => bv.Id == bookingVehicleId && bv.DriverId == driverId);

            if (bookingVehicle == null || bookingVehicle.Status != "Assigned") return false;

            bookingVehicle.Status = "InProgress";
            
            var booking = await _context.Bookings.FindAsync(bookingVehicle.BookingId);
            if (booking != null && booking.Status == "Assigned")
            {
                booking.Status = "InProgress";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EndTripAsync(int userId, int bookingVehicleId, decimal endOdo, decimal tolls)
        {
            var driverId = await GetDriverIdAsync(userId);
            if (driverId == null) return false;

            var bookingVehicle = await _context.BookingVehicles
                .FirstOrDefaultAsync(bv => bv.Id == bookingVehicleId && bv.DriverId == driverId);

            if (bookingVehicle == null || (bookingVehicle.Status != "InProgress" && bookingVehicle.Status != "Active")) return false;

            bookingVehicle.Status = "Completed";
            bookingVehicle.ActualEnd = DateTime.UtcNow;

            // Revert own car & driver back to Available
            if (bookingVehicle.AssignmentType == "Own" || string.IsNullOrEmpty(bookingVehicle.AssignmentType))
            {
                if (bookingVehicle.CarId.HasValue)
                {
                    var car = await _context.Cars.FindAsync(bookingVehicle.CarId.Value);
                    if (car != null) car.Status = "Available";
                }
                if (bookingVehicle.DriverId.HasValue)
                {
                    var driver = await _context.Drivers.FindAsync(bookingVehicle.DriverId.Value);
                    if (driver != null) driver.Status = "Available";
                }
            }
            
            var booking = await _context.Bookings
                .Include(b => b.BookingVehicles)
                .FirstOrDefaultAsync(b => b.Id == bookingVehicle.BookingId);
                
            if (booking != null)
            {
                // Check if all other vehicles are also completed or cancelled
                bool allFinished = booking.BookingVehicles.All(bv => bv.Status == "Completed" || bv.Status == "Cancelled");
                if (allFinished)
                {
                    booking.Status = "Completed";
                }
            }

            // Update ODO or other fare details if needed on bookingVehicle/Booking
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DriverAttendanceSummaryDto?> GetAttendanceHistoryAsync(int userId)
        {
            var driverId = await GetDriverIdAsync(userId);
            if (driverId == null) return null;

            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;

            var records = await _context.DriverAttendances
                .Where(a => a.DriverId == driverId && a.Date.Month == currentMonth && a.Date.Year == currentYear)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            var recordDtos = new List<DriverAttendanceRecordDto>();
            int totalMinutes = 0;

            foreach (var r in records)
            {
                string workingHoursStr = "0h 0m";
                if (r.CheckInTime.HasValue && r.CheckOutTime.HasValue)
                {
                    var duration = r.CheckOutTime.Value - r.CheckInTime.Value;
                    totalMinutes += (int)duration.TotalMinutes;
                    workingHoursStr = $"{(int)duration.TotalHours}h {duration.Minutes}m";
                }

                recordDtos.Add(new DriverAttendanceRecordDto
                {
                    Id = r.Id,
                    Date = r.Date,
                    Status = r.Status,
                    CheckInTime = r.CheckInTime,
                    CheckOutTime = r.CheckOutTime,
                    WorkingHours = workingHoursStr
                });
            }

            int presentDays = records.Count(r => r.Status == "Present");
            
            // Simple absent calculation: days passed in month - present days (excluding weekends if you want, but for simplicity let's do days passed)
            int daysPassedInMonth = DateTime.UtcNow.Day;
            int absentDays = Math.Max(0, daysPassedInMonth - presentDays);

            return new DriverAttendanceSummaryDto
            {
                PresentDays = presentDays,
                AbsentDays = absentDays,
                TotalHours = totalMinutes / 60,
                Records = recordDtos
            };
        }
    }
}
