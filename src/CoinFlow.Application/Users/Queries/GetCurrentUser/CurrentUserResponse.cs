namespace CoinFlow.Application.Users.Queries.GetCurrentUser;

public sealed record CurrentUserResponse(
    Guid UserId,
    string Email,
    string Name,
    DateTime CreatedAt);
