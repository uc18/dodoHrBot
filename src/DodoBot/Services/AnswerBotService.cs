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
                var button = _buttonProvider.Ffff34();
                await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, "еееее", button);
                break;
            }
            case "subscribe":
            {
                var button = _buttonProvider.Ffff2();
                await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId, "Выбери частоту", button);
                break;
            }
            case "sendVacancies":
            {
                await _messageService.SendMessageToUser(callbackQuery.Message.Chat.Id,
                    "Данная возможность пока недоступна");
                break;
            }
            case "specialty":
            {
                var splittedData = callbackQuery.Data.Split("-");
                if (splittedData[1].Equals("55"))
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

                        var user = _userContextProvider.GetCandidateContext(callbackQuery.User.Id);

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

                    var backMainMenuButton = _buttonProvider.GetBackMenuButton();
                    if (splittedData[0].ToLower().Equals(Subspecialty.ToLower()))
                    {
                        if (int.TryParse(splittedData[1], out var subSpecialtyId))
                        {
                            var sortedVacancy = allVacancies
                                .Where(t => t.Speciality == 55 && t.SubSpeciality == subSpecialtyId
                                                               && t.CareerPublication == 46)
                                .ToList();

                            if (cityFormat != null)
                            {
                                var cityDictionary = cityFormat.Fields.ToDictionary(t => t.Id, t => t.Name);
                                if (sortedVacancy.Count > 0)
                                {
                                    var vacancyToText = sortedVacancy.Select(t => new VacancyDto
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

                                    var vacancyText = BusExtensions.PrepareVacancyText(vacancyToText);
                                    var aaa = _buttonProvider.Ffff();
                                    await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                                        callbackQuery.Message.MessageId, vacancyText,
                                        aaa);
                                }
                                else
                                {
                                    await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                                        callbackQuery.Message.MessageId, "Вакансий нет",
                                        backMainMenuButton);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (int.TryParse(splittedData[1], out var specId))
                        {
                            var sortedVacancy = allVacancies
                                .Where(t => t.Speciality == specId && t.CareerPublication == 46)
                                .ToList();

                            if (cityFormat != null)
                            {
                                var cityDictionary = cityFormat
                                    .Fields
                                    .ToDictionary(t => t.Id, t => t.Name);

                                if (sortedVacancy.Count > 0)
                                {
                                    var vacancyToText = sortedVacancy.Select(t => new VacancyDto
                                        {
                                            Id = t.Id,
                                            Speciality =
                                                t.Speciality.HasValue ? activeStream[t.Speciality.Value] : null,
                                            SubSpeciality = t.SubSpeciality.HasValue
                                                ? activeSpecialty[t.SubSpeciality.Value]
                                                : null,
                                            Position = t.Position ?? string.Empty,
                                            Money = t.Money ?? string.Empty,
                                            VacancyCity = t.VacancyCity
                                                .Select(b => cityDictionary[b]).BuildCommaString(),
                                            Grade = t.Grade.Select(b => activeGrades[b]).BuildCommaString(),
                                            WorkFormat = t.WorkFormat.Select(b => activeWorkFormat[b])
                                                .BuildCommaString()
                                        })
                                        .ToList();

                                    var vacancyText = BusExtensions.PrepareVacancyText(vacancyToText);
                                    var aaa = _buttonProvider.Ffff();
                                    await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                                        callbackQuery.Message.MessageId, vacancyText,
                                        aaa);
                                }
                                else
                                {
                                    await _messageService.ChangeAllContentInMessage(callbackQuery.Message.Chat.Id,
                                        callbackQuery.Message.MessageId, "Вакансий нет",
                                        backMainMenuButton);
                                }
                            }
                        }
                    }
                }

                break;
            }
            case "subspecialty":
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
}