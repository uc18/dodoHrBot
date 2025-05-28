using System;
using System.Threading;
using System.Threading.Tasks;
using DodoBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DodoBot.BackgroundServices;

public class BackgroundTelegramMessageProcessor : BackgroundService
{
    private readonly IServiceProvider _provider;
    private long _offset;

    public BackgroundTelegramMessageProcessor(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1000), stoppingToken);
            await DoAsync(stoppingToken);
        }
    }

    private async Task DoAsync(CancellationToken stoppingToken)
    {
        var scope = _provider.CreateScope();
        var messageService = scope.ServiceProvider.GetRequiredService<DodoBotMessageService>();
        var answerService = scope.ServiceProvider.GetRequiredService<AnswerBotService>();

        var update = await messageService.CheckNewUpdate(_offset);

        if (update != null)
        {
            _offset = update.UpdateId;

            if (update.Message != null)
            {
                await answerService.ProcessTextMessage(update.Message);
            }

            if (update.CallbackQuery != null)
            {
                await answerService.ProcessCallbackMessage(update.CallbackQuery);
            }
        }
    }
}