using System.Threading.Tasks;
using Saas_Car_Management.Core.DTOs;

namespace Saas_Car_Management.Core.Interfaces
{
    public interface IDriverPortalRepository
    {
        Task<DriverTripDto?> GetTripByTokenAsync(string token);
        Task<bool> StartTripAsync(string token, int startOdometer);
        Task<bool> CompleteTripAsync(string token, int endOdometer);
    }
}
