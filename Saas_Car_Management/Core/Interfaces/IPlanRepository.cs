using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saas_Car_Management.Core.DTOs;

namespace Saas_Car_Management.Core.Interfaces
{
    public interface IPlanRepository
    {
        Task<IEnumerable<PlanDto>> GetPlansAsync();
        Task<PlanDto?> GetPlanByIdAsync(int id);
        Task<PlanDto?> CreatePlanAsync(CreatePlanDto dto);
        Task<bool> UpdatePlanAsync(int id, CreatePlanDto dto);
        Task<bool> DeletePlanAsync(int id);
    }
}
