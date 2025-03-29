using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using TelegramLibrary.Models;
using TelegramLibrary.Models.Keyboard;

namespace TelegramLibrary;

public class TelegramBot
{
    private readonly string _telegramUrl;
    private readonly string _telegramBotToken;
    private readonly HttpClient _httpClient;

    public TelegramBot(string telegramUrl, string telegramBotToken, HttpClient httpClient)
    {
        _telegramUrl = telegramUrl;
        _telegramBotToken = telegramBotToken;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Получить обновления
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    public async Task<TelegramResponse<Update[]>> GetUpdate(long offset)
    {
        var uri = PrepareUpdateUri(offset);

        var result = await _httpClient.SendAsync(PrepareRequestMessage(uri));
        return await result.Content.ReadFromJsonAsync<TelegramResponse<Update[]>>();
    }

    /// <summary>
    /// Отправить сообщение
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="textMessageFromBot"></param>
    /// <returns></returns>
    public Task SendMessage(long chatId, string textMessageFromBot)
    {
        var messageUri = PrepareMessageUri(chatId, textMessageFromBot);
        return SendHttpMessage(messageUri);
    }

    /// <summary>
    /// Отправить инлайновое сообщение
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="message"></param>
    /// <param name="keyboard"></param>
    /// <returns></returns>
    public Task SendInlineKeyboardMessage(long chatId, string message, InlineKeyboardMarkup keyboard)
    {
        var messageUri = PrepareInlineKeyBoardMessageUri(chatId, message, keyboard);
        return SendHttpMessage(messageUri);
    }

    public Task SendAnswerCallBackQuery(long callBackQuery)
    {
        var messageUri = PrepareAnswerCallBackQuery(callBackQuery);
        return SendHttpMessage(messageUri);
    }

    public Task ChangeInlineKeyBoard(long chatId, long messageId, InlineKeyboardMarkup keyboard)
    {
        var messageUri = PrepareEditReplyMarkupMessage(chatId, messageId, keyboard);
        return SendHttpMessage(messageUri);
    }

    public Task ChangeTextMessage(long chatId, long messageId, string newTextMessage)
    {
        var messageUri = PrepareEditMessage(chatId, messageId, newTextMessage);
        return SendHttpMessage(messageUri);
    }

    public Task ChangeAllOnMessage(long chatId, long messageId, string newTextMessage, InlineKeyboardMarkup keyboard)
    {
        var messageUri = PrepareEditMessageAndKeyBoard(chatId, messageId, newTextMessage, keyboard);
        return SendHttpMessage(messageUri);
    }

    public Task DeleteMessage(long chatId, long messageId)
    {
        var messageUri = PrepareDeleteMessage(chatId, messageId);
        return SendHttpMessage(messageUri);
    }

    private async Task SendHttpMessage(string messageUri)
    {
        var result = await _httpClient.SendAsync(PrepareRequestMessage(messageUri));
        await result.Content.ReadAsStringAsync();
    }

    private HttpRequestMessage PrepareRequestMessage(string requestUriString)
    {
        var req = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(requestUriString)
        };

        var userAgent = new ProductInfoHeaderValue("DotNetMegatron", "1.0");

        req.Headers.UserAgent.Add(userAgent);

        return req;
    }

    private string PrepareUpdateUri(long offset)
    {
        var sb = new StringBuilder();
        sb.Append(_telegramUrl);
        sb.Append(_telegramBotToken);
        sb.Append("/getUpdates");
        sb.Append($"?offset={offset}");
        return sb.ToString();
    }

    private string PrepareMessageUri(long chatId, string message)
    {
        var sb = new StringBuilder();
        sb.Append(_telegramUrl);
        sb.Append(_telegramBotToken);
        sb.Append("/sendMessage");
        sb.Append($"?chat_id={chatId}");
        sb.Append($"&text={message}");
        return sb.ToString();
    }

