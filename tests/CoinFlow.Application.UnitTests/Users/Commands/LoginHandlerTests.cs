using CoinFlow.Application.Abstractions.Authentication;
using CoinFlow.Application.Abstractions.Persistence;
using CoinFlow.Application.Users.Commands.Login;
using CoinFlow.Domain.Exceptions.Users;
using CoinFlow.Domain.Users;
using NSubstitute;
using Shouldly;

namespace CoinFlow.Application.UnitTests.Users.Commands;

public class LoginHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenGenerator _tokenGenerator = Substitute.For<ITokenGenerator>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    public LoginHandler CreateHandler()
        => new(_userRepository, _passwordHasher, _tokenGenerator, _unitOfWork);

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnTokens()
    {
        var email = Email.Create("user@coinflow.test");
        var user = User.Create(email, "valid-hash", "Matheus");
        var command = new LoginCommand(email.Value, "correct-password");
        var tokenPair = new TokenPair(
            "access-token", "refresh-token",
            DateTime.UtcNow.AddMinutes(15), DateTime.UtcNow.AddDays(7));

        _userRepository.GetByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify(command.Password, user.PasswordHash).Returns(true);
        _tokenGenerator.Generate(user).Returns(tokenPair);

        var response = await CreateHandler().Handle(command, CancellationToken.None);

        response.AccessToken.ShouldBe("access-token");
        response.RefreshToken.ShouldBe("refresh-token");
        user.RefreshTokens.Count.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_WithNonExistentUser_ShouldThrowInvalidCredentialsException()
    {
        var command = new LoginCommand("user@coinflow.test", "any-password");
        _userRepository.GetByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        await Should.ThrowAsync<InvalidCredentialsException>(() =>
            CreateHandler().Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ShouldThrowInvalidCredentialsException()
    {
        var email = Email.Create("user@coinflow.test");
        var user = User.Create(email, "valid-hash", "Matheus");
        var command = new LoginCommand(email.Value, "wrong-password");

        _userRepository.GetByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify(command.Password, user.PasswordHash).Returns(false);

        await Should.ThrowAsync<InvalidCredentialsException>(() =>
            CreateHandler().Handle(command, CancellationToken.None));
    }
}
