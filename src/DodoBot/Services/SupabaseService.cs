using System;
using System.Threading.Tasks;
using DodoBot.Interfaces;

namespace DodoBot.Services;

public class SupabaseService : ISupabaseService
{
    public Task<bool> SetNewSomething(string streamName)
    {
        Console.WriteLine(streamName);
        return Task.FromResult(false);
    }

    public Task<bool> CreateNewUser()
    {
        return Task.FromResult(false);
    }

    public Task<bool> FindUser()
    {
        return Task.FromResult(false);
    }
}