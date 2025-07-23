using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository;

namespace DodoBot.Extensions;

public static class DbExtensions
{
    public static IServiceCollection AddSupabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddDbContext<SupabaseContext>(p =>
            p.UseNpgsql(configuration.GetConnectionString("SupabaseDbUrl")));
    }
}