﻿namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connection = configuration.GetConnectionString("DefaultConnection");
        services.AddTransient<ApplicationDbSeeder>()
              .AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connection);
        });
       
        return services;
    }
}