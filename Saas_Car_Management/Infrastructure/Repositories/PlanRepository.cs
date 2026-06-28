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
    public class PlanRepository : IPlanRepository
    {
        private readonly ApplicationDbContext _context;

        public PlanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PlanDto>> GetPlansAsync()
        {
            var plans = await _context.Plans.ToListAsync();
            return plans.Select(p => new PlanDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                MaxCars = p.MaxCars,
                MaxUsers = p.MaxUsers,
                EnabledModules = p.EnabledModules.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                IsActive = p.IsActive
            });
        }

        public async Task<PlanDto?> GetPlanByIdAsync(int id)
        {
            var p = await _context.Plans.FindAsync(id);
            if (p == null) return null;

            return new PlanDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                MaxCars = p.MaxCars,
                MaxUsers = p.MaxUsers,
                EnabledModules = p.EnabledModules.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                IsActive = p.IsActive
            };
        }

        public async Task<PlanDto?> CreatePlanAsync(CreatePlanDto dto)
        {
            var plan = new Plan
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                MaxCars = dto.MaxCars,
                MaxUsers = dto.MaxUsers,
                EnabledModules = string.Join(",", dto.EnabledModules),
                IsActive = true
            };

            await _context.Plans.AddAsync(plan);
            await _context.SaveChangesAsync();

            return new PlanDto
            {
                Id = plan.Id,
                Name = plan.Name,
                Description = plan.Description,
                Price = plan.Price,
                MaxCars = plan.MaxCars,
                MaxUsers = plan.MaxUsers,
                EnabledModules = dto.EnabledModules,
                IsActive = plan.IsActive
            };
        }

        public async Task<bool> UpdatePlanAsync(int id, CreatePlanDto dto)
        {
            var plan = await _context.Plans.FindAsync(id);
            if (plan == null) return false;

            plan.Name = dto.Name;
            plan.Description = dto.Description;
            plan.Price = dto.Price;
            plan.MaxCars = dto.MaxCars;
            plan.MaxUsers = dto.MaxUsers;
            plan.EnabledModules = string.Join(",", dto.EnabledModules);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePlanAsync(int id)
        {
            var plan = await _context.Plans.FindAsync(id);
            if (plan == null) return false;

            plan.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
