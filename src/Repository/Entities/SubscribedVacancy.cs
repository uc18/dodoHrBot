using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Entities;

[Table("SubscribedVacancy")]
public class SubscribedVacancy
{
    [Column("Id")]
    public string Id { get; set; }

    [Column("User_Id")]
    public string UserId { get; set; }

    [Column("Speciality_Id")]
    public int? SpecialtyId { get; set; }

    [Column("Subspecialty_Id")]
    public int? SubspecialtyId { get; set; }

    public Candidate Candidates { get; set; }
}