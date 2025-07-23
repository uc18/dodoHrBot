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

    public DbSet<Resource> Resources { get; set; }

    public DbSet<SubscribedVacancy> SubscribedVacancies { get; set; }

    public DbSet<Vacancy> Vacancies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Candidate>()
            .HasOne(e => e.Periodicity)
            .WithOne(e => e.Candidate)
            .HasForeignKey<Periodicity>(e => e.UserId)
            .IsRequired();

        modelBuilder.Entity<Candidate>()
            .HasMany(e => e.SubscribedVacancies)
            .WithOne(e => e.Candidates)
            .HasForeignKey(e => e.UserId)
            .IsRequired();
    }
}