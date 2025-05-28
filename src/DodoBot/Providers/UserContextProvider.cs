using System.Collections.Generic;
using DodoBot.Models;

namespace DodoBot.Providers;

public class UserContextProvider
{
    private readonly Dictionary<long, СandidateInfo> _candidateContexts;

    public UserContextProvider()
    {
        _candidateContexts = new Dictionary<long, СandidateInfo>();
    }

    public void Add(long id)
    {
        _candidateContexts.TryAdd(id, new СandidateInfo());
    }

    public СandidateInfo? GetCandidateContext(long id)
    {
        return _candidateContexts.GetValueOrDefault(id);
    }

    public bool RemoveUserContext(long id)
    {
        return _candidateContexts.Remove(id);
    }
}