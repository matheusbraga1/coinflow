using CoinFlow.Domain.Common;

namespace CoinFlow.Domain.Users;

public sealed class RefreshToken : Entity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private RefreshToken() { }

    internal RefreshToken(Guid userId, string token, DateTime expiresAt)
        : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("O Token não pode estar vazio.", nameof(token));

        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("A data de expiração deve ser uma data futura", nameof(expiresAt));

        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsExpired && !IsRevoked;

    internal void Revoke()
    {
        if (IsRevoked) return;
        RevokedAt = DateTime.UtcNow;
    }
}
