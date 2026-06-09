namespace CoinFlow.Api.Models.Responses;

public sealed record CurrentUserResponse(
    Guid UserId,
    string Email,
    string Name,
    DateTime CreatedAt);
