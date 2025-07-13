using DodoBot.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;

namespace DodoBot.Extensions;

public static class ServicesExtension
{
    public static void AddBackgroundsService(this IServiceCollection services)
    {
        services.AddHostedService<BackgroundTelegramMessageProcessor>();
        services.AddHostedService<BackgroundNotifyVacancyProcessor>();
    }
}