using System.Threading.Tasks;
using DodoBot.Models.Huntflow.Response;

namespace DodoBot.Interfaces.Services;

public interface IHuntflowApi
{
    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    Task<DictionaryResponse?> GetDodoStreamAsync();

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    Task<DictionaryResponse?> GetDodoSubSpecialtyAsync();

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    Task<VacancyResponse?> GetVacanciesAsync(int page);
}