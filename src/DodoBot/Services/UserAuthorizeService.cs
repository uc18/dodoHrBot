using System.Threading.Tasks;
using DodoBot.Interfaces.Repository;
using DodoBot.Interfaces.Services;

namespace DodoBot.Services;

public class UserAuthorizeService : IUserAuthorizeService
{
    private readonly IUserRepository _userRepository;

    public UserAuthorizeService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> IsUserAcceptLegal(string userId)
    {
        if (userId.Length > 0)
        {
            var user = await _userRepository.GetUser(userId);

            if (user != null)
            {

            }
        }

        return false;
    }
}