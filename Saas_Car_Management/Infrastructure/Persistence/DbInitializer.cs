using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saas_Car_Management.Core.Entities;

namespace Saas_Car_Management.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            // 1. Seed Permissions
            var permissions = new List<Permission>
            {
                new Permission { Name = "View Cars", Code = "Car.View", ModuleName = "Cars" },
                new Permission { Name = "Add Cars", Code = "Car.Add", ModuleName = "Cars" },
                new Permission { Name = "Edit Cars", Code = "Car.Edit", ModuleName = "Cars" },

                new Permission { Name = "View Drivers", Code = "Driver.View", ModuleName = "Drivers" },
                new Permission { Name = "Add Drivers", Code = "Driver.Add", ModuleName = "Drivers" },
                new Permission { Name = "Edit Drivers", Code = "Driver.Edit", ModuleName = "Drivers" },

                new Permission { Name = "View Bookings", Code = "Booking.View", ModuleName = "Bookings" },
                new Permission { Name = "Create Bookings", Code = "Booking.Create", ModuleName = "Bookings" },

                new Permission { Name = "View Vendor Management", Code = "VendorManagement.View", ModuleName = "VendorManagement" },
                new Permission { Name = "Create Vendors", Code = "VendorManagement.Create", ModuleName = "VendorManagement" },

                new Permission { Name = "Marketplace Requests & Offers", Code = "Marketplace.Request", ModuleName = "Marketplace" },
                new Permission { Name = "View Reports", Code = "Reports.View", ModuleName = "Reports" }
            };

            foreach (var perm in permissions)
            {
                if (!await context.Permissions.AnyAsync(p => p.Code == perm.Code))
                {
                    await context.Permissions.AddAsync(perm);
                }
            }
            await context.SaveChangesAsync();

            // Reload permissions with database IDs
            var dbPermissions = await context.Permissions.ToListAsync();

            // 2. Seed Subscription Plans
            var plans = new List<Plan>
            {
                new Plan
                {
                    Name = "Basic Plan",
                    Description = "Basic fleet operation capabilities.",
                    Price = 49.00m,
                    MaxCars = 10,
                    MaxUsers = 3,
                    EnabledModules = "Cars,Drivers,Bookings",
                    IsActive = true
                },
                new Plan
                {
                    Name = "Premium Plan",
                    Description = "Access to vendor management, reporting, and marketplace integration.",
                    Price = 149.00m,
                    MaxCars = 50,
                    MaxUsers = 15,
                    EnabledModules = "Cars,Drivers,Bookings,VendorManagement,Marketplace,Reports",
                    IsActive = true
                },
                new Plan
                {
                    Name = "Enterprise Plan",
                    Description = "Unlimited access to all modules and capacity.",
                    Price = 299.00m,
                    MaxCars = 9999,
                    MaxUsers = 999,
                    EnabledModules = "Cars,Drivers,Bookings,VendorManagement,Marketplace,Reports",
                    IsActive = true
                }
            };

            foreach (var plan in plans)
            {
                if (!await context.Plans.AnyAsync(p => p.Name == plan.Name))
                {
                    await context.Plans.AddAsync(plan);
                }
            }
            await context.SaveChangesAsync();

            // 3. Seed Super Admin Role
            var superAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == "SUPER_ADMIN");
            if (superAdminRole == null)
            {
                superAdminRole = new Role
                {
                    Name = "Super Administrator",
                    Code = "SUPER_ADMIN",
                    Description = "Global SaaS administrator.",
                    IsSystemRole = true
                };
                await context.Roles.AddAsync(superAdminRole);
                await context.SaveChangesAsync();

                // Give Super Admin all permissions
                foreach (var perm in dbPermissions)
                {
                    await context.RolePermissions.AddAsync(new RolePermission
                    {
                        RoleId = superAdminRole.Id,
                        PermissionId = perm.Id
                    });
                }
                await context.SaveChangesAsync();
            }

            // 4. Seed Super Admin User
            var existingSuperAdmin = await context.Users.FirstOrDefaultAsync(u => u.Email == "superadmin@fleetflow.com");
            if (existingSuperAdmin == null)
            {
                var user = new User
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    Email = "superadmin@fleetflow.com",
                    PasswordHash = "Password123!",
                    IsActive = true,
                    IsSuperAdmin = true
                };

                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                await context.UserRoles.AddAsync(new UserRole
                {
                    UserId = user.Id,
                    RoleId = superAdminRole.Id
                });
                await context.SaveChangesAsync();
            }
            else
            {
                // Ensure existing super admin has plain text password and IsSuperAdmin flag
                existingSuperAdmin.PasswordHash = "Password123!";
                existingSuperAdmin.IsSuperAdmin = true;
                await context.SaveChangesAsync();
            }

            // Convert any existing BCrypt hashed passwords in DB to plain text Password123!
            var hashedUsers = await context.Users.Where(u => u.PasswordHash.StartsWith("$2a$")).ToListAsync();
            if (hashedUsers.Any())
            {
                foreach (var hu in hashedUsers)
                {
                    hu.PasswordHash = "Password123!";
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
