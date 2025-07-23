using System.Collections.Generic;
using System.Text;
using DodoBot.Models;

namespace DodoBot.Extensions;

public static class VacancyExtension
{
    public static string PrepareVacancyText(IEnumerable<VacancyDto> dodoVacancies)
    {
        var sbVacancy = new StringBuilder();
        foreach (var vacancy in dodoVacancies)
        {
            sbVacancy.AppendLine();

            if (vacancy.Position.Length > 0)
            {
                sbVacancy.AppendLine($"Позиция: {vacancy.Position}");
            }

            sbVacancy.AppendLine($"Подробнее: https://dodoteam.ru/vacancy?vacancyId={vacancy.Id}");
        }

        return sbVacancy.ToString();
    }

    public static string BuildCommaString(this List<string> values)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Готово! Теперь ты будешь в курсе новых вакансий по направлениям: ");
        foreach (var value in values)
        {
            if (value.Contains("&"))
            {
                sb.AppendLine($"- {value.Replace("&", "%26")}");
            }
            else
            {
                sb.AppendLine($"- {value}");
            }
        }

        sb.AppendLine("Никакого спама, только лучшие предложения!");
        return sb.ToString();
    }
}