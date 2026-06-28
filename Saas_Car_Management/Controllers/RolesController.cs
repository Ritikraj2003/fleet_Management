using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saas_Car_Management.Core.DTOs;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Controllers
{
    [Authorize]
    public class RolesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;

        public RolesController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var isSuperAdmin = User.IsInRole("SUPER_ADMIN");
            var roles = await _userRepository.GetRolesAsync(GetTenantId(), isSuperAdmin);
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(int id)
        {
            var isSuperAdmin = User.IsInRole("SUPER_ADMIN");
            var role = await _userRepository.GetRoleAsync(id, GetTenantId(), isSuperAdmin);
            if (role == null) return NotFound();
            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
        {
            var isSuperAdmin = User.IsInRole("SUPER_ADMIN");
            var role = await _userRepository.CreateRoleAsync(GetTenantId(), isSuperAdmin, dto);
            if (role == null) return BadRequest("Unable to create role.");
            return Ok(role);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] CreateRoleDto dto)
        {
            var isSuperAdmin = User.IsInRole("SUPER_ADMIN");
            var success = await _userRepository.UpdateRoleAsync(id, GetTenantId(), isSuperAdmin, dto);
            if (!success) return NotFound();
            return Ok(new { message = "Role updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var isSuperAdmin = User.IsInRole("SUPER_ADMIN");
            var success = await _userRepository.DeleteRoleAsync(id, GetTenantId(), isSuperAdmin);
            if (!success) return BadRequest("Role is system-defined or does not exist.");
            return Ok(new { message = "Role deleted successfully." });
        }
    }
}
