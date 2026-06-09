using CoinFlow.Application.Abstractions.Authentication;
using CoinFlow.Application.Abstractions.Persistence;
using CoinFlow.Application.Users.Commands.RefreshToken;
using CoinFlow.Domain.Exceptions.Users;
using CoinFlow.Domain.Users;
using NSubstitute;
using Shouldly;

namespace CoinFlow.Application.UnitTests.Users.Commands;

public class RefreshTokenHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ITokenGenerator _tokenGenerator = Substitute.For<ITokenGenerator>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private RefreshTokenHandler CreateHandler() => new(_userRepository, _tokenGenerator, _unitOfWork);

    [Fact]
    public async Task Handle_WithValidRefreshToken_ShouldRevokeAndReturnNewTokens()
    {
        var oldToken = "valid-old-token";
        var command = new RefreshTokenCommand(oldToken);

        var email = Email.Create("user@coinflow.test");
        var user = User.Create(email, "correct-hash", "Matheus");

        user.IssueRefreshToken(oldToken, DateTime.UtcNow.AddDays(7));

        var newTokens = new TokenPair(
            "new-access-token", "new-refresh-token",
            DateTime.UtcNow.AddMinutes(15), DateTime.UtcNow.AddDays(7));

        _userRepository.GetByRefreshTokenAsync(oldToken, Arg.Any<CancellationToken>()).Returns(user);
        _tokenGenerator.Generate(user).Returns(newTokens);

        var response = await CreateHandler().Handle(command, CancellationToken.None);

        response.AccessToken.ShouldBe("new-access-token");
        response.RefreshToken.ShouldBe("new-refresh-token");

        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());

        var oldTokenState = user.RefreshTokens.FirstOrDefault(rt => rt.Token == oldToken);
        oldTokenState?.IsActive.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldThrowInvalidRefreshTokenException()
    {
        var command = new RefreshTokenCommand("unknown-token");

        _userRepository.GetByRefreshTokenAsync("unknown-token", Arg.Any<CancellationToken>()).Returns((User?)null);
        
        await Should.ThrowAsync<InvalidRefreshTokenException>(async () =>
            await CreateHandler().Handle(command, CancellationToken.None));

        _tokenGenerator.DidNotReceiveWithAnyArgs().Generate(Arg.Any<User>());
        await _unitOfWork.DidNotReceiveWithAnyArgs().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenTokenIsExpiredOrInactive_ShouldThrowInvalidRefreshTokenException()
    {
        var expiredToken = "expired-token";
        var command = new RefreshTokenCommand(expiredToken);

        var email = Email.Create("user@coinflow.test");
        var user = User.Create(email, "correct-hash", "Matheus");

        user.IssueRefreshToken(expiredToken, DateTime.UtcNow.AddDays(7));

        user.RevokeRefreshToken(expiredToken);

        _userRepository.GetByRefreshTokenAsync(expiredToken, Arg.Any<CancellationToken>()).Returns(user);

        await Should.ThrowAsync<InvalidRefreshTokenException>(async () =>
            await CreateHandler().Handle(command, CancellationToken.None));

        await _unitOfWork.DidNotReceiveWithAnyArgs().CommitAsync(Arg.Any<CancellationToken>());
    }
}
