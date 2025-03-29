using DodoBot.Options;
using Microsoft.Extensions.Options;
using TelegramLibrary;
using TelegramLibrary.Models;

namespace DodoBot.Services;

public class MessageService
{
    private readonly TelegramBot _bot;

    public MessageService(HttpClient httpClient, IOptions<ApplicationOptions> options)
    {
        _bot = new TelegramBot(options.Value.TelegramApiUrl, options.Value.TelegramBotToken, httpClient);
    }

    public void SendMessageUser(long chatUserId, string textMessage)
    {
        _bot.SendMessage(chatUserId, textMessage);
    }

    public async Task<Update?> CheckNewUpdate(long offset)
    {
        var newUpdate = await _bot.GetUpdate(offset + 1);

        if (newUpdate.IsOk && newUpdate.Result != null)
        {
            var updateResult = newUpdate.Result.FirstOrDefault();

            if (updateResult != null)
            {
                return updateResult;
            }
        }

        return null;
    }
}