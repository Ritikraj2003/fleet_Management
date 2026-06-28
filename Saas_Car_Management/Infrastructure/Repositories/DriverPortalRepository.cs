using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saas_Car_Management.Core.DTOs;
using Saas_Car_Management.Core.Interfaces;
using Saas_Car_Management.Infrastructure.Persistence;

namespace Saas_Car_Management.Infrastructure.Repositories
{
    public class DriverPortalRepository : IDriverPortalRepository
    {
        private readonly ApplicationDbContext _context;

        public DriverPortalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DriverTripDto?> GetTripByTokenAsync(string token)
        {
            var bv = await _context.BookingVehicles
                .IgnoreQueryFilters()
                .Include(b => b.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(b => b.Car)
                .Include(b => b.PartnerVehicle)
                .FirstOrDefaultAsync(b => b.MagicToken == token);

            if (bv == null || bv.Booking == null) return null;

            string carDetails = "Assigned Vehicle";
            if (bv.Car != null) carDetails = $"{bv.Car.Make} {bv.Car.Model} ({bv.Car.PlateNumber})";
            else if (bv.PartnerVehicle != null) carDetails = $"{bv.PartnerVehicle.Make} {bv.PartnerVehicle.Model} ({bv.PartnerVehicle.PlateNumber})";

            return new DriverTripDto
            {
                BookingId = bv.BookingId,
                CustomerName = bv.Booking.Customer?.Name ?? "Unknown",
                CustomerPhone = bv.Booking.Customer?.Phone ?? "Unknown",
                PickupLocation = bv.Booking.PickupLocation,
                DropLocation = bv.Booking.DropLocation,
                ScheduledStart = bv.Booking.ScheduledStart,
                Status = bv.Booking.Status,
                Notes = bv.Booking.Notes,
                CarDetails = carDetails,
                AssignmentStatus = bv.Status,
                BookingVehicleId = bv.Id
            };
        }

        public async Task<bool> StartTripAsync(string token, int startOdometer)
        {
            var bv = await _context.BookingVehicles
                .IgnoreQueryFilters()
                .Include(b => b.Booking)
                .FirstOrDefaultAsync(b => b.MagicToken == token);

            if (bv == null || bv.Booking == null) return false;

            bv.Status = "Active";
            bv.ActualStart = DateTime.UtcNow;
            bv.StartOdometer = startOdometer;
            bv.Booking.Status = "InProgress";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteTripAsync(string token, int endOdometer)
        {
            var bv = await _context.BookingVehicles
                .IgnoreQueryFilters()
                .Include(b => b.Booking)
                    .ThenInclude(b => b.DutyType)
                .Include(b => b.Booking)
                    .ThenInclude(b => b.BookingVehicles)
                .FirstOrDefaultAsync(b => b.MagicToken == token);

            if (bv == null || bv.Booking == null) return false;

            bv.Status = "Completed";
            bv.ActualEnd = DateTime.UtcNow;
            bv.EndOdometer = endOdometer;
            
            decimal calculatedDistance = 0;
            if (bv.StartOdometer.HasValue && endOdometer >= bv.StartOdometer.Value)
            {
                calculatedDistance = endOdometer - bv.StartOdometer.Value;
                bv.Distance = calculatedDistance;
            }
            
            decimal calculatedHours = 0;
            if (bv.ActualStart.HasValue)
            {
                calculatedHours = (decimal)(bv.ActualEnd.Value - bv.ActualStart.Value).TotalHours;
                bv.Hours = calculatedHours;
            }

            // Accumulate distance and hours to the main booking object
            bv.Booking.ActualDistance = (bv.Booking.ActualDistance ?? 0) + calculatedDistance;
            bv.Booking.ActualHours = (bv.Booking.ActualHours ?? 0) + calculatedHours;

            // Revert own car & driver back to Available
            if (bv.AssignmentType == "Own")
            {
                if (bv.CarId.HasValue)
                {
                    var car = await _context.Cars.FindAsync(bv.CarId.Value);
                    if (car != null) car.Status = "Available";
                }
                if (bv.DriverId.HasValue)
                {
                    var driver = await _context.Drivers.FindAsync(bv.DriverId.Value);
                    if (driver != null) driver.Status = "Available";
                }
            }

            // CHECK IF ALL VEHICLES ARE COMPLETED OR CANCELLED
            bool allCompleted = bv.Booking.BookingVehicles.All(v => v.Status == "Completed" || v.Status == "Cancelled");

            if (allCompleted)
            {
                bv.Booking.Status = "Completed";

                // Calculate extra charges if duty type exists based on total accumulated distance/hours
                decimal extraCharge = 0;
                decimal kmCharge = 0;
                decimal hrCharge = 0;

                if (bv.Booking.DutyType != null)
                {
                    if (bv.Booking.ActualDistance.Value > bv.Booking.DutyType.MaxKilometers)
                    {
                        kmCharge = (bv.Booking.ActualDistance.Value - bv.Booking.DutyType.MaxKilometers) * bv.Booking.DutyType.ExtraKmRate;
                    }
                    if (bv.Booking.ActualHours.Value > bv.Booking.DutyType.MaxHours)
                    {
                        hrCharge = (bv.Booking.ActualHours.Value - bv.Booking.DutyType.MaxHours) * bv.Booking.DutyType.ExtraHourRate;
                    }
                }

                bv.Booking.ExtraKmCharge = kmCharge;
                bv.Booking.ExtraHourCharge = hrCharge;
                extraCharge = kmCharge + hrCharge;

                if (extraCharge > 0)
                {
                    var invoice = await _context.Invoices.IgnoreQueryFilters().FirstOrDefaultAsync(i => i.BookingId == bv.BookingId);
                    if (invoice != null)
                    {
                        invoice.TotalAmount += extraCharge;
                        invoice.TaxAmount = invoice.TotalAmount * 0.15m;
                    }
                    bv.Booking.TotalAmount += extraCharge;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
