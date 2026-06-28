using System.Collections.Generic;
using System.Threading.Tasks;
using Saas_Car_Management.Core.DTOs;

namespace Saas_Car_Management.Core.Interfaces
{
    public interface IDriverAppRepository
    {
        Task<DriverAppHomeDto?> GetHomeDataAsync(int userId);
        Task<IEnumerable<DriverAppLiveRideDto>> GetLiveRidesAsync(int userId);
        Task<IEnumerable<DriverAppHistoryDto>> GetHistoryRidesAsync(int userId);
        Task<DriverPunchResultDto?> PunchAttendanceAsync(int userId);
        Task<DriverProfileDto?> GetProfileAsync(int userId);
        Task<bool> UpdateProfileAsync(int userId, UpdateDriverProfileDto dto);
        Task<bool> StartTripAsync(int userId, int bookingVehicleId);
        Task<bool> EndTripAsync(int userId, int bookingVehicleId, decimal endOdo, decimal tolls);
        Task<DriverAttendanceSummaryDto?> GetAttendanceHistoryAsync(int userId);
    }
}
