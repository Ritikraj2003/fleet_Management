using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saas_Car_Management.Core.DTOs;

namespace Saas_Car_Management.Core.Interfaces
{
    public interface IDriverRepository
    {
        Task<IEnumerable<DriverDto>> GetDriversAsync(int tenantId);
        Task<DriverDto?> GetDriverByIdAsync(int id, int tenantId);
        Task<DriverDto?> CreateDriverAsync(int tenantId, CreateDriverDto dto);
        Task<bool> UpdateDriverAsync(int id, int tenantId, CreateDriverDto dto);
        Task<bool> DeleteDriverAsync(int id, int tenantId);
        Task<bool> AssignVehicleAsync(int tenantId, int driverId, int carId);
    }
}
