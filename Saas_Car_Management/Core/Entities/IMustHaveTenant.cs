using System;

namespace Saas_Car_Management.Core.Entities
{
    public interface IMustHaveTenant
    {
        int TenantId { get; set; }
    }
}
