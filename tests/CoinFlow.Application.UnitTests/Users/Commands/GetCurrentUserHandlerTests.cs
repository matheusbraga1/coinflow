using CoinFlow.Application.Users.Queries.GetCurrentUser;
using CoinFlow.Domain.Exceptions.Users;
using CoinFlow.Domain.Users;
using NSubstitute;
using Shouldly;

namespace CoinFlow.Application.UnitTests.Users.Commands;

public class GetCurrentUserHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    public GetCurrentUserHandler CreateHandler() =>
        new(_userRepository);

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnCurrentUser()
    {
        var email = Email.Create("user@coinflow.test");
        var user = User.Create(email, "correct-hash", "Matheus");
        var query = new GetCurrentUserQuery(user.Id);

        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        var response = await CreateHandler().Handle(query, CancellationToken.None);

        response.ShouldNotBeNull();
        response.UserId.ShouldBe(user.Id);
        response.Email.ShouldBe(user.Email.Value);

        await _userRepository.Received(1).GetByIdAsync(user.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithUserNotRegistered_ShouldThrowUserNotFoundException()
    {
        var userId = Guid.NewGuid();
        var query = new GetCurrentUserQuery(userId);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);
        
        await Should.ThrowAsync<UserNotFoundException>(async () =>
            await CreateHandler().Handle(query, CancellationToken.None));
        
        await _userRepository.Received(1).GetByIdAsync(userId, Arg.Any<CancellationToken>());
    }
}
