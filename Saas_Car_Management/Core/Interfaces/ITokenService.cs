using System;
using System.Collections.Generic;
using Saas_Car_Management.Core.Entities;

namespace Saas_Car_Management.Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user, string role, IEnumerable<string> permissions);
        RefreshToken GenerateRefreshToken(int userId);
    }
}
