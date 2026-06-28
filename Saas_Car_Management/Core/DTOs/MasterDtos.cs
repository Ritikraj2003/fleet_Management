namespace Saas_Car_Management.Core.DTOs
{
    public class DutyTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int MaxKilometers { get; set; }
        public int MaxHours { get; set; }
        public decimal ExtraKmRate { get; set; }
        public decimal ExtraHourRate { get; set; }
    }

    public class CreateDutyTypeDto
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int MaxKilometers { get; set; }
        public int MaxHours { get; set; }
        public decimal ExtraKmRate { get; set; }
        public decimal ExtraHourRate { get; set; }
    }

    public class UpdateDutyTypeDto
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int MaxKilometers { get; set; }
        public int MaxHours { get; set; }
        public decimal ExtraKmRate { get; set; }
        public decimal ExtraHourRate { get; set; }
    }
}
