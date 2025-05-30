using System.Threading.Tasks;

namespace DodoBot.Interfaces;

public interface IAuthenticateService
{
    Task<string> GetRefreshToken(bool isTokenExpired = false);
}