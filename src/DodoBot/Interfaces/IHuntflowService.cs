using System.Threading.Tasks;
using DodoBot.Models.Huntflow.Response;

namespace DodoBot.Interfaces;

public interface IHuntflowService
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

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    Task<DictionaryResponse?> GetWorkFormatAsync();

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    Task<DictionaryResponse?> GetCityResponseAsync();

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    Task<DictionaryResponse?> GetGradeResponseAsync();
}