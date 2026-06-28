using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var val = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(val, out var res)) return res;
                return null;
            }
        }

        public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

        public int? TenantId
        {
            get
            {
                var tenantClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue("tenant");
                if (int.TryParse(tenantClaim, out var tenantId))
                {
                    return tenantId;
                }
                return null;
            }
        }

        public string? Role => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public IEnumerable<string> Permissions
        {
            get
            {
                var claims = _httpContextAccessor.HttpContext?.User?.FindAll("permissions");
                return claims?.Select(c => c.Value) ?? Enumerable.Empty<string>();
            }
        }
    }
}
