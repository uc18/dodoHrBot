using System.Text.Json.Serialization;

namespace TelegramLibrary.Models;

public record Update
{
    [JsonPropertyName("update_id")]
    public long UpdateId { get; set; }

    [JsonPropertyName("message")]
    public Message Message { get; set; }

    [JsonPropertyName("callback_query")]
    public CallbackQuery CallbackQuery { get; set; }
}