using System.Text.Json.Serialization;

namespace TelegramLibrary.Models;

public class User
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("is_bot")]
    public bool IsBot { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("user_name")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("can_join_groups")]
    public bool CanJoinGroups { get; set; }

    [JsonPropertyName("can_read_all_group_messages")]
    public bool CanReadAllGroupMessages { get; set; }

    [JsonPropertyName("supports_inline_queries")]
    public bool SupportsInlineQueries { get; set; }
}