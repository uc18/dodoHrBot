using System.Collections.Generic;
using System.Linq;
using DodoBot.Models.Huntflow;
using TelegramLibrary.Models.Keyboard;

namespace DodoBot.Providers;

public class ButtonProvider
{
    public InlineKeyboardMarkup MainMenuButtons()
    {
        return new InlineKeyboardMarkup
        {
            Keyboard = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "Просмотреть вакансии",
                        CallbackData = "viewVacancy"
                    }
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "Частота рассылок",
                        CallbackData = "timingNotify"
                    }
                }
            }
        };
    }

    public InlineKeyboardMarkup GetDodoStreamButtons(IEnumerable<HuntflowDictionary> dodoStreams, string typeOfStream)
    {
        var buttons = dodoStreams.Select(stream =>
            new List<InlineKeyboardButton>
            {
                new InlineKeyboardButton
                {
                    Text = stream.Name,
                    CallbackData = $"{typeOfStream}-{stream.Id}"
                }
            }).ToList();

        if (typeOfStream.ToLower().Equals("subspecialty"))
        {
            buttons.Add(new List<InlineKeyboardButton>
            {
                new InlineKeyboardButton
                {
                    Text = "Подписаться на обновления",
                    CallbackData = "subscribe"
                }
            });
        }

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


    public InlineKeyboardMarkup GetBackMenuButton()
    {
        return new InlineKeyboardMarkup
        {
            Keyboard = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "В главное меню",
                        CallbackData = "start"
                    }
                }
            }
        };
    }

    public InlineKeyboardMarkup ButtonForSubscribe()
    {
        return new InlineKeyboardMarkup
        {
            Keyboard = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "Подписаться на обновления",
                        CallbackData = "subscribe"
                    }
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "В главное меню",
                        CallbackData = "start"
                    }
                }
            }
        };
    }

    public InlineKeyboardMarkup ButtonForSetTiming()
    {
        return new InlineKeyboardMarkup
        {
            Keyboard = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "Выбрать частоту рассылки",
                        CallbackData = "timingNotify"
                    }
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "В главное меню",
                        CallbackData = "start"
                    }
                }
            }
        };
    }

    public InlineKeyboardMarkup ButtonTimingView()
    {
        return new InlineKeyboardMarkup
        {
            Keyboard = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "Раз в неделю",
                        CallbackData = "1"
                    }
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "Раз в месяц",
                        CallbackData = "2"
                    }
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "Раз в три месяца",
                        CallbackData = "3"
                    }
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = "В главное меню",
                        CallbackData = "start"
                    }
                }
            }
        };
    }
}