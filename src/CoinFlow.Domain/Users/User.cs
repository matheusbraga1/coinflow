using CoinFlow.Domain.Common;
using CoinFlow.Domain.Exceptions.Users;

namespace CoinFlow.Domain.Users;

public sealed class User : AggregateRoot
{
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private User() { }

    private User(Guid id, Email email, string passwordHash, string name) 
        : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }

    public static User Create(Email email, string passwordHash, string name)
    {
        ArgumentNullException.ThrowIfNull(email);

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new InvalidPasswordHashException();

        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidUsernameException("Nome não pode ser vazio");

        name = name.Trim();

        if (name.Length < 2 || name.Length > 100)
            throw new InvalidUsernameException("O nome deve estar entre 2 e 100 caracteres");

        return new User(Guid.NewGuid(), email, passwordHash, name);
    }

    public RefreshToken IssueRefreshToken(string token, DateTime expiresAt)
    {
        var refreshToken = new RefreshToken(Id, token, expiresAt);
        _refreshTokens.Add(refreshToken);
        return refreshToken;
    }

    public void RevokeRefreshToken(string token)
    {
        var refreshToken = _refreshTokens.FirstOrDefault(rt => rt.Token == token);
        
        if (refreshToken is null)
            throw new RefreshTokenNotFoundException();

        refreshToken.Revoke();
    }

    public void RevokeAllRefreshTokens()
    {
        foreach (var refreshToken in _refreshTokens.Where(x => x.IsActive))
            refreshToken.Revoke();
    }
}
