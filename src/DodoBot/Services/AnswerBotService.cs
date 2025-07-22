using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DodoBot.Constants;
using DodoBot.Extensions;
using DodoBot.Interfaces;
using DodoBot.Interfaces.Integrations;
using DodoBot.Interfaces.Repositories;
using DodoBot.Models;
using DodoBot.Options;
using DodoBot.Providers;
using Microsoft.Extensions.Options;
using Repository.Entities;
using TelegramLibrary.Models;

namespace DodoBot.Services;

public class AnswerBotService(
    DodoBotMessageService messageService,
    ButtonProvider buttonProvider,
    IHuntflowApi huntflowApi,
    ISupabaseRepository supabaseRepository,
    IOptions<ApplicationOptions> options,
    HuntflowService service)
{
    public async Task ProcessTextMessage(Message update)
    {
        if (update.Text.StartsWith("/"))
        {
            var message = update.Text.Remove(0, 1);
            switch (message.ToLower())
            {
                case "start":
                {
                    var internalUserId = await supabaseRepository.AddNewUser(new СandidateInfo
                    {
                        FirstName = update.Chat.FirstName,
                        LastName = update.Chat.LastName,
                        TelegramId = update.Chat.Id
                    });

                    if (internalUserId.Length > 0)
                    {
                        var countedSubscribe = await supabaseRepository.CountUserVacancySubscribe(internalUserId);
                        var button = countedSubscribe > 0
                            ? buttonProvider.MainMenuButtons(countedSubscribe, options.Value.PrivacyPolicyUrl)
                            : buttonProvider.MainMenuButtons(options.Value.PrivacyPolicyUrl);

                        await messageService.SendInlineMessage(update.Chat.Id, StaffConstants.BotAnswer, button);
                    }

                    break;
                }
                case "about":
                {
                    var resources = await supabaseRepository.GetResourcesDodo();
                    var resourcesButtons = buttonProvider.DodoResourcesButton(resources);

                    await messageService.SendInlineMessage(update.Chat.Id, StaffConstants.BotAnswer, resourcesButtons);
                    break;
                }
                case "setting":
                {
                    var userId = await supabaseRepository.GetInternalUserId(update.Chat.Id);
                    if (userId.Length > 0)
                    {
                        var countedSubscribe = await supabaseRepository.CountUserVacancySubscribe(userId);

                        if (countedSubscribe > 0)
                        {
                            var setting = await supabaseRepository.ReadUserSubscribeOptions(userId);
                            var frequencyButton =
                                buttonProvider.ButtonsFrequencySetting(setting);
                            await messageService.SendInlineMessage(update.Chat.Id,
                                StaffConstants.SetEnableOrDisableNotify, frequencyButton);
                        }
                    }
                    break;
                }
                case "legal":
                {
                    var resources = new List<ResourceDto>
                    {
                        new ResourceDto
                        {
                            Name = "Пользовательское соглашение",
                            Url = options.Value.PrivacyPolicyUrl
                        }
                    };
                    var legalButton = buttonProvider.DodoResourcesButton(resources);

                    await messageService.SendInlineMessage(update.Chat.Id, StaffConstants.LegalAnswer, legalButton);
                    break;
                }
                default:
                {
                    await messageService.SendMessageToUser(update.Chat.Id, StaffConstants.DontUnderstandYouUseButton);
                    break;
                }
            }
        }
        else
        {
            await messageService.SendMessageToUser(update.Chat.Id, StaffConstants.DontUnderstandYouUseButton);
        }
    }

    public async Task ProcessCallbackMessage(CallbackQuery callbackQuery)
    {
        var sp = callbackQuery.Data.Split("-");
        switch (sp[0].ToLower())
        {
            case "start":
            {
                var userId = await supabaseRepository.GetInternalUserId(callbackQuery.User.Id);

                var countedSubscribe = await supabaseRepository.CountUserVacancySubscribe(userId);
                var button = countedSubscribe > 0
                    ? buttonProvider.MainMenuButtons(countedSubscribe, options.Value.PrivacyPolicyUrl)
                    : buttonProvider.MainMenuButtons(options.Value.PrivacyPolicyUrl);

                await messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, StaffConstants.BotAnswer, button);
                break;
            }
            case "viewvacancy":
            case "mysubscription":
            {
                var userId = await supabaseRepository.GetInternalUserId(callbackQuery.User.Id);
                var it = await supabaseRepository.GetUserSubSpecialty(userId);
                var bussiness = await supabaseRepository.GetUserSpecialty(userId);

                var itAndBusiness = buttonProvider.EngineeringAndBusiness(it.Count, bussiness.Count);

                await messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, StaffConstants.WellSet, itAndBusiness);

                break;
            }
            case "sendvacancies":
            {
                const int dodoEngineering = 55;
                const int activePublication = 46;
                var mainMenu = buttonProvider.GetMainMenu();
                var userId = await supabaseRepository.GetInternalUserId(callbackQuery.User.Id);
                var userInfo = await supabaseRepository.GetUserSubscription(userId);
                var allOpenVacancies = await service.GetAllVacancies();

                var finalVacancies = new List<VacancyDto>();
                var staffVacancies = allOpenVacancies
                    .Where(t => t.CareerPublication == activePublication)
                    .ToList();

                if (staffVacancies.Any() && userInfo != null)
                {
                    foreach (var staffVacancy in staffVacancies)
                    {
                        if (staffVacancy.Speciality.HasValue)
                        {
                            if (staffVacancy.Speciality == dodoEngineering &&
                                userInfo.Specialty.Contains(staffVacancy.Speciality.Value))
                            {
                                if (staffVacancy.SubSpeciality.HasValue)
                                {
                                    if (userInfo.SubSpecialty.Contains(staffVacancy.SubSpeciality.Value))
                                    {
                                        finalVacancies.Add(new VacancyDto
                                        {
                                            Id = staffVacancy.Id,
                                            Position = staffVacancy.Position ?? "текст позиции не найден",
                                            Url = $"Подробнее: {StaffConstants.DodoUrl}{staffVacancy.Id}"
                                        });
                                        continue;
                                    }
                                }
                            }

                            if (staffVacancy.Speciality != dodoEngineering &&
                                userInfo.Specialty.Contains(staffVacancy.Speciality.Value))
                            {
                                finalVacancies.Add(new VacancyDto
                                {
                                    Id = staffVacancy.Id,
                                    Position = staffVacancy.Position ?? "текст позиции не найден",
                                    Url = $"Подробнее: {StaffConstants.DodoUrl}{staffVacancy.Id}"
                                });
                            }
                        }
                    }

                    var vacancyText = VacancyExtension.PrepareVacancyText(finalVacancies);

                    await messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                        callbackQuery.Message.MessageId, vacancyText, mainMenu);
                }
                else
                {
                    await messageService.SendMessageToUser(callbackQuery.Message.Chat.Id,
                        StaffConstants.VacanciesNotFound);
                }

                break;
            }
            case "it":
            {
                var subSpecialtyResponse = await huntflowApi.GetDodoSubSpecialtyAsync();
                if (subSpecialtyResponse != null)
                {
                    var userId = await supabaseRepository.GetInternalUserId(callbackQuery.User.Id);
                    var userSubSpecialty = await supabaseRepository.GetUserSubSpecialty(userId);

                    var sortedSubSpecialties = subSpecialtyResponse
                        .Fields
                        .Where(t => t.Active)
                        .ToList();

                    var buttons =
                        buttonProvider.GetDodoStreamButtons(sortedSubSpecialties, "subspecialty", userSubSpecialty);

                    await messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                        callbackQuery.Message.MessageId, StaffConstants.OneOrMore, buttons);
                }

                break;
            }
            case "business":
            {
                var dodoStreams = await huntflowApi.GetDodoStreamAsync();
                if (dodoStreams != null)
                {
                    var userId = await supabaseRepository.GetInternalUserId(callbackQuery.User.Id);
                    var userSpecialty = await supabaseRepository.GetUserSpecialty(userId);

                    var sortedStreams = dodoStreams
                        .Fields
                        .Where(t => t.Active && t.Id != 55)
                        .ToList();

                    var buttons = buttonProvider.GetDodoStreamButtons(sortedStreams, "specialty", userSpecialty);
                    await messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                        callbackQuery.Message.MessageId, StaffConstants.OneOrMore, buttons);
                }
                else
                {
                    await messageService.SendMessageToUser(callbackQuery.Message.Chat.Id, StaffConstants.DontWork);
                }
                break;
            }

            case "specialty":
            {
                var dodoStreams = await huntflowApi.GetDodoStreamAsync();
                var userId = await supabaseRepository.GetInternalUserId(callbackQuery.User.Id);

                if (int.TryParse(sp[1], out var specialtyId))
                {
                    if (userId.Length > 0)
                    {
                        var previousSpecialty = await supabaseRepository.GetUserSpecialty(userId);
                        if (previousSpecialty.Contains(specialtyId))
                        {
                            await supabaseRepository.RemoveSpecialty(specialtyId, userId);
                        }
                        else
                        {
                            await supabaseRepository.WriteSpecialty(specialtyId, userId);
                        }
                    }

                    if (dodoStreams != null)
                    {
                        var newSpecialty = await supabaseRepository.GetUserSpecialty(userId);
                        var sortedStreams = dodoStreams
                            .Fields
                            .Where(t => t.Active && t.Id != 55)
                            .ToList();
                        var buttons = buttonProvider.GetDodoStreamButtons(sortedStreams, "specialty", newSpecialty);

                        await messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                            callbackQuery.Message.MessageId, StaffConstants.OneOrMore,
                            buttons);
                    }
                }

                break;
            }
            case "subspecialty":
            {
                var subSpecialtyResponse = await huntflowApi.GetDodoSubSpecialtyAsync();
                var userId = await supabaseRepository.GetInternalUserId(callbackQuery.User.Id);

                if (int.TryParse(sp[1], out var subsSpecialtyId))
                {
                    if (userId.Length > 0)
                    {
                        var previousSubSpecialty = await supabaseRepository.GetUserSubSpecialty(userId);
                        if (previousSubSpecialty.Contains(subsSpecialtyId))
                        {
                            await supabaseRepository.RemoveSubSpecialty(subsSpecialtyId, userId);
                        }
                        else
                        {
                            await supabaseRepository.WriteSubSpecialty(subsSpecialtyId, userId);
                        }
                    }

                    if (subSpecialtyResponse != null)
                    {
                        var newSubSpecialty = await supabaseRepository.GetUserSubSpecialty(userId);
                        var sortedStreams = subSpecialtyResponse
                            .Fields
                            .Where(t => t.Active && t.Id != 55)
                            .ToList();
                        var buttons =
                            buttonProvider.GetDodoStreamButtons(sortedStreams, "subspecialty", newSubSpecialty);
                        await messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                            callbackQuery.Message.MessageId, StaffConstants.OneOrMore,
                            buttons);
                    }
                }

                break;
            }
            case "support":
            {
                await messageService.SendMessageToUser(callbackQuery.Message.Chat.Id,
                    "Напишите на hr-team@dodobrands.io");
                break;
            }
            case "viewcompany":
            {
                var resources = await supabaseRepository.GetResourcesDodo();
                var resourcesButtons = buttonProvider.DodoResourcesButton(resources);

                await messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, "Вот несколько мест, где ты можешь узнать о нас больше:",
                    resourcesButtons);

                break;
            }
            case "back":
            {
                var userId = await supabaseRepository.GetInternalUserId(callbackQuery.User.Id);
                var it = await supabaseRepository.GetUserSubSpecialty(userId);
                var bussiness = await supabaseRepository.GetUserSpecialty(userId);

                var itAndBusiness = buttonProvider.EngineeringAndBusiness(it.Count, bussiness.Count);
                await messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, StaffConstants.WellSet, itAndBusiness);
                break;
            }
            case "set":
            {
                var userId = await supabaseRepository.GetInternalUserId(callbackQuery.User.Id);

                var userInfo = await supabaseRepository.GetUserSubscription(userId);
                var specialtyInfo = await huntflowApi.GetDodoStreamAsync();
                var subSpecialtyInfo = await huntflowApi.GetDodoSubSpecialtyAsync();

                var exists = new List<string>();
                if (specialtyInfo != null && userInfo != null)
                {
                    var specialty = specialtyInfo
                        .Fields
                        .Where(t => t.Active &&
                                    t.Id != 55
                                    && userInfo.Specialty.Contains(t.Id))
                        .Select(t => t.Name);
                    exists.AddRange(specialty);
                }

                if (subSpecialtyInfo != null && userInfo != null)
                {
                    var subSpecialty = subSpecialtyInfo
                        .Fields
                        .Where(t => t.Active
                                    && userInfo.SubSpecialty.Contains(t.Id))
                        .Select(t => t.Name);

                    exists.AddRange(subSpecialty);
                }

                var vacancy = "Нет подписок";
                if (exists.Count > 0)
                {
                    vacancy = exists.BuildCommaString();
                }

                await messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, vacancy, buttonProvider.GetFrequencyButton());

                break;
            }
            case "changefrequency":
            {
                var userId = await supabaseRepository.GetInternalUserId(callbackQuery.User.Id);

                if (userId.Length > 0)
                {
                    if (int.TryParse(sp[1], out var setting))
                    {
                        var newSettings = setting.ConvertIntoEnum();

                        if (newSettings.HasValue)
                        {
                            var result = await supabaseRepository.WriteReadUserSubscribe(userId, newSettings.Value);

                            var timingButton = buttonProvider.ButtonsFrequencySetting(result);

                            var textFrequency = newSettings.Value == PeriodicitySettings.Enable
                                ? StaffConstants.YouAreEnableNotify
                                : StaffConstants.YouAreDisableNotify;

                            await messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                                callbackQuery.Message.MessageId,
                                textFrequency, timingButton);
                        }
                    }
                }

                break;
            }
            case "frequency":
            {
                var userId = await supabaseRepository.GetInternalUserId(callbackQuery.User.Id);

                if (userId.Length > 0)
                {
                    var result = await supabaseRepository.ReadUserSubscribeOptions(userId);

                    if (result.HasValue)
                    {
                        var timingButton = buttonProvider.ButtonsFrequencySetting(result);
                        await messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                            callbackQuery.Message.MessageId, StaffConstants.SetEnableOrDisableNotify, timingButton);
                    }
                    else
                    {
                        result = await supabaseRepository.WriteReadUserSubscribe(userId,
                            PeriodicitySettings.Enable);

                        var timingButton = buttonProvider.ButtonsFrequencySetting(result);
                        await messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                            callbackQuery.Message.MessageId, StaffConstants.SetEnableOrDisableNotify, timingButton);
                    }
                }

                break;
            }
            default:
            {
                var backMainMenuButton = buttonProvider.GetMainMenu();
                await messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, "Неизвестная кнопка",
                    backMainMenuButton);
                break;
            }
        }
    }
}