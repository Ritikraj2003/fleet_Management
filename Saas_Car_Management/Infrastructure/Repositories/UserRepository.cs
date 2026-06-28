using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saas_Car_Management.Core.DTOs;
using Saas_Car_Management.Core.Entities;
using Saas_Car_Management.Core.Interfaces;
using Saas_Car_Management.Infrastructure.Persistence;

namespace Saas_Car_Management.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public UserRepository(ApplicationDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<TokenResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsActive);
            if (user == null || user.PasswordHash != dto.Password)
            {
                return null;
            }

            var userRole = await _context.UserRoles
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => ur.UserId == user.Id);

            var roleCode = userRole?.Role?.Code ?? "USER";
            var permissions = new List<string>();

            if (userRole != null)
            {
                permissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == userRole.RoleId)
                    .Include(rp => rp.Permission)
                    .Select(rp => rp.Permission.Code)
                    .ToListAsync();
            }

            var accessToken = _tokenService.GenerateAccessToken(user, roleCode, permissions);
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

            // Invalidate existing refresh tokens
            var oldTokens = await _context.RefreshTokens.Where(rt => rt.UserId == user.Id).ToListAsync();
            foreach (var ot in oldTokens)
            {
                ot.Revoked = DateTime.UtcNow;
            }

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            var tenant = user.TenantId.HasValue ? await _context.Tenants.FindAsync(user.TenantId.Value) : null;

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                Email = user.Email,
                Role = roleCode,
                TenantId = user.TenantId,
                CompanyName = tenant?.CompanyName,
                Permissions = permissions
            };
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Create Tenant
                var tenant = new Tenant
                {
                    CompanyName = dto.CompanyName,
                    Domain = dto.Domain,
                    PlanId = dto.PlanId,
                    IsActive = true,
                    Status = "Active"
                };
                await _context.Tenants.AddAsync(tenant);
                await _context.SaveChangesAsync();

                // Create Tenant Owner User
                var user = new User
                {
                    TenantId = tenant.Id,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    PasswordHash = dto.Password,
                    IsActive = true
                };
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                // Create Default COMPANY_ADMIN role for the tenant
                var role = new Role
                {
                    TenantId = tenant.Id,
                    Name = "Company Administrator",
                    Code = "COMPANY_ADMIN",
                    Description = "Full access to company fleet settings and bookings.",
                    IsSystemRole = true
                };
                await _context.Roles.AddAsync(role);
                await _context.SaveChangesAsync();

                // Assign COMPANY_ADMIN role to user
                var userRole = new UserRole
                {
                    TenantId = tenant.Id,
                    UserId = user.Id,
                    RoleId = role.Id
                };
                await _context.UserRoles.AddAsync(userRole);

                // Give COMPANY_ADMIN all permissions allowed by their plan
                var plan = await _context.Plans.FindAsync(dto.PlanId);
                if (plan != null)
                {
                    var allowedModules = plan.EnabledModules.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var permissions = await _context.Permissions
                        .Where(p => allowedModules.Contains(p.ModuleName))
                        .ToListAsync();

                    foreach (var perm in permissions)
                    {
                        await _context.TenantPermissions.AddAsync(new TenantPermission
                        {
                            TenantId = tenant.Id,
                            PermissionId = perm.Id
                        });

                        await _context.RolePermissions.AddAsync(new RolePermission
                        {
                            TenantId = tenant.Id,
                            RoleId = role.Id,
                            PermissionId = perm.Id
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto dto)
        {
            var principal = GetPrincipalFromExpiredToken(dto.AccessToken);
            if (principal == null) return null;

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return null;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            var storedRefreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken && rt.UserId == userId);

            if (storedRefreshToken == null || !storedRefreshToken.IsActive) return null;

            // Revoke old refresh token
            storedRefreshToken.Revoked = DateTime.UtcNow;

            // Generate new token responses
            var userRole = await _context.UserRoles
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => ur.UserId == user.Id);

            var roleCode = userRole?.Role?.Code ?? "USER";
            var permissions = new List<string>();

            if (userRole != null)
            {
                permissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == userRole.RoleId)
                    .Include(rp => rp.Permission)
                    .Select(rp => rp.Permission.Code)
                    .ToListAsync();
            }

            var newAccessToken = _tokenService.GenerateAccessToken(user, roleCode, permissions);
            var newRefreshToken = _tokenService.GenerateRefreshToken(user.Id);

            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();

            var tenant = user.TenantId.HasValue ? await _context.Tenants.FindAsync(user.TenantId.Value) : null;

            return new TokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                Email = user.Email,
                Role = roleCode,
                TenantId = user.TenantId,
                CompanyName = tenant?.CompanyName,
                Permissions = permissions
            };
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ReadJwtToken(token);
                return new ClaimsPrincipal(new ClaimsIdentity(principal.Claims, "Bearer"));
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.PasswordHash != dto.CurrentPassword)
            {
                return false;
            }

            user.PasswordHash = dto.NewPassword;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<object?> GetProfileAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;

            var userRole = await _context.UserRoles
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => ur.UserId == userId);

            return new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.IsActive,
                Role = userRole?.Role?.Name ?? "User",
                RoleCode = userRole?.Role?.Code ?? "USER"
            };
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync(int tenantId, bool isSuperAdmin)
        {
            var query = _context.Users.AsQueryable();
            if (!isSuperAdmin)
            {
                query = query.Where(u => u.TenantId == tenantId);
            }

            var users = await query.ToListAsync();
            var dtos = new List<UserDto>();

            foreach (var u in users)
            {
                var userRole = await _context.UserRoles
                    .Include(ur => ur.Role)
                    .FirstOrDefaultAsync(ur => ur.UserId == u.Id);

                dtos.Add(new UserDto
                {
                    Id = u.Id,
                    TenantId = u.TenantId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    RoleName = userRole?.Role?.Name ?? "No Role",
                    RoleCode = userRole?.Role?.Code ?? "NONE"
                });
            }

            return dtos;
        }

        public async Task<UserDto?> CreateUserAsync(int tenantId, bool isSuperAdmin, CreateUserDto dto)
        {
            var tenantIdValue = isSuperAdmin ? (int?)null : tenantId;

            // Check if plan allows more users
            if (tenantIdValue.HasValue)
            {
                var tenant = await _context.Tenants.Include(t => t.Plan).FirstOrDefaultAsync(t => t.Id == tenantIdValue.Value);
                if (tenant != null)
                {
                    var currentUserCount = await _context.Users.CountAsync(u => u.TenantId == tenantIdValue.Value);
                    if (currentUserCount >= tenant.Plan.MaxUsers)
                    {
                        throw new InvalidOperationException("Active staff user limit reached for your subscription plan.");
                    }
                }
            }

            var user = new User
            {
                TenantId = tenantIdValue,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = dto.Password,
                IsActive = true
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var role = await _context.Roles.FindAsync(dto.RoleId);
            if (role != null)
            {
                await _context.UserRoles.AddAsync(new UserRole
                {
                    TenantId = tenantIdValue,
                    UserId = user.Id,
                    RoleId = role.Id
                });
                await _context.SaveChangesAsync();
            }

            return new UserDto
            {
                Id = user.Id,
                TenantId = user.TenantId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsActive = user.IsActive,
                RoleName = role?.Name ?? string.Empty,
                RoleCode = role?.Code ?? string.Empty
            };
        }

        public async Task<bool> UpdateUserAsync(int id, int tenantId, bool isSuperAdmin, CreateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || (!isSuperAdmin && user.TenantId != tenantId)) return false;

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.PasswordHash = dto.Password;
            }

            // Update Role
            var existingUserRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == id);
            if (existingUserRole != null)
            {
                _context.UserRoles.Remove(existingUserRole);
            }

            var role = await _context.Roles.FindAsync(dto.RoleId);
            if (role != null)
            {
                await _context.UserRoles.AddAsync(new UserRole
                {
                    TenantId = user.TenantId,
                    UserId = user.Id,
                    RoleId = role.Id
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id, int tenantId, bool isSuperAdmin)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || (!isSuperAdmin && user.TenantId != tenantId)) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RoleDto>> GetRolesAsync(int tenantId, bool isSuperAdmin)
        {
            var query = _context.Roles.AsQueryable();
            if (!isSuperAdmin)
            {
                query = query.Where(r => r.TenantId == tenantId);
            }

            var roles = await query.ToListAsync();
            var dtos = new List<RoleDto>();

            foreach (var r in roles)
            {
                var perms = await _context.RolePermissions
                    .Where(rp => rp.RoleId == r.Id)
                    .Include(rp => rp.Permission)
                    .Select(rp => rp.Permission.Code)
                    .ToListAsync();

                dtos.Add(new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Code = r.Code,
                    Description = r.Description,
                    IsSystemRole = r.IsSystemRole,
                    Permissions = perms
                });
            }

            return dtos;
        }

        public async Task<RoleDto?> GetRoleAsync(int id, int tenantId, bool isSuperAdmin)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null || (!isSuperAdmin && role.TenantId != tenantId)) return null;

            var perms = await _context.RolePermissions
                .Where(rp => rp.RoleId == role.Id)
                .Include(rp => rp.Permission)
                .Select(rp => rp.Permission.Code)
                .ToListAsync();

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Code = role.Code,
                Description = role.Description,
                IsSystemRole = role.IsSystemRole,
                Permissions = perms
            };
        }

        public async Task<RoleDto?> CreateRoleAsync(int tenantId, bool isSuperAdmin, CreateRoleDto dto)
        {
            var tenantIdValue = isSuperAdmin ? (int?)null : tenantId;

            var role = new Role
            {
                TenantId = tenantIdValue,
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                IsSystemRole = false
            };

            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            // Link permissions
            if (dto.Permissions != null && dto.Permissions.Count > 0)
            {
                var permissions = await _context.Permissions
                    .Where(p => dto.Permissions.Contains(p.Code))
                    .ToListAsync();

                foreach (var perm in permissions)
                {
                    await _context.RolePermissions.AddAsync(new RolePermission
                    {
                        TenantId = tenantIdValue,
                        RoleId = role.Id,
                        PermissionId = perm.Id
                    });
                }
                await _context.SaveChangesAsync();
            }

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Code = role.Code,
                Description = role.Description,
                IsSystemRole = role.IsSystemRole,
                Permissions = dto.Permissions ?? new List<string>()
            };
        }

        public async Task<bool> UpdateRoleAsync(int id, int tenantId, bool isSuperAdmin, CreateRoleDto dto)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null || (!isSuperAdmin && role.TenantId != tenantId)) return false;

            role.Name = dto.Name;
            role.Code = dto.Code;
            role.Description = dto.Description;

            // Remove existing permissions
            var existingPerms = await _context.RolePermissions.Where(rp => rp.RoleId == id).ToListAsync();
            _context.RolePermissions.RemoveRange(existingPerms);

            // Add new permissions
            if (dto.Permissions != null && dto.Permissions.Count > 0)
            {
                var permissions = await _context.Permissions
                    .Where(p => dto.Permissions.Contains(p.Code))
                    .ToListAsync();

                foreach (var perm in permissions)
                {
                    await _context.RolePermissions.AddAsync(new RolePermission
                    {
                        TenantId = role.TenantId,
                        RoleId = role.Id,
                        PermissionId = perm.Id
                    });
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRoleAsync(int id, int tenantId, bool isSuperAdmin)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null || (!isSuperAdmin && role.TenantId != tenantId) || role.IsSystemRole) return false;

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Permission>> GetPermissionsAsync(int tenantId, bool isSuperAdmin)
        {
            if (isSuperAdmin)
            {
                return await _context.Permissions.ToListAsync();
            }

            // Return permissions allowed by tenant's current plan
            var tenant = await _context.Tenants.Include(t => t.Plan).FirstOrDefaultAsync(t => t.Id == tenantId);
            if (tenant == null) return Enumerable.Empty<Permission>();

            var allowedModules = tenant.Plan.EnabledModules.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return await _context.Permissions.Where(p => allowedModules.Contains(p.ModuleName)).ToListAsync();
        }
    }
}
