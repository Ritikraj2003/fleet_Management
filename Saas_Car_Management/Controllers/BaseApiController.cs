using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Saas_Car_Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected int GetTenantId()
        {
            var tenantClaim = User.FindFirst("tenant")?.Value;
            if (int.TryParse(tenantClaim, out var tenantId))
            {
                return tenantId;
            }
            return 0;
        }

        protected int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            return 0;
        }
    }
}
