using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DodoBot.Constants;
using DodoBot.Extensions;
using DodoBot.Interfaces;
using DodoBot.Models;
using DodoBot.Options;
using DodoBot.Providers;
using Microsoft.Extensions.Options;
using Repository.Entities;
using TelegramLibrary.Models;

namespace DodoBot.Services;

public class AnswerBotService
{
    private readonly DodoBotMessageService _messageService;

    private readonly ButtonProvider _buttonProvider;

    private readonly IHuntflowApi _huntflowApi;

    private readonly ISupabaseRepository _supabaseRepository;

    private readonly IOptions<ApplicationOptions> _options;

    private readonly HuntflowService _service;

    public AnswerBotService(DodoBotMessageService messageService, ButtonProvider buttonProvider,
        IHuntflowApi huntflowApi, ISupabaseRepository supabaseRepository,
        IOptions<ApplicationOptions> options, HuntflowService service)
    {
        _messageService = messageService;
        _buttonProvider = buttonProvider;
        _huntflowApi = huntflowApi;
        _supabaseRepository = supabaseRepository;
        _options = options;
        _service = service;
    }

    public async Task ProcessTextMessage(Message update)
    {
        if (update.Text.StartsWith("/"))
        {
            var message = update.Text.Remove(0, 1);
            switch (message.ToLower())
            {
                case "start":
                {
                    var internalUserId = await _supabaseRepository.AddNewUser(new СandidateInfo
                    {
                        FirstName = update.Chat.FirstName,
                        LastName = update.Chat.LastName,
                        TelegramId = update.Chat.Id
                    });

                    if (internalUserId.Length > 0)
                    {
                        var countedSubscribe = await _supabaseRepository.CountUserVacancySubscribe(internalUserId);
                        var button = countedSubscribe > 0
                            ? _buttonProvider.MainMenuButtons(countedSubscribe, _options.Value.PrivacyPolicyUrl)
                            : _buttonProvider.MainMenuButtons(_options.Value.PrivacyPolicyUrl);

                        await _messageService.SendInlineMessage(update.Chat.Id, StaffConstants.BotAnswer, button);
                    }

                    break;
                }
                case "about":
                {
                    var resources = await _supabaseRepository.GetResourcesDodo();
                    var resourcesButtons = _buttonProvider.DodoResourcesButton(resources);

                    await _messageService.SendInlineMessage(update.Chat.Id, StaffConstants.BotAnswer, resourcesButtons);
                    break;
                }
                case "setting":
                {
                    var userId = await _supabaseRepository.GetInternalUserId(update.Chat.Id);
                    if (userId.Length > 0)
                    {
                        var countedSubscribe = await _supabaseRepository.CountUserVacancySubscribe(userId);

                        if (countedSubscribe > 0)
                        {
                            var setting = await _supabaseRepository.ReadUserSubscribeOptions(userId);
                            var frequencyButton =
                                _buttonProvider.ButtonsFrequencySetting(setting);
                            await _messageService.SendInlineMessage(update.Chat.Id, "ttt", frequencyButton);
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
                            Url = _options.Value.PrivacyPolicyUrl
                        }
                    };
                    var legalButton = _buttonProvider.DodoResourcesButton(resources);

                    await _messageService.SendInlineMessage(update.Chat.Id, StaffConstants.LegalAnswer, legalButton);
                    break;
                }
                default:
                {
                    await _messageService.SendMessageToUser(update.Chat.Id, StaffConstants.DontUnderstandYouUseButton);
                    break;
                }
            }
        }
        else
        {
            await _messageService.SendMessageToUser(update.Chat.Id, StaffConstants.DontUnderstandYouUseButton);
        }
    }

