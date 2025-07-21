using System.ComponentModel;

namespace Repository.Entities;

public enum PeriodicitySettings
{
    [Description("Включено")] Enable = 1,

    [Description("Выключено")]
    Disable = 2
}