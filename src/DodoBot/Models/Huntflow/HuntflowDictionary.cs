using System.Text.Json.Serialization;

namespace DodoBot.Models.Huntflow;

public class HuntflowDictionary
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }
}