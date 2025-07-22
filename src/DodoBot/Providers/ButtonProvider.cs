using System.Collections.Generic;
using System.Linq;
using DodoBot.Extensions;
using DodoBot.Models;
using DodoBot.Models.Huntflow;
using Repository.Entities;
using TelegramLibrary.Models.Keyboard;

namespace DodoBot.Providers;

public class ButtonProvider
{
    public InlineKeyboardMarkup MainMenuButtons(string legalUrl)
    {
        return new InlineKeyboardMarkup
        {
            Keyboard = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "Хочу получать вакансии",
                        CallbackData = "viewVacancy"
                    }
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "Пользовательское соглашение",
                        CallbackData = "legal",
                        Url = legalUrl
                    }
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "Узнать больше о компании",
                        CallbackData = "viewCompany"
                    }
                }
            }
        };
    }

    public InlineKeyboardMarkup MainMenuButtons(int countedSubscription, string legalUrl)
    {
        return new InlineKeyboardMarkup
        {
            Keyboard = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = $"Мои подписки [{countedSubscription}]",
                        CallbackData = "mySubscription"
                    }
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "Узнать больше о компании",
                        CallbackData = "viewCompany"
                    }
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "Пользовательское соглашение",
                        CallbackData = "legal",
                        Url = legalUrl
                    }
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "Получить открытые вакансии",
                        CallbackData = "sendvacancies"
                    }
                }
            }
        };
    }

    public InlineKeyboardMarkup EngineeringAndBusiness(int itSubscription, int businessSubscription)
    {
        return new InlineKeyboardMarkup
        {
            Keyboard = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = itSubscription > 0 ? $"IT [{itSubscription}]" : "IT",
                        CallbackData = "it"
                    }
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = businessSubscription > 0 ? $"Бизнес [{businessSubscription}]" : "Бизнес",
                        CallbackData = "business"
                    }
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "⬅️Назад",
                        CallbackData = "start"
                    }
                }
            }
        };
    }

    public InlineKeyboardMarkup DodoResourcesButton(List<ResourceDto> resources)
    {
        var resourceButtons = resources.Select(resource => new List<InlineKeyboardButton>
        {
            new InlineKeyboardButton
            {
                Text = $"{resource.Name}",
                Url = resource.Url
            }
        }).ToList();

        resourceButtons.Add(new List<InlineKeyboardButton>
        {
            new InlineKeyboardButton
            {
                Text = "В главное меню",
                CallbackData = "start"
            }
        });

        return new InlineKeyboardMarkup
        {
            Keyboard = resourceButtons
        };
    }

    public InlineKeyboardMarkup GetDodoStreamButtons(IEnumerable<HuntflowDictionary> dodoStreams, string typeOfStream,
        HashSet<int> existSpecialty)
    {
        var buttons = dodoStreams.Select(stream =>
            new List<InlineKeyboardButton>
            {
                new InlineKeyboardButton
                {
                    Text = existSpecialty.Contains(stream.Id) ? "✅" + stream.Name : "⬜️" + stream.Name,
                    CallbackData = $"{typeOfStream}-{stream.Id}"
                }
            }).ToList();

        buttons.Add(new List<InlineKeyboardButton>
        {
            new InlineKeyboardButton
            {
                Text = "✅Готово",
                CallbackData = "set"
            }
        });

        buttons.Add(new List<InlineKeyboardButton>
        {
            new InlineKeyboardButton
            {
                Text = "⬅️Назад",
                CallbackData = "viewvacancy"
            }
        });

        return new InlineKeyboardMarkup
        {
            Keyboard = buttons
        };
    }

    public InlineKeyboardMarkup GetFrequencyButton()
    {
        return new InlineKeyboardMarkup
        {
            Keyboard = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "Просмотреть открытые вакансии",
                        CallbackData = "sendvacancies"
                    }
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "Управление подпиской",
                        CallbackData = "frequency"
                    }
                }
            }
        };
    }

    public InlineKeyboardMarkup GetMainMenu()
    {
        return new InlineKeyboardMarkup
        {
            Keyboard = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "⬅️Назад",
                        CallbackData = "start"
                    }
                }
            }
        };
    }

    public InlineKeyboardMarkup ButtonsFrequencySetting(PeriodicitySettings? currentPeriodicity)
    {
        var settings = new List<PeriodicitySettings>
        {
            PeriodicitySettings.Enable,
            PeriodicitySettings.Disable
        };


        var buttons = settings.Select(t => new List<InlineKeyboardButton>
        {
            new InlineKeyboardButton
            {
                Text = currentPeriodicity == t ? $"{t.GetDescription()} ✅️" : t.GetDescription(),
                CallbackData = $"changefrequency-{(int)t}"
            }
        }).ToList();

        buttons.Add(new List<InlineKeyboardButton>
        {
            new InlineKeyboardButton
            {
                Text = "В главное меню",
                CallbackData = "start"
            }
        });

        return new InlineKeyboardMarkup
        {
            Keyboard = buttons
        };
    }
}