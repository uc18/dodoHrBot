using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodoBot.Constants;
using DodoBot.Interfaces;
using DodoBot.Interfaces.Repositories;
using DodoBot.Interfaces.Services;
using DodoBot.Models.Dto;
using DodoBot.Models.Huntflow;
using DodoBot.Providers;
using Repository.Entities;

namespace DodoBot.Services;

public class NotifyService(
    HuntflowService service,
    ISupabaseRepository supabaseRepository,
    DodoBotMessageService dodoBotMessageService,
    ButtonProvider buttonProvider)
    : INotifyService
{
    public async Task SendNotifyAllUsers()
    {
        var allVacancies = await service.GetAllVacancies();
        var postedVacancy = await supabaseRepository.ReadExistsVacancy();

        var vacancicesByList = allVacancies.ToList();
        if (vacancicesByList.Count > 0)
        {
            var staffVacancies = new List<HuntflowVacancy>();

            foreach (var newVac in vacancicesByList)
            {
                if (postedVacancy.FirstOrDefault(t => t.VacancyId == newVac.Id) == null &&
                    newVac.CareerPublication == 46)
                {
                    staffVacancies.Add(newVac);
                }
            }

            var vacanciesToSend = new List<VacancyToSend>();
            foreach (var vacancy in staffVacancies)
            {
                var existsVacancy =
                    vacanciesToSend.FirstOrDefault(t =>
                        t.SpecialtyId == vacancy.Speciality && t.SubSpecialityId == vacancy.SubSpeciality);

                if (existsVacancy != null)
                {
                    existsVacancy.VacancyInfos.Add(new VacancyInfo
                    {
                        PositionName = vacancy.Position,
                        PositionId = vacancy.Id
                    });
                }
                else
                {
                    var vacancyToSend = new VacancyToSend
                    {
                        SpecialtyId = vacancy.Speciality,
                        SubSpecialityId = vacancy.SubSpeciality
                    };

                    vacancyToSend.VacancyInfos.Add(new VacancyInfo
                    {
                        PositionName = vacancy.Position,
                        PositionId = vacancy.Id
                    });

                    vacanciesToSend.Add(vacancyToSend);
                }
            }

            var allUsers = await supabaseRepository.GetAllEnabledUsers();
            var usersWithVacancy = new Dictionary<string, List<VacancyToSend>>();

            foreach (var user in allUsers)
            {
                if (usersWithVacancy.TryGetValue(user.UserId, out var vacanciesToSendByUser))
                {
                    var vacancies = vacanciesToSend
                        .Where(t => t.SpecialtyId == user.SpecialtyId && t.SubSpecialityId == user.SubspecialtyId)
                        .ToList();

                    if (vacancies.Count > 0)
                    {
                        vacanciesToSendByUser.AddRange(vacancies);
                    }
                }
                else
                {
                    var vacancies = vacanciesToSend
                        .Where(t => t.SpecialtyId == user.SpecialtyId && t.SubSpecialityId == user.SubspecialtyId)
                        .ToList();

                    if (vacancies.Count > 0)
                    {
                        usersWithVacancy.TryAdd(user.UserId, vacancies);
                    }
                }
            }

            if (usersWithVacancy.Count > 0)
            {
                var startButton = buttonProvider.GetFrequencyButton();
                var perhapsPostedVacancies = new List<int>();
                foreach (var userInfo in usersWithVacancy)
                {
                    var sb = new StringBuilder();
                    var telegramId = await supabaseRepository.GetTelegramUserId(userInfo.Key);

                    if (telegramId.HasValue)
                    {
                        foreach (var vacancysToSend in userInfo.Value)
                        {
                            foreach (var vacancy in vacancysToSend.VacancyInfos)
                            {
                                if (vacancy.PositionName.Contains("&"))
                                {
                                    sb.AppendLine($"Позиция: {vacancy.PositionName.Replace("&", "%26")}");
                                }
                                else
                                {
                                    sb.AppendLine($"Позиция: {vacancy.PositionName}");
                                }

                                sb.AppendLine($"Подробнее: {StaffConstants.DodoUrl}{vacancy.PositionId}");
                                sb.AppendLine("");
                                perhapsPostedVacancies.Add(vacancy.PositionId);
                            }
                        }

                        var result = sb.ToString();

                        await dodoBotMessageService.SendInlineMessage(telegramId.Value, result, startButton);
                    }
                }

                var distinctVacancyId = perhapsPostedVacancies.Distinct().ToList();

                await supabaseRepository.WriteNewVacancy(distinctVacancyId.Select(t => new Vacancy
                {
                    Id = Guid.NewGuid().ToString(),
                    VacancyId = t
                }).ToList());
            }
        }
    }
}