    public async Task ProcessCallbackMessage(CallbackQuery callbackQuery)
    {
        var sp = callbackQuery.Data.Split("-");
        switch (sp[0].ToLower())
        {
            case "start":
            {
                var userId = await _supabaseRepository.GetInternalUserId(callbackQuery.User.Id);

                var countedSubscribe = await _supabaseRepository.CountUserVacancySubscribe(userId);
                var button = countedSubscribe > 0
                    ? _buttonProvider.MainMenuButtons(countedSubscribe, _options.Value.PrivacyPolicyUrl)
                    : _buttonProvider.MainMenuButtons(_options.Value.PrivacyPolicyUrl);

                await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, StaffConstants.BotAnswer, button);
                break;
            }
            case "viewvacancy":
            case "mysubscription":
            {
                var userId = await _supabaseRepository.GetInternalUserId(callbackQuery.User.Id);
                var it = await _supabaseRepository.GetUserSubSpecialty(userId);
                var bussiness = await _supabaseRepository.GetUserSpecialty(userId);

                var itAndBusiness = _buttonProvider.EngineeringAndBusiness(it.Count, bussiness.Count);

                await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, StaffConstants.WellSet, itAndBusiness);

                break;
            }
            case "sendvacancies":
            {
                var mainMenuButtons = _buttonProvider.MainMenuButtons(_options.Value.PrivacyPolicyUrl);
                var userId = await _supabaseRepository.GetInternalUserId(callbackQuery.User.Id);
                var userInfo = await _supabaseRepository.GetUserSubscription(userId);
                var allOpenVacancies = await _service.GetAllVacancies();

                var finalVacancies = new List<VacancyDto>();
                var staffVacancies = allOpenVacancies.ToList();

                if (staffVacancies.Any())
                {
                    foreach (var staffVacancy in staffVacancies)
                    {
                        if (staffVacancy.Speciality.HasValue)
                        {
                            if (userInfo.Specialty.Contains(staffVacancy.Speciality.Value))
                            {
                                finalVacancies.Add(new VacancyDto
                                {
                                    Id = staffVacancy.Id,
                                    Position = staffVacancy.Position,
                                    Url = $"Подробнее: {StaffConstants.DodoUrl}{staffVacancy.Id}"
                                });
                            }
                        }

                        if (staffVacancy.SubSpeciality.HasValue)
                        {
                            if (userInfo.SubSpecialty.Contains(staffVacancy.SubSpeciality.Value))
                            {
                                finalVacancies.Add(new VacancyDto
                                {
                                    Id = staffVacancy.Id,
                                    Position = staffVacancy.Position,
                                    Url = $"Подробнее: {StaffConstants.DodoUrl}{staffVacancy.Id}"
                                });
                            }
                        }
                    }

                    var vacancyText = VacancyExtension.PrepareVacancyText(finalVacancies);

                    await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                        callbackQuery.Message.MessageId, vacancyText, mainMenuButtons);
                }
                else
                {
                    await _messageService.SendMessageToUser(callbackQuery.Message.Chat.Id,
                        StaffConstants.VacanciesNotFound);
                }

