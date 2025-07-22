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

namespace DodoBot.Repositories;

public class SupabaseRepository(SupabaseContext context, ILogger<SupabaseRepository> logger)
    : ISupabaseRepository
{
    public async Task<string> AddNewUser(СandidateInfo candidate)
    {
        var user = await context
            .Candidates
            .FirstOrDefaultAsync(t => t.TelegramId == candidate.TelegramId);

        if (user == null)
        {
            var newUser = await context
                .Candidates
                .AddAsync(new Candidate
                {
                    Id = Guid.NewGuid().ToString(),
                    TelegramId = candidate.TelegramId,
                    FirstName = candidate.FirstName,
                    LastName = candidate.LastName
                });

            await context.SaveChangesAsync();

            return newUser.Entity.Id;
        }

        return user.Id;
    }

    public async Task<string> GetInternalUserId(long telegramId)
    {
        var user = await context
            .Candidates
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TelegramId == telegramId);

        if (user != null)
        {
            return user.Id;
        }

        return string.Empty;
    }

    public async Task<int> CountUserVacancySubscribe(string userId)
    {
        return await context
            .SubscribedVacancies
            .AsNoTracking()
            .CountAsync(t => t.UserId == userId);
    }

    public async Task<HashSet<int>> GetUserSpecialty(string userId)
    {
        var specialtiesUser = await context
            .SubscribedVacancies
            .AsNoTracking()
            .Where(t => t.UserId == userId
                        && t.SpecialtyId != 55
                        && t.SpecialtyId != null)
            .Select(t => t.SpecialtyId!.Value)
            .ToListAsync();

        return specialtiesUser.ToHashSet();
    }

    public async Task<bool> WriteSpecialty(int specialtyId, string userId)
    {
        try
        {
            await context.SubscribedVacancies.AddAsync(new SubscribedVacancy
            {
                SpecialtyId = specialtyId,
                Id = Guid.NewGuid().ToString(),
                UserId = userId
            });

            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            throw;
        }

        return true;
    }

    public async Task<bool> RemoveSpecialty(int specialtyId, string userId)
    {
        try
        {
            var subscribe = await context
                .SubscribedVacancies
                .FirstOrDefaultAsync(t => t.SpecialtyId == specialtyId && t.UserId == userId);

            if (subscribe != null)
            {
                context.SubscribedVacancies.Remove(subscribe);
                await context.SaveChangesAsync();

                return true;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            throw;
        }

        return false;
    }

    public async Task<bool> WriteSubSpecialty(int subSpecialtyId, string userId)
    {
        try
        {
            await context.SubscribedVacancies.AddAsync(new SubscribedVacancy
            {
                SpecialtyId = 55,
                SubspecialtyId = subSpecialtyId,
                Id = Guid.NewGuid().ToString(),
                UserId = userId
            });

            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            throw;
        }

        return true;
    }

    public async Task<bool> RemoveSubSpecialty(int subSpecialtyId, string userId)
    {
        try
        {
            var subscribe = await context
                .SubscribedVacancies
                .FirstOrDefaultAsync(t => t.SubspecialtyId == subSpecialtyId && t.UserId == userId);

            if (subscribe != null)
            {
                context.SubscribedVacancies.Remove(subscribe);
                await context.SaveChangesAsync();

                return true;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            throw;
        }

        return false;
    }

    public async Task<HashSet<int>> GetUserSubSpecialty(string userId)
    {
        var specialtiesUser = await context
            .SubscribedVacancies
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.SubspecialtyId != null)
            .Select(t => t.SubspecialtyId!.Value)
            .ToListAsync();

        return specialtiesUser.ToHashSet();
    }

    public async Task<List<ResourceDto>> GetResourcesDodo()
    {
        var resources = await context
            .Resources
            .AsNoTracking()
            .ToListAsync();

        return resources.Select(t => new ResourceDto
        {
            Name = t.Name,
            Url = t.Url
        }).ToList();
    }

    public async Task<PeriodicitySettings?> ReadUserSubscribeOptions(string userId)
    {
        var candidate = await context
            .Candidates
            .AsNoTracking()
            .Include(t => t.Periodicity)
            .FirstOrDefaultAsync(t => t.Id == userId);

        return candidate?.Periodicity?.Settings;
    }

    public async Task<PeriodicitySettings?> WriteReadUserSubscribe(string userId, PeriodicitySettings setting)
    {
        var candidate = await context
            .Candidates
            .Include(t => t.Periodicity)
            .FirstOrDefaultAsync(t => t.Id == userId);

        if (candidate != null)
        {
            if (candidate.Periodicity == null)
            {
                candidate.Periodicity = new Periodicity
                {
                    Id = Guid.NewGuid().ToString(),
                    Settings = setting,
                    StartNotify = DateTime.UtcNow,
                    UserId = userId
                };
            }
            else
            {
                candidate.Periodicity.Settings = setting;
            }

            await context.SaveChangesAsync();

            return setting;
        }

        return null;
    }

    public async Task<bool> UpdateExistUser(string userId)
    {
        try
        {
            var candidate = await context
                .Candidates
                .Include(t => t.Periodicity)
                .FirstOrDefaultAsync(t => t.Id == userId);

            if (candidate != null)
            {
                candidate.Periodicity = new Periodicity
                {
                    Settings = PeriodicitySettings.Enable,
                    StartNotify = DateTime.UtcNow,
                    Id = Guid.NewGuid().ToString()
                };

                await context.SaveChangesAsync();
                return true;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            throw;
        }

        return false;
    }

    public async Task<CandidateSpecialty?> GetUserSubscription(string userId)
    {
        try
        {
            var userInfo = await context
                .SubscribedVacancies
                .AsNoTracking()
                .Where(t => t.UserId == userId)
                .ToListAsync();

            if (userInfo.Count > 0)
            {
                return new CandidateSpecialty
                {
                    Specialty = userInfo.Where(t => t.SpecialtyId.HasValue).Select(t => t.SpecialtyId!.Value)
                        .ToHashSet(),
                    SubSpecialty = userInfo.Where(t => t.SubspecialtyId.HasValue).Select(t => t.SubspecialtyId!.Value)
                        .ToHashSet()
                };
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return null;
    }

    public async Task<List<SubscribedVacancy>> GetAllEnabledUsers()
    {
        try
        {
            return await context
                .SubscribedVacancies
                .Include(t => t.Candidates)
                .ThenInclude(t => t.Periodicity)
                .AsNoTracking()
                .Where(t => t.Candidates.Periodicity.Settings == PeriodicitySettings.Enable)
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            throw;
        }
    }

    public async Task<List<Vacancy>> ReadExistsVacancy()
    {
        return await context
            .Vacancies
            .AsNoTracking()
            .ToListAsync();
    }


    public async Task WriteNewVacancy(List<Vacancy> postedVacancy)
    {
        await context.Vacancies.AddRangeAsync(postedVacancy);
        await context.SaveChangesAsync();
    }

    public async Task<long?> GetTelegramUserId(string userId)
    {
        var user = await context.Candidates.AsNoTracking().FirstOrDefaultAsync(t => t.Id == userId);

        if (user != null)
        {
            return user.TelegramId;
        }

        return null;
    }
}