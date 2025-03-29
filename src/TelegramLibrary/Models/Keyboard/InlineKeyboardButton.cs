using System.Text.Json.Serialization;

namespace TelegramLibrary.Models.Keyboard;

public class InlineKeyboardButton
{
    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("callback_data")]
    public string CallbackData { get; set; }
}