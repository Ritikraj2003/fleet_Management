using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saas_Car_Management.Core.DTOs;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Controllers
{
    [Authorize]
    public class CarsController : BaseApiController
    {
        private readonly ICarRepository _carRepository;

        public CarsController(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetCars()
        {
            var cars = await _carRepository.GetCarsAsync(GetTenantId());
            return Ok(cars);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCar(int id)
        {
            var car = await _carRepository.GetCarByIdAsync(id, GetTenantId());
            if (car == null) return NotFound();
            return Ok(car);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCar([FromBody] CreateCarDto dto)
        {
            var result = await _carRepository.CreateCarAsync(GetTenantId(), dto);
            if (result == null) return BadRequest("Could not register vehicle.");
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar(int id, [FromBody] CreateCarDto dto)
        {
            var success = await _carRepository.UpdateCarAsync(id, GetTenantId(), dto);
            if (!success) return NotFound();
            return Ok(new { message = "Vehicle information updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var success = await _carRepository.DeleteCarAsync(id, GetTenantId());
            if (!success) return NotFound();
            return Ok(new { message = "Vehicle removed from fleet." });
        }
    }
}
