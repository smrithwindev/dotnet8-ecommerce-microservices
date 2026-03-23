using BuildingBlocks.Web.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder; // Ensure this is included for IApplicationBuilder

namespace ProductApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            // Register your infrastructure services here
            //For example: services.AddScoped<IProductRepository, ProductRepository>();

            SharedServiceContainer.AddSharedServices<ProductDbContext>(services, config, config["MySerilog:FileName"]!);

            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            //Register middleware such as:
            //Global Exception : Handles external errors.
            //Listen to Only Api Gateway : blocks all outsider calls;

            SharedServiceContainer.UseSharedPolicies(app);

            return app;
        }
    }
}
