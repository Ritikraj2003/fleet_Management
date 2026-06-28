using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriverPortalController : ControllerBase
    {
        private readonly IDriverPortalRepository _repository;

        public DriverPortalController(IDriverPortalRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{token}")]
        public async Task<IActionResult> GetTripDetails(string token)
        {
            var trip = await _repository.GetTripByTokenAsync(token);
            if (trip == null) return NotFound("Invalid or expired magic link.");
            return Ok(trip);
        }

        public class OdometerRequest
        {
            public int Odometer { get; set; }
        }

        [HttpPost("{token}/start")]
        public async Task<IActionResult> StartTrip(string token, [FromBody] OdometerRequest req)
        {
            var success = await _repository.StartTripAsync(token, req.Odometer);
            if (!success) return BadRequest("Could not start trip. Invalid token or booking already started.");
            return Ok(new { message = "Trip started successfully." });
        }

        [HttpPost("{token}/complete")]
        public async Task<IActionResult> CompleteTrip(string token, [FromBody] OdometerRequest req)
        {
            var success = await _repository.CompleteTripAsync(token, req.Odometer);
            if (!success) return BadRequest("Could not complete trip. Invalid token or booking not active.");
            return Ok(new { message = "Trip completed successfully." });
        }
    }
}
