using CoinFlow.Domain.Exceptions.Users;
using CoinFlow.Domain.Users;
using Shouldly;

namespace CoinFlow.Domain.UnitTests.Users;

public class UserTests
{
    private static readonly Email ValidEmail = Email.Create("test@coinflow.test");
    private const string ValidPassword = "$2a$12$validhashexample";
    private const string ValidName = "Tester";

    [Fact]
    public void Create_WithValidData_ShouldCreateUser()
    {
        var user = User.Create(ValidEmail, ValidPassword, ValidName);

        user.ShouldNotBeNull();
        user.Id.ShouldNotBe(Guid.Empty);
        user.Email.ShouldBe(ValidEmail);
        user.PasswordHash.ShouldBe(ValidPassword);
        user.Name.ShouldBe(ValidName);
        user.CreatedAt.ShouldBeInRange(
            DateTime.UtcNow.AddSeconds(-5),
            DateTime.UtcNow.AddSeconds(1));
        user.RefreshTokens.ShouldBeEmpty();
    }

    [Fact]
    public void Create_ShouldTrimName()
    {
        var user = User.Create(ValidEmail, ValidPassword, "  Matheus  ");

        user.Name.ShouldBe("Matheus");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_WithEmptyName_ShouldThrowInvalidUserNameException(string invalidName)
    {
        Should.Throw<InvalidUsernameException>(() => User.Create(ValidEmail, ValidPassword, invalidName));
    }

    [Theory]
    [InlineData("A")]
    public void Create_WithTooShortName_ShouldThrowInvalidUserNameException(string invalidName)
    {
        Should.Throw<InvalidUsernameException>(() => User.Create(ValidEmail, ValidPassword, invalidName));
    }

    [Fact]
    public void Create_WithTooLongName_ShouldThrowInvalidUserNameException()
    {
        var invalidName = new string('A', 101);
        Should.Throw<InvalidUsernameException>(() => User.Create(ValidEmail, ValidPassword, invalidName));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_WithEmptyPassword_ShouldThrowInvalidPasswordException(string invalidPassword)
    {
        Should.Throw<InvalidPasswordHashException>(() => User.Create(ValidEmail, invalidPassword, ValidName));
    }

    [Fact]
    public void IssueRefreshToken_ShouldAddTokenToCollection()
    {
        var user = User.Create(ValidEmail, ValidPassword, ValidName);
        var token = "some-refresh-token";
        var expiresAt = DateTime.UtcNow.AddDays(7);

        var refreshToken = user.IssueRefreshToken(token, expiresAt);

        user.RefreshTokens.ShouldContain(refreshToken);
        refreshToken.Token.ShouldBe(token);
        refreshToken.ExpiresAt.ShouldBe(expiresAt);
        refreshToken.UserId.ShouldBe(user.Id);
    }

    [Fact]
    public void RevokeRefreshToken_ShouldMarkAsRevoked()
    {
        var user = User.Create(ValidEmail, ValidPassword, ValidName);
        var token = "some-refresh-token";
        var expiresAt = DateTime.UtcNow.AddDays(7);

        var refreshToken = user.IssueRefreshToken(token, expiresAt);

        user.RevokeRefreshToken(token);

        var revokedToken = user.RefreshTokens.First();
        revokedToken.IsRevoked.ShouldBeTrue();
        revokedToken.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void RevokeRefreshToken_WithUnknownToken_ShouldThrowException()
    {
        var user = User.Create(ValidEmail, ValidPassword, ValidName);

        Should.Throw<RefreshTokenNotFoundException>(() => user.RevokeRefreshToken("unknown-token"));
    }
}
