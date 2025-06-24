using Repository.Entities;

namespace DodoBot.Extensions;

public static class EnumExtension
{
    public static string GetDescription(this PeriodicitySettings value)
    {
        return value switch
        {
            PeriodicitySettings.EveryWeek => "Каждую неделю",
            PeriodicitySettings.EveryMonth => "Каждый месяц",
            PeriodicitySettings.EveryThreeMonth => "Каждые три месяца",
            PeriodicitySettings.Disable => "Отписаться",
            _ => "Не определено"
        };
    }
}