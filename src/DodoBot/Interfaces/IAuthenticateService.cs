using System.Threading.Tasks;

namespace DodoBot.Interfaces;

public interface IAuthenticateService
{
    string GetExistsAccessToken();

    Task<string> GetNewAccessToken();
}