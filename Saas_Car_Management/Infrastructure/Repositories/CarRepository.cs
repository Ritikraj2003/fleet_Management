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
    public class CarRepository : ICarRepository
    {
        private readonly ApplicationDbContext _context;

        public CarRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CarDto>> GetCarsAsync(int tenantId)
        {
            return await _context.Cars
                .Where(c => c.TenantId == tenantId)
                .Select(c => new CarDto
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    Year = c.Year,
                    PlateNumber = c.PlateNumber,
                    Color = c.Color,
                    Status = c.Status,
                    ImagePath = c.ImagePath,
                    InsuranceExpiry = c.InsuranceExpiry,
                    NextMaintenanceDate = c.NextMaintenanceDate,
                    IsOwnVehicle = c.IsOwnVehicle,
                    BaseRate = c.BaseRate
                })
                .ToListAsync();
        }

        public async Task<CarDto?> GetCarByIdAsync(int id, int tenantId)
        {
            var c = await _context.Cars.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);
            if (c == null) return null;

            return new CarDto
            {
                Id = c.Id,
                Make = c.Make,
                Model = c.Model,
                Year = c.Year,
                PlateNumber = c.PlateNumber,
                Color = c.Color,
                Status = c.Status,
                ImagePath = c.ImagePath,
                InsuranceExpiry = c.InsuranceExpiry,
                NextMaintenanceDate = c.NextMaintenanceDate,
                IsOwnVehicle = c.IsOwnVehicle,
                BaseRate = c.BaseRate
            };
        }

        public async Task<CarDto?> CreateCarAsync(int tenantId, CreateCarDto dto)
        {
            // Plan check
            var tenant = await _context.Tenants.Include(t => t.Plan).FirstOrDefaultAsync(t => t.Id == tenantId);
            if (tenant != null)
            {
                var currentCarCount = await _context.Cars.CountAsync(c => c.TenantId == tenantId && c.IsOwnVehicle);
                if (currentCarCount >= tenant.Plan.MaxCars)
                {
                    throw new InvalidOperationException("Active fleet vehicle limit reached for your subscription plan.");
                }
            }

            var c = new Car
            {
                TenantId = tenantId,
                Make = dto.Make,
                Model = dto.Model,
                Year = dto.Year,
                PlateNumber = dto.PlateNumber,
                Color = dto.Color,
                Status = dto.Status,
                ImagePath = dto.ImagePath,
                InsuranceExpiry = dto.InsuranceExpiry?.ToUniversalTime(),
                NextMaintenanceDate = dto.NextMaintenanceDate?.ToUniversalTime(),
                IsOwnVehicle = dto.IsOwnVehicle,
                BaseRate = dto.BaseRate
            };

            await _context.Cars.AddAsync(c);
            await _context.SaveChangesAsync();

            return new CarDto
            {
                Id = c.Id,
                Make = c.Make,
                Model = c.Model,
                Year = c.Year,
                PlateNumber = c.PlateNumber,
                Color = c.Color,
                Status = c.Status,
                ImagePath = c.ImagePath,
                InsuranceExpiry = c.InsuranceExpiry,
                NextMaintenanceDate = c.NextMaintenanceDate,
                IsOwnVehicle = c.IsOwnVehicle,
                BaseRate = c.BaseRate
            };
        }

        public async Task<bool> UpdateCarAsync(int id, int tenantId, CreateCarDto dto)
        {
            var c = await _context.Cars.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);
            if (c == null) return false;

            c.Make = dto.Make;
            c.Model = dto.Model;
            c.Year = dto.Year;
            c.PlateNumber = dto.PlateNumber;
            c.Color = dto.Color;
            c.Status = dto.Status;
            c.ImagePath = dto.ImagePath;
            c.InsuranceExpiry = dto.InsuranceExpiry?.ToUniversalTime();
            c.NextMaintenanceDate = dto.NextMaintenanceDate?.ToUniversalTime();
            c.IsOwnVehicle = dto.IsOwnVehicle;
            c.BaseRate = dto.BaseRate;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCarAsync(int id, int tenantId)
        {
            var c = await _context.Cars.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);
            if (c == null) return false;

            _context.Cars.Remove(c);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
