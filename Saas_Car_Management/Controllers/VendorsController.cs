using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saas_Car_Management.Core.DTOs;
using Saas_Car_Management.Core.Interfaces;
using Saas_Car_Management.Infrastructure.Repositories; // For VendorPaymentDto

namespace Saas_Car_Management.Controllers
{
    [Authorize]
    public class VendorsController : BaseApiController
    {
        private readonly IVendorRepository _vendorRepository;

        public VendorsController(IVendorRepository vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetVendors()
        {
            var vendors = await _vendorRepository.GetVendorsAsync(GetTenantId());
            return Ok(vendors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendor(int id)
        {
            var vendor = await _vendorRepository.GetVendorByIdAsync(id, GetTenantId());
            if (vendor == null) return NotFound();
            return Ok(vendor);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVendor([FromBody] CreatePartnerDto dto)
        {
            var result = await _vendorRepository.CreateVendorAsync(GetTenantId(), dto);
            if (result == null) return BadRequest("Could not add affiliate vendor.");
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendor(int id, [FromBody] CreatePartnerDto dto)
        {
            var success = await _vendorRepository.UpdateVendorAsync(id, GetTenantId(), dto);
            if (!success) return NotFound();
            return Ok(new { message = "Vendor files updated." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            var success = await _vendorRepository.DeleteVendorAsync(id, GetTenantId());
            if (!success) return NotFound();
            return Ok(new { message = "Vendor deactivated." });
        }

        [HttpGet("{partnerId}/vehicles")]
        public async Task<IActionResult> GetVendorVehicles(int partnerId)
        {
            var vehicles = await _vendorRepository.GetVendorVehiclesAsync(partnerId, GetTenantId());
            return Ok(vehicles);
        }

        [HttpGet("vehicles/all")]
        public async Task<IActionResult> GetAllVendorVehicles()
        {
            var vehicles = await _vendorRepository.GetAllVendorVehiclesAsync(GetTenantId());
            return Ok(vehicles);
        }

        [HttpPost("vehicles")]
        public async Task<IActionResult> CreateVendorVehicle([FromBody] CreatePartnerVehicleDto dto)
        {
            var result = await _vendorRepository.CreateVendorVehicleAsync(GetTenantId(), dto);
            return Ok(result);
        }

        [HttpGet("{partnerId}/drivers")]
        public async Task<IActionResult> GetVendorDrivers(int partnerId)
        {
            var drivers = await _vendorRepository.GetVendorDriversAsync(partnerId, GetTenantId());
            return Ok(drivers);
        }

        [HttpGet("drivers/all")]
        public async Task<IActionResult> GetAllVendorDrivers()
        {
            var drivers = await _vendorRepository.GetAllVendorDriversAsync(GetTenantId());
            return Ok(drivers);
        }

        [HttpPost("drivers")]
        public async Task<IActionResult> CreateVendorDriver([FromBody] CreatePartnerDriverDto dto)
        {
            var result = await _vendorRepository.CreateVendorDriverAsync(GetTenantId(), dto);
            return Ok(result);
        }

        [HttpGet("payments")]
        public async Task<IActionResult> GetVendorPayments([FromQuery] int page = 1, [FromQuery] int pageSize = 7)
        {
            var result = await _vendorRepository.GetVendorPaymentsAsync(GetTenantId(), page, pageSize);
            return Ok(result);
        }

        [HttpPost("payments")]
        public async Task<IActionResult> RecordPayment([FromBody] CreateVendorPaymentDto dto)
        {
            var result = await _vendorRepository.RecordPaymentAsync(GetTenantId(), dto);
            if (result == null) return BadRequest("Unable to record vendor settlement.");
            return Ok(result);
        }
    }
}
