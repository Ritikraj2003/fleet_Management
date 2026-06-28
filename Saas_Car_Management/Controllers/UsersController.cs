using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saas_Car_Management.Core.DTOs;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var isSuperAdmin = User.IsInRole("SUPER_ADMIN");
            var users = await _userRepository.GetUsersAsync(GetTenantId(), isSuperAdmin);
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var isSuperAdmin = User.IsInRole("SUPER_ADMIN");
            var result = await _userRepository.CreateUserAsync(GetTenantId(), isSuperAdmin, dto);
            if (result == null) return BadRequest("Could not create user account.");
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] CreateUserDto dto)
        {
            var isSuperAdmin = User.IsInRole("SUPER_ADMIN");
            var success = await _userRepository.UpdateUserAsync(id, GetTenantId(), isSuperAdmin, dto);
            if (!success) return NotFound();
            return Ok(new { message = "User account updated." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var isSuperAdmin = User.IsInRole("SUPER_ADMIN");
            var success = await _userRepository.DeleteUserAsync(id, GetTenantId(), isSuperAdmin);
            if (!success) return NotFound();
            return Ok(new { message = "User deleted successfully." });
        }
    }
}
