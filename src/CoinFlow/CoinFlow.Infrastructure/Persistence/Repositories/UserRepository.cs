using CoinFlow.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CoinFlow.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly CoinFlowDbContext _dbContext;

    public UserRepository(CoinFlowDbContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(User user, CancellationToken ct = default)
        => await _dbContext.Users.AddAsync(user, ct);

    public Task<bool> EmailExistsAsync(Email email, CancellationToken ct = default)
        => _dbContext.Users.AnyAsync(u => u.Email == email, ct);

    public Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default)
        => _dbContext.Users
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(x => x.Email == email, ct);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _dbContext.Users
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<User?> GetByRefreshTokenAsync(string token, CancellationToken ct = default)
    {
        var refreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token, ct);

        if (refreshToken is null)
            return null;

        return await _dbContext.Users
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.Id == refreshToken.Id, ct);
    }

    public void Update(User user) => _dbContext.Users.Update(user);
}
