using gAPI.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using UwvLlm.Infrastructure.Data.Mappings;
using UwvLlm.Infrastructure.Data.UseCases;

namespace UwvLlm.Infrastructure.Data.Extensions;

public static class AddCrudExtensions
{
    public static IServiceCollection AddCrudUseCases(this IServiceCollection services)
    {
        services.AddScoped<IUseCase<Entities.MailMessage, Shared.Public.Dtos.MailMessage, Guid>, MailMessagesUseCase>();
        services.AddScoped<IUseCase<Entities.UserNotification, Shared.Public.Dtos.UserNotification, long>, UserNotificationsUseCase>();
        services.AddScoped<IUseCase<Entities.User, Shared.Public.Dtos.User, Guid>, UsersUseCase>();
        return services;
    }

    public static IServiceCollection AddCrudMappings(this IServiceCollection services)
    {
        services.AddScoped<Mapping<Entities.MailMessage, Shared.Public.Dtos.MailMessage>, MailMessagesMapping>();
        services.AddScoped<Mapping<Entities.UserNotification, Shared.Public.Dtos.UserNotification>, UserNotificationsMapping>();
        services.AddScoped<Mapping<Entities.User, Shared.Public.Dtos.User>, UsersMapping>();
        return services;
    }
}