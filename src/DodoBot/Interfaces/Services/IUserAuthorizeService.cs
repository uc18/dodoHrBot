using System.Threading.Tasks;

namespace DodoBot.Interfaces.Services;

public interface IUserAuthorizeService
{
    Task<bool> IsUserAcceptLegal(string userId);
}