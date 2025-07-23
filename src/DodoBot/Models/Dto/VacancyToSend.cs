using System.Collections.Generic;

namespace DodoBot.Models.Dto;

public class VacancyToSend
{
    public int? SpecialtyId { get; set; }

    public int? SubSpecialityId { get; set; }

    public List<VacancyInfo> VacancyInfos { get; set; } =
        new List<VacancyInfo>();
}