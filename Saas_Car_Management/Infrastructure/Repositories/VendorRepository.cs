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
    public class VendorRepository : IVendorRepository
    {
        private readonly ApplicationDbContext _context;

        public VendorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PartnerDto>> GetVendorsAsync(int tenantId)
        {
            var vendors = await _context.Partners
                .Where(p => p.TenantId == tenantId && p.Type == "Vendor")
                .ToListAsync();

            bool changed = false;
            foreach (var v in vendors)
            {
                if (string.IsNullOrEmpty(v.MagicToken))
                {
                    v.MagicToken = Guid.NewGuid().ToString();
                    changed = true;
                }
            }
            if (changed) await _context.SaveChangesAsync();

            return vendors.Select(p => new PartnerDto
            {
                Id = p.Id,
                Name = p.Name,
                ContactName = p.ContactName,
                Email = p.Email,
                Phone = p.Phone,
                Address = p.Address,
                Type = p.Type,
                Balance = p.Balance,
                IsActive = p.IsActive,
                MagicToken = p.MagicToken
            }).ToList();
        }

        public async Task<PartnerDto?> GetVendorByIdAsync(int id, int tenantId)
        {
            var p = await _context.Partners.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && x.Type == "Vendor");
            if (p == null) return null;

            return new PartnerDto
            {
                Id = p.Id,
                Name = p.Name,
                ContactName = p.ContactName,
                Email = p.Email,
                Phone = p.Phone,
                Address = p.Address,
                Type = p.Type,
                Balance = p.Balance,
                IsActive = p.IsActive,
                MagicToken = p.MagicToken
            };
        }

        public async Task<PartnerDto?> CreateVendorAsync(int tenantId, CreatePartnerDto dto)
        {
            var p = new Partner
            {
                TenantId = tenantId,
                Name = dto.Name,
                ContactName = dto.ContactName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                Type = "Vendor",
                Balance = 0,
                IsActive = true,
                MagicToken = Guid.NewGuid().ToString()
            };

            await _context.Partners.AddAsync(p);
            await _context.SaveChangesAsync();

            return new PartnerDto
            {
                Id = p.Id,
                Name = p.Name,
                ContactName = p.ContactName,
                Email = p.Email,
                Phone = p.Phone,
                Address = p.Address,
                Type = p.Type,
                Balance = p.Balance,
                IsActive = p.IsActive,
                MagicToken = p.MagicToken
            };
        }

        public async Task<bool> UpdateVendorAsync(int id, int tenantId, CreatePartnerDto dto)
        {
            var p = await _context.Partners.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && x.Type == "Vendor");
            if (p == null) return false;

            p.Name = dto.Name;
            p.ContactName = dto.ContactName;
            p.Email = dto.Email;
            p.Phone = dto.Phone;
            p.Address = dto.Address;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVendorAsync(int id, int tenantId)
        {
            var p = await _context.Partners.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && x.Type == "Vendor");
            if (p == null) return false;

            p.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PartnerVehicleDto>> GetVendorVehiclesAsync(int partnerId, int tenantId)
        {
            return await _context.PartnerVehicles
                .Where(pv => pv.PartnerId == partnerId && pv.TenantId == tenantId)
                .Select(pv => new PartnerVehicleDto
                {
                    Id = pv.Id,
                    PartnerId = pv.PartnerId,
                    Make = pv.Make,
                    Model = pv.Model,
                    Year = pv.Year,
                    PlateNumber = pv.PlateNumber,
                    Color = pv.Color,
                    Status = pv.Status,
                    IsActive = pv.IsActive
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PartnerVehicleDto>> GetAllVendorVehiclesAsync(int tenantId)
        {
            return await _context.PartnerVehicles
                .Where(pv => pv.TenantId == tenantId && pv.IsActive)
                .Include(pv => pv.Partner)
                .Select(pv => new PartnerVehicleDto
                {
                    Id = pv.Id,
                    PartnerId = pv.PartnerId,
                    Make = pv.Make,
                    Model = pv.Model,
                    Year = pv.Year,
                    PlateNumber = pv.PlateNumber,
                    Color = pv.Color,
                    Status = pv.Status,
                    IsActive = pv.IsActive,
                    PartnerName = pv.Partner.Name // Assume we can pass this or just need the basics
                })
                .ToListAsync();
        }

        public async Task<PartnerVehicleDto?> CreateVendorVehicleAsync(int tenantId, CreatePartnerVehicleDto dto)
        {
            var pv = new PartnerVehicle
            {
                TenantId = tenantId,
                PartnerId = dto.PartnerId,
                Make = dto.Make,
                Model = dto.Model,
                Year = dto.Year,
                PlateNumber = dto.PlateNumber,
                Color = dto.Color,
                Status = "Available",
                IsActive = true
            };

            await _context.PartnerVehicles.AddAsync(pv);
            await _context.SaveChangesAsync();

            return new PartnerVehicleDto
            {
                Id = pv.Id,
                PartnerId = pv.PartnerId,
                Make = pv.Make,
                Model = pv.Model,
                Year = pv.Year,
                PlateNumber = pv.PlateNumber,
                Color = pv.Color,
                Status = pv.Status,
                IsActive = pv.IsActive
            };
        }

        public async Task<IEnumerable<PartnerDriverDto>> GetVendorDriversAsync(int partnerId, int tenantId)
        {
            return await _context.PartnerDrivers
                .Where(pd => pd.PartnerId == partnerId && pd.TenantId == tenantId)
                .Select(pd => new PartnerDriverDto
                {
                    Id = pd.Id,
                    PartnerId = pd.PartnerId,
                    Name = pd.Name,
                    Phone = pd.Phone,
                    LicenseNumber = pd.LicenseNumber,
                    IsActive = pd.IsActive
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PartnerDriverDto>> GetAllVendorDriversAsync(int tenantId)
        {
            return await _context.PartnerDrivers
                .Where(pd => pd.TenantId == tenantId && pd.IsActive)
                .Include(pd => pd.Partner)
                .Select(pd => new PartnerDriverDto
                {
                    Id = pd.Id,
                    PartnerId = pd.PartnerId,
                    Name = pd.Name,
                    Phone = pd.Phone,
                    LicenseNumber = pd.LicenseNumber,
                    IsActive = pd.IsActive,
                    PartnerName = pd.Partner.Name
                })
                .ToListAsync();
        }

        public async Task<CreatePartnerDriverDto?> CreateVendorDriverAsync(int tenantId, CreatePartnerDriverDto dto)
        {
            var pd = new PartnerDriver
            {
                TenantId = tenantId,
                PartnerId = dto.PartnerId,
                Name = dto.Name,
                Phone = dto.Phone,
                LicenseNumber = dto.LicenseNumber,
                IsActive = true
            };

            await _context.PartnerDrivers.AddAsync(pd);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<PagedVendorPaymentResponseDto> GetVendorPaymentsAsync(int tenantId, int page = 1, int pageSize = 7)
        {
            var query = _context.VendorPayments
                .Where(vp => vp.TenantId == tenantId)
                .Include(vp => vp.Partner)
                .OrderByDescending(vp => vp.PaymentDate);

            var totalCount = await query.CountAsync();

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(vp => new VendorPaymentDto
                {
                    Id = vp.Id,
                    PartnerId = vp.PartnerId,
                    PartnerName = vp.Partner.Name,
                    Amount = vp.Amount,
                    PaymentDate = vp.PaymentDate,
                    ReferenceNumber = vp.ReferenceNumber,
                    Notes = vp.Notes
                })
                .ToListAsync();

            return new PagedVendorPaymentResponseDto
            {
                Data = data,
                TotalCount = totalCount
            };
        }

        public async Task<VendorPaymentDto?> RecordPaymentAsync(int tenantId, CreateVendorPaymentDto dto)
        {
            var partner = await _context.Partners.FirstOrDefaultAsync(p => p.Id == dto.PartnerId && p.TenantId == tenantId);
            if (partner == null) return null;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var vp = new VendorPayment
                {
                    TenantId = tenantId,
                    PartnerId = dto.PartnerId,
                    Amount = dto.Amount,
                    PaymentDate = dto.PaymentDate.ToUniversalTime(),
                    ReferenceNumber = dto.ReferenceNumber ?? "",
                    Notes = dto.Notes ?? ""
                };

                // Deduct from Vendor balance since we paid them
                partner.Balance -= dto.Amount;

                await _context.VendorPayments.AddAsync(vp);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new VendorPaymentDto
                {
                    Id = vp.Id,
                    PartnerId = vp.PartnerId,
                    PartnerName = partner.Name,
                    Amount = vp.Amount,
                    PaymentDate = vp.PaymentDate,
                    ReferenceNumber = vp.ReferenceNumber,
                    Notes = vp.Notes
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"An error occurred while saving the entity changes. Inner: {ex.InnerException?.Message}", ex);
            }
        }

        async Task<PartnerDriverDto?> IVendorRepository.CreateVendorDriverAsync(int tenantId, CreatePartnerDriverDto dto)
        {
            var res = await CreateVendorDriverAsync(tenantId, dto);
            if (res == null) return null;

            // Find pd
            var pd = await _context.PartnerDrivers.FirstOrDefaultAsync(x => x.PartnerId == dto.PartnerId && x.LicenseNumber == dto.LicenseNumber);
            if (pd == null) return null;

            return new PartnerDriverDto
            {
                Id = pd.Id,
                PartnerId = pd.PartnerId,
                Name = pd.Name,
                Phone = pd.Phone,
                LicenseNumber = pd.LicenseNumber,
                IsActive = pd.IsActive
            };
        }

        public async Task<PartnerDto?> GetVendorByTokenAsync(string token)
        {
            var p = await _context.Partners
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.MagicToken == token && x.IsActive && x.Type == "Vendor");
            if (p == null) return null;

            return new PartnerDto
            {
                Id = p.Id,
                Name = p.Name,
                ContactName = p.ContactName,
                Email = p.Email,
                Phone = p.Phone,
                Address = p.Address,
                Type = p.Type,
                Balance = p.Balance,
                IsActive = p.IsActive,
                MagicToken = p.MagicToken
            };
        }

        public async Task<IEnumerable<PartnerVehicleDto>> GetVendorVehiclesByTokenAsync(string token)
        {
            var p = await _context.Partners
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.MagicToken == token && x.IsActive && x.Type == "Vendor");
            if (p == null) return new List<PartnerVehicleDto>();

            return await _context.PartnerVehicles
                .IgnoreQueryFilters()
                .Where(pv => pv.PartnerId == p.Id && pv.IsActive)
                .Select(pv => new PartnerVehicleDto
                {
                    Id = pv.Id,
                    PartnerId = pv.PartnerId,
                    Make = pv.Make,
                    Model = pv.Model,
                    Year = pv.Year,
                    PlateNumber = pv.PlateNumber,
                    Color = pv.Color,
                    Status = pv.Status,
                    IsActive = pv.IsActive
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateVehicleStatusAsync(string token, int vehicleId, string status)
        {
            var p = await _context.Partners
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.MagicToken == token && x.IsActive && x.Type == "Vendor");
            if (p == null) return false;

            var v = await _context.PartnerVehicles
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == vehicleId && x.PartnerId == p.Id);
            if (v == null) return false;

            v.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
