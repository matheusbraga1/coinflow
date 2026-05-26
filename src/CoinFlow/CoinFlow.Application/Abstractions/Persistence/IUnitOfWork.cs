namespace CoinFlow.Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct =  default);
}
