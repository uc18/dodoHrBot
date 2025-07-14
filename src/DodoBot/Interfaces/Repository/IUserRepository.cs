using System.Threading.Tasks;
using Repository.Entities;

namespace DodoBot.Interfaces.Repository;

public interface IUserRepository
{
    Task<Candidate?> GetUser(string userId);
}