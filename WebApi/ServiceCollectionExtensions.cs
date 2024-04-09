namespace WebApi;

public static class ServiceCollectionExtensions
{
    public static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
    {
        var scope = app.ApplicationServices.CreateScope();
        var seeders = scope.ServiceProvider.GetServices<ApplicationDbSeeder>();

        foreach (var seeder in seeders)
        {
            seeder.SeedDataAsync().GetAwaiter().GetResult();
        }

        return app;
    }

    public static IServiceCollection AddIdentitySettings(this IServiceCollection services)
    {
        services
            .AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

        return services;
    }
}
