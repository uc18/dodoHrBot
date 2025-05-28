using System.Collections.Generic;
using System.Text;
using DodoBot.Models;

namespace DodoBot.Extensions;

public static class BusExtensions
{
    public static string PrepareVacancyText(IEnumerable<VacancyDto> dodoVacancies)
    {
        var sbVacancy = new StringBuilder();
        foreach (var vacancy in dodoVacancies)
        {
            sbVacancy.AppendLine();
            sbVacancy.AppendLine($"Стрим: {vacancy.Speciality}");

            if (vacancy.Position.Length > 0)
            {
                sbVacancy.AppendLine($"Позиция: {vacancy.Position}");
            }

            if (vacancy.Money.Length > 0)
            {
                sbVacancy.AppendLine($"Оклад: {vacancy.Money}");
            }

            if (vacancy.VacancyCity.Length == 0)
            {
                sbVacancy.AppendLine("Город: удаленка");
            }
            else
            {
                sbVacancy.AppendLine($"Город: {vacancy.VacancyCity}");
            }

            if (vacancy.WorkFormat.Length > 0)
            {
                sbVacancy.AppendLine($"Режим работы: {vacancy.WorkFormat}");
            }

            if (vacancy.Grade.Length > 0)
            {
                sbVacancy.AppendLine($"Уровень: {vacancy.Grade}");
            }

            sbVacancy.AppendLine($"Подробнее: https://dodoteam.ru/vacancy/?vacancyId={vacancy.Id}");
        }

        return sbVacancy.ToString();
    }

    public static string BuildCommaString(this IEnumerable<string> values)
    {
        var sb = new StringBuilder();

        foreach (var value in values)
        {
            sb.AppendJoin(",", value);
        }

        return sb.ToString();
    }
}