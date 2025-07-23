using System.Text.Json.Serialization;

namespace DodoBot.Models.Huntflow.Response;

public class VacancyResponse
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("total_items")]
    public int TotalItems { get; set; }

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("items")]
    public HuntflowVacancy[] Vacancies { get; set; }
}