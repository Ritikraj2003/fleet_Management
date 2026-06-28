using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorPortalController : ControllerBase
    {
        private readonly IVendorRepository _vendorRepository;

        public VendorPortalController(IVendorRepository vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }

        [HttpGet("{token}")]
        public async Task<IActionResult> GetVendorByToken(string token)
        {
            var vendor = await _vendorRepository.GetVendorByTokenAsync(token);
            if (vendor == null) return NotFound();
            
            return Ok(vendor);
        }

        [HttpGet("{token}/vehicles")]
        public async Task<IActionResult> GetVendorVehicles(string token)
        {
            var vehicles = await _vendorRepository.GetVendorVehiclesByTokenAsync(token);
            return Ok(vehicles);
        }

        [HttpPut("{token}/vehicles/{vehicleId}/status")]
        public async Task<IActionResult> UpdateVehicleStatus(string token, int vehicleId, [FromBody] UpdateStatusDto dto)
        {
            var success = await _vendorRepository.UpdateVehicleStatusAsync(token, vehicleId, dto.Status);
            if (!success) return BadRequest("Could not update vehicle status.");
            return Ok(new { message = "Status updated." });
        }
    }

    public class UpdateStatusDto
    {
        public string Status { get; set; } = string.Empty;
    }
}
