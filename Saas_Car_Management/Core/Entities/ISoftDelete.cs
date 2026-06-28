using System;

namespace Saas_Car_Management.Core.Entities
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}
