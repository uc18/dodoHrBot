namespace DodoBot.Options;

public class ApplicationOptions
{
    public string TelegramBotToken { get; set; }
    public string TelegramApiUrl { get; set; }

    public string HuntflowApiUrl { get; set; }

    public string SupabaseDbUrl { get; set; }

    public HuntflowTokens HuntflowTokens { get; set; }
}

public class HuntflowTokens
{
    public string HuntflowAccessTokenApi { get; set; }

    public string HuntflowRefreshTokenApi { get; set; }
}