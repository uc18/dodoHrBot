using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DodoBot.Interfaces;
using DodoBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository;
using Repository.Entities;

namespace DodoBot.Services;

public class SupabaseService : ISupabaseService
{
    private readonly SupabaseContext _context;

    private readonly ILogger<SupabaseService> _logger;

    public SupabaseService(SupabaseContext context, ILogger<SupabaseService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string> AddNewUser(СandidateInfo candidate)
    {
        var user = await _context
            .Candidates
            .FirstOrDefaultAsync(t => t.TelegramId == candidate.TelegramId);

        if (user == null)
        {
            var newUser = await _context
                .Candidates
                .AddAsync(new Candidate
                {
                    Id = Guid.NewGuid().ToString(),
                    TelegramId = candidate.TelegramId,
                    FirstName = candidate.FirstName,
                    LastName = candidate.LastName
                });

            await _context.SaveChangesAsync();

            return newUser.Entity.Id;
        }

        return user.Id;
    }

    public async Task<string> GetUserId(long telegramId)
    {
        var user = await _context
            .Candidates
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TelegramId == telegramId);

        if (user != null)
        {
            return user.Id;
        }

        return string.Empty;
    }

    public async Task<bool> WritePeriodicitySettings(Candidate candidate)
    {
        try
        {
            await _context.Candidates.AddAsync(candidate);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    public async Task<int> UserVacancySubscribe(string userId)
    {
        return await _context
            .SubscribedVacancies
            .AsNoTracking()
            .CountAsync(t => t.UserId == userId);
    }

    public async Task<HashSet<int>> GetUserSpecialty(string userId)
    {
        var specialtiesUser = await _context
            .SubscribedVacancies
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.SpecialtyId != null)
            .Select(t => t.SpecialtyId!.Value)
            .ToListAsync();

        return specialtiesUser.ToHashSet();
    }

    public async Task<bool> WriteSpecialty(int specialtyId, string userId)
    {
        try
        {
            await _context.SubscribedVacancies.AddAsync(new SubscribedVacancy
            {
                SpecialtyId = specialtyId,
                Id = Guid.NewGuid().ToString(),
                UserId = userId
            });

            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }

        return true;
    }

    public async Task<bool> RemoveSpecialty(int specialtyId, string userId)
    {
        try
        {
            var subscribe = await _context
                .SubscribedVacancies
                .FirstOrDefaultAsync(t => t.SpecialtyId == specialtyId && t.UserId == userId);

            if (subscribe != null)
            {
                _context.SubscribedVacancies.Remove(subscribe);
                await _context.SaveChangesAsync();

                return true;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }

        return false;
    }

    public async Task<bool> WriteSubSpecialty(int subSpecialtyId, string userId)
    {
        try
        {
            await _context.SubscribedVacancies.AddAsync(new SubscribedVacancy
            {
                SubspecialtyId = subSpecialtyId,
                Id = Guid.NewGuid().ToString(),
                UserId = userId
            });

            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }

        return true;
    }

    public async Task<bool> RemoveSubSpecialty(int subSpecialtyId, string userId)
    {
        try
        {
            var subscribe = await _context
                .SubscribedVacancies
                .FirstOrDefaultAsync(t => t.SubspecialtyId == subSpecialtyId && t.UserId == userId);

            if (subscribe != null)
            {
                _context.SubscribedVacancies.Remove(subscribe);
                await _context.SaveChangesAsync();

                return true;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }

        return false;
    }

    public async Task<HashSet<int>> GetUserSubSpecialty(string userId)
    {
        var specialtiesUser = await _context
            .SubscribedVacancies
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.SubspecialtyId != null)
            .Select(t => t.SubspecialtyId!.Value)
            .ToListAsync();

        return specialtiesUser.ToHashSet();
    }

    public async Task<List<ResourceDto>> GetResourcesDodo()
    {
        var resources = await _context
            .Resources
            .AsNoTracking()
            .ToListAsync();

        return resources.Select(t => new ResourceDto
        {
            Name = t.Name,
            Url = t.Url
        }).ToList();
    }

    public async Task<int> ReadUserSubscribeOptions(string userId)
    {
        var candidate = await _context
            .Candidates
            .Include(t => t.Periodicity)
            .FirstOrDefaultAsync(t => t.Id == userId);

        if (candidate != null)
        {
            return (int)candidate.Periodicity.Settings;
        }

        return 0;
    }

    public async Task<bool> UpdateExistUser(string userId)
    {
        try
        {
            var candidate = await _context.Candidates.FirstOrDefaultAsync(t => t.Id == userId);

            if (candidate != null)
            {
                candidate.Periodicity = new Periodicity
                {
                    Settings = PeriodicitySettings.EveryWeek,
                    StartNotify = DateTime.UtcNow,
                    Id = Guid.NewGuid().ToString()
                };

                await _context.SaveChangesAsync();
                return true;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }

        return false;
    }
}