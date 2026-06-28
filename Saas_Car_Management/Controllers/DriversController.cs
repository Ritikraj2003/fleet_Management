using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saas_Car_Management.Core.DTOs;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Controllers
{
    [Authorize]
    public class DriversController : BaseApiController
    {
        private readonly IDriverRepository _driverRepository;

        public DriversController(IDriverRepository driverRepository)
        {
            _driverRepository = driverRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetDrivers()
        {
            var drivers = await _driverRepository.GetDriversAsync(GetTenantId());
            return Ok(drivers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDriver(int id)
        {
            var driver = await _driverRepository.GetDriverByIdAsync(id, GetTenantId());
            if (driver == null) return NotFound();
            return Ok(driver);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDriver([FromBody] CreateDriverDto dto)
        {
            var result = await _driverRepository.CreateDriverAsync(GetTenantId(), dto);
            if (result == null) return BadRequest("Could not add driver record.");
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDriver(int id, [FromBody] CreateDriverDto dto)
        {
            var success = await _driverRepository.UpdateDriverAsync(id, GetTenantId(), dto);
            if (!success) return NotFound();
            return Ok(new { message = "Driver file updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            var success = await _driverRepository.DeleteDriverAsync(id, GetTenantId());
            if (!success) return NotFound();
            return Ok(new { message = "Driver deleted." });
        }

        [HttpPost("assign-vehicle")]
        public async Task<IActionResult> AssignVehicle([FromBody] DriverVehicleAssignmentDto dto)
        {
            var success = await _driverRepository.AssignVehicleAsync(GetTenantId(), dto.DriverId, dto.CarId);
            if (!success) return BadRequest("Assignment failed. Check if IDs are correct.");
            return Ok(new { message = "Vehicle successfully assigned to driver." });
        }
    }

    public class DriverVehicleAssignmentDto
    {
        public int DriverId { get; set; }
        public int CarId { get; set; }
    }
}
