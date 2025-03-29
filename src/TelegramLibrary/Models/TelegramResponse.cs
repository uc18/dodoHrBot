using System.Text.Json.Serialization;

namespace TelegramLibrary.Models;

public class TelegramResponse<T>
{
    [JsonPropertyName("ok")]
    public bool IsOk { get; set; }

    [JsonPropertyName("result")]
    public T? Result { get; set; }
}