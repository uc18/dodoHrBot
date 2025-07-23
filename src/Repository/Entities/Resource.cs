using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Entities;

[Table("Resources")]
public class Resource
{
    [Column("Id")]
    public string Id { get; set; }

    [Column("Name")]
    public string Name { get; set; }

    [Column("Url")]
    public string Url { get; set; }
}