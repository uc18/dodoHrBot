using System.Collections.Generic;
using System.Threading.Tasks;
using DodoBot.Interfaces;
using DodoBot.Models.Huntflow;

namespace DodoBot.Services;

public class HuntflowService
{
    private readonly IHuntflowApi _huntflowApi;

    public HuntflowService(IHuntflowApi huntflowApi)
    {
        _huntflowApi = huntflowApi;
    }

    public async Task<IEnumerable<HuntflowVacancy>> GetAllVacancies()
    {
        var pages = 1;
        var totalPages = 0;
        var notSortedVacancy = await _huntflowApi.GetVacanciesAsync(pages);

        var allVacancies = new List<HuntflowVacancy>();

        if (notSortedVacancy != null)
        {
            allVacancies.AddRange(notSortedVacancy.Vacancies);
            if (notSortedVacancy.TotalPages > pages)
            {
                do
                {
                    pages++;
                    var addingVacancy = await _huntflowApi.GetVacanciesAsync(pages);

                    if (addingVacancy != null)
                    {
                        allVacancies.AddRange(addingVacancy.Vacancies);
                        totalPages = addingVacancy.TotalPages;
                    }
                } while (totalPages > pages);
            }

            return allVacancies;
        }

        return [];
    }
}