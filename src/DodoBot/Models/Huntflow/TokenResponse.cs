using System.Text.Json.Serialization;

namespace DodoBot.Models.Huntflow;

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int AccessTokenExpiration { get; set; }

    [JsonPropertyName("refresh_token_expires_in")]
    public int RefreshTokenExpiration { get; set; }
}