using System.Text.Json.Serialization;

namespace TelegramLibrary.Models.Keyboard;

public class KeyboardButton
{
    [JsonPropertyName("text")]
    public string Text { get; set; }
}