﻿using Infrastructure.Models;
using System.Linq;

namespace Infrastructure.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, 
    ApplicationRole, string, 
    IdentityUserClaim<string>, 
    IdentityUserRole<string>,
    IdentityUserLogin<string>,
    ApplicationRoleClaim, 
    IdentityUserToken<string>>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(p => p.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18, 2)");
        }

        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public DbSet<Employee> Employees => Set<Employee>(); 
}
