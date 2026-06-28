using System;

namespace Saas_Car_Management.Core.Interfaces
{
    public interface ITenantService
    {
        int GetTenantId();
        void SetTenantId(int tenantId);
    }
}
