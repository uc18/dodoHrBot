using System.Text.Json.Serialization;

namespace DodoBot.Models.Huntflow;

public class TokenRequest
{
    [JsonPropertyName("refresh_token")]
    public string Token { get; set; }
}