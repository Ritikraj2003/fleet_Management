using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Controllers
{
    [Authorize]
    public class PermissionsController : BaseApiController
    {
        private readonly IUserRepository _userRepository;

        public PermissionsController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetPermissions()
        {
            var isSuperAdmin = User.IsInRole("SUPER_ADMIN");
            var permissions = await _userRepository.GetPermissionsAsync(GetTenantId(), isSuperAdmin);
            return Ok(permissions);
        }
    }
}