    private string PrepareKeyBoardMessageUri(long chatId, string message, ReplyKeyboardMarkup keyboard)
    {
        var utf8Encoding = new UTF8Encoding();
        var keyboardSerialized = JsonSerializer.Serialize(keyboard);
        var keyboardOnBytes = utf8Encoding.GetBytes(keyboardSerialized);
        var sb = new StringBuilder();
        sb.Append(_telegramUrl);
        sb.Append(_telegramBotToken);
        sb.Append("/sendMessage");
        sb.Append($"?chat_id={chatId}");
        sb.Append($"&text={message}");
        sb.Append($"&reply_markup={HttpUtility.UrlEncode(keyboardOnBytes)}");
        return sb.ToString();
    }

    private string PrepareInlineKeyBoardMessageUri(long chatId, string message, InlineKeyboardMarkup keyboard)
    {
        var utf8Encoding = new UTF8Encoding();
        var keyboardSerialized = JsonSerializer.Serialize(keyboard);
        var keyboardOnBytes = utf8Encoding.GetBytes(keyboardSerialized);
        var sb = new StringBuilder();
        sb.Append(_telegramUrl);
        sb.Append(_telegramBotToken);
        sb.Append("/sendMessage");
        sb.Append($"?chat_id={chatId}");
        sb.Append($"&text={message}");
        sb.Append($"&reply_markup={HttpUtility.UrlEncode(keyboardOnBytes)}");
        return sb.ToString();
    }

    private string PrepareAnswerCallBackQuery(long callBackQueryId)
    {
        var sb = new StringBuilder();
        sb.Append(_telegramUrl);
        sb.Append(_telegramBotToken);
        sb.Append("/answerCallbackQuery");
        sb.Append($"?callback_query_id={callBackQueryId}");
        return sb.ToString();
    }

    private string PrepareEditReplyMarkupMessage(long chatId, long messageId, InlineKeyboardMarkup keyboard)
    {
        var utf8Encoding = new UTF8Encoding();
        var keyboardSerialized = JsonSerializer.Serialize(keyboard);
        var keyboardOnBytes = utf8Encoding.GetBytes(keyboardSerialized);
        var sb = new StringBuilder();
        sb.Append(_telegramUrl);
        sb.Append(_telegramBotToken);
        sb.Append("/editMessageReplyMarkup");
        sb.Append($"?chat_id={chatId}");
        sb.Append($"&message_id={messageId}");
        sb.Append($"&reply_markup={HttpUtility.UrlEncode(keyboardOnBytes)}");
        return sb.ToString();
    }

    private string PrepareEditMessage(long chatId, long messageId, string newTextOnMessage)
    {
        var sb = new StringBuilder();
        sb.Append(_telegramUrl);
        sb.Append(_telegramBotToken);
        sb.Append("/editMessageText");
        sb.Append($"?chat_id={chatId}");
        sb.Append($"&message_id={messageId}");
        sb.Append($"&text={newTextOnMessage}");
        return sb.ToString();
    }

    private string PrepareEditMessageAndKeyBoard(long chatId, long messageId, string newTextOnMessage,
        InlineKeyboardMarkup keyboard)
    {
        var utf8Encoding = new UTF8Encoding();
        var keyboardSerialized = JsonSerializer.Serialize(keyboard);
        var keyboardOnBytes = utf8Encoding.GetBytes(keyboardSerialized);
        var sb = new StringBuilder();
        sb.Append(_telegramUrl);
        sb.Append(_telegramBotToken);
        sb.Append("/editMessageText");
        sb.Append($"?chat_id={chatId}");
        sb.Append($"&message_id={messageId}");
        sb.Append($"&text={newTextOnMessage}");
        sb.Append($"&reply_markup={HttpUtility.UrlEncode(keyboardOnBytes)}");
        return sb.ToString();
    }

    private string PrepareDeleteMessage(long chatId, long messageId)
    {
        var sb = new StringBuilder();
        sb.Append(_telegramUrl);
        sb.Append(_telegramBotToken);
        sb.Append("/deleteMessage");
        sb.Append($"?chat_id={chatId}");
        sb.Append($"&message_id={messageId}");
        return sb.ToString();
    }
}