                break;
            }
            case "it":
            {
                var subSpecialtyResponse = await _huntflowApi.GetDodoSubSpecialtyAsync();
                if (subSpecialtyResponse != null)
                {
                    var userId = await _supabaseRepository.GetInternalUserId(callbackQuery.User.Id);
                    var userSubSpecialty = await _supabaseRepository.GetUserSubSpecialty(userId);

                    var sortedSubSpecialties = subSpecialtyResponse
                        .Fields
                        .Where(t => t.Active)
                        .ToList();

                    var buttons =
                        _buttonProvider.GetDodoStreamButtons(sortedSubSpecialties, "subspecialty", userSubSpecialty);

                    await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                        callbackQuery.Message.MessageId, StaffConstants.OneOrMore, buttons);
                }

                break;
            }
            case "business":
            {
                var dodoStreams = await _huntflowApi.GetDodoStreamAsync();
                if (dodoStreams != null)
                {
                    var userId = await _supabaseRepository.GetInternalUserId(callbackQuery.User.Id);
                    var userSpecialty = await _supabaseRepository.GetUserSpecialty(userId);

                    var sortedStreams = dodoStreams
                        .Fields
                        .Where(t => t.Active && t.Id != 55)
                        .ToList();

                    var buttons = _buttonProvider.GetDodoStreamButtons(sortedStreams, "specialty", userSpecialty);
                    await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                        callbackQuery.Message.MessageId, StaffConstants.OneOrMore, buttons);
                }
                else
                {
                    await _messageService.SendMessageToUser(callbackQuery.Message.Chat.Id, StaffConstants.DontWork);
                }
                break;
            }

            case "specialty":
            {
                var dodoStreams = await _huntflowApi.GetDodoStreamAsync();
                var userId = await _supabaseRepository.GetInternalUserId(callbackQuery.User.Id);

                if (int.TryParse(sp[1], out var specialtyId))
                {
                    if (userId.Length > 0)
                    {
                        var previousSpecialty = await _supabaseRepository.GetUserSpecialty(userId);
                        if (previousSpecialty.Contains(specialtyId))
                        {
                            await _supabaseRepository.RemoveSpecialty(specialtyId, userId);
                        }
                        else
                        {
                            await _supabaseRepository.WriteSpecialty(specialtyId, userId);
                        }
                    }

                    if (dodoStreams != null)
                    {
                        var newSpecialty = await _supabaseRepository.GetUserSpecialty(userId);
                        var sortedStreams = dodoStreams
                            .Fields
                            .Where(t => t.Active && t.Id != 55)
                            .ToList();
                        var buttons = _buttonProvider.GetDodoStreamButtons(sortedStreams, "specialty", newSpecialty);

                        await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                            callbackQuery.Message.MessageId, StaffConstants.OneOrMore,
                            buttons);
                    }
                }

                break;
            }
            case "subspecialty":
            {
                var subSpecialtyResponse = await _huntflowApi.GetDodoSubSpecialtyAsync();
                var userId = await _supabaseRepository.GetInternalUserId(callbackQuery.User.Id);

                if (int.TryParse(sp[1], out var subsSpecialtyId))
                {
                    if (userId.Length > 0)
                    {
                        var previousSubSpecialty = await _supabaseRepository.GetUserSubSpecialty(userId);
                        if (previousSubSpecialty.Contains(subsSpecialtyId))
                        {
                            await _supabaseRepository.RemoveSubSpecialty(subsSpecialtyId, userId);
                        }
                        else
                        {
                            await _supabaseRepository.WriteSubSpecialty(subsSpecialtyId, userId);
                        }
                    }

                    if (subSpecialtyResponse != null)
                    {
                        var newSubSpecialty = await _supabaseRepository.GetUserSubSpecialty(userId);
                        var sortedStreams = subSpecialtyResponse
                            .Fields
                            .Where(t => t.Active && t.Id != 55)
                            .ToList();
                        var buttons =
                            _buttonProvider.GetDodoStreamButtons(sortedStreams, "subspecialty", newSubSpecialty);
                        await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                            callbackQuery.Message.MessageId, StaffConstants.OneOrMore,
                            buttons);
                    }
                }

                break;
            }
            case "support":
            {
                await _messageService.SendMessageToUser(callbackQuery.Message.Chat.Id,
                    "Напишите на hr-team@dodobrands.io");
                break;
            }
            case "viewcompany":
            {
                var resources = await _supabaseRepository.GetResourcesDodo();
                var resourcesButtons = _buttonProvider.DodoResourcesButton(resources);

                await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, "Вот несколько мест, где ты можешь узнать о нас больше:",
                    resourcesButtons);

                break;
            }
            case "back":
            {
                var userId = await _supabaseRepository.GetInternalUserId(callbackQuery.User.Id);
                var it = await _supabaseRepository.GetUserSubSpecialty(userId);
                var bussiness = await _supabaseRepository.GetUserSpecialty(userId);

                var itAndBusiness = _buttonProvider.EngineeringAndBusiness(it.Count, bussiness.Count);
                await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, StaffConstants.WellSet, itAndBusiness);
                break;
            }
            case "set":
            {
                var userId = await _supabaseRepository.GetInternalUserId(callbackQuery.User.Id);

                var userInfo = await _supabaseRepository.GetUserSubscription(userId);
                var specialtyInfo = await _huntflowApi.GetDodoStreamAsync();
                var subSpecialtyInfo = await _huntflowApi.GetDodoSubSpecialtyAsync();

                var exists = new List<string>();
                if (specialtyInfo != null)
                {
                    var specialty = specialtyInfo
                        .Fields
                        .Where(t => t.Active && userInfo.Specialty.Contains(t.Id))
                        .Select(t => t.Name);
                    exists.AddRange(specialty);
                }

                if (subSpecialtyInfo != null)
                {
                    var subSpecialty = subSpecialtyInfo
                        .Fields
                        .Where(t => t.Active && userInfo.SubSpecialty.Contains(t.Id))
                        .Select(t => t.Name);

                    exists.AddRange(subSpecialty);
                }

                var vacancy = exists.BuildCommaString();
                await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, vacancy, _buttonProvider.GetBackMenuButton());

                break;
            }
            case "changefrequency":
            {
                var userId = await _supabaseRepository.GetInternalUserId(callbackQuery.User.Id);

                if (userId.Length > 0)
                {
                    if (int.TryParse(sp[1], out var setting))
                    {
                        var newSettings = setting.ConvertIntoEnum();

                        if (newSettings.HasValue)
                        {
                            var result = await _supabaseRepository.WriteReadUserSubscribe(userId, newSettings.Value);

                            var timingButton = _buttonProvider.ButtonsFrequencySetting(result);
                            await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                                callbackQuery.Message.MessageId,
                                "Теперь у тебя другая частота рассылки и она отмечена ✅", timingButton);
                        }
                    }
                }

                break;
            }
            case "frequency":
            {
                var userId = await _supabaseRepository.GetInternalUserId(callbackQuery.User.Id);

                if (userId.Length > 0)
                {
                    var result = await _supabaseRepository.ReadUserSubscribeOptions(userId);

                    if (result.HasValue)
                    {
                        var timingButton = _buttonProvider.ButtonsFrequencySetting(result);
                        await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                            callbackQuery.Message.MessageId, "Выбери частоту рассылки: ", timingButton);
                    }
                    else
                    {
                        result = await _supabaseRepository.WriteReadUserSubscribe(userId,
                            PeriodicitySettings.Enable);

                        var timingButton = _buttonProvider.ButtonsFrequencySetting(result);
                        await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                            callbackQuery.Message.MessageId, "Выбери частоту рассылки: ", timingButton);
                    }
                }

                break;
            }
            default:
            {
                var backMainMenuButton = _buttonProvider.GetBackMenuButton();
                await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, "Неизвестная кнопка",
                    backMainMenuButton);
                break;
            }
        }
    }
}