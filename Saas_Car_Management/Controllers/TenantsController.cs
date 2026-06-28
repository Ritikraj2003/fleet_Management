using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saas_Car_Management.Core.DTOs;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Controllers
{
    [Authorize(Roles = "SUPER_ADMIN")]
    public class TenantsController : BaseApiController
    {
        private readonly ITenantRepository _tenantRepository;

        public TenantsController(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetTenants()
        {
            var tenants = await _tenantRepository.GetTenantsAsync();
            return Ok(tenants);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantDto dto)
        {
            var result = await _tenantRepository.CreateTenantAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}/subscription")]
        public async Task<IActionResult> UpdateSubscription(int id, [FromBody] int planId)
        {
            var success = await _tenantRepository.UpdateSubscriptionAsync(id, planId);
            if (!success) return NotFound();
            return Ok(new { message = "Tenant subscription plan updated." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTenant(int id)
        {
            var success = await _tenantRepository.DeleteTenantAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Tenant suspended successfully." });
        }
    }
}
