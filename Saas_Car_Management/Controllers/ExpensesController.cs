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
    public class ExpensesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Expense> _repository;

        public ExpensesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.Repository<Expense>();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var expenses = await _repository.GetAllAsync();
            var dtos = expenses.Select(e => new ExpenseDto
            {
                Id = e.Id,
                Category = e.Category,
                Amount = e.Amount,
                ExpenseDate = e.ExpenseDate,
                Description = e.Description,
                CarId = e.CarId
            }).ToList();

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _repository.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null) return NotFound();

            var dto = new ExpenseDto
            {
                Id = entity.Id,
                Category = entity.Category,
                Amount = entity.Amount,
                ExpenseDate = entity.ExpenseDate,
                Description = entity.Description,
                CarId = entity.CarId
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExpenseDto dto)
        {
            var entity = new Expense
            {
                Category = dto.Category,
                Amount = dto.Amount,
                ExpenseDate = dto.ExpenseDate,
                Description = dto.Description,
                CarId = dto.CarId
            };
            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            
            return Ok(entity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExpenseDto dto)
        {
            var entity = await _repository.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null) return NotFound();

            entity.Category = dto.Category;
            entity.Amount = dto.Amount;
            entity.ExpenseDate = dto.ExpenseDate;
            entity.Description = dto.Description;
            entity.CarId = dto.CarId;

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
