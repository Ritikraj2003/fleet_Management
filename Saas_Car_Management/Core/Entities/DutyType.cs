using System.ComponentModel.DataAnnotations;

namespace Saas_Car_Management.Core.Entities
{
    public class DutyType : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty; // e.g. "HR-KM (Local)", "Day-KM (Outstation)", "Flat Rate"
        
        public int MaxKilometers { get; set; }
        public int MaxHours { get; set; }
        
        public decimal ExtraKmRate { get; set; }
        public decimal ExtraHourRate { get; set; }
    }
}
