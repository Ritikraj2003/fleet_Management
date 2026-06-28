using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saas_Car_Management.Core.DTOs;
using Saas_Car_Management.Core.Entities;

namespace Saas_Car_Management.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<TokenResponseDto?> LoginAsync(LoginDto dto);
        Task RegisterAsync(RegisterDto dto);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto dto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);
        Task<object?> GetProfileAsync(int userId);
        
        Task<IEnumerable<UserDto>> GetUsersAsync(int tenantId, bool isSuperAdmin);
        Task<UserDto?> CreateUserAsync(int tenantId, bool isSuperAdmin, CreateUserDto dto);
        Task<bool> UpdateUserAsync(int id, int tenantId, bool isSuperAdmin, CreateUserDto dto);
        Task<bool> DeleteUserAsync(int id, int tenantId, bool isSuperAdmin);

        Task<IEnumerable<RoleDto>> GetRolesAsync(int tenantId, bool isSuperAdmin);
        Task<RoleDto?> GetRoleAsync(int id, int tenantId, bool isSuperAdmin);
        Task<RoleDto?> CreateRoleAsync(int tenantId, bool isSuperAdmin, CreateRoleDto dto);
        Task<bool> UpdateRoleAsync(int id, int tenantId, bool isSuperAdmin, CreateRoleDto dto);
        Task<bool> DeleteRoleAsync(int id, int tenantId, bool isSuperAdmin);

        Task<IEnumerable<Permission>> GetPermissionsAsync(int tenantId, bool isSuperAdmin);
    }
}
