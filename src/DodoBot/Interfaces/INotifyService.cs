using System.Threading.Tasks;

namespace DodoBot.Interfaces;

public interface INotifyService
{
    Task SendNotifyAllUsers();
}