namespace Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterMediatR(this IServiceCollection services)
    {
        return services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }
}
