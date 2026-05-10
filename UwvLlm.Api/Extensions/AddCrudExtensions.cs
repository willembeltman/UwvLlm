using gAPI.Core.Interfaces;
using UwvLlm.Infrastructure.Data.Mappings;
using UwvLlm.Infrastructure.Data.UseCases;

namespace UwvLlm.Api.Extensions;

public static class AddCrudExtensions
{
    public static IServiceCollection AddCrudUseCases(this IServiceCollection services)
    {
        services.AddScoped<IUseCase<UwvLlm.Infrastructure.Data.Entities.MailMessage, UwvLlm.Shared.Public.Dtos.MailMessage, Guid>, MailMessagesUseCase>();
        services.AddScoped<IUseCase<UwvLlm.Infrastructure.Data.Entities.UserNotification, UwvLlm.Shared.Public.Dtos.UserNotification, long>, UserNotificationsUseCase>();
        services.AddScoped<IUseCase<UwvLlm.Infrastructure.Data.Entities.User, UwvLlm.Shared.Public.Dtos.User, Guid>, UsersUseCase>();
        return services;
    }

    public static IServiceCollection AddCrudMappings(this IServiceCollection services)
    {
        services.AddScoped<Mapping<UwvLlm.Infrastructure.Data.Entities.MailMessage, UwvLlm.Shared.Public.Dtos.MailMessage>, MailMessagesMapping>();
        services.AddScoped<Mapping<UwvLlm.Infrastructure.Data.Entities.UserNotification, UwvLlm.Shared.Public.Dtos.UserNotification>, UserNotificationsMapping>();
        services.AddScoped<Mapping<UwvLlm.Infrastructure.Data.Entities.User, UwvLlm.Shared.Public.Dtos.User>, UsersMapping>();
        return services;
    }
}