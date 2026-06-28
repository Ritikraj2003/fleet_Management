using System;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Infrastructure.Services
{
    public class TenantService : ITenantService
    {
        private int _tenantId;

        public int GetTenantId()
        {
            return _tenantId;
        }

        public void SetTenantId(int tenantId)
        {
            _tenantId = tenantId;
        }
    }
}
