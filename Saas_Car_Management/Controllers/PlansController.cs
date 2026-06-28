using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saas_Car_Management.Core.DTOs;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Controllers
{
    [Authorize(Roles = "SUPER_ADMIN")]
    public class PlansController : BaseApiController
    {
        private readonly IPlanRepository _planRepository;

        public PlansController(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPlans()
        {
            var plans = await _planRepository.GetPlansAsync();
            return Ok(plans);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlan(int id)
        {
            var plan = await _planRepository.GetPlanByIdAsync(id);
            if (plan == null) return NotFound();
            return Ok(plan);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlan([FromBody] CreatePlanDto dto)
        {
            var result = await _planRepository.CreatePlanAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlan(int id, [FromBody] CreatePlanDto dto)
        {
            var success = await _planRepository.UpdatePlanAsync(id, dto);
            if (!success) return NotFound();
            return Ok(new { message = "Plan updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlan(int id)
        {
            var success = await _planRepository.DeletePlanAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Plan deactivated successfully." });
        }
    }
}
