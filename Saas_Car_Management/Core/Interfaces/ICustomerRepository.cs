using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saas_Car_Management.Core.DTOs;

namespace Saas_Car_Management.Core.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<CustomerDto>> GetCustomersAsync(int tenantId);
        Task<CustomerDto?> GetCustomerByIdAsync(int id, int tenantId);
        Task<CustomerDto?> CreateCustomerAsync(int tenantId, CreateCustomerDto dto);
        Task<bool> UpdateCustomerAsync(int id, int tenantId, CreateCustomerDto dto);
        Task<bool> DeleteCustomerAsync(int id, int tenantId);
    }
}
