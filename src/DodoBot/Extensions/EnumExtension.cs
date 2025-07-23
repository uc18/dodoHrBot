using Repository.Entities;

namespace DodoBot.Extensions;

public static class EnumExtension
{
    public static string GetDescription(this PeriodicitySettings value)
    {
        return value switch
        {
            PeriodicitySettings.Enable => "Включено",
            PeriodicitySettings.Disable => "Отписаться",
            _ => "Не определено"
        };
    }

    public static PeriodicitySettings? ConvertIntoEnum(this int setting)
    {
        return setting switch
        {
            1 => PeriodicitySettings.Enable,
            2 => PeriodicitySettings.Disable,
            _ => null
        };
    }
}