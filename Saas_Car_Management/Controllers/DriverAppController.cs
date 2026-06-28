using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DriverAppController : BaseApiController
    {
        private readonly IDriverAppRepository _repository;

        public DriverAppController(IDriverAppRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("home")]
        public async Task<IActionResult> GetHomeData()
        {
            var data = await _repository.GetHomeDataAsync(GetUserId());
            if (data == null) return Unauthorized("Not registered as a Driver.");

            return Ok(data);
        }

        [HttpGet("live")]
        public async Task<IActionResult> GetLiveRides()
        {
            var rides = await _repository.GetLiveRidesAsync(GetUserId());
            return Ok(rides);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistoryRides()
        {
            var history = await _repository.GetHistoryRidesAsync(GetUserId());
            return Ok(history);
        }

        [HttpPost("attendance/punch")]
        public async Task<IActionResult> PunchAttendance()
        {
            var result = await _repository.PunchAttendanceAsync(GetUserId());
            if (result == null) return Unauthorized("Not registered as a Driver.");

            if (result.Message.Contains("Already punched out"))
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }

        [HttpGet("attendance/history")]
        public async Task<IActionResult> GetAttendanceHistory()
        {
            var history = await _repository.GetAttendanceHistoryAsync(GetUserId());
            if (history == null) return Unauthorized("Not registered as a Driver.");

            return Ok(history);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var profile = await _repository.GetProfileAsync(GetUserId());
            if (profile == null) return Unauthorized("Not registered as a Driver.");

            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] Saas_Car_Management.Core.DTOs.UpdateDriverProfileDto dto)
        {
            var success = await _repository.UpdateProfileAsync(GetUserId(), dto);
            if (!success) return Unauthorized("Not registered as a Driver.");

            return Ok(new { Message = "Profile updated successfully." });
        }

        [HttpPost("trip/{bookingVehicleId}/start")]
        public async Task<IActionResult> StartTrip(int bookingVehicleId)
        {
            var success = await _repository.StartTripAsync(GetUserId(), bookingVehicleId);
            if (!success) return BadRequest("Unable to start trip. Invalid assignment or state.");
            return Ok(new { Message = "Trip started." });
        }

        [HttpPost("trip/{bookingVehicleId}/end")]
        public async Task<IActionResult> EndTrip(int bookingVehicleId, [FromBody] Saas_Car_Management.Core.DTOs.EndTripRequestDto request)
        {
            var success = await _repository.EndTripAsync(GetUserId(), bookingVehicleId, request.EndOdo, request.Tolls);
            if (!success) return BadRequest("Unable to end trip. Invalid assignment or state.");
            return Ok(new { Message = "Trip completed." });
        }
    }
}
