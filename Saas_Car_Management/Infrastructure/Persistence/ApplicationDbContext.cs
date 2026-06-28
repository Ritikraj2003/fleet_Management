using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Saas_Car_Management.Core.Entities;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ITenantService _tenantService;
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ITenantService tenantService,
            ICurrentUserService currentUserService) : base(options)
        {
            _tenantService = tenantService;
            _currentUserService = currentUserService;
        }

        public int CurrentTenantId => _tenantService.GetTenantId();

        // DbSets
        public DbSet<Plan> Plans { get; set; } = null!;
        public DbSet<Tenant> Tenants { get; set; } = null!;
        public DbSet<Permission> Permissions { get; set; } = null!;
        public DbSet<TenantPermission> TenantPermissions { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;
        public DbSet<RolePermission> RolePermissions { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Car> Cars { get; set; } = null!;
        public DbSet<Driver> Drivers { get; set; } = null!;
        public DbSet<DriverVehicleAssignment> DriverVehicleAssignments { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<DutyType> DutyTypes { get; set; } = null!;
        public DbSet<BookingVehicle> BookingVehicles { get; set; } = null!;
        public DbSet<RideTracking> RideTrackings { get; set; } = null!;
        public DbSet<Partner> Partners { get; set; } = null!;
        public DbSet<PartnerVehicle> PartnerVehicles { get; set; } = null!;
        public DbSet<PartnerDriver> PartnerDrivers { get; set; } = null!;
        public DbSet<MarketplaceRequest> MarketplaceRequests { get; set; } = null!;
        public DbSet<MarketplaceOffer> MarketplaceOffers { get; set; } = null!;
        public DbSet<MarketplaceAssignment> MarketplaceAssignments { get; set; } = null!;
        public DbSet<MarketplaceVehicleAvailability> MarketplaceVehicleAvailabilities { get; set; } = null!;
        public DbSet<MarketplaceTransaction> MarketplaceTransactions { get; set; } = null!;
        public DbSet<Invoice> Invoices { get; set; } = null!;
        public DbSet<VendorPayment> VendorPayments { get; set; } = null!;
        public DbSet<Expense> Expenses { get; set; } = null!;
        public DbSet<VehicleMaintenance> VehicleMaintenances { get; set; } = null!;
        public DbSet<DriverAttendance> DriverAttendances { get; set; } = null!;
        public DbSet<Subscription> Subscriptions { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<Saas_Car_Management.Core.Entities.File> Files { get; set; } = null!;
        public DbSet<EntityFile> EntityFiles { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Relationships and Cascades
            modelBuilder.Entity<Tenant>()
                .HasOne(t => t.Plan)
                .WithMany()
                .HasForeignKey(t => t.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Plan)
                .WithMany()
                .HasForeignKey(s => s.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany()
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany()
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TenantPermission>()
                .HasOne(tp => tp.Permission)
                .WithMany()
                .HasForeignKey(tp => tp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookingVehicle>()
                .HasOne(bv => bv.Booking)
                .WithMany(b => b.BookingVehicles)
                .HasForeignKey(bv => bv.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Apply global query filters dynamically for Soft Delete & Tenant Isolation
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var type = entityType.ClrType;

                if (typeof(ISoftDelete).IsAssignableFrom(type) && typeof(IMustHaveTenant).IsAssignableFrom(type))
                {
                    var method = typeof(ApplicationDbContext)
                        .GetMethod(nameof(SetTenantAndSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Instance)?
                        .MakeGenericMethod(type);
                    method?.Invoke(this, new object[] { modelBuilder });
                }
                else if (typeof(ISoftDelete).IsAssignableFrom(type))
                {
                    var method = typeof(ApplicationDbContext)
                        .GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Instance)?
                        .MakeGenericMethod(type);
                    method?.Invoke(this, new object[] { modelBuilder });
                }
                else if (typeof(IMustHaveTenant).IsAssignableFrom(type))
                {
                    var method = typeof(ApplicationDbContext)
                        .GetMethod(nameof(SetTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)?
                        .MakeGenericMethod(type);
                    method?.Invoke(this, new object[] { modelBuilder });
                }
            }
        }

        private void SetTenantAndSoftDeleteFilter<T>(ModelBuilder modelBuilder) where T : class, ISoftDelete, IMustHaveTenant
        {
            modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted && e.TenantId == CurrentTenantId);
        }

        private void SetSoftDeleteFilter<T>(ModelBuilder modelBuilder) where T : class, ISoftDelete
        {
            modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }

        private void SetTenantFilter<T>(ModelBuilder modelBuilder) where T : class, IMustHaveTenant
        {
            modelBuilder.Entity<T>().HasQueryFilter(e => e.TenantId == CurrentTenantId);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var tenantId = _tenantService.GetTenantId();
            var userId = _currentUserService.UserId;

            var auditEntries = new List<AuditLogEntry>();

            foreach (var entry in ChangeTracker.Entries())
            {
                // Auto tenant injection
                if (entry.Entity is IMustHaveTenant tenantEntity)
                {
                    if (entry.State == EntityState.Added && tenantEntity.TenantId == 0)
                    {
                        tenantEntity.TenantId = tenantId;
                    }
                }

                // Soft Delete
                if (entry.Entity is ISoftDelete softDeleteEntity)
                {
                    if (entry.State == EntityState.Deleted)
                    {
                        entry.State = EntityState.Modified;
                        softDeleteEntity.IsDeleted = true;
                        softDeleteEntity.DeletedAt = DateTime.UtcNow;
                    }
                }

                // BaseEntity tracking
                if (entry.Entity is BaseEntity baseEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        baseEntity.CreatedAt = DateTime.UtcNow;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        baseEntity.UpdatedAt = DateTime.UtcNow;
                    }
                }

                // Audit Logging preparation
                if (entry.Entity is not AuditLog && entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                {
                    var auditEntry = new AuditLogEntry(entry)
                    {
                        TenantId = tenantId == 0 ? null : tenantId,
                        UserId = userId,
                        Action = entry.State.ToString(),
                        EntityName = entry.Metadata.ClrType.Name,
                        EntityId = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString() ?? string.Empty
                    };
                    auditEntries.Add(auditEntry);
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            if (auditEntries.Count > 0)
            {
                var logs = auditEntries.Select(e => e.ToAuditLog()).ToList();
                AuditLogs.AddRange(logs);
                await base.SaveChangesAsync(cancellationToken);
            }

            return result;
        }
    }

    public class AuditLogEntry
    {
        public AuditLogEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public EntityEntry Entry { get; }
        public int? TenantId { get; set; }
        public int? UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public Dictionary<string, object?> KeyValues { get; } = new();
        public Dictionary<string, object?> OldValues { get; } = new();
        public Dictionary<string, object?> NewValues { get; } = new();
        public List<string> ChangedColumns { get; } = new();

        public AuditLog ToAuditLog()
        {
            foreach (var property in Entry.Properties)
            {
                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (Entry.State)
                {
                    case EntityState.Added:
                        NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified && !Equals(property.OriginalValue, property.CurrentValue))
                        {
                            ChangedColumns.Add(propertyName);
                            OldValues[propertyName] = property.OriginalValue;
                            NewValues[propertyName] = property.CurrentValue;
                        }
                        break;
                }
            }

            return new AuditLog
            {
                TenantId = TenantId,
                UserId = UserId,
                Action = Action,
                EntityName = EntityName,
                EntityId = EntityId,
                Changes = JsonSerializer.Serialize(new
                {
                    keys = KeyValues,
                    oldValues = OldValues,
                    newValues = NewValues,
                    changedColumns = ChangedColumns
                }),
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
