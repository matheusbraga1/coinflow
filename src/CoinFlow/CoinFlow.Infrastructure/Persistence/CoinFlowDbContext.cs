using CoinFlow.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CoinFlow.Infrastructure.Persistence;

public sealed class CoinFlowDbContext : DbContext
{
    public CoinFlowDbContext(DbContextOptions<CoinFlowDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoinFlowDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
