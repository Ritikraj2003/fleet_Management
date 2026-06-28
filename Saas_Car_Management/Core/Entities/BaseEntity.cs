using System;

namespace Saas_Car_Management.Core.Entities
{
    public abstract class BaseEntity : ISoftDelete
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // ISoftDelete members
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}
