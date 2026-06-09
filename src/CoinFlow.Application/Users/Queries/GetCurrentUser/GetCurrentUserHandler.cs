using CoinFlow.Domain.Exceptions.Users;
using CoinFlow.Domain.Users;
using MediatR;

namespace CoinFlow.Application.Users.Queries.GetCurrentUser;

public sealed class GetCurrentUserHandler 
    : IRequestHandler<GetCurrentUserQuery, CurrentUserResponse>
{
    private readonly IUserRepository _userRepository;

    public GetCurrentUserHandler(IUserRepository userRepository) => _userRepository = userRepository;

    public async Task<CurrentUserResponse> Handle(GetCurrentUserQuery query, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(query.UserId, ct);

        if (user is null)
            throw new UserNotFoundException(query.UserId);

        return new CurrentUserResponse(
            user.Id, 
            user.Email.Value, 
            user.Name, 
            user.CreatedAt);
    }
}
