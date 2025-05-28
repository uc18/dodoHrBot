using System.Threading.Tasks;

namespace DodoBot.Interfaces;

public interface ISupabaseService
{
    Task<bool> SetNewSomething(string streamName);

    Task<bool> CreateNewUser();
    Task<bool> FindUser();
}