using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Entities;

[Table("Candidates")]
public class Candidate
{
    [Column("id")]
    public string Id { get; set; }

    [Column("Telegram_Id")]
    public long TelegramId { get; set; }

    [Column("First_name")]
    public string FirstName { get; set; }

    [Column("Last_name")]
    public string LastName { get; set; }

    [Column("Speciality_name")]
    public string SpecialityName { get; set; }
}