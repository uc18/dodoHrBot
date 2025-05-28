namespace DodoBot.Models;

public class VacancyDto
{
    public int Id { get; set; }

    public string Money { get; set; }

    public string Position { get; set; }

    public string VacancyCity { get; set; }

    public string? Speciality { get; set; }

    public string? SubSpeciality { get; set; }

    public string Grade { get; set; }

    public string WorkFormat { get; set; }
}