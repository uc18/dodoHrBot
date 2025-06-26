using System.ComponentModel;

namespace Repository.Entities;

public enum PeriodicitySettings
{
    [Description("Раз в неделю")]
    EveryWeek = 1,

    [Description("Раз в месяц")]
    EveryMonth = 2,

    [Description("Раз в три месяца")]
    EveryThreeMonth = 3,

    [Description("Выключено")]
    Disable = 4
}