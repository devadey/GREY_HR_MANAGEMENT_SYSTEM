namespace Infrastructure;

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


    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services
            .AddTransient<ITokenService, TokenService>()
            .AddTransient<IUserService, UserService>();
        return services;
    }


    public static IServiceCollection AddEmployeeService (this IServiceCollection services)
    {
        services
            .AddTransient<IEmployeeService, EmployeeService>();
        return services;
    }
}
