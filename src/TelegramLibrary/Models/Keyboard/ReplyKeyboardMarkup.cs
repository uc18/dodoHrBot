using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TelegramLibrary.Models.Keyboard;

public class ReplyKeyboardMarkup
{
    [JsonPropertyName("keyboard")]
    public List<List<KeyboardButton>> Keyboard { get; set; }

    [JsonPropertyName("resize_keyboard")]
    public bool ResizeKeyboard { get; set; } = true;

    [JsonPropertyName("one_time_keyboard")]
    public bool OneTimeKeyboard { get; set; } = true;
}