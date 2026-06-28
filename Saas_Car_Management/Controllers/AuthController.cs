using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saas_Car_Management.Core.DTOs;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _userRepository.LoginAsync(dto);
            if (result == null)
            {
                return Unauthorized("Invalid credentials or account is suspended.");
            }
            return Ok(result);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            await _userRepository.RegisterAsync(dto);
            return Ok(new { message = "Registration successful. Please login." });
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            var result = await _userRepository.RefreshTokenAsync(dto);
            if (result == null)
            {
                return BadRequest("Invalid or expired refresh token.");
            }
            return Ok(result);
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var success = await _userRepository.ChangePasswordAsync(GetUserId(), dto);
            if (!success)
            {
                return BadRequest("Password update failed. Verify current password.");
            }
            return Ok(new { message = "Password changed successfully." });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var profile = await _userRepository.GetProfileAsync(GetUserId());
            if (profile == null)
            {
                return NotFound("Profile not found.");
            }
            return Ok(profile);
        }
    }
}
