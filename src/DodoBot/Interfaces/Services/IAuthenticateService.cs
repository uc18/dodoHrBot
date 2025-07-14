using System.Threading.Tasks;

namespace DodoBot.Interfaces.Services;

public interface IAuthenticateService
{
    Task<string> GetRefreshToken();
}