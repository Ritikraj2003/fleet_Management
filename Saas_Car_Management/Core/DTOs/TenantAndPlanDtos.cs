using System;
using System.Collections.Generic;

namespace Saas_Car_Management.Core.DTOs
{
    public class PlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int MaxCars { get; set; }
        public int MaxUsers { get; set; }
        public List<string> EnabledModules { get; set; } = new();
        public bool IsActive { get; set; }
    }

    public class CreatePlanDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int MaxCars { get; set; }
        public int MaxUsers { get; set; }
        public List<string> EnabledModules { get; set; } = new();
    }

    public class TenantDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public int PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateTenantDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public int PlanId { get; set; }
    }

    public class TenantPermissionDto
    {
        public int TenantId { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
}
