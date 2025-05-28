namespace DodoBot.Constants;

public static class UriApiConstants
{
    private const string DodoId = "2";

    public static string Speciality = $"/v2/accounts/{DodoId}/dictionaries/speciality";

    public static string SubSpeciality = $"/v2/accounts/{DodoId}/dictionaries/subspeciality";

    public static string Vacancies = $"/v2/accounts/{DodoId}/vacancies";

    public static string WorkFormat = $"/v2/accounts/{DodoId}/dictionaries/work_format";

    public static string City = $"/v2/accounts/{DodoId}/dictionaries/vacancy_city";

    public static string Grade = $"/v2/accounts/{DodoId}/dictionaries/grade";

    public static string RefreshTokenUri = "/v2/token/refresh";
}