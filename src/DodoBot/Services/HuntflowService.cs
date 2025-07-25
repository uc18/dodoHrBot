using System.Collections.Generic;
using System.Threading.Tasks;
using DodoBot.Interfaces;
using DodoBot.Interfaces.Integrations;
using DodoBot.Models.Huntflow;

namespace DodoBot.Services;

public class HuntflowService(IHuntflowApi huntflowApi)
{
    public async Task<IEnumerable<HuntflowVacancy>> GetAllVacancies()
    {
        var pages = 1;
        var totalPages = 0;
        var notSortedVacancy = await huntflowApi.GetVacanciesAsync(pages);

        var allVacancies = new List<HuntflowVacancy>();

        if (notSortedVacancy != null)
        {
            allVacancies.AddRange(notSortedVacancy.Vacancies);
            if (notSortedVacancy.TotalPages > pages)
            {
                do
                {
                    pages++;
                    var addingVacancy = await huntflowApi.GetVacanciesAsync(pages);

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