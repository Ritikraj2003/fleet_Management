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
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsAsync(int tenantId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.TenantId == tenantId && b.Status != "Completed" && b.Status != "Cancelled")
                .Include(b => b.Customer)
                .Include(b => b.DutyType)
                .Include(b => b.BookingVehicles)
                .OrderByDescending(b => b.Id)
                .ToListAsync();

            var dtos = new List<BookingDto>();
            foreach (var b in bookings)
            {
                var vehicles = new List<BookingVehicleDto>();
                foreach (var bv in b.BookingVehicles)
                {
                    string? carName = null;
                    if (bv.CarId.HasValue)
                    {
                        var car = await _context.Cars.FindAsync(bv.CarId.Value);
                        carName = car != null ? $"{car.Make} {car.Model}" : null;
                    }

                    string? driverName = null;
                    if (bv.DriverId.HasValue)
                    {
                        var drv = await _context.Drivers.FindAsync(bv.DriverId.Value);
                        driverName = drv != null ? $"{drv.FirstName} {drv.LastName}" : null;
                    }

                    string? partnerVehicleName = null;
                    if (bv.PartnerVehicleId.HasValue)
                    {
                        var pv = await _context.PartnerVehicles.FindAsync(bv.PartnerVehicleId.Value);
                        partnerVehicleName = pv != null ? $"{pv.Make} {pv.Model}" : null;
                    }

                    string? partnerDriverName = null;
                    if (bv.PartnerDriverId.HasValue)
                    {
                        var pd = await _context.PartnerDrivers.FindAsync(bv.PartnerDriverId.Value);
                        partnerDriverName = pd?.Name;
                    }

                    vehicles.Add(new BookingVehicleDto
                    {
                        Id = bv.Id,
                        BookingId = bv.BookingId,
                        CarId = bv.CarId,
                        CarName = carName,
                        DriverId = bv.DriverId,
                        DriverName = driverName,
                        PartnerVehicleId = bv.PartnerVehicleId,
                        PartnerVehicleName = partnerVehicleName,
                        PartnerDriverId = bv.PartnerDriverId,
                        PartnerDriverName = partnerDriverName,
                        AssignmentType = bv.AssignmentType,
                        Status = bv.Status,
                        ActualStart = bv.ActualStart,
                        ActualEnd = bv.ActualEnd,
                        Quantity = bv.Quantity,
                        RateType = bv.RateType,
                        BaseRate = bv.BaseRate,
                        Distance = bv.Distance,
                        Hours = bv.Hours,
                        Fare = bv.Fare,
                        MagicToken = bv.MagicToken
                    });
                }

                dtos.Add(new BookingDto
                {
                    Id = b.Id,
                    CustomerId = b.CustomerId,
                    CustomerName = b.Customer?.Name ?? "Unknown",
                    DutyTypeId = b.DutyTypeId,
                    DutyTypeName = b.DutyType?.Name,
                    ActualDistance = b.ActualDistance,
                    ActualHours = b.ActualHours,
                    ExtraKmCharge = b.ExtraKmCharge,
                    ExtraHourCharge = b.ExtraHourCharge,
                    BookingDate = b.BookingDate,
                    ScheduledStart = b.ScheduledStart,
                    ScheduledEnd = b.ScheduledEnd,
                    Status = b.Status,
                    TotalAmount = b.TotalAmount,
                    Notes = b.Notes,
                    PickupLocation = b.PickupLocation,
                    DropLocation = b.DropLocation,
                    BookingVehicles = vehicles
                });
            }

            return dtos;
        }

        public async Task<PagedBookingResponseDto> GetBookingHistoryAsync(int tenantId, int page = 1, int pageSize = 10, string search = "")
        {
            var query = _context.Bookings
                .Where(b => b.TenantId == tenantId)
                .Include(b => b.Customer)
                .Include(b => b.DutyType)
                .Include(b => b.BookingVehicles)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                query = query.Where(b => 
                    (b.Customer != null && b.Customer.Name.ToLower().Contains(s)) ||
                    (b.Notes != null && b.Notes.ToLower().Contains(s)) ||
                    b.Id.ToString().Contains(s) ||
                    (b.PickupLocation != null && b.PickupLocation.ToLower().Contains(s)) ||
                    (b.DropLocation != null && b.DropLocation.ToLower().Contains(s)));
            }

            var totalCount = await query.CountAsync();

            var bookings = await query
                .OrderByDescending(b => b.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = new List<BookingDto>();
            foreach (var b in bookings)
            {
                var vehicles = new List<BookingVehicleDto>();
                foreach (var bv in b.BookingVehicles)
                {
                    string? carName = null;
                    if (bv.CarId.HasValue)
                    {
                        var car = await _context.Cars.FindAsync(bv.CarId.Value);
                        carName = car != null ? $"{car.Make} {car.Model}" : null;
                    }

                    string? driverName = null;
                    if (bv.DriverId.HasValue)
                    {
                        var drv = await _context.Drivers.FindAsync(bv.DriverId.Value);
                        driverName = drv != null ? $"{drv.FirstName} {drv.LastName}" : null;
                    }

                    string? partnerVehicleName = null;
                    if (bv.PartnerVehicleId.HasValue)
                    {
                        var pv = await _context.PartnerVehicles.FindAsync(bv.PartnerVehicleId.Value);
                        partnerVehicleName = pv != null ? $"{pv.Make} {pv.Model}" : null;
                    }

                    string? partnerDriverName = null;
                    if (bv.PartnerDriverId.HasValue)
                    {
                        var pd = await _context.PartnerDrivers.FindAsync(bv.PartnerDriverId.Value);
                        partnerDriverName = pd?.Name;
                    }

                    vehicles.Add(new BookingVehicleDto
                    {
                        Id = bv.Id,
                        BookingId = bv.BookingId,
                        CarId = bv.CarId,
                        CarName = carName,
                        DriverId = bv.DriverId,
                        DriverName = driverName,
                        PartnerVehicleId = bv.PartnerVehicleId,
                        PartnerVehicleName = partnerVehicleName,
                        PartnerDriverId = bv.PartnerDriverId,
                        PartnerDriverName = partnerDriverName,
                        AssignmentType = bv.AssignmentType,
                        Status = bv.Status,
                        ActualStart = bv.ActualStart,
                        ActualEnd = bv.ActualEnd,
                        Quantity = bv.Quantity,
                        RateType = bv.RateType,
                        BaseRate = bv.BaseRate,
                        Distance = bv.Distance,
                        Hours = bv.Hours,
                        Fare = bv.Fare,
                        MagicToken = bv.MagicToken
                    });
                }

                dtos.Add(new BookingDto
                {
                    Id = b.Id,
                    CustomerId = b.CustomerId,
                    CustomerName = b.Customer?.Name ?? "Unknown",
                    DutyTypeId = b.DutyTypeId,
                    DutyTypeName = b.DutyType?.Name,
                    ActualDistance = b.ActualDistance,
                    ActualHours = b.ActualHours,
                    ExtraKmCharge = b.ExtraKmCharge,
                    ExtraHourCharge = b.ExtraHourCharge,
                    BookingDate = b.BookingDate,
                    ScheduledStart = b.ScheduledStart,
                    ScheduledEnd = b.ScheduledEnd,
                    Status = b.Status,
                    TotalAmount = b.TotalAmount,
                    Notes = b.Notes,
                    PickupLocation = b.PickupLocation,
                    DropLocation = b.DropLocation,
                    BookingVehicles = vehicles
                });
            }

            return new PagedBookingResponseDto
            {
                Data = dtos,
                TotalCount = totalCount
            };
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int id, int tenantId)
        {
            var b = await _context.Bookings
                .Where(x => x.Id == id && x.TenantId == tenantId)
                .Include(x => x.Customer)
                .Include(x => x.DutyType)
                .Include(x => x.BookingVehicles)
                .FirstOrDefaultAsync();

            if (b == null) return null;

            var vehicles = new List<BookingVehicleDto>();
            foreach (var bv in b.BookingVehicles)
            {
                string? carName = null;
                if (bv.CarId.HasValue)
                {
                    var car = await _context.Cars.FindAsync(bv.CarId.Value);
                    carName = car != null ? $"{car.Make} {car.Model}" : null;
                }

                string? driverName = null;
                if (bv.DriverId.HasValue)
                {
                    var drv = await _context.Drivers.FindAsync(bv.DriverId.Value);
                    driverName = drv != null ? $"{drv.FirstName} {drv.LastName}" : null;
                }

                string? partnerVehicleName = null;
                if (bv.PartnerVehicleId.HasValue)
                {
                    var pv = await _context.PartnerVehicles.FindAsync(bv.PartnerVehicleId.Value);
                    partnerVehicleName = pv != null ? $"{pv.Make} {pv.Model}" : null;
                }

                string? partnerDriverName = null;
                if (bv.PartnerDriverId.HasValue)
                {
                    var pd = await _context.PartnerDrivers.FindAsync(bv.PartnerDriverId.Value);
                    partnerDriverName = pd?.Name;
                }

                vehicles.Add(new BookingVehicleDto
                {
                    Id = bv.Id,
                    BookingId = bv.BookingId,
                    CarId = bv.CarId,
                    CarName = carName,
                    DriverId = bv.DriverId,
                    DriverName = driverName,
                    PartnerVehicleId = bv.PartnerVehicleId,
                    PartnerVehicleName = partnerVehicleName,
                    PartnerDriverId = bv.PartnerDriverId,
                    PartnerDriverName = partnerDriverName,
                    AssignmentType = bv.AssignmentType,
                    Status = bv.Status,
                    ActualStart = bv.ActualStart,
                    ActualEnd = bv.ActualEnd,
                    Quantity = bv.Quantity,
                    RateType = bv.RateType,
                    BaseRate = bv.BaseRate,
                    Distance = bv.Distance,
                    Hours = bv.Hours,
                    Fare = bv.Fare,
                    MagicToken = bv.MagicToken
                });
            }

            return new BookingDto
            {
                Id = b.Id,
                CustomerId = b.CustomerId,
                CustomerName = b.Customer?.Name ?? "Unknown",
                DutyTypeId = b.DutyTypeId,
                DutyTypeName = b.DutyType?.Name,
                ActualDistance = b.ActualDistance,
                ActualHours = b.ActualHours,
                ExtraKmCharge = b.ExtraKmCharge,
                ExtraHourCharge = b.ExtraHourCharge,
                BookingDate = b.BookingDate,
                ScheduledStart = b.ScheduledStart,
                ScheduledEnd = b.ScheduledEnd,
                Status = b.Status,
                TotalAmount = b.TotalAmount,
                Notes = b.Notes,
                PickupLocation = b.PickupLocation,
                DropLocation = b.DropLocation,
                BookingVehicles = vehicles
            };
        }

        public async Task<BookingDto?> CreateBookingAsync(int tenantId, CreateBookingDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = new Booking
                {
                    TenantId = tenantId,
                    CustomerId = dto.CustomerId,
                    DutyTypeId = dto.DutyTypeId,
                    ScheduledStart = dto.ScheduledStart.ToUniversalTime(),
                    ScheduledEnd = dto.ScheduledEnd.ToUniversalTime(),
                    TotalAmount = dto.TotalAmount,
                    Notes = dto.Notes,
                    PickupLocation = dto.PickupLocation,
                    DropLocation = dto.DropLocation,
                    Status = "Assigned"
                };

                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();

                var vehiclesList = new List<BookingVehicleDto>();

                foreach (var v in dto.Vehicles)
                {
                    var bv = new BookingVehicle
                    {
                        TenantId = tenantId,
                        BookingId = booking.Id,
                        AssignmentType = v.AssignmentType,
                        Status = "Assigned",
                        Quantity = v.Quantity,
                        RateType = v.RateType,
                        BaseRate = v.BaseRate,
                        Distance = v.Distance,
                        Hours = v.Hours,
                        Fare = v.Fare
                    };

                    if (v.AssignmentType == "Own")
                    {
                        bv.CarId = v.CarId;
                        bv.DriverId = v.DriverId;

                        // Mark own car & driver as OnRide
                        if (v.CarId.HasValue)
                        {
                            var car = await _context.Cars.FindAsync(v.CarId.Value);
                            if (car != null) car.Status = "OnRide";
                        }
                        if (v.DriverId.HasValue)
                        {
                            var driver = await _context.Drivers.FindAsync(v.DriverId.Value);
                            if (driver != null) driver.Status = "Active";
                        }
                    }
                    else if (v.AssignmentType == "Vendor")
                    {
                        bv.PartnerVehicleId = v.PartnerVehicleId;
                        bv.PartnerDriverId = v.PartnerDriverId;

                        // Pay vendor/partner model logic: update partner balance
                        if (v.PartnerId.HasValue)
                        {
                            var partner = await _context.Partners.FindAsync(v.PartnerId.Value);
                            if (partner != null)
                            {
                                partner.Balance += dto.TotalAmount / dto.Vehicles.Count; // Simple cost splitting
                            }
                        }
                    }
                    else if (v.AssignmentType == "Marketplace")
                    {
                        // v.CarId contains the MarketplaceAssignment.Id from frontend
                        if (v.CarId.HasValue)
                        {
                            var assignment = await _context.MarketplaceAssignments.FindAsync(v.CarId.Value);
                            if (assignment != null)
                            {
                                // Mark it as completed immediately so it disappears from the dropdown
                                assignment.Status = "Completed";
                                
                                // Optional: map actual car id if needed, but to avoid FK issues with cross-tenant, we leave as null
                                // bv.CarId = assignment.CarId; 
                            }
                        }

                        bv.CarId = null; // Do NOT set this to MarketplaceAssignment.Id to avoid FK violation
                        bv.DriverId = null;
                    }

                    await _context.BookingVehicles.AddAsync(bv);
                    await _context.SaveChangesAsync();
                }

                // Generate Invoice
                var invoice = new Invoice
                {
                    TenantId = tenantId,
                    BookingId = booking.Id,
                    InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{booking.Id}",
                    IssueDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(30),
                    TotalAmount = dto.TotalAmount,
                    TaxAmount = dto.TotalAmount * 0.15m, // 15% VAT
                    PaidAmount = 0,
                    Status = "Unpaid"
                };
                await _context.Invoices.AddAsync(invoice);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return await GetBookingByIdAsync(booking.Id, tenantId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> StartBookingAsync(int id, int tenantId)
        {
            var b = await _context.Bookings
                .Include(x => x.BookingVehicles)
                .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);

            if (b == null) return false;

            b.Status = "InProgress";
            foreach (var bv in b.BookingVehicles)
            {
                bv.Status = "InProgress";
                bv.ActualStart = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteBookingAsync(int id, int tenantId, CompleteBookingDto dto = null)
        {
            var b = await _context.Bookings
                .Include(x => x.DutyType)
                .Include(x => x.BookingVehicles)
                .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);

            if (b == null) return false;

            b.Status = "Completed";
            
            decimal extraCharge = 0;
            decimal kmCharge = 0;
            decimal hrCharge = 0;

            if (dto != null)
            {
                b.ActualDistance = dto.ActualDistance;
                b.ActualHours = dto.ActualHours;

                if (b.DutyType != null)
                {
                    if (dto.ActualDistance.HasValue && dto.ActualDistance.Value > b.DutyType.MaxKilometers)
                    {
                        kmCharge = (dto.ActualDistance.Value - b.DutyType.MaxKilometers) * b.DutyType.ExtraKmRate;
                    }
                    if (dto.ActualHours.HasValue && dto.ActualHours.Value > b.DutyType.MaxHours)
                    {
                        hrCharge = (dto.ActualHours.Value - b.DutyType.MaxHours) * b.DutyType.ExtraHourRate;
                    }
                }
            }

            b.ExtraKmCharge = kmCharge;
            b.ExtraHourCharge = hrCharge;
            extraCharge = kmCharge + hrCharge;

            foreach (var bv in b.BookingVehicles)
            {
                bv.Status = "Completed";
                bv.ActualEnd = DateTime.UtcNow;

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
            }

            if (extraCharge > 0)
            {
                var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.BookingId == b.Id);
                if (invoice != null)
                {
                    invoice.TotalAmount += extraCharge;
                    invoice.TaxAmount = invoice.TotalAmount * 0.15m; // Update tax
                }
                b.TotalAmount += extraCharge;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelBookingAsync(int id, int tenantId)
        {
            var b = await _context.Bookings
                .Include(x => x.BookingVehicles)
                .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);

            if (b == null) return false;

            b.Status = "Cancelled";
            foreach (var bv in b.BookingVehicles)
            {
                bv.Status = "Cancelled";

                // Revert own car & driver back to Available
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

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
