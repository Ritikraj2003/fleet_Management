using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Saas_Car_Management.Core.Interfaces;
using Saas_Car_Management.Infrastructure.Persistence;

namespace Saas_Car_Management.API.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class HasPermissionAttribute : TypeFilterAttribute
    {
        public HasPermissionAttribute(string permissionCode) : base(typeof(PermissionFilter))
        {
            Arguments = new object[] { permissionCode };
        }
    }

    public class PermissionFilter : IAsyncAuthorizationFilter
    {
        private readonly string _permissionCode;

        public PermissionFilter(string permissionCode)
        {
            _permissionCode = permissionCode;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user.Identity?.IsAuthenticated != true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Super Admin bypasses all checks
            if (user.IsInRole("SUPER_ADMIN"))
            {
                return;
            }

            var tenantService = context.HttpContext.RequestServices.GetRequiredService<ITenantService>();
            var dbContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();

            var tenantId = tenantService.GetTenantId();
            if (tenantId == 0)
            {
                context.Result = new ForbidResult();
                return;
            }

            var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var roleIds = dbContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToList();

            if (!roleIds.Any())
            {
                context.Result = new ForbidResult();
                return;
            }

            // Check 1: TenantPermissionExists (Tenant has this permission enabled in their subscription)
            var tenantHasPermission = dbContext.TenantPermissions
                .Any(tp => tp.TenantId == tenantId && tp.Permission.Code == _permissionCode);

            if (!tenantHasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }

            // Check 2: RolePermissionExists (Role assigned to user has this permission)
            var roleHasPermission = dbContext.RolePermissions
                .Any(rp => roleIds.Contains(rp.RoleId) && rp.Permission.Code == _permissionCode);

            if (!roleHasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
