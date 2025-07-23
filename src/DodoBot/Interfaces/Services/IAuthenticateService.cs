using System.Threading.Tasks;

namespace DodoBot.Interfaces.Services;

public interface IAuthenticateService
{
    string GetExistsAccessToken();

    Task<string> GetNewAccessToken();
}