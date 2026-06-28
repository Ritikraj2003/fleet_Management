using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.API.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
        {
            if (context.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantHeaderStr))
            {
                if (int.TryParse(tenantHeaderStr, out var tenantId))
                {
                    tenantService.SetTenantId(tenantId);
                }
            }
            else if (context.User.Identity?.IsAuthenticated == true)
            {
                var tenantClaim = context.User.FindFirst("tenant")?.Value;
                if (!string.IsNullOrEmpty(tenantClaim) && int.TryParse(tenantClaim, out var tenantId))
                {
                    tenantService.SetTenantId(tenantId);
                }
            }

            await _next(context);
        }
    }
}
