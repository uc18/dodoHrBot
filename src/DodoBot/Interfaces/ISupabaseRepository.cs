using System.Collections.Generic;
using System.Threading.Tasks;
using DodoBot.Models;
using Repository.Entities;

namespace DodoBot.Interfaces;

public interface ISupabaseRepository
{
    /// <summary>
    /// Добавить нового пользователя
    /// </summary>
    /// <param name="candidate"></param>
    /// <returns></returns>
    Task<string> AddNewUser(СandidateInfo candidate);

    /// <summary>
    /// Вернуть количество пользовательских подписок
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<int> CountUserVacancySubscribe(string userId);

    /// <summary>
    /// Вернуть ресурсы Додо
    /// </summary>
    /// <returns></returns>
    Task<List<ResourceDto>> GetResourcesDodo();

    /// <summary>
    /// Получить пользовательский идентификатор
    /// </summary>
    /// <param name="telegramId"></param>
    /// <returns></returns>
    Task<string> GetInternalUserId(long telegramId);

    /// <summary>
    /// Получить коллекцию пользовательских подписок в бизнесе
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<HashSet<int>> GetUserSpecialty(string userId);

    /// <summary>
    /// Получить коллекцию пользовательских подписок в it
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<HashSet<int>> GetUserSubSpecialty(string userId);

    /// <summary>
    /// Записать подписку в бизнесе
    /// </summary>
    /// <param name="specialtyId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<bool> WriteSpecialty(int specialtyId, string userId);

    /// <summary>
    /// Отписаться от подписки на бизнес
    /// </summary>
    /// <param name="specialtyId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<bool> RemoveSpecialty(int specialtyId, string userId);

    /// <summary>
    /// Подписаться на вакансии в it
    /// </summary>
    /// <param name="specialtyId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<bool> WriteSubSpecialty(int specialtyId, string userId);

    /// <summary>
    /// Отписаться от вакансий в it
    /// </summary>
    /// <param name="specialtyId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<bool> RemoveSubSpecialty(int specialtyId, string userId);

    /// <summary>
    ///
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<PeriodicitySettings?> ReadUserSubscribeOptions(string userId);

    /// <summary>
    ///
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="setting"></param>
    /// <returns></returns>
    Task<PeriodicitySettings?> WriteReadUserSubscribe(string userId, PeriodicitySettings setting);

    /// <summary>
    ///
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<bool> UpdateExistUser(string userId);

    /// <summary>
    ///
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<CandidateSpecialty?> GetUserSubscription(string userId);

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    Task<List<Vacancy>> ReadExistsVacancy();

    /// <summary>
    ///
    /// </summary>
    /// <param name="postedVacancy"></param>
    /// <returns></returns>
    Task WriteNewVacancy(List<Vacancy> postedVacancy);

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    Task<List<SubscribedVacancy>> GetAllUser();

    /// <summary>
    ///
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<long?> GetTelegramUserId(string userId);
}