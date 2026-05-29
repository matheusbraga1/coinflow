using MediatR;

namespace CoinFlow.Application.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery(Guid UserId) : IRequest<CurrentUserResponse>;
