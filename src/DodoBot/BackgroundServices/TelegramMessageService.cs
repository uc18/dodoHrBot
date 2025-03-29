using DodoBot.Services;

namespace DodoBot.BackgroundServices;

public class TelegramMessageService : BackgroundService
{
    private readonly IServiceProvider _provider;
    private long _offset;

    public TelegramMessageService(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            await DoAsync(stoppingToken);
        }
    }

    private async Task DoAsync(CancellationToken stoppingToken)
    {
        var scope = _provider.CreateScope();
        var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();

        var update = await messageService.CheckNewUpdate(_offset);

        if (update != null)
        {
            _offset = update.UpdateId;

            if (update.Message != null)
            {
                messageService.SendMessageUser(update.Message.Chat.Id, "Используй кнопки");
            }
        }
    }
}