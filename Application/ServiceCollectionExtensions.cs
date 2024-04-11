using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterMediatR(this IServiceCollection services,
        Action<MediatRServiceConfiguration> configuration)
        {
            var serviceConfig = new MediatRServiceConfiguration();

            configuration.Invoke(serviceConfig);

            return services.AddMediatR(serviceConfig);
        }
    }
}
