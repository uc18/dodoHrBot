using Microsoft.EntityFrameworkCore;
using Repository.Entities;

namespace Repository;

public class SupabaseContext : DbContext
{
    public SupabaseContext(DbContextOptions<SupabaseContext> options)
        : base(options)
    {
    }

    public DbSet<Candidate> Candidates { get; set; }

    public DbSet<Periodicity> Periodicities { get; set; }
}