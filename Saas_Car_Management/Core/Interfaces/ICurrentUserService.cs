using System;
using System.Collections.Generic;

namespace Saas_Car_Management.Core.Interfaces
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? Email { get; }
        int? TenantId { get; }
        string? Role { get; }
        bool IsAuthenticated { get; }
        IEnumerable<string> Permissions { get; }
    }
}
