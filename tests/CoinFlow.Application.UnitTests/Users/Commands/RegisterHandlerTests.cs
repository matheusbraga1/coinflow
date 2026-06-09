using CoinFlow.Application.Abstractions.Authentication;
using CoinFlow.Application.Abstractions.Persistence;
using CoinFlow.Application.Users.Commands.Register;
using CoinFlow.Application.Users.Exceptions;
using CoinFlow.Domain.Exceptions.Users;
using CoinFlow.Domain.Users;
using NSubstitute;
using Shouldly;

namespace CoinFlow.Application.UnitTests.Users.Commands;

public class RegisterHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private RegisterHandler CreateHandler() => new(_userRepository, _passwordHasher, _unitOfWork);

    [Fact]
    public async Task Handle_WithValidData_ShouldRegisterUser()
    {
        var command = new RegisterCommand("user@coinflow.test", "Senha@123", "Matheus");
        _userRepository.EmailExistsAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>()).Returns(false);
        _passwordHasher.Hash(command.Password).Returns("hashed-password");

        var response = await CreateHandler().Handle(command, CancellationToken.None);

        response.ShouldNotBeNull();
        response.Email.ShouldBe(command.Email);
        response.Name.ShouldBe(command.Name);

        await _userRepository.Received(1).AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldThrowEmailAlreadyExistsException()
    {
        var command = new RegisterCommand("user@coinflow.test", "Senha@123", "Matheus");
        _userRepository.EmailExistsAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>()).Returns(true);

        await Should.ThrowAsync<EmailAlreadyExistsException>(() =>
            CreateHandler().Handle(command, CancellationToken.None));

        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ShouldThrowInvalidEmailException()
    {
        var command = new RegisterCommand("not-an-email", "Senha@123", "Matheus");

        await Should.ThrowAsync<InvalidEmailException>(() =>
            CreateHandler().Handle(command, CancellationToken.None));
    }
}
