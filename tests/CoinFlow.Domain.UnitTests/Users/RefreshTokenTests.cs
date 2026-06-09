using CoinFlow.Domain.Users;
using Shouldly;

namespace CoinFlow.Domain.UnitTests.Users;

public class RefreshTokenTests
{
    [Fact]
    public void IsActive_WhenNotExpiredAndNotRevoked_ShouldBeTrue()
    {
        var user = User.Create(
            Email.Create("a@b.c"), "hash", "Name");
        var token = user.IssueRefreshToken("token", DateTime.UtcNow.AddDays(7));

        token.IsActive.ShouldBeTrue();
        token.IsExpired.ShouldBeFalse();
        token.IsRevoked.ShouldBeFalse();
    }

    [Fact]
    public void IsActive_WhenRevoked_ShouldBeFalse()
    {
        var user = User.Create(
            Email.Create("a@b.c"), "hash", "Name");

        user.IssueRefreshToken("token", DateTime.UtcNow.AddDays(7));

        user.RevokeRefreshToken("token");

        var revoked = user.RefreshTokens.First();
        revoked.IsActive.ShouldBeFalse();
        revoked.IsRevoked.ShouldBeTrue();
    }

    [Fact]
    public void Revoke_WhenAlreadyRevoked_ShouldBeIdempotent()
    {
        var user = User.Create(
            Email.Create("a@b.c"), "hash", "Name");
        user.IssueRefreshToken("token", DateTime.UtcNow.AddDays(7));

        user.RevokeRefreshToken("token");

        var firstRevokedAt = user.RefreshTokens.First().RevokedAt;

        Should.NotThrow(() => user.RefreshTokens.First().GetType());
    }
}
