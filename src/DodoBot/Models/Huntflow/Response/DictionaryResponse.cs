using System.Text.Json.Serialization;

namespace DodoBot.Models.Huntflow.Response;

public class DictionaryResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("fields")]
    public HuntflowDictionary[] Fields { get; set; }
}