using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DodoBot.Constants;
using DodoBot.Extensions;
using DodoBot.Interfaces;
using DodoBot.Models;
using DodoBot.Models.Huntflow;
using DodoBot.Providers;
using TelegramLibrary.Models;

namespace DodoBot.Services;

public class AnswerBotService
{
    private readonly DodoBotMessageService _messageService;

    private readonly ButtonProvider _buttonProvider;

    private readonly IHuntflowService _huntflowService;

    private readonly ISupabaseService _supabaseService;

    private readonly UserContextProvider _userContextProvider;

    private const string Subspecialty = "subspecialty";

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
            if (await _supabaseService.FindUser())
            {
                var resultCreate = await _supabaseService.CreateNewUser();
            }

            var message = update.Text.Remove(0, 1);
            if (message.ToLower().Equals("start"))
            {
                var mainMenuButtons = _buttonProvider.MainMenuButtons();
                await _messageService.SendInlineMessage(update.Chat.Id, StaffConstants.BotAnswer, mainMenuButtons);
                _userContextProvider.Add(update.From.Id);
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
        switch (sp[0])
        {
            case "start":
            {
                var mainMenuButtons = _buttonProvider.MainMenuButtons();
                await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, StaffConstants.BotAnswer, mainMenuButtons);
                _userContextProvider.RemoveUserContext(callbackQuery.User.Id);
                break;
            }
            case "viewVacancy":
            {
                var dodoStreams = await _huntflowService.GetDodoStreamAsync();
                if (dodoStreams != null)
                {
                    var sortedStreams = dodoStreams
                        .Fields
                        .Where(t => t.Active)
                        .ToList();

                    var buttons = _buttonProvider.GetDodoStreamButtons(sortedStreams, "specialty");
                    await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                        callbackQuery.Message.MessageId, "Выбери стрим: ", buttons);
                    _userContextProvider.Add(callbackQuery.User.Id);
                }
                else
                {
                    await _messageService.SendMessageToUser(callbackQuery.Message.Chat.Id, StaffConstants.IDontWork);
                }

                break;
            }
            case "timingNotify":
            {
                var button = _buttonProvider.ButtonTimingView();
                await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, "еееее", button);
                break;
            }
            case "subscribe":
            {
                var button = _buttonProvider.ButtonForSetTiming();
                await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, "Выбери частоту", button);
                break;
            }
            case "1":
            case "2":
            case "3":
            {
                await _messageService.SendMessageToUser(callbackQuery.Message.Chat.Id, "Поздравляю!");
                break;
            }
            case "specialty":
            {
                var user = _userContextProvider.GetCandidateContext(callbackQuery.User.Id);
                const string dodoEngineeringSpecialty = "55";
                if (sp[1].Equals(dodoEngineeringSpecialty))
                {
                    var subSpecialtyResponse = await _huntflowService.GetDodoSubSpecialtyAsync();
                    if (subSpecialtyResponse != null)
                    {
                        var sortedSubSpecialties = subSpecialtyResponse
                            .Fields
                            .Where(t => t.Active)
                            .ToList();

                        var buttons = _buttonProvider.GetDodoStreamButtons(sortedSubSpecialties, "subspecialty");

                        await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                            callbackQuery.Message.MessageId, "Выбери стрим: ", buttons);

                        if (user != null)
                        {
                            if (int.TryParse(callbackQuery.Data, out var streamId))
                            {
                                user.Speciality = streamId;
                            }
                        }
                    }
                }
                else
                {
                    var backMainMenuButton = _buttonProvider.GetBackMenuButton();

                    if (int.TryParse(sp[1], out var specialtyId))
                    {
                        var vacancy = await GetVacancyTextAsync(specialtyId, false);
                        if (user != null)
                        {
                            user.Speciality = specialtyId;
                        }

                        if (vacancy.Count > 0)
                        {
                            var vacancyText = VacancyExtension.PrepareVacancyText(vacancy);
                            var aaa = _buttonProvider.ButtonForSubscribe();
                            await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                                callbackQuery.Message.MessageId, vacancyText,
                                aaa);
                            break;
                        }
                    }

                    await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                        callbackQuery.Message.MessageId, "Вакансий нет",
                        backMainMenuButton);
                }

                break;
            }
            case "subspecialty":
            {
                var backMainMenuButton = _buttonProvider.GetBackMenuButton();
                var user = _userContextProvider.GetCandidateContext(callbackQuery.User.Id);

                if (int.TryParse(sp[1], out var specialtyId))
                {
                    var vacancyToText = await GetVacancyTextAsync(specialtyId, true);

                    if (user != null)
                    {
                        user.Subspeciality = specialtyId;
                    }

                    if (vacancyToText.Count > 0)
                    {
                        var vacancyText = VacancyExtension.PrepareVacancyText(vacancyToText);
                        var buttons = _buttonProvider.ButtonForSubscribe();
                        await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                            callbackQuery.Message.MessageId, vacancyText,
                            buttons);
                        break;
                    }
                }

                await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, "Вакансий нет",
                    backMainMenuButton);

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
        int totalPages = 0;
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