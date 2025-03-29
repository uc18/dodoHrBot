using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TelegramLibrary.Models.Keyboard;

public class InlineKeyboardMarkup
{
    [JsonPropertyName("inline_keyboard")]
    public List<List<InlineKeyboardButton>> Keyboard { get; set; }
}