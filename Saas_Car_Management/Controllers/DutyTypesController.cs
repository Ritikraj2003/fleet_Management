using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saas_Car_Management.Core.DTOs;
using Saas_Car_Management.Core.Entities;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DutyTypesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDutyTypeRepository _repository;

        public DutyTypesController(IUnitOfWork unitOfWork, IDutyTypeRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _repository.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _repository.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDutyTypeDto dto)
        {
            var entity = new DutyType
            {
                Name = dto.Name,
                Type = dto.Type,
                MaxKilometers = dto.MaxKilometers,
                MaxHours = dto.MaxHours,
                ExtraKmRate = dto.ExtraKmRate,
                ExtraHourRate = dto.ExtraHourRate
            };
            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return Ok(entity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDutyTypeDto dto)
        {
            var entity = await _repository.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null) return NotFound();

            entity.Name = dto.Name;
            entity.Type = dto.Type;
            entity.MaxKilometers = dto.MaxKilometers;
            entity.MaxHours = dto.MaxHours;
            entity.ExtraKmRate = dto.ExtraKmRate;
            entity.ExtraHourRate = dto.ExtraHourRate;

            _repository.Update(entity);
            await _unitOfWork.CompleteAsync();
            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _repository.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null) return NotFound();

            _repository.Remove(entity);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
}
