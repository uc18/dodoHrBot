using System.Threading.Tasks;

namespace DodoBot.Interfaces.Services;

public interface INotifyService
{
    Task SendNotifyAllUsers();
}