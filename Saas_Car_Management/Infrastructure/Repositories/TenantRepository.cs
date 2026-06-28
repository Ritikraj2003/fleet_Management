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
    public class TenantRepository : ITenantRepository
    {
        private readonly ApplicationDbContext _context;

        public TenantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TenantDto>> GetTenantsAsync()
        {
            return await _context.Tenants
                .Include(t => t.Plan)
                .Select(t => new TenantDto
                {
                    Id = t.Id,
                    CompanyName = t.CompanyName,
                    Domain = t.Domain,
                    PlanId = t.PlanId,
                    PlanName = t.Plan.Name,
                    Status = t.Status,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<TenantDto?> CreateTenantAsync(CreateTenantDto dto)
        {
            var tenant = new Tenant
            {
                CompanyName = dto.CompanyName,
                Domain = dto.Domain,
                PlanId = dto.PlanId,
                Status = "Active",
                IsActive = true
            };

            await _context.Tenants.AddAsync(tenant);
            await _context.SaveChangesAsync();

            var plan = await _context.Plans.FindAsync(dto.PlanId);

            return new TenantDto
            {
                Id = tenant.Id,
                CompanyName = tenant.CompanyName,
                Domain = tenant.Domain,
                PlanId = tenant.PlanId,
                PlanName = plan?.Name ?? string.Empty,
                Status = tenant.Status,
                IsActive = tenant.IsActive,
                CreatedAt = tenant.CreatedAt
            };
        }

        public async Task<bool> UpdateSubscriptionAsync(int tenantId, int planId)
        {
            var tenant = await _context.Tenants.FindAsync(tenantId);
            if (tenant == null) return false;

            var plan = await _context.Plans.FindAsync(planId);
            if (plan == null) return false;

            tenant.PlanId = planId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTenantAsync(int id)
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null) return false;

            tenant.IsActive = false;
            tenant.Status = "Suspended";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
