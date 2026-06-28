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
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerDto>> GetCustomersAsync(int tenantId)
        {
            return await _context.Customers
                .Where(c => c.TenantId == tenantId && !c.IsDeleted)
                .Select(c => new CustomerDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    Phone = c.Phone,
                    Address = c.Address,
                    IsActive = c.IsActive
                })
                .ToListAsync();
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(int id, int tenantId)
        {
            var c = await _context.Customers.FindAsync(id);
            if (c == null || c.TenantId != tenantId || c.IsDeleted) return null;

            return new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                IsActive = c.IsActive
            };
        }

        public async Task<CustomerDto?> CreateCustomerAsync(int tenantId, CreateCustomerDto dto)
        {
            var c = new Customer
            {
                TenantId = tenantId,
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                IsActive = true
            };

            await _context.Customers.AddAsync(c);
            await _context.SaveChangesAsync();

            return new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                IsActive = c.IsActive
            };
        }

        public async Task<bool> UpdateCustomerAsync(int id, int tenantId, CreateCustomerDto dto)
        {
            var c = await _context.Customers.FindAsync(id);
            if (c == null || c.TenantId != tenantId || c.IsDeleted) return false;

            c.Name = dto.Name;
            c.Email = dto.Email;
            c.Phone = dto.Phone;
            c.Address = dto.Address;

            _context.Customers.Update(c);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCustomerAsync(int id, int tenantId)
        {
            var c = await _context.Customers.FindAsync(id);
            if (c == null || c.TenantId != tenantId || c.IsDeleted) return false;

            c.IsDeleted = true;
            c.DeletedAt = DateTime.UtcNow;
            _context.Customers.Update(c);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
