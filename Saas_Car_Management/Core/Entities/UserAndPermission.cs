using System;

namespace Saas_Car_Management.Core.Entities
{
    public class Permission : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty; // e.g. "Car.View", "Car.Add"
        public string ModuleName { get; set; } = string.Empty; // e.g. "Cars", "Drivers", "VendorManagement", "Marketplace"
    }

    public class TenantPermission : BaseEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int PermissionId { get; set; }

        // Navigation
        public Permission Permission { get; set; } = null!;
    }

    public class User : BaseEntity
    {
        public int? TenantId { get; set; } // Null for Super Admin
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsSuperAdmin { get; set; } = false;
    }

    public class Role : BaseEntity
    {
        public int? TenantId { get; set; } // Null for Global Roles
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty; // e.g. "SUPER_ADMIN", "COMPANY_ADMIN", "FLEET_MANAGER"
        public string Description { get; set; } = string.Empty;
        public bool IsSystemRole { get; set; } = false;
    }

    public class UserRole : BaseEntity
    {
        public int? TenantId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        // Navigation
        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }

    public class RolePermission : BaseEntity
    {
        public int? TenantId { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        // Navigation
        public Role Role { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }

    public class RefreshToken : BaseEntity
    {
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Revoked { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsActive => Revoked == null && !IsExpired;

        // Navigation
        public User User { get; set; } = null!;
    }
}
