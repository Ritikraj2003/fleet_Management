using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saas_Car_Management.Core.DTOs;

namespace Saas_Car_Management.Core.Interfaces
{
    public interface ICarRepository
    {
        Task<IEnumerable<CarDto>> GetCarsAsync(int tenantId);
        Task<CarDto?> GetCarByIdAsync(int id, int tenantId);
        Task<CarDto?> CreateCarAsync(int tenantId, CreateCarDto dto);
        Task<bool> UpdateCarAsync(int id, int tenantId, CreateCarDto dto);
        Task<bool> DeleteCarAsync(int id, int tenantId);
    }
}
