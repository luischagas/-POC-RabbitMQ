using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQExample.Api.Extensions;
using RabbitMQExample.Domain.Interfaces;
using RabbitMQExample.Domain.Models;
using RabbitMQExample.Infrastructure.Rabbit;
using RabbitMQExample.Infrastructure.Rabbit.Providers;

namespace RabbitMQExample.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqConfigurations = new RabbitMqSettings();
            new ConfigureFromConfigurationOptions<RabbitMqSettings>(
                    configuration.GetSection("RabbitMQConfigurations"))
                .Configure(rabbitMqConfigurations);

            services.AddSingleton(rabbitMqConfigurations);

            services.AddSingleton<IResponseRabbitService, ResponseRabbitService>();

            services.AddSingleton<IRabbitDatabaseProvider, RabbitDatabaseProvider>();

            services.AddHostedService<ConsumeRabbitMqHostedService>();

            return services;
        }
    }
}