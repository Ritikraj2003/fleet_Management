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
    public class DriverRepository : IDriverRepository
    {
        private readonly ApplicationDbContext _context;

        public DriverRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DriverDto>> GetDriversAsync(int tenantId)
        {
            return await _context.Drivers
                .Where(d => d.TenantId == tenantId)
                .Select(d => new DriverDto
                {
                    Id = d.Id,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Email = d.Email,
                    Phone = d.Phone,
                    LicenseNumber = d.LicenseNumber,
                    LicenseExpiry = d.LicenseExpiry,
                    Status = d.Status,
                    PhotoPath = d.PhotoPath,
                    UserId = d.UserId
                })
                .ToListAsync();
        }

        public async Task<DriverDto?> GetDriverByIdAsync(int id, int tenantId)
        {
            var d = await _context.Drivers.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);
            if (d == null) return null;

            return new DriverDto
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Email = d.Email,
                Phone = d.Phone,
                LicenseNumber = d.LicenseNumber,
                LicenseExpiry = d.LicenseExpiry,
                Status = d.Status,
                PhotoPath = d.PhotoPath,
                UserId = d.UserId
            };
        }

        public async Task<DriverDto?> CreateDriverAsync(int tenantId, CreateDriverDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int? driverUserId = null;

                if (dto.CreateLogin)
                {
                    // Create User Login Credentials for Driver
                    var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.TenantId == tenantId && r.Code == "DRIVER");
                    if (defaultRole == null)
                    {
                        // If "DRIVER" role doesn't exist, create it
                        defaultRole = new Role
                        {
                            TenantId = tenantId,
                            Name = "Driver Portal Access",
                            Code = "DRIVER",
                            Description = "Driver app functions only.",
                            IsSystemRole = true
                        };
                        await _context.Roles.AddAsync(defaultRole);
                        await _context.SaveChangesAsync();
                    }

                    var user = new User
                    {
                        TenantId = tenantId,
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        Email = dto.Email,
                        PasswordHash = "Driver@123", // Default temp password
                        IsActive = true
                    };
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    await _context.UserRoles.AddAsync(new UserRole
                    {
                        TenantId = tenantId,
                        UserId = user.Id,
                        RoleId = defaultRole.Id
                    });
                    await _context.SaveChangesAsync();

                    driverUserId = user.Id;
                }

                var d = new Driver
                {
                    TenantId = tenantId,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    LicenseNumber = dto.LicenseNumber,
                    LicenseExpiry = dto.LicenseExpiry.ToUniversalTime(),
                    Status = dto.Status,
                    PhotoPath = dto.PhotoPath,
                    UserId = driverUserId
                };

                await _context.Drivers.AddAsync(d);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new DriverDto
                {
                    Id = d.Id,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Email = d.Email,
                    Phone = d.Phone,
                    LicenseNumber = d.LicenseNumber,
                    LicenseExpiry = d.LicenseExpiry,
                    Status = d.Status,
                    PhotoPath = d.PhotoPath,
                    UserId = d.UserId
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateDriverAsync(int id, int tenantId, CreateDriverDto dto)
        {
            var d = await _context.Drivers.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);
            if (d == null) return false;

            d.FirstName = dto.FirstName;
            d.LastName = dto.LastName;
            d.Email = dto.Email;
            d.Phone = dto.Phone;
            d.LicenseNumber = dto.LicenseNumber;
            d.LicenseExpiry = dto.LicenseExpiry.ToUniversalTime();
            d.Status = dto.Status;
            d.PhotoPath = dto.PhotoPath;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDriverAsync(int id, int tenantId)
        {
            var d = await _context.Drivers.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);
            if (d == null) return false;

            _context.Drivers.Remove(d);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignVehicleAsync(int tenantId, int driverId, int carId)
        {
            var driver = await _context.Drivers.FirstOrDefaultAsync(x => x.Id == driverId && x.TenantId == tenantId);
            var car = await _context.Cars.FirstOrDefaultAsync(x => x.Id == carId && x.TenantId == tenantId);

            if (driver == null || car == null) return false;

            // Release driver's existing active assignments
            var existingAssignments = await _context.DriverVehicleAssignments
                .Where(a => a.DriverId == driverId && a.ReleasedAt == null)
                .ToListAsync();

            foreach (var ea in existingAssignments)
            {
                ea.ReleasedAt = DateTime.UtcNow;
            }

            // Create new assignment
            var ass = new DriverVehicleAssignment
            {
                TenantId = tenantId,
                DriverId = driverId,
                CarId = carId,
                AssignedAt = DateTime.UtcNow
            };

            await _context.DriverVehicleAssignments.AddAsync(ass);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
