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
                        CallbackData = "timing-notify"
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

    public InlineKeyboardMarkup Ffff()
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

    public InlineKeyboardMarkup Ffff2()
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

    public InlineKeyboardMarkup Ffff34()
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
                        Text = "Раз в полгода",
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

    public InlineKeyboardMarkup Ffff3()
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
                        CallbackData = "something"
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