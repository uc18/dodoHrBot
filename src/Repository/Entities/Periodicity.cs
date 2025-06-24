using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Entities;

[Table("Periodicity")]
public class Periodicity
{
    [Column("id")]
    public string Id { get; set; }

    [Column("User_Id")]
    public string UserId { get; set; }

    [Column("Periodicity_Id")]
    public PeriodicitySettings Settings { get; set; }

    [Column("StartNotify")]
    public DateTime StartNotify { get; set; }

    public Candidate Candidate { get; set; }
}