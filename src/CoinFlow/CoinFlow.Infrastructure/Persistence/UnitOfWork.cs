using CoinFlow.Application.Abstractions.Persistence;

namespace CoinFlow.Infrastructure.Persistence;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly CoinFlowDbContext _dbContext;

    public UnitOfWork(CoinFlowDbContext dbContext) => _dbContext = dbContext;

    public Task<int> CommitAsync(CancellationToken ct = default) => _dbContext.SaveChangesAsync(ct);
}
