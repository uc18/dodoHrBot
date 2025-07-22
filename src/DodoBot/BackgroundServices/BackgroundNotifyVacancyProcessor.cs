using System;
using System.Threading;
using System.Threading.Tasks;
using DodoBot.Interfaces;
using DodoBot.Interfaces.Services;
using DodoBot.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DodoBot.BackgroundServices;

public class BackgroundNotifyVacancyProcessor : BackgroundService
{
    private readonly IOptions<ApplicationOptions> _options;

    private readonly ILogger<BackgroundNotifyVacancyProcessor> _logger;

    private readonly IServiceProvider _serviceProvider;

    public BackgroundNotifyVacancyProcessor(IOptions<ApplicationOptions> options,
        ILogger<BackgroundNotifyVacancyProcessor> logger, IServiceProvider serviceProvider)
    {
        _options = options;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var dateNow = DateTime.UtcNow;
            var timeStartBackgroundJob = Convert.ToDateTime(_options.Value.TimeStartBackgroundJob);
            timeStartBackgroundJob = timeStartBackgroundJob.AddDays(1);

            var timeStart = timeStartBackgroundJob - dateNow;
            _logger.Log(LogLevel.Information, $"BackgroundJob: next launch in {timeStart} hours");
            await Task.Delay(timeStart, stoppingToken);

            await DoAsync(stoppingToken);
        }
    }

    private async Task DoAsync(CancellationToken stoppingToken)
    {
        var scope = _serviceProvider.CreateScope();
        var notifyService = scope.ServiceProvider.GetRequiredService<INotifyService>();
        await notifyService.SendNotifyAllUsers();
    }
}