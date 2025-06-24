using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DodoBot.Constants;
using DodoBot.Extensions;
using DodoBot.Interfaces;
using DodoBot.Models;
using DodoBot.Models.Huntflow;
using DodoBot.Providers;
using Repository.Entities;
using TelegramLibrary.Models;

namespace DodoBot.Services;

public class AnswerBotService
{
    private readonly DodoBotMessageService _messageService;

    private readonly ButtonProvider _buttonProvider;

    private readonly IHuntflowService _huntflowService;

    private readonly ISupabaseService _supabaseService;

    private readonly UserContextProvider _userContextProvider;

    public AnswerBotService(DodoBotMessageService messageService, ButtonProvider buttonProvider,
        IHuntflowService huntflowService, ISupabaseService supabaseService, UserContextProvider user)
    {
        _messageService = messageService;
        _buttonProvider = buttonProvider;
        _huntflowService = huntflowService;
        _supabaseService = supabaseService;
        _userContextProvider = user;
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
                    var internalUserId = await _supabaseService.AddNewUser(new СandidateInfo
                    {
                        FirstName = update.Chat.FirstName,
                        LastName = update.Chat.LastName,
                        TelegramId = update.Chat.Id
                    });

                    if (internalUserId.Length > 0)
                    {
                        var countedSubscribe = await _supabaseService.UserVacancySubscribe(internalUserId);
                        var button = countedSubscribe > 0
                            ? _buttonProvider.MainMenuButtons(countedSubscribe)
                            : _buttonProvider.MainMenuButtons();

                        await _messageService.SendInlineMessage(update.Chat.Id, StaffConstants.BotAnswer, button);
                    }

                    break;
                }
                case "about":
                {
                    var resources = await _supabaseService.GetResourcesDodo();
                    var resourcesButtons = _buttonProvider.DodoResourcesButton(resources);

                    await _messageService.SendInlineMessage(update.Chat.Id, StaffConstants.BotAnswer, resourcesButtons);
                    break;
                }
                case "setting":
                {
                    if (false)
                    {
                        var userId = await _supabaseService.GetUserId(update.Chat.Id);

                        if (userId.Length > 0)
                        {
                            var countedSubscribe = await _supabaseService.UserVacancySubscribe(userId);

                            if (countedSubscribe > 0)
                            {
                                var t = await _supabaseService.ReadUserSubscribeOptions(userId);
                                var aa = new List<PeriodicitySettings>
                                {
                                    PeriodicitySettings.EveryWeek,
                                    PeriodicitySettings.EveryMonth,
                                    PeriodicitySettings.EveryThreeMonth,
                                    PeriodicitySettings.Disable
                                };
                                var y = _buttonProvider.ButtonTimingView(aa);
                                //await _messageService.SendMessageToUser(update.Chat.Id, t);
                            }
                        }
                    }

                    await _messageService.SendMessageToUser(update.Chat.Id, StaffConstants.ButtonDoesntWork);
                    break;
                }
                case "legal":
                {
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
                var userId = await _supabaseService.GetUserId(callbackQuery.User.Id);

                var countedSubscribe = await _supabaseService.UserVacancySubscribe(userId);
                var button = countedSubscribe > 0
                    ? _buttonProvider.MainMenuButtons(countedSubscribe)
                    : _buttonProvider.MainMenuButtons();

                await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, StaffConstants.BotAnswer, button);
                break;
            }
            case "viewvacancy":
            case "mysubscription":
            {
                var userId = await _supabaseService.GetUserId(callbackQuery.User.Id);
                var it = await _supabaseService.GetUserSubSpecialty(userId);
                var bussiness = await _supabaseService.GetUserSpecialty(userId);

                var itAndBusiness = _buttonProvider.EngineeringOrBusiness(it.Count, bussiness.Count);

                await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, StaffConstants.WellSet, itAndBusiness);

                break;
            }
            case "it":
            {
                var subSpecialtyResponse = await _huntflowService.GetDodoSubSpecialtyAsync();
                if (subSpecialtyResponse != null)
                {
                    var userId = await _supabaseService.GetUserId(callbackQuery.User.Id);
                    var userSubSpecialty = await _supabaseService.GetUserSubSpecialty(userId);

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
                var dodoStreams = await _huntflowService.GetDodoStreamAsync();
                if (dodoStreams != null)
                {
                    var userId = await _supabaseService.GetUserId(callbackQuery.User.Id);
                    var userSpecialty = await _supabaseService.GetUserSpecialty(userId);

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
                var dodoStreams = await _huntflowService.GetDodoStreamAsync();
                var userId = await _supabaseService.GetUserId(callbackQuery.User.Id);

                if (int.TryParse(sp[1], out var specialtyId))
                {
                    if (userId.Length > 0)
                    {
                        var previousSpecialty = await _supabaseService.GetUserSpecialty(userId);
                        if (previousSpecialty.Contains(specialtyId))
                        {
                            await _supabaseService.RemoveSpecialty(specialtyId, userId);
                        }
                        else
                        {
                            await _supabaseService.WriteSpecialty(specialtyId, userId);
                        }
                    }

                    if (dodoStreams != null)
                    {
                        var newSpecialty = await _supabaseService.GetUserSpecialty(userId);
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
                var subSpecialtyResponse = await _huntflowService.GetDodoSubSpecialtyAsync();
                var userId = await _supabaseService.GetUserId(callbackQuery.User.Id);

                if (int.TryParse(sp[1], out var subsSpecialtyId))
                {
                    if (userId.Length > 0)
                    {
                        var previousSubSpecialty = await _supabaseService.GetUserSubSpecialty(userId);
                        if (previousSubSpecialty.Contains(subsSpecialtyId))
                        {
                            await _supabaseService.RemoveSubSpecialty(subsSpecialtyId, userId);
                        }
                        else
                        {
                            await _supabaseService.WriteSubSpecialty(subsSpecialtyId, userId);
                        }
                    }

                    if (subSpecialtyResponse != null)
                    {
                        var newSubSpecialty = await _supabaseService.GetUserSubSpecialty(userId);
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
                await _messageService.SendMessageToUser(callbackQuery.Message.Chat.Id, "Напишите на help@dodopizza.ru");
                break;
            }
            case "viewcompany":
            {
                var resources = await _supabaseService.GetResourcesDodo();
                var resourcesButtons = _buttonProvider.DodoResourcesButton(resources);

                await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, "Вот несколько мест, где ты можешь узнать о нас больше:",
                    resourcesButtons);

                break;
            }
            case "back":
            {
                var userId = await _supabaseService.GetUserId(callbackQuery.User.Id);
                var it = await _supabaseService.GetUserSubSpecialty(userId);
                var bussiness = await _supabaseService.GetUserSpecialty(userId);

                var itAndBusiness = _buttonProvider.EngineeringOrBusiness(it.Count, bussiness.Count);
                await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, StaffConstants.WellSet, itAndBusiness);
                break;
            }
            case "set":
            {
                var userId = await _supabaseService.GetUserId(callbackQuery.User.Id);
                await _supabaseService.UpdateExistUser(userId);

                var countedSubscribe = await _supabaseService.UserVacancySubscribe(userId);
                var button = countedSubscribe > 0
                    ? _buttonProvider.MainMenuButtons(countedSubscribe)
                    : _buttonProvider.MainMenuButtons();

                await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, StaffConstants.WellSet, button);

                break;
            }
            case "frequency":
            {
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

    private async Task<List<VacancyDto>> GetVacancyTextAsync(int specialityId, bool isSubSpecialty)
    {
        var cityFormat = await _huntflowService.GetCityResponseAsync();
        var gradeResponse = await _huntflowService.GetGradeResponseAsync();
        var workFormat = await _huntflowService.GetWorkFormatAsync();
        var subSpecialtyResponse = await _huntflowService.GetDodoSubSpecialtyAsync();
        var dodoStreams = await _huntflowService.GetDodoStreamAsync();

        var pages = 1;
        var totalPages = 0;
        var notSortedVacancy = await _huntflowService.GetVacanciesAsync(pages);

        var allVacancies = new List<Vacancy>();

        if (notSortedVacancy != null)
        {
            allVacancies.AddRange(notSortedVacancy.Vacancies);
            if (notSortedVacancy.TotalPages > pages)
            {
                do
                {
                    pages++;
                    var addingVacancy = await _huntflowService.GetVacanciesAsync(pages);

                    if (addingVacancy != null)
                    {
                        allVacancies.AddRange(addingVacancy.Vacancies);
                        totalPages = addingVacancy.TotalPages;
                    }
                } while (totalPages > pages);
            }
        }

        var activeSpecialty = new Dictionary<int, string>();
        var activeStream = new Dictionary<int, string>();
        var activeGrades = new Dictionary<int, string>();
        var activeWorkFormat = new Dictionary<int, string>();
        var cityDictionary = new Dictionary<int, string>();

        if (subSpecialtyResponse != null)
        {
            activeSpecialty = subSpecialtyResponse
                .Fields
                .Where(t => t.Active)
                .ToDictionary(t => t.Id, t => t.Name);
        }

        if (dodoStreams != null)
        {
            activeStream = dodoStreams
                .Fields
                .Where(t => t.Active)
                .ToDictionary(t => t.Id, t => t.Name);
        }

        if (gradeResponse != null)
        {
            activeGrades = gradeResponse
                .Fields
                .Where(t => t.Active)
                .ToDictionary(t => t.Id, t => t.Name);
        }

        if (workFormat != null)
        {
            activeWorkFormat = workFormat
                .Fields
                .Where(t => t.Active)
                .ToDictionary(t => t.Id, t => t.Name);
        }

        var sortedVacancies = new List<Vacancy>();

        if (isSubSpecialty)
        {
            sortedVacancies = allVacancies
                .Where(t => t.Speciality == 55 && t.SubSpeciality == specialityId
                                               && t.CareerPublication == 46)
                .ToList();
        }
        else
        {
            sortedVacancies = allVacancies
                .Where(t => t.Speciality == specialityId
                            && t.CareerPublication == 46)
                .ToList();
        }

        if (cityFormat != null)
        {
            cityDictionary = cityFormat.Fields.ToDictionary(t => t.Id, t => t.Name);
        }

        if (sortedVacancies.Count > 0)
        {
            var vacancyToText = sortedVacancies.Select(t => new VacancyDto
                {
                    Id = t.Id,
                    Speciality = t.Speciality.HasValue
                        ? activeStream[t.Speciality.Value]
                        : null,
                    SubSpeciality = t.SubSpeciality.HasValue
                        ? activeSpecialty[t.SubSpeciality.Value]
                        : null,
                    VacancyCity = t.VacancyCity
                        .Select(b => cityDictionary[b])
                        .BuildCommaString(),
                    Position = t.Position ?? string.Empty,
                    Money = t.Money ?? string.Empty,
                    Grade = t.Grade
                        .Select(b => activeGrades[b])
                        .BuildCommaString(),
                    WorkFormat = t.WorkFormat
                        .Select(b => activeWorkFormat[b])
                        .BuildCommaString()
                })
                .ToList();

            return vacancyToText;
        }

        return [];
    }
}