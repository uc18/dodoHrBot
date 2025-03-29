using System.Text.Json.Serialization;

namespace TelegramLibrary.Models;

public class Message
{
    [JsonPropertyName("message_id")]
    public long MessageId { get; set; }

    [JsonPropertyName("from")]
    public User From { get; set; }

    [JsonPropertyName("sender_chat")]
    public Chat SenderChat { get; set; }

    [JsonPropertyName("date")]
    public long Date { get; set; }

    [JsonPropertyName("chat")]
    public Chat Chat { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}