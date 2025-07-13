using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Entities;

[Table("Vacancies")]
public class Vacancy
{
    [Column("Id")]
    public string Id { get; set; }

    [Column("VacancyId")]
    public int VacancyId { get; set; }
}