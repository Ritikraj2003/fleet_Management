using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saas_Car_Management.Core.DTOs;

namespace Saas_Car_Management.Core.Interfaces
{
    public interface ITenantRepository
    {
        Task<IEnumerable<TenantDto>> GetTenantsAsync();
        Task<TenantDto?> CreateTenantAsync(CreateTenantDto dto);
        Task<bool> UpdateSubscriptionAsync(int tenantId, int planId);
        Task<bool> DeleteTenantAsync(int id);
    }
}
