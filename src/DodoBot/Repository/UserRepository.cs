using System.Threading.Tasks;
using DodoBot.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Entities;

namespace DodoBot.Repository;

public class UserRepository : IUserRepository
{
    private readonly SupabaseContext _context;

    public UserRepository(SupabaseContext context)
    {
        _context = context;
    }

    public async Task<Candidate?> GetUser(string userId)
    {
        var userInfo = await _context
            .Candidates
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == userId);

        return userInfo != null ? userInfo : null;
    }
}