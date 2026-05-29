using gAPI.Core.ServiceBus.Interfaces;
using gAPI.Core.ServiceBus.Services;
using Microsoft.Extensions.DependencyInjection;

namespace gAPI.Core.ServiceBus.Extensions;

public static class AddServiceServiceBusExtension
{
    public static IServiceCollection AddServiceBus(this IServiceCollection services)
    {
        services.AddSingleton<IRabbitServiceBusConnectionProvider, RabbitServiceBusConnectionProvider>();
        services.AddSingleton<IServiceBusHandlerRegistry, ServiceBusHandlerRegistry>();
        services.AddSingleton<IServiceBusReceiver, ServiceBusReceiver>();
        services.AddSingleton<IServiceBusSender, ServiceBusSender>();
        services.AddSingleton<IConsoleService, ConsoleService>();
        return services;
    }
}
