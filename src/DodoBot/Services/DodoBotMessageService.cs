using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DodoBot.Options;
using Microsoft.Extensions.Options;
using TelegramLibrary;
using TelegramLibrary.Models;
using TelegramLibrary.Models.Keyboard;

namespace DodoBot.Services;

public class DodoBotMessageService
{
    private readonly TelegramBot _bot;

    public DodoBotMessageService(HttpClient httpClient, IOptions<ApplicationOptions> options)
    {
        _bot = new TelegramBot(options.Value.TelegramApiUrl, options.Value.TelegramBotToken, httpClient);
    }

    public async Task SendMessageToUser(long chatUserId, string textMessage)
    {
        await _bot.SendMessage(chatUserId, textMessage);
    }

    public async Task SendInlineMessage(long chatUserId, string textMessage, InlineKeyboardMarkup keyboard)
    {
        await _bot.SendInlineKeyboardMessage(chatUserId, textMessage, keyboard);
    }

    public async Task ChangeAllContentInMessage(long chatUserId, long messageId, string newTextOnMessage,
        InlineKeyboardMarkup keyboard)
    {
        await _bot.ChangeAllOnMessage(chatUserId, messageId, newTextOnMessage, keyboard);
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