using System.Text.Json.Serialization;

namespace DodoBot.Models.Huntflow;

public class HuntflowVacancy
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("money")]
    public string? Money { get; set; }

    [JsonPropertyName("position")]
    public string? Position { get; set; }

    [JsonPropertyName("grade")]
    public int[] Grade { get; set; }

    [JsonPropertyName("work_format")]
    public int[] WorkFormat { get; set; }

    [JsonPropertyName("career_publication")]
    public int? CareerPublication { get; set; }

    [JsonPropertyName("vacancy_city")]
    public int[] VacancyCity { get; set; }

    [JsonPropertyName("speciality")]
    public int? Speciality { get; set; }

    [JsonPropertyName("subspeciality")]
    public int? SubSpeciality { get; set; }
}